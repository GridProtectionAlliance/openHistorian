//******************************************************************************************************
//  HistorianReportOperations.cs - Gbtc
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
//  10/14/2019 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using GSF;
using GSF.Data;
using GSF.Data.Model;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using openHistorian.Model;
using openHistorian.Net;
using openHistorian.Snap;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Defines a DataPoint from the Historian.
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// Timestamp.
        /// </summary>
        public ulong Timestamp;

        /// <summary>
        /// PointID of the Signal.
        /// </summary>
        public ulong PointID;

        /// <summary>
        /// Value.
        /// </summary>
        public ulong Value;

        /// <summary>
        /// Flags stored by the openHistorian.
        /// </summary>
        public ulong Flags;

        /// <summary>
        /// Value as a Float.
        /// </summary>
        public float ValueAsSingle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => BitConvert.ToSingle(Value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Value = BitConvert.ToUInt64(value);
        }

        /// <summary>
        /// Check if the Datapoint is empty.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => Timestamp == 0 && PointID == 0 && Value == 0 && Flags == 0;

        /// <summary>
        /// Empty the Data Point.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Timestamp = 0;
            PointID = 0;
            Value = 0;
            Flags = 0;
        }

        /// <summary>
        /// Copy the Datapoint into dataPoint.
        /// </summary>
        /// <param name="dataPoint">Destination of the Copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clone(DataPoint dataPoint)
        {
            dataPoint.Timestamp = Timestamp;
            dataPoint.PointID = PointID;
            dataPoint.Value = Value;
            dataPoint.Flags = Flags;
        }

        /// <summary>
        /// Copy the Datapoint into a new dataPoint.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataPoint Clone()
        {
            DataPoint dataPoint = new DataPoint();
            Clone(dataPoint);
            return dataPoint;
        }

        /// <summary>
        /// Round the Timestamp to the closest possible value accoring to sampling rate.
        /// </summary>
        /// <param name="timestamp">Timestamp to be rounded.</param>
        /// <param name="frameRate">Sampling Rate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RoundTimestamp(DateTime timestamp, int frameRate) => (ulong)Ticks.RoundToSubsecondDistribution(timestamp, frameRate).Value;

        /// <summary>
        /// Round the Timestamp to the closest possible value accoring to sampling rate.
        /// </summary>
        /// <param name="timestamp">Timestamp to be rounded.</param>
        /// <param name="frameRate">Sampling Rate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RoundTimestamp(ulong timestamp, int frameRate) => (ulong)Ticks.RoundToSubsecondDistribution((long)timestamp, frameRate).Value;

        /// <summary>
        /// Check if two timestamps align.
        /// </summary>
        /// <param name="left">Timestamp to be compared.</param>
        /// <param name="right">Timestamp to be compared.</param>
        /// <param name="frameRate">Sampling Rate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareTimestamps(ulong left, ulong right, int frameRate) => RoundTimestamp(left, frameRate).CompareTo(RoundTimestamp(right, frameRate));
    }

    /// <summary>
    /// Defines a Condensed DataPoint from the Historian.
    /// Includes Sums; number of points; min; max;
    /// </summary>
    public class CondensedDataPoint
    {
        /// <summary>
        /// PointID of the Signal.
        /// </summary>
        public ulong PointID;

        /// <summary>
        /// Minimum.
        /// </summary>
        public double Min;

        /// <summary>
        /// Maximum.
        /// </summary>
        public double Max;

        /// <summary>
        /// Total Number of Points.
        /// </summary>
        public int TotalPoints;

        /// <summary>
        /// Sum (X).
        /// </summary>
        public double Sum;

        /// <summary>
        /// Sum (X*X).
        /// </summary>
        public double SqrSum;

        /// <summary>
        /// nu,ber of points exceeding alert threshold.
        /// </summary>
        public int Alert;

        /// <summary>
        /// Intializes a new condesed data point.
        /// </summary>
        /// <param name="PointID">PointID.</param>
        public static CondensedDataPoint EmptyPoint(ulong PointID)
        {
            return new CondensedDataPoint
            {
                Max = double.MinValue,
                Min = double.MaxValue,
                SqrSum = 0,
                Sum = 0,
                TotalPoints = 0,
                PointID = PointID,
                Alert = 0
            };
        }
    }


    /// <summary>
    /// Defines a connection to read Data from the OpenHistorian.
    /// </summary>
    public sealed class ReportHistorianReader : IDisposable
    {
        private readonly SnapClient m_client;
        private readonly ClientDatabaseBase<HistorianKey, HistorianValue> m_database;
        private readonly TreeStream<HistorianKey, HistorianValue> m_stream;
        private readonly HistorianKey m_key;
        private readonly HistorianValue m_value;
        private bool m_disposed;

        /// <summary>
        /// Creates a new <see cref="ReportHistorianReader"/>.
        /// </summary>
        /// <param name="server">Snapserver to connect to <see cref="SnapServer"/>.</param>
        /// <param name="instanceName">Name of the instance to connect to.</param>
        /// <param name="startTime">Starttime.</param>
        /// <param name="endTime">Endtime.</param>
        /// <param name="frameRate">SamplingRate of the signal.</param>
        /// <param name="pointIDs">PointIDs to be collected.</param>
        public ReportHistorianReader(SnapServer server, string instanceName, DateTime startTime, DateTime endTime, int frameRate, IEnumerable<ulong> pointIDs)
        {
            m_client = SnapClient.Connect(server);
            m_database = m_client.GetDatabase<HistorianKey, HistorianValue>(instanceName);
            m_key = new HistorianKey();
            m_value = new HistorianValue();

            SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(DataPoint.RoundTimestamp(startTime, frameRate), DataPoint.RoundTimestamp(endTime, frameRate));
            MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(pointIDs);

            m_stream = m_database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);
        }

        /// <summary>
        /// Destructor for <see cref="ReportHistorianReader"/>.
        /// </summary>
        ~ReportHistorianReader()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ReportHistorianReader"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ReportHistorianReader"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (m_disposed)
                return;
            
            try
            {
                if (disposing)
                {
                    m_stream?.Dispose();
                    m_database?.Dispose();
                    m_client?.Dispose();
                }
            }
            finally
            {
                m_disposed = true;  // Prevent duplicate dispose.
            }
        }

        /// <summary>
        /// Read DataPoint and advance to the next point.
        /// </summary>
        /// <param name="point">DataPoint object to be updated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadNext(DataPoint point)
        {
            if (!m_stream.Read(m_key, m_value))
                return false;

            point.Timestamp = m_key.Timestamp;
            point.PointID = m_key.PointID;
            point.Value = m_value.Value1;
            point.Flags = m_value.Value3;

            return true;
        }

        /// <summary>
        /// Read DataPoint.
        /// </summary>
        /// <param name="point">DataPoint object to be updated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadCurrent(DataPoint point)
        {
            point.Timestamp = m_key.Timestamp;
            point.PointID = m_key.PointID;
            point.Value = m_value.Value1;
            point.Flags = m_value.Value3;
        }
    }

    /// <summary>
    /// Defines a Client to read historian data using <see cref="ReportHistorianReader"/>.
    /// </summary>
    public sealed class ReportHistorianOperations
    {
        private string m_instance;
        private string m_defaultInstanceName;
        private int m_portNumber;

        /// <summary>
        /// Gets configured instance name for historian.
        /// </summary>
        public string DefaultInstanceName => m_defaultInstanceName;

        /// <summary>
        /// Gets configured port number for historian.
        /// </summary>
        public int PortNumber => m_portNumber;

        /// <summary>
        /// Creates a new <see cref="ReportHistorianOperations"/>.
        /// </summary>
        public ReportHistorianOperations()
        {
            LoadConnectionParameters();
        }

        private void LoadConnectionParameters()
        {
            try
            {
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    TableOperations<IaonOutputAdapter> operations = new TableOperations<IaonOutputAdapter>(connection);
                    IaonOutputAdapter record = operations.QueryRecordWhere("TypeName = {0}", typeof(LocalOutputAdapter).FullName);

                    if (record == null)
                        throw new NullReferenceException("Primary openHistorian adapter instance not found.");

                    Dictionary<string, string> settings = record.ConnectionString.ParseKeyValuePairs();

                    if (!settings.TryGetValue("port", out string setting) || !int.TryParse(setting, out m_portNumber))
                        m_portNumber = Connection.DefaultHistorianPort;

                    if (!settings.TryGetValue("instanceName", out m_defaultInstanceName) || string.IsNullOrWhiteSpace(m_defaultInstanceName))
                        m_defaultInstanceName = record.AdapterName ?? "PPA";
                }
            }
            catch
            {
                m_defaultInstanceName = "PPA";
                m_portNumber = Connection.DefaultHistorianPort;
            }
        }

        /// <summary>
        /// Gets loaded historian adapter instance names.
        /// </summary>
        /// <returns>Historian adapter instance names.</returns>
        public IEnumerable<string> GetInstanceNames() => LocalOutputAdapter.Instances.Keys;

        /// <summary>
        /// Set selected instance name.
        /// </summary>
        /// <param name="instanceName">Instance name that is selected by user.</param>
        public void SetSelectedInstanceName(string instanceName)
        {
            m_instance = instanceName;
        }

        /// <summary>
        /// Gets selected instance name.
        /// </summary>
        /// <returns>Selected instance name.</returns>
        public string GetSelectedInstanceName()
        {
            return m_instance;
        }

        /// <summary>
        /// Reads Data From the Historian and returns condensed DataPoints.
        /// </summary>
        /// <param name="start">Starttime.</param>
        /// <param name="end">EndTime.</param>
        /// <param name="measurements">Measurements to be read.</param>
        /// <param name="threshold">Threshhold used to determine number of alerts.</param>   
        /// <param name="cancelationToken">Cancleation Token for the operation.</param>  
        /// <param name="progress"> Progress Tracker <see cref="IProgress{T}"/>.</param>  
        public IEnumerable<CondensedDataPoint> ReadCondensed(DateTime start, DateTime end, IEnumerable<ActiveMeasurement> measurements, double threshold, CancellationToken cancelationToken, IProgress<ulong> progress)
        {
            // Enumerate measurements once
            ActiveMeasurement[] activeMeasurements = measurements.ToArray();
            List<CondensedDataPoint> result = new List<CondensedDataPoint>(activeMeasurements.Length);

            SnapServer server = GetServer(m_instance)?.Host;
            
            // start by separating all framerates
            foreach (int frameRate in activeMeasurements.Select(item => item.FramesPerSecond.GetValueOrDefault()).Distinct())
            {
                ulong[] pointIDs= activeMeasurements.Where(item => item.FramesPerSecond == frameRate).Select(item => item.PointID).ToArray();
                Dictionary<ulong, CondensedDataPoint> frameRateResult = new Dictionary<ulong, CondensedDataPoint>();

                foreach (ulong key in pointIDs)
                {
                    frameRateResult.Add(key,CondensedDataPoint.EmptyPoint(key));

                    if (cancelationToken.IsCancellationRequested)
                        return result;
                }

                using (ReportHistorianReader reader = new ReportHistorianReader(server, m_instance, start, end, frameRate, pointIDs))
                {
                    DataPoint point = new DataPoint();

                    // this.mProgress.Report(this.m_prevProgress);

                    // Scan to first record
                    if (!reader.ReadNext(point))
                        throw new InvalidOperationException("No data for specified time range in openHistorian connection!");

                    ulong currentTimeStamp = point.Timestamp;

                    while (reader.ReadNext(point))
                    {
                        if (currentTimeStamp < point.Timestamp)
                            progress.Report(point.Timestamp);

                        if (cancelationToken.IsCancellationRequested)
                            return result;

                        if (!float.IsNaN(point.ValueAsSingle))
                        {
                            if (frameRateResult.TryGetValue(point.PointID, out CondensedDataPoint dataPoint) && dataPoint != null)
                            {
                                if (point.ValueAsSingle > dataPoint.Max)
                                    dataPoint.Max = point.ValueAsSingle;

                                if (point.ValueAsSingle < dataPoint.Min)
                                    dataPoint.Min = point.ValueAsSingle;

                                dataPoint.Sum += point.ValueAsSingle;
                                dataPoint.SqrSum += point.ValueAsSingle * point.ValueAsSingle;
                                dataPoint.TotalPoints++;

                                if (point.ValueAsSingle > threshold)
                                    dataPoint.Alert++;
                            }
                        }

                        currentTimeStamp = point.Timestamp;
                    }
                }

                result.AddRange(frameRateResult.Where(item => item.Value.TotalPoints > 0).Select(item => { item.Value.PointID = item.Key; return item.Value; }));

                if (cancelationToken.IsCancellationRequested)
                    return result;
            }
            
            return result;
        }


        private HistorianServer GetServer(string instanceName)
        {
            if (LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter historianAdapter))
                return historianAdapter?.Server;

            return null;
        }

    }
}
