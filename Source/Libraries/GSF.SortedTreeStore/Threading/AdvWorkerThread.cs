//******************************************************************************************************
//  AdvWorkerThread.cs - Gbtc
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
//  3/8/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Functions that might be executed by <see cref="ScheduledTask"/>. 
    /// This allows for multiple types of threads to be created, such as one that uses
    /// the threadpool, or one that uses a dedicated thread that is a foreground thread.
    /// 
    /// All calls to this class need to be properly coordinated.
    /// </summary>
    internal class AdvWorkerThread
    {
        static class State
        {
            public const int NotWorking = 0;
            public const int Running = 1;
            public const int PendingDelayedWork = 2;
            public const int PendingImmediateWork = 3;
            public const int Terminated = 4;
            public const int Undefined = 10;
        }

        volatile int m_stateMachine;
        private volatile int m_sleepTime;
        private volatile bool m_stopExecuting;
        private Thread m_thread;
        private WeakAction m_callback;
        private ManualResetEvent m_threadPausedWaitHandler;
        private ManualResetEvent m_threadDelayWaitHandler;

        /// <summary>
        /// Initializes a <see cref="DedicatedThread"/> that will execute the provided callback.
        /// </summary>
        /// <param name="callback">the callback method to execute when running</param>
        /// <param name="isBackground">determines if the thread is supposed to be a background thread</param>
        /// <param name="priority">specifies the priority of the thread</param>
        public AdvWorkerThread(Action callback, bool isBackground, ThreadPriority priority)
        {
            m_sleepTime = 0;
            m_callback = new WeakAction(callback);
            m_threadPausedWaitHandler = new ManualResetEvent(false);
            m_threadDelayWaitHandler = new ManualResetEvent(false);
            m_thread = new Thread(WorkerThreadLoop);
            m_thread.IsBackground = isBackground;
            m_thread.Priority = priority;
            m_thread.Start();
        }

        /// <summary>
        /// The internal loop that holds the thread.
        /// </summary>
        void WorkerThreadLoop()
        {
            while (true)
            {
                m_threadPausedWaitHandler.WaitOne(-1);

                if (m_stopExecuting)
                {
                    m_threadPausedWaitHandler.Dispose();
                    m_threadPausedWaitHandler = null;
                    m_threadDelayWaitHandler.Dispose();
                    m_threadDelayWaitHandler = null;
                    m_callback = null;
                    m_thread = null;
                    return;
                }

                if (m_sleepTime != 0)
                    m_threadDelayWaitHandler.WaitOne(m_sleepTime);

                WorkingState();
            }
        }

        /// <summary>
        /// Requests that the callback executes immediately.
        /// </summary>
        public void Start()
        {
            SpinWait wait = default(SpinWait);
            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotWorking:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Undefined, State.NotWorking) == State.NotWorking)
                        {
                            m_sleepTime = 0;
                            m_threadPausedWaitHandler.Set();
                            m_stateMachine = State.PendingImmediateWork;
                            return;
                        }
                        break;
                    case State.PendingDelayedWork:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Undefined, State.PendingDelayedWork) == State.PendingDelayedWork)
                        {
                            m_threadDelayWaitHandler.Set();
                            m_stateMachine = State.PendingImmediateWork;
                            return;
                        }
                        break;
                    case State.Running:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingImmediateWork, State.Running) == State.Running)
                        {
                            return;
                        }
                        break;
                    case State.PendingImmediateWork:
                        return;
                    case State.Terminated:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Terminated, State.Terminated) == State.Terminated)
                        {
                            throw new Exception("Cannot call Start after the thread worker has been terminated.");
                        }
                        break;
                    case State.Undefined:
                        break;
                }
                wait.SpinOnce();
            }
        }

        /// <summary>
        /// Requests that the callback executes after the specified interval in milliseconds.
        /// </summary>
        /// <param name="delay">the delay in milliseconds</param>
        public void Start(int delay)
        {
            SpinWait wait = new SpinWait();

            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotWorking:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Undefined, State.NotWorking) == State.NotWorking)
                        {
                            m_sleepTime = delay;
                            m_threadPausedWaitHandler.Set();
                            m_stateMachine = State.PendingDelayedWork;
                            return;
                        }
                        break;
                    case State.Running:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Undefined, State.Running) == State.Running)
                        {
                            m_threadPausedWaitHandler.Reset();
                            m_threadDelayWaitHandler.Reset();
                            m_sleepTime = 0;
                            m_sleepTime = delay;
                            m_stateMachine = State.PendingDelayedWork;
                            return;
                        }
                        break;
                    case State.PendingDelayedWork:
                    case State.PendingImmediateWork:
                    case State.Terminated:
                        return;
                    case State.Undefined:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Terminated, State.Terminated) == State.Terminated)
                        {
                            throw new Exception("Cannot call Start after the thread worker has been terminated.");
                        }
                        break;
                }
                wait.SpinOnce();
            }
        }

        /// <summary>
        /// The method executed in the state: Running. This is invoked by the 
        /// thread when it is time to run this state.
        /// </summary>
        void WorkingState()
        {
            SpinWait wait = new SpinWait();

            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.PendingImmediateWork:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Running, State.PendingImmediateWork) == State.PendingImmediateWork)
                        {
                            m_callback.TryInvoke();
                        }
                        break;
                    case State.PendingDelayedWork:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Running, State.PendingDelayedWork) == State.PendingDelayedWork)
                        {
                            m_callback.TryInvoke();
                        }
                        break;
                    case State.Running:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Undefined, State.Running) == State.Running)
                        {
                            m_threadPausedWaitHandler.Reset();
                            m_threadDelayWaitHandler.Reset();
                            m_sleepTime = 0;
                            m_stateMachine = State.NotWorking;
                            return;
                        }
                        break;
                    case State.Terminated:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Undefined, State.Terminated) == State.Terminated)
                        {
                            throw new Exception("InvalidState");
                        }
                        break;
                    case State.NotWorking:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Undefined, State.NotWorking) == State.NotWorking)
                        {
                            throw new Exception("InvalidState");
                        }
                        break;
                    case State.Undefined:
                        break;
                }
                wait.SpinOnce();
            }
        }

        /// <summary>
        /// Gracefully stops the execution of the custom thread. 
        /// Similiar to Dispose, except, this action must also be properly coordinated.
        /// </summary>
        public void Dispose()
        {
            SpinWait wait = new SpinWait();
            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotWorking:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Undefined, State.NotWorking) == State.NotWorking)
                        {
                            m_stopExecuting = true;
                            m_threadPausedWaitHandler.Set();
                            m_stateMachine = State.Terminated;
                            m_callback.TryInvoke();
                            return;
                        }
                        break;
                    case State.Terminated:
                        return;
                    case State.PendingDelayedWork:
                    case State.PendingImmediateWork:
                    case State.Running:
                        throw new Exception("Invalid State");
                        return;
                    case State.Undefined:
                        break;

                }
                wait.SpinOnce();
            }
        }
    }
}