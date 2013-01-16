////******************************************************************************************************
////  AsyncWorkerEventArgs.cs - Gbtc
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
////  1/12/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;

//namespace GSF.Threading
//{
//    /// <summary>
//    /// Arguments provided to the event handler of <see cref="AsyncWorkerForeground.DoWork"/> that
//    /// allows it to request a time-delayed recall of the event.
//    /// </summary>
//    public class AsyncWorkerEventArgs : EventArgs
//    {
//        TimeSpan m_runAgainDelay;
       
//        /// <summary>
//        /// Gets if the worker thread should recall this event immediately.
//        /// </summary>
//        public bool ShouldRunAgainImmediately { get; private set; }
        
//        /// <summary>
//        /// Gets if the worker thread should run again after a time delay.
//        /// </summary>
//        public bool ShouldRunAgainAfterDelay { get; private set; }

//        public AsyncWorkerEventArgs()
//        {
//            ShouldRunAgainImmediately = false;
//            ShouldRunAgainAfterDelay = false;
//            m_runAgainDelay = new TimeSpan(0);
//        }

//        /// <summary>
//        /// Gets the maximum time delay to wait before recalling this event.
//        /// A value of less than 1 millisecond will cause it to be called immediately.
//        /// </summary>
//        public TimeSpan? RunAgainDelay
//        {
//            get
//            {
//                if (ShouldRunAgainAfterDelay)
//                    return m_runAgainDelay;
//                return null;
//            }
//        }

//        /// <summary>
//        /// Gets the delay time that is equivalent what will be passed to a wait handle.
//        /// So 0 for ShouldRunImmediately, -1 for Forever, and the millisecond count for anything else.
//        /// </summary>
//        public int WaitHandleTime
//        {
//            get
//            {
//                if (ShouldRunAgainImmediately)
//                    return 0;
//                if (ShouldRunAgainAfterDelay)
//                    return (int)m_runAgainDelay.TotalMilliseconds;
//                return -1;
//            }
//        }

//        /// <summary>
//        /// Reruns the event immediately.
//        /// </summary>
//        public void RunAgain()
//        {
//            ShouldRunAgainImmediately = true;
//            ShouldRunAgainAfterDelay = false;
//        }


//        /// <summary>
//        /// Reruns the event after the provided delay.
//        /// </summary>
//        /// <param name="delay">A delay in milliseconds</param>
//        public void RunAgainAfterDelay(int delay)
//        {
//            RunAgainAfterDelay(new TimeSpan(delay * TimeSpan.TicksPerMillisecond));
//        }

//        /// <summary>
//        /// Reruns the event after the provided delay.
//        /// </summary>
//        /// <param name="delay"></param>
//        public void RunAgainAfterDelay(TimeSpan delay)
//        {
//            if (delay.Ticks < 0)
//                throw new ArgumentOutOfRangeException("delay", "must be greater than or equal to zero");
//            if (delay.TotalMilliseconds > int.MaxValue)
//                throw new ArgumentOutOfRangeException("delay", "must be less than Int32.MaxValue");

//            if (ShouldRunAgainImmediately)
//                return;

//            if (ShouldRunAgainAfterDelay)
//            {
//                m_runAgainDelay = new TimeSpan(Math.Min(delay.Ticks, m_runAgainDelay.Ticks));
//            }
//            else
//            {
//                ShouldRunAgainAfterDelay = true;
//                m_runAgainDelay = delay;
//            }

//        }
//    }

//}
