//******************************************************************************************************
//  Worker.cs - Gbtc
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
//  1/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// A static class that assists with spinning off workers.
    /// </summary>
    public static class Worker
    {
        static AutoResetEvent s_waitHandle = new AutoResetEvent(false);
        /// <summary>
        /// Runs the following Action after a predefined delay.
        /// </summary>
        /// <param name="delay">a delay in milliseconds. 0 means run immediately.</param>
        /// <param name="method">The code to execute after the delay</param>
        /// <remarks>The accuracy of this timer method can vary substantially. Don't use this to do coordination. 
        /// Expect += 15 ms or even more.</remarks>
        public static void RunAfterDelay(int delay, Action method)
        {
            if (delay < 0)
                throw new ArgumentOutOfRangeException("delay", "Must be greater than zero.");
            if (method == null)
                throw new ArgumentNullException("method");

            var callback = new WaitRun(delay, method);
        }

        class WaitRun
        {
            RegisteredWaitHandle m_registeredHandle;
            Action m_method;
            public WaitRun(int delay, Action method)
            {
                m_method = method;
                m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(s_waitHandle, BeginRunOnTimer, null, delay, true);
            }
            void BeginRunOnTimer(object state, bool isTimeout)
            {
                try
                {
                    if (m_registeredHandle != null)
                        m_registeredHandle.Unregister(null);

                    m_method();
                }
                catch (Exception)
                {
                }
            }
        }
    }



}
