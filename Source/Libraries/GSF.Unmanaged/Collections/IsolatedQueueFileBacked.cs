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
using GSF.Threading;

namespace openHistorian.Collections
{
    /// <summary>
    /// An interface required to use <see cref="IsolatedQueueFileBacked{T}"/>.
    /// </summary>
    public interface ILoadable
    {
        /// <summary>
        /// Gets the average memory size required for an individual element. 
        /// </summary>
        int InMemorySize { get; }
        /// <summary>
        /// Gets the average space this individual element takes when serialized to the disk.
        /// </summary>
        int OnDiskSize { get; }
        /// <summary>
        /// Saves this element using the provided <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer"></param>
        void Save(BinaryWriter writer);
        /// <summary>
        /// Loads this element using the provided <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader"></param>
        void Load(BinaryReader reader);
    }

    //ToDo: Don't let the diskIO operations apply a lock on the entire Enqueue thread.

    /// <summary>
    /// Provides a high speed queue that has built in isolation of reads from writes. 
    /// This means, reads must be synchronized with other reads, writes must be synchronized with other writes,
    /// however read and write operations do not have to be synchronized.
    /// </summary>
    /// <typeparam name="T">A struct that implements <see cref="ILoadable"/> that will be stored in this queue.</typeparam>
    public partial class IsolatedQueueFileBacked<T> : IDisposable
        where T : struct, ILoadable
    {
        /// <summary>
        /// The queue that the writer always writes to. 
        /// This queue is only read from if not operating in FileMode
        /// </summary>
        ContinuousQueue<IsolatedNode<T>> m_inboundQueue;
        /// <summary>
        /// This queue is where the reader will read from when
        /// operating in FileMode.
        /// </summary>
        ContinuousQueue<IsolatedNode<T>> m_outboundQueue;
        /// <summary>
        /// Contains a queue of <see cref="IsolatedNode{T}"/> so they
        /// don't need to be constructed every time. There is a high
        /// probability that any node will be a Generation 2 object. Therefore
        /// it is advised to pool these objects.
        /// </summary>
        ResourceQueue<IsolatedNode<T>> m_pooledNodes;

        /// <summary>
        /// The node that will be written to. It is probable that the
        /// head and tail are the same instance.
        /// </summary>
        IsolatedNode<T> m_currentHead;
        /// <summary>
        /// The node that will be read from. It is probable that the
        /// head and tail are the same instance.
        /// </summary>
        IsolatedNode<T> m_currentTail;

        ScheduledTask m_workerFlushToFile;
        bool m_disposing;
        bool m_disposed;
        bool m_isFileMode;
        bool m_currentlyWritingFile;
        int m_elementsPerNode;
        object m_syncRoot;

        /// <summary>
        /// Number of nodes that it takes to max out the memory desired for this buffer.
        /// </summary>
        int m_maxNodeCount;
        /// <summary>
        /// The number of nodes to put in each file.
        /// </summary>
        int m_nodesPerFile;

        FileIO m_fileIO;

        /// <summary>
        /// Creates a new <see cref="IsolatedQueueFileBacked{T}"/>. 
        /// </summary>
        /// <param name="path">The disk path to use to save the state of this queue to. 
        /// It is critical that this path and file prefix is unique to the instance of this class.</param>
        /// <param name="filePrefix">The prefix string to add to the beginning of every file in this directory.</param>
        /// <param name="maxInMemorySize">The maximum desired in-memory size before switching to a file storage method.</param>
        /// <param name="individualFileSize">The desired size of each file.</param>
        /// <remarks>The total memory used by this class will be approximately the sum of <see cref="maxInMemorySize"/> and
        /// <see cref="individualFileSize"/> while operating in file mode.</remarks>
        public IsolatedQueueFileBacked(string path, string filePrefix, int maxInMemorySize, int individualFileSize)
        {
            m_fileIO = new FileIO(path, filePrefix);
            m_isFileMode = (m_fileIO.FileCount > 0);

            m_elementsPerNode = 1024;
            m_pooledNodes = new ResourceQueue<IsolatedNode<T>>(() => new IsolatedNode<T>(m_elementsPerNode), 2, 10);
            m_inboundQueue = new ContinuousQueue<IsolatedNode<T>>();
            m_outboundQueue = new ContinuousQueue<IsolatedNode<T>>();
            m_workerFlushToFile = new ScheduledTask(OnWorkerFlushToFileDoWork, OnWorkerFlushToFileCleanupWork);
            m_syncRoot = new object();
            T value = default(T);
            m_maxNodeCount = maxInMemorySize / value.InMemorySize / m_elementsPerNode;
            m_nodesPerFile = individualFileSize / value.OnDiskSize / m_elementsPerNode;

            m_nodesPerFile = Math.Max(m_nodesPerFile, 10);
            m_maxNodeCount = Math.Max(m_maxNodeCount, m_nodesPerFile);
        }

        /// <summary>
        /// Gets if this object has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// Does the writes to the archive file.
        /// </summary>
        void OnWorkerFlushToFileDoWork()
        {
            while (true)
            {
                IsolatedNode<T>[] nodesToWrite;
                lock (m_syncRoot)
                {
                    if (!ShouldFlushToFile())
                        return;

                    m_isFileMode = true;
                    nodesToWrite = m_inboundQueue.Dequeue(m_nodesPerFile);
                    m_currentlyWritingFile = true;
                }
                m_fileIO.DumpToDisk(nodesToWrite);
                lock (m_syncRoot)
                {
                    m_currentlyWritingFile = false;
                }
            }

        }

        bool ShouldFlushToFile()
        {
            if (m_isFileMode)
            {
                return m_inboundQueue.Count > m_nodesPerFile;
            }
            return m_inboundQueue.Count > m_maxNodeCount;
        }

        void OnWorkerFlushToFileCleanupWork()
        {
            while (m_inboundQueue.Count >= m_nodesPerFile)
            {
                IsolatedNode<T>[] nodesToWrite = m_inboundQueue.Dequeue(m_nodesPerFile);
                m_fileIO.DumpToDisk(nodesToWrite);
            }
            if (m_inboundQueue.Count > 0)
            {
                IsolatedNode<T>[] nodesToWrite = m_inboundQueue.Dequeue(m_inboundQueue.Count);
                m_fileIO.DumpToDisk(nodesToWrite);
            }

            if (m_currentTail != null && m_currentTail.Count > 0)
            {
                m_outboundQueue.AddToTail(m_currentTail);
            }
            if (m_outboundQueue.Count > 0)
            {
                IsolatedNode<T>[] nodesToWrite = m_outboundQueue.Dequeue(m_outboundQueue.Count);
                m_fileIO.DumpToDisk(nodesToWrite, false);
            }



        }

        /// <summary>
        /// Writes data to the queue. Will not block
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            if (m_disposing)
                throw new ObjectDisposedException(GetType().FullName);

            if (m_currentHead == null || m_currentHead.IsHeadFull)
            {
                //This can be done without a lock since it will be checked in the worker.
                if (ShouldFlushToFile())
                {
                    m_workerFlushToFile.Start();
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
            if (m_disposing)
                throw new ObjectDisposedException(GetType().FullName);

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
                bool repeat = true;
                bool success = false; //initialization not necessary. Just trying to get rid of a compilier warning.

                while (repeat)
                {
                    repeat = false;
                    bool readFromDisk = false;
                    lock (m_syncRoot)
                    {
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
                                    if (m_currentlyWritingFile)
                                    {
                                        m_currentTail = null;
                                        item = default(T);
                                        return false;
                                    }
                                    m_isFileMode = false;
                                }
                                else
                                {
                                    readFromDisk = true;
                                }
                                repeat = true;
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
                    if (readFromDisk)
                    {
                        m_fileIO.ReadFromDisk(m_outboundQueue, () => m_pooledNodes.Dequeue());
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
                return m_inboundQueue.Count * (long)m_elementsPerNode;
            }
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposing = true;

                m_workerFlushToFile.Dispose();

                m_disposed = true;
            }
        }
    }
}
