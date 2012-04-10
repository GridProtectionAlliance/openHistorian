//******************************************************************************************************
//  BPlusTreeBase_Cache.cs - Gbtc
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
//  2/27/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace openHistorian.V2.Unmanaged.Generic
{
    /// <summary>
    /// Provides support for an in memory binary (plus) tree.  
    /// </summary>
    /// <typeparam name="TKey">The unique key to sort on.</typeparam>
    /// <typeparam name="TValue">They value type to store.</typeparam>
    /// <remarks>Think of this class as a <see cref="SortedList{TKey,TValue}"/> 
    /// for sorting thousands, millions, billions, or more items.  B+ trees do not suffer the same
    /// performance hit that a <see cref="SortedList{TKey,TValue}"/> does. </remarks>
    public abstract partial class BPlusTreeBase<TKey, TValue>
    {
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

        BucketCache[] m_cache;
        //LeafCache m_leafCache;

        void InitializeCache()
        {
            m_cache = new BucketCache[5];
            for (int x = 0; x < 5; x++)
            {
                m_cache[x] = new BucketCache();
            }
            //m_leafCache=new LeafCache();
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
        /// <param name="key">the key to search for</param>
        /// <returns></returns>
        uint CachedInternalNodeGetNodeIndex(TKey key, BucketCache cache)
        {
            int offset;

            //Look forward only if an exact match, otherwise the position is behind me.
            if (InternalNodeSeekToKey(key, out offset))
            {
                //Check if offset is the last valid position.
                if (offset == NodeHeader.Size + sizeof(uint) + m_internalNodeChildCount * m_internalNodeStructureSize)
                {
                    InternalNodeSetStreamOffset(offset);
                    cache.LowerBound = LoadKey(m_internalNodeStream);
                    cache.Bucket = m_internalNodeStream.ReadUInt32();
                    cache.Mode = CacheMode.UpperIsMissing;
                    return cache.Bucket;
                }
                else
                {
                    InternalNodeSetStreamOffset(offset);
                    cache.LowerBound = LoadKey(m_internalNodeStream);
                    cache.Bucket = m_internalNodeStream.ReadUInt32();
                    cache.UpperBound = LoadKey(m_internalNodeStream);
                    cache.Mode = CacheMode.Bounded;
                    return cache.Bucket;
                }

                //InternalNodeSetStreamOffset(offset + m_internalNodeKeySize);
                //return m_stream.ReadUInt32();
                InternalNodeSetStreamOffset(offset + m_internalNodeKeySize);
                return m_internalNodeStream.ReadUInt32();
            }

            //Check if offset is the first entry.
            if (offset == NodeHeader.Size + sizeof(uint))
            {
                InternalNodeSetStreamOffset(offset - 4);
                cache.Bucket = m_internalNodeStream.ReadUInt32();
                cache.UpperBound = LoadKey(m_internalNodeStream);
                cache.Mode = CacheMode.LowerIsMissing;
                return cache.Bucket;
            }
            else if (offset == NodeHeader.Size + sizeof(uint) + m_internalNodeChildCount * m_internalNodeStructureSize)
            {
                //if offset is last entry
                InternalNodeSetStreamOffset(offset - 4 - m_internalNodeKeySize);
                cache.LowerBound = LoadKey(m_internalNodeStream);
                cache.Bucket = m_internalNodeStream.ReadUInt32();
                cache.Mode = CacheMode.UpperIsMissing;
                return cache.Bucket;
            }
            else
            {
                //if offset is bounded
                InternalNodeSetStreamOffset(offset - 4 - m_internalNodeKeySize);
                cache.LowerBound = LoadKey(m_internalNodeStream);
                cache.Bucket = m_internalNodeStream.ReadUInt32();
                cache.UpperBound = LoadKey(m_internalNodeStream);
                cache.Mode = CacheMode.Bounded;
                return cache.Bucket;
            }

            //InternalNodeSetStreamOffset(offset - 4);
            //return m_stream.ReadUInt32();
            InternalNodeSetStreamOffset(offset - 4);
            return m_internalNodeStream.ReadUInt32();
        }

        /// <summary>
        /// If an internal node is modified. The local cache for that node may no longer be valid.
        /// </summary>
        /// <param name="level">the level value of the modified node</param>
        void ClearCache(byte level)
        {
            foreach (BucketCache c in m_cache)
                c.Clear();
            //if (level == 0)
            //    return;
            //m_cache[level - 1].Clear();
        }

        uint CachedGetInternalNodeIndex(byte nodeLevel, uint nodeIndex, TKey key)
        {
            bool cacheInvalid = false;
            for (byte levelCount = nodeLevel; levelCount > 0; levelCount--)
            {
                BucketCache cache = m_cache[levelCount - 1];
                if (IsMatch(key, cache) && !cacheInvalid)
                {
                    nodeIndex = cache.Bucket;
                }
                else
                {
                    cacheInvalid = true;
                    InternalNodeSetCurrentNode(levelCount, nodeIndex, true);
                    nodeIndex = CachedInternalNodeGetNodeIndex(key, cache);
                }
            }
            return nodeIndex;
        }
    }
}
