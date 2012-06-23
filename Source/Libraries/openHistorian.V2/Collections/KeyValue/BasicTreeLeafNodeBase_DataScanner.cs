//******************************************************************************************************
//  BasicTreeLeafNodeBase_DataScanner.cs - Gbtc
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
//  6/23/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.Collections.KeyValue
{
    public partial class BasicTreeLeafNodeBase
    {
        private class DataScanner : IDataScanner
        {
            BasicTreeLeafNodeBase m_tree;
            short m_nodeRecordCount;
            int m_keyIndexOfCurrentKey;
            uint m_leftSiblingNodeIndex, m_rightSiblingNodeIndex;
            uint m_nodeIndex;
            public DataScanner(BasicTreeLeafNodeBase tree)
            {
                m_tree = tree;
            }

            public bool GetNextKey(out long key1, out long key2, out long value1, out long value2)
            {
                //If there are no more records in the current node.
                if (m_keyIndexOfCurrentKey >= m_nodeRecordCount)
                {
                    //If the last leaf node, return false
                    if (m_rightSiblingNodeIndex == 0)
                    {
                        key1 = 0;
                        key2 = 0;
                        value1 = 0;
                        value2 = 0;
                        return false;
                    }

                    //Move to the next node in the linked list.
                    uint previousNode;
                    m_nodeIndex = m_rightSiblingNodeIndex;
                    m_tree.LoadNodeHeader(m_nodeIndex, false, out m_nodeRecordCount, out previousNode, out m_rightSiblingNodeIndex);
                    m_keyIndexOfCurrentKey = 0;
                }

                m_tree.Stream.Position = m_nodeIndex * m_tree.BlockSize + m_keyIndexOfCurrentKey * StructureSize + NodeHeader.Size;

                key1 = m_tree.Stream.ReadInt64();
                key2 = m_tree.Stream.ReadInt64();
                value1 = m_tree.Stream.ReadInt64();
                value2 = m_tree.Stream.ReadInt64();

                //move to the next key
                m_keyIndexOfCurrentKey++;
                return true;
            }

            public void SeekToKey(long key1, long key2)
            {
                m_nodeIndex = m_tree.RootNodeIndex;
                for (byte nodeLevel = m_tree.RootNodeLevel; nodeLevel > 0; nodeLevel--)
                {
                    m_nodeIndex = m_tree.InternalNodeGetIndex(nodeLevel, m_nodeIndex, key1, key2);
                }

                m_tree.LoadNodeHeader(m_nodeIndex, false, out m_nodeRecordCount, out m_leftSiblingNodeIndex, out m_rightSiblingNodeIndex);
                m_tree.FindOffsetOfKey(m_nodeIndex, m_nodeRecordCount, key1, key2, out m_keyIndexOfCurrentKey);
                m_keyIndexOfCurrentKey = (m_keyIndexOfCurrentKey - NodeHeader.Size) / StructureSize;
            }
        }
    }
}
