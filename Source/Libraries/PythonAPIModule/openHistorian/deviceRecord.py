#******************************************************************************************************
#  deviceRecord.py - Gbtc
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
#  02/09/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from gsf import Empty
from typing import Set
from datetime import datetime
from uuid import UUID
from decimal import Decimal

class deviceRecord:
    """
    Represents a record of device metadata in the openHistorian.
    """

    def __init__(self,
            nodeID: UUID,
            deviceID: UUID,
            acronym: str,
            name: str,
            accessID: int,
            parentAcronym: str ="",
            protocolName: str = "",
            framesPerSecond: int = 30,
            companyAcronym: str = "",
            vendorAcronym: str = "",
            vendorDeviceName: str = "",
            longitude: Decimal = Empty.DECIMAL,
            latitude: Decimal = Empty.DECIMAL,
            updatedOn: datetime = Empty.DATETIME
        ):
        """
        Constructs a new `deviceRecord`.
        """
        self.nodeID = nodeID
        self.deviceID = deviceID
        self.acronym = acronym
        self.name = name
        self.accessID = accessID
        self.parentAcronym = parentAcronym
        self.protocolName = protocolName
        self.framesPerSecond = framesPerSecond
        self.companyAcronym = companyAcronym
        self.vendorAcronym = vendorAcronym
        self.vendorDeviceName = vendorDeviceName
        self.longitude = longitude
        self.latitude = latitude
        self.updatedOn = updatedOn
        self.measurements: Set["measurementRecord"] = set()
        self.phasors: Set["phasorRecord"] = set()

    @property
    def NodeID(self) -> UUID: # <DeviceDetail>/<NodeID>
        """
        Gets the guid-based openHistorian node identifier for this `deviceRecord`.
        """
        return self.nodeID

    @property
    def DeviceID(self) -> UUID: # <DeviceDetail>/<UniqueID>
        """
        Gets the unique guid-based identifier for this `deviceRecord`.
        """
        return self.deviceID

    @property
    def Acronym(self) -> str: # <DeviceDetail>/<Acronym>
        """
        Gets the unique alpha-numeric identifier for this `deviceRecord`.
        """
        return self.acronym

    @property
    def Name(self) -> str: # <DeviceDetail>/<Name>
        """
        Gets the free form name of this `deviceRecord`.
        """
        return self.name

    @property
    def AccessID(self) -> int: # <DeviceDetail>/<AccessID>
        """
        Gets the access ID (a.k.a. ID code) for this `deviceRecord`.
        """
        return self.accessID

    @property
    def ParentAcronym(self) -> str: # <DeviceDetail>/<ParentAcronym>
        """
        Gets the parent device alpha-numeric identifier for this `deviceRecord`, if any.
        """
        return self.parentAcronym

    @property
    def ProtocolName(self) -> str: # <DeviceDetail>/<ProtocolName>
        """
        Gets the name of the source protocol for this `deviceRecord`.
        """
        return self.protocolName

    @property
    def FramesPerSecond(self) -> int: # <DeviceDetail>/<FramesPerSecond>
        """
        Gets the data reporting rate, in data frames per second, for this `deviceRecord`.
        """
        return self.framesPerSecond

    @property
    def CompanyAcronym(self) -> str: # <DeviceDetail>/<CompanyAcronym>
        """
        Gets the acronym of the company associated with this `deviceRecord`.
        """
        return self.companyAcronym

    @property
    def VendorAcronym(self) -> str: # <DeviceDetail>/<VendorAcronym>
        """
        Gets the acronym of the vendor associated with this `deviceRecord`.
        """
        return self.vendorAcronym
        
    @property
    def VendorDeviceName(self) -> str: # <DeviceDetail>/<VendorDeviceName>
        """
        Gets the acronym of the vendor device name associated with this `deviceRecord`.
        """
        return self.vendorDeviceName
        
    @property
    def Longitude(self) -> Decimal: # <DeviceDetail>/<Longitude>
        """
        Gets the longitude of this `deviceRecord`.
        """
        return self.longitude

    @property 
    def Latitude(self) -> Decimal: # <DeviceDetail>/<Latitude>
        """
        Gets the latitude of this `deviceRecord`.
        """
        return self.latitude

    @property
    def UpdatedOn(self) -> datetime: # <DeviceDetail>/<UpdatedOn>
        """
        Gets the `datetime` of when this `deviceRecord` was last updated.
        """
        return self.updatedOn

    @property
    def Measurements(self) -> Set["measurementRecord"]:
        """
        Gets `measurementRecord` values associated with this `deviceRecord`.
        """
        return self.measurements

    @property
    def Phasors(self) -> Set["phasorRecord"]:
        """
        Gets `phasorRecord` values associated with this `deviceRecord`.
        """
        return self.phasors