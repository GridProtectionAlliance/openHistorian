//******************************************************************************************************
//  TimestampBase'1.cs - Gbtc
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
//  04/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace GSF.Snap.Types
{
    /// <summary>
    /// Base implementation of a historian key. 
    /// These are the required functions that are 
    /// necessary for the historian engine to operate
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class TimestampBase<TKey>
        : SnapTypeBase<TKey>, IHasTimestampField
        where TKey : SnapTypeBase<TKey>, new()
    {
        /// <summary>
        /// The timestamp stored as native ticks. 
        /// </summary>
        public ulong Timestamp;

        /// <summary>
        /// Attempts to get the timestamp field of a point. This function might fail if the datetime field
        /// is not able to be converted.
        /// </summary>
        /// <param name="timestamp">an output field of the timestamp</param>
        /// <returns>True if a timestamp could be parsed. False otherwise.</returns>
        bool IHasTimestampField.TryGetDateTime(out DateTime timestamp)
        {
            if (Timestamp <= (ulong)DateTime.MaxValue.Ticks)
            {
                timestamp = new DateTime((long)Timestamp);
                return true;
            }
            timestamp = DateTime.MinValue;
            return false;

        }
    }
}