//******************************************************************************************************
//  AsyncProcess.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  12/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;

//ToDo: clean up and implement this class.
namespace openHistorian.Engine.ArchiveWriters
{
    /// <summary>
    /// Class assists in the asynchronous processing that goes on in the archive writer.
    /// </summary>
    internal class AsyncProcess<T>
        where T : EventArgs
    {
        public event EventHandler<T> BeforeRun;
        public event EventHandler<T> Running;
        public event EventHandler<T> AfterRun;
        public event EventHandler<T> HasQuit;
        
        bool m_stopExecuting;
        T m_objectState;
        ManualResetEvent m_resetEvent;
        CurrentState m_state;
        bool m_runAgain;
        object m_syncRoot;
        RegisteredWaitHandle m_registeredHandle;

        bool m_delayRequested;
        TimeSpan m_timeDelay;

        enum CurrentState
        {
            /// <summary>
            /// Means that there is a thread that is actively running the process.
            /// </summary>
            IsRunning,
            /// <summary>
            /// Means that there has been a thread assigned to execute this process, however, it is waiting 
            /// on the thread pool for a signal or a timeout.
            /// </summary>
            IsWaiting,
            /// <summary>
            /// Means that no threads are currently processing the user's request.
            /// </summary>
            HasExited
        }

        public AsyncProcess(T initialState)
        {
            m_runAgain = false;
            m_objectState = initialState;
            m_state = CurrentState.HasExited;
            m_syncRoot = new object();
            m_resetEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Makes sure that the process begins immediately if it is currently not running.
        /// If it is running, it tells the process to run at least one more time. 
        /// </summary>
        public void Run()
        {
            lock (m_syncRoot)
            {
                switch (m_state)
                {
                    case CurrentState.HasExited:
                        ThreadPool.QueueUserWorkItem(BeginRun, null);
                        m_state = CurrentState.IsRunning;
                        break;
                    case CurrentState.IsRunning:
                        m_runAgain = true;
                        break;
                    case CurrentState.IsWaiting:
                        m_resetEvent.Set();
                        m_state = CurrentState.IsRunning;
                        break;
                }
            }
        }

        /// <summary>
        /// Makes sure that the process begins after the specified delay unless 
        /// it is signaled early. If it is currently waiting, this will not
        /// modify the wait time. Instead it will do nothing.
        /// </summary>
        public void RunAfterDelay(TimeSpan delay)
        {
            lock (m_syncRoot)
            {
                switch (m_state)
                {
                    case CurrentState.HasExited:
                        m_resetEvent.Reset();
                        m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_resetEvent, BeginRun, null, delay, true);
                        m_state = CurrentState.IsWaiting;
                        break;
                    case CurrentState.IsRunning:
                        m_delayRequested = true;
                        m_timeDelay = delay;
                        break;
                    case CurrentState.IsWaiting:
                        break;
                }
            }
        }

        public void StopExecuting()
        {
            lock (m_syncRoot)
            {
                m_stopExecuting = true;
            }
        }

        void BeginRun(object state, bool isTimeout)
        {
            lock (m_syncRoot)
            {
                m_registeredHandle.Unregister(null);
            }
            BeginRunInternal();
        }

        void BeginRun(object state)
        {
            BeginRunInternal();
        }

        void BeginRunInternal()
        {
            while (true)
            {
                bool shouldExit = false;
                lock (m_syncRoot)
                {
                    if (m_stopExecuting)
                    {
                        m_state = CurrentState.HasExited;
                        shouldExit = true;
                    }
                    else
                    {
                        m_state = CurrentState.IsRunning;
                        m_runAgain = false;
                        m_delayRequested = false;
                    }
                }
                if (shouldExit)
                {
                    if (HasQuit != null)
                        HasQuit(this, m_objectState);
                    return;
                }

                if (BeforeRun != null)
                    BeforeRun(this, m_objectState);

                if (Running != null)
                    Running(this, m_objectState);

                if (AfterRun != null)
                    AfterRun(this, m_objectState);
                lock (m_syncRoot)
                {
                    if (!m_runAgain)
                    {
                        if (m_delayRequested && !m_stopExecuting)
                        {
                            m_resetEvent.Reset();
                            m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_resetEvent, BeginRun, null, m_timeDelay, true);
                            m_state = CurrentState.IsWaiting;
                            return;
                        }
                        else
                        {
                            m_state = CurrentState.HasExited;
                            return;
                        }
                    }
                }
            }
        }
    }
}
