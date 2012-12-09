////******************************************************************************************************
////  BPlusTreeLongLong.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  5/1/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using openHistorian.IO;

//namespace openHistorian.Collections.Specialized
//{
//    public class BPlusTreeLongLong : BPlusTreeLeafNodeBase<long, long>
//    {
//        public BPlusTreeLongLong(IBinaryStream stream)
//            : base(stream)
//        {
//        }

//        public BPlusTreeLongLong(IBinaryStream stream, int blockSize)
//            : base(stream, blockSize)
//        {
//        }

//        protected override void SaveValue(long value, IBinaryStream stream)
//        {
//            stream.Write(value);
//        }

//        protected override long LoadValue(IBinaryStream stream)
//        {
//            return stream.ReadInt64();
//        }

//        protected override int SizeOfValue()
//        {
//            return 8;
//        }

//        protected override int SizeOfKey()
//        {
//            return 8;
//        }

//        protected override void SaveKey(long value, IBinaryStream stream)
//        {
//            stream.Write(value);
//        }

//        protected override long LoadKey(IBinaryStream stream)
//        {
//            return stream.ReadInt64();
//        }

//        protected override int CompareKeys(long first, long last)
//        {
//            return first.CompareTo(last);
//        }

//        protected override int CompareKeys(long first, IBinaryStream stream)
//        {
//            return first.CompareTo(stream.ReadInt64());
//        }

//        //unsafe protected override bool LeafNodeSeekToKey(long key, out int offset)
//        //{
//        //    int leafStructureSize = m_leafStructureSize;
//        //    long startAddress = m_currentNode * m_blockSize + NodeHeader.Size;
//        //    m_leafNodeStream.Position = startAddress;

//        //    byte* pos;
//        //    byte* start;
//        //    int currentIndex;
//        //    int length;
//        //    m_leafNodeStream.GetRawDataBlock(false, out start, out currentIndex, out length);
//        //    if (length < m_blockSize - NodeHeader.Size)
//        //        throw new Exception();

//        //    start += currentIndex;

//        //    int min = 0;
//        //    int max = m_childCount - 1;

//        //    while (min <= max)
//        //    {
//        //        int mid = min + (max - min >> 1);
//        //        pos = start + leafStructureSize * mid;

//        //        //int tmpKey = LeafNodeCompareKeys(key, m_stream);
//        //        if (key == *(long*)pos)
//        //        {
//        //            offset = NodeHeader.Size + leafStructureSize * mid;
//        //            return true;
//        //        }
//        //        if (key > *(long*)pos)
//        //            min = mid + 1;
//        //        else
//        //            max = mid - 1;
//        //    }
//        //    offset = NodeHeader.Size + leafStructureSize * min;
//        //    return false;
//        //}

//        //unsafe protected override bool InternalNodeSeekToKey(long key, out int offset)
//        //{
//        //    int internalStructureSize = m_internalNodeStructureSize;
//        //    long startAddress = m_internalNodeCurrentNode * m_blockSize + NodeHeader.Size + sizeof(uint);

//        //    m_internalNodeStream.Position = startAddress;

//        //    byte* pos;
//        //    byte* start;
//        //    int currentIndex;
//        //    int length;
//        //    m_internalNodeStream.GetRawDataBlock(false, out start, out currentIndex, out length);
//        //    if (length < m_blockSize - NodeHeader.Size - sizeof(uint))
//        //        throw new Exception();

//        //    start += currentIndex;

//        //    int min = 0;
//        //    int max = m_internalNodeChildCount - 1;

//        //    while (min <= max)
//        //    {
//        //        int mid = min + (max - min >> 1);
//        //        pos = start + internalStructureSize * mid;

//        //        if (key == *(long*)pos)
//        //        {
//        //            offset = NodeHeader.Size + sizeof(uint) + internalStructureSize * mid;
//        //            return true;
//        //        }
//        //        if (key > *(long*)pos)
//        //            min = mid + 1;
//        //        else
//        //            max = mid - 1;
//        //    }

//        //    offset = NodeHeader.Size + sizeof(uint) + internalStructureSize * min;
//        //    return false;
//        //}

//        protected override Guid FileType
//        {
//            get
//            {
//                return Guid.Empty;
//            }
//        }
//    }
//}
