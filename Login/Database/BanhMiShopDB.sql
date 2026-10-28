/* =========================================================
   PROJECT: PHẦN MỀM QUẢN LÝ & BÁN HÀNG QUÁN BÁNH MÌ
   DATABASE: BanhMiShopDB
   SQL SERVER
   ========================================================= */

USE master;
GO

IF DB_ID(N'BanhMiShopDB') IS NULL
BEGIN
    CREATE DATABASE BanhMiShopDB;
END
GO

USE BanhMiShopDB;
GO

IF OBJECT_ID(N'dbo.v_DailyRevenue', N'V') IS NOT NULL
    DROP VIEW dbo.v_DailyRevenue;
GO

IF OBJECT_ID(N'dbo.v_ProductSales', N'V') IS NOT NULL
    DROP VIEW dbo.v_ProductSales;
GO

IF OBJECT_ID(N'dbo.v_LowStock', N'V') IS NOT NULL
    DROP VIEW dbo.v_LowStock;
GO

IF OBJECT_ID(N'dbo.v_OrderSummary', N'V') IS NOT NULL
    DROP VIEW dbo.v_OrderSummary;
GO

IF OBJECT_ID(N'dbo.sp_GetDailyRevenue', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetDailyRevenue;
GO

IF OBJECT_ID(N'dbo.sp_DeductStockForOrder', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DeductStockForOrder;
GO

DROP TABLE IF EXISTS dbo.StockTransactions;
DROP TABLE IF EXISTS dbo.StockReceiptDetails;
DROP TABLE IF EXISTS dbo.StockReceipts;
DROP TABLE IF EXISTS dbo.Suppliers;

DROP TABLE IF EXISTS dbo.EmployeeShifts;
DROP TABLE IF EXISTS dbo.Shifts;

DROP TABLE IF EXISTS dbo.PromotionProducts;
DROP TABLE IF EXISTS dbo.Promotions;

DROP TABLE IF EXISTS dbo.Payments;
DROP TABLE IF EXISTS dbo.OrderDetails;
DROP TABLE IF EXISTS dbo.Orders;
DROP TABLE IF EXISTS dbo.Customers;

DROP TABLE IF EXISTS dbo.RecipeDetails;
DROP TABLE IF EXISTS dbo.Recipes;
DROP TABLE IF EXISTS dbo.Ingredients;

DROP TABLE IF EXISTS dbo.Products;
DROP TABLE IF EXISTS dbo.Categories;

DROP TABLE IF EXISTS dbo.Users;
DROP TABLE IF EXISTS dbo.Employees;
DROP TABLE IF EXISTS dbo.Roles;
GO

CREATE TABLE dbo.Roles
(
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255) NULL,
    CONSTRAINT UQ_Roles_RoleName UNIQUE(RoleName)
);
GO

CREATE TABLE dbo.Employees
(
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode VARCHAR(20) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    DateOfBirth DATE NULL,
    Address NVARCHAR(255) NULL,
    Position NVARCHAR(50) NOT NULL,
    HireDate DATE NOT NULL CONSTRAINT DF_Employees_HireDate DEFAULT CAST(GETDATE() AS DATE),
    Status VARCHAR(20) NOT NULL CONSTRAINT DF_Employees_Status DEFAULT 'ACTIVE',
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Employees_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT UQ_Employees_EmployeeCode UNIQUE(EmployeeCode),
    CONSTRAINT CK_Employees_Status CHECK(Status IN ('ACTIVE', 'INACTIVE', 'RESIGNED'))
);
GO

CREATE TABLE dbo.Users
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT NOT NULL,
    RoleID INT NOT NULL,
    Username VARCHAR(50) NOT NULL,
    -- Lưu chuỗi BCrypt, tuyệt đối không lưu mật khẩu dạng rõ.
    PasswordHash NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSDATETIME(),
    LastLogin DATETIME2 NULL,
    FailedLoginCount INT NOT NULL CONSTRAINT DF_Users_FailedLoginCount DEFAULT 0,
    LockoutEnd DATETIME2 NULL,
    CONSTRAINT UQ_Users_EmployeeID UNIQUE(EmployeeID),
    CONSTRAINT UQ_Users_Username UNIQUE(Username),
    CONSTRAINT FK_Users_Employee FOREIGN KEY(EmployeeID) REFERENCES dbo.Employees(EmployeeID),
    CONSTRAINT FK_Users_Role FOREIGN KEY(RoleID) REFERENCES dbo.Roles(RoleID)
);
GO

