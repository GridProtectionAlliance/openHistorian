#******************************************************************************************************
#  fixedSizeKeyValueEncoder.py - Gbtc
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
from .keyValueEncoderBase import keyValueEncoderBase
from .snapTypeBase import snapTypeBase
from gsf.binaryStream import binaryStream
from gsf import override
from typing import TypeVar, Generic
import numpy as np

TKey = TypeVar('TKey', bound=snapTypeBase)
TValue = TypeVar('TValue', bound=snapTypeBase)

class fixedSizeKeyValueEncoder(Generic[TKey, TValue], keyValueEncoderBase[TKey, TValue]):
    """
    An encoding method that is fixed in size and calls the native read/write functions of the specified type.
    """

    # Source C# references:
    #   PairEncodingBase<TKey, TValue>
    #   StreamEncodingGeneric<TKey, TValue>
    #   PairEncodingFixedSize<TKey, TValue>

    def __init__(self, key: TKey, value: TValue):
        super().__init__(key, value)

        self.definition = encodingDefinition(keyValueEncoding = encodingDefinition.FixedSizeIndividualGuid)
        self.keySize = key.Size
        self.valueSize = value.Size

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
        return False

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
        return self.keySize + self.valueSize

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
        return False

    @property
    @override
    def EndOfStreamSymbol(self) -> np.uint8:
        """
        The byte code to use as the end of stream symbol. Implementors can raise not supported
        type exception if `ContainsEndOfStreamSymbol` is False.
        """
        raise NotImplementedError()

    @override
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
        key.Write(stream)
        value.Write(stream)

    @override
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
        key.Read(stream)
        value.Read(stream)
        return False

