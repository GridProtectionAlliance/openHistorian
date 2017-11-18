--  ----------------------------------------------------------------------------------------------------
--  GSFSchema Data Structures for Oracle - Gbtc
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

-- The following statements are used to create a tablespace, user, and schema.
-- Be sure to change the password.
-- CREATE TABLESPACE GSFSchema_TS DATAFILE 'GSFSchema.dbf' SIZE 20M AUTOEXTEND ON;
-- CREATE TABLESPACE GSFSchema_INDEX DATAFILE 'GSFSchema_index.dbf' SIZE 20M AUTOEXTEND ON;
-- CREATE USER GSFSchema IDENTIFIED BY MyPassword DEFAULT TABLESPACE GSFSchema_TS;
-- GRANT UNLIMITED TABLESPACE TO GSFSchema;
-- GRANT CREATE SESSION TO GSFSchema;
-- ALTER SESSION SET CURRENT_SCHEMA = GSFSchema;

-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW SchemaVersion AS
SELECT 7 AS VersionNumber
FROM dual;

CREATE TABLE ErrorLog(
    ID NUMBER NOT NULL,
    Source VARCHAR2(200) NOT NULL,
    Type VARCHAR2(4000),
    Message VARCHAR2(4000) NOT NULL,
    Detail VARCHAR2(4000) NULL,
    CreatedOn DATE NOT NULL
);

