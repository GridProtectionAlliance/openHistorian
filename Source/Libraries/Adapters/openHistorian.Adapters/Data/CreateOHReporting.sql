CREATE DATABASE OHReporting
GO

USE OHReporting
GO

CREATE TABLE SNRSummary (
[SignalID] [uniqueidentifier] NOT NULL,
[Date] [date] NOT NULL,
[Count] [int] DEFAULT(0) NOT NULL,
[Sum] [float] DEFAULT(0) NOT NULL,
[SquaredSum] [float] DEFAULT(0) NOT NULL,
[Minimum] [float] DEFAULT(0) NOT NULL,
[Maximum] [float] DEFAULT(0) NOT NULL
) ON [PRIMARY]
GO

/* Create Indices on Date and SignalID for Faster Reporting */
CREATE CLUSTERED INDEX TimeIndex
ON SNRSummary (Date)
GO

/* This is neccesarry because SQL Does not allow inserts with Collumns that don't exist in the actual Table */
CREATE VIEW SNR AS
SELECT 
	SignalID,
	Date,
	0 AS SNR
	FROM SNRSummary WHERE 0 > 1
GO

/* Trigger for Insert to Update Existing Data if possible First */
CREATE TRIGGER TR_INSERT_SNR ON SNR
INSTEAD OF INSERT AS 
BEGIN
SELECT * INTO #Insert FROM INSERTED WHERE (SELECT COUNT(Sum) FROM SNRSummary WHERE SNRSummary.SignalID = INSERTED.SignalID AND SNRSummary.Date = INSERTED.Date) = 0;
SELECT * INTO #Duplicate FROM #Insert I1 WHERE (SELECT COUNT (SNR) FROM #Insert I2 WHERE I2.SignalID = I1.SignalID AND I2.Date = I1.Date) > 1;
DELETE #INSERT WHERE #Insert.SignalID in (SELECT SignalID FROM #Duplicate) AND #Insert.Date IN (SELECT Date FROM #Duplicate WHERE #Duplicate.signalID = #Insert.SignalID);

