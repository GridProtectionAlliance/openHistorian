//******************************************************************************************************
//  ScheduledTask.cs - Gbtc
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GSF.Threading
{
    #region [ Enumerations ]

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

    #endregion

    /// <summary>
    /// Represents a way to schedule a task to be executed on a separate thread immediately or after a given time delay.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class ScheduledTask : IDisposable
    {
        #region [ Members ]

        // Events

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

        // Fields
        private int m_workerThreadID;
        private readonly ThreadContainerBase m_thread;  // This cannot be null as it would cause duplicate calls to Start to throw a null reference exception
        private object m_weakCallbackToken;             // A strong reference to the callback
        private ManualResetEvent m_waitForDispose;
        private readonly object m_disposeSync;
        private volatile bool m_disposing;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a <see cref="ScheduledTask"/>.
        /// </summary>
        /// <param name="threadMode">The manner in which the scheduled task executes.</param>
        /// <param name="priority">The thread priority to assign if a dedicated thread is used. This is ignored if using the thread-pool.</param>
        public ScheduledTask(ThreadingMode threadMode = ThreadingMode.ThreadPool, ThreadPriority priority = ThreadPriority.Normal)
        {
            m_workerThreadID = -1;
            m_waitForDispose = new ManualResetEvent(false);
            m_disposeSync = new object();

            switch (threadMode)
            {
                case ThreadingMode.DedicatedForeground:
                    m_thread = new ThreadContainerDedicated(new WeakAction<ThreadContainerBase.CallbackArgs>(OnRunningCallback, out m_weakCallbackToken), false, priority);
                    break;
                case ThreadingMode.DedicatedBackground:
                    m_thread = new ThreadContainerDedicated(new WeakAction<ThreadContainerBase.CallbackArgs>(OnRunningCallback, out m_weakCallbackToken), true, priority);
                    break;
                case ThreadingMode.ThreadPool:
                    m_thread = new ThreadContainerThreadpool(new WeakAction<ThreadContainerBase.CallbackArgs>(OnRunningCallback, out m_weakCallbackToken));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("threadMode");
            }
        }

        /// <summary>
        /// Cleans up the <see cref="ThreadContainerBase"/> thread since that class likely will never be garbage collected.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        ~ScheduledTask()
        {
            //By starting the thread inside the finalizer, the ThreadContainer will exit because its weak reference will be set to null.
            m_disposing = true;
            Thread.MemoryBarrier();
            m_thread.Start();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Starts the task immediately, or if one was scheduled, starts the scheduled task immediately
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this is called after a <see cref="Start(int)"/> the timer will be canceled
        /// and the process will still start immediately. 
        /// </para>
        /// <para>
        /// This method is safe to call from any thread, including the worker thread.
        /// If disposed, this method will no nothing.
        /// </para>
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
        /// <para>
        /// If a timer is currently pending, this function will do nothing. Do not use this
        /// function to reset or restart an existing timer.
        /// </para>
        /// <para>
        /// If called while working, a subsequent timer will be scheduled, but delay will not
        /// start until after the worker has completed.
        /// </para>
        /// <para>
        /// This method is safe to call from any thread, including the worker thread.
        /// If disposed, this method will no nothing.
        /// </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start(int delay)
        {
            m_thread.Start(delay);
        }

        /// <summary>
        /// Starts the disposing process of exiting the worker thread. 
        /// </summary>
        /// <remarks>
        /// <para>Callback will be invoked one more time. Duplicate calls are ignored.</para>
        /// <para>
        /// Unless called from the worker thread, this method will block until the dispose
        /// has successfully completed.
        /// </para>
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly"), SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
        public void Dispose()
        {
            m_disposing = true;
            Thread.MemoryBarrier();
            m_thread.StartDisposal();

            if (m_workerThreadID != Thread.CurrentThread.ManagedThreadId)
                InternalDisposeAllResources();
        }

        // Completes the disposal of the class.
        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
        private void InternalDisposeAllResources()
        {
            lock (m_disposeSync)
            {
                if ((object)m_waitForDispose != null)
                {
                    m_waitForDispose.WaitOne();
                    m_waitForDispose.Dispose();
                    m_weakCallbackToken = null;
                    m_waitForDispose = null;
                    GC.SuppressFinalize(this);
                }
            }
        }

        // Executed by the worker thread
        private void OnRunningCallback(ThreadContainerBase.CallbackArgs args)
        {
            bool disposing = m_disposing;

            if (disposing && args.StartDisposalCallSuccessful)
            {
                args.ShouldDispose = true;
                TryCallback(ScheduledTaskRunningReason.Disposing);
                return;
            }

            TryCallback(ScheduledTaskRunningReason.Running);

            if (disposing)
            {
                args.ShouldDispose = true;
                TryCallback(ScheduledTaskRunningReason.Disposing);
            }
        }

        private void TryCallback(ScheduledTaskRunningReason args)
        {
            m_workerThreadID = Thread.CurrentThread.ManagedThreadId;

            try
            {
                if ((object)Running != null)
                    Running(this, new EventArgs<ScheduledTaskRunningReason>(args));
            }
            catch (Exception ex)
            {
                try
                {
                    if ((object)UnhandledException != null)
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
                    if ((object)Disposing != null)
                        Disposing(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    try
                    {
                        if ((object)UnhandledException != null)
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

            m_workerThreadID = -1;
        }

        #endregion
    }
}