CREATE TABLE dbo.Categories
(
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT 1,
    CONSTRAINT UQ_Categories_CategoryName UNIQUE(CategoryName)
);
GO

CREATE TABLE dbo.Products
(
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode VARCHAR(20) NOT NULL,
    ProductName NVARCHAR(150) NOT NULL,
    CategoryID INT NOT NULL,
    SellingPrice DECIMAL(18,2) NOT NULL,
    Unit NVARCHAR(30) NOT NULL,
    ImagePath NVARCHAR(500) NULL,
    Description NVARCHAR(500) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_Products_ProductCode UNIQUE(ProductCode),
    CONSTRAINT CK_Products_SellingPrice CHECK(SellingPrice >= 0),
    CONSTRAINT FK_Products_Category FOREIGN KEY(CategoryID) REFERENCES dbo.Categories(CategoryID)
);
GO

CREATE TABLE dbo.Ingredients
(
    IngredientID INT IDENTITY(1,1) PRIMARY KEY,
    IngredientCode VARCHAR(20) NOT NULL,
    IngredientName NVARCHAR(150) NOT NULL,
    Unit NVARCHAR(30) NOT NULL,
    MinimumStock DECIMAL(18,3) NOT NULL CONSTRAINT DF_Ingredients_MinimumStock DEFAULT 0,
    CurrentStock DECIMAL(18,3) NOT NULL CONSTRAINT DF_Ingredients_CurrentStock DEFAULT 0,
    CostPrice DECIMAL(18,2) NOT NULL CONSTRAINT DF_Ingredients_CostPrice DEFAULT 0,
    IsActive BIT NOT NULL CONSTRAINT DF_Ingredients_IsActive DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Ingredients_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT UQ_Ingredients_Code UNIQUE(IngredientCode),
    CONSTRAINT CK_Ingredients_Stock CHECK(CurrentStock >= 0),
    CONSTRAINT CK_Ingredients_MinimumStock CHECK(MinimumStock >= 0),
    CONSTRAINT CK_Ingredients_CostPrice CHECK(CostPrice >= 0)
);
GO

CREATE TABLE dbo.Recipes
(
    RecipeID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Recipes_CreatedAt DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT UQ_Recipes_Product UNIQUE(ProductID),
    CONSTRAINT FK_Recipes_Product FOREIGN KEY(ProductID) REFERENCES dbo.Products(ProductID)
);
GO

CREATE TABLE dbo.RecipeDetails
(
    RecipeDetailID INT IDENTITY(1,1) PRIMARY KEY,
    RecipeID INT NOT NULL,
    IngredientID INT NOT NULL,
    Quantity DECIMAL(18,3) NOT NULL,
    CONSTRAINT UQ_RecipeDetails UNIQUE(RecipeID, IngredientID),
    CONSTRAINT CK_RecipeDetails_Quantity CHECK(Quantity > 0),
    CONSTRAINT FK_RecipeDetails_Recipe FOREIGN KEY(RecipeID) REFERENCES dbo.Recipes(RecipeID),
    CONSTRAINT FK_RecipeDetails_Ingredient FOREIGN KEY(IngredientID) REFERENCES dbo.Ingredients(IngredientID)
);
GO

CREATE TABLE dbo.Customers
(
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerCode VARCHAR(20) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NULL,
    Address NVARCHAR(255) NULL,
    Email VARCHAR(100) NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT SYSDATETIME(),
    IsActive BIT NOT NULL CONSTRAINT DF_Customers_IsActive DEFAULT 1,
    CONSTRAINT UQ_Customers_Code UNIQUE(CustomerCode)
);
GO

