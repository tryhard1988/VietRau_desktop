USE QL_User
GO
Exec sp_spaceused;

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,    -- Tên đăng nhập
    PasswordHash NVARCHAR(255) NOT NULL,      -- Mật khẩu đã mã hóa (hash)
    EmployeeCode NVARCHAR(20) NOT NULL UNIQUE, -- Nếu có liên kết đến nhân viên (FK)
    IsActive BIT NOT NULL DEFAULT 1,          -- 1 = hoạt động, 0 = khóa
    CreatedAt DATETIME DEFAULT GETDATE()
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