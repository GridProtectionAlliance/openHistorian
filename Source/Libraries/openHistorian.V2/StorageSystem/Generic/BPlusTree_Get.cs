//******************************************************************************************************
//  BPlusTree_Get.cs - Gbtc
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
//  2/18/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;

namespace openHistorian.V2.StorageSystem.Generic
{
    /// <summary>
    /// This class only concerns itself with the get requirement of the B+ Tree.
    /// </summary>
    public partial class BPlusTree<TKey, TValue>
    {
        /// <summary>
        /// Returns the byte array for the data requested with the following key.  
        /// </summary>
        /// <param name="nodeIndex">the offset for the start of the node.</param>
        /// <param name="nodeLevel">the level number of this index.</param>
        /// <param name="key">the key to match.</param>
        /// <returns>-1 if the key could not be found, the address if the key could be found.</returns>
        /// <remarks>this function is designed to be recursive and will continue calling itself until the data is reached.</remarks>
        TValue GetKey(uint nodeIndex, byte nodeLevel, TKey key)
        {
            m_cache.GetCachedMatch(ref nodeLevel, ref nodeIndex, key);
            
            for (byte levelCount = nodeLevel; levelCount > 0; levelCount--)
            {
                nodeIndex = InternalNodeGetNodeIndex(key, nodeIndex, levelCount);
            }

            return LeafNodeGetValueAddress(key, nodeIndex);
        }

    }
}
