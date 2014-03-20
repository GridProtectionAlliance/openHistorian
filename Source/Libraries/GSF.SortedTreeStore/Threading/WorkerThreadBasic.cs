//******************************************************************************************************
//  ThreadContainerBase.cs - Gbtc
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSF.Threading
{
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

    public class WorkerThreadBasic : IDisposable
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

        object m_syncRoot;
        Action<ThreadContainerCallbackReason> m_callback;

        State m_state;
        ThreadContainerCallbackReason m_runReason;

        bool m_disposing;
        bool m_runAgain;
        bool m_runAgainAfterDelay;
        int m_runAgainDelay;
        ThreadContainerBase m_thread;
        object m_weakCallbackToken;

        public WorkerThreadBasic(Action<ThreadContainerCallbackReason> callback, bool isDedicated = false, bool isBackground = true, ThreadPriority priority = ThreadPriority.Normal)
        {
            m_syncRoot = new object();
            m_callback = callback;
            if (isDedicated)
            {
                m_thread = new ThreadContainerDedicated(new WeakActionFast(OnRunning, out m_weakCallbackToken), isBackground, priority);
            }
            else
            {
                m_thread = new ThreadContainerThreadpool(new WeakActionFast(OnRunning, out m_weakCallbackToken));

            }
        }

        ~WorkerThreadBasic()
        {
            Dispose();


        }

        /// <summary>
        /// Executed by the worker.
        /// </summary>
        protected void OnRunning()
        {
            using (var @lock = new MonitorHelper(m_syncRoot, true))
            {
                ThreadContainerCallbackReason runReason = m_runReason;

                if (m_disposing)
                {
                    m_thread.InternalDispose();
                    m_state = State.Disposed;
                    @lock.Exit();
                    ThreadContainerCallbackReason state = ThreadContainerCallbackReason.Disposing;
                    m_callback(state);
                    return;
                }
                else if (m_state == State.ScheduledToRunAfterDelay ||
                    m_state == State.ScheduledToRun)
                {
                    m_runAgain = false;
                    m_runAgainAfterDelay = false;
                    m_state = State.Running;
                }
                else
                {
                    throw new Exception("Invalid State Within OnRunning: " + m_state.ToString());
                }

                //---------------------------
                @lock.Exit();
                m_callback(runReason);
                @lock.Enter();
                //---------------------------

                if (m_state != State.Running)
                    throw new Exception("State should not have changed within OnRunning: " + m_state.ToString());

                if (m_disposing)
                {
                    m_thread.InternalDispose();
                    m_state = State.Disposed;
                    @lock.Exit();
                    ThreadContainerCallbackReason state = ThreadContainerCallbackReason.Disposing;
                    m_callback(state);
                    return;
                }

                m_thread.InternalAfterRunning();

                if (m_runAgain)
                {
                    m_thread.InternalStart();
                    m_state = State.ScheduledToRun;
                }
                else if (m_runAgainAfterDelay)
                {
                    m_thread.InternalStart(m_runAgainDelay);
                    m_state = State.ScheduledToRunAfterDelay;
                }
                else
                {
                    m_state = State.NotRunning;
                }
                m_runAgain = false;
                m_runAgainAfterDelay = false;
            }
        }

        /// <summary>
        /// Starts instantly
        /// </summary>
        public void Start()
        {
            lock (m_syncRoot)
            {
                if (m_disposing)
                    return;

                if (m_state == State.NotRunning)
                {
                    m_runReason = ThreadContainerCallbackReason.Start;
                    m_thread.InternalStart();
                    m_state = State.ScheduledToRun;
                }
                else if (m_state == State.ScheduledToRunAfterDelay)
                {
                    m_runReason = ThreadContainerCallbackReason.StartPartialDelay;
                    m_thread.InternalCancelTimer();
                    m_state = State.ScheduledToRun;
                }
                else if (m_state == State.ScheduledToRun)
                {
                    //Do Nothing
                }
                else if (m_state == State.Running)
                {
                    m_runAgain = true;
                }
            }
        }

        /// <summary>
        /// Starts after the provided delay.
        /// </summary>
        /// <param name="delay"></param>
        public void Start(int delay)
        {
            lock (m_syncRoot)
            {
                if (m_disposing)
                    return;


                if (m_state == State.NotRunning)
                {
                    m_runReason = ThreadContainerCallbackReason.StartFullDelay;
                    m_thread.InternalStart(delay);
                    m_state = State.ScheduledToRun;
                }
                else if (m_state == State.ScheduledToRunAfterDelay)
                {
                    //Do Nothing
                }
                else if (m_state == State.ScheduledToRun)
                {
                    //Do Nothing
                }
                else if (m_state == State.Running)
                {
                    if (m_runAgainAfterDelay)
                    {
                        m_runAgainDelay = Math.Min(m_runAgainDelay, delay);
                    }
                    else if (m_runAgain)
                    {
                        //Do Nothing   
                    }
                    else
                    {
                        m_runAgainAfterDelay = true;
                        m_runAgainDelay = delay;
                    }
                }
            }
        }

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
                    m_thread.InternalStart();
                    m_state = State.ScheduledToRun;
                }
                else if (m_state == State.ScheduledToRunAfterDelay)
                {
                    m_thread.InternalCancelTimer();
                    m_state = State.ScheduledToRun;
                }
            }
        }


    }
}
