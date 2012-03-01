//******************************************************************************************************
//  TreeLookupCache.cs - Gbtc
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

namespace openHistorian.Core.StorageSystem.Generic
{
    internal class TreeLookupCache<TKey>
        where TKey : struct, ITreeType<TKey>
    {
        BucketCache[] m_cache;
        int m_internalStructureSize;
        BinaryStream m_stream;
        LeafCache m_leafCache;

        public TreeLookupCache(BinaryStream stream, int internalStructureSize)
        {
            m_stream = stream;
            m_internalStructureSize = internalStructureSize;
            m_cache = new BucketCache[10];
            for (int x = 0; x < 10; x++)
            {
                m_cache[x] = new BucketCache();
            }
            m_leafCache=new LeafCache();
        }

        /// <summary>
        /// Most recent node search results are stored in this class
        /// to speed up the lookup process of tree entries.
        /// </summary>
        class BucketCache
        {
            [FlagsAttribute]
            public enum CacheMode : byte
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
            /// The 
            /// </summary>
            public CacheMode Mode;
            /// <summary>
            /// Determines if the key matches the criteria of a previous search result.
            /// </summary>
            /// <param name="currentKey">The key to use to compare results.</param>
            /// <returns></returns>
            public bool IsMatch(TKey currentKey)
            {
                if (Mode == CacheMode.Bounded)
                {
                    if (currentKey.CompareTo(LowerBound) >= 0)
                        if (currentKey.CompareTo(UpperBound) < 0)
                            return true;
                }
                else if (Mode == CacheMode.LowerIsMissing)
                {
                    if (currentKey.CompareTo(UpperBound) < 0)
                        return true;
                }
                else if (Mode == CacheMode.UpperIsMissing)
                {
                    if (currentKey.CompareTo(LowerBound) >= 0)
                        return true;
                }
                return false;
            }

            /// <summary>
            /// Sets the current bucket as an Empty Entry so it never matches.
            /// </summary>
            public void Clear()
            {
                Mode=CacheMode.EmptyEntry;
            }
        }

        /// <summary>
        /// Most recent node search results are stored in this class
        /// to speed up the lookup process of tree entries.
        /// </summary>
        class LeafCache
        {
            /// <summary>
            /// The key that matches the result.
            /// </summary>
            public TKey KeyMatch;
            /// <summary>
            /// The index of the key.
            /// </summary>
            public int PositionIndex;
            /// <summary>
            /// Determines if this entry is valid.
            /// </summary>
            public bool IsValid;

            /// <summary>
            /// Determines if the key matches the criteria of a previous search result.
            /// </summary>
            /// <param name="currentKey">The key to use to compare results.</param>
            /// <param name="lowerRange">The starting position for the binary search method.</param>
            /// <returns>The ending postion for the binary search method.</returns>
            public void UseBounds(TKey currentKey, ref int lowerRange, ref int upperRange)
            {
                if (!IsValid)
                    return;

                int compareResults = currentKey.CompareTo(KeyMatch);
                if (compareResults > 0)
                {
                    lowerRange = PositionIndex;
                    return;
                }
                if (compareResults < 0)
                {
                    upperRange = PositionIndex;
                    return;
                }

                upperRange = PositionIndex;
                lowerRange = PositionIndex;
            }

            /// <summary>
            /// Sets the current bucket as an Empty Entry so it never matches.
            /// </summary>
            public void Clear()
            {
                IsValid = false;
            }
        }

        /// <summary>
        /// If an exact match is found by an internal node, this will add that result to the lookup cache.
        /// </summary>
        /// <param name="level">the level of this node.</param>
        /// <param name="childCount">the number of children in this node.</param>
        /// <param name="startingAddress">the address after all the header data and the first reverse looking child index.</param>
        /// <param name="index">child index value of the match.</param>
        public void CacheExactMatch(byte level, int childCount, long startingAddress, int index)
        {
            BucketCache cache = m_cache[level - 1];

            if (index == childCount - 1)
            {
                //If it is the last entry in the table, 
                //the upper bounds is the same as the upper bounds of its parent
                m_stream.Position = startingAddress + m_internalStructureSize * index;
                cache.LowerBound.LoadValue(m_stream);
                cache.Bucket = m_stream.ReadUInt32();

                if ((m_cache[level].Mode & BucketCache.CacheMode.UpperIsValidMask) > 0)
                {
                    cache.Mode = BucketCache.CacheMode.Bounded;
                    cache.UpperBound = m_cache[level].UpperBound;
                }
                else
                {
                    cache.Mode = BucketCache.CacheMode.UpperIsMissing;
                }
            }
            else
            {
                m_stream.Position = startingAddress + m_internalStructureSize * index;
                cache.LowerBound.LoadValue(m_stream);
                cache.Bucket = m_stream.ReadUInt32();
                cache.UpperBound.LoadValue(m_stream);
                cache.Mode = BucketCache.CacheMode.Bounded;
            }
        }

        /// <summary>
        /// If an exact match is not found by an internal node, this will add that result to the lookup cache.
        /// </summary>
        /// <param name="level">the level of this node.</param>
        /// <param name="childCount">the number of children in this node.</param>
        /// <param name="startingAddress">the address after all the header data and the first reverse looking child index.</param>
        /// <param name="index">child index value of the match.</param>
        public void CacheNotExactMatch(byte level, int childCount, long startingAddress, int index)
        {
            BucketCache cache = m_cache[level - 1];
            
            if (index == 0)
            {
                //if it is before the first key in the table, 
                //the lower bounds is the same as the lower bounds of its parent
                m_stream.Position = startingAddress + m_internalStructureSize * index - 4;
                cache.Bucket = m_stream.ReadUInt32();
                cache.UpperBound.LoadValue(m_stream);
                if ((m_cache[level].Mode & BucketCache.CacheMode.LowerIsValidMask) > 0)
                {
                    cache.Mode = BucketCache.CacheMode.Bounded;
                    cache.LowerBound = m_cache[level].LowerBound;
                }
                else
                {
                    cache.Mode = BucketCache.CacheMode.LowerIsMissing;
                }
            }
            else if (index == childCount)
            {
                //If it is past the last key in the table, 
                //the upper bounds is the same as the upper bounds of its parent
                m_stream.Position = startingAddress + m_internalStructureSize * (index - 1);
                cache.LowerBound.LoadValue(m_stream);
                cache.Bucket = m_stream.ReadUInt32();

                if ((m_cache[level].Mode & BucketCache.CacheMode.UpperIsValidMask) > 0)
                {
                    cache.Mode = BucketCache.CacheMode.Bounded;
                    cache.UpperBound = m_cache[level].UpperBound;
                }
                else
                {
                    cache.Mode = BucketCache.CacheMode.UpperIsMissing;
                }
            }
            else
            {
                //If it is between the upper and lower bounds, the position provided 
                //is the insert postion and therefore needs to go back one key
                m_stream.Position = startingAddress + m_internalStructureSize * (index - 1);
                cache.LowerBound.LoadValue(m_stream);
                cache.Bucket = m_stream.ReadUInt32();
                cache.UpperBound.LoadValue(m_stream);
                cache.Mode = BucketCache.CacheMode.Bounded;
            }

        }

        /// <summary>
        /// If an internal node is modified. The local cache for that node may no longer be valid.
        /// </summary>
        /// <param name="level">the level value of the modified node</param>
        public void ClearCache(byte level)
        {

            m_cache[level - 1].Clear();
        }


        public void GetCachedMatch(ref byte nodeLevel, ref uint nodeIndex, TKey key)
        {
            if (nodeLevel > 0)
            {
                for (byte x = nodeLevel; x > 0; x--)
                {
                    if (m_cache[x - 1].IsMatch(key))
                    {
                        nodeIndex = m_cache[x - 1].Bucket;
                        nodeLevel = (byte)(x - 1);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

    }
}
