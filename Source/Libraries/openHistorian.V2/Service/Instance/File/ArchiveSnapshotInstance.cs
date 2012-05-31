//******************************************************************************************************
//  ArchiveSnapshotInstance.cs - Gbtc
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
//  5/22/2012 - Steven E. Chisholm
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
    /// Provides a user with a read-only instance of an archive.
    /// </summary>
    public class ArchiveSnapshotInstance
    {
        static Guid s_pointDataFile = new Guid("{29D7CCC2-A474-11E1-885A-B52D6288709B}");
        static Guid s_pointMappingGuidToLocalFile = new Guid("{19352E28-A474-11E1-9A11-992D6288709B}");
        static Guid s_pointMappingLocalToGuidFile = new Guid("{1E458732-A474-11E1-9B1B-A82D6288709B}");

        TransactionalRead m_currentTransaction;

        ArchiveFileStream m_streamPointData;
        BinaryStream m_binaryStreamPointData;
        BasicTree m_pointData;

        ArchiveFileStream m_streamPointMappingGuidToLocal;
        BinaryStream m_binaryStreamPointMappingGuidToLocal;
        BasicTree m_pointMappingGuidToLocal;

        ArchiveFileStream m_streamPointMappingLocalToGuid;
        BinaryStream m_binaryStreamPointMappingLocalToGuid;
        BasicTree m_pointMappingLocalToGuid;

        public ArchiveSnapshotInstance(TransactionalRead currentTransaction)
        {
            m_currentTransaction = currentTransaction;
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
        
        public IEnumerable<Tuple<long, long, long, long>> GetData(long pointId, DateTime startDate, DateTime stopDate)
        {
            return null;
        }
        public IEnumerable<Tuple<long, long, long, long>> GetData(DateTime startDate, DateTime stopDate)
        {
            return null;
        }
        public void GetData(Func<long, long, long, long, bool> callback)
        {
            m_pointData.GetRange(callback);
        }

        public void Close()
        {
            //m_tree.Save();
        }

    }
}
