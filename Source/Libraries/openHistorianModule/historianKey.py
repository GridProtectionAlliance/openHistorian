#******************************************************************************************************
#  historianKey.py - Gbtc
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
#  02/02/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from snapTypeBase import snapTypeBase
from remoteBinaryStream import remoteBinaryStream
from common import Limits, Ticks, override
from datetime import datetime
from uuid import UUID
import numpy as np

class historianKey(snapTypeBase): 
    """
    The standard SNAPdb key used for the openHistorian.
    """

    SnapTypeID = UUID("6527d41b-9d04-4bfa-8133-05273d521d46")

    def __init__(self):
        self.Timestamp = np.uint64(0)
        self.PointID = np.uint64(0)
        self.EntryNumber = np.uint64(0)

    @property
    @override
    def TypeID(self) -> UUID:
        """
        The Guid uniquely defining this SNAPdb type. 
        """        
        return historianKey.SnapTypeID

    @property
    @override
    def Size(self) -> int:
        """
        Gets the size of this SNAPdb type when serialized.
        """      
        return 24

    @override
    def SetMin(self):
        """
        Sets the provided SNAPdb type to its minimum value.
        """        
        self.Timestamp = np.uint64(0)
        self.PointID = np.uint64(0)
        self.EntryNumber = np.uint64(0)

    @override
    def SetMax(self):
        """
        Sets the provided SNAPdb type to its maximum value.
        """        
        self.Timestamp = np.uint64(Limits.MAXUINT64)
        self.PointID = np.uint64(Limits.MAXUINT64)
        self.EntryNumber = np.uint64(Limits.MAXUINT64)

    @override
    def Clear(self):
        """
        Clears the SNAPdb type.
        """        
        self.SetMin()

    @override
    def Read(self, stream: remoteBinaryStream):
        """
        Reads this SNAPdb type from the stream.
        """        
        self.Timestamp = stream.ReadUInt64()
        self.PointID = stream.ReadUInt64()
        self.EntryNumber = stream.ReadUInt64()
    
    @override
    def Write(self, stream: remoteBinaryStream):
        """
        Writes this SNAPdb type to the stream.
        """
        stream.WriteUInt64(self.Timestamp)
        stream.WriteUInt64(self.PointID)
        stream.WriteUInt64(self.EntryNumber)

    @override
    def CopyTo(self, destination: "historianKey"):
        """
        Copies this SNAPdb type to the `destination`
        """        
        destination.Timestamp = self.Timestamp
        destination.PointID = self.PointID
        destination.EntryNumber = self.EntryNumber
    
    @override
    def CompareTo(self, other: "historianKey"):
        """
        Compares this SNAPdb type to the `other`
        """
        if self.Timestamp < other.Timestamp:
            return -1
        if self.Timestamp > other.Timestamp:
            return 1
        if self.PointID < other.PointID:
            return -1
        if self.PointID > other.PointID:
            return 1
        if self.EntryNumber < other.EntryNumber:
            return -1
        if self.EntryNumber > other.EntryNumber:
            return 1
        return 0

    def TryGetDateTime(self) -> (datetime, bool):
        """
        Attempts to get the timestamp field of this key instance.
        """
        if self.Timestamp > Limits.MaxTicks:
            return (datetime(1, 1, 1), False)

        return (Ticks.ToDateTime(self.Timestamp), True)

    @property
    def AsDateTime(self) -> datetime:
        """
        Gets `Timestamp` type cast as a `datetime`
        """
        return self.TryGetDateTime()[0]

    @AsDateTime.setter
    def AsDateTime(self, value: datetime):
        """
        Sets `Timestamp` type cast from a `datetime`
        """
        self.Timestamp = Ticks.FromDateTime(value)