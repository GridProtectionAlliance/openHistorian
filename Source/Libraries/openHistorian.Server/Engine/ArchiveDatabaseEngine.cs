//******************************************************************************************************
//  ArchiveDatabaseEngine.cs - Gbtc
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
//  5/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF;
using openHistorian.Engine.ArchiveWriters;
using openHistorian.Engine.Configuration;

namespace openHistorian.Engine
{
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public class ArchiveDatabaseEngine : IHistorianDatabase, IDisposable
    {
        List<ArchiveListRemovalStatus> m_pendingDispose;
        WriteProcessor m_archiveWriter;
        ArchiveList m_archiveList;
        volatile bool m_disposed;

        public ArchiveDatabaseEngine(WriterOptions? writer, params string[] paths)
            : this(new DatabaseConfig(writer, paths))
        {

        }

        public ArchiveDatabaseEngine(DatabaseConfig settings)
            : this(new DatabaseSettings(settings), settings)
        {

        }

        internal ArchiveDatabaseEngine(DatabaseSettings settings, DatabaseConfig config)
        {
            m_pendingDispose = new List<ArchiveListRemovalStatus>();
            m_archiveList = new ArchiveList(settings.AttachedFiles);

            if (settings.ArchiveWriter != null)
            {
                m_archiveWriter = new WriteProcessor(WriteProcessorSettings.CreateFromSettings(config, m_archiveList), m_archiveList);
            }
        }

        public void Write(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (m_archiveWriter == null)
                throw new Exception("Writing is not configured on this historian");
            m_archiveWriter.Write(key1, key2, value1, value2);
        }

        public void Write(IStream256 points)
        {

            m_archiveWriter.Write(points);
            ulong key1, key2, value1, value2;
            while (points.Read(out key1, out key2, out value1, out value2))
                Write(key1, key2, value1, value2);
        }

        public void SoftCommit()
        {
            m_archiveWriter.SoftCommit();
        }

        public void HardCommit()
        {
            m_archiveWriter.HardCommit();
        }

        public void Disconnect()
        {

        }

        /// <summary>
        /// Opens a stream connection that can be used to read 
        /// and write data to the current historian database.
        /// </summary>
        /// <returns></returns>
        public IHistorianDataReader OpenDataReader()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return new ArchiveReader(m_archiveList);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                if (m_archiveWriter != null)
                    m_archiveWriter.Dispose();

                m_archiveList.Dispose();

                foreach (var status in m_pendingDispose)
                {
                    status.Archive.Dispose();
                }
            }
        }

    }
}
