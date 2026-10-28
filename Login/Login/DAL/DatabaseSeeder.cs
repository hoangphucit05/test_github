using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Login.DAL.Entities;

namespace Login.DAL
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(BakeryDbContext context)
        {
            // Toàn bộ dữ liệu mẫu phải được tạo đồng thời; nếu có lỗi giữa chừng,
            // lần khởi động kế tiếp không bị kẹt trong trạng thái dữ liệu dở dang.
            await using var transaction = await context.Database.BeginTransactionAsync();

            // 1. ĐẢM BẢO DANH MỤC VAI TRÒ (Roles: ADMIN, MANAGER, STAFF)
            if (!await context.Roles.AnyAsync(r => r.RoleName == "ADMIN"))
            {
                context.Roles.Add(new Role { RoleName = "ADMIN", Description = "Quản trị viên toàn quyền hệ thống" });
            }
            if (!await context.Roles.AnyAsync(r => r.RoleName == "MANAGER"))
            {
                context.Roles.Add(new Role { RoleName = "MANAGER", Description = "Quản lý chi nhánh và vận hành ca làm" });
            }
            if (!await context.Roles.AnyAsync(r => r.RoleName == "STAFF"))
            {
                context.Roles.Add(new Role { RoleName = "STAFF", Description = "Nhân viên bán hàng và phục vụ" });
            }
            await context.SaveChangesAsync();

            var adminRole = await context.Roles.FirstAsync(r => r.RoleName == "ADMIN");
            var managerRole = await context.Roles.FirstAsync(r => r.RoleName == "MANAGER");
            var staffRole = await context.Roles.FirstAsync(r => r.RoleName == "STAFF");

            // 2. DANH MỤC QUYỀN HỆ THỐNG (Permissions)
            var defaultPermissions = new[]
            {
                new { Code = "AUTH_LOGIN", Name = "Đăng nhập hệ thống", Group = "Hệ thống", Desc = "Quyền truy cập vào phần mềm" },
                new { Code = "USER_VIEW", Name = "Xem danh sách người dùng", Group = "Người dùng", Desc = "Xem danh sách tài khoản và nhân viên" },
                new { Code = "USER_CREATE", Name = "Cấp tài khoản nhân viên", Group = "Người dùng", Desc = "Tạo mới tài khoản nhân viên" },
                new { Code = "USER_EDIT", Name = "Chỉnh sửa/Khóa tài khoản", Group = "Người dùng", Desc = "Cập nhật thông tin hoặc khóa tài khoản" },
                new { Code = "USER_RESET_PWD", Name = "Đặt lại mật khẩu", Group = "Người dùng", Desc = "Cấp lại mật khẩu mới cho người dùng" },
                new { Code = "ROLE_VIEW", Name = "Xem phân quyền vai trò", Group = "Phân quyền", Desc = "Xem ma trận quyền theo vai trò" },
                new { Code = "ROLE_ASSIGN", Name = "Cấu hình quyền cho vai trò", Group = "Phân quyền", Desc = "Chỉnh sửa danh mục quyền của vai trò" },
                new { Code = "BRANCH_VIEW", Name = "Xem danh sách chi nhánh", Group = "Chi nhánh", Desc = "Xem thông tin các cơ sở cửa hàng" },
                new { Code = "BRANCH_MANAGE", Name = "Quản lý chi nhánh", Group = "Chi nhánh", Desc = "Thêm, sửa, đóng mở chi nhánh" },
                new { Code = "SHIFT_VIEW", Name = "Xem danh mục ca làm", Group = "Ca làm", Desc = "Xem thời gian các ca làm việc" },
                new { Code = "SHIFT_MANAGE", Name = "Quản lý ca làm", Group = "Ca làm", Desc = "Thêm, sửa khung giờ ca làm" },
                new { Code = "SHIFT_ASSIGN", Name = "Phân công & Điểm danh ca", Group = "Ca làm", Desc = "Xếp lịch làm việc và ghi nhận điểm danh" }
            };

            foreach (var p in defaultPermissions)
            {
                if (!await context.Permissions.AnyAsync(x => x.PermissionCode == p.Code))
                {
                    context.Permissions.Add(new Permission
                    {
                        PermissionCode = p.Code,
                        PermissionName = p.Name,
                        GroupName = p.Group,
                        Description = p.Desc
                    });
                }
            }
            await context.SaveChangesAsync();

            // 3. GÁN QUYỀN CHO CÁC VAI TRÒ (RolePermissions)
            var allPermissions = await context.Permissions.ToListAsync();

            // Admin: toàn quyền
            foreach (var perm in allPermissions)
            {
                if (!await context.RolePermissions.AnyAsync(rp => rp.RoleID == adminRole.RoleID && rp.PermissionID == perm.PermissionID))
                {
                    context.RolePermissions.Add(new RolePermission { RoleID = adminRole.RoleID, PermissionID = perm.PermissionID });
                }
            }

            // Manager: xem và quản lý nhân viên, ca làm, chi nhánh
            var managerPermCodes = new[] { "AUTH_LOGIN", "USER_VIEW", "USER_CREATE", "BRANCH_VIEW", "SHIFT_VIEW", "SHIFT_MANAGE", "SHIFT_ASSIGN" };
            foreach (var perm in allPermissions.Where(p => managerPermCodes.Contains(p.PermissionCode)))
            {
                if (!await context.RolePermissions.AnyAsync(rp => rp.RoleID == managerRole.RoleID && rp.PermissionID == perm.PermissionID))
                {
                    context.RolePermissions.Add(new RolePermission { RoleID = managerRole.RoleID, PermissionID = perm.PermissionID });
                }
            }

            // Staff: xem ca làm của mình
            var staffPermCodes = new[] { "AUTH_LOGIN", "SHIFT_VIEW" };
            foreach (var perm in allPermissions.Where(p => staffPermCodes.Contains(p.PermissionCode)))
            {
                if (!await context.RolePermissions.AnyAsync(rp => rp.RoleID == staffRole.RoleID && rp.PermissionID == perm.PermissionID))
                {
                    context.RolePermissions.Add(new RolePermission { RoleID = staffRole.RoleID, PermissionID = perm.PermissionID });
                }
            }
            await context.SaveChangesAsync();

            // 4. KHỞI TẠO 2 CHI NHÁNH MẪU (Branches)
            if (!await context.Branches.AnyAsync(b => b.BranchCode == "CN001"))
            {
                context.Branches.Add(new Branch
                {
                    BranchCode = "CN001",
                    BranchName = "Chi nhánh Bánh Mì Bến Nghé",
                    Address = "123 Nguyễn Huệ, Phường Bến Nghé, Quận 1, TP.HCM",
                    Phone = "02838123456",
                    IsActive = true
                });
            }
            if (!await context.Branches.AnyAsync(b => b.BranchCode == "CN002"))
            {
                context.Branches.Add(new Branch
                {
                    BranchCode = "CN002",
                    BranchName = "Chi nhánh Bánh Mì Đa Kao",
                    Address = "45 Đinh Tiên Hoàng, Phường Đa Kao, Quận 1, TP.HCM",
                    Phone = "02838654321",
                    IsActive = true
                });
            }
            await context.SaveChangesAsync();

            var branch1 = await context.Branches.FirstAsync(b => b.BranchCode == "CN001");
            var branch2 = await context.Branches.FirstAsync(b => b.BranchCode == "CN002");

            // Cập nhật gán chi nhánh cho các nhân viên cũ chưa có chi nhánh
            var employeesWithoutBranch = await context.Employees.Where(e => e.BranchID == null).ToListAsync();
            foreach (var emp in employeesWithoutBranch)
            {
                emp.BranchID = branch1.BranchID;
            }
            await context.SaveChangesAsync();

            // Database mới không có dữ liệu từ BanhMiShopDB.sql vẫn phải có đủ nhân
            // viên để các tài khoản mẫu và lưới quản trị hoạt động ngay sau khi seed.
            var defaultEmployees = new[]
            {
                new { Code = "NV001", Name = "Nguyễn Văn An", Position = "Quản lý", Phone = "0901000001", Email = "an@banhmi.vn", BranchID = branch1.BranchID },
                new { Code = "NV002", Name = "Trần Thị Bình", Position = "Nhân viên", Phone = "0901000002", Email = "binh@banhmi.vn", BranchID = branch1.BranchID },
                new { Code = "NV003", Name = "Lê Văn Cường", Position = "Nhân viên", Phone = "0901000003", Email = "cuong@banhmi.vn", BranchID = branch1.BranchID },
                new { Code = "NV005", Name = "Hoàng Minh Trí", Position = "Quản lý cửa hàng", Phone = "0901000005", Email = "manager@banhmi.vn", BranchID = branch2.BranchID }
            };

            foreach (var employee in defaultEmployees)
            {
                if (!await context.Employees.AnyAsync(e => e.EmployeeCode == employee.Code))
                {
                    context.Employees.Add(new Employee
                    {
                        EmployeeCode = employee.Code,
                        FullName = employee.Name,
                        Position = employee.Position,
                        Phone = employee.Phone,
                        Email = employee.Email,
                        Address = "TP. Hồ Chí Minh",
                        HireDate = DateOnly.FromDateTime(DateTime.Today),
                        Status = "ACTIVE",
                        BranchID = employee.BranchID
                    });
                }
            }
            await context.SaveChangesAsync();

            // 5. KHỞI TẠO ĐỦ 3 CA LÀM (Sáng, Chiều, Tối)
            if (!await context.Shifts.AnyAsync(s => s.ShiftCode == "CA01"))
            {
                context.Shifts.Add(new Shift
                {
                    ShiftCode = "CA01",
                    ShiftName = "Ca Sáng",
                    StartTime = new TimeSpan(6, 0, 0),
                    EndTime = new TimeSpan(14, 0, 0),
                    IsActive = true
                });
            }
            if (!await context.Shifts.AnyAsync(s => s.ShiftCode == "CA02"))
            {
                context.Shifts.Add(new Shift
                {
                    ShiftCode = "CA02",
                    ShiftName = "Ca Chiều",
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0),
                    IsActive = true
                });
            }
            if (!await context.Shifts.AnyAsync(s => s.ShiftCode == "CA03"))
            {
                context.Shifts.Add(new Shift
                {
                    ShiftCode = "CA03",
                    ShiftName = "Ca Tối (Qua đêm)",
                    StartTime = new TimeSpan(22, 0, 0),
                    EndTime = new TimeSpan(6, 0, 0),
                    IsActive = true
                });
            }
            await context.SaveChangesAsync();

            // 6. TÀI KHOẢN MẪU BẰNG BCRYPT
            string defaultHash = BCrypt.Net.BCrypt.HashPassword("123456");

            // Không ghi đè mật khẩu của tài khoản đã tồn tại. Hash SHA-256 cũ
            // chỉ được nâng cấp sang BCrypt sau khi người dùng đăng nhập hợp lệ.

            // Đảm bảo có tài khoản Manager mẫu
            if (!await context.Users.AnyAsync(u => u.Username == "manager"))
            {
                // Kiểm tra xem đã có nhân viên quản lý chưa
                var managerEmp = await context.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == "NV005");
                bool managerHasAccount = managerEmp != null &&
                    await context.Users.AnyAsync(u => u.EmployeeID == managerEmp.EmployeeID);
                if (!managerHasAccount)
                {
                    context.Users.Add(new User
                    {
                        EmployeeID = managerEmp!.EmployeeID,
                        RoleID = managerRole.RoleID,
                        Username = "manager",
                        PasswordHash = defaultHash,
                        IsActive = true
                    });
                }
            }

            // Chỉ tạo tài khoản mẫu khi username và nhân viên đều chưa có tài khoản.
            // Sau khi được tạo, mật khẩu chỉ được thay đổi qua AuthService/UserService.
            var initialAccounts = new[]
            {
                new { Username = "admin", EmployeeCode = "NV001", RoleID = adminRole.RoleID },
                new { Username = "staff01", EmployeeCode = "NV002", RoleID = staffRole.RoleID },
                new { Username = "staff02", EmployeeCode = "NV003", RoleID = staffRole.RoleID }
            };

            foreach (var account in initialAccounts)
            {
                bool usernameExists = await context.Users.AnyAsync(u => u.Username == account.Username);
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == account.EmployeeCode);
                bool employeeHasAccount = employee != null &&
                    await context.Users.AnyAsync(u => u.EmployeeID == employee.EmployeeID);

                if (!usernameExists && employee != null && !employeeHasAccount)
                {
                    context.Users.Add(new User
                    {
                        EmployeeID = employee.EmployeeID,
                        RoleID = account.RoleID,
                        Username = account.Username,
                        PasswordHash = defaultHash,
                        IsActive = true
                    });
                }
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
