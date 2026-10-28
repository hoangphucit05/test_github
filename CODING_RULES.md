# CODING_RULES.md — Quy Tắc Code

## Quy Tắc Đặt Tên

### Controls (WinForms)
| Tiền tố | Loại control | Ví dụ |
|---------|-------------|-------|
| `lbl` | Label | `lblTitle`, `lblUsername` |
| `txt` | TextBox | `txtUsername`, `txtPassword` |
| `btn` | Button | `btnLogin`, `btnExit` |
| `dgv` | DataGridView | `dgvPOSMenu`, `dgvPOSCart` |
| `cbo` | ComboBox | `cboPOSCategory`, `cboPOSPaymentMethod` |
| `num` | NumericUpDown | `numPOSDiscount` |
| `chk` | CheckBox | `chkShowPassword` |
| `grp` | GroupBox | `groupMenu`, `groupCart` |
| `pnl` | Panel | `panelTop`, `panelCard`, `pnlCheckout` |
| `tab` | TabPage | `tabPOS`, `tabProducts` |

### Forms
| Prefix | Ví dụ |
|--------|-------|
| `Form` | `FormLogin`, `FormMain` |

### Biến & Fields
- Private fields dùng tiền tố `_` : `_cartTable`, `_vnCulture`
- Null-forgiving operator `= null!` cho fields khởi tạo muộn trong tab

### Method Names
- `Initialize{Name}Tab()` — khởi tạo UI cho tab
- `Load{Name}Tab()` — tải dữ liệu vào tab
- `Recalculate{Name}()` — tính lại giá trị

---

## UI / Layout Rules

### FormLogin
```csharp
// PHẢI dùng AutoScaleMode.None để tránh DPI scaling lỗi
this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

// Kích thước phù hợp màn hình 1366x768 @ 125% DPI
this.ClientSize = new System.Drawing.Size(520, 560);

// panelTop cần đủ cao để icon emoji không bị cắt
this.panelTop.Height = 148;
```

### FormMain
```csharp
// Luôn mở dạng Maximized
this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

// TabControl header phải được ẩn
this.tabMain.ItemSize = new System.Drawing.Size(0, 1);
this.tabMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
```

### DataGridView Cart
```csharp
// KHÔNG dùng AutoGenerateColumns — tránh cột chồng lên nhau
dgvPOSCart.AutoGenerateColumns = false;

// Đặt DataSource SAU KHI đã định nghĩa cột
dgvPOSCart.Columns.Add(...);
dgvPOSCart.DataSource = _cartTable;
```

---

## Database Rules

### Luôn dùng Parameterized Query
```csharp
// ĐÚNG
var params = new List<SqlParameter>
{
    new SqlParameter("@Username", username),
    new SqlParameter("@PasswordHash", hash)
};
DatabaseHelper.ExecuteQuery(sql, params);

// SAI — dễ bị SQL Injection
string sql = "SELECT * FROM Users WHERE Username='" + username + "'";
```

### Error Handling
```csharp
// Bao bọc tất cả DB calls trong try-catch
try
{
    var dt = DatabaseHelper.ExecuteQuery(sql);
    dgvPOSMenu.DataSource = dt;
}
catch (Exception ex)
{
    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi",
        MessageBoxButtons.OK, MessageBoxIcon.Error);
}
```

---

## Số Và Định Dạng

```csharp
// Dùng culture vi-VN cho số tiền Việt Nam
private readonly CultureInfo _vnCulture = new CultureInfo("vi-VN");

// Format số tiền
lblPOSTotal.Text = total.ToString("N0", _vnCulture) + " đ";

// DataGridView — Format cột số tiền
DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
```

---

## Password Hashing

```csharp
// KHÔNG ĐƯỢC ĐỔI thuật toán hash — phải tương thích HASHBYTES SQL Server
public static byte[] HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
}

// Trong SQL: HASHBYTES('SHA2_256', N'123456')
// Trong C#:  SHA256.ComputeHash(Encoding.UTF8.GetBytes("123456"))
// Kết quả:   byte[32] giống nhau
```

---

## Warnings Có Thể Bỏ Qua

Các warning CS8602 (null reference) xuất hiện nhiều trong FormMain.cs là do
fields kiểu `= null!` (null-forgiving). Chúng **an toàn** vì các field này
được gán giá trị trong `InitializeXxxTab()` trước khi dùng.

```
warning CS8602: Dereference of a possibly null reference. → SAFE TO IGNORE
```

---

## Cấu Trúc InitializeXxxTab()

Mỗi tab được khởi tạo theo pattern:

```csharp
private void InitializePOSTab()
{
    // 1. Set màu nền tab
    tabPOS.BackColor = ...;

    // 2. Khởi tạo DataTable (nếu cần)
    _cartTable.Columns.Add(...);

    // 3. Tạo layout container (SplitContainer / Panel / GroupBox)

    // 4. Tạo controls (DataGridView, Button, Label...)

    // 5. Đăng ký event handlers

    // 6. Add controls vào parent
}

private void LoadXxxTab()
{
    // Tải dữ liệu từ DB vào controls
    // Gọi trong FormMain_Load()
}
```
