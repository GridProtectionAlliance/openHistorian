//******************************************************************************************************
//  LeastRecentlyUsedPageReplacement.cs - Gbtc
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
//  4/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using openHistorian.UnmanagedMemory;

namespace openHistorian.IO.Unmanaged
{
    /// <summary>
    /// A page replacement algorithm that utilizes a quasi LRU algorithm.
    /// </summary>
    /// <remarks>
    /// This class is used by <see cref="BufferedFileStream"/> to decide which pages should be replaced.
    /// </remarks>
    unsafe public partial class LeastRecentlyUsedPageReplacement : IDisposable
    {
        #region [ Members ]

        /// <summary>
        /// Contains meta data about each page that is allocated.  
        /// </summary>
        /// <remarks>This structure is passed to the users of this class.</remarks>
        public struct SubPageMetaData
        {
            /// <summary>
            /// A pointer to the first byte of an individual dirty page
            /// </summary>
            public byte* Location;
            /// <summary>
            /// The absolute position of the first byte of an individual page
            /// </summary>
            public long Position;
            /// <summary>
            /// A flag specifiying if the page is dirty
            /// </summary>
            public bool IsDirty;
            /// <summary>
            /// The valid length of the current sub page.
            /// </summary>
            public int Length;
        }

        /// <summary>
        /// A constant used by <see cref="m_ioSessionCurrentlyUsedArrayIndexes"/> to flag 
        /// when an <see cref="IoSession"/> is not referencing any pages.
        /// </summary>
        const int IsCleared = -1;
        /// <summary>
        /// A constant used by <see cref="m_ioSessionCurrentlyUsedArrayIndexes"/> to flag 
        /// when no <see cref="IoSession"/> is assigned to this list position.
        /// </summary>
        const int IsNull = -2;

        int m_bufferPageSize;
        int m_bufferPageSizeMask;
        int m_bufferPageSizeShiftBits;

        int m_dirtyPageSize;
        int m_dirtyPageSizeShiftBits;
        int m_dirtyPageSizeMask;

        bool m_disposed;

        /// <summary>
        /// contains a list of all the meta pages.
        /// </summary>
        /// <remarks>These items in the list are not in any particular order.</remarks>
        PageMetaDataList m_pageList;

        /// <summary>
        /// Contains the currently active IO sessions.
        /// The value is either the array index or <see cref="IsNull"/> or <see cref="IsCleared"/>.
        /// </summary>
        List<int> m_ioSessionCurrentlyUsedArrayIndexes;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of <see cref="LeastRecentlyUsedPageReplacement"/>.
        /// </summary>
        /// <param name="dirtyPageSize">The size of a single dirty page. Must be a power of 2.</param>
        /// <param name="pool">The buffer pool that blocks will be allocated on.</param>
        public LeastRecentlyUsedPageReplacement(int dirtyPageSize, BufferPool pool)
        {
            if (pool.PageSize < 4096)
                throw new ArgumentOutOfRangeException("PageSize Must be greater than 4096", "pool");
            if (dirtyPageSize > pool.PageSize)
                throw new ArgumentOutOfRangeException("Must not be greater than Pool.PageSize", "dirtyPageSize");
            if (!BitMath.IsPowerOfTwo(pool.PageSize))
                throw new ArgumentException("PageSize Must be a power of 2", "pool");
            if (!BitMath.IsPowerOfTwo(dirtyPageSize))
                throw new ArgumentException("Must be a power of 2", "dirtyPageSize");
            if (dirtyPageSize * 64 < pool.PageSize)
                throw new ArgumentException("PageSize Cannot be greater than 64 * dirtyPageSize", "pool");

            m_dirtyPageSize = dirtyPageSize;
            m_dirtyPageSizeMask = dirtyPageSize - 1;
            m_dirtyPageSizeShiftBits = BitMath.CountBitsSet((uint)m_dirtyPageSizeMask);

            m_bufferPageSize = pool.PageSize;
            m_bufferPageSizeMask = pool.PageSize - 1;
            m_bufferPageSizeShiftBits = BitMath.CountBitsSet((uint)m_bufferPageSizeMask);

            m_pageList = new PageMetaDataList(pool);
            m_ioSessionCurrentlyUsedArrayIndexes = new List<int>();
        }

        #endregion

        #region [ Properties ]

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
        /// Attempts to get a sub page. 
        /// </summary>
        /// <param name="position">the absolute position in the stream to get the page for.</param>
        /// <param name="ioSession">the index value of the <see cref="IoSession.IoSessionId"/> of the caller.</param>
        /// <param name="isWriting">a bool indicating if this individual page will be written to.</param>
        /// <param name="subPage">an output field of the resulting sub page</param>
        /// <returns>False if the page does not exists and needs to be added.</returns>
        bool TryGetSubPage(long position, int ioSession, bool isWriting, out SubPageMetaData subPage)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            int positionIndex = (int)(position >> m_bufferPageSizeShiftBits);
            int subPageIndex = (int)((position & m_bufferPageSizeMask) >> m_dirtyPageSizeShiftBits);
            ushort subPageDirtyFlag = (ushort)(1 << subPageIndex);
            int arrayIndex = m_pageList.ToArrayIndex(positionIndex);
            if (arrayIndex < 0)
            {
                subPage = default(SubPageMetaData);
                return false;
            }

            m_ioSessionCurrentlyUsedArrayIndexes[ioSession] = arrayIndex;

            PageMetaDataList.PageMetaData pageMetaData;
            if (isWriting)
                pageMetaData = m_pageList.GetMetaDataPage(arrayIndex, subPageDirtyFlag, 1);
            else
                pageMetaData = m_pageList.GetMetaDataPage(arrayIndex, 0, 1);

