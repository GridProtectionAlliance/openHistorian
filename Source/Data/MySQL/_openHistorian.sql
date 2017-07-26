-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 1 AS VersionNumber;

CREATE TABLE CompressionSetting(
    PointID INT NOT NULL,
    CompressionMinTime BIGINT NOT NULL DEFAULT 0,
    CompressionMaxTime BIGINT NOT NULL DEFAULT 0,
    CompressionLimit DOUBLE NOT NULL DEFAULT 0.0,
 CONSTRAINT PK_CompressionSetting PRIMARY KEY (PointID ASC) 
);