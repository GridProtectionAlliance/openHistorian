CREATE TRIGGER UserAccount_AuditUpdate AFTER UPDATE ON UserAccount FOR EACH ROW 
    BEGIN
        
        IF :OLD.Name != :NEW.Name THEN 
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;

        IF :OLD.Password != :NEW.Password THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'Password', :OLD.Password, :NEW.Password, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.FirstName != :NEW.FirstName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'FirstName', :OLD.FirstName, :NEW.FirstName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LastName != :NEW.LastName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'LastName', :OLD.LastName, :NEW.LastName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.DefaultNodeID != :NEW.DefaultNodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'DefaultNodeID', :OLD.DefaultNodeID, :NEW.DefaultNodeID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Phone != :NEW.Phone THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'Phone', :OLD.Phone, :NEW.Phone, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Email != :NEW.Email THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'Email', :OLD.Email, :NEW.Email, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LockedOut != :NEW.LockedOut THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'LockedOut', :OLD.LockedOut, :NEW.LockedOut, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UseADAuthentication != :NEW.UseADAuthentication THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'UseADAuthentication', :OLD.UseADAuthentication, :NEW.UseADAuthentication, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ChangePasswordOn != :NEW.ChangePasswordOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'ChangePasswordOn', TO_CLOB(:OLD.ChangePasswordOn), TO_CLOB(:NEW.ChangePasswordOn), :NEW.UpdatedBy);
        END IF;

        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;			
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
    END;
    /

