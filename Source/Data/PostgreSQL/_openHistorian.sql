-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 2 AS VersionNumber;

CREATE TABLE CompressionSetting(
    PointID INTEGER NOT NULL PRIMARY KEY,
    CompressionMinTime BIGINT NOT NULL DEFAULT 0,
    CompressionMaxTime BIGINT NOT NULL DEFAULT 0,
    CompressionLimit DOUBLE PRECISION NOT NULL DEFAULT 0.0
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
    ID SERIAL NOT NULL PRIMARY KEY,
    ParentID INTEGER NULL,
    Source VARCHAR(200) NULL,
    StartTime TIMESTAMP NULL,
    StopTime TIMESTAMP NULL,
    Notes TEXT NULL,
    CONSTRAINT FK_EventMarker_EventMarker FOREIGN KEY(ParentID) REFERENCES EventMarker (ID)
);
