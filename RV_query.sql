create database RauVietDB

USE RauVietDB

CREATE TABLE Customers
(
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(50) NOT NULL,
    CustomerCode NVARCHAR(100) NULL
);

-- Tạo lại bảng ProductSKU
CREATE TABLE ProductSKU (
    SKU INT IDENTITY(1,1) PRIMARY KEY,
    ProductNameVN NVARCHAR(200) NOT NULL,
    ProductNameEN NVARCHAR(200) NOT NULL,
    PackingType NVARCHAR(20) NULL,            
    Package NVARCHAR(10) NOT NULL,
    PackingList NVARCHAR(50) NULL,
    BotanicalName NVARCHAR(200) NULL,
    PlantingAreaCode NVARCHAR(100) NULL,
    PriceCNF DECIMAL(18,2) NOT NULL,
    LOTCodeHeader NVARCHAR(10) NULL,
    Priority INT NOT NULL DEFAULT 1
);

CREATE TABLE ProductPacking (
    ProductPackingID INT IDENTITY(1,1) PRIMARY KEY, 
    SKU INT NOT NULL,    
    BarCode NVARCHAR(30) NOT NULL,       
    PLU NVARCHAR(20) NULL, 
    Amount DECIMAL NOT NULL,   
    packing  NVARCHAR(20) NOT NULL, 
    BarCodeEAN13 NVARCHAR(30) NOT NULL,
    ArtNr NVARCHAR(30) NOT NULL,
    GGN NVARCHAR(30) NOT NULL
    FOREIGN KEY (SKU) REFERENCES ProductSKU(SKU),
);

CREATE TABLE ExportCodes (
    ExportCodeID INT IDENTITY(1,1) PRIMARY KEY,
    ExportCode NVARCHAR(50) NOT NULL UNIQUE, -- mã xuất cảng duy nhất
    ExportCodeIndex int NOT NULL,
    ExportDate DATETIME DEFAULT GETDATE(),
    ModifiedAt DATETIME DEFAULT GETDATE(),
    ExchangeRate DECIMAL(18,2) NOT NULL DEFAULT 0,
    ShippingCost DECIMAL(18,2) NOT NULL DEFAULT 0,
    Complete BIT NOT NULL DEFAULT 0,
    InputBy INT NULL,
    PackingBy INT NULL
);

CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY, 
    ExportCodeID INT NOT NULL,
    CustomerID INT NOT NULL,    
    ProductPackingID INT NOT NULL,
    OrderPackingPriceCNF DECIMAL(18,2) NOT NULL,
    PCSOther INT DEFAULT 0 NULL,
    NWOther DECIMAL(18,2) NULL,
    PCSReal INT NULL,
    NWReal DECIMAL(18,2) NULL,
    CartonNo INT NULL,
    CartonSize NVARCHAR(50) NULL,
    CustomerCarton NVARCHAR(50) NULL,
    LOTCode NVARCHAR(15) NULL,
    LOTCodeComplete NVARCHAR(30)

    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    FOREIGN KEY (ProductPackingID) REFERENCES ProductPacking(ProductPackingID),
    FOREIGN KEY (ExportCodeID) REFERENCES ExportCodes(ExportCodeID),
);

CREATE TABLE ContractType (
    ContractTypeID INT IDENTITY(1,1) PRIMARY KEY,   -- Khóa chính, tự tăng
    ContractTypeName NVARCHAR(100) NOT NULL,        -- Tên loại hợp đồng (VD: Toàn thời gian, Thời vụ, Thực tập,...)
    Description NVARCHAR(255) NULL                  -- Mô tả thêm (nếu cần)
);

CREATE TABLE Position (
    PositionID INT IDENTITY(1,1) PRIMARY KEY,    -- Mã chức vụ tự tăng
    PositionName NVARCHAR(100) NOT NULL UNIQUE,  -- Tên chức vụ (VD: Giám đốc, Nhân viên)
    Description NVARCHAR(255) NULL,              -- Mô tả thêm nếu cần
    IsActive BIT DEFAULT 1,                      -- Còn sử dụng hay không
    CreatedAt DATETIME DEFAULT GETDATE()         -- Ngày tạo
);