CREATE TRIGGER UserAccount_AuditDelete AFTER DELETE ON UserAccount
    FOR EACH ROW BEGIN
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'Password', :OLD.Password, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'FirstName', :OLD.FirstName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'LastName', :OLD.LastName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'DefaultNodeID', :OLD.DefaultNodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'Phone', :OLD.Phone, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'Email', :OLD.Email, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'LockedOut', :OLD.LockedOut, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'UseADAuthentication', :OLD.UseADAuthentication, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'ChangePasswordOn', TO_CLOB(:OLD.ChangePasswordOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('UserAccount', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER SecurityGroup_AuditUpdate AFTER UPDATE ON SecurityGroup
    FOR EACH ROW BEGIN
    
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Description != :NEW.Description THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;

        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
    END;
    /

CREATE TRIGGER SecurityGroup_AuditDelete AFTER DELETE ON SecurityGroup
    FOR EACH ROW BEGIN		
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'Description', :OLD.Description, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('SecurityGroup', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
    END;
    /

CREATE TRIGGER ApplicationRole_AuditUpdate AFTER UPDATE ON ApplicationRole
    FOR EACH ROW BEGIN

        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Description != :NEW.Description THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'Description', :OLD.Description, :NEW.Description, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;

        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER ApplicationRole_AuditDelete AFTER DELETE ON ApplicationRole
    FOR EACH ROW BEGIN	
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'Description', :OLD.Description, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('ApplicationRole', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER CalcMeasurement_AuditUpdate AFTER UPDATE ON CalculatedMeasurement
    FOR EACH ROW BEGIN
    
        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Acronym != :NEW.Acronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, :NEW.Acronym, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AssemblyName != :NEW.AssemblyName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'AssemblyName', :OLD.AssemblyName, :NEW.AssemblyName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.TypeName != :NEW.TypeName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'TypeName', :OLD.TypeName, :NEW.TypeName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ConnectionString != :NEW.ConnectionString THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, :NEW.ConnectionString, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ConfigSection != :NEW.ConfigSection THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'ConfigSection', :OLD.ConfigSection, :NEW.ConfigSection, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.InputMeasurements != :NEW.InputMeasurements THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'InputMeasurements', :OLD.InputMeasurements, :NEW.InputMeasurements, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.OutputMeasurements != :NEW.OutputMeasurements THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'OutputMeasurements', :OLD.OutputMeasurements, :NEW.OutputMeasurements, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.MinimumMeasurementsToUse != :NEW.MinimumMeasurementsToUse THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'MinimumMeasurementsToUse', :OLD.MinimumMeasurementsToUse, :NEW.MinimumMeasurementsToUse, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.FramesPerSecond != :NEW.FramesPerSecond THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'FramesPerSecond', :OLD.FramesPerSecond, :NEW.FramesPerSecond, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LagTime != :NEW.LagTime THEN	
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'LagTime', :OLD.LagTime, :NEW.LagTime, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LeadTime != :NEW.LeadTime THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'LeadTime', :OLD.LeadTime, :NEW.LeadTime, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UseLocalClockAsRealTime != :NEW.UseLocalClockAsRealTime THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'UseLocalClockAsRealTime', :OLD.UseLocalClockAsRealTime, :NEW.UseLocalClockAsRealTime, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AllowSortsByArrival != :NEW.AllowSortsByArrival THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'AllowSortsByArrival', :OLD.AllowSortsByArrival, :NEW.AllowSortsByArrival, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.IgnoreBadTimestamps != :NEW.IgnoreBadTimestamps THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'IgnoreBadTimestamps', :OLD.IgnoreBadTimestamps, :NEW.IgnoreBadTimestamps, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.TimeResolution != :NEW.TimeResolution THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'TimeResolution', :OLD.TimeResolution, :NEW.TimeResolution, :NEW.UpdatedBy);
        END IF;

        IF :OLD.AllowPreemptivePublishing != :NEW.AllowPreemptivePublishing THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'AllowPreemptivePublishing', :OLD.AllowPreemptivePublishing, :NEW.AllowPreemptivePublishing, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.PerformTimeReasonabilityCheck != :NEW.PerformTimeReasonabilityCheck THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'PerformTimeReasonabilityCheck', :OLD.PerformTimeReasonabilityCheck, :NEW.PerformTimeReasonabilityCheck, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.DownsamplingMethod != :NEW.DownsamplingMethod THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'DownsamplingMethod', :OLD.DownsamplingMethod, :NEW.DownsamplingMethod, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER CalcMeasurement_AuditDelete AFTER DELETE ON CalculatedMeasurement
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'AssemblyName', :OLD.AssemblyName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'TypeName', :OLD.TypeName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'ConfigSection', :OLD.ConfigSection, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'InputMeasurements', :OLD.InputMeasurements, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'OutputMeasurements', :OLD.OutputMeasurements, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'MinimumMeasurementsToUse', :OLD.MinimumMeasurementsToUse, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'FramesPerSecond', :OLD.FramesPerSecond, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'LagTime', :OLD.LagTime, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'LeadTime', :OLD.LeadTime, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'UseLocalClockAsRealTime', :OLD.UseLocalClockAsRealTime, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'AllowSortsByArrival', :OLD.AllowSortsByArrival, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'IgnoreBadTimestamps', :OLD.IgnoreBadTimestamps, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'TimeResolution', :OLD.TimeResolution, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'AllowPreemptivePublishing', :OLD.AllowPreemptivePublishing, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'PerformTimeReasonabilityCheck', :OLD.PerformTimeReasonabilityCheck, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'DownsamplingMethod', :OLD.DownsamplingMethod, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CalculatedMeasurement', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);

    END;
    /

CREATE TRIGGER Company_AuditUpdate AFTER UPDATE ON Company
    FOR EACH ROW BEGIN

        IF :OLD.Acronym != :NEW.Acronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, :NEW.Acronym, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.MapAcronym != :NEW.MapAcronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'MapAcronym', :OLD.MapAcronym, :NEW.MapAcronym, :NEW.UpdatedBy);
        END IF;

        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Url != :NEW.Url THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'Url', :OLD.Url, :NEW.Url, :NEW.UpdatedBy);
        END IF;

        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
    END;
    /

CREATE TRIGGER Company_AuditDelete AFTER DELETE ON Company
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'MapAcronym', :OLD.MapAcronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'Url', :OLD.Url, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Company', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER CustomActAdapter_AuditUpdate AFTER UPDATE ON CustomActionAdapter
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;

        IF :OLD.AdapterName != :NEW.AdapterName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'AdapterName', :OLD.AdapterName, :NEW.AdapterName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AssemblyName != :NEW.AssemblyName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'AssemblyName', :OLD.AssemblyName, :NEW.AssemblyName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.TypeName != :NEW.TypeName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'TypeName', :OLD.TypeName, :NEW.TypeName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ConnectionString != :NEW.ConnectionString THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, :NEW.ConnectionString, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER CustomActAdapter_AuditDelete AFTER DELETE ON CustomActionAdapter
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'AdapterName', :OLD.AdapterName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'AssemblyName', :OLD.AssemblyName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'TypeName', :OLD.TypeName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomActionAdapter', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER CustomInputAdapter_AuditUpdate AFTER UPDATE ON CustomInputAdapter
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;

        IF :OLD.AdapterName != :NEW.AdapterName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'AdapterName', :OLD.AdapterName, :NEW.AdapterName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AssemblyName != :NEW.AssemblyName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'AssemblyName', :OLD.AssemblyName, :NEW.AssemblyName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.TypeName != :NEW.TypeName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'TypeName', :OLD.TypeName, :NEW.TypeName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ConnectionString != :NEW.ConnectionString THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, :NEW.ConnectionString, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER CustomInputAdapter_AuditDelete AFTER DELETE ON CustomInputAdapter
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'AdapterName', :OLD.AdapterName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'AssemblyName', :OLD.AssemblyName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'TypeName', :OLD.TypeName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomInputAdapter', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER CustomOutAdapter_AuditUpdate AFTER UPDATE ON CustomOutputAdapter
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;

        IF :OLD.AdapterName != :NEW.AdapterName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'AdapterName', :OLD.AdapterName, :NEW.AdapterName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AssemblyName != :NEW.AssemblyName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'AssemblyName', :OLD.AssemblyName, :NEW.AssemblyName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.TypeName != :NEW.TypeName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'TypeName', :OLD.TypeName, :NEW.TypeName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ConnectionString != :NEW.ConnectionString THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, :NEW.ConnectionString, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER CustomOutAdapter_AuditDelete AFTER DELETE ON CustomOutputAdapter
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'AdapterName', :OLD.AdapterName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'AssemblyName', :OLD.AssemblyName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'TypeName', :OLD.TypeName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('CustomOutputAdapter', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER Device_AuditUpdate AFTER UPDATE ON Device
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ParentID != :NEW.ParentID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ParentID', :OLD.ParentID, :NEW.ParentID, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.UniqueID != :NEW.UniqueID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'UniqueID', :OLD.UniqueID, :NEW.UniqueID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Acronym != :NEW.Acronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, :NEW.Acronym, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.OriginalSource != :NEW.OriginalSource THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'OriginalSource', :OLD.OriginalSource, :NEW.OriginalSource, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.IsConcentrator != :NEW.IsConcentrator THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'IsConcentrator', :OLD.IsConcentrator, :NEW.IsConcentrator, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CompanyID != :NEW.CompanyID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'CompanyID', :OLD.CompanyID, :NEW.CompanyID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.HistorianID != :NEW.HistorianID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'HistorianID', :OLD.HistorianID, :NEW.HistorianID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AccessID != :NEW.AccessID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'AccessID', :OLD.AccessID, :NEW.AccessID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.VendorDeviceID != :NEW.VendorDeviceID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'VendorDeviceID', :OLD.VendorDeviceID, :NEW.VendorDeviceID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ProtocolID != :NEW.ProtocolID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ProtocolID', :OLD.ProtocolID, :NEW.ProtocolID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Longitude != :NEW.Longitude THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'Longitude', :OLD.Longitude, :NEW.Longitude, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Latitude != :NEW.Latitude THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'Latitude', :OLD.Latitude, :NEW.Latitude, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.InterconnectionID != :NEW.InterconnectionID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'InterconnectionID', :OLD.InterconnectionID, :NEW.InterconnectionID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ConnectionString != :NEW.ConnectionString THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, :NEW.ConnectionString, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ConnectOnDemand != :NEW.ConnectOnDemand THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ConnectOnDemand', :OLD.ConnectOnDemand, :NEW.ConnectOnDemand, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.TimeZone != :NEW.TimeZone THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'TimeZone', :OLD.TimeZone, :NEW.TimeZone, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.FramesPerSecond != :NEW.FramesPerSecond THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'FramesPerSecond', :OLD.FramesPerSecond, :NEW.FramesPerSecond, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.TimeAdjustmentTicks != :NEW.TimeAdjustmentTicks THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'TimeAdjustmentTicks', :OLD.TimeAdjustmentTicks, :NEW.TimeAdjustmentTicks, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.DataLossInterval != :NEW.DataLossInterval THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'DataLossInterval', :OLD.DataLossInterval, :NEW.DataLossInterval, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AllowedParsingExceptions != :NEW.AllowedParsingExceptions THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'AllowedParsingExceptions', :OLD.AllowedParsingExceptions, :NEW.AllowedParsingExceptions, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ParsingExceptionWindow != :NEW.ParsingExceptionWindow THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ParsingExceptionWindow', :OLD.ParsingExceptionWindow, :NEW.ParsingExceptionWindow, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.DelayedConnectionInterval != :NEW.DelayedConnectionInterval THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'DelayedConnectionInterval', :OLD.DelayedConnectionInterval, :NEW.DelayedConnectionInterval, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AllowUseOfCachedConfiguration != :NEW.AllowUseOfCachedConfiguration THEN	
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'AllowUseOfCachedConfiguration', :OLD.AllowUseOfCachedConfiguration, :NEW.AllowUseOfCachedConfiguration, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AutoStartDataParsingSequence != :NEW.AutoStartDataParsingSequence THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'AutoStartDataParsingSequence', :OLD.AutoStartDataParsingSequence, :NEW.AutoStartDataParsingSequence, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.SkipDisableRealTimeData != :NEW.SkipDisableRealTimeData THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'SkipDisableRealTimeData', :OLD.SkipDisableRealTimeData, :NEW.SkipDisableRealTimeData, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.MeasurementReportingInterval != :NEW.MeasurementReportingInterval THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'MeasurementReportingInterval', :OLD.MeasurementReportingInterval, :NEW.MeasurementReportingInterval, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ContactList != :NEW.ContactList THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ContactList', :OLD.ContactList, :NEW.ContactList, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.MeasuredLines != :NEW.MeasuredLines THEN	
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'MeasuredLines', :OLD.MeasuredLines, :NEW.MeasuredLines, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;
    
    END;
    /

CREATE TRIGGER Device_AuditDelete AFTER DELETE ON Device
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ParentID', :OLD.ParentID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'UniqueID', :OLD.UniqueID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'IsConcentrator', :OLD.IsConcentrator, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'CompanyID', :OLD.CompanyID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'HistorianID', :OLD.HistorianID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'AccessID', :OLD.AccessID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'VendorDeviceID', :OLD.VendorDeviceID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ProtocolID', :OLD.ProtocolID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'Longitude', :OLD.Longitude, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'Latitude', :OLD.Latitude, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'InterconnectionID', :OLD.InterconnectionID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ConnectOnDemand', :OLD.ConnectOnDemand, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'TimeZone', :OLD.TimeZone, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'FramesPerSecond', :OLD.FramesPerSecond, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'TimeAdjustmentTicks', :OLD.TimeAdjustmentTicks, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'DataLossInterval', :OLD.DataLossInterval, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'AllowedParsingExceptions', :OLD.AllowedParsingExceptions, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ParsingExceptionWindow', :OLD.ParsingExceptionWindow, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'DelayedConnectionInterval', :OLD.DelayedConnectionInterval, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'AllowUseOfCachedConfiguration', :OLD.AllowUseOfCachedConfiguration, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'AutoStartDataParsingSequence', :OLD.AutoStartDataParsingSequence, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'SkipDisableRealTimeData', :OLD.SkipDisableRealTimeData, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'MeasurementReportingInterval', :OLD.MeasurementReportingInterval, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'ContactList', :OLD.ContactList, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'MeasuredLines', :OLD.MeasuredLines, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Device', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER Historian_AuditUpdate AFTER UPDATE ON Historian
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN	
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Acronym != :NEW.Acronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, :NEW.Acronym, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;

        IF :OLD.AssemblyName != :NEW.AssemblyName THEN	
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'AssemblyName', :OLD.AssemblyName, :NEW.AssemblyName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.TypeName != :NEW.TypeName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'TypeName', :OLD.TypeName, :NEW.TypeName, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ConnectionString != :NEW.ConnectionString THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, :NEW.ConnectionString, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.IsLocal != :NEW.IsLocal THEN	
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'IsLocal', :OLD.IsLocal, :NEW.IsLocal, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.MeasurementReportingInterval != :NEW.MeasurementReportingInterval THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'MeasurementReportingInterval', :OLD.MeasurementReportingInterval, :NEW.MeasurementReportingInterval, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Description != :NEW.Description THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'Description', :OLD.Description, :NEW.Description, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN	
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER Historian_AuditDelete AFTER DELETE ON Historian
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'AssemblyName', :OLD.AssemblyName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'TypeName', :OLD.TypeName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'IsLocal', :OLD.IsLocal, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'MeasurementReportingInterval', :OLD.MeasurementReportingInterval, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'Description', :OLD.Description, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Historian', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER Measurement_AuditUpdate AFTER UPDATE ON Measurement
    FOR EACH ROW BEGIN

        IF :OLD.HistorianID != :NEW.HistorianID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'HistorianID', :OLD.HistorianID, :NEW.HistorianID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.PointID != :NEW.PointID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'PointID', :OLD.PointID, :NEW.PointID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.DeviceID != :NEW.DeviceID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'DeviceID', :OLD.DeviceID, :NEW.DeviceID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.PointTag != :NEW.PointTag THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'PointTag', :OLD.PointTag, :NEW.PointTag, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AlternateTag != :NEW.AlternateTag THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'AlternateTag', :OLD.AlternateTag, :NEW.AlternateTag, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.SignalTypeID != :NEW.SignalTypeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'SignalTypeID', :OLD.SignalTypeID, :NEW.SignalTypeID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.PhasorSourceIndex != :NEW.PhasorSourceIndex THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'PhasorSourceIndex', :OLD.PhasorSourceIndex, :NEW.PhasorSourceIndex, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.SignalReference != :NEW.SignalReference THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'SignalReference', :OLD.SignalReference, :NEW.SignalReference, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Adder != :NEW.Adder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Adder', :OLD.Adder, :NEW.Adder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Multiplier != :NEW.Multiplier THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Multiplier', :OLD.Multiplier, :NEW.Multiplier, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Description != :NEW.Description THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Description', :OLD.Description, :NEW.Description, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Subscribed != :NEW.Subscribed THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Subscribed', :OLD.Subscribed, :NEW.Subscribed, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.Internal != :NEW.Internal THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Internal', :OLD.Internal, :NEW.Internal, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER Measurement_AuditDelete AFTER DELETE ON Measurement
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'HistorianID', :OLD.HistorianID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'PointID', :OLD.PointID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'DeviceID', :OLD.DeviceID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'PointTag', :OLD.PointTag, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'AlternateTag', :OLD.AlternateTag, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'SignalTypeID', :OLD.SignalTypeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'PhasorSourceIndex', :OLD.PhasorSourceIndex, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'SignalReference', :OLD.SignalReference, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Adder', :OLD.Adder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Multiplier', :OLD.Multiplier, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Description', :OLD.Description, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Subscribed', :OLD.Subscribed, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Internal', :OLD.Internal, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Measurement', 'SignalID', :OLD.SignalID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER Node_AuditUpdate AFTER UPDATE ON Node
    FOR EACH ROW BEGIN

        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CompanyID != :NEW.CompanyID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'CompanyID', :OLD.CompanyID, :NEW.CompanyID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Longitude != :NEW.Longitude THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Longitude', :OLD.Longitude, :NEW.Longitude, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Latitude != :NEW.Latitude THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Latitude', :OLD.Latitude, :NEW.Latitude, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Description != :NEW.Description THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Description', :OLD.Description, :NEW.Description, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ImagePath != :NEW.ImagePath THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'ImagePath', :OLD.ImagePath, :NEW.ImagePath, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Settings != :NEW.Settings THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Settings', :OLD.Settings, :NEW.Settings, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.MenuType != :NEW.MenuType THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'MenuType', :OLD.MenuType, :NEW.MenuType, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.MenuData != :NEW.MenuData THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'MenuData', :OLD.MenuData, :NEW.MenuData, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Master != :NEW.Master THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Master', :OLD.Master, :NEW.Master, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER Node_AuditDelete AFTER DELETE ON Node
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'CompanyID', :OLD.CompanyID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Longitude', :OLD.Longitude, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Latitude', :OLD.Latitude, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Description', :OLD.Description, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'ImagePath', :OLD.ImagePath, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Settings', :OLD.Settings, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'MenuType', :OLD.MenuType, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'MenuData', :OLD.MenuData, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Master', :OLD.Master, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Node', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER OtherDevice_AuditUpdate AFTER UPDATE ON OtherDevice
    FOR EACH ROW BEGIN

        IF :OLD.Acronym != :NEW.Acronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, :NEW.Acronym, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.IsConcentrator != :NEW.IsConcentrator THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'IsConcentrator', :OLD.IsConcentrator, :NEW.IsConcentrator, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CompanyID != :NEW.CompanyID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'CompanyID', :OLD.CompanyID, :NEW.CompanyID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.VendorDeviceID != :NEW.VendorDeviceID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'VendorDeviceID', :OLD.VendorDeviceID, :NEW.VendorDeviceID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Longitude != :NEW.Longitude THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Longitude', :OLD.Longitude, :NEW.Longitude, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Latitude != :NEW.Latitude THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Latitude', :OLD.Latitude, :NEW.Latitude, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.InterconnectionID != :NEW.InterconnectionID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'InterconnectionID', :OLD.InterconnectionID, :NEW.InterconnectionID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Planned != :NEW.Planned THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Planned', :OLD.Planned, :NEW.Planned, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Desired != :NEW.Desired THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Desired', :OLD.Desired, :NEW.Desired, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.InProgress != :NEW.InProgress THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'InProgress', :OLD.InProgress, :NEW.InProgress, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER OtherDevice_AuditDelete AFTER DELETE ON OtherDevice
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'IsConcentrator', :OLD.IsConcentrator, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'CompanyID', :OLD.CompanyID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'VendorDeviceID', :OLD.VendorDeviceID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Longitude', :OLD.Longitude, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Latitude', :OLD.Latitude, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'InterconnectionID', :OLD.InterconnectionID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Planned', :OLD.Planned, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'Desired', :OLD.Desired, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'InProgress', :OLD.InProgress, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OtherDevice', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER OutputStream_AuditUpdate AFTER UPDATE ON OutputStream
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Acronym != :NEW.Acronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, :NEW.Acronym, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Type != :NEW.Type THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'Type', :OLD.Type, :NEW.Type, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ConnectionString != :NEW.ConnectionString THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, :NEW.ConnectionString, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.DataChannel != :NEW.DataChannel THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'DataChannel', :OLD.DataChannel, :NEW.DataChannel, :NEW.UpdatedBy);
        END IF;
        IF :OLD.CommandChannel != :NEW.CommandChannel THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'CommandChannel', :OLD.CommandChannel, :NEW.CommandChannel, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.IDCode != :NEW.IDCode THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'IDCode', :OLD.IDCode, :NEW.IDCode, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AutoPublishConfigFrame != :NEW.AutoPublishConfigFrame THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'AutoPublishConfigFrame', :OLD.AutoPublishConfigFrame, :NEW.AutoPublishConfigFrame, :NEW.UpdatedBy);
        END IF;
        IF :OLD.AutoStartDataChannel != :NEW.AutoStartDataChannel THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'AutoStartDataChannel', :OLD.AutoStartDataChannel, :NEW.AutoStartDataChannel, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.NominalFrequency != :NEW.NominalFrequency THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'NominalFrequency', :OLD.NominalFrequency, :NEW.NominalFrequency, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.FramesPerSecond != :NEW.FramesPerSecond THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'FramesPerSecond', :OLD.FramesPerSecond, :NEW.FramesPerSecond, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LagTime != :NEW.LagTime THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'LagTime', :OLD.LagTime, :NEW.LagTime, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LeadTime != :NEW.LeadTime THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'LeadTime', :OLD.LeadTime, :NEW.LeadTime, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UseLocalClockAsRealTime != :NEW.UseLocalClockAsRealTime THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'UseLocalClockAsRealTime', :OLD.UseLocalClockAsRealTime, :NEW.UseLocalClockAsRealTime, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AllowSortsByArrival != :NEW.AllowSortsByArrival THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'AllowSortsByArrival', :OLD.AllowSortsByArrival, :NEW.AllowSortsByArrival, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.IgnoreBadTimestamps != :NEW.IgnoreBadTimestamps THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'IgnoreBadTimestamps', :OLD.IgnoreBadTimestamps, :NEW.IgnoreBadTimestamps, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.TimeResolution != :NEW.TimeResolution THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'TimeResolution', :OLD.TimeResolution, :NEW.TimeResolution, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AllowPreemptivePublishing != :NEW.AllowPreemptivePublishing THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'AllowPreemptivePublishing', :OLD.AllowPreemptivePublishing, :NEW.AllowPreemptivePublishing, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.PerformTimeReasonabilityCheck != :NEW.PerformTimeReasonabilityCheck THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'PerformTimeReasonabilityCheck', :OLD.PerformTimeReasonabilityCheck, :NEW.PerformTimeReasonabilityCheck, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.DownsamplingMethod != :NEW.DownsamplingMethod THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'DownsamplingMethod', :OLD.DownsamplingMethod, :NEW.DownsamplingMethod, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.DataFormat != :NEW.DataFormat THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'DataFormat', :OLD.DataFormat, :NEW.DataFormat, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CoordinateFormat != :NEW.CoordinateFormat THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'CoordinateFormat', :OLD.CoordinateFormat, :NEW.CoordinateFormat, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CurrentScalingValue != :NEW.CurrentScalingValue THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'CurrentScalingValue', :OLD.CurrentScalingValue, :NEW.CurrentScalingValue, :NEW.UpdatedBy);
        END IF;
        IF :OLD.VoltageScalingValue != :NEW.VoltageScalingValue THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'VoltageScalingValue', :OLD.VoltageScalingValue, :NEW.VoltageScalingValue, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AnalogScalingValue != :NEW.AnalogScalingValue THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'AnalogScalingValue', :OLD.AnalogScalingValue, :NEW.AnalogScalingValue, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.DigitalMaskValue != :NEW.DigitalMaskValue THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'DigitalMaskValue', :OLD.DigitalMaskValue, :NEW.DigitalMaskValue, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;	

    END;
    /

