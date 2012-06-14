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
using openHistorian.V2.Collections;

namespace openHistorian.V2.UnmanagedMemory
{
    public enum BufferPoolCollectionMode
    {
        Normal,
        Emergency,
        Critical
    }
    /// <summary>
    /// This class allocates and pools unmanaged memory.
    /// </summary>
    public class BufferPool : IDisposable
    {
        #region [ Members ]

        bool m_disposed;

        BufferPoolSettings m_settings;

        BufferPoolBlocks m_blocks;

        /// <summary>
        /// Each page will be exactly this size (Based on RAM)
        /// </summary>
        public int PageSize { get; private set; }

        public int PageMask { get; private set; }

        public int PageShiftBits { get; private set; }

        /// <summary>
        /// If after a collection the free space percentage is not at least this much
        /// grow the buffer pool to have this much free.
        /// </summary>
        float m_desiredFreeSpaceAfterCollection = 0.25f;
        /// <summary>
        /// If after a collection the free space percentage is greater than this
        /// then shrink the size of the buffer. 
        /// </summary>
        float m_shrinkIfFreeSpaceAfterCollection = 0.75f;

        /// <summary>
        /// Requests that classes using this buffer pool release any unused buffers.
        /// Failing to do so may result in an out of memory exception.
        /// <remarks>IMPORTANT NOTICE:  All collection must be performed on this thread.
        /// if calling any method of <see cref="BufferPool"/> with a different thread 
        /// this will likely cause a deadlock.  
        /// You must use the calling thread to release all objects.</remarks>
        /// </summary>
        public event Action<BufferPoolCollectionMode> RequestCollection;

        /// <summary>
        /// Used for synchronizing modifications to this class.
        /// </summary>
        object m_syncRoot;

        #endregion

        #region [ Constructors ]

        public BufferPool(int pageSize)
        {
            if (pageSize < 4096 || pageSize > 256 * 1024)
                throw new ArgumentOutOfRangeException("Page size must be between 4KB and 256KB and a power of 2");
            if (!HelperFunctions.IsPowerOfTwo((uint)pageSize))
                throw new ArgumentOutOfRangeException("Page size must be between 4KB and 256KB and a power of 2");

            m_settings = new BufferPoolSettings(pageSize);
            m_blocks = new BufferPoolBlocks(m_settings);
            m_syncRoot = new object();
            PageSize = pageSize;
            PageMask = pageSize - 1;
            PageShiftBits = HelperFunctions.CountBits((uint)PageMask);
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="BufferPool"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~BufferPool()
        {
            Dispose(false);
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
                return m_blocks.AllocatedBytes;
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
                    RequestMoreSpace();
                }
                m_blocks.AllocatePage(out index, out addressPointer);
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
                m_blocks.ReleasePage(pageIndex);
            }
        }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// This procedure will free up unused blocks/allocate more space.
        /// If no more space can be allocated, an out of memory exception will occur.
        /// </summary>
        void RequestMoreSpace()
        {
            if (RequestCollection != null)
                RequestCollection(BufferPoolCollectionMode.Normal);

            if (m_blocks.FreeSpace < m_desiredFreeSpaceAfterCollection)
            {
                GrowBufferToDesiredMaxPercentage();
            }
            else if (m_blocks.FreeSpace > m_shrinkIfFreeSpaceAfterCollection)
            {
                //ToDo: Figure out how to shrink the buffer pool.
            }

            if (m_blocks.IsFull)
                if (RequestCollection != null)
                    RequestCollection(BufferPoolCollectionMode.Emergency);
            if (m_blocks.IsFull)
                if (RequestCollection != null)
                    RequestCollection(BufferPoolCollectionMode.Critical);
            if (m_blocks.IsFull)
            {
                throw new OutOfMemoryException("Buffer pool is out of memory");
            }
        }

        /// <summary>
        /// Grows the buffer region to have the proper desired amount of free memory.
        /// </summary>
        void GrowBufferToDesiredMaxPercentage()
        {
            while (m_blocks.FreeSpace < m_desiredFreeSpaceAfterCollection)
            {
                //If this goes beyond the desired maximum, exit
                if (!m_blocks.CanAllocateMore)
                    return;
                m_blocks.AllocateWinApiBlock();
            }
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="BufferPool"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="BufferPool"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                        m_blocks.Dispose();
                    }
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
