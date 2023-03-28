USE GSFSchema;

DELIMITER $$

CREATE TRIGGER UserAccount_AuditUpdate AFTER UPDATE ON UserAccount FOR EACH ROW 
    BEGIN
        
        IF OLD.Name != NEW.Name THEN 
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.Password != NEW.Password THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Password', OriginalValue = OLD.Password, NewValue = NEW.Password, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.FirstName != NEW.FirstName THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'FirstName', OriginalValue = OLD.FirstName, NewValue = NEW.FirstName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LastName != NEW.LastName THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LastName', OriginalValue = OLD.LastName, NewValue = NEW.LastName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.DefaultNodeID != NEW.DefaultNodeID THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DefaultNodeID', OriginalValue = OLD.DefaultNodeID, NewValue = NEW.DefaultNodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Phone != NEW.Phone THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Phone', OriginalValue = OLD.Phone, NewValue = NEW.Phone, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Email != NEW.Email THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Email', OriginalValue = OLD.Email, NewValue = NEW.Email, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LockedOut != NEW.LockedOut THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LockedOut', OriginalValue = OLD.LockedOut, NewValue = NEW.LockedOut, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UseADAuthentication != NEW.UseADAuthentication THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UseADAuthentication', OriginalValue = OLD.UseADAuthentication, NewValue = NEW.UseADAuthentication, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ChangePasswordOn != NEW.ChangePasswordOn THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ChangePasswordOn', OriginalValue = OLD.ChangePasswordOn, NewValue = NEW.ChangePasswordOn, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;			
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
    END$$

CREATE TRIGGER UserAccount_AuditDelete AFTER DELETE ON UserAccount
    FOR EACH ROW BEGIN
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Password', OriginalValue = OLD.Password, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'FirstName', OriginalValue = OLD.FirstName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LastName', OriginalValue = OLD.LastName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DefaultNodeID', OriginalValue = OLD.DefaultNodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Phone', OriginalValue = OLD.Phone, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Email', OriginalValue = OLD.Email, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LockedOut', OriginalValue = OLD.LockedOut, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UseADAuthentication', OriginalValue = OLD.UseADAuthentication, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ChangePasswordOn', OriginalValue = OLD.ChangePasswordOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'UserAccount', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER SecurityGroup_AuditUpdate AFTER UPDATE ON SecurityGroup
    FOR EACH ROW BEGIN
    
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Description != NEW.Description THEN
            INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NEW.Description, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
    END$$

CREATE TRIGGER SecurityGroup_AuditDelete AFTER DELETE ON SecurityGroup
    FOR EACH ROW BEGIN		
        INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'SecurityGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    END$$

CREATE TRIGGER ApplicationRole_AuditUpdate AFTER UPDATE ON ApplicationRole
    FOR EACH ROW BEGIN

        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Description != NEW.Description THEN
            INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NEW.Description, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER ApplicationRole_AuditDelete AFTER DELETE ON ApplicationRole
    FOR EACH ROW BEGIN	
        INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'ApplicationRole', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER CalculatedMeasurement_AuditUpdate AFTER UPDATE ON CalculatedMeasurement
    FOR EACH ROW BEGIN
    
        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Acronym != NEW.Acronym THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NEW.Acronym, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AssemblyName != NEW.AssemblyName THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AssemblyName', OriginalValue = OLD.AssemblyName, NewValue = NEW.AssemblyName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.TypeName != NEW.TypeName THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TypeName', OriginalValue = OLD.TypeName, NewValue = NEW.TypeName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ConnectionString != NEW.ConnectionString THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NEW.ConnectionString, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ConfigSection != NEW.ConfigSection THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConfigSection', OriginalValue = OLD.ConfigSection, NewValue = NEW.ConfigSection, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.InputMeasurements != NEW.InputMeasurements THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'InputMeasurements', OriginalValue = OLD.InputMeasurements, NewValue = NEW.InputMeasurements, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.OutputMeasurements != NEW.OutputMeasurements THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'OutputMeasurements', OriginalValue = OLD.OutputMeasurements, NewValue = NEW.OutputMeasurements, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.MinimumMeasurementsToUse != NEW.MinimumMeasurementsToUse THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MinimumMeasurementsToUse', OriginalValue = OLD.MinimumMeasurementsToUse, NewValue = NEW.MinimumMeasurementsToUse, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.FramesPerSecond != NEW.FramesPerSecond THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'FramesPerSecond', OriginalValue = OLD.FramesPerSecond, NewValue = NEW.FramesPerSecond, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LagTime != NEW.LagTime THEN	
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LagTime', OriginalValue = OLD.LagTime, NewValue = NEW.LagTime, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LeadTime != NEW.LeadTime THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LeadTime', OriginalValue = OLD.LeadTime, NewValue = NEW.LeadTime, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UseLocalClockAsRealTime != NEW.UseLocalClockAsRealTime THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UseLocalClockAsRealTime', OriginalValue = OLD.UseLocalClockAsRealTime, NewValue = NEW.UseLocalClockAsRealTime, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AllowSortsByArrival != NEW.AllowSortsByArrival THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowSortsByArrival', OriginalValue = OLD.AllowSortsByArrival, NewValue = NEW.AllowSortsByArrival, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.IgnoreBadTimestamps != NEW.IgnoreBadTimestamps THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IgnoreBadTimestamps', OriginalValue = OLD.IgnoreBadTimestamps, NewValue = NEW.IgnoreBadTimestamps, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.TimeResolution != NEW.TimeResolution THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TimeResolution', OriginalValue = OLD.TimeResolution, NewValue = NEW.TimeResolution, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.AllowPreemptivePublishing != NEW.AllowPreemptivePublishing THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowPreemptivePublishing', OriginalValue = OLD.AllowPreemptivePublishing, NewValue = NEW.AllowPreemptivePublishing, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.PerformTimeReasonabilityCheck != NEW.PerformTimeReasonabilityCheck THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'PerformTimeReasonabilityCheck', OriginalValue = OLD.PerformTimeReasonabilityCheck, NewValue = NEW.PerformTimeReasonabilityCheck, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.DownsamplingMethod != NEW.DownsamplingMethod THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DownsamplingMethod', OriginalValue = OLD.DownsamplingMethod, NewValue = NEW.DownsamplingMethod, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER CalculatedMeasurement_AuditDelete AFTER DELETE ON CalculatedMeasurement
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AssemblyName', OriginalValue = OLD.AssemblyName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TypeName', OriginalValue = OLD.TypeName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConfigSection', OriginalValue = OLD.ConfigSection, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'InputMeasurements', OriginalValue = OLD.InputMeasurements, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'OutputMeasurements', OriginalValue = OLD.OutputMeasurements, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MinimumMeasurementsToUse', OriginalValue = OLD.MinimumMeasurementsToUse, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'FramesPerSecond', OriginalValue = OLD.FramesPerSecond, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LagTime', OriginalValue = OLD.LagTime, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LeadTime', OriginalValue = OLD.LeadTime, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UseLocalClockAsRealTime', OriginalValue = OLD.UseLocalClockAsRealTime, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowSortsByArrival', OriginalValue = OLD.AllowSortsByArrival, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IgnoreBadTimestamps', OriginalValue = OLD.IgnoreBadTimestamps, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TimeResolution', OriginalValue = OLD.TimeResolution, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowPreemptivePublishing', OriginalValue = OLD.AllowPreemptivePublishing, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'PerformTimeReasonabilityCheck', OriginalValue = OLD.PerformTimeReasonabilityCheck, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DownsamplingMethod', OriginalValue = OLD.DownsamplingMethod, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CalculatedMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;

    END$$

