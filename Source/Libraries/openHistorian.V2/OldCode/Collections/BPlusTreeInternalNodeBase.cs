////******************************************************************************************************
////  BPlusTreeInternalNodeBase.cs - Gbtc
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
////  4/7/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using openHistorian.V2.IO;

//namespace openHistorian.V2.Collections
//{
//    public abstract class BPlusTreeInternalNodeBase<TKey, TValue> : BPlusTreeBase<TKey, TValue>
//    {

//        #region [ Nexted Types ]

//        [FlagsAttribute]
//        enum CacheMode : byte
//        {
//            /// <summary>
//            /// Set if the cache is empty.
//            /// Matches No Case
//            /// </summary>
//            EmptyEntry = 0,
//            /// <summary>
//            /// Set if there are both upper and lower bounds present.
//            /// Matches [LowerBound,UpperBound)
//            /// </summary>
//            Bounded = 3,
//            /// <summary>
//            /// Set if there is only a lower bound.
//            /// Matches [LowerBound, infinity)
//            /// </summary>
//            UpperIsMissing = 1,
//            /// <summary>
//            /// Set if there is only an upper bound.
//            /// Matches (-infinity, UpperBound)
//            /// </summary>
//            LowerIsMissing = 2,
//            LowerIsValidMask = 1,
//            UpperIsValidMask = 2,
//        }
//        /// <summary>
//        /// Most recent node search results are stored in this class
//        /// to speed up the lookup process of tree entries.
//        /// </summary>
//        class BucketCache
//        {
//            /// <summary>
//            /// The key that bounds the upper range of the bucket.
//            /// </summary>
//            public TKey UpperBound;
//            /// <summary>
//            /// The key that bounds the lower range of the bucket.
//            /// </summary>
//            public TKey LowerBound;
//            /// <summary>
//            /// The index value of the bucket that falls in this range.
//            /// </summary>
//            public uint Bucket;
//            /// <summary>
//            /// The cache for the cache.
//            /// </summary>
//            public CacheMode Mode;
//            /// <summary>
//            /// Sets the current bucket as an Empty Entry so it never matches.
//            /// </summary>
//            public void Clear()
//            {
//                Mode = CacheMode.EmptyEntry;
//            }
//        }

//        /// <summary>
//        /// Assists in the read/write operations of the header of a node.
//        /// </summary>
//        struct NodeHeader
//        {
//            public const int Size = 11;
//            public byte NodeLevel;
//            public short NodeRecordCount;
//            public uint LeftSiblingNodeIndex;
//            public uint RightSiblingNodeIndex;

//            public NodeHeader(IBinaryStream stream, int blockSize, uint nodeIndex)
//            {
//                stream.Position = blockSize * nodeIndex;
//                NodeLevel = stream.ReadByte();
//                NodeRecordCount = stream.ReadInt16();
//                LeftSiblingNodeIndex = stream.ReadUInt32();
//                RightSiblingNodeIndex = stream.ReadUInt32();
//            }
//            public void Save(IBinaryStream stream, int blockSize, uint nodeIndex)
//            {
//                stream.Position = blockSize * nodeIndex;
//                stream.Write(NodeLevel);
//                stream.Write(NodeRecordCount);
//                stream.Write(LeftSiblingNodeIndex);
//                stream.Write(RightSiblingNodeIndex);
//            }
//        }

//        #endregion

//        #region [ Members ]

//        BucketCache[] m_cache;
//        int m_keySize;
//        int m_maximumRecordsPerNode;
//        int m_structureSize;

//        #endregion

//        #region [ Constructors ]

//        protected BPlusTreeInternalNodeBase(IBinaryStream stream)
//            : base(stream)
//        {
//            Initialize();
//        }

//        protected BPlusTreeInternalNodeBase(IBinaryStream stream, int blockSize)
//            : base(stream, blockSize)
//        {
//            Initialize();
//        }

//        #endregion

//        #region [ Properties ]

//        #endregion

//        #region [ Methods ]

