//******************************************************************************************************
//  ScheduledTask.cs - Gbtc
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
    public enum ScheduledTaskRunningReason
    {
        /// <summary>
        /// A normal run was scheduled.
        /// </summary>
        Running,
        /// <summary>
        /// Dispose was called and execution will terminate after this function call.
        /// </summary>
        Disposing,
    }

    /// <summary>
    /// A way to schedule a task to be executed on a seperate thread immediately, or after a given time delay.
    /// </summary>
    public class ScheduledTask
        : IDisposable
    {
        /// <summary>
        /// Occurs every time the task should run.
        /// </summary>
        public event EventHandler<EventArgs<ScheduledTaskRunningReason>> Running;

        /// <summary>
        /// Occurs right before this task is disposed.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Occurs when <see cref="Running"/> or <see cref="Disposing"/> event throws an exception.
        /// </summary>
        public event EventHandler<EventArgs<Exception>> UnhandledException;

        object m_disposeSync;
        volatile bool m_disposing;

        // This cannot be null, because it would cause duplicate calls to Start to throw a null referenced exception.
        readonly ThreadContainerBase m_thread;
        // A strong reference to the callback
        object m_weakCallbackToken;
        ManualResetEvent m_waitForDispose;

        /// <summary>
        /// Creates a <see cref="ScheduledTask"/>
        /// </summary>
        /// <param name="threadMode">the mannor in which the scheduled task executes</param>
        /// <param name="priority">the thread priority to assign if a dedicated thread is used. This is ignored if using the threadpool</param>
        public ScheduledTask(ThreadingMode threadMode = ThreadingMode.ThreadPool, ThreadPriority priority = ThreadPriority.Normal)
        {
            m_waitForDispose = new ManualResetEvent(false);
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

        /// <summary>
        /// Cleans up the <see cref="ThreadContainerBase"/> thread since that class likely will never be garbage collected.
        /// </summary>
        ~ScheduledTask()
        {
            //By starting the thread inside the finalizer, the ThreadContainer will exit because its weak reference will be set to null.
            m_disposing = true;
            Thread.MemoryBarrier();
            m_thread.Start();
        }

        /// <summary>
        /// Executed by the worker thread
        /// </summary>
        void OnRunningCallback(ThreadContainerBase.CallbackArgs args)
        {
            if (m_disposing)
            {
                args.ShouldDispose = true;
                TryCallback(ScheduledTaskRunningReason.Disposing);
                return;
            }

            TryCallback(ScheduledTaskRunningReason.Running);

            if (m_disposing)
            {
                args.ShouldDispose = true;
                TryCallback(ScheduledTaskRunningReason.Disposing);
                return;
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
            m_thread.Start(delay);
        }

        /// <summary>
        /// Starts the disposing process of exiting the worker thread. Will invoke the callback one more time.
        /// Duplicate calls are ignored. This method will block until the dispose has successfully completed.
        /// </summary>
        public void Dispose()
        {
            m_disposing = true;
            Thread.MemoryBarrier();
            m_thread.Start();
            InternalDisposeAllResources();
        }

        /// <summary>
        /// Completes the disposal of the class.
        /// </summary>
        void InternalDisposeAllResources()
        {
            lock (m_disposeSync)
            {
                if (m_waitForDispose != null)
                {
                    m_waitForDispose.WaitOne();
                    m_waitForDispose.Dispose();
                    m_weakCallbackToken = null;
                    m_waitForDispose = null;
                    GC.SuppressFinalize(this);
                }
            }
        }

        void TryCallback(ScheduledTaskRunningReason args)
        {
            try
            {
                if (Running != null)
                    Running(this, new EventArgs<ScheduledTaskRunningReason>(args));
            }
            catch (Exception ex)
            {
                try
                {
                    if (UnhandledException != null)
                        UnhandledException(this, new EventArgs<Exception>(ex));
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.ToString());
                }
            }

            if (args == ScheduledTaskRunningReason.Disposing)
            {
                try
                {
                    if (Disposing != null)
                        Disposing(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (UnhandledException != null)
                            UnhandledException(this, new EventArgs<Exception>(ex));
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
