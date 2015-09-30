//******************************************************************************************************
//  ThreadContainerThreadpool.cs - Gbtc
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
//  3/8/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace GSF.Threading
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    internal class ThreadContainerThreadpool
        : ThreadContainerBase
    {
        /// <summary>
        /// Handle that is created when telling the threadpool to do a delayed start.
        /// </summary>
        private volatile RegisteredWaitHandle m_registeredHandle;

        /// <summary>
        /// The reset event that allows the timer to be short circuited.
        /// </summary>
        private ManualResetEvent m_waitObject;

        public ThreadContainerThreadpool(WeakAction<CallbackArgs> callback)
            : base(callback)
        {
            m_waitObject = new ManualResetEvent(false);
        }

        protected override void InternalStart(int delay)
        {
            m_waitObject.Reset();
            m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_waitObject, BeginRunOnTimer, null, delay, executeOnlyOnce: true);
        }

        protected override void InternalStart_FromWorkerThread(int delay)
        {
            m_waitObject.Reset();
            m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_waitObject, BeginRunOnTimer, null, delay, executeOnlyOnce: true);
        }

        private void BeginRunOnTimer(object state, bool isTimeout)
        {
            //Wait for this race condition to satisfy
            while (m_registeredHandle == null)
                ;
            m_registeredHandle.Unregister(null);
            m_registeredHandle = null;
            OnRunning();
        }

        protected override void InternalCancelTimer()
        {
            m_waitObject.Set();
        }

        protected override void InternalStart()
        {
            ThreadPool.QueueUserWorkItem(BeginRunImmediately);
        }

        protected override void InternalStart_FromWorkerThread()
        {
            ThreadPool.QueueUserWorkItem(BeginRunImmediately);
        }

        private void BeginRunImmediately(object state)
        {
            OnRunning();
        }

        protected override void InternalDispose_FromWorkerThread()
        {
            m_waitObject.Dispose();
            m_waitObject = null;
        }

        protected override void InternalDoNothing_FromWorkerThread()
        {

        }

    }
}
