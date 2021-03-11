//******************************************************************************************************
//  SortedTreeFile.cs - Gbtc
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
//  05/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using GSF.IO.Unmanaged;
using GSF.Snap.Tree;
using GSF.IO.FileStructure;

namespace GSF.Snap.Storage
{
    /// <summary>
    /// Represents a individual self-contained archive file. 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class SortedTreeFile
        : IDisposable
    {
        // {63AB3FEA-14CD-4ECA-939B-0DD23742E170}
        /// <summary>
        /// The main type of the archive file.
        /// </summary>
        internal static readonly Guid FileType = new Guid(0x63ab3fea, 0x14cd, 0x4eca, 0x93, 0x9b, 0x0d, 0xd2, 0x37, 0x42, 0xe1, 0x70);

        // {E0FCA590-F46E-4060-8764-DFDCFC74D728}
        /// <summary>
        /// The guid where the primary archive component exists
        /// </summary>
        internal static readonly Guid PrimaryArchiveType = new Guid(0xe0fca590, 0xf46e, 0x4060, 0x87, 0x64, 0xdf, 0xdc, 0xfc, 0x74, 0xd7, 0x28);

        #region [ Members ]

        private string m_filePath;
        private bool m_disposed;

        private TransactionalFileStructure m_fileStructure;
        private readonly SortedList<SubFileName, IDisposable> m_openedFiles;

        #endregion

        #region [ Constructors ]

        private SortedTreeFile()
        {
            m_openedFiles = new SortedList<SubFileName, IDisposable>();
        }

        /// <summary>
        /// Creates a new in memory archive file.
        /// </summary>
        /// <param name="blockSize">The number of bytes per block in the file.</param>
        /// <param name="flags">Flags to write to the file</param>
        public static SortedTreeFile CreateInMemory(int blockSize = 4096, params Guid[] flags)
        {
            SortedTreeFile file = new SortedTreeFile();
            file.m_filePath = string.Empty;
            file.m_fileStructure = TransactionalFileStructure.CreateInMemory(blockSize, flags);
            return file;
        }

        /// <summary>
        /// Creates an archive file.
        /// </summary>
        /// <param name="file">the path for the file.</param>
        /// <param name="blockSize">The number of bytes per block in the file.</param>
        /// <param name="flags">Flags to write to the file</param>
        public static SortedTreeFile CreateFile(string file, int blockSize = 4096, params Guid[] flags)
        {
            SortedTreeFile af = new SortedTreeFile();
            file = Path.GetFullPath(file);
            af.m_filePath = file;
            af.m_fileStructure = TransactionalFileStructure.CreateFile(file, blockSize, flags);
            return af;
        }

        /// <summary>
        /// Opens an archive file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="isReadOnly"></param>
        /// <returns></returns>
        public static SortedTreeFile OpenFile(string file, bool isReadOnly)
        {
            SortedTreeFile af = new SortedTreeFile();
            file = Path.GetFullPath(file);
            af.m_filePath = file;
            af.m_fileStructure = TransactionalFileStructure.OpenFile(file, isReadOnly);
            if (af.m_fileStructure.Snapshot.Header.ArchiveType != FileType)
                throw new Exception("Archive type is unknown");
            return af;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if the archive file has been disposed. 
        /// </summary>
        public bool IsDisposed => m_disposed;

        /// <summary>
        /// Returns the name of the file.  Returns <see cref="String.Empty"/> if this is a memory archive.
        /// This is the name of the entire path.
        /// </summary>
        public string FilePath => m_filePath;

        /// <summary>
        /// Gets if the file is a memory file.
        /// </summary>
        public bool IsMemoryFile => m_filePath == string.Empty;

        /// <summary>
        /// Gets the name of the file, but only the file, not the entire path.
        /// </summary>
        public string FileName
        {
            get
            {
                if (m_filePath == string.Empty)
                    return string.Empty;
                return Path.GetFileName(m_filePath);
            }
        }

        /// <summary>
        /// Gets the last committed read snapshot on the file system.
        /// </summary>
        /// <returns></returns>
        public ReadSnapshot Snapshot => m_fileStructure.Snapshot;

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        public long ArchiveSize => m_fileStructure.ArchiveSize;

    #endregion

        #region [ Methods ]


        /// <summary>
        /// Changes the extension of the current file.
        /// </summary>
        /// <param name="extension">the new extension</param>
        /// <param name="isReadOnly">If the file should be reopened as readonly</param>
        /// <param name="isSharingEnabled">If the file should share read privileges.</param>
        public void ChangeExtension(string extension, bool isReadOnly, bool isSharingEnabled)
        {
            m_fileStructure.ChangeExtension(extension, isReadOnly, isSharingEnabled);
            m_filePath = m_fileStructure.FileName;
        }

        /// <summary>
        /// Reopens the file with different permissions.
        /// </summary>
        /// <param name="isReadOnly">If the file should be reopened as readonly</param>
        /// <param name="isSharingEnabled">If the file should share read privileges.</param>
        public void ChangeShareMode(bool isReadOnly, bool isSharingEnabled)
        {
            m_fileStructure.ChangeShareMode(isReadOnly, isSharingEnabled);
        }

        /// <summary>
        /// Opens the default table for this TKey and TValue. 
        /// </summary>
        /// <typeparam name="TKey">The key</typeparam>
        /// <typeparam name="TValue">The value</typeparam>
        /// <remarks>
        /// Every Key and Value have their uniquely mapped file, therefore a different file is opened if TKey and TValue are different.
        /// </remarks>
        /// <returns>null if table does not exist</returns>
        public SortedTreeTable<TKey, TValue> OpenTable<TKey, TValue>()
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            return OpenTable<TKey, TValue>(GetFileName<TKey, TValue>());
        }

        /// <summary>
        /// Opens the default table for this TKey and TValue. 
        /// </summary>
        /// <typeparam name="TKey">The key</typeparam>
        /// <typeparam name="TValue">The value</typeparam>
        /// <param name="tableName">the name of an internal table</param>
        /// <remarks>
        /// Every Key and Value have their uniquely mapped file, therefore a different file is opened if TKey and TValue are different.
        /// </remarks>
        /// <returns>null if table does not exist</returns>
        public SortedTreeTable<TKey, TValue> OpenTable<TKey, TValue>(string tableName)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            return OpenTable<TKey, TValue>(GetFileName<TKey, TValue>(tableName));
        }

