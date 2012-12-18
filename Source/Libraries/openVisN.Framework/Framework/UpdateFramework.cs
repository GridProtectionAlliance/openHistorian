//******************************************************************************************************
//  UpdateFramework.cs - Gbtc
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
using System.Threading;
using openHistorian.Data.Query;

namespace openVisN.Framework
{

    public enum ExecutionMode
    {
        Automatic,
        Manual
    }

    public class QueryResultsEventArgs : EventArgs
    {
        public IDictionary<Guid,SignalDataBase> Results { get; private set; }
        public QueryResultsEventArgs(IDictionary<Guid, SignalDataBase> results)
        {
            Results = results;
        }
    }

    public class UpdateFramework
    {
        public event EventHandler<QueryResultsEventArgs> NewQueryResults;

        HistorianQuery m_query;
        object m_syncRoot;
        volatile bool m_enabled;
        Thread m_queryProcessingThread;
        DateTime m_lowerBounds;
        DateTime m_upperBounds;
        DateTime m_focusedDate;
        ManualResetEvent m_executeUpdate;
        ExecutionMode m_mode = ExecutionMode.Manual;

        TimeSpan m_automaticExecutionTimeLag;
        TimeSpan m_automaticDurationWindow;
        TimeSpan m_refreshInterval;

        List<MetadataBase> m_activeSignals;

        public UpdateFramework(HistorianQuery query)
        {
            m_activeSignals=new List<MetadataBase>();
            m_query = query;
            m_syncRoot = new object();
            m_executeUpdate=new ManualResetEvent(false);
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
                    m_enabled = true;
                    m_queryProcessingThread = new Thread(ProcessUpdates);
                    m_queryProcessingThread.IsBackground = true;
                    m_queryProcessingThread.Start();
                }
                m_enabled = value;
            }
        }

        public void Execute()
        {
            m_executeUpdate.Set();
        }

        public void Execute(DateTime startTime, DateTime endTime)
        {
            lock(m_syncRoot)
            {
                m_lowerBounds = startTime;
                m_upperBounds = endTime;
            }
            m_executeUpdate.Set();
        }

        public void UpdateSignals(List<MetadataBase> activeSignals)
        {
            lock(m_syncRoot)
            {
                m_activeSignals = activeSignals;
            }
            m_executeUpdate.Set();
        }

        void ProcessUpdates()
        {
            while (m_enabled)
            {
                if (Mode == ExecutionMode.Manual)
                {
                    m_executeUpdate.WaitOne();
                    m_executeUpdate.Reset();
                    ExecuteQuery();
                }
                else
                {
                    m_executeUpdate.WaitOne(m_refreshInterval);
                    m_executeUpdate.Reset();
                    ExecuteQuery();
                }
            }
        }

        void ExecuteQuery()
        {
            DateTime startTime;
            DateTime stopTime;
            DateTime currentTime;
            List<MetadataBase> activeSignals;

            lock (m_syncRoot)
            {
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
            NewQueryResults(this, new QueryResultsEventArgs(results));
        }



    }
}