//        #region [ Abstract Methods ]

//        protected abstract int SizeOfKey();
//        protected abstract void SaveKey(TKey value, IBinaryStream stream);
//        protected abstract TKey LoadKey(IBinaryStream stream);
//        protected abstract int CompareKeys(TKey first, TKey last);
//        protected abstract int CompareKeys(TKey first, IBinaryStream stream);

//        #endregion

//        #region [ Override Methods ]

//        protected override void InternalNodeCreateNode(uint newNodeIndex, int nodeLevel, uint firstNodeIndex, TKey dividingKey, uint secondNodeIndex)
//        {
//            Stream.Position = newNodeIndex * BlockSize;

//            //Clearing the Node
//            //Level = level;
//            //ChildCount = 1;
//            //NextNode = 0;
//            //PreviousNode = 0;
//            Stream.Write((byte)nodeLevel);
//            Stream.Write((short)1);
//            Stream.Write(0L);
//            Stream.Write(firstNodeIndex);
//            SaveKey(dividingKey, Stream);
//            Stream.Write(secondNodeIndex);
//            ClearCache(nodeLevel);
//        }

//        //protected override void InternalNodeUpdate(int nodeLevel, uint nodeIndex, TKey oldFirstKey, TKey newFirstKey)
//        //{
//        //    throw new NotImplementedException();
//        //    ClearCache(nodeLevel);
//        //}

//        //protected override void InternalNodeRemove(int nodeLevel, uint nodeIndex, TKey key)
//        //{
//        //    throw new NotImplementedException();
//        //    ClearCache(nodeLevel);
//        //}

//        protected override void InternalNodeInsert(int nodeLevel, uint nodeIndex, TKey key, uint childNodeIndex)
//        {
//            ClearCache(nodeLevel);

//            short nodeRecordCount;
//            uint leftSiblingNodeIndex;
//            uint rightSiblingNodeIndex;
//            int offset;

//            LoadNodeHeader(nodeLevel, nodeIndex, true, out nodeRecordCount, out leftSiblingNodeIndex, out rightSiblingNodeIndex);

//            //Find the best location to insert
//            //This is done before checking if a split is required to prevent splitting 
//            //if a duplicate key is found
//            if (FindOffsetOfKey(nodeIndex, nodeRecordCount, key, out offset))
//                throw new Exception("Duplicate Key");

//            //Check if the node needs to be split
//            if (nodeRecordCount >= m_maximumRecordsPerNode)
//            {
//                SplitNodeThenInsert(key, childNodeIndex, nodeIndex);
//                ClearCache(nodeLevel);
//                return;
//            }

//            //set the stream's position to the best insert location.
//            Stream.Position = nodeIndex * BlockSize + offset;

//            int bytesAfterInsertPositionToShift = NodeHeader.Size + sizeof(uint) + m_structureSize * nodeRecordCount - offset;
//            if (bytesAfterInsertPositionToShift > 0)
//            {
//                Stream.InsertBytes(m_structureSize, bytesAfterInsertPositionToShift);
//            }

//            //Insert the data
//            SaveKey(key, Stream);
//            Stream.Write(childNodeIndex);

//            //save the header
//            SaveNodeHeader(nodeIndex, (short)(nodeRecordCount + 1));
//            ClearCache(nodeLevel);
//        }

//        protected override uint InternalNodeGetIndex(int nodeLevel, uint nodeIndex, TKey key)
//        {
//            short nodeRecordCount;
//            uint leftSiblingNodeIndex;
//            uint rightSiblingNodeIndex;

//            LoadNodeHeader(nodeLevel, nodeIndex, false, out nodeRecordCount, out leftSiblingNodeIndex, out rightSiblingNodeIndex);

//            BucketCache cache = m_cache[nodeLevel - 1];
//            if (IsCacheHit(key, cache))
//            {
//                return cache.Bucket;
//            }
//            else
//            {
//                return GetIndexAndCacheResult(key, cache, nodeRecordCount, nodeIndex);
//            }
//        }

