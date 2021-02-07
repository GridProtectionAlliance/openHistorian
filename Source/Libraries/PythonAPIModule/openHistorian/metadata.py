#******************************************************************************************************
#  metadata.py - Gbtc
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
import xml.etree.ElementTree as xmlParser
from typing import Optional, List, Dict, Set
from datetime import datetime
from uuid import UUID
import numpy as np

class metadata:
    """
    Represents a collection of openHistorian metadata.
    """
    def __init__(self, metadataXML: str):
        # Parse metadata
        metadata = xmlParser.fromstring(metadataXML)
                
        # Extra metadata records from MeasurementDetail table rows
        records: List[metadataRecord] = list()

        for measurement in metadata.findall("MeasurementDetail"):
            records.append(metadataRecord(
                # `pointID`: Parse openHistorian point ID from measurement key
                np.uint64(measurement.find("ID").text.split(":")[1]),
                # `signalID`: Extract signal ID, the unique measurement guid
                UUID(measurement.find("SignalID").text),
                # `pointTag`: Extract the measurement point tag
                measurement.find("PointTag").text,
                # `signalReference`: Extract the measurement signal reference
                measurement.find("SignalReference").text,
                # `signalTypeName`: Extract the measurement signal type name
                measurement.find("SignalAcronym").text,
                # `deviceName`: Extract the measurement's parent device name
                measurement.find("DeviceAcronym").text,
                # `description`: Extract the measurement description name
                measurement.find("Description").text,
                # `updatedOn`: Extract the last update time for measurement metadata
                datetime.fromisoformat(measurement.find("UpdatedOn").text)
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

    @property
    def Records(self) -> List[metadataRecord]:
        return self.records

    def LookupPointID(self, pointID: np.uint64) -> Optional[metadataRecord]:
        if pointID in self.pointIDMetaMap:
            return self.pointIDMetaMap[pointID]

        return None

    def LookupSignalID(self, signalID: UUID) -> Optional[metadataRecord]:
        if signalID in self.signalIDMetaMap:
            return self.signalIDMetaMap[signalID]

        return None

    def LookupPointTag(self, pointTag: str) -> Optional[metadataRecord]:
        if pointTag in self.pointTagMetaMap:
            return self.pointTagMetaMap[pointTag]

        return None

    def LookupSignalReference(self, signalReference: str) -> Optional[metadataRecord]:
        if signalReference in self.signalRefMetaMap:
            return self.signalRefMetaMap[signalReference]

        return None

    def MatchSignalType(self, signalType: SignalType) -> List[metadataRecord]:
        matchedRecords: List[metadataRecord] = list()

        signalTypeName = str(signalType)

        for record in self.records:
            if record.signalTypeName == signalTypeName:
                matchedRecords.append(record)

        return matchedRecords

    def TextSearch(self, searchVal: str) -> List[metadataRecord]:
        records = set()

        if searchVal in self.pointTagMetaMap:
            records.add(self.pointTagMetaMap[searchVal])

        if searchVal in self.signalRefMetaMap:
            records.add(self.signalRefMetaMap[searchVal])

        for record in self.records:
            if searchVal in record.Description or searchVal in record.DeviceName:
                records.add(record)

        return list(records)
