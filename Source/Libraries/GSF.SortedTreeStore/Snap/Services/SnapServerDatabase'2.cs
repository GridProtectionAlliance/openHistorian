//******************************************************************************************************
//  SnapServerDatabase`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GSF.Diagnostics;
using GSF.Snap.Filters;
using GSF.Snap.Services.Reader;
using GSF.Snap.Services.Writer;
using GSF.Threading;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Creates an engine for reading/writing data from a SortedTreeStore.
    /// </summary>
    public partial class SnapServerDatabase<TKey, TValue>
         : SnapServerDatabaseBase
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        #region [ Members ]

        // Fields
        //private readonly List<ArchiveListRemovalStatus<TKey, TValue>> m_pendingDispose;
        private readonly TKey m_tmpKey;
        private readonly TValue m_tmpValue;
        private readonly WriteProcessor<TKey, TValue> m_archiveWriter;
        private readonly ArchiveList<TKey, TValue> m_archiveList;
        private volatile bool m_disposed;
        private readonly List<EncodingDefinition> m_supportedStreamingMethods;
        private readonly ServerDatabaseSettings m_settings;
        private readonly RolloverLog m_rolloverLog;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public SnapServerDatabase(ServerDatabaseSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            m_settings = settings.CloneReadonly();
            m_settings.Validate();

            m_tmpKey = new TKey();
            m_tmpValue = new TValue();
            m_supportedStreamingMethods = settings.StreamingEncodingMethods.ToList();

            Info = new DatabaseInfo(m_settings.DatabaseName, m_tmpKey, m_tmpValue, m_supportedStreamingMethods);

            using (Logger.AppendStackMessages(GetSourceDetails()))
            {
                m_archiveList = new ArchiveList<TKey, TValue>(m_settings.ArchiveList);
                m_rolloverLog = new RolloverLog(m_settings.RolloverLog, m_archiveList);

                if (m_settings.SupportsWriting)
                    m_archiveWriter = new WriteProcessor<TKey, TValue>(m_archiveList, m_settings.WriteProcessor, m_rolloverLog);
            }
        }

        /// <summary>
        /// Loads the provided files from all of the specified paths.
        /// </summary>
        /// <param name="paths">all of the paths of archive files to attach. These can either be a path, or an individual file name.</param>
        private void AttachFilesOrPaths(IEnumerable<string> paths)
        {
            m_archiveList.AttachFileOrPath(paths);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets basic information about the current Database.
        /// </summary>
        public override DatabaseInfo Info { get; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets status information for the SortedTreeEngine
        /// </summary>
        /// <param name="status">Target status output <see cref="StringBuilder"/>.</param>
        /// <param name="maxFileListing">Maximum file listing.</param>
        public override void GetFullStatus(StringBuilder status, int maxFileListing = -1)
        {
            m_archiveList.GetFullStatus(status, maxFileListing);
        }

        private List<ArchiveDetails> GetAllAttachedFiles()
        {
            return m_archiveList.GetAllAttachedFiles();
        }

        private void DetatchFiles(List<Guid> files)
        {
            using ArchiveListEditor<TKey, TValue> editor = m_archiveList.AcquireEditLock();
            
            foreach (Guid file in files)
                editor.TryRemoveAndDispose(file);
        }

        private void DeleteFiles(List<Guid> files)
        {
            using ArchiveListEditor<TKey, TValue> editor = m_archiveList.AcquireEditLock();
            
            foreach (Guid file in files)
                editor.TryRemoveAndDelete(file);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="SnapServerDatabase{TKey,TValue}"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                // This will be done regardless of whether the object is finalized or disposed.
                if (disposing)
                {
                    m_disposed = true;

                    if (m_archiveWriter != null)
                        m_archiveWriter.Dispose();

                    m_archiveList.Dispose();
                }
            }
            finally
            {
                m_disposed = true;       // Prevent duplicate dispose.
                base.Dispose(disposing); // Call base class Dispose().
            }
        }

        /// <summary>
        /// Forces a soft commit on the database. A soft commit 
        /// only commits data to memory. This allows other clients to read the data.
        /// While soft committed, this data could be lost during an unexpected shutdown.
        /// Soft commits usually occur within microseconds. 
        /// </summary>
        private void SoftCommit()
        {
            //m_archiveWriter.SoftCommit();
        }

        /// <summary>
        /// Forces a commit to the disk subsystem. Once this returns, the data will not
        /// be lost due to an application crash or unexpected shutdown.
        /// Hard commits can take 100ms or longer depending on how much data has to be committed. 
        /// This requires two consecutive hardware cache flushes.
        /// </summary>
        private void HardCommit()
        {
            //m_archiveWriter.HardCommit();
        }

        private void Write(TKey key, TValue value)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (m_archiveWriter is null)
                throw new Exception("Writing is not configured on this historian");

            m_archiveWriter.Write(key, value);
        }

        private void Write(TreeStream<TKey, TValue> stream)
        {
            TKey key = new TKey();
            TValue value = new TValue();

            // TODO: Pre=buffer the points in the stream. It is possible that this call may be behind a slow socket interface, therefore it will lockup the writing speed.
            while (stream.Read(key, value))
                Write(key, value);
        }

        private SequentialReaderStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions,
            SeekFilterBase<TKey> keySeekFilter,
            MatchFilterBase<TKey, TValue> keyMatchFilter,
            WorkerThreadSynchronization workerThreadSynchronization)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Stats.QueriesExecuted++;
            return new SequentialReaderStream<TKey, TValue>(m_archiveList, readerOptions, keySeekFilter, keyMatchFilter, workerThreadSynchronization);
        }

        /// <summary>
        /// Creates a <see cref="ClientDatabase"/>
        /// </summary>
        /// <returns></returns>
        public override ClientDatabaseBase CreateClientDatabase(SnapClient client, Action<ClientDatabaseBase> onDispose)
        {
            return new ClientDatabase(this, client, onDispose);
        }

        #endregion
    }
}