//******************************************************************************************************
//  ThreadContainerBase.cs - Gbtc
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

namespace GSF.Threading
{
    internal abstract class ThreadContainerBase : IDisposable
    {
        //internal class CallbackResponse
        //{
        //    public bool CallInternalStart;
        //    public bool CallInternalStartDelay;
        //    public int CallInternalStartDelayValue;
        //    public bool CallInternalCancelTimer;
        //    public bool CallInternalDispose;
        //    public bool CallInternalAfterRunning;
        //    public void Clear()
        //    {
        //        CallInternalStart = false;
        //        CallInternalStartDelay = false;
        //        CallInternalStartDelayValue = -1;
        //        CallInternalCancelTimer = false;
        //        CallInternalDispose = false;
        //        CallInternalAfterRunning = false;
        //    }
        //}

        bool m_disposed;

        WeakActionFast m_callback;
        protected ThreadContainerBase(WeakActionFast callback)
        {
            m_callback = callback;
        }

        protected void OnRunning()
        {
            if (!m_callback.TryInvoke())
                Dispose();
        }

        public abstract void Start();
        public abstract void Start(int delay);
        public abstract void CancelTimer();

        public abstract void AfterRunning();
        protected abstract void InternalDispose();

        public void Dispose()
        {
            //Since Dispose can be called from the running thread and from ScheduledTask, it needs to be synchronized.
            lock (this)
            {
                if (m_disposed)
                    return;
                m_disposed = true;
            }
            InternalDispose();
        }


    }
}
