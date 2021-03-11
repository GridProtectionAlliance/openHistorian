//******************************************************************************************************
//  UnmanagedMemoryStreamCore.cs - Gbtc
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
//  9/30/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// Provides a dynamically sizing sequence of unmanaged data.
    /// </summary>
    public class UnmanagedMemoryStreamCore : IDisposable
    {
        /// <summary>
        /// This class was created to allow settings update to be atomic.
        /// </summary>
        private class Settings
        {
            private const int Mask = 63;
            private const int ElementsPerRow = 64;
            private const int ShiftBits = 6;

            public int PageCount
            {
                get;
                private set;
            }

            private IntPtr[][] m_pagePointer;

            public Settings()
            {
                m_pagePointer = new IntPtr[4][];
                PageCount = 0;
            }

            public IntPtr GetPointer(int page)
            {
                return m_pagePointer[page >> ShiftBits][page & Mask];
            }



            public bool AddingRequiresClone => m_pagePointer.Length * ElementsPerRow == PageCount;

            private void EnsureCapacity()
            {
                if (AddingRequiresClone)
                {
                    IntPtr[][] oldPointer = m_pagePointer;

                    m_pagePointer = new IntPtr[m_pagePointer.Length * 2][];
                    oldPointer.CopyTo(m_pagePointer, 0);
                }

                int bigIndex = PageCount >> ShiftBits;
                if (m_pagePointer[bigIndex] is null)
                {
                    m_pagePointer[bigIndex] = new IntPtr[ElementsPerRow];
                }
            }

            public void AddNewPage(IntPtr pagePointer)
            {
                EnsureCapacity();
                int index = PageCount;
                int bigIndex = index >> ShiftBits;
                int smallIndex = index & Mask;
                m_pagePointer[bigIndex][smallIndex] = pagePointer;
                Thread.MemoryBarrier(); //Incrementing the page count must occur after the data is correct.
                PageCount++;
            }

            public Settings Clone()
            {
                return (Settings)MemberwiseClone();
            }

        }

        #region [ Members ]

            private List<Memory> m_memoryBlocks;

        private readonly object m_syncRoot;

        private volatile Settings m_settings;

        /// <summary>
        /// The first position that can be accessed by users of this stream
        /// </summary>
        private long m_firstValidPosition;

        private readonly long m_invertMask;

        /// <summary>
        /// The first position of this stream. This may be different from <see cref="m_firstValidPosition"/> 
        /// due to alignment requirements
        /// </summary>
        private long m_firstAddressablePosition;

        /// <summary>
        /// The number of bits in the page size.
        /// </summary>
        private readonly int m_shiftLength;

        /// <summary>
        /// The size of each page.
        /// </summary>
        private readonly int m_pageSize;

        /// <summary>
        /// Releases all the resources used by the <see cref="MemoryFile"/> object.
        /// </summary>
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Create a new <see cref="UnmanagedMemoryStreamCore"/> that allocates its own unmanaged memory.
        /// </summary>
        protected UnmanagedMemoryStreamCore(int allocationSize = 4096)
        {
            if (!BitMath.IsPowerOfTwo(allocationSize) || allocationSize < 4096 || allocationSize > 1024 * 1024)
                throw new ArgumentOutOfRangeException("allocationSize", "Must be a power of 2 between 4KB and 1MB");
            m_shiftLength = BitMath.CountBitsSet((uint)(allocationSize - 1));
            m_pageSize = allocationSize;
            m_invertMask = ~(allocationSize - 1);
            m_settings = new Settings();
            m_syncRoot = new object();
            m_memoryBlocks = new List<Memory>();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets if the stream has been disposed.
        /// </summary>
        public bool IsDisposed => m_disposed;

        /// <summary>
        /// Gets the length of the current stream.
        /// </summary>
        public long Length => (long)m_pageSize * m_settings.PageCount;

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
            if (m_pageSize % alignment != 0)
                throw new ArgumentException("Must be an even factor of the buffer pool's page size", "alignment");

            m_firstValidPosition = startPosition;
            m_firstAddressablePosition = startPosition - startPosition % alignment;
        }

        /// <summary>
        /// Gets a block for the following Io session.
        /// </summary>
        public void GetBlock(BlockArguments args)
        {
            if (m_disposed)
                throw new ObjectDisposedException("MemoryStream");
            if (args.Position < m_firstValidPosition)
                throw new ArgumentOutOfRangeException("position", "position is before the beginning of the stream");

            args.Length = m_pageSize;
            args.FirstPosition = ((args.Position - m_firstAddressablePosition) & m_invertMask) + m_firstAddressablePosition;
            args.FirstPointer = GetPage(args.Position - m_firstAddressablePosition);

            if (args.FirstPosition < m_firstValidPosition)
            {
                args.FirstPointer += (int)(m_firstValidPosition - args.FirstPosition);
                args.Length -= (int)(m_firstValidPosition - args.FirstPosition);
                args.FirstPosition = m_firstValidPosition;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MemoryFile"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    foreach (Memory page in m_memoryBlocks)
                        page.Dispose();
                }
                finally
                {
                    m_memoryBlocks = null;
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
        private IntPtr GetPage(long position)
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
        private void IncreasePageCount(int pageCount)
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
                    IntPtr pagePointer;
                    Memory block = new Memory(m_pageSize);
                    pagePointer = block.Address;
                    m_memoryBlocks.Add(block);
                    Memory.Clear(pagePointer, m_pageSize);
                    settings.AddNewPage(pagePointer);
                }

                if (cloned)
                {
                    Thread.MemoryBarrier(); // make sure that all of the settings are saved before assigning
                    m_settings = settings;
                }
            }
        }

        #endregion

        //ToDo: Consider removing these methods
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

            if (m_disposed)
                throw new ObjectDisposedException("MemoryStream");
            if (position < m_firstValidPosition)
                throw new ArgumentOutOfRangeException("position", "position is before the beginning of the stream");

            validLength = m_pageSize;
            firstPosition = ((position - m_firstAddressablePosition) & m_invertMask) + m_firstAddressablePosition;
            pointer = GetPage(position - m_firstAddressablePosition);

            if (firstPosition < m_firstValidPosition)
            {
                pointer += (int)(m_firstValidPosition - firstPosition);
                validLength -= (int)(m_firstValidPosition - firstPosition);
                firstPosition = m_firstValidPosition;
            }

            int seekDistance = (int)(position - firstPosition);
            validLength -= seekDistance;
            pointer += seekDistance;
        }

    }
}