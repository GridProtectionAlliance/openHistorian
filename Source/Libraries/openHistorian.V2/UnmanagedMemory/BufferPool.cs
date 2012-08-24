//******************************************************************************************************
//  BufferPool.cs - Gbtc
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
//  3/16/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace openHistorian.V2.UnmanagedMemory
{
    /// <summary>
    /// This class allocates and pools unmanaged memory.
    /// </summary>
    public class BufferPool : IDisposable
    {
        #region [ Members ]

        bool m_disposed;

        BufferPoolSettings m_settings;

        BufferPoolBlocks m_blocks;

        BufferPoolCollectionEngine m_collectionEngine;

        /// <summary>
        /// Each page will be exactly this size (Based on RAM)
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Provides a mask that the user can apply that can 
        /// be used to get the offset position of a page.
        /// </summary>
        public int PageMask { get; private set; }

        /// <summary>
        /// Gets the number of bits that must be shifted to calculate an index of a position.
        /// This is not the same as a page index that is returned by the allocate functions.
        /// </summary>
        public int PageShiftBits { get; private set; }

        /// <summary>
        /// Requests that classes using this buffer pool release any unused buffers.
        /// Failing to do so may result in an out of memory exception.
        /// <remarks>IMPORTANT NOTICE:  All collection must be performed on this thread.
        /// if calling any method of <see cref="BufferPool"/> with a different thread 
        /// this will likely cause a deadlock.  
        /// You must use the calling thread to release all objects.</remarks>
        /// </summary>
        public event EventHandler<CollectionEventArgs> RequestCollection
        {
            add
            {
                lock (m_syncRoot)
                {
                    m_collectionEngine.AddEvent(value);
                }
            }
            remove
            {
                lock (m_syncRoot)
                {
                    m_collectionEngine.RemoveEvent(value);
                }
            }
        }

        /// <summary>
        /// Used for synchronizing modifications to this class.
        /// </summary>
        object m_syncRoot;

        #endregion

        #region [ Constructors ]

        public BufferPool(int pageSize)
        {
            if (pageSize < 4096 || pageSize > 256 * 1024)
                throw new ArgumentOutOfRangeException("pageSize", "Page size must be between 4KB and 256KB and a power of 2");
            if (!HelperFunctions.IsPowerOfTwo((uint)pageSize))
                throw new ArgumentOutOfRangeException("pageSize", "Page size must be between 4KB and 256KB and a power of 2");

            m_settings = new BufferPoolSettings(pageSize);
            m_blocks = new BufferPoolBlocks(m_settings);
            m_syncRoot = new object();
            PageSize = pageSize;
            PageMask = pageSize - 1;
            PageShiftBits = HelperFunctions.CountBits((uint)PageMask);
            m_collectionEngine = new BufferPoolCollectionEngine(this, m_blocks, m_settings);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Defines the target amount of memory. No garbage collection will occur until this threshold has been reached.
        /// </summary>
        public long TargetBufferSize
        {
            get
            {
                return m_settings.TargetBufferSize;
            }
            set
            {
                lock (m_syncRoot)
                {
                    m_settings.TargetBufferSize = value;
                }
            }
        }

        /// <summary>
        /// Defines the maximum amount of memory before excessive garbage collection will occur.
        /// </summary>
        public long MaximumBufferSize
        {
            get
            {
                return m_settings.MaximumBufferSize;
            }
            set
            {
                lock (m_syncRoot)
                {
                    m_settings.MaximumBufferSize = value;
                }
            }
        }


        /// <summary>
        /// Returns the number of bytes currently allocated to the buffer pool from windows.
        /// </summary>
        public long TotalBufferSize
        {
            get
            {
                return m_blocks.BufferPoolSize;
            }
        }

        /// <summary>
        /// Returns the number of bytes currently allocated by the buffer pool to other objects
        /// </summary>
        public long AllocatedBytes
        {
            get
            {
                return m_blocks.AllocatedPagesBytes;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Requests a page from the buffered pool.
        /// If there is not a free one available, method will block
        /// and request a collection of unused pages by raising 
        /// <see cref="RequestCollection"/> event.
        /// </summary>
        /// <param name="index">the index id of the page that was allocated</param>
        /// <param name="addressPointer"> outputs a address that can be used
        /// to access this memory address.  You cannot call release with this parameter.
        /// Use the returned index to release pages.</param>
        /// <remarks>The page allocated will not be initialized, 
        /// so assume that the data is garbage.</remarks>
        public void AllocatePage(out int index, out IntPtr addressPointer)
        {
            lock (m_syncRoot)
            {
                if (m_blocks.IsFull)
                {
                    m_collectionEngine.AllocateMoreFreeSpace();
                }
                if (!m_blocks.TryAllocatePage(out index, out addressPointer))
                {
                    throw new OutOfMemoryException("Buffer Pool is full");
                }
            }
        }

        /// <summary>
        /// Releases the page back to the buffer pool for reallocation.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <remarks>The page released will not be initialized.
        /// Releasing a page is on the honor system.  
        /// Rereferencing a released page will most certainly cause 
        /// unexpected crashing or data corruption or any other unexplained behavior.
        /// </remarks>
        public void ReleasePage(int pageIndex)
        {
            lock (m_syncRoot)
            {
                if (m_blocks.TryReleasePage(pageIndex))
                {
                    //ToDo: Consider calling the garbage collection routine and allow it to consider shrinking the pool.
                }
            }
        }

        public void ReleasePages(IEnumerable<int> pageIndexes)
        {
            lock (m_syncRoot)
            {
                foreach (int x in pageIndexes)
                {
                    m_blocks.TryReleasePage(x);
                }
            }
        }

    
        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="BufferPool"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    m_blocks.Dispose();
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #endregion
    }
}
