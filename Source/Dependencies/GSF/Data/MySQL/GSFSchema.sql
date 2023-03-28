--  ----------------------------------------------------------------------------------------------------
--  GSFSchema Data Structures for MySQL - Gbtc
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
--  03/27/2012 - prasanthgs
--       Added ExceptionLog table for keeping recent exceptions.
--  04/12/2012 - prasanthgs
--       Reworked as per the comments of codeplex reviewers.
--       Added new field Type to ErrorLog table. Removed ExceptionLog table.
--  ----------------------------------------------------------------------------------------------------

CREATE DATABASE GSFSchema CHARACTER SET = UTF8;
USE GSFSchema;

-- The following statements are used to create
-- a user with access to the database.
-- Be sure to change the username and password.
-- CREATE USER NewUser IDENTIFIED BY 'MyPassword';
-- GRANT SELECT, UPDATE, INSERT, DELETE ON GSFSchema.* TO NewUser;

-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW SchemaVersion AS
SELECT 15 AS VersionNumber;

CREATE TABLE ErrorLog(
    ID INT AUTO_INCREMENT NOT NULL,
    Source VARCHAR(200) NOT NULL,
    Type VARCHAR(200) NULL,
    Message TEXT NOT NULL,
    Detail TEXT NULL,
    CreatedOn DATETIME NULL,
    CONSTRAINT PK_ErrorLog PRIMARY KEY (ID ASC)
);

CREATE TABLE Runtime(
    ID INT AUTO_INCREMENT NOT NULL,
    SourceID INT NOT NULL,
    SourceTable VARCHAR(200) NOT NULL,
    CONSTRAINT PK_Runtime PRIMARY KEY (SourceID ASC, SourceTable ASC),
    CONSTRAINT IX_Runtime UNIQUE KEY (ID)
);

CREATE TABLE AuditLog(
    ID INT NOT NULL AUTO_INCREMENT,
    TableName VARCHAR(200) NOT NULL,
    PrimaryKeyColumn VARCHAR(200) NOT NULL,
    PrimaryKeyValue TEXT NOT NULL,
    ColumnName VARCHAR(200) NOT NULL,
    OriginalValue TEXT,
    NewValue TEXT,
    Deleted TINYINT NOT NULL DEFAULT 0,
    UpdatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    PRIMARY KEY (ID)
);

CREATE TABLE Company(
    ID INT AUTO_INCREMENT NOT NULL,
    Acronym VARCHAR(200) NOT NULL,
    MapAcronym NCHAR(10) NOT NULL,
    Name VARCHAR(200) NOT NULL,
    URL TEXT NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
  CONSTRAINT PK_Company PRIMARY KEY (ID ASC)
);

CREATE TABLE TrackedChange(
    ID BIGINT AUTO_INCREMENT NOT NULL,
    TableName VARCHAR(200) NOT NULL,
    PrimaryKeyColumn VARCHAR(200) NOT NULL,
    PrimaryKeyValue TEXT NULL,
    CONSTRAINT PK_TrackedChange PRIMARY KEY (ID ASC)
);

CREATE TABLE ConfigurationEntity(
    SourceName VARCHAR(200) NOT NULL,
    RuntimeName VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0
);

CREATE TABLE Vendor(
    ID INT AUTO_INCREMENT NOT NULL,
    Acronym VARCHAR(200) NULL,
    Name VARCHAR(200) NOT NULL,
    PhoneNumber VARCHAR(200) NULL,
    ContactEmail VARCHAR(200) NULL,
    URL TEXT NULL,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_Vendor PRIMARY KEY (ID ASC)
);

CREATE TABLE Protocol(
    ID INT AUTO_INCREMENT NOT NULL,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NOT NULL,
    Type VARCHAR(200) NOT NULL DEFAULT N'Frame',
    Category VARCHAR(200) NOT NULL DEFAULT N'Phasor',
    AssemblyName VARCHAR(1024) NOT NULL DEFAULT N'PhasorProtocolAdapters.dll',
    TypeName VARCHAR(200) NOT NULL DEFAULT N'PhasorProtocolAdapters.PhasorMeasurementMapper',
    LoadOrder INT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Protocol PRIMARY KEY (ID ASC)
);

CREATE TABLE SignalType(
    ID INT AUTO_INCREMENT NOT NULL,
    Name VARCHAR(200) NOT NULL,
    Acronym VARCHAR(4) NOT NULL,
    Suffix VARCHAR(2) NOT NULL,
    Abbreviation VARCHAR(2) NOT NULL,
    LongAcronym VARCHAR(200) NOT NULL DEFAULT 'Undefined',
    Source VARCHAR(10) NOT NULL,
    EngineeringUnits VARCHAR(10) NULL,
    CONSTRAINT PK_SignalType PRIMARY KEY (ID ASC)
);

CREATE TABLE Interconnection(
    ID INT AUTO_INCREMENT NOT NULL,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NOT NULL,
    LoadOrder INT NULL DEFAULT 0,
    CONSTRAINT PK_Interconnection PRIMARY KEY (ID ASC)
);

CREATE TABLE Node(
    ID NCHAR(36) NOT NULL DEFAULT '',
    Name VARCHAR(200) NOT NULL,
    CompanyID INT NULL,
    Longitude DECIMAL(9, 6) NULL,
    Latitude DECIMAL(9, 6) NULL,
    Description TEXT NULL,
    ImagePath TEXT NULL,
    Settings TEXT NULL,
    MenuType VARCHAR(200) NOT NULL DEFAULT 'File',
    MenuData TEXT NOT NULL,
    Master TINYINT NOT NULL DEFAULT 0,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_Node PRIMARY KEY (ID ASC),
    CONSTRAINT IX_NodeID_Name UNIQUE KEY (Name ASC)
);

CREATE TABLE DataOperation(
    NodeID NCHAR(36) NULL,
    Description TEXT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    MethodName VARCHAR(200) NOT NULL,
    Arguments TEXT NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0
);

CREATE TABLE OtherDevice(
    ID INT AUTO_INCREMENT NOT NULL,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    IsConcentrator TINYINT NOT NULL DEFAULT 0,
    CompanyID INT NULL,
    VendorDeviceID INT NULL,
    Longitude DECIMAL(9, 6) NULL,
    Latitude DECIMAL(9, 6) NULL,
    InterconnectionID INT NULL,
    Planned TINYINT NOT NULL DEFAULT 0,
    Desired TINYINT NOT NULL DEFAULT 0,
    InProgress TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_OtherDevice PRIMARY KEY (ID ASC)
);

CREATE TABLE Device(
    NodeID NCHAR(36) NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    ParentID INT NULL,
    UniqueID NCHAR(36) NULL,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    OriginalSource VARCHAR(200) NULL,
    IsConcentrator TINYINT NOT NULL DEFAULT 0,
    CompanyID INT NULL,
    HistorianID INT NULL,
    AccessID INT NOT NULL DEFAULT 0,
    VendorDeviceID INT NULL,
    ProtocolID INT NULL,
    Longitude DECIMAL(9, 6) NULL,
    Latitude DECIMAL(9, 6) NULL,
    InterconnectionID INT NULL,
    ConnectionString TEXT NULL,
    TimeZone VARCHAR(200) NULL,
    FramesPerSecond INT NULL DEFAULT 30,
    TimeAdjustmentTicks BIGINT NOT NULL DEFAULT 0,
    DataLossInterval DOUBLE NOT NULL DEFAULT 5,
    AllowedParsingExceptions INT NOT NULL DEFAULT 10,
    ParsingExceptionWindow DOUBLE NOT NULL DEFAULT 5,
    DelayedConnectionInterval DOUBLE NOT NULL DEFAULT 5,
    AllowUseOfCachedConfiguration TINYINT NOT NULL DEFAULT 1,
    AutoStartDataParsingSequence TINYINT NOT NULL DEFAULT 1,
    SkipDisableRealTimeData TINYINT NOT NULL DEFAULT 0,
    MeasurementReportingInterval INT NOT NULL DEFAULT 100000,
    ConnectOnDemand TINYINT NOT NULL DEFAULT 1,
    ContactList TEXT NULL,
    MeasuredLines INT NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,	
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_Device PRIMARY KEY (ID ASC),
    CONSTRAINT IX_Device_UniqueID UNIQUE KEY (UniqueID ASC),
    CONSTRAINT IX_Device_NodeID_Acronym UNIQUE KEY (NodeID ASC, Acronym ASC)
);

CREATE TABLE VendorDevice(
    ID INT AUTO_INCREMENT NOT NULL,
    VendorID INT NOT NULL DEFAULT 10,
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    URL TEXT NULL,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_VendorDevice PRIMARY KEY (ID ASC)
);

CREATE TABLE OutputStreamDeviceDigital(
    NodeID NCHAR(36) NOT NULL,
    OutputStreamDeviceID INT NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    Label TEXT NOT NULL,
    MaskValue INT NOT NULL DEFAULT 0,
    LoadOrder INT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_OutputStreamDeviceDigital PRIMARY KEY (ID ASC)
);

CREATE TABLE OutputStreamDevicePhasor(
    NodeID NCHAR(36) NOT NULL,
    OutputStreamDeviceID INT NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    Label VARCHAR(200) NOT NULL,
    Type NCHAR(1) NOT NULL DEFAULT N'V',
    Phase NCHAR(1) NOT NULL DEFAULT N'+',
    ScalingValue INT NOT NULL DEFAULT 0,
    LoadOrder INT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_OutputStreamDevicePhasor PRIMARY KEY (ID ASC)
);

CREATE TABLE OutputStreamDeviceAnalog(
    NodeID NCHAR(36) NOT NULL,
    OutputStreamDeviceID INT NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    Label VARCHAR(200) NOT NULL,
    Type INT NOT NULL DEFAULT 0,
    ScalingValue INT NOT NULL DEFAULT 0,
    LoadOrder INT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_OutputStreamDeviceAnalog PRIMARY KEY (ID ASC)
);

CREATE TABLE Measurement(
    PointID BIGINT AUTO_INCREMENT NOT NULL,
    SignalID NCHAR(36) NOT NULL DEFAULT '',
    HistorianID INT NULL,
    DeviceID INT NULL,
    PointTag VARCHAR(200) NOT NULL,
    AlternateTag TEXT NULL,
    SignalTypeID INT NOT NULL,
    PhasorSourceIndex INT NULL,
    SignalReference VARCHAR(200) NOT NULL,
    Adder DOUBLE NOT NULL DEFAULT 0.0,
    Multiplier DOUBLE NOT NULL DEFAULT 1.0,
    Description TEXT NULL,
    Subscribed TINYINT NOT NULL DEFAULT 0,
    Internal TINYINT NOT NULL DEFAULT 1,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_Measurement PRIMARY KEY (SignalID ASC),
    CONSTRAINT IX_Measurement UNIQUE KEY (PointID ASC)
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
    FramesPerSecond INT NULL,
    ProtocolAcronym VARCHAR(200) NULL,
    ProtocolType VARCHAR(200) NOT NULL DEFAULT 'Frame',
    PhasorID INT NULL,
    PhasorType NCHAR(1) NULL,
    Phase NCHAR(1) NULL,
    Adder DOUBLE NOT NULL DEFAULT 0.0,
    Multiplier DOUBLE NOT NULL DEFAULT 1.0,
    CompanyAcronym VARCHAR(200) NULL,
    Longitude DECIMAL(9, 6) NULL,
    Latitude DECIMAL(9, 6) NULL,
    Description TEXT NULL,
    Enabled TINYINT NOT NULL DEFAULT 0
);

