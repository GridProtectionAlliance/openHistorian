//******************************************************************************************************
//  SynchronousEvent.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/26/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.ComponentModel;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Provides a way to raise events on another thread. The events
    /// will be raised on the thread that constructed this class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This is useful when needing to process data on a certain thread. On instance is 
    /// when preparing data that needs to then be processed on the UI thread. Just construct
    /// this class on the UI thread, then when any thread raises an event, this event will be
    /// queued on the UI thread.
    /// </remarks>
    public class SynchronousEvent<T> 
        : IDisposable
        where T : EventArgs
    {
        private bool m_disposed;
        private readonly ManualResetEvent m_waiting;
        public event EventHandler<T> CustomEvent;
        private readonly AsyncOperation m_asyncEventHelper;

        public SynchronousEvent()
        {
            m_waiting = new ManualResetEvent(false);
            m_asyncEventHelper = AsyncOperationManager.CreateOperation(null);
        }

        /// <summary>
        /// Posts an event on the constructing thread.
        /// This call blocks until the custom event 
        /// Not thread safe.
        /// </summary>
        /// <param name="args"></param>
        public void RaiseEvent(T args)
        {
            if (m_disposed)
                return;
            if (CustomEvent != null)
            {
                m_waiting.Reset();
                Thread.MemoryBarrier();
                if (m_disposed)
                    return;
                m_asyncEventHelper.Post(Callback, args);
                m_waiting.WaitOne();
            }
        }

        private void Callback(object sender)
        {
            if (m_disposed)
                return;
            try
            {
                if (CustomEvent != null)
                {
                    CustomEvent(this, (T)sender);
                }
            }
            finally
            {
                m_waiting.Set();
            }
        }

        /// <summary>
        /// Prevents any future events from processing and
        /// attempts to cancel a pending operation. 
        /// Function returns before any attempts to cancel are successful.
        /// </summary>
        public void Dispose()
        {
            m_disposed = true;
            Thread.MemoryBarrier();
            m_waiting.Set();
        }
    }
}