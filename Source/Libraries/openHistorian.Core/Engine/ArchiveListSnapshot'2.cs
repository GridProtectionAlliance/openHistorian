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

namespace openHistorian.Engine
{
    /// <summary>
    /// Provides a list of resources that each system transaction could be using.
    /// </summary>
    public class ArchiveListSnapshot<TKey, TValue> : IDisposable
        where TKey : class, new()
        where TValue : class, new()
    {
        private bool m_disposed;

        /// <summary>
        /// A callback to tell <see cref="ArchiveList"/> when resources are no longer being used.
        /// </summary>
        private Action<ArchiveListSnapshot<TKey, TValue>> m_onDisposed;

        /// <summary>
        /// A callback to get the latest list of resources from <see cref="ArchiveList"/>.
        /// </summary>
        private Action<ArchiveListSnapshot<TKey, TValue>> m_acquireResources;

        /// <summary>
        /// For future use. Will allow removing certain resources from the client. 
        /// </summary>
        public object ClientConnection;

        /// <summary>
        /// Contains an array of all of the resources currently used by this transaction.
        /// This field can be null or any element of this array can also be null.
        /// </summary>
        private ArchiveTableSummary<TKey, TValue>[] m_tables;


        public ArchiveListSnapshot(Action<ArchiveListSnapshot<TKey, TValue>> onDisposed, Action<ArchiveListSnapshot<TKey, TValue>> acquireResources)
        {
            m_onDisposed = onDisposed;
            m_acquireResources = acquireResources;
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
                m_tables = value;
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
                if (m_onDisposed != null)
                    m_onDisposed.Invoke(this);
                m_onDisposed = null;
                m_acquireResources = null;
                m_disposed = true;
            }
        }

        /// <summary>
        /// Requests from <see cref="ArchiveList"/> that this snapshot get updated.
        /// </summary>
        public void UpdateSnapshot()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_acquireResources.Invoke(this);
        }
    }
}