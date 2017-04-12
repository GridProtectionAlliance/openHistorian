-- TODO: Set proper target database name
ALTER DATABASE [openHistorian] SET TRUSTWORTHY ON
EXEC sp_configure 'clr enabled', 1
RECONFIGURE
GO

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'MakeQuadWord')
   DROP FUNCTION [MakeQuadWord];
GO

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'LowDoubleWord')
   DROP FUNCTION [LowDoubleWord];
GO

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'HighDoubleWord')
   DROP FUNCTION [HighDoubleWord];
GO

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'GetHistorianData')
   DROP FUNCTION [GetHistorianData];
GO

IF EXISTS (SELECT name FROM sysobjects WHERE name = 'GetHistorianDataSampled')
   DROP FUNCTION [GetHistorianDataSampled];
GO

IF EXISTS (SELECT name FROM sys.assemblies WHERE name = 'openHistorian.SqlClr')
   DROP ASSEMBLY [openHistorian.SqlClr];
GO

IF EXISTS (SELECT name FROM sys.assemblies WHERE name = 'openHistorian.Core.SqlClr')
   DROP ASSEMBLY [openHistorian.Core.SqlClr];
GO

IF EXISTS (SELECT name FROM sys.assemblies WHERE name = 'GSF.SortedTreeStore.SqlClr')
   DROP ASSEMBLY [GSF.SortedTreeStore.SqlClr];
GO

IF EXISTS (SELECT name FROM sys.assemblies WHERE name = 'GSF.Core.SqlClr')
   DROP ASSEMBLY [GSF.Core.SqlClr];
GO

-- TODO: Set proper paths for installed assemblies
CREATE ASSEMBLY [GSF.Core.SqlClr] AUTHORIZATION dbo FROM 'C:\Program Files\openHistorian\SqlClr\GSF.Core.SqlClr.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY [GSF.SortedTreeStore.SqlClr] AUTHORIZATION dbo FROM 'C:\Program Files\openHistorian\SqlClr\GSF.SortedTreeStore.SqlClr.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY [openHistorian.Core.SqlClr] AUTHORIZATION dbo FROM 'C:\Program Files\openHistorian\SqlClr\openHistorian.Core.SqlClr.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY [openHistorian.SqlClr] AUTHORIZATION dbo FROM 'C:\Program Files\openHistorian\SqlClr\openHistorian.SqlClr.dll'
WITH PERMISSION_SET = UNSAFE
GO

-- Queries measurement data from the openHistorian
CREATE FUNCTION GetHistorianData(@historianServer nvarchar(256), @instanceName nvarchar(256), @startTime datetime2, @stopTime datetime2, @measurementIDs nvarchar(MAX))
RETURNS TABLE
(
   [ID] bigint,
   [Time] datetime2,
   [Value] real
)
AS EXTERNAL NAME [openHistorian.SqlClr].HistorianFunctions.GetHistorianData;
GO

CREATE FUNCTION GetHistorianDataSampled(@historianServer nvarchar(256), @instanceName nvarchar(256), @startTime datetime2, @stopTime datetime2, @interval time, @measurementIDs nvarchar(MAX))
RETURNS TABLE
(
   [ID] bigint,
   [Time] datetime2,
   [Value] real
)
AS EXTERNAL NAME [openHistorian.SqlClr].HistorianFunctions.GetHistorianDataSampled;
GO

-- Returns the unsigned high-double-word (int) from a quad-word (bigint)
CREATE FUNCTION HighDoubleWord(@quadWord bigint)
RETURNS int
AS EXTERNAL NAME [openHistorian.SqlClr].HistorianFunctions.HighDoubleWord;
GO

-- Returns the low-double-word (int) from a quad-word (bigint)
CREATE FUNCTION LowDoubleWord(@quadWord bigint)
RETURNS int
AS EXTERNAL NAME [openHistorian.SqlClr].HistorianFunctions.LowDoubleWord;
GO

-- Makes a quad-word (bigint) from two double-words (int)
CREATE FUNCTION MakeQuadWord(@highWord int, @lowWord int)
RETURNS bigint
AS EXTERNAL NAME [openHistorian.SqlClr].HistorianFunctions.MakeQuadWord;
GO
