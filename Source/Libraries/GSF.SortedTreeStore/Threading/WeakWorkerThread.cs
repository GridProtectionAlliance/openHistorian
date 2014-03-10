////******************************************************************************************************
////  WeakWorkerThread.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
////  3/8/2014 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Threading;

//namespace GSF.Threading
//{

//    /// <summary>
//    /// Callback method for the <see cref="WeakWorkerThread"/>.
//    /// </summary>
//    /// <param name="executionMode">the mode of execution that caused this event to raise.</param>
//    /// <param name="shouldDispose">Set to true if this worker should dispose after this call. Default state is always false.</param>
//    public delegate void WorkerThreadCallback(WorkerThreadTimeoutResults executionMode, ref bool shouldDispose);

//    /// <summary>
//    /// Metadata about why this worker was called.
//    /// </summary>
//    public enum WorkerThreadTimeoutResults
//    {
//        /// <summary>
//        /// A start with a delay was called initially, however, the full delay was
//        /// not observed as a Start without a delay was called.
//        /// </summary>
//        StartAfterPartialDelay,
//        /// <summary>
//        /// A start with a delay was called initially, and the full delay was observed.
//        /// </summary>
//        StartAfterFullDelay,
//        /// <summary>
//        /// A start without a delay was called.
//        /// </summary>
//        StartNow,
//        /// <summary>
//        /// Dispose was called and execution will terminate after this function call.
//        /// </summary>
//        Disposing
//    }

//    /// <summary>
//    /// A weak referenced <see cref="Thread"/> that will enter a pause state behind a weak reference
//    /// so it can be garbaged collected if it's in a sleep state.
//    /// </summary>
//    /// <remarks>
//    /// WeakThread functions in the specified way. 
//    /// Valid function calls are Start(), Start(delay), and Dispose().
//    /// 
//    /// Start(delay) will schedule the worker thread to run after the specified time interval. 
//    ///  Duplicate calls to this method will be ignored and only the first timer delay will be used.
//    ///  If a worker is already running, this method will queue for it to run again after it finishes
//    ///  with the provided delay. Therefore, the worker will always start sometime after this method.
//    ///
//    /// Start() will request that a worker start on the callback as soon as possible (usually the amount
//    ///  of time it takes to wake up a thread). If there was a delay specified by a prior call, the delay
//    ///  will be canceled and the callback will execute as soon as possible. If a worker is already running, 
//    ///  this method will queue for it to run again after it finishes. Therefore, the worker will always 
//    ///  start sometime after this method.
//    /// 
//    /// Dispose() will stop the execution of the worker as soon as possible. This means not allowing the 
//    /// disposal event to fire. It is not recommended to use this method, rather externally coordinating a proper
//    /// disposal by returning true to the shouldDispose parameter passed to the callback function.
//    /// 
//    /// </remarks>
//    public partial class WeakWorkerThread
//        : IDisposable
//    {
//        /// <summary>
//        /// State variables for the internal state machine.
//        /// </summary>
//        private static class State
//        {
//            /// <summary>
//            /// Indicates the task has been queue for immediate execution, but has not started running yet.
//            /// </summary>
//            public const int ScheduledToRun = 0;
//            /// <summary>
//            /// Indicates that the task is running, but has been requested to run again immediately after finishing.
//            /// </summary>
//            public const int RunAgain = 1;

//            /// <summary>
//            /// Indicates that the task is running, but has been requested to run again after a user specified interval.
//            /// </summary>
//            public const int RunAgainAfterDelay = 2;

//            /// <summary>
//            /// Indicates that the task is scheduled to execute after a user specified delay
//            /// </summary>
//            public const int ScheduledToRunAfterDelay = 3;

//            /// <summary>
//            /// Indicates that the task is not running.
//            /// </summary>
//            public const int NotRunning = 4;

//            /// <summary>
//            /// Indicates the task is currently running.
//            /// </summary>
//            public const int Running = 5;

