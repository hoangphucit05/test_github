using System;
using System.Collections.Generic;

namespace Login.DAL.Entities
{
    public class Shift
    {
        public int ShiftID { get; set; }
        public string ShiftCode { get; set; } = null!;
        public string ShiftName { get; set; } = null!;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<EmployeeShift> EmployeeShifts { get; set; } = new List<EmployeeShift>();
    }
}
