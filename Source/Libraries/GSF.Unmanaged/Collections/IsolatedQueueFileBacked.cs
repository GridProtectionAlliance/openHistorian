//******************************************************************************************************
//  IsolatedQueueFileBacked.cs - Gbtc
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
//  1/4/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GSF.Threading;

namespace openHistorian.Collections
{
    public interface ILoadable
    {
        int SizeOf { get; }
        void Save(BinaryWriter writer);
        void Load(BinaryReader reader);
    }

    //ToDo: Don't let the diskIO operations apply a lock on the entire Enqueue thread.

    public partial class IsolatedQueueFileBacked<T> : IDisposable
        where T : struct, ILoadable
    {
        ContinuousQueue<IsolatedNode<T>> m_inboundQueue;
        ContinuousQueue<IsolatedNode<T>> m_outboundQueue;
        ResourceQueue<IsolatedNode<T>> m_pooledNodes;

        IsolatedNode<T> m_currentHead;
        IsolatedNode<T> m_currentTail;

        AsyncWorker m_workerDumpToFile;
        int m_unitCount;

        object m_syncRoot;
        bool m_isFileMode;
        int m_maxCount;
        int m_itemsPerFile;
        FileIO m_fileIO;

        public IsolatedQueueFileBacked(string path, int maxInMemorySize, int individualFileSize)
        {
            m_fileIO = new FileIO(path);
            m_isFileMode = (m_fileIO.FileCount > 0);

            m_unitCount = 1024;
            m_pooledNodes = new ResourceQueue<IsolatedNode<T>>(() => new IsolatedNode<T>(m_unitCount), 2, 10);
            m_inboundQueue = new ContinuousQueue<IsolatedNode<T>>();
            m_outboundQueue = new ContinuousQueue<IsolatedNode<T>>();
            m_workerDumpToFile = new AsyncWorker();
            m_workerDumpToFile.DoWork += WorkerDumpToFileDoWork;
            m_syncRoot = new object();
            T value = default(T);
            m_maxCount = maxInMemorySize / value.SizeOf;
            m_itemsPerFile = individualFileSize / value.SizeOf;
        }

        void WorkerDumpToFileDoWork(object sender, EventArgs e)
        {
            lock (m_syncRoot)
            {
                //Check for a premature call
                if (m_inboundQueue.Count * (long)m_unitCount < m_maxCount)
                {
                    return;
                }
                if (!m_isFileMode)
                {
                    m_isFileMode = true;
                }
                while (m_inboundQueue.Count * (long)m_unitCount > m_itemsPerFile)
                {
                    int itemsToKeep = Math.Max(1, m_inboundQueue.Count - m_itemsPerFile / m_unitCount);
                    m_fileIO.DumpToDisk(m_inboundQueue, itemsToKeep);
                }
            }
        }

        public void Enqueue(T item)
        {
            if (m_currentHead == null || m_currentHead.IsHeadFull)
            {
                if (m_inboundQueue.Count * (long)m_unitCount > m_maxCount)
                {
                    m_workerDumpToFile.RunWorker();
                }
                m_currentHead = m_pooledNodes.Dequeue();
                m_currentHead.Reset();
                lock (m_syncRoot)
                {
                    m_inboundQueue.Enqueue(m_currentHead);
                }
            }
            m_currentHead.Enqueue(item);
        }

        public bool TryDequeue(out T item)
        {
            if (m_currentTail == null || m_currentTail.IsTailFull)
            {
                if (m_currentTail != null)
                {
                    //Don't reset the node on return since it is still
                    //possible for the enqueue thread to be using it. 
                    //Note: If the enqueue thread pulls it off the queue
                    //immediately, this is ok since it will be coordinated at that point.
                    m_pooledNodes.Enqueue(m_currentTail);
                }

                bool success;
                lock (m_syncRoot)
                {
                TryAgain:
                    if (m_isFileMode)
                    {
                        if (m_outboundQueue.Count > 0)
                        {
                            m_currentTail = m_outboundQueue.Dequeue();
                            success = true;
                        }
                        else
                        {
                            if (m_fileIO.FileCount == 0)
                            {
                                m_isFileMode = false;
                            }
                            else
                            {
                                m_fileIO.ReadFromDisk(m_outboundQueue, () => m_pooledNodes.Dequeue());
                            }
                            goto TryAgain;
                        }
                    }
                    else
                    {
                        if (m_inboundQueue.Count > 0)
                        {
                            m_currentTail = m_inboundQueue.Dequeue();
                            success = true;
                        }
                        else
                        {
                            success = false;
                        }
                    }

                }

                if (!success)
                {
                    m_currentTail = null;
                    item = default(T);
                    return false;
                }
            }
            return m_currentTail.TryDequeue(out item);
        }

        /// <summary>
        /// Since the math for the count is pretty complex, this only gives an idea of the
        /// size of the structure.  In other words, a value of zero does not mean this list is empty.
        /// </summary>
        public long EstimateCount
        {
            get
            {
                //ToDo: properly calculate the number of elements.
                //This should also include the files pending loading.
                return m_inboundQueue.Count * (long)m_unitCount;
            }
        }

        public void Dispose()
        {
            m_workerDumpToFile.Dispose();

        }
    }
}
