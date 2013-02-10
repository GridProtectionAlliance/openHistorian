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
using openHistorian.FileStructure.IO;

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

        DiskMediumBase m_stream;

        int m_blockSize;

        #endregion

        #region [ Constructors ]

        public DiskIo(int blockSize, DiskMediumBase stream, int lastReadOnlyBlock)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
  
            m_blockSize = blockSize;
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

        public FileHeaderBlock Header
        {
            get
            {
                return m_stream.Header;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Occurs when rolling back a transaction. This will free up
        /// any temporary space allocated for the change. 
        /// </summary>
        public void RollbackChanges()
        {
            CheckIsDisposed();
            if (m_stream.IsReadOnly)
                throw new ReadOnlyException();
            m_stream.RollbackChanges();
        }

        /// <summary>
        /// Occurs when committing the following data to the disk.
        /// This will copy any pending data to the disk in a manner that
        /// will protect against corruption.
        /// </summary>
        /// <param name="header"></param>
        public void CommitChanges(FileHeaderBlock header)
        {
            CheckIsDisposed();
            if (m_stream.IsReadOnly)
                throw new ReadOnlyException();
            m_stream.CommitChanges(header);
        }

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
