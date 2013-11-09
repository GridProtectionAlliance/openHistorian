//******************************************************************************************************
//  HistorianInputQueue.cs - Gbtc
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
//  1/5/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GSF;
using GSF.SortedTreeStore;
using GSF.SortedTreeStore.Net;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;
using openHistorian.Queues;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents an output adapter that publishes measurements to TVA Historian for archival.
    /// </summary>
    [Description("openHistorian 2.0 (Remote): forwards measurements to a remote openHistorian for archival.")]
    public class RemoteOutputAdapter : OutputAdapterBase
    {
        #region [ Members ]

        // Constants
        private const int DefaultHistorianPort = 1003;
        private const bool DefaultOutputIsForArchive = true;
        private const string DefaultServer = "localhost";
        private const string DefaultDatabaseName = "default";

        // Fields
        private string m_databaseName;
        private string m_server;
        private int m_port;
        private bool m_outputIsForArchive;
        private HistorianClient<HistorianKey, HistorianValue> m_client;
        private HistorianInputQueue m_inputQueue;
        private long m_measurementsPublished;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteOutputAdapter"/> class.
        /// </summary>
        public RemoteOutputAdapter()
            : base()
        {
            m_port = DefaultHistorianPort;
            m_outputIsForArchive = DefaultOutputIsForArchive;
            m_server = DefaultServer;
            m_databaseName = DefaultDatabaseName;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the default database on the server to use to write data.
        /// </summary>
        [ConnectionStringParameter,
         Description("Defines the default database on the server to use to write data."),
         DefaultValue("default")]
        public string DatabaseName
        {
            get
            {
                return m_databaseName;
            }
            set
            {
                m_databaseName = value;
            }
        }

        /// <summary>
        /// Gets or sets the host name for the server hosting the remote historian.
        /// </summary>
        [ConnectionStringParameter,
         Description("Define the host name of the remote historian."),
         DefaultValue("localhost")]
        public string Server
        {
            get
            {
                return m_server;
            }
            set
            {
                m_server = value;
            }
        }

        /// <summary>
        /// Gets or sets the port on which the remote historian is listening.
        /// </summary>
        [ConnectionStringParameter,
         Description("Define the port on which the remote historian is listening."),
         DefaultValue(1003)]
        public int Port
        {
            get
            {
                return m_port;
            }
            set
            {
                m_port = value;
            }
        }

        /// <summary>
        /// Returns a flag that determines if measurements sent to this <see cref="RemoteOutputAdapter"/> are destined for archival.
        /// </summary>
        [ConnectionStringParameter,
         Description("Define a value that determines whether the measurements are destined for archival."),
         DefaultValue(true)]
        public override bool OutputIsForArchive
        {
            get
            {
                return m_outputIsForArchive;
            }
        }

        /// <summary>
        /// Gets flag that determines if this <see cref="RemoteOutputAdapter"/> uses an asynchronous connection.
        /// </summary>
        protected override bool UseAsyncConnect
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the detailed status of the data output source.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.Append(base.Status);
                status.AppendLine();
                //status.Append(m_historianPublisher.Status);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes this <see cref="RemoteOutputAdapter"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><b>Server</b> is missing from the <see cref="AdapterBase.Settings"/>.</exception>
        public override void Initialize()
        {
            base.Initialize();

            string errorMessage = "{0} is missing from Settings - Example: server=localhost;port=1003;payloadAware=True;conserveBandwidth=True;outputIsForArchive=True;throttleTransmission=True;samplesPerTransmission=100000";
            Dictionary<string, string> settings = Settings;
            string setting;

            // Validate settings.
            if (!settings.TryGetValue("server", out m_server))
                throw new ArgumentException(string.Format(errorMessage, "server"));

            if (settings.TryGetValue("port", out setting))
                m_port = int.Parse(setting);
            else
                settings.Add("port", m_port.ToString());

            if (settings.TryGetValue("outputisforarchive", out setting))
                m_outputIsForArchive = setting.ParseBoolean();
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="RemoteOutputAdapter"/>.
        /// </summary>
        /// <param name="maxLength">Maximum length of the status message.</param>
        /// <returns>Text of the status message.</returns>
        public override string GetShortStatus(int maxLength)
        {
            if (m_outputIsForArchive)
                return string.Format("Published {0} measurements for archival.", m_measurementsPublished).CenterText(maxLength);

            return string.Format("Published {0} measurements for processing.", m_measurementsPublished).CenterText(maxLength);
        }

        /// <summary>
        /// Releases the unmanaged resources used by this <see cref="RemoteOutputAdapter"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (disposing)
                    {
                        if (m_client != null)
                            m_client.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true; // Prevent duplicate dispose.
                    base.Dispose(disposing); // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Attempts to connect to this <see cref="RemoteOutputAdapter"/>.
        /// </summary>
        protected override void AttemptConnection()
        {
            HistorianClientOptions clientOptions = new HistorianClientOptions();
            clientOptions.DefaultDatabase = DatabaseName;
            clientOptions.ServerNameOrIp = Server;
            clientOptions.NetworkPort = Port;
            clientOptions.IsReadOnly = false;
            m_client = new HistorianClient<HistorianKey, HistorianValue>(clientOptions);
            m_inputQueue = new HistorianInputQueue(() => m_client.GetDefaultDatabase());
        }

        /// <summary>
        /// Attempts to disconnect from this <see cref="RemoteOutputAdapter"/>.
        /// </summary>
        protected override void AttemptDisconnection()
        {
            if (m_client != null)
                m_client.Dispose();
        }

        /// <summary>
        /// Publishes <paramref name="measurements"/> for archival.
        /// </summary>
        /// <param name="measurements">Measurements to be archived.</param>
        /// <exception cref="OperationCanceledException">Acknowledgement is not received from historian for published data.</exception>
        protected override void ProcessMeasurements(IMeasurement[] measurements)
        {
            m_inputQueue.Enqueue(new StreamPoints(measurements));
            m_measurementsPublished += measurements.Length;
        }

        private class StreamPoints
            : TreeStream<HistorianKey, HistorianValue>
        {
            private int m_index;
            private readonly IMeasurement[] m_measurements;

            public StreamPoints(IMeasurement[] measurements)
            {
                m_index = 0;
                m_measurements = measurements;
            }

            public bool Read(out ulong timestamp, out ulong pointId, out ulong quality, out ulong value)
            {
                if (m_index < m_measurements.Length)
                {
                    IMeasurement measurement = m_measurements[m_index];
                    timestamp = (ulong)(long)measurement.Timestamp;
                    pointId = measurement.Key.ID;
                    quality = (ulong)measurement.StateFlags;
                    value = BitMath.ConvertToUInt64((float)measurement.AdjustedValue);
                    m_index++;
                    return true;
                }

                timestamp = 0;
                pointId = 0;
                quality = 0;
                value = 0;
                return false;
            }

            public override bool Read()
            {
                if (m_index < m_measurements.Length)
                {
                    IMeasurement measurement = m_measurements[m_index];
                    CurrentKey.Timestamp = (ulong)(long)measurement.Timestamp;
                    CurrentKey.PointID = measurement.Key.ID;

                    CurrentValue.Value1 = BitMath.ConvertToUInt64((float)measurement.AdjustedValue);

                    CurrentValue.Value3 = (ulong)measurement.StateFlags;

                    m_index++;
                    return true;
                }
                CurrentKey.Clear();
                CurrentValue.Clear();
                return false;
            }

            public override void Cancel()
            {
                m_index = m_measurements.Length;
            }
        }

        #endregion
    }
}