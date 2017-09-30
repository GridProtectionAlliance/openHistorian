--  ----------------------------------------------------------------------------------------------------
--  GSFSchema Data Structures for PostgreSQL - Gbtc
--
--  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
--
--  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
--  the NOTICE file distributed with this work for additional information regarding copyright ownership.
--  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
--  not use this file except in compliance with the License. You may obtain a copy of the License at:
--
--      http://www.opensource.org/licenses/MIT
--
--  Unless agreed to in writing, the subject software distributed under the License is distributed on an
--  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
--  License for the specific language governing permissions and limitations.
--
--  Schema Modification History:
--  ----------------------------------------------------------------------------------------------------
--  05/07/2011 - J. Ritchie Carroll
--       Generated original version of schema.
--  07/15/2011 - Stephen C. Wills
--       Translated MySQL script to SQLite.
--  03/27/2012 - prasanthgs
--       Added ExceptionLog table for keeping recent exceptions.
--  04/12/2012 - prasanthgs
--       Reworked as per the comments of codeplex reviewers.
--       Added new field Type to ErrorLog table. Removed ExceptionLog table.
--  05/02/2016 - Stephen C. Wills
--       Translated SQLite script to PostgreSQL.
--  ----------------------------------------------------------------------------------------------------

-- CREATE DATABASE GSFSchema;
-- \c gsfschema

-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW SchemaVersion AS
SELECT 6 AS VersionNumber;

CREATE EXTENSION "uuid-ossp";

CREATE FUNCTION pg_catalog.text(INTEGER) RETURNS TEXT STRICT IMMUTABLE LANGUAGE SQL AS 'SELECT textin(int4out($1));';
CREATE CAST (INTEGER AS TEXT) WITH FUNCTION pg_catalog.text(INTEGER) AS IMPLICIT;
COMMENT ON FUNCTION pg_catalog.text(INTEGER) IS 'convert integer to text';

