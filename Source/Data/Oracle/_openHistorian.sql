-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 2 AS VersionNumber
FROM dual;

CREATE TABLE CompressionSetting(
    PointID NUMBER NOT NULL,
    CompressionMinTime NUMBER(19, 0) DEFAULT 0 NOT NULL,
    CompressionMaxTime NUMBER(19, 0) DEFAULT 0 NOT NULL,
    CompressionLimit NUMBER(9, 6) DEFAULT 0.0 NOT NULL
);

CREATE UNIQUE INDEX IX_CompressionSetting_PointID ON CompressionSetting (PointID ASC) TABLESPACE openHistorian_INDEX;

CREATE VIEW NodeCompressionSetting AS
SELECT
    Node.ID AS NodeID,
    CompressionSetting.PointID,
    CompressionSetting.CompressionMinTime,
    CompressionSetting.CompressionMaxTime,
    CompressionSetting.CompressionLimit
FROM CompressionSetting CROSS JOIN Node;

CREATE TABLE EventMarker(
    ID NUMBER NOT NULL,
    ParentID NUMBER NULL,
    Source VARCHAR2(200) NULL,
    StartTime DATE NULL,
    StopTime DATE NULL,
    Notes VARCHAR2(4000) NULL
);

CREATE UNIQUE INDEX IX_EventMarker_ID ON EventMarker (ID ASC) TABLESPACE openHistorian_INDEX;

ALTER TABLE EventMarker ADD CONSTRAINT PK_EventMarker PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_EventMarker START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_EventMarker BEFORE INSERT ON EventMarker
    FOR EACH ROW BEGIN SELECT SEQ_EventMarker.nextval INTO :NEW.ID FROM dual;
END;

ALTER TABLE EventMarker ADD CONSTRAINT FK_EventMarker_EventMarker FOREIGN KEY(ParentID) REFERENCES EventMarker (ID);
