//******************************************************************************************************
//  ArchiveListSnapshot.cs - Gbtc
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
//  5/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine
{
    /// <summary>
    /// Provides a list of resources that each system transaction could be using.
    /// </summary>
    public class ArchiveListSnapshot<TKey, TValue> : IDisposable
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        /// <summary>
        /// Signals that a disposal of this object has been requested. 
        /// </summary>
        /// <remarks>
        /// A race condition exists such that this class gets a dispose request before the client
        /// registers this event. Therefore, be sure to check <see cref="IsDisposeRequested"/>
        /// after assigning the event handler.
        /// </remarks>
        public event Action DisposeRequested;

        object m_syncDisposing;

        private ManualResetEvent m_connectionDisposed;

        private bool m_disposed;

        /// <summary>
        /// A callback to tell <see cref="ArchiveList{TKey,TValue}"/> when resources are no longer being used.
        /// </summary>
        private Action<ArchiveListSnapshot<TKey, TValue>> m_onDisposed;

        /// <summary>
        /// A callback to get the latest list of resources from <see cref="ArchiveList{TKey,TValue}"/>.
        /// </summary>
        private Action<ArchiveListSnapshot<TKey, TValue>> m_acquireResources;

        bool m_isDisposeRequested;

        /// <summary>
        /// Contains an array of all of the resources currently used by this transaction.
        /// This field can be null or any element of this array can also be null.
        /// </summary>
        private ArchiveTableSummary<TKey, TValue>[] m_tables;

        /// <summary>
        /// Creates an <see cref="ArchiveListSnapshot{TKey,TValue}"/>.
        /// </summary>
        /// <param name="onDisposed"></param>
        /// <param name="acquireResources"></param>
        public ArchiveListSnapshot(Action<ArchiveListSnapshot<TKey, TValue>> onDisposed, Action<ArchiveListSnapshot<TKey, TValue>> acquireResources)
        {
            m_syncDisposing = new object();
            m_onDisposed = onDisposed;
            m_acquireResources = acquireResources;
            m_tables = new ArchiveTableSummary<TKey, TValue>[0];
            m_connectionDisposed = new ManualResetEvent(false);
        }

        /// <summary>
        /// Gets the list of all partitions that are currently in use.  Set partition to null to indicate
        /// that is is no longer needed.  Set the entire array to null to release all partitions.
        /// </summary>
        public ArchiveTableSummary<TKey, TValue>[] Tables
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_tables;
            }
            set
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (value == null)
                    m_tables = new ArchiveTableSummary<TKey, TValue>[0];
                m_tables = value;
            }
        }

        /// <summary>
        /// Attempts to get the file for the provided fileId
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public ArchiveTableSummary<TKey, TValue> TryGetFile(Guid fileId)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            foreach (var table in m_tables)
            {
                if (table.FileId == fileId)
                    return table;
            }
            return null;
        }

        /// <summary>
        /// Gets if the engine is requesting that this snapshot gets disposed.
        /// if this is true this means the engine is waiting for the release
        /// of this object before it can continue its next task.
        /// </summary>
        public bool IsDisposeRequested
        {
            get
            {
                return m_isDisposeRequested;
            }
        }

        /// <summary>
        /// Gets if this class has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// Disposes this class, releasing all resource locks.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_connectionDisposed.Set();
                lock (m_syncDisposing)
                {
                    if (m_connectionDisposed != null)
                    {
                        m_connectionDisposed.Dispose();
                        m_connectionDisposed = null;
                    }
                }

                if (m_onDisposed != null)
                    m_onDisposed.Invoke(this);
                m_onDisposed = null;
                m_acquireResources = null;
                m_disposed = true;
            }
        }

        /// <summary>
        /// Requests from <see cref="ArchiveList{TKey,TValue}"/> that this snapshot get updated.
        /// </summary>
        public void UpdateSnapshot()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_acquireResources.Invoke(this);
        }

        internal void Engine_BeginDropConnection()
        {
            m_isDisposeRequested = true;
            Thread.MemoryBarrier();
            if (DisposeRequested != null)
                DisposeRequested();
        }

        internal void Engine_EndDropConnection()
        {
            lock (m_syncDisposing)
            {
                if (m_connectionDisposed != null)
                {
                    m_connectionDisposed.WaitOne();
                    m_connectionDisposed.Dispose();
                    m_connectionDisposed = null;
                }
            }
        }
    }
}