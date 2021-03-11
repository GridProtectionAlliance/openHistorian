//******************************************************************************************************
//  SimplifiedSubFileStream.cs - Gbtc
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
//  10/16/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;
using GSF.IO.Unmanaged;

namespace GSF.IO.FileStructure
{
    /// <summary>
    /// Provides a file stream that can be used to open a file and does all of the background work 
    /// required to translate virtual position data into physical ones.
    /// </summary>
    internal sealed class SimplifiedSubFileStream
        : ISupportsBinaryStream
    {
        #region [ Members ]

        private BinaryStreamIoSessionBase m_ioStream1;

        /// <summary>
        /// Determines if the file stream has been disposed.
        /// </summary>
        private bool m_disposed;

        /// <summary>
        /// The FileAllocationTable
        /// </summary>
        private readonly FileHeaderBlock m_fileHeaderBlock;

        /// <summary>
        /// The size of the block.
        /// </summary>
        private readonly int m_blockSize;

        /// <summary>
        /// The Disk Subsystem.
        /// </summary>
        private readonly FileStream m_stream;

        /// <summary>
        /// The file used by the stream.
        /// </summary>
        private readonly SubFileHeader m_subFile;


        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an SimplifiedSubFileStream
        /// </summary>
        /// <param name="stream">The location to read from.</param>
        /// <param name="subFile">The file to read.</param>
        /// <param name="fileHeaderBlock">The FileAllocationTable</param>
        internal SimplifiedSubFileStream(FileStream stream, SubFileHeader subFile, FileHeaderBlock fileHeaderBlock)
        {
            if (stream is null)
                throw new ArgumentNullException("stream");
            if (subFile is null)
                throw new ArgumentNullException("subFile");
            if (fileHeaderBlock is null)
                throw new ArgumentNullException("fileHeaderBlock");
            if (subFile.DirectBlock == 0)
                throw new Exception("Must assign subFile.DirectBlock");

            if (fileHeaderBlock.IsReadOnly)
                throw new ArgumentException("This parameter cannot be read only when opening for writing", "fileHeaderBlock");
            if (subFile.IsReadOnly)
                throw new ArgumentException("This parameter cannot be read only when opening for writing", "subFile");

            m_blockSize = fileHeaderBlock.BlockSize;
            m_stream = stream;
            m_subFile = subFile;
            m_fileHeaderBlock = fileHeaderBlock;
        }

        #endregion

        #region [ Properties ]

        internal SubFileHeader SubFile => m_subFile;

        /// <summary>
        /// Gets if this file was opened in readonly mode.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return false;
            }
        }

        /// <summary>
        /// Determines if the file system has been disposed yet.
        /// </summary>
        public bool IsDisposed => m_disposed;

        /// <summary>
        /// Gets the number of available simultaneous read/write sessions.
        /// </summary>
        /// <remarks>This value is used to determine if a binary stream can be cloned
        /// to improve read/write/copy performance.</remarks>
        public int RemainingSupportedIoSessions
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                int count = 0;
                if (m_ioStream1 is null || m_ioStream1.IsDisposed)
                {
                    m_ioStream1 = null;
                    count++;
                }
                return count;
            }
        }

        #endregion

        #region [ Methods ]


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (m_ioStream1 != null)
                    {
                        m_ioStream1.Dispose();
                        m_ioStream1 = null;
                    }
                }
                finally
                {
                    m_disposed = true; // Prevent duplicate dispose.
                }
            }
        }


        /// <summary>
        /// Aquire an IO Session.
        /// </summary>
        BinaryStreamIoSessionBase ISupportsBinaryStream.CreateIoSession()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (RemainingSupportedIoSessions == 0)
                throw new Exception("There are not any remaining IO Sessions");

            m_ioStream1 = new SimplifiedSubFileStreamIoSession(m_stream, m_subFile, m_fileHeaderBlock);
            return m_ioStream1;
        }

        #endregion
    }
}