//******************************************************************************************************
//  WorkerThread_ThreadContainerThreadpool.cs - Gbtc
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

using System;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// A weak referenced <see cref="Thread"/> that will enter a pause state behind a weak reference
    /// so it can be garbaged collected if it's in a sleep state.
    /// </summary>
    public partial class WorkerThread
    {
        private class ThreadContainerThreadpool
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

            ThreadContainerArgs m_state = new ThreadContainerArgs();


            public ThreadContainerThreadpool(Action<ThreadContainerArgs> callback)
                : base(callback)
            {
                m_waitObject = new ManualResetEvent(false);
            }

            public override void Dispose()
            {
                Target = null;
                m_waitObject.Dispose();
                m_waitObject = null;
            }

            public override void Reset()
            {
                m_waitObject.Reset();
            }

            public override void StartLater(int delay)
            {
                m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_waitObject, BeginRunOnTimer, null, delay, executeOnlyOnce: true);
            }

            public override void CancelTimer()
            {
                m_waitObject.Set();
            }

            public override void StartNow()
            {
                ThreadPool.QueueUserWorkItem(BeginRunImmediately);
            }

            /// <summary>
            /// Callback for <see cref="StartNow"/>
            /// </summary>
            /// <param name="state"></param>
            private void BeginRunImmediately(object state)
            {
                m_state.Clear();
                m_state.TimeoutResults = WorkerThreadTimeoutResults.StartNow;
                TryInvoke(m_state);

                if (m_state.ShouldQuit)
                {
                    Dispose();
                    return;
                }
                if (m_state.RepeatNow)
                {
                    StartNow();
                }
                if (m_state.RepeatAfterDelay)
                {
                    StartLater(m_state.RepeatAfterDelayTime);
                }
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


                m_state.Clear();
                if (isTimeout)
                    m_state.TimeoutResults = WorkerThreadTimeoutResults.StartAfterFullDelay;
                else
                    m_state.TimeoutResults = WorkerThreadTimeoutResults.StartAfterPartialDelay;
              
                TryInvoke(m_state);

                if (m_state.ShouldQuit)
                {
                    Dispose();
                    return;
                }
                if (m_state.RepeatNow)
                {
                    StartNow();
                }
                if (m_state.RepeatAfterDelay)
                {
                    StartLater(m_state.RepeatAfterDelayTime);
                }
            }

        }
    }
}
