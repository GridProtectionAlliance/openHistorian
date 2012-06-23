//******************************************************************************************************
//  Archive.cs - Gbtc
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
using System.Collections.Generic;
using openHistorian.V2.Collections.KeyValue;
using openHistorian.V2.FileSystem;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.Service.Instance.File
{
    /// <summary>
    /// Represents a individual self-contained archive file. 
    /// This is one of many files that are part of a given <see cref="Engine"/>.
    /// </summary>
    public class Archive
    {
        static Guid s_pointDataFile = new Guid("{29D7CCC2-A474-11E1-885A-B52D6288709B}");
        static Guid s_pointMappingGuidToLocalFile = new Guid("{19352E28-A474-11E1-9A11-992D6288709B}");
        static Guid s_pointMappingLocalToGuidFile = new Guid("{1E458732-A474-11E1-9B1B-A82D6288709B}");

        VirtualFileSystem m_fileSystem;

        TransactionalEdit m_currentTransaction;

        ArchiveFileStream m_streamPointData;
        BinaryStream m_binaryStreamPointData;
        BasicTree m_pointData;

        ArchiveFileStream m_streamPointMappingGuidToLocal;
        BinaryStream m_binaryStreamPointMappingGuidToLocal;
        BasicTree m_pointMappingGuidToLocal;

        ArchiveFileStream m_streamPointMappingLocalToGuid;
        BinaryStream m_binaryStreamPointMappingLocalToGuid;
        BasicTree m_pointMappingLocalToGuid;

        //public Archive(string file)
        //{
        //    if (File.Exists(file))
        //        OpenFile(file);
        //    else
        //        CreateFile(file);
        //}
        public Archive()
        {
            CreateFileInMemory();
        }

        //void OpenFile(string file)
        //{
        //    m_fileSystem = VirtualFileSystem.OpenArchive(file, false);
        //    m_currentTransaction = m_fileSystem.BeginEdit();
        //    m_stream = m_currentTransaction.OpenFile(0);
        //    m_binaryStream = new BinaryStream(m_stream);
        //    m_tree = new BPlusTree<KeyType, TreeTypeIntFloat>(m_binaryStream);
        //}
        //void CreateFile(string file)
        //{
        //    m_fileSystem = VirtualFileSystem.CreateArchive(file);
        //    m_currentTransaction = m_fileSystem.BeginEdit();
        //    m_stream = m_currentTransaction.CreateFile(new Guid("{7bfa9083-701e-4596-8273-8680a739271d}"), 1);
        //    m_binaryStream = new BinaryStream(m_stream);
        //    m_tree = new BPlusTree<KeyType, TreeTypeIntFloat>(m_binaryStream, ArchiveConstants.DataBlockDataLength);
        //}
        void CreateFileInMemory()
        {
            m_fileSystem = VirtualFileSystem.CreateInMemoryArchive();
        }

        /// <summary>
        /// Aquires a snapshot of the current file system.
        /// </summary>
        /// <returns></returns>
        public ArchiveSnapshot CreateSnapshot()
        {
            return new ArchiveSnapshot(m_fileSystem);
        }

        /// <summary>
        /// Begins an edit of the current archive file.
        /// </summary>
        public void BeginEdit()
        {
            m_currentTransaction = m_fileSystem.BeginEdit();

            m_streamPointData = m_currentTransaction.OpenFile(s_pointDataFile, 1);
            m_binaryStreamPointData = new BinaryStream(m_streamPointData);
            m_pointData = new BasicTree(m_binaryStreamPointData);

            m_streamPointMappingGuidToLocal = m_currentTransaction.OpenFile(s_pointMappingGuidToLocalFile, 1);
            m_binaryStreamPointMappingGuidToLocal = new BinaryStream(m_streamPointMappingGuidToLocal);
            m_pointMappingGuidToLocal = new BasicTree(m_binaryStreamPointMappingGuidToLocal);

            m_streamPointMappingLocalToGuid = m_currentTransaction.OpenFile(s_pointMappingLocalToGuidFile, 1);
            m_binaryStreamPointMappingLocalToGuid = new BinaryStream(m_streamPointMappingLocalToGuid);
            m_pointMappingLocalToGuid = new BasicTree(m_binaryStreamPointMappingLocalToGuid);
        }
        /// <summary>
        /// Commits the edits to the current archive file.
        /// </summary>
        public void CommitEdit()
        {
            //m_pointMappingLocalToGuid.Dispose();
            m_binaryStreamPointMappingLocalToGuid.Dispose();
            m_streamPointMappingLocalToGuid.Dispose();

            //m_pointMappingGuidToLocal.Dispose();
            m_binaryStreamPointMappingGuidToLocal.Dispose();
            m_streamPointMappingGuidToLocal.Dispose();

            //m_pointData.Dispose();
            m_binaryStreamPointData.Dispose();
            m_streamPointData.Dispose();

            m_currentTransaction.CommitAndDispose();
        }

        /// <summary>
        /// Rolls back all edits that are made to the archive file.
        /// </summary>
        public void RollbackEdit()
        {
            //m_pointMappingLocalToGuid.Dispose();
            m_binaryStreamPointMappingLocalToGuid.Dispose();
            m_streamPointMappingLocalToGuid.Dispose();

            //m_pointMappingGuidToLocal.Dispose();
            m_binaryStreamPointMappingGuidToLocal.Dispose();
            m_streamPointMappingGuidToLocal.Dispose();

            //m_pointData.Dispose();
            m_binaryStreamPointData.Dispose();
            m_streamPointData.Dispose();

            m_currentTransaction.RollbackAndDispose();
        }

        public DateTime GetFirstTimeStamp
        {
            get
            {
                return DateTime.Now;
            }
        }
        public DateTime GetLastTimeStamp
        {
            get
            {
                return DateTime.Now;
            } 
        }

        public void AddPoint(long date, long pointId, long value1, long value2)
        {
            m_pointData.Add(date, pointId, value1, value2);
        }

        public IEnumerable<Tuple<DateTime, long, int, float>> GetData(long pointId, DateTime startDate, DateTime stopDate)
        {
            return null;
        }
        public IEnumerable<Tuple<DateTime, long, int, float>> GetData(DateTime startDate, DateTime stopDate)
        {
            return null;
        }

        public void Close()
        {
            m_fileSystem.Dispose();
        }

    }
}
