//******************************************************************************************************
//  IsolatedQueue.cs - Gbtc
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
//  1/4/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace openHistorian.Collections
{
    /// <summary>
    /// Provides a buffer of point data where reads are isolated from writes.
    /// However, reads must be synchronized with other reads and writes must be synchronized with other writes.
    /// </summary>
    /// <remarks>
    /// This class can be slighly quicker than a ConcurrentQueue since it requires that writes
    /// be synchronized with other writes and reads be synchronized with other reads. However, the major benefit 
    /// of this class is that it takes up less memory than a ConcurrentQueue and it uses jagged arrays to discourage
    /// memory fragmentation in the large object pool.
    /// </remarks>
    public class IsolatedQueue<T>
        where T : struct
    {
        ConcurrentQueue<InternalNode> m_blocks;

        InternalNode m_currentHead;
        InternalNode m_currentTail;
        InternalNode m_pooledNode;

        int m_unitCount;

        public IsolatedQueue()
        {
            m_unitCount = 1024;
            m_blocks = new ConcurrentQueue<InternalNode>();
        }

        public void Enqueue(T item)
        {
            if (m_currentHead == null || m_currentHead.IsHeadFull)
            {
                m_currentHead = Interlocked.Exchange(ref m_pooledNode, null);
                if (m_currentHead == null)
                {
                    m_currentHead = new InternalNode(m_unitCount);
                }
                else
                {
                    m_currentHead.Reset();
                }
                m_pooledNode = null;
                m_blocks.Enqueue(m_currentHead);
            }
            m_currentHead.Enqueue(item);
        }

        public bool TryDequeue(out T item)
        {
            if (m_currentTail == null || m_currentTail.IsTailFull)
            {
                if (m_currentTail != null)
                {
                    Interlocked.Exchange(ref m_pooledNode, m_currentTail);
                }
                if (!m_blocks.TryDequeue(out m_currentTail))
                {
                    m_currentTail = null;
                    item = default(T);
                    return false;
                }
            }
            return m_currentTail.TryDequeue(out item);
        }

        /// <summary>
        /// Since the math for the count is pretty complex, this only gives an idea of the
        /// size of the structure.  In other words, a value of zero does not mean this list is empty.
        /// </summary>
        public long EstimateCount
        {
            get
            {
                return m_blocks.Count * (long)m_unitCount;
            }
        }

        class InternalNode
        {
            volatile int m_tail = 0;
            volatile int m_head = 0;
            T[] m_blocks;
            public InternalNode(int count)
            {
                m_blocks = new T[count];
            }

            /// <summary>
            /// Determines if this queue cannot have anything else written to it.
            /// </summary>
            public bool IsHeadFull
            {
                get
                {
                    return m_head >= m_blocks.Length;
                }
            }

            /// <summary>
            /// Determines if this queue cannot have anything else read from it.
            /// </summary>
            public bool IsTailFull
            {
                get
                {
                    return m_tail >= m_blocks.Length;
                }
            }

            /// <summary>
            /// Resets the queue. This operation must be synchronized external 
            /// from this class with the read and write operations. Therefore it 
            /// is only recommended to call this once the item has been returned to a 
            /// buffer pool of some sorts.
            /// </summary>
            public void Reset()
            {
                m_tail = 0;
                m_head = 0;
            }

            /// <summary>
            /// Adds the following item to the queue. Be sure to check if it is full first.
            /// </summary>
            /// <param name="item"></param>
            public void Enqueue(T item)
            {
                m_blocks[m_head] = item;
                Thread.MemoryBarrier();
                m_head++;
            }

            /// <summary>
            /// Attempts to dequeue from the list.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool TryDequeue(out T item)
            {
                if (m_tail == m_head)
                {
                    item = default(T);
                    return false;
                }
                Thread.MemoryBarrier();
                item = m_blocks[m_tail];
                m_tail++;
                return true;
            }


        }
    }


}
