//******************************************************************************************************
//  SortedTree256EncodedLeafNodeBase_TreeScanner.cs - Gbtc
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
//  11/24/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace openHistorian.Collections.KeyValue
{
    internal partial class SortedTree256EncodedLeafNodeBase
    {
        private class TreeScanner : ITreeScanner256
        {
            SortedTree256EncodedLeafNodeBase m_tree;
            long m_positionOfCurrentKey;
            long m_rightSiblingNodeIndex;
            long m_nodeIndex;

            ulong m_lastKey1;
            ulong m_lastKey2;
            ulong m_lastValue1;
            ulong m_lastValue2;
            long m_lastPosition;

            public TreeScanner(SortedTree256EncodedLeafNodeBase tree)
            {
                m_tree = tree;
            }

            public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
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

                    m_lastKey1 = 0;
                    m_lastKey2 = 0;
                    m_lastValue1 = 0;
                    m_lastValue2 = 0;
                }

                m_tree.Stream.Position = m_positionOfCurrentKey;

                m_tree.DecodeNextRecord(ref m_lastKey1, ref m_lastKey2, ref m_lastValue1, ref m_lastValue2);

                key1 = m_lastKey1;
                key2 = m_lastKey2;
                value1 = m_lastValue1;
                value2 = m_lastValue2;

                m_positionOfCurrentKey = m_tree.Stream.Position;
                return true;
            }

            public void SeekToKey(ulong key1, ulong key2)
            {
                m_positionOfCurrentKey = 0;
                m_rightSiblingNodeIndex = 0;
                m_nodeIndex = 0;

                m_lastKey1 = 0;
                m_lastKey2 = 0;
                m_lastValue1 = 0;
                m_lastValue2 = 0;
                m_lastPosition = 0;

                m_nodeIndex = m_tree.FindLeafNodeAddress(key1, key2);

                var header = new NodeHeader(m_tree.Stream, m_tree.BlockSize, m_nodeIndex);
                m_lastPosition = m_nodeIndex * m_tree.BlockSize + header.ValidBytes;
                m_rightSiblingNodeIndex = header.RightSiblingNodeIndex;

                m_tree.Stream.Position = m_nodeIndex * m_tree.BlockSize + NodeHeader.Size;

                ulong prevKey1 = 0;
                ulong prevKey2 = 0;
                ulong prevValue1 = 0;
                ulong prevValue2 = 0;
                long prevPos = m_tree.Stream.Position;

                ulong curKey1 = 0;
                ulong curKey2 = 0;
                ulong curValue1 = 0;
                ulong curValue2 = 0;

                while (m_tree.Stream.Position < m_lastPosition)
                {
                    m_tree.DecodeNextRecord(ref curKey1, ref curKey2, ref curValue1, ref curValue2);

                    int compareKeysResults = CompareKeys(curKey1, curKey2, key1, key2);
                    //if (compareKeysResults >= 0)
                    //{
                    //    m_lastKey1 = curKey1;
                    //    m_lastKey2 = curKey2;
                    //    m_lastValue1 = curValue1;
                    //    m_lastValue2 = curValue2;
                    //    m_positionOfCurrentKey = m_tree.Stream.Position;
                    //    break;
                    //}
                    if (compareKeysResults >= 0)
                    {
                        m_lastKey1 = prevKey1;
                        m_lastKey2 = prevKey2;
                        m_lastValue1 = prevValue1;
                        m_lastValue2 = prevValue2;
                        m_positionOfCurrentKey = prevPos;
                        return;
                    }

                    prevKey1 = curKey1;
                    prevKey2 = curKey2;
                    prevValue1 = curValue1;
                    prevValue2 = curValue2;
                    prevPos = m_tree.Stream.Position;
                }
                //read past the end of the stream, the next valid position is the first entry in the sibling

                if (m_rightSiblingNodeIndex == 0)
                {
                    m_lastPosition = 0;
                    m_positionOfCurrentKey = 0;
                }
                else
                {
                    //ToDo: Consider recursion
                    m_nodeIndex = m_rightSiblingNodeIndex;
                    var headerRight = new NodeHeader(m_tree.Stream, m_tree.BlockSize, m_nodeIndex);
                    m_lastPosition = m_nodeIndex * m_tree.BlockSize + headerRight.ValidBytes;
                    m_rightSiblingNodeIndex = headerRight.RightSiblingNodeIndex;
                    m_positionOfCurrentKey = m_nodeIndex * m_tree.BlockSize + NodeHeader.Size;
                }

            }
        }
    }
}
