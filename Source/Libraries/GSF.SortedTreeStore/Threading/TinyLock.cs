//******************************************************************************************************
//  TinyLock.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  3/7/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Provides a light weight exclusive lock that is approximately 2.5 times faster than <see cref="Monitor"/>.
    /// WARNING: This lock should be used in a Using block, and duplicate calls to Lock without releasing will cause a deadlock.
    /// </summary>
    public class TinyLock
    {
        const int Unlocked = 0;
        const int Locked = 1;
        volatile int m_lock;
        readonly TinyLockRelease m_release;

        /// <summary>
        /// Creates a <see cref="TinyLock"/>
        /// </summary>
        public TinyLock()
        {
            m_lock = Unlocked;
            m_release = new TinyLockRelease(this);
        }

        /// <summary>
        /// Acquires an exclusive lock on this class. Place call in a using block.
        /// Duplicate calls to this within the same thread will cause a deadlock.
        /// </summary>
        /// <returns>A structure that will release the lock.</returns>
        public TinyLockRelease Lock()
        {
            if (Interlocked.CompareExchange(ref m_lock, Locked, Unlocked) != Unlocked)
                return Lock2();
            return m_release;
        }
        /// <summary>
        /// A nested call since 99% of the time, there will not be contention. This prevents stack space being
        /// used for the SpinLock when its not needed.
        /// </summary>
        /// <returns></returns>
        TinyLockRelease Lock2()
        {
            SpinWait spin = default(SpinWait);
            while (Interlocked.CompareExchange(ref m_lock, Locked, Unlocked) != Unlocked)
                spin.SpinOnce();
            return m_release;
        }

        /// <summary>
        /// A structure that will allow releasing of a lock. This is returned by <see cref="Lock"/>.
        /// </summary>
        public struct TinyLockRelease : IDisposable
        {
            readonly TinyLock m_tinyLock;
            internal TinyLockRelease(TinyLock tinyLock)
            {
                if ((object)tinyLock == null)
                    throw new ArgumentNullException("tinyLock");
                if (tinyLock.m_release.m_tinyLock != null)
                    throw new Exception("Object is already locked");
                m_tinyLock = tinyLock;
            }

            /// <summary>
            /// Releases an acquired lock.
            /// </summary>
            public void Dispose()
            {
                //A volatile write implies that even if this is inlined, the unlock will never be reordered above its current location.
                m_tinyLock.m_lock = Unlocked;
            }
        }
    }
}
