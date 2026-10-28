using System;
using System.Collections.Generic;

namespace Login.DAL.Entities
{
    public class Branch
    {
        public int BranchID { get; set; }
        public string BranchCode { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
