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

from enum import IntEnum
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

    def __init__(self, pointID: np.uint64, signalID: UUID, signalTypeName: str = "UNKN", deviceName: str = "", description: str = ""):
        self.pointID = pointID
        self.signalID = signalID
        self.signalTypeName = signalTypeName
        self.deviceName = deviceName
        self.description = description

    @property
    def PointID(self) -> np.uint64:
        return self.pointID

    @property
    def SignalID(self) -> UUID:
        return self.signalID

    @property
    def SignalTypeName(self) -> str:
        return self.signalTypeName

    @property
    def AsSignalType(self)-> SignalType:
        try:
            return SignalType(self.signalTypeName)
        except:
            return SignalType.UNKN

    @property
    def DeviceName(self) -> str:
        return self.deviceName

    @property
    def Description(self) -> str:
        return self.description




