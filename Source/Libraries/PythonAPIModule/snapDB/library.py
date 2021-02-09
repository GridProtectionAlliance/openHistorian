#******************************************************************************************************
#  library.py - Gbtc
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

from .snapTypeBase import snapTypeBase
from .encodingDefinition import encodingDefinition
from .keyValueEncoderBase import keyValueEncoderBase
from gsf import static_init
from typing import Optional, Dict
from uuid import UUID

@static_init
class library:
    """
    Registered library of SNAPdb types.
    """

    @classmethod
    def static_init(cls):
        cls.typeNameIDMap: Dict[str, UUID] = dict()
        cls.typeIDNameMap: Dict[UUID, str] = dict()
        cls.guidEncoderMap: Dict[UUID, keyValueEncoderBase] = dict()

    @classmethod
    def RegisterType(cls, snapType: snapTypeBase):
        typeName = type(snapType).__name__
        typeID = snapType.TypeID

        cls.typeNameIDMap[typeName] = typeID
        cls.typeIDNameMap[typeID] = typeName

    @classmethod
    def RegisterEncoder(cls, encoder: keyValueEncoderBase):
        if encoder.Definition.IsKeyValueEncoded:
            cls.guidEncoderMap[encoder.Definition.KeyValueEncodingMethod] = encoder
        else:
            raise RuntimeError("Separate key/value type encoding is not currently supported by Python API")

    @classmethod
    def LookupTypeName(cls, typeID: UUID) -> Optional[str]:
        if typeID in cls.typeIDNameMap:
            return cls.typeIDNameMap[typeID]

        return None

    @classmethod
    def LookupTypeID(cls, typeName: str) -> Optional[UUID]:
        if typeName in cls.typeNameIDMap:
            return cls.typeNameIDMap[typeName]

        return None

    @classmethod
    def LookupEncoder(cls, definition: encodingDefinition) -> Optional[keyValueEncoderBase]:
        if definition.IsKeyValueEncoded:
            encoderID = definition.KeyValueEncodingMethod

            if encoderID in cls.guidEncoderMap:
                return cls.guidEncoderMap[encoderID]

            return None
        else:
            raise RuntimeError("Separate key/value type encoding is not currently supported by Python API")