CREATE TABLE Department (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,   -- Mã phòng ban tự tăng
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE, -- Tên phòng ban (VD: Hành chính, Kỹ thuật...)
    Description NVARCHAR(255) NULL,               -- Mô tả (tùy chọn)
    IsActive BIT DEFAULT 1,                       -- Trạng thái hoạt động (1 = còn, 0 = ngưng)
    CreatedAt DATETIME DEFAULT GETDATE()          -- Ngày tạo bản ghi
);
select * from Employee
CREATE TABLE Employee (
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,               -- Mã nhân viên tự tăng
    EmployeeCode NVARCHAR(20) NOT NULL UNIQUE,              -- Mã nhân viên (VD: NV001)
    FullName NVARCHAR(100) NOT NULL,                        -- Họ và tên
    BirthDate DATE NULL,                                    -- Ngày sinh
    HireDate DATE NULL,                                     -- Ngày vào làm
    Gender BIT NULL,                                        -- 1 = Nam, 0 = Nữ

    -- 🏠 Thông tin cá nhân
    Hometown NVARCHAR(150) NULL,                            -- Quê quán
    Address NVARCHAR(200) NULL,                             -- Địa chỉ hiện tại
   
    CitizenID NVARCHAR(20) NULL,                            -- Số CCCD/CMND
    IssueDate DATE NULL,                                    -- Ngày cấp CCCD
    IssuePlace NVARCHAR(100) NULL,                          -- Nơi cấp CCCD

    -- 💼 Thông tin công việc
    PositionID INT NULL,                                    -- Chức vụ (FK)
    DepartmentID INT NULL,                                  -- Phòng ban (FK)
    ContractTypeID INT NULL,                                -- Loại hợp đồng (FK)

    -- ⚙️ Trạng thái
    IsActive BIT DEFAULT 1,                                 -- Đang làm việc (1 = còn làm, 0 = nghỉ)
    canCreateUserName BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),                   -- Ngày tạo bản ghi

    -- 🔗 Khóa ngoại
    CONSTRAINT FK_Employee_Position FOREIGN KEY (PositionID)
        REFERENCES Position(PositionID),

    CONSTRAINT FK_Employee_Department FOREIGN KEY (DepartmentID)
        REFERENCES Department(DepartmentID),

    CONSTRAINT FK_Employee_ContractType FOREIGN KEY (ContractTypeID)
        REFERENCES ContractType(ContractTypeID)
);

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,    -- Tên đăng nhập
    PasswordHash NVARCHAR(255) NOT NULL,      -- Mật khẩu đã mã hóa (hash)
    EmployeeID INT NULL,                      -- Nếu có liên kết đến nhân viên (FK)
    IsActive BIT NOT NULL DEFAULT 1,          -- 1 = hoạt động, 0 = khóa
    CreatedAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Users_Employee FOREIGN KEY (EmployeeID)
        REFERENCES Employee(EmployeeID)
);

select * from Roles
CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleCode NVARCHAR(255) NOT NULL UNIQUE,-- Ví dụ: Admin, Manager, Staff
    RoleName NVARCHAR(50) NOT NULL     
);

UPDATE Roles
SET RoleCode = N'nldh'
WHERE RoleID = 5;

INSERT INTO Roles (RoleCode, RoleName)
VALUES
    (N'xegkh',       N'Xuất Excel Gửi Khách Hàng');

CREATE TABLE UserRoles (
    UserID INT NOT NULL,
    RoleID INT NOT NULL,
    PRIMARY KEY (UserID, RoleID),

    CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserID)
        REFERENCES Users(UserID)
        ON DELETE CASCADE,

    CONSTRAINT FK_UserRoles_Role FOREIGN KEY (RoleID)
        REFERENCES Roles(RoleID)
        ON DELETE CASCADE
);

CREATE PROCEDURE sp_UpdateUserRoles
    @UserID INT,
    @RoleIDs NVARCHAR(MAX)  -- Chuỗi "1,3,5"
AS
BEGIN
    SET NOCOUNT ON;

    -- Xóa các role không còn được chọn
    DELETE FROM UserRoles
    WHERE UserID = @UserID
      AND RoleID NOT IN (
          SELECT CAST(value AS INT)
          FROM STRING_SPLIT(@RoleIDs, ',')
      );

    -- Thêm role mới chưa có
    INSERT INTO UserRoles (UserID, RoleID)
    SELECT @UserID, CAST(value AS INT)
    FROM STRING_SPLIT(@RoleIDs, ',')
    WHERE CAST(value AS INT) NOT IN (
        SELECT RoleID FROM UserRoles WHERE UserID = @UserID
    );
END

CREATE PROCEDURE sp_ChangeUserPassword
    @Username NVARCHAR(50),
    @NewPassword NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentHash NVARCHAR(255);

    -- Lấy hash hiện tại trong DB
    SELECT @CurrentHash = PasswordHash 
    FROM Users 
    WHERE Username = @Username AND IsActive = 1;

    IF @CurrentHash IS NULL
    BEGIN
        RAISERROR('Người dùng không tồn tại hoặc bị khóa.', 16, 1);
        RETURN;
    END;

    -- Cập nhật mật khẩu mới (đã hash sẵn ở C#)
    UPDATE Users
    SET PasswordHash = @NewPassword
    WHERE Username = @Username;

    SELECT 'Đổi mật khẩu thành công!' AS Message;
END;
