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

select * from ContractType
CREATE TABLE ContractType (
    ContractTypeID INT IDENTITY(1,1) PRIMARY KEY,   -- Khóa chính, tự tăng
    ContractTypeCode NVARCHAR(30) NOT NULL UNIQUE,
    ContractTypeName NVARCHAR(100) NOT NULL,        -- Tên loại hợp đồng (VD: Toàn thời gian, Thời vụ, Thực tập,...)
    Description NVARCHAR(255) NULL                  -- Mô tả thêm (nếu cần)
);

CREATE TABLE Position (
    PositionID INT IDENTITY(1,1) PRIMARY KEY,    -- Mã chức vụ tự tăng
    PositionCode NVARCHAR(30) NOT NULL UNIQUE,
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

CREATE TABLE Employee (
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,               -- Mã nhân viên tự tăng
    EmployeeCode NVARCHAR(20) NOT NULL UNIQUE,              -- Mã nhân viên (VD: NV001)
    FullName NVARCHAR(100) NOT NULL,                        -- Họ và tên
    BirthDate DATE NULL,                                    -- Ngày sinh
    HireDate DATE NULL,                                     -- Ngày vào làm
    Gender BIT NULL,                                        -- 1 = Nam, 0 = Nữ
    ProbationSalaryPercent  DECIMAL(5,2) NOT NULL DEFAULT 0.85,
    PhoneNumber NVARCHAR(20) NULL,     -- Số điện thoại liên hệ
    NoteResign NVARCHAR(255) NULL,     -- Ghi chú ra/vào công ty

    BankName NVARCHAR(100) NULL,
    BankBranch NVARCHAR(100) NULL,         -- Chi nhánh
    BankAccountNumber NVARCHAR(50) NULL,   -- Số tài khoản
    BankAccountHolder NVARCHAR(100) NULL,  -- Chủ tài khoản

    SocialInsuranceNumber NVARCHAR(50) NULL,  -- Số sổ bảo hiểm xã hội
    HealthInsuranceNumber NVARCHAR(50) NULL,  -- Số thẻ bảo hiểm y tế

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
    SalaryGradeID INT NULL
    -- ⚙️ Trạng thái
    IsActive BIT DEFAULT 1,                                 -- Đang làm việc (1 = còn làm, 0 = nghỉ)
    canCreateUserName BIT NOT NULL DEFAULT 0,
    IsInsuranceRefund BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),                   -- Ngày tạo bản ghi

    -- 🔗 Khóa ngoại
    CONSTRAINT FK_Employee_Position FOREIGN KEY (PositionID)
        REFERENCES Position(PositionID),

    CONSTRAINT FK_Employee_Department FOREIGN KEY (DepartmentID)
        REFERENCES Department(DepartmentID),

    CONSTRAINT FK_Employee_ContractType FOREIGN KEY (ContractTypeID)
        REFERENCES ContractType(ContractTypeID)

    CONSTRAINT FK_Employee_SalaryGrade FOREIGN KEY (SalaryGradeID) 
        REFERENCES SalaryGrade(SalaryGradeID);
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

CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleCode NVARCHAR(255) NOT NULL UNIQUE,-- Ví dụ: Admin, Manager, Staff
    RoleName NVARCHAR(50) NOT NULL     
);

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

CREATE TABLE Attendance (
    AttendanceID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode NVARCHAR(20) NOT NULL,      -- Mã nhân viên thay cho EmployeeID
    WorkDate DATE NOT NULL,                   -- Ngày làm việc
    WorkingHours FLOAT NULL,                  -- sẽ tính bằng procedure
    Note NVARCHAR(255) NULL,
    AttendanceLog NVARCHAR(255) NOT NULL DEFAULT ''

    -- Nếu muốn ràng buộc với bảng Employee
    CONSTRAINT FK_Attendance_Employee FOREIGN KEY (EmployeeCode)
        REFERENCES Employee(EmployeeCode),

    CONSTRAINT UQ_Attendance UNIQUE (EmployeeCode, WorkDate)
);

CREATE TABLE Holiday (
    HolidayDate DATE PRIMARY KEY,
    HolidayName NVARCHAR(100)
);

