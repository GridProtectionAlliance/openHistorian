//******************************************************************************************************
//  SubscriptionFramework.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  12/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace openVisN
{
    public class SubscriptionFramework
    {

        public enum ExecutionMode
        {
            Automatic,
            Manual
        }

        object m_syncRoot;
        Thread m_queryProcessingThread;
        DateTime m_lowerBounds;
        DateTime m_upperBounds;
        DateTime m_focusedDate;
        ManualResetEvent m_executeUpdate;
        TimeSpan m_automaticExecutionTimeLag;
        ExecutionMode m_mode;
        TimeSpan m_automaticDuration;
        TimeSpan m_refreshInterval;

        List<TerminalSignals> m_allTerminals;
        List<SignalDefinition> m_activeSignals;
 
        public event EventHandler UpdateModeChanged;
        public event EventHandler PointsChanged;
        public event EventHandler StateUpdated;
        public event Action<DateTime, DateTime> DateRangeChanged;

        List<ISubscriber> m_subscribers;

        public SubscriptionFramework()
        {
            
        }

        public void AddSubscriber(ISubscriber subscriber)
        {
            m_subscribers.Add(subscriber);
        }

        public void RemoveSubscriber(ISubscriber subscriber)
        {
            m_subscribers.Remove(subscriber);
        }

        public void ChangeDateRange(DateTime lowerBounds, DateTime upperBounds)
        {

        }

        public void RefreshQuery()
        {
            
        }





    }
}
