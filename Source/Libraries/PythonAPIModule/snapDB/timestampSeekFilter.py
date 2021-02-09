#******************************************************************************************************
#  timestampSeekFilter.py - Gbtc
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
#  02/06/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from .seekFilterBase import seekFilterBase
from gsf.binaryStream import binaryStream
from gsf import Ticks
from typing import Optional
from datetime import datetime, timedelta
from uuid import UUID
import numpy as np

class timestampSeekFilter(seekFilterBase):
    """
    Creates a seek filter based on time ranges and optional intervals.
    """

    SnapTypeID = UUID("0f0f9478-dc42-4eef-9f26-231a942ef1fa")
    AllTimes: Optional[seekFilterBase] = None

    def __init__(self, firstTime: np.uint64, lastTime: np.uint64, mainInterval: np.uint64 = 0, subInterval: np.uint64 = 0, tolerance: np.uint64 = 0):
        """
        Creates a seek filter over a single date range (inclusive list).
        
        Parameters
        ----------
        firstTime: the first time if the query (inclusive).
        lastTime: the last time of the query (inclusive).
        mainInterval: the smallest interval that is exact.
        subInterval: the interval that will be parsed (round possible).
        tolerance: the width of every window.
        """
        self.firstTime = firstTime
        self.lastTime = lastTime
        self.mainInterval = mainInterval
        self.subInterval = subInterval
        self.tolerance = tolerance

    @property
    def TypeID(self) -> UUID:
        """
        Gets the Guid uniquely defining this SNAPdb seek filter type.
        """
        return timestampSeekFilter.SnapTypeID

    def Save(self, stream: binaryStream):
        """
        Serializes the filter to a stream.
        """
        targetFixedRange = self.mainInterval == 0 and self.subInterval == 0 and self.tolerance == 0

        stream.WriteByte(1 if targetFixedRange else 2)
        stream.WriteUInt64(self.firstTime)
        stream.WriteUInt64(self.lastTime)

        if targetFixedRange:
            return

        stream.WriteUInt64(self.mainInterval)
        stream.WriteUInt64(self.subInterval)
        stream.WriteUInt64(self.tolerance)

    @staticmethod
    def CreateFromRange(firstTime: datetime, lastTime: datetime):
        """
        Creates a seek filter over a single date range (inclusive list).
        
        Parameters
        ----------
        firstTime: the first time if the query (inclusive).
        lastTime: the last time of the query (inclusive).
        """
        return timestampSeekFilter(
            Ticks.FromDateTime(firstTime),
            Ticks.FromDateTime(lastTime))

    @staticmethod
    def CreateFromRangeInterval(firstTime: datetime, lastTime: datetime, interval: timedelta,  tolerance: timedelta):
        """
        Creates a seek filter over a single date range (inclusive list), skipping values
        based on specified `timedelta` interval and tolerance.
        
        Parameters
        ----------
        firstTime: the first time if the query (inclusive).
        lastTime: the last time of the query (inclusive).
        interval: the exact interval for the scan. Example: 0.1 seconds.
        tolerance: the width of every window. Example: 0.001 seconds.
        """
        return timestampSeekFilter(
            Ticks.FromDateTime(firstTime),
            Ticks.FromDateTime(lastTime),
            Ticks.FromTimeDelta(interval),
            Ticks.FromTimeDelta(interval),
            Ticks.FromTimeDelta(tolerance))

    @staticmethod
    def CreateFromRangeSubInterval(firstTime: datetime, lastTime: datetime, mainInterval: timedelta, subInterval: timedelta, tolerance: timedelta):
        """
        Creates a seek filter over a single date range (inclusive list), skipping values
        based on specified `timedelta` main /sub intervals and tolerance.
        
        Parameters
        ----------
        firstTime: the first time if the query (inclusive).
        lastTime: the last time of the query (inclusive).
        mainInterval: the smallest interval that is exact. Example: 0.1 seconds.
        subInterval: the interval that will be parsed (round possible). Example: 0.0333333 seconds.
        tolerance: the width of every window. Example: 0.001 seconds.
        """
        return timestampSeekFilter(
            Ticks.FromDateTime(firstTime),
            Ticks.FromDateTime(lastTime),
            Ticks.FromTimeDelta(mainInterval),
            Ticks.FromTimeDelta(subInterval),
            Ticks.FromTimeDelta(tolerance))
