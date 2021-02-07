#******************************************************************************************************
#  metadataRecord.py - Gbtc
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

from gsf import Empty
from enum import IntEnum
from datetime import datetime
from uuid import UUID
import numpy as np

class SignalType(IntEnum):
    """
    Represents common signal types for openHistorian metadata. This list may
    not be exhaustive for some openHistorian deployments. If value is set to
    `UNKN`, check the string based `SignalTypeName` in the `metadataRecord`.
    """
    IPHM = 1    # Current phase magnitude
    IPHA = 2    # Current phase angle
    VPHM = 3    # Voltage phase magnitude
    VPHA = 4    # Voltage phase angle
    FREQ = 5    # Frequency
    DFDT = 6    # Frequency derivative, i.e., Δfreq / Δtime
    ALOG = 7    # Analog value (scalar)
    FLAG = 8    # Status flags (16-bit)
    DIGI = 9    # Digital value (16-bit)
    CALC = 10   # Calculated value
    STAT = 11   # Statistic value
    ALRM = 12   # Alarm state
    QUAL = 13   # Quality flags (16-bit)
    UNKN = -1   # Unknown type, see `SignalTypeName`

class metadataRecord:
    """
    Represents a record of measurement metadata the openHistorian.
    """

    def __init__(self, 
            pointID: np.uint64,
            signalID: UUID,
            pointTag: str,
            signalReference: str = "",
            signalTypeName: str = "UNKN",
            deviceName: str = "",
            description: str = "",
            updatedOn: datetime = Empty.DATETIME
        ):
        """
        Constructs a new `metadataRecord`.
        """
        self.pointID = pointID
        self.signalID = signalID
        self.pointTag = pointTag
        self.signalReference = signalReference
        self.signalTypeName = signalTypeName
        self.deviceName = deviceName
        self.description = description
        self.updatedOn = updatedOn

    @property
    def PointID(self) -> np.uint64:
        """
        Gets the openHistorian point ID for this `metadataRecord`.
        """
        return self.pointID

    @property
    def SignalID(self) -> UUID:
        """
        Gets the unique guid-based signal identifier for this `metadataRecord`.
        """
        return self.signalID

    @property
    def PointTag(self) -> str:
        """
        Gets the unique point tag for this `metadataRecord`.
        """
        return self.pointTag

    @property
    def SignalReference(self) -> str:
        """
        Gets the unique signal reference for this `metadataRecord`.
        """
        return self.signalReference

    @property
    def SignalTypeName(self) -> str:
        """
        Gets the signal type name for this `metadataRecord`.
        """
        return self.signalTypeName

    @property
    def AsSignalType(self)-> SignalType:
        """
        Gets the `SignalType` enumeration for this `metadataRecord`, if it can be mapped
        to `SignalTypeName`; otherwise, returns `SignalType.UNKN`.
        """
        try:
            return SignalType(self.signalTypeName)
        except:
            return SignalType.UNKN

    @property
    def DeviceName(self) -> str:
        """
        Gets the name of the associated device for this `metadataRecord`.
        """
        return self.deviceName

    @property
    def Description(self) -> str:
        """
        Gets the description for this `metadataRecord`.
        """
        return self.description

    @property
    def UpdatedOn(self) -> datetime:
        """
        Gets the `datetime` of when this `metadataRecord` was last updated.
        """
        return self.updatedOn
