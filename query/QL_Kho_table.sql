USE QL_Kho
GO
Exec sp_spaceused;

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