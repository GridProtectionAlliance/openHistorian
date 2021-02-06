#******************************************************************************************************
#  common.py - Gbtc
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
#  02/01/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from typing import Sequence
from datetime import datetime, timedelta
from uuid import UUID
from enum import IntEnum
from struct import pack
import numpy as np

def static_init(cls):
    """
    Marks a class as having a static initialization function and
    executes the function when class is statically constructed.
    """
    if getattr(cls, "static_init", None):
        cls.static_init()

    return cls

def override(self):
    """
    Marks a method as an override (for documentation purposes).
    """
    return self

class Empty:
    GUID = UUID("00000000-0000-0000-0000-000000000000")

class Limits(IntEnum):
    MAXTICKS = 3155378975999999999
    MAXBYTE = 255
    MAXINT16 = 32767
    MAXUINT16 = 65535
    MAXINT32 = 2147483647
    MAXUINT32 = 4294967295
    MAXINT64 = 9223372036854775807
    MAXUINT64 = 18446744073709551615

class ByteSize(IntEnum):
    INT8 = 1
    UINT8 = 1
    INT16 = 2
    UINT16 = 2
    INT32 = 4
    UINT32 = 4
    INT64 = 8
    UINT64 = 8
    ENC7BIT = 9

class Ticks:
    @staticmethod
    def FromDateTime(dt: datetime) -> np.uint64:
        return np.uint64((dt - datetime(1, 1, 1)).total_seconds() * 10000000)

    @staticmethod
    def FromTimeDelta(td: timedelta) -> np.uint64:
        return np.uint64(td.total_seconds() * 10000000)
    
    @staticmethod
    def ToDateTime(ticks: np.uint64) -> datetime:
        return datetime(1, 1, 1) + timedelta(microseconds = ticks // 10)

class BitConvert:
    @staticmethod
    def ToUInt64(value: np.float32) -> np.uint64:
        return np.frombuffer(struct.pack("f", np.float64(value)), np.uint64)[0]

    @staticmethod
    def ToSingle(value: np.uint64) -> np.float32:
        return np.frombuffer(np.uint32(value).to_bytes(ByteSize.UINT32, "little"), np.float32)[0]

class Validate:
    @staticmethod
    def Parameters(array: Sequence, startIndex: int, length: int):
        if array is None:
            raise TypeError("array is None")

        if startIndex < 0:
            raise ValueError("startIndex cannot be negative")

        if length < 0:
            raise ValueError("value cannot be negative")

        if startIndex + length > len(array):
            raise ValueError("startIndex of " + str(startIndex) + " and length of " + str(length) + " will exceed array size of " + str(len(array)))
