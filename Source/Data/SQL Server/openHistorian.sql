-- =============================================================================
-- openHistorian Data Structures for SQL Server 
--
-- Grid Protection Alliance, 2010
-- Copyright © 2010.  All Rights Reserved.
--
-- Mihir Brahmbhatt
-- 10/14/2010
-- =============================================================================

USE [master]
GO
CREATE DATABASE [openHistorian];
GO
ALTER DATABASE [openHistorian] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [openHistorian] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [openHistorian] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [openHistorian] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [openHistorian] SET ARITHABORT OFF 
GO
ALTER DATABASE [openHistorian] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [openHistorian] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [openHistorian] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [openHistorian] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [openHistorian] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [openHistorian] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [openHistorian] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [openHistorian] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [openHistorian] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [openHistorian] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [openHistorian] SET  ENABLE_BROKER 
GO
ALTER DATABASE [openHistorian] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [openHistorian] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [openHistorian] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [openHistorian] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [openHistorian] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [openHistorian] SET  READ_WRITE 
GO
ALTER DATABASE [openHistorian] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [openHistorian] SET  MULTI_USER 
GO
ALTER DATABASE [openHistorian] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [openHistorian] SET DB_CHAINING OFF 
GO
-- The next three commented statements are used to create a user with access to the openHistorian database.
-- Be sure to change the username and password.
-- Replace-all from NewUser to the desired username is the preferred method of changing the username.

--IF  NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'NewUser')
--CREATE LOGIN [NewUser] WITH PASSWORD=N'MyPassword', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
--GO
USE [openHistorian]
GO
--CREATE USER [NewUser] FOR LOGIN [NewUser]
--GO
CREATE ROLE [openHistorianManagerRole] AUTHORIZATION [dbo]
GO
--EXEC sp_addrolemember N'openHistorianManagerRole', N'NewUser'
--GO
EXEC sp_addrolemember N'db_datareader', N'openHistorianManagerRole'
GO
EXEC sp_addrolemember N'db_datawriter', N'openHistorianManagerRole'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLog](
      [ID] [int] IDENTITY(1,1) NOT NULL,
      [Source] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
      [Message] [varchar](1024) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
      [Detail] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
      [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_ErrorLog_CreatedOn]  DEFAULT (getdate()),
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
      [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Company](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Acronym] [nvarchar](50) NOT NULL,
	[MapAcronym] [nchar](3) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[URL] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_Company_LoadOrder]  DEFAULT ((0)),
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ConfigurationEntity](
	[SourceName] [nvarchar](100) NOT NULL,
	[RuntimeName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_ConfigurationEntity_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_ConfigurationEntity_Enabled]  DEFAULT ((0))
) ON [PRIMARY]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Node](
	[ID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Node_ID]  DEFAULT (newid()),
	[Name] [nvarchar](100) NOT NULL,
	[CompanyID] [int] NULL,
	[Longitude] [decimal](9, 6) NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Description] [nvarchar](max) NULL,
	[ImagePath] [nvarchar](max) NULL,
	--[TimeSeriesDataServiceUrl] [nvarchar](max) NULL,
	--[RemoteStatusServiceUrl] [nvarchar](max) NULL,
	--[RealTimeStatisticServiceUrl] [nvarchar](max) NULL,
	[Master] [bit] NOT NULL CONSTRAINT [DF_Node_Master]  DEFAULT ((0)),
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_Node_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_Node_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_Node] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [DataOperation](
	[NodeID] [uniqueidentifier] NULL,
	[Description] [nvarchar](max) NULL,
	[AssemblyName] [nvarchar](max) NOT NULL,
	[TypeName] [nvarchar](max) NOT NULL,
	[MethodName] [nvarchar](255) NOT NULL,
	[Arguments] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_DataOperation_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_DataOperation_Enabled]  DEFAULT ((0))
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Measurement](
	[NodeID] [uniqueidentifier] NULL,
	[Source] [nvarchar] (50) NOT NULL,
	[SignalID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Measurement_SignalID]  DEFAULT (newid()),
	[HistorianID] [int] NULL,
	[PointID] [int] IDENTITY(1,1) NOT NULL,
	[DeviceID] [int] NULL,
	[PointTag] [nvarchar](50) NOT NULL,
	[AlternateTag] [nvarchar](50) NULL,
	--[SignalTypeID] [int] NOT NULL,
	--[PhasorSourceIndex] [int] NULL,
	[SignalReference] [nvarchar](24) NOT NULL,
	[Adder] [float] NOT NULL CONSTRAINT [DF_Measurement_Adder]  DEFAULT ((0.0)),
	[Multiplier] [float] NOT NULL CONSTRAINT [DF_Measurement_Multiplier]  DEFAULT ((1.0)),
	[Description] [nvarchar](max) NULL,
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_Measurement_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_Measurement] PRIMARY KEY CLUSTERED 
(
	[SignalID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Measurement_PointID] ON [Measurement] 
(
	[PointID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Measurement_PointTag] ON [Measurement] 
(
	[PointTag] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
/*
CREATE UNIQUE NONCLUSTERED INDEX [IX_Measurement_SignalReference] ON [Measurement] 
(
	[SignalReference] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
*/
CREATE NONCLUSTERED INDEX [IX_Measurement_NodeID] ON [Measurement] 
(
	[NodeID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ActionAdapter](
	[NodeID] [uniqueidentifier] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AdapterName] [nvarchar](50) NOT NULL,
	[AssemblyName] [nvarchar](max) NOT NULL,
	[TypeName] [nvarchar](max) NOT NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_CustomActionAdapter_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_CustomActionAdapter_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_CustomActionAdapter] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [InputAdapter](
	[NodeID] [uniqueidentifier] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AdapterName] [nvarchar](50) NOT NULL,
	[AssemblyName] [nvarchar](max) NOT NULL,
	[TypeName] [nvarchar](max) NOT NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_CustomInputAdapter_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_CustomInputAdapter_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_CustomInputAdapter] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OutputAdapter](
	[NodeID] [uniqueidentifier] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AdapterName] [nvarchar](50) NOT NULL,
	[AssemblyName] [nvarchar](max) NOT NULL,
	[TypeName] [nvarchar](max) NOT NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[LoadOrder] [int] NOT NULL CONSTRAINT [DF_CustomOutputAdapter_LoadOrder]  DEFAULT ((0)),
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_CustomOutputAdapter_Enabled]  DEFAULT ((0)),
 CONSTRAINT [PK_CustomOutputAdapter] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[IaonTreeView] AS
