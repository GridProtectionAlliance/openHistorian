#******************************************************************************************************
#  matchFilterBase.py - Gbtc
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

from gsf.binaryStream import binaryStream
from abc import ABC, abstractmethod
from uuid import UUID

class matchFilterBase(ABC):
    """
    Represents a filter that matches based on the keys and values.
    """

    @property
    @abstractmethod
    def TypeID(self) -> UUID:
        """
        Gets the Guid uniquely defining this SNAPdb match filter type.
        """

    @abstractmethod
    def Save(self, stream: binaryStream):
        """
        Serializes the filter to a stream.
        """

    # The following code is only for server side operations, so client API does not need to implement

    # Base type generics
    #from typing import TypeVar, Generic
    #TKey = TypeVar('TKey', bound=snapTypeBase)
    #TValue = TypeVar('TValue', bound=snapTypeBase)
    #, Generic[TKey, TValue]

    #@abstractmethod
    #def Contains(key: TKey, value: TValue) -> bool:
    #    """
    #    Determines if a Key/value is contained in the filter.
    #    """
