//******************************************************************************************************
//  TransactionalFileStructure.cs - Gbtc
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
//  9/30/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;

namespace GSF.IO.FileStructure
{
    public sealed class TransactionalFileStructure 
        : IDisposable
    {
        #region [ Members ]

        private TransactionService m_file;
        private int m_blockSize;

        #endregion

        #region [ Constructors ]

        private TransactionalFileStructure()
        {
        }

        /// <summary>
        /// Creates a new inMemory archive file
        /// </summary>
        public static TransactionalFileStructure CreateInMemory(int blockSize = 4096)
        {
            TransactionalFileStructure fs = new TransactionalFileStructure();
            fs.m_blockSize = blockSize;
            fs.m_file = TransactionService.CreateInMemory(blockSize);
            return fs;
        }

        /// <summary>
        /// Creates a new archive file
        /// </summary>
        public static TransactionalFileStructure CreateFile(string archiveFile, int blockSize = 4096)
        {
            if (archiveFile == null)
                throw new ArgumentNullException("archiveFile");
            if (File.Exists(archiveFile))
                throw new Exception("ArchiveFile Already Exists");

            TransactionalFileStructure fs = new TransactionalFileStructure();
            fs.m_blockSize = blockSize;
            fs.m_file = TransactionService.CreateFile(archiveFile, blockSize);
            return fs;
        }

        /// <summary>
        /// Creates a new archive file
        /// </summary>
        public static TransactionalFileStructure OpenFile(string archiveFile, bool isReadOnly)
        {
            if (archiveFile == null)
                throw new ArgumentNullException("archiveFile");
            if (!File.Exists(archiveFile))
                throw new Exception("ArchiveFile Does Not Exists");

            TransactionalFileStructure fs = new TransactionalFileStructure();
            fs.m_file = TransactionService.OpenFile(archiveFile, isReadOnly);
            fs.m_blockSize = fs.m_file.BlockSize;
            return fs;
        }

        #endregion

        #region [ Properties ]

        public int BlockSize
        {
            get
            {
                return m_blockSize;
            }
        }

        public int DataBlockSize
        {
            get
            {
                return m_blockSize - FileStructureConstants.BlockFooterLength;
            }
        }

        //public Guid ArchiveID
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public bool ReadOnly
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public List<object> FeatureList
        //{
        //    get { throw new NotImplementedException(); }
        //}

        public long ArchiveSize
        {
            get
            {
                return m_file.ArchiveSize;
            }
        }

        public Guid ArchiveType
        {
            get
            {
                return m_file.ArchiveType;
            }
        }

        public byte[] UserData
        {
            get
            {
                return m_file.UserData;
            }
        }

        //public long FreeSpace
        //{
        //    get { return m_file.FreeSpace; }
        //}

        //public Guid[] Features
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public DataStream OpenFeature(Guid FeatureID)
        //{
        //    throw new NotImplementedException();
        //}

        //public DataStream CreateFeature(Guid FeatureID)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region [ Methods ]

        public TransactionalEdit BeginEdit()
        {
            return m_file.BeginEditTransaction();
        }

        public TransactionalRead BeginRead()
        {
            return m_file.GetCurrentSnapshot();
        }

        //public long GrowArchive(long GrowAmount)
        //{
        //    throw new NotImplementedException();
        //}

        public void Dispose()
        {
            if (m_file != null)
            {
                m_file.Dispose();
                m_file = null;
            }
        }

        #endregion

        public bool ContainsSubFile(SubFileName fileName)
        {
            return m_file.GetCurrentSnapshot().ContainsSubFile(fileName);
        }
    }
}