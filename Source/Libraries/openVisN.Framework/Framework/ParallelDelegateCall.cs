//******************************************************************************************************
//  ParallelDelegateCall.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace openVisN.Framework
{
    public static class ParallelDelegateCall
    {
        public static void ParallelRunAndWait<T>(this EventHandler<T> eventToRun, object sender, T variable)
            where T : EventArgs
        {
            _ = new ParallelRun<T>(eventToRun, sender, variable);
        }

        private class ParallelRun<T>
            where T : EventArgs
        {
            private readonly int m_numberOfDelegates;
            private int m_delegatesExecuted;
            private readonly object m_sender;
            private readonly T m_variable;
            private readonly ManualResetEvent m_waitForComplete;
            private List<Exception> m_exceptionsThrown;

            //StringBuilder m_sb;

            public ParallelRun(EventHandler<T> eventToRun, object sender, T variable)
            {
                //m_sb = new StringBuilder();
                m_sender = sender;
                m_variable = variable;
                Delegate[] delegates = eventToRun.GetInvocationList();
                m_numberOfDelegates = delegates.Count();
                m_delegatesExecuted = 0;
                m_waitForComplete = new ManualResetEvent(false);
                //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

                ThreadPool.GetAvailableThreads(out int workerThreads, out int completionPortThreads);
                foreach (Delegate d in delegates)
                {
                    ThreadPool.QueueUserWorkItem(Process, d);
                }
                m_waitForComplete.WaitOne();
                //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;
                //Clipboard.SetText(m_sb.ToString());

                if (m_exceptionsThrown != null)
                    throw new Exception("There were errors");
            }

            public void Process(object callback)
            {
                EventHandler<T> d2 = (EventHandler<T>)callback;
                d2.Invoke(m_sender, m_variable);
                if (Interlocked.Increment(ref m_delegatesExecuted) == m_numberOfDelegates)
                {
                    m_waitForComplete.Set();
                }
            }
        }
    }
}