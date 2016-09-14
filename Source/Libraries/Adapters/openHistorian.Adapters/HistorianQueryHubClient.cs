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
using System.Collections.Generic;
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
        private SnapClient m_connection;
        private ClientDatabaseBase<HistorianKey, HistorianValue> m_database;
        private CancellationToken m_cancellationToken;
        private bool m_disposed;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets historian connection instance, creating a new one if needed.
        /// </summary>
        private SnapClient Connection
        {
            get
            {
                if ((object)m_connection == null)
                {
                    try
                    {
                        HistorianServer serverInstance;

                        if (LocalOutputAdapter.ServerInstances.TryGetValue(TrendValueAPI.InstanceName, out serverInstance))
                        {
                            if ((object)serverInstance == null)
                                throw new InvalidOperationException("Failed to access internal historian server instance.");

                            m_connection = SnapClient.Connect(serverInstance.Host);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogException(new InvalidOperationException($"Failed to connect to historian: {ex.Message}", ex));
                    }
                }

                return m_connection;
            }
        }

        private ClientDatabaseBase<HistorianKey, HistorianValue> Database
        {
            get
            {
                if ((object)m_database == null)
                {
                    try
                    {
                        SnapClient connection = Connection;

                        if ((object)connection != null)
                            m_database = Connection.GetDatabase<HistorianKey, HistorianValue>(TrendValueAPI.InstanceName);
                    }
                    catch (Exception ex)
                    {
                        LogException(new InvalidOperationException($"Failed to access historian database instance \"{TrendValueAPI.InstanceName}\": {ex.Message}", ex));
                    }
                }

                return m_database;
            }
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
                        m_database?.Dispose();
                        m_connection?.Dispose();
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
        /// Read historian data from server.
        /// </summary>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <param name="seriesLimit">Maximum number of points per series.</param>
        /// <param name="forceLimit">Flag that determines if series limit should be strictly enforced.</param>
        /// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
        public IEnumerable<TrendValue> GetHistorianData(DateTime startTime, DateTime stopTime, ulong[] measurementIDs, Resolution resolution, int seriesLimit, bool forceLimit)
        {
            // Cancel any running query
            CancellationToken cancellationToken = new CancellationToken();
            Interlocked.Exchange(ref m_cancellationToken, cancellationToken)?.Cancel();

            return TrendValueAPI.GetHistorianData(Database, startTime, stopTime, measurementIDs, resolution, seriesLimit, forceLimit, cancellationToken);
        }

        /// <summary>
        /// If the openHistorian adapter parameters get updated, e.g., listening port or instance name, this function can be called to refresh the values.
        /// </summary>
        public void RefreshConnectionParameters()
        {
            TrendValueAPI.LoadConnectionParameters();
            Interlocked.Exchange(ref m_connection, null)?.Dispose();
        }

        #endregion
    }
}