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

from snapDB.snapTypeBase import snapTypeBase
from snapDB.enumerations import QualityFlags
from gsf.binaryStream import binaryStream
from gsf import Limits, ByteSize, override
from uuid import UUID
import struct
import numpy as np

class historianValue(snapTypeBase):
    """
    The standard SNAPdb value used for the openHistorian.
    """

    SnapTypeID = UUID("24dde7dc-67f9-42b6-a11b-e27c3e62d9ef")

    def __init__(self):
        # Value 1 should be where the first 64 bits of the field is stored. For 32 bit values, use this field only.
        self.Value1 = np.uint64(0)
        
        # Should only be used if value cannot be entirely stored in Value1. Compression penalty occurs when using this field.
        self.Value2 = np.uint64(0)
        
        # Should contain any kind of digital data such as Quality. Compression penalty occurs when used for any other type of field.
        self.Value3 = np.uint64(0)

    @property
    @override
    def TypeID(self) -> UUID:
        """
        Gets the Guid uniquely defining this SNAPdb type. 
        """        
        return historianValue.SnapTypeID

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
        self.Value1 = np.uint64(0)
        self.Value2 = np.uint64(0)
        self.Value3 = np.uint64(0)

    @override
    def SetMax(self):
        """
        Sets the provided SNAPdb type to its maximum value.
        """        
        self.Value1 = np.uint64(Limits.MAXUINT64)
        self.Value2 = np.uint64(Limits.MAXUINT64)
        self.Value3 = np.uint64(Limits.MAXUINT64)

    @override
    def Clear(self):
        """
        Clears the SNAPdb type.
        """        
        self.SetMin()

    @override
    def Read(self, stream: binaryStream):
        """
        Reads this SNAPdb type from the stream.
        """        
        self.Value1 = stream.ReadUInt64()
        self.Value2 = stream.ReadUInt64()
        self.Value3 = stream.ReadUInt64()
    
    @override
    def Write(self, stream: binaryStream):
        """
        Writes this SNAPdb type to the stream.
        """
        stream.WriteUInt64(self.Value1)
        stream.WriteUInt64(self.Value2)
        stream.WriteUInt64(self.Value3)

    @override
    def CopyTo(self, destination: "historianValue"):
        """
        Copies this SNAPdb type to the `destination`
        """        
        destination.Value1 = self.Value1;
        destination.Value2 = self.Value2;
        destination.Value3 = self.Value3;
    
    @override
    def CompareTo(self, other: "historianValue"):
        """
        Compares this SNAPdb type to the `other`
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
        return np.frombuffer(int(np.uint32(self.Value1)).to_bytes(ByteSize.UINT32, "little"), np.float32)[0]

    @AsSingle.setter
    def AsSingle(self, value: np.float32):
        """
        Sets `Value1` type cast from a single.
        """
        buffer = bytearray(8)
        valueBytes = struct.pack("<f", value)

        for i in range(4):
            buffer[i] = valueBytes[i]

        self.Value1 = np.frombuffer(buffer, np.uint64)[0]

    @property
    def AsQuality(self) -> QualityFlags:
        """
        Gets `Value3` type cast as `QualityFlags`.
        """
        return QualityFlags(self.Value3)

    @AsQuality.setter
    def AsQuality(self, value: QualityFlags):
        """
        Sets `Value3` type cast from `QualityFlags`.
        """
        self.Value3 = np.uint64(value.value)

    def ToString(self) -> str:
        return f"{self.AsSingle:.3f} [{self.AsQuality}]"