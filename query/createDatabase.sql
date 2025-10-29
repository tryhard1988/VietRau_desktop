BACKUP DATABASE QLNV
TO DISK = N'C:\VR\QLNV.bak';

RESTORE FILELISTONLY FROM DISK = N'C:\VR\QLNV.bak';

RESTORE DATABASE QL_User
FROM DISK = N'C:\VR\QLNV.bak'
WITH MOVE N'RauVietDB' TO N'C:\SQLData\QL_User.mdf',
     MOVE N'RauVietDB_Log'  TO N'C:\SQLData\QL_User_History.ldf',
     REPLACE;


USE QLNV
Drop PROCEDURE sp_UpdateUserRoles

-- Nếu chưa có user thì tạo mới và map với login
CREATE USER ql_user FOR LOGIN ql_user;
GO

-- Cấp quyền truy cập (tối thiểu đọc + ghi)
EXEC sp_addrolemember N'db_datareader', N'ql_user';
EXEC sp_addrolemember N'db_datawriter', N'ql_user';
EXEC sp_addrolemember N'db_owner', N'ql_user';
-- (tuỳ chọn) nếu muốn user có toàn quyền trong DB này:
-- EXEC sp_addrolemember N'db_owner', N'qlnv_vr_history';
GO

USE SalaryLock;
SELECT name FROM sys.database_principals WHERE type = 'S';
