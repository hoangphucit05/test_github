using System;

namespace BaiTap_WinForms.Models
{
    public static class UserSession
    {
        public static int UserID { get; set; }
        public static int EmployeeID { get; set; }
        public static string Username { get; set; } = string.Empty;
        public static string FullName { get; set; } = string.Empty;
        public static string RoleName { get; set; } = string.Empty;
        public static string Position { get; set; } = string.Empty;

        public static bool IsAdmin => RoleName.Equals("ADMIN", StringComparison.OrdinalIgnoreCase);

        public static void Clear()
        {
            UserID = 0;
            EmployeeID = 0;
            Username = string.Empty;
            FullName = string.Empty;
            RoleName = string.Empty;
            Position = string.Empty;
        }
    }
}
