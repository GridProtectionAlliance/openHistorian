-- Script auto-generated generated from Access database openHistorian.mdb method GenerateInitialDataScript
-- 11/5/2010 4:53:57 PM

USE openHistorian

INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('AEP', 'AEP', 'American Electric Power', 1);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('AGP', 'AGP', 'Allegheny Power', 2);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('AMR', 'AMR', 'Ameren', 3);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('ATC', 'ATC', 'American Transmission Company', 4);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('BCH', 'BCH', 'British Columbia Hydro', 5);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('BPA', 'BPA', 'Bonneville Power Administration', 6);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('ConED', 'CED', 'ConEdison', 7);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('DOM', 'DOM', 'Dominion', 8);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('EES', 'EES', 'Entergy Energy Services', 9);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('EXE', 'EXE', 'Exelon Energy', 10);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('FEN', 'FEN', 'First Energy', 11);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('FPL', 'FPL', 'Florida Power & Light Company', 12);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('HQC', 'HQC', 'Hydro Quebec', 13);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('LIPA', 'LPA', 'Long Island Power Authority', 14);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('LAWP', 'LWP', 'Los Angeles Dept of Water and Power', 15);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('MDA', 'MDA', 'Ameritech', 16);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('MDK', 'MDK', 'Montana-Dakota ', 17);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('MISO', 'MSO', 'Midwest ISO', 18);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('MTB', 'MTB', 'Manitoba Hydro', 19);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('METC', 'MTC', 'Michigan Electric Transmission Co.', 20);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('NYPA', 'NYP', 'New York Power Authority', 21);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('DDL', 'DDL', 'Desired Device Locations', 22);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('PPL', 'PPL', 'PPL Electric Utilities', 23);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('PGE', 'PGE', 'Pacific Gas and Electric', 24);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('PJM', 'PJM', 'PJM Interconnection', 25);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('SCE', 'SCE', 'Southern California Edison', 26);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('SOCO', 'SOC', 'Southern Company', 27);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('SPP', 'SPP', 'Southwest Power Pool', 28);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('SWT', 'SWT', 'Southwest (APS and SRP)', 29);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('TVA', 'TVA', 'Tennessee Valley Authority', 30);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('VT', 'VT ', 'Virginia Tech', 31);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('WAPA', 'WPA', 'Western Area Power Administration', 32);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('NEISO', 'NEI', 'New England ISO', 33);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('MPC', 'MPC', 'Minnkota Power Collective', 34);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('MAM', 'MAM', 'MidAmerican Power', 35);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('HEC', 'HEC', 'Hawaiian Electric Company', 36);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('OGE', 'OGE', 'Oklahoma Gas & Electric', 37);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('ITC', 'ITC', 'International Transmission Company', 38);
INSERT INTO Company(Acronym, MapAcronym, Name, LoadOrder) VALUES('DUKE', 'DUK', 'Duke Power', 39);
INSERT INTO ConfigurationEntity(SourceName, RuntimeName, Description, LoadOrder, Enabled) VALUES('InputAdapter', 'InputAdapters', 'Defines IInputAdapter definitions for a Historian node', 1, 1);
INSERT INTO ConfigurationEntity(SourceName, RuntimeName, Description, LoadOrder, Enabled) VALUES('ActionAdapter', 'ActionAdapters', 'Defines IActionAdapter definitions for a Historian node', 2, 1);
INSERT INTO ConfigurationEntity(SourceName, RuntimeName, Description, LoadOrder, Enabled) VALUES('OutputAdapter', 'OutputAdapters', 'Defines IOutputAdapter definitions for a Historian node', 3, 1);
INSERT INTO Node(Name, CompanyID, Description, RemoteStatusServiceUrl, Master, LoadOrder, Enabled) VALUES('Default', NULL, 'Default node', 'Server=localhost:8600', 0, 0, 1);
UPDATE Node SET ID='e7a5235d-cb6f-4864-a96e-a8686f36e599' WHERE Name='Default';
INSERT INTO DataOperation(NodeId, Description, AssemblyName, TypeName, MethodName, Arguments, LoadOrder, Enabled) VALUES('e7a5235d-cb6f-4864-a96e-a8686f36e599','Optimize Local Historian Settings', 'HistorianAdapters.dll', 'HistorianAdapters.LocalOutputAdapter', 'OptimizeLocalHistorianSettings', '', 1, 1);
INSERT INTO InputAdapter(NodeID, AdapterName, AssemblyName, TypeName, ConnectionString, LoadOrder, Enabled) VALUES('e7a5235d-cb6f-4864-a96e-a8686f36e599', 'InputAdapter', 'HistorianAdapters.dll', 'HistorianAdapters.LocalOutputAdapter', 'protocol=TCP;server=localhost;port=1003;initiateConnection=False;', 0, 1);
INSERT INTO OutputAdapter(NodeID, AdapterName, AssemblyName, TypeName, ConnectionString, LoadOrder, Enabled) VALUES('e7a5235d-cb6f-4864-a96e-a8686f36e599', 'OutputAdapter', 'HistorianAdapters.dll', 'HistorianAdapters.LocalOutputAdapter', 'instancename=devarchive;archivepath=C:\My Archives;refreshmetadata=false;', 0, 1);