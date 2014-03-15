//******************************************************************************************************
//  AdvTask.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed wiAth this work for additional information regarding copyright ownership.
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
//   3/8/2014 - Added some logic behind the states so the Start() and Start(int) methods can be inlined.
//
//******************************************************************************************************

//------------------------------------------------------------------------------------------------------
// Warning: This class contains very low-level logic and optimized to have minimal locking
//          Before making any changes, be sure to consult the experts. Any bugs can introduce
//          a race condition that will be very difficult to detect and fix.
//          Additional Functional Requests should result in another class being created rather than modifying this one.
//------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Arguments passed to callbacks that share something about the 
    /// state of the task.
    /// </summary>
    public class AdvTaskEventArgs
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
        public AdvTaskEventArgs(bool isOnInterval, bool isDisposing, bool isRerun)
        {
            IsOnInterval = isOnInterval;
            IsDisposing = isDisposing;
            IsRerun = isRerun;
        }
    }

    /// <summary>
    /// Provides a time sceduled task that can either be canceled prematurely or told to execute early.
    /// </summary>
    public class AdvTask
        : IDisposable
    {
        /// <summary>
        /// A state description, any state less than or equal to this satisfies the Start() Condition.
        /// </summary>
        public const int WillRunAgain = State.TerminateQueuedRunAgain;

        /// <summary>
        /// A state description, anything less than or equal to this satisifies the Start(delay) condition.
        /// </summary>
        public const int WillRunEventually = State.ScheduledToRunAfterDelay;

        /// <summary>
        /// State variables for the internal state machine.
        /// </summary>
        private static class State
        {
            /// <summary>
            /// Indicates the task has been queue for immediate execution, but has not started running yet.
            /// </summary>
            public const int ScheduledToRun = 0;
            /// <summary>
            /// Indicates that the task is running, but has been requested to run again immediately after finishing.
            /// </summary>
            public const int RunAgain = 1;
            /// <summary>
            /// Indicates that the worker thread should run one more time before terminating.
            /// </summary>
            public const int TerminateQueuedRunAgain = 2;

            //Will Run Again before this position

            /// <summary>
            /// Indicates that the task is running, but has been requested to run again after a user specified interval.
            /// </summary>
            public const int RunAgainAfterDelay = 3;

            /// <summary>
            /// Indicates that the task is scheduled to execute after a user specified delay
            /// </summary>
            public const int ScheduledToRunAfterDelay = 4;

            //Will Run Eventually, before this position.

            /// <summary>
            /// Indicates that the task is not running.
            /// </summary>
            public const int NotRunning = 5;

            /// <summary>
            /// Indicates the task is currently running.
            /// </summary>
            public const int Running = 6;

            /// <summary>
            /// Indicates that the worker thread should terminate before running the task.
            /// </summary>
            public const int TerminateQueued = 7;

            /// <summary>
            /// Indicates that the class has been Terminated and no more execution is possible.
            /// </summary>
            public const int Terminated = 8;

            /// <summary>
            /// A state to set the machine to when addional work needs to be done before finalizing the next state.
            /// Never leave in this state, never change from this state unless you are the one who set this state.
            /// </summary>
            public const int PendingAction = 100;
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
        public volatile int m_stateMachine;

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
        /// <param name="priority">Specifies the priority of the thread that executes the task. Not valid when using a ThreadPool thread</param>
        public AdvTask(ThreadingMode threadMode, ThreadPriority priority = ThreadPriority.Normal)
        {
            if (threadMode == ThreadingMode.DedicatedForeground)
            {
                m_workerThread = new DedicatedThread(ProcessRunningState, false, priority);
            }
            else if (threadMode == ThreadingMode.DedicatedBackground)
            {
                m_workerThread = new DedicatedThread(ProcessRunningState, true, priority);
            }
            else if (threadMode == ThreadingMode.ThreadPool)
            {
                m_workerThread = new ThreadpoolThread(ProcessRunningState);
            }
            else
            {
                throw new ArgumentOutOfRangeException("threadMode");
            }
            m_stateMachine = State.NotRunning;
            m_hasQuitWaitHandle = new ManualResetEvent(false);
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="ScheduledTask"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~AdvTask()
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

        public bool Enabled
        {
            get
            {
                return true;
            }
            set
            {
                
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
        /// Immediately starts the task and will attempt to use the current thread if 
        /// it can preempt the worker thread. 
        /// Otherwise, it will stall until the worker thread finishes its work.
        /// </summary>
        public void StartWithCurrentThread()
        {
            SpinWait wait = default(SpinWait);

            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotRunning:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.NotRunning) == State.NotRunning)
                        {
                            m_workerThread.StartNow();
                            m_stateMachine = State.ScheduledToRun;
                            return;
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.ScheduledToRunAfterDelay) == State.ScheduledToRunAfterDelay)
                        {
                            m_workerThread.ShortCircuitDelayRequest();
                            m_stateMachine = State.ScheduledToRun;
                            return;
                        }
                        break;
                    case State.Running:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.RunAgain, State.Running) == State.Running)
                        {
                            return;
                        }
                        break;
                    case State.RunAgainAfterDelay:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.RunAgain, State.RunAgainAfterDelay) == State.RunAgainAfterDelay)
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
                wait.SpinOnce();
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
            //Tiny quick check that will likely be inlined by the compiler.
            if (m_stateMachine > WillRunAgain)
                StartSlower();
        }

        /// <summary>
        /// Immediately starts the task. 
        /// </summary>
        /// <remarks>
        /// If this is called after a Start(Delay) the timer will be short circuited 
        /// and the process will still start immediately. 
        /// </remarks>
        public void StartSlower()
        {
            SpinWait wait = default(SpinWait);

            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotRunning:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.NotRunning) == State.NotRunning)
                        {
                            m_workerThread.StartNow();
                            m_stateMachine = State.ScheduledToRun;
                            return;
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.ScheduledToRunAfterDelay) == State.ScheduledToRunAfterDelay)
                        {
                            m_workerThread.ShortCircuitDelayRequest();
                            m_stateMachine = State.ScheduledToRun;
                            return;
                        }
                        break;
                    case State.Running:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.RunAgain, State.Running) == State.Running)
                        {
                            return;
                        }
                        break;
                    case State.RunAgainAfterDelay:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.RunAgain, State.RunAgainAfterDelay) == State.RunAgainAfterDelay)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start(int delay)
        {
            //Tiny quick check that will likely be inlined by the compiler.
            if (m_stateMachine > WillRunEventually)
                StartSlower(delay);
        }

        /// <summary>
        /// Starts a timer to run the task after a provided interval. 
        /// </summary>
        /// <param name="delay">the delay in milliseconds before the task should run</param>
        /// <remarks>
        /// If already running on a timer, this function will do nothing. Do not use this function to
        /// reset or restart an existing timer.
        /// </remarks>
        void StartSlower(int delay)
        {
            SpinWait wait = new SpinWait();

            while (true)
            {
                switch (m_stateMachine)
                {
                    case State.NotRunning:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.NotRunning) == State.NotRunning)
                        {
                            m_workerThread.StartLater(delay);
                            m_stateMachine = State.ScheduledToRunAfterDelay;
                            return;
                        }
                        break;
                    case State.Running:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.Running) == State.Running)
                        {
                            m_workerThread.ResetTimer();
                            m_delayRequested = delay;
                            m_stateMachine = State.RunAgainAfterDelay;
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
                wait.SpinOnce();
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
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Running, State.ScheduledToRun) == State.ScheduledToRun)
                        {
                            ProcessClientCallback(isOnInterval: false, isRerun: false);
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Running, State.ScheduledToRunAfterDelay) == State.ScheduledToRunAfterDelay)
                        {
                            ProcessClientCallback(isOnInterval: true, isRerun: false);
                        }
                        break;
                    case State.Running:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.Running) == State.Running)
                        {
                            m_workerThread.ResetTimer();
                            m_stateMachine = State.NotRunning;
                            return;
                        }
                        break;
                    case State.RunAgain:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.Running, State.RunAgain) == State.RunAgain)
                        {
                            ProcessClientCallback(isOnInterval: false, isRerun: true);
                        }
                        break;
                    case State.RunAgainAfterDelay:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.RunAgainAfterDelay) == State.RunAgainAfterDelay)
                        {
                            m_workerThread.StartLater(m_delayRequested);
                            m_stateMachine = State.ScheduledToRunAfterDelay;
                            return;
                        }
                        break;
                    case State.TerminateQueued:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.TerminateQueued) == State.TerminateQueued)
                        {
                            m_workerThread.StopExecution();
                            m_hasQuitWaitHandle.Set();
                            m_stateMachine = State.Terminated;
                            ProcessClientCallbackDisposing(false, false);
                            return;
                        }
                        break;
                    case State.TerminateQueuedRunAgain:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.TerminateQueued, State.TerminateQueuedRunAgain) == State.TerminateQueuedRunAgain)
                        {
                            ProcessClientCallback(isOnInterval: false, isRerun: false);
                        }
                        break;
                    case State.Terminated:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.Terminated) == State.Terminated)
                        {
                            throw new Exception("InvalidState");
                        }
                        break;
                    case State.NotRunning:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.NotRunning) == State.NotRunning)
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
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.NotRunning) == State.NotRunning)
                        {
                            m_workerThread.StopExecution();
                            m_hasQuitWaitHandle.Set();
                            m_stateMachine = State.Terminated;
                            ProcessClientCallbackDisposing(false, false);
                            return;
                        }
                        break;
                    case State.ScheduledToRunAfterDelay:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.PendingAction, State.ScheduledToRunAfterDelay) == State.ScheduledToRunAfterDelay)
                        {
                            m_workerThread.ShortCircuitDelayRequest();
                            m_stateMachine = State.TerminateQueuedRunAgain;
                            return;
                        }
                        break;
                    case State.ScheduledToRun:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.TerminateQueuedRunAgain, State.ScheduledToRun) == State.ScheduledToRun)
                        {
                            return;
                        }
                        break;
                    case State.Running:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.TerminateQueued, State.Running) == State.Running)
                        {
                            return;
                        }
                        break;
                    case State.RunAgain:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.TerminateQueuedRunAgain, State.RunAgain) == State.RunAgain)
                        {
                            return;
                        }
                        break;
                    case State.RunAgainAfterDelay:
                        if (Interlocked.CompareExchange(ref m_stateMachine, State.TerminateQueuedRunAgain, State.RunAgainAfterDelay) == State.RunAgainAfterDelay)
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