CREATE TABLE Statistic(
    ID INT AUTO_INCREMENT NOT NULL,
    Source VARCHAR(20) NOT NULL,
    SignalIndex INT NOT NULL,
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    MethodName VARCHAR(200) NOT NULL,
    Arguments TEXT NULL,
    Enabled TINYINT NOT NULL DEFAULT 0,
    DataType VARCHAR(200) NULL,
    DisplayFormat VARCHAR(200) NULL,
    IsConnectedState TINYINT NOT NULL DEFAULT 0,
    LoadOrder INT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Statistic PRIMARY KEY (ID ASC),
    CONSTRAINT IX_Statistic_Source_SignalIndex UNIQUE KEY (Source ASC, SignalIndex ASC)
);

CREATE TABLE OutputStreamMeasurement(
    NodeID NCHAR(36) NOT NULL,
    AdapterID INT NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    HistorianID INT NULL,
    PointID BIGINT NOT NULL,
    SignalReference VARCHAR(200) NOT NULL,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_OutputStreamMeasurement PRIMARY KEY (ID ASC)
);

CREATE TABLE OutputStreamDevice(
    NodeID NCHAR(36) NOT NULL,
    AdapterID INT NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    IDCode INT NOT NULL DEFAULT 0,
    Acronym VARCHAR(200) NOT NULL,
    BpaAcronym VARCHAR(4) NULL,
    Name VARCHAR(200) NOT NULL,
    PhasorDataFormat VARCHAR(15) NULL,
    FrequencyDataFormat VARCHAR(15) NULL,
    AnalogDataFormat VARCHAR(15) NULL,
    CoordinateFormat VARCHAR(15) NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_OutputStreamDevice PRIMARY KEY (ID ASC)
);

CREATE TABLE Phasor(
    ID INT AUTO_INCREMENT NOT NULL,
    DeviceID INT NOT NULL,
    Label VARCHAR(200) NOT NULL,
    Type NCHAR(1) NOT NULL DEFAULT N'V',
    Phase NCHAR(1) NOT NULL DEFAULT N'+',
    DestinationPhasorID INT NULL,
    SourceIndex INT NOT NULL DEFAULT 0,
    BaseKV INT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_Phasor PRIMARY KEY (ID ASC),
    CONSTRAINT IX_Phasor_DeviceID_SourceIndex UNIQUE KEY (DeviceID ASC, SourceIndex ASC)
);

CREATE TABLE CalculatedMeasurement(
    NodeID NCHAR(36) NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    ConnectionString TEXT NULL,
    ConfigSection VARCHAR(200) NULL,
    InputMeasurements TEXT NULL,
    OutputMeasurements TEXT NULL,
    MinimumMeasurementsToUse INT NOT NULL DEFAULT -1,
    FramesPerSecond INT NOT NULL DEFAULT 30,
    LagTime DOUBLE NOT NULL DEFAULT 3.0,
    LeadTime DOUBLE NOT NULL DEFAULT 1.0,
    UseLocalClockAsRealTime TINYINT NOT NULL DEFAULT 0,
    AllowSortsByArrival TINYINT NOT NULL DEFAULT 1,
    IgnoreBadTimeStamps TINYINT NOT NULL DEFAULT 0,
    TimeResolution INT NOT NULL DEFAULT 10000,
    AllowPreemptivePublishing TINYINT NOT NULL DEFAULT 1,
    PerformTimeReasonabilityCheck TINYINT NOT NULL DEFAULT 1,
    DownsamplingMethod VARCHAR(15) NOT NULL DEFAULT N'LastReceived',
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_CalculatedMeasurement PRIMARY KEY (ID ASC)
);

CREATE TABLE CustomActionAdapter(
    NodeID NCHAR(36) NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    AdapterName VARCHAR(200) NOT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    ConnectionString TEXT NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_CustomActionAdapter PRIMARY KEY (ID ASC)
);

CREATE TABLE Historian(
    NodeID NCHAR(36) NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    AssemblyName TEXT NULL,
    TypeName TEXT NULL,
    ConnectionString TEXT NULL,
    IsLocal TINYINT NOT NULL DEFAULT 1,
    MeasurementReportingInterval INT NOT NULL DEFAULT 100000,
    Description TEXT NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_Historian PRIMARY KEY (ID ASC)
);

CREATE TABLE CustomInputAdapter(
    NodeID NCHAR(36) NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    AdapterName VARCHAR(200) NOT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    ConnectionString TEXT NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_CustomInputAdapter PRIMARY KEY (ID ASC)
);

CREATE TABLE CustomFilterAdapter(
    NodeID NCHAR(36) NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    AdapterName VARCHAR(200) NOT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    ConnectionString TEXT NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_CustomFilterAdapter PRIMARY KEY (ID ASC)
);

CREATE TABLE OutputStream(
    NodeID NCHAR(36) NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    Type INT NOT NULL DEFAULT 0,
    ConnectionString TEXT NULL,
    DataChannel TEXT NULL,
    CommandChannel TEXT NULL,
    IDCode INT NOT NULL DEFAULT 0,
    AutoPublishConfigFrame TINYINT NOT NULL DEFAULT 0,
    AutoStartDataChannel TINYINT NOT NULL DEFAULT 1,
    NominalFrequency INT NOT NULL DEFAULT 60,
    FramesPerSecond INT NOT NULL DEFAULT 30,
    LagTime DOUBLE NOT NULL DEFAULT 3.0,
    LeadTime DOUBLE NOT NULL DEFAULT 1.0,
    UseLocalClockAsRealTime TINYINT NOT NULL DEFAULT 0,
    AllowSortsByArrival TINYINT NOT NULL DEFAULT 1,
    IgnoreBadTimeStamps TINYINT NOT NULL DEFAULT 0,
    TimeResolution INT NOT NULL DEFAULT 330000,
    AllowPreemptivePublishing TINYINT NOT NULL DEFAULT 1,
    PerformTimeReasonabilityCheck TINYINT NOT NULL DEFAULT 1,
    DownsamplingMethod VARCHAR(15) NOT NULL DEFAULT N'LastReceived',
    DataFormat VARCHAR(15) NOT NULL DEFAULT N'FloatingPoint',
    CoordinateFormat VARCHAR(15) NOT NULL DEFAULT N'Polar',
    CurrentScalingValue INT NOT NULL DEFAULT 2423,
    VoltageScalingValue INT NOT NULL DEFAULT 2725785,
    AnalogScalingValue INT NOT NULL DEFAULT 1373291,
    DigitalMaskValue INT NOT NULL DEFAULT -65536,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_OutputStream PRIMARY KEY (ID ASC),
    CONSTRAINT IX_OutputStream_NodeID_Acronym UNIQUE KEY (NodeID ASC, Acronym ASC)
);

CREATE TABLE PowerCalculation(
    NodeID NCHAR(36) NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    CircuitDescription TEXT NULL,
    VoltageAngleSignalID NCHAR(36) NOT NULL,
    VoltageMagSignalID NCHAR(36) NOT NULL,
    CurrentAngleSignalID NCHAR(36) NOT NULL,
    CurrentMagSignalID NCHAR(36) NOT NULL,
    ActivePowerOutputSignalID NCHAR(36) NULL,
    ReactivePowerOutputSignalID NCHAR(36) NULL,
    ApparentPowerOutputSignalID NCHAR(36) NULL,
    Enabled TINYINT NOT NULL,
    CONSTRAINT PK_PowerCalculation PRIMARY KEY (ID ASC)
);

CREATE TABLE Alarm(
    NodeID NCHAR(36) NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    TagName VARCHAR(200) NOT NULL,
    SignalID NCHAR(36) NOT NULL,
    AssociatedMeasurementID NCHAR(36) NULL,
    Description TEXT NULL,
    Severity INT NOT NULL,
    Operation INT NOT NULL,
    SetPoint DOUBLE NULL,
    Tolerance DOUBLE NULL,
    Delay DOUBLE NULL,
    Hysteresis DOUBLE NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_Alarm PRIMARY KEY (ID ASC)
);

CREATE TABLE AlarmLog(
    ID INT AUTO_INCREMENT NOT NULL,
    SignalID NCHAR(36) NOT NULL,
    PreviousState INT NULL,
    NewState INT NULL,
    Ticks BIGINT NOT NULL,
    Timestamp DATETIME NOT NULL,
    Value FLOAT NOT NULL,
    CONSTRAINT PK_AlarmLog PRIMARY KEY (ID ASC)
);

CREATE TABLE CustomOutputAdapter(
    NodeID NCHAR(36) NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    AdapterName VARCHAR(200) NOT NULL,
    AssemblyName TEXT NOT NULL,
    TypeName TEXT NOT NULL,
    ConnectionString TEXT NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_CustomOutputAdapter PRIMARY KEY (ID ASC)
);

CREATE TABLE AccessLog (
    ID INT(11) NOT NULL AUTO_INCREMENT,
    UserName VARCHAR(200) NOT NULL,
    AccessGranted TINYINT NOT NULL,
    CreatedOn DATETIME NULL,
    CONSTRAINT PK_AccessLog PRIMARY KEY (ID ASC)
);

CREATE TABLE UserAccount (
    ID NCHAR(36) NOT NULL DEFAULT '',
    Name VARCHAR(200) NOT NULL,
    Password VARCHAR(200) NULL,
    FirstName VARCHAR(200) NULL,
    LastName VARCHAR(200) NULL,
    DefaultNodeID NCHAR(36) NOT NULL,
    Phone VARCHAR(200) NULL,
    Email VARCHAR(200) NULL,
    LockedOut TINYINT NOT NULL DEFAULT 0,
    UseADAuthentication TINYINT NOT NULL DEFAULT 1,
    ChangePasswordOn DATETIME NULL,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_UserAccount PRIMARY KEY (ID ASC),
    CONSTRAINT IX_UserAccount UNIQUE KEY (Name ASC)
);

CREATE TABLE SecurityGroup (
    ID NCHAR(36) NOT NULL DEFAULT '',
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_SecurityGroup PRIMARY KEY (ID ASC),
    CONSTRAINT IX_SecurityGroup UNIQUE KEY (Name ASC)
);

CREATE TABLE ApplicationRole (
    ID NCHAR(36) NOT NULL DEFAULT '',
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    NodeID NCHAR(36) NOT NULL,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_ApplicationRole PRIMARY KEY (ID ASC),
    CONSTRAINT IX_ApplicationRole UNIQUE KEY (NodeID ASC, Name ASC)
);

CREATE TABLE ApplicationRoleSecurityGroup (
    ApplicationRoleID NCHAR(36) NOT NULL,
    SecurityGroupID NCHAR(36) NOT NULL  
);

CREATE TABLE ApplicationRoleUserAccount (
    ApplicationRoleID NCHAR(36) NOT NULL,
    UserAccountID NCHAR(36) NOT NULL  
);

