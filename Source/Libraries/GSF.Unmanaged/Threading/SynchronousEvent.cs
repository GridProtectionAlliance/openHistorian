//******************************************************************************************************
//  SynchronousEvent.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
    public class SynchronousEvent<T>
    {
        ManualResetEvent m_waiting;
        public event EventHandler<T> CustomEvent;

        AsyncOperation m_asyncEventHelper;

        public SynchronousEvent()
        {
            m_waiting = new ManualResetEvent(false);
            m_asyncEventHelper = AsyncOperationManager.CreateOperation(null);
        }

        public void RaiseEvent(T args)
        {
            if (CustomEvent != null)
            {
                m_waiting.Reset();
                m_asyncEventHelper.Post(Callback, args);
                m_waiting.WaitOne();
            }

        }

        void Callback(object sender)
        {
            if (CustomEvent != null)
            {
                CustomEvent(this, (T)sender);
            }
            m_waiting.Set();
        }
    }
}
