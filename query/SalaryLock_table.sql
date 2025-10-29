USE SalaryLock
GO
Exec sp_spaceused;

CREATE TABLE SalaryLock (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Month INT NOT NULL,
    Year INT NOT NULL,
    IsLocked BIT NOT NULL DEFAULT 0,
    CONSTRAINT UQ_SalaryLock UNIQUE (Month, Year) 
);
