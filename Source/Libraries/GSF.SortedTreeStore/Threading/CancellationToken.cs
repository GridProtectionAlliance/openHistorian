////******************************************************************************************************
////  CancellationToken.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://opensource.org/licenses/MIT
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  05/17/2014 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.Runtime.CompilerServices;

//namespace GSF.Threading
//{
//    public class CancellationToken
//    {
//        private object m_syncRoot;

//        /// <summary>
//        /// The callback that will cancel an existing action.
//        /// </summary>
//        private Action m_cancellationAction;

//        private volatile bool m_cancelSignaled;

//        private volatile bool m_cancelAcknowledged;

//        private volatile bool m_canSignalCancel;

//        /// <summary>
//        /// Creates a <see cref="WorkerThreadSynchronization"/>.
//        /// </summary>
//        public CancellationToken(Action onCancelAction)
//        {
//            m_syncRoot = new object();
//            m_cancellationAction = onCancelAction;
//            m_cancelSignaled = false;
//        }

//        /// <summary>
//        /// Signals a cancellation to occur.
//        /// </summary>
//        public void Signal()
//        {
//            m_cancelSignaled = true;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public void PulseSafeToCancel()
//        {
//            if (m_isCallbackWaiting)
//                PulseValidRegionSlow();
//        }

//        [MethodImpl(MethodImplOptions.NoInlining)]
//        private void PulseSafeToCancel()
//        {
//            lock (m_pendingCallback)
//            {
//                m_isCallbackWaiting = false;
//                foreach (var callback in m_pendingCallback)
//                {
//                    callback.Run();
//                }
//                m_pendingCallback.Clear();
//            }
//        }

//        /// <summary>
//        /// Enters a region where a callback can occur.
//        /// </summary>
//        public void BeginSafeToCancelRegion()
//        {
//            lock (m_pendingCallback)
//            {
//                m_inValidCallbackRegion = true;

//                if (m_isCallbackWaiting)
//                {
//                    m_isCallbackWaiting = false;
//                    foreach (var callback in m_pendingCallback)
//                    {
//                        callback.Run();
//                    }
//                    m_pendingCallback.Clear();
//                }
//            }
//        }

//        /// <summary>
//        /// Exits a region where a callback can occur.
//        /// </summary>
//        public void EndSafeToCancelRegion()
//        {
//            lock (m_pendingCallback)
//            {
//                m_inValidCallbackRegion = false;
//            }
//        }

//    }
//}
