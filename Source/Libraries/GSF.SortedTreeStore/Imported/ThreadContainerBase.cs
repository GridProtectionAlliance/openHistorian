//******************************************************************************************************
//  ThreadContainerBase.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  03/08/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

//------------------------------------------------------------------------------------------------------
// WARNING: This class contains very low-level logic and is optimized to have minimal locking. Before
//          making any changes, be sure to consult the author as any bugs can introduce a race
//          condition that will be very difficult to detect and fix. Additional desired functionality
//          should likely result in another class being created rather than modifying this one.
//------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Threading;

#pragma warning disable 420

namespace GSF.Threading
{
    internal abstract class ThreadContainerBase
    {
        public class CallbackArgs
        {
            public bool ShouldDispose;

            /// <summary>
            /// Gets if StartDisposal() method is the only item that triggered this run.
            /// </summary>
            public bool StartDisposalCallSuccessful;

            public void Clear()
            {
                ShouldDispose = false;
                StartDisposalCallSuccessful = false;
            }
        }

        /// <summary>
        /// State variables for the internal state machine.
        /// </summary>
        static class State
        {
            /// <summary>
            /// Indicates that the task is not running.
            /// </summary>
            public const int NotRunning = 1;

            /// <summary>
            /// Indicates that the task is scheduled to execute after a user specified delay
            /// </summary>
            public const int ScheduledToRunAfterDelay = 2;

            /// <summary>
            /// Indicates the task has been queue for immediate execution, but has not started running yet.
            /// </summary>
            public const int ScheduledToRun = 3;

            /// <summary>
            /// Once in a running state, only the worker thread can change its state.
            /// </summary>
            public const int Running = 4;

            /// <summary>
            /// Once reaching this state, the effect of RunAgain being set will no longer be valid.
            /// </summary>
            public const int AfterRunning = 5;

            /// <summary>
            /// A disposed state
            /// </summary>
            public const int Disposed = 6;

            public const int Invalid = -1;
        }

        volatile bool m_runAgain;

        /// <summary>
        /// A value less than 0 means false. 
        /// </summary>
        volatile int m_runAgainAfterDelay;

        volatile int m_state;

        WeakAction<CallbackArgs> m_callback;

        CallbackArgs m_args;

        volatile bool m_startDisposalCallSuccessful = false;

        protected ThreadContainerBase(WeakAction<CallbackArgs> callback)
        {
            m_runAgain = false;
            m_runAgainAfterDelay = -1;

            m_args = new CallbackArgs();
            m_args.Clear();

            m_callback = callback;
            m_state = State.NotRunning;
        }

        protected void OnRunning()
        {
            SpinWait wait = new SpinWait();
            while (true)
            {
                int state = m_state;
                if (state == State.ScheduledToRun && Interlocked.CompareExchange(ref m_state, State.Running, State.ScheduledToRun) == State.ScheduledToRun)
                    break;
                if (state == State.ScheduledToRunAfterDelay && Interlocked.CompareExchange(ref m_state, State.Running, State.ScheduledToRunAfterDelay) == State.ScheduledToRunAfterDelay)
                    break;
                wait.SpinOnce();
            }
            wait.Reset();

            m_runAgain = false;
            m_runAgainAfterDelay = -1;

            Thread.MemoryBarrier();

            m_args.StartDisposalCallSuccessful = m_startDisposalCallSuccessful;
            bool failedRun = !m_callback.TryInvoke(m_args);

            if (m_args.ShouldDispose || failedRun)
            {
                InternalDispose_FromWorkerThread();
                Interlocked.Exchange(ref m_state, State.Disposed);
                return;
            }

            Interlocked.Exchange(ref m_state, State.AfterRunning); //Notifies that the RunAgain and RunAgainAfterDelay variables are going to be used 
            //                                                       to make decisions. Therefore, if setting these variables after this point, modifying the state machine will be 
            //                                                       necessary

            if (m_runAgain)
            {
                Interlocked.Exchange(ref m_state, State.ScheduledToRun);
                InternalStart_FromWorkerThread();
            }
            else if (m_runAgainAfterDelay >= 0)
            {
                InternalStart_FromWorkerThread(m_runAgainAfterDelay);
                Interlocked.Exchange(ref m_state, State.ScheduledToRunAfterDelay);
            }
            else
            {
                InternalDoNothing_FromWorkerThread();
                Interlocked.Exchange(ref m_state, State.NotRunning);
            }
        }

