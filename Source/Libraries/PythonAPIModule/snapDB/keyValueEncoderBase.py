#******************************************************************************************************
#  keyValueEncoderBase.py - Gbtc
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

from .encodingDefinition import encodingDefinition
from .snapTypeBase import snapTypeBase
from gsf.binaryStream import binaryStream
from abc import ABC, abstractmethod
from typing import TypeVar, Generic
import numpy as np

TKey = TypeVar('TKey', bound=snapTypeBase)
TValue = TypeVar('TValue', bound=snapTypeBase)

class keyValueEncoderBase(ABC, Generic[TKey, TValue]):
    """
    Represents an encoding method that takes both a key and a value to encode.
    """

    # Source C# references:
    #   PairEncodingBase<TKey, TValue>
    #   StreamEncodingGeneric<TKey, TValue>

    def __init__(self, key: TKey, value: TValue):
        self.prevKey = key
        self.prevValue = value

    @property
    @abstractmethod
    def Definition(self) -> encodingDefinition:
        """
        Gets the encoding definition that this class implements.
        """

    @property
    @abstractmethod
    def UsesPreviousKey(self) -> bool:
        """
        Gets if the previous key will need to be presented to the encoding algorithms to
        property encode the next sample. Returning false will cause `None` to be passed
        in a parameters to the encoding.
        """

    @property
    @abstractmethod
    def UsesPreviousValue(self) -> bool:
        """
        Gets if the previous value will need to be presented to the encoding algorithms to
        property encode the next sample. Returning false will cause `None` to be passed
        in a parameters to the encoding.
        """

    @property
    @abstractmethod
    def MaxCompressionSize(self) -> int:
        """
        Gets the maximum amount of space that is required for the compression algorithm. This
        prevents lower levels from having overflows on the underlying streams. It is critical
        that this value be correct. Error on the side of too large of a value as a value too
        small will corrupt data and be next to impossible to track down the point of corruption
        """

    @property
    @abstractmethod
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

    @property
    @abstractmethod
    def EndOfStreamSymbol(self) -> np.uint8:
        """
        The byte code to use as the end of stream symbol. Implementors can raise not supported
        type exception if `ContainsEndOfStreamSymbol` is False.
        """

    @abstractmethod
    def Encode(self, stream: binaryStream, prevKey: TKey, prevValue: TValue, key: TKey, value: TValue):
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

    @abstractmethod
    def Decode(self, stream: binaryStream, prevKey: TKey, prevValue: TValue, key: TKey, value: TValue) -> bool:
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
  
    def WriteEndOfStream(self, stream: binaryStream) -> int:
        """
        Writes the end of the stream symbol to the `stream`.

        Parameters
        ----------
        stream: where to write the data
        """
        if self.ContainsEndOfStreamSymbol:
            return stream.WriteByte(self.EndOfStreamSymbol)

        return stream.WriteByte(0)

    def StreamEncode(self, stream: binaryStream, key: TKey, value: TValue):
        """
        Encodes the current `key` and `value` to the `stream`.

        Parameters
        ----------
        stream: where to write the data
        key: the SNAPdb key type to encode
        value: the SNAPdb value type to encode
        """
        if not self.ContainsEndOfStreamSymbol:
            stream.WriteByte(1)

        self.Encode(stream, self.prevKey, self.prevValue, key, value)
        
        key.CopyTo(self.prevKey)
        value.CopyTo(self.prevValue)

    def TryStreamDecode(self, stream: binaryStream, key: TKey, value: TValue) -> bool:
        """
        Attempts to decode the next `key` and `value` from the `stream`.

        Parameters
        ----------
        stream: where to read the data
        key: the target SNAPdb key type to hold decoded data
        value: the target SNAPdb value type to hold decoded data

        Returns
        -------
        `True` if successful; otherwise, `False` if end of the stream has been reached.
        """
        if not self.ContainsEndOfStreamSymbol:
            if stream.ReadByte() == 0:
                return False

        endOfStream = self.Decode(stream, self.prevKey, self.prevValue, key, value)
        
        key.CopyTo(self.prevKey)
        value.CopyTo(self.prevValue)

        return not endOfStream

    def ResetEncoder(self):
        """
        Resets the encoder. Some encoders maintain streaming state
        data that should be reset when reading from a new stream.
        """
        self.prevKey.Clear()
        self.prevValue.Clear()