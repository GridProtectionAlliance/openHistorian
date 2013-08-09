////******************************************************************************************************
////  DiskIOUnbuffered.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  12/30/2011 - Steven E. Chisholm
////       Generated original version of source code.
////
////******************************************************************************************************

//using System;
//using System.IO;

//namespace openHistorian.Core.StorageSystem.File
//{
//    /// <summary>
//    /// Provides the interface layer that talkes to 
//    /// both the memory buffer and the archive file.
//    /// </summary>
//    internal class DiskIoUnbuffered : DiskIoBase
//    {

//        #region [ Members ]

//        /// <summary>
//        /// This file option is not provided under the .NET library, but still work.  
//        /// This prevents the OS from caching the data read from the disk.  
//        /// The cache is to be managed at a higher level.
//        /// if configured, all disk IO has to occur at the hardware sector level, 
//        /// which used to be 512 bytes, but is now 4k
//        /// </summary>
//        const FileOptions FileFlagNoBuffering = (FileOptions)0; // I enabled buffering by commenting out this value: 0x20000000;

//        /// <summary>
//        /// prevents duplicate calls to Dispose.
//        /// </summary>
//        bool m_disposed;

//        /// <summary>
//        /// The unbuffered file stream
//        /// </summary>
//        protected FileStream File;

//        /// <summary>
//        /// The name of the file that was opened.
//        /// </summary>
//        protected string FileName;

//        /// <summary>
//        /// Determines if the file is opened in readonly mode.
//        /// </summary>
//        bool m_isReadOnly;

//        /// <summary>
//        /// The size of the file.  A negative value means that this class does not know the size of the file.
//        /// </summary>
//        long m_fileSize;

//        #endregion

//        #region [ Constructors ]

//        /// <summary>
//        /// Opens an existing archive file.
//        /// </summary>
//        /// <param name="fileName"></param>
//        /// <param name="isReadOnly">Determines if the file will be opened in read only mode.</param>
//        protected DiskIoUnbuffered(string fileName, bool isReadOnly)
//        {
//            FileName = fileName;
//            m_isReadOnly = isReadOnly;
//            if (isReadOnly)
//            {
//                File = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 8, FileOptions.RandomAccess | FileFlagNoBuffering);
//            }
//            else
//            {
//                File = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 8, FileOptions.RandomAccess | FileFlagNoBuffering | FileOptions.WriteThrough);
//            }
//            m_fileSize = -1;
//        }

//        /// <summary>
//        /// Creates a new archive file
//        /// </summary>
//        /// <param name="fileName">the file name</param>
//        protected DiskIoUnbuffered(string fileName)
//        {
//            m_isReadOnly = false;
//            File = new FileStream(fileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read, 8, FileOptions.RandomAccess | FileFlagNoBuffering | FileOptions.WriteThrough);
//            m_fileSize = -1;
//        }

//        /// <summary>
//        /// Releases the unmanaged resources before the <see cref="DiskIoUnbuffered"/> object is reclaimed by <see cref="GC"/>.
//        /// </summary>
//        ~DiskIoUnbuffered()
//        {
//            Dispose(false);
//        }

//        #endregion

//        #region [ Properties ]

//        /// <summary>
//        /// Gets if the file has been opened in readonly mode
//        /// </summary>
//        public override bool IsReadOnly
//        {
//            get
//            {
//                return m_isReadOnly;
//            }
//        }

//        /// <summary>
//        /// Return the size of the file archive in bytes
//        /// </summary>
//        /// <remarks>
//        /// The file size is cached because the filestream's length property calls WIN_API every time it is accessed.
//        /// </remarks>
//        public override long FileSize
//        {
//            get
//            {
//                if (m_fileSize < 0)
//                {
//                    m_fileSize = File.Length;
//                }
//                return m_fileSize;
//            }
//        }

//        /// <summary>
//        /// Get/Set the file pointer of the file stream
//        /// </summary>
//        /// <remarks>
//        /// This property exists because setting the position calls WIN_API.  
//        /// If the position doesn't change, there is no reason to set it.
//        /// </remarks>
//        long Position
//        {
//            get
//            {
//                return File.Position;
//            }
//            set
//            {
//                if (File.Position != value)
//                {
//                    File.Position = value;
//                }
//            }
//        }

//        #endregion

//        #region [ Methods ]

//        /// <summary>
//        /// Resizes the file to the requested size
//        /// </summary>
//        /// <param name="requestedSize">The size to resize to</param>
//        /// <returns>The actual size of the file after the resize</returns>
//        protected override long SetFileLength(long requestedSize)
//        {
//            m_fileSize = -1;
//            File.SetLength(requestedSize);
//            return FileSize;
//        }
//        /// <summary>
//        /// Writes the following data to the stream
//        /// </summary>
//        /// <param name="blockIndex">the block where to write the data</param>
//        /// <param name="data">the data to write</param>
//        protected override void WriteBlock(uint blockIndex, byte[] data)
//        {
//            Position = blockIndex * ArchiveConstants.BlockSize;
//            File.Write(data, 0, data.Length);
//        }
//        /// <summary>
//        /// Tries to read data from the following file
//        /// </summary>
//        /// <param name="blockIndex">the block where to write the data</param>
//        /// <param name="data">the data to write</param>
//        /// <returns>A status whether the read was sucessful. See <see cref="IoReadState"/>.</returns>
//        protected override IoReadState ReadBlock(uint blockIndex, byte[] data)
//        {
//            Position = blockIndex * ArchiveConstants.BlockSize;
//            File.Read(data, 0, data.Length);
//            return IoReadState.Valid;
//        }

//        /// <summary>
//        /// Releases all the resources used by the <see cref="DiskIoUnbuffered"/> object.
//        /// </summary>
//        public override void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        /// <summary>
//        /// Releases the unmanaged resources used by the <see cref="DiskIoUnbuffered"/> object and optionally releases the managed resources.
//        /// </summary>
//        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
//        protected void Dispose(bool disposing)
//        {
//            if (!m_disposed)
//            {
//                try
//                {
//                    // This will be done regardless of whether the object is finalized or disposed.

//                    if (disposing)
//                    {
//                        // This will be done only when the object is disposed by calling Dispose().
//                        if (File != null)
//                            File.Dispose();
//                        File = null;
//                    }
//                }
//                finally
//                {
//                    m_disposed = true;  // Prevent duplicate dispose.
//                }
//            }
//        }

//        #endregion

//        #region [ Static ]

//        /// <summary>
//        /// Opens an existing file archive for unbuffered read/write operations.
//        /// </summary>
//        /// <param name="fileName">The path to the archive file.</param>
//        /// <param name="isReadOnly">Determines if the file is going to be opened in readonly mode.</param>
//        /// <remarks>
//        /// Since buffering the data will be the responsibility of another layer, 
//        /// allowing the OS to buffer the data decreases performance (slightly) and
//        /// wastes system memory that can be better used by the historian's buffer.
//        /// </remarks>
//        public static DiskIoUnbuffered OpenFile(string fileName, bool isReadOnly)
//        {
//            return new DiskIoUnbuffered(fileName, isReadOnly);
//        }

//        /// <summary>
//        /// Creates a new archive file at the given path
//        /// </summary>
//        /// <param name="fileName">The file name of the archive to write.  This file must not already exist.</param>
//        /// <returns></returns>
//        public static DiskIoUnbuffered CreateFile(string fileName)
//        {
//            return new DiskIoUnbuffered(fileName);
//        }

//        #endregion


//    }
//}

