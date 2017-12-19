-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 1 AS VersionNumber
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
