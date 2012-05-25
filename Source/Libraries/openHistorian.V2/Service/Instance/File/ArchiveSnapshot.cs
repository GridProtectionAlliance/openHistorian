//******************************************************************************************************
//  ArchiveSnapshot.cs - Gbtc
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
using openHistorian.V2.Providers;

namespace openHistorian.V2.Service.Instance.File
{
    /// <summary>
    /// Represents a individual self-contained archive file. This is one of many files that are part of a given <see cref="Engine"/>.
    /// </summary>
    public class ArchiveSnapshot
    {
        VirtualFileSystem m_fileSystem;

        TransactionalRead m_currentTransaction;

        public ArchiveSnapshot(VirtualFileSystem fileSystem)
        {
            m_fileSystem = fileSystem;

            m_currentTransaction = m_fileSystem.BeginRead();
        }

        public ArchiveSnapshotInstance OpenInstance()
        {
            return new ArchiveSnapshotInstance(m_currentTransaction);
        }

        public void Close()
        {
            m_currentTransaction.Dispose();
        }

    }
}
