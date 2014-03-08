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
//  2/14/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

//------------------------------------------------------------------------------------------------------
// Warning: This class contains very low-level logic and optimized to have minimal locking
//          Before making any changes, be sure to consult the experts. Any bugs can introduce
//          a race condition that will be very difficult to detect and fix. Additional Functional 
//          Requests should result in another class being created rather than modifying this one.
//------------------------------------------------------------------------------------------------------

using System;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Provides a real time worker thead that uses the thread pool to call events. 
    /// Calls made to this class can be asynchronous and come with very little overhead.
    /// </summary>
    /// <remarks>
    /// This class guarantees that <see cref="DoWork"/> will be called after a call to 
    /// <see cref="Signal"/> has occurred. 
    /// 
    /// All callbacks are done on the threadpool. Therefore this class does not need to be disposed.
    /// 
    /// Speed for <see cref="Signal"/> is on the order of 190 million calls per second per thread.
    /// Speed for <see cref="DoWork"/> event is on the order of 600,000 calls per second.
    /// </remarks>
    public class WorkerThread
    {
        /// <summary>
        /// State variables for the internal state machine.
        /// </summary>
        private static class State
        {
            /// <summary>
            /// Indicates that the task is not running.
            /// </summary>
            public const int NotRunning = 0;

            /// <summary>
            /// Indicates the task is currently running.
            /// </summary>
            public const int Running = 1;

            /// <summary>
            /// Indicates the task has been queue for immediate execution, but has not started running yet.
            /// </summary>
            public const int ScheduledToRun = 2;

        }

        /// <summary>
        /// The current state of the state machine.
        /// </summary>
        private readonly StateMachine m_stateMachine;

        /// <summary>
        /// Occurs after <see cref="Signal"/> has been called. 
        /// </summary>
        /// <remarks>
        /// Due to a race condition, it is possible that this could be 
        /// called when no additional work needs to be complete. This side 
        /// effect was considered acceptable as additional coordination would
        /// have been very complex and resulted is a much slower <see cref="Signal"/>
        /// method return time.
        /// 
        /// However, it is gaurenteed that after <see cref="Signal"/> is called, but
        /// this event will be raised.
        /// </remarks>
        public event EventHandler DoWork;

        /// <summary>
        /// Occurs when unhandled exceptions occur in the worker or disposed thread;
        /// </summary>
        public event UnhandledExceptionEventHandler OnException;

        /// <summary>
        /// The callback delegate so a new one is not instanced each time.
        /// </summary>
        private WaitCallback m_callback;

        /// <summary>
        /// Creates a task that can be manually scheduled to run.
        /// </summary>
        public WorkerThread()
        {
            m_stateMachine = new StateMachine(State.NotRunning);
            m_callback = new WaitCallback(ProcessRunningState);
        }

        /// <summary>
        /// Signals that the task run as soon as possible
        /// </summary>
        /// <remarks>
        /// Virtual methods execute approximately 30% faster. This method is not intended to be overridden. 
        /// </remarks>
        public virtual void Signal()
        {
            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotRunning:
                        if (m_stateMachine.TryChangeState(State.NotRunning, State.ScheduledToRun))
                        {
                            ThreadPool.QueueUserWorkItem(m_callback);
                            return;
                        }
                        break;
                    case State.Running:
                        if (m_stateMachine.TryChangeState(State.Running, State.ScheduledToRun))
                        {
                            return;
                        }
                        break;
                    case State.ScheduledToRun:
                        return;
                }
            }
        }

        /// <summary>
        /// The method executed in the state: Running. This is invoked by the 
        /// thread when it is time to run this state.
        /// </summary>
        private void ProcessRunningState(object state)
        {
            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.ScheduledToRun:
                        if (m_stateMachine.TryChangeState(State.ScheduledToRun, State.Running))
                        {
                            ProcessClientCallback();
                        }
                        break;
                    case State.Running:
                        if (m_stateMachine.TryChangeState(State.Running, State.NotRunning))
                        {
                            return;
                        }
                        break;
                    case State.NotRunning:
                        if (m_stateMachine.TryChangeState(State.NotRunning, State.NotRunning))
                        {
                            throw new Exception("InvalidState");
                        }
                        break;
                }
            }
        }

        private void ProcessClientCallback()
        {
            try
            {
                if (DoWork != null)
                    DoWork(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                try
                {
                    if (OnException != null)
                        OnException(this, new UnhandledExceptionEventArgs(ex, false));
                }
                catch (Exception)
                {
                }
            }
        }

    }
}