-- =============================================================================
-- openHistorian Data Structures for MySQL 
--
-- Grid Protection Alliance, 2010
-- Copyright © 2010.  All Rights Reserved.
--
-- Mihir Brahmbhatt
-- 10/14/2010
-- =============================================================================

-- Execute the following from the command prompt to create database:
-- 	  mysql -uroot -p <"openHistorian.sql"

CREATE DATABASE openHistorian CHARACTER SET = latin1;
USE openHistorian;

-- The following statements are used to create
-- a user with access to the database.
-- Be sure to change the username and password.
-- CREATE USER NewUser IDENTIFIED BY 'MyPassword';
-- GRANT SELECT, UPDATE, INSERT ON openHistorian.* TO NewUser;

CREATE TABLE ErrorLog(
	ID INT AUTO_INCREMENT NOT NULL,
	Source NVARCHAR(256) NOT NULL,
	Message NVARCHAR(1024) NOT NULL,
	Detail LONGTEXT NULL,
	CreatedOn DATETIME NOT NULL DEFAULT 0,
	CONSTRAINT PK_ErrorLog PRIMARY KEY (ID ASC)
);

CREATE TRIGGER UpdateErrorLogDatetime BEFORE INSERT ON ErrorLog FOR EACH ROW
SET NEW.CreatedOn = NOW();

CREATE TABLE Company(
	ID INT AUTO_INCREMENT NOT NULL,
	Acronym NVARCHAR(50) NOT NULL,
	MapAcronym NCHAR(3) NOT NULL,
	Name NVARCHAR(100) NOT NULL,
	URL LONGTEXT NULL,
	LoadOrder INT NOT NULL DEFAULT 0,
  CONSTRAINT PK_Company PRIMARY KEY (ID ASC)
);

CREATE TABLE ConfigurationEntity(
	SourceName NVARCHAR(100) NOT NULL,
	RuntimeName NVARCHAR(100) NOT NULL,
	Description LONGTEXT NULL,
	LoadOrder INT NOT NULL DEFAULT 0,
	Enabled TINYINT NOT NULL DEFAULT 0
);

CREATE TABLE Node(
	ID NCHAR(36) NULL,
	Name NVARCHAR(100) NOT NULL,
	CompanyID INT NULL,
	Longitude DECIMAL(9, 6) NULL,
	Latitude DECIMAL(9, 6) NULL,
	Description LONGTEXT NULL,
	ImagePath LONGTEXT NULL,
	TimeSeriesDataServiceUrl LONGTEXT NULL,
	RemoteStatusServiceUrl LONGTEXT NULL,
	RealTimeStatisticServiceUrl LONGTEXT NULL,
	Master TINYINT NOT NULL DEFAULT 0,
	LoadOrder INT NOT NULL DEFAULT 0,
	Enabled TINYINT NOT NULL DEFAULT 0,
	CONSTRAINT PK_Node PRIMARY KEY (ID ASC)
);

CREATE TRIGGER UpdateNodeGuid BEFORE INSERT ON Node FOR EACH ROW
SET NEW.ID = UUID();

CREATE TABLE DataOperation(
	NodeID NCHAR(36) NULL,
	Description LONGTEXT NULL,
	AssemblyName TEXT NOT NULL,
	TypeName TEXT NOT NULL,
	MethodName NVARCHAR(255) NOT NULL,
	Arguments LONGTEXT NULL,
	LoadOrder INT NOT NULL DEFAULT 0,
	Enabled TINYINT NOT NULL DEFAULT 0
);

CREATE TABLE Measurement(
	NodeID	INT NULL,
	Source NVARCHAR(50) NOT NULL,
	SignalID NCHAR(36) NULL,
	HistorianID INT NULL,
	PointID INT AUTO_INCREMENT NOT NULL,
	DeviceID INT NULL,
	PointTag NVARCHAR(50) NOT NULL,
	AlternateTag NVARCHAR(50) NULL,
	--SignalTypeID INT NOT NULL,
	--PhasorSourceIndex INT NULL,
	SignalReference NVARCHAR(24) NOT NULL,
	Adder DOUBLE NOT NULL DEFAULT 0.0,
	Multiplier DOUBLE NOT NULL DEFAULT 1.0,
	Description LONGTEXT NULL,
	Enabled TINYINT NOT NULL DEFAULT 0,
	CONSTRAINT PK_Measurement PRIMARY KEY (SignalID ASC),
	CONSTRAINT IX_Measurement UNIQUE KEY (PointID ASC),
	CONSTRAINT IX_Measurement_NodeID (NodeID ASC),
	CONSTRAINT IX_Measurement_PointTag UNIQUE KEY (PointTag ASC),
	CONSTRAINT IX_Measurement_SignalReference UNIQUE KEY (SignalReference ASC)
);