CREATE TRIGGER OutputStream_AuditDelete AFTER DELETE ON OutputStream
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'Type', :OLD.Type, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'ConnectionString', :OLD.ConnectionString, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'DataChannel', :OLD.DataChannel, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'CommandChannel', :OLD.CommandChannel, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'IDCode', :OLD.IDCode, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'AutoPublishConfigFrame', :OLD.AutoPublishConfigFrame, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'AutoStartDataChannel', :OLD.AutoStartDataChannel, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'NominalFrequency', :OLD.NominalFrequency, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'FramesPerSecond', :OLD.FramesPerSecond, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'LagTime', :OLD.LagTime, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'LeadTime', :OLD.LeadTime, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'UseLocalClockAsRealTime', :OLD.UseLocalClockAsRealTime, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'AllowSortsByArrival', :OLD.AllowSortsByArrival, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'IgnoreBadTimestamps', :OLD.IgnoreBadTimestamps, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'TimeResolution', :OLD.TimeResolution, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'AllowPreemptivePublishing', :OLD.AllowPreemptivePublishing, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'PerformTimeReasonabilityCheck', :OLD.PerformTimeReasonabilityCheck, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'DownsamplingMethod', :OLD.DownsamplingMethod, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'DataFormat', :OLD.DataFormat, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'CoordinateFormat', :OLD.CoordinateFormat, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'CurrentScalingValue', :OLD.CurrentScalingValue, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'VoltageScalingValue', :OLD.VoltageScalingValue, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'AnalogScalingValue', :OLD.AnalogScalingValue, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'DigitalMaskValue', :OLD.DigitalMaskValue, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStream', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER OutputStreamDevice_AuditUpdate AFTER UPDATE ON OutputStreamDevice
    FOR EACH ROW BEGIN
    
        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AdapterID != :NEW.AdapterID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'AdapterID', :OLD.AdapterID, :NEW.AdapterID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.IDCode != :NEW.IDCode THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'IDCode', :OLD.IDCode, :NEW.IDCode, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Acronym != :NEW.Acronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, :NEW.Acronym, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.BpaAcronym != :NEW.BpaAcronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'BpaAcronym', :OLD.BpaAcronym, :NEW.BpaAcronym, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.PhasorDataFormat != :NEW.PhasorDataFormat THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'PhasorDataFormat', :OLD.PhasorDataFormat, :NEW.PhasorDataFormat, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.FrequencyDataFormat != :NEW.FrequencyDataFormat THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'FrequencyDataFormat', :OLD.FrequencyDataFormat, :NEW.FrequencyDataFormat, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.AnalogDataFormat != :NEW.AnalogDataFormat THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'AnalogDataFormat', :OLD.AnalogDataFormat, :NEW.AnalogDataFormat, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CoordinateFormat != :NEW.CoordinateFormat THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'CoordinateFormat', :OLD.CoordinateFormat, :NEW.CoordinateFormat, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER OutputStreamDevice_AuditDelete AFTER DELETE ON OutputStreamDevice
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'AdapterID', :OLD.AdapterID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'IDCode', :OLD.IDCode, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'BpaAcronym', :OLD.BpaAcronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'PhasorDataFormat', :OLD.PhasorDataFormat, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'FrequencyDataFormat', :OLD.FrequencyDataFormat, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'AnalogDataFormat', :OLD.AnalogDataFormat, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'CoordinateFormat', :OLD.CoordinateFormat, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevice', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER OutStreamDevAnalog_AuditUpdate AFTER UPDATE ON OutputStreamDeviceAnalog
    FOR EACH ROW BEGIN
    
        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.OutputStreamDeviceID != :NEW.OutputStreamDeviceID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'OutputStreamDeviceID', :OLD.OutputStreamDeviceID, :NEW.OutputStreamDeviceID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Label != :NEW.Label THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'Label', :OLD.Label, :NEW.Label, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Type != :NEW.Type THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'Type', :OLD.Type, :NEW.Type, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ScalingValue != :NEW.ScalingValue THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'ScalingValue', :OLD.ScalingValue, :NEW.ScalingValue, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER OutStreamDevAnalog_AuditDelete AFTER DELETE ON OutputStreamDeviceAnalog
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'OutputStreamDeviceID', :OLD.OutputStreamDeviceID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'Label', :OLD.Label, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'Type', :OLD.Type, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'ScalingValue', :OLD.ScalingValue, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceAnalog', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);

    END;
    /

