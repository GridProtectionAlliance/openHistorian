#******************************************************************************************************
#  snapTypeBase.py - Gbtc
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

from gsf.binaryStream import binaryStream
from abc import ABC, abstractmethod
from uuid import UUID

class snapTypeBase(ABC):
    """
    Abstract base class used as a key or value type in a SNAPdb sorted tree. 
    """

    @property
    @abstractmethod
    def TypeID(self) -> UUID:
        """
        Gets the Guid uniquely defining this SNAPdb type. 
        """

    @property
    @abstractmethod
    def Size(self) -> int:
        """
        Gets the size of this SNAPdb type when serialized.
        """
 
    @abstractmethod
    def SetMin(self):
        """
        Sets the provided SNAPdb type to its minimum value.
        """

    @abstractmethod
    def SetMax(self):
        """
        Sets the provided SNAPdb type to its maximum value.
        """
 
    @abstractmethod
    def Clear(self):
        """
        Clears the SNAPdb type.
        """

    @abstractmethod
    def Read(self, stream: binaryStream):
        """
        Reads this SNAPdb type from the stream.
        """

    @abstractmethod
    def Write(self, stream: binaryStream):
        """
        Writes this SNAPdb type to the stream.
        """

    @abstractmethod
    def CopyTo(self, destination: "snapTypeBase"):
        """
        Copies this SNAPdb type to the `destination`
        """

    @abstractmethod
    def CompareTo(self, other: "snapTypeBase"):
        """
        Compares this SNAPdb type to the `other`
        """