CREATE TRIGGER UpdateMeasurementGuid BEFORE INSERT ON Measurement FOR EACH ROW
SET NEW.SignalID = UUID();

CREATE TABLE ActionAdapter(
	NodeID NCHAR(36) NOT NULL,
	ID INT AUTO_INCREMENT NOT NULL,
	AdapterName NVARCHAR(50) NOT NULL,
	AssemblyName TEXT NOT NULL,
	TypeName TEXT NOT NULL,
	ConnectionString LONGTEXT NULL,
	LoadOrder INT NOT NULL DEFAULT 0,
	Enabled TINYINT NOT NULL DEFAULT 0,
	CONSTRAINT PK_ActionAdapter PRIMARY KEY (ID ASC)
);

CREATE TABLE InputAdapter(
	NodeID NCHAR(36) NOT NULL,
	ID INT AUTO_INCREMENT NOT NULL,
	AdapterName NVARCHAR(50) NOT NULL,
	AssemblyName TEXT NOT NULL,
	TypeName TEXT NOT NULL,
	ConnectionString LONGTEXT NULL,
	LoadOrder INT NOT NULL DEFAULT 0,
	Enabled TINYINT NOT NULL DEFAULT 0,
	CONSTRAINT PK_InputAdapter PRIMARY KEY (ID ASC)
);

CREATE TABLE OutputAdapter(
	NodeID NCHAR(36) NOT NULL,
	ID INT AUTO_INCREMENT NOT NULL,
	AdapterName NVARCHAR(50) NOT NULL,
	AssemblyName TEXT NOT NULL,
	TypeName TEXT NOT NULL,
	ConnectionString LONGTEXT NULL,
	LoadOrder INT NOT NULL DEFAULT 0,
	Enabled TINYINT NOT NULL DEFAULT 0,
	CONSTRAINT PK_OutputAdapter PRIMARY KEY (ID ASC)
);

ALTER TABLE Node ADD CONSTRAINT FK_Node_Company FOREIGN KEY(CompanyID) REFERENCES Company (ID);

--ALTER TABLE Measurement ADD CONSTRAINT FK_Measurement_Device FOREIGN KEY(DeviceID) REFERENCES Device (ID) ON DELETE CASCADE;

--ALTER TABLE Measurement ADD CONSTRAINT FK_Measurement_Historian FOREIGN KEY(HistorianID) REFERENCES Historian (ID);

--ALTER TABLE Measurement ADD CONSTRAINT FK_Measurement_SignalType FOREIGN KEY(SignalTypeID) REFERENCES SignalType (ID);

ALTER TABLE Measurement ADD CONSTRAINT FK_Measurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE DataOperation ADD CONSTRAINT FK_DataOperation_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE ActionAdapter ADD CONSTRAINT FK_ActionAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE InputAdapter ADD CONSTRAINT FK_InputAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputAdapter ADD CONSTRAINT FK_OutputAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

CREATE VIEW ActiveMeasurement
AS
SELECT NodeID, CONCAT_WS(':', Source, CAST(PointID AS CHAR)) AS ID, SignalID, PointTag, AlternateTag, SignalReference, Adder, Multiplier, Description
FROM Measurement
WHERE Measurement.Enabled <> 0;

CREATE VIEW IaonTreeView AS
SELECT     'Action Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, COALESCE(ConnectionString, '') AS ConnectionString
FROM         ActionAdapter
UNION ALL
SELECT     'Input Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, COALESCE(ConnectionString, '') AS ConnectionString
FROM         InputAdapter
UNION ALL
SELECT     'Output Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, COALESCE(ConnectionString, '') AS ConnectionString
FROM         OutputAdapter;
 
 /* View for openHistorian Manager APP */
CREATE VIEW NodeDetail
AS
SELECT N.ID, N.Name, N.CompanyID AS CompanyID, COALESCE(N.Longitude, 0) AS Longitude, COALESCE(N.Latitude, 0) AS Latitude, 
		COALESCE(N.Description, '') AS Description, COALESCE(N.ImagePath, '') AS ImagePath, N.Master, N.LoadOrder, N.Enabled, 
		COALESCE(N.TimeSeriesDataServiceUrl, '') AS TimeSeriesDataServiceUrl, COALESCE(N.RemoteStatusServiceUrl, '') AS RemoteStatusServiceUrl,	
		COALESCE(N.RealTimeStatisticServiceUrl, '') AS RealTimeStatisticServiceUrl, COALESCE(C.Name, '') AS CompanyName