CREATE TRIGGER Company_AuditUpdate AFTER UPDATE ON Company
    FOR EACH ROW BEGIN

        IF OLD.Acronym != NEW.Acronym THEN
            INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NEW.Acronym, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.MapAcronym != NEW.MapAcronym THEN
            INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MapAcronym', OriginalValue = OLD.MapAcronym, NewValue = NEW.MapAcronym, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Url != NEW.Url THEN
            INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Url', OriginalValue = OLD.Url, NewValue = NEW.Url, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
    END$$

CREATE TRIGGER Company_AuditDelete AFTER DELETE ON Company
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MapAcronym', OriginalValue = OLD.MapAcronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Url', OriginalValue = OLD.Url, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Company', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER CustomActionAdapter_AuditUpdate AFTER UPDATE ON CustomActionAdapter
    FOR EACH ROW BEGIN

        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.AdapterName != NEW.AdapterName THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AdapterName', OriginalValue = OLD.AdapterName, NewValue = NEW.AdapterName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AssemblyName != NEW.AssemblyName THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AssemblyName', OriginalValue = OLD.AssemblyName, NewValue = NEW.AssemblyName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.TypeName != NEW.TypeName THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TypeName', OriginalValue = OLD.TypeName, NewValue = NEW.TypeName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ConnectionString != NEW.ConnectionString THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NEW.ConnectionString, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER CustomActionAdapter_AuditDelete AFTER DELETE ON CustomActionAdapter
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AdapterName', OriginalValue = OLD.AdapterName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AssemblyName', OriginalValue = OLD.AssemblyName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TypeName', OriginalValue = OLD.TypeName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomActionAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER CustomInputAdapter_AuditUpdate AFTER UPDATE ON CustomInputAdapter
    FOR EACH ROW BEGIN

        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.AdapterName != NEW.AdapterName THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AdapterName', OriginalValue = OLD.AdapterName, NewValue = NEW.AdapterName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AssemblyName != NEW.AssemblyName THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AssemblyName', OriginalValue = OLD.AssemblyName, NewValue = NEW.AssemblyName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.TypeName != NEW.TypeName THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TypeName', OriginalValue = OLD.TypeName, NewValue = NEW.TypeName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ConnectionString != NEW.ConnectionString THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NEW.ConnectionString, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER CustomInputAdapter_AuditDelete AFTER DELETE ON CustomInputAdapter
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AdapterName', OriginalValue = OLD.AdapterName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AssemblyName', OriginalValue = OLD.AssemblyName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TypeName', OriginalValue = OLD.TypeName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomInputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER CustomOutputAdapter_AuditUpdate AFTER UPDATE ON CustomOutputAdapter
    FOR EACH ROW BEGIN

        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.AdapterName != NEW.AdapterName THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AdapterName', OriginalValue = OLD.AdapterName, NewValue = NEW.AdapterName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AssemblyName != NEW.AssemblyName THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AssemblyName', OriginalValue = OLD.AssemblyName, NewValue = NEW.AssemblyName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.TypeName != NEW.TypeName THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TypeName', OriginalValue = OLD.TypeName, NewValue = NEW.TypeName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ConnectionString != NEW.ConnectionString THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NEW.ConnectionString, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER CustomOutputAdapter_AuditDelete AFTER DELETE ON CustomOutputAdapter
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AdapterName', OriginalValue = OLD.AdapterName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AssemblyName', OriginalValue = OLD.AssemblyName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TypeName', OriginalValue = OLD.TypeName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'CustomOutputAdapter', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER Device_AuditUpdate AFTER UPDATE ON Device
    FOR EACH ROW BEGIN

        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ParentID != NEW.ParentID THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ParentID', OriginalValue = OLD.ParentID, NewValue = NEW.ParentID, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.UniqueID != NEW.UniqueID THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UniqueID', OriginalValue = OLD.UniqueID, NewValue = NEW.UniqueID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Acronym != NEW.Acronym THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NEW.Acronym, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.OriginalSource != NEW.OriginalSource THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'OriginalSource', OriginalValue = OLD.OriginalSource, NewValue = NEW.OriginalSource, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.IsConcentrator != NEW.IsConcentrator THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IsConcentrator', OriginalValue = OLD.IsConcentrator, NewValue = NEW.IsConcentrator, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CompanyID != NEW.CompanyID THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CompanyID', OriginalValue = OLD.CompanyID, NewValue = NEW.CompanyID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.HistorianID != NEW.HistorianID THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'HistorianID', OriginalValue = OLD.HistorianID, NewValue = NEW.HistorianID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AccessID != NEW.AccessID THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AccessID', OriginalValue = OLD.AccessID, NewValue = NEW.AccessID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.VendorDeviceID != NEW.VendorDeviceID THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'VendorDeviceID', OriginalValue = OLD.VendorDeviceID, NewValue = NEW.VendorDeviceID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ProtocolID != NEW.ProtocolID THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ProtocolID', OriginalValue = OLD.ProtocolID, NewValue = NEW.ProtocolID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Longitude != NEW.Longitude THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Longitude', OriginalValue = OLD.Longitude, NewValue = NEW.Longitude, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Latitude != NEW.Latitude THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Latitude', OriginalValue = OLD.Latitude, NewValue = NEW.Latitude, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.InterconnectionID != NEW.InterconnectionID THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'InterconnectionID', OriginalValue = OLD.InterconnectionID, NewValue = NEW.InterconnectionID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ConnectionString != NEW.ConnectionString THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NEW.ConnectionString, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ConnectOnDemand != NEW.ConnectOnDemand THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectOnDemand', OriginalValue = OLD.ConnectOnDemand, NewValue = NEW.ConnectOnDemand, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.TimeZone != NEW.TimeZone THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TimeZone', OriginalValue = OLD.TimeZone, NewValue = NEW.TimeZone, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.FramesPerSecond != NEW.FramesPerSecond THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'FramesPerSecond', OriginalValue = OLD.FramesPerSecond, NewValue = NEW.FramesPerSecond, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.TimeAdjustmentTicks != NEW.TimeAdjustmentTicks THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TimeAdjustmentTicks', OriginalValue = OLD.TimeAdjustmentTicks, NewValue = NEW.TimeAdjustmentTicks, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.DataLossInterval != NEW.DataLossInterval THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DataLossInterval', OriginalValue = OLD.DataLossInterval, NewValue = NEW.DataLossInterval, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AllowedParsingExceptions != NEW.AllowedParsingExceptions THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowedParsingExceptions', OriginalValue = OLD.AllowedParsingExceptions, NewValue = NEW.AllowedParsingExceptions, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ParsingExceptionWindow != NEW.ParsingExceptionWindow THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ParsingExceptionWindow', OriginalValue = OLD.ParsingExceptionWindow, NewValue = NEW.ParsingExceptionWindow, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.DelayedConnectionInterval != NEW.DelayedConnectionInterval THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DelayedConnectionInterval', OriginalValue = OLD.DelayedConnectionInterval, NewValue = NEW.DelayedConnectionInterval, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AllowUseOfCachedConfiguration != NEW.AllowUseOfCachedConfiguration THEN	
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowUseOfCachedConfiguration', OriginalValue = OLD.AllowUseOfCachedConfiguration, NewValue = NEW.AllowUseOfCachedConfiguration, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AutoStartDataParsingSequence != NEW.AutoStartDataParsingSequence THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AutoStartDataParsingSequence', OriginalValue = OLD.AutoStartDataParsingSequence, NewValue = NEW.AutoStartDataParsingSequence, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.SkipDisableRealTimeData != NEW.SkipDisableRealTimeData THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'SkipDisableRealTimeData', OriginalValue = OLD.SkipDisableRealTimeData, NewValue = NEW.SkipDisableRealTimeData, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.MeasurementReportingInterval != NEW.MeasurementReportingInterval THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MeasurementReportingInterval', OriginalValue = OLD.MeasurementReportingInterval, NewValue = NEW.MeasurementReportingInterval, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ContactList != NEW.ContactList THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ContactList', OriginalValue = OLD.ContactList, NewValue = NEW.ContactList, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.MeasuredLines != NEW.MeasuredLines THEN	
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MeasuredLines', OriginalValue = OLD.MeasuredLines, NewValue = NEW.MeasuredLines, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
    END$$

