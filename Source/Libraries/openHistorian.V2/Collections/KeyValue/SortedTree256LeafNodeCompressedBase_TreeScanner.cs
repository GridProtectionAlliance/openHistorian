//******************************************************************************************************
//  SortedTree256LeafNodeCompressedBase_TreeScanner.cs - Gbtc
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
//  11/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace openHistorian.V2.Collections.KeyValue
{
    public partial class SortedTree256LeafNodeCompressedBase
    {
        private class TreeScanner : ITreeScanner256
        {
            SortedTree256LeafNodeCompressedBase m_tree;
            long m_positionOfCurrentKey;
            long m_rightSiblingNodeIndex;
            long m_nodeIndex;

            ulong m_lastKey1;
            ulong m_lastKey2;
            ulong m_lastValue1;
            ulong m_lastValue2;
            long m_lastPosition;

            public TreeScanner(SortedTree256LeafNodeCompressedBase tree)
            {
                m_tree = tree;
            }

            public bool GetNextKey(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
            {
                //If there are no more records in the current node.
                if (m_positionOfCurrentKey >= m_lastPosition)
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
                    m_lastPosition = m_tree.BlockSize * m_nodeIndex + header.ValidBytes;
                    m_rightSiblingNodeIndex = header.RightSiblingNodeIndex;
                    m_positionOfCurrentKey = m_tree.BlockSize * m_nodeIndex + NodeHeader.Size;
                }

                m_tree.Stream.Position = m_positionOfCurrentKey;

                m_lastKey1 ^= m_tree.Stream.ReadUInt64();
                m_lastKey2 ^= m_tree.Stream.ReadUInt64();
                m_lastValue1 ^= m_tree.Stream.ReadUInt64();
                m_lastValue2 ^= m_tree.Stream.ReadUInt64();

                key1 = m_lastKey1;
                key2 = m_lastKey2;
                value1 = m_lastValue1;
                value2 = m_lastValue2;

                m_positionOfCurrentKey = m_tree.Stream.Position;
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
                m_lastPosition = m_nodeIndex * m_tree.BlockSize + header.ValidBytes;
                m_rightSiblingNodeIndex = header.RightSiblingNodeIndex;

                ulong curKey1 = 0;
                ulong curKey2 = 0;
                ulong curValue1 = 0;
                ulong curValue2 = 0;

                while (m_tree.Stream.Position < m_lastPosition)
                {
                    curKey1 = curKey1 ^ m_tree.Stream.Read7BitUInt64();
                    curKey2 = curKey2 ^ m_tree.Stream.Read7BitUInt64();
                    curValue1 = curValue1 ^ m_tree.Stream.Read7BitUInt64();
                    curValue2 = curValue2 ^ m_tree.Stream.Read7BitUInt64();

                    int compareKeysResults = CompareKeys(key1, key2, curKey1, curKey2);
                    if (compareKeysResults >= 0)
                    {
                        break;
                    }
                }
                m_lastKey1 = curKey1;
                m_lastKey2 = curKey2;
                m_lastValue1 = curValue1;
                m_lastValue2 = curValue2;
                m_positionOfCurrentKey = m_tree.Stream.Position;
            }
        }
    }
}
