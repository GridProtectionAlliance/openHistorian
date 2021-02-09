#******************************************************************************************************
#  __init__.py - Gbtc
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
#  02/06/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from .historianKey import historianKey
from .historianValue import historianValue
from .historianKeyValueEncoder import historianKeyValueEncoder
from snapDB.library import library
from snapDB.fixedSizeKeyValueEncoder import fixedSizeKeyValueEncoder
from gsf import static_init

@static_init
class snapDBTypeRegistration:
    """
    openHistorian SNAPdb type registration.
    """

    @classmethod
    def static_init(cls):
        # Register known SNAPdb key/value types. Future versions could dynamically scan
        # for modules inheriting `snapTypeBase` and automatically register them. For now
        # this class just manually registers `historianKey` and `historianValue` for use
        # by the `openHistorian` API
        library.RegisterType(historianKey())
        library.RegisterType(historianValue())

        # Register known SNAPdb key/value encoders. Future versions could dynamically scan
        # for modules inheriting `keyValueEncoderBase` and automatically register them. For
        # now this class just manually registers `historianKeyValueEncoder` and the generic
        # `fixedSizeKeyValueEncoder` for `historianKey` and `historianValue` for use by the
        # `openHistorian` API
        library.RegisterEncoder(historianKeyValueEncoder())
        library.RegisterEncoder(fixedSizeKeyValueEncoder(historianKey(), historianValue()))
