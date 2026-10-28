# ARCHITECTURE.md — Kiến Trúc Kỹ Thuật

## Tổng Quan

Dự án dùng kiến trúc **2 lớp đơn giản** (phù hợp với WinForms học tập):

```
┌──────────────────────────────────────────┐
│          PRESENTATION LAYER              │
│   FormLogin.cs    FormMain.cs            │
│   FormLogin.Designer.cs  FormMain.Designer.cs │
└──────────────┬───────────────────────────┘
               │  gọi trực tiếp
┌──────────────▼───────────────────────────┐
│           DATA ACCESS LAYER              │
│   DatabaseHelper.cs  (static methods)   │
│   UserSession.cs     (static state)     │
└──────────────┬───────────────────────────┘
               │  SQL qua SqlConnection
┌──────────────▼───────────────────────────┐
│         SQL SERVER DATABASE              │
│   BanhMiShopDB @ localhost\SQLEXPRESS01 │
│   Tables / Views / Stored Procedures    │
└──────────────────────────────────────────┘
```

---

## Luồng Đăng Nhập

```
Program.cs (entry point)
    └─> Mở FormLogin (ShowDialog)
           │
           ├─ Nhập username / password
           ├─ Bấm "Đăng nhập"
           │      └─> DatabaseHelper.HashPassword(password) → byte[32]
           │      └─> Query: SELECT u.*, e.FullName, r.RoleName
           │              FROM Users u JOIN Employees e JOIN Roles r
           │              WHERE Username=@u AND PasswordHash=@p AND IsActive=1
           │
           ├─ Tìm thấy → UserSession.Set(UserID, EmployeeID, FullName, RoleName)
           │             DialogResult = OK
           │
           └─ Program.cs nhận OK → Mở FormMain (Application.Run)
                  └─> FormMain_Load()
                         ├─ lblUserInfo.Text = UserSession.FullName
                         └─ if (!IsAdmin): ẩn btnNavEmployees, btnNavReports
```

---

## FormMain — Pattern Sidebar + Hidden TabControl

FormMain dùng pattern: **TabControl với tab headers ẩn** (ItemSize.Height = 1px),
kết hợp với các Button sidebar để chuyển tab.

```
FormMain (Maximized, WindowState=Maximized)
├── panelTop      (Dock=Top, H=65)    ← Header cam #D35400
├── panelSidebar  (Dock=Left, W=220)  ← Sidebar tối #2C3E50
│   ├── btnNavPOS          (Dock=Top, H=55) → SwitchTab(0)
│   ├── btnNavProducts     (Dock=Top, H=55) → SwitchTab(1)
│   ├── btnNavIngredients  (Dock=Top, H=55) → SwitchTab(2)
│   ├── btnNavReports      (Dock=Top, H=55) → SwitchTab(3)
│   └── btnNavEmployees    (Dock=Top, H=55) → SwitchTab(4)
└── panelContent  (Dock=Fill)
    └── tabMain   (hidden headers, Dock=Fill)
        ├── tabPOS [0]          → InitializePOSTab()
        ├── tabProducts [1]     → InitializeProductsTab()
        ├── tabIngredients [2]  → InitializeIngredientsTab()
        ├── tabReports [3]      → InitializeReportsTab()
        └── tabEmployees [4]    → InitializeEmployeesTab()
```

**Lưu ý:** Mỗi tab được khởi tạo bởi `InitializeXxxTab()` trong `FormMain.cs`.
UI của tab được tạo bằng **code C#** (không dùng VS Designer kéo thả).

---

## POS Tab — Layout SplitContainer

