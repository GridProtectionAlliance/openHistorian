//******************************************************************************************************
//  WorkerThread.cs - Gbtc
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
//  3/15/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GSF.Threading
{

    /// <summary>
    /// Callback method for the <see cref="WorkerThread"/>.
    /// </summary>
    /// <param name="executionMode">the mode of execution that caused this event to raise.</param>
    public delegate void WorkerThreadCallback(WorkerThreadTimeoutResults executionMode);

    /// <summary>
    /// Metadata about why this worker was called.
    /// </summary>
    public enum WorkerThreadTimeoutResults
    {
        /// <summary>
        /// A start with a delay was called initially, however, the full delay was
        /// not observed as a Start without a delay was called.
        /// </summary>
        StartAfterPartialDelay,
        /// <summary>
        /// A start with a delay was called initially, and the full delay was observed.
        /// </summary>
        StartAfterFullDelay,
        /// <summary>
        /// A start without a delay was called.
        /// </summary>
        StartNow,
        /// <summary>
        /// Dispose was called and execution will terminate after this function call.
        /// </summary>
        Disposing,
        /// <summary>
        /// The worker is executing on the calling thread, not the worker thread.
        /// </summary>
        RunningOnLocalThread
    }

    /// <summary>
    /// A weak referenced <see cref="Thread"/> that will enter a pause state behind a weak reference
    /// so it can be garbaged collected if it's in a sleep state.
    /// </summary>
    /// <remarks>
    /// WeakThread functions in the specified way. 
    /// Valid function calls are Start(), Start(delay), and Dispose().
    /// 
    /// Start(delay) will schedule the worker thread to run after the specified time interval. 
    ///  Duplicate calls to this method will be ignored and only the first timer delay will be used.
    ///  If a worker is already running, this method will queue for it to run again after it finishes
    ///  with the provided delay. Therefore, the worker will always start sometime after this method.
    ///
    /// Start() will request that a worker start on the callback as soon as possible (usually the amount
    ///  of time it takes to wake up a thread). If there was a delay specified by a prior call, the delay
    ///  will be canceled and the callback will execute as soon as possible. If a worker is already running, 
    ///  this method will queue for it to run again after it finishes. Therefore, the worker will always 
    ///  start sometime after this method.
    /// 
    /// Dispose() will stop the execution of the worker as soon as possible. This means not allowing the 
    /// disposal event to fire. It is not recommended to use this method, rather externally coordinating a proper
    /// disposal by returning true to the shouldDispose parameter passed to the callback function.
    /// 
    /// </remarks>
    public partial class WorkerThread
        : IDisposable
    {
        /// <summary>
        /// State variables for the internal state machine.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// Indicates that the task is not running.
            /// </summary>
            NotRunning = 1,

            /// <summary>
            /// Indicates that the task is scheduled to execute after a user specified delay
            /// </summary>
            ScheduledToRunAfterDelay = 2,

            /// <summary>
            /// Indicates the task has been queue for immediate execution, but has not started running yet.
            /// </summary>
            ScheduledToRun = 3,

            /// <summary>
            /// Indicates the task is currently running.
            /// </summary>
            RunningOnWorkerThread = 4,

            /// <summary>
            /// Indicates the task is running on the current thread.
            /// </summary>
            RunningOnMyThread = 5,

            /// <summary>
            /// Indicates that the class has been Terminated and no more execution is possible.
            /// </summary>
            Disposed = 6
        }

        #region [ State Variables ]


        bool m_runAgainOnMyThread;
        /// <summary>
        /// A state variable that means a RunOnMyThread has been called and the worker thread is no longer needed and can be canceled.
        /// Note, this should be a synchronized call since a race condition exists that might requeue work before the worker gets canceled.
        /// </summary>
        /// <remarks>
        /// Only Cleared within <see cref="OnWorkerThreadRunning"/>. 
        /// </remarks>
        bool m_cancelingBackgroudWorker;
        /// <summary>
        /// A state variable that means the class should be disposed at its earliest convinence.
        /// </summary>
        bool m_disposing;
        /// <summary>
        /// A state variable meaning that the worker thread is waiting for a signal from the user's current thread before beginning.
        /// </summary>
        bool m_workerThreadWaiting;
        /// <summary>
        /// A state variable meaning that the current thread is awaiting a signal from the worker thread when complete.
        /// </summary>
        bool m_currentThreadWaiting;
        /// <summary>
        /// A state variable meaning that the worker will run immediately at least once after this variable has returned true
        /// </summary>
        volatile bool m_willRun;
        /// <summary>
        /// A state variable meaning that the worker will run either immediately or after a time delay at least once after this variable has returned true.
        /// </summary>
        volatile bool m_willRunAfterDelay;
        /// <summary>
        /// A state variable storing the delay associated with a delayed start. This variable is only valid if a re-run was queued.
        /// </summary>
        int m_runDelay;

        State m_state;

        /// <summary>
        /// A state variable containing the thread if of the current worker thread. A value less than 0 means invalid.
        /// </summary>
        int m_workerThreadId;

        object m_syncRoot;

        #endregion

        ManualResetEvent m_currentThreadWait;
        ManualResetEvent m_workerThreadWait;
        ManualResetEvent m_disposingWait;

        WorkerThreadCallback m_callback;

        /// <summary>
        /// Used to prevent concurrent calls to the Dispose method.
        /// </summary>
        object m_disposingConcurrentCallLock;


        //A reference of the delegate must be kept since ThreadContainer places this in a weak reference.
        //Otherwise it will get collected since only references to the delegate will be strong references.
        object m_containerCallback;

        ScheduledTask m_thread;

        /// <summary>
        /// Creates a <see cref="WorkerThread"/> that raises the provided event when invoked.
        /// </summary>
        /// <param name="callback">the back to raise with the </param>
        /// <param name="threadMode">parameter to pass to the underlying thread</param>
        /// <param name="priority">parameter to pass to the underlying thread</param>
        public WorkerThread(WorkerThreadCallback callback, ThreadingMode threadMode = ThreadingMode.ThreadPool, ThreadPriority priority = ThreadPriority.Normal)
        {
            m_syncRoot = new object();
            m_disposing = false;
            m_workerThreadWaiting = false;
            m_workerThreadWait = new ManualResetEvent(false);
            m_currentThreadWait = new ManualResetEvent(false);
            m_disposingWait = new ManualResetEvent(false);
            m_runAgainOnMyThread = false;
            m_currentThreadWaiting = false;
            m_willRun = false;
            m_willRunAfterDelay = false;
            m_runDelay = 0;
            m_state = State.NotRunning;
            m_disposingConcurrentCallLock = new object();
            m_callback = callback;
            m_thread = new ScheduledTask(threadMode, priority);
            m_thread.OnRunning += OnWorkerThreadRunning;
        }

        void OnThreadDisposed()
        {
            try
            {
                if (m_disposingWait != null)
                    m_disposingWait.Set();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// The callback from the worker thread to start processing the state machine.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        void OnWorkerThreadRunning(ThreadContainerCallbackReason info)
        {
            if (info == ThreadContainerCallbackReason.Disposing)
            {

            }
            WorkerThreadTimeoutResults info2;
            if (info == ThreadContainerCallbackReason.Start)
            {
                info2 = WorkerThreadTimeoutResults.StartNow;
            }
            else if (info == ThreadContainerCallbackReason.StartFullDelay)
            {
                info2 = WorkerThreadTimeoutResults.StartAfterFullDelay;
            }
            else if (info == ThreadContainerCallbackReason.StartPartialDelay)
            {
                info2 = WorkerThreadTimeoutResults.StartAfterPartialDelay;
            }
            else
            {
                throw new Exception();
            }

            using (var @lock = new MonitorHelper(m_syncRoot, true))
            {
                if (m_state == State.Disposed)
                {
                    m_thread.Dispose();
                    return;
                }

                if (m_cancelingBackgroudWorker)
                {
                    m_cancelingBackgroudWorker = false;
                    return;
                }

                if (m_state == State.NotRunning)
                    throw new Exception("Invalid State in OnWorkerThreadRunning: NotRunning");

                if (m_state == State.RunningOnWorkerThread)
                    throw new Exception("Invalid State in OnWorkerThreadRunning: RunningOnWorkerThread");


                if (m_state == State.RunningOnMyThread)
                {
                    //Wait for state machine to be released
                    m_workerThreadWaiting = true;
                    m_workerThreadWait.Reset();
                    @lock.Exit();
                    m_workerThreadWait.WaitOne();
                    //Upon exit, the state 
                    @lock.Enter();
                }

                if (m_state == State.ScheduledToRunAfterDelay ||
                    m_state == State.ScheduledToRun ||
                    m_state == State.RunningOnWorkerThread)
                {

                    if (!m_disposing)
                    {
                        m_willRunAfterDelay = false;
                        m_willRun = false;
                        m_state = State.RunningOnWorkerThread;
                        @lock.Exit();
                        OnCallback(info2);
                        @lock.Enter();
                        if (m_state != State.RunningOnWorkerThread)
                            throw new Exception("Invalid State: " + m_state.ToString() + " Expecting RunningOnWorkerThread");
                    }

                    if (m_disposing)
                    {
                        m_state = State.RunningOnWorkerThread;
                        @lock.Exit();
                        OnCallback(WorkerThreadTimeoutResults.Disposing);
                        @lock.Enter();
                        if (m_state != State.RunningOnWorkerThread)
                            throw new Exception("Invalid State: " + m_state.ToString() + " Expecting RunningOnWorkerThread");

                        OnSetToDispose();
                        m_state = State.Disposed;
                        @lock.Exit();
                        InternalDispose();
                        m_thread.Dispose();
                        return;
                    }
                }
                else
                {
                    throw new Exception("Invalid State: " + m_state.ToString() + " Expecting ScheduledToRunAfterDelay, ScheduledToRun, or RunningOnWorkerThread");
                }

                if (m_currentThreadWaiting)
                {
                    m_currentThreadWaiting = false;
                    m_state = State.RunningOnMyThread; //Changing the state
                    m_currentThreadWait.Set();
                }
                else if (m_willRun)
                {
                    m_thread.Start();
                    m_state = State.ScheduledToRun;
                }
                else if (m_willRunAfterDelay)
                {
                    m_thread.Start(m_runDelay);
                    m_state = State.ScheduledToRunAfterDelay;
                }
                else
                {
                    m_state = State.NotRunning;
                }
            }
        }

        /// <summary>
        /// Requests that the worker run on the current thread, preempting a queued worker thread if one exists.
        /// if the background is already working, this method will block until the worker finished, then immediately
        /// call the worker with the current thread.
        /// </summary>
        public void StartOnMyThread()
        {
        RinseAndRepeat:

            using (var @lock = new MonitorHelper(m_syncRoot, true))
            {
                if (m_state == State.Disposed)
                    return;

                if (m_state == State.RunningOnMyThread)
                {
                    if (Thread.CurrentThread.ManagedThreadId == m_workerThreadId)
                    {
                        m_runAgainOnMyThread = true;
                        return;
                    }
                    throw new NotSupportedException("Calling StartOnMyThread from different threads concurrently is not supported.");
                }

                if (m_state == State.RunningOnWorkerThread)
                {
                    m_workerThreadWaiting = true;
                    m_workerThreadWait.Reset();
                    @lock.Exit();
                    m_workerThreadWait.WaitOne();
                    @lock.Enter();
                }

                if (m_state == State.NotRunning ||
                    m_state == State.ScheduledToRunAfterDelay ||
                    m_state == State.ScheduledToRun ||
                    m_state == State.RunningOnMyThread)
                {
                    if (!m_disposing)
                    {
                        m_willRunAfterDelay = false;
                        m_willRun = false;
                        m_state = State.RunningOnMyThread;
                        @lock.Exit();
                        OnCallback(WorkerThreadTimeoutResults.RunningOnLocalThread);
                        @lock.Enter();
                        if (m_state != State.RunningOnMyThread)
                            throw new Exception("Invalid State: " + m_state.ToString() + " Expecting RunningOnMyThread");
                    }

                    if (m_disposing)
                    {
                        m_state = State.RunningOnMyThread;
                        @lock.Exit();
                        OnCallback(WorkerThreadTimeoutResults.Disposing);
                        @lock.Enter();

                        if (m_state != State.RunningOnMyThread)
                            throw new Exception("Invalid State: " + m_state.ToString() + " Expecting RunningOnMyThread");

                        OnSetToDispose();
                        m_state = State.Disposed;
                        @lock.Exit();
                        InternalDispose();
                        return;
                    }
                }
                else
                {
                    throw new Exception("Invalid State: " + m_state.ToString() + " Expecting NotRunning, ScheduledToRunAfterDelay, ScheduledToRun, or RunningOnWorkerThread");
                }

                if (m_runAgainOnMyThread)
                {
                    m_runAgainOnMyThread = false;
                    goto RinseAndRepeat;
                }
                else if (m_workerThreadWaiting)
                {
                    m_workerThreadWaiting = false;
                    m_state = State.RunningOnWorkerThread;
                    @lock.Exit();
                    m_workerThreadWait.Set();
                }
                else if (m_willRun)
                {
                    m_thread.Start();
                    m_state = State.ScheduledToRun;
                }
                else if (m_willRunAfterDelay)
                {
                    m_thread.Start(m_runDelay);
                    m_state = State.ScheduledToRunAfterDelay;
                }
                else
                {
                    m_state = State.NotRunning;
                }
            }
        }


        /// <summary>
        /// Requests that the worker thread invoke the callback as soon as possible.
        /// </summary>
        /// <remarks>
        /// Calling this after calling Start(delay) will cause the timer to be canceled and will invoke 
        /// the callback as soon as possible.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start()
        {
            //This method will likely be inlined by the compiler since it is so small and meets the requirements for inlining.
            if (!m_willRun)
                Start2();
        }

        void Start2()
        {
            lock (m_syncRoot)
            {
                if (m_cancelingBackgroudWorker)
                {
                    m_cancelingBackgroudWorker = false;
                    return;
                }
                if (m_state == State.RunningOnWorkerThread || m_state == State.RunningOnMyThread)
                {
                    m_willRunAfterDelay = true;
                    m_willRun = true;
                }
                else if (m_state == State.NotRunning)
                {
                    m_willRunAfterDelay = true;
                    m_willRun = true;
                    m_thread.Start();
                    m_state = State.ScheduledToRun;
                }
                else if (m_state == State.ScheduledToRunAfterDelay)
                {
                    m_willRunAfterDelay = true;
                    m_willRun = true;
                    m_thread.Start();
                    m_state = State.ScheduledToRun;
                }
            }
        }
        /// <summary>
        /// Requests that the worker thread invoke the callback after the specified interval.
        /// </summary>
        /// <param name="delay">the delay in milliseconds</param>
        /// <remarks>
        /// The delay used is only the first delay passed to this class. The timer will not restart until the 
        /// callback has been called. Calling Start() without a delay will cancel the timer and start as soon as possible.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start(int delay)
        {
            if (!m_willRunAfterDelay)
                Start2(delay);
        }

        void Start2(int delay)
        {
            lock (m_syncRoot)
            {
                if (m_cancelingBackgroudWorker)
                {
                    m_cancelingBackgroudWorker = false;
                    return;
                }
                if (m_state == State.NotRunning)
                {
                    m_willRunAfterDelay = true;
                    m_thread.Start(delay);
                    m_state = State.ScheduledToRunAfterDelay;
                }
                else if (m_state == State.RunningOnWorkerThread || m_state == State.RunningOnMyThread)
                {
                    m_willRunAfterDelay = true;
                    m_runDelay = delay;
                }
            }
        }

        void OnCallback(WorkerThreadTimeoutResults results)
        {
            m_workerThreadId = Thread.CurrentThread.ManagedThreadId;
            try
            {
                var callback = m_callback;
                if (callback != null)
                {
                    callback(results);
                }
                else
                {
                    Dispose();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                m_workerThreadId = -1;
            }
        }

        /// <summary>
        /// Stops the execution of the worker. Will block until the worker thread finishes executing. 
        /// This function will not block if called from within the worker thread 
        /// (whether a dedicated worker thread, or the user's current thread if calling <see cref="StartOnMyThread"/>.)
        /// </summary>
        public void Dispose()
        {
            if (!m_disposing)
            {
                lock (m_syncRoot)
                {
                    if (m_state == State.Disposed)
                        return;

                    m_willRun = true;
                    m_willRunAfterDelay = true;
                    m_disposing = true;
                    if (m_state == State.NotRunning)
                    {
                        m_thread.Start();
                        m_state = State.ScheduledToRun;
                    }
                    if (m_state == State.ScheduledToRunAfterDelay)
                    {
                        m_thread.Start();
                        m_state = State.ScheduledToRun;
                    }
                }
            }

            if (m_workerThreadId == Thread.CurrentThread.ManagedThreadId)
                return;

            //waiting for dispose to actually be called.
            InternalDispose();
        }

        /// <summary>
        /// Call this when changing to the dispose state.
        /// </summary>
        private void OnSetToDispose()
        {
            m_willRun = true;
            m_willRunAfterDelay = true;
            m_disposing = true;
            m_cancelingBackgroudWorker = true;
            m_disposingWait.Set();
        }

        /// <summary>
        /// Disposes of the objects, waits until m_disposingWait to be signaled.
        /// </summary>
        private void InternalDispose()
        {
            lock (m_disposingConcurrentCallLock)
            {
                GC.SuppressFinalize(this);
                if (m_disposingWait != null)
                {
                    m_disposingWait.WaitOne();
                    m_disposingWait.Dispose();
                    m_disposingWait = null;
                    m_workerThreadWait.Dispose();
                    m_workerThreadWait = null;
                    m_currentThreadWait.Dispose();
                    m_currentThreadWait = null;
                }
            }
        }
    }
}
