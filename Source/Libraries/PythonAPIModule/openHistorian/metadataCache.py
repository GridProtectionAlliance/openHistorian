#******************************************************************************************************
#  metadataCache.py - Gbtc
#
#  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
#
#  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
#  the NOTICE file distributed with this work for additional information regarding copyright ownership.
#  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
#  file except in compliance with the License. You may obtain a copy of the License at:
#
#      http://opensource.org/licenses/MIT
#
#  Unless agreed to in writing, the subject software distributed under the License is distributed on an
#  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
#  License for the specific language governing permissions and limitations.
#
#  Code Modification History:
#  ----------------------------------------------------------------------------------------------------
#  02/07/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from .measurementRecord import measurementRecord, SignalType
from .deviceRecord import deviceRecord
from .phasorRecord import phasorRecord
from gsf import Empty
import xml.etree.ElementTree as xmlParser
from typing import Optional, List, Dict, Set
from datetime import datetime
from uuid import UUID, uuid1
from decimal import Decimal
import numpy as np

class metadataCache:
    """
    Represents a collection of openHistorian metadata.
    """
    def __init__(self, metadataXML: str):
        # Parse metadata
        metadata = xmlParser.fromstring(metadataXML)
                
        # Extract measurement records from MeasurementDetail table rows
        measurementRecords: List[measurementRecord] = list()

        for measurement in metadata.findall("MeasurementDetail"):        
            # Get element text or empty string when value is None
            getElementText = lambda elementName: metadataCache.__getElementText(measurement, elementName)

            # Parse openHistorian instance name and point ID from measurement key
            (instanceName, pointID) = metadataCache.__getMeasurementKey(measurement)
            
            if pointID == 0:
                continue

            measurementRecords.append(measurementRecord(
                # `instanceName`: Source instance name of measurement
                instanceName,
                # `pointID`: openHistorian point ID of measurement
                pointID,
                # `signalID`: Extract signal ID, the unique measurement guid
                metadataCache.__getGuid(measurement, "SignalID"),
                # `pointTag`: Extract the measurement point tag
                getElementText("PointTag"),
                # `signalReference`: Extract the measurement signal reference
                getElementText("SignalReference"),
                # `signalTypeName`: Extract the measurement signal type name
                getElementText("SignalAcronym"),
                # `deviceAcronym`: Extract the measurement's parent device acronym
                getElementText("DeviceAcronym"),
                # `description`: Extract the measurement description name
                getElementText("Description"),
                # `updatedOn`: Extract the last update time for measurement metadata
                metadataCache.__getUpdatedOn(measurement)
            ))
        
        self.pointIDMeasurementMap: Dict[np.uint64, measurementRecord] = dict()
        
        for measurement in measurementRecords:
            self.pointIDMeasurementMap[measurement.PointID] = measurement

        self.signalIDMeasurementMap: Dict[UUID, measurementRecord] = dict()
        
        for measurement in measurementRecords:
            self.signalIDMeasurementMap[measurement.SignalID] = measurement
        
        self.pointTagMeasurementMap: Dict[str, measurementRecord] = dict()
        
        for measurement in measurementRecords:
            self.pointTagMeasurementMap[measurement.PointTag] = measurement
        
        self.signalRefMeasurementMap: Dict[str, measurementRecord] = dict()
        
        for measurement in measurementRecords:
            self.signalRefMeasurementMap[measurement.SignalReference] = measurement

        self.measurementRecords: List[measurementRecord] = measurementRecords
                
        # Extract device records from DeviceDetail table rows
        deviceRecords: List[deviceRecord] = list()

        for device in metadata.findall("DeviceDetail"):        
            # Get element text or empty string when value is None
            getElementText = lambda elementName: metadataCache.__getElementText(device, elementName)

            deviceRecords.append(deviceRecord(
                # `nodeID`: Extract node ID guid for the device
                metadataCache.__getGuid(device, "NodeID"),
                # `deviceID`: Extract device ID, the unique device guid
                metadataCache.__getGuid(device, "UniqueID"),
                # `acronym`: Alpha-numeric identifier of the device
                getElementText("Acronym"),
                # `name`: Free form name for the device
                getElementText("Name"),
                # `accessID`: Access ID for the device
                metadataCache.__getInt(device, "AccessID"),
                # `parentAcronym`: Alpha-numeric parent identifier of the device
                getElementText("ParentAcronym"),
                # `protocolName`: Protocol name of the device
                getElementText("ProtocolName"),
                # `framesPerSecond`: Data rate for the device
                metadataCache.__getInt(device, "FramesPerSecond"),
                # `companyAcronym`: Company acronym of the device
                getElementText("CompanyAcronym"),
                # `vendorAcronym`: Vendor acronym of the device
                getElementText("VendorAcronym"),
                # `vendorDeviceName`: Vendor device name of the device
                getElementText("VendorDeviceName"),
                # `longitude`: Longitude of the device
                metadataCache.__getDecimal(device, "Longitude"),
                # `latitude`: Latitude of the device
                metadataCache.__getDecimal(device, "Latitude"),
                # `updatedOn`: Extract the last update time for device metadata
                metadataCache.__getUpdatedOn(device)
            ))
        
        self.deviceAcronymDeviceMap: Dict[str, deviceRecord] = dict()

        for device in deviceRecords:
            self.deviceAcronymDeviceMap[device.Acronym] = device

        self.deviceIDDeviceMap: Dict[UUID, deviceRecord] = dict()

        for device in deviceRecords:
            self.deviceIDDeviceMap[device.DeviceID] = device

        self.deviceRecords: List[deviceRecord] = deviceRecords
        
        # Associate measurements with parent devices
        for measurement in measurementRecords:
            device = self.LookupDeviceByAcronym(measurement.DeviceAcronym)

            if device is not None:
                measurement.Device = device
                device.Measurements.add(measurement)

        # Extract phasor records from PhasorDetail table rows
        phasorRecords: List[phasorRecord] = list()

        for phasor in metadata.findall("PhasorDetail"):        
            # Get element text or empty string when value is None
            getElementText = lambda elementName: metadataCache.__getElementText(phasor, elementName)
            
            phasorRecords.append(phasorRecord(
                # `id`: unique integer identifier for phasor
                metadataCache.__getInt(phasor, "ID"),
                # `deviceAcronym`: Alpha-numeric identifier of the associated device
                getElementText("DeviceAcronym"),
                # `label`: Free form label for the phasor
                getElementText("Label"),
                # `type`: Phasor type for the phasor
                metadataCache.__getChar(phasor, "Type"),
                # `phase`: Phasor phase for the phasor
                metadataCache.__getChar(phasor, "Phase"),
                # `sourceIndex`: Source index for the phasor
                metadataCache.__getInt(phasor, "SourceIndex"),
                # `baseKV`: BaseKV level for the phasor
                metadataCache.__getInt(phasor, "BaseKV"),
                # `updatedOn`: Extract the last update time for phasor metadata
                metadataCache.__getUpdatedOn(phasor)
            ))
            
        # Associate phasors with parent device and associated angle/magnitude measurements
        for phasor in phasorRecords:
            device = self.LookupDeviceByAcronym(phasor.DeviceAcronym)

            if device is not None:
                phasor.Device = device
                device.Phasors.add(phasor)
                
                angle = self.LookupMeasurementBySignalReference(f"{device.Acronym}-PA{phasor.SourceIndex}")
                magnitude = self.LookupMeasurementBySignalReference(f"{device.Acronym}-PM{phasor.SourceIndex}")

                if angle is not None and magnitude is not None:
                    phasor.Measurements.clear()

                    angle.Phasor = phasor
                    phasor.Measurements.append(angle) # Must be index 0

                    magnitude.Phasor = phasor
                    phasor.Measurements.append(magnitude) # Must be index 1

        self.phasorRecords: List[phasorRecord] = phasorRecords

    @staticmethod
    def __getElementText(elementRoot, elementName: str):
        element = elementRoot.find(elementName)
        return "" if element is None else "" if element.text is None else element.text.strip()
    
    @staticmethod
    def __getMeasurementKey(elementRoot) -> (str, np.uint64):
        elementText = metadataCache.__getElementText(elementRoot, "ID")
        defaultValue = ("_", np.uint64(0))

        try: 
            parts = elementText.split(":")

            if len(parts) != 2:
                return defaultValue

            return (parts[0], np.uint64(parts[1]))
        except:
            return defaultValue

    @staticmethod
    def __getGuid(elementRoot, elementName: str) -> UUID:
        elementText = metadataCache.__getElementText(elementRoot, elementName)
        defaultValue = uuid1()

        if elementText == "":
            return defaultValue

        try:
            return UUID(elementText)
        except:
            return defaultValue

    @staticmethod
    def __getInt(elementRoot, elementName: str) -> int:
        elementText = metadataCache.__getElementText(elementRoot, elementName)
        defaultValue = 0

        if elementText == "":
            return defaultValue

        try:
            return int(elementText)
        except:
            return defaultValue

    @staticmethod
    def __getDecimal(elementRoot, elementName: str) -> Decimal:
        elementText = metadataCache.__getElementText(elementRoot, elementName)
        defaultValue = Empty.DECIMAL

        if elementText == "":
            return defaultValue

        try:
            return Decimal(elementText)
        except:
            return defaultValue

    @staticmethod
    def __getChar(elementRoot, elementName: str) -> str:
        elementText = metadataCache.__getElementText(elementRoot, elementName)
        defaultValue = " "

        if elementText == "":
            return defaultValue

        try:
            return elementText[0]
        except:
            return defaultValue

    @staticmethod
    def __getUpdatedOn(elementRoot) -> datetime:
        elementText = metadataCache.__getElementText(elementRoot, "UpdatedOn")
        defaultValue = datetime.utcnow()

        if elementText == "":
            return defaultValue

        try:
            # Interestingly the Python `datetime.fromisoformat` will only
            # parse fractional seconds with 3 or 6 digits. Since in STTP
            # metadata fractional seconds often just have 2 digits, we
            # have to work much harder to make this parse properly.
            timeZone = None

            if ":" in elementText:
                tzParts = elementText.split("-")
                count = len(tzParts)

                elementText = "-".join(tzParts[:count - 1])
                timeZone = tzParts[count - 1]

            fsParts = elementText.split(".")

            if len(fsParts) == 1:
                return datetime.fromisoformat(elementText)

            dateTimePart = fsParts[0]
            fracSecPart = fsParts[1]

            if len(fracSecPart) == 3 or len(fracSecPart) == 6:
                return datetime.fromisoformat(elementText)

            elementText = f"{dateTimePart}.{fracSecPart.ljust(3, '0')}"

            if timeZone is not None:
                elementText = f"{elementText}-{timeZone}"

            return datetime.fromisoformat(elementText)
        except:
            return defaultValue

    @property
    def MeasurementRecords(self) -> List[measurementRecord]:
        return self.measurementRecords

    @property
    def DeviceRecords(self) -> List[deviceRecord]:
        return self.deviceRecords

    @property
    def PhasorRecords(self) -> List[phasorRecord]:
        return self.phasorRecords

    def LookupMeasurementByPointID(self, pointID: np.uint64) -> Optional[measurementRecord]:
        if pointID in self.pointIDMeasurementMap:
            return self.pointIDMeasurementMap[pointID]

        return None

    def LookupMeasurementBySignalID(self, signalID: UUID) -> Optional[measurementRecord]:
        if signalID in self.signalIDMeasurementMap:
            return self.signalIDMeasurementMap[signalID]

        return None

    def LookupMeasurementByPointTag(self, pointTag: str) -> Optional[measurementRecord]:
        if pointTag in self.pointTagMeasurementMap:
            return self.pointTagMeasurementMap[pointTag]

        return None

    def LookupMeasurementBySignalReference(self, signalReference: str) -> Optional[measurementRecord]:
        if signalReference in self.signalRefMeasurementMap:
            return self.signalRefMeasurementMap[signalReference]

        return None

    def GetMeasurementsBySignalType(self, signalType: SignalType, instanceName: Optional[str] = None) -> List[measurementRecord]:
        matchedRecords: List[measurementRecord] = list()

        signalTypeName = str(signalType)

        #                             012345678901
        if signalTypeName.startswith("SignalType."):
            signalTypeName = signalTypeName[11:]

        for record in self.measurementRecords:
            if record.signalTypeName == signalTypeName:
                if instanceName is None or record.InstanceName == instanceName:
                    matchedRecords.append(record)

        return matchedRecords

    def GetMeasurementsByTextSearch(self, searchVal: str, instanceName: Optional[str] = None) -> List[measurementRecord]:
        records = set()

        if searchVal in self.pointTagMeasurementMap:
            record = self.pointTagMeasurementMap[searchVal]
            
            if instanceName is None or record.InstanceName == instanceName:
                records.add(record)

        if searchVal in self.signalRefMeasurementMap:
            record = self.signalRefMeasurementMap[searchVal]
            
            if instanceName is None or record.InstanceName == instanceName:
                records.add(record)

        for record in self.measurementRecords:
            if searchVal in record.Description or searchVal in record.DeviceName:
                if instanceName is None or record.InstanceName == instanceName:
                    records.add(record)

        return list(records)
    
    @staticmethod
    def ToPointIDList(records: List[measurementRecord]) -> List[np.uint64]:
        pointIDs = set()

        for record in records:
            pointIDs.add(record.PointID)

        return list(pointIDs)

    def LookupDeviceByAcronym(self, deviceAcronym: UUID) -> Optional[deviceRecord]:
        if deviceAcronym in self.deviceAcronymDeviceMap:
            return self.deviceAcronymDeviceMap[deviceAcronym]

        return None

    def LookupDeviceByID(self, deviceID: UUID) -> Optional[deviceRecord]:
        if deviceID in self.deviceIDDeviceMap:
            return self.deviceIDDeviceMap[deviceID]

        return None

    def GetDevicesByTextSearch(self, searchVal: str, instanceName: Optional[str] = None) -> List[deviceRecord]:
        records = set()

        if searchVal in self.deviceAcronymDeviceMap:
            records.add(self.deviceAcronymDeviceMap[searchVal])

        for record in self.deviceRecords:
            if (searchVal in record.Acronym or 
                searchVal in record.Name or 
                searchVal in record.ParentAcronym or
                searchVal in record.CompanyAcronym or 
                searchVal in record.VendorAcronym or 
                searchVal in record.VendorDeviceName):
                if instanceName is None or record.InstanceName == instanceName:
                    records.add(record)

        return list(records)
