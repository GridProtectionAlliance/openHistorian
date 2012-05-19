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
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.V2.Collections.KeyValue;
using openHistorian.V2.FileSystem;
using openHistorian.V2.IO.Unmanaged;
using openHistorian.V2.Providers;

namespace openHistorian.V2.Service.Instance.File
{
    /// <summary>
    /// Represents a individual self-contained archive file. This is one of many files that are part of a given <see cref="Engine"/>.
    /// </summary>
    public class Archive
    {
        VirtualFileSystem m_fileSystem;
        TransactionalEdit m_currentTransaction;
        ArchiveFileStream m_stream1;
        BinaryStream m_binaryStream1;
        BasicTree m_tree;

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
            m_currentTransaction = m_fileSystem.BeginEdit();
            m_stream1 = m_currentTransaction.CreateFile(new Guid("{7bfa9083-701e-4596-8273-8680a739271c}"), 1);
            m_binaryStream1 = new BinaryStream(m_stream1);
            m_tree = new BasicTree(m_binaryStream1, ArchiveConstants.DataBlockDataLength);
        }

        public IMetaDataProvider[] MetaData;
        public ITimeSeriesDataProvider[] TimeSeriesData;
        
        public void AddPoint(long date, long pointId, long value1, long value2)
        {
            m_tree.Add(date,pointId,value1,value2);
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
            //m_tree.Save();
            m_stream1.Flush();
            m_currentTransaction.Commit();
            m_fileSystem.Dispose();
        }

    }
}
