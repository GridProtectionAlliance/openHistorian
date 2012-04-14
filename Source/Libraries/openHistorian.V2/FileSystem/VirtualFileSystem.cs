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

using System;

namespace openHistorian.V2.FileSystem
{
    public class VirtualFileSystem : IDisposable
    {
        #region [ Members ]

        FileSystemSnapshotService m_file;

        #endregion
        #region [ Constructors ]

        /// <summary>
        /// Creates a new inMemory archive file
        /// </summary>
        private VirtualFileSystem()
        {
            m_file = FileSystemSnapshotService.CreateInMemory();
        }

        ///// <summary>
        ///// Creates a new archive file
        ///// </summary>
        ///// <param name="archiveFile"></param>
        //private VirtualFileSystem(string archiveFile)
        //{
        //    if (archiveFile == null)
        //        throw new ArgumentNullException("archiveFile");

        //    if (System.IO.File.Exists(archiveFile))
        //        throw new Exception("ArchiveFile Already Exists");

        //    m_file = FileSystemSnapshotService.CreateFile(archiveFile);
        //}

        ///// <summary>
        ///// Opens an existing archvie file.
        ///// </summary>
        ///// <param name="archiveFile">The path to the archvie file</param>
        ///// <param name="isReadOnly"></param>
        //private VirtualFileSystem(string archiveFile, bool isReadOnly)
        //{
        //    if (archiveFile == null)
        //        throw new ArgumentNullException("archiveFile");

        //    if (!System.IO.File.Exists(archiveFile))
        //        throw new Exception("ArchiveFile Does Not Exist");

        //    m_file = FileSystemSnapshotService.OpenFile(archiveFile, isReadOnly);
        //}

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

        public TransactionalEdit BeginEdit()
        {
            return m_file.BeginEditTransaction();
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
        #region [ Operators ]
        #endregion
        #region [ Static ]

        //public static VirtualFileSystem CreateArchive(string archiveFile)
        //{
        //    return new VirtualFileSystem(archiveFile);
        //}

        //public static VirtualFileSystem OpenArchive(string archiveFile, bool isReadOnly)
        //{
        //    return new VirtualFileSystem(archiveFile, isReadOnly);
        //}

        public static VirtualFileSystem CreateInMemoryArchive()
        {
            return new VirtualFileSystem();
        }

     
        #endregion

    }
}
