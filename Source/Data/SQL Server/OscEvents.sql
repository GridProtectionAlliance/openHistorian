/****** Object:  Table [dbo].[OscEvents]    Script Date: 12/14/2021 8:22:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OscEvents](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [ParentID] [int] NULL,
    [Source] [varchar](200) NULL,
    [StartTime] [datetime] NULL,
    [StopTime] [datetime] NULL,
    [FrequencyBand1] [float] NULL,
    [FrequencyBand2] [float] NULL,
    [FrequencyBand3] [float] NULL,
    [FrequencyBand4] [float] NULL,
    [MagnitudeBand1] [float] NULL,
    [MagnitudeBand2] [float] NULL,
    [MagnitudeBand3] [float] NULL,
    [MagnitudeBand4] [float] NULL,
    [Notes] [varchar](max) NULL,
    CONSTRAINT [PK_OscEvents] PRIMARY KEY CLUSTERED
    ( [ID] ASC ) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[OscEvents] WITH CHECK ADD CONSTRAINT [FK_OscEvents_OscEvents] FOREIGN KEY([ParentID])
REFERENCES [dbo].[OscEvents] ([ID])
GO