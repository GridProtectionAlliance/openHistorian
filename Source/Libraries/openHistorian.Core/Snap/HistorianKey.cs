//******************************************************************************************************
//  HistorianKey.cs - Gbtc
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
using GSF;
using GSF.IO;
using GSF.Snap.Types;

namespace openHistorian.Snap
{
    /// <summary>
    /// The standard key used for the historian.
    /// </summary>
    public class HistorianKey
        : TimestampPointIDBase<HistorianKey>
    {
        // TODO: Engine, not user, should accommodate incrementing EntryNumber for duplicates.
        /// <summary>
        /// The number of the entry. This allows for duplicate values to be stored using the same Timestamp and PointID.
        /// </summary>
        /// <remarks>
        /// When writing data, this property is managed by the historian engine. Do not change this value in your code.
        /// </remarks>
        public ulong EntryNumber;

        public override Guid GenericTypeGuid =>
            // {6527D41B-9D04-4BFA-8133-05273D521D46}
            new Guid(0x6527d41b, 0x9d04, 0x4bfa, 0x81, 0x33, 0x05, 0x27, 0x3d, 0x52, 0x1d, 0x46);

        public override int Size => 24;

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
        /// Sets the key to the default values.
        /// </summary>
        public override void Clear()
        {
            Timestamp = 0;
            PointID = 0;
            EntryNumber = 0;
        }

        public override void Read(BinaryStreamBase stream)
        {
            Timestamp = stream.ReadUInt64();
            PointID = stream.ReadUInt64();
            EntryNumber = stream.ReadUInt64();
        }

        public override void Write(BinaryStreamBase stream)
        {
            stream.Write(Timestamp);
            stream.Write(PointID);
            stream.Write(EntryNumber);
        }

        public override void CopyTo(HistorianKey destination)
        {
            destination.Timestamp = Timestamp;
            destination.PointID = PointID;
            destination.EntryNumber = EntryNumber;
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

        public override unsafe int CompareTo(byte* stream)
        {
            if (Timestamp < *(ulong*)stream)
                return -1;
            if (Timestamp > *(ulong*)stream)
                return 1;
            if (PointID < *(ulong*)(stream + 8))
                return -1;
            if (PointID > *(ulong*)(stream + 8))
                return 1;
            if (EntryNumber < *(ulong*)(stream + 16))
                return -1;
            if (EntryNumber > *(ulong*)(stream + 16))
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
        /// Conveniently type cast the Timestamp as <see cref="DateTime"/>.
        /// </summary>
        /// <remarks>
        /// Assignments expected to be in UTC.
        /// </remarks>
        public DateTime TimestampAsDate
        {
            get => new DateTime((long)Timestamp, DateTimeKind.Utc);
            set => Timestamp = (ulong)value.Ticks;
        }

        /// <summary>
        /// Gets or sets timestamp restricted to millisecond resolution.
        /// </summary>
        public ulong MillisecondTimestamp
        {
            get => Timestamp / Ticks.PerMillisecond * Ticks.PerMillisecond;
            set => Timestamp = value / Ticks.PerMillisecond * Ticks.PerMillisecond;
        }

        public override string ToString()
        {
            if (Timestamp <= (ulong)DateTime.MaxValue.Ticks)
                return TimestampAsDate.ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "/" + PointID;
            return Timestamp.ToString() + "/" + PointID;

        }

        #region [ Optional Overrides ]

        // Read(byte*)
        // Write(byte*)
        // IsLessThan(T)
        // IsEqualTo(T)
        // IsGreaterThan(T)
        // IsLessThanOrEqualTo(T)
        // IsBetween(T,T)

        public override unsafe void Read(byte* stream)
        {
            Timestamp = *(ulong*)stream;
            PointID = *(ulong*)(stream + 8);
            EntryNumber = *(ulong*)(stream + 16);
        }
        public override unsafe void Write(byte* stream)
        {
            *(ulong*)stream = Timestamp;
            *(ulong*)(stream + 8) = PointID;
            *(ulong*)(stream + 16) = EntryNumber;
        }
        public override bool IsLessThan(HistorianKey right)
        {
            if (Timestamp != right.Timestamp)
                return Timestamp < right.Timestamp;

            //Implied left.Timestamp == right.Timestamp
            if (PointID != right.PointID)
                return PointID < right.PointID;

            //Implied left.EntryNumber == right.EntryNumber
            return EntryNumber < right.EntryNumber;
        }
        public override bool IsEqualTo(HistorianKey right)
        {
            return Timestamp == right.Timestamp && PointID == right.PointID && EntryNumber == right.EntryNumber;
        }
        public override bool IsGreaterThan(HistorianKey right)
        {
            if (Timestamp != right.Timestamp)
                return Timestamp > right.Timestamp;

            //Implied left.Timestamp == right.Timestamp
            if (PointID != right.PointID)
                return PointID > right.PointID;

            //Implied left.EntryNumber == right.EntryNumber
            return EntryNumber > right.EntryNumber;
        }
        public override bool IsGreaterThanOrEqualTo(HistorianKey right)
        {
            if (Timestamp != right.Timestamp)
                return Timestamp > right.Timestamp;

            //Implied left.Timestamp == right.Timestamp
            if (PointID != right.PointID)
                return PointID > right.PointID;

            //Implied left.EntryNumber == right.EntryNumber
            return EntryNumber >= right.EntryNumber;
        }

        //public override bool IsBetween(HistorianKey lowerBounds, HistorianKey upperBounds)
        //{
        //    
        //}

        //public override SortedTreeTypeMethods<HistorianKey> CreateValueMethods()
        //{
        //    
        //}

        #endregion
    }
}