        /// <summary>
        /// Opens the table for the provided file name.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="fileName">the filename to open</param>
        /// <returns>null if table does not exist</returns>
        private SortedTreeTable<TKey, TValue> OpenTable<TKey, TValue>(SubFileName fileName)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            if (!m_openedFiles.ContainsKey(fileName))
            {
                if (!m_fileStructure.Snapshot.Header.ContainsSubFile(fileName))
                    return null;
                m_openedFiles.Add(fileName, new SortedTreeTable<TKey, TValue>(m_fileStructure, fileName, this));
            }
            return (SortedTreeTable<TKey, TValue>)m_openedFiles[fileName];
        }

        /// <summary>
        /// Opens the default table for this TKey and TValue. If it does not exists, 
        /// it will be created with the provided compression method.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="storageMethod">The method of compression to utilize in this table.</param>
        /// <param name="maxSortedTreeBlockSize">the maximum desired block size for a SortedTree. Must be at least 1024.</param>
        /// <param name="tableName">the name of an internal table</param>
        /// <returns></returns>
        public SortedTreeTable<TKey, TValue> OpenOrCreateTable<TKey, TValue>(EncodingDefinition storageMethod, string tableName, int maxSortedTreeBlockSize = 4096)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            if (storageMethod is null)
                throw new ArgumentNullException("storageMethod");

            SubFileName fileName = GetFileName<TKey, TValue>(tableName);
            return OpenOrCreateTable<TKey, TValue>(storageMethod, fileName, maxSortedTreeBlockSize);
        }

        /// <summary>
        /// Opens the default table for this TKey and TValue. If it does not exists, 
        /// it will be created with the provided compression method.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="storageMethod">The method of compression to utilize in this table.</param>
        /// <param name="maxSortedTreeBlockSize">the maximum desired block size for a SortedTree. Must be at least 1024.</param>
        /// <returns></returns>
        public SortedTreeTable<TKey, TValue> OpenOrCreateTable<TKey, TValue>(EncodingDefinition storageMethod, int maxSortedTreeBlockSize = 4096)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            if (storageMethod is null)
                throw new ArgumentNullException("storageMethod");

            SubFileName fileName = GetFileName<TKey, TValue>();
            return OpenOrCreateTable<TKey, TValue>(storageMethod, fileName, maxSortedTreeBlockSize);
        }

        private SortedTreeTable<TKey, TValue> OpenOrCreateTable<TKey, TValue>(EncodingDefinition storageMethod, SubFileName fileName, int maxSortedTreeBlockSize)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            if (!m_openedFiles.ContainsKey(fileName))
            {
                if (!m_fileStructure.Snapshot.Header.ContainsSubFile(fileName))
                    CreateArchiveFile<TKey, TValue>(fileName, storageMethod, maxSortedTreeBlockSize);
                m_openedFiles.Add(fileName, new SortedTreeTable<TKey, TValue>(m_fileStructure, fileName, this));
            }
            return (SortedTreeTable<TKey, TValue>)m_openedFiles[fileName];
        }

        /// <summary>
        /// Helper method. Creates the <see cref="SubFileName"/> for the default table.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        private SubFileName GetFileName<TKey, TValue>()
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            Guid keyType = new TKey().GenericTypeGuid;
            Guid valueType = new TValue().GenericTypeGuid;
            return SubFileName.Create(PrimaryArchiveType, keyType, valueType);
        }

        private SubFileName GetFileName<TKey, TValue>(string fileName)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            Guid keyType = new TKey().GenericTypeGuid;
            Guid valueType = new TValue().GenericTypeGuid;
            return SubFileName.Create(fileName, keyType, valueType);
        }

        private void CreateArchiveFile<TKey, TValue>(SubFileName fileName, EncodingDefinition storageMethod, int maxSortedTreeBlockSize)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            if (maxSortedTreeBlockSize < 1024)
                throw new ArgumentOutOfRangeException(nameof(maxSortedTreeBlockSize), "Must be greater than 1024");
            if (storageMethod is null)
                throw new ArgumentNullException("storageMethod");

            using (TransactionalEdit trans = m_fileStructure.BeginEdit())
            {
                using (SubFileStream fs = trans.CreateFile(fileName))
                using (BinaryStream bs = new BinaryStream(fs))
                {
                    int blockSize = m_fileStructure.Snapshot.Header.DataBlockSize;

                    while (blockSize > maxSortedTreeBlockSize)
                    {
                        blockSize >>= 2;
                    }

                    SortedTree<TKey, TValue> tree = SortedTree<TKey, TValue>.Create(bs, blockSize, storageMethod);
                    tree.Flush();
                }
                trans.ArchiveType = FileType;
                trans.CommitAndDispose();
            }
        }


        /// <summary>
        /// Closes the archive file. If there is a current transaction, 
        /// that transaction is immediately rolled back and disposed.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                foreach (IDisposable d in m_openedFiles.Values)
                {
                    d.Dispose();
                }
                m_openedFiles.Clear();
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
            if (m_filePath != string.Empty)
            {
                File.Delete(m_filePath);
            }
        }

        #endregion


    }
}