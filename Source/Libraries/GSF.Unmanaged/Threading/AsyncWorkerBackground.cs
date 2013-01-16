////******************************************************************************************************
////  AsyncWorkerBackground.cs - Gbtc
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
////  1/12/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Threading;

//namespace GSF.Threading
//{
//    /// <summary>
//    /// Provides a lightweight way to schedule 
//    /// work on a seperate thread that is gaurenteed to run
//    /// at least once after calling the Run method.
//    /// </summary>
//    internal class AsyncWorkerBackground : IAsyncWorker
//    {

//        /// <summary>
//        /// Event occurs on a seperate thread and will be repeatedly
//        /// called evertime <see cref="RunWorker"/> is called unless 
//        /// <see cref="Dispose"/> is called first.
//        /// </summary>
//        public event EventHandler<AsyncWorkerEventArgs> DoWork;

//        /// <summary>
//        /// Event occurs only once on a seperate thread
//        /// when this class is disposed.
//        /// </summary>
//        public event EventHandler CleanupWork;

//        volatile bool m_disposed;
//        volatile bool m_disposing;

//        ManualResetEvent m_doneCleaningUp;

//        ScheduledTask m_task;

//        static class State
//        {
//            /// <summary>
//            /// Means that there is a thread that is actively running the process.
//            /// This state can be exchanged with IsWaiting or ShouldRunAgain.
//            /// </summary>
//            public const int IsRunning = 0;

//            public const int NotRunning = 1;

//            /// <summary>
//            /// Means the worker thread has entered a wait state. And has not exited it yet.
//            /// This state can only be chaged to IsRunning by the worker thread.
//            /// </summary>
//            public const int IsWaiting = 2;

//            /// <summary>
//            /// This means that the non-worker thread has requested that the worker thread run 
//            /// at least one more time.
//            /// This state can only be set by non-worker threads, but can be set to IsRunning by the worker thread
//            /// </summary>
//            public const int ShouldRunAgain = 3;
//        }

//        /// <summary>
//        /// Value is of type CurrentState but an integer so Interlocked.CompareExchange can occur.
//        /// </summary>
//        int m_state;

//        public AsyncWorkerBackground()
//        {
//            m_doneCleaningUp = new ManualResetEvent(false);
//            m_state = State.IsRunning;
//            m_task = new ScheduledTask(RunWorkerThread);
//        }

//        /// <summary>
//        /// Gets if the class has been disposed;
//        /// </summary>
//        public bool IsDisposed
//        {
//            get
//            {
//                return m_disposed;
//            }
//        }

//        /// <summary>
//        /// Gets if this class is no longer going to support
//        /// running again. Check <see cref="IsDisposed"/> to see
//        /// if the class has been completely disposed.
//        /// </summary>
//        public bool IsDisposing
//        {
//            get
//            {
//                return m_disposing;
//            }
//        }

//        /// <summary>
//        /// Makes sure that the process begins immediately if it is currently not running.
//        /// If it is running, it tells the process to run at least one more time. 
//        /// </summary>
//        public void RunWorker()
//        {
//           //If the class is already told to run again at this point
//            //it is garenteed that this function will run at least one more time.
//            //and can thus quit early.
//            Thread.MemoryBarrier();
//            if (m_state == State.ShouldRunAgain)
//                return;
            
//            m_task.Start();
//        }

      

//        void RunWorkerThread()
//        {
//            int state;

//        RunAgain:
//            if (m_disposing)
//            {
//                if (CleanupWork != null)
//                    CleanupWork(this, EventArgs.Empty);
//                m_doneCleaningUp.Set();
//            }

//            Thread.MemoryBarrier();
//            m_state = State.IsRunning;
//            Thread.MemoryBarrier();

//            var args = new AsyncWorkerEventArgs();
//            if (DoWork != null)
//                DoWork(this, args);

//            Thread.MemoryBarrier();

//            if (m_state == State.ShouldRunAgain || args.ShouldRunAgainImmediately || args.WaitHandleTime == 0)
//            {
//                goto RunAgain;
//            }

//            if (args.ShouldRunAgainAfterDelay && args.WaitHandleTime > 1)
//            {
//                state = Interlocked.CompareExchange(ref m_state, State.IsWaiting, State.IsRunning);
//                if (state == State.IsRunning) //This means the exchange was a success
//                {
//                    m_task.Start(args.WaitHandleTime);
//                    return;
//                }
//                else //occurs when told to run again
//                {
//                    goto RunAgain;
//                }
//            }

//            state = Interlocked.CompareExchange(ref m_state, State.NotRunning, State.IsRunning);
//            if (state == State.IsRunning) //This means the exchange was a success
//            {
//                return;
//            }
//            else //occurs when told to run again
//            {
//                goto RunAgain;
//            }

//        }

//        /// <summary>
//        /// Stops future calls to this class and 
//        /// waits until the worker thread is finished.
//        /// </summary>
//        public void Dispose()
//        {
//            if (!m_disposing)
//            {
//                m_disposing = true;
//                RunWorker();
//                m_doneCleaningUp.WaitOne();
//                m_disposed = true;
//            }
//        }
//        public void FinalizedDisposed()
//        {
//            //ToDO: something different for FinalizedDisposed();
//            Dispose();
//        }

//        bool TryChangeState(int prevState, int nextState)
//        {
//            Thread.MemoryBarrier();
//            return m_state == prevState &&
//                Interlocked.CompareExchange(ref m_state, nextState, prevState) == prevState;
//        }
//    }
//}
