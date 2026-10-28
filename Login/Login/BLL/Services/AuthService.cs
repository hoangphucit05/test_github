using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Login.BLL.DTOs;
using Login.DAL;
using Login.DAL.Entities;

namespace Login.BLL.Services
{
    public class AuthService
    {
        private const int MaxFailedLoginAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
        private readonly BakeryDbContext _context;

        public AuthService(BakeryDbContext context)
        {
            _context = context;
        }

        public async Task<AuthResult> LoginAsync(string username, string password, string? expectedRoleVi = null)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return AuthResult.Fail("Tên đăng nhập không được để trống!");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return AuthResult.Fail("Mật khẩu không được để trống!");
            }

            // Truy vấn người dùng kèm thông tin Role, Permissions, Employee, Branch
            var user = await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.Branch)
                .FirstOrDefaultAsync(u => u.Username == username.Trim());

            if (user == null)
            {
                return AuthResult.Fail("Tài khoản hoặc mật khẩu không chính xác!");
            }

            // Kiểm tra trạng thái tài khoản
            DateTime now = DateTime.Now;
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > now)
            {
                return AuthResult.Fail("Tài khoản tạm khóa do nhập sai mật khẩu nhiều lần. Vui lòng thử lại sau 15 phút.");
            }

            // Hết thời gian khóa thì cho phép đếm lại từ đầu.
            if (user.LockoutEnd.HasValue)
            {
                user.LockoutEnd = null;
                user.FailedLoginCount = 0;
            }

            if (!user.IsActive)
            {
                return AuthResult.Fail("Tài khoản này đã bị khóa. Vui lòng liên hệ Quản trị viên!");
            }

            // Kiểm tra trạng thái nhân viên
            if (user.Employee != null && user.Employee.Status != "ACTIVE")
            {
                return AuthResult.Fail("Hồ sơ nhân viên đang không ở trạng thái hoạt động!");
            }

            // Xác thực mật khẩu băm (hỗ trợ BCrypt và legacy SHA-256)
            bool isPasswordValid = VerifyPassword(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                user.FailedLoginCount++;
                if (user.FailedLoginCount >= MaxFailedLoginAttempts)
                {
                    user.LockoutEnd = now.Add(LockoutDuration);
                }
                await _context.SaveChangesAsync();
                return AuthResult.Fail("Tài khoản hoặc mật khẩu không chính xác!");
            }

            // Nếu mật khẩu cũ chưa nâng cấp lên BCrypt, tự động nâng cấp hash
            if (user.PasswordHash.StartsWith("0x") || user.PasswordHash.Length < 60)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }

            // Kiểm tra vai trò phù hợp với nút chọn trên giao diện (nếu có)
            if (!string.IsNullOrEmpty(expectedRoleVi))
            {
                string expectedCode = MapVietnameseRoleToCode(expectedRoleVi);
                if (!string.IsNullOrEmpty(expectedCode) && !user.Role.RoleName.Equals(expectedCode, StringComparison.OrdinalIgnoreCase))
                {
                    return AuthResult.Fail($"Tài khoản này thuộc vai trò '{user.Role.RoleName}', không phải '{expectedRoleVi}'. Vui lòng chọn đúng vai trò!");
                }
            }

            // Cập nhật thời gian đăng nhập cuối
            user.LastLogin = DateTime.Now;
            user.FailedLoginCount = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            // Khởi tạo phiên làm việc
            var permissions = user.Role.RolePermissions
                .Select(rp => rp.Permission.PermissionCode)
                .ToList();

            var session = new UserSession
            {
                UserID = user.UserID,
                Username = user.Username,
                FullName = user.Employee?.FullName ?? user.Username,
                RoleName = user.Role.RoleName,
                Position = user.Employee?.Position,
                BranchID = user.Employee?.BranchID,
                BranchName = user.Employee?.Branch?.BranchName,
                LastLogin = user.LastLogin,
                Permissions = permissions
            };

            return AuthResult.Success(session, $"Đăng nhập thành công! Xin chào {session.FullName} ({session.RoleName}).");
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (!VerifyPassword(oldPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        private static bool VerifyPassword(string inputPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash)) return false;

            // Kiểm tra nếu là BCrypt hash
            if (storedHash.StartsWith("$2a$") || storedHash.StartsWith("$2b$") || storedHash.StartsWith("$2y$"))
            {
                try
                {
                    return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
                }
                catch
                {
                    return false;
                }
            }

            // Fallback: SHA-256 legacy check
            using var sha = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputPassword));
            string hex = "0x" + BitConverter.ToString(bytes).Replace("-", "");

            return string.Equals(storedHash, hex, StringComparison.OrdinalIgnoreCase);
        }

        private static string MapVietnameseRoleToCode(string vietnameseRole)
        {
            return vietnameseRole.Trim().ToLower() switch
            {
                "quản trị viên" or "admin" => "ADMIN",
                "quản lý" or "manager" => "MANAGER",
                "nhân viên" or "staff" => "STAFF",
                _ => string.Empty
            };
        }
    }
}
