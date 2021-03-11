//******************************************************************************************************
//  WorkerThreadSynchronization.cs - Gbtc
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
//  05/17/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using GSF.Diagnostics;

namespace GSF.Threading
{
    /// <summary>
    /// Creates a synchronization helper that will assist object synchronizing in a tight inner loop.
    /// </summary>
    /// <remarks>
    /// For streaming protocols, it is cost prohibited to tightly coordinate the inner loop. This class
    /// will help coordinate these efforts by signaling when a good time to perform synchronized work would be.
    /// For Example, when writing to a socket, calling <see cref="BeginSafeToCallbackRegion"/> would be good to do
    /// when the socket makes any kind of blocking call, such as flusing to an underlying socket. Upon returning
    /// from this command, calling <see cref="EndSafeToCallbackRegion"/> will return this class to a state where callback
    /// will not be executed.
    /// 
    /// It is critical that <see cref="BeginSafeToCallbackRegion"/>, <see cref="EndSafeToCallbackRegion"/>, and 
    /// <see cref="PulseSafeToCallback"/> only be called by the worker thread, as these methods are not thread safe and
    /// control the state of the <see cref="WorkerThreadSynchronization"/>.
    /// 
    /// In easy terms. When you (the worker) get a convenient time, I need to do something that might modify your 
    /// current working state. Let me know when I can do that.
    /// </remarks>
    public class WorkerThreadSynchronization
        : DisposableLoggingClassBase
    {
        /// <summary>
        /// A callback request. Cancel this request when the callback is no longer needed.
        /// </summary>
        public class CallbackRequest
            : IDisposable
        {
            private Action m_callback;

            /// <summary>
            /// Creates a callback request.
            /// </summary>
            /// <param name="callback">the action to perform.</param>
            public CallbackRequest(Action callback)
            {
                m_callback = callback;
            }

            /// <summary>
            /// Cancels the callback.
            /// </summary>
            public void Cancel()
            {
                m_callback = null;
            }

            /// <summary>
            /// Disposes of the callback
            /// </summary>
            public void Dispose()
            {
                m_callback = null;
            }

            /// <summary>
            /// Executes the callback item
            /// </summary>
            public void Run()
            {
                Action callback = Interlocked.Exchange(ref m_callback, null);
                if ((object)callback != null)
                {
                    callback();
                }
            }
        }

        private readonly object m_syncRoot;

        private readonly List<CallbackRequest> m_pendingCallbacks;

        /// <summary>
        /// Only to be set by the worker thread.
        /// </summary>
        private volatile bool m_isSafeToCallback;

        /// <summary>
        /// Only to be set within lock(m_syncRoot)
        /// </summary>
        private volatile bool m_isCallbackWaiting;

        /// <summary>
        /// Gets if this method is currently executing, which means tighter coordination is required.
        /// </summary>
        private volatile bool m_isRequestCallbackMethodProcessing;

        /// <summary>
        /// Creates a <see cref="WorkerThreadSynchronization"/>.
        /// </summary>
        public WorkerThreadSynchronization()
            : base(MessageClass.Component)
        {
            m_syncRoot = new object();
            m_isSafeToCallback = false;
            m_isCallbackWaiting = false;
            m_isRequestCallbackMethodProcessing = false;
            m_pendingCallbacks = new List<CallbackRequest>();
        }

        /// <summary>
        /// Requests that the following action be completed as soon as reasonably possible. 
        /// This will either be done immediately, or be queued for the next approriate time.
        /// </summary>
        /// <param name="callback">action to perform</param>
        /// <returns>
        /// A cancelation object. Use in a using block.
        /// </returns>
        public CallbackRequest RequestCallback(Action callback)
        {
            CallbackRequest request = new CallbackRequest(callback);

            lock (m_syncRoot)
            {
                m_isRequestCallbackMethodProcessing = true;
                Thread.MemoryBarrier();
                //------------------------------------------
                if (m_isSafeToCallback)
                {
                    request.Run();
                }
                else
                {
                    m_isCallbackWaiting = true;
                    m_pendingCallbacks.Add(request);
                }
                //-----------------------------------------
                Thread.MemoryBarrier();
                m_isRequestCallbackMethodProcessing = false;
            }

            return request;
        }

        /// <summary>
        /// Signals that if any callbacks are pending, 
        /// go ahead and run them now. Otherwise, wait.
        /// </summary>
        /// <remarks>
        /// This method will be inlined and has virtually no 
        /// overhead so long as a callback is not waiting.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PulseSafeToCallback()
        {
            if (m_isRequestCallbackMethodProcessing || m_isCallbackWaiting)
                ExecuteAllCallbacks();
        }

        /// <summary>
        /// Enters a region where a callback can occur.
        /// </summary>
        /// <remarks>
        /// A good place to put this is before a long 
        /// action where it is possible that the thread
        /// might be blocked.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginSafeToCallbackRegion()
        {
            //Do a double tap so locking is not required.
            //This method will cordinate with RequestCallback
            m_isSafeToCallback = true;
            Thread.MemoryBarrier(); //Since volatile reads can be reordered above writes.
            if (m_isCallbackWaiting)
            {
                ExecuteAllCallbacks();
            }
        }

        /// <summary>
        /// Exits a region where future callbacks cannot occur.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndSafeToCallbackRegion()
        {
            m_isSafeToCallback = false;
            Thread.MemoryBarrier(); //Since volatile reads can be reordered above writes.
            if (m_isRequestCallbackMethodProcessing || m_isCallbackWaiting)
            {
                //If I set m_isSafeToCallback while RequestCallback was running,
                //I need to check and see if the RequestCallback got the message.
                ExecuteAllCallbacks();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExecuteAllCallbacks()
        {
            lock (m_syncRoot)
            {
                if (m_isCallbackWaiting)
                {
                    m_isCallbackWaiting = false;
                    foreach (CallbackRequest callback in m_pendingCallbacks)
                    {
                        try
                        {
                            callback.Run();
                        }
                        catch (Exception ex)
                        {
                            Log.Publish(MessageLevel.Error, "Unexpected error when invoking callbacks", null, null, ex);
                        }
                    }
                    m_pendingCallbacks.Clear();
                }
            }
        }

    }
}
