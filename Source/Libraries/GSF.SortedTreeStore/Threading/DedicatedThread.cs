//******************************************************************************************************
//  DedicatedThread.cs - Gbtc
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
//  1/15/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Implements a dedicated thread that is either a foreground or a background thread. 
    /// If foreground, this means that it will
    /// prevent an application from exiting if it's still doing work. Therefore be sure to 
    /// dispose of the thread before exiting the application. (Which will be done from ScheduledTask via
    /// weak references if an object becomes orphaned)
    /// </summary>
    internal class DedicatedThread
        : CustomThreadBase
    {
        private Thread m_thread;
        private WeakAction m_callback;
        private ManualResetEvent m_go;
        private ManualResetEvent m_sleep;
        private volatile int m_sleepTime;
        private volatile bool m_stopExecuting;

        /// <summary>
        /// Initializes a <see cref="DedicatedThread"/> that will execute the provided callback.
        /// </summary>
        /// <param name="callback">the callback method to execute when running</param>
        /// <param name="isBackground">determines if the thread is supposed to be a background thread</param>
        /// <param name="priority">specifies the priority of the thread</param>
        public DedicatedThread(Action callback, bool isBackground, ThreadPriority priority)
        {
            m_sleepTime = 0;
            m_callback = new WeakAction(callback);
            m_go = new ManualResetEvent(false);
            m_sleep = new ManualResetEvent(false);
            m_thread = new Thread(RunWorkerThread);
            m_thread.IsBackground = isBackground;
            m_thread.Priority = priority;
            m_thread.Start();
        }

        /// <summary>
        /// The internal loop that holds the thread.
        /// </summary>
        private void RunWorkerThread()
        {
            while (true)
            {
                m_go.WaitOne(-1);

                if (m_stopExecuting)
                {
                    m_go.Dispose();
                    m_go = null;
                    m_sleep.Dispose();
                    m_sleep = null;
                    m_callback = null;
                    m_thread = null;
                    return;
                }

                if (m_sleepTime != 0)
                    m_sleep.WaitOne(m_sleepTime);

                m_callback.TryInvoke();
            }
        }

        /// <summary>
        /// Requests that the callback executes immediately.
        /// </summary>
        public override void StartNow()
        {
            m_sleepTime = 0;
            m_go.Set();
        }

        /// <summary>
        /// Requests that the callback executes after the specified interval in milliseconds.
        /// </summary>
        /// <param name="delay">the delay in milliseconds</param>
        public override void StartLater(int delay)
        {
            m_sleepTime = delay;
            m_go.Set();
        }

        /// <summary>
        /// Requests that a previous delay be canceled and the callback be executed immediately
        /// </summary>
        public override void ShortCircuitDelayRequest()
        {
            m_sleep.Set();
        }

        /// <summary>
        /// A reset will return the thread to a non-executing/ready state.
        /// </summary>
        public override void ResetTimer()
        {
            m_go.Reset();
            m_sleep.Reset();
            m_sleepTime = 0;
        }

        /// <summary>
        /// Gracefully stops the execution of the custom thread. 
        /// Similiar to Dispose, except, this action must also be properly coordinated.
        /// </summary>
        public override void StopExecution()
        {
            m_stopExecuting = true;
            m_go.Set();
        }
    }
}