CREATE TABLE dbo.Orders
(
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderCode VARCHAR(30) NOT NULL,
    CustomerID INT NULL,
    EmployeeID INT NOT NULL,
    OrderDate DATETIME2 NOT NULL CONSTRAINT DF_Orders_OrderDate DEFAULT SYSDATETIME(),
    SubTotal DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_SubTotal DEFAULT 0,
    DiscountAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_DiscountAmount DEFAULT 0,
    TotalAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_Orders_TotalAmount DEFAULT 0,
    PaymentStatus VARCHAR(20) NOT NULL CONSTRAINT DF_Orders_PaymentStatus DEFAULT 'UNPAID',
    OrderStatus VARCHAR(20) NOT NULL CONSTRAINT DF_Orders_OrderStatus DEFAULT 'PENDING',
    Note NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT UQ_Orders_OrderCode UNIQUE(OrderCode),
    CONSTRAINT CK_Orders_Amount CHECK(SubTotal >= 0 AND DiscountAmount >= 0 AND TotalAmount >= 0),
    CONSTRAINT CK_Orders_PaymentStatus CHECK(PaymentStatus IN ('UNPAID', 'PAID', 'REFUNDED')),
    CONSTRAINT CK_Orders_OrderStatus CHECK(OrderStatus IN ('PENDING', 'CONFIRMED', 'COMPLETED', 'CANCELLED')),
    CONSTRAINT FK_Orders_Customer FOREIGN KEY(CustomerID) REFERENCES dbo.Customers(CustomerID),
    CONSTRAINT FK_Orders_Employee FOREIGN KEY(EmployeeID) REFERENCES dbo.Employees(EmployeeID)
);
GO

CREATE TABLE dbo.OrderDetails
(
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity DECIMAL(18,3) NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_OrderDetails_Discount DEFAULT 0,
    TotalAmount AS ((Quantity * UnitPrice) - DiscountAmount) PERSISTED,
    CONSTRAINT CK_OrderDetails_Quantity CHECK(Quantity > 0),
    CONSTRAINT CK_OrderDetails_Price CHECK(UnitPrice >= 0),
    CONSTRAINT CK_OrderDetails_Discount CHECK(DiscountAmount >= 0),
    CONSTRAINT FK_OrderDetails_Order FOREIGN KEY(OrderID) REFERENCES dbo.Orders(OrderID),
    CONSTRAINT FK_OrderDetails_Product FOREIGN KEY(ProductID) REFERENCES dbo.Products(ProductID)
);
GO

CREATE TABLE dbo.Payments
(
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    PaymentMethod VARCHAR(30) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PaidAt DATETIME2 NOT NULL CONSTRAINT DF_Payments_PaidAt DEFAULT SYSDATETIME(),
    PaymentStatus VARCHAR(20) NOT NULL CONSTRAINT DF_Payments_Status DEFAULT 'SUCCESS',
    ReferenceCode VARCHAR(100) NULL,
    CONSTRAINT UQ_Payments_Order UNIQUE(OrderID),
    CONSTRAINT CK_Payments_Amount CHECK(Amount >= 0),
    CONSTRAINT CK_Payments_Method CHECK(PaymentMethod IN ('CASH', 'BANK_TRANSFER')),
    CONSTRAINT CK_Payments_Status CHECK(PaymentStatus IN ('SUCCESS', 'FAILED', 'REFUNDED')),
    CONSTRAINT FK_Payments_Order FOREIGN KEY(OrderID) REFERENCES dbo.Orders(OrderID)
);
GO

CREATE TABLE dbo.Promotions
(
    PromotionID INT IDENTITY(1,1) PRIMARY KEY,
    PromotionCode VARCHAR(30) NOT NULL,
    PromotionName NVARCHAR(150) NOT NULL,
    PromotionType VARCHAR(30) NOT NULL,
    DiscountValue DECIMAL(18,2) NOT NULL,
    MinimumOrderAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_Promotions_MinOrder DEFAULT 0,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Promotions_IsActive DEFAULT 1,
    Description NVARCHAR(500) NULL,
    CONSTRAINT UQ_Promotions_Code UNIQUE(PromotionCode),
    CONSTRAINT CK_Promotions_Type CHECK(PromotionType IN ('PERCENT', 'AMOUNT', 'BUY_QUANTITY')),
    CONSTRAINT CK_Promotions_Discount CHECK(DiscountValue >= 0),
    CONSTRAINT CK_Promotions_Date CHECK(EndDate >= StartDate),
    CONSTRAINT CK_Promotions_MinOrder CHECK(MinimumOrderAmount >= 0)
);
GO

