#******************************************************************************************************
#  seekFilterBase.py - Gbtc
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

class seekFilterBase(ABC):
    """
    Represents a filter that is based on a series of ranges of the key value.
    """

    @property
    @abstractmethod
    def TypeID(self) -> UUID:
        """
        Gets the Guid uniquely defining this SNAPdb seek filter type.
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
    #, Generic[TKey]

    #@property
    #@abstractmethod
    #def StartOfFrame(self) -> TKey:
    #    """
    #    Gets the start of the frame to search, inclusive.
    #    """
    
    #@StartOfFrame.setter
    #@abstractmethod
    #def StartOfFrame(self, value: TKey):
    #    """
    #    Sets the start of the frame to search, inclusive.
    #    """

    #@property
    #@abstractmethod
    #def EndOfFrame(self) -> TKey:
    #    """
    #    Gets the end of the frame to search, inclusive.
    #    """
    
    #@EndOfFrame.setter
    #@abstractmethod
    #def EndOfFrame(self, value: TKey):
    #    """
    #    Sets the end of the frame to search, inclusive.
    #    """

    #@property
    #@abstractmethod
    #def StartOfRange(self) -> TKey:
    #    """
    #    Gets the start of the range to search, inclusive.
    #    """
    
    #@StartOfRange.setter
    #@abstractmethod
    #def StartOfRange(self, value: TKey):
    #    """
    #    Sets the start of the range to search, inclusive.
    #    """

    #@property
    #@abstractmethod
    #def EndOfRange(self) -> TKey:
    #    """
    #    Gets the end of the range to search, inclusive.
    #    """
    
    #@EndOfFrame.setter
    #@abstractmethod
    #def EndOfRange(self, value: TKey):
    #    """
    #    Sets the end of the range to search, inclusive.
    #    """

    #@abstractmethod
    #def Reset(self):
    #    """
    #    Resets the iterative nature of the filter. 

    #    Since a time filter is a set of date ranges, this will reset the frame so a
    #    call to `NextWindow` will return the first window of the sequence.
    #    """

    #@abstractmethod
    #def NextWindow(self) -> bool:
    #    """
    #    Gets the next search window.

    #    Returns
    #    -------
    #    `True` if window exists; otherwise, `False` if finished.
    #    """
