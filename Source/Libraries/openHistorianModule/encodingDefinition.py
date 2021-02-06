#******************************************************************************************************
#  encodingDefinition.py - Gbtc
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

from remoteBinaryStream import remoteBinaryStream
from common import Empty
from typing import Optional
from uuid import UUID
import numpy as np

class encodingDefinition:
    """
    Represents an immutable class that represents the compression method
    used by the SNAPdb SortedTreeStore.
    """
    
    FixedSizeIndividualGuid = UUID("1dea326d-a63a-4f73-b51c-7b3125c6da55")

    def __init__(self, stream: Optional[remoteBinaryStream] = None, keyValueEncoding: Optional[UUID] = None, keyEncoding: Optional[UUID] = None, valueEncoding: Optional[UUID] = None):
        """
        Creates an `encodingDefinition`.
        """
        if stream is not None:
            self.__initFromStream(stream)
        elif keyValueEncoding is not None:
            self.__initFromCombinedKeyValue(keyValueEncoding)
        else:
            self.__initFromSeparateKeyValue(keyEncoding, valueEncoding)

    def __initFromStream(self, stream: remoteBinaryStream):
        """
        Initializes an `encodingDefinition` from a stream.
        """
        code = stream.ReadByte()

        if code == 1:
            self.keyEncodingMethod = Empty.GUID
            self.valueEncodingMethod = Empty.GUID
            self.keyValueEncodingMethod = stream.ReadGuid()
            self.isKeyValueEncoded = True
        elif code == 2:
            self.keyEncodingMethod = stream.ReadGuid()
            self.valueEncodingMethod = stream.ReadGuid()
            self.keyValueEncodingMethod = Empty.GUID
            self.isKeyValueEncoded = False

        self.isFixedSizeEncoding = (
            self.keyValueEncodingMethod == encodingDefinition.FixedSizeIndividualGuid or
            self.keyEncodingMethod == encodingDefinition.FixedSizeIndividualGuid and
            self.valueEncodingMethod == encodingDefinition.FixedSizeIndividualGuid
        )

    def __initFromCombinedKeyValue(self, keyValueEncoding: UUID):
        """
        Initializes an `encodingDefinition` from a combined key/value encoding method with the provided `UUID`
        """

        self.keyEncodingMethod = Empty.GUID
        self.valueEncodingMethod = Empty.GUID
        self.keyValueEncodingMethod = keyValueEncoding
        self.isKeyValueEncoded = True
        
        self.isFixedSizeEncoding = keyValueEncoding == encodingDefinition.FixedSizeIndividualGuid
        
    def __initFromSeparateKeyValue(self, keyEncoding: UUID, valueEncoding: UUID):
        """
        Initializes an `encodingDefinition` from an encoding method that independently compresses the key and the value.
        """

        self.keyEncodingMethod = keyEncoding
        self.valueEncodingMethod = valueEncoding
        self.keyValueEncodingMethod = Empty.GUID
        self.isKeyValueEncoded = False
        
        self.isFixedSizeEncoding = (
            self.keyEncodingMethod == encodingDefinition.FixedSizeIndividualGuid and
            self.valueEncodingMethod == encodingDefinition.FixedSizeIndividualGuid
        )

    @property
    def IsKeyValueEncoded(self) -> bool:
        return self.isKeyValueEncoded

    @property
    def IsFixedSizeEncoding(self) -> bool:
        return self.isFixedSizeEncoding

    @property
    def KeyEncodingMethod(self) -> UUID:
        if self.isKeyValueEncoded:
            raise RuntimeError("Not valid for encoding method")

        return self.keyEncodingMethod

    @property
    def ValueEncodingMethod(self) -> UUID:
        if self.isKeyValueEncoded:
            raise RuntimeError("Not valid for encoding method")

        return self.valueEncodingMethod

    @property
    def KeyValueEncodingMethod(self) -> UUID:
        if not self.isKeyValueEncoded:
            raise RuntimeError("Not valid for encoding method")

        return self.keyValueEncodingMethod

    def ToString(self) -> str:
        if self.isKeyValueEncoded:
            return "{" + str(self.keyValueEncodingMethod) + "}";
        
        return "{" + str(self.keyEncodingMethod) + "} / {" + str(self.valueEncodingMethod) + "}";

    def Save(self, stream: remoteBinaryStream):
        if self.isKeyValueEncoded:
            stream.WriteByte(1)
            stream.WriteGuid(self.keyValueEncodingMethod)
        else:
            stream.WriteByte(2)
            stream.WriteGuid(self.keyEncodingMethod)
            stream.WriteGuid(self.valueEncodingMethod)