            bool isSubPageDirty = ((pageMetaData.IsDirtyFlags & subPageDirtyFlag) != 0);

            subPage = new SubPageMetaData
            {
                IsDirty = isSubPageDirty,
                Location = pageMetaData.LocationOfPage + subPageIndex * m_dirtyPageSize,
                Position = position & ~(long)m_dirtyPageSizeMask,
                Length = m_dirtyPageSize
            };
            return true;
        }

        /// <summary>
        /// Adds a new page to the list of available pages unless it alread exists.
        /// NOTE: The page added must be the entire page and cannot be a subset.
        /// I.E. Equal to the buffer pool size.
        /// </summary>
        /// <param name="position">The position of the first byte in the page</param>
        /// <param name="data">the data to be copied to the internal buffer</param>
        /// <param name="startIndex">the starting index of <see cref="data"/> to copy</param>
        /// <param name="length">the length to copy, must be equal to the buffer pool page size</param>
        /// <returns>True if the page was sucessfully added. False if it already exists and was not added.</returns>
        bool TryAddNewPage(long position, byte[] data, int startIndex, int length)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (length != m_bufferPageSize)
                throw new ArgumentException("Must be equal to BufferPool.PageSize");
            if (data.Length < (long)startIndex + length)
                throw new ArgumentException("Array is not large enough for startIndex + length.", "data");

            int positionIndex = (int)(position >> m_bufferPageSizeShiftBits);
            int subPageIndex = (int)((position & m_bufferPageSizeMask) >> m_dirtyPageSizeShiftBits);
            if (subPageIndex != 0)
                throw new ArgumentException("Is not at the start of a page boundry", "position");

            int arrayIndex = m_pageList.ToArrayIndex(positionIndex);
            if (arrayIndex >= 0)
            {
                return false;
            }

            arrayIndex = m_pageList.AllocateNewPage(positionIndex);

            var pageMetaData = m_pageList.GetMetaDataPage(arrayIndex, 0, 0);
            Marshal.Copy(data, startIndex, (IntPtr)pageMetaData.LocationOfPage, length);

            return true;
        }

        /// <summary>
        /// Creates a new IO Session.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// In order to prevent pages that are currently in use from being garbage collected,
        /// IO Sessions are used to keep track of currently used blocks.
        /// One IO session should be used for each unique request.  
        /// </remarks>
        public IoSession CreateNewIoSession()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            for (int x = 0; x < m_ioSessionCurrentlyUsedArrayIndexes.Count; x++)
            {
                if (m_ioSessionCurrentlyUsedArrayIndexes[x] == IsNull)
                {
                    m_ioSessionCurrentlyUsedArrayIndexes[x] = IsCleared;
                    return new IoSession(this, x);
                }
            }
            m_ioSessionCurrentlyUsedArrayIndexes.Add(IsCleared);
            return new IoSession(this, m_ioSessionCurrentlyUsedArrayIndexes.Count - 1);
        }

        /// <summary>
        /// Removes the current IoSession from the list of available session IDs
        /// This is done on a dispose operation.
        /// </summary>
        /// <param name="ioSession"></param>
        void ReleaseIoSession(int ioSession)
        {
            if (m_disposed)
                return;
            m_ioSessionCurrentlyUsedArrayIndexes[ioSession] = IsNull;
        }

        /// <summary>
        /// De-references the current IoSession's page.
        /// </summary>
        /// <param name="ioSession"></param>
        void ClearIoSession(int ioSession)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_ioSessionCurrentlyUsedArrayIndexes[ioSession] = IsCleared;
        }

        /// <summary>
        /// Executes a collection cycle of the pages that are unused.
        /// </summary>
        /// <returns></returns>
        public int DoCollection(CollectionEventArgs e)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return m_pageList.DoCollection(1, CheckForCollection, e);
        }

        /// <summary>
        /// Returns true if the page can be collected because it is not currently being referenced.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool CheckForCollection(int index)
        {
            return !m_ioSessionCurrentlyUsedArrayIndexes.Contains(index);
        }

        /// <summary>
        /// Marks the entire page as written to the disk.
        /// </summary>
        /// <param name="pageMetaData"></param>
        /// <remarks>Calling this function does not write the data to the 
        /// base stream.  It only sets the status of the corresponding page.</remarks>
        public void ClearDirtyBits(PageMetaDataList.PageMetaData pageMetaData)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            //ToDo: Commenting this out could cause some problems in the future. I'll need to somehow handle this properly. 
            //ToDO: The problem exists when a user opens a read transaction and has a hold on a block that needs to be flushed.
            //ToDo: Since this block may tell the binary stream that you can write to it, this flag is thrown.
            //if (m_ioSessionCurrentlyUsedArrayIndexes.Contains(pageMetaData.ArrayIndex))
            //    throw new NotSupportedException("A page's dirty bits can only be cleared if the page is currently not being used.");

            m_pageList.ClearDirtyBits(pageMetaData.ArrayIndex);
        }

        /// <summary>
        /// Returns all dirty pages in this class.  If pages are subsequentially written to the disk,
        /// be sure to clear the dirty bits via <see cref="ClearDirtyBits"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PageMetaDataList.PageMetaData> GetDirtyPages(bool skipPagesInUse = false)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return m_pageList.GetDirtyPages(x => (skipPagesInUse && m_ioSessionCurrentlyUsedArrayIndexes.Contains(x)));
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="LeastRecentlyUsedPageReplacement"/> object.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    m_pageList.Dispose();
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #region [ Helper Methods ]

        #endregion

        #endregion

    }
}