CREATE TABLE ErrorLog(
    ID SERIAL NOT NULL PRIMARY KEY,
    Source VARCHAR(200) NOT NULL,
    Type VARCHAR(200) NULL,
    Message TEXT NOT NULL,
    Detail TEXT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Runtime(
    ID SERIAL NOT NULL PRIMARY KEY,
    SourceID INTEGER NOT NULL,
    SourceTable VARCHAR(200) NOT NULL,
    CONSTRAINT IX_Runtime UNIQUE (ID)
);

CREATE TABLE AuditLog(
    ID SERIAL NOT NULL PRIMARY KEY,
    TableName VARCHAR(200) NOT NULL,
    PrimaryKeyColumn VARCHAR(200) NOT NULL,
    PrimaryKeyValue TEXT NOT NULL,
    ColumnName VARCHAR(200) NOT NULL,
    OriginalValue TEXT,
    NewValue TEXT,
    Deleted SMALLINT NOT NULL DEFAULT 0,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Company(
    ID SERIAL NOT NULL PRIMARY KEY,
    Acronym VARCHAR(200) NOT NULL,
    MapAcronym NCHAR(10) NOT NULL,
    Name VARCHAR(200) NOT NULL,
    URL TEXT NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT ''
);

CREATE TABLE TrackedChange(
    ID SERIAL NOT NULL PRIMARY KEY,
    TableName VARCHAR(200) NOT NULL,
    PrimaryKeyColumn VARCHAR(200) NOT NULL,
    PrimaryKeyValue TEXT NULL
);

CREATE TABLE ConfigurationEntity(
    SourceName VARCHAR(200) NOT NULL,
    RuntimeName VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0
);

CREATE TABLE Vendor(
    ID SERIAL NOT NULL PRIMARY KEY,
    Acronym VARCHAR(200) NULL,
    Name VARCHAR(200) NOT NULL,
    PhoneNumber VARCHAR(200) NULL,
    ContactEmail VARCHAR(200) NULL,
    URL TEXT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT ''
);

CREATE TABLE Protocol(
    ID SERIAL NOT NULL PRIMARY KEY,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NOT NULL,
    Type VARCHAR(200) NOT NULL DEFAULT 'Frame',
    Category VARCHAR(200) NOT NULL DEFAULT 'Phasor',
    AssemblyName VARCHAR(1024) NOT NULL DEFAULT 'PhasorProtocolAdapters.dll',
    TypeName VARCHAR(200) NOT NULL DEFAULT 'PhasorProtocolAdapters.PhasorMeasurementMapper',
    LoadOrder INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE SignalType(
    ID SERIAL NOT NULL PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Acronym VARCHAR(4) NOT NULL,
    Suffix VARCHAR(2) NOT NULL,
    Abbreviation VARCHAR(2) NOT NULL,
    LongAcronym VARCHAR(200) NOT NULL DEFAULT 'Undefined',
    Source VARCHAR(10) NOT NULL,
    EngineeringUnits VARCHAR(10) NULL
);

CREATE TABLE Interconnection(
    ID SERIAL NOT NULL PRIMARY KEY,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NOT NULL,
    LoadOrder INTEGER NULL DEFAULT 0
);

CREATE TABLE Node(
    ID NCHAR(36) NOT NULL DEFAULT CAST(uuid_generate_v4() AS NCHAR(36)),
    Name VARCHAR(200) NOT NULL,
    CompanyID INTEGER NULL,
    Longitude DECIMAL(9, 6) NULL,
    Latitude DECIMAL(9, 6) NULL,
    Description TEXT NULL,
    ImagePath TEXT NULL,
    Settings TEXT NULL,
    MenuType VARCHAR(200) NOT NULL DEFAULT 'File',
    MenuData TEXT NOT NULL,
    Master SMALLINT NOT NULL DEFAULT 0,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT PK_Node PRIMARY KEY (ID),
    CONSTRAINT FK_Node_Company FOREIGN KEY(CompanyID) REFERENCES Company (ID),
    CONSTRAINT IX_NodeID_Name UNIQUE (Name)
);

CREATE TABLE DataOperation(
    NodeID NCHAR(36) NULL,
    Description TEXT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    MethodName VARCHAR(200) NOT NULL,
    Arguments TEXT NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CONSTRAINT FK_DataOperation_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE VendorDevice(
    ID SERIAL NOT NULL PRIMARY KEY,
    VendorID INTEGER NOT NULL DEFAULT 10,
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    URL TEXT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_VendorDevice_Vendor FOREIGN KEY(VendorID) REFERENCES Vendor (ID)
);

CREATE TABLE OtherDevice(
    ID SERIAL NOT NULL PRIMARY KEY,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    IsConcentrator SMALLINT NOT NULL DEFAULT 0,
    CompanyID INTEGER NULL,
    VendorDeviceID INTEGER NULL,
    Longitude DECIMAL(9, 6) NULL,
    Latitude DECIMAL(9, 6) NULL,
    InterconnectionID INTEGER NULL,
    Planned SMALLINT NOT NULL DEFAULT 0,
    Desired SMALLINT NOT NULL DEFAULT 0,
    InProgress SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_OtherDevice_Company FOREIGN KEY(CompanyID) REFERENCES Company (ID),
    CONSTRAINT FK_OtherDevice_Interconnection FOREIGN KEY(InterconnectionID) REFERENCES Interconnection (ID),
    CONSTRAINT FK_OtherDevice_VendorDevice FOREIGN KEY(VendorDeviceID) REFERENCES VendorDevice (ID)
);

CREATE TABLE Device(
    NodeID NCHAR(36) NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    ParentID INTEGER NULL,
    UniqueID NCHAR(36) NOT NULL DEFAULT CAST(uuid_generate_v4() AS NCHAR(36)),
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    OriginalSource VARCHAR(200) NULL,
    IsConcentrator SMALLINT NOT NULL DEFAULT 0,
    CompanyID INTEGER NULL,
    HistorianID INTEGER NULL,
    AccessID INTEGER NOT NULL DEFAULT 0,
    VendorDeviceID INTEGER NULL,
    ProtocolID INTEGER NULL,
    Longitude DECIMAL(9, 6) NULL,
    Latitude DECIMAL(9, 6) NULL,
    InterconnectionID INTEGER NULL,
    ConnectionString TEXT NULL,
    TimeZone VARCHAR(200) NULL,
    FramesPerSecond INTEGER NULL DEFAULT 30,
    TimeAdjustmentTicks BIGINT NOT NULL DEFAULT 0,
    DataLossInterval DOUBLE PRECISION NOT NULL DEFAULT 5,
    AllowedParsingExceptions INTEGER NOT NULL DEFAULT 10,
    ParsingExceptionWindow DOUBLE PRECISION NOT NULL DEFAULT 5,
    DelayedConnectionInterval DOUBLE PRECISION NOT NULL DEFAULT 5,
    AllowUseOfCachedConfiguration SMALLINT NOT NULL DEFAULT 1,
    AutoStartDataParsingSequence SMALLINT NOT NULL DEFAULT 1,
    SkipDisableRealTimeData SMALLINT NOT NULL DEFAULT 0,
    MeasurementReportingInterval INTEGER NOT NULL DEFAULT 100000,
    ConnectOnDemand SMALLINT NOT NULL DEFAULT 1,
    ContactList TEXT NULL,
    MeasuredLines INTEGER NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT IX_Device_UniqueID UNIQUE (UniqueID),
    CONSTRAINT IX_Device_NodeID_Acronym UNIQUE (NodeID, Acronym),
    CONSTRAINT FK_Device_Company FOREIGN KEY(CompanyID) REFERENCES Company (ID),
    CONSTRAINT FK_Device_Device FOREIGN KEY(ParentID) REFERENCES Device (ID),    
    CONSTRAINT FK_Device_Interconnection FOREIGN KEY(InterconnectionID) REFERENCES Interconnection (ID),
    CONSTRAINT FK_Device_Node FOREIGN KEY(NodeID) REFERENCES Node (ID),
    CONSTRAINT FK_Device_Protocol FOREIGN KEY(ProtocolID) REFERENCES Protocol (ID),
    CONSTRAINT FK_Device_VendorDevice FOREIGN KEY(VendorDeviceID) REFERENCES VendorDevice (ID)
);

CREATE TABLE OutputStream(
    NodeID NCHAR(36) NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    Type INTEGER NOT NULL DEFAULT 0,
    ConnectionString TEXT NULL,
    DataChannel TEXT NULL,
    CommandChannel TEXT NULL,
    IDCode INTEGER NOT NULL DEFAULT 0,
    AutoPublishConfigFrame SMALLINT NOT NULL DEFAULT 0,
    AutoStartDataChannel SMALLINT NOT NULL DEFAULT 1,
    NominalFrequency INTEGER NOT NULL DEFAULT 60,
    FramesPerSecond INTEGER NOT NULL DEFAULT 30,
    LagTime DOUBLE PRECISION NOT NULL DEFAULT 3.0,
    LeadTime DOUBLE PRECISION NOT NULL DEFAULT 1.0,
    UseLocalClockAsRealTime SMALLINT NOT NULL DEFAULT 0,
    AllowSortsByArrival SMALLINT NOT NULL DEFAULT 1,
    IgnoreBadTimeStamps SMALLINT NOT NULL DEFAULT 0,
    TimeResolution INTEGER NOT NULL DEFAULT 330000,
    AllowPreemptivePublishing SMALLINT NOT NULL DEFAULT 1,
    PerformTimeReasonabilityCheck SMALLINT NOT NULL DEFAULT 1,
    DownsamplingMethod VARCHAR(15) NOT NULL DEFAULT 'LastReceived',
    DataFormat VARCHAR(15) NOT NULL DEFAULT 'FloatingPoint',
    CoordinateFormat VARCHAR(15) NOT NULL DEFAULT 'Polar',
    CurrentScalingValue INTEGER NOT NULL DEFAULT 2423,
    VoltageScalingValue INTEGER NOT NULL DEFAULT 2725785,
    AnalogScalingValue INTEGER NOT NULL DEFAULT 1373291,
    DigitalMaskValue INTEGER NOT NULL DEFAULT -65536,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_OutputStream_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT IX_OutputStream_NodeID_Acronym UNIQUE (NodeID, Acronym)
);

CREATE TABLE OutputStreamDevice(
    NodeID NCHAR(36) NOT NULL,
    AdapterID INTEGER NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    IDCode INTEGER NOT NULL DEFAULT 0,
    Acronym VARCHAR(200) NOT NULL,
    BpaAcronym VARCHAR(4) NULL,
    Name VARCHAR(200) NOT NULL,
    PhasorDataFormat VARCHAR(15) NULL,
    FrequencyDataFormat VARCHAR(15) NULL,
    AnalogDataFormat VARCHAR(15) NULL,
    CoordinateFormat VARCHAR(15) NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_OutputStreamDevice_Node FOREIGN KEY(NodeID) REFERENCES Node (ID),
    CONSTRAINT FK_OutputStreamDevice_OutputStream FOREIGN KEY(AdapterID) REFERENCES OutputStream (ID) ON DELETE CASCADE
);

CREATE TABLE OutputStreamDeviceDigital(
    NodeID NCHAR(36) NOT NULL,
    OutputStreamDeviceID INTEGER NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    Label TEXT NOT NULL,
    MaskValue INTEGER NOT NULL DEFAULT 0,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_OutputStreamDeviceDigital_Node FOREIGN KEY(NodeID) REFERENCES Node (ID),
    CONSTRAINT FK_OutputStreamDeviceDigital_OutputStreamDevice FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE
);

CREATE TABLE OutputStreamDevicePhasor(
    NodeID NCHAR(36) NOT NULL,
    OutputStreamDeviceID INTEGER NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    Label VARCHAR(200) NOT NULL,
    Type NCHAR(1) NOT NULL DEFAULT 'V',
    Phase NCHAR(1) NOT NULL DEFAULT '+',
    ScalingValue INTEGER NOT NULL DEFAULT 0,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_OutputStreamDevicePhasor_Node FOREIGN KEY(NodeID) REFERENCES Node (ID),
    CONSTRAINT FK_OutputStreamDevicePhasor_OutputStreamDevice FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE
);

CREATE TABLE OutputStreamDeviceAnalog(
    NodeID NCHAR(36) NOT NULL,
    OutputStreamDeviceID INTEGER NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    Label VARCHAR(16) NOT NULL,
    Type INTEGER NOT NULL DEFAULT 0,
    ScalingValue INTEGER NOT NULL DEFAULT 0,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_OutputStreamDeviceAnalog_Node FOREIGN KEY(NodeID) REFERENCES Node (ID),
    CONSTRAINT FK_OutputStreamDeviceAnalog_OutputStreamDevice FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE
);

CREATE TABLE Measurement(
    PointID BIGSERIAL NOT NULL,
    SignalID NCHAR(36) NOT NULL PRIMARY KEY DEFAULT CAST(uuid_generate_v4() AS NCHAR(36)),
    HistorianID INTEGER NULL,
    DeviceID INTEGER NULL,
    PointTag VARCHAR(200) NOT NULL,
    AlternateTag TEXT NULL,
    SignalTypeID INTEGER NOT NULL,
    PhasorSourceIndex INTEGER NULL,
    SignalReference VARCHAR(200) NOT NULL,
    Adder DOUBLE PRECISION NOT NULL DEFAULT 0.0,
    Multiplier DOUBLE PRECISION NOT NULL DEFAULT 1.0,
    Description TEXT NULL,
    Subscribed SMALLINT NOT NULL DEFAULT 0,
    Internal SMALLINT NOT NULL DEFAULT 1,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT IX_Measurement UNIQUE (PointID),
    CONSTRAINT FK_Measurement_Device FOREIGN KEY(DeviceID) REFERENCES Device (ID) ON DELETE CASCADE,    
    CONSTRAINT FK_Measurement_SignalType FOREIGN KEY(SignalTypeID) REFERENCES SignalType (ID)
);

CREATE TABLE ImportedMeasurement(
    NodeID NCHAR(36) NULL,
    SourceNodeID NCHAR(36) NULL,
    SignalID NCHAR(36) NULL,
    Source VARCHAR(200) NOT NULL,
    PointID BIGINT NOT NULL,
    PointTag VARCHAR(200) NOT NULL,
    AlternateTag VARCHAR(200) NULL,
    SignalTypeAcronym VARCHAR(4) NULL,
    SignalReference TEXT NOT NULL,
    FramesPerSecond INTEGER NULL,
    ProtocolAcronym VARCHAR(200) NULL,
    ProtocolType VARCHAR(200) NOT NULL DEFAULT 'Frame',
    PhasorID INTEGER NULL,
    PhasorType NCHAR(1) NULL,
    Phase NCHAR(1) NULL,
    Adder DOUBLE PRECISION NOT NULL DEFAULT 0.0,
    Multiplier DOUBLE PRECISION NOT NULL DEFAULT 1.0,
    CompanyAcronym VARCHAR(200) NULL,
    Longitude DECIMAL(9, 6) NULL,
    Latitude DECIMAL(9, 6) NULL,
    Description TEXT NULL,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CONSTRAINT FK_ImportedMeasurement_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Statistic(
    ID SERIAL NOT NULL PRIMARY KEY,
    Source VARCHAR(20) NOT NULL,
    SignalIndex INTEGER NOT NULL,
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    MethodName VARCHAR(200) NOT NULL,
    Arguments TEXT NULL,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    DataType VARCHAR(200) NULL,
    DisplayFormat TEXT NULL,
    IsConnectedState SMALLINT NOT NULL DEFAULT 0,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT IX_Statistic_Source_SignalIndex UNIQUE (Source, SignalIndex)
);

CREATE TABLE Phasor(
    ID SERIAL NOT NULL PRIMARY KEY,
    DeviceID INTEGER NOT NULL,
    Label VARCHAR(200) NOT NULL,
    Type NCHAR(1) NOT NULL DEFAULT 'V',
    Phase NCHAR(1) NOT NULL DEFAULT '+',
    DestinationPhasorID INTEGER NULL,
    SourceIndex INTEGER NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_Phasor_Device FOREIGN KEY(DeviceID) REFERENCES Device (ID) ON DELETE CASCADE,
    CONSTRAINT FK_Phasor_Phasor FOREIGN KEY(DestinationPhasorID) REFERENCES Phasor (ID)
);

CREATE TABLE CalculatedMeasurement(
    NodeID NCHAR(36) NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    ConnectionString TEXT NULL,
    ConfigSection VARCHAR(200) NULL,
    InputMeasurements TEXT NULL,
    OutputMeasurements TEXT NULL,
    MinimumMeasurementsToUse INTEGER NOT NULL DEFAULT -1,
    FramesPerSecond INTEGER NOT NULL DEFAULT 30,
    LagTime DOUBLE PRECISION NOT NULL DEFAULT 3.0,
    LeadTime DOUBLE PRECISION NOT NULL DEFAULT 1.0,
    UseLocalClockAsRealTime SMALLINT NOT NULL DEFAULT 0,
    AllowSortsByArrival SMALLINT NOT NULL DEFAULT 1,
    IgnoreBadTimeStamps SMALLINT NOT NULL DEFAULT 0,
    TimeResolution INTEGER NOT NULL DEFAULT 10000,
    AllowPreemptivePublishing SMALLINT NOT NULL DEFAULT 1,
    PerformTimeReasonabilityCheck SMALLINT NOT NULL DEFAULT 1,
    DownsamplingMethod VARCHAR(15) NOT NULL DEFAULT 'LastReceived',
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_CalculatedMeasurement_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE CustomActionAdapter(
    NodeID NCHAR(36) NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    AdapterName VARCHAR(200) NOT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    ConnectionString TEXT NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_CustomActionAdapter_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Historian(
    NodeID NCHAR(36) NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    AssemblyName TEXT NULL,
    TypeName TEXT NULL,
    ConnectionString TEXT NULL,
    IsLocal SMALLINT NOT NULL DEFAULT 1,
    MeasurementReportingInterval INTEGER NOT NULL DEFAULT 100000,
    Description TEXT NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_Historian_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE OutputStreamMeasurement(
    NodeID NCHAR(36) NOT NULL,
    AdapterID INTEGER NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    HistorianID INTEGER NULL,
    PointID BIGINT NOT NULL,
    SignalReference VARCHAR(200) NOT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_OutputStreamMeasurement_Historian FOREIGN KEY(HistorianID) REFERENCES Historian (ID),
    CONSTRAINT FK_OutputStreamMeasurement_Measurement FOREIGN KEY(PointID) REFERENCES Measurement (PointID) ON DELETE CASCADE,
    CONSTRAINT FK_OutputStreamMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID),
    CONSTRAINT FK_OutputStreamMeasurement_OutputStream FOREIGN KEY(AdapterID) REFERENCES OutputStream (ID) ON DELETE CASCADE
);

CREATE TABLE CustomInputAdapter(
    NodeID NCHAR(36) NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    AdapterName VARCHAR(200) NOT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    ConnectionString TEXT NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_CustomInputAdapter_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE PowerCalculation(
    NodeID NCHAR(36) NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    CircuitDescription TEXT NULL,
    VoltageAngleSignalID NCHAR(36) NOT NULL,
    VoltageMagSignalID NCHAR(36) NOT NULL,
    CurrentAngleSignalID NCHAR(36) NOT NULL,
    CurrentMagSignalID NCHAR(36) NOT NULL,
    ActivePowerOutputSignalID NCHAR(36) NULL,
    ReactivePowerOutputSignalID NCHAR(36) NULL,
    ApparentPowerOutputSignalID NCHAR(36) NULL,
    Enabled SMALLINT NOT NULL,
    CONSTRAINT FK_PowerCalculation_Measurement1 FOREIGN KEY(ApparentPowerOutputSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_PowerCalculation_Measurement2 FOREIGN KEY(CurrentAngleSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_PowerCalculation_Measurement3 FOREIGN KEY(CurrentMagSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_PowerCalculation_Measurement4 FOREIGN KEY(ReactivePowerOutputSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_PowerCalculation_Measurement5 FOREIGN KEY(ActivePowerOutputSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_PowerCalculation_Measurement6 FOREIGN KEY(VoltageAngleSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_PowerCalculation_Measurement7 FOREIGN KEY(VoltageMagSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Alarm(
    NodeID NCHAR(36) NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    TagName VARCHAR(200) NOT NULL,
    SignalID NCHAR(36) NOT NULL,
    AssociatedMeasurementID NCHAR(36) NULL,
    Description TEXT NULL,
    Severity INTEGER NOT NULL,
    Operation INTEGER NOT NULL,
    SetPoint DOUBLE PRECISION NULL,
    Tolerance DOUBLE PRECISION NULL,
    Delay DOUBLE PRECISION NULL,
    Hysteresis DOUBLE PRECISION NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_Alarm_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_Alarm_Measurement_SignalID FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_Alarm_Measurement_AssociatedMeasurementID FOREIGN KEY(AssociatedMeasurementID) REFERENCES Measurement (SignalID)
);

CREATE TABLE AlarmLog(
    ID SERIAL NOT NULL PRIMARY KEY,
    SignalID NCHAR(36) NOT NULL,
    PreviousState INTEGER NULL,
    NewState INTEGER NULL,
    Ticks INTEGER NOT NULL,
    Timestamp TIMESTAMP NOT NULL,
    Value DOUBLE PRECISION NOT NULL,
    CONSTRAINT FK_AlarmLog_Measurement FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_AlarmLog_Alarm_PreviousState FOREIGN KEY(PreviousState) REFERENCES Alarm (ID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_AlarmLog_Alarm_NewState FOREIGN KEY(NewState) REFERENCES Alarm (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE CustomOutputAdapter(
    NodeID NCHAR(36) NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    AdapterName VARCHAR(200) NOT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    ConnectionString TEXT NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_CustomOutputAdapter_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE AccessLog (
    ID SERIAL NOT NULL PRIMARY KEY,
    UserName VARCHAR(200) NOT NULL,
    AccessGranted SMALLINT NOT NULL,	
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE UserAccount (
    ID NCHAR(36) NOT NULL DEFAULT CAST(uuid_generate_v4() AS NCHAR(36)),
    Name VARCHAR(200) NOT NULL,
    Password VARCHAR(200) DEFAULT NULL,
    FirstName VARCHAR(200) DEFAULT NULL,
    LastName VARCHAR(200) DEFAULT NULL,
    DefaultNodeID NCHAR(36) NOT NULL,
    Phone VARCHAR(200) DEFAULT NULL,
    Email VARCHAR(200) DEFAULT NULL,
    LockedOut SMALLINT NOT NULL DEFAULT 0,
    UseADAuthentication SMALLINT NOT NULL DEFAULT 1,
    ChangePasswordOn TIMESTAMP DEFAULT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT PK_UserAccount PRIMARY KEY (ID),
    CONSTRAINT IX_UserAccount UNIQUE (Name),
    CONSTRAINT FK_useraccount FOREIGN KEY (DefaultNodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE SecurityGroup (
    ID NCHAR(36) NOT NULL DEFAULT CAST(uuid_generate_v4() AS NCHAR(36)),
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT PK_SecurityGorup PRIMARY KEY (ID),
    CONSTRAINT IX_SecurityGorup UNIQUE (Name)
);

CREATE TABLE ApplicationRole (
    ID NCHAR(36) NOT NULL DEFAULT CAST(uuid_generate_v4() AS NCHAR(36)),
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    NodeID NCHAR(36) NOT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT 'Admin',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT 'Admin',
    CONSTRAINT PK_ApplicationRole PRIMARY KEY (ID),
    CONSTRAINT IX_ApplicationRole UNIQUE (NodeID, Name),
    CONSTRAINT FK_applicationrole FOREIGN KEY (NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE ApplicationRoleSecurityGroup (
    ApplicationRoleID NCHAR(36) NOT NULL,
    SecurityGroupID NCHAR(36) NOT NULL,
    CONSTRAINT FK_applicationrolesecuritygroup_applicationrole FOREIGN KEY (ApplicationRoleID) REFERENCES applicationrole (ID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_applicationrolesecuritygroup_securitygroup FOREIGN KEY (SecurityGroupID) REFERENCES securitygroup (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE ApplicationRoleUserAccount (
    ApplicationRoleID NCHAR(36) NOT NULL,
    UserAccountID NCHAR(36) NOT NULL,
    CONSTRAINT FK_applicationroleuseraccount_useraccount FOREIGN KEY (UserAccountID) REFERENCES useraccount (ID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_applicationroleuseraccount_applicationrole FOREIGN KEY (ApplicationRoleID) REFERENCES applicationrole (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE SecurityGroupUserAccount (
    SecurityGroupID NCHAR(36) NOT NULL,
    UserAccountID NCHAR(36) NOT NULL,
    CONSTRAINT FK_securitygroupuseraccount_useraccount FOREIGN KEY (UserAccountID) REFERENCES useraccount (ID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_securitygroupuseraccount_securitygroup FOREIGN KEY (SecurityGroupID) REFERENCES securitygroup (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

-- ----------------------------------------------------------------------------

CREATE TABLE Subscriber (
    NodeID NCHAR(36) NOT NULL,
    ID NCHAR(36) NOT NULL DEFAULT CAST(uuid_generate_v4() AS NCHAR(36)),
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    SharedSecret VARCHAR(200) NULL,
    AuthKey TEXT NULL,
    ValidIPAddresses TEXT NULL,
    RemoteCertificateFile VARCHAR(500) NULL,
    ValidPolicyErrors VARCHAR(200) NULL,
    ValidChainFlags VARCHAR(500) NULL,
    AccessControlFilter TEXT NULL,
    Enabled SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT PK_Subscriber PRIMARY KEY (NodeID, ID),
    CONSTRAINT IX_Subscriber_NodeID_Acronym UNIQUE (NodeID, Acronym),
    CONSTRAINT FK_Subscriber_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE SubscriberMeasurement(
    NodeID NCHAR(36) NOT NULL,
    SubscriberID NCHAR(36) NOT NULL,
    SignalID NCHAR(36) NOT NULL,
    Allowed SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT PK_SubscriberMeasurement PRIMARY KEY (NodeID, SubscriberID, SignalID),
    CONSTRAINT FK_SubscriberMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_SubscriberMeasurement_Measurement FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_SubscriberMeasurement_Subscriber FOREIGN KEY(NodeID, SubscriberID) REFERENCES Subscriber (NodeID, ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE MeasurementGroup (
    NodeID NCHAR(36) NOT NULL,
    ID SERIAL NOT NULL PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    FilterExpression TEXT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_MeasurementGroup_Node FOREIGN KEY(NodeID) REFERENCES node (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE SubscriberMeasurementGroup (
    NodeID NCHAR(36) NOT NULL,
    SubscriberID NCHAR(36) NOT NULL,
    MeasurementGroupID INTEGER NOT NULL,
    Allowed SMALLINT NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT PK_SubscriberMeasurementGroup PRIMARY KEY (NodeID, SubscriberID, MeasurementGroupID),
    CONSTRAINT FK_SubscriberMeasurementGroup_Node FOREIGN KEY(NodeID) REFERENCES Node (ID),
    CONSTRAINT FK_SubscriberMeasurementGroup_Subscriber FOREIGN KEY(NodeID, SubscriberID) REFERENCES Subscriber (NodeID, ID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_SubscriberMeasurementGroup_MeasurementGroup FOREIGN KEY(MeasurementGroupID) REFERENCES MeasurementGroup (ID)
);

CREATE TABLE MeasurementGroupMeasurement (
    NodeID NCHAR(36) NOT NULL,
    MeasurementGroupID INTEGER NOT NULL,
    SignalID NCHAR(36) NOT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT PK_MeasurementGroupMeasurement PRIMARY KEY (NodeID, MeasurementGroupID, SignalID),
    CONSTRAINT FK_MeasurementGroupMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID),
    CONSTRAINT FK_MeasurementGroupMeasurement_Measurement FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_MeasurementGroupMeasurement_MeasurementGroup FOREIGN KEY(MeasurementGroupID) REFERENCES MeasurementGroup (ID) ON DELETE CASCADE ON UPDATE CASCADE
);

-- ----------------------------------------------------------------------------

CREATE UNIQUE INDEX PK_Runtime ON Runtime (SourceID, SourceTable);

CREATE UNIQUE INDEX PK_Measurement ON Measurement (SignalID);

CREATE VIEW NodeInfo
AS
SELECT Node.ID AS NodeID, Node.Name, Company.Name AS CompanyName, Node.Longitude, Node.Latitude,
 Node.Description, Node.ImagePath, Node.Settings, Node.MenuType, Node.MenuData, Node.Master, Node.Enabled
FROM Node LEFT OUTER JOIN Company ON Node.CompanyID = Company.ID;

CREATE VIEW RuntimeOutputStreamMeasurement
AS
SELECT OutputStreamMeasurement.NodeID, Runtime.ID AS AdapterID, Historian.Acronym AS Historian, 
 OutputStreamMeasurement.PointID, OutputStreamMeasurement.SignalReference
FROM OutputStreamMeasurement LEFT OUTER JOIN
 Historian ON OutputStreamMeasurement.HistorianID = Historian.ID LEFT OUTER JOIN
 Runtime ON OutputStreamMeasurement.AdapterID = Runtime.SourceID AND Runtime.SourceTable = 'OutputStream'
ORDER BY OutputStreamMeasurement.HistorianID, OutputStreamMeasurement.PointID;

CREATE VIEW RuntimeHistorian
AS
SELECT Historian.NodeID, Runtime.ID, Historian.Acronym AS AdapterName,
 COALESCE(NULLIF(TRIM(Historian.AssemblyName), ''), 'HistorianAdapters.dll') AS AssemblyName, 
 COALESCE(NULLIF(TRIM(Historian.TypeName), ''), CASE WHEN IsLocal <> 0 THEN 'HistorianAdapters.LocalOutputAdapter' ELSE 'HistorianAdapters.RemoteOutputAdapter' END) AS TypeName, 
 COALESCE(Historian.ConnectionString || ';', '') ||
 COALESCE('instanceName=' || Historian.Acronym || ';', '') ||
 COALESCE('sourceids=' || Historian.Acronym || ';', '') ||
 COALESCE('measurementReportingInterval=' || CAST(Historian.MeasurementReportingInterval AS TEXT), '') AS ConnectionString
FROM Historian LEFT OUTER JOIN
 Runtime ON Historian.ID = Runtime.SourceID AND Runtime.SourceTable = 'Historian'
WHERE Historian.Enabled <> 0
ORDER BY Historian.LoadOrder;

CREATE VIEW RuntimeDevice
AS
SELECT Device.NodeID, Runtime.ID, Device.Acronym AS AdapterName, Protocol.AssemblyName, Protocol.TypeName,
 COALESCE(Device.ConnectionString || ';', '') ||
 COALESCE('isConcentrator=' || Device.IsConcentrator || ';', '') ||
 COALESCE('accessID=' || CAST(Device.AccessID AS TEXT) || ';', '') ||
 COALESCE('timeZone=' || Device.TimeZone || ';', '') ||
 COALESCE('timeAdjustmentTicks=' || Device.TimeAdjustmentTicks || ';', '') ||
 COALESCE('phasorProtocol=' || Protocol.Acronym || ';', '') ||
 COALESCE('dataLossInterval=' || Device.DataLossInterval || ';', '') ||
 COALESCE('allowedParsingExceptions=' || CAST(Device.AllowedParsingExceptions AS TEXT) || ';', '') ||
 COALESCE('parsingExceptionWindow=' || Device.ParsingExceptionWindow || ';', '') ||
 COALESCE('delayedConnectionInterval=' || Device.DelayedConnectionInterval || ';', '') ||
 COALESCE('allowUseOfCachedConfiguration=' || Device.AllowUseOfCachedConfiguration || ';', '') ||
 COALESCE('autoStartDataParsingSequence=' || Device.AutoStartDataParsingSequence || ';', '') ||
 COALESCE('skipDisableRealTimeData=' || Device.SkipDisableRealTimeData || ';', '') ||
 COALESCE('measurementReportingInterval=' || CAST(Device.MeasurementReportingInterval AS TEXT) || ';', '') ||
 COALESCE('connectOnDemand=' || Device.ConnectOnDemand, '') AS ConnectionString
FROM Device LEFT OUTER JOIN
 Protocol ON Device.ProtocolID = Protocol.ID LEFT OUTER JOIN
 Runtime ON Device.ID = Runtime.SourceID AND Runtime.SourceTable = 'Device'
WHERE (Device.Enabled <> 0 AND Device.ParentID IS NULL)
ORDER BY Device.LoadOrder;

CREATE VIEW RuntimeCustomOutputAdapter
AS
SELECT CustomOutputAdapter.NodeID, Runtime.ID, CustomOutputAdapter.AdapterName, 
 TRIM(CustomOutputAdapter.AssemblyName) AS AssemblyName, TRIM(CustomOutputAdapter.TypeName) AS TypeName, CustomOutputAdapter.ConnectionString
FROM CustomOutputAdapter LEFT OUTER JOIN
 Runtime ON CustomOutputAdapter.ID = Runtime.SourceID AND Runtime.SourceTable = 'CustomOutputAdapter'
WHERE (CustomOutputAdapter.Enabled <> 0)
ORDER BY CustomOutputAdapter.LoadOrder;

CREATE VIEW RuntimeInputStreamDevice
AS
SELECT Device.NodeID, Runtime_P.ID AS ParentID, Runtime.ID, Device.Acronym, Device.Name, Device.AccessID
FROM Device LEFT OUTER JOIN
 Runtime ON Device.ID = Runtime.SourceID AND Runtime.SourceTable = 'Device' LEFT OUTER JOIN
 Runtime AS Runtime_P ON Device.ParentID = Runtime_P.SourceID AND Runtime_P.SourceTable = 'Device'
WHERE (Device.IsConcentrator = 0) AND (Device.Enabled <> 0) AND (Device.ParentID IS NOT NULL)
ORDER BY Device.LoadOrder;

CREATE VIEW RuntimeCustomInputAdapter
AS
SELECT CustomInputAdapter.NodeID, Runtime.ID, CustomInputAdapter.AdapterName, 
 TRIM(CustomInputAdapter.AssemblyName) AS AssemblyName, TRIM(CustomInputAdapter.TypeName) AS TypeName, CustomInputAdapter.ConnectionString
FROM CustomInputAdapter LEFT OUTER JOIN
 Runtime ON CustomInputAdapter.ID = Runtime.SourceID AND Runtime.SourceTable = 'CustomInputAdapter'
WHERE (CustomInputAdapter.Enabled <> 0)
ORDER BY CustomInputAdapter.LoadOrder;

CREATE VIEW RuntimeOutputStreamDevice
AS
SELECT OutputStreamDevice.NodeID, Runtime.ID AS ParentID, OutputStreamDevice.ID, OutputStreamDevice.IDCode, OutputStreamDevice.Acronym, 
 OutputStreamDevice.BpaAcronym, OutputStreamDevice.Name, NULLIF(OutputStreamDevice.PhasorDataFormat, '') AS PhasorDataFormat, NULLIF(OutputStreamDevice.FrequencyDataFormat, '') AS FrequencyDataFormat,
 NULLIF(OutputStreamDevice.AnalogDataFormat, '') AS AnalogDataFormat, NULLIF(OutputStreamDevice.CoordinateFormat, '') AS CoordinateFormat, OutputStreamDevice.LoadOrder
FROM OutputStreamDevice LEFT OUTER JOIN
 Runtime ON OutputStreamDevice.AdapterID = Runtime.SourceID AND Runtime.SourceTable = 'OutputStream'
WHERE (OutputStreamDevice.Enabled <> 0)
ORDER BY OutputStreamDevice.LoadOrder;

CREATE VIEW RuntimeOutputStream
AS
SELECT OutputStream.NodeID, Runtime.ID, OutputStream.Acronym AS AdapterName, 
 'PhasorProtocolAdapters.dll'::TEXT AS AssemblyName, 
 CASE Type WHEN 1 THEN 'PhasorProtocolAdapters.BpaPdcStream.Concentrator' WHEN 2 THEN 'PhasorProtocolAdapters.Iec61850_90_5.Concentrator' ELSE 'PhasorProtocolAdapters.IeeeC37_118.Concentrator' END AS TypeName,
 COALESCE(OutputStream.ConnectionString || ';', '') ||
 COALESCE('dataChannel={' || OutputStream.DataChannel || '};', '') ||
 COALESCE('commandChannel={' || OutputStream.CommandChannel || '};', '') ||
 COALESCE('idCode=' || CAST(OutputStream.IDCode AS TEXT) || ';', '') ||
 COALESCE('autoPublishConfigFrame=' || OutputStream.AutoPublishConfigFrame || ';', '') ||
 COALESCE('autoStartDataChannel=' || OutputStream.AutoStartDataChannel || ';', '') ||
 COALESCE('nominalFrequency=' || CAST(OutputStream.NominalFrequency AS TEXT) || ';', '') ||
 COALESCE('lagTime=' || OutputStream.LagTime || ';', '') ||
 COALESCE('leadTime=' || OutputStream.LeadTime || ';', '') ||
 COALESCE('framesPerSecond=' || CAST(OutputStream.FramesPerSecond AS TEXT) || ';', '') ||
 COALESCE('useLocalClockAsRealTime=' || OutputStream.UseLocalClockAsRealTime || ';', '') ||
 COALESCE('allowSortsByArrival=' || OutputStream.AllowSortsByArrival || ';', '') ||
 COALESCE('ignoreBadTimestamps=' || OutputStream.IgnoreBadTimeStamps || ';', '') ||
 COALESCE('timeResolution=' || CAST(OutputStream.TimeResolution AS TEXT) || ';', '') ||
 COALESCE('allowPreemptivePublishing=' || OutputStream.AllowPreemptivePublishing || ';', '') ||
 COALESCE('downsamplingMethod=' || OutputStream.DownsamplingMethod || ';', '') ||
 COALESCE('dataFormat=' || OutputStream.DataFormat || ';', '') ||
 COALESCE('coordinateFormat=' || OutputStream.CoordinateFormat || ';', '') ||
 COALESCE('currentScalingValue=' || CAST(OutputStream.CurrentScalingValue AS TEXT) || ';', '') ||
 COALESCE('voltageScalingValue=' || CAST(OutputStream.VoltageScalingValue AS TEXT) || ';', '') ||
 COALESCE('analogScalingValue=' || CAST(OutputStream.AnalogScalingValue AS TEXT) || ';', '') ||
 COALESCE('performTimestampReasonabilityCheck=' || OutputStream.PerformTimeReasonabilityCheck || ';', '') ||
 COALESCE('digitalMaskValue=' || CAST(OutputStream.DigitalMaskValue AS TEXT), '') AS ConnectionString
FROM OutputStream LEFT OUTER JOIN
 Runtime ON OutputStream.ID = Runtime.SourceID AND Runtime.SourceTable = 'OutputStream'
WHERE (OutputStream.Enabled <> 0)
ORDER BY OutputStream.LoadOrder;

CREATE VIEW RuntimeCustomActionAdapter
AS
SELECT CustomActionAdapter.NodeID, Runtime.ID, CustomActionAdapter.AdapterName, 
 TRIM(CustomActionAdapter.AssemblyName) AS AssemblyName, TRIM(CustomActionAdapter.TypeName) AS TypeName, CustomActionAdapter.ConnectionString
FROM CustomActionAdapter LEFT OUTER JOIN
 Runtime ON CustomActionAdapter.ID = Runtime.SourceID AND Runtime.SourceTable = 'CustomActionAdapter'
WHERE (CustomActionAdapter.Enabled <> 0)
ORDER BY CustomActionAdapter.LoadOrder;

CREATE VIEW RuntimeCalculatedMeasurement
AS
SELECT CalculatedMeasurement.NodeID, Runtime.ID, CalculatedMeasurement.Acronym AS AdapterName, 
 TRIM(CalculatedMeasurement.AssemblyName) AS AssemblyName, TRIM(CalculatedMeasurement.TypeName) AS TypeName,
 COALESCE(CalculatedMeasurement.ConnectionString || ';', '') ||
 COALESCE('configurationSection=' || ConfigSection || ';', '') ||
 COALESCE('minimumMeasurementsToUse=' || CAST(CalculatedMeasurement.MinimumMeasurementsToUse AS TEXT) || ';', '') ||
 COALESCE('framesPerSecond=' || CAST(CalculatedMeasurement.FramesPerSecond AS TEXT) || ';', '') ||
 COALESCE('lagTime=' || CalculatedMeasurement.LagTime || ';', '') ||
 COALESCE('leadTime=' || CalculatedMeasurement.LeadTime || ';', '') ||
 COALESCE('inputMeasurementKeys={' || InputMeasurements || '};', '') ||
 COALESCE('outputMeasurements={' || OutputMeasurements || '};', '') ||
 COALESCE('ignoreBadTimestamps=' || CalculatedMeasurement.IgnoreBadTimeStamps || ';', '') ||
 COALESCE('timeResolution=' || CAST(CalculatedMeasurement.TimeResolution AS TEXT) || ';', '') ||
 COALESCE('allowPreemptivePublishing=' || CalculatedMeasurement.AllowPreemptivePublishing || ';', '') ||
 COALESCE('performTimestampReasonabilityCheck=' || CalculatedMeasurement.PerformTimeReasonabilityCheck || ';', '') ||
 COALESCE('downsamplingMethod=' || CalculatedMeasurement.DownsamplingMethod || ';', '') ||
 COALESCE('useLocalClockAsRealTime=' || CalculatedMeasurement.UseLocalClockAsRealTime, '') AS ConnectionString
FROM CalculatedMeasurement LEFT OUTER JOIN
 Runtime ON CalculatedMeasurement.ID = Runtime.SourceID AND Runtime.SourceTable = 'CalculatedMeasurement'
WHERE (CalculatedMeasurement.Enabled <> 0)
ORDER BY CalculatedMeasurement.LoadOrder;

CREATE VIEW ActiveMeasurement
AS
SELECT Node.ID AS NodeID,
 COALESCE(Device.NodeID, Historian.NodeID) AS SourceNodeID,
 COALESCE(Historian.Acronym, Device.Acronym, '__') || ':' || CAST(Measurement.PointID AS TEXT) AS ID,
 Measurement.SignalID, Measurement.PointTag, Measurement.AlternateTag, Measurement.SignalReference,
 Measurement.Internal, Measurement.Subscribed, Device.Acronym AS Device,
 CASE WHEN Device.IsConcentrator = 0 AND Device.ParentID IS NOT NULL THEN RuntimeP.ID ELSE Runtime.ID END AS DeviceID,
 COALESCE(Device.FramesPerSecond, 30) AS FramesPerSecond,
 Protocol.Acronym AS Protocol, Protocol.Type AS ProtocolType, Measurement.SignalType, Measurement.EngineeringUnits, Phasor.ID AS PhasorID,
 Phasor.Type AS PhasorType, Phasor.Phase, Measurement.Adder, Measurement.Multiplier,
 Device.CompanyAcronym AS Company, Device.Longitude, Device.Latitude, Measurement.Description, Measurement.UpdatedOn
FROM (SELECT Measurement.*, SignalType.Acronym AS SignalType, SignalType.EngineeringUnits AS EngineeringUnits FROM Measurement LEFT OUTER JOIN
     SignalType ON Measurement.SignalTypeID = SignalType.ID) AS Measurement LEFT OUTER JOIN
    (SELECT Device.*, Company.Acronym AS CompanyAcronym FROM Device LEFT OUTER JOIN
     Company ON Device.CompanyID = Company.ID) AS Device ON Device.ID = Measurement.DeviceID LEFT OUTER JOIN
    Phasor ON Measurement.DeviceID = Phasor.DeviceID AND Measurement.PhasorSourceIndex = Phasor.SourceIndex LEFT OUTER JOIN
    Protocol ON Device.ProtocolID = Protocol.ID LEFT OUTER JOIN
    Historian ON Measurement.HistorianID = Historian.ID LEFT OUTER JOIN
    Runtime ON Device.ID = Runtime.SourceID AND Runtime.SourceTable = 'Device' LEFT OUTER JOIN
    Runtime AS RuntimeP ON RuntimeP.SourceID = Device.ParentID AND RuntimeP.SourceTable = 'Device'
    CROSS JOIN Node
WHERE (Device.Enabled <> 0 OR Device.Enabled IS NULL) AND (Measurement.Enabled <> 0)
UNION ALL
SELECT NodeID, SourceNodeID, (Source || ':' || CAST(PointID AS TEXT)) AS ID, SignalID, PointTag,
    AlternateTag, SignalReference, 0 AS Internal, 1 AS Subscribed, NULL AS Device, NULL AS DeviceID,
    FramesPerSecond, ProtocolAcronym AS Protocol, ProtocolType, SignalTypeAcronym AS SignalType, '' AS EngineeringUnits, PhasorID, PhasorType, Phase, Adder, Multiplier,
    CompanyAcronym AS Company, Longitude, Latitude, Description, NULL AS UpdatedOn
FROM ImportedMeasurement
WHERE ImportedMeasurement.Enabled <> 0;

CREATE VIEW RuntimeStatistic
AS
SELECT Node.ID AS NodeID, Statistic.ID AS ID, Statistic.Source, Statistic.SignalIndex, Statistic.Name, Statistic.Description,
    Statistic.AssemblyName, Statistic.TypeName, Statistic.MethodName, Statistic.Arguments, Statistic.IsConnectedState, Statistic.DataType, 
                      Statistic.DisplayFormat, Statistic.Enabled
FROM Statistic, Node;

CREATE VIEW IaonOutputAdapter
AS
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeHistorian
UNION
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeCustomOutputAdapter;

CREATE VIEW IaonInputAdapter
AS
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeDevice
UNION
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeCustomInputAdapter;

CREATE VIEW IaonActionAdapter
AS
SELECT Node.ID AS NodeID, 0 AS ID, 'PHASOR!SERVICES' AS AdapterName, 'PhasorProtocolAdapters.dll' AS AssemblyName, 'PhasorProtocolAdapters.CommonPhasorServices' AS TypeName, '' AS ConnectionString
FROM Node
UNION
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeOutputStream
UNION
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeCalculatedMeasurement
UNION
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeCustomActionAdapter;
      
CREATE VIEW MeasurementDetail
AS
SELECT     Device.CompanyID, Device.CompanyAcronym, Device.CompanyName, Measurement.SignalID, 
                      Measurement.HistorianID, Historian.Acronym AS HistorianAcronym, Historian.ConnectionString AS HistorianConnectionString, 
                      Measurement.PointID, Measurement.PointTag, Measurement.AlternateTag, Measurement.DeviceID,  COALESCE (Device.NodeID, Historian.NodeID) AS NodeID, 
                      Device.Acronym AS DeviceAcronym, Device.Name AS DeviceName, COALESCE(Device.FramesPerSecond, 30) AS FramesPerSecond, Device.Enabled AS DeviceEnabled, Device.ContactList, 
                      Device.VendorDeviceID, VendorDevice.Name AS VendorDeviceName, VendorDevice.Description AS VendorDeviceDescription, 
                      Device.ProtocolID, Protocol.Acronym AS ProtocolAcronym, Protocol.Name AS ProtocolName, Measurement.SignalTypeID, 
                      Measurement.PhasorSourceIndex, Phasor.Label AS PhasorLabel, Phasor.Type AS PhasorType, Phasor.Phase, 
                      Measurement.SignalReference, Measurement.Adder, Measurement.Multiplier, Measurement.Description, Measurement.Subscribed, Measurement.Internal, Measurement.Enabled, 
                      COALESCE (Measurement.EngineeringUnits, '') AS EngineeringUnits, Measurement.Source, Measurement.SignalAcronym, 
                      Measurement.SignalName, Measurement.SignalTypeSuffix, Device.Longitude, Device.Latitude,
                      (COALESCE(Historian.Acronym, Device.Acronym, '__') || ':' || CAST(Measurement.PointID AS TEXT)) AS ID, Measurement.UpdatedOn
FROM         (SELECT Measurement.*, SignalType.Acronym AS SignalAcronym, SignalType.Name AS SignalName, SignalType.Suffix AS SignalTypeSuffix, SignalType.EngineeringUnits, SignalType.Source FROM Measurement LEFT OUTER JOIN
                       SignalType ON Measurement.SignalTypeID = SignalType.ID) AS Measurement LEFT OUTER JOIN
                      (SELECT Device.*, Company.Acronym AS CompanyAcronym, Company.Name AS CompanyName FROM Device LEFT OUTER JOIN
                       Company ON Device.CompanyID = Company.ID) AS Device ON Device.ID = Measurement.DeviceID LEFT OUTER JOIN
                      Phasor ON Measurement.DeviceID = Phasor.DeviceID AND 
                      Measurement.PhasorSourceIndex = Phasor.SourceIndex LEFT OUTER JOIN
                      VendorDevice ON Device.VendorDeviceID = VendorDevice.ID LEFT OUTER JOIN
                      Protocol ON Device.ProtocolID = Protocol.ID LEFT OUTER JOIN
                      Historian ON Measurement.HistorianID = Historian.ID;

CREATE VIEW HistorianMetadata
AS
SELECT PointID AS HistorianID, CASE WHEN SignalAcronym = 'DIGI' THEN 1 ELSE 0 END AS DataType, PointTag AS Name, SignalReference AS Synonym1, 
SignalAcronym AS Synonym2, AlternateTag AS Synonym3, Description, VendorDeviceDescription AS HardwareInfo, ''::VARCHAR(512) AS Remarks, 
HistorianAcronym AS PlantCode, 1 AS UnitNumber, DeviceAcronym AS SystemName, ProtocolID AS SourceID, Enabled, 1.0 / FramesPerSecond AS ScanRate, 
0 AS CompressionMinTime, 0 AS CompressionMaxTime, EngineeringUnits,
CASE SignalAcronym WHEN 'FREQ' THEN 59.95 WHEN 'VPHM' THEN 475000 WHEN 'IPHM' THEN 0 WHEN 'VPHA' THEN -181 WHEN 'IPHA' THEN -181 ELSE 0 END AS LowWarning,
CASE SignalAcronym WHEN 'FREQ' THEN 60.05 WHEN 'VPHM' THEN 525000 WHEN 'IPHM' THEN 3150 WHEN 'VPHA' THEN 181 WHEN 'IPHA' THEN 181 ELSE 0 END AS HighWarning,
CASE SignalAcronym WHEN 'FREQ' THEN 59.90 WHEN 'VPHM' THEN 450000 WHEN 'IPHM' THEN 0 WHEN 'VPHA' THEN -181 WHEN 'IPHA' THEN -181 ELSE 0 END AS LowAlarm,
CASE SignalAcronym WHEN 'FREQ' THEN 60.10 WHEN 'VPHM' THEN 550000 WHEN 'IPHM' THEN 3300 WHEN 'VPHA' THEN 181 WHEN 'IPHA' THEN 181 ELSE 0 END AS HighAlarm,
CASE SignalAcronym WHEN 'FREQ' THEN 59.95 WHEN 'VPHM' THEN 475000 WHEN 'IPHM' THEN 0 WHEN 'VPHA' THEN -180 WHEN 'IPHA' THEN -180 ELSE 0 END AS LowRange,
CASE SignalAcronym WHEN 'FREQ' THEN 60.05 WHEN 'VPHM' THEN 525000 WHEN 'IPHM' THEN 3000 WHEN 'VPHA' THEN 180 WHEN 'IPHA' THEN 180 ELSE 0 END AS HighRange,
0.0 AS CompressionLimit, 0.0 AS ExceptionLimit, CASE SignalAcronym WHEN 'DIGI' THEN 0 ELSE 7 END AS DisplayDigits, ''::VARCHAR(24) AS SetDescription,
''::VARCHAR(24) AS ClearDescription, 0 AS AlarmState, 5 AS ChangeSecurity, 0 AS AccessSecurity, 0 AS StepCheck, 0 AS AlarmEnabled, 0 AS AlarmFlags, 0 AS AlarmDelay,
0 AS AlarmToFile, 0 AS AlarmByEmail, 0 AS AlarmByPager, 0 AS AlarmByPhone, ContactList AS AlarmEmails, ''::VARCHAR(40) AS AlarmPagers, ''::VARCHAR(40) AS AlarmPhones
FROM MeasurementDetail;

CREATE VIEW CurrentAlarmState
AS
SELECT
    SignalsWithAlarms.SignalID,
    CurrentState.NewState AS State,
    CurrentState.Timestamp,
    CurrentState.Value
FROM
    (
        SELECT DISTINCT Measurement.SignalID
        FROM Measurement JOIN Alarm ON Measurement.SignalID = Alarm.SignalID
        WHERE Alarm.Enabled <> 0
    ) AS SignalsWithAlarms
    LEFT OUTER JOIN
    (
        SELECT
            Log1.SignalID,
            Log1.NewState,
            Log1.Timestamp,
            Log1.Value
        FROM
            AlarmLog AS Log1 LEFT OUTER JOIN
            AlarmLog AS Log2 ON Log1.SignalID = Log2.SignalID AND Log1.Ticks < Log2.Ticks
        WHERE
            Log2.ID IS NULL
    ) AS CurrentState
    ON SignalsWithAlarms.SignalID = CurrentState.SignalID;

CREATE VIEW CalculatedMeasurementDetail
AS
SELECT CM.NodeID, CM.ID, CM.Acronym, COALESCE(CM.Name, '') AS Name, CM.AssemblyName, CM.TypeName, COALESCE(CM.ConnectionString, '') AS ConnectionString,
        COALESCE(CM.ConfigSection, '') AS ConfigSection, COALESCE(CM.InputMeasurements, '') AS InputMeasurements, COALESCE(CM.OutputMeasurements, '') AS OutputMeasurements,
        CM.MinimumMeasurementsToUse, CM.FramesPerSecond, CM.LagTime, CM.LeadTime, CM.UseLocalClockAsRealTime, CM.AllowSortsByArrival, CM.LoadOrder, CM.Enabled,
        N.Name AS NodeName, CM.IgnoreBadTimeStamps, CM.TimeResolution, CM.AllowPreemptivePublishing, COALESCE(CM.DownsamplingMethod, '') AS DownsamplingMethod, CM.PerformTimeReasonabilityCheck
FROM CalculatedMeasurement CM, Node N
WHERE CM.NodeID = N.ID;

CREATE VIEW HistorianDetail
AS
SELECT H.NodeID, H.ID, H.Acronym, COALESCE(H.Name, '') AS Name, COALESCE(H.AssemblyName, '') AS AssemblyName, COALESCE(H.TypeName, '') AS TypeName, 
    COALESCE(H.ConnectionString, '') AS ConnectionString, H.IsLocal, COALESCE(H.Description, '') AS Description, H.LoadOrder, H.Enabled, N.Name AS NodeName, H.MeasurementReportingInterval 
FROM Historian AS H INNER JOIN Node AS N ON H.NodeID = N.ID;

CREATE VIEW NodeDetail
AS
SELECT N.ID, N.Name, N.CompanyID AS CompanyID, COALESCE(N.Longitude, 0) AS Longitude, COALESCE(N.Latitude, 0) AS Latitude, 
        COALESCE(N.Description, '') AS Description, COALESCE(N.ImagePath, '') AS ImagePath, COALESCE(N.Settings, '') AS Settings, N.MenuType, N.MenuData, N.Master, N.LoadOrder, N.Enabled, COALESCE(C.Name, '') AS CompanyName
FROM Node N LEFT JOIN Company C 
ON N.CompanyID = C.ID;

CREATE VIEW VendorDetail
AS
Select ID, COALESCE(Acronym, '') AS Acronym, Name, COALESCE(PhoneNumber, '') AS PhoneNumber, COALESCE(ContactEmail, '') AS ContactEmail, COALESCE(URL, '') AS URL 
FROM Vendor;

CREATE VIEW CustomActionAdapterDetail AS
SELECT     CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, COALESCE(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
                      CA.Enabled, N.Name AS NodeName
FROM         CustomActionAdapter AS CA INNER JOIN Node AS N ON CA.NodeID = N.ID;
 
CREATE VIEW CustomInputAdapterDetail AS
SELECT     CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, COALESCE(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
                      CA.Enabled, N.Name AS NodeName
FROM         CustomInputAdapter AS CA INNER JOIN Node AS N ON CA.NodeID = N.ID;
 
CREATE VIEW CustomOutputAdapterDetail AS
SELECT     CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, COALESCE(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
                      CA.Enabled, N.Name AS NodeName
FROM         CustomOutputAdapter AS CA INNER JOIN Node AS N ON CA.NodeID = N.ID;
 
CREATE VIEW IaonTreeView AS
SELECT     'Action Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, COALESCE(ConnectionString, '') AS ConnectionString
FROM         IaonActionAdapter
UNION ALL
SELECT     'Input Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, COALESCE(ConnectionString, '') AS ConnectionString
FROM         IaonInputAdapter
UNION ALL
SELECT     'Output Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, COALESCE(ConnectionString, '') AS ConnectionString
FROM         IaonOutputAdapter;
 
CREATE VIEW OtherDeviceDetail AS
SELECT     OD.ID, OD.Acronym, COALESCE(OD.Name, '') AS Name, OD.IsConcentrator, OD.CompanyID, OD.VendorDeviceID, OD.Longitude, OD.Latitude, 
                      OD.InterconnectionID, OD.Planned, OD.Desired, OD.InProgress, COALESCE(C.Name, '') AS CompanyName, COALESCE(C.Acronym, '') AS CompanyAcronym, 
                      COALESCE(C.MapAcronym, '') AS CompanyMapAcronym, COALESCE(VD.Name, '') AS VendorDeviceName, COALESCE(I.Name, '') AS InterconnectionName
FROM         OtherDevice AS OD LEFT OUTER JOIN
                      Company AS C ON OD.CompanyID = C.ID LEFT OUTER JOIN
                      VendorDevice AS VD ON OD.VendorDeviceID = VD.ID LEFT OUTER JOIN
                      Interconnection AS I ON OD.InterconnectionID = I.ID;
 
CREATE VIEW VendorDeviceDistribution AS
SELECT Device.NodeID, Vendor.Name AS VendorName, COUNT(*) AS DeviceCount 
FROM Device  LEFT OUTER JOIN
    VendorDevice ON Device.VendorDeviceID = VendorDevice.ID INNER JOIN
    Vendor ON VendorDevice.VendorID = Vendor.ID
GROUP BY Device.NodeID, Vendor.Name;

CREATE VIEW VendorDeviceDetail
AS
SELECT     VD.ID, VD.VendorID, VD.Name, COALESCE(VD.Description, '') AS Description, COALESCE(VD.URL, '') AS URL, V.Name AS VendorName, 
                      V.Acronym AS VendorAcronym
FROM         VendorDevice AS VD INNER JOIN Vendor AS V ON VD.VendorID = V.ID;
                      
CREATE VIEW DeviceDetail
AS
SELECT     D.NodeID, D.ID, D.ParentID, D.UniqueID, D.Acronym, COALESCE(D.Name, '') AS Name, D.OriginalSource, D.IsConcentrator, D.CompanyID, D.HistorianID, D.AccessID, D.VendorDeviceID, 
                      D.ProtocolID, D.Longitude, D.Latitude, D.InterconnectionID, COALESCE(D.ConnectionString, '') AS ConnectionString, COALESCE(D.TimeZone, '') AS TimeZone, 
                      COALESCE(D.FramesPerSecond, 30) AS FramesPerSecond, D.TimeAdjustmentTicks, D.DataLossInterval, D.ConnectOnDemand, COALESCE(D.ContactList, '') AS ContactList, D.MeasuredLines, D.LoadOrder, D.Enabled, COALESCE(C.Name, '') 
                      AS CompanyName, COALESCE(C.Acronym, '') AS CompanyAcronym, COALESCE(C.MapAcronym, '') AS CompanyMapAcronym, COALESCE(H.Acronym, '') 
                      AS HistorianAcronym, COALESCE(VD.VendorAcronym, '') AS VendorAcronym, COALESCE(VD.Name, '') AS VendorDeviceName, COALESCE(P.Name, '') 
                      AS ProtocolName, P.Type AS ProtocolType, P.Category, COALESCE(I.Name, '') AS InterconnectionName, N.Name AS NodeName, COALESCE(PD.Acronym, '') AS ParentAcronym, D.CreatedOn, D.AllowedParsingExceptions, 
                      D.ParsingExceptionWindow, D.DelayedConnectionInterval, D.AllowUseOfCachedConfiguration, D.AutoStartDataParsingSequence, D.SkipDisableRealTimeData, 
                      D.MeasurementReportingInterval, D.UpdatedOn
FROM         Device AS D LEFT OUTER JOIN
                      Company AS C ON C.ID = D.CompanyID LEFT OUTER JOIN
                      Historian AS H ON H.ID = D.HistorianID LEFT OUTER JOIN
                      VendorDeviceDetail AS VD ON VD.ID = D.VendorDeviceID LEFT OUTER JOIN
                      Protocol AS P ON P.ID = D.ProtocolID LEFT OUTER JOIN
                      Interconnection AS I ON I.ID = D.InterconnectionID LEFT OUTER JOIN
                      Node AS N ON N.ID = D.NodeID LEFT OUTER JOIN
                      Device AS PD ON PD.ID = D.ParentID;
 
CREATE VIEW MapData AS
SELECT     'Device' AS DeviceType, NodeID, ID, Acronym, COALESCE(Name, '') AS Name, CompanyMapAcronym, CompanyName, VendorDeviceName, Longitude, 
                      Latitude, 1 AS Reporting, 0 AS Inprogress, 0 AS Planned, 0 AS Desired
FROM         DeviceDetail AS D
UNION ALL
SELECT     'OtherDevice' AS DeviceType, NULL AS NodeID, ID, Acronym, COALESCE(Name, '') AS Name, CompanyMapAcronym, CompanyName, VendorDeviceName, 
                      Longitude, Latitude, 0 AS Reporting, 1 AS Inprogress, 1 AS Planned, 1 AS Desired
FROM         OtherDeviceDetail AS OD;

CREATE VIEW OutputStreamDetail AS
SELECT     OS.NodeID, OS.ID, OS.Acronym, COALESCE(OS.Name, '') AS Name, OS.Type, COALESCE(OS.ConnectionString, '') AS ConnectionString, OS.IDCode, 
                      COALESCE(OS.CommandChannel, '') AS CommandChannel, COALESCE(OS.DataChannel, '') AS DataChannel, OS.AutoPublishConfigFrame, 
                      OS.AutoStartDataChannel, OS.NominalFrequency, OS.FramesPerSecond, OS.LagTime, OS.LeadTime, OS.UseLocalClockAsRealTime, 
                      OS.AllowSortsByArrival, OS.LoadOrder, OS.Enabled, N.Name AS NodeName, OS.DigitalMaskValue, OS.AnalogScalingValue, 
                      OS.VoltageScalingValue, OS.CurrentScalingValue, OS.CoordinateFormat, OS.DataFormat, OS.DownsamplingMethod, 
                      OS.AllowPreemptivePublishing, OS.TimeResolution, OS.IgnoreBadTimeStamps, OS.PerformTimeReasonabilityCheck
FROM         OutputStream AS OS INNER JOIN Node AS N ON OS.NodeID = N.ID;
                      
CREATE VIEW OutputStreamMeasurementDetail AS
SELECT     OSM.NodeID, OSM.AdapterID, OSM.ID, OSM.HistorianID, OSM.PointID, OSM.SignalReference, M.PointTag AS SourcePointTag, COALESCE(H.Acronym, '') 
                      AS HistorianAcronym
FROM         OutputStreamMeasurement AS OSM INNER JOIN
                      Measurement AS M ON M.PointID = OSM.PointID LEFT OUTER JOIN
                      Historian AS H ON H.ID = OSM.HistorianID;
      
CREATE VIEW OutputStreamDeviceDetail AS
SELECT OSD.NodeID, OSD.AdapterID, OSD.ID, OSD.Acronym, COALESCE(OSD.BpaAcronym, '') AS BpaAcronym, OSD.Name, OSD.LoadOrder, OSD.Enabled, 
            COALESCE(PhasorDataFormat, '') AS PhasorDataFormat, COALESCE(FrequencyDataFormat, '') AS FrequencyDataFormat, 
            COALESCE(AnalogDataFormat, '') AS AnalogDataFormat, COALESCE(CoordinateFormat, '') AS CoordinateFormat, IDCode,
                    CASE 
                        WHEN EXISTS (Select Acronym From Device Where Acronym = OSD.Acronym) THEN 0 
                        ELSE 1 
                    END AS Virtual
FROM OutputStreamDevice OSD;
                      
CREATE VIEW PhasorDetail AS
SELECT P.*, COALESCE(DP.Label, '') AS DestinationPhasorLabel, D.Acronym AS DeviceAcronym
FROM Phasor P LEFT OUTER JOIN Phasor DP ON P.DestinationPhasorID = DP.ID
      LEFT OUTER JOIN Device D ON P.DeviceID = D.ID;

CREATE VIEW StatisticMeasurement AS
SELECT     MeasurementDetail.*
FROM MeasurementDetail WHERE MeasurementDetail.SignalAcronym = 'STAT';

CREATE VIEW AppRoleSecurityGroupDetail AS 
SELECT ApplicationRoleSecurityGroup.ApplicationRoleID AS ApplicationRoleID,ApplicationRoleSecurityGroup.SecurityGroupID AS SecurityGroupID,ApplicationRole.Name AS ApplicationRoleName,ApplicationRole.Description AS ApplicationRoleDescription,SecurityGroup.Name AS SecurityGroupName,SecurityGroup.Description AS SecurityGroupDescription 
FROM ((ApplicationRoleSecurityGroup JOIN ApplicationRole ON((ApplicationRoleSecurityGroup.ApplicationRoleID = ApplicationRole.ID))) 
    JOIN SecurityGroup ON((ApplicationRoleSecurityGroup.SecurityGroupID = SecurityGroup.ID)));

CREATE VIEW AppRoleUserAccountDetail AS 
SELECT ApplicationRoleUserAccount.ApplicationRoleID AS ApplicationRoleID,ApplicationRoleUserAccount.UserAccountID AS UserAccountID,UserAccount.Name AS UserName,UserAccount.FirstName AS FirstName,UserAccount.LastName AS LastName,UserAccount.Email AS Email,ApplicationRole.Name AS ApplicationRoleName,ApplicationRole.Description AS ApplicationRoleDescription 
FROM ((ApplicationRoleUserAccount JOIN ApplicationRole ON((ApplicationRoleUserAccount.ApplicationRoleID = ApplicationRole.ID))) JOIN UserAccount ON((ApplicationRoleUserAccount.UserAccountID = UserAccount.ID)));

CREATE VIEW SecurityGroupUserAccountDetail AS 
SELECT SecurityGroupUserAccount.SecurityGroupID AS SecurityGroupID,SecurityGroupUserAccount.UserAccountID AS UserAccountID,UserAccount.Name AS UserName,UserAccount.FirstName AS FirstName,UserAccount.LastName AS LastName,UserAccount.Email AS Email,SecurityGroup.Name AS SecurityGroupName,SecurityGroup.Description AS SecurityGroupDescription 
FROM ((SecurityGroupUserAccount JOIN SecurityGroup ON((SecurityGroupUserAccount.SecurityGroupID = SecurityGroup.ID))) JOIN UserAccount ON((SecurityGroupUserAccount.UserAccountID = UserAccount.ID)));

CREATE VIEW SubscriberMeasurementDetail AS 
SELECT SubscriberMeasurement.NodeID AS NodeID, SubscriberMeasurement.SubscriberID AS SubscriberID, Subscriber.Acronym AS SubscriberAcronym, COALESCE(Subscriber.Name, '') AS SubscriberName, 
SubscriberMeasurement.SignalID AS SignalID, SubscriberMeasurement.Allowed AS Allowed, Measurement.PointID AS PointID, Measurement.PointTag AS PointTag, Measurement.SignalReference AS SignalReference
FROM ((SubscriberMeasurement JOIN Subscriber ON (SubscriberMeasurement.SubscriberID = Subscriber.ID)) JOIN Measurement ON (SubscriberMeasurement.SignalID = Measurement.SignalID));

CREATE VIEW SubscriberMeasGroupDetail AS 
SELECT SubscriberMeasurementGroup.NodeID AS NodeID, SubscriberMeasurementGroup.SubscriberID AS SubscriberID, Subscriber.Acronym AS SubscriberAcronym, COALESCE(Subscriber.Name, '') AS SubscriberName, 
SubscriberMeasurementGroup.MeasurementGroupID AS MeasurementGroupID, SubscriberMeasurementGroup.Allowed AS Allowed, MeasurementGroup.Name AS MeasurementGroupName
FROM ((SubscriberMeasurementGroup JOIN Subscriber ON (SubscriberMeasurementGroup.SubscriberID = Subscriber.ID)) JOIN MeasurementGroup ON (SubscriberMeasurementGroup.MeasurementGroupID = MeasurementGroup.ID));

CREATE VIEW MeasurementGroupMeasDetail AS 
SELECT MeasurementGroupMeasurement.MeasurementGroupID AS MeasurementGroupID, MeasurementGroup.Name AS MeasurementGroupName,
MeasurementGroupMeasurement.SignalID AS SignalID, Measurement.PointID AS PointID, Measurement.PointTag AS PointTag, Measurement.SignalReference AS SignalReference
FROM ((MeasurementGroupMeasurement JOIN MeasurementGroup ON (MeasurementGroupMeasurement.MeasurementGroupID = MeasurementGroup.ID)) JOIN Measurement ON (MeasurementGroupMeasurement.SignalID = Measurement.SignalID));

CREATE VIEW TrackedTable AS
SELECT 'Measurement' AS Name
UNION
SELECT 'ActiveMeasurement' AS Name
UNION
SELECT 'Device' AS Name
UNION
SELECT 'OutputStream' AS Name
UNION
SELECT 'OutputStreamDevice' AS Name
UNION
SELECT 'OutputStreamMeasurement' AS Name;

-- CustomActionAdapter_RuntimeSync_Insert --
CREATE FUNCTION CustomActionAdapter_RuntimeSync_InsertFn() RETURNS TRIGGER
AS $CustomActionAdapter_RuntimeSync_InsertFn$
BEGIN
    INSERT INTO Runtime(SourceID, SourceTable) VALUES(NEW.ID, 'CustomActionAdapter');
    RETURN NEW;
END;
$CustomActionAdapter_RuntimeSync_InsertFn$ LANGUAGE plpgsql;

CREATE TRIGGER CustomActionAdapter_RuntimeSync_Insert AFTER INSERT ON CustomActionAdapter
FOR EACH ROW EXECUTE PROCEDURE CustomActionAdapter_RuntimeSync_InsertFn();

-- CustomActionAdapter_RuntimeSync_Delete --
CREATE FUNCTION CustomActionAdapter_RuntimeSync_DeleteFn() RETURNS TRIGGER
AS $CustomActionAdapter_RuntimeSync_DeleteFn$
BEGIN
    DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = 'CustomActionAdapter';
    RETURN OLD;
END;
$CustomActionAdapter_RuntimeSync_DeleteFn$ LANGUAGE plpgsql;

CREATE TRIGGER CustomActionAdapter_RuntimeSync_Delete BEFORE DELETE ON CustomActionAdapter
FOR EACH ROW EXECUTE PROCEDURE CustomActionAdapter_RuntimeSync_DeleteFn();

-- CustomInputAdapter_RuntimeSync_Insert --
CREATE FUNCTION CustomInputAdapter_RuntimeSync_InsertFn() RETURNS TRIGGER
AS $CustomInputAdapter_RuntimeSync_InsertFn$
BEGIN
    INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, 'CustomInputAdapter');
    RETURN NEW;
END;
$CustomInputAdapter_RuntimeSync_InsertFn$ LANGUAGE plpgsql;

CREATE TRIGGER CustomInputAdapter_RuntimeSync_Insert AFTER INSERT ON CustomInputAdapter
FOR EACH ROW EXECUTE PROCEDURE CustomInputAdapter_RuntimeSync_InsertFn();

-- CustomInputAdapter_RuntimeSync_Delete --
CREATE FUNCTION CustomInputAdapter_RuntimeSync_DeleteFn() RETURNS TRIGGER
AS $CustomInputAdapter_RuntimeSync_DeleteFn$
BEGIN
    DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = 'CustomInputAdapter';
    RETURN OLD;
END;
$CustomInputAdapter_RuntimeSync_DeleteFn$ LANGUAGE plpgsql;

CREATE TRIGGER CustomInputAdapter_RuntimeSync_Delete BEFORE DELETE ON CustomInputAdapter
FOR EACH ROW EXECUTE PROCEDURE CustomInputAdapter_RuntimeSync_DeleteFn();

-- CustomOutputAdapter_RuntimeSync_Insert --
CREATE FUNCTION CustomOutputAdapter_RuntimeSync_InsertFn() RETURNS TRIGGER
AS $CustomOutputAdapter_RuntimeSync_InsertFn$
BEGIN
    INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, 'CustomOutputAdapter');
    RETURN NEW;
END;
$CustomOutputAdapter_RuntimeSync_InsertFn$ LANGUAGE plpgsql;

CREATE TRIGGER CustomOutputAdapter_RuntimeSync_Insert AFTER INSERT ON CustomOutputAdapter
FOR EACH ROW EXECUTE PROCEDURE CustomOutputAdapter_RuntimeSync_InsertFn();

-- CustomOutputAdapter_RuntimeSync_Delete --
CREATE FUNCTION CustomOutputAdapter_RuntimeSync_DeleteFn() RETURNS TRIGGER
AS $CustomOutputAdapter_RuntimeSync_DeleteFn$
BEGIN
    DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = 'CustomOutputAdapter';
    RETURN OLD;
END;
$CustomOutputAdapter_RuntimeSync_DeleteFn$ LANGUAGE plpgsql;

CREATE TRIGGER CustomOutputAdapter_RuntimeSync_Delete BEFORE DELETE ON CustomOutputAdapter
FOR EACH ROW EXECUTE PROCEDURE CustomOutputAdapter_RuntimeSync_DeleteFn();

-- Device_RuntimeSync_Insert --
CREATE FUNCTION Device_RuntimeSync_InsertFn() RETURNS TRIGGER
AS $Device_RuntimeSync_InsertFn$
BEGIN
    INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, 'Device');
    RETURN NEW;
END;
$Device_RuntimeSync_InsertFn$ LANGUAGE plpgsql;

CREATE TRIGGER Device_RuntimeSync_Insert AFTER INSERT ON Device
FOR EACH ROW EXECUTE PROCEDURE Device_RuntimeSync_InsertFn();

-- Device_RuntimeSync_Delete --
CREATE FUNCTION Device_RuntimeSync_DeleteFn() RETURNS TRIGGER
AS $Device_RuntimeSync_DeleteFn$
BEGIN
    DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = 'Device';
    RETURN OLD;
END;
$Device_RuntimeSync_DeleteFn$ LANGUAGE plpgsql;

CREATE TRIGGER Device_RuntimeSync_Delete BEFORE DELETE ON Device
FOR EACH ROW EXECUTE PROCEDURE Device_RuntimeSync_DeleteFn();

-- CalculatedMeasurement_RuntimeSync_Insert --
CREATE FUNCTION CalculatedMeasurement_RuntimeSync_InsertFn() RETURNS TRIGGER
AS $CalculatedMeasurement_RuntimeSync_InsertFn$
BEGIN
    INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, 'CalculatedMeasurement');
    RETURN NEW;
END;
$CalculatedMeasurement_RuntimeSync_InsertFn$ LANGUAGE plpgsql;

CREATE TRIGGER CalculatedMeasurement_RuntimeSync_Insert AFTER INSERT ON CalculatedMeasurement
FOR EACH ROW EXECUTE PROCEDURE CalculatedMeasurement_RuntimeSync_InsertFn();

-- CalculatedMeasurement_RuntimeSync_Delete --
CREATE FUNCTION CalculatedMeasurement_RuntimeSync_DeleteFn() RETURNS TRIGGER
AS $CalculatedMeasurement_RuntimeSync_DeleteFn$
BEGIN
    DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = 'CalculatedMeasurement';
    RETURN OLD;
END;
$CalculatedMeasurement_RuntimeSync_DeleteFn$ LANGUAGE plpgsql;

CREATE TRIGGER CalculatedMeasurement_RuntimeSync_Delete BEFORE DELETE ON CalculatedMeasurement
FOR EACH ROW EXECUTE PROCEDURE CalculatedMeasurement_RuntimeSync_DeleteFn();

-- OutputStream_RuntimeSync_Insert --
CREATE FUNCTION OutputStream_RuntimeSync_InsertFn() RETURNS TRIGGER
AS $OutputStream_RuntimeSync_InsertFn$
BEGIN
    INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, 'OutputStream');
    RETURN NEW;
END;
$OutputStream_RuntimeSync_InsertFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStream_RuntimeSync_Insert AFTER INSERT ON OutputStream
FOR EACH ROW EXECUTE PROCEDURE OutputStream_RuntimeSync_InsertFn();

-- OutputStream_RuntimeSync_Delete --
CREATE FUNCTION OutputStream_RuntimeSync_DeleteFn() RETURNS TRIGGER
AS $OutputStream_RuntimeSync_DeleteFn$
BEGIN
    DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = 'OutputStream';
    RETURN OLD;
END;
$OutputStream_RuntimeSync_DeleteFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStream_RuntimeSync_Delete BEFORE DELETE ON OutputStream
FOR EACH ROW EXECUTE PROCEDURE OutputStream_RuntimeSync_DeleteFn();

-- Historian_RuntimeSync_Insert --
CREATE FUNCTION Historian_RuntimeSync_InsertFn() RETURNS TRIGGER
AS $Historian_RuntimeSync_InsertFn$
BEGIN
    INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, 'Historian');
    RETURN NEW;
END;
$Historian_RuntimeSync_InsertFn$ LANGUAGE plpgsql;

CREATE TRIGGER Historian_RuntimeSync_Insert AFTER INSERT ON Historian
FOR EACH ROW EXECUTE PROCEDURE Historian_RuntimeSync_InsertFn();

-- Historian_RuntimeSync_Delete --
CREATE FUNCTION Historian_RuntimeSync_DeleteFn() RETURNS TRIGGER
AS $Historian_RuntimeSync_DeleteFn$
BEGIN
    DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = 'Historian';
    RETURN OLD;
END;
$Historian_RuntimeSync_DeleteFn$ LANGUAGE plpgsql;

CREATE TRIGGER Historian_RuntimeSync_Delete BEFORE DELETE ON Historian
FOR EACH ROW EXECUTE PROCEDURE Historian_RuntimeSync_DeleteFn();

-- Node_InsertDefault --
CREATE FUNCTION Node_InsertDefaultFn() RETURNS TRIGGER
AS $Node_InsertDefaultFn$
BEGIN
    INSERT INTO MeasurementGroup(NodeID, Name, Description, FilterExpression) VALUES(NEW.ID, 'AllMeasurements', 'All measurements defined in ActiveMeasurements', 'FILTER ActiveMeasurements WHERE SignalID IS NOT NULL');
    RETURN NEW;
END;
$Node_InsertDefaultFn$ LANGUAGE plpgsql;

CREATE TRIGGER Node_InsertDefault AFTER INSERT ON Node
FOR EACH ROW EXECUTE PROCEDURE Node_InsertDefaultFn();

-- ***********************
-- Company Change Tracking
-- ***********************

CREATE FUNCTION Company_UpdateTrackerFn() RETURNS TRIGGER
AS $Company_UpdateTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM ActiveMeasurement WHERE Company = NEW.Acronym;
    RETURN NEW;
END;
$Company_UpdateTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER Company_UpdateTracker AFTER UPDATE ON Company
FOR EACH ROW WHEN (OLD.Acronym <> NEW.Acronym)
EXECUTE PROCEDURE Company_UpdateTrackerFn();

-- **********************
-- Device Change Tracking
-- **********************

-- Device_InsertTracker --
CREATE FUNCTION Device_InsertTrackerFn() RETURNS TRIGGER
AS $Device_InsertTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Device', 'ID', NEW.ID);
    RETURN NEW;
END;
$Device_InsertTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER Device_InsertTracker AFTER INSERT ON Device
FOR EACH ROW EXECUTE PROCEDURE Device_InsertTrackerFn();

-- Device_UpdateTracker1 --
CREATE FUNCTION Device_UpdateTracker1Fn() RETURNS TRIGGER
AS $Device_UpdateTracker1Fn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Device', 'ID', NEW.ID);
    RETURN NEW;
END;
$Device_UpdateTracker1Fn$ LANGUAGE plpgsql;

CREATE TRIGGER Device_UpdateTracker1 AFTER UPDATE ON Device
FOR EACH ROW EXECUTE PROCEDURE Device_UpdateTracker1Fn();

-- Device_UpdateTracker2 --
CREATE FUNCTION Device_UpdateTracker2Fn() RETURNS TRIGGER
AS $Device_UpdateTracker2Fn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE DeviceID = NEW.ID;
    RETURN NEW;
END;
$Device_UpdateTracker2Fn$ LANGUAGE plpgsql;

CREATE TRIGGER Device_UpdateTracker2 AFTER UPDATE ON Device
FOR EACH ROW WHEN
(
    OLD.NodeID <> NEW.NodeID OR
    OLD.Acronym <> NEW.Acronym OR
    OLD.IsConcentrator <> NEW.IsConcentrator OR
    OLD.ParentID <> NEW.ParentID OR
    OLD.FramesPerSecond <> NEW.FramesPerSecond OR
    OLD.Longitude <> NEW.Longitude OR
    OLD.Latitude <> NEW.Latitude OR
    OLD.CompanyID <> NEW.CompanyID OR
    OLD.ProtocolID <> NEW.ProtocolID OR
    OLD.Enabled <> NEW.Enabled
)
EXECUTE PROCEDURE Device_UpdateTracker2Fn();

-- Device_DeleteTracker --
CREATE FUNCTION Device_DeleteTrackerFn() RETURNS TRIGGER
AS $Device_DeleteTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Device', 'ID', OLD.ID);
    RETURN OLD;
END;
$Device_DeleteTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER Device_DeleteTracker AFTER DELETE ON Device
FOR EACH ROW EXECUTE PROCEDURE Device_DeleteTrackerFn();

-- *************************
-- Historian Change Tracking
-- *************************

CREATE FUNCTION Historian_UpdateTrackerFn() RETURNS TRIGGER
AS $Historian_UpdateTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE HistorianID = NEW.ID;
    RETURN NEW;
END;
$Historian_UpdateTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER Historian_UpdateTracker AFTER UPDATE ON Historian
FOR EACH ROW WHEN
(
    OLD.NodeID <> NEW.NodeID OR
    OLD.Acronym <> NEW.Acronym
)
EXECUTE PROCEDURE Historian_UpdateTrackerFn();

-- ***************************
-- Measurement Change Tracking
-- ***************************

-- Measurement_InsertTracker --
CREATE FUNCTION Measurement_InsertTrackerFn() RETURNS TRIGGER
AS $Measurement_InsertTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Measurement', 'PointID', NEW.PointID);
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('ActiveMeasurement', 'SignalID', NEW.SignalID);
    RETURN NEW;
END;
$Measurement_InsertTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER Measurement_InsertTracker AFTER INSERT ON Measurement
FOR EACH ROW EXECUTE PROCEDURE Measurement_InsertTrackerFn();

-- Measurement_UpdateTracker1 --
CREATE FUNCTION Measurement_UpdateTracker1Fn() RETURNS TRIGGER
AS $Measurement_UpdateTracker1Fn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Measurement', 'PointID', NEW.PointID);
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('ActiveMeasurement', 'SignalID', NEW.SignalID);
    RETURN NEW;
END;
$Measurement_UpdateTracker1Fn$ LANGUAGE plpgsql;

CREATE TRIGGER Measurement_UpdateTracker1 AFTER UPDATE ON Measurement
FOR EACH ROW EXECUTE PROCEDURE Measurement_UpdateTracker1Fn();

-- Measurement_UpdateTracker2 --
CREATE FUNCTION Measurement_UpdateTracker2Fn() RETURNS TRIGGER
AS $Measurement_UpdateTracker2Fn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('ActiveMeasurement', 'SignalID', OLD.SignalID);
    RETURN NEW;
END;
$Measurement_UpdateTracker2Fn$ LANGUAGE plpgsql;

CREATE TRIGGER Measurement_UpdateTracker2 AFTER UPDATE ON Measurement
FOR EACH ROW WHEN (NEW.SignalID <> OLD.SignalID)
EXECUTE PROCEDURE Measurement_UpdateTracker2Fn();

-- Measurement_DeleteTracker --
CREATE FUNCTION Measurement_DeleteTrackerFn() RETURNS TRIGGER
AS $Measurement_DeleteTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Measurement', 'PointID', OLD.PointID);
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('ActiveMeasurement', 'SignalID', OLD.SignalID);
    RETURN OLD;
END;
$Measurement_DeleteTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER Measurement_DeleteTracker AFTER DELETE ON Measurement
FOR EACH ROW EXECUTE PROCEDURE Measurement_DeleteTrackerFn();

-- ****************************
-- OutputStream Change Tracking
-- ****************************

-- OutputStream_InsertTracker --
CREATE FUNCTION OutputStream_InsertTrackerFn() RETURNS TRIGGER
AS $OutputStream_InsertTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStream', 'ID', NEW.ID);
    RETURN NEW;
END;
$OutputStream_InsertTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStream_InsertTracker AFTER INSERT ON OutputStream
FOR EACH ROW EXECUTE PROCEDURE OutputStream_InsertTrackerFn();

-- OutputStream_UpdateTracker --
CREATE FUNCTION OutputStream_UpdateTrackerFn() RETURNS TRIGGER
AS $OutputStream_UpdateTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStream', 'ID', NEW.ID);
    RETURN NEW;
END;
$OutputStream_UpdateTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStream_UpdateTracker AFTER UPDATE ON OutputStream
FOR EACH ROW EXECUTE PROCEDURE OutputStream_UpdateTrackerFn();

-- OutputStream_DeleteTracker --
CREATE FUNCTION OutputStream_DeleteTrackerFn() RETURNS TRIGGER
AS $OutputStream_DeleteTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStream', 'ID', OLD.ID);
    RETURN OLD;
END;
$OutputStream_DeleteTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStream_DeleteTracker AFTER DELETE ON OutputStream
FOR EACH ROW EXECUTE PROCEDURE OutputStream_DeleteTrackerFn();

-- **********************************
-- OutputStreamDevice Change Tracking
-- **********************************

-- OutputStreamDevice_InsertTracker --
CREATE FUNCTION OutputStreamDevice_InsertTrackerFn() RETURNS TRIGGER
AS $OutputStreamDevice_InsertTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamDevice', 'ID', NEW.ID);
    RETURN NEW;
END;
$OutputStreamDevice_InsertTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStreamDevice_InsertTracker AFTER INSERT ON OutputStreamDevice
FOR EACH ROW EXECUTE PROCEDURE OutputStreamDevice_InsertTrackerFn();

-- OutputStreamDevice_UpdateTracker --
CREATE FUNCTION OutputStreamDevice_UpdateTrackerFn() RETURNS TRIGGER
AS $OutputStreamDevice_UpdateTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamDevice', 'ID', NEW.ID);
    RETURN NEW;
END;
$OutputStreamDevice_UpdateTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStreamDevice_UpdateTracker AFTER UPDATE ON OutputStreamDevice
FOR EACH ROW EXECUTE PROCEDURE OutputStreamDevice_UpdateTrackerFn();

-- OutputStreamDevice_DeleteTracker --
CREATE FUNCTION OutputStreamDevice_DeleteTrackerFn() RETURNS TRIGGER
AS $OutputStreamDevice_DeleteTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamDevice', 'ID', OLD.ID);
    RETURN OLD;
END;
$OutputStreamDevice_DeleteTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStreamDevice_DeleteTracker AFTER DELETE ON OutputStreamDevice
FOR EACH ROW EXECUTE PROCEDURE OutputStreamDevice_DeleteTrackerFn();

-- ***************************************
-- OutputStreamMeasurement Change Tracking
-- ***************************************

-- OutputStreamMeasurement_InsertTracker --
CREATE FUNCTION OutputStreamMeasurement_InsertTrackerFn() RETURNS TRIGGER
AS $OutputStreamMeasurement_InsertTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamMeasurement', 'ID', NEW.ID);
    RETURN NEW;
END;
$OutputStreamMeasurement_InsertTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStreamMeasurement_InsertTracker AFTER INSERT ON OutputStreamMeasurement
FOR EACH ROW EXECUTE PROCEDURE OutputStreamMeasurement_InsertTrackerFn();

-- OutputStreamMeasurement_UpdateTracker --
CREATE FUNCTION OutputStreamMeasurement_UpdateTrackerFn() RETURNS TRIGGER
AS $OutputStreamMeasurement_UpdateTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamMeasurement', 'ID', NEW.ID);
    RETURN NEW;
END;
$OutputStreamMeasurement_UpdateTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStreamMeasurement_UpdateTracker AFTER UPDATE ON OutputStreamMeasurement
FOR EACH ROW EXECUTE PROCEDURE OutputStreamMeasurement_UpdateTrackerFn();

-- OutputStreamMeasurement_DeleteTracker --
CREATE FUNCTION OutputStreamMeasurement_DeleteTrackerFn() RETURNS TRIGGER
AS $OutputStreamMeasurement_DeleteTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamMeasurement', 'ID', OLD.ID);
    RETURN OLD;
END;
$OutputStreamMeasurement_DeleteTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER OutputStreamMeasurement_DeleteTracker AFTER DELETE ON OutputStreamMeasurement
FOR EACH ROW EXECUTE PROCEDURE OutputStreamMeasurement_DeleteTrackerFn();

-- **********************
-- Phasor Change Tracking
-- **********************

-- Phasor_UpdateTracker1 --
CREATE FUNCTION Phasor_UpdateTracker1Fn() RETURNS TRIGGER
AS $Phasor_UpdateTracker1Fn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM ActiveMeasurement WHERE PhasorID = NEW.ID;
    RETURN NEW;
END;
$Phasor_UpdateTracker1Fn$ LANGUAGE plpgsql;

CREATE TRIGGER Phasor_UpdateTracker1 AFTER UPDATE ON Phasor
FOR EACH ROW WHEN
(
    OLD.Type <> NEW.Type OR
    OLD.Phase <> NEW.Phase
)
EXECUTE PROCEDURE Phasor_UpdateTracker1Fn();

-- Phasor_UpdateTracker2 --
CREATE FUNCTION Phasor_UpdateTracker2Fn() RETURNS TRIGGER
AS $Phasor_UpdateTracker2Fn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE DeviceID = OLD.DeviceID AND PhasorSourceIndex = OLD.SourceIndex;
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE DeviceID = NEW.DeviceID AND PhasorSourceIndex = NEW.SourceIndex;
    RETURN NEW;
END;
$Phasor_UpdateTracker2Fn$ LANGUAGE plpgsql;

CREATE TRIGGER Phasor_UpdateTracker2 AFTER UPDATE ON Phasor
FOR EACH ROW WHEN
(
    OLD.DeviceID <> NEW.DeviceID OR
    OLD.SourceIndex <> NEW.SourceIndex
)
EXECUTE PROCEDURE Phasor_UpdateTracker2Fn();

-- ************************
-- Protocol Change Tracking
-- ************************

CREATE FUNCTION Protocol_UpdateTrackerFn() RETURNS TRIGGER
AS $Protocol_UpdateTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM ActiveMeasurement WHERE Protocol = NEW.Acronym;
    RETURN NEW;
END;
$Protocol_UpdateTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER Protocol_UpdateTracker AFTER UPDATE ON Protocol
FOR EACH ROW WHEN
(
    OLD.Acronym <> NEW.Acronym OR
    OLD.Type <> NEW.Type
)
EXECUTE PROCEDURE Protocol_UpdateTrackerFn();

-- **************************
-- SignalType Change Tracking
-- **************************

CREATE FUNCTION SignalType_UpdateTrackerFn() RETURNS TRIGGER
AS $SignalType_UpdateTrackerFn$
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE SignalTypeID = NEW.ID;
    RETURN NEW;
END;
$SignalType_UpdateTrackerFn$ LANGUAGE plpgsql;

CREATE TRIGGER SignalType_UpdateTracker AFTER UPDATE ON SignalType
FOR EACH ROW WHEN (OLD.Acronym <> NEW.Acronym)
EXECUTE PROCEDURE SignalType_UpdateTrackerFn();
