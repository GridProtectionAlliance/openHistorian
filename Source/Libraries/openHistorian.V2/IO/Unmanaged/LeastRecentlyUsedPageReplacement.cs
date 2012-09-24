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
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.IO.Unmanaged
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
            public byte* Location;
            public long Position;
            public bool IsDirty;
            public int Length;
        }

        const int IsCleared = -1;
        const int IsNull = -2;

        int m_bufferPageSize;
        int m_bufferPageSizeMask;
        int m_bufferPageSizeShiftBits;

        int m_dirtyPageSize;

        int m_dirtyPageSizeShiftBits;

        int m_dirtyPageSizeMask;

        // Delegates

        // Events

        // Fields
        bool m_disposed;

        /// <summary>
        /// contains a list of all the meta pages.
        /// </summary>
        /// <remarks>These items in the list are not in any particular order.</remarks>
        PageMetaDataList m_pageList;

        /// <summary>
        /// Contains the currently active IO sessions that cannot be cleaned up by a collection.
        /// </summary>
        List<int> m_activeBlockIndexes;

        #endregion

        #region [ Constructors ]

        public LeastRecentlyUsedPageReplacement(int dirtyPageSize, BufferPool pool)
        {
            m_dirtyPageSize = dirtyPageSize;
            m_dirtyPageSizeMask = dirtyPageSize - 1;
            m_dirtyPageSizeShiftBits = BitMath.CountBitsSet((uint)m_dirtyPageSizeMask);

            m_bufferPageSize = pool.PageSize;
            m_bufferPageSizeMask = pool.PageSize - 1;
            m_bufferPageSizeShiftBits = BitMath.CountBitsSet((uint)m_bufferPageSizeMask);

            m_pageList = new PageMetaDataList(pool);
            m_activeBlockIndexes = new List<int>();
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

            m_activeBlockIndexes[ioSession] = arrayIndex;

            var pageMetaData = GetPageMetaData(isWriting, subPageDirtyFlag, arrayIndex);
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

        SubPageMetaData CreateNewSubPage(long position, int ioSession, bool isWriting, byte[] data, int startIndex)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            int positionIndex = (int)(position >> m_bufferPageSizeShiftBits);
            int subPageIndex = (int)((position & m_bufferPageSizeMask) >> m_dirtyPageSizeShiftBits);
            ushort subPageDirtyFlag = (ushort)(1 << subPageIndex);
            int arrayIndex = m_pageList.ToArrayIndex(positionIndex);
            if (arrayIndex >= 0)
            {
                throw new Exception("Page already exists");
            }

            arrayIndex = m_pageList.AllocateNewPage(positionIndex);
            m_activeBlockIndexes[ioSession] = arrayIndex;

            var pageMetaData = GetPageMetaData(isWriting, subPageDirtyFlag, arrayIndex);
            bool isSubPageDirty = ((pageMetaData.IsDirtyFlags & subPageDirtyFlag) != 0);

            Marshal.Copy(data, startIndex, (IntPtr)pageMetaData.LocationOfPage, m_bufferPageSize);

            return new SubPageMetaData
            {
                IsDirty = isSubPageDirty,
                Location = pageMetaData.LocationOfPage + subPageIndex * m_dirtyPageSize,
                Position = position & ~(long)m_dirtyPageSizeMask,
                Length = m_dirtyPageSize
            };
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

            for (int x = 0; x < m_activeBlockIndexes.Count; x++)
            {
                if (m_activeBlockIndexes[x] == IsNull)
                {
                    m_activeBlockIndexes[x] = IsCleared;
                    return new IoSession(this, x);
                }
            }
            m_activeBlockIndexes.Add(IsCleared);
            return new IoSession(this, m_activeBlockIndexes.Count - 1);
        }

        void ReleaseIoSession(int ioSession)
        {
            if (m_disposed)
                return;
            m_activeBlockIndexes[ioSession] = IsNull;
        }

        void ClearIoSession(int ioSession)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_activeBlockIndexes[ioSession] = IsCleared;
        }

        /// <summary>
        /// Executes a collection cycle of the pages that are unused.
        /// </summary>
        /// <returns></returns>
        public int DoCollection()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return m_pageList.DoCollection(1, CheckForCollection);
        }

        bool CheckForCollection(int index, int positionIndex)
        {
            return !m_activeBlockIndexes.Contains(index);
        }

        /// <summary>
        /// Marks the entire page as written to the disk.
        /// </summary>
        /// <param name="pageMetaData"></param>
        /// <remarks>These pages are 64KB in size. Calling this function does not write the data to the 
        /// base stream.  It only sets the status of the corresponding page.</remarks>
        public void ClearDirtyBits(PageMetaDataList.PageMetaData pageMetaData)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (m_activeBlockIndexes.Contains(pageMetaData.ArrayIndex))
                throw new NotSupportedException("A page's dirty bits can only be cleared if the page is currently not being used.");

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

            return m_pageList.GetDirtyPages(x => (skipPagesInUse && m_activeBlockIndexes.Contains(x)));
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

        PageMetaDataList.PageMetaData GetPageMetaData(bool isWriting, ulong subPageDirtyFlag, int arrayIndex)
        {
            if (isWriting)
                return m_pageList.GetMetaDataPage(arrayIndex, subPageDirtyFlag, 1);
            else
                return m_pageList.GetMetaDataPage(arrayIndex, 0, 1);
        }

        #endregion

        #endregion

    }
}
