using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Login.DAL;
using Login.DAL.Entities;

namespace Login.BLL.Services
{
    public class BranchService
    {
        private readonly BakeryDbContext _context;

        public BranchService(BakeryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Branch>> GetAllAsync(bool? onlyActive = null, string? searchKeyword = null)
        {
            var query = _context.Branches
                .Include(b => b.Employees)
                .AsQueryable();

            if (onlyActive.HasValue)
            {
                query = query.Where(b => b.IsActive == onlyActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                string kw = searchKeyword.Trim().ToLower();
                query = query.Where(b => b.BranchCode.ToLower().Contains(kw) ||
                                         b.BranchName.ToLower().Contains(kw) ||
                                         (b.Address != null && b.Address.ToLower().Contains(kw)) ||
                                         (b.Phone != null && b.Phone.Contains(kw)));
            }

            return await query.OrderBy(b => b.BranchCode).ToListAsync();
        }

        public async Task<Branch?> GetByIdAsync(int branchId)
        {
            return await _context.Branches
                .Include(b => b.Employees)
                .FirstOrDefaultAsync(b => b.BranchID == branchId);
        }

        public async Task<(bool Success, string Message)> CreateAsync(Branch branch)
        {
            if (string.IsNullOrWhiteSpace(branch.BranchCode))
                return (false, "Mã chi nhánh không được để trống!");

            if (string.IsNullOrWhiteSpace(branch.BranchName))
                return (false, "Tên chi nhánh không được để trống!");

            branch.BranchCode = branch.BranchCode.Trim().ToUpper();

            // Kiểm tra mã chi nhánh duy nhất
            bool exists = await _context.Branches.AnyAsync(b => b.BranchCode == branch.BranchCode);
            if (exists)
            {
                return (false, $"Mã chi nhánh '{branch.BranchCode}' đã tồn tại trong hệ thống!");
            }

            branch.CreatedAt = DateTime.Now;
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return (true, "Thêm chi nhánh thành công!");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Branch branch)
        {
            var existing = await _context.Branches.FindAsync(branch.BranchID);
            if (existing == null)
                return (false, "Không tìm thấy chi nhánh cần cập nhật!");

            branch.BranchCode = branch.BranchCode.Trim().ToUpper();

            // Kiểm tra trùng mã với chi nhánh khác
            bool codeDuplicate = await _context.Branches
                .AnyAsync(b => b.BranchID != branch.BranchID && b.BranchCode == branch.BranchCode);
            if (codeDuplicate)
            {
                return (false, $"Mã chi nhánh '{branch.BranchCode}' đã được sử dụng bởi chi nhánh khác!");
            }

            existing.BranchCode = branch.BranchCode;
            existing.BranchName = branch.BranchName.Trim();
            existing.Address = branch.Address?.Trim();
            existing.Phone = branch.Phone?.Trim();
            existing.IsActive = branch.IsActive;

            await _context.SaveChangesAsync();
            return (true, "Cập nhật chi nhánh thành công!");
        }

        public async Task<(bool Success, string Message)> ToggleStatusAsync(int branchId)
        {
            var branch = await _context.Branches.FindAsync(branchId);
            if (branch == null) return (false, "Không tìm thấy chi nhánh!");

            branch.IsActive = !branch.IsActive;
            await _context.SaveChangesAsync();

            string statusText = branch.IsActive ? "kích hoạt" : "ngừng hoạt động";
            return (true, $"Đã chuyển chi nhánh sang trạng thái {statusText}!");
        }

        public async Task<(bool Success, string Message)> SoftDeleteAsync(int branchId)
        {
            var branch = await _context.Branches
                .Include(b => b.Employees)
                .FirstOrDefaultAsync(b => b.BranchID == branchId);

            if (branch == null) return (false, "Không tìm thấy chi nhánh!");

            // Kiểm tra nhân viên đang hoạt động tại chi nhánh
            int activeEmployees = branch.Employees.Count(e => e.Status == "ACTIVE");
            if (activeEmployees > 0)
            {
                return (false, $"Không thể xóa chi nhánh vì vẫn còn {activeEmployees} nhân viên đang làm việc. Vui lòng điều chuyển nhân viên trước!");
            }

            branch.IsActive = false;
            await _context.SaveChangesAsync();
            return (true, "Đã ngừng hoạt động chi nhánh (xóa mềm) thành công!");
        }
    }
}
