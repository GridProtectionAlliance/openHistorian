//******************************************************************************************************
//  ScheduledTask.cs - Gbtc
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
//  1/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Provides a time sceduled task that can either be canceled prematurely or told to execute early.
    /// </summary>
    public class ScheduledTask : IDisposable
    {
        volatile bool m_disposed;
        volatile bool m_isDisposing;
        //Class must be wrapped inside of another class so a Finalize method
        //will actually be called when this task is no longer being referenced.
        Internal m_internal;

        /// <summary>
        /// Creates a task that can be manually scheduled to run.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="onDisposing"></param>
        /// <param name="onException"></param>
        public ScheduledTask(Action callback, Action onDisposing = null, Action<Exception> onException = null)
        {
            m_internal = new Internal(callback, onDisposing, onException);
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="ScheduledTask"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~ScheduledTask()
        {
            m_isDisposing = true;
            if (m_internal != null)
                m_internal.Dispose();
            m_disposed = true;
            m_internal = null;
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="ScheduledTask"/> object.
        /// </summary>
        public void Dispose()
        {
            m_isDisposing = true;
            if (m_internal != null)
                m_internal.Dispose();
            m_internal = null;
            m_disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Immediately starts the task. 
        /// </summary>
        /// <remarks>
        /// If this is called after a Start(Delay) the timer will be short circuited 
        /// and the process will still start immediately. 
        /// </remarks>
        public void Start()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_internal.Start();
        }

        public void Start(TimeSpan delay)
        {
            Start(delay.Milliseconds);
        }

        /// <summary>
        /// Starts a timer to run the task after a provided interval. 
        /// </summary>
        /// <param name="delay"></param>
        /// <remarks>
        /// If already running on a timer, this function will do nothing. Do not use this function to
        /// reset or restart an existing timer.
        /// </remarks>
        public void Start(int delay)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_internal.Start(delay);
        }

        /// <summary>
        /// Gets if this class is trying to dispose. This is useful for the worker thread to know.
        /// So it can try to quit early if need be since the disposing thread will be blocked 
        /// until the worker has complete. 
        /// </summary>
        public bool IsDisposing
        {
            get
            {
                return m_isDisposing;
            }
        }

        /// <summary>
        /// Determines when everything has been disposed. 
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return m_disposed;
            }
        }

        class Internal
        {
            enum NextAction
            {
                RunAgain,
                Quit
            }

            static class State
            {
                public const int NotRunning = 0;
                public const int ScheduledToRunAfterDelay = 1;
                public const int ScheduledToRun = 2;
                public const int Running = 3;
                public const int Resetting = 4;
                public const int RunAgain = 5;
                public const int RunAgainAfterDelayIntermediate = 6;
                public const int RunAgainAfterDelay = 7;
                public const int Disposed = 8;
            }

            ManualResetEvent m_hasQuit;
            StateMachine m_state;
            ManualResetEvent m_resetEvent;
            RegisteredWaitHandle m_registeredHandle;
            Action m_callback;
            Action m_disposingCallback;
            Action<Exception> m_exceptionCallback;
            int m_delayRequested;
            volatile bool m_disposing;

            /// <summary>
            /// Creates a task that can be manually scheduled to run.
            /// </summary>
            /// <param name="callback">The method to repeatedly call</param>
            /// <param name="disposing">The method to call once upon disposing</param>
            /// <param name="exceptions">A callback for exception processing that may occur</param>
            public Internal(Action callback, Action disposing, Action<Exception> exceptions)
            {
                m_exceptionCallback = exceptions;
                m_disposingCallback = disposing;
                m_callback = callback;
                m_state = new StateMachine(State.NotRunning);
                m_resetEvent = new ManualResetEvent(false);
                m_hasQuit = new ManualResetEvent(false);
            }

            /// <summary>
            /// Immediately starts the task. 
            /// </summary>
            /// <remarks>
            /// If this is called after a Start(Delay) the timer will be short circuited 
            /// and the process will still start immediately. 
            /// </remarks>
            public void Start()
            {
                SpinWait wait = new SpinWait();
                while (true)
                {
                    if (m_disposing)
                        return;

                    int state = m_state;
                    switch (state)
                    {
                        case State.NotRunning:
                            if (m_state.TryChangeState(State.NotRunning, State.ScheduledToRun))
                            {
                                ThreadPool.QueueUserWorkItem(BeginRunOnTimer);
                                return;
                            }
                            break;
                        case State.ScheduledToRunAfterDelay:
                            if (m_state.TryChangeState(State.ScheduledToRunAfterDelay, State.ScheduledToRun))
                            {
                                m_resetEvent.Set();
                                return;
                            }
                            break;
                        case State.Running:
                            if (m_state.TryChangeState(State.Running, State.RunAgain))
                            {
                                return;
                            }
                            break;
                        case State.RunAgainAfterDelay:
                            if (m_state.TryChangeState(State.RunAgainAfterDelay, State.RunAgain))
                            {
                                return;
                            }
                            break;
                        case State.Resetting:
                        case State.RunAgainAfterDelayIntermediate:
                            //Wait for it to transition to its next state
                            break;
                        case State.RunAgain:
                        case State.ScheduledToRun:
                            return;
                    }

                    wait.SpinOnce();
                }
            }

            /// <summary>
            /// Starts a timer to run the task after a provided interval. 
            /// </summary>
            /// <param name="delay"></param>
            /// <remarks>
            /// If already running on a timer, this function will do nothing. Do not use this function to
            /// reset or restart an existing timer.
            /// </remarks>
            public void Start(int delay)
            {
                SpinWait wait = new SpinWait();

                while (true)
                {
                    if (m_disposing)
                        return;

                    int state = m_state;
                    switch (state)
                    {
                        case State.NotRunning:
                            if (m_state.TryChangeState(State.NotRunning, State.ScheduledToRunAfterDelay))
                            {
                                m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_resetEvent, BeginRunOnTimer, null, delay, true);
                                return;
                            }
                            break;
                        case State.Running:
                            if (m_state.TryChangeState(State.Running, State.RunAgainAfterDelayIntermediate))
                            {
                                m_resetEvent.Reset();
                                m_delayRequested = delay;
                                m_state.SetState(State.RunAgainAfterDelay);
                                return;
                            }
                            break;
                        case State.Resetting:
                            //Wait for it to transition to its next state
                            break;
                        case State.ScheduledToRunAfterDelay:
                        case State.ScheduledToRun:
                        case State.RunAgain:
                        case State.RunAgainAfterDelayIntermediate:
                        case State.RunAgainAfterDelay:
                            return;
                    }
                    wait.SpinOnce();
                }
            }


            #region [  The Worker Thread  ]

            void BeginRunOnTimer(object state)
            {
                InternalBeginRunOnTimer();
            }

            void BeginRunOnTimer(object state, bool isTimeout)
            {
                if (m_registeredHandle != null)
                    m_registeredHandle.Unregister(null);

                InternalBeginRunOnTimer();
            }

            void InternalBeginRunOnTimer()
            {
                if (m_state.TryChangeStates(State.ScheduledToRunAfterDelay, State.ScheduledToRun, State.Running))
                {
                    while (true)
                    {
                        if (m_disposing)
                        {
                            m_hasQuit.Set();
                            return;
                        }

                        try
                        {
                            m_callback();
                        }
                        catch (Exception ex)
                        {
                            if (m_exceptionCallback != null)
                            {
                                try
                                {
                                    m_exceptionCallback(ex);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }

                        if (CheckAfterExecuteAction() == NextAction.Quit)
                        {
                            return;
                        }
                    }

                }

                throw new Exception("State machine is not thread safe.");
            }

            NextAction CheckAfterExecuteAction()
            {
                //Process State Machine:
                SpinWait wait = new SpinWait();
                wait.Reset();

                while (true)
                {
                    if (m_disposing)
                    {
                        m_hasQuit.Set();
                        return NextAction.Quit;
                    }

                    int state = m_state;
                    switch (state)
                    {
                        case State.Running:
                            if (m_state.TryChangeState(State.Running, State.Resetting))
                            {
                                m_resetEvent.Reset();
                                m_state.SetState(State.NotRunning);
                                return NextAction.Quit;
                            }
                            break;
                        case State.RunAgain:
                            if (m_state.TryChangeState(State.RunAgain, State.Running))
                            {
                                m_state.SetState(State.Running);
                                return NextAction.RunAgain;
                            }
                            break;
                        case State.RunAgainAfterDelayIntermediate:
                            //wait for state to exit
                            break;
                        case State.RunAgainAfterDelay:
                            if (m_state.TryChangeState(State.RunAgainAfterDelay, State.ScheduledToRunAfterDelay))
                            {
                                m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_resetEvent, BeginRunOnTimer, null, m_delayRequested, true);
                                return NextAction.Quit;
                            }
                            break;
                        case State.Disposed:
                            break;
                        default:
                            throw new Exception("Should never be in this state.");

                    }
                    if (m_disposing)
                    {
                        m_hasQuit.Set();
                        return NextAction.Quit;
                    }
                    wait.SpinOnce();
                }

            }


            #endregion

            /// <summary>
            /// Stops all future calls to this class, and waits for the worker thread to quit before returning. 
            /// </summary>
            public void Dispose()
            {
                SpinWait wait = new SpinWait();

                if (!m_disposing)
                {
                    m_disposing = true;
                    Thread.MemoryBarrier();
                    while (true)
                    {
                        int state = m_state;
                        switch (state)
                        {
                            case State.NotRunning:
                                if (m_state.TryChangeState(State.NotRunning, State.Disposed))
                                {
                                    CallDisposeCallback();

                                    return;
                                }
                                break;
                            case State.ScheduledToRunAfterDelay:
                                if (m_state.TryChangeState(State.ScheduledToRunAfterDelay, State.ScheduledToRun))
                                {
                                    m_resetEvent.Set();
                                    m_hasQuit.WaitOne();
                                    CallDisposeCallback();

                                    return;
                                }
                                break;
                            case State.ScheduledToRun:
                            case State.Running:
                            case State.RunAgain:
                                m_hasQuit.WaitOne();
                                CallDisposeCallback();

                                return;
                            case State.RunAgainAfterDelayIntermediate:
                                break;
                            case State.RunAgainAfterDelay:
                                m_state.TryChangeState(State.RunAgainAfterDelay, State.RunAgain);
                                break;
                            case State.Resetting:
                                //Wait for it to transition to its next state
                                break;

                        }
                        wait.SpinOnce();
                    }
                }

            }

            void CallDisposeCallback()
            {
                try
                {
                    if (m_disposingCallback != null)
                        m_disposingCallback();
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (m_exceptionCallback != null)
                            m_exceptionCallback(ex);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }



    }
}
