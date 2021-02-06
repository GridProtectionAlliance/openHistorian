#******************************************************************************************************
#  openHistorian.py - Gbtc
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

from openHistorian.historianInstance import historianInstance
from openHistorian.historianKey import historianKey
from openHistorian.historianValue import historianValue
from snapDB.snapConnection import snapConnection
from snapDB.encodingDefinition import encodingDefinition
from gsf import override
from typing import Optional

class historianConnection(snapConnection[historianKey, historianValue]):
    """
    Defines API functionality for connecting to an openHistorian instance then
    reading and writing measurement data from the instance.

    This class is an instance of the `snapConnection` implemented for the
    openHistorian `historianKey` and `historianValue` SNAPdb types.
    """

    def __init__(self, hostAddress: str):
        super().__init__(hostAddress, historianKey(), historianValue())

    @override
    def OpenInstance(self, instanceName: str, definition: Optional[encodingDefinition] = None) -> historianInstance:
        return super().OpenInstance(instanceName, definition)