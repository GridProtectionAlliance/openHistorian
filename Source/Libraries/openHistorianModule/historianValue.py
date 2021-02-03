#******************************************************************************************************
#  historianValue.py - Gbtc
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
from common import Limits, BitConvert
from uuid import UUID
import numpy as np

class historianValue(snapTypeBase):
    """
    The standard SNAPdb value used for the openHistorian.
    """

    TypeGuid = UUID("24dde7dc-67f9-42b6-a11b-e27c3e62d9ef")

    def __init__(self):
        # Value 1 should be where the first 64 bits of the field is stored. For 32 bit values, use this field only.
        self.Value1 = np.uint64(0)
        
        # Should only be used if value cannot be entirely stored in Value1. Compression penalty occurs when using this field.
        self.Value2 = np.uint64(0)
        
        # Should contain any kind of digital data such as Quality. Compression penalty occurs when used for any other type of field.
        self.Value3 = np.uint64(0)

    @property
    def GenericTypeGuid(self) -> UUID:
        """
        The Guid uniquely defining this SNAPdb type. 
        """        
        return historianValue.TypeGuid

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
        self.Value1 = 0
        self.Value2 = 0
        self.Value3 = 0

    def SetMax(self):
        """
        Sets the provided SNAPdb type to its maximum value.
        """        
        self.Value1 = Limits.MaxUInt64
        self.Value2 = Limits.MaxUInt64
        self.Value3 = Limits.MaxUInt64

    def Clear(self):
        """
        Clears the SNAPdb type.
        """        
        self.SetMin()

    def Read(self, stream: remoteBinaryStream):
        """
        Reads the provided SNAPdb type from the stream.
        """        
        self.Value1 = stream.ReadUInt64()
        self.Value2 = stream.ReadUInt64()
        self.Value3 = stream.ReadUInt64()

    def CopyTo(self, destination: "historianValue"):
        """
        Copies this SNAPdb type to the `destination`
        """        
        destination.Value1 = self.Value1;
        destination.Value2 = self.Value2;
        destination.Value3 = self.Value3;
    
    def CompareTo(self, other: "historianValue"):
        """
        Copies this SNAPdb type to the `other`
        """
        if self.Value1 < other.Value1:
            return -1
        
        if self.Value1 > other.Value1:
            return 1
        
        if self.Value2 < other.Value2:
            return -1
        
        if self.Value2 > other.Value2:
            return 1
        
        if self.Value3 < other.Value3:
            return -1
        
        if self.Value3 > other.Value3:
            return 1
        
        return 0
    
    @property
    def AsSingle(self) -> np.float32:
        """
        Gets `Value1` type cast as a single.
        """
        return BitConvert.ToSingle(self.Value1)

    @AsSingle.setter
    def AsSingle(self, value: np.float32):
        """
        Sets `Value1` type cast from a single.
        """
        self.Value1 = BitConvert.ToUInt64(value)
