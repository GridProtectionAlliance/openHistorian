//******************************************************************************************************
//  SortedPointBuffer.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/26/2014 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF.Snap.Collection;

namespace openHistorian.Snap
{
    /// <summary>
    /// Represents a sorted point buffer that can properly handle or remove duplicates.
    /// </summary>
    public class SortedPointBuffer :
        SortedPointBuffer<HistorianKey, HistorianValue>
    {
        /// <summary>
        /// Creates a new <see cref="SortedPointBuffer"/>.
        /// </summary>
        /// <param name="capacity">The maximum number of items that can be stored in the buffer.</param>
        /// <param name="removeDuplicates">Flag that specifies if buffer should remove duplicate key values upon reading.</param>
        public SortedPointBuffer(int capacity, bool removeDuplicates)
            : base(capacity, removeDuplicates ? (Action<HistorianKey, HistorianKey>)null : (left, right) =>
            {
                right.EntryNumber = left.EntryNumber + 1;
            })
        {
        }
    }
}
