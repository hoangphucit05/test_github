using System;
using System.Collections.Generic;

namespace Login.DAL.Entities
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string Position { get; set; } = null!;
        public DateOnly HireDate { get; set; }
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, INACTIVE, RESIGNED
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key to Branch (nullable for backward compatibility with existing data)
        public int? BranchID { get; set; }

        // Navigation properties
        public virtual Branch? Branch { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<EmployeeShift> EmployeeShifts { get; set; } = new List<EmployeeShift>();
    }
}