//        protected override uint InternalNodeGetFirstIndex(int nodeLevel, uint nodeIndex)
//        {
//            short childCount;
//            uint previousNode;
//            uint nextNode;

//            LoadNodeHeader(nodeLevel, nodeIndex, false, out childCount, out previousNode, out nextNode);

//            if (childCount > 0)
//            {
//                Stream.Position = nodeIndex * BlockSize + NodeHeader.Size;
//                return Stream.ReadUInt32();
//            }
//            throw new Exception("Internal Nodes should never have zero children");
//        }

//        //protected override uint InternalNodeGetLastIndex(int nodeLevel, uint nodeIndex)
//        //{
//        //    throw new NotImplementedException();
//        //    //short childCount;
//        //    //uint previousNode;
//        //    //uint nextNode;

//        //    //LoadCurrentNode(nodeLevel, nodeIndex, false, out childCount, out previousNode, out nextNode);

//        //    //if (childCount > 0)
//        //    //{
//        //    //    Stream.Position = nodeIndex * BlockSize + NodeHeader.Size;
//        //    //    return Stream.ReadUInt32();
//        //    //}
//        //    //throw new Exception("Internal Nodes should never have zero children");
//        //}


//        #endregion

//        #region [Helper Methods ]

//        void Initialize()
//        {
//            m_keySize = SizeOfKey();
//            m_structureSize = m_keySize + sizeof(uint);
//            m_maximumRecordsPerNode = (BlockSize - NodeHeader.Size) / (m_structureSize);
//            m_cache = new BucketCache[5];
//            for (int x = 0; x < 5; x++)
//            {
//                m_cache[x] = new BucketCache();
//            }
//        }

//        NodeHeader LoadNodeHeader(uint nodeIndex)
//        {
//            return new NodeHeader(Stream, BlockSize, nodeIndex);
//        }

//        void LoadNodeHeader(int nodeLevel, uint nodeIndex, bool isForWriting, out short nodeRecordCount, out uint leftSiblingNodeIndex, out uint rightSiblingNodeIndex)
//        {
//            Stream.Position = nodeIndex * BlockSize;
//            Stream.UpdateLocalBuffer(isForWriting);

//            if (Stream.ReadByte() != nodeLevel)
//                throw new Exception("The current node is not an internal node.");
//            nodeRecordCount = Stream.ReadInt16();
//            leftSiblingNodeIndex = Stream.ReadUInt32();
//            rightSiblingNodeIndex = Stream.ReadUInt32();
//        }
//        void SaveNodeHeader(uint nodeIndex, short nodeRecordCount)
//        {
//            Stream.Position = nodeIndex * BlockSize + 1;
//            Stream.Write(nodeRecordCount);
//        }

//        /// <summary>
//        /// Starting from the first byte of the node, 
//        /// this will seek the current node for the best match of the key provided.
//        /// </summary>
//        /// <param name="nodeIndex">the index of the node to search</param>
//        /// <param name="nodeRecordCount">the number of records already in the current node</param>
//        /// <param name="key">the key to search for</param>
//        /// <param name="offset">the offset from the start of the node where the index was found</param>
//        /// <returns>true the key was found in the node, false if was not found.</returns>
//        bool FindOffsetOfKey(uint nodeIndex, int nodeRecordCount, TKey key, out int offset)
//        {
//            long addressOfFirstKey = nodeIndex * BlockSize + NodeHeader.Size + sizeof(uint);
//            int searchLowerBoundsIndex = 0;
//            int searchHigherBoundsIndex = nodeRecordCount - 1;

//            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
//            {
//                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);
//                Stream.Position = addressOfFirstKey + m_structureSize * currentTestIndex;
//                int compareKeysResults = CompareKeys(key, Stream); ;
//                if (compareKeysResults == 0)
//                {
//                    offset = NodeHeader.Size + sizeof(uint) + m_structureSize * currentTestIndex;
//                    return true;
//                }
//                if (compareKeysResults > 0)
//                    searchLowerBoundsIndex = currentTestIndex + 1;
//                else
//                    searchHigherBoundsIndex = currentTestIndex - 1;
//            }
//            offset = NodeHeader.Size + sizeof(uint) + m_structureSize * searchLowerBoundsIndex;
//            return false;
//        }

