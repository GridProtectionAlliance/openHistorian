//******************************************************************************************************
//  ThreadContainerDedicated.cs - Gbtc
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

using System.Threading;

namespace GSF.Threading
{
    internal class ThreadContainerDedicated
        : ThreadContainerBase
    {
        private volatile bool m_shouldQuit;
        private Thread m_thread;
        private ManualResetEvent m_threadPausedWaitHandler;
        private ManualResetEvent m_threadSleepWaitHandler;
        private volatile int m_sleepTime;

        public ThreadContainerDedicated(WeakActionFast callback, bool isBackground, ThreadPriority priority)
            : base(callback)
        {
            m_threadPausedWaitHandler = new ManualResetEvent(false);
            m_threadSleepWaitHandler = new ManualResetEvent(false);
            m_thread = new Thread(ThreadLoop);
            m_thread.IsBackground = isBackground;
            m_thread.Priority = priority;
            m_thread.Start();
        }

        void ThreadLoop()
        {
            while (true)
            {
                if (m_shouldQuit)
                {
                    Quit();
                    return;
                }
                m_threadPausedWaitHandler.WaitOne(-1);

                if (m_shouldQuit)
                {
                    Quit();
                    return;
                }

                if (m_sleepTime != 0)
                {
                    m_threadSleepWaitHandler.WaitOne(m_sleepTime);
                }

                if (m_shouldQuit)
                {
                    Quit();
                    return;
                }

                OnRunning();
            }
        }

        void Quit()
        {
            m_threadPausedWaitHandler.Dispose();
            m_threadPausedWaitHandler = null;
            m_threadSleepWaitHandler.Dispose();
            m_threadSleepWaitHandler = null;
            m_thread = null;
        }

        public override void InternalDispose()
        {
            m_shouldQuit = true;
            InternalStart();
        }

        public override void InternalAfterRunning()
        {
            m_threadPausedWaitHandler.Reset();
            m_threadSleepWaitHandler.Reset();
        }

        public override void InternalStart(int delay)
        {
            m_sleepTime = delay;
            m_threadPausedWaitHandler.Set();
        }

        public override void InternalCancelTimer()
        {
            m_threadSleepWaitHandler.Set();
            m_threadPausedWaitHandler.Set();
        }

        public override void InternalStart()
        {
            m_sleepTime = 0;
            m_threadSleepWaitHandler.Set();
            m_threadPausedWaitHandler.Set();
        }

    }
}