CREATE TABLE dbo.PromotionProducts
(
    PromotionProductID INT IDENTITY(1,1) PRIMARY KEY,
    PromotionID INT NOT NULL,
    ProductID INT NOT NULL,
    MinimumQuantity DECIMAL(18,3) NOT NULL CONSTRAINT DF_PromotionProducts_MinQty DEFAULT 1,
    CONSTRAINT UQ_PromotionProducts UNIQUE(PromotionID, ProductID),
    CONSTRAINT CK_PromotionProducts_MinQty CHECK(MinimumQuantity > 0),
    CONSTRAINT FK_PromotionProducts_Promotion FOREIGN KEY(PromotionID) REFERENCES dbo.Promotions(PromotionID),
    CONSTRAINT FK_PromotionProducts_Product FOREIGN KEY(ProductID) REFERENCES dbo.Products(ProductID)
);
GO

CREATE TABLE dbo.Shifts
(
    ShiftID INT IDENTITY(1,1) PRIMARY KEY,
    ShiftCode VARCHAR(20) NOT NULL,
    ShiftName NVARCHAR(100) NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Shifts_IsActive DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Shifts_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT UQ_Shifts_Code UNIQUE(ShiftCode)
);
GO

CREATE TABLE dbo.EmployeeShifts
(
    EmployeeShiftID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT NOT NULL,
    ShiftID INT NOT NULL,
    WorkDate DATE NOT NULL,
    CheckIn DATETIME2 NULL,
    CheckOut DATETIME2 NULL,
    Status VARCHAR(20) NOT NULL CONSTRAINT DF_EmployeeShifts_Status DEFAULT 'SCHEDULED',
    Note NVARCHAR(255) NULL,
    CONSTRAINT UQ_EmployeeShifts UNIQUE(EmployeeID, ShiftID, WorkDate),
    CONSTRAINT CK_EmployeeShifts_Status CHECK(Status IN ('SCHEDULED', 'WORKING', 'COMPLETED', 'ABSENT')),
    CONSTRAINT FK_EmployeeShifts_Employee FOREIGN KEY(EmployeeID) REFERENCES dbo.Employees(EmployeeID),
    CONSTRAINT FK_EmployeeShifts_Shift FOREIGN KEY(ShiftID) REFERENCES dbo.Shifts(ShiftID)
);
GO

CREATE TABLE dbo.Suppliers
(
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierCode VARCHAR(20) NOT NULL,
    SupplierName NVARCHAR(150) NOT NULL,
    Phone VARCHAR(20) NULL,
    Address NVARCHAR(255) NULL,
    Email VARCHAR(100) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Suppliers_IsActive DEFAULT 1,
    CONSTRAINT UQ_Suppliers_Code UNIQUE(SupplierCode)
);
GO

CREATE TABLE dbo.StockReceipts
(
    ReceiptID INT IDENTITY(1,1) PRIMARY KEY,
    ReceiptCode VARCHAR(30) NOT NULL,
    SupplierID INT NOT NULL,
    EmployeeID INT NOT NULL,
    ReceiptDate DATETIME2 NOT NULL CONSTRAINT DF_StockReceipts_Date DEFAULT SYSDATETIME(),
    TotalAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_StockReceipts_Total DEFAULT 0,
    Note NVARCHAR(500) NULL,
    CONSTRAINT UQ_StockReceipts_Code UNIQUE(ReceiptCode),
    CONSTRAINT CK_StockReceipts_Total CHECK(TotalAmount >= 0),
    CONSTRAINT FK_StockReceipts_Supplier FOREIGN KEY(SupplierID) REFERENCES dbo.Suppliers(SupplierID),
    CONSTRAINT FK_StockReceipts_Employee FOREIGN KEY(EmployeeID) REFERENCES dbo.Employees(EmployeeID)
);
GO

