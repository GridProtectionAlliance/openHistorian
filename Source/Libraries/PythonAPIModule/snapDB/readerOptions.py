#******************************************************************************************************
#  readerOptions.py - Gbtc
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

from gsf.binaryStream import binaryStream
from gsf import Ticks
from datetime import timedelta

class readerOptions:
    """
    Contains the options to use for executing an individual read request.
    """
    
    # Source C# reference: SortedTreeEngineReaderOptions

    def __init__(self, timeout: timedelta = timedelta(), maxReturnedCount: int = 0, maxScanCount: int = 0, maxSeekCount: int = 0):
        self.timeout = timeout
        self.maxReturnedCount = maxReturnedCount
        self.maxScanCount = maxScanCount
        self.maxSeekCount = maxSeekCount

    @property
    def Timeout(self) -> timedelta:
        """
        The time before the query times out.
        """
        return self.timeout

    @property
    def MaxReturnedCount(self) -> int:
        """
        The maximum number of points to return. 0 means no limit.
        """
        return self.maxReturnedCount

    @property
    def MaxScanCount(self) -> int:
        """
        The maximum number of points to scan to get the results set. 
        This includes any point that was filtered.
        """
        return self.maxScanCount

    @property
    def MaxSeekCount(self) -> int:
        """
        The maximum number of seeks permitted. 0 means no limit.
        """
        return self.maxSeekCount

    def Save(self, stream: binaryStream):
        stream.WriteByte(0)
        stream.WriteUInt64(Ticks.FromTimeDelta(self.timeout))
        stream.WriteUInt64(self.maxReturnedCount)
        stream.WriteUInt64(self.maxScanCount)
        stream.WriteUInt64(self.maxSeekCount)