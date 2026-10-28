using System;

namespace Login.DAL.Entities
{
    public class EmployeeShift
    {
        public int EmployeeShiftID { get; set; }
        public int EmployeeID { get; set; }
        public int ShiftID { get; set; }
        public DateOnly WorkDate { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Status { get; set; } = "SCHEDULED"; // SCHEDULED, WORKING, COMPLETED, ABSENT
        public string? Note { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual Shift Shift { get; set; } = null!;
    }
}
