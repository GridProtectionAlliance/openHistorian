//******************************************************************************************************
//  SubFileStream.cs - Gbtc
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
//  12/10/2011 - Steven E. Chisholm
//       Generated original version of source code.
//  6/1/2012 - Steven E. Chisholm
//       Removed the inheritance from System.IO.Stream since it wasn't used.
//       And prevented concurrent access to this class
//
//******************************************************************************************************

using System;
using openHistorian.IO;
using openHistorian.IO.Unmanaged;

namespace openHistorian.FileStructure
{
    /// <summary>
    ///Provides a file stream that can be used to open a file and does all of the background work 
    ///required to translate virtual position data into physical ones.
    /// </summary>
    public sealed partial class SubFileStream : ISupportsBinaryStream
    {
        #region [ Members ]

        IoSession m_ioStream;

        /// <summary>
        /// Determines if the file stream has been disposed.
        /// </summary>
        bool m_disposed;
        /// <summary>
        /// Determines if the filestream can be written to.
        /// </summary>
        bool m_isReadOnly;
        /// <summary>
        /// This address is used to determine if the block being referenced is an old block or a new one. 
        /// Any addresses greater than or equal to this are new blocks for this transaction. Values before this are old.
        /// </summary>
        int m_lastReadOnlyBlock;
        /// <summary>
        /// The FileAllocationTable
        /// </summary>
        FileHeaderBlock m_fileHeaderBlock;
        /// <summary>
        /// The Disk Subsystem.
        /// </summary>
        DiskIo m_dataReader;
        /// <summary>
        /// The file used by the stream.
        /// </summary>
        SubFileMetaData m_subFile;

        int m_blockSize;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an ArchiveFileStream
        /// </summary>
        /// <param name="dataReader">The location to read from.</param>
        /// <param name="subFile">The file to read.</param>
        /// <param name="fileHeaderBlock">The FileAllocationTable</param>
        /// <param name="accessMode">Determines if the file stream allows writing.</param>
        internal SubFileStream(int blockSize, DiskIo dataReader, SubFileMetaData subFile, FileHeaderBlock fileHeaderBlock, AccessMode accessMode)
        {
            m_blockSize = blockSize;
            m_isReadOnly = (accessMode == AccessMode.ReadOnly);
            m_lastReadOnlyBlock = fileHeaderBlock.LastAllocatedBlock;
            m_fileHeaderBlock = fileHeaderBlock;
            m_dataReader = dataReader;
            m_subFile = subFile;
        }

        #endregion

        #region [ Properties ]

        public bool IsReadOnly
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_isReadOnly;
            }
        }

        /// <summary>
        /// Determines if the file system has been disposed yet.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        #endregion

        #region [ Methods ]

        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (m_ioStream != null)
                    {
                        m_ioStream.Dispose();
                        m_ioStream = null;
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                }
            }
        }

        #endregion

        public int RemainingSupportedIoSessions
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (m_ioStream == null || m_ioStream.IsDisposed)
                    return 1;
                return 0;
            }
        }

        public SubFileMetaData SubFile
        {
            get
            {
                return m_subFile;
            }
        }

        IBinaryStreamIoSession ISupportsBinaryStream.GetNextIoSession()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (RemainingSupportedIoSessions == 0)
                throw new Exception("There are not any remaining IO Sessions");
            m_ioStream = new IoSession(m_blockSize, this);
            return m_ioStream;
        }

        public BinaryStreamBase CreateBinaryStream()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return new BinaryStream(this);
        }
    }
}
