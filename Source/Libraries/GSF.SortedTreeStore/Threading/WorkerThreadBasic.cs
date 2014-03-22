//******************************************************************************************************
//  WorkerThreadBasic.cs - Gbtc
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

//------------------------------------------------------------------------------------------------------
// Warning: This class contains very low-level logic and optimized to have minimal locking
//          Before making any changes, be sure to consult the experts. Any bugs can introduce
//          a race condition that will be very difficult to detect and fix.
//          Additional Functional Requests should result in another class being created rather than modifying this one.
//------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Specifies the threading mode to use for the <see cref="ScheduledTask"/>
    /// </summary>
    public enum ThreadingMode
    {
        /// <summary>
        /// A dedicated thread that is a foreground thread.
        /// </summary>
        DedicatedForeground,
        /// <summary>
        /// A dedicated thread that is a background thread.
        /// </summary>
        DedicatedBackground,
        /// <summary>
        /// A background thread from the thread pool.
        /// </summary>
        ThreadPool
    }

    /// <summary>
    /// Metadata about why this worker was called.
    /// </summary>
    public enum ThreadContainerCallbackReason
    {
        /// <summary>
        /// A start with a delay was called initially, however, the full delay was
        /// not observed as a Start without a delay was called.
        /// </summary>
        StartPartialDelay,
        /// <summary>
        /// A start with a delay was called initially, and the full delay was observed.
        /// </summary>
        StartFullDelay,
        /// <summary>
        /// A start without a delay was called.
        /// </summary>
        Start,
        /// <summary>
        /// Dispose was called and execution will terminate after this function call.
        /// </summary>
        Disposing,
    }

    public class ScheduledTask : IDisposable
    {

        /// <summary>
        /// State variables for the internal state machine.
        /// </summary>
        enum State
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
            Running = 4,

            /// <summary>
            /// A disposed state
            /// </summary>
            Disposed = 5
        }


        /// <summary>
        /// The worker event
        /// </summary>
        public event Action<ThreadContainerCallbackReason> OnRunning;

        public event Action OnDispose;

        /// <summary>
        /// Occurs when then <see cref="OnRunning"/> event throws an exception.
        /// </summary>
        public event Action<Exception> OnException;

        object m_disposeSync;
        object m_syncRoot;
        State m_state;
        ThreadContainerCallbackReason m_runReason;
        bool m_disposing;
        //volatile bool m_runAgain;
        //volatile bool m_runAgainAfterDelay;
        int m_runAgainDelay;
        ThreadContainerBase m_thread;
        object m_weakCallbackToken;
        ManualResetEvent m_waitForDispose;

        public ScheduledTask(ThreadingMode threadMode = ThreadingMode.ThreadPool, ThreadPriority priority = ThreadPriority.Normal)
        {
            m_state = State.NotRunning;
            m_waitForDispose = new ManualResetEvent(false);
            m_syncRoot = new object();
            m_disposeSync = new object();
            if (threadMode == ThreadingMode.DedicatedForeground)
            {
                m_thread = new ThreadContainerDedicated(new WeakActionFast<ThreadContainerBase.CallbackArgs>(OnRunningCallback, out m_weakCallbackToken), false, priority);
            }
            else if (threadMode == ThreadingMode.DedicatedBackground)
            {
                m_thread = new ThreadContainerDedicated(new WeakActionFast<ThreadContainerBase.CallbackArgs>(OnRunningCallback, out m_weakCallbackToken), true, priority);
            }
            else if (threadMode == ThreadingMode.ThreadPool)
            {
                m_thread = new ThreadContainerThreadpool(new WeakActionFast<ThreadContainerBase.CallbackArgs>(OnRunningCallback, out m_weakCallbackToken));
            }
            else
            {
                throw new ArgumentOutOfRangeException("threadMode");
            }
        }

        ~ScheduledTask()
        {
            Dispose();
        }

        /// <summary>
        /// Executed by the worker.
        /// </summary>
        void OnRunningCallback(ThreadContainerBase.CallbackArgs args)
        {
            using (var @lock = new MonitorHelper(m_syncRoot, true))
            {
                ThreadContainerCallbackReason runReason = m_runReason;

                if (m_disposing)
                {
                    args.ShouldDispose = true;
                    m_state = State.Disposed;
                    @lock.Exit();
                    TryCallback(ThreadContainerCallbackReason.Disposing);
                    return;
                }

                else if (m_state == State.ScheduledToRunAfterDelay ||
                    m_state == State.ScheduledToRun || m_state == State.NotRunning)
                {
                    m_state = State.Running;
                }
                else
                {
                    throw new Exception("Invalid State Within OnRunning: " + m_state.ToString());
                }

                //---------------------------
                @lock.Exit();

                //m_runAgain = false;
                //m_runAgainAfterDelay = false;

                TryCallback(runReason);

                @lock.Enter();
                //---------------------------

                if (m_state != State.Running)
                    throw new Exception("State should not have changed within OnRunning: " + m_state.ToString());

                if (m_disposing)
                {
                    args.ShouldDispose = true;
                    m_state = State.Disposed;
                    @lock.Exit();
                    TryCallback(ThreadContainerCallbackReason.Disposing);
                    return;
                }

                m_state = State.NotRunning;

                //if (m_runAgain)
                //{
                //    m_thread.Start();
                //    m_state = State.ScheduledToRun;
                //}
                //else if (m_runAgainAfterDelay)
                //{
                //    m_runAgain = false;
                //    m_thread.Start(m_runAgainDelay);
                //    m_state = State.ScheduledToRunAfterDelay;
                //}
                //else
                //{
                //    m_runAgain = false;
                //    m_runAgainAfterDelay = false;
                //    m_state = State.NotRunning;
                //}
            }
        }

        /// <summary>
        /// Immediately starts the task. 
        /// </summary>
        /// <remarks>
        /// If this is called after a Start(Delay) the timer will be short circuited 
        /// and the process will still start immediately. 
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start()
        {
            m_thread.Start();
            //if (!m_runAgain)
            //    StartSlower();
        }

        //void StartSlower()
        //{
        //    lock (m_syncRoot)
        //    {
        //        if (m_disposing)
        //            return;

        //        if (m_state == State.NotRunning)
        //        {
        //            m_runAgain = true;
        //            m_runReason = ThreadContainerCallbackReason.Start;
        //            m_thread.Start();
        //            m_state = State.ScheduledToRun;
        //        }
        //        else if (m_state == State.ScheduledToRunAfterDelay)
        //        {
        //            m_runAgain = true;
        //            m_runReason = ThreadContainerCallbackReason.StartPartialDelay;
        //            m_thread.Start();
        //            m_state = State.ScheduledToRun;
        //        }
        //        else if (m_state == State.ScheduledToRun)
        //        {
        //            m_runAgain = true;
        //        }
        //        else if (m_state == State.Running)
        //        {
        //            m_runAgain = true;
        //        }
        //    }
        //}

        /// <summary>
        /// Starts a timer to run the task after a provided interval. 
        /// </summary>
        /// <param name="delay">the delay in milliseconds before the task should run</param>
        /// <remarks>
        /// If already running on a timer, this function will do nothing. Do not use this function to
        /// reset or restart an existing timer.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start(int delay)
        {
            m_thread.Start(delay);

            ////Tiny quick check that will likely be inlined by the compiler.
            //if (!m_runAgainAfterDelay)
            //    StartSlower(delay);
        }

        //void StartSlower(int delay)
        //{
        //    lock (m_syncRoot)
        //    {
        //        if (m_disposing)
        //            return;

        //        if (m_state == State.NotRunning)
        //        {
        //            m_runAgainAfterDelay = true;
        //            m_runAgainDelay = delay;
        //            m_runReason = ThreadContainerCallbackReason.StartFullDelay;
        //            m_thread.Start(delay);
        //            m_state = State.ScheduledToRunAfterDelay;
        //        }
        //        else if (m_state == State.ScheduledToRunAfterDelay)
        //        {
        //            m_runAgainAfterDelay = true;
        //        }
        //        else if (m_state == State.ScheduledToRun)
        //        {
        //            m_runAgainAfterDelay = true;
        //        }
        //        else if (m_state == State.Running)
        //        {
        //            if (m_runAgainAfterDelay)
        //            {
        //                m_runAgainDelay = Math.Min(m_runAgainDelay, delay);
        //            }
        //            else if (m_runAgain)
        //            {
        //                m_runAgainAfterDelay = true;
        //            }
        //            else
        //            {
        //                m_runAgainAfterDelay = true;
        //                m_runAgainDelay = delay;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Starts the disposing process of exiting the worker thread. Will invoke the callback one more time.
        /// duplicate calls are ignored. This method does not block.
        /// </summary>
        public void Dispose()
        {
            lock (m_syncRoot)
            {
                if (m_disposing)
                    return;

                m_disposing = true;

                if (m_state == State.NotRunning)
                {
                    m_runReason = ThreadContainerCallbackReason.Disposing;
                    m_thread.Start();
                    m_state = State.ScheduledToRun;
                }
                else if (m_state == State.ScheduledToRunAfterDelay)
                {
                    m_thread.Start();
                    m_state = State.ScheduledToRun;
                }
            }
        }

        public void DisposeAndWait()
        {
            Dispose();
            InternalDisposeAllResources();
        }

        void InternalDisposeAllResources()
        {
            lock (m_disposeSync)
            {
                if (m_waitForDispose != null)
                {
                    m_waitForDispose.WaitOne();
                    m_waitForDispose.Dispose();
                    m_waitForDispose = null;
                }
            }
        }

        void TryCallback(ThreadContainerCallbackReason args)
        {
            try
            {
                if (OnRunning != null)
                    OnRunning(args);
            }
            catch (Exception ex)
            {
                try
                {
                    if (OnException != null)
                        OnException(ex);
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.ToString());
                }
            }

            if (args == ThreadContainerCallbackReason.Disposing)
            {
                try
                {
                    if (OnDispose != null)
                        OnDispose();
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (OnException != null)
                            OnException(ex);
                    }
                    catch (Exception ex2)
                    {
                        Debug.WriteLine(ex2.ToString());
                    }
                }
                m_waitForDispose.Set();
                InternalDisposeAllResources();
            }
        }
    }
}
