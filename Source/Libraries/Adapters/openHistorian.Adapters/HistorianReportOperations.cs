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
using GSF;
using GSF.Data;
using GSF.Data.Model;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Web.Hubs;
using GSF.Web.Model;
using Microsoft.AspNet.SignalR.Hubs;
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
    ///  Includes Sums; number of points; min; max;
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
        public double min;

        /// <summary>
        /// Maximum.
        /// </summary>
        public double max;

        /// <summary>
        /// Total Number of Points.
        /// </summary>
        public int totalPoints;

        /// <summary>
        /// Sum (X).
        /// </summary>
        public double sum;

        /// <summary>
        /// Sum (X*X).
        /// </summary>
        public double sqrsum;

        /// <summary>
        /// nu,ber of points exceeding alert threshold.
        /// </summary>
        public int alert;

        /// <summary>
        /// Intializes a new condesed data point.
        /// </summary>
        /// <param name="PointID">PointID.</param>
        public static CondensedDataPoint EmptyPoint(ulong PointID)
        {
            CondensedDataPoint point = new CondensedDataPoint();
            point.max = double.MinValue;
            point.min = double.MaxValue;
            point.sqrsum = 0;
            point.sum = 0;
            point.totalPoints = 0;
            point.PointID = PointID;
            point.alert = 0;
            return point;
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
        private readonly MatchFilterBase<HistorianKey, HistorianValue> m_pointFilter;
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
            m_pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(pointIDs);

            m_stream = m_database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, m_pointFilter);
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
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if ((object)m_stream != null)
                            m_stream.Dispose();

                        if ((object)m_database != null)
                            m_database.Dispose();

                        if ((object)m_client != null)
                            m_client.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Read DataPoint and advance to the next point.
        /// </summary>
        /// <param name="point"> Datapoint object to be updated.</param>
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
        /// <param name="point"> Datapoint object to be updated.</param>
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
    /// Defines a Client to read distorian data using <see cref="ReportHistorianReader"/>.
    /// </summary>
    public sealed class ReportHistorianOperations : IDisposable
    {
        private String m_instance;
        private string s_defaultInstanceName;
        private int s_portNumber;
        private bool m_disposed;

        /// <summary>
        /// Gets configured instance name for historian.
        /// </summary>
        public string DefaultInstanceName => s_defaultInstanceName;

        /// <summary>
        /// Gets configured port number for historian.
        /// </summary>
        public int PortNumber => s_portNumber;

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

                    if ((object)record == null)
                        throw new NullReferenceException("Primary openHistorian adapter instance not found.");

                    Dictionary<string, string> settings = record.ConnectionString.ParseKeyValuePairs();
                    string setting;

                    if (!settings.TryGetValue("port", out setting) || !int.TryParse(setting, out s_portNumber))
                        s_portNumber = Connection.DefaultHistorianPort;

                    if (!settings.TryGetValue("instanceName", out s_defaultInstanceName) || string.IsNullOrWhiteSpace(s_defaultInstanceName))
                        s_defaultInstanceName = record.AdapterName ?? "PPA";
                }
            }
            catch
            {
                s_defaultInstanceName = "PPA";
                s_portNumber = Connection.DefaultHistorianPort;
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
        /// Destructor for <see cref="ReportHistorianOperations"/>.
        /// </summary>
        ~ReportHistorianOperations()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ReportHistorianOperations"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ReportHistorianOperations"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Reads Data From the Historian and returns condensed DataPoints.
        /// </summary>
        /// <param name="start">Starttime.</param>
        /// <param name="end">EndTime.</param>
        /// <param name="measurements">Measurements to be read.</param>
        /// <param name="threshold">Threshhold used to determine number of alerts.</param>   
        /// <param name="cancelationToken">Cancleation Token for the operation.</param>  
        public IEnumerable<CondensedDataPoint> ReadCondensed(DateTime start, DateTime end, IEnumerable<ActiveMeasurement> measurements, double threshold, System.Threading.CancellationToken cancelationToken)
        {
            List<CondensedDataPoint> result = new List<CondensedDataPoint>(measurements.Count());

            SnapServer server = GetServer(this.m_instance)?.Host;
            
            //start by sepperating all framerates
            foreach (int framerate in measurements.Select(item => item.FramesPerSecond).Distinct())
            {
                ulong[] pointIDs= measurements.Where(item => item.FramesPerSecond == framerate).Select(item => item.PointID).ToArray();
                Dictionary<ulong, CondensedDataPoint> frameRateResult = new Dictionary<ulong, CondensedDataPoint>();

                foreach (ulong key in pointIDs)
                {
                    frameRateResult.Add(key,CondensedDataPoint.EmptyPoint(key));
                    if (cancelationToken.IsCancellationRequested)
                    {
                        return result;
                    }
                }

                using (ReportHistorianReader reader = new ReportHistorianReader(server, this.m_instance, start, end, framerate, pointIDs))
                {
                    DataPoint point = new DataPoint();

                    // Scan to first record
                    if (!reader.ReadNext(point))
                        throw new InvalidOperationException("No data for specified time range in openHistorian connection!");

                    while (reader.ReadNext(point))
                    {
                        if (cancelationToken.IsCancellationRequested)
                        {
                            return result;
                        }

                        if (!Single.IsNaN(point.ValueAsSingle))
                        {
                            if (point.ValueAsSingle > frameRateResult[point.PointID].max)
                                frameRateResult[point.PointID].max = point.ValueAsSingle;
                            if (point.ValueAsSingle < frameRateResult[point.PointID].min)
                                frameRateResult[point.PointID].min = point.ValueAsSingle;

                            frameRateResult[point.PointID].sum += point.ValueAsSingle;
                            frameRateResult[point.PointID].sqrsum += point.ValueAsSingle * point.ValueAsSingle;
                            frameRateResult[point.PointID].totalPoints++;
                            if (point.ValueAsSingle > threshold)
                            {
                                frameRateResult[point.PointID].alert++;
                            }
                        }
                    }
                }

                result.AddRange(frameRateResult.Where(item => item.Value.totalPoints > 0).Select(item => { item.Value.PointID = item.Key; return item.Value; }));

                if (cancelationToken.IsCancellationRequested)
                {
                    return result;
                }
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