//            /// <summary>
//            /// Indicates that the class has been Terminated and no more execution is possible.
//            /// </summary>
//            public const int Disposed = 7;

//            /// <summary>
//            /// A state to set the machine to when addional work needs to be done before finalizing the next state.
//            /// Never leave in this state, never change from this state unless you are the one who set this state.
//            /// </summary>
//            public const int PendingAction = 100;
//        }

//        /// <summary>
//        /// Used to prevent concurrent calls to the Dispose method.
//        /// </summary>
//        object m_disposingConcurrentCallLock;

//        volatile int m_stateMachine;

//        volatile int m_pendingDelay;

//        volatile WorkerThreadCallback m_callback;

//        //A reference of the delegate must be kept since ThreadContainer places this in a weak reference.
//        //Otherwise it will get collected since only references to the delegate will be strong references.
//        Func<WorkerThreadTimeoutResults, bool> m_containerCallback;

//        ThreadContainer m_thread;

//        /// <summary>
//        /// Creates a <see cref="WeakWorkerThread"/> that raises the provided event when invoked.
//        /// </summary>
//        /// <param name="callback">the back to raise with the </param>
//        /// <param name="isBackground">parameter to pass to the underlying thread</param>
//        /// <param name="priority">parameter to pass to the underlying thread</param>
//        public WeakWorkerThread(WorkerThreadCallback callback, bool isBackground, ThreadPriority priority)
//        {
//            m_stateMachine = State.NotRunning;
//            m_disposingConcurrentCallLock = new object();
//            m_callback = callback;
//            m_containerCallback = OnRunning;
//            m_thread = new ThreadContainer(m_containerCallback, isBackground, priority);
//        }

//        /// <summary>
//        /// Finalizer to dispose of the thread.
//        /// </summary>
//        ~WeakWorkerThread()
//        {
//            Dispose();
//        }

//        bool OnRunning(WorkerThreadTimeoutResults info)
//        {
//            bool shouldDispose;
//            SpinWait wait = new SpinWait();
//            while (true)
//            {
//                switch (m_stateMachine)
//                {
//                    case State.ScheduledToRun:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Running, State.ScheduledToRun) == State.ScheduledToRun)
//                        {
//                            OnCallback(info, out shouldDispose);
//                            if (shouldDispose)
//                            {
//                                OnCallback(WorkerThreadTimeoutResults.Disposing, out shouldDispose);
//                                m_stateMachine = State.Disposed;
//                                return false;
//                            }
//                        }
//                        break;
//                    case State.ScheduledToRunAfterDelay:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Running, State.ScheduledToRunAfterDelay) == State.ScheduledToRunAfterDelay)
//                        {
//                            OnCallback(info, out shouldDispose);
//                            if (shouldDispose)
//                            {
//                                OnCallback(WorkerThreadTimeoutResults.Disposing, out shouldDispose);
//                                m_stateMachine = State.Disposed; //Introduces a race condition
//                                return false;
//                            }
//                        }
//                        break;
//                    case State.Running:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.NotRunning, State.Running) == State.Running)
//                        {
//                            return true;
//                        }
//                        break;
//                    case State.RunAgain:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.RunAgain) == State.RunAgain)
//                        {
//                            m_thread.Invoke();
//                            m_stateMachine = State.ScheduledToRun;
//                            return true;
//                        }
//                        break;
//                    case State.RunAgainAfterDelay:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.RunAgainAfterDelay) == State.RunAgainAfterDelay)
//                        {
//                            m_thread.Invoke(m_pendingDelay);
//                            m_stateMachine = State.ScheduledToRunAfterDelay;
//                            return true;
//                        }
//                        break;
//                    case State.Disposed:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Disposed, State.Disposed) == State.Disposed)
//                        {
//                            return false;
//                        }
//                        break;
//                    case State.NotRunning:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.NotRunning) == State.NotRunning)
//                        {
//                            throw new Exception("InvalidState");
//                        }
//                        break;
//                    case State.PendingAction:
//                        break;
//                }
//                wait.SpinOnce();
//            }
//        }

