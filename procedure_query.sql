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

Drop PROCEDURE sp_GetEmployeeAllowanceHistory_ByMonth
CREATE PROCEDURE sp_GetEmployeeAllowanceHistory_ByMonth
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ScopeCode,
        AllowanceTypeID,
        EmployeeCode,
        AllowanceName,
        Amount,
        IsInsuranceIncluded        
    FROM EmployeeAllowanceHistory
    WHERE Month = @Month
      AND Year = @Year
    ORDER BY EmployeeCode, AllowanceName;
END;
GO


Drop PROCEDURE sp_GetEmployeeAllowance
CREATE PROCEDURE sp_GetEmployeeAllowance
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.EmployeeCode,
        at.AllowanceName, 
        at.AllowanceTypeID,
        s.ScopeCode,
        ISNULL(a.Amount, 0) AS Amount,
        at.IsInsuranceIncluded
    FROM Employee e
    CROSS JOIN AllowanceType at
    LEFT JOIN ApplyScope s ON at.ApplyScopeID = s.ApplyScopeID

    OUTER APPLY (
        SELECT 
            CAST(ma.Amount AS INT) AS Amount, 
            ma.Note, 
            N'Phụ cấp tháng' AS Source
        FROM MonthlyAllowance ma
        WHERE ma.EmployeeCode = e.EmployeeCode 
            AND ma.AllowanceTypeID = at.AllowanceTypeID
            AND ma.Month = @Month 
            AND ma.Year = @Year

        UNION ALL

        SELECT 
            CAST(ea.Amount AS INT), 
            ea.Note, 
            N'Theo nhân viên'
        FROM EmployeeAllowance ea
        WHERE ea.EmployeeCode = e.EmployeeCode 
            AND ea.AllowanceTypeID = at.AllowanceTypeID

        UNION ALL

        SELECT 
            CAST(pa.Amount AS INT), 
            pa.Note, 
            N'Theo chức vụ'
        FROM PositionAllowance pa
        WHERE pa.PositionID = e.PositionID 
            AND pa.AllowanceTypeID = at.AllowanceTypeID

        UNION ALL

        SELECT 
            CAST(da.Amount AS INT), 
            da.Note, 
            N'Theo phòng ban'
        FROM DepartmentAllowance da
        WHERE da.DepartmentID = e.DepartmentID 
            AND da.AllowanceTypeID = at.AllowanceTypeID
    ) a

    WHERE e.IsActive = 1
    ORDER BY e.EmployeeCode, at.AllowanceTypeID, at.IsInsuranceIncluded;
END;
GO

drop PROCEDURE sp_GetEmployeeSalarySummary
CREATE PROCEDURE sp_GetEmployeeSalarySummary
    @Month INT,
    @Year INTsp_GetEmployeeDeductions
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.EmployeeCode,

        -- 💰 Lương cơ bản (thử việc tính theo %)
        CASE 
            WHEN ct.ContractTypeCode = 't_viec' THEN ISNULL(sal.BaseSalary, 0) * e.ProbationSalaryPercent
            ELSE ISNULL(sal.BaseSalary, 0)
        END AS BaseSalary,

        -- 💵 Lương đóng BH (chỉ áp dụng cho chính thức)
        CASE 
            WHEN ct.ContractTypeCode = 'c_thuc' THEN ISNULL(sal.InsuranceBaseSalary, 0)
            ELSE 0
        END AS InsuranceBaseSalary

    FROM Employee e
    LEFT JOIN ContractType ct 
        ON e.ContractTypeID = ct.ContractTypeID

    OUTER APPLY (
        SELECT TOP 1 
            esi.BaseSalary, 
            esi.InsuranceBaseSalary
        FROM EmployeeSalaryInfo esi
        WHERE esi.EmployeeCode = e.EmployeeCode
          AND (esi.Year < @Year OR (esi.Year = @Year AND esi.Month <= @Month))
        ORDER BY esi.Year DESC, esi.Month DESC
    ) sal

    WHERE e.IsActive = 1
    ORDER BY e.EmployeeCode;
END;
GO






