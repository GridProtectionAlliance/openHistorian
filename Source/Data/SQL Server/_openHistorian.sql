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
    Node.ID,
    CompressionSetting.PointID,
    CompressionSetting.CompressionMinTime,
    CompressionSetting.CompressionMaxTime,
    CompressionSetting.CompressionLimit
FROM CompressionSetting CROSS JOIN Node
GO
