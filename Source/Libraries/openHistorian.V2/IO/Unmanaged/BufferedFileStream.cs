//******************************************************************************************************
//  BufferedFileStream.cs - Gbtc
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
using System.IO;
using openHistorian.V2.Collections;
using openHistorian.V2.IO.Unmanaged;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.Unmanaged
{
    /// <summary>
    /// A buffered file stream utilizes the buffer pool to intellectually cache
    /// the contents of files.  
    /// </summary>
    /// <remarks>
    /// The cache algorithm is a least recently used algorithm.
    /// but will place more emphysis on object that are repeatidly accessed over 
    /// once that are rarely accessed. This is accomplised by incrementing a counter
    /// every time a page is accessed and dividing by 2 every time a collection occurs from the buffer pool.
    /// </remarks>
    unsafe public class BufferedFileStream : ISupportsBinaryStream
    {
        /// <summary>
        /// Gets the size of a sub page. A sub page is the smallest
        /// unit of I/O that can occur. This will then set dirty bits on this level.
        /// </summary>
        const int SubPageSize = 4096;

        /// <summary>
        /// A buffer to use to read/write from a disk.
        /// </summary>
        /// <remarks>Since all disk IO inside .NET must be with a managed type. 
        /// This buffer provides a means to do the disk IO</remarks>
        /// ToDo: Create multiple static blocks so concurrent IO can occur.
        static byte[] s_tmpBuffer = new byte[BufferPool.PageSize];

        ///// <summary>
        ///// Contains meta data about each page that is allocated.  
        ///// Check <see cref="BufferedFileStream.m_isPageMetaDataNotNull"/> to see if the page is used.
        ///// </summary>
        //struct PageMetaData
        //{
        //    public byte* Location;
        //    public int Index;
        //    public int ReferencedCount;
        //    public ushort IsDirtyFlags;
        //}

        ///// <summary>
        ///// contains a list of all the meta pages.
        ///// </summary>
        ///// <remarks>These items in the list are not in any particular order.
        ///// To lookup the correct pages, use <see cref="m_allocatedPagesLookupList"/> </remarks>
        //List<PageMetaData> m_listOfPageMetaData;

        ///// <summary>
        ///// A bit array to quickly find which <see cref="PageMetaData"/> entries are being used in
        ///// <see cref="m_listOfPageMetaData"/>
        ///// </summary>
        //BitArray m_isPageMetaDataNotNull;

        ///// <summary>
        ///// Contains the currently active IO sessions that cannot be cleaned up by a collection.
        ///// </summary>
        //List<int> m_activeBlockIndexes;

        ///// <summary>
        ///// Contains all of the pages that are cached for the file stream.
        ///// </summary>
        //SortedList<int, int> m_allocatedPagesLookupList;

        /// <summary>
        /// An event raised when this class has been disposed.
        /// </summary>
        /// <remarks>It is important for anything that utilizes IO Sessions discontinue use of them
        /// after this event since that can cause data corruption.</remarks>
        public event EventHandler StreamDisposed;

        /// <summary>
        /// The file stream use by this class.
        /// </summary>
        FileStream m_baseStream;

        LeastRecentlyUsedPageReplacement m_pageReplacementAlgorithm;

        public BufferedFileStream(FileStream stream)
        {
            m_pageReplacementAlgorithm = new LeastRecentlyUsedPageReplacement();
            //m_listOfPageMetaData = new List<PageMetaData>();
            //m_isPageMetaDataNotNull = new BitArray(80000, false);
            //m_activeBlockIndexes = new List<int>();
            //m_allocatedPagesLookupList = new SortedList<int, int>();
            m_baseStream = stream;

            BufferPool.RequestCollection += new Action<BufferPoolCollectionMode>(BufferPool_RequestCollection);
        }

        //public void Read(long position, byte[] data, int start, int length)
        //{
        //    int address = (int)(position >> BufferPool.PageShiftBits);
        //    int offset = (int)(position & BufferPool.PageMask);
        //    long streamPosition = position & ~BufferPool.PageMask;
        //    int availableLength = BufferPool.PageSize - offset;

        //    Block cachePage;
        //    int cachePageIndex;

        //    //if the page is not in the entry, read it from the underlying stream.
        //    if (!m_list.TryGetValue(address, out cachePageIndex))
        //    {
        //        cachePage
        //        IntPtr ptr;
        //        cachePage.Index = BufferPool.AllocatePage(out ptr);
        //        cachePage.Location = (byte*)ptr;

        //        BaseStream.Position = streamPosition;
        //        lock (s_tmpBuffer)
        //        {
        //            BaseStream.Read(s_tmpBuffer, 0, s_tmpBuffer.Length);
        //            Marshal.Copy(s_tmpBuffer, 0, ptr, s_tmpBuffer.Length);
        //        }
        //        m_list.Add(address, cachePage);
        //    }

        //    if (availableLength < length)
        //    {
        //        Marshal.Copy((IntPtr)cachePage.Location + offset, data, start, availableLength);
        //        Read(position + availableLength, data, start + availableLength, length - availableLength);
        //    }
        //    else
        //    {
        //        Marshal.Copy((IntPtr)cachePage.Location + offset, data, start, length);
        //    }
        //}

        //public void Write(long position, byte[] data, int start, int length)
        //{
        //    int address = (int)(position >> BufferPool.PageShiftBits);
        //    int offset = (int)(position & BufferPool.PageMask);
        //    long streamPosition = position & ~BufferPool.PageMask;
        //    int availableLength = BufferPool.PageSize - offset;

        //    Block cachePage;

        //    //if the page is not in the entry, read it from the underlying stream.
        //    if (!m_list.TryGetValue(address, out cachePage))
        //    {
        //        IntPtr ptr;
        //        cachePage.Index = BufferPool.AllocatePage(out ptr);
        //        cachePage.Location = (byte*)ptr;

        //        BaseStream.Position = streamPosition;
        //        lock (s_tmpBuffer)
        //        {
        //            BaseStream.Read(s_tmpBuffer, 0, s_tmpBuffer.Length);
        //            Marshal.Copy(s_tmpBuffer, 0, ptr, s_tmpBuffer.Length);
        //            //Populate with junk
        //            for (int x = 0; x < BufferPool.PageSize; x += 8)
        //            {
        //                *(long*)ptr = (long)ptr * x;
        //            }
        //        }
        //        m_list.Add(address, cachePage);
        //    }

        //    if (availableLength < length)
        //    {
        //        Marshal.Copy(data, start, (IntPtr)cachePage.Location + offset, availableLength);
        //        Write(position + availableLength, data, start + availableLength, length - availableLength);
        //    }
        //    else
        //    {
        //        Marshal.Copy(data, start, (IntPtr)cachePage.Location + offset, length);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public void Flush(bool waitForWriteToDisk = false)
        {
            foreach (var block in m_pageReplacementAlgorithm.GetDirtyPages())
            {
                m_baseStream.Position = block.StreamPosition;
                Marshal.Copy((IntPtr)block.LocationOfPage, s_tmpBuffer, 0, s_tmpBuffer.Length);
                m_baseStream.Write(s_tmpBuffer, 0, s_tmpBuffer.Length);

                m_pageReplacementAlgorithm.ClearDirtyBits(block.StreamPosition);
            }
            m_baseStream.Flush(waitForWriteToDisk);
            //ToDo: Call the actual WinAPI function which is the WaitForFlush.
        }

        int ISupportsBinaryStream.RemainingSupportedIoSessions
        {
            get
            {
                return int.MaxValue;
            }
        }

        void ISupportsBinaryStream.GetBlock(int ioSession, long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
        {
            int pageIndex = (int)(position >> BufferPool.PageShiftBits);
            int blockIndexWithinPageAddress = (int)(position & BufferPool.PageMask) / 4096;
            long startingFileStreamPositionForPage = (position & ~BufferPool.PageMask);

            bool isEmptyPage;
            var pageMetaData = m_pageReplacementAlgorithm.TryGetPageOrCreateNew(position, ioSession, isWriting, out isEmptyPage);
            if (isEmptyPage)
            {
                IntPtr ptr = (IntPtr)pageMetaData.LocationOfPage;
                m_baseStream.Position = pageMetaData.StreamPosition;
                lock (s_tmpBuffer)
                {
                    m_baseStream.Read(s_tmpBuffer, 0, s_tmpBuffer.Length);
                    Marshal.Copy(s_tmpBuffer, 0, ptr, s_tmpBuffer.Length);
                }
            }
            firstPointer = (IntPtr)pageMetaData.StreamPosition + blockIndexWithinPageAddress * 4096;
            length = 4096;
            firstPosition = startingFileStreamPositionForPage + blockIndexWithinPageAddress * 4096;
            supportsWriting = (((pageMetaData.IsDirtyFlags >> blockIndexWithinPageAddress) & 1) == 1);
        }

        void ISupportsBinaryStream.ReleaseIoSession(int ioSession)
        {
            m_pageReplacementAlgorithm.ReleaseIoSession(ioSession);
        }

        int ISupportsBinaryStream.GetNextIoSession()
        {
            return m_pageReplacementAlgorithm.GetFreeIoSession();
        }

        void IDisposable.Dispose()
        {
            if (StreamDisposed != null)
                StreamDisposed.Invoke(this, EventArgs.Empty);
        }

        void BufferPool_RequestCollection(BufferPoolCollectionMode obj)
        {
            if (obj == BufferPoolCollectionMode.Critical)
            {
                Flush();
            }
            m_pageReplacementAlgorithm.DoCollection();
        }
    }
}