//        /// <summary>
//        /// Splits an existing node into two halfs
//        /// </summary>
//        void SplitNodeThenInsert(TKey key, uint value, uint nodeIndex)
//        {
//            NodeHeader firstNodeHeader = LoadNodeHeader(nodeIndex);
//            NodeHeader secondNodeHeader = default(NodeHeader);

//            //This should never be the case, but it's here none the less.
//            if (firstNodeHeader.NodeRecordCount < 3)
//                throw new Exception("cannot split a node with fewer than 2 children");

//            //Determine how many entries to shift on the split.
//            short recordsInTheFirstNode = (short)(firstNodeHeader.NodeRecordCount >> 1); // divide by 2.
//            short recordsInTheSecondNode = (short)(firstNodeHeader.NodeRecordCount - recordsInTheFirstNode - 1);

//            uint secondNodeIndex = GetNextNewNodeIndex();
//            long sourceStartingAddress = nodeIndex * BlockSize + NodeHeader.Size + sizeof(uint) + m_structureSize * recordsInTheFirstNode + m_keySize;
//            long targetStartingAddress = secondNodeIndex * BlockSize + NodeHeader.Size;

//            //lookup the first key that will be copied
//            Stream.Position = sourceStartingAddress - m_keySize;
//            TKey dividingKey = LoadKey(Stream);

//            //do the copy
//            Stream.Copy(sourceStartingAddress, targetStartingAddress, recordsInTheSecondNode * m_structureSize + sizeof(uint));

//            //update the node that was the old right sibling
//            if (firstNodeHeader.RightSiblingNodeIndex != 0)
//            {
//                NodeHeader oldRightSibling = LoadNodeHeader(firstNodeHeader.RightSiblingNodeIndex);
//                oldRightSibling.LeftSiblingNodeIndex = secondNodeIndex;
//                oldRightSibling.Save(Stream, BlockSize, firstNodeHeader.RightSiblingNodeIndex);
//            }

//            //update the second header
//            secondNodeHeader.NodeLevel = firstNodeHeader.NodeLevel;
//            secondNodeHeader.NodeRecordCount = recordsInTheSecondNode;
//            secondNodeHeader.LeftSiblingNodeIndex = nodeIndex;
//            secondNodeHeader.RightSiblingNodeIndex = firstNodeHeader.RightSiblingNodeIndex;
//            secondNodeHeader.Save(Stream, BlockSize, secondNodeIndex);

//            //update the first header
//            firstNodeHeader.NodeRecordCount = recordsInTheFirstNode;
//            firstNodeHeader.RightSiblingNodeIndex = secondNodeIndex;
//            firstNodeHeader.Save(Stream, BlockSize, nodeIndex);

//            NodeWasSplit(firstNodeHeader.NodeLevel, nodeIndex, dividingKey, secondNodeIndex);
//            if (CompareKeys(key, dividingKey) > 0)
//            {
//                InternalNodeInsert(firstNodeHeader.NodeLevel, secondNodeIndex, key, value);
//            }
//            else
//            {
//                InternalNodeInsert(firstNodeHeader.NodeLevel, nodeIndex, key, value);
//            }
//        }

//        #endregion

//        #endregion

//        /// <summary>
//        /// If an internal node is modified. The local cache for that node may no longer be valid.
//        /// </summary>
//        /// <param name="level">the level value of the modified node</param>
//        /// <remarks>There is a bug in the cacheing algorithm. This bug will be fixed in the beta release.  
//        /// for now, i've effectively disabled caching.</remarks>
//        void ClearCache(int level)
//        {
//            //foreach (BucketCache c in m_cache)
//            //    c.Clear();
//        }

