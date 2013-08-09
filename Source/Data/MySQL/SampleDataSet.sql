-- Script auto-generated generated from Access database openHistorian.mdb method GenerateSampleDataScript
-- 7/8/2010 1:07:40 PM

USE openHistorian;

INSERT INTO Node(Name, CompanyID, Description, Settings, MenuType, MenuData, Master, LoadOrder, Enabled) 
	VALUES('Default', NULL, 'Default node', 'RemoteStatusServerConnectionString={server=localhost:8510; integratedSecurity=true}; dataPublisherPort=6175', 
			'File', 'Menu.xml', 1, 0, 1);
UPDATE Node SET ID='8736F6C7-AD41-4B43-B4F6-E684E0D4AD20' WHERE Name='Default';
INSERT INTO Historian(NodeID, Acronym, Name, AssemblyName, TypeName, ConnectionString, IsLocal, Description, LoadOrder, Enabled) VALUES('8736F6C7-AD41-4B43-B4F6-E684E0D4AD20', 'DEVARCHIVE', 'Local Development Archive', 'TestingAdapters.dll', 'TestingAdapters.VirtualOutputAdapter', '', 1, 'Local development archive', 0, 1);
INSERT INTO Device(NodeID, Acronym, Name, IsConcentrator, CompanyID, HistorianID, AccessID, VendorDeviceID, ProtocolID, Longitude, Latitude, InterconnectionID, ConnectionString, MeasuredLines, LoadOrder, Enabled) VALUES('8736F6C7-AD41-4B43-B4F6-E684E0D4AD20', 'TESTDEVICE', 'Test Device', 0, 30, 1, 2, 2, 3, -89.8038, 35.3871, 1, 'transportProtocol=File; file=Sample1344.PmuCapture; useHighResolutionInputTimer=True', 3, 0, 1);
INSERT INTO Phasor(DeviceID, Label, Type, Phase, SourceIndex) VALUES(1, '500 kV Bus 1', 'V', '+', 1);
INSERT INTO Phasor(DeviceID, Label, Type, Phase, SourceIndex) VALUES(1, '500 kV Bus 2', 'V', '+', 2);
INSERT INTO Phasor(DeviceID, Label, Type, Phase, SourceIndex) VALUES(1, 'Cordova', 'I', '+', 3);
INSERT INTO Phasor(DeviceID, Label, Type, Phase, SourceIndex) VALUES(1, 'Dell', 'I', '+', 4);
INSERT INTO Phasor(DeviceID, Label, Type, Phase, SourceIndex) VALUES(1, 'Lagoon Creek', 'I', '+', 5);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE:ABBS', 8, NULL, 'TESTDEVICE-SF', 'Test Device ABB-521 Status Flags', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE:ABBF', 5, NULL, 'TESTDEVICE-FQ', 'Test Device ABB-521 Frequency', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE:ABBD1', 9, NULL, 'TESTDEVICE-DV1', 'Test Device ABB-521 Digital Value 1', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE:ABBDF', 6, NULL, 'TESTDEVICE-DF', 'Test Device ABB-521 Frequency Delta (dF/dt)', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE-BUS1:ABBV', 3, 1, 'TESTDEVICE-PM1', 'Test Device ABB-521 500 kV Bus 1 Positive Sequence Voltage Magnitude', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE-BUS1:ABBVH', 4, 1, 'TESTDEVICE-PA1', 'Test Device ABB-521 500 kV Bus 1 Positive Sequence Voltage Phase Angle', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE-BUS2:ABBV', 3, 2, 'TESTDEVICE-PM2', 'Test Device ABB-521 500 kV Bus 2 Positive Sequence Voltage Magnitude', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE-BUS2:ABBVH', 4, 2, 'TESTDEVICE-PA2', 'Test Device ABB-521 500 kV Bus 2 Positive Sequence Voltage Phase Angle', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE-CORD:ABBI', 1, 3, 'TESTDEVICE-PM3', 'Test Device ABB-521 Cordova Positive Sequence Current Magnitude', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE-CORD:ABBIH', 2, 3, 'TESTDEVICE-PA3', 'Test Device ABB-521 Cordova Positive Sequence Current Phase Angle', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE-DELL:ABBI', 1, 4, 'TESTDEVICE-PM4', 'Test Device ABB-521 Dell Positive Sequence Current Magnitude', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE-DELL:ABBIH', 2, 4, 'TESTDEVICE-PA4', 'Test Device ABB-521 Dell Positive Sequence Current Phase Angle', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE-LAGO:ABBI', 1, 5, 'TESTDEVICE-PM5', 'Test Device ABB-521 Lagoon Creek Positive Sequence Current Magnitude', 1);
INSERT INTO Measurement(HistorianID, DeviceID, PointTag, SignalTypeID, PhasorSourceIndex, SignalReference, Description, Enabled) VALUES(1, 1, 'TVA_TESTDEVICE-LAGO:ABBIH', 2, 5, 'TESTDEVICE-PA5', 'Test Device ABB-521 Lagoon Creek Positive Sequence Current Phase Angle', 1);
INSERT INTO ApplicationRole (Name, Description, NodeID) VALUES ('Administrator', 'Administrator Role', '8736F6C7-AD41-4B43-B4F6-E684E0D4AD20');
INSERT INTO ApplicationRole (Name, Description, NodeID) VALUES ('Editor', 'Editor', '8736F6C7-AD41-4B43-B4F6-E684E0D4AD20');
INSERT INTO ApplicationRole (Name, Description, NodeID) VALUES ('Viewer', 'Viewer Role', '8736F6C7-AD41-4B43-B4F6-E684E0D4AD20');