/* Remove Duplicates form INSERT*/
SELECT * INTO #Update FROM INSERTED WHERE (SELECT COUNT(Sum) FROM SNRSummary WHERE SNRSummary.SignalID = INSERTED.SignalID AND SNRSummary.Date = INSERTED.Date) > 0;
SELECT COUNT(#Update.SNR) AS CT, SUM(#Update.SNR) AS SM,SUM(#Update.SNR*#Update.SNR) AS SSM, MIN(#Update.SNR) AS Min, MAX(#Update.SNR) AS Max, #Update.Date, #Update.SignalID INTO #UpdateGrp 
	FROM #Update GROUP BY #Update.Date, #Update.SignalID;

INSERT INTO SNRSummary (SignalId,Date, Count, Sum, SquaredSum, Minimum, Maximum) SELECT
		#Insert.SignalID, #Insert.Date, 1, #Insert.SNR,(#Insert.SNR*#Insert.SNR),#Insert.SNR,#Insert.SNR FROM #Insert

UPDATE SNRSummary SET 
	Count = Count + CT,
	Sum = Sum + SM, 
	SquaredSum = SquaredSum + SSM, 
	Minimum = (CASE WHEN Minimum > Min THEN Min ELSE Minimum END), 
	Maximum= (CASE WHEN Maximum < Max THEN Max ELSE Maximum END)
	FROM #UpdateGrp WHERE (SNRSummary.Date = #UpdateGrp.Date AND SNRSummary.SignalID = #UpdateGrp.SignalID);


INSERT INTO SNRSummary (SignalId,Date, Count, Sum, SquaredSum, Minimum, Maximum) SELECT
		#Duplicate.SignalID, #Duplicate.Date, COUNT(#Duplicate.SNR), SUM(#Duplicate.SNR),SUM(#Duplicate.SNR*#Duplicate.SNR),MIN(#Duplicate.SNR),Max(#Duplicate.SNR) FROM #Duplicate GROUP BY #Duplicate.Date, #Duplicate.SignalID

END
GO


CREATE TABLE UnbalanceSummary (
[PositivePhaseSignalID] [uniqueidentifier] NOT NULL,
[Date] [date] NOT NULL,
[SignalType] VARCHAR(1) NOT NULL,
[Count] [int] DEFAULT(0) NOT NULL,
[AlarmCount] [int] DEFAULT(0) NOT NULL,
[Sum] [float] DEFAULT(0) NOT NULL,
[SquaredSum] [float] DEFAULT(0) NOT NULL,
[Minimum] [float] DEFAULT(0) NOT NULL,
[Maximum] [float] DEFAULT(0) NOT NULL,
[AlarmActiveCount] [int] DEFAULT(0) NOT NULL,
) ON [PRIMARY]
GO

/* Create Indices on Date and SignalID for Faster Reporting */
CREATE CLUSTERED INDEX TimeIndex
ON UnbalanceSummary (Date)
GO

/* This is neccesarry because SQL Does not allow inserts with Collumns that don't exist in the actual Table */
CREATE VIEW Unbalance AS
SELECT 
	SignalType,
	PositivePhaseSignalID,
	Date,
	Sum AS S0S1,
	AlarmCount AS Alarm,
	AlarmActiveCount AS ActiveAlarm
	FROM UnbalanceSummary WHERE 0 > 1
GO

/* Trigger for Insert to Update Existing Data if possible First */
CREATE TRIGGER TR_INSERT_Unbalance ON Unbalance
INSTEAD OF INSERT AS 
BEGIN
SELECT * INTO #Insert FROM INSERTED WHERE (SELECT COUNT(Sum) FROM UnbalanceSummary WHERE UnbalanceSummary.PositivePhaseSignalID = INSERTED.PositivePhaseSignalID AND UnbalanceSummary.Date = INSERTED.Date) = 0;
SELECT * INTO #Duplicate FROM #Insert I1 WHERE (SELECT COUNT (S0S1) FROM #Insert I2 WHERE I2.PositivePhaseSignalID = I1.PositivePhaseSignalID AND I2.Date = I1.Date) > 1;
DELETE #INSERT WHERE #Insert.PositivePhaseSignalID in (SELECT PositivePhaseSignalID FROM #Duplicate) AND #Insert.Date IN (SELECT Date FROM #Duplicate WHERE #Duplicate.PositivePhaseSignalID = #Insert.PositivePhaseSignalID);

/* Remove Duplicates form INSERT*/
SELECT * INTO #Update FROM INSERTED WHERE (SELECT COUNT(Sum) FROM UnbalanceSummary WHERE UnbalanceSummary.PositivePhaseSignalID = INSERTED.PositivePhaseSignalID AND UnbalanceSummary.Date = INSERTED.Date) > 0;
	
SELECT COUNT(#Update.S0S1) AS CT, SUM(#Update.S0S1) AS SM,SUM(#Update.S0S1*#Update.S0S1) AS SSM, MIN(#Update.S0S1) AS Min, MAX(#Update.S0S1) AS Max, #Update.Date, #Update.PositivePhaseSignalID, MAX(#Update.SignalType) AS STId , SUM(#Update.Alarm) AS AC, SUM(#Update.ActiveAlarm) AS AAC INTO #UpdateGrp 
	FROM #Update GROUP BY #Update.Date, #Update.PositivePhaseSignalID;

INSERT INTO UnbalanceSummary (PositivePhaseSignalID,Date, Count, Sum, SquaredSum, Minimum, Maximum,AlarmCount,SignalType,AlarmActiveCount) SELECT
		#Insert.PositivePhaseSignalID, #Insert.Date, 1, #Insert.S0S1,(#Insert.S0S1*#Insert.S0S1),#Insert.S0S1,#Insert.S0S1, #Insert.Alarm, #Insert.SignalType, #Insert.ActiveAlarm FROM #Insert

UPDATE UnbalanceSummary SET 
	Count = Count + CT,
	Sum = Sum + SM, 
	SquaredSum = SquaredSum + SSM, 
	Minimum = (CASE WHEN Minimum > Min THEN Min ELSE Minimum END), 
	Maximum= (CASE WHEN Maximum < Max THEN Max ELSE Maximum END),
	AlarmCount = AlarmCount + AC,
	AlarmActiveCount = AlarmActiveCount + AAC
	FROM #UpdateGrp WHERE (UnbalanceSummary.Date = #UpdateGrp.Date AND UnbalanceSummary.PositivePhaseSignalID = #UpdateGrp.PositivePhaseSignalID);


INSERT INTO UnbalanceSummary (PositivePhaseSignalID,Date, Count, Sum, SquaredSum, Minimum, Maximum,AlarmCount,SignalType,AlarmActiveCount) SELECT
		#Duplicate.PositivePhaseSignalID, #Duplicate.Date, COUNT(#Duplicate.S0S1), SUM(#Duplicate.S0S1),SUM(#Duplicate.S0S1*#Duplicate.S0S1),MIN(#Duplicate.S0S1),Max(#Duplicate.S0S1), SUM(#Duplicate.Alarm), MAX(#Duplicate.SignalType),SUM(#Duplicate.ActiveAlarm) FROM #Duplicate GROUP BY #Duplicate.Date, #Duplicate.PositivePhaseSignalID

END
GO


CREATE VIEW ActiveMeasurement AS 
SELECT * FROM openHistorianv26.dbo.ActiveMeasurement