using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Login.DAL;
using Login.DAL.Entities;

namespace Login.BLL.Services
{
    public class UserService
    {
        private readonly BakeryDbContext _context;

        public UserService(BakeryDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync(string? keyword = null, int? roleId = null, bool? isActive = null)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)
                    .ThenInclude(e => e.Branch)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim().ToLower();
                query = query.Where(u => u.Username.ToLower().Contains(kw) ||
                                         u.Employee.FullName.ToLower().Contains(kw) ||
                                         u.Employee.EmployeeCode.ToLower().Contains(kw));
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.RoleID == roleId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            return await query.OrderBy(u => u.Username).ToListAsync();
        }

        public async Task<List<Employee>> GetEmployeesWithoutAccountAsync()
        {
            // Lấy danh sách nhân viên đang làm việc mà chưa có tài khoản đăng nhập
            return await _context.Employees
                .Include(e => e.Branch)
                .Where(e => e.Status == "ACTIVE" && e.User == null)
                .OrderBy(e => e.FullName)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> CreateUserForEmployeeAsync(
            int currentAdminUserId,
            int employeeId,
            int roleId,
            string username,
            string rawPassword,
            int? branchId = null,
            bool isActive = true)
        {
            // 1. Kiểm tra quyền Admin
            var currentAdmin = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserID == currentAdminUserId);

            if (currentAdmin == null || currentAdmin.Role.RoleName != "ADMIN")
            {
                return (false, "Chỉ Quản trị viên (Admin) mới có quyền cấp tài khoản!");
            }

            // 2. Không cho phép tự tạo tài khoản Admin thứ hai một cách bất thường nếu role được chọn là ADMIN mà không phải do super-admin
            var targetRole = await _context.Roles.FindAsync(roleId);
            if (targetRole == null)
            {
                return (false, "Vai trò được chọn không hợp lệ!");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                return (false, "Tên đăng nhập không được để trống!");
            }

            if (!PasswordPolicy.IsValid(rawPassword))
            {
                return (false, "Mật khẩu phải có ít nhất 6 ký tự!");
            }

            username = username.Trim().ToLower();

            // 3. Kiểm tra tính duy nhất của Username
            bool userExists = await _context.Users.AnyAsync(u => u.Username == username);
            if (userExists)
            {
                return (false, $"Tên đăng nhập '{username}' đã được sử dụng!");
            }

            // 4. Kiểm tra nhân viên
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            if (employee == null || employee.Status != "ACTIVE")
            {
                return (false, "Nhân viên không tồn tại hoặc đã nghỉ việc!");
            }

            if (employee.User != null)
            {
                return (false, $"Nhân viên '{employee.FullName}' đã có tài khoản ('{employee.User.Username}'). Mỗi nhân viên chỉ được cấp tối đa 1 tài khoản!");
            }

            if (branchId.HasValue)
            {
                bool validBranch = await _context.Branches
                    .AnyAsync(b => b.BranchID == branchId.Value && b.IsActive);
                if (!validBranch)
                {
                    return (false, "Chi nhánh được chọn không tồn tại hoặc đã ngừng hoạt động!");
                }

                employee.BranchID = branchId.Value;
            }

            // 5. Băm mật khẩu bằng BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(rawPassword);

            var newUser = new User
            {
                EmployeeID = employeeId,
                RoleID = roleId,
                Username = username,
                PasswordHash = passwordHash,
                IsActive = isActive,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return (true, $"Cấp tài khoản '{username}' cho nhân viên '{employee.FullName}' thành công!");
        }

        public async Task<(bool Success, string Message)> ToggleUserStatusAsync(int currentAdminUserId, int targetUserId)
        {
            var currentAdmin = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserID == currentAdminUserId);

            if (currentAdmin == null || currentAdmin.Role.RoleName != "ADMIN")
            {
                return (false, "Chỉ Quản trị viên mới có quyền khóa/mở khóa tài khoản!");
            }

            if (currentAdminUserId == targetUserId)
            {
                return (false, "Bạn không thể tự khóa tài khoản của chính mình!");
            }

            var targetUser = await _context.Users.FindAsync(targetUserId);
            if (targetUser == null) return (false, "Không tìm thấy người dùng!");

            targetUser.IsActive = !targetUser.IsActive;
            await _context.SaveChangesAsync();

            string status = targetUser.IsActive ? "kích hoạt" : "khóa";
            return (true, $"Đã {status} tài khoản '{targetUser.Username}' thành công!");
        }

        public async Task<(bool Success, string Message)> ResetPasswordByAdminAsync(int currentAdminUserId, int targetUserId, string newPassword)
        {
            var currentAdmin = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserID == currentAdminUserId);

            if (currentAdmin == null || currentAdmin.Role.RoleName != "ADMIN")
            {
                return (false, "Chỉ Quản trị viên mới có quyền đặt lại mật khẩu người dùng!");
            }

            if (!PasswordPolicy.IsValid(newPassword))
            {
                return (false, "Mật khẩu mới phải có ít nhất 6 ký tự!");
            }

            var targetUser = await _context.Users.FindAsync(targetUserId);
            if (targetUser == null) return (false, "Không tìm thấy tài khoản!");

            targetUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            targetUser.FailedLoginCount = 0;
            targetUser.LockoutEnd = null;
            await _context.SaveChangesAsync();

            return (true, $"Đặt lại mật khẩu cho tài khoản '{targetUser.Username}' thành công!");
        }
    }
}
