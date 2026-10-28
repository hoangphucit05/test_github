using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Login.DAL;
using Login.DAL.Entities;

namespace Login.BLL.Services
{
    public class ShiftService
    {
        private readonly BakeryDbContext _context;

        public ShiftService(BakeryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Shift>> GetAllAsync(bool? onlyActive = null)
        {
            var query = _context.Shifts.AsQueryable();

            if (onlyActive.HasValue)
            {
                query = query.Where(s => s.IsActive == onlyActive.Value);
            }

            return await query.OrderBy(s => s.StartTime).ToListAsync();
        }

        public async Task<Shift?> GetByIdAsync(int shiftId)
        {
            return await _context.Shifts
                .Include(s => s.EmployeeShifts)
                .FirstOrDefaultAsync(s => s.ShiftID == shiftId);
        }

        public async Task<(bool Success, string Message)> CreateAsync(Shift shift)
        {
            if (string.IsNullOrWhiteSpace(shift.ShiftCode))
                return (false, "Mã ca làm không được để trống!");

            if (string.IsNullOrWhiteSpace(shift.ShiftName))
                return (false, "Tên ca làm không được để trống!");

            if (shift.StartTime == shift.EndTime)
                return (false, "Giờ bắt đầu và giờ kết thúc không được trùng nhau!");

            shift.ShiftCode = shift.ShiftCode.Trim().ToUpper();

            bool exists = await _context.Shifts.AnyAsync(s => s.ShiftCode == shift.ShiftCode);
            if (exists)
            {
                return (false, $"Mã ca làm '{shift.ShiftCode}' đã tồn tại!");
            }

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();
            return (true, "Thêm ca làm mới thành công!");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Shift shift)
        {
            var existing = await _context.Shifts.FindAsync(shift.ShiftID);
            if (existing == null)
                return (false, "Không tìm thấy ca làm cần cập nhật!");

            if (shift.StartTime == shift.EndTime)
                return (false, "Giờ bắt đầu và giờ kết thúc không được trùng nhau!");

            shift.ShiftCode = shift.ShiftCode.Trim().ToUpper();

            bool codeDuplicate = await _context.Shifts
                .AnyAsync(s => s.ShiftID != shift.ShiftID && s.ShiftCode == shift.ShiftCode);
            if (codeDuplicate)
            {
                return (false, $"Mã ca làm '{shift.ShiftCode}' đã được sử dụng bởi ca khác!");
            }

            existing.ShiftCode = shift.ShiftCode;
            existing.ShiftName = shift.ShiftName.Trim();
            existing.StartTime = shift.StartTime;
            existing.EndTime = shift.EndTime;
            existing.IsActive = shift.IsActive;

            await _context.SaveChangesAsync();
            return (true, "Cập nhật ca làm thành công!");
        }

        public async Task<(bool Success, string Message)> ToggleStatusAsync(int shiftId)
        {
            var shift = await _context.Shifts.FindAsync(shiftId);
            if (shift == null) return (false, "Không tìm thấy ca làm!");

            shift.IsActive = !shift.IsActive;
            await _context.SaveChangesAsync();

            string statusText = shift.IsActive ? "kích hoạt" : "ngừng sử dụng";
            return (true, $"Đã chuyển ca làm sang trạng thái {statusText}!");
        }

        public async Task<(bool Success, string Message)> SoftDeleteAsync(int shiftId)
        {
            var shift = await _context.Shifts
                .Include(s => s.EmployeeShifts)
                .FirstOrDefaultAsync(s => s.ShiftID == shiftId);

            if (shift == null) return (false, "Không tìm thấy ca làm!");

            // Kiểm tra xem ca làm có lịch phân công chưa hoàn thành trong tương lai
            var today = DateOnly.FromDateTime(DateTime.Today);
            int pendingAssignments = shift.EmployeeShifts.Count(es => es.WorkDate >= today && es.Status != "COMPLETED");
            if (pendingAssignments > 0)
            {
                return (false, $"Không thể ngừng sử dụng ca làm vì đang có {pendingAssignments} lịch phân công sắp tới chưa hoàn thành!");
            }

            shift.IsActive = false;
            await _context.SaveChangesAsync();
            return (true, "Đã ngừng sử dụng ca làm (xóa mềm) thành công!");
        }

        // --- PHÂN CÔNG CA LÀM (EmployeeShift) ---
        public async Task<List<EmployeeShift>> GetShiftAssignmentsAsync(DateOnly fromDate, DateOnly toDate, int? branchId = null)
        {
            var query = _context.EmployeeShifts
                .Include(es => es.Employee)
                    .ThenInclude(e => e.Branch)
                .Include(es => es.Shift)
                .Where(es => es.WorkDate >= fromDate && es.WorkDate <= toDate);

            if (branchId.HasValue)
            {
                query = query.Where(es => es.Employee.BranchID == branchId.Value);
            }

            return await query.OrderBy(es => es.WorkDate).ThenBy(es => es.Shift.StartTime).ToListAsync();
        }

        public async Task<(bool Success, string Message)> AssignShiftAsync(int employeeId, int shiftId, DateOnly workDate, string? note = null)
        {
            var emp = await _context.Employees.FindAsync(employeeId);
            if (emp == null || emp.Status != "ACTIVE")
                return (false, "Nhân viên không tồn tại hoặc đã nghỉ việc!");

            var shift = await _context.Shifts.FindAsync(shiftId);
            if (shift == null || !shift.IsActive)
                return (false, "Ca làm không tồn tại hoặc đã ngừng sử dụng!");

            // Kiểm tra trùng lịch phân công
            bool alreadyAssigned = await _context.EmployeeShifts
                .AnyAsync(es => es.EmployeeID == employeeId && es.ShiftID == shiftId && es.WorkDate == workDate);
            if (alreadyAssigned)
            {
                return (false, "Nhân viên đã được xếp ca làm này trong ngày đã chọn!");
            }

            var assignment = new EmployeeShift
            {
                EmployeeID = employeeId,
                ShiftID = shiftId,
                WorkDate = workDate,
                Status = "SCHEDULED",
                Note = note
            };

            _context.EmployeeShifts.Add(assignment);
            await _context.SaveChangesAsync();
            return (true, "Phân công ca làm thành công!");
        }
    }
}
