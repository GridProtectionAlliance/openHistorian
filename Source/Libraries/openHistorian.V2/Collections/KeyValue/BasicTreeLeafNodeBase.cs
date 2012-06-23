//******************************************************************************************************
//  BasicTreeLeafNodeBase.cs - Gbtc
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
using openHistorian.V2.IO;

namespace openHistorian.V2.Collections.KeyValue
{
    public abstract partial class BasicTreeLeafNodeBase : BasicTreeInternalNodeBase
    {
        #region [ Nexted Types ]

        /// <summary>
        /// Assists in the read/write operations of the header of a node.
        /// </summary>
        struct NodeHeader
        {
            public const int Size = 11;
            public byte NodeLevel;
            public short NodeRecordCount;
            public uint LeftSiblingNodeIndex;
            public uint RightSiblingNodeIndex;

            public NodeHeader(IBinaryStream stream, int blockSize, uint nodeIndex)
            {
                stream.Position = blockSize * nodeIndex;
                NodeLevel = stream.ReadByte();
                NodeRecordCount = stream.ReadInt16();
                LeftSiblingNodeIndex = stream.ReadUInt32();
                RightSiblingNodeIndex = stream.ReadUInt32();
            }
            public void Save(IBinaryStream stream, int blockSize, uint nodeIndex)
            {
                stream.Position = blockSize * nodeIndex;
                stream.Write(NodeLevel);
                stream.Write(NodeRecordCount);
                stream.Write(LeftSiblingNodeIndex);
                stream.Write(RightSiblingNodeIndex);
            }
        }
    
        #endregion

        #region [ Members ]

        const int KeySize = 16;
        int m_maximumRecordsPerNode;
        const int StructureSize = KeySize + 16;

        #endregion

        #region [ Constructors ]

        protected BasicTreeLeafNodeBase(IBinaryStream stream)
            : base(stream)
        {
            Initialize();
        }


        protected BasicTreeLeafNodeBase(IBinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
            Initialize();
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        #region [ Override Methods ]

        protected override void LeafNodeCreateEmptyNode(uint newNodeIndex)
        {
            Stream.Position = BlockSize * newNodeIndex;

            //Clearing the Node
            //Level = 0;
            //ChildCount = 0;
            //NextNode = 0;
            //PreviousNode = 0;
            Stream.Write(0L);
            Stream.Write(0);
        }

        protected override bool LeafNodeInsert(uint nodeIndex, long key1, long key2, long value1, long value2)
        {
            short nodeRecordCount;
            uint leftSiblingNodeIndex;
            uint rightSiblingNodeIndex;
            int offset;

            LoadNodeHeader(nodeIndex, true, out nodeRecordCount, out leftSiblingNodeIndex, out rightSiblingNodeIndex);

            //Find the best location to insert
            //This is done before checking if a split is required to prevent splitting 
            //if a duplicate key is found
            if (FindOffsetOfKey(nodeIndex, nodeRecordCount, key1, key2, out offset)) //If found
                return false;

            //Check if the node needs to be split
            if (nodeRecordCount >= m_maximumRecordsPerNode)
            {
                SplitNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                return true;
            }

            //set the stream's position to the best insert location.
            Stream.Position = nodeIndex * BlockSize + offset;

            //Determine the number of bytes that need to be shifted in order to insert the key
            int bytesAfterInsertPositionToShift = NodeHeader.Size + StructureSize * nodeRecordCount - offset;
            if (bytesAfterInsertPositionToShift > 0)
            {
                Stream.InsertBytes(StructureSize, bytesAfterInsertPositionToShift);
            }

            //Insert the data
            Stream.Write(key1);
            Stream.Write(key2);
            Stream.Write(value1);
            Stream.Write(value2);

            //save the header
            SaveNodeHeader(nodeIndex, (short)(nodeRecordCount + 1));
            return true;
        }

        /// <summary>
        /// Outputs the value associated with the provided key in the given node.
        /// </summary>
        /// <param name="nodeIndex">the node to search</param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns>true if the key was found, false if the key was not found.</returns>
        protected override bool LeafNodeGetValue(uint nodeIndex, long key1, long key2, out long value1, out long value2)
        {
            short nodeRecordCount;
            uint leftSiblingNodeIndex;
            uint rightSiblingNodeIndex;
            int offset;

            LoadNodeHeader(nodeIndex, false, out nodeRecordCount, out leftSiblingNodeIndex, out rightSiblingNodeIndex);

            if (FindOffsetOfKey(nodeIndex, nodeRecordCount, key1, key2, out offset))
            {
                Stream.Position = nodeIndex * BlockSize + (offset + KeySize);
                value1 = Stream.ReadInt64();
                value2 = Stream.ReadInt64();
                return true;
            }
            value1 = 0;
            value2 = 0;
            return false;
        }

        protected override IDataScanner LeafNodeGetScanner()
        {
            return new DataScanner(this);
        } 
   
        #endregion

        #region [ Helper Methods ]

        void Initialize()
        {
            m_maximumRecordsPerNode = (BlockSize - NodeHeader.Size) / (StructureSize);
        }

        NodeHeader LoadNodeHeader(uint nodeIndex)
        {
            return new NodeHeader(Stream, BlockSize, nodeIndex);
        }

        void LoadNodeHeader(uint nodeIndex, bool isForWriting, out short nodeRecordCount, out uint leftSiblingNodeIndex, out uint rightSiblingNodeIndex)
        {
            Stream.Position = nodeIndex * BlockSize;
            Stream.UpdateLocalBuffer(isForWriting);

            if (Stream.ReadByte() != 0)
                throw new Exception("The current node is not a leaf.");
            nodeRecordCount = Stream.ReadInt16();
            leftSiblingNodeIndex = Stream.ReadUInt32();
            rightSiblingNodeIndex = Stream.ReadUInt32();
        }

        void SaveNodeHeader(uint nodeIndex, short nodeRecordCount)
        {
            Stream.Position = nodeIndex * BlockSize + 1;
            Stream.Write(nodeRecordCount);
        }


        /// <summary>
        /// Seeks to the location of the key or the position where the key could be inserted to preserve order.
        /// </summary>
        /// <param name="nodeIndex">the index of the node to search</param>
        /// <param name="nodeRecordCount">the number of records already in the current node</param>
        /// <param name="key1">the key to look for</param>
        /// <param name="key2">the key to look for</param>
        /// <param name="offset">the offset from the start of the node where the index was found</param>
        /// <returns>true the key was found in the node, false if was not found.</returns>
        bool FindOffsetOfKey(uint nodeIndex, int nodeRecordCount, long key1, long key2, out int offset)
        {
            long addressOfFirstKey = nodeIndex * BlockSize + NodeHeader.Size;
            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = nodeRecordCount - 1;

            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
            {
                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);
                Stream.Position = addressOfFirstKey + StructureSize * currentTestIndex;
                long compareKey1 = Stream.ReadInt64();
                long compareKey2 = Stream.ReadInt64();
                int compareKeysResults = CompareKeys(key1, key2, compareKey1, compareKey2);
                if (compareKeysResults == 0) //if keys match, result is found.
                {
                    offset = NodeHeader.Size + StructureSize * currentTestIndex;
                    return true;
                }
                if (compareKeysResults > 0) //if the key is greater than the test index, change the lower bounds
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else //if the key is less than the current test index, change the upper bounds.
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }
            offset = NodeHeader.Size + StructureSize * searchLowerBoundsIndex;
            return false;
        }

