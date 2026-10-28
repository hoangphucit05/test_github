using System.Collections.Generic;

namespace Login.DAL.Entities
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = null!;
        public string? Description { get; set; }

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
