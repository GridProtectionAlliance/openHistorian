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
using System.ComponentModel;
using System.IO;
using System.Text;
using GSF.Diagnostics;
using GSF.SortedTreeStore.Client;
using GSF.SortedTreeStore.Filters;
using GSF.SortedTreeStore.Server.Reader;
using GSF.SortedTreeStore.Server.Writer;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Server
{
    /// <summary>
    /// Creates an engine for reading/writing data from a SortedTreeStore.
    /// </summary>
    public class SortedTreeEngine<TKey, TValue>
        : SortedTreeEngineBase<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        #region [ Members ]

        // Fields
        //private readonly List<ArchiveListRemovalStatus<TKey, TValue>> m_pendingDispose;
        private TKey m_tmpKey;
        private TValue m_tmpValue;
        private WriteProcessor<TKey, TValue> m_archiveWriter;
        private ArchiveList<TKey, TValue> m_archiveList;
        private volatile bool m_disposed;
        private string m_databaseName;
        private LogReporter m_log;

        /// <summary>
        /// Event is raised when there is an exception encountered while processing.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the exception that was thrown.
        /// </remarks>
        public event EventHandler<EventArgs<Exception>> ProcessException;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an engine for reading/writing data from a SortedTreeStore.
        /// </summary>
        /// <param name="databaseName">the name of this database instance</param>
        /// <param name="writer">the write mode of this instance</param>
        /// <param name="encodingMethod">the method for encoding the archive files</param>
        /// <param name="writePath">the location where to write all of the archive files.</param>
        /// <param name="importPaths">all of the import paths of archive files. These can either be a path, or an individual file name.</param>
        public SortedTreeEngine(string databaseName, WriterMode writer, EncodingDefinition encodingMethod, string writePath, IEnumerable<string> importPaths = null)
        {
            if (databaseName == null)
                throw new ArgumentNullException("databaseName");
            switch (writer)
            {
                case WriterMode.None:
                case WriterMode.InMemory:
                case WriterMode.OnDisk:
                    break;
                default:
                    throw new InvalidEnumArgumentException("writer", (int)writer, typeof(WriterMode));
            }
            if (writePath == null)
                throw new ArgumentNullException("writePath");
            if (!Directory.Exists(writePath))
                throw new ArgumentException("Not an existing directory", "writePath");

            m_tmpKey = new TKey();
            m_tmpValue = new TValue();
            m_databaseName = databaseName;
            m_archiveList = new ArchiveList<TKey, TValue>();
            Logger.Default.Register(this, "GSF.SortedTreeStore.Engine.SortedTreeEngine<TKey, TValue>", "GSF.SortedTreeStore.Engine.SortedTreeEngine<TKey, TValue>", () => databaseName);

            if (writer == WriterMode.InMemory)
            {
                m_archiveWriter = WriteProcessor<TKey, TValue>.CreateInMemory(m_archiveList, encodingMethod);
            }
            else if (writer == WriterMode.OnDisk)
            {
                m_archiveWriter = WriteProcessor<TKey, TValue>.CreateOnDisk(m_archiveList, encodingMethod, writePath);
            }

            if (importPaths != null)
                AttachFilesOrPaths(importPaths);
        }

        /// <summary>
        /// Loads the provided files from all of the specified paths.
        /// </summary>
        /// <param name="paths">all of the paths of archive files to attach. These can either be a path, or an individual file name.</param>
        public void AttachFilesOrPaths(IEnumerable<string> paths)
        {
            m_archiveList.LoadFiles(GetAttachedFiles(paths));
        }

        /// <summary>
        /// Converts a list of file or paths to a list of all files.
        /// </summary>
        /// <param name="paths">the path to file names or directories to enumerate.</param>
        /// <returns></returns>
        IEnumerable<string> GetAttachedFiles(IEnumerable<string> paths)
        {
            var attachedFiles = new List<string>();
            foreach (string path in paths)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        attachedFiles.Add(path);
                    }
                    else if (Directory.Exists(path))
                    {
                        attachedFiles.AddRange(Directory.GetFiles(path, "*.d2", SearchOption.TopDirectoryOnly));
                    }
                    else
                    {
                        m_log.LogMessage(VerboseLevel.Warning, -1, "File or path does not exist", path);
                    }

                }
                catch (Exception ex)
                {
                    m_log.LogMessage(VerboseLevel.Error, -1, "Unknown error occured while attaching paths", "Path: " + path, null, ex);
                }

            }
            return attachedFiles;
        }


        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets status information for the SortedTreeEngine
        /// </summary>
        /// <param name="status">where to append the status log.</param>
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

        public override void Write(TreeStream<TKey, TValue> stream)
        {
            TKey key = new TKey();
            TValue value = new TValue();
            //ToDo: Prebuffer the points in the stream. It is possible that this call may be behind a slow socket interface, therefore it will lockup the writing speed.
            while (stream.Read(key, value))
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