//******************************************************************************************************
//  ArchiveFileSummaryExtensions'2.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  5/25/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace openHistorian.Engine
{
    /// <summary>
    /// Contains an immutable class of the current partition
    /// along with its most recent snapshot.
    /// </summary>
    public static class ArchiveTableSummaryExtension
    {
        /// <summary>
        /// Determines if this table might contain data for the provided times.
        /// </summary>
        /// <param name="summary">the table to search through.</param>
        /// <param name="startTime">the first time to search for</param>
        /// <param name="stopTime">the last time to search for</param>
        /// <returns></returns>
        public static bool Contains<TKey, TValue>(this ArchiveTableSummary<TKey, TValue> summary, ulong startTime, ulong stopTime)
            where TKey : HistorianKeyBase<TKey>, new()
            where TValue : class, new()
        {
            //If the archive file is empty, it will always be searched.  
            //Since this will likely never happen and has little performance 
            //implications, I have decided not to include logic that would exclude this case.
            return !(startTime > summary.LastKey.Timestamp || stopTime < summary.FirstKey.Timestamp);
        }

    }
}