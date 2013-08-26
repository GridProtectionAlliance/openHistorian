sp_configure 'show advanced options', 1;
GO
RECONFIGURE;
GO
sp_configure 'clr enabled', 1;
GO
RECONFIGURE;
GO
ALTER DATABASE TestDatabase SET TRUSTWORTHY ON
GO
USE TestDatabase
GO
CREATE ASSEMBLY GSF_Unmanaged from 'C:\SQLDLLS\GSF.Unmanaged.dll' WITH PERMISSION_SET = UNSAFE