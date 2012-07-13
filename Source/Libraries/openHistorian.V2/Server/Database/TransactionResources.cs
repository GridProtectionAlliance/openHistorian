//******************************************************************************************************
//  TransactionResources.cs - Gbtc
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
//  5/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Provides a list of resources that each system transaction could be using.
    /// </summary>
    class TransactionResources : IDisposable
    {
        bool m_disposed;
        Action<TransactionResources> m_onDisposed;
        Action<TransactionResources> m_acquireResources;

        public object ClientConnection;

        /// <summary>
        /// Contains an array of all of the resources currently used by this transaction.
        /// This field can be null or any element of this array can also be null.
        /// </summary>
        PartitionSummary[] m_tables;

        public TransactionResources(Action<TransactionResources> onDisposed, Action<TransactionResources> acquireResources)
        {
            m_onDisposed = onDisposed;
            m_acquireResources = acquireResources;
        }
        
        public PartitionSummary[] Tables
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

        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

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

        public void RequestResourceUpdate()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_acquireResources.Invoke(this);
        }

    }
}