CREATE TRIGGER Device_AuditDelete AFTER DELETE ON Device
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ParentID', OriginalValue = OLD.ParentID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UniqueID', OriginalValue = OLD.UniqueID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IsConcentrator', OriginalValue = OLD.IsConcentrator, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CompanyID', OriginalValue = OLD.CompanyID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'HistorianID', OriginalValue = OLD.HistorianID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AccessID', OriginalValue = OLD.AccessID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'VendorDeviceID', OriginalValue = OLD.VendorDeviceID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ProtocolID', OriginalValue = OLD.ProtocolID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Longitude', OriginalValue = OLD.Longitude, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Latitude', OriginalValue = OLD.Latitude, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'InterconnectionID', OriginalValue = OLD.InterconnectionID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectOnDemand', OriginalValue = OLD.ConnectOnDemand, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TimeZone', OriginalValue = OLD.TimeZone, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'FramesPerSecond', OriginalValue = OLD.FramesPerSecond, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TimeAdjustmentTicks', OriginalValue = OLD.TimeAdjustmentTicks, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DataLossInterval', OriginalValue = OLD.DataLossInterval, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowedParsingExceptions', OriginalValue = OLD.AllowedParsingExceptions, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ParsingExceptionWindow', OriginalValue = OLD.ParsingExceptionWindow, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DelayedConnectionInterval', OriginalValue = OLD.DelayedConnectionInterval, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowUseOfCachedConfiguration', OriginalValue = OLD.AllowUseOfCachedConfiguration, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AutoStartDataParsingSequence', OriginalValue = OLD.AutoStartDataParsingSequence, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'SkipDisableRealTimeData', OriginalValue = OLD.SkipDisableRealTimeData, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MeasurementReportingInterval', OriginalValue = OLD.MeasurementReportingInterval, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ContactList', OriginalValue = OLD.ContactList, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MeasuredLines', OriginalValue = OLD.MeasuredLines, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Device', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER Historian_AuditUpdate AFTER UPDATE ON Historian
    FOR EACH ROW BEGIN

        IF OLD.NodeID != NEW.NodeID THEN	
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Acronym != NEW.Acronym THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NEW.Acronym, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.AssemblyName != NEW.AssemblyName THEN	
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AssemblyName', OriginalValue = OLD.AssemblyName, NewValue = NEW.AssemblyName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.TypeName != NEW.TypeName THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TypeName', OriginalValue = OLD.TypeName, NewValue = NEW.TypeName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ConnectionString != NEW.ConnectionString THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NEW.ConnectionString, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.IsLocal != NEW.IsLocal THEN	
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IsLocal', OriginalValue = OLD.IsLocal, NewValue = NEW.IsLocal, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.MeasurementReportingInterval != NEW.MeasurementReportingInterval THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MeasurementReportingInterval', OriginalValue = OLD.MeasurementReportingInterval, NewValue = NEW.MeasurementReportingInterval, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Description != NEW.Description THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NEW.Description, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN	
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER Historian_AuditDelete AFTER DELETE ON Historian
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AssemblyName', OriginalValue = OLD.AssemblyName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TypeName', OriginalValue = OLD.TypeName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IsLocal', OriginalValue = OLD.IsLocal, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MeasurementReportingInterval', OriginalValue = OLD.MeasurementReportingInterval, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Historian', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER Measurement_AuditUpdate AFTER UPDATE ON Measurement
    FOR EACH ROW BEGIN

        IF OLD.HistorianID != NEW.HistorianID THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'HistorianID', OriginalValue = OLD.HistorianID, NewValue = NEW.HistorianID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.PointID != NEW.PointID THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'PointID', OriginalValue = OLD.PointID, NewValue = NEW.PointID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.DeviceID != NEW.DeviceID THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'DeviceID', OriginalValue = OLD.DeviceID, NewValue = NEW.DeviceID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.PointTag != NEW.PointTag THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'PointTag', OriginalValue = OLD.PointTag, NewValue = NEW.PointTag, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AlternateTag != NEW.AlternateTag THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'AlternateTag', OriginalValue = OLD.AlternateTag, NewValue = NEW.AlternateTag, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.SignalTypeID != NEW.SignalTypeID THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'SignalTypeID', OriginalValue = OLD.SignalTypeID, NewValue = NEW.SignalTypeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.PhasorSourceIndex != NEW.PhasorSourceIndex THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'PhasorSourceIndex', OriginalValue = OLD.PhasorSourceIndex, NewValue = NEW.PhasorSourceIndex, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.SignalReference != NEW.SignalReference THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'SignalReference', OriginalValue = OLD.SignalReference, NewValue = NEW.SignalReference, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Adder != NEW.Adder THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Adder', OriginalValue = OLD.Adder, NewValue = NEW.Adder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Multiplier != NEW.Multiplier THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Multiplier', OriginalValue = OLD.Multiplier, NewValue = NEW.Multiplier, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Description != NEW.Description THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NEW.Description, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Subscribed != NEW.Subscribed THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Subscribed', OriginalValue = OLD.Subscribed, NewValue = NEW.Subscribed, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.Internal != NEW.Internal THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Internal', OriginalValue = OLD.Internal, NewValue = NEW.Internal, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER Measurement_AuditDelete AFTER DELETE ON Measurement
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'HistorianID', OriginalValue = OLD.HistorianID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'PointID', OriginalValue = OLD.PointID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'DeviceID', OriginalValue = OLD.DeviceID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'PointTag', OriginalValue = OLD.PointTag, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'AlternateTag', OriginalValue = OLD.AlternateTag, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'SignalTypeID', OriginalValue = OLD.SignalTypeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'PhasorSourceIndex', OriginalValue = OLD.PhasorSourceIndex, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'SignalReference', OriginalValue = OLD.SignalReference, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Adder', OriginalValue = OLD.Adder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Multiplier', OriginalValue = OLD.Multiplier, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Subscribed', OriginalValue = OLD.Subscribed, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Internal', OriginalValue = OLD.Internal, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Measurement', PrimaryKeyColumn = 'SignalID', PrimaryKeyValue = OLD.SignalID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER Node_AuditUpdate AFTER UPDATE ON Node
    FOR EACH ROW BEGIN

        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CompanyID != NEW.CompanyID THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CompanyID', OriginalValue = OLD.CompanyID, NewValue = NEW.CompanyID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Longitude != NEW.Longitude THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Longitude', OriginalValue = OLD.Longitude, NewValue = NEW.Longitude, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Latitude != NEW.Latitude THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Latitude', OriginalValue = OLD.Latitude, NewValue = NEW.Latitude, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Description != NEW.Description THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NEW.Description, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ImagePath != NEW.ImagePath THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ImagePath', OriginalValue = OLD.ImagePath, NewValue = NEW.ImagePath, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Settings != NEW.Settings THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Settings', OriginalValue = OLD.Settings, NewValue = NEW.Settings, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.MenuType != NEW.MenuType THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MenuType', OriginalValue = OLD.MenuType, NewValue = NEW.MenuType, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.MenuData != NEW.MenuData THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MenuData', OriginalValue = OLD.MenuData, NewValue = NEW.MenuData, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Master != NEW.Master THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Master', OriginalValue = OLD.Master, NewValue = NEW.Master, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER Node_AuditDelete AFTER DELETE ON Node
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CompanyID', OriginalValue = OLD.CompanyID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Longitude', OriginalValue = OLD.Longitude, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Latitude', OriginalValue = OLD.Latitude, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ImagePath', OriginalValue = OLD.ImagePath, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Settings', OriginalValue = OLD.Settings, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MenuType', OriginalValue = OLD.MenuType, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MenuData', OriginalValue = OLD.MenuData, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Master', OriginalValue = OLD.Master, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Node', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER OtherDevice_AuditUpdate AFTER UPDATE ON OtherDevice
    FOR EACH ROW BEGIN

        IF OLD.Acronym != NEW.Acronym THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NEW.Acronym, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.IsConcentrator != NEW.IsConcentrator THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IsConcentrator', OriginalValue = OLD.IsConcentrator, NewValue = NEW.IsConcentrator, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CompanyID != NEW.CompanyID THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CompanyID', OriginalValue = OLD.CompanyID, NewValue = NEW.CompanyID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.VendorDeviceID != NEW.VendorDeviceID THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'VendorDeviceID', OriginalValue = OLD.VendorDeviceID, NewValue = NEW.VendorDeviceID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Longitude != NEW.Longitude THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Longitude', OriginalValue = OLD.Longitude, NewValue = NEW.Longitude, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Latitude != NEW.Latitude THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Latitude', OriginalValue = OLD.Latitude, NewValue = NEW.Latitude, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.InterconnectionID != NEW.InterconnectionID THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'InterconnectionID', OriginalValue = OLD.InterconnectionID, NewValue = NEW.InterconnectionID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Planned != NEW.Planned THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Planned', OriginalValue = OLD.Planned, NewValue = NEW.Planned, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Desired != NEW.Desired THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Desired', OriginalValue = OLD.Desired, NewValue = NEW.Desired, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.InProgress != NEW.InProgress THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'InProgress', OriginalValue = OLD.InProgress, NewValue = NEW.InProgress, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER OtherDevice_AuditDelete AFTER DELETE ON OtherDevice
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IsConcentrator', OriginalValue = OLD.IsConcentrator, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CompanyID', OriginalValue = OLD.CompanyID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'VendorDeviceID', OriginalValue = OLD.VendorDeviceID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Longitude', OriginalValue = OLD.Longitude, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Latitude', OriginalValue = OLD.Latitude, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'InterconnectionID', OriginalValue = OLD.InterconnectionID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Planned', OriginalValue = OLD.Planned, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Desired', OriginalValue = OLD.Desired, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'InProgress', OriginalValue = OLD.InProgress, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OtherDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER OutputStream_AuditUpdate AFTER UPDATE ON OutputStream
    FOR EACH ROW BEGIN

        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Acronym != NEW.Acronym THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NEW.Acronym, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Type != NEW.Type THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Type', OriginalValue = OLD.Type, NewValue = NEW.Type, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ConnectionString != NEW.ConnectionString THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NEW.ConnectionString, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.DataChannel != NEW.DataChannel THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DataChannel', OriginalValue = OLD.DataChannel, NewValue = NEW.DataChannel, UpdatedBy = NEW.UpdatedBy;
        END IF;
        IF OLD.CommandChannel != NEW.CommandChannel THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CommandChannel', OriginalValue = OLD.CommandChannel, NewValue = NEW.CommandChannel, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.IDCode != NEW.IDCode THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IDCode', OriginalValue = OLD.IDCode, NewValue = NEW.IDCode, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AutoPublishConfigFrame != NEW.AutoPublishConfigFrame THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AutoPublishConfigFrame', OriginalValue = OLD.AutoPublishConfigFrame, NewValue = NEW.AutoPublishConfigFrame, UpdatedBy = NEW.UpdatedBy;
        END IF;
        IF OLD.AutoStartDataChannel != NEW.AutoStartDataChannel THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AutoStartDataChannel', OriginalValue = OLD.AutoStartDataChannel, NewValue = NEW.AutoStartDataChannel, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.NominalFrequency != NEW.NominalFrequency THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NominalFrequency', OriginalValue = OLD.NominalFrequency, NewValue = NEW.NominalFrequency, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.FramesPerSecond != NEW.FramesPerSecond THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'FramesPerSecond', OriginalValue = OLD.FramesPerSecond, NewValue = NEW.FramesPerSecond, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LagTime != NEW.LagTime THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LagTime', OriginalValue = OLD.LagTime, NewValue = NEW.LagTime, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LeadTime != NEW.LeadTime THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LeadTime', OriginalValue = OLD.LeadTime, NewValue = NEW.LeadTime, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UseLocalClockAsRealTime != NEW.UseLocalClockAsRealTime THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UseLocalClockAsRealTime', OriginalValue = OLD.UseLocalClockAsRealTime, NewValue = NEW.UseLocalClockAsRealTime, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AllowSortsByArrival != NEW.AllowSortsByArrival THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowSortsByArrival', OriginalValue = OLD.AllowSortsByArrival, NewValue = NEW.AllowSortsByArrival, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.IgnoreBadTimestamps != NEW.IgnoreBadTimestamps THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IgnoreBadTimestamps', OriginalValue = OLD.IgnoreBadTimestamps, NewValue = NEW.IgnoreBadTimestamps, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.TimeResolution != NEW.TimeResolution THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TimeResolution', OriginalValue = OLD.TimeResolution, NewValue = NEW.TimeResolution, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AllowPreemptivePublishing != NEW.AllowPreemptivePublishing THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowPreemptivePublishing', OriginalValue = OLD.AllowPreemptivePublishing, NewValue = NEW.AllowPreemptivePublishing, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.PerformTimeReasonabilityCheck != NEW.PerformTimeReasonabilityCheck THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'PerformTimeReasonabilityCheck', OriginalValue = OLD.PerformTimeReasonabilityCheck, NewValue = NEW.PerformTimeReasonabilityCheck, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.DownsamplingMethod != NEW.DownsamplingMethod THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DownsamplingMethod', OriginalValue = OLD.DownsamplingMethod, NewValue = NEW.DownsamplingMethod, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.DataFormat != NEW.DataFormat THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DataFormat', OriginalValue = OLD.DataFormat, NewValue = NEW.DataFormat, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CoordinateFormat != NEW.CoordinateFormat THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CoordinateFormat', OriginalValue = OLD.CoordinateFormat, NewValue = NEW.CoordinateFormat, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CurrentScalingValue != NEW.CurrentScalingValue THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CurrentScalingValue', OriginalValue = OLD.CurrentScalingValue, NewValue = NEW.CurrentScalingValue, UpdatedBy = NEW.UpdatedBy;
        END IF;
        IF OLD.VoltageScalingValue != NEW.VoltageScalingValue THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'VoltageScalingValue', OriginalValue = OLD.VoltageScalingValue, NewValue = NEW.VoltageScalingValue, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AnalogScalingValue != NEW.AnalogScalingValue THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AnalogScalingValue', OriginalValue = OLD.AnalogScalingValue, NewValue = NEW.AnalogScalingValue, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.DigitalMaskValue != NEW.DigitalMaskValue THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DigitalMaskValue', OriginalValue = OLD.DigitalMaskValue, NewValue = NEW.DigitalMaskValue, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;	

    END$$