CREATE TRIGGER OutStrDevDigital_AuditUpdate AFTER UPDATE ON OutputStreamDeviceDigital
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.OutputStreamDeviceID != :NEW.OutputStreamDeviceID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'OutputStreamDeviceID', :OLD.OutputStreamDeviceID, :NEW.OutputStreamDeviceID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Label != :NEW.Label THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'Label', :OLD.Label, :NEW.Label, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.MaskValue != :NEW.MaskValue THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'MaskValue', :OLD.MaskValue, :NEW.MaskValue, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER OutStrDevDigital_AuditDelete AFTER DELETE ON OutputStreamDeviceDigital
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'OutputStreamDeviceID', :OLD.OutputStreamDeviceID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'Label', :OLD.Label, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'MaskValue', :OLD.MaskValue, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDeviceDigital', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER OutStreamDevPhasor_AuditUpdate AFTER UPDATE ON OutputStreamDevicePhasor
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.OutputStreamDeviceID != :NEW.OutputStreamDeviceID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'OutputStreamDeviceID', :OLD.OutputStreamDeviceID, :NEW.OutputStreamDeviceID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Label != :NEW.Label THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'Label', :OLD.Label, :NEW.Label, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Type != :NEW.Type THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'Type', :OLD.Type, :NEW.Type, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.Phase != :NEW.Phase THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'Phase', :OLD.Phase, :NEW.Phase, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ScalingValue != :NEW.ScalingValue THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'ScalingValue', :OLD.ScalingValue, :NEW.ScalingValue, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER OutStreamDevPhasor_AuditDelete AFTER DELETE ON OutputStreamDevicePhasor
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'OutputStreamDeviceID', :OLD.OutputStreamDeviceID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'Label', :OLD.Label, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'Type', :OLD.Type, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'Phase', :OLD.Phase, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'ScalingValue', :OLD.ScalingValue, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamDevicePhasor', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER OutputStreamMeas_AuditUpdate AFTER UPDATE ON OutputStreamMeasurement
    FOR EACH ROW BEGIN
    
        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;

        IF :OLD.AdapterID != :NEW.AdapterID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'AdapterID', :OLD.AdapterID, :NEW.AdapterID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.HistorianID != :NEW.HistorianID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'HistorianID', :OLD.HistorianID, :NEW.HistorianID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.PointID != :NEW.PointID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'PointID', :OLD.PointID, :NEW.PointID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.SignalReference != :NEW.SignalReference THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'SignalReference', :OLD.SignalReference, :NEW.SignalReference, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;
        
    END;
    /

