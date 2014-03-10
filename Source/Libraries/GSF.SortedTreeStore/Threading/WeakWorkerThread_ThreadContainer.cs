////******************************************************************************************************
////  WeakWorkerThread`1.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
////  3/8/2014 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Runtime.CompilerServices;
//using System.Threading;

//namespace GSF.Threading
//{

    

//    /// <summary>
//    /// A weak referenced <see cref="Thread"/> that will enter a pause state behind a weak reference
//    /// so it can be garbaged collected if it's in a sleep state.
//    /// </summary>
//    public partial class WeakWorkerThread
//    {

//        private class ThreadContainer
//            : WeakReference
//        {
//            private Thread m_thread;
//            private ManualResetEvent m_threadPausedWaitHandler;
//            private ManualResetEvent m_threadSleepWaitHandler;
//            private volatile int m_sleepTime;

//            public ThreadContainer(Func<WorkerThreadTimeoutResults, bool> callback, bool isBackground, ThreadPriority priority)
//                : base(callback)
//            {
//                m_threadPausedWaitHandler = new ManualResetEvent(false);
//                m_threadSleepWaitHandler = new ManualResetEvent(false);
//                m_thread = new Thread(ThreadLoop);
//                m_thread.IsBackground = isBackground;
//                m_thread.Priority = priority;
//                m_thread.Start();
//            }

//            void ThreadLoop()
//            {
//                WorkerThreadTimeoutResults state;
//                while (true)
//                {
//                    state = WorkerThreadTimeoutResults.StartNow;
//                    m_threadPausedWaitHandler.WaitOne(-1);
//                    m_threadPausedWaitHandler.Reset();

//                    if (m_sleepTime != 0)
//                    {
//                        if (m_threadSleepWaitHandler.WaitOne(m_sleepTime))
//                        {
//                            state = WorkerThreadTimeoutResults.StartAfterPartialDelay;
//                        }
//                        else
//                        {
//                            state = WorkerThreadTimeoutResults.StartAfterFullDelay;
//                        }
//                    }
//                    m_threadSleepWaitHandler.Reset();
                    
//                    if (!TryInvoke(state))
//                    {
//                        Quit();
//                        return;
//                    }
//                }
//            }

//            /// <summary>
//            /// Attempts to call the weak delegate. 
//            /// </summary>
//            /// <param name="state">state variables to pass</param>
//            /// <returns>True if successful and the delegate returned true. False if the code needs to exit</returns>
//            /// <remarks>
//            /// This method must not be inlined because a strong reference to this weak reference exists in this function. 
//            /// Inlining would put the strong reference in the sleeping method, preventing collection.
//            /// </remarks>
//            [MethodImpl(MethodImplOptions.NoInlining)]
//            bool TryInvoke(WorkerThreadTimeoutResults state)
//            {
//                Func<WorkerThreadTimeoutResults, bool> callback = (Func<WorkerThreadTimeoutResults, bool>)Target;
//                if (callback != null)
//                {
//                    return callback(state);
//                }
//                else
//                {
//                    return false;
//                }
//            }

//            void Quit()
//            {
//                m_threadPausedWaitHandler.Dispose();
//                m_threadPausedWaitHandler = null;
//                m_threadSleepWaitHandler.Dispose();
//                m_threadSleepWaitHandler = null;
//                m_thread = null;
//            }

//            public void CancelTimer()
//            {
//                m_sleepTime = 0;
//                m_threadSleepWaitHandler.Set();
//                m_threadPausedWaitHandler.Set();
//            }

//            public void Invoke()
//            {
//                m_sleepTime = 0;
//                m_threadSleepWaitHandler.Set();
//                m_threadPausedWaitHandler.Set();
//            }

//            public void Invoke(int delay)
//            {
//                m_sleepTime = delay;
//                m_threadPausedWaitHandler.Set();
//            }
//        }
//    }
//}
