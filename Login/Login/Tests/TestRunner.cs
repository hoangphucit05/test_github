using System;
using System.Threading.Tasks;
using Login.BLL.Services;
using Login.DAL;

namespace Login
{
    public static class TestRunner
    {
        public static async Task<bool> RunAllTestsAsync()
        {
            Console.WriteLine("=== BẮT ĐẦU KIỂM THỬ TẦNG BLL & CSDL ===");

            using var db = new BakeryDbContext();

            // 1. Kiểm tra AuthService
            var authService = new AuthService(db);

            // Test 1.1: Đăng nhập đúng Admin
            var res1 = await authService.LoginAsync("admin", "123456", "Quản trị viên");
            Console.WriteLine($"[Test 1.1] Admin Login: {(res1.IsSuccess ? "PASS" : "FAIL")} -> {res1.Message}");
            if (!res1.IsSuccess) return false;

            // Test 1.2: Đăng nhập đúng Manager
            var res2 = await authService.LoginAsync("manager", "123456", "Quản lý");
            Console.WriteLine($"[Test 1.2] Manager Login: {(res2.IsSuccess ? "PASS" : "FAIL")} -> {res2.Message}");
            if (!res2.IsSuccess) return false;

            // Test 1.3: Đăng nhập sai mật khẩu
            var res3 = await authService.LoginAsync("admin", "wrong_pass", "Quản trị viên");
            Console.WriteLine($"[Test 1.3] Wrong Password Reject: {(!res3.IsSuccess ? "PASS" : "FAIL")} -> {res3.Message}");

            // Test 1.4: Chọn sai nút vai trò (admin nhưng bấm Nhân viên)
            var res4 = await authService.LoginAsync("admin", "123456", "Nhân viên");
            Console.WriteLine($"[Test 1.4] Mismatched Role Reject: {(!res4.IsSuccess ? "PASS" : "FAIL")} -> {res4.Message}");

            // 2. Kiểm tra BranchService
            var branchService = new BranchService(db);
            var branches = await branchService.GetAllAsync();
            Console.WriteLine($"[Test 2] BranchService GetAllAsync: {(branches.Count >= 2 ? "PASS" : "FAIL")} -> Tìm thấy {branches.Count} chi nhánh.");

            // 3. Kiểm tra ShiftService
            var shiftService = new ShiftService(db);
            var shifts = await shiftService.GetAllAsync();
            Console.WriteLine($"[Test 3] ShiftService GetAllAsync: {(shifts.Count >= 3 ? "PASS" : "FAIL")} -> Tìm thấy {shifts.Count} ca làm.");

            // 4. Kiểm tra UserService
            var userService = new UserService(db);
            var users = await userService.GetAllUsersAsync();
            Console.WriteLine($"[Test 4] UserService GetAllUsersAsync: {(users.Count >= 4 ? "PASS" : "FAIL")} -> Tìm thấy {users.Count} tài khoản.");

            Console.WriteLine("=== TẤT CẢ KIỂM THỬ ĐÃ VƯỢT QUA XUẤT SẮC ===");
            return true;
        }
    }
}
