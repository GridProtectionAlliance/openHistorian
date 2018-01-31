//******************************************************************************************************
//  LocalInputAdapter.cs - Gbtc
//
//  Copyright © 2015, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/23/2015 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using GSF;
using GSF.Collections;
using GSF.Diagnostics;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using Timer = System.Timers.Timer;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents an output adapter that publishes measurements to openHistorian for archival.
    /// </summary>
    [Description("openHistorian 2.0 (Local): reads data from a local openHistorian for replay.")]
    public class LocalInputAdapter : InputAdapterBase
    {
        #region [ Members ]

        // Constants
        private const long DefaultPublicationInterval = Ticks.PerSecond;

        // Fields
        private Timer m_readTimer;
        private string m_historianServer;
        private string m_instanceName;
        private Connection m_archiveReader;
        private IEnumerator<IMeasurement> m_dataReader;
        private long m_publicationTime;
        private int[] m_historianIDs;
        private DateTime m_startTime;
        private DateTime m_stopTime;
        private Ticks m_currentTimeOffset;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="LocalInputAdapter"/>.
        /// </summary>
        public LocalInputAdapter()
        {
            m_historianServer = "127.0.0.1";

            // Setup a read timer
            m_readTimer = new Timer();
            m_readTimer.Elapsed += m_readTimer_Elapsed;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets instance name defined for this <see cref="LocalInputAdapter"/>.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the instance name the archive to read. Leave this value blank to default to the adapter name."),
        DefaultValue("")]
        public string InstanceName
        {
            get
            {
                if (string.IsNullOrEmpty(m_instanceName))
                    return Name.ToLower();

                return m_instanceName;
            }
            set
            {
                m_instanceName = value;
            }
        }

        /// <summary>
        /// Gets or sets archive path for this <see cref="LocalInputAdapter"/>.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the historian server connection string, e.g., \"192.168.1.10\" or \"myhistorian:38402\"."),
        DefaultValue("127.0.0.1")]
        public string HistorianServer
        {
            get
            {
                return m_historianServer;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value), "The historianServer setting must be specified.");

                m_historianServer = value;
            }
        }

        /// <summary>
        /// Gets or sets the publication interval for this <see cref="LocalInputAdapter"/>.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the publication time interval in 100-nanosecond tick intervals for reading historical data."),
        DefaultValue(DefaultPublicationInterval)]
        public long PublicationInterval { get; set; }

        /// <summary>
        /// Gets the start time for reading data.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the start time for reading data into real-time session, or do not define to start reading from the beginning of the available data. Either StartTimeConstraint or StopTimeConstraint must be defined in order to start reading data into real-time session. Value should not be defined when using adapter for subscription based temporal session support."),
        DefaultValue("")]
        public new string StartTimeConstraint { get; set; }

        /// <summary>
        /// Gets the stop time for reading data.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the stop time for reading data into real-time session, or do not define to keep reading until the end of the available data. Either StartTimeConstraint or StopTimeConstraint must be defined in order to start reading data into real-time session. Value should not be defined when using adapter for subscription based temporal session support."),
        DefaultValue("")]
        public new string StopTimeConstraint { get; set; }

        /// <summary>
        /// Gets or sets a value that determines whether timestamps are
        /// simulated for the purposes of real-time concentration.
        /// </summary>
        [ConnectionStringParameter,
        Description("Indicate whether timestamps are simulated for real-time concentration."),
        DefaultValue(false)]
        public bool SimulateTimestamp { get; set; }

        /// <summary>
        /// Gets or sets value that determines if the input data should be replayed repeatedly.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define if the input data should be replayed repeatedly."),
        DefaultValue(false)]
        public bool AutoRepeat { get; set; }

        /// <summary>
        /// Gets the flag indicating if this adapter supports temporal processing.
        /// </summary>
        public override bool SupportsTemporalProcessing
        {
            get
            {
                // If the start/time constraints are defined in the connection string, it is expected that this adapter
                // will be used in real-time. For temporal sessions these properties will be defined via method call to
                // the SetTemporalConstraint function.
                Dictionary<string, string> settings = Settings;
                return !(settings.ContainsKey("startTimeConstraint") || settings.ContainsKey("stopTimeConstraint"));
            }
        }

        /// <summary>
        /// Gets or sets the desired processing interval, in milliseconds, for the adapter.
        /// </summary>
        /// <remarks>
        /// With the exception of the values of -1 and 0, this value specifies the desired processing interval for data, i.e.,
        /// basically a delay, or timer interval, over which to process data. A value of -1 means to use the default processing
        /// interval while a value of 0 means to process data as fast as possible.
        /// </remarks>
        public override int ProcessingInterval
        {
            get
            {
                return base.ProcessingInterval;
            }
            set
            {
                base.ProcessingInterval = value;

                // Set read timer interval to the requested processing interval
                m_readTimer.Interval = value <= 0 ? 1 : value;
            }
        }

        /// <summary>
        /// Gets flag that determines if this <see cref="LocalInputAdapter"/> uses an asynchronous connection.
        /// </summary>
        protected override bool UseAsyncConnect => false;

        /// <summary>
        /// Returns the detailed status of the data input source.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.Append(base.Status);

                status.AppendFormat("             Instance name: {0}\r\n", InstanceName);
                status.AppendFormat("          Historian server: {0}\r\n", HistorianServer);
                status.AppendFormat("      Publication interval: {0}\r\n", PublicationInterval);
                status.AppendFormat("               Auto-repeat: {0}\r\n", AutoRepeat);
                status.AppendFormat("            Start time-tag: {0:yyyy-MM-dd HH:mm:ss.fff}\r\n", StartTime);
                status.AppendFormat("             Stop time-tag: {0:yyyy-MM-dd HH:mm:ss.fff}\r\n", StopTime);
                status.AppendFormat("        Simulate timestamp: {0}\r\n", SimulateTimestamp);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="LocalInputAdapter"/> object and optionally releases the managed resources.
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
                        if ((object)m_readTimer != null)
                        {
                            m_readTimer.Elapsed -= m_readTimer_Elapsed;
                            m_readTimer.Dispose();
                            m_readTimer = null;
                        }

                        if ((object)m_archiveReader != null)
                        {
                            m_archiveReader.Dispose();
                            m_archiveReader = null;
                        }
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
        /// Initializes this <see cref="LocalInputAdapter"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><b>HistorianID</b>, <b>Server</b>, <b>Port</b>, <b>Protocol</b>, or <b>InitiateConnection</b> is missing from the <see cref="AdapterBase.Settings"/>.</exception>
        public override void Initialize()
        {
            const string errorMessage = "{0} is missing from settings - Example: instanceName=PPA; publicationInterval=333333";

            base.Initialize();

            Dictionary<string, string> settings = Settings;
            string setting;
            long value;

            // Validate settings.
            settings.TryGetValue("instanceName", out setting);
            InstanceName = setting;

            if (((object)OutputSourceIDs == null || OutputSourceIDs.Length == 0) && string.IsNullOrEmpty(InstanceName))
                throw new ArgumentException(string.Format(errorMessage, "instanceName"));

            if (settings.TryGetValue("historianServer", out setting))
                HistorianServer = setting;

            if (settings.TryGetValue("publicationInterval", out setting) && long.TryParse(setting, out value))
                PublicationInterval = value;
            else
                PublicationInterval = DefaultPublicationInterval;

            if (settings.TryGetValue("simulateTimestamp", out setting))
                SimulateTimestamp = setting.ParseBoolean();

            if (settings.TryGetValue("autoRepeat", out setting))
                AutoRepeat = setting.ParseBoolean();

            // Define output measurements this input adapter can support based on the instance name (if not already defined)
            OutputSourceIDs = new[] { InstanceName };
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="LocalInputAdapter"/>.
        /// </summary>
        /// <param name="maxLength">Maximum length of the status message.</param>
        /// <returns>Text of the status message.</returns>
        public override string GetShortStatus(int maxLength)
        {
            if (Enabled && m_publicationTime > 0)
                return $"Publishing data for {(Ticks)m_publicationTime:yyyy-MM-dd HH:mm:ss.fff}...".CenterText(maxLength);

            return "Not currently publishing data".CenterText(maxLength);
        }

        /// <summary>
        /// Attempts to connect to this <see cref="LocalInputAdapter"/>.
        /// </summary>
        protected override void AttemptConnection()
        {
            // This adapter is only engaged for history, so we don't process any data unless a temporal constraint is defined
            if (this.TemporalConstraintIsDefined())
            {
                // Turn off read timer if it's active
                m_readTimer.Enabled = false;

                if ((object)m_archiveReader != null)
                {
                    m_archiveReader.Dispose();
                    m_archiveReader = null;
                }

                // Attempt to connect to historian
                m_archiveReader = new Connection(HistorianServer, InstanceName);
                ThreadPool.QueueUserWorkItem(StartDataReader);
            }
        }

        /// <summary>
        /// Attempts to disconnect from this <see cref="LocalInputAdapter"/>.
        /// </summary>
        protected override void AttemptDisconnection()
        {
            if ((object)m_readTimer != null)
            {
                m_readTimer.Enabled = false;

                lock (m_readTimer)
                    m_dataReader = null;
            }

            if ((object)m_archiveReader != null)
            {
                m_archiveReader.Dispose();
                m_archiveReader = null;
            }
        }

        // Kick start read process for historian
        private void StartDataReader(object state)
        {
            MeasurementKey[] requestedKeys = SupportsTemporalProcessing ? RequestedOutputMeasurementKeys : OutputMeasurements.MeasurementKeys().ToArray();

            if (Enabled && (object)m_archiveReader != null && (object)requestedKeys != null && requestedKeys.Length > 0)
            {
                m_historianIDs = requestedKeys.Select(key => unchecked((int)key.ID)).ToArray();
                m_publicationTime = 0;

                // Start data read from historian
                lock (m_readTimer)
                {
                    m_startTime = base.StartTimeConstraint < DateTime.MinValue ? DateTime.MinValue : base.StartTimeConstraint > DateTime.MaxValue ? DateTime.MaxValue : base.StartTimeConstraint;
                    m_stopTime = base.StopTimeConstraint < DateTime.MinValue ? DateTime.MinValue : base.StopTimeConstraint > DateTime.MaxValue ? DateTime.MaxValue : base.StopTimeConstraint;

                    m_currentTimeOffset = 0;
                    m_dataReader = MeasurementAPI.GetHistorianData(m_archiveReader, m_startTime, m_stopTime, m_historianIDs.ToDelimitedString(',')).GetEnumerator();
                    m_readTimer.Enabled = m_dataReader.MoveNext();

                    if (m_readTimer.Enabled)
                    {
                        OnStatusMessage(MessageLevel.Info, "Starting historical data read...");
                    }
                    else
                    {
                        OnStatusMessage(MessageLevel.Warning, "No historical data was available to read for given time-frame.");
                        OnProcessingComplete();
                    }
                }
            }
            else
            {
                m_readTimer.Enabled = false;
                OnStatusMessage(MessageLevel.Warning, "No measurement keys have been requested for reading, historian reader is idle.");
                OnProcessingComplete();
            }
        }

        // Process next data read
        private void m_readTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<IMeasurement> measurements = new List<IMeasurement>();

            if (Monitor.TryEnter(m_readTimer))
            {
                try
                {
                    IMeasurement currentPoint = m_dataReader.Current ?? new Measurement();
                    long timestamp = currentPoint.Timestamp;

                    if (m_publicationTime == 0)
                        m_publicationTime = timestamp;

                    // Set next reasonable publication time
                    while (m_publicationTime < timestamp)
                        m_publicationTime += PublicationInterval;

                    do
                    {
                        // Convert historical timestamp to simulated real-time, if requested
                        if (SimulateTimestamp)
                        {
                            if (m_currentTimeOffset == 0)
                            {
                                Ticks currentTime = new Ticks(DateTime.UtcNow.Ticks).BaselinedTimestamp(BaselineTimeInterval.Second);
                                Ticks historicalTime = currentPoint.Timestamp.BaselinedTimestamp(BaselineTimeInterval.Second);
                                m_currentTimeOffset = currentTime - historicalTime;
                            }

                            currentPoint.Timestamp += m_currentTimeOffset;
                        }

                        // Add current measurement to the collection for publication
                        measurements.Add(currentPoint);

                        // Attempt to move to next record
                        if (m_dataReader.MoveNext())
                        {
                            // Read record value
                            currentPoint = m_dataReader.Current ?? new Measurement();
                            timestamp = currentPoint.Timestamp;
                        }
                        else
                        {
                            if (timestamp < m_stopTime.Ticks && m_startTime.Ticks < timestamp)
                            {
                                // Could be attempting read with a future end time - in these cases attempt to re-read current data
                                // from now to end time in case any new data as been archived in the mean-time
                                m_startTime = new DateTime(timestamp + Ticks.PerMillisecond);
                                m_dataReader = MeasurementAPI.GetHistorianData(m_archiveReader, m_startTime, m_stopTime, m_historianIDs.ToDelimitedString(',')).GetEnumerator();

                                if (!m_dataReader.MoveNext())
                                {
                                    // Finished reading all available data
                                    m_readTimer.Enabled = false;

                                    if (AutoRepeat)
                                        ThreadPool.QueueUserWorkItem(StartDataReader);
                                    else
                                        OnProcessingComplete();
                                }
                            }
                            else
                            {
                                // Finished reading all available data
                                m_readTimer.Enabled = false;

                                if (AutoRepeat)
                                    ThreadPool.QueueUserWorkItem(StartDataReader);
                                else
                                    OnProcessingComplete();
                            }

                            break;
                        }
                    }
                    while (timestamp <= m_publicationTime);
                }
                catch (InvalidOperationException)
                {
                    // Pooled timer thread executed after last read, verify timer has stopped
                    m_readTimer.Enabled = false;
                }
                finally
                {
                    Monitor.Exit(m_readTimer);
                }
            }

            // Publish all measurements for this time interval
            if (measurements.Count > 0)
                OnNewMeasurements(measurements);
        }

        #endregion
    }
}
