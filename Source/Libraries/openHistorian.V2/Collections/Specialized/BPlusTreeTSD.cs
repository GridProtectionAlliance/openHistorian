//******************************************************************************************************
//  BPlusTreeTSD.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  5/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using openHistorian.V2.Collections.BPlusTreeTypes;
using openHistorian.V2.IO;

namespace openHistorian.V2.Collections.Specialized
{
    public class BPlusTreeTSD : BPlusTreeLeafNodeBase<DateTimeLong, IntegerFloat>
    {

        public BPlusTreeTSD(IBinaryStream stream)
            : base(stream)
        {
        }

        public BPlusTreeTSD(IBinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
        }

        protected override void SaveValue(IntegerFloat value, IBinaryStream stream)
        {
            value.SaveValue(stream);
        }

        protected override IntegerFloat LoadValue(IBinaryStream stream)
        {
            IntegerFloat value = default(IntegerFloat);
            value.LoadValue(stream);
            return value;
        }

        protected override int SizeOfValue()
        {
            return 8;
        }

        protected override int SizeOfKey()
        {
            return 16;
        }

        protected override void SaveKey(DateTimeLong value, IBinaryStream stream)
        {
            value.SaveValue(stream);
        }

        protected override DateTimeLong LoadKey(IBinaryStream stream)
        {
            DateTimeLong value = default(DateTimeLong);
            value.LoadValue(stream);
            return value;
        }

        protected override int CompareKeys(DateTimeLong first, DateTimeLong last)
        {
            return first.CompareTo(last);
        }

        protected override int CompareKeys(DateTimeLong first, IBinaryStream stream)
        {
            return first.CompareToStream(stream);
        }

        protected override Guid FileType
        {
            get
            {
                return Guid.Empty;
            }
        }

        //unsafe protected override bool LeafNodeSeekToKey(KeyType key, out int offset)
        //{
        //    int leafStructureSize = m_leafStructureSize;
        //    long startAddress = m_currentNode * m_blockSize + NodeHeader.Size;
        //    m_leafNodeStream.Position = startAddress;

        //    byte* pos;
        //    byte* start;
        //    int currentIndex;
        //    int length;
        //    m_leafNodeStream.GetRawDataBlock(false, out start, out currentIndex, out length);
        //    if (length < m_blockSize - NodeHeader.Size)
        //        throw new Exception();

        //    start += currentIndex;

        //    int min = 0;
        //    int max = m_childCount - 1;

        //    long key1 = key.Time.Ticks;
        //    long key2 = key.Key;

        //    while (min <= max)
        //    {
        //        int mid = min + (max - min >> 1);
        //        pos = start + leafStructureSize * mid;

        //        //int tmpKey = LeafNodeCompareKeys(key, m_stream);
        //        if (key1 == *(long*)pos && key2 == *(long*)(pos + 8))
        //        {
        //            offset = NodeHeader.Size + leafStructureSize * mid;
        //            return true;
        //        }
        //        if (key1 > *(long*)pos || (key1 == *(long*)pos && key2 > *(long*)(pos + 8)))
        //            min = mid + 1;
        //        else
        //            max = mid - 1;
        //    }
        //    offset = NodeHeader.Size + leafStructureSize * min;
        //    return false;
        //}

        //unsafe protected override bool InternalNodeSeekToKey(KeyType key, out int offset)
        //{
        //    int internalStructureSize = m_internalNodeStructureSize;
        //    long startAddress = m_internalNodeCurrentNode * m_blockSize + NodeHeader.Size + sizeof(uint);

        //    m_internalNodeStream.Position = startAddress;

        //    byte* pos;
        //    byte* start;
        //    int currentIndex;
        //    int length;
        //    m_internalNodeStream.GetRawDataBlock(false, out start, out currentIndex, out length);
        //    if (length < m_blockSize - NodeHeader.Size - sizeof(uint))
        //        throw new Exception();

        //    start += currentIndex;

        //    int min = 0;
        //    int max = m_internalNodeChildCount - 1;

        //    long key1 = key.Time.Ticks;
        //    long key2 = key.Key;

        //    while (min <= max)
        //    {
        //        int mid = min + (max - min >> 1);
        //        pos = start + internalStructureSize * mid;

        //        if (key1 == *(long*)pos && key2 == *(long*)(pos + 8))
        //        {
        //            offset = NodeHeader.Size + sizeof(uint) + internalStructureSize * mid;
        //            return true;
        //        }
        //        if (key1 > *(long*)pos || (key1 == *(long*)pos && key2 > *(long*)(pos + 8)))
        //            min = mid + 1;
        //        else
        //            max = mid - 1;
        //    }

        //    offset = NodeHeader.Size + sizeof(uint) + internalStructureSize * min;
        //    return false;
        //}


    }
}