CREATE OR ALTER PROCEDURE sp_InsertHolidayAndAttendance
    @HolidayDate DATE,
    @HolidayName NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1️⃣ Thêm vào bảng Holiday
        INSERT INTO Holiday (HolidayDate, HolidayName)
        VALUES (@HolidayDate, @HolidayName);

        -- 2️⃣ Thêm vào LeaveAttendance cho nhân viên chính thức
        INSERT INTO LeaveAttendance (EmployeeCode, LeaveTypeCode, DateOff, Note, UpdatedHistory)
        SELECT 
            e.EmployeeCode,
            'NL_1' AS LeaveTypeCode,
            @HolidayDate AS DateOff,
            @HolidayName,
            CONCAT('Tự động tạo ngày: ', CONVERT(NVARCHAR(10), GETDATE(), 120))
        FROM Employee e
        INNER JOIN ContractType ct ON e.ContractTypeID = ct.ContractTypeID
        WHERE ct.ContractTypeCode = 'c_thuc'
          AND e.IsActive = 1
          AND NOT EXISTS (
                SELECT 1 
                FROM LeaveAttendance la 
                WHERE la.EmployeeCode = e.EmployeeCode 
                  AND la.DateOff = @HolidayDate
          );

        COMMIT TRANSACTION;
        RETURN 1; -- ✅ Thành công
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        RETURN 0; -- ❌ Thất bại
    END CATCH
END;
GO

CREATE OR ALTER PROCEDURE sp_DeleteHolidayAndAttendance
    @HolidayDate DATE,
    @IsDeleted BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @IsDeleted = 0;  -- mặc định là false

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1️⃣ Xóa trong bảng LeaveAttendance trước
        DELETE FROM LeaveAttendance
        WHERE DateOff = @HolidayDate
          AND LeaveTypeCode = 'NL_1';

        -- 2️⃣ Sau đó xóa trong bảng Holiday
        DELETE FROM Holiday
        WHERE HolidayDate = @HolidayDate;

        COMMIT TRANSACTION;

        SET @IsDeleted = 1;  -- ✅ nếu không lỗi, coi như thành công
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @IsDeleted = 0;  -- ❌ có lỗi

        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrSeverity INT = ERROR_SEVERITY();
        RAISERROR(@ErrMsg, @ErrSeverity, 1);
    END CATCH
END;
GO

Drop PROCEDURE sp_GetAnnualLeaveReport
CREATE PROCEDURE sp_GetAnnualLeaveReport
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        E.EmployeeCode,                 
        ISNULL(ALB.Month, '') AS [Month],
        ISNULL(LV.LeaveCount, 0) AS LeaveCount
    FROM Employee AS E
    LEFT JOIN Position AS P 
        ON E.PositionID = P.PositionID
    LEFT JOIN ContractType AS CT
        ON E.ContractTypeID = CT.ContractTypeID
    LEFT JOIN AnnualLeaveBalance AS ALB 
        ON E.EmployeeCode = ALB.EmployeeCode
        AND ALB.Year = @Year
    LEFT JOIN (
        SELECT 
            EmployeeCode,
            COUNT(DateOff) AS LeaveCount
        FROM LeaveAttendance la
        INNER JOIN LeaveType lt
            ON la.LeaveTypeCode = lt.LeaveTypeCode
        WHERE lt.IsDeductAnnualLeave = 1
        GROUP BY EmployeeCode
    ) AS LV 
        ON E.EmployeeCode = LV.EmployeeCode
    WHERE 
        E.IsActive = 1
        AND CT.ContractTypeCode = 'c_thuc'
    ORDER BY 
        E.EmployeeCode;
END;
GO
EXEC sp_GetAnnualLeaveReport @Year = 2025;

Drop PROCEDURE sp_GetRemainingLeave
CREATE PROCEDURE sp_GetRemainingLeave
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.EmployeeCode,
        -- 🔹 Phép còn lại = Tổng số phép có - Số phép đã nghỉ
        ISNULL(
            (LEN(alb.Month) - LEN(REPLACE(alb.Month, ',', '')) + 1),
            0
        )
        - ISNULL(lv.UsedLeave, 0) AS RemainingLeave

    FROM Employee e
    LEFT JOIN Position p 
        ON e.PositionID = p.PositionID
    LEFT JOIN ContractType ct 
        ON e.ContractTypeID = ct.ContractTypeID
    LEFT JOIN AnnualLeaveBalance alb 
        ON e.EmployeeCode = alb.EmployeeCode 
        AND alb.Year = @Year
    LEFT JOIN (
        SELECT 
            EmployeeCode,
            COUNT(*) AS UsedLeave
        FROM LeaveAttendance la
        INNER JOIN LeaveType lt
            ON la.LeaveTypeCode = lt.LeaveTypeCode
        WHERE lt.IsDeductAnnualLeave = 1
          AND YEAR(DateOff) = @Year
        GROUP BY EmployeeCode
    ) lv 
        ON e.EmployeeCode = lv.EmployeeCode
    WHERE 
        e.IsActive = 1
        AND ct.ContractTypeCode = 'c_thuc'
    ORDER BY e.EmployeeCode;
