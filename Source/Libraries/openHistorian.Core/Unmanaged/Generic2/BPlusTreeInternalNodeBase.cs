//******************************************************************************************************
//  BPlusTreeInternalNodeBase.cs - Gbtc
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

namespace openHistorian.Core.Unmanaged.Generic2
{
    public abstract class BPlusTreeInternalNodeBase<TKey, TValue> : BPlusTreeBase<TKey, TValue>
    {

        #region [ Nexted Types ]

        [FlagsAttribute]
        enum CacheMode : byte
        {
            /// <summary>
            /// Set if the cache is empty.
            /// Matches No Case
            /// </summary>
            EmptyEntry = 0,
            /// <summary>
            /// Set if there are both upper and lower bounds present.
            /// Matches [LowerBound,UpperBound)
            /// </summary>
            Bounded = 3,
            /// <summary>
            /// Set if there is only a lower bound.
            /// Matches [LowerBound, infinity)
            /// </summary>
            UpperIsMissing = 1,
            /// <summary>
            /// Set if there is only an upper bound.
            /// Matches (-infinity, UpperBound)
            /// </summary>
            LowerIsMissing = 2,
            LowerIsValidMask = 1,
            UpperIsValidMask = 2,
        }
        /// <summary>
        /// Most recent node search results are stored in this class
        /// to speed up the lookup process of tree entries.
        /// </summary>
        class BucketCache
        {
            /// <summary>
            /// The key that bounds the upper range of the bucket.
            /// </summary>
            public TKey UpperBound;
            /// <summary>
            /// The key that bounds the lower range of the bucket.
            /// </summary>
            public TKey LowerBound;
            /// <summary>
            /// The index value of the bucket that falls in this range.
            /// </summary>
            public uint Bucket;
            /// <summary>
            /// The cache for the cache.
            /// </summary>
            public CacheMode Mode;
            /// <summary>
            /// Sets the current bucket as an Empty Entry so it never matches.
            /// </summary>
            public void Clear()
            {
                Mode = CacheMode.EmptyEntry;
            }
        }

        /// <summary>
        /// Assists in the read/write operations of the header of a node.
        /// </summary>
        struct NodeHeader
        {
            public const int Size = 11;
            public byte Level;
            public short ChildCount;
            public uint PreviousNode;
            public uint NextNode;

            public void Load(BinaryStream stream, int blockSize, uint nodeIndex)
            {
                stream.Position = blockSize * nodeIndex;
                Load(stream);
            }

            public void Load(BinaryStream stream)
            {
                Level = stream.ReadByte();
                ChildCount = stream.ReadInt16();
                PreviousNode = stream.ReadUInt32();
                NextNode = stream.ReadUInt32();
            }
            /// <summary>
            /// Saves the node data to the underlying stream. 
            /// </summary>
            /// <param name="header"></param>
            /// <param name="nodeIndex"></param>
            public void Save(BinaryStream stream, int blockSize, uint nodeIndex)
            {
                stream.Position = blockSize * nodeIndex;
                Save(stream);
            }
            /// <summary>
            /// Saves the node data to the underlying stream.
            /// </summary>
            /// <param name="stream"></param>
            /// <remarks>
            /// From the current position on the stream, the node header data is written to the stream.
            /// The position after calling this function is at the end of the header
            /// </remarks>
            public void Save(BinaryStream stream)
            {
                stream.Write(Level);
                stream.Write(ChildCount);
                stream.Write(PreviousNode);
                stream.Write(NextNode);
            }
        }

        #endregion

        #region [ Members ]

        BucketCache[] m_cache;
        int m_keySize;
        int m_maximumChildren;
        int m_structureSize;
        uint m_currentNode;
        short m_childCount;
        uint m_nextNode;
        uint m_previousNode;

        #endregion

        #region [ Constructors ]

        protected BPlusTreeInternalNodeBase(BinaryStream stream)
            : base(stream)
        {
            Initialize();
        }

