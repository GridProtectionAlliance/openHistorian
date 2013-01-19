//******************************************************************************************************
//  DiskIo.cs - Gbtc
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
//  3/24/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Data;
using GSF.IO.Unmanaged;

namespace openHistorian.FileStructure
{
    internal sealed class DiskIo : IDisposable
    {
        #region [ Members ]

        bool m_disposed;
        /// <summary>
        /// The boundry which marks the last page that is read only.
        /// </summary>
        int m_lastReadOnlyBlock;

        ISupportsBinaryStreamAdvanced m_stream;

        int m_blockSize;

        #endregion

        #region [ Constructors ]

        public DiskIo(int blockSize, ISupportsBinaryStreamAdvanced stream, int lastReadOnlyBlock)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (stream.BlockSize <= 0 || stream.BlockSize % blockSize != 0)
                throw new ArgumentException("The block size of the stream must be a multiple of " + blockSize.ToString() + ".", "stream");

            m_blockSize = blockSize;
            m_stream = stream;
            m_lastReadOnlyBlock = lastReadOnlyBlock;

            m_stream.BlockAboutToBeWrittenToDisk += m_stream_BlockAboutToBeWrittenToDisk;
            m_stream.BlockLoadedFromDisk += m_stream_BlockLoadedFromDisk;
        }



        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets if the disk supports writing.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                CheckIsDisposed();
                return m_stream.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets if the class has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// Gets the current size of the file.
        /// </summary>
        public long FileSize
        {
            get
            {
                CheckIsDisposed();
                return m_stream.Length;
            }
        }

        /// <summary>
        /// Returns the last block that is readonly.
        /// </summary>
        public int LastReadonlyBlock
        {
            get
            {
                return m_lastReadOnlyBlock;
            }
        }

        #endregion

        #region [ Methods ]

        public void Flush()
        {
            CheckIsDisposed();
            if (m_stream.IsReadOnly)
                throw new ReadOnlyException();
            m_stream.Flush();
        }

        //ToDo: Activate these functions.
        ///// <summary>
        ///// Flushes all edits to the underlying stream.
        ///// </summary>
        //void Flush(int lastReadOnlyBlock)
        //{
        //    CheckIsDisposed();
        //    if (m_stream.IsReadOnly)
        //        throw new ReadOnlyException();
        //    m_stream.Flush();
        //    m_lastReadOnlyBlock = lastReadOnlyBlock;
        //}

        ///// <summary>
        ///// Will invalidate all of the changes that have been made so during the next flush cycle, 
        ///// the data will not have to be written to the disk
        ///// </summary>
        ///// <param name="lastValidBlock">the block to roll back to, cannot be less than the last committed block.</param>
        //void RollbackAllWrites(int lastValidBlock)
        //{
        //    if (lastValidBlock < m_lastReadOnlyBlock)
        //        throw new ArgumentOutOfRangeException("lastValidBlock", "Cannot roll back beyond the committed writes");
        //    m_stream.TrimEditsAfterPosition((lastValidBlock + 1L) * m_blockSize);
        //}

        /// <summary>
        /// Creates a <see cref="DiskIoSession"/> that can be used to perform basic read/write functions.
        /// </summary>
        /// <returns></returns>
        public DiskIoSession CreateDiskIoSession()
        {
            CheckIsDisposed();
            return new DiskIoSession(m_blockSize, this, m_stream);
        }