CREATE TABLE dbo.StockReceiptDetails
(
    ReceiptDetailID INT IDENTITY(1,1) PRIMARY KEY,
    ReceiptID INT NOT NULL,
    IngredientID INT NOT NULL,
    Quantity DECIMAL(18,3) NOT NULL,
    UnitCost DECIMAL(18,2) NOT NULL,
    TotalCost AS (Quantity * UnitCost) PERSISTED,
    CONSTRAINT CK_StockReceiptDetails_Quantity CHECK(Quantity > 0),
    CONSTRAINT CK_StockReceiptDetails_UnitCost CHECK(UnitCost >= 0),
    CONSTRAINT FK_StockReceiptDetails_Receipt FOREIGN KEY(ReceiptID) REFERENCES dbo.StockReceipts(ReceiptID),
    CONSTRAINT FK_StockReceiptDetails_Ingredient FOREIGN KEY(IngredientID) REFERENCES dbo.Ingredients(IngredientID)
);
GO

CREATE TABLE dbo.StockTransactions
(
    TransactionID BIGINT IDENTITY(1,1) PRIMARY KEY,
    IngredientID INT NOT NULL,
    TransactionType VARCHAR(20) NOT NULL,
    Quantity DECIMAL(18,3) NOT NULL,
    ReferenceType VARCHAR(30) NULL,
    ReferenceID INT NULL,
    EmployeeID INT NULL,
    TransactionDate DATETIME2 NOT NULL CONSTRAINT DF_StockTransactions_Date DEFAULT SYSDATETIME(),
    Note NVARCHAR(500) NULL,
    CONSTRAINT CK_StockTransactions_Type CHECK(TransactionType IN ('IN', 'OUT', 'ADJUSTMENT')),
    CONSTRAINT CK_StockTransactions_Quantity CHECK(Quantity > 0),
    CONSTRAINT FK_StockTransactions_Ingredient FOREIGN KEY(IngredientID) REFERENCES dbo.Ingredients(IngredientID),
    CONSTRAINT FK_StockTransactions_Employee FOREIGN KEY(EmployeeID) REFERENCES dbo.Employees(EmployeeID)
);
GO

CREATE INDEX IX_Products_CategoryID ON dbo.Products(CategoryID);
CREATE INDEX IX_Products_ProductName ON dbo.Products(ProductName);
CREATE INDEX IX_Orders_OrderDate ON dbo.Orders(OrderDate);
CREATE INDEX IX_Orders_EmployeeID ON dbo.Orders(EmployeeID);
CREATE INDEX IX_Orders_CustomerID ON dbo.Orders(CustomerID);
CREATE INDEX IX_Orders_Status ON dbo.Orders(OrderStatus);
CREATE INDEX IX_OrderDetails_OrderID ON dbo.OrderDetails(OrderID);
CREATE INDEX IX_OrderDetails_ProductID ON dbo.OrderDetails(ProductID);
CREATE INDEX IX_Ingredients_Name ON dbo.Ingredients(IngredientName);
CREATE INDEX IX_StockTransactions_IngredientID ON dbo.StockTransactions(IngredientID);
CREATE INDEX IX_StockTransactions_Date ON dbo.StockTransactions(TransactionDate);
CREATE INDEX IX_EmployeeShifts_WorkDate ON dbo.EmployeeShifts(WorkDate);
CREATE INDEX IX_EmployeeShifts_EmployeeID ON dbo.EmployeeShifts(EmployeeID);
CREATE INDEX IX_Promotions_Date ON dbo.Promotions(StartDate, EndDate);
GO

INSERT INTO dbo.Roles (RoleName, Description)
VALUES
(N'ADMIN', N'Quản trị hệ thống'),
(N'STAFF', N'Nhân viên bán hàng');
GO

