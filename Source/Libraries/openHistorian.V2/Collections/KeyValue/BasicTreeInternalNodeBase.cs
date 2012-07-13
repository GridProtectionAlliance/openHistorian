//******************************************************************************************************
//  BasicTreeInternalNodeBase.cs - Gbtc
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
    public abstract partial class BasicTreeInternalNodeBase : BasicTreeBase
    {
        #region [ Members ]

        const int KeySize = sizeof(ulong) + sizeof(ulong);
        const int StructureSize = KeySize + sizeof(long);
        int m_maximumRecordsPerNode;

        #endregion

        #region [ Constructors ]

        protected BasicTreeInternalNodeBase(IBinaryStream stream)
            : base(stream)
        {
            Initialize();
        }

        protected BasicTreeInternalNodeBase(IBinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
            Initialize();
        }

        #endregion

        #region [ Methods ]

        #region [ Override Methods ]

        protected override void InternalNodeCreateNode(long newNodeIndex, byte nodeLevel, long firstNodeIndex, ulong dividingKey1, ulong dividingKey2, long secondNodeIndex)
        {
            Stream.Position = newNodeIndex * BlockSize;
            NodeHeader.Save(Stream, nodeLevel, 1, 0, 0);
            Stream.Write(firstNodeIndex);
            Stream.Write(dividingKey1);
            Stream.Write(dividingKey2);
            Stream.Write(secondNodeIndex);
        }

        protected override void InternalNodeInsert(byte nodeLevel, long nodeIndex, ulong key1, ulong key2, long childNodeIndex)
        {
            int offset;
            var header = new NodeHeader(Stream, BlockSize, nodeIndex, nodeLevel);

            //Find the best location to insert
            //This is done before checking if a split is required to prevent splitting 
            //if a duplicate key is found
            if (FindOffsetOfKey(nodeIndex, header.NodeRecordCount, key1, key2, out offset))
                throw new Exception("Duplicate Key");

            //Check if the node needs to be split
            if (header.NodeRecordCount >= m_maximumRecordsPerNode)
            {
                SplitNodeThenInsert(key1, key2, childNodeIndex, nodeIndex, nodeLevel);
                return;
            }

            //set the stream's position to the best insert location.
            Stream.Position = nodeIndex * BlockSize + offset;

            int bytesAfterInsertPositionToShift = NodeHeader.Size + sizeof(long) + StructureSize * header.NodeRecordCount - offset;
            if (bytesAfterInsertPositionToShift > 0)
            {
                Stream.InsertBytes(StructureSize, bytesAfterInsertPositionToShift);
            }

            //Insert the data
            Stream.Write(key1);
            Stream.Write(key2);
            Stream.Write(childNodeIndex);

            //save the header
            header.NodeRecordCount++;
            header.Save(Stream, BlockSize, nodeIndex);
        }

        protected override long InternalNodeGetNodeIndexAddress(byte nodeLevel, long nodeIndex, ulong key1, ulong key2)
        {
            int offset;
            var header = new NodeHeader(Stream, BlockSize, nodeIndex, nodeLevel);

            if (FindOffsetOfKey(nodeIndex, header.NodeRecordCount, key1, key2, out offset))
            {
                //An exact match was found, return the value that is currently being pointed to.
                Stream.Position = nodeIndex * BlockSize + offset + KeySize;
                return Stream.ReadInt64();
            }
            else
            {
                Stream.Position = nodeIndex * BlockSize + (offset - sizeof(long));
                return Stream.ReadInt64();
                //An exact match was not found. Determine if before or after.

                //Check if offset is the first entry.
                if (offset == NodeHeader.Size + sizeof(long))
                {
                    Stream.Position = nodeIndex * BlockSize + (offset - sizeof(long));
                    return Stream.ReadInt64();
                }
                else
                {
                    Stream.Position = nodeIndex * BlockSize + offset - sizeof(long);
                    return Stream.ReadInt64();
                }
            }
        }
        protected override BucketInfo InternalNodeGetNodeIndexAddressBucket(byte nodeLevel, long nodeIndex, ulong key1, ulong key2)
        {
            var header = new NodeHeader(Stream, BlockSize, nodeIndex, nodeLevel);
            return FindOffsetOfKey(nodeIndex, header.NodeRecordCount, key1, key2);
        }



        #endregion

        #region [Helper Methods ]

        void Initialize()
        {
            m_maximumRecordsPerNode = (BlockSize - NodeHeader.Size - sizeof(long)) / StructureSize;
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="nodeIndex">the index of the node to search</param>
        /// <param name="nodeRecordCount">the number of records already in the current node</param>
        /// <param name="key1">the key to search for</param>
        /// <param name="key2">the key to search for</param>
        /// <param name="offset">the offset from the start of the node where the index was found</param>
        /// <returns>true the key was found in the node, false if was not found. 
        /// If false, the offset is where the key could be inserted</returns>
        /// <remarks>
        /// Search method is a binary search algorithm
        /// </remarks>
        bool FindOffsetOfKey(long nodeIndex, int nodeRecordCount, ulong key1, ulong key2, out int offset)
        {
            long addressOfFirstKey = nodeIndex * BlockSize + NodeHeader.Size + sizeof(long);
            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = nodeRecordCount - 1;

            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
            {
                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);
                Stream.Position = addressOfFirstKey + StructureSize * currentTestIndex;

                ulong compareKey1 = Stream.ReadUInt64();
                ulong compareKey2 = Stream.ReadUInt64();

                int compareKeysResults = CompareKeys(key1, key2, compareKey1, compareKey2);
                if (compareKeysResults == 0)
                {
                    offset = NodeHeader.Size + sizeof(long) + StructureSize * currentTestIndex;
                    return true;
                }
                if (compareKeysResults > 0)
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }
            offset = NodeHeader.Size + sizeof(long) + StructureSize * searchLowerBoundsIndex;
            return false;
        }

        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="nodeIndex">the index of the node to search</param>
        /// <param name="nodeRecordCount">the number of records already in the current node</param>
        /// <param name="key1">the key to search for</param>
        /// <param name="key2">the key to search for</param>
        /// <returns>data about the bucket</returns>
        /// <remarks>
        /// Search method is a binary search algorithm
        /// </remarks>
        BucketInfo FindOffsetOfKey(long nodeIndex, int nodeRecordCount, ulong key1, ulong key2)
        {
            BucketInfo bucket = default(BucketInfo);
            bucket.IsValid = true;

            long addressOfFirstKey = nodeIndex * BlockSize + NodeHeader.Size + sizeof(long);
            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = nodeRecordCount - 1;

            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
            {
                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);
                Stream.Position = addressOfFirstKey + StructureSize * currentTestIndex;

                ulong compareKey1 = Stream.ReadUInt64();
                ulong compareKey2 = Stream.ReadUInt64();

                int compareKeysResults = CompareKeys(key1, key2, compareKey1, compareKey2);
                if (compareKeysResults == 0)
                {
                    //offset = NodeHeader.Size + sizeof(long) + StructureSize * currentTestIndex;
                    bucket.LowerKey1 = compareKey1;
                    bucket.LowerKey2 = compareKey2;
                    bucket.IsLowerNull = false;
                    bucket.NodeIndex = Stream.ReadInt64();
                    
                    if (currentTestIndex == nodeRecordCount)
                    {
                        bucket.IsUpperNull = true;
                    }
                    else
                    {
                        bucket.IsUpperNull = false;
                        bucket.UpperKey1 = Stream.ReadUInt64();
                        bucket.UpperKey2 = Stream.ReadUInt64();
                    }
                    return bucket;
                }
                if (compareKeysResults > 0)
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }

            if (searchLowerBoundsIndex==0)
            {
                Stream.Position = addressOfFirstKey - sizeof (long);
                bucket.IsLowerNull = true;
                bucket.NodeIndex = Stream.ReadInt64();
            }
            else
            {
                Stream.Position = addressOfFirstKey + searchLowerBoundsIndex * StructureSize - sizeof(long) - KeySize;
                bucket.IsLowerNull = false;
                bucket.LowerKey1 = Stream.ReadUInt64();
                bucket.LowerKey2 = Stream.ReadUInt64();
                bucket.NodeIndex = Stream.ReadInt64();
            }

            if (searchLowerBoundsIndex == nodeRecordCount)
            {
                bucket.IsUpperNull = true;
            }
            else
            {
                bucket.IsUpperNull = false;
                bucket.UpperKey1 = Stream.ReadUInt64();
                bucket.UpperKey2 = Stream.ReadUInt64();
            }

            return bucket;
        }

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        void SplitNodeThenInsert(ulong key1, ulong key2, long value, long firstNodeIndex, byte nodeLevel)
        {
            NodeHeader firstNodeHeader = new NodeHeader(Stream, BlockSize, firstNodeIndex, nodeLevel);
            NodeHeader secondNodeHeader = default(NodeHeader);

            //Determine how many entries to shift on the split.
            int recordsInTheFirstNode = (firstNodeHeader.NodeRecordCount >> 1); // divide by 2.
            int recordsInTheSecondNode = (firstNodeHeader.NodeRecordCount - recordsInTheFirstNode - 1); //The first entry of the second node is moved to a parent.

            long secondNodeIndex = GetNextNewNodeIndex();
            long sourceStartingAddress = firstNodeIndex * BlockSize + NodeHeader.Size + sizeof(long) + StructureSize * recordsInTheFirstNode + KeySize;
            long targetStartingAddress = secondNodeIndex * BlockSize + NodeHeader.Size;

            //lookup the dividing key
            Stream.Position = sourceStartingAddress - KeySize;
            ulong dividingKey1 = Stream.ReadUInt64();
            ulong dividingKey2 = Stream.ReadUInt64();

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, recordsInTheSecondNode * StructureSize + sizeof(long));

            //update the node that was the old right sibling
            if (firstNodeHeader.RightSiblingNodeIndex != 0)
            {
                NodeHeader oldRightSibling = new NodeHeader(Stream, BlockSize, firstNodeHeader.RightSiblingNodeIndex, nodeLevel);
                oldRightSibling.LeftSiblingNodeIndex = secondNodeIndex;
                oldRightSibling.Save(Stream, BlockSize, firstNodeHeader.RightSiblingNodeIndex);
            }

            //update the second header
            secondNodeHeader.NodeLevel = nodeLevel;
            secondNodeHeader.NodeRecordCount = recordsInTheSecondNode;
            secondNodeHeader.LeftSiblingNodeIndex = firstNodeIndex;
            secondNodeHeader.RightSiblingNodeIndex = firstNodeHeader.RightSiblingNodeIndex;
            secondNodeHeader.Save(Stream, BlockSize, secondNodeIndex);

            //update the first header
            firstNodeHeader.NodeRecordCount = recordsInTheFirstNode;
            firstNodeHeader.RightSiblingNodeIndex = secondNodeIndex;
            firstNodeHeader.Save(Stream, BlockSize, firstNodeIndex);

            NodeWasSplit(nodeLevel, firstNodeIndex, dividingKey1, dividingKey2, secondNodeIndex);
            if (CompareKeys(key1, key2, dividingKey1, dividingKey2) > 0)
            {
                InternalNodeInsert(nodeLevel, secondNodeIndex, key1, key2, value);
            }
            else
            {
                InternalNodeInsert(nodeLevel, firstNodeIndex, key1, key2, value);
            }
        }

        #endregion

        #endregion
    }
}