CREATE TABLE OvertimeType (
    OvertimeTypeID INT IDENTITY(1,1) PRIMARY KEY,      -- Khóa chính tự tăng
    OvertimeName NVARCHAR(100) NOT NULL,              -- Tên loại tăng ca
    SalaryFactor FLOAT NOT NULL DEFAULT 1,            -- Hệ số nhân lương    
    IsActive BIT DEFAULT 1                             -- Có áp dụng hay không
);

CREATE TABLE OvertimeAttendance (
    OvertimeAttendanceID INT IDENTITY(1,1) PRIMARY KEY,  -- Khóa chính tự tăng
    EmployeeCode NVARCHAR(20) NOT NULL,                     -- Mã nhân viên thay cho EmployeeID
    WorkDate DATE NOT NULL,                               -- Ngày tăng ca
    StartTime TIME NOT NULL,                              -- Giờ bắt đầu tăng ca
    EndTime TIME NOT NULL,                                -- Giờ kết thúc tăng ca
    OvertimeTypeID INT NOT NULL,                          -- Loại tăng ca (FK)
    Note NVARCHAR(255) NULL,                              -- Ghi chú
    UpdatedHistory NVARCHAR(MAX) NULL,                         -- Ai cập nhật bản ghi

    -- Khóa ngoại
    CONSTRAINT FK_OvertimeAttendance_Employee FOREIGN KEY (EmployeeCode)
        REFERENCES Employee(EmployeeCode),

    CONSTRAINT FK_OvertimeAttendance_Type FOREIGN KEY (OvertimeTypeID)
        REFERENCES OvertimeType(OvertimeTypeID)
);

CREATE TABLE AnnualLeaveBalance (
    BalanceID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode NVARCHAR(20) NOT NULL,    
    Year INT NOT NULL,
    Month NVARCHAR(100) NOT NULL

    CONSTRAINT FK_AnnualLeaveBalance_Employee FOREIGN KEY (EmployeeCode)
        REFERENCES Employee(EmployeeCode)

         CONSTRAINT UQ_AnnualLeaveBalance_Employee_Year UNIQUE (EmployeeCode, Year)
);
select * from LeaveType
CREATE TABLE LeaveType (
    LeaveTypeID INT IDENTITY(1,1) PRIMARY KEY,        -- Khóa chính tự tăng
    LeaveTypeCode NVARCHAR(20) NOT NULL UNIQUE,       -- Mã loại phép (AL, SL, UL,...)
    LeaveTypeName NVARCHAR(100) NOT NULL,             -- Tên loại phép (Nghỉ phép năm, Nghỉ bệnh,...)
    IsPaid BIT DEFAULT 1,                             -- Có hưởng lương không (1: có, 0: không)
    IsDeductAnnualLeave BIT NOT NULL DEFAULT 0
);

CREATE TABLE LeaveAttendance (
    LeaveID INT IDENTITY(1,1) PRIMARY KEY,       -- Khóa chính tự tăng
    EmployeeCode NVARCHAR(20) NOT NULL,          -- Mã nhân viên
    LeaveTypeCode NVARCHAR(20) NOT NULL,         -- Loại phép (FK)
    DateOff DATE NOT NULL,                       -- Ngày nghỉ
    Note NVARCHAR(255) NULL,                     -- Ghi chú
    UpdatedHistory NVARCHAR(MAX) NULL,           -- Lịch sử cập nhật
    LeaveHours DECIMAL(5,2) NOT NULL DEFAULT 8,

    CONSTRAINT FK_LeaveAttendance_Employee FOREIGN KEY (EmployeeCode)
        REFERENCES Employee(EmployeeCode),

    CONSTRAINT FK_LeaveAttendance_LeaveType FOREIGN KEY (LeaveTypeCode)
        REFERENCES LeaveType(LeaveTypeCode),

    CONSTRAINT UQ_LeaveAttendance_Employee_Date UNIQUE (EmployeeCode, DateOff)
);

CREATE TABLE ApplyScope (
    ApplyScopeID INT IDENTITY(1,1) PRIMARY KEY,   -- Khóa chính tự tăng
    ScopeCode NVARCHAR(20) NOT NULL UNIQUE,       -- Mã nhóm áp dụng (VD: EMP, DEP, POS, ALL)
    ScopeName NVARCHAR(100) NOT NULL,             -- Tên hiển thị (VD: Nhân viên, Phòng ban, Chức vụ, Toàn công ty)
);