CREATE TRIGGER OutputStreamMeas_AuditDelete AFTER DELETE ON OutputStreamMeasurement
    FOR EACH ROW BEGIN
    
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'AdapterID', :OLD.AdapterID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'HistorianID', :OLD.HistorianID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'PointID', :OLD.PointID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'SignalReference', :OLD.SignalReference, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('OutputStreamMeasurement', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        
    END;
    /

CREATE TRIGGER Phasor_AuditUpdate AFTER UPDATE ON Phasor 
    FOR EACH ROW BEGIN

        IF :OLD.DeviceID != :NEW.DeviceID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'DeviceID', :OLD.DeviceID, :NEW.DeviceID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Label != :NEW.Label THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'Label', :OLD.Label, :NEW.Label, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Type != :NEW.Type THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'Type', :OLD.Type, :NEW.Type, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Phase != :NEW.Phase THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'Phase', :OLD.Phase, :NEW.Phase, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.DestinationPhasorID != :NEW.DestinationPhasorID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'DestinationPhasorID', :OLD.DestinationPhasorID, :NEW.DestinationPhasorID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.SourceIndex != :NEW.SourceIndex THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'SourceIndex', :OLD.SourceIndex, :NEW.SourceIndex, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;
    
    END;
    /

CREATE TRIGGER Phasor_AuditDelete AFTER DELETE ON Phasor
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'DeviceID', :OLD.DeviceID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'Label', :OLD.Label, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'Type', :OLD.Type, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'Phase', :OLD.Phase, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'DestinationPhasorID', :OLD.DestinationPhasorID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'SourceIndex', :OLD.SourceIndex, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Phasor', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER Alarm_AuditUpdate AFTER UPDATE ON Alarm 
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;

        IF :OLD.TagName != :NEW.TagName THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'TagName', :OLD.TagName, :NEW.TagName, :NEW.UpdatedBy);
        END IF;

        IF :OLD.SignalID != :NEW.SignalID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'SignalID', :OLD.SignalID, :NEW.SignalID, :NEW.UpdatedBy);
        END IF;

        IF :OLD.AssociatedMeasurementID != :NEW.AssociatedMeasurementID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'AssociatedMeasurementID', :OLD.AssociatedMeasurementID, :NEW.AssociatedMeasurementID, :NEW.UpdatedBy);
        END IF;

        IF :OLD.Description != :NEW.Description THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Description', :OLD.Description, :NEW.Description, :NEW.UpdatedBy);
        END IF;

        IF :OLD.Severity != :NEW.Severity THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Severity', :OLD.Severity, :NEW.Severity, :NEW.UpdatedBy);
        END IF;

        IF :OLD.Operation != :NEW.Operation THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Operation', :OLD.Operation, :NEW.Operation, :NEW.UpdatedBy);
        END IF;

        IF :OLD.SetPoint != :NEW.SetPoint THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'SetPoint', :OLD.SetPoint, :NEW.SetPoint, :NEW.UpdatedBy);
        END IF;

        IF :OLD.Tolerance != :NEW.Tolerance THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Tolerance', :OLD.Tolerance, :NEW.Tolerance, :NEW.UpdatedBy);
        END IF;

        IF :OLD.Delay != :NEW.Delay THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Delay', :OLD.Delay, :NEW.Delay, :NEW.UpdatedBy);
        END IF;

        IF :OLD.Hysteresis != :NEW.Hysteresis THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Hysteresis', :OLD.Hysteresis, :NEW.Hysteresis, :NEW.UpdatedBy);
        END IF;

        IF :OLD.LoadOrder != :NEW.LoadOrder THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, :NEW.LoadOrder, :NEW.UpdatedBy);
        END IF;

        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;
    
    END;
    /

