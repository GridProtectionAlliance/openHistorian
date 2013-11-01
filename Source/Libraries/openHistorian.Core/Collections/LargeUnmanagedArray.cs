//******************************************************************************************************
//  LargeUnmanagedArray.cs - Gbtc
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
//  9/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.UnmanagedMemory;

namespace GSF.Collections
{
    /// <summary>
    /// Allows the creation of a large array that is backed by unmanaged memory that is located in the buffer pool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LargeUnmanagedArray<T> : ILargeArray<T>
    {
        private struct Block
        {
            public IntPtr Ptr;
            public int Index;
        }

        private bool m_disposed;
        private MemoryPool m_pool;
        private readonly int m_sizeOfT;
        private readonly int m_elementsPerBlock;
        private List<Block> m_blocks;
        private Func<IntPtr, T> m_read;
        private Action<IntPtr, T> m_write;

        public LargeUnmanagedArray(int sizeOfT, MemoryPool pool, Func<IntPtr, T> read, Action<IntPtr, T> write)
        {
            m_read = read;
            m_write = write;
            m_pool = pool;
            m_blocks = new List<Block>();
            m_sizeOfT = sizeOfT;
            m_elementsPerBlock = pool.PageSize / sizeOfT;
        }

        private void Expand()
        {
            int index;
            IntPtr ptr;

            m_pool.AllocatePage(out index, out ptr);
            m_blocks.Add(new Block() {Index = index, Ptr = ptr});
        }

        public T this[int index]
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (index >= Capacity || index < 0)
                    throw new ArgumentOutOfRangeException("index", "index is outside the bounds of the array");
                int major = index / m_elementsPerBlock;
                int minor = index - major * m_elementsPerBlock;
                return m_read(m_blocks[major].Ptr + m_sizeOfT * minor);
            }
            set
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (index >= Capacity || index < 0)
                    throw new ArgumentOutOfRangeException("index", "index is outside the bounds of the array");
                int major = index / m_elementsPerBlock;
                int minor = index - major * m_elementsPerBlock;
                m_write(m_blocks[major].Ptr + m_sizeOfT * minor, value);
            }
        }

        /// <summary>
        /// Gets the number of items in the array.
        /// </summary>
        public int Capacity
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return m_elementsPerBlock * m_blocks.Count;
            }
        }

        public int SetCapacity(int length)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            while (length > Capacity)
            {
                Expand();
            }
            return Capacity;
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                m_pool.ReleasePages(GetBlocks());
                m_pool = null;
                m_blocks = null;
                m_read = null;
                m_write = null;
            }
        }

        private IEnumerable<int> GetBlocks()
        {
            foreach (Block block in m_blocks)
            {
                yield return block.Index;
            }
        }
    }
}