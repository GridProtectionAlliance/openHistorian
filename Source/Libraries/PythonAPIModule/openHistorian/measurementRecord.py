#******************************************************************************************************
#  measurementRecord.py - Gbtc
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

from .deviceRecord import deviceRecord
from .phasorRecord import phasorRecord
from gsf import Empty
from typing import Optional
from enum import IntEnum
from datetime import datetime
from uuid import UUID
import numpy as np

class SignalType(IntEnum):
    """
    Represents common signal types for openHistorian metadata. This list may
    not be exhaustive for some openHistorian deployments. If value is set to
    `UNKN`, check the string based `SignalTypeName` in the `measurementRecord`.
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
  
    @classmethod
    def Parse(cls, name):
        return getattr(cls, name.upper(), None)

class measurementRecord:
    """
    Represents a record of measurement metadata in the openHistorian.
    """

    def __init__(self,
            instanceName: str,
            pointID: np.uint64,
            signalID: UUID,
            pointTag: str,
            signalReference: str = "",
            signalTypeName: str = "UNKN",
            deviceAcronym: str = "",
            description: str = "",
            updatedOn: datetime = Empty.DATETIME
        ):
        """
        Constructs a new `measurementRecord`.
        """
        self.instanceName = instanceName
        self.pointID = pointID
        self.signalID = signalID
        self.pointTag = pointTag
        self.signalReference = signalReference
        self.signalTypeName = signalTypeName

        try:
            self.signalType = SignalType.Parse(self.signalTypeName)
        except:
            self.signalType = SignalType.UNKN

        self.deviceAcronym = deviceAcronym
        self.description = description
        self.updatedOn = updatedOn
        self.device: Optional[deviceRecord] = None
        self.phasor: Optional[phasorRecord] = None

    @property
    def InstanceName(self) -> str: # <MeasurementDetail>/<ID> (left part of measurement key)
        """
        Gets the openHistorian client database instance for this `measurementRecord`.
        """
        return self.instanceName

    @property
    def PointID(self) -> np.uint64: # <MeasurementDetail>/<ID> (right part of measurement key)
        """
        Gets the openHistorian point ID for this `measurementRecord`.
        """
        return self.pointID

    @property
    def SignalID(self) -> UUID: # <MeasurementDetail>/<SignalID>
        """
        Gets the unique guid-based signal identifier for this `measurementRecord`.
        """
        return self.signalID

    @property
    def PointTag(self) -> str: # <MeasurementDetail>/<PointTag>
        """
        Gets the unique point tag for this `measurementRecord`.
        """
        return self.pointTag

    @property
    def SignalReference(self) -> str: # <MeasurementDetail>/<SignalReference>
        """
        Gets the unique signal reference for this `measurementRecord`.
        """
        return self.signalReference

    @property
    def SignalTypeName(self) -> str: # <MeasurementDetail>/<SignalAcronym>
        """
        Gets the signal type name for this `measurementRecord`.
        """
        return self.signalTypeName

    @property
    def AsSignalType(self)-> SignalType:
        """
        Gets the `SignalType` enumeration for this `measurementRecord`, if it can be mapped
        to `SignalTypeName`; otherwise, returns `SignalType.UNKN`.
        """
        return self.signalType

    @property
    def DeviceAcronym(self) -> str: # <MeasurementDetail>/<DeviceAcronym>
        """
        Gets the alpha-numeric identifier of the associated device for this `measurementRecord`.
        """
        return self.deviceAcronym

    @property
    def Description(self) -> str: # <MeasurementDetail>/<Description>
        """
        Gets the description for this `measurementRecord`.
        """
        return self.description

    @property
    def UpdatedOn(self) -> datetime: # <MeasurementDetail>/<UpdatedOn>
        """
        Gets the `datetime` of when this `measurementRecord` was last updated.
        """
        return self.updatedOn

    @property
    def Device(self) -> Optional[deviceRecord]:
        """
        Gets the associated `deviceRecord` for this `measurementRecord`.
        """
        return self.device

    @Device.setter
    def Device(self, value: Optional[deviceRecord]):
        """
        Sets the associated `deviceRecord` for this `measurementRecord`.
        """
        self.device = value

    @property
    def Phasor(self) -> Optional[phasorRecord]:
        """
        Gets the associated `phasorRecord` for this `measurementRecord`,
        or `None` if not applicable.
        """
        return self.phasor

    @Phasor.setter
    def Phasor(self, value: Optional[phasorRecord]):
        """
        Sets the associated `phasorRecord` for this `measurementRecord`.
        """
        self.phasor = value