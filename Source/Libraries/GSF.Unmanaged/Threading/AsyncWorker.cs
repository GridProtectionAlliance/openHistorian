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
//using System.Threading;

//namespace GSF.Threading
//{
//    /// <summary>
//    /// Provides a lightweight way to schedule 
//    /// work on a seperate thread that is gaurenteed to run
//    /// at least once after calling the Run method.
//    /// </summary>
//    public class AsyncWorker : IDisposable
//    {
//        /// <summary>
//        /// I cannot directly reference this class because this class can have a 
//        /// blocked thread that prevents it from being disposed. Therefore,
//        /// when AsyncWorker is no longer referenced, the finalize method will clean up any 
//        /// underlying code.
//        /// </summary>
//        IAsyncWorker m_wrapper;

//        public AsyncWorker(bool useThreadPool = false)
//        {
//            if (useThreadPool)
//            {
//                m_wrapper = new AsyncWorkerBackground();
//            }
//            else
//            {
//                m_wrapper = new AsyncWorkerForeground();
//            }
//        }
//        ~AsyncWorker()
//        {
//            if (m_wrapper != null) 
//                m_wrapper.FinalizedDisposed();
//            m_wrapper = null;
//        }

//        public event EventHandler<AsyncWorkerEventArgs> DoWork
//        {
//            add
//            {
//                m_wrapper.DoWork += value;
//            }
//            remove
//            {
//                m_wrapper.DoWork -= value;
//            }
//        }

//        public event EventHandler CleanupWork
//        {
//            add
//            {
//                m_wrapper.CleanupWork += value;
//            }
//            remove
//            {
//                m_wrapper.CleanupWork -= value;
//            }
//        }

//        public bool IsDisposed
//        {
//            get
//            {
//                return m_wrapper.IsDisposed;
//            }
//        }

//        public bool IsDisposing
//        {
//            get
//            {
//                return m_wrapper.IsDisposing;
//            }
//        }

//        public void RunWorker()
//        {
//            m_wrapper.RunWorker();
//        }

//        public void Dispose()
//        {
//            if (m_wrapper != null) 
//                m_wrapper.Dispose();
//            m_wrapper = null;
//            GC.SuppressFinalize(this);
//        }
//    }
//}
