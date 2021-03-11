//******************************************************************************************************
//  TinyLock.cs - Gbtc
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
//  3/7/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Provides a light weight exclusive lock that is approximately 2.5 times faster than <see cref="Monitor"/>.
    /// WARNING: This lock should be used in a Using block, and duplicate calls to Lock without releasing will cause a deadlock.
    /// </summary>
    /// <remarks>
    /// After writing this class I did some review of the methodology. 
    /// Reviewing this article: http://www.adammil.net/blog/v111_Creating_High-Performance_Locks_and_Lock-free_Code_for_NET_.html
    /// Brings up stability issues with the lock. Namely what happens when unhandled exceptions occurs when acquiring and releasing the lock.
    /// I have intentionally left out any kind of protection against this as it severly reduces the speed of this code. Therefore
    /// do not use this locking method where a Thread.Abort() might be used as a control method. 
    /// </remarks>
    public class HalfLock
    {
        private const int Unlocked = 0;
        private const int Locked = 1;
        private int m_lock;
        private readonly HalfLockRelease m_release;

        /// <summary>
        /// Creates a <see cref="TinyLock"/>
        /// </summary>
        public HalfLock()
        {
            m_lock = Unlocked;
            m_release = new HalfLockRelease(this);
        }

        /// <summary>
        /// Acquires an exclusive lock on this class. Place call in a using block.
        /// Duplicate calls to this within the same thread will cause a deadlock.
        /// </summary>
        /// <returns>A structure that will release the lock. 
        /// This struct will always be the exact same value. Therefore it can be 
        /// stored once if desired, however, be careful when using it this way as inproper synchronization
        /// will be easier to occur.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HalfLockRelease Lock()
        {
            if (Interlocked.Exchange(ref m_lock, Locked) != Unlocked) //If I successfully changed the state from unlocked to locked, then I now acquire the lock.
                LockSlower();
            return m_release;
        }

        /// <summary>
        /// A nested call since 99% of the time, there will not be contention. This prevents stack space being
        /// used for the SpinLock when its not needed.
        /// </summary>
        /// <returns></returns>
        private void LockSlower()
        {
            SpinWait spin = default;
            while (Interlocked.Exchange(ref m_lock, Locked) != Unlocked)
                spin.SpinOnce();
        }

        /// <summary>
        /// A structure that will allow releasing of a lock. This is returned by <see cref="Lock"/>.
        /// </summary>
        public struct HalfLockRelease : IDisposable
        {
            private readonly HalfLock m_halfLock;
            internal HalfLockRelease(HalfLock halfLock)
            {
                if (halfLock is null)
                    throw new ArgumentNullException("halfLock");
                if (halfLock.m_release.m_halfLock != null)
                    throw new Exception("Object is already locked");
                m_halfLock = halfLock;
            }

            /// <summary>
            /// Releases an acquired lock.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                //A volatile write implies that even if this is inlined, the unlock will never be reordered above its current location.
                m_halfLock.m_lock = Unlocked;
            }
        }
    }
}
