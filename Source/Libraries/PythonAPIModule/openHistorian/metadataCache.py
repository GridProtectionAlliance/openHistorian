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

from openHistorian.metadataRecord import metadataRecord, SignalType
from gsf import Empty
import xml.etree.ElementTree as xmlParser
from typing import Optional, List, Dict, Set
from datetime import datetime
from uuid import UUID, uuid1
import numpy as np

class metadataCache:
    """
    Represents a collection of openHistorian metadata.
    """
    def __init__(self, metadataXML: str):
        # Parse metadata
        metadata = xmlParser.fromstring(metadataXML)
                
        # Extra metadata records from MeasurementDetail table rows
        records: List[metadataRecord] = list()

        for measurement in metadata.findall("MeasurementDetail"):        
            # Get element text or empty string when value is None
            getElementText = lambda elementName: metadataCache.__getElementText(measurement, elementName)

            # Parse openHistorian instance name and point ID from measurement key
            (instanceName, pointID) = metadataCache.__getMeasurementKey(measurement)
            
            if pointID == 0:
                continue

            records.append(metadataRecord(
                # `instanceName`: Source instance name of measurement
                instanceName,
                # `pointID`: openHistorian point ID of measurement
                pointID,
                # `signalID`: Extract signal ID, the unique measurement guid
                metadataCache.__getSignalID(measurement),
                # `pointTag`: Extract the measurement point tag
                getElementText("PointTag"),
                # `signalReference`: Extract the measurement signal reference
                getElementText("SignalReference"),
                # `signalTypeName`: Extract the measurement signal type name
                getElementText("SignalAcronym"),
                # `deviceName`: Extract the measurement's parent device name
                getElementText("DeviceAcronym"),
                # `description`: Extract the measurement description name
                getElementText("Description"),
                # `updatedOn`: Extract the last update time for measurement metadata
                metadataCache.__getUpdatedOn(measurement)
            ))
        
        self.pointIDMetaMap: Dict[np.uint64, metadataRecord] = dict()
        
        for record in records:
            self.pointIDMetaMap[record.PointID] = record

        self.signalIDMetaMap: Dict[UUID, metadataRecord] = dict()
        
        for record in records:
            self.signalIDMetaMap[record.SignalID] = record
        
        self.pointTagMetaMap: Dict[str, metadataRecord] = dict()
        
        for record in records:
            self.pointTagMetaMap[record.PointTag] = record
        
        self.signalRefMetaMap: Dict[str, metadataRecord] = dict()
        
        for record in records:
            self.signalRefMetaMap[record.SignalReference] = record

        self.records: List[metadataRecord] = records
    
    @staticmethod
    def __getElementText(elementRoot, elementName: str):
        element = elementRoot.find(elementName)
        return "" if element is None else element.text
    
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
    def __getSignalID(elementRoot) -> UUID:
        elementText = metadataCache.__getElementText(elementRoot, "SignalID")
        defaultValue = uuid1()

        if elementText == "":
            return defaultValue

        try:
            return UUID(elementText)
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

            elementText = dateTimePart + "." + fracSecPart.ljust(3, "0")

            if timeZone is not None:
                elementText = elementText + "-" + timeZone

            return datetime.fromisoformat(elementText)
        except:
            return defaultValue

    @property
    def Records(self) -> List[metadataRecord]:
        return self.records

    def LookupByPointID(self, pointID: np.uint64) -> Optional[metadataRecord]:
        if pointID in self.pointIDMetaMap:
            return self.pointIDMetaMap[pointID]

        return None

    def LookupBySignalID(self, signalID: UUID) -> Optional[metadataRecord]:
        if signalID in self.signalIDMetaMap:
            return self.signalIDMetaMap[signalID]

        return None

    def LookupByPointTag(self, pointTag: str) -> Optional[metadataRecord]:
        if pointTag in self.pointTagMetaMap:
            return self.pointTagMetaMap[pointTag]

        return None

    def LookupBySignalReference(self, signalReference: str) -> Optional[metadataRecord]:
        if signalReference in self.signalRefMetaMap:
            return self.signalRefMetaMap[signalReference]

        return None

    def MatchSignalType(self, signalType: SignalType, instanceName: Optional[str] = None) -> List[metadataRecord]:
        matchedRecords: List[metadataRecord] = list()

        signalTypeName = str(signalType)

        #                             12345678901
        if signalTypeName.startswith("SignalType."):
            signalTypeName = signalTypeName[11:]

        for record in self.records:
            if record.signalTypeName == signalTypeName:
                if instanceName is None or record.InstanceName == instanceName:
                    matchedRecords.append(record)

        return matchedRecords

    def TextSearch(self, searchVal: str, instanceName: Optional[str] = None) -> List[metadataRecord]:
        records = set()

        if searchVal in self.pointTagMetaMap:
            records.add(self.pointTagMetaMap[searchVal])

        if searchVal in self.signalRefMetaMap:
            records.add(self.signalRefMetaMap[searchVal])

        for record in self.records:
            if searchVal in record.Description or searchVal in record.DeviceName:
                if instanceName is None or record.InstanceName == instanceName:
                    records.add(record)

        return list(records)
    
    @staticmethod
    def ToPointIDList(records: List[metadataRecord]) -> List[np.uint64]:
        pointIDs = list()

        for record in records:
            pointIDs.append(record.PointID)

        return pointIDs