FROM Node N LEFT JOIN Company C 
ON N.CompanyID = C.ID;
 /*--------------------------------------*/

/*
CREATE TRIGGER ActionAdapter_RuntimeSync_Insert AFTER INSERT ON ActionAdapter
FOR EACH ROW INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, N'CustomActionAdapter');

CREATE TRIGGER CustomActionAdapter_RuntimeSync_Delete BEFORE DELETE ON CustomActionAdapter
FOR EACH ROW DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = N'CustomActionAdapter';

CREATE TRIGGER CustomInputAdapter_RuntimeSync_Insert AFTER INSERT ON CustomInputAdapter
FOR EACH ROW INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, N'CustomInputAdapter');

CREATE TRIGGER CustomInputAdapter_RuntimeSync_Delete BEFORE DELETE ON CustomInputAdapter
FOR EACH ROW DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = N'CustomInputAdapter';

CREATE TRIGGER CustomOutputAdapter_RuntimeSync_Insert AFTER INSERT ON CustomOutputAdapter
FOR EACH ROW INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, N'CustomOutputAdapter');

CREATE TRIGGER CustomOutputAdapter_RuntimeSync_Delete BEFORE DELETE ON CustomOutputAdapter
FOR EACH ROW DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = N'CustomOutputAdapter';

*/


/*
CREATE FUNCTION StringToGuid(str CHAR(36)) RETURNS BINARY(16)
RETURN CONCAT(UNHEX(LEFT(str, 8)), UNHEX(MID(str, 10, 4)), UNHEX(MID(str, 15, 4)), UNHEX(MID(str, 20, 4)), UNHEX(RIGHT(str, 12)));

CREATE FUNCTION GuidToString(guid BINARY(16)) RETURNS CHAR(36) 
RETURN CONCAT(HEX(LEFT(guid, 4)), '-', HEX(MID(guid, 5, 2)), '-', HEX(MID(guid, 7, 2)), '-', HEX(MID(guid, 9, 2)), '-', HEX(RIGHT(guid, 6)));

CREATE FUNCTION NewGuid() RETURNS BINARY(16) 
RETURN StringToGuid(UUID());

DELIMITER $$
CREATE PROCEDURE GetFormattedMeasurements(measurementSql TEXT, includeAdjustments TINYINT, OUT measurements TEXT)
BEGIN
	DECLARE done INT DEFAULT 0;
	DECLARE measurementID INT;
	DECLARE archiveSource NVARCHAR(50);
	DECLARE adder FLOAT DEFAULT 0.0;
	DECLARE multiplier FLOAT DEFAULT 1.1;	
	DECLARE selectedMeasurements CURSOR FOR SELECT * FROM temp;
	DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

	CREATE TEMPORARY TABLE temp
	(
		MeasurementID INT,
		ArchiveSource NVARCHAR(50),
		Adder FLOAT,
		Multiplier FLOAT
	)
	TABLESPACE MEMORY;
	
	SET @insertSQL = CONCAT('INSERT INTO temp ', measurementSql);
	PREPARE stmt FROM @insertSQL;
	EXECUTE stmt;
	DEALLOCATE PREPARE stmt;

	OPEN selectedMeasurements;	
	SET measurements = '';
	
	-- Step through selected measurements
	REPEAT
		-- Get next row from measurements SQL
		FETCH selectedMeasurements INTO measurementID, archiveSource, adder, multiplier;

		IF NOT done THEN
			IF LENGTH(measurements) > 0 THEN
				SET measurements = CONCAT(measurements, ';');
			END IF;
			
			IF includeAdjustments <> 0 AND (adder <> 0.0 OR multiplier <> 1.0) THEN
				SET measurements = CONCAT(measurements, archiveSource, ':', measurementID, ',', adder, ',', multiplier);
			ELSE
				SET measurements = CONCAT(measurements, archiveSource, ':', measurementID);
			END IF;

		END IF;
	UNTIL done END REPEAT;

	CLOSE selectedMeasurements;
	DROP TABLE temp;
END$$
DELIMITER ;

DELIMITER $$
CREATE FUNCTION FormatMeasurements(measurementSql TEXT, includeAdjustments TINYINT)
RETURNS TEXT 
BEGIN
  DECLARE measurements TEXT; 

	CALL GetFormattedMeasurements(measurementSql, includeAdjustments, measurements);

	IF LENGTH(measurements) > 0 THEN
		SET measurements = CONCAT('{', measurements, '}');
	ELSE
		SET measurements = NULL;
	END IF;
		
	RETURN measurements;
END$$
DELIMITER ;
*/
