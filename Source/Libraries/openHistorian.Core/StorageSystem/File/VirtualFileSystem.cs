//******************************************************************************************************
//  VirtualFileSystem.cs - Gbtc
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
//  9/30/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************
using System.Collections.Generic;
using System;
using System.IO;

namespace openHistorian.Core.StorageSystem.File
{
    public class VirtualFileSystem : IStorageSystem , IDisposable
    {
        #region [ Members ]
        #region [ Constants ]
        #endregion
        #region [ Delegates ]
        #endregion
        #region [ Events ]
        #endregion
        #region [ Fields ]

        FileSystemSnapshotService m_file;

        #endregion
        #endregion
        #region [ Constructors ]

        /// <summary>
        /// Creates a new archive file
        /// </summary>
        /// <param name="ArchiveFile"></param>
        /// <param name="PageSize"></param>
        /// <param name="InitialSize"></param>
        private VirtualFileSystem(string ArchiveFile, int PageSize, long InitialSize)
        {
            if (ArchiveFile == null)
                throw new ArgumentNullException("ArchiveFile");

            if (System.IO.File.Exists(ArchiveFile))
                throw new Exception("ArchiveFile Already Exists");

            m_file = FileSystemSnapshotService.CreateFile(ArchiveFile);
        }
       
        /// <summary>
        /// Opens an existing archvie file.
        /// </summary>
        /// <param name="ArchiveFile">The path to the archvie file</param>
        /// <param name="isReadOnly"></param>
        private VirtualFileSystem(string ArchiveFile,bool isReadOnly)
        {
            if (ArchiveFile == null)
                throw new ArgumentNullException("ArchiveFile");

            if (!System.IO.File.Exists(ArchiveFile))
                throw new Exception("ArchiveFile Does Not Exist");

            m_file = FileSystemSnapshotService.OpenFile(ArchiveFile, isReadOnly);
        }

        ~VirtualFileSystem()
        {
            Dispose();
        }

        #endregion
        #region [ Properties ]

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

        //public long FileSize
        //{
        //    get { return m_file.FileSize; }
        //}

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
        #region [ Operators ]
        #endregion
        #region [ Static ]
        #region [ Fields ]
        #endregion
        #region [ Constructors ]
        #endregion
        #region [ Properties ]
        #endregion
        #region [ Methods ]

        public static VirtualFileSystem CreateArchive(string File)
        {
            int PageSize = 65536;
            long InitialSize = 100000000;
            return CreateArchive(File, PageSize, InitialSize);
        }

        public static VirtualFileSystem CreateArchive(string ArchiveFile, int PageSize, long InitialSize)
        {
            return new VirtualFileSystem(ArchiveFile, PageSize, InitialSize);
        }

        public static VirtualFileSystem OpenArchive(string ArchiveFile, bool isReadOnly)
        {
            return new VirtualFileSystem(ArchiveFile, isReadOnly);
        }

        #endregion
        #endregion

    }
}
