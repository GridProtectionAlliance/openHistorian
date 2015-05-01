-- TODO: Set proper target database name
ALTER DATABASE [MyDatabase] SET TRUSTWORTHY ON
EXEC sp_configure 'clr enabled', 1
RECONFIGURE
GO

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'GetHistorianData')
   DROP FUNCTION [GetHistorianData];
GO

IF EXISTS (SELECT name FROM sys.assemblies WHERE name = 'openHistorian.SqlClr')
   DROP ASSEMBLY [openHistorian.SqlClr];
GO

IF EXISTS (SELECT name FROM sys.assemblies WHERE name = 'openHistorian.SqlClr.Core')
   DROP ASSEMBLY [openHistorian.SqlClr.Core];
GO

IF EXISTS (SELECT name FROM sys.assemblies WHERE name = 'GSF.SqlClr.SortedTreeStore')
   DROP ASSEMBLY [GSF.SqlClr.SortedTreeStore];
GO

-- TODO: Set proper paths for installed assemblies
CREATE ASSEMBLY [GSF.SqlClr.SortedTreeStore] AUTHORIZATION dbo FROM 'C:\Program Files\openHistorian\GSF.SqlClr.SortedTreeStore.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY [openHistorian.SqlClr.Core] AUTHORIZATION dbo FROM 'C:\Program Files\openHistorian\openHistorian.SqlClr.Core.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY [openHistorian.SqlClr] AUTHORIZATION dbo FROM 'C:\Program Files\openHistorian\openHistorian.SqlClr.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE FUNCTION GetHistorianData(@historianServer nvarchar(256), @instanceName nvarchar(256), @startTime datetime2, @stopTime datetime2, @channelIDs nvarchar(4000) = null)
RETURNS TABLE
(
   ChannelID bigint,
   Time datetime2,
   Value real
)
AS EXTERNAL NAME [openHistorian.SqlClr].SqlProcedures.GetHistorianData;
GO