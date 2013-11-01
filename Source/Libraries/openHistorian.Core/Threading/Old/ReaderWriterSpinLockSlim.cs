////******************************************************************************************************
////  ReaderWriterSpinLockSlim.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  1/5/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////*****************************************************************************************************

//using System;
//using System.Threading;

//namespace GSF.Threading
//{
//    /// <summary>
//    /// Represents a fast, lightweight reader/writer lock that uses spinning to perform locking with no thread affinity. No recursive
//    /// acquires or upgradable locks are allowed (i.e., all entered locks must be exited before entering another lock).
//    /// </summary>
//    /// <remarks>
//    /// <para>
//    /// <see cref="ReaderWriterSpinLockSlim"/> uses a single word of memory and only spins when contention arises so no events are necessary.
//    /// </para>
//    /// <para>
//    /// This reader/writer lock uses <see cref="SpinWait"/> to spin the CPU instead of engaging event based locking. As a result it
//    /// should only be used in cases where lock times are expected to be very small, reads are very frequent and writes are rare.
//    /// If hold times for write locks can be lengthy, it will be better to use <see cref="ReaderWriterLockSlim"/> instead to avoid
//    /// unnecessary CPU utilization due to spinning incurred by waiting reads.
//    /// </para>
//    /// </remarks>
//    public class ReaderWriterSpinLockSlim
//    {
//        #region [ Members ]

//        // Fields
//        private volatile int m_readers;
//        private volatile int m_writer;

//        #endregion

//        #region [ Methods ]

//        /// <summary>
//        /// Enters the lock in write mode.
//        /// </summary>
//        /// <remarks>
//        /// Upon successful aquistion of a write lock, use the <c>finally</c> block of a <c>try/finally</c> statement to call <see cref="ExitWriteLock"/>.
//        /// One <see cref="ExitWriteLock"/> should be called for each <see cref="EnterWriteLock"/>.
//        /// </remarks>
//        public void EnterWriteLock()
//        {
//            SpinWait sw = new SpinWait();

//            while (true)
//            {
//                if (m_writer == 0 && Interlocked.Exchange(ref m_writer, 1) == 0)
//                {
//                    // We now hold the write lock, and prevent new readers -
//                    // but we must ensure no readers exist before proceeding.

//                    while (m_readers != 0)
//                        sw.SpinOnce();

//                    break;
//                }

//                // We failed to take the write lock; wait a bit and retry.
//                sw.SpinOnce();
//            }
//        }

//        /// <summary>
//        /// Exits write mode.
//        /// </summary>
//        public void ExitWriteLock()
//        {
//            // No need for a CAS.
//            m_writer = 0;
//        }

//        /// <summary>
//        /// Enters the lock in read mode.
//        /// </summary>
//        /// <remarks>
//        /// Upon successful aquistion of a read lock, use the <c>finally</c> block of a <c>try/finally</c> statement to call <see cref="ExitReadLock"/>.
//        /// One <see cref="ExitReadLock"/> should be called for each <see cref="EnterReadLock"/>.
//        /// </remarks>
//        public void EnterReadLock()
//        {
//            SpinWait sw = new SpinWait();

//            // Wait until there are no writers.
//            while (true)
//            {
//                while (m_writer == 1)
//                    sw.SpinOnce();

//                // Try to take the read lock.
//                Interlocked.Increment(ref m_readers);

//                if (m_writer == 0)
//                {
//                    // Success, no writer, proceed.
//                    break;
//                }

//                // Back off, to let the writer go through.
//                Interlocked.Decrement(ref m_readers);
//            }
//        }

//        /// <summary>
//        /// Exits read mode.
//        /// </summary>
//        /// <exception cref="InvalidOperationException">Cannot exit read lock when there are no readers.</exception>
//        public void ExitReadLock()
//        {
//            // Just note that the current reader has left the lock.
//            Interlocked.Decrement(ref m_readers);
//        }

//        #endregion
//    }
//}