//        /// <summary>
//        /// Determines if the key matches the criteria of a previous search result.
//        /// </summary>
//        /// <param name="currentKey">The key to use to compare results.</param>
//        /// <returns></returns>
//        /// <remarks>There is a bug in the cacheing algorithm. This bug will be fixed in the beta release.  
//        /// for now, i've effectively disabled caching.</remarks>
//        bool IsCacheHit(TKey currentKey, BucketCache cache)
//        {
//            return false;
//            if (cache.Mode == CacheMode.Bounded)
//            {
//                if (CompareKeys(currentKey, cache.LowerBound) >= 0)
//                    if (CompareKeys(currentKey, cache.UpperBound) < 0)
//                        return true;
//            }
//            else if (cache.Mode == CacheMode.LowerIsMissing)
//            {
//                if (CompareKeys(currentKey, cache.UpperBound) < 0)
//                    return true;
//            }
//            else if (cache.Mode == CacheMode.UpperIsMissing)
//            {
//                if (CompareKeys(currentKey, cache.LowerBound) >= 0)
//                    return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// Starting from the end of the internal node header, 
//        /// this method will return the node index value that contains the provided key
//        /// </summary>
//        /// <returns></returns>
//        /// <remarks>There is a bug in the cacheing algorithm. This bug will be fixed in the beta release.  
//        /// for now, i've effectively disabled caching.</remarks>
//        uint GetIndexAndCacheResult(TKey key, BucketCache cache, int nodeRecordCount, uint nodeIndex)
//        {
//            int offset;

//            //Look forward only if an exact match, otherwise the position is behind me.
//            if (FindOffsetOfKey(nodeIndex, nodeRecordCount, key, out offset))
//            {
//                //Check if offset is the last valid position.
//                if (offset == NodeHeader.Size + sizeof(uint) + nodeRecordCount * m_structureSize)
//                {
//                    Stream.Position = nodeIndex * BlockSize + offset;
//                    cache.LowerBound = LoadKey(Stream);
//                    cache.Bucket = Stream.ReadUInt32();
//                    cache.Mode = CacheMode.UpperIsMissing;
//                    return cache.Bucket;
//                }
//                else
//                {
//                    Stream.Position = nodeIndex * BlockSize + offset;
//                    cache.LowerBound = LoadKey(Stream);
//                    cache.Bucket = Stream.ReadUInt32();
//                    cache.UpperBound = LoadKey(Stream);
//                    cache.Mode = CacheMode.Bounded;
//                    return cache.Bucket;
//                }
//            }

//            //Check if offset is the first entry.
//            if (offset == NodeHeader.Size + sizeof(uint))
//            {
//                Stream.Position = nodeIndex * BlockSize + (offset - 4);
//                cache.Bucket = Stream.ReadUInt32();
//                cache.UpperBound = LoadKey(Stream);
//                cache.Mode = CacheMode.LowerIsMissing;
//                cache.Mode = CacheMode.EmptyEntry;
//                return cache.Bucket;
//            }
//            else if (offset == NodeHeader.Size + sizeof(uint) + nodeRecordCount * m_structureSize)
//            {
//                //if offset is last entry
//                Stream.Position = nodeIndex * BlockSize + (offset - 4 - m_keySize);
//                cache.LowerBound = LoadKey(Stream);
//                cache.Bucket = Stream.ReadUInt32();
//                cache.Mode = CacheMode.UpperIsMissing;
//                cache.Mode = CacheMode.EmptyEntry;
//                return cache.Bucket;
//            }
//            else
//            {
//                //if offset is bounded
//                Stream.Position = nodeIndex * BlockSize + (offset - 4 - m_keySize);
//                cache.LowerBound = LoadKey(Stream);
//                cache.Bucket = Stream.ReadUInt32();
//                cache.UpperBound = LoadKey(Stream);
//                cache.Mode = CacheMode.Bounded;
//                cache.Mode = CacheMode.EmptyEntry;
//                return cache.Bucket;
//            }
//        }

//    }
//}
