//******************************************************************************************************
//  SortedTree256LeafNodeBase_TreeScanner.cs - Gbtc
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

namespace openHistorian.Collections.KeyValue
{
    internal partial class SortedTree256LeafNodeBase
    {
        private class TreeScanner : ITreeScanner256
        {
            SortedTree256LeafNodeBase m_tree;
            int m_nodeRecordCount;
            int m_keyIndexOfCurrentKey;
            long m_rightSiblingNodeIndex;
            long m_nodeIndex;
            public TreeScanner(SortedTree256LeafNodeBase tree)
            {
                m_tree = tree;
            }

            public bool GetNextKey(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
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
                    m_nodeIndex = m_rightSiblingNodeIndex;
                    var header = new NodeHeader(m_tree.Stream, m_tree.BlockSize, m_nodeIndex);
                    m_nodeRecordCount = header.NodeRecordCount;
                    m_rightSiblingNodeIndex = header.RightSiblingNodeIndex;
                    m_keyIndexOfCurrentKey = 0;
                }

                m_tree.Stream.Position = m_nodeIndex * m_tree.BlockSize + m_keyIndexOfCurrentKey * StructureSize + NodeHeader.Size;

                key1 = m_tree.Stream.ReadUInt64();
                key2 = m_tree.Stream.ReadUInt64();
                value1 = m_tree.Stream.ReadUInt64();
                value2 = m_tree.Stream.ReadUInt64();

                //move to the next key
                m_keyIndexOfCurrentKey++;
                return true;
            }

            public void SeekToKey(ulong key1, ulong key2)
            {
                m_nodeIndex = m_tree.RootNodeIndexAddress;
                for (byte nodeLevel = m_tree.RootNodeLevel; nodeLevel > 0; nodeLevel--)
                {
                    m_nodeIndex = m_tree.InternalNodeGetNodeIndexAddress(nodeLevel, m_nodeIndex, key1, key2);
                }
                var header = new NodeHeader(m_tree.Stream, m_tree.BlockSize, m_nodeIndex);
                m_nodeRecordCount = header.NodeRecordCount;
                m_rightSiblingNodeIndex = header.RightSiblingNodeIndex;
                m_tree.FindOffsetOfKey(m_nodeIndex, m_nodeRecordCount, key1, key2, out m_keyIndexOfCurrentKey);
                m_keyIndexOfCurrentKey = (m_keyIndexOfCurrentKey - NodeHeader.Size) / StructureSize;
            }
        }
    }
}
