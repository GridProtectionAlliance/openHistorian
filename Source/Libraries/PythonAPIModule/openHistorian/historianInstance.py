#******************************************************************************************************
#  historianInstance.py - Gbtc
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
from snapDB.snapClientDatabase import snapClientDatabase
from snapDB.databaseInfo import databaseInfo
from gsf.binaryStream import binaryStream
from typing import Callable

class historianInstance(snapClientDatabase[historianKey, historianValue]):
    """
    Represents a `snapClientDatabase` instance for a `historianKey` and `historianValue`.
    """
    def __init__(self, stream: binaryStream, info: databaseInfo, onDispose: Callable[[], None]):
        super().__init__(stream, info, onDispose, historianKey(), historianValue())
