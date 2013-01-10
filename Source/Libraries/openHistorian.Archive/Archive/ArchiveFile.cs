//******************************************************************************************************
//  ArchiveFile.cs - Gbtc
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
//  5/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Data;
using System.IO;
using openHistorian.Collections.KeyValue;
using openHistorian.FileStructure;
using openHistorian.IO.Unmanaged;

namespace openHistorian.Archive
{
    public enum CompressionMethod
    {
        None,
        DeltaEncoded,
        TimeSeriesEncoded,
        TimeSeriesEncoded2
    }

    /// <summary>
    /// Represents a individual self-contained archive file. 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public partial class ArchiveFile : IDisposable
    {
        #region [ Members ]

        // {644FF6C8-ED70-4966-9E5D-E43B51FF4801}
        internal static readonly Guid FileType = new Guid(0x644ff6c8, 0xed70, 0x4966, 0x9e, 0x5d, 0xe4, 0x3b, 0x51, 0xff, 0x48, 0x1);
        // {BABDB82D-7457-46D9-899C-D76036F0312E}
        internal static readonly Guid PointDataFile = new Guid(0xbabdb82d, 0x7457, 0x46d9, 0x89, 0x9c, 0xd7, 0x60, 0x36, 0xf0, 0x31, 0x2e);

        string m_fileName;
        ulong m_firstKey;
        ulong m_lastKey;
        bool m_disposed;

        TransactionalFileStructure m_fileStructure;

        Editor m_activeEditor;

        #endregion

        #region [ Constructors ]

        ArchiveFile()
        {
        }

        /// <summary>
        /// Creates a new in memory archive file.
        /// </summary>
        /// <param name="compression">the compression method that will be used for the file.</param>
        /// <param name="blockSize">The number of bytes per block in the file.</param>
        public static ArchiveFile CreateInMemory(CompressionMethod compression = CompressionMethod.None, int blockSize = 4096)
        {
            var af = new ArchiveFile();
            af.m_fileName = string.Empty;
            af.m_fileStructure = TransactionalFileStructure.CreateInMemory(blockSize);
            af.InitializeNewFile(compression);
            return af;
        }

        /// <summary>
        /// Creates an archive file.
        /// </summary>
        /// <param name="file">the path for the file.</param>
        /// <param name="compression">the compression method that will be used for the file.</param>
        /// <param name="blockSize">The number of bytes per block in the file.</param>
        public static ArchiveFile CreateFile(string file, CompressionMethod compression = CompressionMethod.None, int blockSize = 4096)
        {
            var af = new ArchiveFile();
            af.m_fileName = file;
            af.m_fileStructure = TransactionalFileStructure.CreateFile(file, blockSize);
            af.InitializeNewFile(compression);
            return af;
        }

        /// <summary>
        /// Opens an archive file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="accessMode"></param>
        /// <returns></returns>
        public static ArchiveFile OpenFile(string file, AccessMode accessMode)
        {
            var af = new ArchiveFile();
            af.m_fileName = file;
            af.m_fileStructure = TransactionalFileStructure.OpenFile(file, accessMode);
            if (af.m_fileStructure.ArchiveType != FileType)
                throw new Exception("Archive type is unknown");

            if (!af.LoadUserData(af.m_fileStructure.UserData))
            {
                using (var snapshot = af.AcquireReadSnapshot().CreateReadSnapshot())
                {
                    af.m_firstKey = snapshot.FirstKey;
                    af.m_lastKey = snapshot.LastKey;
                }
            }
            return af;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if the archive file has been disposed. 
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// The first key.  Note: Values only update on commit.
        /// </summary>
        public ulong FirstKey
        {
            get
            {
                return m_firstKey;
            }
        }

        /// <summary>
        /// The last key.  Note: Values only update on commit.
        /// </summary>
        public ulong LastKey
        {
            get
            {
                return m_lastKey;
            }
        }

        /// <summary>
        /// Returns the name of the file.  Returns <see cref="String.Empty"/> if this is a memory archive.
        /// </summary>
        public string FileName
        {
            get
            {
                return m_fileName;
            }
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        public long FileSize
        {
            get
            {
                return m_fileStructure.ArchiveSize;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Sets parameters controlling how this file responds to file resizing requests.
        /// Currently does nothing.
        /// </summary>
        /// <param name="initialFileSize"></param>
        /// <param name="autoGrowthSize"></param>
        /// <param name="requiredFreeSpaceForAutoGrowth"></param>
        public void SetFileSize(long initialFileSize, long autoGrowthSize, long requiredFreeSpaceForAutoGrowth)
        {

        }

        /// <summary>
        /// Called only by the constructor if a new archive file will be created.
        /// </summary>
        void InitializeNewFile(CompressionMethod compression)
        {
            using (var trans = m_fileStructure.BeginEdit())
            {
                using (var fs = trans.CreateFile(PointDataFile, 1))
                using (var bs = new BinaryStream(fs))
                {
                    var blockSize = m_fileStructure.DataBlockSize;
                    while (blockSize > 4096)
                    {
                        blockSize /= 2;
                    }

                    var tree = SortedTree256Initializer.Create(bs, blockSize, compression);
                    m_firstKey = tree.FirstKey;
                    m_lastKey = tree.LastKey;

                }
                trans.ArchiveType = FileType;
                trans.UserData = SaveUserData();
                trans.CommitAndDispose();
            }
        }

        /// <summary>
        /// Acquires a read snapshot of the current archive file.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Once the snapshot has been acquired, any future commits
        /// will not effect this snapshot. The snapshot has a tiny footprint
        /// and allows an unlimited number of reads that can be created.
        /// </remarks>
        public ArchiveFileSnapshotInfo AcquireReadSnapshot()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return new ArchiveFileSnapshotInfo(m_fileStructure);
        }

        /// <summary>
        /// Allows the user to get a read snapshot on the database.
        /// </summary>
        /// <returns></returns>
        public ArchiveFileReadSnapshot BeginRead()
        {
            return AcquireReadSnapshot().CreateReadSnapshot();
        }

        /// <summary>
        /// Begins an edit of the current archive file.
        /// </summary>
        /// <remarks>
        /// Concurrent editing of a file is not supported. Subsequent calls will
        /// throw an exception rather than blocking. This is to encourage
        /// proper synchronization at a higher layer. 
        /// Wrap the return value of this function in a Using block so the dispose
        /// method is always called. 
        /// </remarks>
        /// <example>
        /// using (ArchiveFile.ArchiveFileEditor editor = archiveFile.BeginEdit())
        /// {
        ///     editor.AddPoint(0, 0, 0, 0);
        ///     editor.AddPoint(1, 1, 1, 1);
        ///     editor.Commit();
        /// }
        /// </example>
        public Editor BeginEdit()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_activeEditor != null)
                throw new Exception("Only one concurrent edit is supported");
            m_activeEditor = new Editor(this);
            return m_activeEditor;
        }

        /// <summary>
        /// Closes the archive file. If there is a current transaction, 
        /// that transaction is immediately rolled back and disposed.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                if (m_activeEditor != null)
                    m_activeEditor.Dispose();
                m_fileStructure.Dispose();
                m_disposed = true;
            }
        }

        /// <summary>
        /// Closes and deletes the Archive File. Also calls dispose.
        /// If this is a memory archive, it will release the memory space to the buffer pool.
        /// </summary>
        public void Delete()
        {
            Dispose();
            if (m_fileName != string.Empty)
            {
                File.Delete(m_fileName);
            }
        }

        bool LoadUserData(byte[] userData)
        {
            try
            {
                var ms = new System.IO.MemoryStream(userData);
                var rd = new BinaryReader(ms);
                switch (rd.ReadByte())
                {
                    case 1:
                        m_firstKey = rd.ReadUInt64();
                        m_lastKey = rd.ReadUInt64();
                        break;
                    default:
                        throw new VersionNotFoundException();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        byte[] SaveUserData()
        {
            var ms = new System.IO.MemoryStream();
            var wr = new BinaryWriter(ms);
            wr.Write((byte)1);
            wr.Write(m_firstKey);
            wr.Write(m_lastKey);
            return ms.ToArray();
        }

        #endregion

    }
}
