#******************************************************************************************************
#  pointIDMatchFilter.py - Gbtc
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

from .matchFilterBase import matchFilterBase
from gsf.binaryStream import binaryStream
from gsf import Limits
from typing import Set, List, Optional
from uuid import UUID
import numpy as np

class pointIDMatchFilter(matchFilterBase):
    """
    Creates a match filter based lists of point IDs.
    """

    SnapTypeID = UUID("2034a3e3-f92e-4749-9306-b04dc36fd743")
    AllPoints: Optional[matchFilterBase] = None

    def __init__(self, pointIDs: Set[np.uint64]):
        """
        Creates a match filter from the set of points provided.
        """
        self.pointIDs = pointIDs

    @property
    def TypeID(self) -> UUID:
        """
        Gets the Guid uniquely defining this SNAPdb match filter type.
        """
        return pointIDMatchFilter.SnapTypeID


    def Save(self, stream: binaryStream):
        """
        Serializes the filter to a stream.
        """
        maxValue = max(self.pointIDs)
        targetUInt32 = maxValue <= Limits.MAXUINT32

        stream.WriteByte(1 if targetUInt32 else 2)
        stream.WriteUInt64(maxValue)
        stream.WriteInt32(len(self.pointIDs))

        if targetUInt32:
            for pointID in self.pointIDs:
                stream.WriteUInt32(pointID)
        else:
            for pointID in self.pointIDs:
                stream.WriteUInt64(pointID)

    @staticmethod
    def CreateFromList(pointIDs: List[np.uint64]):
        """
        Creates a match filter from the list of points provided.
        """
        return pointIDMatchFilter(set(pointIDs))
