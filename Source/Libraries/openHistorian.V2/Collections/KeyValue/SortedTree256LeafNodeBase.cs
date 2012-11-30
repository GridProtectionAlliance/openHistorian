//******************************************************************************************************
//  SortedTree256LeafNodeBase.cs - Gbtc
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

using openHistorian.V2.IO;

namespace openHistorian.V2.Collections.KeyValue
{
    public abstract partial class SortedTree256LeafNodeBase : SortedTree256InternalNodeBase
    {

        #region [ Members ]

        const int KeySize = sizeof(ulong) + sizeof(ulong);
        const int StructureSize = KeySize + sizeof(ulong) + sizeof(ulong);
        int m_maximumRecordsPerNode;

        #endregion

        #region [ Constructors ]

        protected SortedTree256LeafNodeBase(BinaryStreamBase stream)
            : base(stream)
        {
            Initialize();
        }


        protected SortedTree256LeafNodeBase(BinaryStreamBase stream, int blockSize)
            : base(stream, blockSize)
        {
            Initialize();
        }

        #endregion

        #region [ Methods ]

        #region [ Override Methods ]

        protected override void LeafNodeCreateEmptyNode(long newNodeIndex)
        {
            Stream.Position = BlockSize * newNodeIndex;
            NodeHeader.Save(Stream, 0, 0, 0);
        }

        //protected override bool LeafNodeInsert(IDataScanner dataScanner, long nodeIndex, ref ulong key1, ref ulong key2, ref ulong value1, ref ulong value2, ref bool isValid)
        //{
        //    //ulong firstKey1
        //    //while (isValid)
        //    //{
                
        //    //}
           

        //    //return true;
        //}

        protected override bool LeafNodeInsert(long nodeIndex, ulong key1, ulong key2, ulong value1, ulong value2)
        {
            int offset;
            var header = new NodeHeader(Stream, BlockSize, nodeIndex);

            //Find the best location to insert
            //This is done before checking if a split is required to prevent splitting 
            //if a duplicate key is found
            if (FindOffsetOfKey(nodeIndex, header.NodeRecordCount, key1, key2, out offset)) //If found
                return false;

            //Check if the node needs to be split
            if (header.NodeRecordCount >= m_maximumRecordsPerNode)
            {
                SplitNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                return true;
            }

            //set the stream's position to the best insert location.
            Stream.Position = nodeIndex * BlockSize + offset;

            //Determine the number of bytes that need to be shifted in order to insert the key
            int bytesAfterInsertPositionToShift = NodeHeader.Size + StructureSize * header.NodeRecordCount - offset;
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
            header.NodeRecordCount++;
            header.Save(Stream, BlockSize, nodeIndex);
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
        protected override bool LeafNodeGetValue(long nodeIndex, ulong key1, ulong key2, out ulong value1, out ulong value2)
        {
            int offset;
            var header = new NodeHeader(Stream, BlockSize, nodeIndex);

            if (FindOffsetOfKey(nodeIndex, header.NodeRecordCount, key1, key2, out offset))
            {
                Stream.Position = nodeIndex * BlockSize + (offset + KeySize);
                value1 = Stream.ReadUInt64();
                value2 = Stream.ReadUInt64();
                return true;
            }
            value1 = 0;
            value2 = 0;
            return false;
        }

        protected override ITreeScanner256 LeafNodeGetScanner()
        {
            return new TreeScanner(this);
        }

        #endregion

        #region [ Helper Methods ]

        void Initialize()
        {
            m_maximumRecordsPerNode = (BlockSize - NodeHeader.Size) / StructureSize;
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
        bool FindOffsetOfKey(long nodeIndex, int nodeRecordCount, ulong key1, ulong key2, out int offset)
        {
            long addressOfFirstKey = nodeIndex * BlockSize + NodeHeader.Size;
            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = nodeRecordCount - 1;

            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
            {
                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);
                Stream.Position = addressOfFirstKey + StructureSize * currentTestIndex;

                ulong compareKey1 = Stream.ReadUInt64();
                ulong compareKey2 = Stream.ReadUInt64();

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

        void SplitNodeThenInsert(ulong key1, ulong key2, ulong value1, ulong value2, long firstNodeIndex)
        {
            NodeHeader firstNodeHeader = new NodeHeader(Stream, BlockSize, firstNodeIndex);
            NodeHeader secondNodeHeader = default(NodeHeader);

            //Determine how many entries to shift on the split.
            short recordsInTheFirstNode = (short)(firstNodeHeader.NodeRecordCount >> 1); // divide by 2.
            short recordsInTheSecondNode = (short)(firstNodeHeader.NodeRecordCount - recordsInTheFirstNode);

            long secondNodeIndex = GetNextNewNodeIndex();
            long sourceStartingAddress = firstNodeIndex * BlockSize + NodeHeader.Size + StructureSize * recordsInTheFirstNode;
            long targetStartingAddress = secondNodeIndex * BlockSize + NodeHeader.Size;

            //lookup the first key that will be copied
            Stream.Position = sourceStartingAddress;
            ulong dividingKey1 = Stream.ReadUInt64();
            ulong dividingKey2 = Stream.ReadUInt64();

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, recordsInTheSecondNode * StructureSize);

            //update the node that was the old right sibling
            if (firstNodeHeader.RightSiblingNodeIndex != 0)
            {
                NodeHeader oldRightSibling = new NodeHeader(Stream, BlockSize, firstNodeHeader.RightSiblingNodeIndex);
                oldRightSibling.LeftSiblingNodeIndex = secondNodeIndex;
                oldRightSibling.Save(Stream, BlockSize, firstNodeHeader.RightSiblingNodeIndex);
            }

            //update the second header
            secondNodeHeader.NodeRecordCount = recordsInTheSecondNode;
            secondNodeHeader.LeftSiblingNodeIndex = firstNodeIndex;
            secondNodeHeader.RightSiblingNodeIndex = firstNodeHeader.RightSiblingNodeIndex;
            secondNodeHeader.Save(Stream, BlockSize, secondNodeIndex);

            //update the first header
            firstNodeHeader.NodeRecordCount = recordsInTheFirstNode;
            firstNodeHeader.RightSiblingNodeIndex = secondNodeIndex;
            firstNodeHeader.Save(Stream, BlockSize, firstNodeIndex);

            NodeWasSplit(0, firstNodeIndex, dividingKey1, dividingKey2, secondNodeIndex);
            if (CompareKeys(key1, key2, dividingKey1, dividingKey2) > 0)
            {
                LeafNodeInsert(secondNodeIndex, key1, key2, value1, value2);
            }
            else
            {
                LeafNodeInsert(firstNodeIndex, key1, key2, value1, value2);
            }
        }


        #endregion

        #endregion


    }
}
