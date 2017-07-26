-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 1 AS VersionNumber;

CREATE TABLE CompressionSetting(
    PointID INTEGER PRIMARY KEY NOT NULL,
    CompressionMinTime INTEGER NOT NULL DEFAULT 0,
    CompressionMaxTime INTEGER NOT NULL DEFAULT 0,
    CompressionLimit REAL NOT NULL DEFAULT 0.0
 );

CREATE VIEW NodeCompressionSetting AS
SELECT NodeID, PointID, CompressionMinTime, CompressionMaxTime, CompressionLimit
FROM CompressionSetting CROSS JOIN Node;