//        /// <summary>
//        /// Requests that the worker thread invoke the callback as soon as possible.
//        /// </summary>
//        /// <remarks>
//        /// Calling this after calling Start(delay) will cause the timer to be canceled and will invoke 
//        /// the callback as soon as possible.
//        /// </remarks>
//        public void Start()
//        {
//            SpinWait wait = default(SpinWait);

//            while (true)
//            {
//                switch (m_stateMachine)
//                {
//                    case State.NotRunning:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.NotRunning) == State.NotRunning)
//                        {
//                            m_thread.Invoke();
//                            m_stateMachine = State.ScheduledToRun;
//                            return;
//                        }
//                        break;
//                    case State.ScheduledToRunAfterDelay:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.ScheduledToRunAfterDelay) == State.ScheduledToRunAfterDelay)
//                        {
//                            m_thread.CancelTimer();
//                            m_stateMachine = State.ScheduledToRun;
//                            return;
//                        }
//                        break;
//                    case State.Running:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.RunAgain, State.Running) == State.Running)
//                        {
//                            return;
//                        }
//                        break;
//                    case State.RunAgainAfterDelay:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.RunAgain, State.RunAgainAfterDelay) == State.RunAgainAfterDelay)
//                        {
//                            return;
//                        }
//                        break;
//                    case State.RunAgain:
//                    case State.ScheduledToRun:
//                    case State.Disposed:
//                        return;
//                    case State.PendingAction:
//                        break;
//                }
//                wait.SpinOnce();
//            }
//        }
//        /// <summary>
//        /// Requests that the worker thread invoke the callback after the specified interval.
//        /// </summary>
//        /// <param name="delay">the delay in milliseconds</param>
//        /// <remarks>
//        /// The delay used is only the first delay passed to this class. The timer will not restart until the 
//        /// callback has been called. Calling Start() without a delay will cancel the timer and start as soon as possible.
//        /// </remarks>
//        public void Start(int delay)
//        {
//            SpinWait wait = new SpinWait();

//            while (true)
//            {
//                switch (m_stateMachine)
//                {
//                    case State.NotRunning:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.NotRunning) == State.NotRunning)
//                        {
//                            m_thread.Invoke(delay);
//                            m_stateMachine = State.ScheduledToRunAfterDelay;
//                            return;
//                        }
//                        break;
//                    case State.Running:
//                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.Running) == State.Running)
//                        {
//                            m_pendingDelay = delay;
//                            m_stateMachine = State.RunAgainAfterDelay;
//                            return;
//                        }
//                        break;
//                    case State.ScheduledToRunAfterDelay:
//                    case State.ScheduledToRun:
//                    case State.RunAgain:
//                    case State.RunAgainAfterDelay:
//                    case State.Disposed:
//                        return;
//                    case State.PendingAction:
//                        break;
//                }
//                wait.SpinOnce();
//            }
//        }

//        void OnCallback(WorkerThreadTimeoutResults results, out bool shouldDispose)
//        {
//            shouldDispose = false;
//            try
//            {
//                lock (m_disposingConcurrentCallLock)
//                {
//                    var callback = m_callback;
//                    if (callback != null)
//                    {
//                        callback(results, ref shouldDispose);

//                        if (m_callback == null)
//                            shouldDispose = true;
//                    }
//                    else
//                    {
//                        shouldDispose = true;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        /// <summary>
//        /// Terminates the callback as soon as possible. Not a recommended action. Rather a user should properly dispose of
//        /// the worker through the callback method.
//        /// </summary>
//        public void Dispose()
//        {
//            m_callback = null;
//            Start();
//            lock (m_disposingConcurrentCallLock)
//            {
//                //wait for the worker to finish.
//            }
//        }
//    }


//}
