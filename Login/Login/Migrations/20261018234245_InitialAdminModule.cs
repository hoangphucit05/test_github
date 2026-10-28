using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Login.Migrations
{
    /// <inheritdoc />
    public partial class InitialAdminModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. TẠO BẢNG CHI NHÁNH (Branches) NẾU CHƯA CÓ
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.Branches', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Branches
                    (
                        BranchID INT IDENTITY(1,1) NOT NULL,
                        BranchCode NVARCHAR(20) NOT NULL,
                        BranchName NVARCHAR(150) NOT NULL,
                        Address NVARCHAR(255) NULL,
                        Phone NVARCHAR(20) NULL,
                        IsActive BIT NOT NULL CONSTRAINT DF_Branches_IsActive DEFAULT 1,
                        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Branches_CreatedAt DEFAULT SYSDATETIME(),
                        CONSTRAINT PK_Branches PRIMARY KEY (BranchID),
                        CONSTRAINT UQ_Branches_BranchCode UNIQUE (BranchCode)
                    );
                END
            ");

            // 2. TẠO BẢNG QUYỀN (Permissions) NẾU CHƯA CÓ
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.Permissions', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Permissions
                    (
                        PermissionID INT IDENTITY(1,1) NOT NULL,
                        PermissionCode NVARCHAR(50) NOT NULL,
                        PermissionName NVARCHAR(100) NOT NULL,
                        GroupName NVARCHAR(50) NOT NULL,
                        Description NVARCHAR(255) NULL,
                        CONSTRAINT PK_Permissions PRIMARY KEY (PermissionID),
                        CONSTRAINT UQ_Permissions_PermissionCode UNIQUE (PermissionCode)
                    );
                END
            ");

            // 3. ĐẢM BẢO BẢNG VAI TRÒ (Roles) CÓ ĐỦ VAI TRÒ MANAGER
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Roles
                    (
                        RoleID INT IDENTITY(1,1) NOT NULL,
                        RoleName NVARCHAR(50) NOT NULL,
                        Description NVARCHAR(255) NULL,
                        CONSTRAINT PK_Roles PRIMARY KEY (RoleID),
                        CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName)
                    );
                END

                IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = 'MANAGER')
                BEGIN
                    INSERT INTO dbo.Roles (RoleName, Description) VALUES (N'MANAGER', N'Quản lý cửa hàng');
                END
            ");

            // 4. TẠO CÁC BẢNG CỐT LÕI KHI KHỞI TẠO DATABASE TRẮNG
            // Các bảng này đã có trong BanhMiShopDB.sql của phiên bản cũ; điều kiện
            // IF OBJECT_ID giữ nguyên dữ liệu của các database đã tồn tại.
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Employees
                    (
                        EmployeeID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Employees PRIMARY KEY,
                        EmployeeCode NVARCHAR(20) NOT NULL,
                        FullName NVARCHAR(100) NOT NULL,
                        Phone NVARCHAR(20) NULL,
                        Email NVARCHAR(100) NULL,
                        DateOfBirth DATE NULL,
                        Address NVARCHAR(255) NULL,
                        Position NVARCHAR(50) NOT NULL,
                        HireDate DATE NOT NULL CONSTRAINT DF_Employees_HireDate DEFAULT CAST(GETDATE() AS DATE),
                        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Employees_Status DEFAULT 'ACTIVE',
                        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Employees_CreatedAt DEFAULT SYSDATETIME(),
                        BranchID INT NULL,
                        CONSTRAINT UQ_Employees_EmployeeCode UNIQUE (EmployeeCode),
                        CONSTRAINT FK_Employees_Branches_BranchID FOREIGN KEY (BranchID)
                            REFERENCES dbo.Branches (BranchID)
                    );
                    CREATE INDEX IX_Employees_BranchID ON dbo.Employees (BranchID);
                END

                IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Users
                    (
                        UserID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
                        EmployeeID INT NOT NULL,
                        RoleID INT NOT NULL,
                        Username NVARCHAR(50) NOT NULL,
                        PasswordHash NVARCHAR(255) NOT NULL,
                        IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
                        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSDATETIME(),
                        LastLogin DATETIME2 NULL,
                        CONSTRAINT UQ_Users_EmployeeID UNIQUE (EmployeeID),
                        CONSTRAINT UQ_Users_Username UNIQUE (Username),
                        CONSTRAINT FK_Users_Employees_EmployeeID FOREIGN KEY (EmployeeID)
                            REFERENCES dbo.Employees (EmployeeID),
                        CONSTRAINT FK_Users_Roles_RoleID FOREIGN KEY (RoleID)
                            REFERENCES dbo.Roles (RoleID)
                    );
                    CREATE INDEX IX_Users_RoleID ON dbo.Users (RoleID);
                END

                IF OBJECT_ID(N'dbo.Shifts', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Shifts
                    (
                        ShiftID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Shifts PRIMARY KEY,
                        ShiftCode NVARCHAR(20) NOT NULL,
                        ShiftName NVARCHAR(100) NOT NULL,
                        StartTime TIME NOT NULL,
                        EndTime TIME NOT NULL,
                        IsActive BIT NOT NULL CONSTRAINT DF_Shifts_IsActive DEFAULT 1,
                        CONSTRAINT UQ_Shifts_ShiftCode UNIQUE (ShiftCode)
                    );
                END

                IF OBJECT_ID(N'dbo.EmployeeShifts', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.EmployeeShifts
                    (
                        EmployeeShiftID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_EmployeeShifts PRIMARY KEY,
                        EmployeeID INT NOT NULL,
                        ShiftID INT NOT NULL,
                        WorkDate DATE NOT NULL,
                        CheckIn DATETIME2 NULL,
                        CheckOut DATETIME2 NULL,
                        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_EmployeeShifts_Status DEFAULT 'SCHEDULED',
                        Note NVARCHAR(255) NULL,
                        CONSTRAINT UQ_EmployeeShifts_Employee_Shift_Date UNIQUE (EmployeeID, ShiftID, WorkDate),
                        CONSTRAINT FK_EmployeeShifts_Employees_EmployeeID FOREIGN KEY (EmployeeID)
                            REFERENCES dbo.Employees (EmployeeID),
                        CONSTRAINT FK_EmployeeShifts_Shifts_ShiftID FOREIGN KEY (ShiftID)
                            REFERENCES dbo.Shifts (ShiftID)
                    );
                END
            ");

            // 5. TẠO BẢNG PHÂN QUYỀN VAI TRÒ (RolePermissions) NẾU CHƯA CÓ
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.RolePermissions', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.RolePermissions
                    (
                        RoleID INT NOT NULL,
                        PermissionID INT NOT NULL,
                        CONSTRAINT PK_RolePermissions PRIMARY KEY (RoleID, PermissionID),
                        CONSTRAINT FK_RolePermissions_Roles_RoleID FOREIGN KEY (RoleID) REFERENCES dbo.Roles (RoleID) ON DELETE CASCADE,
                        CONSTRAINT FK_RolePermissions_Permissions_PermissionID FOREIGN KEY (PermissionID) REFERENCES dbo.Permissions (PermissionID) ON DELETE CASCADE
                    );
                END
            ");

            // 6. BỔ SUNG CỘT BranchID VÀO BẢNG NHÂN VIÊN (Employees) NẾU CHƯA CÓ
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.Employees', N'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Employees') AND name = 'BranchID')
                    BEGIN
                        ALTER TABLE dbo.Employees ADD BranchID INT NULL;
                    END

                    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Employees_Branches_BranchID')
                    BEGIN
                        ALTER TABLE dbo.Employees ADD CONSTRAINT FK_Employees_Branches_BranchID 
                            FOREIGN KEY (BranchID) REFERENCES dbo.Branches (BranchID);
                    END

                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Employees_BranchID' AND object_id = OBJECT_ID(N'dbo.Employees'))
                    BEGIN
                        CREATE NONCLUSTERED INDEX IX_Employees_BranchID ON dbo.Employees (BranchID);
                    END
                END
            ");

            // 7. CHUYỂN ĐỔI KIỂU PasswordHash TRONG Users TỪ VARBINARY SANG NVARCHAR(255) AN TOÀN
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
                BEGIN
                    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'PasswordHash' AND DATA_TYPE = 'varbinary')
                    BEGIN
                        ALTER TABLE dbo.Users ADD PasswordHash_New NVARCHAR(255) NULL;
                        EXEC('UPDATE dbo.Users SET PasswordHash_New = CONVERT(NVARCHAR(255), PasswordHash, 1)');
                        ALTER TABLE dbo.Users DROP COLUMN PasswordHash;
                        EXEC sp_rename 'dbo.Users.PasswordHash_New', 'PasswordHash', 'COLUMN';
                        ALTER TABLE dbo.Users ALTER COLUMN PasswordHash NVARCHAR(255) NOT NULL;
                    END
                END
            ");

            // 8. ĐẢM BẢO CÓ ĐỦ 3 CA LÀM (SÁNG, CHIỀU, TỐI) TRONG BẢNG Shifts
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.Shifts', N'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM dbo.Shifts WHERE ShiftCode = 'CA03')
                    BEGIN
                        INSERT INTO dbo.Shifts (ShiftCode, ShiftName, StartTime, EndTime, IsActive)
                        VALUES ('CA03', N'Ca Tối', '22:00', '06:00', 1);
                    END
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Chỉ rollback các đối tượng thuộc module Admin mới tạo, không xóa bảng nghiệp vụ của nhóm
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.RolePermissions', N'U') IS NOT NULL
                    DROP TABLE dbo.RolePermissions;

                IF OBJECT_ID(N'dbo.Permissions', N'U') IS NOT NULL
                    DROP TABLE dbo.Permissions;

                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Employees_Branches_BranchID')
                    ALTER TABLE dbo.Employees DROP CONSTRAINT FK_Employees_Branches_BranchID;

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Employees_BranchID' AND object_id = OBJECT_ID(N'dbo.Employees'))
                    DROP INDEX IX_Employees_BranchID ON dbo.Employees;

                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Employees') AND name = 'BranchID')
                    ALTER TABLE dbo.Employees DROP COLUMN BranchID;

                IF OBJECT_ID(N'dbo.Branches', N'U') IS NOT NULL
                    DROP TABLE dbo.Branches;
            ");
        }
    }
}
