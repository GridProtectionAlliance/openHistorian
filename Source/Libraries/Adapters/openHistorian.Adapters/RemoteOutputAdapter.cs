//******************************************************************************************************
//  HistorianInputQueue.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  01/05/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//  01/12/2015 - J. Ritchie Carroll
//       Updated adapter properties and initialization.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GSF;
using GSF.Snap.Services.Net;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using openHistorian.Net;
using openHistorian.Queues;
using openHistorian.Snap;

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
        private const string DefaultServer = "localhost";

        // Fields
        private string m_instanceName;
        private string m_server;
        private int m_port;
        private SnapNetworkClient m_client;
        private HistorianInputQueue m_inputQueue;
        private long m_measurementsPublished;
        private readonly HistorianKey m_key;
        private readonly HistorianValue m_value;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteOutputAdapter"/> class.
        /// </summary>
        public RemoteOutputAdapter()
        {
            m_port = LocalOutputAdapter.DefaultPort;
            m_server = DefaultServer;
            m_key = new HistorianKey();
            m_value = new HistorianValue();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets instance name defined for this <see cref="RemoteOutputAdapter"/>.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the instance name for the historian. Leave this value blank to default to the adapter name."),
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
        /// Gets or sets the host name for the server hosting the remote historian.
        /// </summary>
        [ConnectionStringParameter,
         Description("Define the host name of the remote historian."),
         DefaultValue(DefaultServer)]
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
         DefaultValue(LocalOutputAdapter.DefaultPort)]
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
        public override bool OutputIsForArchive
        {
            get
            {
                return true;
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
                status.AppendFormat("   Historian instance name: {0}\r\n", InstanceName);
                status.AppendFormat("         Remote connection: {0}:{1}\r\n", Server, Port);

                if ((object)m_inputQueue != null)
                    status.AppendFormat("        Current queue size: {0}\r\n", m_inputQueue.Size);

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

            const string errorMessage = "{0} is missing from Settings - Example: instanceName=PPA; server=localhost; port=38402";
            Dictionary<string, string> settings = Settings;
            string setting;

            // Validate settings.
            if (!settings.TryGetValue("instanceName", out m_instanceName))
                m_instanceName = null;

            if (!settings.TryGetValue("server", out m_server))
                throw new ArgumentException(string.Format(errorMessage, "server"));

            if (settings.TryGetValue("port", out setting))
                m_port = int.Parse(setting);
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="RemoteOutputAdapter"/>.
        /// </summary>
        /// <param name="maxLength">Maximum length of the status message.</param>
        /// <returns>Text of the status message.</returns>
        public override string GetShortStatus(int maxLength)
        {
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
            m_client = new HistorianClient(Server, Port);
            m_inputQueue = new HistorianInputQueue(() => m_client.GetDatabase<HistorianKey, HistorianValue>(InstanceName));
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
        protected override void ProcessMeasurements(IMeasurement[] measurements)
        {
            foreach (IMeasurement measurement in measurements)
            {
                m_key.Timestamp = (ulong)(long)measurement.Timestamp;
                m_key.PointID = measurement.Key.ID;

                // Since current time-series measurements are basically all floats - values fit into first value,
                // this will change as value types for time-series framework expands
                m_value.Value1 = BitConvert.ToUInt64((float)measurement.AdjustedValue);
                m_value.Value3 = (ulong)measurement.StateFlags;

                m_inputQueue.Enqueue(m_key, m_value);
            }

            m_measurementsPublished += measurements.Length;
        }

        #endregion
    }
}