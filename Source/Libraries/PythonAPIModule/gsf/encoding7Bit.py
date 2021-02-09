#******************************************************************************************************
#  encoding7Bit.py - Gbtc
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

from . import ByteSize
from typing import Callable
import numpy as np

class encoding7Bit:
    """
    Defines 7-bit encoding/decoding functions.
    """

    @staticmethod
    def WriteInt16(streamWriter: Callable[[np.uint8], int], value: np.int16) -> int:
        """
        Writes 16-bit signed integer value using 7-bit encoding to the provided stream writer.
        
        Parameters
        ----------
        streamWriter: function to write a byte value to a stream
        value: 16-bit value to write
        """

        return WriteUInt16(streamWriter, np.frombuffer(int(value).to_bytes(ByteSize.INT16, "little", signed=True), np.uint16)[0])

    @staticmethod
    def WriteUInt16(streamWriter: Callable[[np.uint8], int], value: np.uint16) -> int:
        """
        Writes 16-bit unsigned integer value using 7-bit encoding to the provided stream writer.
        
        Parameters
        ----------
        streamWriter: function to write a byte value to a stream
        value: 16-bit value to write
        """

        np128 = np.uint16(128)
        np7 = np.uint16(7)
        npStreamWriter = lambda uint16: streamWriter(np.uint8(uint16))

        if value < np128:
            npStreamWriter(value) #1
            return 1

        npStreamWriter(value | np128) #1
        npStreamWriter(value >> np7) #2
        return 2
        
    @staticmethod
    def WriteInt32(streamWriter: Callable[[np.uint8], int], value: np.int32) -> int:
        """
        Writes 32-bit signed integer value using 7-bit encoding to the provided stream writer.
        
        Parameters
        ----------
        streamWriter: function to write a byte value to a stream
        value: 32-bit value to write
        """
        
        return WriteUInt32(streamWriter, np.frombuffer(int(value).to_bytes(ByteSize.INT32, "little", signed=True), np.uint32)[0])

    @staticmethod
    def WriteUInt32(streamWriter: Callable[[np.uint8], int], value: np.uint32) -> int:
        """
        Writes 32-bit unsigned integer value using 7-bit encoding to the provided stream writer.
        
        Parameters
        ----------
        streamWriter: function to write a byte value to a stream
        value: 32-bit value to write
        """

        np128 = np.uint32(128)
        np7 = np.uint32(7)
        npStreamWriter = lambda uint32: streamWriter(np.uint8(uint32))

        if value < np128:
            npStreamWriter(value) #1
            return 1

        npStreamWriter(value | np128) #1
        if value < np128 * np128:
            npStreamWriter(value >> np7) #2
            return 2

        npStreamWriter((value >> np7) | np128) #2
        if value < np128 * np128 * np128:
            npStreamWriter(value >> (np7 + np7)) #3
            return 3
            
        npStreamWriter((value >> (np7 + np7)) | np128) #3
        if value < np128 * np128 * np128 * np128:
            npStreamWriter(value >> (np7 + np7 + np7)) #4
            return 4
            
        npStreamWriter((value >> (np7 + np7 + np7)) | np128) #4        
        npStreamWriter(value >> (np7 + np7 + np7 + np7)) #5
        return 5

    @staticmethod
    def WriteInt64(streamWriter: Callable[[np.uint8], int], value: np.int64) -> int:
        """
        Writes 64-bit signed integer value using 7-bit encoding to the provided stream writer.
        
        Parameters
        ----------
        streamWriter: function to write a byte value to a stream
        value: 64-bit value to write
        """

        return WriteUInt64(streamWriter, np.frombuffer(int(value).to_bytes(ByteSize.INT64, "little", signed=True), np.uint64)[0])

    @staticmethod
    def WriteUInt64(streamWriter: Callable[[np.uint8], int], value: np.uint64) -> int:
        """
        Writes 64-bit unsigned integer value using 7-bit encoding to the provided stream writer.
        
        Parameters
        ----------
        streamWriter: function to write a byte value to a stream
        value: 64-bit value to write
        """

        np128 = np.uint64(128)
        np7 = np.uint64(7)
        npStreamWriter = lambda uint64: streamWriter(np.uint8(uint64))

        if value < np128:
            npStreamWriter(value) #1
            return 1

        npStreamWriter(value | np128) #1
        if value < np128 * np128:
            npStreamWriter(value >> np7) #2
            return 2

        npStreamWriter((value >> np7) | np128) #2
        if value < np128 * np128 * np128:
            npStreamWriter(value >> (np7 + np7)) #3
            return 3
            
        npStreamWriter((value >> (np7 + np7)) | np128) #3
        if value < np128 * np128 * np128 * np128:
            npStreamWriter(value >> (np7 + np7 + np7)) #4
            return 4
            
        npStreamWriter((value >> (np7 + np7 + np7)) | np128) #4
        if value < np128 * np128 * np128 * np128 * np128:
            npStreamWriter(value >> (np7 + np7 + np7 + np7)) #5
            return 5
            
        npStreamWriter((value >> (np7 + np7 + np7 + np7)) | np128) #5
        if value < np128 * np128 * np128 * np128 * np128 * np128:
            npStreamWriter(value >> (np7 + np7 + np7 + np7 + np7)) #6
            return 6
            
        npStreamWriter((value >> (np7 + np7 + np7 + np7 + np7)) | np128) #6
        if value < np128 * np128 * np128 * np128 * np128 * np128 * np128:
            npStreamWriter(value >> (np7 + np7 + np7 + np7 + np7 + np7)) #7
            return 7
            
        npStreamWriter((value >> (np7 + np7 + np7 + np7 + np7 + np7)) | np128) #7
        if value < np128 * np128 * np128 * np128 * np128 * np128 * np128 * np128:
            npStreamWriter(value >> (np7 + np7 + np7 + np7 + np7 + np7 + np7)) #8
            return 8
            
        npStreamWriter(value >> (np7 + np7 + np7 + np7 + np7 + np7 + np7) | np128) #8
        npStreamWriter(value >> (np7 + np7 + np7 + np7 + np7 + np7 + np7 + np7)) #9
        return 9

    @staticmethod
    def ReadInt16(streamReader: Callable[[], np.uint8]) -> np.int16:
        """
        Reads 16-bit signed integer value using 7-bit encoding from the provided stream reader.
        
        Parameters
        ----------
        streamReader: function to read a byte value from a stream

        Notes
        -----
        Call expects one to two bytes to be available in base stream.
        """

        return np.frombuffer(int(ReadUInt16(streamReader)).to_bytes(ByteSize.UINT16, "little"), np.int16)[0]

    @staticmethod
    def ReadUInt16(streamReader: Callable[[], np.uint8]) -> np.uint16:
        """
        Reads 16-bit unsigned integer value using 7-bit encoding from the provided stream reader.
        
        Parameters
        ----------
        streamReader: function to read a byte value from a stream

        Notes
        -----
        Call expects one to two bytes to be available in base stream.
        """

        np128 = np.uint16(128)
        np7 = np.uint16(7)
        npStreamReader = lambda: np.uint16(streamReader())

        value = npStreamReader()

        if value < np128:
            return value

        value ^= (npStreamReader() << (np7))
        return value ^ np.uint16(0x80)

    @staticmethod
    def ReadInt32(streamReader: Callable[[], np.uint8]) -> np.int32:
        """
        Reads 32-bit signed integer value using 7-bit encoding from the provided stream reader.
        
        Parameters
        ----------
        streamReader: function to read a byte value from a stream

        Notes
        -----
        Call expects one to five bytes to be available in base stream.
        """

        return np.frombuffer(int(ReadUInt32(streamReader)).to_bytes(ByteSize.UINT32, "little"), np.int32)[0]

    @staticmethod
    def ReadUInt32(streamReader: Callable[[], np.uint8]) -> np.uint32:
        """
        Reads 32-bit unsigned integer value using 7-bit encoding from the provided stream reader.
        
        Parameters
        ----------
        streamReader: function to read a byte value from a stream

        Notes
        -----
        Call expects one to five bytes to be available in base stream.
        """

        np128 = np.uint32(128)
        np7 = np.uint32(7)
        npStreamReader = lambda: np.uint32(streamReader())

        value = npStreamReader()

        if value < np128:
            return value

        value ^= (npStreamReader() << (np7))
        if value < np128 * np128:
            return value ^ np.uint32(0x80)

        value ^= (npStreamReader() << (np7 + np7))
        if value < np128 * np128 * np128:
            return value ^ np.uint32(0x4080)

        value ^= (npStreamReader() << (np7 + np7 + np7))
        if value < np128 * np128 * np128 * np128:
            return value ^ np.uint32(0x204080)

        value ^= (npStreamReader() << (np7 + np7 + np7 + np7))
        return value ^ np.uint32(0x10204080)

    @staticmethod
    def ReadInt64(streamReader: Callable[[], np.uint8]) -> np.int64:
        """
        Reads 64-bit signed integer value using 7-bit encoding from the provided stream reader.
        
        Parameters
        ----------
        streamWriter: function to write a byte value to a stream
        value: 64-bit value to write

        Notes
        -----
        Call expects one to nine bytes to be available in base stream.
        """

        return np.frombuffer(int(ReadUInt64(streamReader)).to_bytes(ByteSize.UINT64, "little"), np.int64)[0]

    @staticmethod
    def ReadUInt64(streamReader: Callable[[], np.uint8]) -> np.uint64:
        """
        Reads 64-bit unsigned integer value using 7-bit encoding from the provided stream reader.
        
        Parameters
        ----------
        streamWriter: function to write a byte value to a stream
        value: 64-bit value to write

        Notes
        -----
        Call expects one to nine bytes to be available in base stream.
        """

        np128 = np.uint64(128)
        np7 = np.uint64(7)
        npStreamReader = lambda: np.uint64(streamReader())

        value = npStreamReader()

        if value < np128:
            return value

        value ^= (npStreamReader() << (np7))
        if value < np128 * np128:
            return value ^ np.uint64(0x80)

        value ^= (npStreamReader() << (np7 + np7))
        if value < np128 * np128 * np128:
            return value ^ np.uint64(0x4080)

        value ^= (npStreamReader() << (np7 + np7 + np7))
        if value < np128 * np128 * np128 * np128:
            return value ^ np.uint64(0x204080)

        value ^= (npStreamReader() << (np7 + np7 + np7 + np7))
        if value < np128 * np128 * np128 * np128 * np128:
            return value ^ np.uint64(0x10204080)

        value ^= (npStreamReader() << (np7 + np7 + np7 + np7 + np7))
        if value < np128 * np128 * np128 * np128 * np128 * np128:
            return value ^ np.uint64(0x810204080)

        value ^= (npStreamReader() << (np7 + np7 + np7 + np7 + np7 + np7))
        if value < np128 * np128 * np128 * np128 * np128 * np128 * np128:
            return value ^ np.uint64(0x40810204080)

        value ^= (npStreamReader() << (np7 + np7 + np7 + np7 + np7 + np7 + np7))
        if value < np128 * np128 * np128 * np128 * np128 * np128 * np128 * np128:
            return value ^ np.uint64(0x2040810204080)

        value ^= (npStreamReader() << (np7 + np7 + np7 + np7 + np7 + np7 + np7 + np7))
        return value ^ np.uint64(0x102040810204080)
