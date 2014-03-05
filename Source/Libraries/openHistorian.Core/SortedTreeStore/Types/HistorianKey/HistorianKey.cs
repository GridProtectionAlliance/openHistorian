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
using System.Collections;
using GSF.IO;
using GSF.SortedTreeStore.Encoding;
using GSF.SortedTreeStore.Net.Compression;
using GSF.SortedTreeStore.Tree.TreeNodes;
using GSF.SortedTreeStore.Types;
using openHistorian.SortedTreeStore.Types.CustomCompression.Ts;

namespace openHistorian.Collections
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

        public override Guid GenericTypeGuid
        {
            get
            {
                // {6527D41B-9D04-4BFA-8133-05273D521D46}
                return new Guid(0x6527d41b, 0x9d04, 0x4bfa, 0x81, 0x33, 0x05, 0x27, 0x3d, 0x52, 0x1d, 0x46);
            }
        }

        public override int Size
        {
            get
            {
                return 24;
            }
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

        public override IEnumerable GetEncodingMethods()
        {
            var list = new ArrayList();
            list.Add(new CreateTsCombinedEncoding());
            list.Add(new CreateHistorianFixedSizeCombinedEncoding());
            list.Add(new CreateHistorianFixedSizeDualSingleEncoding());
            //list.Add(new CreateHistorianCompressionTs());
            return list;

            //CreateHistorianCompressionDelta.Register();
            //CreateHistorianCompressionTs.Register();
            CreateHistorianCompressedStream.Register();
            //CreateHistorianPointCollection.Register();
        }

        public override string ToString()
        {
            return TimestampAsDate.ToString("MM/dd/yyyy HH:mm:ss.fffffff") + " " + PointID.ToString();
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

            //Implide left.Timestamp == right.Timestamp
            if (PointID != right.PointID)
                return PointID < right.PointID;

            //Implide left.EntryNumber == right.EntryNumber
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

            //Implide left.Timestamp == right.Timestamp
            if (PointID != right.PointID)
                return PointID > right.PointID;

            //Implide left.EntryNumber == right.EntryNumber
            return EntryNumber > right.EntryNumber;
        }
        public override bool IsGreaterThanOrEqualTo(HistorianKey right)
        {
            if (Timestamp != right.Timestamp)
                return Timestamp > right.Timestamp;

            //Implide left.Timestamp == right.Timestamp
            if (PointID != right.PointID)
                return PointID > right.PointID;

            //Implide left.EntryNumber == right.EntryNumber
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