        protected BPlusTreeInternalNodeBase(BinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
            Initialize();
        }

        #endregion

        #region [ Abstract Methods ]

        protected abstract int SizeOfKey();
        protected abstract void SaveKey(TKey value, BinaryStream stream);
        protected abstract TKey LoadKey(BinaryStream stream);
        protected abstract int CompareKeys(TKey first, TKey last);
        protected abstract int CompareKeys(TKey first, BinaryStream stream);

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        #endregion

        protected override void InternalNodeUpdate(int indexLevel, uint nodeIndex, TKey oldFirstKey, TKey newFirstKey)
        {
            throw new NotImplementedException();
        }
       
        protected override uint InternalNodeCreateEmptyNode(int indexLevel, uint currentIndex, TKey middleKey, uint newIndex)
        {
            uint nodeAddress = AllocateNewNode();
            Stream.Position = nodeAddress * BlockSize;

            //Clearing the Node
            //Level = level;
            //ChildCount = 1;
            //NextNode = 0;
            //PreviousNode = 0;
            Stream.Write((byte)indexLevel);
            Stream.Write((short)1);
            Stream.Write(0L);
            Stream.Write(currentIndex);
            SaveKey(middleKey, Stream);
            Stream.Write(newIndex);
            return nodeAddress;
        }

        protected override uint InternalNodeGetFirstIndex(int indexLevel, uint nodeIndex)
        {
            throw new NotImplementedException();
        }
        
        protected override uint InternalNodeGetIndex(int indexLevel, uint nodeIndex, TKey key)
        {
            BucketCache cache = m_cache[indexLevel - 1];
            if (IsMatch(key, cache))
            {
                return cache.Bucket;
            }
            else
            {
                InternalNodeSetCurrentNode((byte)indexLevel, nodeIndex, true);
                return GetIndexAndCacheResult(key, cache);
            }
        }

        protected override void InternalNodeInsert(int indexLevel, uint nodeIndex, TKey key, uint childNodeIndex)
        {
            int offset;

            long nodePositionStart = m_currentNode * BlockSize;

            if (m_childCount >= m_maximumChildren)
            {
                InternalNodeSplitNode(key, nodeIndex);
                return;
            }

            if (InternalNodeSeekToKey(key, out offset))
                throw new Exception("Duplicate Key");

            int spaceToMove = NodeHeader.Size + sizeof(uint) + m_structureSize * m_childCount - offset;

            if (spaceToMove > 0)
            {
                Stream.Position = m_currentNode * BlockSize + offset;
                Stream.InsertBytes(m_structureSize, spaceToMove);
            }

            Stream.Position = m_currentNode * BlockSize + offset;
            SaveKey(key, Stream);
            Stream.Write(nodeIndex);

            m_childCount++;
            Stream.Position = m_currentNode * BlockSize + 1;
            Stream.Write(m_childCount);
        }

        protected override void InternalNodeRemove(int indexLevel, uint nodeIndex, TKey key)
        {
            throw new NotImplementedException();
        }

        #region [ Methods ]

        #region [Helper Methods ]

        void Initialize()
        {
            m_keySize = SizeOfKey();
            m_structureSize = m_keySize + sizeof(uint);
            m_maximumChildren = (BlockSize - NodeHeader.Size) / (m_structureSize);
            m_cache = new BucketCache[5];
            for (int x = 0; x < 5; x++)
            {
                m_cache[x] = new BucketCache();
            }
        }

