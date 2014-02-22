//******************************************************************************************************
//  HistorianKey.cs - Gbtc
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
//  4/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.SortedTreeStore;
using GSF.SortedTreeStore.Engine;
using GSF.SortedTreeStore.Net.Compression;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Tree.TreeNodes;

namespace openHistorian.Collections
{
    /// <summary>
    /// The standard key used for the historian.
    /// </summary>
    public class HistorianKey
        : EngineKeyBase<HistorianKey>
    {
        // These values are inherited from base class:
        ///// <summary>
        ///// The timestamp stored as native ticks. 
        ///// </summary>
        //public ulong Timestamp;

        ///// <summary>
        ///// The id number of the point.
        ///// </summary>
        //public ulong PointID;

        // TODO: Engine, not user, should accommodate incrementing EntryNumber for duplicates.
        /// <summary>
        /// The number of the entry. This allows for duplicate values to be stored using the same Timestamp and PointID.
        /// </summary>
        /// <remarks>
        /// When writing data, this property is managed by the historian engine. Do not change this value in your code.
        /// </remarks>
        public ulong EntryNumber;

        /// <summary>
        /// Sets all of the values in this class to their minimum value
        /// </summary>
        public void SetMin()
        {
            Timestamp = 0;
            PointID = 0;
            EntryNumber = 0;
        }

        /// <summary>
        /// Sets all of the values in this class to their maximum value
        /// </summary>
        public void SetMax()
        {
            Timestamp = ulong.MaxValue;
            PointID = ulong.MaxValue;
            EntryNumber = ulong.MaxValue;
        }

        /// <summary>
        /// Sets the key to the default values.
        /// </summary>
        public void Clear()
        {
            Timestamp = 0;
            PointID = 0;
            EntryNumber = 0;
        }

        /// <summary>
        /// Compares the current instance to <see cref="other"/>.
        /// </summary>
        /// <param name="other">the key to compare to</param>
        /// <returns></returns>
        public override int CompareTo(HistorianKey other)
        {
            if (Timestamp < other.Timestamp)
                return -1;
            if (Timestamp > other.Timestamp)
                return 1;
            if (PointID < other.PointID)
                return -1;
            if (PointID > other.PointID)
                return 1;
            if (EntryNumber < other.EntryNumber)
                return -1;
            if (EntryNumber > other.EntryNumber)
                return 1;
            return 0;
        }

        /// <summary>
        /// Creates a clone of the HistorianKey
        /// </summary>
        /// <returns></returns>
        public HistorianKey Clone()
        {
            HistorianKey key = new HistorianKey();
            key.Timestamp = Timestamp;
            key.PointID = PointID;
            key.EntryNumber = EntryNumber;
            return key;
        }

        /// <summary>
        /// Conviently type cast the Timestamp as <see cref="DateTime"/>.
        /// </summary>
        public DateTime TimestampAsDate
        {
            get
            {
                return new DateTime((long)Timestamp);
            }
            set
            {
                Timestamp = (ulong)value.Ticks;
            }
        }

        public override SortedTreeKeyMethodsBase<HistorianKey> CreateKeyMethods()
        {
            return new KeyMethodsHistorianKey();
        }

        public override void RegisterCustomKeyImplementations()
        {
            CreateHistorianCompressionDelta.Register();
            CreateHistorianCompressionTs.Register();
            CreateHistorianCompressedStream.Register();
            CreateHistorianPointCollection.Register();
        }

        public override string ToString()
        {
            return TimestampAsDate.ToString("MM/dd/yyyy HH:mm:ss.fffffff") + " " + PointID.ToString();
        }
    }
}