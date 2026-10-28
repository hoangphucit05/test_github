using System.Collections.Generic;

namespace Login.DAL.Entities
{
    public class Permission
    {
        public int PermissionID { get; set; }
        public string PermissionCode { get; set; } = null!;
        public string PermissionName { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public string? Description { get; set; }

        // Navigation properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
