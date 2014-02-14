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

//------------------------------------------------------------------------------------------------------
// Warning: This class contains very low-level logic and optimized to have minimal locking
//          Before making any changes, be sure to consult the experts. Any bugs can introduce
//          a race condition that will be very difficult to detect and fix.
//          Additional Functional Requests should result in another class being created rather than modifying this one.
//------------------------------------------------------------------------------------------------------

using System;
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
    /// Arguments passed to callbacks that share something about the 
    /// state of the task.
    /// </summary>
    public class ScheduledTaskEventArgs
        : EventArgs
    {
        /// <summary>
        /// True if execution is the result of an interval timing out.
        /// False if running immediately.
        /// </summary>
        public bool IsOnInterval
        {
            get;
            private set;
        }

        /// <summary>
        /// True if this is the last time a function is being executed. 
        /// </summary>
        public bool IsDisposing
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns true if this is an immediate rerun. False otherwise.
        /// </summary>
        public bool IsRerun
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new <see cref="ScheduledTaskEventArgs"/>
        /// </summary>
        /// <param name="isOnInterval"></param>
        /// <param name="isDisposing"></param>
        /// <param name="isRerun"></param>
        public ScheduledTaskEventArgs(bool isOnInterval, bool isDisposing, bool isRerun)
        {
            IsOnInterval = isOnInterval;
            IsDisposing = isDisposing;
            IsRerun = isRerun;
        }
    }

    /// <summary>
    /// Provides a time sceduled task that can either be canceled prematurely or told to execute early.
    /// </summary>
    public class ScheduledTask
        : IDisposable
    {

        /// <summary>
        /// State variables for the internal state machine.
        /// </summary>
        private static class State
        {
            /// <summary>
            /// A state to set the machine to when addional work needs to be done before finalizing the next state.
            /// Never leave in this state, never change from this state unless you are the one who set this state.
            /// </summary>
            public const int PendingAction = 0;

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
            /// Indicates the task is currently running.
            /// </summary>
            public const int Running = 4;

            /// <summary>
            /// Indicates that the task is running, but has been requested to run again immediately after finishing.
            /// </summary>
            public const int RunAgain = 5;

            /// <summary>
            /// Indicates that the task is running, but has been requested to run again after a user specified interval.
            /// </summary>
            public const int RunAgainAfterDelay = 6;

            /// <summary>
            /// Indicates that the worker thread should run one more time before terminating.
            /// </summary>
            public const int TerminateQueuedRunAgain = 7;

            /// <summary>
            /// Indicates that the worker thread should terminate before running the task.
            /// </summary>
            public const int TerminateQueued = 8;

            /// <summary>
            /// Indicates that the class has been Terminated and no more execution is possible.
            /// </summary>
            public const int Terminated = 9;
        }

        /// <summary>
        /// The thread that runs the task.
        /// </summary>
        private readonly CustomThreadBase m_workerThread;
        /// <summary>
        /// The wait handle that signals after the worker thread has been completed and is now disposed.
        /// </summary>
        private ManualResetEvent m_hasQuitWaitHandle;
        /// <summary>
        /// The current state of the state machine.
        /// </summary>
        private readonly StateMachine m_stateMachine;

        /// <summary>
        /// The delay time in milliseconds that has been requested when a RunAgainAfterDelay has been queued
        /// </summary>
        private volatile int m_delayRequested;

        /// <summary>
        /// Only occurs at most once when the <see cref="ScheduledTask"/> has been disposed.
        /// </summary>
        public event EventHandler<ScheduledTaskEventArgs> OnDispose;

        /// <summary>
        /// Occurs every time the schedule runs. 
        /// </summary>
        /// <remarks>
        /// This occurs after <see cref="OnEvent"/>.
        /// </remarks>
        public event EventHandler<ScheduledTaskEventArgs> OnRunWorker;

        /// <summary>
        /// Occurs when disposing or working. 
        /// </summary>
        /// <remarks>
        /// This event occurs before <see cref="OnDispose"/> and before <see cref="OnRunWorker"/>.
        /// </remarks>
        public event EventHandler<ScheduledTaskEventArgs> OnEvent;

        /// <summary>
        /// Occurs when unhandled exceptions occur in the worker or disposed thread;
        /// </summary>
        public event UnhandledExceptionEventHandler OnException;


        /// <summary>
        /// Creates a task that can be manually scheduled to run.
        /// </summary>
        /// <param name="threadMode">
        /// Determines if this thread is to be a foreground or background thread.
        /// </param>
        public ScheduledTask(ThreadingMode threadMode)
        {
            if (threadMode == ThreadingMode.DedicatedForeground)
            {
                m_workerThread = new DedicatedThread(ProcessRunningState, false);
            }
            else if (threadMode == ThreadingMode.DedicatedBackground)
            {
                m_workerThread = new DedicatedThread(ProcessRunningState, true);
            }
            else if (threadMode == ThreadingMode.ThreadPool)
            {
                m_workerThread = new ThreadpoolThread(ProcessRunningState);
            }
            else
            {
                throw new ArgumentOutOfRangeException("threadMode");
            }
            m_stateMachine = new StateMachine(State.NotRunning);
            m_hasQuitWaitHandle = new ManualResetEvent(false);
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="ScheduledTask"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~ScheduledTask()
        {
            Terminate();
        }

        /// <summary>
        /// Stops all future calls to this class, and waits for the worker thread to quit before returning. 
        /// Releases all the resources used by the <see cref="ScheduledTask"/> object.
        /// </summary>
        public void Dispose()
        {
            Terminate();

            //Potential for extended contention on this lock, but that's ok because it would have to wait for
            //m_hasQuitWaitHandle anyway.
            lock (m_workerThread)
            {
                if (m_hasQuitWaitHandle != null)
                {
                    m_hasQuitWaitHandle.WaitOne();
                    m_hasQuitWaitHandle = null;
                }
            }
        }

        /// <summary>
        /// Disposes this class, but does not wait for the worker to finish.
        /// </summary>
        public void DisposeWithoutWait()
        {
            Terminate();
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
            SpinWait wait = default(SpinWait);

            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotRunning:
                        if (m_stateMachine.TryChangeState(State.NotRunning, State.PendingAction))
                        {
                            m_workerThread.StartNow();
                            m_stateMachine.SetState(State.ScheduledToRun);
                            return;
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (m_stateMachine.TryChangeState(State.ScheduledToRunAfterDelay, State.PendingAction))
                        {
                            m_workerThread.ShortCircuitDelayRequest();
                            m_stateMachine.SetState(State.ScheduledToRun);
                            return;
                        }
                        break;
                    case State.Running:
                        if (m_stateMachine.TryChangeState(State.Running, State.RunAgain))
                        {
                            return;
                        }
                        break;
                    case State.RunAgainAfterDelay:
                        if (m_stateMachine.TryChangeState(State.RunAgainAfterDelay, State.RunAgain))
                        {
                            return;
                        }
                        break;
                    case State.RunAgain:
                    case State.ScheduledToRun:
                    case State.Terminated:
                    case State.TerminateQueued:
                    case State.TerminateQueuedRunAgain:
                        return;
                    case State.PendingAction:
                        break;
                }

                //Interlocked.Increment(ref m_spinLock);
                //wait.SpinOnce();
            }
        }

        /// <summary>
        /// Starts a timer to run the task after a provided interval. 
        /// </summary>
        /// <param name="delay">the delay</param>
        /// <remarks>
        /// If already running on a timer, this function will do nothing. Do not use this function to
        /// reset or restart an existing timer.
        /// </remarks>
        public void Start(TimeSpan delay)
        {
            Start(delay.Milliseconds);
        }

        /// <summary>
        /// Starts a timer to run the task after a provided interval. 
        /// </summary>
        /// <param name="delay">the delay in milliseconds before the task should run</param>
        /// <remarks>
        /// If already running on a timer, this function will do nothing. Do not use this function to
        /// reset or restart an existing timer.
        /// </remarks>
        public void Start(int delay)
        {
            SpinWait wait = new SpinWait();

            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotRunning:
                        if (m_stateMachine.TryChangeState(State.NotRunning, State.PendingAction))
                        {
                            m_workerThread.StartLater(delay);
                            m_stateMachine.SetState(State.ScheduledToRunAfterDelay);
                            return;
                        }
                        break;
                    case State.Running:
                        if (m_stateMachine.TryChangeState(State.Running, State.PendingAction))
                        {
                            m_workerThread.ResetTimer();
                            m_delayRequested = delay;
                            m_stateMachine.SetState(State.RunAgainAfterDelay);
                            return;
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                    case State.ScheduledToRun:
                    case State.RunAgain:
                    case State.RunAgainAfterDelay:
                    case State.Terminated:
                    case State.TerminateQueued:
                    case State.TerminateQueuedRunAgain:
                        return;
                    case State.PendingAction:
                        break;
                }
                //wait.SpinOnce();
            }
        }


        /// <summary>
        /// The method executed in the state: Running. This is invoked by the 
        /// thread when it is time to run this state.
        /// </summary>
        private void ProcessRunningState()
        {
            SpinWait wait = new SpinWait();

            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.ScheduledToRun:
                        if (m_stateMachine.TryChangeState(State.ScheduledToRun, State.Running))
                        {
                            ProcessClientCallback(isOnInterval: false, isRerun: false);
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (m_stateMachine.TryChangeState(State.ScheduledToRunAfterDelay, State.Running))
                        {
                            ProcessClientCallback(isOnInterval: true, isRerun: false);
                        }
                        break;
                    case State.Running:
                        if (m_stateMachine.TryChangeState(State.Running, State.PendingAction))
                        {
                            m_workerThread.ResetTimer();
                            m_stateMachine.SetState(State.NotRunning);
                            return;
                        }
                        break;
                    case State.RunAgain:
                        if (m_stateMachine.TryChangeState(State.RunAgain, State.Running))
                        {
                            ProcessClientCallback(isOnInterval: false, isRerun: true);
                        }
                        break;
                    case State.RunAgainAfterDelay:
                        if (m_stateMachine.TryChangeState(State.RunAgainAfterDelay, State.PendingAction))
                        {
                            m_workerThread.StartLater(m_delayRequested);
                            m_stateMachine.SetState(State.ScheduledToRunAfterDelay);
                            return;
                        }
                        break;
                    case State.TerminateQueued:
                        if (m_stateMachine.TryChangeState(State.TerminateQueued, State.PendingAction))
                        {
                            m_workerThread.StopExecution();
                            m_hasQuitWaitHandle.Set();
                            m_stateMachine.SetState(State.Terminated);
                            ProcessClientCallbackDisposing(false, false);
                            return;
                        }
                        break;
                    case State.TerminateQueuedRunAgain:
                        if (m_stateMachine.TryChangeState(State.TerminateQueuedRunAgain, State.TerminateQueued))
                        {
                            ProcessClientCallback(isOnInterval: false, isRerun: false);
                        }
                        break;
                    case State.Terminated:
                        if (m_stateMachine.TryChangeState(State.Terminated, State.PendingAction))
                        {
                            throw new Exception("InvalidState");
                        }
                        break;
                    case State.NotRunning:
                        if (m_stateMachine.TryChangeState(State.NotRunning, State.PendingAction))
                        {
                            throw new Exception("InvalidState");
                        }
                        break;
                    case State.PendingAction:
                        break;
                }
                //wait.SpinOnce();
            }
        }

        /// <summary>
        /// Terminates the execution of the worker gracefully.
        /// </summary>
        void Terminate()
        {
            SpinWait wait = new SpinWait();
            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotRunning:
                        if (m_stateMachine.TryChangeState(State.NotRunning, State.PendingAction))
                        {
                            m_workerThread.StopExecution();
                            m_hasQuitWaitHandle.Set();
                            m_stateMachine.SetState(State.Terminated);
                            ProcessClientCallbackDisposing(false, false);
                            return;
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (m_stateMachine.TryChangeState(State.ScheduledToRunAfterDelay, State.PendingAction))
                        {
                            m_workerThread.ShortCircuitDelayRequest();
                            m_stateMachine.SetState(State.TerminateQueuedRunAgain);
                            return;
                        }
                        break;
                    case State.ScheduledToRun:
                        if (m_stateMachine.TryChangeState(State.ScheduledToRun, State.TerminateQueuedRunAgain))
                        {
                            return;
                        }
                        break;
                    case State.Running:
                        if (m_stateMachine.TryChangeState(State.Running, State.TerminateQueued))
                        {
                            return;
                        }
                        break;
                    case State.RunAgain:
                        if (m_stateMachine.TryChangeState(State.RunAgain, State.TerminateQueuedRunAgain))
                        {
                            return;
                        }
                        break;
                    case State.RunAgainAfterDelay:
                        if (m_stateMachine.TryChangeState(State.RunAgainAfterDelay, State.TerminateQueuedRunAgain))
                        {
                            return;
                        }
                        break;
                    case State.Terminated:
                    case State.TerminateQueued:
                    case State.TerminateQueuedRunAgain:
                        return;
                    case State.PendingAction:
                        break;

                }
                //wait.SpinOnce();
            }
        }

        private void ProcessClientCallbackDisposing(bool isOnInterval, bool isRerun)
        {
            var eventArgs = new ScheduledTaskEventArgs(isOnInterval, true, isRerun);

            if (!eventArgs.IsDisposing)
                throw new Exception("Internal state error");

            try
            {
                if (OnEvent != null)
                    OnEvent(this, eventArgs);
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

            try
            {
                if (OnDispose != null)
                    OnDispose(this, eventArgs);
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

        private void ProcessClientCallback(bool isOnInterval, bool isRerun)
        {
            var eventArgs = new ScheduledTaskEventArgs(isOnInterval, false, isRerun);

            if (eventArgs.IsDisposing)
                throw new Exception();

            try
            {
                if (OnEvent != null)
                    OnEvent(this, eventArgs);
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

            try
            {
                if (OnRunWorker != null)
                    OnRunWorker(this, eventArgs);
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