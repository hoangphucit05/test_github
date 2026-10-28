# DATABASE.md — Thiết Kế Database

## Thông Tin Kết Nối

| Mục | Giá trị |
|-----|---------|
| **Tên database** | `BanhMiShopDB` |
| **SQL Server instance** | `localhost\SQLEXPRESS01` |
| **Xác thực** | Windows Authentication (Integrated Security) |
| **File script** | `BanhMiShopDB.sql` (trong thư mục gốc) |
| **Connection String** | `Server=localhost\SQLEXPRESS01;Database=BanhMiShopDB;Integrated Security=true;TrustServerCertificate=True;` |

---

## Bảng Dữ Liệu (Tables)

### 1. Roles — Vai trò người dùng
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| RoleID | INT PK | ID vai trò |
| RoleName | NVARCHAR(50) | Tên: ADMIN, STAFF |
| Description | NVARCHAR(200) | Mô tả quyền |

### 2. Employees — Nhân viên
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| EmployeeID | INT PK | ID nhân viên |
| EmployeeCode | NVARCHAR(20) | Mã (NV001, NV002...) |
| FullName | NVARCHAR(100) | Họ tên |
| Phone | NVARCHAR(20) | Số điện thoại |
| Email | NVARCHAR(100) | Email |
| HireDate | DATE | Ngày vào làm |
| IsActive | BIT | 1=đang làm, 0=nghỉ |

### 3. Users — Tài khoản đăng nhập
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| UserID | INT PK | ID user |
| Username | NVARCHAR(50) UNIQUE | Tên đăng nhập |
| PasswordHash | VARBINARY(64) | SHA-256 hash |
| EmployeeID | INT FK→Employees | Nhân viên tương ứng |
| RoleID | INT FK→Roles | Vai trò |
| IsActive | BIT | 1=active |

### 4. Categories — Danh mục món
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| CategoryID | INT PK | ID danh mục |
| CategoryName | NVARCHAR(100) | Tên: Bánh mì, Nước uống, Cà phê... |
| IsActive | BIT | Hiển thị hay không |

### 5. Products — Sản phẩm / Món ăn
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| ProductID | INT PK | ID sản phẩm |
| ProductCode | NVARCHAR(20) | Mã: SP001, SP002... |
| ProductName | NVARCHAR(200) | Tên món |
| CategoryID | INT FK→Categories | Danh mục |
| SellingPrice | DECIMAL(18,2) | Giá bán (VNĐ) |
| Unit | NVARCHAR(30) | Đơn vị: Ổ, Chai, Ly |
| IsActive | BIT | Đang bán hay không |

### 6. Ingredients — Nguyên liệu
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| IngredientID | INT PK | ID nguyên liệu |
| IngredientCode | NVARCHAR(20) | Mã: NL001... |
| IngredientName | NVARCHAR(200) | Tên nguyên liệu |
| Unit | NVARCHAR(30) | Đơn vị: kg, lít, cái |
| CurrentStock | DECIMAL(18,3) | Tồn kho hiện tại |
| MinimumStock | DECIMAL(18,3) | Mức tồn kho tối thiểu |
| UnitCost | DECIMAL(18,2) | Giá nhập (VNĐ) |

### 7. ProductRecipes — Công thức nguyên liệu
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| RecipeID | INT PK | ID công thức |
| ProductID | INT FK→Products | Sản phẩm |
| IngredientID | INT FK→Ingredients | Nguyên liệu |
| QuantityRequired | DECIMAL(18,3) | Lượng cần cho 1 đơn vị sản phẩm |

### 8. Orders — Đơn hàng
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| OrderID | INT PK | ID đơn hàng |
| OrderCode | NVARCHAR(20) | Mã đơn: DH-20260824-001 |
| OrderDate | DATETIME | Ngày giờ đặt |
| TotalAmount | DECIMAL(18,2) | Tổng tiền |
| DiscountAmount | DECIMAL(18,2) | Giảm giá |
| FinalAmount | DECIMAL(18,2) | Thực thu |
| PaymentMethod | NVARCHAR(30) | CASH / BANK_TRANSFER |
| Status | NVARCHAR(20) | COMPLETED / CANCELLED |
| UserID | INT FK→Users | Nhân viên tạo đơn |
| Notes | NVARCHAR(500) | Ghi chú |

### 9. OrderDetails — Chi tiết đơn hàng
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| OrderDetailID | INT PK | ID dòng chi tiết |
| OrderID | INT FK→Orders | Đơn hàng |
| ProductID | INT FK→Products | Sản phẩm |
| Quantity | DECIMAL(18,2) | Số lượng |
| UnitPrice | DECIMAL(18,2) | Đơn giá |
| TotalAmount | DECIMAL(18,2) | Thành tiền |

### 10. StockTransactions — Lịch sử kho
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| TransactionID | INT PK | ID giao dịch |
| IngredientID | INT FK→Ingredients | Nguyên liệu |
| TransactionType | NVARCHAR(20) | IN=nhập, OUT=xuất |
| Quantity | DECIMAL(18,3) | Số lượng |
| TransactionDate | DATETIME | Ngày giờ |
| Notes | NVARCHAR(500) | Lý do |

---

## Views

### v_DailyRevenue — Doanh thu theo ngày
```sql
Tính: ngày, số đơn, tổng doanh thu, tổng giảm giá, thực thu
Dùng trong: Module BÁO CÁO
```

### v_ProductSales — Sản phẩm bán chạy
```sql
Tính: sản phẩm, danh mục, tổng số lượng bán, tổng doanh thu
Dùng trong: Module BÁO CÁO (bảng xếp hạng)
```

### v_LowStock — Nguyên liệu sắp hết
```sql
Lọc: nguyên liệu có CurrentStock <= MinimumStock
Dùng trong: Module KHO NGUYÊN LIỆU (cảnh báo hàng thấp)
```

---

## Stored Procedures

### sp_DeductStockForOrder
```sql
-- Đầu vào: @OrderID INT
-- Tự động đọc OrderDetails của đơn hàng
-- Tính lượng nguyên liệu cần = Quantity × QuantityRequired (từ ProductRecipes)
-- Trừ CurrentStock trong Ingredients
-- Ghi lịch sử vào StockTransactions (TransactionType='OUT')
-- Gọi từ C#: sau khi INSERT Order + OrderDetails thành công
```

---

## Dữ Liệu Mẫu

Sau khi chạy `BanhMiShopDB.sql`, database sẽ có:
- 2 Roles: ADMIN, STAFF
- 2 Employees: Nguyễn Văn An (Admin), Trần Thị Bình (Staff)
- 2 Users: admin/123456, staff01/123456
- 4 Categories: Bánh mì, Nước uống, Cà phê, Trà
- 12 Products: SP001-SP012
- ~8 Ingredients: Bột mì, Thịt heo, Chả lụa, Pate...
- ProductRecipes: Công thức cho mỗi bánh mì
