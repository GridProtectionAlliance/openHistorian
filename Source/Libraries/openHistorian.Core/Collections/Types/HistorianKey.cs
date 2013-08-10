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

using GSF.IO;

namespace openHistorian.Collections
{
    public class HistorianKey
        : HistorianKeyBase<HistorianKey>
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

        public override void CopyTo(HistorianKey other)
        {
            other.Timestamp = Timestamp;
            other.PointID = PointID;
            other.EntryNumber = EntryNumber;
        }

        public override void SetMin()
        {
            Timestamp = 0;
            PointID = 0;
            EntryNumber = 0;
        }

        public override void SetMax()
        {
            Timestamp = ulong.MaxValue;
            PointID = ulong.MaxValue;
            EntryNumber = ulong.MaxValue;
        }

        public override void Write(BinaryStreamBase stream)
        {
            stream.Write(Timestamp);
            stream.Write(PointID);
            stream.Write(EntryNumber);
        }

        public override void Read(BinaryStreamBase stream)
        {
            Timestamp = stream.ReadUInt64();
            PointID = stream.ReadUInt64();
            EntryNumber = stream.ReadUInt64();
        }

        public override void WriteCompressed(BinaryStreamBase stream, HistorianKey previousKey)
        {
            //stream.Write(Timestamp);
            //stream.Write(PointID);
            //stream.Write(EntryNumber);
            stream.Write7Bit(previousKey.Timestamp ^ Timestamp);
            stream.Write7Bit(previousKey.PointID ^ PointID);
            stream.Write7Bit(previousKey.EntryNumber ^ EntryNumber);
        }

        public override void ReadCompressed(BinaryStreamBase stream, HistorianKey previousKey)
        {
            //Timestamp = stream.ReadUInt64();
            //PointID = stream.ReadUInt64();
            //EntryNumber = stream.ReadUInt64();
            Timestamp = stream.Read7BitUInt64() ^ previousKey.Timestamp;
            PointID = stream.Read7BitUInt64() ^ previousKey.PointID;
            EntryNumber = stream.Read7BitUInt64() ^ previousKey.EntryNumber;
        }

        public override void Clear()
        {
            Timestamp = 0;
            PointID = 0;
            EntryNumber = 0;
        }

        public HistorianKey Clone()
        {
            HistorianKey key = new HistorianKey();
            key.Timestamp = Timestamp;
            key.PointID = PointID;
            key.EntryNumber = EntryNumber;
            return key;
        }
    }
}