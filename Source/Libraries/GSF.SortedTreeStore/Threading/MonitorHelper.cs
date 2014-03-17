//******************************************************************************************************
//  SmallLock.cs - Gbtc
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
//  3/7/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GSF.Threading
{
    /// <summary>
    /// Designed to be a wrapper around an <see cref="Object"/> 
    /// to assist in propery implementation of <see cref="Monitor"/>
    /// when more advance functionality is desired.
    /// </summary>
    public struct MonitorHelper : IDisposable
    {
        readonly object m_object;
        bool m_isLocked;
        public MonitorHelper(object syncRoot, bool enter)
        {
            m_object = syncRoot;
            m_isLocked = false;
            if (enter)
                Enter();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enter()
        {
            if (m_isLocked)
                ErrorHelper1();
            Monitor.Enter(m_object);
            m_isLocked = true;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryEnter()
        {
            if (m_isLocked)
                ErrorHelper1();
            m_isLocked = Monitor.TryEnter(m_object);
            return m_isLocked;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Exit()
        {
            if (!m_isLocked)
                ErrorHelper2();
            m_isLocked = false;
            Monitor.Exit(m_object);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (m_isLocked)
            {
                m_isLocked = false;
                Monitor.Exit(m_object);
            }
        }

        void ErrorHelper1()
        {
            throw new Exception("Duplicate calls to Monitor.Enter()");
        }
        void ErrorHelper2()
        {
            throw new Exception("Duplicate calls to Monitor.Exit()");
        }
    }
}
