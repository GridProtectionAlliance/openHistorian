//******************************************************************************************************
//  SubscriptionFramework.cs - Gbtc
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
using System.Linq;
using openHistorian.Data.Query;
using GSF.Snap.Services;
using openHistorian.Net;
using openVisN.Calculations;
using openVisN.Library;

namespace openVisN.Framework
{
    public class SubscriptionFramework : IDisposable
    {
        private HistorianServer m_localServer;

        private readonly SignalAssignment m_angleReference;
        private readonly UpdateFramework m_updateFramework;
        private object m_syncRoot;
        private string m_database;

        private readonly HashSet<SignalGroup> m_allSignalGroups;
        private readonly HashSet<SignalGroup> m_activeSignalGroups;
        private readonly HashSet<MetadataBase> m_activeSignals;
        private readonly HashSet<MetadataBase> m_allSignals;

        public event EventHandler UpdateModeChanged;
        public event EventHandler PointsChanged;
        public event EventHandler StateUpdated;
        public event Action<DateTime, DateTime> DateRangeChanged;

        private readonly List<ISubscriber> m_subscribers;

        public SubscriptionFramework()
        {
            m_angleReference = new SignalAssignment();
            m_subscribers = new List<ISubscriber>();

            m_allSignalGroups = new HashSet<SignalGroup>();
            m_allSignals = new HashSet<MetadataBase>();
            m_activeSignalGroups = new HashSet<SignalGroup>();
            m_activeSignals = new HashSet<MetadataBase>();

            ReLoadSignalsAndSignalGroups();
            m_updateFramework = new UpdateFramework();
        }

        public SubscriptionFramework(string[] paths)
            : this()
        {
            Start(paths);
        }

        public void Start(string[] paths)
        {
            m_database = "PPA";
            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig(m_database, null, false);
            settings.ImportPaths.AddRange(paths);
            m_localServer = new HistorianServer(settings);
            HistorianQuery query = new HistorianQuery(SnapClient.Connect(m_localServer.Host));

            m_updateFramework.Start(query);
            m_updateFramework.Mode = ExecutionMode.Manual;
            m_updateFramework.Enabled = true;
        }

        public void Start(string ip, int port, string database)
        {
            HistorianClient client = new HistorianClient(ip, port);
            m_database = database;
            HistorianQuery query = new HistorianQuery(client);
            m_updateFramework.Start(query);
            m_updateFramework.Mode = ExecutionMode.Manual;
            m_updateFramework.Enabled = true;
        }

        public IEnumerable<MetadataBase> AllSignals => m_allSignals;

        public IEnumerable<SignalGroup> AllSignalGroups => m_allSignalGroups;

        public void ReLoadSignalsAndSignalGroups()
        {
            m_angleReference.GetPoints(out MetadataBase angleReference);

            AllSignals allSignals = new AllSignals();
            AllSignalGroups allSignalGroups = new AllSignalGroups();

            m_allSignals.Clear();
            allSignals.Signals.ForEach(x => m_allSignals.Add(x.MakeSignal()));
            Dictionary<ulong, MetadataBase> allPoints = m_allSignals.ToDictionary(signal => signal.HistorianId.Value);

            m_allSignalGroups.Clear();
            allSignalGroups.SignalGroups.ForEach(x =>
            {
                SignalGroup group = x.CreateGroup(allPoints, angleReference);
                group.GetAllSignals().ToList().ForEach(y => m_allSignals.Add(y));
                m_allSignalGroups.Add(group);
            });
        }

        public void AddSubscriber(ISubscriber subscriber)
        {
            m_subscribers.Add(subscriber);
            subscriber.Initialize(this);
        }

        public void RemoveSubscriber(ISubscriber subscriber)
        {
            m_subscribers.Remove(subscriber);
            RefreshActivePoints();
        }

        public void ActivateSignalGroup(SignalGroup signalGroup)
        {
            m_activeSignalGroups.Add(signalGroup);
            RefreshActivePoints();
        }

        public void DeactivateSignalGroup(SignalGroup signalGroup)
        {
            m_activeSignalGroups.Remove(signalGroup);
            RefreshActivePoints();
        }

        public void ActivateSignal(MetadataBase signal)
        {
            m_activeSignals.Add(signal);
            RefreshActivePoints();
        }

        public void DeactivateSignal(MetadataBase signal)
        {
            m_activeSignals.Remove(signal);
            RefreshActivePoints();
        }

        public void SetAngleReference(MetadataBase signal)
        {
            m_angleReference.SetNewBaseSignal(signal);
            RefreshActivePoints();
        }

        private void RefreshActivePoints()
        {
            HashSet<MetadataBase> currentSignals = new HashSet<MetadataBase>();
            foreach (MetadataBase signal in m_activeSignals)
                currentSignals.Add(signal);
            foreach (ISubscriber subscriber in m_subscribers)
                subscriber.GetAllDesiredSignals(currentSignals, m_activeSignalGroups);

            foreach (MetadataBase signal in currentSignals.ToArray())
            {
                signal.Calculations.AddDependentPoints(currentSignals);
            }
            m_updateFramework.UpdateSignals(currentSignals.ToList());
        }

        public void ChangeDateRange(DateTime lowerBounds, DateTime upperBounds, object token = null)
        {
            m_updateFramework.Execute(lowerBounds, upperBounds, token);
        }


        public void RefreshQuery()
        {
            m_updateFramework.Execute();
        }

        public UpdateFramework Updater => m_updateFramework;

        public void Dispose()
        {
            m_updateFramework.Dispose();
        }
    }
}