CREATE TRIGGER Alarm_AuditDelete AFTER DELETE ON Alarm
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'TagName', :OLD.TagName, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'SignalID', :OLD.SignalID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'AssociatedMeasurementID', :OLD.AssociatedMeasurementID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Description', :OLD.Description, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Severity', :OLD.Severity, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Operation', :OLD.Operation, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'SetPoint', :OLD.SetPoint, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Tolerance', :OLD.Tolerance, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Delay', :OLD.Delay, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Hysteresis', :OLD.Hysteresis, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'LoadOrder', :OLD.LoadOrder, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Alarm', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER Vendor_AuditUpdate AFTER UPDATE ON Vendor
    FOR EACH ROW BEGIN

        IF :OLD.Acronym != :NEW.Acronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, :NEW.Acronym, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.PhoneNumber != :NEW.PhoneNumber THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'PhoneNumber', :OLD.PhoneNumber, :NEW.PhoneNumber, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.ContactEmail != :NEW.ContactEmail THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'ContactEmail', :OLD.ContactEmail, :NEW.ContactEmail, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Url != :NEW.Url THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'Url', :OLD.Url, :NEW.Url, :NEW.UpdatedBy);
        END IF;

        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER Vendor_AuditDelete AFTER DELETE ON Vendor
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'PhoneNumber', :OLD.PhoneNumber, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'ContactEmail', :OLD.ContactEmail, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'Url', :OLD.Url, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Vendor', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /

