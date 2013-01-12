//******************************************************************************************************
//  AsyncWorker.cs - Gbtc
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
//  12/26/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Provides a lightweight way to schedule 
    /// work on a seperate thread that is gaurenteed to run
    /// at least once after calling the Run method.
    /// </summary>
    public class AsyncWorker : IDisposable
    {
        /// <summary>
        /// Event occurs on a seperate thread and will be repeatedly
        /// called evertime <see cref="RunWorker"/> or 
        /// <see cref="RunWorkerAfterDelay"/> is called unless 
        /// <see cref="Dispose"/> is called first.
        /// </summary>
        public event EventHandler DoWork;

        /// <summary>
        /// Event occurs only once on a seperate thread
        /// when this class is disposed.
        /// </summary>
        public event EventHandler CleanupWork;

        int m_runCount;
        bool m_disposed;
        bool m_disposing;
        bool m_runAgain;
        bool m_delayRequested;
        CurrentState m_state;
        object m_syncRoot;
        ManualResetEvent m_runAgainEvent;
        ManualResetEvent m_doneCleaningUp;
        RegisteredWaitHandle m_registeredHandle;
        TimeSpan m_timeDelay;

        enum CurrentState
        {
            /// <summary>
            /// Means that there is a thread that is actively running the process.
            /// </summary>
            IsRunning,
            /// <summary>
            /// Means that there has been a thread assigned to execute this process, however, it is waiting 
            /// on the thread pool for a signal or a timeout.
            /// </summary>
            IsWaiting,
            /// <summary>
            /// Means that no threads are currently processing the user's request.
            /// </summary>
            IsDoingNothing
        }

        public AsyncWorker()
        {
            m_delayRequested = false;
            m_runAgain = false;
            m_state = CurrentState.IsDoingNothing;
            m_syncRoot = new object();
            m_runAgainEvent = new ManualResetEvent(false);
            m_doneCleaningUp = new ManualResetEvent(false);
        }

        /// <summary>
        /// Gets if the class has been disposed;
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        /// <summary>
        /// Gets if this class is no longer going to support
        /// running again. Check <see cref="IsDisposed"/> to see
        /// if the class has been completely disposed.
        /// </summary>
        public bool IsDisposing
        {
            get
            {
                return m_disposing;
            }
        }

        /// <summary>
        /// Makes sure that the process begins immediately if it is currently not running.
        /// If it is running, it tells the process to run at least one more time. 
        /// </summary>
        public void RunWorker()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            //If the class is already told to run again at this point
            //it is garenteed that this function will run at least one more time.
            //and can thus quit early.
            Thread.MemoryBarrier();
            if (m_runAgain)
                return;

            lock (m_syncRoot)
            {
                if (m_disposing)
                    return;

                switch (m_state)
                {
                    case CurrentState.IsDoingNothing:
                        ThreadPool.QueueUserWorkItem(BeginRun, null);
                        m_state = CurrentState.IsRunning;
                        break;
                    case CurrentState.IsRunning:
                        m_runAgain = true;
                        break;
                    case CurrentState.IsWaiting:
                        m_runAgainEvent.Set();
                        m_state = CurrentState.IsRunning;
                        break;
                }
            }
        }

        /// <summary>
        /// Makes sure that the process begins after the specified delay unless 
        /// it is signaled early. If it is currently waiting, this will not
        /// modify the wait time. Instead it will do nothing.
        /// </summary>
        public void RunWorkerAfterDelay(TimeSpan delay)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            //If the class is already told to run again at this point
            //it is garenteed that this function will run at least one more time.
            //and can thus quit early.
            Thread.MemoryBarrier();
            if (m_runAgain)
                return;

            lock (m_syncRoot)
            {
                if (m_disposing)
                    return;

                switch (m_state)
                {
                    case CurrentState.IsDoingNothing:
                        m_runAgainEvent.Reset();
                        m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_runAgainEvent, BeginRunOnTimer, null, delay, true);
                        m_state = CurrentState.IsWaiting;
                        break;
                    case CurrentState.IsRunning:
                        m_delayRequested = true;
                        m_timeDelay = delay;
                        break;
                    case CurrentState.IsWaiting:
                        break;
                }
            }
        }

        void BeginRunOnTimer(object state, bool isTimeout)
        {
            //Locking again may not be required. However, a race condition does exist in
            //the unlikely event that this code is executed before m_registeredHandle has been assigned.
            lock (m_syncRoot)
            {
                m_registeredHandle.Unregister(null);
            }
            BeginRunInternal();
        }

        void BeginRun(object state)
        {
            BeginRunInternal();
        }

        void BeginRunInternal()
        {
            if (m_runCount != 0)
                throw new Exception("Internal Error, only one async worker should be running at a time.");
            m_runCount++;
            
            while (true)
            {
                bool shouldQuit = false;
                lock (m_syncRoot)
                {
                    if (m_disposing)
                    {
                        m_state = CurrentState.IsDoingNothing;
                        shouldQuit = true;
                    }
                    else
                    {
                        Thread.MemoryBarrier();
                        m_state = CurrentState.IsRunning;
                        m_runAgain = false;
                        Thread.MemoryBarrier();
                        m_delayRequested = false;
                    }
                }

                if (shouldQuit)
                {
                    if (CleanupWork != null)
                        CleanupWork(this, EventArgs.Empty);

                    m_doneCleaningUp.Set();
                    m_runCount--;
                    return;
                }

                if (DoWork != null)
                    DoWork(this, EventArgs.Empty);

                lock (m_syncRoot)
                {
                    if (!m_disposing)
                    {
                        if (!m_runAgain)
                        {
                            if (m_delayRequested)
                            {
                                m_runAgainEvent.Reset();
                                m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_runAgainEvent, BeginRunOnTimer, null, m_timeDelay, true);
                                m_state = CurrentState.IsWaiting;
                                m_runCount--;
                                return;
                            }
                            else
                            {
                                m_state = CurrentState.IsDoingNothing;
                                m_runCount--;
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stops future calls to this class and 
        /// waits until the worker thread is finished.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                bool callCleanupFromCurrentThread=false;
                lock (m_syncRoot)
                {
                    m_disposing = true;
                    switch (m_state)
                    {
                        case CurrentState.IsDoingNothing:
                            callCleanupFromCurrentThread = true;
                            m_state = CurrentState.IsRunning;
                            break;
                        case CurrentState.IsRunning:
                            m_runAgain = true;
                            break;
                        case CurrentState.IsWaiting:
                            m_runAgainEvent.Set();
                            m_state = CurrentState.IsRunning;
                            break;
                    }
                }
                if (callCleanupFromCurrentThread)
                {
                    if (CleanupWork != null)
                        CleanupWork(this, EventArgs.Empty);
                    m_doneCleaningUp.Set();
                }
                else
                {
                    m_doneCleaningUp.WaitOne();
                }
                m_disposed = true;
            }
        }
    }
}