CREATE TRIGGER OutputStream_AuditDelete AFTER DELETE ON OutputStream
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Type', OriginalValue = OLD.Type, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ConnectionString', OriginalValue = OLD.ConnectionString, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DataChannel', OriginalValue = OLD.DataChannel, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CommandChannel', OriginalValue = OLD.CommandChannel, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IDCode', OriginalValue = OLD.IDCode, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AutoPublishConfigFrame', OriginalValue = OLD.AutoPublishConfigFrame, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AutoStartDataChannel', OriginalValue = OLD.AutoStartDataChannel, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NominalFrequency', OriginalValue = OLD.NominalFrequency, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'FramesPerSecond', OriginalValue = OLD.FramesPerSecond, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LagTime', OriginalValue = OLD.LagTime, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LeadTime', OriginalValue = OLD.LeadTime, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UseLocalClockAsRealTime', OriginalValue = OLD.UseLocalClockAsRealTime, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowSortsByArrival', OriginalValue = OLD.AllowSortsByArrival, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IgnoreBadTimestamps', OriginalValue = OLD.IgnoreBadTimestamps, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'TimeResolution', OriginalValue = OLD.TimeResolution, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AllowPreemptivePublishing', OriginalValue = OLD.AllowPreemptivePublishing, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'PerformTimeReasonabilityCheck', OriginalValue = OLD.PerformTimeReasonabilityCheck, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DownsamplingMethod', OriginalValue = OLD.DownsamplingMethod, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DataFormat', OriginalValue = OLD.DataFormat, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CoordinateFormat', OriginalValue = OLD.CoordinateFormat, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CurrentScalingValue', OriginalValue = OLD.CurrentScalingValue, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'VoltageScalingValue', OriginalValue = OLD.VoltageScalingValue, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AnalogScalingValue', OriginalValue = OLD.AnalogScalingValue, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DigitalMaskValue', OriginalValue = OLD.DigitalMaskValue, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStream', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER OutputStreamDevice_AuditUpdate AFTER UPDATE ON OutputStreamDevice
    FOR EACH ROW BEGIN
    
        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AdapterID != NEW.AdapterID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AdapterID', OriginalValue = OLD.AdapterID, NewValue = NEW.AdapterID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.IDCode != NEW.IDCode THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IDCode', OriginalValue = OLD.IDCode, NewValue = NEW.IDCode, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Acronym != NEW.Acronym THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NEW.Acronym, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.BpaAcronym != NEW.BpaAcronym THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'BpaAcronym', OriginalValue = OLD.BpaAcronym, NewValue = NEW.BpaAcronym, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.PhasorDataFormat != NEW.PhasorDataFormat THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'PhasorDataFormat', OriginalValue = OLD.PhasorDataFormat, NewValue = NEW.PhasorDataFormat, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.FrequencyDataFormat != NEW.FrequencyDataFormat THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'FrequencyDataFormat', OriginalValue = OLD.FrequencyDataFormat, NewValue = NEW.FrequencyDataFormat, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AnalogDataFormat != NEW.AnalogDataFormat THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AnalogDataFormat', OriginalValue = OLD.AnalogDataFormat, NewValue = NEW.AnalogDataFormat, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CoordinateFormat != NEW.CoordinateFormat THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CoordinateFormat', OriginalValue = OLD.CoordinateFormat, NewValue = NEW.CoordinateFormat, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER OutputStreamDevice_AuditDelete AFTER DELETE ON OutputStreamDevice
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AdapterID', OriginalValue = OLD.AdapterID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'IDCode', OriginalValue = OLD.IDCode, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'BpaAcronym', OriginalValue = OLD.BpaAcronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'PhasorDataFormat', OriginalValue = OLD.PhasorDataFormat, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'FrequencyDataFormat', OriginalValue = OLD.FrequencyDataFormat, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AnalogDataFormat', OriginalValue = OLD.AnalogDataFormat, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CoordinateFormat', OriginalValue = OLD.CoordinateFormat, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER OutputStreamDeviceAnalog_AuditUpdate AFTER UPDATE ON OutputStreamDeviceAnalog
    FOR EACH ROW BEGIN
    
        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.OutputStreamDeviceID != NEW.OutputStreamDeviceID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'OutputStreamDeviceID', OriginalValue = OLD.OutputStreamDeviceID, NewValue = NEW.OutputStreamDeviceID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Label != NEW.Label THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Label', OriginalValue = OLD.Label, NewValue = NEW.Label, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Type != NEW.Type THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Type', OriginalValue = OLD.Type, NewValue = NEW.Type, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ScalingValue != NEW.ScalingValue THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ScalingValue', OriginalValue = OLD.ScalingValue, NewValue = NEW.ScalingValue, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER OutputStreamDeviceAnalog_AuditDelete AFTER DELETE ON OutputStreamDeviceAnalog
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'OutputStreamDeviceID', OriginalValue = OLD.OutputStreamDeviceID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Label', OriginalValue = OLD.Label, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Type', OriginalValue = OLD.Type, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ScalingValue', OriginalValue = OLD.ScalingValue, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceAnalog', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;

    END$$

