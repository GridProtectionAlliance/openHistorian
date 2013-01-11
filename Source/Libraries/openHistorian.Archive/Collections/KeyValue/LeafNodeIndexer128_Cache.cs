//******************************************************************************************************
//  LeafNodeIndexer128_Cache.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  1/11/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Collections.KeyValue
{
    public partial class LeafNodeIndexer128
    {
        struct NodeDetails
        {
            public ulong LowerKey1;
            public ulong LowerKey2;
            public ulong UpperKey1;
            public ulong UpperKey2;
            public long NodeIndex;
            public bool IsLowerNull;
            public bool IsUpperNull;
            public bool IsValid;
            public bool Contains(ulong key1, ulong key2)
            {
                return IsValid &&
                    (IsLowerNull || (IsLessThanOrEqualTo(LowerKey1, LowerKey2, key1, key2))) &&
                    (IsUpperNull || (IsLessThan(key1, key2, UpperKey1, UpperKey2)));
            }


            static int CompareKeys(ulong firstKey1, ulong firstKey2, ulong secondKey1, ulong secondKey2)
            {
                if (firstKey1 > secondKey1) return 1;
                if (firstKey1 < secondKey1) return -1;

                if (firstKey2 > secondKey2) return 1;
                if (firstKey2 < secondKey2) return -1;

                return 0;
            }

            /// <summary>
            /// Returns true if the first key is greater than or equal to the later key
            /// </summary>
            static bool IsGreaterThanOrEqualTo(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
            {
                return (key1 > compareKey1) | ((key1 == compareKey1) & (key2 >= compareKey2));
            }

            /// <summary>
            /// Returns true if the first key is greater than the later key.
            /// </summary>
            static bool IsGreaterThan(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
            {
                return (key1 > compareKey1) | ((key1 == compareKey1) & (key2 > compareKey2));
            }

            /// <summary>
            /// Returns true if the first key is less than or equal to the later key
            /// </summary>
            static bool IsLessThanOrEqualTo(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
            {
                return (key1 < compareKey1) | ((key1 == compareKey1) & (key2 <= compareKey2));
            }

            /// <summary>
            /// Returns true if the first key is less than the later key.
            /// </summary>
            static bool IsLessThan(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
            {
                return (key1 < compareKey1) | ((key1 == compareKey1) & (key2 < compareKey2));
            }

            static bool IsEqual(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
            {
                return (key1 == compareKey1) & (key2 == compareKey2);
            }

            static bool IsNotEqual(ulong key1, ulong key2, ulong compareKey1, ulong compareKey2)
            {
                return (key1 != compareKey1) | (key2 != compareKey2);
            }


        }

        class IndexCache
        {
            NodeDetails[] m_cache;
            int m_lowestValidCache;
            public IndexCache()
            {
                m_cache = new NodeDetails[6];
                ClearCache();
            }

            public void ClearCache()
            {
                m_lowestValidCache = m_cache.Length + 1;
            }

            public void InvalidateCache(int nodeLevel)
            {
                m_lowestValidCache = nodeLevel + 1;
            }

            /// <summary>
            /// Returns true and outputs the node index if the node contains the provide key.
            /// otherwise returns false and nodeIndex remains the same.
            /// </summary>
            public bool NodeContains(int nodeLevel, ulong key1, ulong key2, ref long nodeIndex)
            {
                if (m_lowestValidCache > nodeLevel)
                    return false;
                NodeDetails nodeDetails = m_cache[nodeLevel - 1];
                if (nodeDetails.Contains(key1, key2))
                {
                    nodeIndex = nodeDetails.NodeIndex;
                    return true;
                }
                else
                {
                    //Invalidate this layer and all lower layers.
                    //Array.Clear(m_cache, 0, nodeLevel);
                    m_lowestValidCache = nodeLevel + 1;
                    return false;
                }
            }

            /// <summary>
            /// Caches the provided node node
            /// </summary>
            /// <param name="nodeLevel"></param>
            /// <param name="nodeDetails"></param>
            public void CacheNode(int nodeLevel, NodeDetails nodeDetails)
            {
                m_lowestValidCache = nodeLevel; //Array.Clear(m_cache, 0, nodeLevel);
                m_cache[nodeLevel - 1] = nodeDetails;
            }

        }


    }
}