INSERT INTO dbo.Employees (EmployeeCode, FullName, Phone, Email, DateOfBirth, Address, Position, HireDate, Status)
VALUES
('NV001', N'Nguyễn Văn An', '0901000001', 'an@banhmi.vn', '2000-01-15', N'TP. Hồ Chí Minh', N'Quản lý', '2025-01-01', 'ACTIVE'),
('NV002', N'Trần Thị Bình', '0901000002', 'binh@banhmi.vn', '2002-03-20', N'TP. Hồ Chí Minh', N'Nhân viên', '2025-02-01', 'ACTIVE'),
('NV003', N'Lê Văn Cường', '0901000003', 'cuong@banhmi.vn', '2001-06-10', N'TP. Hồ Chí Minh', N'Nhân viên', '2025-03-01', 'ACTIVE'),
('NV004', N'Phạm Thị Dung', '0901000004', 'dung@banhmi.vn', '2003-08-12', N'TP. Hồ Chí Minh', N'Nhân viên', '2025-04-01', 'ACTIVE');
GO

-- Tài khoản mẫu được tạo bởi DatabaseSeeder để mật khẩu được băm bằng BCrypt.

INSERT INTO dbo.Categories (CategoryName, Description, IsActive)
VALUES
(N'Bánh mì', N'Các loại bánh mì truyền thống và hiện đại', 1),
(N'Nước uống', N'Nước giải khát đóng chai', 1),
(N'Cà phê', N'Cà phê pha máy và pha phin', 1),
(N'Trà', N'Các loại trà trái cây', 1);
GO

INSERT INTO dbo.Products (ProductCode, ProductName, CategoryID, SellingPrice, Unit, IsActive)
VALUES
('SP001', N'Bánh mì thịt', 1, 20000, N'Ổ', 1),
('SP002', N'Bánh mì trứng', 1, 15000, N'Ổ', 1),
('SP003', N'Bánh mì chả lụa', 1, 18000, N'Ổ', 1),
('SP004', N'Bánh mì xíu mại', 1, 20000, N'Ổ', 1),
('SP005', N'Bánh mì pate chả lụa', 1, 22000, N'Ổ', 1),
('SP006', N'Bánh mì đặc biệt', 1, 25000, N'Ổ', 1),
('SP007', N'Coca Cola', 2, 15000, N'Chai', 1),
('SP008', N'Pepsi', 2, 15000, N'Chai', 1),
('SP009', N'Nước suối Aquafina', 2, 10000, N'Chai', 1),
('SP010', N'Cà phê đá', 3, 18000, N'Ly', 1),
('SP011', N'Cà phê sữa đá', 3, 20000, N'Ly', 1),
('SP012', N'Trà đào cam sả', 4, 30000, N'Ly', 1);
GO

INSERT INTO dbo.Ingredients (IngredientCode, IngredientName, Unit, MinimumStock, CurrentStock, CostPrice, IsActive)
VALUES
('NL001', N'Ổ bánh mì', N'Ổ', 50, 100, 3000, 1),
('NL002', N'Thịt xá xíu', N'Kg', 2, 5, 150000, 1),
('NL003', N'Pate', N'Kg', 1, 3, 120000, 1),
('NL004', N'Trứng gà', N'Quả', 30, 60, 3000, 1),
('NL005', N'Chả lụa', N'Kg', 2, 4, 180000, 1),
('NL006', N'Xíu mại', N'Kg', 2, 4, 140000, 1),
('NL007', N'Rau ngò', N'Kg', 0.5, 1, 20000, 1),
('NL008', N'Dưa leo', N'Kg', 1, 3, 15000, 1),
('NL009', N'Coca Cola (Chai)', N'Chai', 24, 48, 8000, 1),
('NL010', N'Cà phê bột', N'Kg', 1, 5, 200000, 1),
('NL011', N'Sữa đặc', N'Hộp', 5, 15, 22000, 1);
GO

INSERT INTO dbo.Recipes (ProductID, Description)
VALUES
(1, N'Công thức bánh mì thịt truyền thống'),
(2, N'Công thức bánh mì trứng ốp la'),
(6, N'Công thức bánh mì đặc biệt thập cẩm');
GO

