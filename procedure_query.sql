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

DROP PROCEDURE UpdateAttendanceAuto;

CREATE OR ALTER PROCEDURE UpdateAttendanceAuto
    @EmployeeID INT,
    @ShiftID INT,
    @WorkDate DATE,
    @CheckIn DATETIME,
    @CheckOut DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @WorkingHours FLOAT,
        @StartTime TIME,
        @EndTime TIME,
        @AdjCheckIn DATETIME,
        @AdjCheckOut DATETIME;

    -- ✅ 1. Nếu ShiftID NULL → ca 4 mặc định (không xác định)
    IF @ShiftID IS NULL
    BEGIN
        SET @ShiftID = 4;
        SET @WorkingHours = 0;
    END
    ELSE
    BEGIN
        -- ✅ 2. Lấy giờ bắt đầu & kết thúc ca
        SELECT 
            @StartTime = StartTime, 
            @EndTime = EndTime
        FROM Shift
        WHERE ShiftID = @ShiftID;

        -- ✅ 3. Điều chỉnh CheckIn, CheckOut theo khung giờ ca
        SET @AdjCheckIn = CASE 
            WHEN CAST(@CheckIn AS TIME) < @StartTime 
                THEN DATEADD(SECOND, DATEDIFF(SECOND, 0, @StartTime), CAST(@WorkDate AS DATETIME))
            ELSE @CheckIn
        END;

        SET @AdjCheckOut = CASE 
            WHEN CAST(@CheckOut AS TIME) > @EndTime 
                THEN DATEADD(SECOND, DATEDIFF(SECOND, 0, @EndTime), CAST(@WorkDate AS DATETIME))
            ELSE @CheckOut
        END;

        -- ✅ 4. Tính WorkingHours (nếu có đủ dữ liệu)
        IF @AdjCheckIn IS NULL OR @AdjCheckOut IS NULL
            SET @WorkingHours = 0;
        ELSE
            SET @WorkingHours = DATEDIFF(MINUTE, @AdjCheckIn, @AdjCheckOut) / 60.0;
    END

    -- ✅ 5. Ghi vào bảng Attendance
    IF NOT EXISTS (
        SELECT 1 FROM Attendance 
        WHERE EmployeeID = @EmployeeID AND WorkDate = @WorkDate AND ShiftID = @ShiftID
    )
    BEGIN
        INSERT INTO Attendance (EmployeeID, WorkDate, ShiftID, CheckIn, CheckOut, WorkingHours)
        VALUES (@EmployeeID, @WorkDate, @ShiftID, @AdjCheckIn, @AdjCheckOut, @WorkingHours);
    END
    ELSE
    BEGIN
        UPDATE Attendance
        SET CheckIn = @AdjCheckIn,
            CheckOut = @AdjCheckOut,
            WorkingHours = @WorkingHours
        WHERE EmployeeID = @EmployeeID AND WorkDate = @WorkDate AND ShiftID = @ShiftID;
    END;
END;
GO

Drop TYPE dbo.AttendanceTableType
CREATE TYPE dbo.AttendanceTableType AS TABLE
(
    EmployeeCode NVARCHAR(20),
    WorkDate DATE,
    WorkingHours FLOAT,
    Note NVARCHAR(255),
    AttendanceLog NVARCHAR(255)
);
GO

Drop PROCEDURE dbo.UpsertAttendanceBatch
CREATE OR ALTER PROCEDURE dbo.UpsertAttendanceBatch
    @AttendanceList dbo.AttendanceTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    MERGE Attendance AS target
    USING @AttendanceList AS src
        ON target.EmployeeCode = src.EmployeeCode 
        AND target.WorkDate = src.WorkDate
    WHEN MATCHED THEN
        UPDATE SET 
            WorkingHours = src.WorkingHours,
            Note = src.Note,
            AttendanceLog = src.AttendanceLog
    WHEN NOT MATCHED THEN
        INSERT (EmployeeCode, WorkDate, WorkingHours, Note, AttendanceLog)
        VALUES (src.EmployeeCode, src.WorkDate, src.WorkingHours, src.Note, src.AttendanceLog);
END
GO


