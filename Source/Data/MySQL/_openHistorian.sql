-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 2 AS VersionNumber;

CREATE TABLE CompressionSetting(
    PointID INT NOT NULL,
    CompressionMinTime BIGINT NOT NULL DEFAULT 0,
    CompressionMaxTime BIGINT NOT NULL DEFAULT 0,
    CompressionLimit DOUBLE NOT NULL DEFAULT 0.0,
    CONSTRAINT PK_CompressionSetting PRIMARY KEY (PointID ASC) 
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
    ID INT AUTO_INCREMENT NOT NULL,
    ParentID INT NULL,
    Source VARCHAR(200) NULL,
    StartTime DATETIME NULL,
    StopTime DATETIME NULL,
    Notes TEXT NULL,
    CONSTRAINT PK_EventMarker PRIMARY KEY (ID ASC)
);

ALTER TABLE EventMarker ADD CONSTRAINT FK_EventMarker_EventMarker FOREIGN KEY(ParentID) REFERENCES EventMarker (ID);
