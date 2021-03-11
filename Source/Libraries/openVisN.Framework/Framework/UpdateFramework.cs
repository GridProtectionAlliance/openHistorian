//******************************************************************************************************
//  UpdateFramework.cs - Gbtc
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
//  12/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using GSF.Threading;
using openHistorian.Data.Query;

namespace openVisN.Framework
{
    public enum ExecutionMode
    {
        Automatic,
        Manual
    }

    public class ExecutionModeEventArgs : EventArgs
    {
        public ExecutionMode Mode
        {
            get;
            private set;
        }

        public ExecutionModeEventArgs(ExecutionMode mode)
        {
            Mode = mode;
        }
    }

    public class QueryResultsEventArgs : EventArgs
    {
        public DateTime StartTime
        {
            get;
            private set;
        }

        public DateTime EndTime
        {
            get;
            private set;
        }

        public object RequestedToken
        {
            get;
            private set;
        }

        public IDictionary<Guid, SignalDataBase> Results
        {
            get;
            private set;
        }

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
        private volatile bool m_disposing;
        private readonly ScheduledTask m_async;

        public event EventHandler BeforeExecuteQuery;
        public event EventHandler AfterQuery;
        public event EventHandler AfterExecuteQuery;
        public event EventHandler<QueryResultsEventArgs> NewQueryResults;
        public event EventHandler<QueryResultsEventArgs> SynchronousNewQueryResults;
        public event EventHandler<QueryResultsEventArgs> ParallelWithControlLockNewQueryResults;
        public event EventHandler<ExecutionModeEventArgs> ExecutionModeChanged;

        private readonly SynchronousEvent<QueryResultsEventArgs> m_syncEvent;

        private TimeSpan m_refreshInterval;
        private readonly AutomaticPlayback m_playback;
        private object m_requestToken;
        private HistorianQuery m_query;
        private readonly object m_syncRoot;
        private volatile bool m_enabled;
        private DateTime m_lowerBounds;
        private DateTime m_upperBounds;
        private DateTime m_focusedDate;
        private ExecutionMode m_mode = ExecutionMode.Manual;

        private List<MetadataBase> m_activeSignals;

        public UpdateFramework()
        {
            m_refreshInterval = new TimeSpan(1 * TimeSpan.TicksPerSecond);
            m_playback = new AutomaticPlayback();
            m_enabled = true;
            m_syncEvent = new SynchronousEvent<QueryResultsEventArgs>();
            m_syncEvent.CustomEvent += m_syncEvent_CustomEvent;
            m_async = new ScheduledTask(ThreadingMode.DedicatedForeground);
            m_async.Running += AsyncDoWork;
            m_async.UnhandledException += OnError;
            m_activeSignals = new List<MetadataBase>();
            m_syncRoot = new object();
        }

        void OnError(object sender, EventArgs eventArgs)
        {
            //string data = e.ExceptionObject.ToString();
            //File.WriteAllText("c:\\error.txt",data);
        }

        public void Start(HistorianQuery query)
        {
            m_query = query;
        }

        private void m_syncEvent_CustomEvent(object sender, QueryResultsEventArgs e)
        {
            if (SynchronousNewQueryResults != null)
            {
                SynchronousNewQueryResults(this, e);
            }
            if (ParallelWithControlLockNewQueryResults != null)
            {
                ParallelWithControlLockNewQueryResults(sender, e);
            }
        }

        private void AsyncDoWork(object sender, EventArgs eventArgs)
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
                token = m_requestToken;
                m_requestToken = null;
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

            IDictionary<Guid, SignalDataBase> results = m_query.GetQueryResult(startTime, stopTime, 0, activeSignals);

            QueryResultsEventArgs queryResults = new QueryResultsEventArgs(results, token, startTime, stopTime);

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
                    m_async.Start(m_refreshInterval.Milliseconds);
                }
            }

            if (AfterExecuteQuery != null)
                AfterExecuteQuery(this, EventArgs.Empty);
        }

        public ExecutionMode Mode
        {
            get => m_mode;
            set
            {
                lock (m_syncRoot)
                {
                    bool updated = m_mode != value;
                    m_requestToken = null;
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
            get => m_enabled;
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
                m_requestToken = null;
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
                m_requestToken = token;
            }
        }

        public double PlaybackSpeed
        {
            get => m_playback.PlaybackSpeed;
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

                m_requestToken = token;
                m_lowerBounds = startTime;
                m_upperBounds = endTime;
            }
            m_async.Start();
        }

        public void UpdateSignals(List<MetadataBase> activeSignals)
        {
            lock (m_syncRoot)
            {
                m_requestToken = null;
                m_activeSignals = activeSignals;
            }
            m_async.Start();
        }

        public void Dispose()
        {
            m_disposing = true;
            Thread.MemoryBarrier();
            m_syncEvent.Dispose();
            m_async.Dispose();
        }
    }
}