CREATE TABLE SecurityGroupUserAccount (
    SecurityGroupID NCHAR(36) NOT NULL,
    UserAccountID NCHAR(36) NOT NULL  
);

-- ----------------------------------------------------------------------------

CREATE TABLE Subscriber (
    NodeID NCHAR(36) NOT NULL,
    ID NCHAR(36) NOT NULL DEFAULT '',
    Acronym VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NULL,
    SharedSecret VARCHAR(200) NULL,
    AuthKey TEXT NULL,
    ValidIPAddresses TEXT NULL,
    RemoteCertificateFile VARCHAR(500) NULL,
    ValidPolicyErrors VARCHAR(200) NULL,
    ValidChainFlags VARCHAR(500) NULL,
    AccessControlFilter TEXT NULL,
    Enabled TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_Subscriber PRIMARY KEY (NodeID ASC, ID ASC),
    CONSTRAINT IX_Subscriber_NodeID_Acronym UNIQUE KEY (NodeID ASC, Acronym ASC)
);

CREATE TABLE SubscriberMeasurement(
    NodeID NCHAR(36) NOT NULL,
    SubscriberID NCHAR(36) NOT NULL,
    SignalID NCHAR(36) NOT NULL,
    Allowed TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_SubscriberMeasurement PRIMARY KEY (NodeID ASC, SubscriberID ASC, SignalID ASC)
);

CREATE TABLE SubscriberMeasurementGroup (
    NodeID NCHAR(36) NOT NULL,
    SubscriberID NCHAR(36) NOT NULL,
    MeasurementGroupID INT NOT NULL,
    Allowed TINYINT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_SubscriberMeasurementGroup PRIMARY KEY (NodeID ASC, SubscriberID ASC, MeasurementGroupID ASC)
);

CREATE TABLE MeasurementGroup (
    NodeID NCHAR(36) NOT NULL,
    ID INT AUTO_INCREMENT NOT NULL,
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    FilterExpression TEXT NULL,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_MeasurementGroup PRIMARY KEY (ID ASC)
);

CREATE TABLE MeasurementGroupMeasurement (
    NodeID NCHAR(36) NOT NULL,
    MeasurementGroupID INT NOT NULL,
    SignalID NCHAR(36) NOT NULL,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_MeasurementGroupMeasurement PRIMARY KEY (NodeID ASC, MeasurementGroupID ASC, SignalID ASC)
);

