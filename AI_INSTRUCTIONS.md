# AI_INSTRUCTIONS.md — Hướng Dẫn Cho AI

> Đây là file dành cho AI assistant (Claude, Gemini, ChatGPT...) để hiểu nhanh project và tiếp tục hỗ trợ phát triển.

---

## TÓM TẮT PROJECT

- **Loại:** Windows Forms Desktop App (.NET 10, C#)
- **Mục đích:** Quản lý & bán hàng tiệm bánh mì nhỏ
- **Database:** SQL Server Express, instance `localhost\SQLEXPRESS01`, DB `BanhMiShopDB`
- **IDE:** Visual Studio Community 2022/2026
- **Màn hình người dùng:** 1366×768, DPI 125%

---

## FILE MAP

### Files quan trọng nhất cần đọc trước

| File | Mô tả |
|------|-------|
| `BaiTap_WinForms/Forms/FormMain.cs` | **Logic chính** — 5 module + InitializeXxxTab() |
| `BaiTap_WinForms/Forms/FormMain.Designer.cs` | **UI khung** — panelTop, sidebar, tabControl |
| `BaiTap_WinForms/Forms/FormLogin.cs` | Logic đăng nhập |
| `BaiTap_WinForms/Forms/FormLogin.Designer.cs` | UI đăng nhập (đọc để hiểu layout) |
| `BaiTap_WinForms/Data/DatabaseHelper.cs` | Connection string + DB methods |
| `BaiTap_WinForms/Models/UserSession.cs` | Session state static class |
| `BaiTap_WinForms/Program.cs` | Entry point |
| `BanhMiShopDB.sql` | Toàn bộ schema + data SQL Server |

---

## CONNECTION STRING

```csharp
// Vị trí: DatabaseHelper.cs
"Server=localhost\\SQLEXPRESS01;Database=BanhMiShopDB;Integrated Security=true;TrustServerCertificate=True;"
```

⚠️ **KHÔNG THAY ĐỔI** connection string mà không hỏi user, vì phụ thuộc vào cài đặt SQL Server của máy họ.

---

## CÁCH BUILD & CHẠY

```powershell
# Thư mục build
cd c:\Users\quoc_\source\repos\BaiTap_WinForms\BaiTap_WinForms

# Build
dotnet build

# Chạy
dotnet run
```

Hoặc bấm **F5** trong Visual Studio.

---

## WARNINGS AN TOÀN BỎ QUA

```
warning CS8602: Dereference of a possibly null reference.
warning CS8601: Possible null reference assignment.
```

Những warning này xuất hiện ở `FormMain.cs` do fields dùng `= null!` pattern.
Chúng **an toàn** vì luôn được gán trong `InitializeXxxTab()` trước khi sử dụng.

---

## CÁC ĐIỀU KHÔNG ĐƯỢC ĐỔI

| Điều | Lý do |
|------|-------|
| Thuật toán hash SHA-256 với UTF-8 | Phải khớp với `HASHBYTES('SHA2_256',...)` trong SQL Server |
| `AutoScaleMode.None` trên FormLogin | Tránh lỗi DPI trên màn hình 125% |
| `AutoGenerateColumns = false` trên dgvPOSCart | Tránh cột bị chồng lên nhau |
| Connection string format | Phải dùng `localhost\SQLEXPRESS01` (tên instance của user) |
| `WindowState = Maximized` trên FormMain | Yêu cầu thiết kế |

---

## CÁCH THÊM MODULE MỚI

1. Thêm TabPage mới vào `FormMain.Designer.cs`:
```csharp
private System.Windows.Forms.TabPage tabCustomers;
// trong InitializeComponent():
this.tabCustomers = new System.Windows.Forms.TabPage();
this.tabMain.Controls.Add(this.tabCustomers);
```

2. Thêm button sidebar vào `FormMain.Designer.cs`:
```csharp
private System.Windows.Forms.Button btnNavCustomers;
// config giống các btnNav khác (Dock=Top, H=55, FlatStyle.Flat...)
this.btnNavCustomers.Click += new System.EventHandler(this.btnNav_Click);
```

3. Trong `FormMain.cs`:
```csharp
// Thêm vào SwitchTab logic
else if (sender == btnNavCustomers) SwitchTab(5, btnNavCustomers);

// Thêm button vào array trong SwitchTab()
var buttons = new[] { btnNavPOS, ..., btnNavCustomers };

// Viết method khởi tạo tab
private void InitializeCustomersTab() { ... }
private void LoadCustomersTab() { ... }

// Gọi trong FormMain_Load()
InitializeCustomersTab();
LoadCustomersTab();
```

---

## CÁCH THÊM BẢNG DATABASE MỚI

1. Mở SSMS, kết nối `localhost\SQLEXPRESS01`
2. Chạy câu lệnh CREATE TABLE trong `BanhMiShopDB`
3. Cập nhật `BanhMiShopDB.sql` để đồng bộ

---

## CÁCH THÊM FORM MỚI (Popup / Dialog)

```csharp
// Tạo file mới: Forms/FormNewItem.cs
public partial class FormNewItem : Form
{
    public FormNewItem() { InitializeComponent(); }
}

// Tạo FormNewItem.Designer.cs tương ứng

// Mở từ FormMain
var dlg = new FormNewItem();
if (dlg.ShowDialog() == DialogResult.OK)
{
    // Xử lý kết quả
}
```

---

## THÔNG TIN DPI & MÀN HÌNH

- Màn hình user: **1366×768**, DPI scaling **125%**
- Với FormLogin (`AutoScaleMode.None`):
  - `ClientSize.Height` tối đa nên là **~550px** để không bị cắt
  - Công thức: `768px / 1.25 - taskbar(~32) - titlebar(~30) ≈ 552px` usable
- FormMain không bị ảnh hưởng vì dùng `Dock` và `Maximized`

---

## STYLE GUIDE UI

### Màu sắc
| Màu | Hex / RGB | Dùng cho |
|-----|-----------|---------|
| Cam chính | `#D35400` / `RGB(211,84,0)` | Header FormMain, nút chính |
| Cam đăng nhập | `#D9531E` / `RGB(217,83,30)` | Header FormLogin, nút Login |
| Sidebar tối | `#2C3E50` / `RGB(44,62,80)` | Sidebar FormMain |
| Cam active sidebar | `#E67E22` / `RGB(230,126,34)` | Tab đang chọn |
| Xanh lá thêm | `#27AE60` / `RGB(39,174,96)` | Nút thêm vào giỏ |
| Đỏ xóa | `#C0392B` / `RGB(192,57,43)` | Nút logout, số tiền phải trả |
| Xám nhạt | `#F3F4F6` / `RGB(243,244,246)` | Nền form |

### Font
- **Chính:** Segoe UI
- **Header / Title:** Segoe UI, 15-16pt, Bold
- **Label:** Segoe UI, 10-11pt
- **Emoji:** Segoe UI Emoji

---

## TÌNH TRẠNG HIỆN TẠI (24/08/2026)

✅ Đã hoàn thành:
- Database schema + seed data
- Đăng nhập với SHA-256
- Phân quyền ADMIN/STAFF
- POS bán hàng + tự động trừ kho
- CRUD thực đơn
- Quản lý kho nguyên liệu
- Báo cáo doanh thu
- Danh sách nhân viên
- UI đăng nhập đẹp (cam/trắng)
- Cột giỏ hàng không bị chồng

🔲 Có thể làm tiếp:
- Thêm module Khách hàng (Customers)
- Thêm module Nhà cung cấp (Suppliers)
- In hóa đơn (PrintDocument)
- Export báo cáo Excel
- Backup database
- Thêm tìm kiếm nâng cao
- Lịch sử đơn hàng chi tiết
