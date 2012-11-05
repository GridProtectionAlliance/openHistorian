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
using openHistorian.V2.Collections.KeyValue;
using openHistorian.V2.FileStructure;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.Server.Database.Archive
{
    /// <summary>
    /// Represents a individual self-contained archive file. 
    /// This is one of many files that are part of a given <see cref="ArchiveDatabaseEngine"/>.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public partial class ArchiveFile : IDisposable
    {
        #region [ Members ]

        // {644FF6C8-ED70-4966-9E5D-E43B51FF4801}
        public static readonly Guid s_fileType = new Guid(0x644ff6c8, 0xed70, 0x4966, 0x9e, 0x5d, 0xe4, 0x3b, 0x51, 0xff, 0x48, 0x1);
        // {BABDB82D-7457-46D9-899C-D76036F0312E}
        public static readonly Guid s_pointDataFile = new Guid(0xbabdb82d, 0x7457, 0x46d9, 0x89, 0x9c, 0xd7, 0x60, 0x36, 0xf0, 0x31, 0x2e);

        string m_fileName;
        ulong m_firstKey;
        ulong m_lastKey;
        bool m_disposed;

        TransactionalFileStructure m_fileStructure;

        ArchiveFileEditor m_activeEditor;

        #endregion

        #region [ Constructors ]

        ArchiveFile()
        {
        }


        /// <summary>
        /// Creates a new in memory archive file.
        /// </summary>
        public static ArchiveFile CreateInMemory(int blockSize = 4096)
        {
            var af = new ArchiveFile();
            af.m_fileName = string.Empty;
            af.m_fileStructure = TransactionalFileStructure.CreateInMemory(blockSize);
            af.InitializeNewFile();
            return af;
        }

        /// <summary>
        /// Creates an archive file.
        /// </summary>
        /// <param name="file">the path for the file</param>
        public static ArchiveFile CreateFile(string file, int blockSize = 4096)
        {
            var af = new ArchiveFile();
            af.m_fileName = file;
            af.m_fileStructure = TransactionalFileStructure.CreateFile(file);
            af.InitializeNewFile();
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
            if (af.m_fileStructure.ArchiveType != s_fileType)
                throw new Exception("Archive type is unknown");
            
            if (!af.LoadUserData(af.m_fileStructure.UserData))
            {
                using (var snapshot = af.CreateSnapshot().OpenInstance())
                {
                    af.m_firstKey = snapshot.FirstKey;
                    af.m_lastKey = snapshot.LastKey;
                }
            }
            return af;
        }

        #endregion

        #region [ Properties ]

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

        public void SetFileSize(long initialFileSize, long autoGrowthSize, long requiredFreeSpaceForAutoGrowth)
        {

        }

        /// <summary>
        /// Called only by the constructor if a new archive file will be created.
        /// </summary>
        void InitializeNewFile()
        {
            using (var trans = m_fileStructure.BeginEdit())
            {
                using (var fs = trans.CreateFile(s_pointDataFile, 1))
                using (var bs = new BinaryStream(fs))
                {
                    var tree = new SortedTree256(bs, m_fileStructure.DataBlockSize);
                    m_firstKey = tree.FirstKey;
                    m_lastKey = tree.LastKey;
                }
                trans.ArchiveType = s_fileType;
                trans.UserData = SaveUserData();
                trans.CommitAndDispose();
            }
        }

        /// <summary>
        /// Aquires a snapshot of the current file system.
        /// </summary>
        /// <returns></returns>
        public ArchiveFileSnapshot CreateSnapshot()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return new ArchiveFileSnapshot(m_fileStructure);
        }

        /// <summary>
        /// Begins an edit of the current archive file.
        /// </summary>
        public ArchiveFileEditor BeginEdit()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_activeEditor != null)
                throw new Exception("Only one concurrent edit is supported");
            m_activeEditor = new ArchiveFileEditor(this);
            return m_activeEditor;
        }

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
        /// Closes and deletes the partition. Also calls dispose.
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
                var ms = new System.IO.MemoryStream();
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
