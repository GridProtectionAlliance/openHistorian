INSERT INTO Protocol(Acronym, Name, Type, Category, AssemblyName, TypeName, LoadOrder) VALUES('COMTRADE', 'COMTRADE Import', 'Measurement', 'Imported', 'TestingAdapters.dll', 'TestingAdapters.VirtualInputAdapter', 15)
GO

INSERT INTO ConfigurationEntity(SourceName, RuntimeName, Description, LoadOrder, Enabled) VALUES('NodeCompressionSetting', 'CompressionSettings', 'Defines information about measurement compression settings', 19, 1)
GO