CREATE OR ALTER PROCEDURE Upsert_AnnualLeaveBalance
    @EmployeeCode NVARCHAR(20),
    @Year INT,
    @Month NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM AnnualLeaveBalance
        WHERE EmployeeCode = @EmployeeCode AND Year = @Year
    )
    BEGIN
        -- 🔄 Nếu đã tồn tại → cập nhật tháng (hoặc các cột khác sau này)
        UPDATE AnnualLeaveBalance
        SET Month = @Month
        WHERE EmployeeCode = @EmployeeCode AND Year = @Year;
    END
    ELSE
    BEGIN
        -- 🆕 Nếu chưa có → thêm mới
        INSERT INTO AnnualLeaveBalance (EmployeeCode, Year, Month)
        VALUES (@EmployeeCode, @Year, @Month);
    END
END;
GO


CREATE TYPE AnnualLeaveBalanceType AS TABLE
(
    EmployeeCode NVARCHAR(20),
    Year INT,
    Month NVARCHAR(100)
);
GO

CREATE OR ALTER PROCEDURE Upsert_AnnualLeaveBalance_Batch
    @AnnualLeaveList AnnualLeaveBalanceType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    -- Cập nhật những dòng đã tồn tại
    UPDATE ALB
    SET ALB.Month = T.Month
    FROM AnnualLeaveBalance AS ALB
    INNER JOIN @AnnualLeaveList AS T
        ON ALB.EmployeeCode = T.EmployeeCode
        AND ALB.Year = T.Year;

    -- Thêm mới những dòng chưa có
    INSERT INTO AnnualLeaveBalance (EmployeeCode, Year, Month)
    SELECT T.EmployeeCode, T.Year, T.Month
    FROM @AnnualLeaveList AS T
    WHERE NOT EXISTS (
        SELECT 1 
        FROM AnnualLeaveBalance AS ALB
        WHERE ALB.EmployeeCode = T.EmployeeCode
          AND ALB.Year = T.Year
    );
END;
GO


CREATE FUNCTION dbo.fn_AddMonthToList (
    @MonthList NVARCHAR(100),
    @NewMonth INT
)
RETURNS NVARCHAR(100)
AS
BEGIN
    IF @MonthList IS NULL OR LTRIM(RTRIM(@MonthList)) = ''
        RETURN CAST(@NewMonth AS NVARCHAR(2));

    IF ',' + @MonthList + ',' LIKE '%,' + CAST(@NewMonth AS NVARCHAR(2)) + ',%'
        RETURN @MonthList;

    RETURN @MonthList + ',' + CAST(@NewMonth AS NVARCHAR(2));
END
GO


CREATE PROCEDURE dbo.UpdateAnnualLeaveMonthList
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentYear INT = YEAR(GETDATE());
    DECLARE @CurrentMonth INT = MONTH(GETDATE());

    /*
        1️⃣ Thêm dòng mới cho những nhân viên đủ điều kiện mà:
            - chưa có AnnualLeaveBalance trong năm hiện tại
    */
    INSERT INTO AnnualLeaveBalance (EmployeeCode, [Year], [Month])
    SELECT e.EmployeeCode, @CurrentYear, CAST(@CurrentMonth AS NVARCHAR(10))
    FROM Employee e
    INNER JOIN ContractType ct ON e.ContractTypeID = ct.ContractTypeID
    WHERE e.IsActive = 1
      AND ct.ContractTypeCode = 'c_thuc'
      AND NOT EXISTS (
          SELECT 1
          FROM AnnualLeaveBalance alb
          WHERE alb.EmployeeCode = e.EmployeeCode
            AND alb.[Year] = @CurrentYear
      );

    /*
        2️⃣ Cập nhật dòng đã có: thêm tháng mới vào cột Month
    */
    UPDATE alb
    SET alb.[Month] = dbo.fn_AddMonthToList(alb.[Month], @CurrentMonth)
    FROM AnnualLeaveBalance alb
    INNER JOIN Employee e ON alb.EmployeeCode = e.EmployeeCode
    INNER JOIN ContractType ct ON e.ContractTypeID = ct.ContractTypeID
    WHERE alb.[Year] = @CurrentYear
      AND e.IsActive = 1
      AND ct.ContractTypeCode = 'c_thuc';
END
GO
