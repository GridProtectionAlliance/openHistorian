//******************************************************************************************************
//  UpdateFramework.cs - Gbtc
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
//  12/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using openHistorian.Data.Query;
using GSF.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace openVisN.Framework
{

    public enum ExecutionMode
    {
        Automatic,
        Manual
    }

    public class ExecutionModeEventArgs : EventArgs
    {
        public ExecutionMode Mode { get; private set; }
        public ExecutionModeEventArgs(ExecutionMode mode)
        {
            Mode = mode;
        }
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
        ScheduledTask m_async;

        public event EventHandler BeforeExecuteQuery;
        public event EventHandler AfterQuery;
        public event EventHandler AfterExecuteQuery;
        public event EventHandler<QueryResultsEventArgs> NewQueryResults;
        public event EventHandler<QueryResultsEventArgs> SynchronousNewQueryResults;
        public event EventHandler<QueryResultsEventArgs> ParallelWithControlLockNewQueryResults;
        public event EventHandler<ExecutionModeEventArgs> ExecutionModeChanged;

        SynchronousEvent<QueryResultsEventArgs> m_syncEvent;

        TimeSpan m_refreshInterval;
        AutomaticPlayback m_playback;
        object m_RequestToken;
        HistorianQuery m_query;
        object m_syncRoot;
        volatile bool m_enabled;
        DateTime m_lowerBounds;
        DateTime m_upperBounds;
        DateTime m_focusedDate;
        ExecutionMode m_mode = ExecutionMode.Manual;

        List<MetadataBase> m_activeSignals;

        public UpdateFramework()
        {
            m_refreshInterval = new TimeSpan(1 * TimeSpan.TicksPerSecond);
            m_playback = new AutomaticPlayback();
            m_enabled = true;
            m_syncEvent = new SynchronousEvent<QueryResultsEventArgs>();
            m_syncEvent.CustomEvent += m_syncEvent_CustomEvent;
            m_async = new ScheduledTask(AsyncDoWork);
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
            {
                SynchronousNewQueryResults(this, e);
            }
            if (ParallelWithControlLockNewQueryResults != null)
            {
                ParallelWithControlLockNewQueryResults.ParallelRunAndWait(sender, e);
            }
        }

        void AsyncDoWork()
        {
            if (BeforeExecuteQuery != null)
                BeforeExecuteQuery(this, EventArgs.Empty);
            DateTime startTime;
            DateTime stopTime;
            DateTime currentTime;
            object token;
            List<MetadataBase> activeSignals;

            lock (m_syncRoot)
            {
                token = m_RequestToken;
                m_RequestToken = null;
                if (Mode == ExecutionMode.Manual)
                {
                    startTime = m_lowerBounds;
                    stopTime = m_upperBounds;
                    currentTime = m_focusedDate;
                    activeSignals = m_activeSignals;
                }
                else
                {
                    m_playback.GetTimes(out startTime, out stopTime);
                    currentTime = stopTime;
                    activeSignals = m_activeSignals;
                }
            }

            var results = m_query.GetQueryResult(startTime, stopTime, 0, activeSignals);

            var queryResults = new QueryResultsEventArgs(results, token, startTime, stopTime);

            if (AfterQuery != null)
                AfterQuery(this, EventArgs.Empty);

            if (NewQueryResults != null)
                NewQueryResults.ParallelRunAndWait(this, queryResults);
            if (SynchronousNewQueryResults != null || ParallelWithControlLockNewQueryResults != null)
                m_syncEvent.RaiseEvent(new QueryResultsEventArgs(results, token, startTime, stopTime));

            lock (m_syncRoot)
            {
                if (Mode == ExecutionMode.Automatic)
                {
                    m_async.Start(m_refreshInterval);
                }
            }

            if (AfterExecuteQuery != null)
                AfterExecuteQuery(this, EventArgs.Empty);
        }

        public ExecutionMode Mode
        {
            get
            {
                return m_mode;
            }
            set
            {
                lock (m_syncRoot)
                {
                    bool updated = m_mode != value;
                    m_RequestToken = null;
                    m_mode = value;
                    if (updated && ExecutionModeChanged != null)
                        ExecutionModeChanged(this, new ExecutionModeEventArgs(value));
                    m_playback.LiveModeSelected();
                    m_async.Start();
                }

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
                    m_async.Dispose();
                m_enabled = value;
            }
        }

        public void Execute()
        {
            lock (m_syncRoot)
                m_RequestToken = null;
            m_async.Start();
        }

        public void SwitchToAutomatic(bool useCurrentWindow)
        {
            if (useCurrentWindow)
            {
                lock (m_syncRoot)
                {
                    DateTime startTime = m_lowerBounds;
                    DateTime stoptime = m_upperBounds;
                    TimeSpan windowSize = stoptime - startTime;
                    m_refreshInterval = new TimeSpan(1 * TimeSpan.TicksPerMillisecond);

                    m_playback.ChangeWindowSize(windowSize);
                    m_playback.StartPlaybackFrom(stoptime);
                    Mode = ExecutionMode.Automatic;
                }
            }
            else
            {

            }
        }

        public void SwitchToManual(bool useCurrentWindow)
        {
            if (useCurrentWindow)
            {
                lock (m_syncRoot)
                {
                    m_playback.GetTimes(out m_lowerBounds, out m_upperBounds);
                    Mode = ExecutionMode.Manual;
                }
            }
            else
            {

            }
        }


        public void GetTimeWindow(out DateTime startTime, out DateTime endTime)
        {
            lock (m_syncRoot)
            {
                startTime = m_lowerBounds;
                endTime = m_upperBounds;
            }
        }

        public void SetWindowDuration(TimeSpan duration, object token)
        {
            lock (m_syncRoot)
            {
                m_playback.ChangeWindowSize(duration);
                m_RequestToken = token;
            }
        }

        public double PlaybackSpeed
        {
            get
            {
                return m_playback.PlaybackSpeed;
            }
            set
            {
                lock (m_syncRoot)
                {
                    m_playback.PlaybackSpeed = value;
                }
            }

        }


        public void Execute(DateTime startTime, DateTime endTime, object token)
        {
            lock (m_syncRoot)
            {
                if (Mode == ExecutionMode.Automatic)
                {
                    return;
                }

                m_RequestToken = token;
                m_lowerBounds = startTime;
                m_upperBounds = endTime;
            }
            m_async.Start();
        }

        public void UpdateSignals(List<MetadataBase> activeSignals)
        {
            lock (m_syncRoot)
            {
                m_RequestToken = null;
                m_activeSignals = activeSignals;
            }
            m_async.Start();
        }

        public void Dispose()
        {
            m_async.Dispose();
        }
    }
}
