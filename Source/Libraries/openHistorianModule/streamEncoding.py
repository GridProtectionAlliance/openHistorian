#******************************************************************************************************
#  streamEncoding.py - Gbtc
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
#  01/31/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from encoding7Bit import encoding7Bit
from common import ByteSize
from typing import Callable
import numpy as np

class streamEncoding:
    """
    Defines functions for encoding and decoding native types to and from a stream.
    For this class, a stream is simply an abstract notion based on provided functions
    that read and write byte buffers as Python `bytes` objects advancing a position
    in the base stream. The read/write functions simply wrap a base object that can
    handle input and output as bytes, e.g., a `bytearray` or a `socket`.
    """

    def __init__(self, read: Callable[[int], bytes], write: Callable[[bytes], int]):
        """
        Parameters
        ----------
        read : func(length: int) -> bytes
            Read function that accepts desired number of bytes to read and returns `bytes` object of read bytes.
            Actual length of returned `bytes` object may be less than desired number of bytes.
        write: func(buffer: bytes) -> int
            Write function that accepts a `bytes` object and returns count of bytes written. It is expected that
            call to write will successfully write all bytes, i.e., returned length should match `buffer` length.
        """
        self.read = read
        self.write = write

    def Write(self, sourceBuffer: bytes, offset: int, count: int) -> int:
        if self.write(sourceBuffer[offset:offset + count]) != count:
            raise RuntimeError("Failed to write " + str(count) + " bytes to stream")

        return count

    def Read(self, targetBuffer: bytearray, offset: int, count: int) -> int:
        # `count` is requested size, value is treated as max return size
        buffer = self.read(count)
        readLength = len(buffer)

        for i in range(readLength):
            targetBuffer[offset + i] = buffer[i] 

        return readLength

    def WriteByte(self, value: np.uint8) -> int:
        size = ByteSize.UINT8

        if self.write(value.to_bytes(size, "little")) != size:
            raise RuntimeError("Failed to write 1-byte to stream")

        return size

    def ReadByte(self) -> np.uint8:
        size = ByteSize.UINT8

        # call expects one byte to be available in base stream
        buffer = self.read(size)

        if len(buffer) != size:
            raise RuntimeError("Failed to read 1-byte from stream")
        
        return np.uint8(buffer[0])

    def WriteBoolean(self, value: bool) -> int:
        if value:
            self.WriteByte(1)
        else:
            self.WriteByte(0)

        return 1

    def ReadBoolean(self) -> bool:
        # call expects one byte to be available in base stream
        return self.ReadByte() != 0

    def WriteInt16(self, value: np.int16) -> int:
        size = ByteSize.INT16

        if self.write(value.to_bytes(size, "little", signed=True)) != size:
            raise RuntimeError("Failed to write 2-bytes to stream")

        return size

    def ReadInt16(self) -> np.int16:
        size = ByteSize.INT16

        # call expects two bytes to be available in base stream
        buffer = self.read(size)

        if len(buffer) != size:
            raise RuntimeError("Failed to read 2-bytes from stream")

        return np.frombuffer(buffer, np.int16)[0]

    def WriteUInt16(self, value: np.uint16) -> int:
        size = ByteSize.UINT16

        if self.write(value.to_bytes(size, "little")) != size:
            raise RuntimeError("Failed to write 2-bytes to stream")

        return size

    def ReadUInt16(self) -> np.uint16:
        size = ByteSize.UINT16

        # call expects two bytes to be available in base stream
        buffer = self.read(size)

        if len(buffer) != size:
            raise RuntimeError("Failed to read 2-bytes from stream")

        return np.frombuffer(buffer, np.uint16)[0]

    def WriteInt32(self, value: np.int32) -> int:
        size = ByteSize.INT32

        if self.write(value.to_bytes(size, "little", signed=True)) != size:
            raise RuntimeError("Failed to write 4-bytes to stream")

        return size

    def ReadInt32(self) -> np.int32:
        size = ByteSize.INT32

        # call expects four bytes to be available in base stream
        buffer = self.read(size)

        if len(buffer) != size:
            raise RuntimeError("Failed to read 4-bytes from stream")

        return np.frombuffer(buffer, np.int32)[0]

    def WriteUInt32(self, value: np.uint32) -> int:
        size = ByteSize.UINT32

        if self.write(value.to_bytes(size, "little")) != size:
            raise RuntimeError("Failed to write 4-bytes to stream")

        return size

    def ReadUInt32(self) -> np.uint32:
        size = ByteSize.UINT32

        # call expects four bytes to be available in base stream
        buffer = self.read(size)

        if len(buffer) != size:
            raise RuntimeError("Failed to read 4-bytes from stream")

        return np.frombuffer(buffer, np.uint32)[0]

    def WriteInt64(self, value: np.int64) -> int:
        size = ByteSize.INT64

        if self.write(value.to_bytes(size, "little", signed=True)) != size:
            raise RuntimeError("Failed to write 8-bytes to stream")

        return size

    def ReadInt64(self) -> np.int64:
        size = ByteSize.INT64

        # call expects eight bytes to be available in base stream
        buffer = self.read(size)

        if len(buffer) != size:
            raise RuntimeError("Failed to read 8-bytes from stream")

        return np.frombuffer(buffer, np.int64)[0]

    def WriteUInt64(self, value: np.uint64) -> int:
        size = ByteSize.UINT64

        if self.write(value.to_bytes(size, "little")) != size:
            raise RuntimeError("Failed to write 8-bytes to stream")

        return size

    def ReadUInt64(self) -> np.uint64:
        size = ByteSize.UINT64

        # call expects eight bytes to be available in base stream
        buffer = self.read(size)

        if len(buffer) != size:
            raise RuntimeError("Failed to read 8-bytes from stream")

        return np.frombuffer(buffer, np.uint64)[0]
    
    def Write7BitUInt32(self, value: np.uint32) -> int:
        return encoding7Bit.WriteUInt32(self.WriteByte, value)

    def Read7BitUInt32(self) -> np.uint32:
        # call expects one to five bytes to be available in base stream
        return encoding7Bit.ReadUInt32(self.ReadByte)
    
    def Write7BitUInt64(self, value: np.uint64) -> int:
        return encoding7Bit.WriteUInt64(self.WriteByte, value)

    def Read7BitUInt64(self) -> np.uint64:
        # call expects one to nine bytes to be available in base stream
        return encoding7Bit.ReadUInt64(self.ReadByte)
