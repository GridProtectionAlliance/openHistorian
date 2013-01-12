//******************************************************************************************************
//  TimeoutOperation.cs - Gbtc
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
//  1/5/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//*****************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSF.Threading
{

    public class TimeoutOperation
    {
        object m_syncRoot = new object();
        RegisteredWaitHandle m_registeredHandle;
        ManualResetEvent m_resetEvent;
        Action m_callback;

        public void RegisterTimeout(TimeSpan interval, Action callback)
        {

            lock (m_syncRoot)
            {
                if (m_callback != null)
                    throw new Exception("Duplicate calls are not permitted");

                m_callback = callback;
                m_resetEvent = new ManualResetEvent(false);
                m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_resetEvent, BeginRun, null, interval, true);
            }

        }

        void BeginRun(object state, bool isTimeout)
        {
            lock (m_syncRoot)
            {
                if (m_registeredHandle == null)
                    return;
                m_registeredHandle.Unregister(null);
                m_resetEvent.Dispose();
                m_callback();
                m_resetEvent = null;
                m_registeredHandle = null;
                m_callback = null;
            }

        }

        public void Cancel()
        {
            lock (m_syncRoot)
            {
                if (m_registeredHandle != null)
                {
                    m_registeredHandle.Unregister(null);
                    m_resetEvent.Dispose();
                    m_resetEvent = null;
                    m_registeredHandle = null;
                    m_callback = null;
                }
            }

        }
    }
}
