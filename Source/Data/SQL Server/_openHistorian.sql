-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW [dbo].[LocalSchemaVersion] AS
SELECT 2 AS VersionNumber
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


CREATE TABLE [dbo].[EventMarker](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [ParentID] [int] NULL,
    [Source] [varchar](200) NULL,
    [StartTime] [datetime] NULL,
    [StopTime] [datetime] NULL,
    [Notes] [varchar](max) NULL,
    CONSTRAINT [PK_EventMarker] PRIMARY KEY CLUSTERED
    ( [ID] ASC ) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[EventMarker] WITH CHECK ADD CONSTRAINT [FK_EventMarker_EventMarker] FOREIGN KEY([ParentID])
REFERENCES [dbo].[EventMarker] ([ID])
GO