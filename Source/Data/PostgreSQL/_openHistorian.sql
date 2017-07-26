-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 1 AS VersionNumber;

CREATE TABLE CompressionSetting(
    PointID INTEGER NOT NULL PRIMARY KEY,
    CompressionMinTime BIGINT NOT NULL DEFAULT 0,
    CompressionMaxTime BIGINT NOT NULL DEFAULT 0,
    CompressionLimit DOUBLE PRECISION NOT NULL DEFAULT 0.0
);

CREATE VIEW NodeCompressionSetting AS
SELECT NodeID, PointID, CompressionMinTime, CompressionMaxTime, CompressionLimit
FROM CompressionSetting CROSS JOIN Node;