CREATE TRIGGER OutputStreamDeviceDigital_AuditUpdate AFTER UPDATE ON OutputStreamDeviceDigital
    FOR EACH ROW BEGIN

        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.OutputStreamDeviceID != NEW.OutputStreamDeviceID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'OutputStreamDeviceID', OriginalValue = OLD.OutputStreamDeviceID, NewValue = NEW.OutputStreamDeviceID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Label != NEW.Label THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Label', OriginalValue = OLD.Label, NewValue = NEW.Label, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.MaskValue != NEW.MaskValue THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MaskValue', OriginalValue = OLD.MaskValue, NewValue = NEW.MaskValue, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER OutputStreamDeviceDigital_AuditDelete AFTER DELETE ON OutputStreamDeviceDigital
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'OutputStreamDeviceID', OriginalValue = OLD.OutputStreamDeviceID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Label', OriginalValue = OLD.Label, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'MaskValue', OriginalValue = OLD.MaskValue, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDeviceDigital', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER OutputStreamDevicePhasor_AuditUpdate AFTER UPDATE ON OutputStreamDevicePhasor
    FOR EACH ROW BEGIN

        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.OutputStreamDeviceID != NEW.OutputStreamDeviceID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'OutputStreamDeviceID', OriginalValue = OLD.OutputStreamDeviceID, NewValue = NEW.OutputStreamDeviceID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Label != NEW.Label THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Label', OriginalValue = OLD.Label, NewValue = NEW.Label, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Type != NEW.Type THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Type', OriginalValue = OLD.Type, NewValue = NEW.Type, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.Phase != NEW.Phase THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Phase', OriginalValue = OLD.Phase, NewValue = NEW.Phase, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ScalingValue != NEW.ScalingValue THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ScalingValue', OriginalValue = OLD.ScalingValue, NewValue = NEW.ScalingValue, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER OutputStreamDevicePhasor_AuditDelete AFTER DELETE ON OutputStreamDevicePhasor
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'OutputStreamDeviceID', OriginalValue = OLD.OutputStreamDeviceID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Label', OriginalValue = OLD.Label, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Type', OriginalValue = OLD.Type, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Phase', OriginalValue = OLD.Phase, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ScalingValue', OriginalValue = OLD.ScalingValue, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamDevicePhasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER OutputStreamMeasurement_AuditUpdate AFTER UPDATE ON OutputStreamMeasurement
    FOR EACH ROW BEGIN
    
        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.AdapterID != NEW.AdapterID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AdapterID', OriginalValue = OLD.AdapterID, NewValue = NEW.AdapterID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.HistorianID != NEW.HistorianID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'HistorianID', OriginalValue = OLD.HistorianID, NewValue = NEW.HistorianID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.PointID != NEW.PointID THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'PointID', OriginalValue = OLD.PointID, NewValue = NEW.PointID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.SignalReference != NEW.SignalReference THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'SignalReference', OriginalValue = OLD.SignalReference, NewValue = NEW.SignalReference, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
    END$$