INSERT INTO dbo.RecipeDetails (RecipeID, IngredientID, Quantity)
VALUES
(1, 1, 1),
(1, 2, 0.08),
(1, 3, 0.02),
(1, 7, 0.01),
(1, 8, 0.01),
(2, 1, 1),
(2, 4, 1),
(2, 7, 0.01),
(2, 8, 0.01),
(3, 1, 1),
(3, 2, 0.05),
(3, 5, 0.05),
(3, 4, 1),
(3, 3, 0.02),
(3, 7, 0.01),
(3, 8, 0.01);
GO

INSERT INTO dbo.Shifts (ShiftCode, ShiftName, StartTime, EndTime, IsActive)
VALUES
('CA01', N'Ca Sáng', '06:00', '14:00', 1),
('CA02', N'Ca Chiều', '14:00', '22:00', 1);
GO

INSERT INTO dbo.Suppliers (SupplierCode, SupplierName, Phone, Address, Email)
VALUES
('NCC001', N'Lò bánh mì ABC', '0909123456', N'Quận 1, TP.HCM', 'contact@abc.vn'),
('NCC002', N'Công ty TNHH Nước Giải Khát', '0909654321', N'Quận 3, TP.HCM', 'sales@nuoc.vn');
GO

CREATE VIEW dbo.v_DailyRevenue
AS
SELECT 
    CAST(OrderDate AS DATE) AS SaleDate,
    COUNT(OrderID) AS TotalOrders,
    SUM(TotalAmount) AS TotalRevenue
FROM dbo.Orders
WHERE OrderStatus = 'COMPLETED'
GROUP BY CAST(OrderDate AS DATE);
GO

CREATE VIEW dbo.v_ProductSales
AS
SELECT 
    p.ProductID,
    p.ProductCode,
    p.ProductName,
    SUM(od.Quantity) AS TotalQuantitySold,
    SUM(od.TotalAmount) AS TotalRevenue
FROM dbo.OrderDetails od
JOIN dbo.Orders o ON od.OrderID = o.OrderID
JOIN dbo.Products p ON od.ProductID = p.ProductID
WHERE o.OrderStatus = 'COMPLETED'
GROUP BY p.ProductID, p.ProductCode, p.ProductName;
GO

CREATE VIEW dbo.v_LowStock
AS
SELECT 
    IngredientCode,
    IngredientName,
    Unit,
    CurrentStock,
    MinimumStock
FROM dbo.Ingredients
WHERE CurrentStock <= MinimumStock AND IsActive = 1;
GO

CREATE PROCEDURE dbo.sp_DeductStockForOrder
    @OrderID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS(SELECT 1 FROM dbo.Orders WHERE OrderID = @OrderID AND OrderStatus = 'COMPLETED')
    BEGIN
        PRINT 'Order is not valid or not completed.';
        RETURN;
    END

    DECLARE @ProductID INT, @Qty DECIMAL(18,3);
    
    DECLARE curOrderDetails CURSOR FOR 
    SELECT ProductID, Quantity FROM dbo.OrderDetails WHERE OrderID = @OrderID;
    
    OPEN curOrderDetails;
    FETCH NEXT FROM curOrderDetails INTO @ProductID, @Qty;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Recipes WHERE ProductID = @ProductID)
        BEGIN
            UPDATE i
            SET i.CurrentStock = i.CurrentStock - (rd.Quantity * @Qty)
            FROM dbo.Ingredients i
            JOIN dbo.RecipeDetails rd ON i.IngredientID = rd.IngredientID
            JOIN dbo.Recipes r ON rd.RecipeID = r.RecipeID
            WHERE r.ProductID = @ProductID;
            
            INSERT INTO dbo.StockTransactions (IngredientID, TransactionType, Quantity, ReferenceType, ReferenceID, Note)
            SELECT 
                rd.IngredientID,
                'OUT',
                rd.Quantity * @Qty,
                'ORDER',
                @OrderID,
                N'Bán hàng: Order ' + CAST(@OrderID AS NVARCHAR)
            FROM dbo.RecipeDetails rd
            JOIN dbo.Recipes r ON rd.RecipeID = r.RecipeID
            WHERE r.ProductID = @ProductID;
        END
        
        FETCH NEXT FROM curOrderDetails INTO @ProductID, @Qty;
    END
    
    CLOSE curOrderDetails;
    DEALLOCATE curOrderDetails;
END
GO
