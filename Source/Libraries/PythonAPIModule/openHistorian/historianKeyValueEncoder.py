#******************************************************************************************************
#  historianKeyValueEncoder.py - Gbtc
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
#  02/05/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from .historianKey import historianKey
from .historianValue import historianValue
from snapDB.encodingDefinition import encodingDefinition
from snapDB.keyValueEncoderBase import keyValueEncoderBase
from gsf.binaryStream import binaryStream
from gsf import Limits, override
from uuid import UUID
import numpy as np

class historianKeyValueEncoder(keyValueEncoderBase[historianKey, historianValue]):
    """
    Represents an openHistorian specific SNAPdb stream encoder, i.e.,
    an encoder for the `historianKey` and `historianValue` types
    """

    SnapTypeID = UUID("0418b3a7-f631-47af-bbfa-8b9bc0378328")
    EndOfStream = 0xFF

    # Source C# references:
    #   PairEncodingBase<TKey, TValue>
    #   StreamEncodingGeneric<TKey, TValue>
    #   HistorianStreamEncoding

    def __init__(self):
        super().__init__(historianKey(), historianValue())

        self.definition = encodingDefinition(keyValueEncoding = historianKeyValueEncoder.SnapTypeID)
        self.prevKey = historianKey()
        self.prevValue = historianValue()

    @property
    @override
    def Definition(self) -> encodingDefinition:
        """
        Gets the encoding definition that this class implements.
        """
        return self.definition

    @property
    @override
    def UsesPreviousKey(self) -> bool:
        """
        Gets if the previous key will need to be presented to the encoding algorithms to
        property encode the next sample. Returning false will cause `None` to be passed
        in a parameters to the encoding.
        """
        return True

    @property
    @override
    def UsesPreviousValue(self) -> bool:
        """
        Gets if the previous value will need to be presented to the encoding algorithms to
        property encode the next sample. Returning false will cause `None` to be passed
        in a parameters to the encoding.
        """
        return False

    @property
    @override
    def MaxCompressionSize(self) -> int:
        """
        Gets the maximum amount of space that is required for the compression algorithm. This
        prevents lower levels from having overflows on the underlying streams. It is critical
        that this value be correct. Error on the side of too large of a value as a value too
        small will corrupt data and be next to impossible to track down the point of corruption
        """
        return 55 # 3 extra bytes just to be safe

    @property
    @override
    def ContainsEndOfStreamSymbol(self) -> bool:
        """
        Gets if the stream supports a symbol that represents that the end of the stream
        has been encountered.

        An example of a symbol would be the byte code 0xFF. In this case, if the first byte
        of the word is 0xFF, the encoding has specifically designated this as the end of the
        stream. Therefore, calls to Decompress will result in an end of stream exception.
        
        Failing to reserve a code as the end of stream will mean that streaming points will
        include its own symbol to represent the end of the stream, taking 1 extra byte per
        point encoded.
        """
        return True

    @property
    @override
    def EndOfStreamSymbol(self) -> np.uint8:
        """
        The byte code to use as the end of stream symbol. Implementors can raise not supported
        type exception if `ContainsEndOfStreamSymbol` is False.
        """
        return historianKeyValueEncoder.EndOfStream

    @override
    def Encode(self, stream: binaryStream, prevKey: historianKey, prevValue: historianValue, key: historianKey, value: historianValue):
        """
        Encodes `key` and `value` to the provided `stream`.

        Parameters
        ----------
        stream: where to write the data
        prevKey: the previous key if required by `UsesPreviousKey`; otherwise `None`.
        prevValue: the previous value if required by `UsesPreviousValue`; otherwise `None`.
        key: the SNAPdb key type to encode
        value: the SNAPdb value type to encode
        """
        if (key.Timestamp == prevKey.Timestamp and
                (np.uint64(key.PointID) ^ np.uint64(prevKey.PointID)) < np.uint64(64) and
                key.EntryNumber == 0 and
                value.Value1 <= Limits.MAXUINT32 and
                value.Value2 == 0 and
                value.Value3 == 0):

            if value.Value1 == 0:
                stream.WriteByte(np.uint8(np.uint64(key.PointID) ^ np.uint64(prevKey.PointID)));
            else:
                stream.WriteByte(np.uint8((np.uint64(key.PointID) ^ np.uint64(prevKey.PointID)) | np.uint64(64)));
                stream.WriteUInt32(np.uint32(value.Value1));

            return

        code = np.uint8(128)

        if key.Timestamp != prevKey.Timestamp:
            code |= np.uint8(64)

        if key.EntryNumber != 0:
            code |= np.uint8(32)

        if value.Value1 > Limits.MAXUINT32:
            code |= np.uint8(16)
        elif value.Value1 > 0:
            code |= np.uint8(8)

        if value.Value2 != 0:
            code |= np.uint8(4)

        if value.Value3 > Limits.MAXUINT32:
            code |= np.uint8(2)
        elif value.Value3 > 0:
            code |= np.uint8(1)

        stream.WriteByte(np.uint8(code))

        if key.Timestamp != prevKey.Timestamp:
            stream.Write7BitUInt64(np.uint64(key.Timestamp) ^ np.uint64(prevKey.Timestamp))

        stream.Write7BitUInt64(np.uint64(key.PointID) ^ np.uint64(prevKey.PointID))

        if key.EntryNumber != 0:
            stream.Write7BitUInt64(key.EntryNumber)

        if value.Value1 > Limits.MAXUINT32:
            stream.WriteUInt64(value.Value1)
        elif value.Value1 > 0:
            stream.WriteUInt32(np.uint32(value.Value1))

        if value.Value2 != 0:
            stream.WriteUInt64(value.Value2)

        if value.Value3 > Limits.MAXUINT32:
            stream.WriteUInt64(value.Value3)
        elif value.Value3 > 0:
            stream.WriteUInt32(np.uint32(value.Value3))

    @override
    def Decode(self, stream: binaryStream, prevKey: historianKey, prevValue: historianValue, key: historianKey, value: historianValue) -> bool:
        """
        Decodes `key` and `value` from the provided `stream`.

        Parameters
        ----------
        stream: where to read the data
        prevKey: the previous key if required by `UsesPreviousKey`; otherwise `None`.
        prevValue: the previous value if required by `UsesPreviousValue`; otherwise `None`.
        key: the target SNAPdb key type to hold decoded data
        value: the target SNAPdb value type to hold decoded data

        Returns
        -------
        When `ContainsEndOfStreamSymbol` is `True`, value determines if the end of the stream symbol is detected;
        otherwise, if `ContainsEndOfStreamSymbol` is `False`, result is always `False`
        """
        code = stream.ReadByte()

        if code == historianKeyValueEncoder.EndOfStream:
            return True

        if code < 128:
            if code < 64:
                key.Timestamp = prevKey.Timestamp
                key.PointID = np.uint64(prevKey.PointID) ^ np.uint64(code)
                key.EntryNumber = 0
                value.Value1 = 0
                value.Value2 = 0
                value.Value3 = 0
            else:
                key.Timestamp = prevKey.Timestamp
                key.PointID = np.uint64(prevKey.PointID) ^ np.uint64(code) ^ np.uint64(64)
                key.EntryNumber = 0
                value.Value1 = stream.ReadUInt32()
                value.Value2 = 0
                value.Value3 = 0

            return False

        if (code & 64) != 0: # T is set
            key.Timestamp = np.uint64(prevKey.Timestamp) ^ stream.Read7BitUInt64()
        else:
            key.Timestamp = prevKey.Timestamp

        key.PointID = np.uint64(prevKey.PointID) ^ stream.Read7BitUInt64()

        if (code & 32) != 0: # E is set
            key.EntryNumber = stream.Read7BitUInt64()
        else:
            key.EntryNumber = 0

        if (code & 16) != 0: # V1 High is set
            value.Value1 = stream.ReadUInt64()
        elif (code & 8) != 0: # V1 low is set
            value.Value1 = stream.ReadUInt32()
        else:
            value.Value1 = 0

        if (code & 4) != 0: # V2 is set
            value.Value2 = stream.ReadUInt64()
        else:
            value.Value2 = 0

        if (code & 2) != 0: # V1 High is set
            value.Value3 = stream.ReadUInt64()
        elif (code & 1) != 0: # V1 low is set
            value.Value3 = stream.ReadUInt32()
        else:
            value.Value3 = 0

        return False