CREATE TRIGGER OutputStreamMeasurement_AuditDelete AFTER DELETE ON OutputStreamMeasurement
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AdapterID', OriginalValue = OLD.AdapterID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'HistorianID', OriginalValue = OLD.HistorianID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'PointID', OriginalValue = OLD.PointID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'SignalReference', OriginalValue = OLD.SignalReference, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'OutputStreamMeasurement', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        
    END$$

CREATE TRIGGER Phasor_AuditUpdate AFTER UPDATE ON Phasor 
    FOR EACH ROW BEGIN

        IF OLD.DeviceID != NEW.DeviceID THEN
            INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DeviceID', OriginalValue = OLD.DeviceID, NewValue = NEW.DeviceID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Label != NEW.Label THEN
            INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Label', OriginalValue = OLD.Label, NewValue = NEW.Label, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Type != NEW.Type THEN
            INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Type', OriginalValue = OLD.Type, NewValue = NEW.Type, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Phase != NEW.Phase THEN
            INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Phase', OriginalValue = OLD.Phase, NewValue = NEW.Phase, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.DestinationPhasorID != NEW.DestinationPhasorID THEN
            INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DestinationPhasorID', OriginalValue = OLD.DestinationPhasorID, NewValue = NEW.DestinationPhasorID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.SourceIndex != NEW.SourceIndex THEN
            INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'SourceIndex', OriginalValue = OLD.SourceIndex, NewValue = NEW.SourceIndex, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
    END$$

