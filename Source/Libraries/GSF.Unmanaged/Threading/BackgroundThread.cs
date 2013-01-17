//******************************************************************************************************
//  BackgroundThread.cs - Gbtc
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
    abstract class CustomThreadBase : IDisposable
    {
        public abstract void StartNow();
        public abstract void StartLater(int delay);
        public abstract void ShortCircuitDelayRequest();
        public abstract void ResetTimer();
        public abstract void Dispose();
    }

    class BackgroundThread : CustomThreadBase
    {
        RegisteredWaitHandle m_registeredHandle;
        Action m_callback;
        ManualResetEvent m_resetEvent;

        public BackgroundThread(Action callback)
        {
            m_callback = callback;
            m_resetEvent = new ManualResetEvent(false);
        }

        public override void StartNow()
        {
            ThreadPool.QueueUserWorkItem(BeginRunOnTimer);
        }
        public override void StartLater(int delay)
        {
            m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_resetEvent, BeginRunOnTimer, null, delay, true);
        }
        public override void ShortCircuitDelayRequest()
        {
            m_resetEvent.Set();
        }

        public override void ResetTimer()
        {
            m_resetEvent.Reset();
        }

        void BeginRunOnTimer(object state)
        {
            m_callback();
        }

        void BeginRunOnTimer(object state, bool isTimeout)
        {
            if (m_registeredHandle != null)
                m_registeredHandle.Unregister(null);

            m_callback();
        }

        public override void Dispose()
        {
            //Nothing required for background threads.
        }
    }
}
