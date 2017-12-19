INSERT INTO Protocol(Acronym, Name, Type, Category, AssemblyName, TypeName, LoadOrder) VALUES('Modbus', 'Modbus Poller', 'Measurement', 'Device', 'ModbusAdapters.dll', 'ModbusAdapters.ModbusPoller', 13);
INSERT INTO Protocol(Acronym, Name, Type, Category, AssemblyName, TypeName, LoadOrder) VALUES('COMTRADE', 'COMTRADE Import', 'Measurement', 'Imported', 'TestingAdapters.dll', 'TestingAdapters.VirtualInputAdapter', 14);

INSERT INTO ConfigurationEntity(SourceName, RuntimeName, Description, LoadOrder, Enabled) VALUES('NodeCompressionSetting', 'CompressionSettings', 'Defines information about measurement compression settings', 19, 1);
