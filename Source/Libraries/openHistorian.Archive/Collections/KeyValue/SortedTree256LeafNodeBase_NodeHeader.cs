//******************************************************************************************************
//  SortedTree256LeafNodeBase_NodeHeader.cs - Gbtc
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
//  4/7/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using openHistorian.IO;

namespace openHistorian.Collections.KeyValue
{
    internal abstract partial class SortedTree256LeafNodeBase
    {

        /// <summary>
        /// Assists in the read/write operations of the header of a node.
        /// </summary>
        struct NodeHeader
        {
            public const int Size = 21;
            public int NodeRecordCount;
            public long LeftSiblingNodeIndex;
            public long RightSiblingNodeIndex;

            public NodeHeader(BinaryStreamBase stream, int blockSize, long nodeIndex)
            {
                stream.Position = blockSize * nodeIndex;
                if (stream.ReadByte() != 0)
                    throw new Exception("The current node is not a leaf node."); 
                NodeRecordCount = stream.ReadInt32();
                LeftSiblingNodeIndex = stream.ReadInt64();
                RightSiblingNodeIndex = stream.ReadInt64();
            }
            public void Save(BinaryStreamBase stream, int blockSize, long nodeIndex)
            {
                stream.Position = blockSize * nodeIndex;
                stream.Write((byte)0);
                stream.Write(NodeRecordCount);
                stream.Write(LeftSiblingNodeIndex);
                stream.Write(RightSiblingNodeIndex);
            }

            public static void Save(BinaryStreamBase stream, int nodeRecordCount, long leftSiblingNodeIndex, long rightSiblingNodeIndex)
            {
                stream.Write((byte)0);
                stream.Write(nodeRecordCount);
                stream.Write(leftSiblingNodeIndex);
                stream.Write(rightSiblingNodeIndex);
            }

        }


    }
}
