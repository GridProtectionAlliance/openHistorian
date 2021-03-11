//******************************************************************************************************
//  HistorianOperationsHubClient.cs - Gbtc
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
using GSF;
using GSF.Snap.Services;
using GSF.Threading;
using GSF.Web.Hubs;
using openHistorian.Model;
using openHistorian.Net;
using openHistorian.Snap;
using CancellationToken = GSF.Threading.CancellationToken;
using Random = GSF.Security.Cryptography.Random;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Defines enumeration of supported timestamp representations.
    /// </summary>
    public enum TimestampType
    {
        /// <summary>
        /// Timestamps are in Ticks.
        /// </summary>
        Ticks,
        /// <summary>
        /// Timestamps are in Unix milliseconds.
        /// </summary>
        UnixMilliseconds,
        /// <summary>
        /// Timestamps are in Unix seconds.
        /// </summary>
        UnixSeconds
    }

    /// <summary>
    /// Represents the current operational state for a given historian write.
    /// </summary>
    public class HistorianWriteOperationState
    {
        private long m_stopTime;

        /// <summary>
        /// Gets or sets the cancellation token for a historian write operation.
        /// </summary>
        public CancellationToken CancellationToken { get; } = new CancellationToken();

        /// <summary>
        /// Gets or sets progress that represents number of completed historian writes.
        /// </summary>
        public long Progress { get; set; }

        /// <summary>
        /// Gets or sets total number of historian writes to be completed.
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// Gets or sets flag that indicates if write operation has successfully completed.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Gets or sets flag that indicates if write operation has failed.
        /// </summary>
        public bool Failed { get; set; }

        /// <summary>
        /// Gets or sets failure reason message when <see cref="Failed"/> is <c>true</c>.
        /// </summary>
        public string FailedReason { get; set; }

        /// <summary>
        /// Gets or sets the start time, in Ticks, of the write operation.
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// Gets or sets the stop time, in Ticks, of the write operation.
        /// </summary>
        public long StopTime
        {
            get => m_stopTime > 0 ? m_stopTime : DateTime.UtcNow.Ticks;
            set => m_stopTime = value;
        }

        /// <summary>
        /// Gets the calculated import rate, in writes per second, of the write operation.
        /// </summary>
        public long ImportRate => (long)(Progress / new Ticks(StopTime - StartTime).ToSeconds());

        /// <summary>
        /// Gets the estimated remaining import time as an elapsed time string.
        /// </summary>
        public string RemainingTimeEstimate => Ticks.FromSeconds((Total - Progress) / (double)ImportRate).ToElapsedTimeString(0);

        /// <summary>
        /// Gets total operation time as an elapsed time string.
        /// </summary>
        public string TotalOperationTime => new Ticks(StopTime - StartTime).ToElapsedTimeString(2);
    }

    /// <summary>
    /// Represents a client instance of a SignalR Hub for historian data operations.
    /// </summary>
    public class HistorianOperationsHubClient : HubClientBase
    {
        #region [ Members ]

        // Fields
        private readonly ConcurrentDictionary<uint, HistorianWriteOperationState> m_historianWriteOperationStates;
        private string m_instanceName = TrendValueAPI.DefaultInstanceName;
        private CancellationToken m_readCancellationToken;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="HistorianOperationsHubClient"/>.
        /// </summary>
        public HistorianOperationsHubClient()
        {
            m_historianWriteOperationStates = new ConcurrentDictionary<uint, HistorianWriteOperationState>();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HistorianOperationsHubClient"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                if (disposing)
                {
                    HistorianWriteOperationState[] operationStates = m_historianWriteOperationStates.Values.ToArray();
                    m_historianWriteOperationStates.Clear();

                    foreach (HistorianWriteOperationState operationState in operationStates)
                        operationState.CancellationToken?.Cancel();
                }
            }
            finally
            {
                m_disposed = true;          // Prevent duplicate dispose.
                base.Dispose(disposing);    // Call base class Dispose().
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
        /// Begins a new historian write operation.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="values">Enumeration of <see cref="TrendValue"/> instances to write.</param>
        /// <param name="totalValues">Total values to write, if known in advance.</param>
        /// <param name="timestampType">Type of timestamps.</param>
        /// <returns>New operational state handle.</returns>
        public uint BeginHistorianWrite(string instanceName, IEnumerable<TrendValue> values, long totalValues = 0, TimestampType timestampType = TimestampType.UnixMilliseconds)
        {
            HistorianWriteOperationState operationState = new HistorianWriteOperationState
            {
                Total = totalValues
            };

            uint operationHandle = Random.UInt32;

            while (!m_historianWriteOperationStates.TryAdd(operationHandle, operationState))
                operationHandle = Random.UInt32;

            new Thread(() =>
            {
                operationState.StartTime = DateTime.UtcNow.Ticks;

                try
                {
                    SnapServer server = GetServer(instanceName)?.Host;

                    if (server is null)
                        throw new InvalidOperationException($"Server is null for instance [{instanceName}].");

                    using (SnapClient connection = SnapClient.Connect(server))
                    using (ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName))
                    {
                        if (database is null)
                            throw new InvalidOperationException($"Database is null for instance [{instanceName}].");

                        HistorianKey key = new HistorianKey();
                        HistorianValue value = new HistorianValue();

                        foreach (TrendValue trendValue in values)
                        {
                            key.PointID = (ulong)trendValue.ID;

                            switch (timestampType)
                            {
                                case TimestampType.Ticks:
                                    key.Timestamp = (ulong)trendValue.Timestamp;
                                    break;
                                case TimestampType.UnixSeconds:
                                    key.Timestamp = (ulong)trendValue.Timestamp * 10000000UL + 621355968000000000UL;
                                    break;
                                case TimestampType.UnixMilliseconds:
                                    key.Timestamp = (ulong)trendValue.Timestamp * 10000UL + 621355968000000000UL;
                                    break;
                            }

                            value.AsSingle = (float)trendValue.Value;

                            database.Write(key, value);
                            operationState.Progress++;

                            if (operationState.CancellationToken.IsCancelled)
                                break;
                        }

                        operationState.Completed = !operationState.CancellationToken.IsCancelled;
                    }
                }
                catch (Exception ex)
                {
                    operationState.Failed = true;
                    operationState.FailedReason = ex.Message;
                }

                // Schedule operation handle to be removed
                CancelHistorianWrite(operationHandle);

                operationState.StopTime = DateTime.UtcNow.Ticks;
            })
            {
                IsBackground = true
            }
            .Start();

            return operationHandle;
        }

        /// <summary>
        /// Gets current historian write operation state for specified handle.
        /// </summary>
        /// <param name="operationHandle">Handle to historian write operation state.</param>
        /// <returns>Current historian write operation state.</returns>
        public HistorianWriteOperationState GetHistorianWriteState(uint operationHandle)
        {
            if (m_historianWriteOperationStates.TryGetValue(operationHandle, out HistorianWriteOperationState operationState))
                return operationState;

            // Returned a cancelled operation state if operation handle was not found
            operationState = new HistorianWriteOperationState();
            operationState.CancellationToken.Cancel();

            return operationState;
        }

        /// <summary>
        /// Cancels a historian write operation.
        /// </summary>
        /// <param name="operationHandle">Handle to historian write operation state.</param>
        /// <returns><c>true</c> if operation was successfully terminated; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Any <see cref="HistorianWriteOperationState"/> associated with the <paramref name="operationHandle"/>
        /// will remain available for query from the <see cref="GetHistorianWriteState"/> for 30 seconds after
        /// successful cancellation of the operation.
        /// </remarks>
        public bool CancelHistorianWrite(uint operationHandle)
        {
            if (!m_historianWriteOperationStates.TryGetValue(operationHandle, out HistorianWriteOperationState operationState))
                return false;

            // Immediately signal for operation cancellation
            operationState.CancellationToken.Cancel();

            // Schedule operation handle to be removed after about 30 seconds
            Action action = () => m_historianWriteOperationStates.TryRemove(operationHandle, out operationState);
            action.DelayAndExecute(30000);

            return true;
        }

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
        /// <param name="timestampType">Type of timestamps.</param>
        /// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
        public IEnumerable<TrendValue> GetHistorianData(string instanceName, DateTime startTime, DateTime stopTime, ulong[] measurementIDs, Resolution resolution, int seriesLimit, bool forceLimit, TimestampType timestampType = TimestampType.UnixMilliseconds)
        {
            // Cancel any running operation
            CancellationToken cancellationToken = new CancellationToken();
            Interlocked.Exchange(ref m_readCancellationToken, cancellationToken)?.Cancel();

            SnapServer server = GetServer(instanceName)?.Host;
            IEnumerable<TrendValue> values = TrendValueAPI.GetHistorianData(server, instanceName, startTime, stopTime, measurementIDs, resolution, seriesLimit, forceLimit, cancellationToken);

            switch (timestampType)
            {
                case TimestampType.Ticks:
                    return values.Select(value =>
                    {
                        value.Timestamp = value.Timestamp * 10000.0D + 621355968000000000.0D;
                        return value;
                    });
                case TimestampType.UnixSeconds:
                    return values.Select(value =>
                    {
                        value.Timestamp = value.Timestamp / 1000.0D;
                        return value;
                    });
                default:
                    return values;
            }
        }

        private HistorianServer GetServer(string instanceName)
        {
            if (LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter historianAdapter))
                return historianAdapter?.Server;

            return null;
        }

        #endregion
    }
}