select * from AllowanceType
CREATE TABLE AllowanceType (
    AllowanceTypeID INT IDENTITY(1,1) PRIMARY KEY,   -- Khóa chính tự tăng
    AllowanceCode NVARCHAR(20) NOT NULL UNIQUE,
    AllowanceName NVARCHAR(100) NOT NULL UNIQUE,     -- Tên phụ cấp (VD: Phụ cấp ăn trưa)
    IsInsuranceIncluded BIT DEFAULT 0,               -- Có tính đóng BHXH/BHYT/BHTN không
    ApplyScopeID INT NOT NULL,                       -- Nhóm áp dụng (FK -> ApplyScope)
    IsActive BIT DEFAULT 1,                          -- Còn áp dụng hay không

    CONSTRAINT FK_AllowanceType_ApplyScope FOREIGN KEY (ApplyScopeID)
        REFERENCES ApplyScope(ApplyScopeID)
);

CREATE TABLE DepartmentAllowance (
    DepartmentAllowanceID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentID INT NOT NULL,
    AllowanceTypeID INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Note NVARCHAR(255) NULL,

    CONSTRAINT UQ_DepartmentAllowance UNIQUE (DepartmentID, AllowanceTypeID),
    CONSTRAINT FK_DepartmentAllowance_Department FOREIGN KEY (DepartmentID)
        REFERENCES Department(DepartmentID),
    CONSTRAINT FK_DepartmentAllowance_AllowanceType FOREIGN KEY (AllowanceTypeID)
        REFERENCES AllowanceType(AllowanceTypeID)
);

CREATE TABLE PositionAllowance (
    PositionAllowanceID INT IDENTITY(1,1) PRIMARY KEY,
    PositionID INT NOT NULL,
    AllowanceTypeID INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Note NVARCHAR(255) NULL,

    CONSTRAINT UQ_PositionAllowance UNIQUE (PositionID, AllowanceTypeID),
    CONSTRAINT FK_PositionAllowance_Position FOREIGN KEY (PositionID)
        REFERENCES Position(PositionID),
    CONSTRAINT FK_PositionAllowance_AllowanceType FOREIGN KEY (AllowanceTypeID)
        REFERENCES AllowanceType(AllowanceTypeID)
);

CREATE TABLE EmployeeAllowance (
    EmployeeAllowanceID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode NVARCHAR(20) NOT NULL,
    AllowanceTypeID INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Note NVARCHAR(255) NULL,

    CONSTRAINT UQ_EmployeeAllowance UNIQUE (EmployeeCode, AllowanceTypeID),
    CONSTRAINT FK_EmployeeAllowance_Employee FOREIGN KEY (EmployeeCode)
        REFERENCES Employee(EmployeeCode),
    CONSTRAINT FK_EmployeeAllowance_AllowanceType FOREIGN KEY (AllowanceTypeID)
        REFERENCES AllowanceType(AllowanceTypeID)
);


CREATE TABLE MonthlyAllowance (
    MonthlyAllowanceID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode NVARCHAR(20) NOT NULL,
    AllowanceTypeID INT NOT NULL,
    Month INT NOT NULL,               -- Tháng phát sinh
    Year INT NOT NULL,                -- Năm phát sinh
    Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Note NVARCHAR(255) NULL,

    CONSTRAINT FK_MonthlyAllowance_Employee FOREIGN KEY (EmployeeCode)
        REFERENCES Employee(EmployeeCode),
    CONSTRAINT FK_MonthlyAllowance_AllowanceType FOREIGN KEY (AllowanceTypeID)
        REFERENCES AllowanceType(AllowanceTypeID),

    CONSTRAINT UQ_MonthlyAllowance UNIQUE (EmployeeCode, AllowanceTypeID, Month, Year)
);

CREATE TABLE SalaryGrade (
    SalaryGradeID INT IDENTITY(1,1) PRIMARY KEY,      -- Khóa chính tự tăng
    GradeName NVARCHAR(100) NOT NULL,                 -- Tên bậc lương (ví dụ: Bậc 1, Bậc 2,...)
    Salary INT NOT NULL,                    -- Mức lương tối thiểu của bậc    
    Note NVARCHAR(255) NULL,                          -- Ghi chú
    IsActive BIT NOT NULL DEFAULT 1,                  -- Còn hiệu lực hay không
);

