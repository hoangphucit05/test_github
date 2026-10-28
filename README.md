# 🥪 Phần Mềm Quản Lý & Bán Hàng Quán Bánh Mì

> Ứng dụng Windows Desktop (WinForms C#) quản lý toàn diện một tiệm bánh mì nhỏ: bán hàng POS, quản lý thực đơn, kho nguyên liệu, báo cáo doanh thu và nhân viên.

---

## 📌 Thông Tin Dự Án

| Mục | Chi tiết |
|-----|----------|
| **Tên dự án** | BaiTap_WinForms (Tiệm Bánh Mì Ngon) |
| **Loại ứng dụng** | Windows Forms Desktop (.NET 10) |
| **Ngôn ngữ** | C# |
| **Cơ sở dữ liệu** | SQL Server Express (BanhMiShopDB) |
| **IDE** | Visual Studio Community 2022/2026 |
| **Workload VS** | .NET desktop development |
| **NuGet** | Microsoft.Data.SqlClient v7.0.2 |
| **Người tạo** | Quoc |
| **Ngày bắt đầu** | 24/08/2026 |

---

## 🚀 Cách Chạy Dự Án

### Yêu cầu hệ thống
- Windows 10/11
- .NET 10 SDK
- SQL Server Express (instance: `localhost\SQLEXPRESS01`)
- Visual Studio Community 2022+ (workload: **.NET desktop development**)

### Bước 1: Tạo Database
1. Mở **SQL Server Management Studio (SSMS)**
2. Kết nối tới `localhost\SQLEXPRESS01`
3. Mở file `BanhMiShopDB.sql` trong thư mục gốc dự án
4. Bấm **F5** để chạy toàn bộ script → tạo database `BanhMiShopDB`

### Bước 2: Chạy Ứng Dụng
1. Mở file `BaiTap_WinForms.slnx` bằng Visual Studio
2. Bấm **F5** (hoặc nút ▶ Start)
3. Đăng nhập bằng tài khoản mặc định

---

## 🔑 Tài Khoản Mặc Định

| Username | Password | Role |
|----------|----------|------|
| `admin` | `123456` | ADMIN |
| `staff01` | `123456` | STAFF |

> Mật khẩu được mã hóa bằng **SHA-256** (tương thích `HASHBYTES('SHA2_256',...)` trong SQL Server).

---

## 🏗️ Cấu Trúc Thư Mục

```
BaiTap_WinForms/
├── BanhMiShopDB.sql              ← Script SQL tạo database đầy đủ
├── BaiTap_WinForms.slnx          ← File Solution Visual Studio
├── README.md                     ← Tổng quan dự án
├── PROJECT_CONTEXT.md            ← Bối cảnh & lịch sử làm việc
├── ARCHITECTURE.md               ← Kiến trúc kỹ thuật
├── DATABASE.md                   ← Thiết kế database
├── CODING_RULES.md               ← Quy tắc code
├── CHANGELOG.md                  ← Lịch sử thay đổi
├── AI_INSTRUCTIONS.md            ← Hướng dẫn cho AI
└── BaiTap_WinForms/              ← Thư mục project C#
    ├── Data/
    │   └── DatabaseHelper.cs     ← Kết nối SQL, hash mật khẩu SHA-256
    ├── Models/
    │   └── UserSession.cs        ← Trạng thái phiên đăng nhập (static)
    ├── Forms/
    │   ├── FormLogin.cs          ← Logic đăng nhập
    │   ├── FormLogin.Designer.cs ← UI đăng nhập (520x560, cam/trắng)
    │   ├── FormMain.cs           ← Logic chính + InitializeXxxTab()
    │   └── FormMain.Designer.cs  ← UI khung chính (sidebar + tabControl)
    ├── Program.cs                ← Entry point
    └── BaiTap_WinForms.csproj   ← Cấu hình project (.NET 10 Windows)
```

---

## 🎯 Các Module Chức Năng

| Module | Icon | Mô tả |
|--------|------|-------|
| **BÁN HÀNG (POS)** | 🛒 | Chọn món, tính tiền, thanh toán, tự động trừ nguyên liệu qua stored procedure |
| **THỰC ĐƠN** | 🥪 | CRUD sản phẩm và danh mục món ăn |
| **KHO NGUYÊN LIỆU** | 🥬 | Theo dõi tồn kho, cảnh báo hết hàng (view v_LowStock), nhập kho |
| **BÁO CÁO** | 📊 | Doanh thu theo ngày (v_DailyRevenue), sản phẩm bán chạy (v_ProductSales) |
| **NHÂN VIÊN** | 👥 | Danh sách nhân viên và vai trò |

---

## 👥 Phân Quyền

| Role | Quyền truy cập |
|------|----------------|
| **ADMIN** | Toàn bộ 5 module |
| **STAFF** | Chỉ module BÁN HÀNG (POS) |
