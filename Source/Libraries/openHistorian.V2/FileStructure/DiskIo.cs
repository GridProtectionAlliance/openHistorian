//******************************************************************************************************
//  DiskIo.cs - Gbtc
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
//  3/24/2012 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Data;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.FileStructure
{
    internal sealed class DiskIo : IDisposable
    {
        #region [ Members ]

        bool m_disposed;
        /// <summary>
        /// The boundry which marks the last page that is read only.
        /// </summary>
        int m_lastReadOnlyBlock;
        ISupportsBinaryStreamSizing m_stream;

        #endregion

        #region [ Constructors ]

        public DiskIo(ISupportsBinaryStreamSizing stream, int lastReadOnlyBlock)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (stream.BlockSize <= 0 || stream.BlockSize % FileStructureConstants.BlockSize != 0)
                throw new ArgumentException("The block size of the stream must be a multiple of " + FileStructureConstants.BlockSize.ToString() + ".", "stream");
            m_stream = stream;
            m_lastReadOnlyBlock = lastReadOnlyBlock;
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

        /// <summary>
        /// Flushes all edits to the underlying stream.
        /// </summary>
        void Flush(int lastReadOnlyBlock)
        {
            CheckIsDisposed();
            if (m_stream.IsReadOnly)
                throw new ReadOnlyException();
            m_stream.Flush();
            m_lastReadOnlyBlock = lastReadOnlyBlock;
        }

        /// <summary>
        /// Will invalidate all of the changes that have been made so during the next flush cycle, 
        /// the data will not have to be written to the disk
        /// </summary>
        /// <param name="lastValidBlock">the block to roll back to, cannot be less than the last committed block.</param>
        void RollbackAllWrites(int lastValidBlock)
        {
            if (lastValidBlock < m_lastReadOnlyBlock)
                throw new ArgumentOutOfRangeException("lastValidBlock", "Cannot roll back beyond the committed writes");
            m_stream.TrimEditsAfterPosition((lastValidBlock + 1L) * FileStructureConstants.BlockSize);
        }

        /// <summary>
        /// Creates a <see cref="DiskIoSession"/> that can be used to perform basic read/write functions.
        /// </summary>
        /// <returns></returns>
        public DiskIoSession CreateDiskIoSession()
        {
            CheckIsDisposed();
            return new DiskIoSession(this, m_stream);
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
            if (nextUnallocatedBlock * FileStructureConstants.BlockSize > size)
            {
                //if shrinking beyond the allocated space, 
                //adjust the size exactly to the allocated space.
                size = nextUnallocatedBlock * FileStructureConstants.BlockSize;
            }
            else
            {
                long remainder = (size % FileStructureConstants.BlockSize);
                //if there will be a fragmented page remaining
                if (remainder != 0)
                {
                    //if the requested size is not a multiple of the page size
                    //round up to the nearest page
                    size = size + FileStructureConstants.BlockSize - remainder;
                }
            }
            return m_stream.SetLength(size);
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

    }
}
