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

from snapTypeBase import snapTypeBase
from common import static_init
from uuid import UUID

# Import SNAPdb types for manual registration
from historianKey import historianKey
from historianValue import historianValue

@static_init
class library:
    """
    Registered library of SNAPdb types.
    """

    # Future versions could dyamically scan for modules inheriting `snapTypeBase`
    # and automatically register them, for now this class just manually registers
    # `historianKey` and `historianValue` for use by the `openHistorian` API

    @classmethod
    def static_init(cls):
        cls.typeNameGuidMap = dict()
        cls.typeGuidNameMap = dict()

        cls.Register(historianKey())
        cls.Register(historianValue())

    @classmethod
    def Register(cls, snapType: snapTypeBase):
        typeName = type(snapType).__name__
        typeGuid = snapType.GenericTypeGuid

        cls.typeNameGuidMap[typeName] = typeGuid
        cls.typeGuidNameMap[typeGuid] = typeName

    @classmethod
    def LookupType(cls, typeGuid: UUID) -> str:
        return cls.typeGuidNameMap[typeGuid]