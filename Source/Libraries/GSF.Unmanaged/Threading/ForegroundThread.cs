//******************************************************************************************************
//  ForegroundThread.cs - Gbtc
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GSF.Threading
{
    class ForegroundThread : CustomThreadBase
    {
        Thread m_thread;
        Action m_callback;
        ManualResetEvent m_go;
        ManualResetEvent m_sleep;
        volatile int m_sleepTime;
        volatile bool m_disposed;

        public ForegroundThread(Action callback)
        {
            m_sleepTime = 0;
            m_callback = callback;
            m_go = new ManualResetEvent(false);
            m_sleep = new ManualResetEvent(false);
            m_thread = new Thread(RunWorkerThread);
            m_thread.Start();
        }

        void RunWorkerThread()
        {
            //Initially m_state == State.IsRunning;
            while (true)
            {
                if (m_disposed)
                    return;
                //Thread.CurrentThread.IsBackground = true;
                m_go.WaitOne(-1);
                m_sleep.WaitOne(m_sleepTime);
                //Thread.CurrentThread.IsBackground = false;
                if (m_disposed)
                    return;
                m_callback();
            }
        }

        public override void StartNow()
        {
            m_sleepTime = 0;
            m_sleep.Set();
            m_go.Set();
        }

        public override void StartLater(int delay)
        {
            m_sleepTime = delay;
            m_go.Set();
        }

        public override void ShortCircuitDelayRequest()
        {
            m_sleep.Set();
        }

        public override void ResetTimer()
        {
            m_go.Reset();
            m_sleep.Reset();
            m_sleepTime = 0;
        }

        public override void Dispose()
        {
            m_disposed = true;
            m_sleepTime = 0;
            m_sleep.Set();
            m_go.Set();
        }
    }
}
