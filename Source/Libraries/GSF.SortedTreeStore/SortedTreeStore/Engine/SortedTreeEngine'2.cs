//******************************************************************************************************
//  SortedTreeEngine`2.cs - Gbtc
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
using System.IO;
using System.Linq;
using System.Text;
using GSF.SortedTreeStore.Engine.Reader;
using GSF.SortedTreeStore.Engine.Writer;
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine
{
    // TODO: Create a constructor that takes WriteProcessorSettings as a parameter so these can be passed in via HistorianServer
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public class SortedTreeEngine<TKey, TValue>
        : SortedTreeEngineBase<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        #region [ Members ]

        // Fields
        //private readonly List<ArchiveListRemovalStatus<TKey, TValue>> m_pendingDispose;
        TKey m_tmpKey;
        TValue m_tmpValue;
        private readonly WriteProcessor<TKey, TValue> m_archiveWriter;
        private readonly ArchiveList<TKey, TValue> m_archiveList;
        private volatile bool m_disposed;
        private string m_databaseName;
        public event UnhandledExceptionEventHandler Exception;

        #endregion

        #region [ Constructors ]

        public SortedTreeEngine(string databaseName, WriterMode writer, EncodingDefinition encodingMethod, params string[] paths)
        {
            m_tmpKey = new TKey();
            m_tmpValue = new TValue();
            m_databaseName = databaseName;
            //m_pendingDispose = new List<ArchiveListRemovalStatus<TKey, TValue>>();
            m_archiveList = new ArchiveList<TKey, TValue>(GetAttachedFiles(paths));

            if (writer == WriterMode.InMemory)
            {
                m_archiveWriter = WriteProcessor<TKey, TValue>.CreateInMemory(m_archiveList, encodingMethod);
                m_archiveWriter.Exception += OnException;
            }
            else if (writer == WriterMode.OnDisk)
            {
                m_archiveWriter = WriteProcessor<TKey, TValue>.CreateOnDisk(m_archiveList, encodingMethod, paths[0]);
                m_archiveWriter.Exception += OnException;
            }

        }

        void OnException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledExceptionEventHandler handler = Exception;
            if (handler != null)
                handler(sender, e);
        }

        List<string> GetAttachedFiles(string[] paths)
        {
            var attachedFiles = new List<string>();

            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    attachedFiles.Add(path);
                }
                else if (Directory.Exists(path))
                {
                    attachedFiles.AddRange(Directory.GetFiles(path, "*.d2", SearchOption.TopDirectoryOnly));
                }
            }
            return attachedFiles;
        }


        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        public void GetFullStatus(StringBuilder status)
        {
            m_archiveList.GetFullStatus(status);
        }

        public override void Write(TKey key, TValue value)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (m_archiveWriter == null)
                throw new Exception("Writing is not configured on this historian");

            m_archiveWriter.Write(key, value);
        }

        public override void Write(TreeStream<TKey, TValue> points)
        {
            TKey key = new TKey();
            TValue value = new TValue();
            //ToDo: Prebuffer the points in the stream. It is possible that this call may be behind a slow socket interface, therefore it will lockup the writing speed.
            while (points.Read(key, value))
                Write(key, value);
        }

        public override DatabaseInfo Info
        {
            get
            {
                return new DatabaseInfo(m_databaseName, m_tmpKey, m_tmpValue);
            }
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


        public override TreeStream<TKey, TValue> Read(SortedTreeEngineReaderOptions readerOptions, SeekFilterBase<TKey> keySeekFilter, MatchFilterBase<TKey, TValue> keyMatchFilter)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Stats.QueriesExecuted++;
            return new SequentialReaderStream<TKey, TValue>(m_archiveList, readerOptions, keySeekFilter, keyMatchFilter);
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

                //foreach (ArchiveListRemovalStatus<TKey, TValue> status in m_pendingDispose)
                //{
                //    status.Archive.Dispose();
                //}
            }
        }

        #endregion
    }
}