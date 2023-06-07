CREATE TABLE [dbo].[ThreePhaseSet](
	[PositiveSequence] [uniqueidentifier] NOT NULL,
	[NegativeSequence] [uniqueidentifier] NOT NULL,
	[ZeroSequence] [uniqueidentifier] NOT NULL,
	[S0S1] [uniqueidentifier] NOT NULL,
	[S2S1] [uniqueidentifier] NOT NULL,
	[SignalType] [varchar](1) NOT NULL
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[SNRMapping](
	[InputKey] [uniqueidentifier] NOT NULL,
	[OutputKey] [uniqueidentifier] NOT NULL
) ON [PRIMARY]
GO


