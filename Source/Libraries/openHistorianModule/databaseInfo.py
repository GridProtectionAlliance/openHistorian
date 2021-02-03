#******************************************************************************************************
#  databaseInfo.py - Gbtc
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
from encodingDefinition import encodingDefinition
from library import library
from typing import List
from uuid import UUID
import numpy as np

class databaseInfo:
    """
    Defines details for an openHistorian client database instance.
    """

    def __init__(self, stream: remoteBinaryStream):
        version = stream.ReadByte()

        if version != 1:
            raise RuntimeError("Unknown openHistorian version: " + str(version))

        self.databaseName = stream.ReadString().strip().upper()
        self.keyTypeID = stream.ReadGuid()
        self.valueTypeID = stream.ReadGuid()
        
        count = stream.ReadInt32()
        definitions = []

        for i in range(count):
            definitions.append(encodingDefinition(stream = stream))

        self.streamingModes = definitions
        self.keyType = library.LookupType(self.keyTypeID)
        self.valueType = library.LookupType(self.valueTypeID)

    @property
    def Version(self) -> int:
        return self.version

    @property
    def DatabaseName(self) -> str:
        return self.databaseName

    @property
    def KeyTypeID(self) -> UUID:
        return self.keyTypeID

    @property
    def ValueTypeID(self) -> UUID:
        return self.valueTypeID

    @property
    def KeyType(self) -> str:
        return self.keyType

    @property
    def ValueType(self) -> str:
        return self.valueType

    @property
    def SupportedStreamingModes(self) -> List[encodingDefinition]:
        return self.streamingModes