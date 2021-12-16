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
    [TriggeringMagnitudeBand1] [float] NULL,
    [TriggeringMagnitudeBand2] [float] NULL,
    [TriggeringMagnitudeBand3] [float] NULL,
    [TriggeringMagnitudeBand4] [float] NULL,
    [MaximumMagnitudeBand1] [float] NULL,
    [MaximumMagnitudeBand2] [float] NULL,
    [MaximumMagnitudeBand3] [float] NULL,
    [MaximumMagnitudeBand4] [float] NULL,
    [AverageMagnitudeBand1] [float] NULL,
    [AverageMagnitudeBand2] [float] NULL,
    [AverageMagnitudeBand3] [float] NULL,
    [AverageMagnitudeBand4] [float] NULL,
    [Notes] [varchar](max) NULL,
    CONSTRAINT [PK_OscEvents] PRIMARY KEY CLUSTERED
    ( [ID] ASC ) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[OscEvents] WITH CHECK ADD CONSTRAINT [FK_OscEvents_OscEvents] FOREIGN KEY([ParentID])
REFERENCES [dbo].[OscEvents] ([ID])
GO