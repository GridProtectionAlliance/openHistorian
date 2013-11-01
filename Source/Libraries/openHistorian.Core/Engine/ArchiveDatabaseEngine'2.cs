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
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Collections;
using openHistorian.Collections.Generic;
using openHistorian.Engine.ArchiveWriters;

namespace openHistorian.Engine
{
    // TODO: Create a constructor that takes WriteProcessorSettings as a parameter so these can be passed in via HistorianServer
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public class ArchiveDatabaseEngine<TKey, TValue>
        : HistorianDatabaseBase<TKey, TValue>
        where TKey : HistorianKeyBase<TKey>, new()
        where TValue : HistorianValueBase<TValue>, new()
    {
        #region [ Members ]

        // Fields
        private readonly List<ArchiveListRemovalStatus<TKey, TValue>> m_pendingDispose;
        private readonly WriteProcessor<TKey, TValue> m_archiveWriter;
        private readonly ArchiveList<TKey, TValue> m_archiveList;
        private volatile bool m_disposed;

        #endregion

        #region [ Constructors ]

        public ArchiveDatabaseEngine(WriterMode writer, params string[] paths)
            : this(new DatabaseConfig(writer, paths))
        {
        }

        public ArchiveDatabaseEngine(DatabaseConfig settings)
        {
            m_pendingDispose = new List<ArchiveListRemovalStatus<TKey, TValue>>();
            m_archiveList = new ArchiveList<TKey, TValue>(settings.GetAttachedFiles());

            if (settings.WriterMode != WriterMode.None)
            {
                WriteProcessorSettings<TKey, TValue> writeSettings = WriteProcessorSettings<TKey, TValue>.CreateFromSettings(settings, m_archiveList);
                m_archiveWriter = new WriteProcessor<TKey, TValue>(writeSettings, m_archiveList);
            }

        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        public override void Write(TKey key, TValue value)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (m_archiveWriter == null)
                throw new Exception("Writing is not configured on this historian");

            m_archiveWriter.Write(key, value);
        }

        public override void Write(KeyValueStream<TKey, TValue> points)
        {
            //ToDo: Prebuffer the points in the stream. It is possible that this call may be behind a slow socket interface, therefore it will lockup the writing speed.
            while (points.Read())
                Write(points.CurrentKey, points.CurrentValue);
        }

        public override void SoftCommit()
        {
            //m_archiveWriter.SoftCommit();
        }

        public override void HardCommit()
        {
            //m_archiveWriter.HardCommit();
        }

        public override void Disconnect()
        {
        }

        /// <summary>
        /// Opens a stream connection that can be used to read 
        /// and write data to the current historian database.
        /// </summary>
        /// <returns></returns>
        public override HistorianDataReaderBase<TKey, TValue> OpenDataReader()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return new ArchiveReaderSequential<TKey, TValue>(m_archiveList);
            //return new ArchiveReader<TKey, TValue>(m_archiveList);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                if (m_archiveWriter != null)
                    m_archiveWriter.Dispose();

                m_archiveList.Dispose();

                foreach (ArchiveListRemovalStatus<TKey, TValue> status in m_pendingDispose)
                {
                    status.Archive.Dispose();
                }
            }
        }

        #endregion
    }
}