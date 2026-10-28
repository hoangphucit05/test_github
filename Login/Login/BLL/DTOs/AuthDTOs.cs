using System;
using System.Collections.Generic;

namespace Login.BLL.DTOs
{
    public class UserSession
    {
        public int UserID { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string? Position { get; set; }
        public int? BranchID { get; set; }
        public string? BranchName { get; set; }
        public DateTime? LastLogin { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();

        public bool HasPermission(string permissionCode)
        {
            if (RoleName == "ADMIN") return true;
            return Permissions.Contains(permissionCode);
        }
    }

    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserSession? Session { get; set; }

        public static AuthResult Success(UserSession session, string message = "Đăng nhập thành công!")
        {
            return new AuthResult { IsSuccess = true, Message = message, Session = session };
        }

        public static AuthResult Fail(string message)
        {
            return new AuthResult { IsSuccess = false, Message = message };
        }
    }
}