SELECT     'Action Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ISNULL(ConnectionString, '') AS ConnectionString
FROM         dbo.ActionAdapter
UNION ALL
SELECT     'Input Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ISNULL(ConnectionString, '') AS ConnectionString
FROM         dbo.InputAdapter
UNION ALL
SELECT     'Output Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ISNULL(ConnectionString, '') AS ConnectionString
FROM         dbo.OutputAdapter
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [ActiveMeasurement]
AS
SELECT		NodeID, Source + ':' + CONVERT(NVARCHAR(10), PointID) AS ID, SignalID,
					PointTag, AlternateTag, SignalReference, Adder, Multiplier, Description
FROM		dbo.Measurement
WHERE		dbo.Measurement.Enabled <> 0
GO

ALTER TABLE [dbo].[Node]  WITH CHECK ADD  CONSTRAINT [FK_Node_Company] FOREIGN KEY([CompanyID])
REFERENCES [Company] ([ID])
GO

ALTER TABLE [dbo].[DataOperation]  WITH CHECK ADD  CONSTRAINT [FK_DataOperation_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO

ALTER TABLE [dbo].[Measurement]  WITH CHECK ADD  CONSTRAINT [FK_Measurement_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
--ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ActionAdapter]  WITH CHECK ADD  CONSTRAINT [FK_ActionAdapter_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO

ALTER TABLE [dbo].[InputAdapter]  WITH CHECK ADD  CONSTRAINT [FK_InputAdapter_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])
GO

ALTER TABLE [dbo].[OutputAdapter]  WITH CHECK ADD  CONSTRAINT [FK_OutputAdapter_Node] FOREIGN KEY([NodeID])
REFERENCES [Node] ([ID])

/*
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [GetFormattedMeasurements]
	@measurementSql NVARCHAR(max),
	@includeAdjustments BIT,
	@measurements NVARCHAR(max) OUTPUT
AS
	-- Fill the table variable with the rows for your result set
	DECLARE @measurementID INT
	DECLARE @archiveSource NVARCHAR(50)
	DECLARE @adder FLOAT
	DECLARE @multiplier FLOAT

	SET @measurements = ''

	CREATE TABLE #temp
	(
		[MeasurementID] INT,
		[ArchiveSource] NVARCHAR(50),
		[Adder] FLOAT,
		[Multiplier] FLOAT
	)

	INSERT INTO #temp EXEC sp_executesql @measurementSql

	DECLARE SelectedMeasurements CURSOR LOCAL FAST_FORWARD FOR SELECT * FROM #temp
	OPEN SelectedMeasurements

	-- Get first row from measurements SQL
	FETCH NEXT FROM SelectedMeasurements INTO @measurementID, @archiveSource, @adder, @multiplier

	-- Step through selected measurements
	WHILE @@FETCH_STATUS = 0
	BEGIN		
		IF LEN(@measurements) > 0
			SET @measurements = @measurements + ';'

		IF @includeAdjustments <> 0 AND (@adder <> 0.0 OR @multiplier <> 1.0)
			SET @measurements = @measurements + @archiveSource + ':' + @measurementID + ',' + @adder + ',' + @multiplier
		ELSE
			SET @measurements = @measurements + @archiveSource + ':' + @measurementID
		
		-- Get next row from measurements SQL
		FETCH NEXT FROM SelectedMeasurements INTO @measurementID, @archiveSource, @adder, @multiplier
	END

	CLOSE SelectedMeasurements
	DEALLOCATE SelectedMeasurements

	DROP TABLE #temp

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [FormatMeasurements] (@measurementSql NVARCHAR(max), @includeAdjustments BIT)
RETURNS NVARCHAR(max) 
AS
BEGIN
    DECLARE @measurements NVARCHAR(max) 

	SET @measurements = ''

	EXEC GetFormattedMeasurements @measurementSql, @includeAdjustments, @measurements

	IF LEN(@measurements) > 0
		SET @measurements = '{' + @measurements + '}'
	ELSE
		SET @measurements = NULL
		
	RETURN @measurements
END

GO
*/
