//******************************************************************************************************
//  SubscriptionFramework.cs - Gbtc
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
using System.Linq;
using System.Threading;
using openHistorian;
using openHistorian.Engine;
using openVisN.Calculations;
using openVisN.Library;
using openHistorian.Data.Query;

namespace openVisN.Framework
{
    public class SubscriptionFramework
    {

        public event EventHandler<QueryResultsEventArgs> NewQueryResults
        {
            add
            {
                m_updateFramework.NewQueryResults += value;
            }
            remove
            {
                m_updateFramework.NewQueryResults -= value;
            }
        }

        public event EventHandler<QueryResultsEventArgs> SynchronousNewQueryResults
        {
            add
            {
                m_updateFramework.SynchronousNewQueryResults += value;
            }
            remove
            {
                m_updateFramework.SynchronousNewQueryResults -= value;
            }
        }

        SignalAssignment m_angleReference;
        UpdateFramework m_updateFramework;
        object m_syncRoot;

        HashSet<SignalGroup> m_allSignalGroups;
        HashSet<SignalGroup> m_activeSignalGroups;
        HashSet<MetadataBase> m_activeSignals;
        HashSet<MetadataBase> m_allSignals;

        public event EventHandler UpdateModeChanged;
        public event EventHandler PointsChanged;
        public event EventHandler StateUpdated;
        public event Action<DateTime, DateTime> DateRangeChanged;

        List<ISubscriber> m_subscribers;

        public SubscriptionFramework()
        {
            m_angleReference = new SignalAssignment();
            m_subscribers = new List<ISubscriber>();

            m_allSignalGroups = new HashSet<SignalGroup>();
            m_allSignals = new HashSet<MetadataBase>();
            m_activeSignalGroups = new HashSet<SignalGroup>();
            m_activeSignals = new HashSet<MetadataBase>();

            LoadSignalsAndSignalGroups();
            m_updateFramework = new UpdateFramework();

        }

        public SubscriptionFramework(string[] paths)
            : this()
        {
            Start(paths);
        }

        public void Start(string[] paths)
        {
            HistorianDatabaseCollection databaseCollection = new HistorianDatabaseCollection();
            databaseCollection.Add("Full Resolution Synchrophasor", new ArchiveDatabaseEngine(null, paths));
            HistorianQuery query = new HistorianQuery(databaseCollection);
           m_updateFramework.Start(query);
            m_updateFramework.Mode = ExecutionMode.Manual;
            m_updateFramework.Enabled = true;

        }

        public IEnumerable<MetadataBase> AllSignals
        {
            get
            {
                return m_allSignals;
            }
        }

        public IEnumerable<SignalGroup> AllSignalGroups
        {
            get
            {
                return m_allSignalGroups;
            }
        }

        void LoadSignalsAndSignalGroups()
        {
            MetadataBase angleReference;
            m_angleReference.GetPoints(out angleReference);

            var allSignals = new AllSignals();
            var allSignalGroups = new AllSignalGroups();

            allSignals.Signals.ForEach(x => m_allSignals.Add(x.MakeSignal()));
            Dictionary<ulong, MetadataBase> allPoints = m_allSignals.ToDictionary(signal => signal.HistorianId.Value);

            allSignalGroups.SignalGroups.ForEach(x =>
                {
                    var group = x.CreateGroup(allPoints, angleReference);
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

        void RefreshActivePoints()
        {
            HashSet<MetadataBase> currentSignals = new HashSet<MetadataBase>();
            foreach (var signal in m_activeSignals)
                currentSignals.Add(signal);
            foreach (var subscriber in m_subscribers)
                subscriber.GetAllDesiredSignals(currentSignals, m_activeSignalGroups);

            foreach (var signal in currentSignals.ToArray())
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



    }
}