        /// <summary>
        /// This will resize the file to the provided size in bytes;
        /// If resizing smaller than the allocated space, this number is 
        /// increased to the allocated space.  
        /// If file size is not a multiple of the page size, it is rounded up to
        /// the nearest page boundry
        /// </summary>
        /// <param name="size">The number of bytes to make the file.</param>
        /// <param name="nextUnallocatedBlock">the next free block.  
        /// This value is used to ensure that the archive file is not 
        /// reduced beyond this limit causing data coruption</param>
        /// <returns>The size that the file is after this call</returns>
        /// <remarks>Passing 0 to this function will effectively trim out 
        /// all of the free space in this file.</remarks>
        public long SetFileLength(long size, int nextUnallocatedBlock)
        {
            CheckIsDisposed();
            if (nextUnallocatedBlock * (long)m_blockSize > size)
            {
                //if shrinking beyond the allocated space, 
                //adjust the size exactly to the allocated space.
                size = nextUnallocatedBlock * (long)m_blockSize;
            }
            else
            {
                long remainder = (size % m_blockSize);
                //if there will be a fragmented page remaining
                if (remainder != 0)
                {
                    //if the requested size is not a multiple of the page size
                    //round up to the nearest page
                    size = size + m_blockSize - remainder;
                }
            }
            return m_stream.SetLength(size + size / 16);//Always increase the file size by at least 6.25%
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DiskIo"/> object and optionally releases the managed resources.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (m_stream != null)
                    {
                        m_stream.Dispose();
                        m_stream = null;
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }


        /// <summary>
        /// Checks 2 flags and throws the correct exceptions if this class is disposed.
        /// </summary>
        void CheckIsDisposed()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_stream.IsDisposed)
                throw new ObjectDisposedException(m_stream.GetType().FullName);
        }


        #endregion

        #region [ Event Processing ]

        unsafe void m_stream_BlockLoadedFromDisk(object sender, StreamBlockEventArgs e)
        {
            if ((e.Position & (m_blockSize - 1)) != 0)
                throw new Exception("Position not alligned on block boundary");
            if ((e.Length & (m_blockSize - 1)) != 0)
                throw new Exception("Length is not a multiple of the block size");

            for (int offset = 0; offset < e.Length; offset += m_blockSize)
            {
                long checksum1;
                int checksum2;
                byte* data = (byte*)e.Data + offset;
                ComputeChecksum(data, out checksum1, out checksum2);
                long checksumInData1 = *(long*)(data + m_blockSize - 16);
                int checksumInData2 = *(int*)(data + m_blockSize - 8);
                if (checksum1 == checksumInData1 && checksum2 == checksumInData2)
                {
                    //Record checksum is valid and put zeroes in all other fields.
                    *(int*)(data + m_blockSize - 4) = 1;
                }
                else
                {
                    //Record checksum is not valid and put zeroes in all other fields.
                    *(int*)(data + m_blockSize - 4) = 2;
                }
            }
        }

        unsafe void m_stream_BlockAboutToBeWrittenToDisk(object sender, StreamBlockEventArgs e)
        {
            if ((e.Position & (m_blockSize - 1)) != 0)
                throw new Exception("Position not alligned on block boundary");
            if ((e.Length & (m_blockSize - 1)) != 0)
                throw new Exception("Length is not a multiple of the block size");

            for (int offset = 0; offset < e.Length; offset += m_blockSize)
            {
                byte* data = (byte*)e.Data + offset;
                
                //Determine if the checksum needs to be recomputed.
                if (data[m_blockSize - 3] != 0)
                {
                    long checksum1;
                    int checksum2;
                    ComputeChecksum(data, out checksum1, out checksum2);
                    *(long*)(data + m_blockSize - 16) = checksum1;
                    *(int*)(data + m_blockSize - 8) = checksum2;
                }
                //reset value to null;
                *(int*)(data + m_blockSize - 4) = 0;
            }
        }

        /// <summary>
        /// Computes the custom checksum of the data.
        /// </summary>
        /// <param name="data">the data to compute the checksum for.</param>
        /// <param name="checksum1">the 64 bit component of this checksum</param>
        /// <param name="checksum2">the 32 bit component of this checksum</param>
        unsafe void ComputeChecksum(byte* data, out long checksum1, out int checksum2)
        {
            //checksum1 = 0;
            //checksum2 = 0;
            //return;
            ChecksumCount += 1;
            ulong* ptr = (ulong*)data;

            ulong a = 1;
            ulong b = 0;

            int iterationCount = m_blockSize / 8 - 2;

            for (int x = 0; x < iterationCount; x++)
            {
                a += ptr[x];
                b += a;
            }
            checksum1 = (long)b;
            checksum2 = (int)a ^ (int)(a >> 32);
        }

        #endregion

        /// <summary>
        /// Checks how many times the checksum was computed.  This is used to see IO amplification.
        /// It is currently a debug term that will soon disappear.
        /// </summary>
        static internal long ChecksumCount;

    }
}