ALTER TABLE Subscriber ADD CONSTRAINT FK_Subscriber_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE SubscriberMeasurement ADD CONSTRAINT FK_SubscriberMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE SubscriberMeasurement ADD CONSTRAINT FK_SubscriberMeasurement_Measurement FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE SubscriberMeasurement ADD CONSTRAINT FK_SubscriberMeasurement_Subscriber FOREIGN KEY(NodeID, SubscriberID) REFERENCES Subscriber (NodeID, ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE SubscriberMeasurementGroup ADD CONSTRAINT FK_SubscriberMeasurementGroup_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE SubscriberMeasurementGroup ADD CONSTRAINT FK_SubscriberMeasurementGroup_Subscriber FOREIGN KEY(NodeID, SubscriberID) REFERENCES Subscriber (NodeID, ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE SubscriberMeasurementGroup ADD CONSTRAINT FK_SubscriberMeasurementGroup_MeasurementGroup FOREIGN KEY(MeasurementGroupID) REFERENCES MeasurementGroup (ID);

ALTER TABLE MeasurementGroup ADD CONSTRAINT FK_MeasurementGroup_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE MeasurementGroupMeasurement ADD CONSTRAINT FK_MeasurementGroupMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE MeasurementGroupMeasurement ADD CONSTRAINT FK_MeasurementGroupMeasurement_Measurement FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE MeasurementGroupMeasurement ADD CONSTRAINT FK_MeasurementGroupMeasurement_MeasurementGroup FOREIGN KEY(MeasurementGroupID) REFERENCES MeasurementGroup (ID) ON DELETE CASCADE ON UPDATE CASCADE;

-- ----------------------------------------------------------------------------

ALTER TABLE Node ADD CONSTRAINT FK_Node_Company FOREIGN KEY(CompanyID) REFERENCES Company (ID);

ALTER TABLE DataOperation ADD CONSTRAINT FK_DataOperation_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE OtherDevice ADD CONSTRAINT FK_OtherDevice_Company FOREIGN KEY(CompanyID) REFERENCES Company (ID);

ALTER TABLE OtherDevice ADD CONSTRAINT FK_OtherDevice_Interconnection FOREIGN KEY(InterconnectionID) REFERENCES Interconnection (ID);

ALTER TABLE OtherDevice ADD CONSTRAINT FK_OtherDevice_VendorDevice FOREIGN KEY(VendorDeviceID) REFERENCES VendorDevice (ID);

ALTER TABLE Device ADD CONSTRAINT FK_Device_Company FOREIGN KEY(CompanyID) REFERENCES Company (ID);

ALTER TABLE Device ADD CONSTRAINT FK_Device_Device FOREIGN KEY(ParentID) REFERENCES Device (ID);

ALTER TABLE Device ADD CONSTRAINT FK_Device_Interconnection FOREIGN KEY(InterconnectionID) REFERENCES Interconnection (ID);

ALTER TABLE Device ADD CONSTRAINT FK_Device_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE Device ADD CONSTRAINT FK_Device_Protocol FOREIGN KEY(ProtocolID) REFERENCES Protocol (ID);

ALTER TABLE Device ADD CONSTRAINT FK_Device_VendorDevice FOREIGN KEY(VendorDeviceID) REFERENCES VendorDevice (ID);

ALTER TABLE VendorDevice ADD CONSTRAINT FK_VendorDevice_Vendor FOREIGN KEY(VendorID) REFERENCES Vendor (ID);

ALTER TABLE OutputStreamDeviceDigital ADD CONSTRAINT FK_OutputStreamDeviceDigital_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputStreamDeviceDigital ADD CONSTRAINT FK_OutputStreamDeviceDigital_OutputStreamDevice FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE;

ALTER TABLE OutputStreamDevicePhasor ADD CONSTRAINT FK_OutputStreamDevicePhasor_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputStreamDevicePhasor ADD CONSTRAINT FK_OutputStreamDevicePhasor_OutputStreamDevice FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE;

ALTER TABLE OutputStreamDeviceAnalog ADD CONSTRAINT FK_OutputStreamDeviceAnalog_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputStreamDeviceAnalog ADD CONSTRAINT FK_OutputStreamDeviceAnalog_OutputStreamDevice FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE;

ALTER TABLE Measurement ADD CONSTRAINT FK_Measurement_Device FOREIGN KEY(DeviceID) REFERENCES Device (ID) ON DELETE CASCADE;

ALTER TABLE Measurement ADD CONSTRAINT FK_Measurement_SignalType FOREIGN KEY(SignalTypeID) REFERENCES SignalType (ID);

ALTER TABLE ImportedMeasurement ADD CONSTRAINT FK_ImportedMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE OutputStreamMeasurement ADD CONSTRAINT FK_OutputStreamMeasurement_Historian FOREIGN KEY(HistorianID) REFERENCES Historian (ID);

ALTER TABLE OutputStreamMeasurement ADD CONSTRAINT FK_OutputStreamMeasurement_Measurement FOREIGN KEY(PointID) REFERENCES Measurement (PointID) ON DELETE CASCADE;

ALTER TABLE OutputStreamMeasurement ADD CONSTRAINT FK_OutputStreamMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputStreamMeasurement ADD CONSTRAINT FK_OutputStreamMeasurement_OutputStream FOREIGN KEY(AdapterID) REFERENCES OutputStream (ID) ON DELETE CASCADE;

ALTER TABLE OutputStreamDevice ADD CONSTRAINT FK_OutputStreamDevice_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputStreamDevice ADD CONSTRAINT FK_OutputStreamDevice_OutputStream FOREIGN KEY(AdapterID) REFERENCES OutputStream (ID) ON DELETE CASCADE;

ALTER TABLE Phasor ADD CONSTRAINT FK_Phasor_Device FOREIGN KEY(DeviceID) REFERENCES Device (ID) ON DELETE CASCADE;

ALTER TABLE Phasor ADD CONSTRAINT FK_Phasor_Phasor FOREIGN KEY(DestinationPhasorID) REFERENCES Phasor (ID);

ALTER TABLE CalculatedMeasurement ADD CONSTRAINT FK_CalculatedMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE CustomActionAdapter ADD CONSTRAINT FK_CustomActionAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE Historian ADD CONSTRAINT FK_Historian_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE CustomInputAdapter ADD CONSTRAINT FK_CustomInputAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE CustomFilterAdapter ADD CONSTRAINT FK_CustomFilterAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE OutputStream ADD CONSTRAINT FK_OutputStream_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement1 FOREIGN KEY(ApparentPowerOutputSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement2 FOREIGN KEY(CurrentAngleSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement3 FOREIGN KEY(CurrentMagSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement4 FOREIGN KEY(ReactivePowerOutputSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement5 FOREIGN KEY(ActivePowerOutputSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement6 FOREIGN KEY(VoltageAngleSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement7 FOREIGN KEY(VoltageMagSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE Alarm ADD CONSTRAINT FK_Alarm_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE Alarm ADD CONSTRAINT FK_Alarm_Measurement_SignalID FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE Alarm ADD CONSTRAINT FK_Alarm_Measurement_AssociatedMeasurementID FOREIGN KEY(AssociatedMeasurementID) REFERENCES Measurement (SignalID);

ALTER TABLE AlarmLog ADD CONSTRAINT FK_AlarmLog_Measurement FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE AlarmLog ADD CONSTRAINT FK_AlarmLog_Alarm_PreviousState FOREIGN KEY(PreviousState) REFERENCES Alarm (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE AlarmLog ADD CONSTRAINT FK_AlarmLog_Alarm_NewState FOREIGN KEY(NewState) REFERENCES Alarm (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE CustomOutputAdapter ADD CONSTRAINT FK_CustomOutputAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE ApplicationRoleSecurityGroup ADD CONSTRAINT FK_ApplicationRoleSecurityGroup_ApplicationRole FOREIGN KEY (ApplicationRoleID) REFERENCES ApplicationRole (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE ApplicationRoleSecurityGroup ADD CONSTRAINT FK_ApplicationRoleSecurityGroup_SecurityGroup FOREIGN KEY (SecurityGroupID) REFERENCES SecurityGroup (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE UserAccount ADD CONSTRAINT FK_UserAccount FOREIGN KEY (DefaultNodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE ApplicationRole ADD CONSTRAINT FK_ApplicationRole FOREIGN KEY (NodeID) REFERENCES Node (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE ApplicationRoleUserAccount ADD CONSTRAINT FK_ApplicationRoleUserAccount_UserAccount FOREIGN KEY (UserAccountID) REFERENCES UserAccount (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE ApplicationRoleUserAccount ADD CONSTRAINT FK_ApplicationRoleUserAccount_ApplicationRole FOREIGN KEY (ApplicationRoleID) REFERENCES ApplicationRole (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE SecurityGroupUserAccount ADD CONSTRAINT FK_SecurityGroupUserAccount_UserAccount FOREIGN KEY (UserAccountID) REFERENCES UserAccount (ID) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE SecurityGroupUserAccount ADD CONSTRAINT FK_SecurityGroupUserAccount_SecurityGroup FOREIGN KEY (SecurityGroupID) REFERENCES SecurityGroup (ID) ON DELETE CASCADE ON UPDATE CASCADE;

CREATE VIEW NodeInfo
AS
SELECT Node.ID AS NodeID, Node.Name, Company.Name AS CompanyName, Node.Longitude, Node.Latitude, Node.Description,
    Node.ImagePath, Node.Settings, Node.MenuType, Node.MenuData, Node.Master, Node.Enabled
FROM Node LEFT OUTER JOIN Company ON Node.CompanyID = Company.ID;

CREATE VIEW RuntimeOutputStreamMeasurement
AS
SELECT OutputStreamMeasurement.NodeID, Runtime.ID AS AdapterID, Historian.Acronym AS Historian, 
    OutputStreamMeasurement.PointID, OutputStreamMeasurement.SignalReference
FROM OutputStreamMeasurement LEFT OUTER JOIN
    Historian ON OutputStreamMeasurement.HistorianID = Historian.ID LEFT OUTER JOIN
    Runtime ON OutputStreamMeasurement.AdapterID = Runtime.SourceID AND Runtime.SourceTable = N'OutputStream'
ORDER BY OutputStreamMeasurement.HistorianID, OutputStreamMeasurement.PointID;

CREATE VIEW RuntimeHistorian
AS
SELECT Historian.NodeID, Runtime.ID, Historian.Acronym AS AdapterName,
    COALESCE(NULLIF(TRIM(Historian.AssemblyName), ''), N'HistorianAdapters.dll') AS AssemblyName, 
    COALESCE(NULLIF(TRIM(Historian.TypeName), ''), IF(IsLocal = 1, N'HistorianAdapters.LocalOutputAdapter', N'HistorianAdapters.RemoteOutputAdapter')) AS TypeName, 
    CONCAT_WS(';', Historian.ConnectionString, CONCAT(N'instanceName=', Historian.Acronym), CONCAT(N'sourceids=', Historian.Acronym),
    CONCAT(N'measurementReportingInterval=', CAST(Historian.MeasurementReportingInterval AS CHAR))) AS ConnectionString
FROM Historian LEFT OUTER JOIN
    Runtime ON Historian.ID = Runtime.SourceID AND Runtime.SourceTable = N'Historian'
WHERE (Historian.Enabled <> 0)
ORDER BY Historian.LoadOrder;

CREATE VIEW RuntimeDevice
AS
SELECT Device.NodeID, Runtime.ID, Device.Acronym AS AdapterName, Protocol.AssemblyName, Protocol.TypeName,
    CONCAT_WS(';', Device.ConnectionString, CONCAT(N'isConcentrator=', CAST(Device.IsConcentrator AS CHAR)),
    CONCAT(N'accessID=', CAST(Device.AccessID AS CHAR)),
    IF(Device.TimeZone IS NULL, N'', CONCAT(N'timeZone=', Device.TimeZone)),
    CONCAT(N'timeAdjustmentTicks=', CAST(Device.TimeAdjustmentTicks AS CHAR)),
    IF(Protocol.Acronym IS NULL, N'', CONCAT(N'phasorProtocol=', Protocol.Acronym)),
    CONCAT(N'dataLossInterval=', CAST(Device.DataLossInterval AS CHAR)),
    CONCAT(N'allowedParsingExceptions=', CAST(Device.AllowedParsingExceptions AS CHAR)),
    CONCAT(N'parsingExceptionWindow=', CAST(Device.ParsingExceptionWindow AS CHAR)),
    CONCAT(N'delayedConnectionInterval=', CAST(Device.DelayedConnectionInterval AS CHAR)),
    CONCAT(N'allowUseOfCachedConfiguration=', CAST(Device.AllowUseOfCachedConfiguration AS CHAR)),
    CONCAT(N'autoStartDataParsingSequence=', CAST(Device.AutoStartDataParsingSequence AS CHAR)),
    CONCAT(N'skipDisableRealTimeData=', CAST(Device.SkipDisableRealTimeData AS CHAR)),
    CONCAT(N'measurementReportingInterval=', CAST(Device.MeasurementReportingInterval AS CHAR)),
    CONCAT(N'connectOnDemand=', CAST(Device.ConnectOnDemand AS CHAR))) AS ConnectionString
FROM Device LEFT OUTER JOIN
    Protocol ON Device.ProtocolID = Protocol.ID LEFT OUTER JOIN
    Runtime ON Device.ID = Runtime.SourceID AND Runtime.SourceTable = N'Device'
WHERE (Device.Enabled <> 0 AND Device.ParentID IS NULL)
ORDER BY Device.LoadOrder;

CREATE VIEW RuntimeCustomOutputAdapter
AS
SELECT CustomOutputAdapter.NodeID, Runtime.ID, CustomOutputAdapter.AdapterName, 
    TRIM(CustomOutputAdapter.AssemblyName) AS AssemblyName, TRIM(CustomOutputAdapter.TypeName) AS TypeName, CustomOutputAdapter.ConnectionString
FROM CustomOutputAdapter LEFT OUTER JOIN
    Runtime ON CustomOutputAdapter.ID = Runtime.SourceID AND Runtime.SourceTable = N'CustomOutputAdapter'
WHERE (CustomOutputAdapter.Enabled <> 0)
ORDER BY CustomOutputAdapter.LoadOrder;

CREATE VIEW RuntimeInputStreamDevice
AS
SELECT Device.NodeID, Runtime_P.ID AS ParentID, Runtime.ID, Device.Acronym, Device.Name, Device.AccessID
FROM Device LEFT OUTER JOIN
    Runtime ON Device.ID = Runtime.SourceID AND Runtime.SourceTable = N'Device' LEFT OUTER JOIN
    Runtime AS Runtime_P ON Device.ParentID = Runtime_P.SourceID AND Runtime_P.SourceTable = N'Device'
WHERE (Device.IsConcentrator = 0) AND (Device.Enabled <> 0) AND (Device.ParentID IS NOT NULL)
ORDER BY Device.LoadOrder;

CREATE VIEW RuntimeCustomInputAdapter
AS
SELECT CustomInputAdapter.NodeID, Runtime.ID, CustomInputAdapter.AdapterName, 
    TRIM(CustomInputAdapter.AssemblyName) AS AssemblyName, TRIM(CustomInputAdapter.TypeName) AS TypeName, CustomInputAdapter.ConnectionString
FROM CustomInputAdapter LEFT OUTER JOIN
    Runtime ON CustomInputAdapter.ID = Runtime.SourceID AND Runtime.SourceTable = N'CustomInputAdapter'
WHERE (CustomInputAdapter.Enabled <> 0)
ORDER BY CustomInputAdapter.LoadOrder;

CREATE VIEW RuntimeCustomFilterAdapter
AS
SELECT CustomFilterAdapter.NodeID, Runtime.ID, CustomFilterAdapter.AdapterName, 
    TRIM(CustomFilterAdapter.AssemblyName) AS AssemblyName, TRIM(CustomFilterAdapter.TypeName) AS TypeName, CustomFilterAdapter.ConnectionString
FROM CustomFilterAdapter LEFT OUTER JOIN
    Runtime ON CustomFilterAdapter.ID = Runtime.SourceID AND Runtime.SourceTable = N'CustomFilterAdapter'
WHERE (CustomFilterAdapter.Enabled <> 0)
ORDER BY CustomFilterAdapter.LoadOrder;

CREATE VIEW RuntimeOutputStreamDevice
AS
SELECT OutputStreamDevice.NodeID, Runtime.ID AS ParentID, OutputStreamDevice.ID, OutputStreamDevice.IDCode, OutputStreamDevice.Acronym, 
    OutputStreamDevice.BpaAcronym, OutputStreamDevice.Name, NULLIF(OutputStreamDevice.PhasorDataFormat, '') AS PhasorDataFormat, NULLIF(OutputStreamDevice.FrequencyDataFormat, '') AS FrequencyDataFormat,
    NULLIF(OutputStreamDevice.AnalogDataFormat, '') AS AnalogDataFormat, NULLIF(OutputStreamDevice.CoordinateFormat, '') AS CoordinateFormat, OutputStreamDevice.LoadOrder
FROM OutputStreamDevice LEFT OUTER JOIN
    Runtime ON OutputStreamDevice.AdapterID = Runtime.SourceID AND Runtime.SourceTable = N'OutputStream'
WHERE (OutputStreamDevice.Enabled <> 0)
ORDER BY OutputStreamDevice.LoadOrder;

CREATE VIEW RuntimeOutputStream
AS
SELECT OutputStream.NodeID, Runtime.ID, OutputStream.Acronym AS AdapterName, 
    N'PhasorProtocolAdapters.dll' AS AssemblyName, 
    CASE Type WHEN 1 THEN N'PhasorProtocolAdapters.BpaPdcStream.Concentrator' WHEN 2 THEN N'PhasorProtocolAdapters.Iec61850_90_5.Concentrator' ELSE N'PhasorProtocolAdapters.IeeeC37_118.Concentrator' END AS TypeName,
    CONCAT_WS(';', OutputStream.ConnectionString,
    IF(OutputStream.DataChannel IS NULL, N'', CONCAT(N'dataChannel={', OutputStream.DataChannel, N'}')),
    IF(OutputStream.CommandChannel IS NULL, N'', CONCAT(N'commandChannel={', OutputStream.CommandChannel, N'}')),
    CONCAT(N'idCode=', CAST(OutputStream.IDCode AS CHAR)),
    CONCAT(N'autoPublishConfigFrame=', CAST(OutputStream.AutoPublishConfigFrame AS CHAR)),
    CONCAT(N'autoStartDataChannel=', CAST(OutputStream.AutoStartDataChannel AS CHAR)),
    CONCAT(N'nominalFrequency=', CAST(OutputStream.NominalFrequency AS CHAR)),
    CONCAT(N'lagTime=', CAST(OutputStream.LagTime AS CHAR)),
    CONCAT(N'leadTime=', CAST(OutputStream.LeadTime AS CHAR)),
    CONCAT(N'framesPerSecond=', CAST(OutputStream.FramesPerSecond AS CHAR)),
    CONCAT(N'useLocalClockAsRealTime=', CAST(OutputStream.UseLocalClockAsRealTime AS CHAR)),
    CONCAT(N'allowSortsByArrival=', CAST(OutputStream.AllowSortsByArrival AS CHAR)),
    CONCAT(N'ignoreBadTimestamps=', CAST(OutputStream.IgnoreBadTimeStamps AS CHAR)),
    CONCAT(N'timeResolution=', CAST(OutputStream.TimeResolution AS CHAR)),
    CONCAT(N'allowPreemptivePublishing=', CAST(OutputStream.AllowPreemptivePublishing AS CHAR)),
    CONCAT(N'downsamplingMethod=', OutputStream.DownsamplingMethod),
    CONCAT(N'dataFormat=', OutputStream.DataFormat),
    CONCAT(N'coordinateFormat=', OutputStream.CoordinateFormat),
    CONCAT(N'currentScalingValue=', CAST(OutputStream.CurrentScalingValue AS CHAR)),
    CONCAT(N'voltageScalingValue=', CAST(OutputStream.VoltageScalingValue AS CHAR)),
    CONCAT(N'analogScalingValue=', CAST(OutputStream.AnalogScalingValue AS CHAR)),
    CONCAT(N'performTimestampReasonabilityCheck=', CAST(OutputStream.PerformTimeReasonabilityCheck AS CHAR)),
    CONCAT(N'digitalMaskValue=', CAST(OutputStream.DigitalMaskValue AS CHAR))) AS ConnectionString
FROM OutputStream LEFT OUTER JOIN
    Runtime ON OutputStream.ID = Runtime.SourceID AND Runtime.SourceTable = N'OutputStream'
WHERE (OutputStream.Enabled <> 0)
ORDER BY OutputStream.LoadOrder;

CREATE VIEW RuntimeCustomActionAdapter
AS
SELECT CustomActionAdapter.NodeID, Runtime.ID, CustomActionAdapter.AdapterName, 
    TRIM(CustomActionAdapter.AssemblyName) AS AssemblyName, TRIM(CustomActionAdapter.TypeName) AS TypeName, CustomActionAdapter.ConnectionString
FROM CustomActionAdapter LEFT OUTER JOIN
    Runtime ON CustomActionAdapter.ID = Runtime.SourceID AND Runtime.SourceTable = N'CustomActionAdapter'
WHERE (CustomActionAdapter.Enabled <> 0)
ORDER BY CustomActionAdapter.LoadOrder;

CREATE VIEW RuntimeCalculatedMeasurement
AS
SELECT CalculatedMeasurement.NodeID, Runtime.ID, CalculatedMeasurement.Acronym AS AdapterName, 
    TRIM(CalculatedMeasurement.AssemblyName) AS AssemblyName, TRIM(CalculatedMeasurement.TypeName) AS TypeName,
    CONCAT_WS(';', IF (CalculatedMeasurement.ConnectionString IS NULL, N'', CalculatedMeasurement.ConnectionString), IF(ConfigSection IS NULL, N'', CONCAT(N'configurationSection=', ConfigSection)),
    CONCAT(N'minimumMeasurementsToUse=', CAST(CalculatedMeasurement.MinimumMeasurementsToUse AS CHAR)),
    CONCAT(N'framesPerSecond=', CAST(CalculatedMeasurement.FramesPerSecond AS CHAR)),
    CONCAT(N'lagTime=', CAST(CalculatedMeasurement.LagTime AS CHAR)),
    CONCAT(N'leadTime=', CAST(CalculatedMeasurement.LeadTime AS CHAR)),
    IF(InputMeasurements IS NULL, N'', CONCAT(N'inputMeasurementKeys={', InputMeasurements, N'}')),
    IF(OutputMeasurements IS NULL, N'', CONCAT(N'outputMeasurements={', OutputMeasurements, N'}')),
    CONCAT(N'ignoreBadTimestamps=', CAST(CalculatedMeasurement.IgnoreBadTimeStamps AS CHAR)),
    CONCAT(N'timeResolution=', CAST(CalculatedMeasurement.TimeResolution AS CHAR)),
    CONCAT(N'allowPreemptivePublishing=', CAST(CalculatedMeasurement.AllowPreemptivePublishing AS CHAR)),
    CONCAT(N'performTimestampReasonabilityCheck=', CAST(CalculatedMeasurement.PerformTimeReasonabilityCheck AS CHAR)),
    CONCAT(N'downsamplingMethod=', CalculatedMeasurement.DownsamplingMethod),
    CONCAT(N'useLocalClockAsRealTime=', CAST(CalculatedMeasurement.UseLocalClockAsRealTime AS CHAR))) AS ConnectionString
FROM CalculatedMeasurement LEFT OUTER JOIN
    Runtime ON CalculatedMeasurement.ID = Runtime.SourceID AND Runtime.SourceTable = N'CalculatedMeasurement'
WHERE (CalculatedMeasurement.Enabled <> 0)
ORDER BY CalculatedMeasurement.LoadOrder;

CREATE VIEW ActiveMeasurement
AS
SELECT Node.ID AS NodeID, COALESCE(Device.NodeID, Historian.NodeID) AS SourceNodeID, CONCAT_WS(':', COALESCE(Historian.Acronym, Device.Acronym, '__'), 
    CAST(Measurement.PointID AS CHAR)) AS ID, Measurement.SignalID, Measurement.PointTag, Measurement.AlternateTag, Measurement.SignalReference, Measurement.Internal, Measurement.Subscribed,
    Device.Acronym AS Device, CASE WHEN Device.IsConcentrator = 0 AND Device.ParentID IS NOT NULL THEN RuntimeP.ID ELSE Runtime.ID END AS DeviceID, COALESCE(Device.FramesPerSecond, 30) AS FramesPerSecond, 
    Protocol.Acronym AS Protocol, Protocol.Type AS ProtocolType, SignalType.Acronym AS SignalType, SignalType.EngineeringUnits, 
    Phasor.ID AS PhasorID, Phasor.Label AS PhasorLabel, Phasor.Type AS PhasorType, Phasor.Phase, Phasor.BaseKV,
    Measurement.Adder, Measurement.Multiplier, Company.Acronym AS Company, 
    Device.Longitude, Device.Latitude, Measurement.Description, Measurement.UpdatedOn
FROM Company RIGHT OUTER JOIN
    Device ON Company.ID = Device.CompanyID RIGHT OUTER JOIN
    Measurement LEFT OUTER JOIN
    SignalType ON Measurement.SignalTypeID = SignalType.ID ON Device.ID = Measurement.DeviceID LEFT OUTER JOIN
    Phasor ON Measurement.DeviceID = Phasor.DeviceID AND 
    Measurement.PhasorSourceIndex = Phasor.SourceIndex LEFT OUTER JOIN
    Protocol ON Device.ProtocolID = Protocol.ID LEFT OUTER JOIN
    Historian ON Measurement.HistorianID = Historian.ID LEFT OUTER JOIN
    Runtime ON Device.ID = Runtime.SourceID AND Runtime.SourceTable = N'Device' LEFT OUTER JOIN
    Runtime AS RuntimeP ON RuntimeP.SourceID = Device.ParentID AND RuntimeP.SourceTable = N'Device'
    CROSS JOIN Node
WHERE (Device.Enabled <> 0 OR Device.Enabled IS NULL) AND (Measurement.Enabled <> 0)
UNION ALL
SELECT NodeID, SourceNodeID, CONCAT_WS(':', Source, CAST(PointID AS CHAR)) AS ID, SignalID, PointTag,
    AlternateTag, SignalReference, 0 AS Internal, 1 AS Subscribed, NULL AS Device, NULL AS DeviceID,
    FramesPerSecond, ProtocolAcronym AS Protocol, ProtocolType, SignalTypeAcronym AS SignalType, '' AS EngineeringUnits, 
    PhasorID, '' AS PhasorLabel, PhasorType, Phase, 0 AS BaseKV,
    Adder, Multiplier, CompanyAcronym AS Company, 
    Longitude, Latitude, Description, UTC_TIMESTAMP() AS UpdatedOn
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
SELECT Node.ID AS NodeID, 0 AS ID, N'PHASOR!SERVICES' AS AdapterName, N'PhasorProtocolAdapters.dll' AS AssemblyName, N'PhasorProtocolAdapters.CommonPhasorServices' AS TypeName, N'' AS ConnectionString
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

CREATE VIEW IaonFilterAdapter
AS
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeCustomFilterAdapter;
      
CREATE VIEW MeasurementDetail
AS
SELECT     Device.CompanyID, Company.Acronym AS CompanyAcronym, Company.Name AS CompanyName, Measurement.SignalID, 
    Measurement.HistorianID, Historian.Acronym AS HistorianAcronym, Historian.ConnectionString AS HistorianConnectionString, 
    Measurement.PointID, Measurement.PointTag, Measurement.AlternateTag, Measurement.DeviceID,  COALESCE (Device.NodeID, Historian.NodeID) AS NodeID, 
    Device.Acronym AS DeviceAcronym, Device.Name AS DeviceName, COALESCE(Device.FramesPerSecond, 30) AS FramesPerSecond, Device.Enabled AS DeviceEnabled, Device.ContactList, 
    Device.VendorDeviceID, VendorDevice.Name AS VendorDeviceName, VendorDevice.Description AS VendorDeviceDescription, 
    Device.ProtocolID, Protocol.Acronym AS ProtocolAcronym, Protocol.Name AS ProtocolName, Measurement.SignalTypeID, 
    Measurement.PhasorSourceIndex, Phasor.Label AS PhasorLabel, Phasor.Type AS PhasorType, Phasor.Phase, Phasor.BaseKV,
    Measurement.SignalReference, Measurement.Adder, Measurement.Multiplier, Measurement.Description, Measurement.Subscribed, Measurement.Internal, Measurement.Enabled, 
    COALESCE (SignalType.EngineeringUnits, N'') AS EngineeringUnits, SignalType.Source, SignalType.Acronym AS SignalAcronym, 
    SignalType.Name AS SignalName, SignalType.Suffix AS SignalTypeSuffix, Device.Longitude, Device.Latitude,
    CONCAT_WS(':', COALESCE(Historian.Acronym, Device.Acronym, '__'), CAST(Measurement.PointID AS CHAR)) AS ID, Measurement.UpdatedOn
FROM Company RIGHT OUTER JOIN
    Device ON Company.ID = Device.CompanyID RIGHT OUTER JOIN
    Measurement LEFT OUTER JOIN
    SignalType ON Measurement.SignalTypeID = SignalType.ID ON Device.ID = Measurement.DeviceID LEFT OUTER JOIN
    Phasor ON Measurement.DeviceID = Phasor.DeviceID AND 
    Measurement.PhasorSourceIndex = Phasor.SourceIndex LEFT OUTER JOIN
    VendorDevice ON Device.VendorDeviceID = VendorDevice.ID LEFT OUTER JOIN
    Protocol ON Device.ProtocolID = Protocol.ID LEFT OUTER JOIN
    Historian ON Measurement.HistorianID = Historian.ID;

CREATE VIEW HistorianMetadata
AS
SELECT PointID AS HistorianID, IF(SignalAcronym = N'DIGI', 1, 0) AS DataType, PointTag AS Name, SignalReference AS Synonym1, 
    SignalAcronym AS Synonym2, AlternateTag AS Synonym3, Description, VendorDeviceDescription AS HardwareInfo, N'' AS Remarks, 
    HistorianAcronym AS PlantCode, 1 AS UnitNumber, DeviceAcronym AS SystemName, ProtocolID AS SourceID, Enabled, 1 / FramesPerSecond AS ScanRate, 
    0 AS CompressionMinTime, 0 AS CompressionMaxTime, EngineeringUnits,
    CASE SignalAcronym WHEN N'FREQ' THEN 59.95 WHEN N'VPHM' THEN 475000 WHEN N'IPHM' THEN 0 WHEN N'VPHA' THEN -181 WHEN N'IPHA' THEN -181 ELSE 0 END AS LowWarning,
    CASE SignalAcronym WHEN N'FREQ' THEN 60.05 WHEN N'VPHM' THEN 525000 WHEN N'IPHM' THEN 3150 WHEN N'VPHA' THEN 181 WHEN N'IPHA' THEN 181 ELSE 0 END AS HighWarning,
    CASE SignalAcronym WHEN N'FREQ' THEN 59.90 WHEN N'VPHM' THEN 450000 WHEN N'IPHM' THEN 0 WHEN N'VPHA' THEN -181 WHEN N'IPHA' THEN -181 ELSE 0 END AS LowAlarm,
    CASE SignalAcronym WHEN N'FREQ' THEN 60.10 WHEN N'VPHM' THEN 550000 WHEN N'IPHM' THEN 3300 WHEN N'VPHA' THEN 181 WHEN N'IPHA' THEN 181 ELSE 0 END AS HighAlarm,
    CASE SignalAcronym WHEN N'FREQ' THEN 59.95 WHEN N'VPHM' THEN 475000 WHEN N'IPHM' THEN 0 WHEN N'VPHA' THEN -180 WHEN N'IPHA' THEN -180 ELSE 0 END AS LowRange,
    CASE SignalAcronym WHEN N'FREQ' THEN 60.05 WHEN N'VPHM' THEN 525000 WHEN N'IPHM' THEN 3000 WHEN N'VPHA' THEN 180 WHEN N'IPHA' THEN 180 ELSE 0 END AS HighRange,
    0.0 AS CompressionLimit, 0.0 AS ExceptionLimit, CASE SignalAcronym WHEN N'DIGI' THEN 0 ELSE 7 END AS DisplayDigits, N'' AS SetDescription,
    '' AS ClearDescription, 0 AS AlarmState, 5 AS ChangeSecurity, 0 AS AccessSecurity, 0 AS StepCheck, 0 AS AlarmEnabled, 0 AS AlarmFlags, 0 AS AlarmDelay,
    0 AS AlarmToFile, 0 AS AlarmByEmail, 0 AS AlarmByPager, 0 AS AlarmByPhone, ContactList AS AlarmEmails, N'' AS AlarmPagers, N'' AS AlarmPhones
FROM MeasurementDetail;

CREATE VIEW SignalsWithAlarms
AS
SELECT DISTINCT Measurement.SignalID
FROM Measurement JOIN Alarm ON Measurement.SignalID = Alarm.SignalID
WHERE Alarm.Enabled <> 0;

CREATE VIEW CurrentLoggedAlarmState
AS
SELECT
    Log1.SignalID,
    Log1.NewState,
    Log1.Timestamp,
    Log1.Value
FROM
    AlarmLog AS Log1 LEFT OUTER JOIN
    AlarmLog AS Log2 ON Log1.SignalID = Log2.SignalID AND Log1.Ticks < Log2.Ticks
WHERE
    Log2.ID IS NULL;

CREATE VIEW CurrentAlarmState
AS
SELECT SignalsWithAlarms.SignalID, CurrentLoggedAlarmState.NewState AS State, CurrentLoggedAlarmState.Timestamp, CurrentLoggedAlarmState.Value
FROM SignalsWithAlarms LEFT OUTER JOIN CurrentLoggedAlarmState ON SignalsWithAlarms.SignalID = CurrentLoggedAlarmState.SignalID;

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
SELECT ID, COALESCE(Acronym, '') AS Acronym, Name, COALESCE(PhoneNumber, '') AS PhoneNumber, COALESCE(ContactEmail, '') AS ContactEmail, COALESCE(URL, '') AS URL 
FROM Vendor;

CREATE VIEW CustomActionAdapterDetail AS
SELECT CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, COALESCE(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
    CA.Enabled, N.Name AS NodeName
FROM CustomActionAdapter AS CA INNER JOIN Node AS N ON CA.NodeID = N.ID;
 
CREATE VIEW CustomInputAdapterDetail AS
SELECT CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, COALESCE(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
    CA.Enabled, N.Name AS NodeName
FROM CustomInputAdapter AS CA INNER JOIN Node AS N ON CA.NodeID = N.ID;
 
CREATE VIEW CustomOutputAdapterDetail AS
SELECT CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, COALESCE(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
    CA.Enabled, N.Name AS NodeName
FROM CustomOutputAdapter AS CA INNER JOIN Node AS N ON CA.NodeID = N.ID;
 
CREATE VIEW CustomFilterAdapterDetail AS
SELECT CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, COALESCE(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
    CA.Enabled, N.Name AS NodeName
FROM CustomFilterAdapter AS CA INNER JOIN Node AS N ON CA.NodeID = N.ID;
 
CREATE VIEW IaonTreeView AS
SELECT 'Action Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, COALESCE(ConnectionString, '') AS ConnectionString
FROM IaonActionAdapter
UNION ALL
SELECT 'Input Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, COALESCE(ConnectionString, '') AS ConnectionString
FROM IaonInputAdapter
UNION ALL
SELECT 'Output Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, COALESCE(ConnectionString, '') AS ConnectionString
FROM IaonOutputAdapter
UNION ALL
SELECT 'Filter Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, COALESCE(ConnectionString, '') AS ConnectionString
FROM IaonFilterAdapter;
 
CREATE VIEW OtherDeviceDetail AS
SELECT OD.ID, OD.Acronym, COALESCE(OD.Name, '') AS Name, OD.IsConcentrator, OD.CompanyID, OD.VendorDeviceID, OD.Longitude, OD.Latitude, 
    OD.InterconnectionID, OD.Planned, OD.Desired, OD.InProgress, COALESCE(C.Name, '') AS CompanyName, COALESCE(C.Acronym, '') AS CompanyAcronym, 
    COALESCE(C.MapAcronym, '') AS CompanyMapAcronym, COALESCE(VD.Name, '') AS VendorDeviceName, COALESCE(I.Name, '') AS InterconnectionName
FROM OtherDevice AS OD LEFT OUTER JOIN
    Company AS C ON OD.CompanyID = C.ID LEFT OUTER JOIN
    VendorDevice AS VD ON OD.VendorDeviceID = VD.ID LEFT OUTER JOIN
    Interconnection AS I ON OD.InterconnectionID = I.ID;
 
CREATE VIEW VendorDeviceDistribution AS
SELECT Device.NodeID, Vendor.Name AS VendorName, COUNT(*) AS DeviceCount 
FROM Device LEFT OUTER JOIN 
    VendorDevice ON Device.VendorDeviceID = VendorDevice.ID INNER JOIN
    Vendor ON VendorDevice.VendorID = Vendor.ID
GROUP BY Device.NodeID, Vendor.Name;

CREATE VIEW VendorDeviceDetail
AS
SELECT VD.ID, VD.VendorID, VD.Name, COALESCE(VD.Description, '') AS Description, COALESCE(VD.URL, '') AS URL, V.Name AS VendorName, 
    V.Acronym AS VendorAcronym
FROM VendorDevice AS VD INNER JOIN Vendor AS V ON VD.VendorID = V.ID;
                      
CREATE VIEW DeviceDetail
AS
SELECT D.NodeID, D.ID, D.ParentID, D.UniqueID, D.Acronym, COALESCE(D.Name, '') AS Name, D.OriginalSource, D.IsConcentrator, D.CompanyID, D.HistorianID, D.AccessID, D.VendorDeviceID, 
    D.ProtocolID, D.Longitude, D.Latitude, D.InterconnectionID, COALESCE(D.ConnectionString, '') AS ConnectionString, COALESCE(D.TimeZone, '') AS TimeZone, 
    COALESCE(D.FramesPerSecond, 30) AS FramesPerSecond, D.TimeAdjustmentTicks, D.DataLossInterval, D.ConnectOnDemand, COALESCE(D.ContactList, '') AS ContactList, D.MeasuredLines, D.LoadOrder, D.Enabled, COALESCE(C.Name, '') 
    AS CompanyName, COALESCE(C.Acronym, '') AS CompanyAcronym, COALESCE(C.MapAcronym, '') AS CompanyMapAcronym, COALESCE(H.Acronym, '') 
    AS HistorianAcronym, COALESCE(VD.VendorAcronym, '') AS VendorAcronym, COALESCE(VD.Name, '') AS VendorDeviceName, COALESCE(P.Name, '') 
    AS ProtocolName, P.Type AS ProtocolType, P.Category, COALESCE(I.Name, '') AS InterconnectionName, N.Name AS NodeName, COALESCE(PD.Acronym, '') AS ParentAcronym, D.CreatedOn, D.AllowedParsingExceptions, 
    D.ParsingExceptionWindow, D.DelayedConnectionInterval, D.AllowUseOfCachedConfiguration, D.AutoStartDataParsingSequence, D.SkipDisableRealTimeData, 
    D.MeasurementReportingInterval, D.UpdatedOn
FROM Device AS D LEFT OUTER JOIN
    Company AS C ON C.ID = D.CompanyID LEFT OUTER JOIN
    Historian AS H ON H.ID = D.HistorianID LEFT OUTER JOIN
    VendorDeviceDetail AS VD ON VD.ID = D.VendorDeviceID LEFT OUTER JOIN
    Protocol AS P ON P.ID = D.ProtocolID LEFT OUTER JOIN
    Interconnection AS I ON I.ID = D.InterconnectionID LEFT OUTER JOIN
    Node AS N ON N.ID = D.NodeID LEFT OUTER JOIN
    Device AS PD ON PD.ID = D.ParentID;
 
CREATE VIEW MapData AS
SELECT 'Device' AS DeviceType, NodeID, ID, Acronym, COALESCE(Name, '') AS Name, CompanyMapAcronym, CompanyName, VendorDeviceName, Longitude, 
    Latitude, true AS Reporting, false AS Inprogress, false AS Planned, false AS Desired
FROM DeviceDetail AS D
UNION ALL
SELECT 'OtherDevice' AS DeviceType, NULL AS NodeID, ID, Acronym, COALESCE(Name, '') AS Name, CompanyMapAcronym, CompanyName, VendorDeviceName, 
    Longitude, Latitude, false AS Reporting, true AS Inprogress, true AS Planned, true AS Desired
FROM OtherDeviceDetail AS OD;

CREATE VIEW OutputStreamDetail AS
SELECT OS.NodeID, OS.ID, OS.Acronym, COALESCE(OS.Name, '') AS Name, OS.Type, COALESCE(OS.ConnectionString, '') AS ConnectionString, OS.IDCode, 
    COALESCE(OS.CommandChannel, '') AS CommandChannel, COALESCE(OS.DataChannel, '') AS DataChannel, OS.AutoPublishConfigFrame, 
    OS.AutoStartDataChannel, OS.NominalFrequency, OS.FramesPerSecond, OS.LagTime, OS.LeadTime, OS.UseLocalClockAsRealTime, 
    OS.AllowSortsByArrival, OS.LoadOrder, OS.Enabled, N.Name AS NodeName, OS.DigitalMaskValue, OS.AnalogScalingValue, 
    OS.VoltageScalingValue, OS.CurrentScalingValue, OS.CoordinateFormat, OS.DataFormat, OS.DownsamplingMethod, 
    OS.AllowPreemptivePublishing, OS.TimeResolution, OS.IgnoreBadTimeStamps, OS.PerformTimeReasonabilityCheck
FROM OutputStream AS OS INNER JOIN Node AS N ON OS.NodeID = N.ID;
                      
CREATE VIEW OutputStreamMeasurementDetail AS
SELECT OSM.NodeID, OSM.AdapterID, OSM.ID, OSM.HistorianID, OSM.PointID, OSM.SignalReference, M.PointTag AS SourcePointTag, COALESCE(H.Acronym, '') 
    AS HistorianAcronym
FROM OutputStreamMeasurement AS OSM INNER JOIN
    Measurement AS M ON M.PointID = OSM.PointID LEFT OUTER JOIN
    Historian AS H ON H.ID = OSM.HistorianID;
      
CREATE VIEW OutputStreamDeviceDetail AS
SELECT OSD.NodeID, OSD.AdapterID, OSD.ID, OSD.Acronym, COALESCE(OSD.BpaAcronym, '') AS BpaAcronym, OSD.Name, OSD.LoadOrder, OSD.Enabled, 
    COALESCE(PhasorDataFormat, '') AS PhasorDataFormat, COALESCE(FrequencyDataFormat, '') AS FrequencyDataFormat, 
    COALESCE(AnalogDataFormat, '') AS AnalogDataFormat, COALESCE(CoordinateFormat, '') AS CoordinateFormat, IDCode,
            CASE 
                WHEN EXISTS (Select Acronym From Device Where Acronym = OSD.Acronym) THEN FALSE 
                ELSE TRUE 
            END AS `Virtual`
FROM OutputStreamDevice OSD;
                      
CREATE VIEW PhasorDetail AS
SELECT P.*, COALESCE(DP.Label, '') AS DestinationPhasorLabel, D.Acronym AS DeviceAcronym
FROM Phasor P LEFT OUTER JOIN Phasor DP ON P.DestinationPhasorID = DP.ID
LEFT OUTER JOIN Device D ON P.DeviceID = D.ID;

CREATE VIEW StatisticMeasurement AS
SELECT MeasurementDetail.*
FROM MeasurementDetail 
WHERE MeasurementDetail.SignalAcronym = 'STAT';

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

CREATE TRIGGER Node_AllMeasurementsGroup AFTER INSERT ON Node
FOR EACH ROW INSERT INTO MeasurementGroup(NodeID, Name, Description, FilterExpression) VALUES(NEW.ID, 'AllMeasurements', 'All measurements defined in ActiveMeasurements', 'FILTER ActiveMeasurements WHERE SignalID IS NOT NULL');

CREATE TRIGGER CustomActionAdapter_RuntimeSync_Insert AFTER INSERT ON CustomActionAdapter
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

CREATE TRIGGER CustomFilterAdapter_RuntimeSync_Insert AFTER INSERT ON CustomFilterAdapter
FOR EACH ROW INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, N'CustomFilterAdapter');

CREATE TRIGGER CustomFilterAdapter_RuntimeSync_Delete BEFORE DELETE ON CustomFilterAdapter
FOR EACH ROW DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = N'CustomFilterAdapter';

DELIMITER $$
CREATE TRIGGER Device_RuntimeSync_Insert AFTER INSERT ON Device FOR EACH ROW
BEGIN
    INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, N'Device');
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Device', 'ID', NEW.ID);
END$$
DELIMITER ;

CREATE TRIGGER Device_RuntimeSync_Delete BEFORE DELETE ON Device
FOR EACH ROW DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = N'Device';

CREATE TRIGGER CalculatedMeasurement_RuntimeSync_Insert AFTER INSERT ON CalculatedMeasurement
FOR EACH ROW INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, N'CalculatedMeasurement');

CREATE TRIGGER CalculatedMeasurement_RuntimeSync_Delete BEFORE DELETE ON CalculatedMeasurement
FOR EACH ROW DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = N'CalculatedMeasurement';

DELIMITER $$
CREATE TRIGGER OutputStream_RuntimeSync_Insert AFTER INSERT ON OutputStream FOR EACH ROW
BEGIN
    INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, N'OutputStream');
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStream', 'ID', NEW.ID);
END$$
DELIMITER ;

CREATE TRIGGER OutputStream_RuntimeSync_Delete BEFORE DELETE ON OutputStream
FOR EACH ROW DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = N'OutputStream';

CREATE TRIGGER Historian_RuntimeSync_Insert AFTER INSERT ON Historian
FOR EACH ROW INSERT INTO Runtime (SourceID, SourceTable) VALUES(NEW.ID, N'Historian');

CREATE TRIGGER Historian_RuntimeSync_Delete BEFORE DELETE ON Historian
FOR EACH ROW DELETE FROM Runtime WHERE SourceID = OLD.ID AND SourceTable = N'Historian';

CREATE TRIGGER AccessLog_InsertDefault BEFORE INSERT ON AccessLog 
FOR EACH ROW SET NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP());

CREATE TRIGGER ApplicationRole_InsertDefault BEFORE INSERT ON ApplicationRole 
FOR EACH ROW SET NEW.ID = IF(NEW.ID <> '', NEW.ID, UUID()), NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER SecurityGroup_InsertDefault BEFORE INSERT ON SecurityGroup 
FOR EACH ROW SET NEW.ID = IF(NEW.ID <> '', NEW.ID, UUID()), NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER UserAccount_InsertDefault BEFORE INSERT ON UserAccount 
FOR EACH ROW SET NEW.ID = IF(NEW.ID <> '', NEW.ID, UUID()), NEW.ChangePasswordOn = COALESCE(NEW.ChangePasswordOn, ADDDATE(UTC_TIMESTAMP(), 90)), NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER CalculatedMeasurement_InsertDefault BEFORE INSERT ON CalculatedMeasurement
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER Company_InsertDefault BEFORE INSERT ON Company
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER CustomActionAdapter_InsertDefault BEFORE INSERT ON CustomActionAdapter
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER CustomInputAdapter_InsertDefault BEFORE INSERT ON CustomInputAdapter
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER CustomOutputAdapter_InsertDefault BEFORE INSERT ON CustomOutputAdapter
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER CustomFilterAdapter_InsertDefault BEFORE INSERT ON CustomFilterAdapter
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER Device_InsertDefault BEFORE INSERT ON Device
FOR EACH ROW SET NEW.UniqueID = COALESCE(NEW.UniqueID, UUID()), NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER Historian_InsertDefault BEFORE INSERT ON Historian
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER Subscriber_InsertDefault BEFORE INSERT ON Subscriber
FOR EACH ROW SET NEW.ID = IF(NEW.ID <> '', NEW.ID, UUID()), NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER SubscriberMeasurement_InsertDefault BEFORE INSERT ON SubscriberMeasurement
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER SubscriberMeasurementGroup_InsertDefault BEFORE INSERT ON SubscriberMeasurementGroup
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER MeasurementGroup_InsertDefault BEFORE INSERT ON MeasurementGroup
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER MeasurementGroupMeasurement_InsertDefault BEFORE INSERT ON MeasurementGroupMeasurement
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER Measurement_InsertDefault BEFORE INSERT ON Measurement
FOR EACH ROW SET NEW.SignalID = IF(NEW.SignalID <> '', NEW.SignalID, UUID()), NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER Node_InsertDefault BEFORE INSERT ON Node
FOR EACH ROW SET NEW.ID = IF(NEW.ID <> '', NEW.ID, UUID()), NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER OtherDevice_InsertDefault BEFORE INSERT ON OtherDevice
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER OutputStream_InsertDefault BEFORE INSERT ON OutputStream
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER OutputStreamDevice_InsertDefault BEFORE INSERT ON OutputStreamDevice
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER OutputStreamDeviceAnalog_InsertDefault BEFORE INSERT ON OutputStreamDeviceAnalog
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER OutputStreamDeviceDigital_InsertDefault BEFORE INSERT ON OutputStreamDeviceDigital
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER OutputStreamDevicePhasor_InsertDefault BEFORE INSERT ON OutputStreamDevicePhasor
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER OutputStreamMeasurement_InsertDefault BEFORE INSERT ON OutputStreamMeasurement
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER Phasor_InsertDefault BEFORE INSERT ON Phasor
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER Alarm_InsertDefault BEFORE INSERT ON Alarm
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER Vendor_InsertDefault BEFORE INSERT ON Vendor
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER VendorDevice_InsertDefault BEFORE INSERT ON VendorDevice
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER ErrorLog_InsertDefault BEFORE INSERT ON ErrorLog FOR EACH ROW
SET NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP());

CREATE TRIGGER AuditLog_InsertDefault BEFORE INSERT ON AuditLog FOR EACH ROW
SET NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

-- ***********************
-- Company Change Tracking
-- ***********************

DELIMITER $$
CREATE TRIGGER Company_UpdateTracker AFTER UPDATE ON Company FOR EACH ROW
BEGIN
    CASE WHEN OLD.Acronym <> NEW.Acronym THEN
        INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM ActiveMeasurement WHERE Company = NEW.Acronym;
    ELSE BEGIN END;
    END CASE;
END$$
DELIMITER ;

-- **********************
-- Device Change Tracking
-- **********************

-- This trigger has been combined with the Device_RuntimeSync_Insert trigger
-- CREATE TRIGGER Device_InsertTracker AFTER INSERT ON Device FOR EACH ROW
-- INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Device', 'ID', NEW.ID);

DELIMITER $$
CREATE TRIGGER Device_UpdateTracker1 AFTER UPDATE ON Device FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Device', 'ID', NEW.ID);
    
    CASE WHEN OLD.NodeID <> NEW.NodeID
           OR OLD.Acronym <> NEW.Acronym
           OR OLD.IsConcentrator <> NEW.IsConcentrator
           OR (OLD.ParentID IS NULL AND NEW.ParentID IS NOT NULL)
           OR (OLD.ParentID IS NOT NULL AND NEW.ParentID IS NULL)
           OR (OLD.ParentID IS NOT NULL AND NEW.ParentID IS NOT NULL AND OLD.ParentID <> NEW.ParentID)
           OR (OLD.FramesPerSecond IS NULL AND NEW.FramesPerSecond IS NOT NULL)
           OR (OLD.FramesPerSecond IS NOT NULL AND NEW.FramesPerSecond IS NULL)
           OR (OLD.FramesPerSecond IS NOT NULL AND NEW.FramesPerSecond IS NOT NULL AND OLD.FramesPerSecond <> NEW.FramesPerSecond)
           OR (OLD.Longitude IS NULL AND NEW.Longitude IS NOT NULL)
           OR (OLD.Longitude IS NOT NULL AND NEW.Longitude IS NULL)
           OR (OLD.Longitude IS NOT NULL AND NEW.Longitude IS NOT NULL AND OLD.Longitude <> NEW.Longitude)
           OR (OLD.Latitude IS NULL AND NEW.Latitude IS NOT NULL)
           OR (OLD.Latitude IS NOT NULL AND NEW.Latitude IS NULL)
           OR (OLD.Latitude IS NOT NULL AND NEW.Latitude IS NOT NULL AND OLD.Latitude <> NEW.Latitude)
           OR (OLD.CompanyID IS NULL AND NEW.CompanyID IS NOT NULL)
           OR (OLD.CompanyID IS NOT NULL AND NEW.CompanyID IS NULL)
           OR (OLD.CompanyID IS NOT NULL AND NEW.CompanyID IS NOT NULL AND OLD.CompanyID <> NEW.CompanyID)
           OR (OLD.ProtocolID IS NULL AND NEW.ProtocolID IS NOT NULL)
           OR (OLD.ProtocolID IS NOT NULL AND NEW.ProtocolID IS NULL)
           OR (OLD.ProtocolID IS NOT NULL AND NEW.ProtocolID IS NOT NULL AND OLD.ProtocolID <> NEW.ProtocolID)
           OR OLD.Enabled <> NEW.Enabled
    THEN
         INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE DeviceID = NEW.ID;
    ELSE BEGIN END;
    END CASE;
END$$
DELIMITER ;

CREATE TRIGGER Device_DeleteTracker AFTER DELETE ON Device FOR EACH ROW
INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Device', 'ID', OLD.ID);

-- *************************
-- Historian Change Tracking
-- *************************

DELIMITER $$
CREATE TRIGGER Historian_UpdateTracker AFTER UPDATE ON Historian FOR EACH ROW
BEGIN
    CASE WHEN OLD.NodeID <> NEW.NodeID
           OR OLD.Acronym <> NEW.Acronym
    THEN
        INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE HistorianID = NEW.ID;
    ELSE BEGIN END;
    END CASE;
END$$
DELIMITER ;

-- ***************************
-- Measurement Change Tracking
-- ***************************

DELIMITER $$
CREATE TRIGGER Measurement_InsertTracker AFTER INSERT ON Measurement FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Measurement', 'PointID', NEW.PointID);
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE PointID = NEW.PointID AND SignalID IS NOT NULL;
END$$
DELIMITER ;

DELIMITER $$
CREATE TRIGGER Measurement_UpdateTracker AFTER UPDATE ON Measurement FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Measurement', 'PointID', NEW.PointID);
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('ActiveMeasurement', 'SignalID', NEW.SignalID);
    
    CASE WHEN OLD.SignalID <> NEW.SignalID
    THEN
        INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('ActiveMeasurement', 'SignalID', OLD.SignalID);
    ELSE BEGIN END;
    END CASE;
END$$
DELIMITER ;

DELIMITER $$
CREATE TRIGGER Measurement_DeleteTracker AFTER DELETE ON Measurement FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Measurement', 'PointID', OLD.PointID);
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('ActiveMeasurement', 'SignalID', OLD.SignalID);
END$$
DELIMITER ;

-- ****************************
-- OutputStream Change Tracking
-- ****************************

-- This trigger has been combined with the OutputStream_RuntimeSync_Insert trigger
-- CREATE TRIGGER OutputStream_InsertTracker AFTER INSERT ON OutputStream FOR EACH ROW
-- INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStream', 'ID', NEW.ID);

CREATE TRIGGER OutputStream_UpdateTracker AFTER UPDATE ON OutputStream FOR EACH ROW
INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStream', 'ID', NEW.ID);

CREATE TRIGGER OutputStream_DeleteTracker AFTER DELETE ON OutputStream FOR EACH ROW
INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStream', 'ID', OLD.ID);

-- **********************************
-- OutputStreamDevice Change Tracking
-- **********************************

CREATE TRIGGER OutputStreamDevice_InsertTracker AFTER INSERT ON OutputStreamDevice FOR EACH ROW
INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamDevice', 'ID', NEW.ID);

CREATE TRIGGER OutputStreamDevice_UpdateTracker AFTER UPDATE ON OutputStreamDevice FOR EACH ROW
INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamDevice', 'ID', NEW.ID);

CREATE TRIGGER OutputStreamDevice_DeleteTracker AFTER DELETE ON OutputStreamDevice FOR EACH ROW
INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamDevice', 'ID', OLD.ID);

-- ***************************************
-- OutputStreamMeasurement Change Tracking
-- ***************************************

CREATE TRIGGER OutputStreamMeasurement_InsertTracker AFTER INSERT ON OutputStreamMeasurement FOR EACH ROW
INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamMeasurement', 'ID', NEW.ID);

CREATE TRIGGER OutputStreamMeasurement_UpdateTracker AFTER UPDATE ON OutputStreamMeasurement FOR EACH ROW
INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamMeasurement', 'ID', NEW.ID);

CREATE TRIGGER OutputStreamMeasurement_DeleteTracker AFTER DELETE ON OutputStreamMeasurement FOR EACH ROW
INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamMeasurement', 'ID', OLD.ID);

-- **********************
-- Phasor Change Tracking
-- **********************

DELIMITER $$
CREATE TRIGGER Phasor_UpdateTracker AFTER UPDATE ON Phasor FOR EACH ROW
BEGIN
    CASE WHEN OLD.Type <> NEW.Type
           OR OLD.Phase <> NEW.Phase
    THEN
        INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM ActiveMeasurement WHERE PhasorID = NEW.ID;
    ELSE BEGIN END;
    END CASE;
    
    CASE WHEN OLD.DeviceID <> NEW.DeviceID
           OR OLD.SourceIndex <> NEW.SourceIndex
    THEN
        INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE DeviceID = OLD.DeviceID AND PhasorSourceIndex = OLD.SourceIndex;
        INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE DeviceID = NEW.DeviceID AND PhasorSourceIndex = NEW.SourceIndex;
    ELSE BEGIN END;
    END CASE;
END$$
DELIMITER ;

-- ************************
-- Protocol Change Tracking
-- ************************

DELIMITER $$
CREATE TRIGGER Protocol_UpdateTracker AFTER UPDATE ON Protocol FOR EACH ROW
BEGIN
    CASE WHEN OLD.Acronym <> NEW.Acronym
           OR OLD.Type <> NEW.Type
    THEN
        INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM ActiveMeasurement WHERE Protocol = NEW.Acronym;
    ELSE BEGIN END;
    END CASE;
END$$
DELIMITER ;

-- **************************
-- SignalType Change Tracking
-- **************************

DELIMITER $$
CREATE TRIGGER SignalType_UpdateTracker AFTER UPDATE ON SignalType FOR EACH ROW
BEGIN
    CASE WHEN OLD.Acronym <> NEW.Acronym
    THEN
        INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement WHERE SignalTypeID = NEW.ID;
    ELSE BEGIN END;
    END CASE;
END$$
DELIMITER ;



-- CREATE FUNCTION StringToGuid(str CHAR(36)) RETURNS BINARY(16)
-- RETURN CONCAT(UNHEX(LEFT(str, 8)), UNHEX(MID(str, 10, 4)), UNHEX(MID(str, 15, 4)), UNHEX(MID(str, 20, 4)), UNHEX(RIGHT(str, 12)));

-- CREATE FUNCTION GuidToString(guid BINARY(16)) RETURNS CHAR(36) 
-- RETURN CONCAT(HEX(LEFT(guid, 4)), '-', HEX(MID(guid, 5, 2)), '-', HEX(MID(guid, 7, 2)), '-', HEX(MID(guid, 9, 2)), '-', HEX(RIGHT(guid, 6)));

-- CREATE FUNCTION NewGuid() RETURNS BINARY(16) 
-- RETURN StringToGuid(UUID());

-- DELIMITER $$
-- CREATE PROCEDURE GetFormattedMeasurements(measurementSql TEXT, includeAdjustments TINYINT, OUT measurements TEXT)
-- BEGIN
--     DECLARE done INT DEFAULT 0;
--     DECLARE measurementID INT;
--     DECLARE archiveSource VARCHAR(50);
--     DECLARE adder FLOAT DEFAULT 0.0;
--     DECLARE multiplier FLOAT DEFAULT 1.1;	
--     DECLARE selectedMeasurements CURSOR FOR SELECT * FROM temp;
--     DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

--     CREATE TEMPORARY TABLE temp
--     (
--         MeasurementID INT,
--         ArchiveSource VARCHAR(50),
--         Adder FLOAT,
--         Multiplier FLOAT
--     )
--     TABLESPACE MEMORY;
    
--     SET @insertSQL = CONCAT('INSERT INTO temp ', measurementSql);
--     PREPARE stmt FROM @insertSQL;
--     EXECUTE stmt;
--     DEALLOCATE PREPARE stmt;

--     OPEN selectedMeasurements;	
--     SET measurements = '';
    
--     --  Step through selected measurements
--     REPEAT
--         --  Get next row from measurements SQL
--         FETCH selectedMeasurements INTO measurementID, archiveSource, adder, multiplier;

--         IF NOT done THEN
--             IF LENGTH(measurements) > 0 THEN
--                 SET measurements = CONCAT(measurements, ';');
--             END IF;
            
--             IF includeAdjustments <> 0 AND (adder <> 0.0 OR multiplier <> 1.0) THEN
--                 SET measurements = CONCAT(measurements, archiveSource, ':', measurementID, ',', adder, ',', multiplier);
--             ELSE
--                 SET measurements = CONCAT(measurements, archiveSource, ':', measurementID);
--             END IF;

--         END IF;
--     UNTIL done END REPEAT;

--     CLOSE selectedMeasurements;
--     DROP TABLE temp;
-- END$$
-- DELIMITER ;

-- DELIMITER $$
-- CREATE FUNCTION FormatMeasurements(measurementSql TEXT, includeAdjustments TINYINT)
-- RETURNS TEXT 
-- BEGIN
--   DECLARE measurements TEXT; 

--     CALL GetFormattedMeasurements(measurementSql, includeAdjustments, measurements);

--     IF LENGTH(measurements) > 0 THEN
--         SET measurements = CONCAT('{', measurements, '}');
--     ELSE
--         SET measurements = NULL;
--     END IF;
        
--     RETURN measurements;
-- END$$
-- DELIMITER ;

-- **************************
-- Alarm Panel Data
-- **************************


CREATE TABLE AlarmState(
    ID int AUTO_INCREMENT NOT NULL,
    State varchar(50) NULL,
    Color varchar(50) NULL,
    RecommendedAction varchar(500) NULL,
    PRIMARY KEY(ID)
);

CREATE TABLE AlarmDevice(
    ID int AUTO_INCREMENT NOT NULL,
    DeviceID int NULL,
    StateID int NULL,
    TimeStamp datetime NULL,
    DisplayData varchar(10) NULL,
    PRIMARY KEY(ID)
);

ALTER TABLE AlarmDevice
ADD CONSTRAINT FK_AlarmDevice_Device
FOREIGN KEY(DeviceID) REFERENCES Device(ID)
ON DELETE CASCADE
ON UPDATE CASCADE;

ALTER TABLE AlarmDevice
ADD CONSTRAINT FK_AlarmDevice_AlarmState
FOREIGN KEY(StateID) REFERENCES AlarmState(ID)
ON DELETE CASCADE
ON UPDATE CASCADE;

CREATE VIEW AlarmDeviceStateView AS
SELECT AlarmDevice.ID, Device.Name, AlarmState.State, AlarmState.Color, AlarmDevice.DisplayData, Device.ID AS DeviceID
FROM AlarmDevice
    INNER JOIN AlarmState ON AlarmDevice.StateID = AlarmState.ID
    INNER JOIN Device ON AlarmDevice.DeviceID = Device.ID;