CREATE TABLE EmployeeSalaryInfo (
    SalaryInfoID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode NVARCHAR(20) NOT NULL,
    Month INT NOT NULL,
    Year INT NOT NULL,
    BaseSalary INT NOT NULL,    
    InsuranceBaseSalary INT NOT NULL DEFAULT 0,
    Note NVARCHAR(255) NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

select * from DeductionType 
CREATE TABLE DeductionType (
    DeductionTypeID INT IDENTITY(1,1) PRIMARY KEY,     -- Khóa chính
    DeductionTypeCode NVARCHAR(20) NOT NULL UNIQUE,    -- Mã khoản trừ (VD: ADV, VEG, CEP)
    DeductionTypeName NVARCHAR(100) NOT NULL,          -- Tên khoản trừ (VD: Ứng lương, Tiền rau)
    IsActive BIT DEFAULT 1                             -- Còn sử dụng
);

CREATE TABLE EmployeeDeduction (
    EmployeeDeductionID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode NVARCHAR(20) NOT NULL,
    DeductionTypeCode NVARCHAR(20) NOT NULL,
    DeductionMonth INT NOT NULL,
    DeductionYear INT NOT NULL,
    DeductionDate DATE NOT NULL,
    Amount INT NOT NULL DEFAULT 0,
    Note NVARCHAR(255) NULL,
    UpdateHistory NVARCHAR(255) NULL,

    CONSTRAINT FK_EmployeeDeduction_Employee FOREIGN KEY (EmployeeCode)
        REFERENCES Employee(EmployeeCode),
    CONSTRAINT FK_EmployeeDeduction_DeductionTypeCode FOREIGN KEY (DeductionTypeCode)
        REFERENCES DeductionType(DeductionTypeCode)
);

select * from EmployeeAllowanceHistory
drop table EmployeeAllowanceHistory
CREATE TABLE EmployeeAllowanceHistory (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ScopeCode NVARCHAR(20) NOT NULL,
    AllowanceTypeID INT NOT NULL,
    EmployeeCode NVARCHAR(20) NOT NULL,
    AllowanceName NVARCHAR(100) NULL,
    IsInsuranceIncluded BIT DEFAULT 0,
    Amount INT NOT NULL DEFAULT 0,
    Month INT NOT NULL,
    Year INT NOT NULL
);

UPDATE SalaryLock
SET IsLocked = 0
WHERE ID = 4;

select * from SalaryLock
CREATE TABLE SalaryLock (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Month INT NOT NULL,
    Year INT NOT NULL,
    IsLocked BIT NOT NULL DEFAULT 0,
    CONSTRAINT UQ_SalaryLock UNIQUE (Month, Year) 
);

select * from EmployeeSalaryHistory
Drop TABLE EmployeeSalaryHistory
CREATE TABLE EmployeeSalaryHistory (
    SalaryInfoID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode NVARCHAR(20) NOT NULL,
    ContractTypeName NVARCHAR(100) NOT NULL,
    Month INT NOT NULL,
    Year INT NOT NULL,
    BaseSalary DECIMAL(18,2) NOT NULL,              
    NetSalary DECIMAL(18,2) NOT NULL,               -- Lương thực lãnh (đã bao gồm tất cả)
    NetInsuranceSalary DECIMAL(18,2) NOT NULL,      -- Lương tính đóng bảo hiểm
    InsuranceAllowance DECIMAL(18,2) DEFAULT 0,     -- Phụ cấp đóng bảo hiểm
    NonInsuranceAllowance DECIMAL(18,2) DEFAULT 0,  -- Phụ cấp không đóng bảo hiểm
    OvertimeSalary DECIMAL(18,2) DEFAULT 0,         -- Tiền lương tăng ca
    LeaveSalary DECIMAL(18,2) DEFAULT 0,            -- Tiền lương ngày nghỉ
    DeductionAmount DECIMAL(18,2) DEFAULT 0,        -- Các khoản trừ
    
    CreatedAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT UQ_EmployeeSalaryInfo_Employee_Month_Year UNIQUE (EmployeeCode, Month, Year)
);