        void InternalNodeSetCurrentNode(byte nodeLevel, uint nodeIndex, bool isForWriting)
        {
            m_currentNode = nodeIndex;
            Stream.Position = nodeIndex * BlockSize;
            Stream.UpdateLocalBuffer(isForWriting);

            if (Stream.ReadByte() != nodeLevel)
                throw new Exception("The current node is not a leaf.");
            m_childCount = Stream.ReadInt16();
            m_previousNode = Stream.ReadUInt32();
            m_nextNode = Stream.ReadUInt32();
        }
        void InternalNodeSetCurrentNode(uint nodeIndex, bool isForWriting)
        {
            m_currentNode = nodeIndex;
            Stream.Position = nodeIndex * BlockSize;
            Stream.UpdateLocalBuffer(isForWriting);

            Stream.ReadByte(); //node level
            m_childCount = Stream.ReadInt16();
            m_previousNode = Stream.ReadUInt32();
            m_nextNode = Stream.ReadUInt32();
        }


        /// <summary>
        /// Starting from the first byte of the node, 
        /// this will seek the current node for the best match of the key provided.
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <param name="offset"></param>
        /// <returns>the stream positioned at the spot corresponding to the returned search results.</returns>
        bool InternalNodeSeekToKey(TKey key, out int offset)
        {
            long startAddress = m_currentNode * BlockSize + NodeHeader.Size + sizeof(uint);

            int min = 0;
            int max = m_childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                Stream.Position = startAddress + m_structureSize * mid;

                int tmpKey = CompareKeys(key, Stream); ;
                if (tmpKey == 0)
                {
                    offset = NodeHeader.Size + sizeof(uint) + m_structureSize * mid;
                    return true;
                }
                if (tmpKey > 0)
                    min = mid + 1;
                else
                    max = mid - 1;
            }

            offset = NodeHeader.Size + sizeof(uint) + m_structureSize * min;
            return false;
        }

  

        /// <summary>
        /// Splits an existing node into two halfs
        /// </summary>
        void InternalNodeSplitNode(TKey key, uint childNodeIndex)
        {
            uint currentNode = m_currentNode;
            uint oldNextNode = m_nextNode;
            TKey firstKeyInGreaterNode = default(TKey);

            NodeHeader origionalNode = default(NodeHeader);
            NodeHeader newNode = default(NodeHeader);
            NodeHeader foreignNode = default(NodeHeader);

            origionalNode.Load(Stream, BlockSize, m_currentNode);

            if (origionalNode.ChildCount < 2)
                throw new Exception("cannot split a node with fewer than 2 children");

            short itemsInFirstNode = (short)(origionalNode.ChildCount >> 1); // divide by 2.
            short itemsInSecondNode = (short)(origionalNode.ChildCount - itemsInFirstNode);

            uint greaterNodeIndex = AllocateNewNode();
            long sourceStartingAddress = m_currentNode * BlockSize + NodeHeader.Size + sizeof(uint) + m_structureSize * itemsInFirstNode;
            long targetStartingAddress = greaterNodeIndex * BlockSize + NodeHeader.Size + sizeof(uint);

            //lookup the first key that will be copied
            Stream.Position = sourceStartingAddress;
            firstKeyInGreaterNode = LoadKey(Stream);

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, itemsInSecondNode * m_structureSize);
            //Set the lookback position as invalid since this node should never be parsed for data before the first key.
            Stream.Position = targetStartingAddress - sizeof(uint);
            Stream.Write(0u);

            //update the first header
            origionalNode.ChildCount = itemsInFirstNode;
            origionalNode.NextNode = greaterNodeIndex;
            origionalNode.Save(Stream, BlockSize, currentNode);

            //update the second header
            newNode.Level = origionalNode.Level;
            newNode.ChildCount = itemsInSecondNode;
            newNode.PreviousNode = currentNode;
            newNode.NextNode = oldNextNode;
            newNode.Save(Stream, BlockSize, greaterNodeIndex);

