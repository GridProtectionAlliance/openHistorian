//******************************************************************************************************
//  UpdateFramework.cs - Gbtc
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
//  12/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Data.Query;
using GSF.Threading;

namespace openVisN.Framework
{

    public enum ExecutionMode
    {
        Automatic,
        Manual
    }

    public class QueryResultsEventArgs : EventArgs
    {
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public object RequestedToken { get; private set; }
        public IDictionary<Guid, SignalDataBase> Results { get; private set; }
        public QueryResultsEventArgs(IDictionary<Guid, SignalDataBase> results, object requestToken, DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
            RequestedToken = requestToken;
            Results = results;
        }
    }

    public class UpdateFramework : IDisposable
    {
        AsyncRunner m_async;

        public event EventHandler<QueryResultsEventArgs> NewQueryResults;
        public event EventHandler<QueryResultsEventArgs> SynchronousNewQueryResults;

        SynchronousEvent<QueryResultsEventArgs> m_syncEvent;

        object m_RequestToken;
        HistorianQuery m_query;
        object m_syncRoot;
        volatile bool m_enabled;
        DateTime m_lowerBounds;
        DateTime m_upperBounds;
        DateTime m_focusedDate;
        ExecutionMode m_mode = ExecutionMode.Manual;

        TimeSpan m_automaticExecutionTimeLag;
        TimeSpan m_automaticDurationWindow;
        TimeSpan m_refreshInterval;

        List<MetadataBase> m_activeSignals;

        public UpdateFramework()
        {
            m_enabled = true;
            m_syncEvent = new SynchronousEvent<QueryResultsEventArgs>();
            m_syncEvent.CustomEvent += m_syncEvent_CustomEvent;
            m_async = new AsyncRunner();
            m_async.Running += m_async_Running;
            m_activeSignals = new List<MetadataBase>();
            m_syncRoot = new object();
        }

        public void Start(HistorianQuery query)
        {
            m_query = query;
        }

        void m_syncEvent_CustomEvent(object sender, QueryResultsEventArgs e)
        {
            if (SynchronousNewQueryResults != null)
                SynchronousNewQueryResults(this, e);
        }

        void m_async_Running(object sender, EventArgs e)
        {
            DateTime startTime;
            DateTime stopTime;
            DateTime currentTime;
            object token;
            List<MetadataBase> activeSignals;

            lock (m_syncRoot)
            {
                token = m_RequestToken;
                if (Mode == ExecutionMode.Manual)
                {
                    startTime = m_lowerBounds;
                    stopTime = m_upperBounds;
                    currentTime = m_focusedDate;
                    activeSignals = m_activeSignals;
                }
                else
                {
                    startTime = DateTime.UtcNow.Subtract(m_automaticExecutionTimeLag);
                    stopTime = startTime.Add(m_automaticDurationWindow);
                    currentTime = startTime;
                    activeSignals = m_activeSignals;
                }
            }

            var results = m_query.GetQueryResult(startTime, stopTime, 0, activeSignals);

            if (NewQueryResults != null)
                NewQueryResults(this, new QueryResultsEventArgs(results, token, startTime, stopTime));
            if (SynchronousNewQueryResults != null)
                m_syncEvent.RaiseEvent(new QueryResultsEventArgs(results, token, startTime, stopTime));

            lock (m_syncRoot)
            {
                if (Mode == ExecutionMode.Automatic)
                {
                    m_async.RunAfterDelay(m_refreshInterval);
                }
            }
        }

        public ExecutionMode Mode
        {
            get
            {
                return m_mode;
            }
            set
            {
                m_mode = value;
                m_async.Run();
            }
        }

        public bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                if (!m_enabled && value)
                {
                    throw new Exception("Cannot be restarted");
                }
                if (!value)
                    m_async.StopExecuting();
                m_enabled = value;
            }
        }

        public void Execute()
        {
            lock (m_syncRoot)
                m_RequestToken = null;
            m_async.Run();
        }

        public void Execute(DateTime startTime, DateTime endTime, object token)
        {
            lock (m_syncRoot)
            {
                m_RequestToken = token;
                m_lowerBounds = startTime;
                m_upperBounds = endTime;
            }
            m_async.Run();
        }

        public void UpdateSignals(List<MetadataBase> activeSignals)
        {
            lock (m_syncRoot)
            {
                m_RequestToken = null;
                m_activeSignals = activeSignals;
            }
            m_async.Run();
        }

        public void Dispose()
        {
            m_async.StopExecuting();
        }
    }
}