END;
GO

EXEC sp_GetRemainingLeave @Year = 2025;

drop PROCEDURE sp_GetPaidLeaveByMonth
CREATE PROCEDURE sp_GetPaidLeaveByMonth
    @Year INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        la.EmployeeCode,
        lt.LeaveTypeCode,
        la.DateOff,
        lt.LeaveTypeName
        
    FROM LeaveAttendance la
    INNER JOIN LeaveType lt
        ON la.LeaveTypeCode = lt.LeaveTypeCode
    WHERE lt.IsPaid = 1
      AND YEAR(la.DateOff) = @Year
      AND MONTH(la.DateOff) = @Month
    ORDER BY la.EmployeeCode, la.DateOff;
END;
GO




Drop PROCEDURE sp_GetEmployeeDeductions
CREATE PROCEDURE sp_GetEmployeeDeductions
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ed.EmployeeDeductionID,
        ed.EmployeeCode,
        ed.DeductionTypeCode,
        ed.DeductionDate,
        ed.Note,
        ed.updateHistory,
        dt.DeductionTypeName,
        ed.Amount
        
    FROM EmployeeDeduction ed
    INNER JOIN DeductionType dt
        ON ed.DeductionTypeCode = dt.DeductionTypeCode
    WHERE ed.DeductionYear = @Year
      AND dt.IsActive = 1
    ORDER BY ed.EmployeeCode, ed.DeductionDate;
END;
GO

Drop PROCEDURE sp_GetAttendanceSummary
CREATE PROCEDURE sp_GetAttendanceSummary
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        a.EmployeeCode,
        a.WorkDate,
        a.WorkingHours,
        a.Note,
        a.AttendanceLog,
        la.LeaveHours,
        -- Ghép tên loại phép + số giờ nghỉ
        CASE 
            WHEN la.LeaveHours IS NOT NULL 
            THEN lt.LeaveTypeName + N': ' + CAST(la.LeaveHours AS NVARCHAR(10)) + N' tiếng'
            ELSE NULL
        END AS LeaveTypeName,

        -- Gộp các loại tăng ca trong cùng ngày
        STRING_AGG(ot.OvertimeName, CHAR(13) + CHAR(10)) AS OvertimeName

    FROM Attendance a
        -- Join nghỉ phép
        LEFT JOIN LeaveAttendance la 
            ON a.EmployeeCode = la.EmployeeCode 
           AND a.WorkDate = la.DateOff
        LEFT JOIN LeaveType lt 
            ON la.LeaveTypeCode = lt.LeaveTypeCode

        -- Join tăng ca
        LEFT JOIN OvertimeAttendance oa
            ON a.EmployeeCode = oa.EmployeeCode
           AND a.WorkDate = oa.WorkDate
        LEFT JOIN OvertimeType ot
            ON oa.OvertimeTypeID = ot.OvertimeTypeID

    WHERE MONTH(a.WorkDate) = @Month
      AND YEAR(a.WorkDate) = @Year

    GROUP BY 
        a.EmployeeCode, 
        a.WorkDate, 
        a.WorkingHours, 
        a.Note, 
        a.AttendanceLog, 
        lt.LeaveTypeName, 
        la.LeaveHours

    ORDER BY 
        a.EmployeeCode, 
        a.WorkDate;
END;
GO



DROP TYPE IF EXISTS EmployeeAllowanceHistoryTableType;
GO

CREATE TYPE EmployeeAllowanceHistoryTableType AS TABLE
(
    ScopeCode NVARCHAR(20),
    AllowanceTypeID INT,
    EmployeeCode NVARCHAR(20),
    AllowanceName NVARCHAR(100),
    IsInsuranceIncluded BIT,
    Amount INT,
    Month INT,
    Year INT
);
GO

