-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 2 AS VersionNumber;

CREATE TABLE CompressionSetting(
    PointID INTEGER PRIMARY KEY NOT NULL,
    CompressionMinTime INTEGER NOT NULL DEFAULT 0,
    CompressionMaxTime INTEGER NOT NULL DEFAULT 0,
    CompressionLimit REAL NOT NULL DEFAULT 0.0
 );

CREATE VIEW NodeCompressionSetting AS
SELECT
    Node.ID AS NodeID,
    CompressionSetting.PointID,
    CompressionSetting.CompressionMinTime,
    CompressionSetting.CompressionMaxTime,
    CompressionSetting.CompressionLimit
FROM CompressionSetting CROSS JOIN Node;

CREATE TABLE EventMarker(
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    ParentID INTEGER NULL,
    Source VARCHAR(200) NULL,
    StartTime DATETIME NULL,
    StopTime DATETIME NULL,
    Notes TEXT NULL,
    CONSTRAINT FK_EventMarker_EventMarker FOREIGN KEY(ParentID) REFERENCES EventMarker (ID)
);