CREATE TRIGGER VendorDevice_AuditUpdate AFTER UPDATE ON VendorDevice
    FOR EACH ROW BEGIN

        IF :OLD.VendorID != :NEW.VendorID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'VendorID', :OLD.VendorID, :NEW.VendorID, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Description != :NEW.Description THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'Description', :OLD.Description, :NEW.Description, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Url != :NEW.Url THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'Url', :OLD.Url, :NEW.Url, :NEW.UpdatedBy);
        END IF;

        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER VendorDevice_AuditDelete AFTER DELETE ON VendorDevice
    FOR EACH ROW BEGIN

        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'VendorID', :OLD.VendorID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'Description', :OLD.Description, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'Url', :OLD.Url, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('VendorDevice', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
    
    END;
    /
    
CREATE TRIGGER MeasurementGroup_AuditUpdate AFTER UPDATE ON MeasurementGroup
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.Description != :NEW.Description THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'Description', :OLD.Description, :NEW.Description, :NEW.UpdatedBy);
        END IF;

        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER MeasurementGroup_AuditDelete AFTER DELETE ON MeasurementGroup
    FOR EACH ROW BEGIN	
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'Description', :OLD.Description, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('MeasurementGroup', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
    
    END;
    /
    
CREATE TRIGGER Subscriber_AuditUpdate AFTER UPDATE ON Subscriber
    FOR EACH ROW BEGIN

        IF :OLD.NodeID != :NEW.NodeID THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, :NEW.NodeID, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.Acronym != :NEW.Acronym THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, :NEW.Acronym, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.Name != :NEW.Name THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'Name', :OLD.Name, :NEW.Name, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.SharedSecret != :NEW.SharedSecret THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'SharedSecret', :OLD.SharedSecret, :NEW.SharedSecret, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.AuthKey != :NEW.AuthKey THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'AuthKey', :OLD.AuthKey, :NEW.AuthKey, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.ValidIPAddresses != :NEW.ValidIPAddresses THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'ValidIPAddresses', :OLD.ValidIPAddresses, :NEW.ValidIPAddresses, :NEW.UpdatedBy);
        END IF;
        
        IF :OLD.Enabled != :NEW.Enabled THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, :NEW.Enabled, :NEW.UpdatedBy);
        END IF;

        IF :OLD.UpdatedOn != :NEW.UpdatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), TO_CLOB(:NEW.UpdatedOn), :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.UpdatedBy != :NEW.UpdatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, :NEW.UpdatedBy, :NEW.UpdatedBy);
        END IF;

        IF :OLD.CreatedBy != :NEW.CreatedBy THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, :NEW.CreatedBy, :NEW.UpdatedBy);
        END IF;
    
        IF :OLD.CreatedOn != :NEW.CreatedOn THEN
            INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), TO_CLOB(:NEW.CreatedOn), :NEW.UpdatedBy);
        END IF;

    END;
    /

CREATE TRIGGER Subscriber_AuditDelete AFTER DELETE ON Subscriber
    FOR EACH ROW BEGIN	
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'NodeID', :OLD.NodeID, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'Acronym', :OLD.Acronym, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'Name', :OLD.Name, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'SharedSecret', :OLD.SharedSecret, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'AuthKey', :OLD.AuthKey, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'ValidIPAddresses', :OLD.ValidIPAddresses, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'Enabled', :OLD.Enabled, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'UpdatedOn', TO_CLOB(:OLD.UpdatedOn), NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'UpdatedBy', :OLD.UpdatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'CreatedBy', :OLD.CreatedBy, NULL, '1', context.get_current_user);
        INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) VALUES ('Subscriber', 'ID', :OLD.ID, 'CreatedOn', TO_CLOB(:OLD.CreatedOn), NULL, '1', context.get_current_user);
    
    END;
    /