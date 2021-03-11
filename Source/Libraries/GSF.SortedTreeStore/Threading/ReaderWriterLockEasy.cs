//******************************************************************************************************
//  ReaderWriterLockEasy.cs - Gbtc
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
//  10/09/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// A read lock object
    /// </summary>
    public struct DisposableReadLock
        : IDisposable
    {
        private ReaderWriterLock m_l;
        /// <summary>
        /// Creates a read lock
        /// </summary>
        /// <param name="l"></param>
        public DisposableReadLock(ReaderWriterLock l)
        {
            m_l = l;
            l.AcquireReaderLock(Timeout.Infinite);
        }

        public void Dispose()
        {
            if (m_l != null)
            {
                m_l.ReleaseReaderLock();
                m_l = null;
            }
        }
    }

    /// <summary>
    /// A read lock object
    /// </summary>
    public struct DisposableWriteLock
        : IDisposable
    {
        private ReaderWriterLock m_l;
        /// <summary>
        /// Creates a read lock
        /// </summary>
        /// <param name="l"></param>
        public DisposableWriteLock(ReaderWriterLock l)
        {
            m_l = l;
            l.AcquireWriterLock(Timeout.Infinite);
        }

        public void Dispose()
        {
            if (m_l != null)
            {
                m_l.ReleaseWriterLock();
                m_l = null;
            }
        }
    }

    /// <summary>
    /// A simplified implementation of a <see cref="ReaderWriterLockSlim"/>. This allows for more 
    /// user friendly code to be written.
    /// </summary>
    public class ReaderWriterLockEasy
    {
        private readonly ReaderWriterLock m_lock = new ReaderWriterLock();

        /// <summary>
        /// Enters a read lock. Be sure to call within a using block.
        /// </summary>
        /// <returns></returns>
        public DisposableReadLock EnterReadLock()
        {
            return new DisposableReadLock(m_lock);
        }

        /// <summary>
        /// Enters a write lock. Be sure to call within a using block.
        /// </summary>
        /// <returns></returns>
        public DisposableWriteLock EnterWriteLock()
        {
            return new DisposableWriteLock(m_lock);
        }
    }
}