Drop PROCEDURE sp_SaveEmployeeAllowanceHistory_Batch
CREATE PROCEDURE sp_SaveEmployeeAllowanceHistory_Batch
    @AllowanceData EmployeeAllowanceHistoryTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Month INT, @Year INT;

    -- Lấy tháng và năm từ dữ liệu truyền vào
    SELECT TOP 1 
        @Month = Month, 
        @Year = Year 
    FROM @AllowanceData;

    -- Kiểm tra dữ liệu đầu vào hợp lệ
    IF @Month IS NULL OR @Year IS NULL
    BEGIN
        RAISERROR(N'Dữ liệu tháng/năm không hợp lệ.', 16, 1);
        RETURN;
    END;

    -- Kiểm tra xem tháng đó đã bị khóa hay chưa
    IF EXISTS (
        SELECT 1 
        FROM SalaryLock 
        WHERE Month = @Month 
          AND Year = @Year 
          AND IsLocked = 1
    )
    BEGIN
        RAISERROR(N'Tháng %d/%d đã bị khóa, không thể cập nhật phụ cấp.', 16, 1, @Month, @Year);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Lưu dữ liệu phụ cấp (MERGE)
        MERGE EmployeeAllowanceHistory AS target
        USING @AllowanceData AS src
        ON target.EmployeeCode = src.EmployeeCode
           AND target.AllowanceTypeID = src.AllowanceTypeID
           AND target.Month = src.Month
           AND target.Year = src.Year
        WHEN MATCHED THEN
            UPDATE SET 
                target.ScopeCode = src.ScopeCode,
                target.AllowanceName = src.AllowanceName,
                target.Amount = src.Amount,
                target.IsInsuranceIncluded = src.IsInsuranceIncluded
        WHEN NOT MATCHED THEN
            INSERT (ScopeCode, AllowanceTypeID, EmployeeCode, AllowanceName, IsInsuranceIncluded, Amount, Month, Year)
            VALUES (src.ScopeCode, src.AllowanceTypeID, src.EmployeeCode, src.AllowanceName, src.IsInsuranceIncluded, src.Amount, src.Month, src.Year);

        -- Nếu cần tự động khóa tháng sau khi lưu
        -- EXEC sp_SetSalaryLock @Month = @Month, @Year = @Year, @IsLocked = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END;
GO



CREATE PROCEDURE sp_SetSalaryLock
    @Month INT,
    @Year INT,
    @IsLocked BIT
AS
BEGIN
    SET NOCOUNT ON;

    MERGE SalaryLock AS target
    USING (SELECT @Month AS Month, @Year AS Year, @IsLocked AS IsLocked) AS src
    ON target.Month = src.Month AND target.Year = src.Year
    WHEN MATCHED THEN
        UPDATE SET target.IsLocked = src.IsLocked
    WHEN NOT MATCHED THEN
        INSERT (Month, Year, IsLocked) VALUES (src.Month, src.Year, src.IsLocked);
END;
GO

Drop TYPE EmployeeSalaryHistoryTableType
CREATE TYPE EmployeeSalaryHistoryTableType AS TABLE (
    EmployeeCode NVARCHAR(20),
    ContractTypeName NVARCHAR(100),
    Month INT,
    Year INT,
    BaseSalary DECIMAL(18,2),
    NetSalary DECIMAL(18,2),
    NetInsuranceSalary DECIMAL(18,2),
    InsuranceAllowance DECIMAL(18,2),
    NonInsuranceAllowance DECIMAL(18,2),
    OvertimeSalary DECIMAL(18,2),
    LeaveSalary DECIMAL(18,2),
    DeductionAmount DECIMAL(18,2)
);
DROP PROCEDURE sp_UpsertEmployeeSalaryHistory_Batch
CREATE PROCEDURE sp_UpsertEmployeeSalaryHistory_Batch
    @SalaryData EmployeeSalaryHistoryTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        MERGE EmployeeSalaryHistory AS target
        USING @SalaryData AS src
        ON target.EmployeeCode = src.EmployeeCode
           AND target.Month = src.Month
           AND target.Year = src.Year

        WHEN MATCHED THEN
            UPDATE SET 
                target.ContractTypeName = src.ContractTypeName,
                target.BaseSalary = src.BaseSalary,
                target.NetSalary = src.NetSalary,
                target.NetInsuranceSalary = src.NetInsuranceSalary,
                target.InsuranceAllowance = src.InsuranceAllowance,
                target.NonInsuranceAllowance = src.NonInsuranceAllowance,
                target.OvertimeSalary = src.OvertimeSalary,
                target.LeaveSalary = src.LeaveSalary,
                target.DeductionAmount = src.DeductionAmount,
                target.CreatedAt = GETDATE()

        WHEN NOT MATCHED THEN
            INSERT (
                EmployeeCode, ContractTypeName, Month, Year, BaseSalary,
                NetSalary, NetInsuranceSalary, 
                InsuranceAllowance, NonInsuranceAllowance,
                OvertimeSalary, LeaveSalary, DeductionAmount, CreatedAt
            )
            VALUES (
                src.EmployeeCode, src.ContractTypeName, src.Month, src.Year, src.BaseSalary,
                src.NetSalary, src.NetInsuranceSalary, 
                src.InsuranceAllowance, src.NonInsuranceAllowance,
                src.OvertimeSalary, src.LeaveSalary, src.DeductionAmount, GETDATE()
            );
    END TRY
    BEGIN CATCH
        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrMsg, 16, 1);
    END CATCH
