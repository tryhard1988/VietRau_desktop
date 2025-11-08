use QL_Kho_History

SELECT *
FROM ExportHistory

DELETE FROM ExportHistory WHERE ExportHistoryID=1

INSERT INTO Roles (RoleName, RoleCode)
VALUES (N'Chấm Công', 'cc');

UPDATE Roles
SET RoleName = N'Xem Thông Kê Nhân Sự Và Kho'
WHERE RoleCode = 'tknsvk';

USE QL_User;
GO
EXEC sp_spaceused;