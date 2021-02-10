#******************************************************************************************************
#  phasorRecord.py - Gbtc
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
from typing import Optional, List
from enum import IntEnum
from datetime import datetime

class CompositePhasorMeasurement(IntEnum):
    ANGLE = 0
    MAGNITUDE = 1

class phasorRecord:
    """
    Represents a record of phasor metadata in the openHistorian.
    """

    def __init__(self,
            id: int,
            deviceAcronym: str,
            label: str,
            type: str,
            phase: str,
            sourceIndex: int,
            baseKV: int = 500,
            updatedOn: datetime = Empty.DATETIME
        ):
        """
        Constructs a new `phasorRecord`.
        """
        self.id = id
        self.deviceAcronym = deviceAcronym
        self.label = label
        self.type = type
        self.phase = phase
        self.sourceIndex = sourceIndex
        self.baseKV = baseKV
        self.updatedOn = updatedOn
        self.device: Optional["deviceRecord"] = None
        self.measurements: List["measurementRecord"] = list()

    @property
    def ID(self) -> int: # <PhasorDetail>/<ID>
        """
        Gets the unique integer identifier for this `phasorRecord`.
        """
        return self.id

    @property
    def DeviceAcronym(self) -> str: # <PhasorDetail>/<DeviceAcronym>
        """
        Gets the alpha-numeric identifier of the associated device for this `phasorRecord`.
        """
        return self.deviceAcronym

    @property
    def Label(self) -> str: # <PhasorDetail>/<Label>
        """
        Gets the free form label for this `phasorRecord`.
        """
        return self.label

    @property
    def Type(self) -> str: # <PhasorDetail>/<Type>
        """
        Gets the phasor type, i.e., "I" or "V", for current or voltage, respectively, for this `phasorRecord`. 
        """
        return self.type

    @property
    def Phase(self) -> str: # <PhasorDetail>/<Phase>
        """
        Gets the phase of this `phasorRecord`, e.g., "A", "B", "C", "+", "-", "0", etc.
        """
        return self.phase

    @property
    def BaseKV(self) -> int:  # <PhasorDetail>/<BaseKV>
        """
        Gets the base, i.e., nominal, kV level for this `phasorRecord`.
        """
        return self.sourceIndex

    @property
    def SourceIndex(self) -> int:  # <PhasorDetail>/<SourceIndex>
        """
        Gets the source index, i.e., the 1-based ordering index of the phasor in its original context, for this `phasorRecord`.
        """
        return self.sourceIndex

    @property
    def UpdatedOn(self) -> datetime: # <PhasorDetail>/<UpdatedOn>
        """
        Gets the `datetime` of when this `phasorRecord` was last updated.
        """
        return self.updatedOn

    @property
    def Device(self) -> Optional["deviceRecord"]:
        """
        Gets the associated `deviceRecord` for this `phasorRecord`.
        """
        return self.device

    @Device.setter
    def Device(self, value: Optional["deviceRecord"]):
        """
        Sets the associated `deviceRecord` for this `phasorRecord`.
        """
        self.device = value

    @property
    def Measurements(self) -> List["measurementRecord"]:
        """
        Gets the two `measurementRecord` values, i.e., the angle and magnitude, associated with this `phasorRecord`.
        """
        return self.measurements

    @property
    def AngleMeasurement(self) -> Optional["measurementRecord"]:
        """
        Gets the associated angle `measurementRecord`, or `None` if not available.
        """
        return None if len(self.measurements) <= CompositePhasorMeasurement.ANGLE else \
           self.measurements[CompositePhasorMeasurement.ANGLE]

    @property
    def MagnitudeMeasurement(self) -> Optional["measurementRecord"]:
        """
        Gets the associated magnitude `measurementRecord`, or `None` if not available.
        """
        return None if len(self.measurements) <= CompositePhasorMeasurement.MAGNITUDE else \
            self.measurements[CompositePhasorMeasurement.MAGNITUDE]