        /// <summary>
        /// Same as Start() except notifies on the callback during a race condition that this is the one that was first to schedule the task.
        /// </summary>
        /// <returns></returns>
        public void StartDisposal()
        {
            m_runAgain = true;
            SpinWait wait = new SpinWait();
            while (true)
            {
                int state = Interlocked.CompareExchange(ref m_state, 0, 0);
                switch (state)
                {
                    case State.Disposed:
                    case State.ScheduledToRun:
                    case State.Running:
                        return;
                    case State.NotRunning:
                        if (Interlocked.CompareExchange(ref m_state, State.Invalid, State.NotRunning) == State.NotRunning)
                        {
                            m_startDisposalCallSuccessful = true;
                            InternalStart();
                            Interlocked.Exchange(ref m_state, State.ScheduledToRun);
                            return;
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (Interlocked.CompareExchange(ref m_state, State.Invalid, State.ScheduledToRunAfterDelay) == State.ScheduledToRunAfterDelay)
                        {
                            InternalCancelTimer();
                            Interlocked.Exchange(ref m_state, State.ScheduledToRun);
                            return;
                        }
                        break;
                    case State.AfterRunning:
                    case State.Invalid:
                        wait.SpinOnce();
                        break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start()
        {
            if (!m_runAgain)
                StartSlower();
        }

        void StartSlower()
        {
            m_runAgain = true;
            SpinWait wait = new SpinWait();
            while (true)
            {
                int state = Interlocked.CompareExchange(ref m_state, 0, 0);
                switch (state)
                {
                    case State.Disposed:
                    case State.ScheduledToRun:
                    case State.Running:
                        return;
                    case State.NotRunning:
                        if (Interlocked.CompareExchange(ref m_state, State.Invalid, State.NotRunning) == State.NotRunning)
                        {
                            InternalStart();
                            Interlocked.Exchange(ref m_state, State.ScheduledToRun);
                            return;
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (Interlocked.CompareExchange(ref m_state, State.Invalid, State.ScheduledToRunAfterDelay) == State.ScheduledToRunAfterDelay)
                        {
                            InternalCancelTimer();
                            Interlocked.Exchange(ref m_state, State.ScheduledToRun);
                            return;
                        }
                        break;
                    case State.AfterRunning:
                    case State.Invalid:
                        wait.SpinOnce();
                        break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start(int delay)
        {
            if (m_runAgainAfterDelay >= 0)
                return;
            StartSlower(delay);
        }

        void StartSlower(int delay)
        {
            if (delay < 0)
                throw new ArgumentException("Cannot be less than zero", "delay");

            SpinWait wait = new SpinWait();
            m_runAgainAfterDelay = delay;
            while (true)
            {
                int state = Interlocked.CompareExchange(ref m_state, 0, 0);
                switch (state)
                {
                    case State.Disposed:
                    case State.ScheduledToRun:
                    case State.Running:
                    case State.ScheduledToRunAfterDelay:
                        return;
                    case State.NotRunning:
                        if (Interlocked.CompareExchange(ref m_state, State.Invalid, State.NotRunning) == State.NotRunning)
                        {
                            InternalStart(delay);
                            Interlocked.Exchange(ref m_state, State.ScheduledToRunAfterDelay);
                        }
                        break;
                    case State.AfterRunning:
                    case State.Invalid:
                        wait.SpinOnce();
                        break;
                }
            }
        }

        protected abstract void InternalStart_FromWorkerThread();
        protected abstract void InternalStart_FromWorkerThread(int delay);
        protected abstract void InternalDispose_FromWorkerThread();
        protected abstract void InternalDoNothing_FromWorkerThread();

        protected abstract void InternalStart();
        protected abstract void InternalStart(int delay);
        protected abstract void InternalCancelTimer();

    }
}