CREATE TRIGGER Phasor_AuditDelete AFTER DELETE ON Phasor
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DeviceID', OriginalValue = OLD.DeviceID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Label', OriginalValue = OLD.Label, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Type', OriginalValue = OLD.Type, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Phase', OriginalValue = OLD.Phase, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'DestinationPhasorID', OriginalValue = OLD.DestinationPhasorID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'SourceIndex', OriginalValue = OLD.SourceIndex, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Phasor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER Alarm_AuditUpdate AFTER UPDATE ON Alarm
    FOR EACH ROW BEGIN
    
        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.TagName != NEW.TagName THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'TagName', OriginalValue = OLD.TagName, NewValue = NEW.TagName, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.SignalID != NEW.SignalID THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'SignalID', OriginalValue = OLD.SignalID, NewValue = NEW.SignalID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.AssociatedMeasurementID != NEW.AssociatedMeasurementID THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'AssociatedMeasurementID', OriginalValue = OLD.AssociatedMeasurementID, NewValue = NEW.AssociatedMeasurementID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Description != NEW.Description THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NEW.Description, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Severity != NEW.Severity THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Severity', OriginalValue = OLD.Severity, NewValue = NEW.Severity, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Operation != NEW.Operation THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Operation', OriginalValue = OLD.Operation, NewValue = NEW.Operation, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.SetPoint != NEW.SetPoint THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'SetPoint', OriginalValue = OLD.SetPoint, NewValue = NEW.SetPoint, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Tolerance != NEW.Tolerance THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Tolerance', OriginalValue = OLD.Tolerance, NewValue = NEW.Tolerance, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Delay != NEW.Delay THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Delay', OriginalValue = OLD.Delay, NewValue = NEW.Delay, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Hysteresis != NEW.Hysteresis THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Hysteresis', OriginalValue = OLD.Hysteresis, NewValue = NEW.Hysteresis, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.LoadOrder != NEW.LoadOrder THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NEW.LoadOrder, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
    END$$
    
