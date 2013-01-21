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
    public enum ThreadingMode
    {
        Foreground,
        Background
    }
    public class ScheduledTaskEventArgs : EventArgs
    {
        /// <summary>
        /// True if execution is the result of an interval timing out.
        /// False if running immediately.
        /// </summary>
        public bool IsOnInterval { get; private set; }
        /// <summary>
        /// True if this is the last time a function is being executed. 
        /// </summary>
        public bool IsDisposing { get; private set; }
        /// <summary>
        /// Returns true if this is an immediate rerun. False otherwise.
        /// </summary>
        public bool IsRerun { get; private set; }

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
    public partial class ScheduledTask : IDisposable
    {
        volatile bool m_disposed;
        volatile bool m_isDisposing;
        //Class must be wrapped inside of another class so a Finalize method
        //will actually be called when this task is no longer being referenced.
        Internal m_internal;

        /// <summary>
        /// Only occurs once when the schedule is being disposed
        /// </summary>
        public event EventHandler<ScheduledTaskEventArgs> OnDispose;
        /// <summary>
        /// Occurs every time the schedule runs.
        /// </summary>
        public event EventHandler<ScheduledTaskEventArgs> OnRunWorker;
        /// <summary>
        /// Occurs when disposing or working.
        /// </summary>
        public event EventHandler<ScheduledTaskEventArgs> OnEvent;
        /// <summary>
        /// Occurs when unhandled exceptions in the worker or disposed thread;
        /// </summary>
        public event UnhandledExceptionEventHandler OnException;

        /// <summary>
        /// Creates a task that can be manually scheduled to run.
        /// </summary>
        /// <param name="threadMode">Determines if this thread is to be a foreground or background thread.
        /// A background thread will run on the thread pool while a foreground thread will be a dedicated thread.</param>
        public ScheduledTask(ThreadingMode threadMode)
        {
            m_internal = new Internal(Callback, threadMode == ThreadingMode.Foreground);
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="ScheduledTask"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~ScheduledTask()
        {
            m_isDisposing = true;
            if (m_internal != null)
                m_internal.Finalized();
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

        public void DisposeWithoutWait()
        {
            m_isDisposing = true;
            if (m_internal != null)
                m_internal.Finalized();
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

        /// <summary>
        /// Immediately starts the task. Will not signal the class to run again if
        /// it is currently running.
        /// </summary>
        /// <remarks>
        /// If this is called after a Start(Delay) the timer will be short circuited 
        /// and the process will still start immediately. 
        /// </remarks>
        public void StartIfNotRunning()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            m_internal.StartIfNotRunning();
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

        void Callback()
        {
            var args = m_internal.EventArgs;
            if (args.IsDisposing)
            {
                try
                {
                    if (OnDispose != null)
                        OnDispose(this, args);
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
            else
            {
                try
                {
                    if (OnRunWorker != null)
                        OnRunWorker(this, args);
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

            try
            {
                if (OnEvent != null)
                    OnEvent(this, args);
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

    }
}
