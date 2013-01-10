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
        ContinuousQueue<IsolatedNode<T>> m_blocks;
        ResourceQueue<IsolatedNode<T>> m_pooledNodes;

        IsolatedNode<T> m_currentHead;
        IsolatedNode<T> m_currentTail;

        int m_unitCount;

        public IsolatedQueue()
        {
            m_unitCount = 1024;
            m_pooledNodes = new ResourceQueue<IsolatedNode<T>>(() => new IsolatedNode<T>(m_unitCount), 2, 10);
            m_blocks = new ContinuousQueue<IsolatedNode<T>>();
        }

        public void Enqueue(T item)
        {
            if (m_currentHead == null || m_currentHead.IsHeadFull)
            {
                m_currentHead = m_pooledNodes.Dequeue();
                m_currentHead.Reset();
                lock (m_blocks)
                {
                    m_blocks.Enqueue(m_currentHead);
                }
            }
            m_currentHead.Enqueue(item);
        }

        public bool TryDequeue(out T item)
        {
            if (m_currentTail == null || m_currentTail.IsTailFull)
            {
                if (m_currentTail != null)
                {
                    //Don't reset the node on return since it is still
                    //possible for the enqueue thread to be using it. 
                    //Note: If the enqueue thread pulls it off the queue
                    //immediately, this is ok since it will be coordinated at that point.
                    m_pooledNodes.Enqueue(m_currentTail);
                }

                bool success;
                lock (m_blocks)
                {
                    if (m_blocks.Count > 0)
                    {
                        m_currentTail = m_blocks.Dequeue();
                        success = true;
                    }
                    else
                    {
                        success = false;
                    }
                }

                if (!success)
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
    }


}
