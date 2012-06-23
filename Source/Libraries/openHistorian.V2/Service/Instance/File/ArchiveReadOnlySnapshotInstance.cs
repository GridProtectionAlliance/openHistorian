//******************************************************************************************************
//  ArchiveReadOnlySnapshotInstance.cs - Gbtc
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
using openHistorian.V2.Collections.KeyValue;
using openHistorian.V2.FileSystem;

namespace openHistorian.V2.Service.Instance.File
{
    /// <summary>
    /// Provides a user with a read-only instance of an archive.
    /// This class is not thread safe.
    /// </summary>
    public class ArchiveReadOnlySnapshotInstance : IDisposable
    {
        static Guid s_pointDataFile = new Guid("{29D7CCC2-A474-11E1-885A-B52D6288709B}");
        
        bool m_disposed;
        BasicTreeContainer m_dataTree;

        public ArchiveReadOnlySnapshotInstance(TransactionalRead currentTransaction)
        {
            m_dataTree = new BasicTreeContainer(currentTransaction, s_pointDataFile, 1);
        }

        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        public IDataScanner GetDataRange()
        {
            return m_dataTree.GetDataRange();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (m_dataTree != null)
                    {
                        m_dataTree.Dispose();
                        m_dataTree = null;
                    }
                }
                finally
                {
                    m_disposed = true;
                }
            }
        }
    }
}
