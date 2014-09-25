////******************************************************************************************************
////  IsolatedNode.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  1/4/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////
////******************************************************************************************************

//using System;
//using System.Threading;

//namespace GSF.Collections
//{
//    /// <summary>
//    /// Represents an individual node that allows for items to be added and removed from the 
//    /// queue independently and without locks. 
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    internal class IsolatedNode<T>
//    {
//        private volatile int m_lastBlock;
//        private volatile int m_tail = 0;
//        private volatile int m_head = 0;
//        private readonly T[] m_blocks;

//        public IsolatedNode(int count)
//        {
//            m_blocks = new T[count];
//            m_lastBlock = m_blocks.Length;
//        }

//        /// <summary>
//        /// Determines if this queue cannot have anything else written to it.
//        /// </summary>
//        public bool IsHeadFull
//        {
//            get
//            {
//                return m_head >= m_lastBlock;
//            }
//        }

//        /// <summary>
//        /// Determines if this queue cannot have anything else read from it.
//        /// </summary>
//        public bool IsTailFull
//        {
//            get
//            {
//                return m_tail >= m_lastBlock;
//            }
//        }

//        /// <summary>
//        /// Prevents future writing to this node without reseting it.
//        /// Must be called by coordinating with the writer thread to be thread safe.
//        /// </summary>
//        public void FlagAsFull()
//        {
//            m_lastBlock = m_head;
//        }

//        /// <summary>
//        /// Resets the queue. This operation must be synchronized external 
//        /// from this class with the read and write operations. Therefore it 
//        /// is only recommended to call this once the item has been returned to a 
//        /// buffer pool of some sorts.
//        /// </summary>
//        public void Reset()
//        {
//            if (m_tail != m_head)
//                Array.Clear(m_blocks, m_tail, m_head - m_tail);
//            m_tail = 0;
//            m_head = 0;
//            m_lastBlock = m_blocks.Length;
//        }

//        /// <summary>
//        /// Adds the following item to the queue. Be sure to check if it is full first.
//        /// </summary>
//        /// <param name="item"></param>
//        public void Enqueue(T item)
//        {
//            m_blocks[m_head] = item;
//            Thread.MemoryBarrier();
//            m_head++;
//        }

//        /// <summary>
//        /// Gets the number of items in the node. Note, this may return
//        /// the wrong value if not properly synchronized with writers and readers.
//        /// </summary>
//        public int Count
//        {
//            get
//            {
//                return m_head - m_tail;
//            }
//        }

//        /// <summary>
//        /// Attempts to dequeue from the list.
//        /// </summary>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public bool TryDequeue(out T item)
//        {
//            if (m_tail == m_head)
//            {
//                item = default(T);
//                return false;
//            }
//            Thread.MemoryBarrier();
//            item = m_blocks[m_tail];
//            m_blocks[m_tail] = default(T);
//            m_tail++;
//            return true;
//        }
//    }
//}