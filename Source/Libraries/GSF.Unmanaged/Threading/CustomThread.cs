////******************************************************************************************************
////  AsyncWorker.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  12/26/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace GSF.Threading
//{
//    public abstract class CustomThreadBase
//    {
//        public event Action ThreadStarted;
//        public event Action ThreadExited;

//        Thread m_currentThread;
//        RegisteredWaitHandle m_registeredHandle;

//        protected CustomThreadBase()
//        {

//        }



//    }

//    class ThreadPoolCustomThread
//    {
//        public event Action Run;
//        public event Action Quitting;

//        object m_syncRoot;
//        int m_runCount;
//        ManualResetEvent m_runAgainEvent;
//        RegisteredWaitHandle m_registeredHandle;
//        ManualResetEvent m_doneCleaningUp;

//        public void Join()
//        {
//            m_doneCleaningUp.WaitOne();
//        }

//        public void Start()
//        {
//            ThreadPool.QueueUserWorkItem(BeginRun, null);
//        }

//        public void StartAfterDelay(TimeSpan delay)
//        {
//            m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_runAgainEvent, BeginRunOnTimer, null, delay, true);
//        }

//        void BeginRunOnTimer(object state, bool isTimeout)
//        {
//            //Locking again may not be required. However, a race condition does exist in
//            //the unlikely event that this code is executed before m_registeredHandle has been assigned.
//            lock (m_syncRoot)
//            {
//                m_registeredHandle.Unregister(null);
//            }
//            DoRun();
//        }

//        void BeginRun(object state)
//        {
//            DoRun();
//        }

//        void DoRun()
//        {
//            if (m_runCount != 0)
//                throw new Exception("Internal Error, only one async worker should be running at a time.");
//            m_runCount++;
//            if (Run != null)
//                Run();
//            m_runCount--;
//        }


//    }
//}