END;
GO

Drop PROCEDURE sp_GetEmployeeAllowance_ChuyenCan
ALTER PROCEDURE sp_GetEmployeeAllowance_ChuyenCan
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.EmployeeCode,
        e.FullName,
        p.PositionName,
        ct.ContractTypeName,

        -- Phụ cấp chuyên cần (chỉ tính theo EmployeeAllowance hoặc PositionAllowance)
        ISNULL(ea.Amount, ISNULL(pa.Amount, 0)) AS AllowanceAmount,

        -- Khoản trừ chuyên cần (tổng trong tháng)
        ISNULL(SUM(ed.Amount), 0) AS DeductionAmount

    FROM Employee e
    LEFT JOIN Position p 
        ON e.PositionID = p.PositionID
    LEFT JOIN ContractType ct 
        ON e.ContractTypeID = ct.ContractTypeID
    LEFT JOIN AllowanceType at
        ON at.AllowanceCode = 'ALW_chuyenCan'
    LEFT JOIN PositionAllowance pa
        ON pa.PositionID = e.PositionID 
        AND pa.AllowanceTypeID = at.AllowanceTypeID
    LEFT JOIN EmployeeAllowance ea
        ON ea.EmployeeCode = e.EmployeeCode
        AND ea.AllowanceTypeID = at.AllowanceTypeID
    LEFT JOIN EmployeeDeduction ed
        ON ed.EmployeeCode = e.EmployeeCode
        AND ed.DeductionTypeCode = 'ATT'
        AND ed.DeductionMonth = @Month
        AND ed.DeductionYear = @Year

    WHERE e.IsActive = 1
    GROUP BY e.EmployeeCode, e.FullName, p.PositionName, ct.ContractTypeName, ea.Amount, pa.Amount;
END;
GO


EXEC sp_GetEmployeeAllowance_ChuyenCan @Month = 8, @Year = 2025;


CREATE PROCEDURE sp_GetEmployeeLeave_PT_KP
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        la.EmployeeCode,
        lt.LeaveTypeName,
        la.DateOff,
        la.LeaveHours,
        la.Note
    FROM LeaveAttendance la
    JOIN LeaveType lt 
        ON la.LeaveTypeCode = lt.LeaveTypeCode
    WHERE la.LeaveTypeCode IN ('PT_1', 'KP_1')
      AND MONTH(la.DateOff) = @Month
      AND YEAR(la.DateOff) = @Year
    ORDER BY la.EmployeeCode, la.DateOff;
END;
GO

CREATE PROCEDURE sp_UpsertEmployeeDeduction
    @EmployeeCode NVARCHAR(50),
    @DeductionTypeCode NVARCHAR(50),
    @DeductionMonth INT,
    @DeductionYear INT,
    @DeductionDate DATE,
    @Amount DECIMAL(18,2),
    @Note NVARCHAR(250) = NULL,
    @UpdateHistory NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    MERGE EmployeeDeduction AS target
    USING (SELECT 
               @EmployeeCode AS EmployeeCode,
               @DeductionTypeCode AS DeductionTypeCode,
               @DeductionMonth AS DeductionMonth,
               @DeductionYear AS DeductionYear,
               @DeductionDate AS DeductionDate,
               @Amount AS Amount,
               @Note AS Note,
               @UpdateHistory AS UpdateHistory
          ) AS source
    ON target.EmployeeCode = source.EmployeeCode
       AND target.DeductionTypeCode = source.DeductionTypeCode
       AND target.DeductionMonth = source.DeductionMonth
       AND target.DeductionYear = source.DeductionYear
    WHEN MATCHED THEN
        UPDATE SET 
            DeductionDate = source.DeductionDate,
            Amount = source.Amount,
            Note = source.Note,
            UpdateHistory = source.UpdateHistory
    WHEN NOT MATCHED THEN
        INSERT (EmployeeCode, DeductionTypeCode, DeductionMonth, DeductionYear, DeductionDate, Amount, Note, UpdateHistory)
        VALUES (source.EmployeeCode, source.DeductionTypeCode, source.DeductionMonth, source.DeductionYear, source.DeductionDate, source.Amount, source.Note, source.UpdateHistory)
    OUTPUT INSERTED.EmployeeDeductionID;
END
GO