```
tabPOS
└── SplitContainer (Dock=Fill, SplitterDistance=640)
    ├── Panel1 (Left — Thực Đơn)
    │   └── panelMenu (Dock=Fill, Padding=10)
    │       └── groupBox "THỰC ĐƠN MÓN" (Dock=Fill)
    │           ├── pnlMenuFilter (Dock=Top, H=45)
    │           │   ├── Label "Danh mục:" + cboPOSCategory (ComboBox)
    │           │   └── Label "Tìm món:" + txtPOSSearch (TextBox)
    │           ├── dgvPOSMenu (Dock=Fill)  ← DataGridView sản phẩm
    │           └── btnAddCart (Dock=Bottom, H=40)
    └── Panel2 (Right — Đơn Hàng)
        └── panelCart (Dock=Fill, Padding=10)
            └── groupBox "ĐƠN HÀNG HIỆN TẠI" (Dock=Fill)
                ├── dgvPOSCart (Dock=Fill)  ← Giỏ hàng (cột thủ công)
                └── pnlCheckout (Dock=Bottom, H=280)
                    ├── Label Tổng tiền + lblPOSTotal
                    ├── Label Giảm giá + numPOSDiscount
                    ├── Label KHÁCH PHẢI TRẢ + lblPOSFinal (đỏ, bold)
                    ├── Label Hình thức TT + cboPOSPaymentMethod
                    ├── Label Tiền khách đưa + txtPOSCustomerMoney
                    ├── Label Tiền thối + lblPOSChange (xanh)
                    ├── Label Ghi chú + txtPOSNote
                    ├── btnPOSCheckout (cam, W=210, H=42)
                    └── btnPOSClear (xám, W=95, H=42)
```

---

## DataGridView Cart — Cột Thủ Công

```csharp
dgvPOSCart.AutoGenerateColumns = false;  // QUAN TRỌNG

// Các cột định nghĩa thủ công:
// ProductID   → Visible=false (ẩn, dùng nội bộ)
// ProductCode → Visible=false (ẩn)
// ProductName → AutoSizeMode=Fill, MinimumWidth=120 (tự giãn)
// UnitPrice   → Width=90, Format="N0", Align=Right
// Quantity    → Width=48, Align=Center (có thể edit)
// TotalAmount → Width=100, Format="N0", ReadOnly=true
```

---

## DatabaseHelper — API

```csharp
public static class DatabaseHelper
{
    // Connection string
    public static string ConnectionString =
        "Server=localhost\\SQLEXPRESS01;Database=BanhMiShopDB;" +
        "Integrated Security=true;TrustServerCertificate=True;";

    // Trả về DataTable (dùng cho grid, report)
    public static DataTable ExecuteQuery(string sql, List<SqlParameter>? p = null)

    // INSERT/UPDATE/DELETE → trả về số dòng bị ảnh hưởng
    public static int ExecuteNonQuery(string sql, List<SqlParameter>? p = null)

    // Trả về scalar (COUNT, MAX, ...)
    public static object? ExecuteScalar(string sql, List<SqlParameter>? p = null)

    // SHA-256 → byte[32], tương thích HASHBYTES SQL Server
    public static byte[] HashPassword(string password)
}
```

---

## UserSession — Global State

```csharp
public static class UserSession
{
    public static int    UserID     { get; set; }
    public static int    EmployeeID { get; set; }
    public static string FullName   { get; set; } = "";
    public static string RoleName   { get; set; } = "";  // "ADMIN" | "STAFF"
    public static bool   IsAdmin    => RoleName == "ADMIN";
    public static void   Clear()    { /* reset tất cả về default */ }
}
```

---

## DPI & Layout Rules

| Rule | Lý do |
|------|-------|
| `AutoScaleMode.None` trên FormLogin | Tránh UI bị scale sai trên màn hình 125% DPI |
| `Location` tuyệt đối cho controls trong FormLogin | Kiểm soát chính xác vị trí |
| `Dock` cho controls trong FormMain | Tự co giãn khi cửa sổ thay đổi kích thước |
| FormMain `WindowState = Maximized` | Tận dụng toàn bộ màn hình |
| `SplitterDistance = 640` trong POS | Đủ không gian cho cả thực đơn và giỏ hàng |
