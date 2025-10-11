create database RauVietDB

USE RauVietDB

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


CREATE PROCEDURE dbo.UpdateEmployeeShiftsByString
    @EmployeeID INT,
    @ShiftIDs VARCHAR(MAX)  -- ví dụ: '1,2,3'
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Chuyển chuỗi CSV thành bảng tạm
    DECLARE @ShiftTable TABLE (ShiftID INT);

    INSERT INTO @ShiftTable (ShiftID)
    SELECT TRY_CAST(value AS INT)
    FROM STRING_SPLIT(@ShiftIDs, ',')  -- SQL Server 2016+ hỗ trợ STRING_SPLIT
    WHERE TRY_CAST(value AS INT) IS NOT NULL;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Xóa những ca hiện tại không còn trong danh sách mới
        DELETE es
        FROM EmployeeShift es
        LEFT JOIN @ShiftTable s ON es.ShiftID = s.ShiftID
        WHERE es.EmployeeID = @EmployeeID AND s.ShiftID IS NULL;

        -- Thêm những ca mới chưa có
        INSERT INTO EmployeeShift (EmployeeID, ShiftID)
        SELECT @EmployeeID, s.ShiftID
        FROM @ShiftTable s
        LEFT JOIN EmployeeShift es 
            ON es.EmployeeID = @EmployeeID AND es.ShiftID = s.ShiftID
        WHERE es.EmployeeShiftID IS NULL;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END



