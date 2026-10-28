using System;

namespace Login.DAL.Entities
{
    public class User
    {
        public int UserID { get; set; }
        public int EmployeeID { get; set; }
        public int RoleID { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEnd { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
