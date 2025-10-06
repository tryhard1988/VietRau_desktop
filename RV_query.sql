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

select * from Position
CREATE TABLE Position (
    PositionID INT IDENTITY(1,1) PRIMARY KEY,    -- Mã chức vụ tự tăng
    PositionName NVARCHAR(100) NOT NULL UNIQUE,  -- Tên chức vụ (VD: Giám đốc, Nhân viên)
    Description NVARCHAR(255) NULL,              -- Mô tả thêm nếu cần
    IsActive BIT DEFAULT 1,                      -- Còn sử dụng hay không
    CreatedAt DATETIME DEFAULT GETDATE()         -- Ngày tạo
);

select * from Department
CREATE TABLE Department (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,   -- Mã phòng ban tự tăng
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE, -- Tên phòng ban (VD: Hành chính, Kỹ thuật...)
    Description NVARCHAR(255) NULL,               -- Mô tả (tùy chọn)
    IsActive BIT DEFAULT 1,                       -- Trạng thái hoạt động (1 = còn, 0 = ngưng)
    CreatedAt DATETIME DEFAULT GETDATE()          -- Ngày tạo bản ghi
);

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
    CreatedAt DATETIME DEFAULT GETDATE(),                   -- Ngày tạo bản ghi

    -- 🔗 Khóa ngoại
    CONSTRAINT FK_Employee_Position FOREIGN KEY (PositionID)
        REFERENCES Position(PositionID),

    CONSTRAINT FK_Employee_Department FOREIGN KEY (DepartmentID)
        REFERENCES Department(DepartmentID),

    CONSTRAINT FK_Employee_ContractType FOREIGN KEY (ContractTypeID)
        REFERENCES ContractType(ContractTypeID)
);
