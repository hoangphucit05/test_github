# CHANGELOG.md — Lịch Sử Thay Đổi

Tất cả thay đổi quan trọng của dự án được ghi lại tại đây.

---

## [v0.6] — 24/08/2026 (Tối)

### Fixed
- **POS Tab — Cột giỏ hàng bị chồng lên nhau**
  - Nguyên nhân: `AutoSizeColumnsMode.Fill` chia đều 6 cột vào ~400px → cột quá hẹp
  - Sửa: Dùng `AutoGenerateColumns = false`, định nghĩa cột thủ công
  - Cột: ProductName (Fill), UnitPrice (90px), Quantity (48px), TotalAmount (100px)
  - ProductID và ProductCode được ẩn (Visible=false)
- **POS Tab — Panel đơn hàng quá hẹp**
  - Nguyên nhân: `SplitterDistance = 750` chiếm quá nhiều chỗ
  - Sửa: Giảm xuống `640` để panel phải rộng hơn ~100px

---

## [v0.5] — 24/08/2026 (Chiều-Tối)

### Fixed
- **FormLogin — Giao diện bị cắt chữ do DPI Scaling 125%**
  - Nguyên nhân: Form không dùng `AutoScaleMode.None`, chữ bị truncate bên phải
  - Sửa: `AutoScaleMode = AutoScaleMode.None` + form rộng 520px, centered labels
- **FormLogin — Form cao quá bị tràn xuống dưới taskbar**
  - Nguyên nhân: `ClientSize.Height = 580` + DPI 125% = 725px thực, màn hình 768px
  - Sửa: Giảm xuống `Height = 560`, điều chỉnh `panelCard` và vị trí controls
- **FormLogin — Icon 🥪 bị cắt phần dưới**
  - Nguyên nhân: `panelTop.Height = 140` không đủ cho emoji 28pt
  - Sửa: Tăng lên `Height = 148`, `lblIcon.Size.Height = 46`
- **FormLogin — Các phần tử quá sát nhau**
  - Sửa: Tăng khoảng cách giữa các label→textbox, checkbox→button

### Added
- Viết lại `FormLogin.Designer.cs` với thiết kế mới:
  - Header cam `#D9531E` với emoji 🥪, tiêu đề và subtitle
  - Card trắng với bo viền, shadow-like border
  - Nút ĐĂNG NHẬP cam nổi bật
  - Hint text nhắc tài khoản mặc định

---

## [v0.4] — 24/08/2026 (Chiều)

### Added
- **`FormMain.cs` + `FormMain.Designer.cs`** — Giao diện chính hoàn chỉnh
- **Module POS (BÁN HÀNG)**:
  - Danh sách sản phẩm với lọc danh mục và tìm kiếm
  - Giỏ hàng có thể chỉnh số lượng
  - Tính tổng tiền, giảm giá, tiền thối
  - Hỗ trợ CASH và chuyển khoản
  - Gọi `sp_DeductStockForOrder` để trừ nguyên liệu tự động
- **Module THỰC ĐƠN**: CRUD sản phẩm và danh mục
- **Module KHO**: Tồn kho, filter v_LowStock, nhập kho nhanh
- **Module BÁO CÁO**: Doanh thu (v_DailyRevenue), bán chạy (v_ProductSales)
- **Module NHÂN VIÊN**: Danh sách nhân viên
- Phân quyền: STAFF ẩn module Báo cáo và Nhân viên

---

## [v0.3] — 24/08/2026 (Buổi trưa)

### Added
- **`Data/DatabaseHelper.cs`**:
  - Connection string tới `localhost\SQLEXPRESS01`
  - `ExecuteQuery()`, `ExecuteNonQuery()`, `ExecuteScalar()`
  - `HashPassword()` — SHA-256 UTF-8
- **`Models/UserSession.cs`** — Static session holder
- **`FormLogin.cs` + `FormLogin.Designer.cs`**:
  - Xác thực username + password hash
  - Checkbox "Hiện mật khẩu"
  - Nút Test kết nối SQL
  - Tự động điền admin/123456

---

## [v0.2] — 24/08/2026 (Buổi sáng)

### Added
- **`BanhMiShopDB.sql`** — Script SQL Server đầy đủ:
  - 10 bảng: Roles, Employees, Users, Categories, Products, Ingredients, ProductRecipes, Orders, OrderDetails, StockTransactions
  - 3 views: `v_DailyRevenue`, `v_ProductSales`, `v_LowStock`
  - 1 stored procedure: `sp_DeductStockForOrder`
  - Dữ liệu mẫu: 2 users, 12 sản phẩm, 8 nguyên liệu

---

## [v0.1] — 24/08/2026 (Khởi đầu)

### Added
- Cài Visual Studio Community với workload .NET desktop development
- Tạo project WinForms mới: `BaiTap_WinForms` (net10.0-windows)
- Thêm NuGet: `Microsoft.Data.SqlClient` v7.0.2
- Xóa Form1 mặc định, tạo cấu trúc thư mục `Data/`, `Models/`, `Forms/`
- Build thành công (0 errors)