CREATE UNIQUE INDEX IX_ErrorLog_ID ON ErrorLog (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE ErrorLog ADD CONSTRAINT PK_ErrorLog PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_ErrorLog START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_ErrorLog BEFORE INSERT ON ErrorLog
    FOR EACH ROW BEGIN SELECT SEQ_ErrorLog.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE Runtime(
    ID NUMBER NOT NULL,
    SourceID NUMBER NOT NULL,
    SourceTable VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_Runtime_Source ON Runtime (SourceID ASC, SourceTable ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_Runtime_ID ON Runtime (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Runtime ADD CONSTRAINT PK_Runtime PRIMARY KEY (SourceID, SourceTable);

CREATE SEQUENCE SEQ_Runtime START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Runtime BEFORE INSERT ON Runtime
    FOR EACH ROW BEGIN SELECT SEQ_Runtime.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE AuditLog(
    ID NUMBER NOT NULL,
    TableName VARCHAR2(200) NOT NULL,
    PrimaryKeyColumn VARCHAR2(200) NOT NULL,
    PrimaryKeyValue VARCHAR2(4000) NOT NULL,
    ColumnName VARCHAR2(200) NOT NULL,
    OriginalValue VARCHAR2(4000),
    NewValue VARCHAR2(4000),
    Deleted NUMBER DEFAULT 0 NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    PRIMARY KEY (ID)
);

CREATE SEQUENCE SEQ_AuditLog START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_AuditLog BEFORE INSERT ON AuditLog
    FOR EACH ROW BEGIN SELECT SEQ_AuditLog.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE Company(
    ID NUMBER NOT NULL,
    COMPANYPARENTID  NUMBER,
    REGIONID         NUMBER,
    Acronym VARCHAR2(200) NOT NULL,
    MapAcronym VARCHAR2(10) NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    URL VARCHAR2(4000) NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_Company_ID ON Company (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Company ADD CONSTRAINT PK_Company PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_Company START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Company BEFORE INSERT ON Company
    FOR EACH ROW BEGIN SELECT SEQ_Company.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE TrackedChange(
    ID NUMBER NOT NULL,
    TableName VARCHAR(200) NOT NULL,
    PrimaryKeyColumn VARCHAR(200) NOT NULL,
    PrimaryKeyValue VARCHAR(4000) NULL
);

CREATE UNIQUE INDEX IX_TrackedChange_ID ON TrackedChange (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE TrackedChange ADD CONSTRAINT PK_TrackedChange PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_TrackedChange START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_TrackedChange BEFORE INSERT ON TrackedChange
    FOR EACH ROW BEGIN SELECT SEQ_TrackedChange.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE ConfigurationEntity(
    SourceName VARCHAR2(200) NOT NULL,
    RuntimeName VARCHAR2(200) NOT NULL,
    Description VARCHAR2(4000) NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL
);

CREATE TABLE Vendor(
    ID NUMBER NOT NULL,
    Acronym VARCHAR2(200) NULL,
    Name VARCHAR2(200) NOT NULL,
    PhoneNumber VARCHAR2(200) NULL,
    ContactEmail VARCHAR2(200) NULL,
    URL VARCHAR2(4000) NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_Vendor_ID ON Vendor (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Vendor ADD CONSTRAINT PK_Vendor PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_Vendor START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Vendor BEFORE INSERT ON Vendor
    FOR EACH ROW BEGIN SELECT SEQ_Vendor.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE Protocol(
    ID NUMBER NOT NULL,
    Acronym VARCHAR2(200) NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    Type VARCHAR2(200) DEFAULT 'Frame' NOT NULL,
    Category VARCHAR2(200) DEFAULT 'Phasor' NOT NULL,
    AssemblyName VARCHAR2(1024) DEFAULT 'PhasorProtocolAdapters.dll' NOT NULL,
    TypeName VARCHAR2(200) DEFAULT 'PhasorProtocolAdapters.PhasorMeasurementMapper' NOT NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL
);

CREATE UNIQUE INDEX IX_Protocol_ID ON Protocol (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Protocol ADD CONSTRAINT PK_Protocol PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_Protocol START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Protocol BEFORE INSERT ON Protocol
    FOR EACH ROW BEGIN SELECT SEQ_Protocol.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE SignalType(
    ID NUMBER NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    Acronym VARCHAR2(4) NOT NULL,
    Suffix VARCHAR2(2) NOT NULL,
    Abbreviation VARCHAR2(2) NOT NULL,
    LongAcronym VARCHAR(200) DEFAULT 'Undefined' NOT NULL,
    Source VARCHAR2(10) NOT NULL,
    EngineeringUnits VARCHAR2(10) NULL
);

CREATE UNIQUE INDEX IX_SignalType_ID ON SignalType (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE SignalType ADD CONSTRAINT PK_SignalType PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_SignalType START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_SignalType BEFORE INSERT ON SignalType
    FOR EACH ROW BEGIN SELECT SEQ_SignalType.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE Interconnection(
    ID NUMBER NOT NULL,
    Acronym VARCHAR2(200) NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    LoadOrder NUMBER DEFAULT 0 NULL
);

CREATE UNIQUE INDEX IX_Interconnection_ID ON Interconnection (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Interconnection ADD CONSTRAINT PK_Interconnection PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_Interconnection START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Interconnection BEFORE INSERT ON Interconnection
    FOR EACH ROW BEGIN SELECT SEQ_Interconnection.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE Node(
    ID VARCHAR2(36) NULL,
    Name VARCHAR2(200) NOT NULL,
    CompanyID NUMBER NULL,
    Longitude NUMBER(9, 6) NULL,
    Latitude NUMBER(9, 6) NULL,
    Description VARCHAR2(4000) NULL,
    ImagePath VARCHAR2(4000) NULL,
    Settings VARCHAR2(4000) NULL,
    MenuType VARCHAR2(200) DEFAULT 'File' NOT NULL,
    MenuData VARCHAR2(4000) NOT NULL,
    Master NUMBER DEFAULT 0 NOT NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_Node_ID ON Node (ID ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_Node_Name ON Node (Name ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Node ADD CONSTRAINT PK_Node PRIMARY KEY (ID);

CREATE TABLE DataOperation(
    NodeID VARCHAR2(36) NULL,
    Description VARCHAR2(4000) NULL,
    AssemblyName VARCHAR2(4000) NOT NULL,
    TypeName VARCHAR2(4000) NOT NULL,
    MethodName VARCHAR2(200) NOT NULL,
    Arguments VARCHAR2(4000) NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL
);

CREATE TABLE OtherDevice(
    ID NUMBER NOT NULL,
    Acronym VARCHAR2(200) NOT NULL,
    Name VARCHAR2(200) NULL,
    IsConcentrator NUMBER DEFAULT 0 NOT NULL,
    CompanyID NUMBER NULL,
    VendorDeviceID NUMBER NULL,
    Longitude NUMBER(9, 6) NULL,
    Latitude NUMBER(9, 6) NULL,
    InterconnectionID NUMBER NULL,
    Planned NUMBER DEFAULT 0 NOT NULL,
    Desired NUMBER DEFAULT 0 NOT NULL,
    InProgress NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_OtherDevice_ID ON OtherDevice (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE OtherDevice ADD CONSTRAINT PK_OtherDevice PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_OtherDevice START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_OtherDevice BEFORE INSERT ON OtherDevice
    FOR EACH ROW BEGIN SELECT SEQ_OtherDevice.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE Device(
    NodeID VARCHAR2(36) NOT NULL,
    ID NUMBER NOT NULL,
    ParentID NUMBER NULL,
    UniqueID VARCHAR2(36) NULL,
    Acronym VARCHAR2(200) NOT NULL,
    Name VARCHAR2(200) NULL,
    OriginalSource VARCHAR2(200) NULL,
    IsConcentrator NUMBER DEFAULT 0 NOT NULL,
    CompanyID NUMBER NULL,
    HistorianID NUMBER NULL,
    AccessID NUMBER DEFAULT 0 NOT NULL,
    VendorDeviceID NUMBER NULL,
    ProtocolID NUMBER NULL,
    Longitude NUMBER(9, 6) NULL,
    Latitude NUMBER(9, 6) NULL,
    InterconnectionID NUMBER NULL,
    ConnectionString VARCHAR2(4000) NULL,
    TimeZone VARCHAR2(200) NULL,
    FramesPerSecond NUMBER DEFAULT 30 NULL,
    TimeAdjustmentTicks NUMBER(19,0) DEFAULT 0 NOT NULL,
    DataLossInterval NUMBER DEFAULT 5 NOT NULL,
    AllowedParsingExceptions NUMBER DEFAULT 10 NOT NULL,
    ParsingExceptionWindow NUMBER DEFAULT 5 NOT NULL,
    DelayedConnectionInterval NUMBER DEFAULT 5 NOT NULL,
    AllowUseOfCachedConfiguration NUMBER DEFAULT 1 NOT NULL,
    AutoStartDataParsingSequence NUMBER DEFAULT 1 NOT NULL,
    SkipDisableRealTimeData NUMBER DEFAULT 0 NOT NULL,
    MeasurementReportingInterval NUMBER DEFAULT 100000 NOT NULL,
    ConnectOnDemand NUMBER DEFAULT 1 NOT NULL,
    ContactList VARCHAR2(4000) NULL,
    MeasuredLines NUMBER NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,    
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_Device_ID ON Device (ID ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_Device_UniqueID ON Device (UniqueID ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_Device_NodeID_Acronym ON Device (NodeID ASC, Acronym ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Device ADD CONSTRAINT PK_Device PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_Device START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Device BEFORE INSERT ON Device
    FOR EACH ROW BEGIN SELECT SEQ_Device.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE VendorDevice(
    ID NUMBER NOT NULL,
    VendorID NUMBER DEFAULT 10 NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    Description VARCHAR2(4000) NULL,
    URL VARCHAR2(4000) NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_VendorDevice_ID ON VendorDevice (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE VendorDevice ADD CONSTRAINT PK_VendorDevice PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_VendorDevice START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_VendorDevice BEFORE INSERT ON VendorDevice
    FOR EACH ROW BEGIN SELECT SEQ_VendorDevice.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE OutputStreamDeviceDigital(
    NodeID VARCHAR2(36) NOT NULL,
    OutputStreamDeviceID NUMBER NOT NULL,
    ID NUMBER NOT NULL,
    Label VARCHAR2(4000) NOT NULL,
    MaskValue NUMBER DEFAULT 0 NOT NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_OutStreamDevDigital_ID ON OutputStreamDeviceDigital (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE OutputStreamDeviceDigital ADD CONSTRAINT PK_OutStreamDevDigital PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_OutStreamDevDigital START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_OutStreamDevDigital BEFORE INSERT ON OutputStreamDeviceDigital
    FOR EACH ROW BEGIN SELECT SEQ_OutStreamDevDigital.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE OutputStreamDevicePhasor(
    NodeID VARCHAR2(36) NOT NULL,
    OutputStreamDeviceID NUMBER NOT NULL,
    ID NUMBER NOT NULL,
    Label VARCHAR2(200) NOT NULL,
    Type VARCHAR2(1) DEFAULT 'V' NOT NULL,
    Phase VARCHAR2(1) DEFAULT '+' NOT NULL,
    ScalingValue NUMBER DEFAULT 0 NOT NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_OutStreamDevPhasor ON OutputStreamDevicePhasor (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE OutputStreamDevicePhasor ADD CONSTRAINT PK_OutStreamDevPhasor PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_OutStreamDevPhasor START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_OutStreamDevPhasor BEFORE INSERT ON OutputStreamDevicePhasor
    FOR EACH ROW BEGIN SELECT SEQ_OutStreamDevPhasor.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE OutputStreamDeviceAnalog(
    NodeID VARCHAR2(36) NOT NULL,
    OutputStreamDeviceID NUMBER NOT NULL,
    ID NUMBER NOT NULL,
    Label VARCHAR2(16) NOT NULL,
    Type NUMBER DEFAULT 0 NOT NULL,
    ScalingValue NUMBER DEFAULT 0 NOT NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);


CREATE UNIQUE INDEX IX_OutStreamDevAnalog ON OutputStreamDeviceAnalog (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE OutputStreamDeviceAnalog ADD CONSTRAINT PK_OutStreamDevAnalog PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_OutStreamDevAnalog START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_OutStreamDevAnalog BEFORE INSERT ON OutputStreamDeviceAnalog
    FOR EACH ROW BEGIN SELECT SEQ_OutStreamDevAnalog.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE Measurement(
    PointID NUMBER NOT NULL,
    SignalID VARCHAR2(36) NULL,
    HistorianID NUMBER NULL,
    DeviceID NUMBER NULL,
    PointTag VARCHAR2(200) NOT NULL,
    AlternateTag VARCHAR2(4000) NULL,
    SignalTypeID NUMBER NOT NULL,
    PhasorSourceIndex NUMBER NULL,
    SignalReference VARCHAR2(200) NOT NULL,
    Adder NUMBER DEFAULT 0.0 NOT NULL,
    Multiplier NUMBER DEFAULT 1.0 NOT NULL,
    Description VARCHAR2(4000) NULL,
    Subscribed NUMBER DEFAULT 0 NOT NULL,
    Internal NUMBER DEFAULT 1 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_Measurement_SignalID ON Measurement (SignalID ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_Measurement_PointID ON Measurement (PointID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Measurement ADD CONSTRAINT PK_Measurement PRIMARY KEY (SignalID);

ALTER TABLE Measurement ADD CONSTRAINT UQ_Measurement_PointID UNIQUE (PointID);

CREATE SEQUENCE SEQ_Measurement START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Measurement BEFORE INSERT ON Measurement
    FOR EACH ROW BEGIN SELECT SEQ_Measurement.nextval INTO :NEW.PointID FROM dual;
END;
/

CREATE TABLE ImportedMeasurement(
    NodeID VARCHAR2(36) NULL,
    SourceNodeID VARCHAR2(36) NULL,
    SignalID VARCHAR2(36) NULL,
    Source VARCHAR2(200) NOT NULL,
    PointID NUMBER NOT NULL,
    PointTag VARCHAR2(200) NOT NULL,
    AlternateTag VARCHAR2(200) NULL,
    SignalTypeAcronym VARCHAR2(4) NULL,
    SignalReference VARCHAR2(4000) NOT NULL,
    FramesPerSecond NUMBER NULL,
    ProtocolAcronym VARCHAR2(200) NULL,
    ProtocolType VARCHAR2(200) DEFAULT 'Frame' NOT NULL,
    PhasorID NUMBER NULL,
    PhasorType VARCHAR2(1) NULL,
    Phase VARCHAR2(1) NULL,
    Adder NUMBER DEFAULT 0.0 NOT NULL,
    Multiplier NUMBER DEFAULT 1.0 NOT NULL,
    CompanyAcronym VARCHAR2(200) NULL,
    Longitude NUMBER(9, 6) NULL,
    Latitude NUMBER(9, 6) NULL,
    Description VARCHAR2(4000) NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL
);

CREATE TABLE Statistic(
    ID NUMBER NOT NULL,
    Source VARCHAR2(20) NOT NULL,
    SignalIndex NUMBER NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    Description VARCHAR2(4000) NULL,
    AssemblyName VARCHAR2(4000) NOT NULL,
    TypeName VARCHAR2(4000) NOT NULL,
    MethodName VARCHAR2(200) NOT NULL,
    Arguments VARCHAR2(4000) NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    DataType VARCHAR2(200) NULL,
    DisplayFormat VARCHAR2(200) NULL,
    IsConnectedState NUMBER DEFAULT 0 NOT NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL
);

CREATE UNIQUE INDEX IX_Statistic_ID ON Statistic (ID ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_Statistic_Source_SigIndex ON Statistic (Source ASC, SignalIndex ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Statistic ADD CONSTRAINT PK_Statistic PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_Statistic START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Statistic BEFORE INSERT ON Statistic
    FOR EACH ROW BEGIN SELECT SEQ_Statistic.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE OutputStreamMeasurement(
    NodeID VARCHAR2(36) NOT NULL,
    AdapterID NUMBER NOT NULL,
    ID NUMBER NOT NULL,
    HistorianID NUMBER NULL,
    PointID NUMBER NOT NULL,
    SignalReference VARCHAR2(200) NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_OutputStreamMeasurement_ID ON OutputStreamMeasurement (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE OutputStreamMeasurement ADD CONSTRAINT PK_OutputStreamMeasurement PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_OutputStreamMeasurement START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_OutputStreamMeasurement BEFORE INSERT ON OutputStreamMeasurement
    FOR EACH ROW BEGIN SELECT SEQ_OutputStreamMeasurement.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE OutputStreamDevice(
    NodeID VARCHAR2(36) NOT NULL,
    AdapterID NUMBER NOT NULL,
    ID NUMBER NOT NULL,
    IDCode NUMBER DEFAULT 0 NOT NULL,
    Acronym VARCHAR2(200) NOT NULL,
    BpaAcronym VARCHAR2(4) NULL,
    Name VARCHAR2(200) NOT NULL,
    PhasorDataFormat VARCHAR2(15) NULL,
    FrequencyDataFormat VARCHAR2(15) NULL,
    AnalogDataFormat VARCHAR2(15) NULL,
    CoordinateFormat VARCHAR2(15) NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_OutputStreamDevice_ID ON OutputStreamDevice (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE OutputStreamDevice ADD CONSTRAINT PK_OutputStreamDevice PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_OutputStreamDevice START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_OutputStreamDevice BEFORE INSERT ON OutputStreamDevice
    FOR EACH ROW BEGIN SELECT SEQ_OutputStreamDevice.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE Phasor(
    ID NUMBER NOT NULL,
    DeviceID NUMBER NOT NULL,
    Label VARCHAR2(200) NOT NULL,
    Type VARCHAR2(1) DEFAULT 'V' NOT NULL,
    Phase VARCHAR2(1) DEFAULT '+' NOT NULL,
    DestinationPhasorID NUMBER NULL,
    SourceIndex NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_Phasor_ID ON Phasor (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Phasor ADD CONSTRAINT PK_Phasor PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_Phasor START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Phasor BEFORE INSERT ON Phasor
    FOR EACH ROW BEGIN SELECT SEQ_Phasor.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE CalculatedMeasurement(
    NodeID VARCHAR2(36) NOT NULL,
    ID NUMBER NOT NULL,
    Acronym VARCHAR2(200) NOT NULL,
    Name VARCHAR2(200) NULL,
    AssemblyName VARCHAR2(4000) NOT NULL,
    TypeName VARCHAR2(4000) NOT NULL,
    ConnectionString VARCHAR2(4000) NULL,
    ConfigSection VARCHAR2(200) NULL,
    InputMeasurements VARCHAR2(4000) NULL,
    OutputMeasurements VARCHAR2(4000) NULL,
    MinimumMeasurementsToUse NUMBER DEFAULT -1 NOT NULL,
    FramesPerSecond NUMBER DEFAULT 30 NOT NULL,
    LagTime NUMBER DEFAULT 3.0 NOT NULL,
    LeadTime NUMBER DEFAULT 1.0 NOT NULL,
    UseLocalClockAsRealTime NUMBER DEFAULT 0 NOT NULL,
    AllowSortsByArrival NUMBER DEFAULT 1 NOT NULL,
    IgnoreBadTimeStamps NUMBER DEFAULT 0 NOT NULL,
    TimeResolution NUMBER DEFAULT 10000 NOT NULL,
    AllowPreemptivePublishing NUMBER DEFAULT 1 NOT NULL,
    PerformTimeReasonabilityCheck NUMBER DEFAULT 1 NOT NULL,
    DownsamplingMethod VARCHAR2(15) DEFAULT 'LastReceived' NOT NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_CalculatedMeasurement_ID ON CalculatedMeasurement (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE CalculatedMeasurement ADD CONSTRAINT PK_CalculatedMeasurement PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_CalculatedMeasurement START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_CalculatedMeasurement BEFORE INSERT ON CalculatedMeasurement
    FOR EACH ROW BEGIN SELECT SEQ_CalculatedMeasurement.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE CustomActionAdapter(
    NodeID VARCHAR2(36) NOT NULL,
    ID NUMBER NOT NULL,
    AdapterName VARCHAR2(200) NOT NULL,
    AssemblyName VARCHAR2(4000) NOT NULL,
    TypeName VARCHAR2(4000) NOT NULL,
    ConnectionString VARCHAR2(4000) NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_CustomActionAdapter_ID ON CustomActionAdapter (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE CustomActionAdapter ADD CONSTRAINT PK_CustomActionAdapter PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_CustomActionAdapter START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_CustomActionAdapter BEFORE INSERT ON CustomActionAdapter
    FOR EACH ROW BEGIN SELECT SEQ_CustomActionAdapter.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE Historian(
    NodeID VARCHAR2(36) NOT NULL,
    ID NUMBER NOT NULL,
    Acronym VARCHAR2(200) NOT NULL,
    Name VARCHAR2(200) NULL,
    AssemblyName VARCHAR2(4000) NULL,
    TypeName VARCHAR2(4000) NULL,
    ConnectionString VARCHAR2(4000) NULL,
    IsLocal NUMBER DEFAULT 1 NOT NULL,
    MeasurementReportingInterval NUMBER DEFAULT 100000 NOT NULL,
    Description VARCHAR2(4000) NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_Historian_ID ON Historian (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Historian ADD CONSTRAINT PK_Historian PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_Historian START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Historian BEFORE INSERT ON Historian
    FOR EACH ROW BEGIN SELECT SEQ_Historian.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE CustomInputAdapter(
    NodeID VARCHAR2(36) NOT NULL,
    ID NUMBER NOT NULL,
    AdapterName VARCHAR2(200) NOT NULL,
    AssemblyName VARCHAR2(4000) NOT NULL,
    TypeName VARCHAR2(4000) NOT NULL,
    ConnectionString VARCHAR2(4000) NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_CustomInputAdapter ON CustomInputAdapter (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE CustomInputAdapter ADD CONSTRAINT PK_CustomInputAdapter PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_CustomInputAdapter START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_CustomInputAdapter BEFORE INSERT ON CustomInputAdapter
    FOR EACH ROW BEGIN SELECT SEQ_CustomInputAdapter.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE CustomFilterAdapter(
    NodeID VARCHAR2(36) NOT NULL,
    ID NUMBER NOT NULL,
    AdapterName VARCHAR2(200) NOT NULL,
    AssemblyName VARCHAR2(4000) NOT NULL,
    TypeName VARCHAR2(4000) NOT NULL,
    ConnectionString VARCHAR2(4000) NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_CustomFilterAdapter ON CustomFilterAdapter (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE CustomFilterAdapter ADD CONSTRAINT PK_CustomFilterAdapter PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_CustomFilterAdapter START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_CustomFilterAdapter BEFORE INSERT ON CustomFilterAdapter
    FOR EACH ROW BEGIN SELECT SEQ_CustomFilterAdapter.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE OutputStream(
    NodeID VARCHAR2(36) NOT NULL,
    ID NUMBER NOT NULL,
    Acronym VARCHAR2(200) NOT NULL,
    Name VARCHAR2(200) NULL,
    Type NUMBER DEFAULT 0 NOT NULL,
    ConnectionString VARCHAR2(4000) NULL,
    DataChannel VARCHAR2(4000) NULL,
    CommandChannel VARCHAR2(4000) NULL,
    IDCode NUMBER DEFAULT 0 NOT NULL,
    AutoPublishConfigFrame NUMBER DEFAULT 0 NOT NULL,
    AutoStartDataChannel NUMBER DEFAULT 1 NOT NULL,
    NominalFrequency NUMBER DEFAULT 60 NOT NULL,
    FramesPerSecond NUMBER DEFAULT 30 NOT NULL,
    LagTime NUMBER DEFAULT 3.0 NOT NULL,
    LeadTime NUMBER DEFAULT 1.0 NOT NULL,
    UseLocalClockAsRealTime NUMBER DEFAULT 0 NOT NULL,
    AllowSortsByArrival NUMBER DEFAULT 1 NOT NULL,
    IgnoreBadTimeStamps NUMBER DEFAULT 0 NOT NULL,
    TimeResolution NUMBER DEFAULT 330000 NOT NULL,
    AllowPreemptivePublishing NUMBER DEFAULT 1 NOT NULL,
    PerformTimeReasonabilityCheck NUMBER DEFAULT 1 NOT NULL,
    DownsamplingMethod VARCHAR2(15) DEFAULT 'LastReceived' NOT NULL,
    DataFormat VARCHAR2(15) DEFAULT 'FloatingPoint' NOT NULL,
    CoordinateFormat VARCHAR2(15) DEFAULT 'Polar' NOT NULL,
    CurrentScalingValue NUMBER DEFAULT 2423 NOT NULL,
    VoltageScalingValue NUMBER DEFAULT 2725785 NOT NULL,
    AnalogScalingValue NUMBER DEFAULT 1373291 NOT NULL,
    DigitalMaskValue NUMBER DEFAULT -65536 NOT NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_OutputStream_ID ON OutputStream (ID ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_OutputStream_NodeID_Acronym ON OutputStream (NodeID ASC, Acronym ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE OutputStream ADD CONSTRAINT PK_OutputStream PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_OutputStream START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_OutputStream BEFORE INSERT ON OutputStream
    FOR EACH ROW BEGIN SELECT SEQ_OutputStream.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE PowerCalculation(
    NodeID VARCHAR2(36) NULL,
    ID NUMBER NOT NULL,
    CircuitDescription VARCHAR2(4000) NULL,
    VoltageAngleSignalID VARCHAR2(36) NOT NULL,
    VoltageMagSignalID VARCHAR2(36) NOT NULL,
    CurrentAngleSignalID VARCHAR2(36) NOT NULL,
    CurrentMagSignalID VARCHAR2(36) NOT NULL,
    ActivePowerOutputSignalID VARCHAR2(36) NULL,
    ReactivePowerOutputSignalID VARCHAR2(36) NULL,
    ApparentPowerOutputSignalID VARCHAR2(36) NULL,
    Enabled NUMBER NOT NULL
);

CREATE UNIQUE INDEX IX_PowerCalculation_ID ON PowerCalculation (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE PowerCalculation ADD CONSTRAINT PK_PowerCalculation PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_PowerCalculation START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_PowerCalculation BEFORE INSERT ON PowerCalculation
    FOR EACH ROW BEGIN SELECT SEQ_PowerCalculation.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE Alarm(
    NodeID VARCHAR2(36) NOT NULL,
    ID NUMBER NOT NULL,
    TagName VARCHAR2(200) NOT NULL,
    SignalID VARCHAR2(36) NOT NULL,
    AssociatedMeasurementID VARCHAR2(36) NULL,
    Description VARCHAR2(4000) NULL,
    Severity NUMBER NOT NULL,
    Operation NUMBER NOT NULL,
    SetPoint NUMBER NULL,
    Tolerance NUMBER  NULL,
    Delay NUMBER NULL,
    Hysteresis NUMBER NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_Alarm_ID ON Alarm (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Alarm ADD CONSTRAINT PK_Alarm PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_Alarm START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Alarm BEFORE INSERT ON Alarm
    FOR EACH ROW BEGIN SELECT SEQ_Alarm.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE AlarmLog(
    ID NUMBER NOT NULL,
    SignalID VARCHAR2(36) NOT NULL,
    PreviousState NUMBER NULL,
    NewState NUMBER NULL,
    Ticks NUMBER NOT NULL,
    Timestamp TIMESTAMP NOT NULL,
    Value NUMBER NOT NULL
);

CREATE UNIQUE INDEX IX_AlarmLog_ID ON AlarmLog (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE AlarmLog ADD CONSTRAINT PK_AlarmLog PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_AlarmLog START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_AlarmLog BEFORE INSERT ON AlarmLog
    FOR EACH ROW BEGIN SELECT SEQ_AlarmLog.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE CustomOutputAdapter(
    NodeID VARCHAR2(36) NOT NULL,
    ID NUMBER NOT NULL,
    AdapterName VARCHAR2(200) NOT NULL,
    AssemblyName VARCHAR2(4000) NOT NULL,
    TypeName VARCHAR2(4000) NOT NULL,
    ConnectionString VARCHAR2(4000) NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_CustomOutputAdapter_ID ON CustomOutputAdapter (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE CustomOutputAdapter ADD CONSTRAINT PK_CustomOutputAdapter PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_CustomOutputAdapter START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_CustomOutputAdapter BEFORE INSERT ON CustomOutputAdapter
    FOR EACH ROW BEGIN SELECT SEQ_CustomOutputAdapter.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE AccessLog (
    ID NUMBER NOT NULL,
    UserName VARCHAR2(200) NOT NULL,
    AccessGranted NUMBER NOT NULL,
    "COMMENT" VARCHAR2(4000),
    CreatedOn DATE NOT NULL
);

CREATE UNIQUE INDEX IX_AccessLog_ID ON AccessLog (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE AccessLog ADD CONSTRAINT PK_AccessLog PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_AccessLog START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_AccessLog BEFORE INSERT ON AccessLog
    FOR EACH ROW BEGIN SELECT SEQ_AccessLog.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE UserAccount (
    ID VARCHAR2(36) NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    Password VARCHAR2(200) DEFAULT NULL,
    FirstName VARCHAR2(200) DEFAULT NULL,
    LastName VARCHAR2(200) DEFAULT NULL,
    DefaultNodeID VARCHAR2(36) NOT NULL,
    Phone VARCHAR2(200) DEFAULT NULL,
    Email VARCHAR2(200) DEFAULT NULL,
    LockedOut NUMBER DEFAULT 0 NOT NULL,
    UseADAuthentication NUMBER DEFAULT 1 NOT NULL,
    ChangePasswordOn DATE DEFAULT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_UserAccount_ID ON UserAccount (ID ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_UserAccount_Name ON UserAccount (Name ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE UserAccount ADD CONSTRAINT PK_UserAccount PRIMARY KEY (ID);

CREATE TABLE SecurityGroup (
    ID VARCHAR2(36) NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    Description VARCHAR2(4000),
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_SecurityGroup_ID ON SecurityGroup (ID ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_SecurityGroup_Name ON SecurityGroup (Name ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE SecurityGroup ADD CONSTRAINT PK_SecurityGroup PRIMARY KEY (ID);

CREATE TABLE ApplicationRole (
    ID VARCHAR2(36) NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    Description VARCHAR2(4000),
    NodeID VARCHAR2(36) NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_ApplicationRole_ID ON ApplicationRole (ID ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_ApplicationRole_NodeID_Name ON ApplicationRole (NodeID ASC, Name ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE ApplicationRole ADD CONSTRAINT PK_ApplicationRole PRIMARY KEY (ID);

CREATE TABLE ApplicationRoleSecurityGroup (
    ApplicationRoleID VARCHAR2(36) NOT NULL,
    SecurityGroupID VARCHAR2(36) NOT NULL  
);

CREATE TABLE ApplicationRoleUserAccount (
    ApplicationRoleID VARCHAR2(36) NOT NULL,
    UserAccountID VARCHAR2(36) NOT NULL  
);

CREATE TABLE SecurityGroupUserAccount (
    SecurityGroupID VARCHAR2(36) NOT NULL,
    UserAccountID VARCHAR2(36) NOT NULL  
);

CREATE TABLE Subscriber (
    NodeID VARCHAR2(36) NOT NULL,
    ID VARCHAR2(36) NOT NULL,
    Acronym VARCHAR2(200) NOT NULL,
    Name VARCHAR2(200) NULL,
    SharedSecret VARCHAR2(200) NULL,
    AuthKey VARCHAR2(4000) NULL,
    ValidIPAddresses VARCHAR2(4000) NULL,
    RemoteCertificateFile VARCHAR2(500) NULL,
    ValidPolicyErrors VARCHAR2(200) NULL,
    ValidChainFlags VARCHAR2(500) NULL,
    AccessControlFilter VARCHAR2(4000) NULL,
    Enabled NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_Subscriber_NodeID_ID ON Subscriber (NodeID ASC, ID ASC) TABLESPACE GSFSchema_INDEX;

CREATE UNIQUE INDEX IX_Subscriber_NodeID_Acronym ON Subscriber (NodeID ASC, Acronym ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE Subscriber ADD CONSTRAINT PK_Subscriber PRIMARY KEY (NodeID, ID);

CREATE TABLE SubscriberMeasurement(
    NodeID VARCHAR2(36) NOT NULL,
    SubscriberID VARCHAR2(36) NOT NULL,
    SignalID VARCHAR2(36) NOT NULL,
    Allowed NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_SubscriberMeasurement ON SubscriberMeasurement (NodeID ASC, SubscriberID ASC, SignalID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE SubscriberMeasurement ADD CONSTRAINT PK_SubscriberMeasurement PRIMARY KEY (NodeID, SubscriberID, SignalID);

CREATE TABLE SubscriberMeasurementGroup (
    NodeID VARCHAR2(36) NOT NULL,
    SubscriberID VARCHAR2(36) NOT NULL,
    MeasurementGroupID NUMBER NOT NULL,
    Allowed NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_SubscriberMeasurementGroup ON SubscriberMeasurementGroup (NodeID ASC, SubscriberID ASC, MeasurementGroupID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE SubscriberMeasurementGroup ADD CONSTRAINT PK_SubscriberMeasurementGroup PRIMARY KEY (NodeID, SubscriberID, MeasurementGroupID);

CREATE TABLE MeasurementGroup (
    NodeID VARCHAR2(36) NOT NULL,
    ID NUMBER NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    Description VARCHAR2(4000),
    FilterExpression VARCHAR2(4000),
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_MeasurementGroup_ID ON MeasurementGroup (ID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE MeasurementGroup ADD CONSTRAINT PK_MeasurementGroup PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_MeasurementGroup START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_MeasurementGroup BEFORE INSERT ON MeasurementGroup
    FOR EACH ROW BEGIN SELECT SEQ_MeasurementGroup.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE MeasurementGroupMeasurement (
    NodeID VARCHAR2(36) NOT NULL,
    MeasurementGroupID NUMBER NOT NULL,
    SignalID VARCHAR2(36) NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_MeasurementGroupMeasurement ON MeasurementGroupMeasurement (NodeID ASC, MeasurementGroupID ASC, SignalID ASC) TABLESPACE GSFSchema_INDEX;

ALTER TABLE MeasurementGroupMeasurement ADD CONSTRAINT PK_MeasurementGroupMeasurement PRIMARY KEY (NodeID, MeasurementGroupID, SignalID);

ALTER TABLE Subscriber ADD CONSTRAINT FK_Subscriber_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE SubscriberMeasurement ADD CONSTRAINT FK_SubscriberMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE SubscriberMeasurement ADD CONSTRAINT FK_SubscriberMeasure_Measure FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE SubscriberMeasurement ADD CONSTRAINT FK_SubscribeMeasure_Subscribe FOREIGN KEY(NodeID, SubscriberID) REFERENCES Subscriber (NodeID, ID) ON DELETE CASCADE;

ALTER TABLE SubscriberMeasurementGroup ADD CONSTRAINT FK_SubscriberMeasureGrp_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE SubscriberMeasurementGroup ADD CONSTRAINT FK_SubscribMeasureGrp_Subscrib FOREIGN KEY(NodeID, SubscriberID) REFERENCES Subscriber (NodeID, ID) ON DELETE CASCADE;

ALTER TABLE SubscriberMeasurementGroup ADD CONSTRAINT FK_SubscribMeasurGrp_MeasurGrp FOREIGN KEY(MeasurementGroupID) REFERENCES MeasurementGroup (ID);

ALTER TABLE MeasurementGroup ADD CONSTRAINT FK_MeasurementGroup_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE MeasurementGroupMeasurement ADD CONSTRAINT FK_MeasureGrpMeasure_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE MeasurementGroupMeasurement ADD CONSTRAINT FK_MeasureGrpMeasure_Measure FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE MeasurementGroupMeasurement ADD CONSTRAINT FK_MeasurGrpMeasur_MeasurGrp FOREIGN KEY(MeasurementGroupID) REFERENCES MeasurementGroup (ID) ON DELETE CASCADE;

ALTER TABLE Node ADD CONSTRAINT FK_Node_Company FOREIGN KEY(CompanyID) REFERENCES Company (ID);

ALTER TABLE DataOperation ADD CONSTRAINT FK_DataOperation_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

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

ALTER TABLE OutputStreamDeviceDigital ADD CONSTRAINT FK_OutStreamDeviceDigital_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputStreamDeviceDigital ADD CONSTRAINT FK_OutStrDevDigital_OutStrDev FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE;

ALTER TABLE OutputStreamDevicePhasor ADD CONSTRAINT FK_OutStreamDevicePhasor_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputStreamDevicePhasor ADD CONSTRAINT FK_OutStrDevPhasor_OutStrDev FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE;

ALTER TABLE OutputStreamDeviceAnalog ADD CONSTRAINT FK_OutStreamDeviceAnalog_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputStreamDeviceAnalog ADD CONSTRAINT FK_OutStrDevAnalog_OutStDev FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE;

ALTER TABLE Measurement ADD CONSTRAINT FK_Measurement_Device FOREIGN KEY(DeviceID) REFERENCES Device (ID) ON DELETE CASCADE;

ALTER TABLE Measurement ADD CONSTRAINT FK_Measurement_SignalType FOREIGN KEY(SignalTypeID) REFERENCES SignalType (ID);

ALTER TABLE ImportedMeasurement ADD CONSTRAINT FK_ImportedMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE OutputStreamMeasurement ADD CONSTRAINT FK_OutStrMeasurement_Historian FOREIGN KEY(HistorianID) REFERENCES Historian (ID);

ALTER TABLE OutputStreamMeasurement ADD CONSTRAINT FK_OutStrMeasure_Measure FOREIGN KEY(PointID) REFERENCES Measurement (PointID) ON DELETE CASCADE;

ALTER TABLE OutputStreamMeasurement ADD CONSTRAINT FK_OutStreamMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputStreamMeasurement ADD CONSTRAINT FK_OutStrMeasurement_OutStr FOREIGN KEY(AdapterID) REFERENCES OutputStream (ID) ON DELETE CASCADE;

ALTER TABLE OutputStreamDevice ADD CONSTRAINT FK_OutputStreamDevice_Node FOREIGN KEY(NodeID) REFERENCES Node (ID);

ALTER TABLE OutputStreamDevice ADD CONSTRAINT FK_OutStreamDevice_OutStream FOREIGN KEY(AdapterID) REFERENCES OutputStream (ID) ON DELETE CASCADE;

ALTER TABLE Phasor ADD CONSTRAINT FK_Phasor_Device FOREIGN KEY(DeviceID) REFERENCES Device (ID) ON DELETE CASCADE;

ALTER TABLE Phasor ADD CONSTRAINT FK_Phasor_Phasor FOREIGN KEY(DestinationPhasorID) REFERENCES Phasor (ID);

ALTER TABLE CalculatedMeasurement ADD CONSTRAINT FK_CalculatedMeasurement_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE CustomActionAdapter ADD CONSTRAINT FK_CustomActionAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE Historian ADD CONSTRAINT FK_Historian_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE CustomInputAdapter ADD CONSTRAINT FK_CustomInputAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE CustomFilterAdapter ADD CONSTRAINT FK_CustomFilterAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE OutputStream ADD CONSTRAINT FK_OutputStream_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement1 FOREIGN KEY(ApparentPowerOutputSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement2 FOREIGN KEY(CurrentAngleSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement3 FOREIGN KEY(CurrentMagSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement4 FOREIGN KEY(ReactivePowerOutputSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement5 FOREIGN KEY(ActivePowerOutputSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement6 FOREIGN KEY(VoltageAngleSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE PowerCalculation ADD CONSTRAINT FK_PowerCalculation_Measurement7 FOREIGN KEY(VoltageMagSignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE Alarm ADD CONSTRAINT FK_Alarm_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE Alarm ADD CONSTRAINT FK_Alarm_Measurement_SignalID FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE Alarm ADD CONSTRAINT FK_Alarm_Meas_AssocMeasID FOREIGN KEY(AssociatedMeasurementID) REFERENCES Measurement (SignalID);

ALTER TABLE AlarmLog ADD CONSTRAINT FK_AlarmLog_Measurement FOREIGN KEY(SignalID) REFERENCES Measurement (SignalID) ON DELETE CASCADE;

ALTER TABLE AlarmLog ADD CONSTRAINT FK_AlarmLog_Alarm_PrevState FOREIGN KEY(PreviousState) REFERENCES Alarm (ID) ON DELETE CASCADE;

ALTER TABLE AlarmLog ADD CONSTRAINT FK_AlarmLog_Alarm_NewState FOREIGN KEY(NewState) REFERENCES Alarm (ID) ON DELETE CASCADE;

ALTER TABLE CustomOutputAdapter ADD CONSTRAINT FK_CustomOutputAdapter_Node FOREIGN KEY(NodeID) REFERENCES Node (ID) ON DELETE CASCADE;

ALTER TABLE ApplicationRoleSecurityGroup ADD CONSTRAINT FK_AppRoleSecurityGrp_AppRole FOREIGN KEY (ApplicationRoleID) REFERENCES applicationrole (ID) ON DELETE CASCADE;

ALTER TABLE ApplicationRoleSecurityGroup ADD CONSTRAINT FK_AppRoleSecurGrp_SecurGrp FOREIGN KEY (SecurityGroupID) REFERENCES securitygroup (ID) ON DELETE CASCADE;

ALTER TABLE UserAccount ADD CONSTRAINT FK_useraccount FOREIGN KEY (DefaultNodeID) REFERENCES node (ID) ON DELETE CASCADE;

ALTER TABLE ApplicationRole ADD CONSTRAINT FK_applicationrole FOREIGN KEY (NodeID) REFERENCES node (ID) ON DELETE CASCADE;

ALTER TABLE ApplicationRoleUserAccount ADD CONSTRAINT FK_AppRoleUserAcct_UserAcct FOREIGN KEY (UserAccountID) REFERENCES useraccount (ID) ON DELETE CASCADE;

ALTER TABLE ApplicationRoleUserAccount ADD CONSTRAINT FK_AppRoleUserAccount_AppRole FOREIGN KEY (ApplicationRoleID) REFERENCES applicationrole (ID) ON DELETE CASCADE;

ALTER TABLE SecurityGroupUserAccount ADD CONSTRAINT FK_SecurityGrpUsrAcct_UsrAcct FOREIGN KEY (UserAccountID) REFERENCES useraccount (ID) ON DELETE CASCADE;

ALTER TABLE SecurityGroupUserAccount ADD CONSTRAINT FK_SecurGrpUsrAcct_SecurGrp FOREIGN KEY (SecurityGroupID) REFERENCES securitygroup (ID) ON DELETE CASCADE;

-- VIEWS

CREATE VIEW NodeInfo
AS
SELECT Node.ID AS NodeID, Node.Name, Company.Name AS CompanyName, Node.Longitude,
    Node.Latitude, Node.Description, Node.ImagePath, Node.Settings, Node.MenuType,
    Node.MenuData, Node.Master, Node.Enabled
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
SELECT   Historian.NodeID,
              Runtime.ID,
              Historian.Acronym AS AdapterName,
              CASE TRIM (Historian.AssemblyName)
                 WHEN '0' THEN 'HistorianAdapters.dll'
                 ELSE Historian.AssemblyName
              END
                 AS AssemblyName,
              CASE TRIM (Historian.TypeName)
                 WHEN '0'
                 THEN
                    (CASE IsLocal
                        WHEN 1 THEN 'HistorianAdapters.LocalOutputAdapter'
                        ELSE 'HistorianAdapters.RemoteOutputAdapter'
                     END)
                 ELSE
                    Historian.TypeName
              END
                 AS TypeName,
              (   Historian.ConnectionString
               || ';'
               || ('instanceName=' || Historian.Acronym)
               || ';'
               || ('sourceids=' || Historian.Acronym)
               || ';'
               || 'measurementReportingInterval='
               || Historian.MeasurementReportingInterval)
                 AS ConnectionString
       FROM      Historian
              LEFT OUTER JOIN
                 Runtime
              ON Historian.ID = Runtime.SourceID
                 AND Runtime.SourceTable = 'Historian'
      WHERE   (Historian.Enabled <> 0)
   ORDER BY   Historian.LoadOrder;

CREATE VIEW RuntimeDevice
AS
SELECT Device.NodeID, Runtime.ID, Device.Acronym AS AdapterName, Protocol.AssemblyName AS AssemblyName, Protocol.TypeName AS TypeName,
    Device.ConnectionString || ';' ||
    'isConcentrator=' || Device.IsConcentrator || ';' ||
    'accessID=' || Device.AccessID || ';' ||
    NVL2(Device.TimeZone, 'timeZone=' || Device.TimeZone, '') || ';' ||
    'timeAdjustmentTicks=' || Device.TimeAdjustmentTicks || ';' ||
    NVL2(Protocol.Acronym, 'phasorProtocol=' || Protocol.Acronym, '') || ';' ||
    'dataLossInterval=' || Device.DataLossInterval || ';' ||
    'allowedParsingExceptions=' || Device.AllowedParsingExceptions || ';' ||
    'parsingExceptionWindow=' || Device.ParsingExceptionWindow || ';' ||
    'delayedConnectionInterval=' || Device.DelayedConnectionInterval || ';' ||
    'allowUseOfCachedConfiguration=' || Device.AllowUseOfCachedConfiguration || ';' ||
    'autoStartDataParsingSequence=' || Device.AutoStartDataParsingSequence || ';' ||
    'skipDisableRealTimeData=' || Device.SkipDisableRealTimeData || ';' ||
    'measurementReportingInterval=' || Device.MeasurementReportingInterval || ';' ||
    'connectOnDemand=' || Device.ConnectOnDemand AS ConnectionString
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
    Runtime Runtime_P ON Device.ParentID = Runtime_P.SourceID AND Runtime_P.SourceTable = 'Device'
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

CREATE VIEW RuntimeCustomFilterAdapter
AS
SELECT CustomFilterAdapter.NodeID, Runtime.ID, CustomFilterAdapter.AdapterName, 
    TRIM(CustomFilterAdapter.AssemblyName) AS AssemblyName, TRIM(CustomFilterAdapter.TypeName) AS TypeName, CustomFilterAdapter.ConnectionString
FROM CustomFilterAdapter LEFT OUTER JOIN
    Runtime ON CustomFilterAdapter.ID = Runtime.SourceID AND Runtime.SourceTable = 'CustomFilterAdapter'
WHERE (CustomFilterAdapter.Enabled <> 0)
ORDER BY CustomFilterAdapter.LoadOrder;

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
    'PhasorProtocolAdapters.dll' AS AssemblyName, 
    CASE Type WHEN 1 THEN 'PhasorProtocolAdapters.BpaPdcStream.Concentrator' WHEN 2 THEN 'PhasorProtocolAdapters.Iec61850_90_5.Concentrator' ELSE 'PhasorProtocolAdapters.IeeeC37_118.Concentrator' END AS TypeName,
    OutputStream.ConnectionString || ';' ||
    NVL2(OutputStream.DataChannel, 'dataChannel={' || OutputStream.DataChannel || '}', '') || ';' ||
    NVL2(OutputStream.CommandChannel, 'commandChannel={' || OutputStream.CommandChannel || '}', '') || ';' ||
    'idCode=' || OutputStream.IDCode || ';' ||
    'autoPublishConfigFrame=' || OutputStream.AutoPublishConfigFrame || ';' ||
    'autoStartDataChannel=' || OutputStream.AutoStartDataChannel || ';' ||
    'nominalFrequency=' || OutputStream.NominalFrequency || ';' ||
    'lagTime=' || OutputStream.LagTime || ';' ||
    'leadTime=' || OutputStream.LeadTime || ';' ||
    'framesPerSecond=' || OutputStream.FramesPerSecond || ';' ||
    'useLocalClockAsRealTime=' || OutputStream.UseLocalClockAsRealTime || ';' ||
    'allowSortsByArrival=' || OutputStream.AllowSortsByArrival || ';' ||
    'ignoreBadTimestamps=' || OutputStream.IgnoreBadTimeStamps || ';' ||
    'timeResolution=' || OutputStream.TimeResolution || ';' ||
    'allowPreemptivePublishing=' || OutputStream.AllowPreemptivePublishing || ';' ||
    'downsamplingMethod=' || OutputStream.DownsamplingMethod || ';' ||
    'dataFormat=' || OutputStream.DataFormat || ';' ||
    'coordinateFormat=' || OutputStream.CoordinateFormat || ';' ||
    'currentScalingValue=' || OutputStream.CurrentScalingValue || ';' ||
    'voltageScalingValue=' || OutputStream.VoltageScalingValue || ';' ||
    'analogScalingValue=' || OutputStream.AnalogScalingValue || ';' ||
    'performTimestampReasonabilityCheck=' || OutputStream.PerformTimeReasonabilityCheck || ';' ||
    'digitalMaskValue=' || OutputStream.DigitalMaskValue AS ConnectionString
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
    NVL2(CalculatedMeasurement.ConnectionString, CalculatedMeasurement.ConnectionString, '') || ';' ||
    NVL2(ConfigSection, 'configurationSection=' || ConfigSection, '') || ';' ||
    'minimumMeasurementsToUse=' || CalculatedMeasurement.MinimumMeasurementsToUse || ';' ||
    'framesPerSecond=' || CalculatedMeasurement.FramesPerSecond || ';' ||
    'lagTime=' || CalculatedMeasurement.LagTime || ';' ||
    'leadTime=' || CalculatedMeasurement.LeadTime || ';' ||
    NVL2(InputMeasurements, 'inputMeasurementKeys={' || InputMeasurements || '}', '') || ';' ||
    NVL2(OutputMeasurements, 'outputMeasurements={' || OutputMeasurements || '}', '') || ';' ||
    'ignoreBadTimestamps=' || CalculatedMeasurement.IgnoreBadTimeStamps || ';' ||
    'timeResolution=' || CalculatedMeasurement.TimeResolution || ';' ||
    'allowPreemptivePublishing=' || CalculatedMeasurement.AllowPreemptivePublishing || ';' ||
    'performTimestampReasonabilityCheck=' || CalculatedMeasurement.PerformTimeReasonabilityCheck || ';' ||
    'downsamplingMethod=' || CalculatedMeasurement.DownsamplingMethod || ';' ||
    'useLocalClockAsRealTime=' || CalculatedMeasurement.UseLocalClockAsRealTime AS ConnectionString
FROM CalculatedMeasurement LEFT OUTER JOIN
    Runtime ON CalculatedMeasurement.ID = Runtime.SourceID AND Runtime.SourceTable = 'CalculatedMeasurement'
WHERE (CalculatedMeasurement.Enabled <> 0)
ORDER BY CalculatedMeasurement.LoadOrder;

CREATE VIEW ActiveMeasurement
AS
SELECT Node.ID AS NodeID, COALESCE(Device.NodeID, Historian.NodeID) AS SourceNodeID, COALESCE(Historian.Acronym, Device.Acronym, '__') || ':' ||
    Measurement.PointID AS ID, Measurement.SignalID, Measurement.PointTag, Measurement.AlternateTag, Measurement.SignalReference AS SignalReference, Measurement.Internal, Measurement.Subscribed,
    Device.Acronym AS Device, CASE WHEN Device.IsConcentrator = 0 AND Device.ParentID IS NOT NULL THEN RuntimeP.ID ELSE Runtime.ID END AS DeviceID, COALESCE(Device.FramesPerSecond, 30) AS FramesPerSecond, 
    Protocol.Acronym AS Protocol, Protocol.Type AS ProtocolType, SignalType.Acronym AS SignalType, SignalType.EngineeringUnits, Phasor.ID AS PhasorID, Phasor.Type AS PhasorType, Phasor.Phase, Measurement.Adder, 
    Measurement.Multiplier, Company.Acronym AS Company, Device.Longitude, Device.Latitude, Measurement.Description, Measurement.UpdatedOn
FROM Company RIGHT OUTER JOIN
    Device ON Company.ID = Device.CompanyID RIGHT OUTER JOIN
    Measurement LEFT OUTER JOIN
    SignalType ON Measurement.SignalTypeID = SignalType.ID ON Device.ID = Measurement.DeviceID LEFT OUTER JOIN
    Phasor ON Measurement.DeviceID = Phasor.DeviceID AND 
    Measurement.PhasorSourceIndex = Phasor.SourceIndex LEFT OUTER JOIN
    Protocol ON Device.ProtocolID = Protocol.ID LEFT OUTER JOIN
    Historian ON Measurement.HistorianID = Historian.ID LEFT OUTER JOIN
    Runtime ON Device.ID = Runtime.SourceID AND Runtime.SourceTable = 'Device' LEFT OUTER JOIN
    Runtime RuntimeP ON RuntimeP.SourceID = Device.ParentID AND RuntimeP.SourceTable = 'Device'
    CROSS JOIN Node
WHERE (Device.Enabled <> 0 OR Device.Enabled IS NULL) AND (Measurement.Enabled <> 0)
UNION ALL
SELECT NodeID, SourceNodeID, Source || ':' || PointID AS ID, SignalID, PointTag,
    AlternateTag, SignalReference, 0 AS Internal, 1 AS Subscribed, NULL AS Device, NULL AS DeviceID,
    FramesPerSecond, ProtocolAcronym AS Protocol, ProtocolType, SignalTypeAcronym AS SignalType, '' AS EngineeringUnits, PhasorID, PhasorType, Phase, Adder, Multiplier,
    CompanyAcronym AS Company, Longitude, Latitude, Description, SYSDATE AS UpdatedOn
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
UNION ALL
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeCustomOutputAdapter;

CREATE VIEW IaonInputAdapter
AS
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeDevice
UNION ALL
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeCustomInputAdapter;

CREATE VIEW IaonActionAdapter
AS
SELECT Node.ID AS NodeID, 0 AS ID, 'PHASOR!SERVICES' AS AdapterName, 'PhasorProtocolAdapters.dll' AS AssemblyName, 'PhasorProtocolAdapters.CommonPhasorServices' AS TypeName, '' AS ConnectionString
FROM Node
UNION ALL
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeOutputStream
UNION ALL
SELECT NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM RuntimeCalculatedMeasurement
UNION ALL
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
    Measurement.PhasorSourceIndex, Phasor.Label AS PhasorLabel, Phasor.Type AS PhasorType, Phasor.Phase, 
    Measurement.SignalReference, Measurement.Adder, Measurement.Multiplier, Measurement.Description, Measurement.Subscribed, Measurement.Internal, Measurement.Enabled, 
    COALESCE (SignalType.EngineeringUnits, '') AS EngineeringUnits, SignalType.Source, SignalType.Acronym AS SignalAcronym, 
    SignalType.Name AS SignalName, SignalType.Suffix AS SignalTypeSuffix, Device.Longitude, Device.Latitude,
    COALESCE(Historian.Acronym, Device.Acronym, '__') || ':' || Measurement.PointID AS ID, Measurement.UpdatedOn
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
SELECT PointID AS HistorianID, CASE SignalAcronym WHEN 'DIGI' THEN 1 ELSE 0 END AS DataType, PointTag AS Name, SignalReference AS Synonym1, 
    SignalAcronym AS Synonym2, AlternateTag AS Synonym3, Description, VendorDeviceDescription AS HardwareInfo, '' AS Remarks, 
    HistorianAcronym AS PlantCode, 1 AS UnitNumber, DeviceAcronym AS SystemName, ProtocolID AS SourceID, Enabled, CAST(1 / FramesPerSecond AS NUMBER(10,10)) AS ScanRate, 
    0 AS CompressionMinTime, 0 AS CompressionMaxTime, EngineeringUnits,
    CASE SignalAcronym WHEN 'FREQ' THEN 59.95 WHEN 'VPHM' THEN 475000 WHEN 'IPHM' THEN 0 WHEN 'VPHA' THEN -181 WHEN 'IPHA' THEN -181 ELSE 0 END AS LowWarning,
    CASE SignalAcronym WHEN 'FREQ' THEN 60.05 WHEN 'VPHM' THEN 525000 WHEN 'IPHM' THEN 3150 WHEN 'VPHA' THEN 181 WHEN 'IPHA' THEN 181 ELSE 0 END AS HighWarning,
    CASE SignalAcronym WHEN 'FREQ' THEN 59.90 WHEN 'VPHM' THEN 450000 WHEN 'IPHM' THEN 0 WHEN 'VPHA' THEN -181 WHEN 'IPHA' THEN -181 ELSE 0 END AS LowAlarm,
    CASE SignalAcronym WHEN 'FREQ' THEN 60.10 WHEN 'VPHM' THEN 550000 WHEN 'IPHM' THEN 3300 WHEN 'VPHA' THEN 181 WHEN 'IPHA' THEN 181 ELSE 0 END AS HighAlarm,
    CASE SignalAcronym WHEN 'FREQ' THEN 59.95 WHEN 'VPHM' THEN 475000 WHEN 'IPHM' THEN 0 WHEN 'VPHA' THEN -180 WHEN 'IPHA' THEN -180 ELSE 0 END AS LowRange,
    CASE SignalAcronym WHEN 'FREQ' THEN 60.05 WHEN 'VPHM' THEN 525000 WHEN 'IPHM' THEN 3000 WHEN 'VPHA' THEN 180 WHEN 'IPHA' THEN 180 ELSE 0 END AS HighRange,
    0.0 AS CompressionLimit, 0.0 AS ExceptionLimit, CASE SignalAcronym WHEN 'DIGI' THEN 0 ELSE 7 END AS DisplayDigits, '' AS SetDescription,
    '' AS ClearDescription, 0 AS AlarmState, 5 AS ChangeSecurity, 0 AS AccessSecurity, 0 AS StepCheck, 0 AS AlarmEnabled, 0 AS AlarmFlags, 0 AS AlarmDelay,
    0 AS AlarmToFile, 0 AS AlarmByEmail, 0 AS AlarmByPager, 0 AS AlarmByPhone, ContactList AS AlarmEmails, '' AS AlarmPagers, '' AS AlarmPhones
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
    ) SignalsWithAlarms
    LEFT OUTER JOIN
    (
        SELECT
            Log1.SignalID,
            Log1.NewState,
            Log1.Timestamp,
            Log1.Value
        FROM
            AlarmLog Log1 LEFT OUTER JOIN
            AlarmLog Log2 ON Log1.SignalID = Log2.SignalID AND Log1.Ticks < Log2.Ticks
        WHERE
            Log2.ID IS NULL
    ) CurrentState
    ON SignalsWithAlarms.SignalID = CurrentState.SignalID;

CREATE VIEW CalculatedMeasurementDetail
AS
SELECT CM.NodeID, CM.ID, CM.Acronym, COALESCE(CM.Name, '') AS Name, CM.AssemblyName, CM.TypeName, CM.ConnectionString AS ConnectionString,
    COALESCE(CM.ConfigSection, '') AS ConfigSection, CM.InputMeasurements AS InputMeasurements, CM.OutputMeasurements AS OutputMeasurements,
    CM.MinimumMeasurementsToUse, CM.FramesPerSecond, CM.LagTime, CM.LeadTime, CM.UseLocalClockAsRealTime, CM.AllowSortsByArrival, CM.LoadOrder, CM.Enabled,
    N.Name AS NodeName, CM.IgnoreBadTimeStamps, CM.TimeResolution, CM.AllowPreemptivePublishing, COALESCE(CM.DownsamplingMethod, '') AS DownsamplingMethod, CM.PerformTimeReasonabilityCheck
FROM CalculatedMeasurement CM, Node N
WHERE CM.NodeID = N.ID;

CREATE VIEW HistorianDetail
AS
SELECT H.NodeID, H.ID, H.Acronym, COALESCE(H.Name, '') AS Name, H.AssemblyName AS AssemblyName, H.TypeName AS TypeName, 
    H.ConnectionString AS ConnectionString, H.IsLocal, H.Description AS Description, H.LoadOrder, H.Enabled, N.Name AS NodeName, H.MeasurementReportingInterval 
FROM Historian H INNER JOIN Node N ON H.NodeID = N.ID;

CREATE VIEW NodeDetail
AS
SELECT N.ID, N.Name, N.CompanyID AS CompanyID, COALESCE(N.Longitude, 0) AS Longitude, COALESCE(N.Latitude, 0) AS Latitude, 
    N.Description AS Description, N.ImagePath AS ImagePath, N.Settings AS Settings, N.MenuType, N.MenuData, N.Master, N.LoadOrder, N.Enabled, COALESCE(C.Name, '') AS CompanyName
FROM Node N LEFT JOIN Company C 
ON N.CompanyID = C.ID;

CREATE VIEW VendorDetail
AS
SELECT ID, COALESCE(Acronym, '') AS Acronym, Name, COALESCE(PhoneNumber, '') AS PhoneNumber, COALESCE(ContactEmail, '') AS ContactEmail, URL AS URL 
FROM Vendor;

CREATE VIEW CustomActionAdapterDetail AS
SELECT CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, CA.ConnectionString AS ConnectionString, CA.LoadOrder, 
    CA.Enabled, N.Name AS NodeName
FROM CustomActionAdapter CA INNER JOIN Node N ON CA.NodeID = N.ID;
 
CREATE VIEW CustomInputAdapterDetail AS
SELECT CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, CA.ConnectionString AS ConnectionString, CA.LoadOrder, 
    CA.Enabled, N.Name AS NodeName
FROM CustomInputAdapter CA INNER JOIN Node N ON CA.NodeID = N.ID;
 
CREATE VIEW CustomOutputAdapterDetail AS
SELECT CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, CA.ConnectionString AS ConnectionString, CA.LoadOrder, 
    CA.Enabled, N.Name AS NodeName
FROM CustomOutputAdapter CA INNER JOIN Node N ON CA.NodeID = N.ID;
 
CREATE VIEW CustomFilterAdapterDetail AS
SELECT CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, CA.ConnectionString AS ConnectionString, CA.LoadOrder, 
    CA.Enabled, N.Name AS NodeName
FROM CustomFilterAdapter CA INNER JOIN Node N ON CA.NodeID = N.ID;
 
CREATE VIEW IaonTreeView AS
SELECT 'Action Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString AS ConnectionString
FROM IaonActionAdapter
UNION ALL
SELECT 'Input Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString AS ConnectionString
FROM IaonInputAdapter
UNION ALL
SELECT 'Output Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString AS ConnectionString
FROM IaonOutputAdapter
UNION ALL
SELECT 'Filter Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString AS ConnectionString
FROM IaonFilterAdapter;
 
CREATE VIEW OtherDeviceDetail AS
SELECT OD.ID, OD.Acronym, COALESCE(OD.Name, '') AS Name, OD.IsConcentrator, OD.CompanyID, OD.VendorDeviceID, OD.Longitude, OD.Latitude, 
    OD.InterconnectionID, OD.Planned, OD.Desired, OD.InProgress, COALESCE(C.Name, '') AS CompanyName, COALESCE(C.Acronym, '') AS CompanyAcronym, 
    COALESCE(C.MapAcronym, '') AS CompanyMapAcronym, COALESCE(VD.Name, '') AS VendorDeviceName, COALESCE(I.Name, '') AS InterconnectionName
FROM OtherDevice OD LEFT OUTER JOIN
    Company C ON OD.CompanyID = C.ID LEFT OUTER JOIN
    VendorDevice VD ON OD.VendorDeviceID = VD.ID LEFT OUTER JOIN
    Interconnection I ON OD.InterconnectionID = I.ID;
 
CREATE VIEW VendorDeviceDistribution AS
SELECT Device.NodeID, Vendor.Name AS VendorName, COUNT(*) AS DeviceCount 
FROM Device LEFT OUTER JOIN 
    VendorDevice ON Device.VendorDeviceID = VendorDevice.ID INNER JOIN
    Vendor ON VendorDevice.VendorID = Vendor.ID
GROUP BY Device.NodeID, Vendor.Name;

CREATE VIEW VendorDeviceDetail
AS
SELECT VD.ID, VD.VendorID, VD.Name, VD.Description AS Description, VD.URL AS URL, V.Name AS VendorName, 
    V.Acronym AS VendorAcronym
FROM VendorDevice VD INNER JOIN Vendor V ON VD.VendorID = V.ID;
                      
CREATE VIEW DeviceDetail
AS
SELECT D.NodeID, D.ID, D.ParentID, D.UniqueID, D.Acronym, COALESCE(D.Name, '') AS Name, D.OriginalSource, D.IsConcentrator, D.CompanyID, D.HistorianID, D.AccessID, D.VendorDeviceID, 
    D.ProtocolID, D.Longitude, D.Latitude, D.InterconnectionID, D.ConnectionString AS ConnectionString, COALESCE(D.TimeZone, '') AS TimeZone, 
    COALESCE(D.FramesPerSecond, 30) AS FramesPerSecond, D.TimeAdjustmentTicks, D.DataLossInterval, D.ConnectOnDemand, D.ContactList AS ContactList, D.MeasuredLines, D.LoadOrder, D.Enabled, COALESCE(C.Name, '') 
    AS CompanyName, COALESCE(C.Acronym, '') AS CompanyAcronym, COALESCE(C.MapAcronym, '') AS CompanyMapAcronym, COALESCE(H.Acronym, '') 
    AS HistorianAcronym, COALESCE(VD.VendorAcronym, '') AS VendorAcronym, COALESCE(VD.Name, '') AS VendorDeviceName, COALESCE(P.Name, '') 
    AS ProtocolName, P.Type AS ProtocolType, P.Category, COALESCE(I.Name, '') AS InterconnectionName, N.Name AS NodeName, COALESCE(PD.Acronym, '') AS ParentAcronym, D.CreatedOn, D.AllowedParsingExceptions, 
    D.ParsingExceptionWindow, D.DelayedConnectionInterval, D.AllowUseOfCachedConfiguration, D.AutoStartDataParsingSequence, D.SkipDisableRealTimeData, 
    D.MeasurementReportingInterval, D.UpdatedOn
FROM Device D LEFT OUTER JOIN
    Company C ON C.ID = D.CompanyID LEFT OUTER JOIN
    Historian H ON H.ID = D.HistorianID LEFT OUTER JOIN
    VendorDeviceDetail VD ON VD.ID = D.VendorDeviceID LEFT OUTER JOIN
    Protocol P ON P.ID = D.ProtocolID LEFT OUTER JOIN
    Interconnection I ON I.ID = D.InterconnectionID LEFT OUTER JOIN
    Node N ON N.ID = D.NodeID LEFT OUTER JOIN
    Device PD ON PD.ID = D.ParentID;
 
CREATE VIEW MapData AS
SELECT 'Device' AS DeviceType, NodeID, ID, Acronym, COALESCE(Name, '') AS Name, CompanyMapAcronym, CompanyName, VendorDeviceName, Longitude, 
    Latitude, 1 AS Reporting, 0 AS Inprogress, 0 AS Planned, 0 AS Desired
FROM DeviceDetail D
UNION ALL
SELECT 'OtherDevice' AS DeviceType, NULL AS NodeID, ID, Acronym, COALESCE(Name, '') AS Name, CompanyMapAcronym, CompanyName, VendorDeviceName, 
    Longitude, Latitude, 0 AS Reporting, 1 AS Inprogress, 1 AS Planned, 1 AS Desired
FROM OtherDeviceDetail OD;

CREATE VIEW OutputStreamDetail AS
SELECT OS.NodeID, OS.ID, OS.Acronym, COALESCE(OS.Name, '') AS Name, OS.Type, OS.ConnectionString AS ConnectionString, OS.IDCode, 
    OS.CommandChannel AS CommandChannel, OS.DataChannel AS DataChannel, OS.AutoPublishConfigFrame, 
    OS.AutoStartDataChannel, OS.NominalFrequency, OS.FramesPerSecond, OS.LagTime, OS.LeadTime, OS.UseLocalClockAsRealTime, 
    OS.AllowSortsByArrival, OS.LoadOrder, OS.Enabled, N.Name AS NodeName, OS.DigitalMaskValue, OS.AnalogScalingValue, 
    OS.VoltageScalingValue, OS.CurrentScalingValue, OS.CoordinateFormat, OS.DataFormat, OS.DownsamplingMethod, 
    OS.AllowPreemptivePublishing, OS.TimeResolution, OS.IgnoreBadTimeStamps, OS.PerformTimeReasonabilityCheck
FROM OutputStream OS INNER JOIN Node N ON OS.NodeID = N.ID;
                      
CREATE VIEW OutputStreamMeasurementDetail AS
SELECT OSM.NodeID, OSM.AdapterID, OSM.ID, OSM.HistorianID, OSM.PointID, OSM.SignalReference, M.PointTag AS SourcePointTag, COALESCE(H.Acronym, '') 
    AS HistorianAcronym
FROM OutputStreamMeasurement OSM INNER JOIN
    Measurement M ON M.PointID = OSM.PointID LEFT OUTER JOIN
    Historian H ON H.ID = OSM.HistorianID;
      
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
SELECT MeasurementDetail.CompanyID, MeasurementDetail.CompanyAcronym, MeasurementDetail.CompanyName, MeasurementDetail.SignalID, MeasurementDetail.HistorianID, MeasurementDetail.HistorianAcronym, MeasurementDetail.HistorianConnectionString, MeasurementDetail.PointID, MeasurementDetail.PointTag, MeasurementDetail.AlternateTag, MeasurementDetail.DeviceID, 
    MeasurementDetail.NodeID, MeasurementDetail.DeviceAcronym, MeasurementDetail.DeviceName, MeasurementDetail.FramesPerSecond, MeasurementDetail.DeviceEnabled, MeasurementDetail.ContactList, MeasurementDetail.VendorDeviceID, MeasurementDetail.VendorDeviceName, MeasurementDetail.VendorDeviceDescription, MeasurementDetail.ProtocolID, 
    MeasurementDetail.ProtocolAcronym, MeasurementDetail.ProtocolName, MeasurementDetail.SignalTypeID, MeasurementDetail.PhasorSourceIndex, MeasurementDetail.PhasorLabel, MeasurementDetail.PhasorType, MeasurementDetail.Phase, MeasurementDetail.SignalReference, MeasurementDetail.Adder, MeasurementDetail.Multiplier, MeasurementDetail.Description, 
    MeasurementDetail.Subscribed, MeasurementDetail.Internal, MeasurementDetail.Enabled, MeasurementDetail.EngineeringUnits, MeasurementDetail.Source, MeasurementDetail.SignalAcronym, MeasurementDetail.SignalName, MeasurementDetail.SignalTypeSuffix, MeasurementDetail.Longitude, MeasurementDetail.Latitude, MeasurementDetail.ID
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
SELECT 'Measurement' AS Name FROM dual
UNION
SELECT 'ActiveMeasurement' AS Name FROM dual
UNION
SELECT 'Device' AS Name FROM dual
UNION
SELECT 'OutputStream' AS Name FROM dual
UNION
SELECT 'OutputStreamDevice' AS Name FROM dual
UNION
SELECT 'OutputStreamMeasurement' AS Name FROM dual;

CREATE PACKAGE guid_state AS
    last_new_guid VARCHAR(36);
END;
/

CREATE FUNCTION NEW_GUID RETURN NCHAR AS
    guid VARCHAR2(36);
BEGIN
    SELECT SYS_GUID() INTO guid FROM dual;
    
    guid :=
        SUBSTR(guid,  1, 8) || '-' ||
        SUBSTR(guid,  9, 4) || '-' ||
        SUBSTR(guid, 13, 4) || '-' ||
        SUBSTR(guid, 17, 4) || '-' ||
        SUBSTR(guid, 21);
        
    guid_state.last_new_guid := LOWER(guid);
        
    RETURN guid_state.last_new_guid;
END;
/

CREATE TRIGGER Node_AllMeasurementsGroup AFTER INSERT ON Node FOR EACH ROW
BEGIN
    INSERT INTO MeasurementGroup (NodeID, Name, Description, FilterExpression)
    VALUES(COALESCE(:NEW.ID, guid_state.last_new_guid), 'AllMeasurements', 'All measurements defined in ActiveMeasurements', 'FILTER ActiveMeasurements WHERE SignalID IS NOT NULL');
END;
/

CREATE TRIGGER Node_AllMeasurementsGroup2 BEFORE UPDATE ON Node FOR EACH ROW
BEGIN
    DELETE FROM MeasurementGroup
    WHERE :OLD.ID <> :NEW.ID
      AND NodeID = :OLD.ID
      AND Name = 'AllMeasurements';
END;
/

CREATE TRIGGER Node_AllMeasurementsGroup3 AFTER UPDATE ON Node FOR EACH ROW
BEGIN
    INSERT INTO MeasurementGroup (NodeID, Name, Description, FilterExpression)
    SELECT :NEW.ID, 'AllMeasurements', 'All measurements defined in ActiveMeasurements', 'FILTER ActiveMeasurements WHERE SignalID IS NOT NULL'
    FROM dual
    WHERE :OLD.ID <> :NEW.ID;
END;
/

CREATE TRIGGER CustActAdaptr_RuntimSync_Insrt AFTER INSERT ON CustomActionAdapter
    FOR EACH ROW BEGIN INSERT INTO Runtime (SourceID, SourceTable) VALUES(:NEW.ID, 'CustomActionAdapter');
END;
/

CREATE TRIGGER CustActAdaptr_RuntimeSync_Del BEFORE DELETE ON CustomActionAdapter
    FOR EACH ROW BEGIN DELETE FROM Runtime WHERE SourceID = :OLD.ID AND SourceTable = 'CustomActionAdapter';
END;
/

CREATE TRIGGER CustInAdaptr_RuntimeSync_Insrt AFTER INSERT ON CustomInputAdapter
    FOR EACH ROW BEGIN INSERT INTO Runtime (SourceID, SourceTable) VALUES(:NEW.ID, 'CustomInputAdapter');
END;
/

CREATE TRIGGER CustInAdaptr_RuntimeSync_Del BEFORE DELETE ON CustomInputAdapter
    FOR EACH ROW BEGIN DELETE FROM Runtime WHERE SourceID = :OLD.ID AND SourceTable = 'CustomInputAdapter';
END;
/

CREATE TRIGGER CustOutAdaptr_RuntimSync_Insrt AFTER INSERT ON CustomOutputAdapter
    FOR EACH ROW BEGIN INSERT INTO Runtime (SourceID, SourceTable) VALUES(:NEW.ID, 'CustomOutputAdapter');
END;
/

CREATE TRIGGER CustOutAdaptr_RuntimeSync_Del BEFORE DELETE ON CustomOutputAdapter
    FOR EACH ROW BEGIN DELETE FROM Runtime WHERE SourceID = :OLD.ID AND SourceTable = 'CustomOutputAdapter';
END;
/

CREATE TRIGGER CustFtrAdaptr_RuntimeSync_Insrt AFTER INSERT ON CustomFilterAdapter
    FOR EACH ROW BEGIN INSERT INTO Runtime (SourceID, SourceTable) VALUES(:NEW.ID, 'CustomFilterAdapter');
END;
/

CREATE TRIGGER CustFtrAdaptr_RuntimeSync_Del BEFORE DELETE ON CustomFilterAdapter
    FOR EACH ROW BEGIN DELETE FROM Runtime WHERE SourceID = :OLD.ID AND SourceTable = 'CustomFilterAdapter';
END;
/

CREATE TRIGGER Device_RuntimeSync_Insert AFTER INSERT ON Device
    FOR EACH ROW BEGIN INSERT INTO Runtime (SourceID, SourceTable) VALUES(:NEW.ID, 'Device');
END;
/

CREATE TRIGGER Device_RuntimeSync_Delete BEFORE DELETE ON Device
    FOR EACH ROW BEGIN DELETE FROM Runtime WHERE SourceID = :OLD.ID AND SourceTable = 'Device';
END;
/

CREATE TRIGGER CalcMeasure_RuntimeSync_Insrt AFTER INSERT ON CalculatedMeasurement
    FOR EACH ROW BEGIN INSERT INTO Runtime (SourceID, SourceTable) VALUES(:NEW.ID, 'CalculatedMeasurement');
END;
/

CREATE TRIGGER CalcMeasure_RuntimeSync_Del BEFORE DELETE ON CalculatedMeasurement
    FOR EACH ROW BEGIN DELETE FROM Runtime WHERE SourceID = :OLD.ID AND SourceTable = 'CalculatedMeasurement';
END;
/

CREATE TRIGGER OutputStream_RuntimeSync_Insrt AFTER INSERT ON OutputStream
    FOR EACH ROW BEGIN INSERT INTO Runtime (SourceID, SourceTable) VALUES(:NEW.ID, 'OutputStream');
END;
/

CREATE TRIGGER OutputStream_RuntimeSync_Del BEFORE DELETE ON OutputStream
    FOR EACH ROW BEGIN DELETE FROM Runtime WHERE SourceID = :OLD.ID AND SourceTable = 'OutputStream';
END;
/

CREATE TRIGGER Historian_RuntimeSync_Insert AFTER INSERT ON Historian
    FOR EACH ROW BEGIN INSERT INTO Runtime (SourceID, SourceTable) VALUES(:NEW.ID, 'Historian');
END;
/

CREATE TRIGGER Historian_RuntimeSync_Delete BEFORE DELETE ON Historian
    FOR EACH ROW BEGIN DELETE FROM Runtime WHERE SourceID = :OLD.ID AND SourceTable = 'Historian';
END;
/

CREATE TRIGGER AccessLog_InsertDefault BEFORE INSERT ON AccessLog FOR EACH ROW BEGIN
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER ApplicationRole_InsertDefault BEFORE INSERT ON ApplicationRole FOR EACH ROW BEGIN
    IF :NEW.ID IS NULL THEN
        SELECT NEW_GUID() INTO :NEW.ID FROM dual;
    END IF;
    
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER SecurityGroup_InsertDefault BEFORE INSERT ON SecurityGroup FOR EACH ROW BEGIN
    IF :NEW.ID IS NULL THEN
        SELECT NEW_GUID() INTO :NEW.ID FROM dual;
    END IF;
    
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER UserAccount_InsertDefault BEFORE INSERT ON UserAccount FOR EACH ROW BEGIN
    IF :NEW.ID IS NULL THEN
        SELECT NEW_GUID() INTO :NEW.ID FROM dual;
    END IF;
    
    IF :NEW.ChangePasswordOn IS NULL THEN
        SELECT SYSDATE + 90 INTO :NEW.ChangePasswordOn FROM dual;
    END IF;
    
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER CalcMeasurement_InsertDefault BEFORE INSERT ON CalculatedMeasurement FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER Company_InsertDefault BEFORE INSERT ON Company FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER CustomActAdapter_InsertDefault BEFORE INSERT ON CustomActionAdapter FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER CustInputAdapter_InsertDefault BEFORE INSERT ON CustomInputAdapter FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER CustomOutAdapter_InsertDefault BEFORE INSERT ON CustomOutputAdapter FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER CustFtrAdapter_InsertDefault BEFORE INSERT ON CustomFilterAdapter FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER Device_InsertDefault BEFORE INSERT ON Device FOR EACH ROW BEGIN
    IF :NEW.UniqueID IS NULL THEN
        SELECT NEW_GUID() INTO :NEW.UniqueID FROM dual;
    END IF;

    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER Historian_InsertDefault BEFORE INSERT ON Historian FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER Subscriber_InsertDefault BEFORE INSERT ON Subscriber FOR EACH ROW BEGIN
    IF :NEW.ID IS NULL THEN
        SELECT NEW_GUID() INTO :NEW.ID FROM dual;
    END IF;
    
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER SubMeasurement_InsertDefault BEFORE INSERT ON SubscriberMeasurement FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER SubMeasGroup_InsertDefault BEFORE INSERT ON SubscriberMeasurementGroup FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER MeasurementGroup_InsertDefault BEFORE INSERT ON MeasurementGroup FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER MeasGroupMeas_InsertDefault BEFORE INSERT ON MeasurementGroupMeasurement FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER Measurement_InsertDefault BEFORE INSERT ON Measurement FOR EACH ROW BEGIN
    IF :NEW.SignalID IS NULL THEN
        SELECT NEW_GUID() INTO :NEW.SignalID FROM dual;
    END IF;
    
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER Node_InsertDefault BEFORE INSERT ON Node FOR EACH ROW BEGIN
    IF :NEW.ID IS NULL THEN
        SELECT NEW_GUID() INTO :NEW.ID FROM dual;
    END IF;
    
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER OtherDevice_InsertDefault BEFORE INSERT ON OtherDevice FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER OutputStream_InsertDefault BEFORE INSERT ON OutputStream FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER OutStreamDevice_InsertDefault BEFORE INSERT ON OutputStreamDevice FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER OutStrDevAnalog_InsrtDefault BEFORE INSERT ON OutputStreamDeviceAnalog FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER OutStrDevDigital_InsrtDefault BEFORE INSERT ON OutputStreamDeviceDigital FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER OutStrDevPhasor_InsrtDefault BEFORE INSERT ON OutputStreamDevicePhasor FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER OutStrMeasurement_InsrtDefault BEFORE INSERT ON OutputStreamMeasurement FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER Phasor_InsertDefault BEFORE INSERT ON Phasor FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER Alarm_InsertDefault BEFORE INSERT ON Alarm FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER Vendor_InsertDefault BEFORE INSERT ON Vendor FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER VendorDevice_InsertDefault BEFORE INSERT ON VendorDevice FOR EACH ROW BEGIN
    IF :NEW.CreatedBy IS NULL THEN
        SELECT USER INTO :NEW.CreatedBy FROM dual;
    END IF;
    
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
    
    IF :NEW.UpdatedBy IS NULL THEN
        SELECT USER INTO :NEW.UpdatedBy FROM dual;
    END IF;
    
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER ErrorLog_InsertDefault BEFORE INSERT ON ErrorLog FOR EACH ROW BEGIN
    IF :NEW.CreatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.CreatedOn FROM dual;
    END IF;
END;
/

CREATE TRIGGER AuditLog_InsertDefault BEFORE INSERT ON AuditLog FOR EACH ROW BEGIN
    IF :NEW.UpdatedOn IS NULL THEN
        SELECT SYSDATE INTO :NEW.UpdatedOn FROM dual;
    END IF;
END;
/

-- ***********************
-- Company Change Tracking
-- ***********************

CREATE TRIGGER Company_UpdateTracker AFTER UPDATE ON Company FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', Measurement.SignalID
    FROM Device RIGHT OUTER JOIN
         Measurement ON Device.ID = Measurement.DeviceID
    WHERE :OLD.Acronym <> :NEW.Acronym
      AND Device.CompanyID = :NEW.ID;
END;
/

-- **********************
-- Device Change Tracking
-- **********************

CREATE TRIGGER Device_InsertTracker AFTER INSERT ON Device FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Device', 'ID', :NEW.ID);
END;
/

CREATE TRIGGER Device_UpdateTracker1 AFTER UPDATE ON Device FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Device', 'ID', :NEW.ID);
    
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID
    FROM Measurement
    WHERE (:OLD.NodeID <> :NEW.NodeID
        OR :OLD.Acronym <> :NEW.Acronym
        OR :OLD.IsConcentrator <> :NEW.IsConcentrator
        OR :OLD.ParentID <> :NEW.ParentID
        OR :OLD.FramesPerSecond <> :NEW.FramesPerSecond
        OR :OLD.Longitude <> :NEW.Longitude
        OR :OLD.Latitude <> :NEW.Latitude
        OR :OLD.CompanyID <> :NEW.CompanyID
        OR :OLD.ProtocolID <> :NEW.ProtocolID
        OR :OLD.Enabled <> :NEW.Enabled)
       AND DeviceID = :NEW.ID;
END;
/

CREATE TRIGGER Device_DeleteTracker AFTER DELETE ON Device FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Device', 'ID', :OLD.ID);
END;
/

-- *************************
-- Historian Change Tracking
-- *************************

CREATE TRIGGER Historian_UpdateTracker AFTER UPDATE ON Historian FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID
    FROM Measurement
    WHERE (:OLD.NodeID <> :NEW.NodeID
        OR :OLD.Acronym <> :NEW.Acronym)
       AND HistorianID = :NEW.ID;
END;
/

-- ***************************
-- Measurement Change Tracking
-- ***************************

CREATE TRIGGER Measurement_InsertTracker AFTER INSERT ON Measurement FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Measurement', 'PointID', :NEW.PointID);
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('ActiveMeasurement', 'SignalID', COALESCE(:NEW.SignalID, guid_state.last_new_guid));
END;
/

CREATE TRIGGER Measurement_UpdateTracker AFTER UPDATE ON Measurement FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Measurement', 'PointID', :NEW.PointID);
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) SELECT 'ActiveMeasurement', 'SignalID', :OLD.SignalID FROM dual WHERE :NEW.SignalID <> :OLD.SignalID;
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('ActiveMeasurement', 'SignalID', :NEW.SignalID);
END;
/

CREATE TRIGGER Measurement_DeleteTracker AFTER DELETE ON Measurement FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('Measurement', 'PointID', :OLD.PointID);
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('ActiveMeasurement', 'SignalID', :OLD.SignalID);
END;
/

-- ****************************
-- OutputStream Change Tracking
-- ****************************

CREATE TRIGGER OutputStream_InsertTracker AFTER INSERT ON OutputStream FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStream', 'ID', :NEW.ID);
END;
/

CREATE TRIGGER OutputStream_UpdateTracker AFTER UPDATE ON OutputStream FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStream', 'ID', :NEW.ID);
END;
/

CREATE TRIGGER OutputStream_DeleteTracker AFTER DELETE ON OutputStream FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStream', 'ID', :OLD.ID);
END;
/

-- **********************************
-- OutputStreamDevice Change Tracking
-- **********************************

CREATE TRIGGER OutStreamDevice_InsertTracker AFTER INSERT ON OutputStreamDevice FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamDevice', 'ID', :NEW.ID);
END;
/

CREATE TRIGGER OutStreamDevice_UpdateTracker AFTER UPDATE ON OutputStreamDevice FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamDevice', 'ID', :NEW.ID);
END;
/

CREATE TRIGGER OutStreamDevice_DeleteTracker AFTER DELETE ON OutputStreamDevice FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamDevice', 'ID', :OLD.ID);
END;
/

-- ***************************************
-- OutputStreamMeasurement Change Tracking
-- ***************************************

CREATE TRIGGER OutStrMeas_InsertTracker AFTER INSERT ON OutputStreamMeasurement FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamMeasurement', 'ID', :NEW.ID);
END;
/

CREATE TRIGGER OutStrMeas_UpdateTracker AFTER UPDATE ON OutputStreamMeasurement FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamMeasurement', 'ID', :NEW.ID);
END;
/

CREATE TRIGGER OutStrMeas_DeleteTracker AFTER DELETE ON OutputStreamMeasurement FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue) VALUES('OutputStreamMeasurement', 'ID', :OLD.ID);
END;
/

-- **********************
-- Phasor Change Tracking
-- **********************

CREATE TRIGGER Phasor_UpdateTracker AFTER UPDATE ON Phasor FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID
    FROM Measurement
    WHERE (:OLD.DeviceID <> :NEW.DeviceID
        OR :OLD.SourceIndex <> :NEW.SourceIndex)
       AND DeviceID = :OLD.DeviceID
       AND PhasorSourceIndex = :OLD.SourceIndex;
    
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID
    FROM Measurement
    WHERE (:OLD.Type <> :NEW.Type
        OR :OLD.Phase <> :NEW.Phase
        OR :OLD.DeviceID <> :NEW.DeviceID
        OR :OLD.SourceIndex <> :NEW.SourceIndex)
       AND DeviceID = :NEW.DeviceID
       AND PhasorSourceIndex = :NEW.SourceIndex;
END;
/

-- ************************
-- Protocol Change Tracking
-- ************************

CREATE TRIGGER Protocol_UpdateTracker AFTER UPDATE ON Protocol FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', Measurement.SignalID
    FROM Device RIGHT OUTER JOIN
         Measurement ON Device.ID = Measurement.DeviceID
    WHERE (:OLD.Acronym <> :NEW.Acronym
        OR :OLD.Type <> :NEW.Type)
       AND Device.ProtocolID = :NEW.ID;
END;
/

-- **************************
-- SignalType Change Tracking
-- **************************

CREATE TRIGGER SignalType_UpdateTracker AFTER UPDATE ON SignalType FOR EACH ROW
BEGIN
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID
    FROM Measurement
    WHERE :OLD.Acronym <> :NEW.Acronym
      AND SignalTypeID = :NEW.ID;
END;
/

CREATE PACKAGE context AS
    PROCEDURE set_current_user(
        new_current_user IN VARCHAR2);
    FUNCTION get_current_user RETURN VARCHAR2;
END;
/

CREATE PACKAGE BODY context AS
    current_user VARCHAR2(200);
    
    PROCEDURE set_current_user(new_current_user IN VARCHAR2) IS
    BEGIN
        current_user := new_current_user;
    END;
    
    FUNCTION get_current_user RETURN VARCHAR2 IS
    BEGIN
        RETURN current_user;
    END;
END;
/