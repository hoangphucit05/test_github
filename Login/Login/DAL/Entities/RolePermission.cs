namespace Login.DAL.Entities
{
    public class RolePermission
    {
        public int RoleID { get; set; }
        public int PermissionID { get; set; }

        // Navigation properties
        public virtual Role Role { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
    }
}
