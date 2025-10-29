USE QLNV_VR_History
GO
Exec sp_spaceused;

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
    IsInsuranceRefund BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT UQ_EmployeeSalaryInfo_Employee_Month_Year UNIQUE (EmployeeCode, Month, Year)
);

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