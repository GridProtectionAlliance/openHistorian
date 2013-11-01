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
using GSF.IO;
using openHistorian.Collections.Generic;

namespace openHistorian.Collections
{
    /// <summary>
    /// The standard key used for the historian.
    /// </summary>
    public class HistorianKey
        : HistorianKeyBase<HistorianKey>, ISortedTreeKey<HistorianKey>
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
        /// Copies the data from this class to <see cref="other"/>
        /// </summary>
        /// <param name="other">the destination of the copy</param>
        public override void CopyTo(HistorianKey other)
        {
            other.Timestamp = Timestamp;
            other.PointID = PointID;
            other.EntryNumber = EntryNumber;
        }

        /// <summary>
        /// Sets all of the values in this class to their minimum value
        /// </summary>
        public override void SetMin()
        {
            Timestamp = 0;
            PointID = 0;
            EntryNumber = 0;
        }

        /// <summary>
        /// Sets all of the values in this class to their maximum value
        /// </summary>
        public override void SetMax()
        {
            Timestamp = ulong.MaxValue;
            PointID = ulong.MaxValue;
            EntryNumber = ulong.MaxValue;
        }

        /// <summary>
        /// Serializes this key to the <see cref="stream"/> in a fixed sized method.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public override void Write(BinaryStreamBase stream)
        {
            stream.Write(Timestamp);
            stream.Write(PointID);
            stream.Write(EntryNumber);
        }

        /// <summary>
        /// Reads data from the provided <see cref="stream"/> in a fixed size method.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        public override void Read(BinaryStreamBase stream)
        {
            Timestamp = stream.ReadUInt64();
            PointID = stream.ReadUInt64();
            EntryNumber = stream.ReadUInt64();
        }

        /// <summary>
        /// Serializes this key to the <see cref="stream"/> in a condensed method.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="previousKey">the previous value that was serialized</param>
        public override void WriteCompressed(BinaryStreamBase stream, HistorianKey previousKey)
        {
            //stream.Write(Timestamp);
            //stream.Write(PointID);
            //stream.Write(EntryNumber);
            stream.Write7Bit(previousKey.Timestamp ^ Timestamp);
            stream.Write7Bit(previousKey.PointID ^ PointID);
            stream.Write7Bit(previousKey.EntryNumber ^ EntryNumber);
        }

        /// <summary>
        /// Reads data from the provided <see cref="stream"/> in a condensed method.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        /// <param name="previousKey">the previous value that was serialized</param>
        public override void ReadCompressed(BinaryStreamBase stream, HistorianKey previousKey)
        {
            //Timestamp = stream.ReadUInt64();
            //PointID = stream.ReadUInt64();
            //EntryNumber = stream.ReadUInt64();
            Timestamp = stream.Read7BitUInt64() ^ previousKey.Timestamp;
            PointID = stream.Read7BitUInt64() ^ previousKey.PointID;
            EntryNumber = stream.Read7BitUInt64() ^ previousKey.EntryNumber;
        }

        /// <summary>
        /// Sets the key to the default values.
        /// </summary>
        public override void Clear()
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

        public override TreeKeyMethodsBase<HistorianKey> CreateKeyMethods()
        {
            return new KeyMethodsHistorianKey();
        }
    }
}