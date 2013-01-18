//******************************************************************************************************
//  ScheduledTask_Internal.cs - Gbtc
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
using System.Threading;

namespace GSF.Threading
{
    public partial class ScheduledTask
    {

        /// <summary>
        /// The guts of the Scheduled Task
        /// </summary>
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

            CustomThreadBase m_thread;
            ManualResetEvent m_hasQuit;
            StateMachine m_state;
            WeakAction m_callback;
            WeakAction m_disposingCallback;
            WeakAction<Exception> m_exceptionCallback;
            volatile int m_delayRequested;
            volatile bool m_disposing;

            /// <summary>
            /// Creates a task that can be manually scheduled to run.
            /// </summary>
            /// <param name="callback">The method to repeatedly call</param>
            /// <param name="disposing">The method to call once upon disposing</param>
            /// <param name="exceptions">A callback for exception processing that may occur</param>
            /// <param name="isForeground"></param>
            public Internal(Action callback, Action disposing, Action<Exception> exceptions, bool isForeground)
            {
                if (isForeground)
                {
                    m_thread = new ForegroundThread(InternalBeginRunOnTimer);
                }
                else
                {
                    m_thread = new BackgroundThread(InternalBeginRunOnTimer);

                }
                m_exceptionCallback = new WeakAction<Exception>(exceptions);
                m_disposingCallback = new WeakAction(disposing);
                m_callback = new WeakAction(callback);
                m_state = new StateMachine(State.NotRunning);
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
                                m_thread.StartNow();
                                return;
                            }
                            break;
                        case State.ScheduledToRunAfterDelay:
                            if (m_state.TryChangeState(State.ScheduledToRunAfterDelay, State.ScheduledToRun))
                            {
                                m_thread.ShortCircuitDelayRequest();
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
                                m_thread.StartLater(delay);
                                return;
                            }
                            break;
                        case State.Running:
                            if (m_state.TryChangeState(State.Running, State.RunAgainAfterDelayIntermediate))
                            {
                                m_thread.ResetTimer();
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

            void InternalBeginRunOnTimer()
            {
                SpinWait wait = new SpinWait();
                while (true)
                {
                    if (m_state.TryChangeStates(State.ScheduledToRunAfterDelay, State.ScheduledToRun, State.Running))
                    {
                        while (true)
                        {
                            if (m_disposing)
                            {
                                m_thread.Dispose();
                                m_hasQuit.Set();
                                return;
                            }

                            try
                            {
                                m_callback.TryInvoke();
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    m_exceptionCallback.TryInvoke(ex);
                                }
                                catch (Exception)
                                {
                                }
                            }

                            if (CheckAfterExecuteAction() == NextAction.Quit)
                            {
                                return;
                            }
                        }
                    }
                    wait.SpinOnce();
                }
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
                        m_thread.Dispose();
                        m_hasQuit.Set();
                        return NextAction.Quit;
                    }

                    int state = m_state;
                    switch (state)
                    {
                        case State.Running:
                            if (m_state.TryChangeState(State.Running, State.Resetting))
                            {
                                m_thread.ResetTimer();
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
                                m_thread.StartLater(m_delayRequested);
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
                        m_thread.Dispose();
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
                DisposeMethod(true);
            }

            /// <summary>
            /// Attempts to clean up all resources used by this class. This method is similiar to <see cref="Dispose"/>
            /// except it does not wait for the worker thread to actually quit, and the disposing callback is not executed.
            /// </summary>
            public void Finalized()
            {
                DisposeMethod(false);
            }

            void DisposeMethod(bool waitForExit)
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
                                    m_thread.Dispose();
                                    if (waitForExit)
                                        CallDisposeCallback();
                                    return;
                                }
                                break;
                            case State.ScheduledToRunAfterDelay:
                                if (m_state.TryChangeState(State.ScheduledToRunAfterDelay, State.ScheduledToRun))
                                {
                                    m_thread.ShortCircuitDelayRequest();
                                    if (waitForExit)
                                    {
                                        m_hasQuit.WaitOne();
                                        CallDisposeCallback();
                                    }
                                    return;
                                }
                                break;
                            case State.ScheduledToRun:
                            case State.Running:
                            case State.RunAgain:
                                if (waitForExit)
                                {
                                    m_hasQuit.WaitOne();
                                    CallDisposeCallback();
                                }
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
                    m_disposingCallback.TryInvoke();
                }
                catch (Exception ex)
                {
                    try
                    {
                        m_exceptionCallback.TryInvoke(ex);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

    }
}
