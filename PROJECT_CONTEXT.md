# PROJECT_CONTEXT.md — Bối Cảnh & Lịch Sử Dự Án

## 🎯 Mục Tiêu

Dự án được tạo với mục tiêu **học tập lập trình Windows Forms (WinForms) bằng C# trên .NET 10**.
Đây là bài tập thực hành xây dựng ứng dụng quản lý bán hàng cho một tiệm bánh mì nhỏ — đủ phức tạp để học các khái niệm thực tế nhưng không quá lớn.

---

## 🛠️ Công Cụ Đã Sử Dụng

| Công cụ | Phiên bản | Mục đích |
|---------|-----------|---------|
| **Visual Studio Community** | 2022/2026 | IDE chính để viết code C# |
| **.NET SDK** | 10.0.400 | Framework chạy ứng dụng |
| **SQL Server Express** | 2019/2022 | Cơ sở dữ liệu |
| **SSMS** | 20+ | Quản lý SQL Server, chạy script |
| **Microsoft.Data.SqlClient** | 7.0.2 | NuGet package kết nối SQL từ C# |
| **Google Antigravity (AI)** | — | Hỗ trợ viết code, debug, thiết kế UI |

---

## 🖥️ Môi Trường Máy Tính

- **OS:** Windows (với DPI scaling 125%)
- **Độ phân giải màn hình:** ~1366×768
- **SQL Server instance:** `localhost\SQLEXPRESS01`
- **Database:** `BanhMiShopDB`
- **Xác thực SQL:** Windows Authentication (`Integrated Security=true`)

---

## 📅 Lịch Sử Làm Việc (24/08/2026)

### Sáng — Cài Đặt & Database
- Cài Visual Studio Community với workload **.NET desktop development**
- Tạo project WinForms mới trong VS (`net10.0-windows`)
- Viết SQL script đầy đủ: bảng, view, stored procedure, dữ liệu mẫu
- Chạy script trong SSMS → tạo thành công `BanhMiShopDB`

### Chiều — Backend & Form Chính
- Thêm NuGet `Microsoft.Data.SqlClient` vào project
- Tạo `DatabaseHelper.cs` — lớp truy cập dữ liệu tập trung
- Tạo `UserSession.cs` — lưu thông tin đăng nhập toàn cục
- Tạo `FormLogin.cs/.Designer.cs` — đăng nhập với xác thực SHA-256
- Tạo `FormMain.cs/.Designer.cs` — giao diện chính với 5 module đầy đủ

### Tối — Sửa Giao Diện & Tài Liệu
- Sửa `FormLogin.Designer.cs`:
  - Giao diện bị cắt chữ do DPI 125% → dùng `AutoScaleMode.None`
  - Form quá cao (580px) → giảm xuống 560px, điều chỉnh layout
  - Icon 🥪 bị cắt → tăng `panelTop.Height` = 148px
- Sửa POS tab (`FormMain.cs`):
  - Các cột giỏ hàng bị chồng lên nhau
  - `SplitterDistance` 750 → 640
  - `AutoGenerateColumns=false`, dùng cột thủ công với độ rộng cố định
- Tạo 7 file tài liệu Markdown

---

## 💡 Những Điều Đã Học

1. Cách tạo WinForms project với .NET 10
2. Kết nối SQL Server từ C# dùng `Microsoft.Data.SqlClient`
3. Hash mật khẩu SHA-256 tương thích `HASHBYTES` SQL Server
4. `SplitContainer` để chia giao diện POS thành 2 cột
5. `DataGridView` với `AutoGenerateColumns=false` để kiểm soát cột
6. `AutoScaleMode.None` để tránh lỗi DPI scaling
7. `TabControl` với hidden tabs + sidebar buttons để navigate
8. Session state dùng `static class`
9. Phân quyền theo Role (ADMIN/STAFF)
10. Stored Procedure tự động trừ nguyên liệu khi thanh toán
11. Views SQL để tính toán báo cáo phức tạp
