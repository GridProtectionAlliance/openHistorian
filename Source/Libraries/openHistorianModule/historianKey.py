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
from common import Limits, Ticks
from datetime import datetime
from uuid import UUID
import numpy as np

class historianKey(snapTypeBase): 
    """
    The standard SNAPdb key used for the openHistorian.
    """

    TypeGuid = UUID("6527d41b-9d04-4bfa-8133-05273d521d46")

    def __init__(self):
        self.Timestamp = np.uint64(0)
        self.PointID = np.uint64(0)
        self.EntryNumber = np.uint64(0)

    @property
    def GenericTypeGuid(self) -> UUID:
        """
        The Guid uniquely defining this SNAPdb type. 
        """        
        return historianKey.TypeGuid

    @property
    def Size(self) -> int:
        """
        Gets the size of this SNAPdb type when serialized.
        """      
        return 24

    def SetMin(self):
        """
        Sets the provided SNAPdb type to its minimum value.
        """        
        self.Timestamp = 0
        self.PointID = 0
        self.EntryNumber = 0

    def SetMax(self):
        """
        Sets the provided SNAPdb type to its maximum value.
        """        
        self.Timestamp = Limits.MaxUInt64
        self.PointID = Limits.MaxUInt64
        self.EntryNumber = Limits.MaxUInt64

    def Clear(self):
        """
        Clears the SNAPdb type.
        """        
        self.SetMin()

    def Read(self, stream: remoteBinaryStream):
        """
        Reads the provided SNAPdb type from the stream.
        """        
        self.Timestamp = stream.ReadUInt64()
        self.PointID = stream.ReadUInt64()
        self.EntryNumber = stream.ReadUInt64()

    def CopyTo(self, destination: "historianKey"):
        """
        Copies this SNAPdb type to the `destination`
        """        
        destination.Timestamp = self.Timestamp;
        destination.PointID = self.PointID;
        destination.EntryNumber = self.EntryNumber;
    
    def CompareTo(self, other: "historianKey"):
        """
        Copies this SNAPdb type to the `other`
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