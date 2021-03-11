//******************************************************************************************************
//  PageLock.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  2/9/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure.Media
{
    internal partial class PageReplacementAlgorithm
    {
        /// <summary>
        /// Used to hold a lock on a page to prevent it from being collected by the collection engine.
        /// </summary>
        abstract internal class PageLock
            : BinaryStreamIoSessionBase, IEquatable<PageLock>
        {
            private readonly PageReplacementAlgorithm m_parent;
            private readonly int m_hashCode;
            private int m_currentPageIndex;
            private bool m_disposed;

            /// <summary>
            /// Creates an unallocated block.
            /// </summary>
            protected PageLock(PageReplacementAlgorithm parent)
            {
                m_hashCode = DateTime.UtcNow.Ticks.GetHashCode();
                m_parent = parent;
                m_currentPageIndex = -1;

                lock (m_parent.m_syncRoot)
                {
                    if (m_parent.m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    m_parent.m_arrayIndexLocks.Add(this);
                }
            }

            /// <summary>
            /// Gets the page index associated with the page
            /// that is cached. 
            /// Returns a -1 if no page is currently being used.
            /// </summary>
            public int CurrentPageIndex => m_currentPageIndex;

            /// <summary>
            /// Releases a lock
            /// </summary>
            public override void Clear()
            {
                m_currentPageIndex = -1;
            }

            protected override void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    m_currentPageIndex = -1;
                    m_disposed = true;
                    if (disposing)
                    {
                        lock (m_parent.m_syncRoot)
                        {
                            if (!m_parent.m_disposed)
                            {
                                m_parent.m_arrayIndexLocks.Remove(this);
                            }
                        }
                    }
                }
                base.Dispose(disposing);
            }


            /// <summary>
            /// Attempts to get a sub page. 
            /// </summary>
            /// <param name="position">the absolute position in the stream to get the page for.</param>
            /// <param name="location">a pointer for the page</param>
            /// <returns>False if the page does not exists and needs to be added.</returns>
            public bool TryGetSubPage(long position, out IntPtr location)
            {
                lock (m_parent.m_syncRoot)
                {
                    if (m_parent.m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    if (position < 0)
                        throw new ArgumentOutOfRangeException("position", "Cannot be negative");
                    if (position > m_parent.m_maxValidPosition)
                        throw new ArgumentOutOfRangeException("position", "Position index can no longer be specified as an Int32");
                    if ((position & m_parent.m_memoryPageSizeMask) != 0)
                        throw new ArgumentOutOfRangeException("position", "must lie on a page boundary");

                    int positionIndex = (int)(position >> m_parent.m_memoryPageSizeShiftBits);
                    if (m_parent.m_pageList.TryGetPageIndex(positionIndex, out int pageIndex))
                    {
                        m_currentPageIndex = pageIndex;
                        location = m_parent.m_pageList.GetPointerToPage(pageIndex, 1);
                        return true;
                    }
                    location = default;
                    return false;
                }
            }

            /// <summary>
            /// Adds a page to the list of available pages if it does not exist.
            /// otherwise, returns the page already in the list.
            /// </summary>
            /// <param name="position">The position of the first byte in the page</param>
            /// <param name="startOfMemoryPoolPage">the pointer acquired by the memory pool to this data.</param>
            /// <param name="memoryPoolIndex">the memory pool index for this data</param>
            /// <param name="wasPageAdded">Determines if the page provided was indeed added to the list.</param>
            /// <returns>The pointer to the page for this position</returns>
            /// <remarks>If <see cref="wasPageAdded"/> is false, the calling function should 
            /// return the page back to the memory pool.
            /// </remarks>
            public IntPtr GetOrAddPage(long position, IntPtr startOfMemoryPoolPage, int memoryPoolIndex, out bool wasPageAdded)
            {
                lock (m_parent.m_syncRoot)
                {
                    if (m_parent.m_disposed)
                        throw new ObjectDisposedException(GetType().FullName);
                    if (position < 0)
                        throw new ArgumentOutOfRangeException("position", "Cannot be negative");
                    if (position > m_parent.m_maxValidPosition)
                        throw new ArgumentOutOfRangeException("position", "Position index can no longer be specified as an Int32");
                    if ((position & m_parent.m_memoryPageSizeMask) != 0)
                        throw new ArgumentOutOfRangeException("position", "must lie on a page boundary");
                    int positionIndex = (int)(position >> m_parent.m_memoryPageSizeShiftBits);

                    if (m_parent.m_pageList.TryGetPageIndex(positionIndex, out int pageIndex))
                    {
                        m_currentPageIndex = pageIndex;
                        IntPtr location = m_parent.m_pageList.GetPointerToPage(pageIndex, 1);
                        wasPageAdded = false;
                        return location;
                    }
                    m_currentPageIndex = m_parent.m_pageList.AddNewPage(positionIndex, startOfMemoryPoolPage, memoryPoolIndex); 
                    wasPageAdded = true;
                    return startOfMemoryPoolPage;
                }
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <returns>
            /// true if the specified object  is equal to the current object; otherwise, false.
            /// </returns>
            /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
            public override bool Equals(object obj)
            {
                return ReferenceEquals(this, obj);
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(PageLock other)
            {
                return ReferenceEquals(null, other);
            }

            /// <summary>
            /// Serves as a hash function for a particular type. 
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="T:System.Object"/>.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override int GetHashCode()
            {
                return m_hashCode;
            }
        }
    }
}