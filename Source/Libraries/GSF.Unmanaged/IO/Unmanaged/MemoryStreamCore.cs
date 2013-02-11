//******************************************************************************************************
//  MemoryStreamCore.cs - Gbtc
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
//  2/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using GSF.UnmanagedMemory;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// Provides a dynamically sizing sequence of unmanaged data.
    /// </summary>
    public class MemoryStreamCore : IDisposable
    {
        /// <summary>
        /// This class was created to allow settings update to be atomic.
        /// </summary>
        class Settings
        {
            const int Mask = 1023;
            const int ElementsPerRow = 1024;
            const int ShiftBits = 10;

            public int PageCount { get; private set; }
            int[][] m_pageIndex;
            IntPtr[][] m_pagePointer;

            public Settings()
            {
                m_pageIndex = new int[4][];
                m_pagePointer = new IntPtr[4][];
                PageCount = 0;
            }

            public IntPtr GetPointer(int page)
            {
                return m_pagePointer[page >> ShiftBits][page & Mask];
            }

            int GetIndex(int page)
            {
                return m_pageIndex[page >> ShiftBits][page & Mask];
            }

            public bool AddingRequiresClone
            {
                get
                {
                    return m_pagePointer.Length * ElementsPerRow == PageCount;
                }
            }

            void EnsureCapacity()
            {
                if (AddingRequiresClone)
                {
                    var oldIndex = m_pageIndex;
                    var oldPointer = m_pagePointer;

                    m_pageIndex = new int[m_pageIndex.Length * 2][];
                    m_pagePointer = new IntPtr[m_pagePointer.Length * 2][];
                    oldIndex.CopyTo(m_pageIndex, 0);
                    oldPointer.CopyTo(m_pagePointer, 0);
                }

                int bigIndex = PageCount >> ShiftBits;
                if (m_pageIndex[bigIndex] == null)
                {
                    m_pageIndex[bigIndex] = new int[ElementsPerRow];
                    m_pagePointer[bigIndex] = new IntPtr[ElementsPerRow];
                }

            }

            public void AddNewPage(IntPtr pagePointer, int pageIndex)
            {
                EnsureCapacity();
                int index = PageCount;
                int bigIndex = index >> ShiftBits;
                int smallIndex = index & Mask;
                m_pageIndex[bigIndex][smallIndex] = pageIndex;
                m_pagePointer[bigIndex][smallIndex] = pagePointer;
                Thread.MemoryBarrier(); //Incrementing the page count must occur after the data is correct.
                PageCount++;
            }

            public Settings Clone()
            {
                return (Settings)MemberwiseClone();
            }

            /// <summary>
            /// Returns all of the buffer pool page indexes used by this class
            /// </summary>
            /// <returns></returns>
            public IEnumerable<int> GetAllPageIndexes()
            {
                for (int x = 0; x < PageCount; x++)
                {
                    yield return GetIndex(x);
                }
            }
        }

        #region [ Members ]

        object m_syncRoot;

        volatile Settings m_settings;

        /// <summary>
        /// The buffer pool to utilize
        /// </summary>
        BufferPool m_pool;

        /// <summary>
        /// The first position that can be accessed by users of this stream
        /// </summary>
        long m_firstValidPosition;
        long m_invertMask;
        /// <summary>
        /// The first position of this stream. This may be different from <see cref="m_firstValidPosition"/> 
        /// due to alignment requirements
        /// </summary>
        long m_firstAddressablePosition;
        /// <summary>
        /// The number of bits in the page size.
        /// </summary>
        int m_shiftLength;
        /// <summary>
        /// The size of each page.
        /// </summary>
        int m_pageSize;
        /// <summary>
        /// Releases all the resources used by the <see cref="MemoryFile"/> object.
        /// </summary>
        bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="MemoryStreamCore"/> using the default <see cref="BufferPool"/>.
        /// </summary>
        public MemoryStreamCore()
            : this(GSF.Globals.BufferPool)
        {
        }

        /// <summary>
        /// Create a new <see cref="MemoryStreamCore"/>
        /// </summary>
        public MemoryStreamCore(BufferPool pool)
        {
            m_pool = pool;
            m_shiftLength = pool.PageShiftBits;
            m_pageSize = pool.PageSize;
            m_invertMask = ~(long)(pool.PageSize - 1);
            m_settings = new Settings();
            m_syncRoot = new object();
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="MemoryStreamCore"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~MemoryStreamCore()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets if the stream has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// Gets the length of the current stream.
        /// </summary>
        public long Length
        {
            get
            {
                return (long)m_pageSize * m_settings.PageCount;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Configure the natural alignment of the data.
        /// </summary>
        /// <param name="startPosition">The first addressable position</param>
        public void ConfigureAlignment(long startPosition)
        {
            ConfigureAlignment(startPosition, 1);
        }

        /// <summary>
        /// Configure the natural alignment of the data.
        /// </summary>
        /// <param name="startPosition">The first addressable position</param>
        /// <param name="alignment">Forces alignment on this boundary.
        /// Alignment must be a factor of the BufferPool's page boudary.</param>
        public void ConfigureAlignment(long startPosition, int alignment)
        {
            if (startPosition < 0)
                throw new ArgumentOutOfRangeException("startPosition", "Cannot be negative");
            if (alignment <= 0)
                throw new ArgumentOutOfRangeException("alignment", "Must be a positive factor of the buffer pool's page size.");
            if (alignment > m_pageSize)
                throw new ArgumentOutOfRangeException("alignment", "Cannot be greater than the buffer pool's page size.");
            if ((m_pageSize % alignment) != 0)
                throw new ArgumentException("Must be an even factor of the buffer pool's page size", "alignment");

            m_firstValidPosition = startPosition;
            m_firstAddressablePosition = startPosition - (startPosition % alignment);
        }

        /// <summary>
        /// Gets a block for the following Io session.
        /// </summary>
        /// <param name="position">the block returned must contain this position</param>
        /// <param name="firstPointer">the pointer for the first byte of the block</param>
        /// <param name="firstPosition">the position that corresponds to <see cref="firstPointer"/></param>
        /// <param name="length">the length of the block</param>
        public void GetBlock(long position, out IntPtr firstPointer, out long firstPosition, out int length)
        {
            if (m_disposed)
                throw new ObjectDisposedException("MemoryStream");
            if (position < m_firstValidPosition)
                throw new ArgumentOutOfRangeException("position", "position is before the beginning of the stream");

            length = m_pageSize;
            firstPosition = ((position - m_firstAddressablePosition) & m_invertMask) + m_firstAddressablePosition;
            firstPointer = GetPage(position - m_firstAddressablePosition);

            if (firstPosition < m_firstValidPosition)
            {
                firstPointer += (int)(m_firstValidPosition - firstPosition);
                length -= (int)(m_firstValidPosition - firstPosition);
                firstPosition = m_firstValidPosition;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MemoryFile"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    m_pool.ReleasePages(m_settings.GetAllPageIndexes());
                }
                finally
                {
                    m_pool = null;
                    m_settings = null;
                    m_disposed = true;
                }
            }
        }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Returns the page that corresponds to the absolute position.  
        /// This function will also autogrow the stream.
        /// </summary>
        /// <param name="position">The position to use to calculate the page to retrieve</param>
        /// <returns></returns>
        IntPtr GetPage(long position)
        {
            Settings settings = m_settings;

            int pageIndex = (int)(position >> m_shiftLength);

            if (pageIndex >= settings.PageCount)
            {
                IncreasePageCount(pageIndex + 1);
                settings = m_settings;
            }

            return settings.GetPointer(pageIndex);
        }

        /// <summary>
        /// Increases the size of the Memory Stream and updated the settings if needed
        /// </summary>
        /// <param name="pageCount"></param>
        void IncreasePageCount(int pageCount)
        {
            lock (m_syncRoot)
            {
                bool cloned = false;
                Settings settings = m_settings;
                if (settings.AddingRequiresClone)
                {
                    cloned = true;
                    settings = settings.Clone();
                }

                //If there are not enough pages in the stream, add enough.
                while (pageCount > settings.PageCount)
                {
                    int pageIndex;
                    IntPtr pagePointer;
                    m_pool.AllocatePage(out pageIndex, out pagePointer);
                    Memory.Clear(pagePointer, m_pool.PageSize);
                    settings.AddNewPage(pagePointer, pageIndex);
                }

                if (cloned)
                {
                    Thread.MemoryBarrier(); // make sure that all of the settings are saved before assigning
                    m_settings = settings;
                }
            }
        }

        #endregion

        /// <summary>
        /// Reads from the underlying stream the requested set of data. 
        /// This function is more user friendly than calling GetBlock().
        /// </summary>
        /// <param name="position">the starting position of the read</param>
        /// <param name="pointer">an output pointer to <see cref="position"/>.</param>
        /// <param name="validLength">the number of bytes that are valid after this position.</param>
        /// <returns></returns>
        public void ReadBlock(long position, out IntPtr pointer, out int validLength)
        {
            long firstPosition;
            GetBlock(position, out pointer, out firstPosition, out validLength);
            int seekDistance = (int)(position - firstPosition);
            validLength -= seekDistance;
            pointer += seekDistance;
        }

        public void CopyTo(long position, IntPtr dest, int length)
        {
        TryAgain:

            IntPtr src;
            int validLength;
            ReadBlock(position, out src, out validLength);
            if (validLength < length)
            {
                Memory.Copy(src,dest,validLength);
                length -= validLength;
                dest += validLength;
                goto TryAgain;
            }
            Memory.Copy(src, dest, length);
        }
    }
}
