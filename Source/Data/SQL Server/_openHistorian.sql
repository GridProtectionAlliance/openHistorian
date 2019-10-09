-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW [dbo].[LocalSchemaVersion] AS
SELECT 1 AS VersionNumber
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompressionSetting](
    [PointID] [int] NOT NULL,
    [CompressionMinTime] [bigint] NOT NULL DEFAULT ((0)),
    [CompressionMaxTime] [bigint] NOT NULL DEFAULT ((0)),
    [CompressionLimit] [float] NOT NULL DEFAULT ((0.0)),
 CONSTRAINT [PK_CompressionSetting] PRIMARY KEY CLUSTERED 
(
    [PointID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE VIEW NodeCompressionSetting AS
SELECT
    Node.ID AS NodeID,
    CompressionSetting.PointID,
    CompressionSetting.CompressionMinTime,
    CompressionSetting.CompressionMaxTime,
    CompressionSetting.CompressionLimit
FROM CompressionSetting CROSS JOIN Node
GO

-- **************************
-- SNR and Unbalance Report
-- **************************
CREATE VIEW SNRMeasurment AS
SELECT 
	   [NodeID]
      ,[SourceNodeID]
      ,[ID]
      ,[SignalID]
      ,[PointTag]
      ,[AlternateTag]
      ,[SignalReference]
      ,[Internal]
      ,[Subscribed]
      ,[Device]
      ,[DeviceID]
      ,[FramesPerSecond]
      ,[Protocol]
      ,[ProtocolType]
      ,[SignalType]
      ,[EngineeringUnits]
      ,[PhasorID]
      ,[PhasorType]
      ,[Phase]
      ,[Adder]
      ,[Multiplier]
      ,[Company]
      ,[Longitude]
      ,[Latitude]
      ,[Description]
      ,[UpdatedOn]
	  ,(CASE WHEN ([SignalReference] LIKE '%-SNR') THEN (SELECT 0) ELSE (CASE WHEN ([PointTag] LIKE '%I-UBAL') THEN (SELECT 1) ELSE (SELECT 2) END) END) AS [UnbalanceFlag]
	  FROM ActiveMeasurement
	  WHERE (SignalReference LIKE '%-SNR' OR SignalReference LIKE '%-UBAL') AND SignalType = 'CALC'
GO