CREATE TRIGGER Alarm_AuditDelete AFTER DELETE ON Alarm
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'TagName', OriginalValue = OLD.TagName, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'SignalID', OriginalValue = OLD.SignalID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'AssociatedMeasurementID', OriginalValue = OLD.AssociatedMeasurementID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Severity', OriginalValue = OLD.Severity, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Operation', OriginalValue = OLD.Operation, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'SetPoint', OriginalValue = OLD.SetPoint, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Tolerance', OriginalValue = OLD.Tolerance, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Delay', OriginalValue = OLD.Delay, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Hysteresis', OriginalValue = OLD.Hysteresis, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'LoadOrder', OriginalValue = OLD.LoadOrder, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Alarm', PrimaryKeyColumn = 'ID', PrimaryKeyVAlue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER Vendor_AuditUpdate AFTER UPDATE ON Vendor
    FOR EACH ROW BEGIN

        IF OLD.Acronym != NEW.Acronym THEN
            INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NEW.Acronym, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.PhoneNumber != NEW.PhoneNumber THEN
            INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'PhoneNumber', OriginalValue = OLD.PhoneNumber, NewValue = NEW.PhoneNumber, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.ContactEmail != NEW.ContactEmail THEN
            INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ContactEmail', OriginalValue = OLD.ContactEmail, NewValue = NEW.ContactEmail, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Url != NEW.Url THEN
            INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Url', OriginalValue = OLD.Url, NewValue = NEW.Url, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER Vendor_AuditDelete AFTER DELETE ON Vendor
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'PhoneNumber', OriginalValue = OLD.PhoneNumber, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ContactEmail', OriginalValue = OLD.ContactEmail, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Url', OriginalValue = OLD.Url, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Vendor', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$

CREATE TRIGGER VendorDevice_AuditUpdate AFTER UPDATE ON VendorDevice
    FOR EACH ROW BEGIN

        IF OLD.VendorID != NEW.VendorID THEN
            INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'VendorID', OriginalValue = OLD.VendorID, NewValue = NEW.VendorID, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Description != NEW.Description THEN
            INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NEW.Description, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Url != NEW.Url THEN
            INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Url', OriginalValue = OLD.Url, NewValue = NEW.Url, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER VendorDevice_AuditDelete AFTER DELETE ON VendorDevice
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'VendorID', OriginalValue = OLD.VendorID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Url', OriginalValue = OLD.Url, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'VendorDevice', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$
    
CREATE TRIGGER MeasurementGroup_AuditUpdate AFTER UPDATE ON MeasurementGroup
    FOR EACH ROW BEGIN

        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.Description != NEW.Description THEN
            INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NEW.Description, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER MeasurementGroup_AuditDelete AFTER DELETE ON MeasurementGroup
    FOR EACH ROW BEGIN	
        INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Description', OriginalValue = OLD.Description, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'MeasurementGroup', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$
    
CREATE TRIGGER Subscriber_AuditUpdate AFTER UPDATE ON Subscriber
    FOR EACH ROW BEGIN

        IF OLD.NodeID != NEW.NodeID THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NEW.NodeID, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.Acronym != NEW.Acronym THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NEW.Acronym, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.Name != NEW.Name THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NEW.Name, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.SharedSecret != NEW.SharedSecret THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'SharedSecret', OriginalValue = OLD.SharedSecret, NewValue = NEW.SharedSecret, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.AuthKey != NEW.AuthKey THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AuthKey', OriginalValue = OLD.AuthKey, NewValue = NEW.AuthKey, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.ValidIPAddresses != NEW.ValidIPAddresses THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ValidIPAddresses', OriginalValue = OLD.ValidIPAddresses, NewValue = NEW.ValidIPAddresses, UpdatedBy = NEW.UpdatedBy;
        END IF;
        
        IF OLD.Enabled != NEW.Enabled THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NEW.Enabled, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.UpdatedOn != NEW.UpdatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NEW.UpdatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.UpdatedBy != NEW.UpdatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NEW.UpdatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;

        IF OLD.CreatedBy != NEW.CreatedBy THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NEW.CreatedBy, UpdatedBy = NEW.UpdatedBy;
        END IF;
    
        IF OLD.CreatedOn != NEW.CreatedOn THEN
            INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NEW.CreatedOn, UpdatedBy = NEW.UpdatedBy;
        END IF;

    END$$

CREATE TRIGGER Subscriber_AuditDelete AFTER DELETE ON Subscriber
    FOR EACH ROW BEGIN	
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'NodeID', OriginalValue = OLD.NodeID, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Acronym', OriginalValue = OLD.Acronym, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Name', OriginalValue = OLD.Name, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'SharedSecret', OriginalValue = OLD.SharedSecret, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'AuthKey', OriginalValue = OLD.AuthKey, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'ValidIPAddresses', OriginalValue = OLD.ValidIPAddresses, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'Enabled', OriginalValue = OLD.Enabled, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedOn', OriginalValue = OLD.UpdatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'UpdatedBy', OriginalValue = OLD.UpdatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedBy', OriginalValue = OLD.CreatedBy, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
        INSERT INTO AuditLog SET TableName = 'Subscriber', PrimaryKeyColumn = 'ID', PrimaryKeyValue = OLD.ID, ColumnName = 'CreatedOn', OriginalValue = OLD.CreatedOn, NewValue = NULL, Deleted = '1', UpdatedBy = @context;
    
    END$$
    
DELIMITER ;
