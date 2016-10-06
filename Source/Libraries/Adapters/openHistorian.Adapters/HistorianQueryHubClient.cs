//******************************************************************************************************
//  HistorianQueryHubClient.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  06/07/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GSF.Snap.Services;
using GSF.Web.Hubs;
using openHistorian.Model;
using openHistorian.Net;
using openHistorian.Snap;
using CancellationToken = GSF.Threading.CancellationToken;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a client instance of a SignalR Hub for historian data queries.
    /// </summary>
    public class HistorianQueryHubClient : HubClientBase
    {
        #region [ Members ]

        // Fields
        private readonly ConcurrentDictionary<string, SnapClient> m_connections;
        private readonly ConcurrentDictionary<string, ClientDatabaseBase<HistorianKey, HistorianValue>> m_databases;
        private string m_instanceName = TrendValueAPI.DefaultInstanceName;
        private CancellationToken m_cancellationToken;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="HistorianQueryHubClient"/>.
        /// </summary>
        public HistorianQueryHubClient()
        {
            m_connections = new ConcurrentDictionary<string, SnapClient>(StringComparer.OrdinalIgnoreCase);
            m_databases = new ConcurrentDictionary<string, ClientDatabaseBase<HistorianKey, HistorianValue>>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HistorianQueryHubClient"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        m_databases?.Values.ToList().ForEach(database => database?.Dispose());
                        m_connections?.Values.ToList().ForEach(connection => connection?.Dispose());
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Set selected instance name.
        /// </summary>
        /// <param name="instanceName">Instance name that is selected by user.</param>
        public void SetSelectedInstanceName(string instanceName)
        {
            m_instanceName = instanceName;
        }

        /// <summary>
        /// Gets selected instance name.
        /// </summary>
        /// <returns>Selected instance name.</returns>
        public string GetSelectedInstanceName()
        {
            return m_instanceName;
        }

        /// <summary>
        /// Gets loaded historian adapter instance names.
        /// </summary>
        /// <returns>Historian adapter instance names.</returns>
        public IEnumerable<string> GetInstanceNames() => TrendValueAPI.GetInstanceNames();

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <param name="seriesLimit">Maximum number of points per series.</param>
        /// <param name="forceLimit">Flag that determines if series limit should be strictly enforced.</param>
        /// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
        public IEnumerable<TrendValue> GetHistorianData(string instanceName, DateTime startTime, DateTime stopTime, ulong[] measurementIDs, Resolution resolution, int seriesLimit, bool forceLimit)
        {
            // Cancel any running query
            CancellationToken cancellationToken = new CancellationToken();
            Interlocked.Exchange(ref m_cancellationToken, cancellationToken)?.Cancel();

            return TrendValueAPI.GetHistorianData(GetDatabase(instanceName), startTime, stopTime, measurementIDs, resolution, seriesLimit, forceLimit, cancellationToken);
        }

        private SnapClient GetConnection(string instanceName)
        {
            SnapClient connection;

            if (m_connections.TryGetValue(instanceName, out connection))
                return connection;

            try
            {
                HistorianServer serverInstance = null;
                LocalOutputAdapter historianAdapter;

                if (LocalOutputAdapter.Instances.TryGetValue(instanceName, out historianAdapter))
                    serverInstance = historianAdapter?.Server;

                if ((object)serverInstance == null)
                    return null;

                connection = SnapClient.Connect(serverInstance.Host);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Failed to connect to historian \"{instanceName}\": {ex.Message}", ex));
            }

            if ((object)connection != null)
                m_connections[instanceName] = connection;

            return connection;
        }

        private ClientDatabaseBase<HistorianKey, HistorianValue> GetDatabase(string instanceName)
        {
            ClientDatabaseBase<HistorianKey, HistorianValue> database;

            if (m_databases.TryGetValue(instanceName, out database))
                return database;

            try
            {
                database = GetConnection(instanceName)?.GetDatabase<HistorianKey, HistorianValue>(instanceName);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Failed to access historian database instance \"{instanceName}\": {ex.Message}", ex));
            }

            if ((object)database != null)
                m_databases[instanceName] = database;

            return database;
        }

        #endregion
    }
}