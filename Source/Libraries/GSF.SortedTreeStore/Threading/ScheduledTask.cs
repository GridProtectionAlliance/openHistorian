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

        private enum NextAction
        {
            RunAgain,
            Quit
        }

        /// <summary>
        /// State variables for the internal state machine.
        /// </summary>
        private static class State
        {
            /// <summary>
            /// A prestage event that is set right before NotRunning is set.
            /// </summary>
            public const int NotRunningPending = 0;
            /// <summary>
            /// Indicates that the task is not running.
            /// </summary>
            public const int NotRunning = 1;

            /// <summary>
            /// A prestage event that is set right before ScheduledToRunAfterDelay is set.
            /// </summary>
            public const int ScheduledToRunAfterDelayPending = 2;
            /// <summary>
            /// Indicates that the task is scheduled to execute after a user specified delay
            /// </summary>
            public const int ScheduledToRunAfterDelay = 3;

            /// <summary>
            /// A prestage event that is set right before ScheduledToRun is set.
            /// </summary>
            public const int ScheduledToRunPending = 4;
            /// <summary>
            /// Indicates the task has been queue for immediate execution, but has not started running yet.
            /// </summary>
            public const int ScheduledToRun = 5;
            /// <summary>
            /// Indicates the task is currently running.
            /// </summary>
            public const int Running = 6;
            /// <summary>
            /// Indicates that the task is running, but has been requested to run again immediately after finishing.
            /// </summary>
            public const int RunAgain = 7;

            /// <summary>
            /// A prestage event that is set right before RunAgainAfterDelay is set.
            /// </summary>
            public const int RunAgainAfterDelayPending = 8;
            /// <summary>
            /// Indicates that the task is running, but has been requested to run again after a user specified interval.
            /// </summary>
            public const int RunAgainAfterDelay = 9;
            /// <summary>
            /// Indicates that the class has been disposed and no more execution is possible.
            /// </summary>
            public const int Disposed = 10;
        }

        /// <summary>
        /// The thread that runs the task.
        /// </summary>
        private readonly CustomThreadBase m_workerThread;
        /// <summary>
        /// The wait handle that signals after the worker thread has been completed and is now disposed.
        /// </summary>
        private readonly ManualResetEvent m_hasQuitWaitHandle;
        /// <summary>
        /// The current state of the state machine.
        /// </summary>
        private readonly StateMachine m_stateMachine;

        /// <summary>
        /// The delay time in milliseconds that has been requested when a RunAgainAfterDelay has been queued
        /// </summary>
        private volatile int m_delayRequested;
        /// <summary>
        /// A non-zero value means true.  Occurs when Disposing has been initiated for the task.
        /// </summary>
        private volatile int m_disposeCalled = 0;

        /// <summary>
        /// Occurs when the class has been completely disposed.
        /// </summary>
        private volatile bool m_completelyDisposed;

        /// <summary>
        /// Used as a lightweight spinlock.
        /// </summary>
        private int m_spinLock;

        /// <summary>
        /// Only occurs at most once when the <see cref="ScheduledTask"/> has been disposed.
        /// This will only occur if <see cref="Dispose"/> method is called. It will not be raised
        /// if either the finalizer disposes of the class, or if <see cref="DisposeWithoutWait"/> is called.
        /// </summary>
        /// <remarks>
        /// The thread that executes this OnDispose event is the same thread that called <see cref="Dispose"/> method.
        /// </remarks>
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
                m_workerThread = new DedicatedThread(ProcessStateRunning, false);
            }
            else if (threadMode == ThreadingMode.DedicatedBackground)
            {
                m_workerThread = new DedicatedThread(ProcessStateRunning, true);
            }
            else if (threadMode == ThreadingMode.ThreadPool)
            {
                m_workerThread = new ThreadpoolThread(ProcessStateRunning);
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
            DisposeMethod(waitForExit: false);
        }

        /// <summary>
        /// Gets if this class is trying to dispose. This is useful for the worker thread to know.
        /// So it can try to quit early if need be since the disposing thread will be blocked 
        /// until the worker has complete. 
        /// </summary>
        public bool DisposedCalled
        {
            get
            {
                return m_disposeCalled != 0;
            }
        }

        /// <summary>
        /// Determines when everything has been disposed. 
        /// </summary>
        public bool CompletelyDisposed
        {
            get
            {
                return m_completelyDisposed;
            }
        }

        /// <summary>
        /// Stops all future calls to this class, and waits for the worker thread to quit before returning. 
        /// Releases all the resources used by the <see cref="ScheduledTask"/> object.
        /// </summary>
        public void Dispose()
        {
            DisposeMethod(waitForExit: true);
        }

        /// <summary>
        /// Disposes this class, but does not wait for the worker to finish.
        /// </summary>
        public void DisposeWithoutWait()
        {
            DisposeMethod(waitForExit: false);
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
            if (m_completelyDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            SpinWait wait = default(SpinWait);

            while (true)
            {
                if (m_disposeCalled != 0)
                    return;
                switch (m_stateMachine)
                {
                    case State.NotRunning:
                        if (m_stateMachine.TryChangeState(State.NotRunning, State.ScheduledToRunPending))
                        {
                            m_workerThread.StartNow();
                            m_stateMachine.SetState(State.ScheduledToRun);
                            return;
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (m_stateMachine.TryChangeState(State.ScheduledToRunAfterDelay, State.ScheduledToRunPending))
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
                    case State.ScheduledToRunPending:
                    case State.ScheduledToRun:
                    case State.Disposed:
                        return;
                    case State.RunAgainAfterDelayPending:
                    case State.NotRunningPending:
                    case State.ScheduledToRunAfterDelayPending:
                        break;
                    default:
                        throw new Exception("Unknown State");
                }

                //Interlocked.Increment(ref m_spinLock);
                wait.SpinOnce();
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
            if (m_completelyDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            SpinWait wait = new SpinWait();

            while (true)
            {
                if (m_disposeCalled != 0)
                    return;

                switch (m_stateMachine)
                {
                    case State.NotRunning:
                        if (m_stateMachine.TryChangeState(State.NotRunning, State.ScheduledToRunAfterDelayPending))
                        {
                            m_workerThread.StartLater(delay);
                            m_stateMachine.SetState(State.ScheduledToRunAfterDelay);
                            return;
                        }
                        break;
                    case State.Running:
                        if (m_stateMachine.TryChangeState(State.Running, State.RunAgainAfterDelayPending))
                        {
                            m_workerThread.ResetTimer();
                            m_delayRequested = delay;
                            m_stateMachine.SetState(State.RunAgainAfterDelay);
                            return;
                        }
                        break;
                    case State.ScheduledToRunAfterDelayPending:
                    case State.ScheduledToRunAfterDelay:
                    case State.ScheduledToRunPending:
                    case State.ScheduledToRun:
                    case State.RunAgain:
                    case State.RunAgainAfterDelayPending:
                    case State.RunAgainAfterDelay:
                    case State.Disposed:
                        return;
                    case State.NotRunningPending:
                        break;
                    default:
                        throw new Exception("Unknown State");
                }
                wait.SpinOnce();
            }
        }


        /// <summary>
        /// The method executed in the state: Running. This is invoked by the 
        /// thread when it is time to run this state.
        /// </summary>
        private void ProcessStateRunning()
        {
            SpinWait wait = new SpinWait();
            while (true)
            {
                bool wasScheduled = m_stateMachine.TryChangeState(State.ScheduledToRunAfterDelay, State.Running);
                bool wasRunNow = m_stateMachine.TryChangeState(State.ScheduledToRun, State.Running);
                bool isRerun = false;
                if (wasScheduled || wasRunNow)
                {
                    while (true)
                    {
                        if (m_disposeCalled != 0)
                        {
                            SetStateToDisposeWorkerThread();
                            m_workerThread.Dispose();
                            m_hasQuitWaitHandle.Set();
                            m_completelyDisposed = true;
                            return;
                        }

                        ProcessClientCallback(new ScheduledTaskEventArgs(isOnInterval: wasScheduled, isDisposing: false, isRerun: isRerun));

                        if (CheckAfterRunAction() == NextAction.Quit)
                        {
                            return;
                        }
                        isRerun = true;
                        wasScheduled = false;
                        wasRunNow = false;
                    }
                }
                wait.SpinOnce();
            }
        }

        /// <summary>
        /// Occurs after the <see cref="ProcessStateRunning"/>
        /// to set the machine in its next state.
        /// </summary>
        /// <returns></returns>
        private NextAction CheckAfterRunAction()
        {
            SpinWait wait = new SpinWait();

            while (true)
            {
                if (m_disposeCalled != 0)
                {
                    SetStateToDisposeWorkerThread();
                    m_workerThread.Dispose();
                    m_hasQuitWaitHandle.Set();
                    m_completelyDisposed = true;
                    return NextAction.Quit;
                }

                switch (m_stateMachine)
                {
                    case State.Running:
                        if (m_stateMachine.TryChangeState(State.Running, State.NotRunningPending))
                        {
                            m_workerThread.ResetTimer();
                            m_stateMachine.SetState(State.NotRunning);
                            return NextAction.Quit;
                        }
                        break;
                    case State.RunAgain:
                        if (m_stateMachine.TryChangeState(State.RunAgain, State.Running))
                        {
                            return NextAction.RunAgain;
                        }
                        break;
                    case State.RunAgainAfterDelay:
                        if (m_stateMachine.TryChangeState(State.RunAgainAfterDelay, State.ScheduledToRunAfterDelayPending))
                        {
                            m_workerThread.StartLater(m_delayRequested);
                            m_stateMachine.SetState(State.ScheduledToRunAfterDelay);
                            return NextAction.Quit;
                        }
                        break;
                    case State.Disposed:
                    case State.RunAgainAfterDelayPending:
                        break;
                    default:
                        throw new Exception("Should never be in this state.");
                }
                if (m_disposeCalled != 0)
                {
                    SetStateToDisposeWorkerThread();
                    m_workerThread.Dispose();
                    m_hasQuitWaitHandle.Set();
                    m_completelyDisposed = true;
                    return NextAction.Quit;
                }
                wait.SpinOnce();
            }
        }

        /// <summary>
        /// Attempts to clean up all resources used by this class. This method is similiar to <see cref="Dispose"/>
        /// except it does not wait for the worker thread to actually quit, and the disposing callback is not executed.
        /// </summary>
        void DisposeMethod(bool waitForExit)
        {
            SpinWait wait = new SpinWait();

            if (m_disposeCalled == 0)
            {
                //Prevents duplicate calls in an async condition.
                if (Interlocked.Increment(ref m_disposeCalled) == 1)
                {
                    Thread.MemoryBarrier();
                    while (true)
                    {
                        switch (m_stateMachine)
                        {
                            case State.NotRunning:
                                if (m_stateMachine.TryChangeState(State.NotRunning, State.Disposed))
                                {
                                    m_workerThread.Dispose();
                                    m_completelyDisposed = true;
                                    m_hasQuitWaitHandle.Set();
                                    if (waitForExit)
                                    {
                                        ProcessClientCallbackDisposing(new ScheduledTaskEventArgs(false, true, false));
                                    }
                                    return;
                                }
                                break;
                            case State.ScheduledToRunAfterDelay:
                                if (m_stateMachine.TryChangeState(State.ScheduledToRunAfterDelay, State.ScheduledToRunPending))
                                {
                                    m_workerThread.ShortCircuitDelayRequest();
                                    m_stateMachine.SetState(State.ScheduledToRun);
                                    if (waitForExit)
                                    {
                                        m_hasQuitWaitHandle.WaitOne();
                                        ProcessClientCallbackDisposing(new ScheduledTaskEventArgs(false, true, false));
                                    }
                                    return;
                                }
                                break;
                            case State.ScheduledToRun:
                            case State.Running:
                            case State.RunAgain:
                                if (waitForExit)
                                {
                                    m_hasQuitWaitHandle.WaitOne();
                                    ProcessClientCallbackDisposing(new ScheduledTaskEventArgs(false, true, false));
                                }
                                return;
                            case State.RunAgainAfterDelay:
                                m_stateMachine.TryChangeState(State.RunAgainAfterDelay, State.RunAgain);
                                break;
                        }
                        wait.SpinOnce();
                    }
                }

            }
        }


        /// <summary>
        /// Safely changes the state of the worker to Dispose. 
        /// This should only be called by the working thread.
        /// </summary>
        private void SetStateToDisposeWorkerThread()
        {
            SpinWait wait = new SpinWait();

            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotRunning:
                        if (m_stateMachine.TryChangeState(State.NotRunning, State.Disposed))
                            return;
                        break;
                    case State.Running:
                        if (m_stateMachine.TryChangeState(State.Running, State.Disposed))
                            return;
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (m_stateMachine.TryChangeState(State.ScheduledToRunAfterDelay, State.Disposed))
                            return;
                        break;
                    case State.ScheduledToRun:
                        if (m_stateMachine.TryChangeState(State.ScheduledToRun, State.Disposed))
                            return;
                        break;
                    case State.RunAgain:
                        if (m_stateMachine.TryChangeState(State.RunAgain, State.Disposed))
                            return;
                        break;
                    case State.RunAgainAfterDelay:
                        if (m_stateMachine.TryChangeState(State.RunAgainAfterDelay, State.Disposed))
                            return;
                        break;
                    case State.Disposed:
                        return;
                    case State.ScheduledToRunPending:
                    case State.RunAgainAfterDelayPending:
                    case State.ScheduledToRunAfterDelayPending:
                    case State.NotRunningPending:
                        break;
                    default:
                        throw new Exception("Unknown State");
                }
                wait.SpinOnce();
            }
        }

        private void ProcessClientCallbackDisposing(ScheduledTaskEventArgs eventArgs)
        {
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


        private void ProcessClientCallback(ScheduledTaskEventArgs eventArgs)
        {
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