        void SplitNodeThenInsert(long key1, long key2, long value1, long value2, uint nodeIndex)
        {
            NodeHeader firstNodeHeader = LoadNodeHeader(nodeIndex);
            NodeHeader secondNodeHeader = default(NodeHeader);

            //This should never be the case, but it's here none the less.
            if (firstNodeHeader.NodeRecordCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");

            //Determine how many entries to shift on the split.
            short recordsInTheFirstNode = (short)(firstNodeHeader.NodeRecordCount >> 1); // divide by 2.
            short recordsInTheSecondNode = (short)(firstNodeHeader.NodeRecordCount - recordsInTheFirstNode);

            uint secondNodeIndex = GetNextNewNodeIndex();
            long sourceStartingAddress = nodeIndex * BlockSize + NodeHeader.Size + StructureSize * recordsInTheFirstNode;
            long targetStartingAddress = secondNodeIndex * BlockSize + NodeHeader.Size;

            //lookup the first key that will be copied
            Stream.Position = sourceStartingAddress;
            long dividingKey1 = Stream.ReadInt64();
            long dividingKey2 = Stream.ReadInt64();

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, recordsInTheSecondNode * StructureSize);

            //update the node that was the old right sibling
            if (firstNodeHeader.RightSiblingNodeIndex != 0)
            {
                NodeHeader oldRightSibling = LoadNodeHeader(firstNodeHeader.RightSiblingNodeIndex);
                oldRightSibling.LeftSiblingNodeIndex = secondNodeIndex;
                oldRightSibling.Save(Stream, BlockSize, firstNodeHeader.RightSiblingNodeIndex);
            }

            //update the second header
            secondNodeHeader.NodeLevel = 0;
            secondNodeHeader.NodeRecordCount = recordsInTheSecondNode;
            secondNodeHeader.LeftSiblingNodeIndex = nodeIndex;
            secondNodeHeader.RightSiblingNodeIndex = firstNodeHeader.RightSiblingNodeIndex;
            secondNodeHeader.Save(Stream, BlockSize, secondNodeIndex);

            //update the first header
            firstNodeHeader.NodeRecordCount = recordsInTheFirstNode;
            firstNodeHeader.RightSiblingNodeIndex = secondNodeIndex;
            firstNodeHeader.Save(Stream, BlockSize, nodeIndex);

            NodeWasSplit(0, nodeIndex, dividingKey1, dividingKey2, secondNodeIndex);
            if (CompareKeys(key1, key2, dividingKey1, dividingKey2) > 0)
            {
                LeafNodeInsert(secondNodeIndex, key1, key2, value1, value2);
            }
            else
            {
                LeafNodeInsert(nodeIndex, key1, key2, value1, value2);
            }
        }


        #endregion

        #endregion


    }
}