            //update the node that used to be after the first one.
            if (oldNextNode != 0)
            {
                foreignNode.Load(Stream, BlockSize, oldNextNode);
                foreignNode.PreviousNode = greaterNodeIndex;
                foreignNode.Save(Stream, BlockSize, oldNextNode);
            }
            NodeWasSplit(origionalNode.Level, currentNode, firstKeyInGreaterNode, greaterNodeIndex);
            if (CompareKeys(key, firstKeyInGreaterNode) > 0)
            {
                InternalNodeInsert(origionalNode.Level,greaterNodeIndex,key,childNodeIndex);
            }
            else
            {
                InternalNodeInsert(origionalNode.Level, currentNode, key, childNodeIndex);
            }

        }

        #endregion

        #endregion



        /// <summary>
        /// If an internal node is modified. The local cache for that node may no longer be valid.
        /// </summary>
        /// <param name="level">the level value of the modified node</param>
        void ClearCache(byte level)
        {
            foreach (BucketCache c in m_cache)
                c.Clear();
        }

        /// <summary>
        /// Determines if the key matches the criteria of a previous search result.
        /// </summary>
        /// <param name="currentKey">The key to use to compare results.</param>
        /// <returns></returns>
        bool IsMatch(TKey currentKey, BucketCache cache)
        {
            if (cache.Mode == CacheMode.Bounded)
            {
                if (CompareKeys(currentKey, cache.LowerBound) >= 0)
                    if (CompareKeys(currentKey, cache.UpperBound) < 0)
                        return true;
            }
            else if (cache.Mode == CacheMode.LowerIsMissing)
            {
                if (CompareKeys(currentKey, cache.UpperBound) < 0)
                    return true;
            }
            else if (cache.Mode == CacheMode.UpperIsMissing)
            {
                if (CompareKeys(currentKey, cache.LowerBound) >= 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Starting from the end of the internal node header, 
        /// this method will return the node index value that contains the provided key
        /// </summary>
        /// <returns></returns>
        uint GetIndexAndCacheResult(TKey key, BucketCache cache)
        {
            int offset;

            //Look forward only if an exact match, otherwise the position is behind me.
            if (InternalNodeSeekToKey(key, out offset))
            {
                //Check if offset is the last valid position.
                if (offset == NodeHeader.Size + sizeof(uint) + m_childCount * m_structureSize)
                {
                    Stream.Position = m_currentNode * BlockSize + offset;
                    cache.LowerBound = LoadKey(Stream);
                    cache.Bucket = Stream.ReadUInt32();
                    cache.Mode = CacheMode.UpperIsMissing;
                    return cache.Bucket;
                }
                else
                {
                    Stream.Position = m_currentNode * BlockSize + offset;
                    cache.LowerBound = LoadKey(Stream);
                    cache.Bucket = Stream.ReadUInt32();
                    cache.UpperBound = LoadKey(Stream);
                    cache.Mode = CacheMode.Bounded;
                    return cache.Bucket;
                }
            }

            //Check if offset is the first entry.
            if (offset == NodeHeader.Size + sizeof(uint))
            {
                Stream.Position = m_currentNode * BlockSize + (offset - 4);
                cache.Bucket = Stream.ReadUInt32();
                cache.UpperBound = LoadKey(Stream);
                cache.Mode = CacheMode.LowerIsMissing;
                return cache.Bucket;
            }
            else if (offset == NodeHeader.Size + sizeof(uint) + m_childCount * m_structureSize)
            {
                //if offset is last entry
                Stream.Position = m_currentNode * BlockSize + (offset - 4 - m_keySize);
                cache.LowerBound = LoadKey(Stream);
                cache.Bucket = Stream.ReadUInt32();
                cache.Mode = CacheMode.UpperIsMissing;
                return cache.Bucket;
            }
            else
            {
                //if offset is bounded
                Stream.Position = m_currentNode * BlockSize + (offset - 4 - m_keySize);
                cache.LowerBound = LoadKey(Stream);
                cache.Bucket = Stream.ReadUInt32();
                cache.UpperBound = LoadKey(Stream);
                cache.Mode = CacheMode.Bounded;
                return cache.Bucket;
            }
        }

    }
}
