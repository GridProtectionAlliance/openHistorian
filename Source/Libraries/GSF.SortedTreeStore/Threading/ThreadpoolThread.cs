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
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Implements a background thread that uses the threadpool. 
    /// </summary> 
    internal class ThreadpoolThread
        : CustomThreadBase
    {
        /// <summary>
        /// Handle that is created when telling the threadpool to do a delayed start.
        /// </summary>
        private volatile RegisteredWaitHandle m_registeredHandle;

        /// <summary>
        /// The callback to execute
        /// </summary>
        private readonly WeakAction m_callback;

        /// <summary>
        /// The reset event that allows the timer to be short circuited.
        /// </summary>
        private readonly ManualResetEvent m_waitObject;

        /// <summary>
        /// Initializes a <see cref="ThreadpoolThread"/> that will execute the provided callback.
        /// </summary>
        /// <param name="callback">the callback method to execute when running</param>
        public ThreadpoolThread(Action callback)
        {
            m_callback = new WeakAction(callback);
            m_waitObject = new ManualResetEvent(false);
        }

        /// <summary>
        /// Requests that the callback executes immediately.
        /// </summary>
        public override void StartNow()
        {
            ThreadPool.QueueUserWorkItem(BeginRunImmediately);
        }

        /// <summary>
        /// Requests that the callback executes after the specified interval in milliseconds.
        /// </summary>
        /// <param name="delay">the delay in milliseconds</param>
        public override void StartLater(int delay)
        {
            m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_waitObject, BeginRunOnTimer, null, delay, executeOnlyOnce: true);
        }

        /// <summary>
        /// Requests that a previous delay be canceled and the callback be executed immediately
        /// </summary>
        public override void ShortCircuitDelayRequest()
        {
            m_waitObject.Set();
        }

        /// <summary>
        /// A reset will return the thread to a non-executing/ready state.
        /// </summary>
        public override void ResetTimer()
        {
           m_waitObject.Reset();
        }

        /// <summary>
        /// Callback for <see cref="StartNow"/>
        /// </summary>
        /// <param name="state"></param>
        private void BeginRunImmediately(object state)
        {
            m_callback.TryInvoke();
        }

        /// <summary>
        /// Callback for <see cref="StartLater"/> or when this is short circuited.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="isTimeout"></param>
        private void BeginRunOnTimer(object state, bool isTimeout)
        {
            //Wait for this race condition to satisfy
            while (m_registeredHandle == null)
                ;
            m_registeredHandle.Unregister(null);
            m_registeredHandle = null;

            m_callback.TryInvoke();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            //Nothing required for background threads.
        }
    }
}