//******************************************************************************************************
//  InputAdapter.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  06/01/2006 - J. Ritchie Carroll
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/17/2009 - Pinal C. Patel
//       Re-wrote the adapter to utilize existing historian components.
//  11/18/2009 - Pinal C. Patel
//       Removed the need for HistorianID in Settings by using adapter Name instead.
//  03/05/2010 - Pinal C. Patel
//       Added status updates for when the parser is having trouble parsing data.
//  11/03/2010 - Mihir Brahmbhatt
//       Updated openHistorian Reference
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using TimeSeriesFramework;
using TimeSeriesFramework.Adapters;
using TVA;
using TVA.Communication;
using openHistorian;

namespace HistorianAdapters
{
    /// <summary>
    /// Represents an input adapters that listens for time-series data from TVA Historian.
    /// </summary>
    public class InputAdapter : InputAdapterBase
    {
        #region [ Members ]

        // Fields
        private DataListener m_historianDataListener;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="InputAdapter"/> class.
        /// </summary>
        public InputAdapter()
            : base()
        {
            m_historianDataListener = new DataListener();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Returns the detailed status of the data input source.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.Append(base.Status);
                status.AppendLine();
                status.Append(m_historianDataListener.Status);

                return status.ToString();
            }
        }

        /// <summary>
        /// Gets flag that determines if this <see cref="InputAdapter"/> uses an asynchronous connection.
        /// </summary>
        protected override bool UseAsyncConnect
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes this <see cref="InputAdapter"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><b>HistorianID</b>, <b>Server</b>, <b>Port</b>, <b>Protocol</b>, or <b>InitiateConnection</b> is missing from the <see cref="AdapterBase.Settings"/>.</exception>
        public override void Initialize()
        {
            base.Initialize();

            string server;
            string port;
            string protocol;
            string outbound;
            string message = "{0} is missing from Settings - Example: protocol=UDP;server=openpdc;port=2004;initiateConnection=True";
            Dictionary<string, string> settings = Settings;

            // Validate settings.
            if (!settings.TryGetValue("server", out server))
                throw new ArgumentException(string.Format(message, "server"));

            if (!settings.TryGetValue("port", out port))
                throw new ArgumentException(string.Format(message, "port"));

            if (!settings.TryGetValue("protocol", out protocol))
                throw new ArgumentException(string.Format(message, "protocol"));

            if (!settings.TryGetValue("initiateconnection", out outbound))
                throw new ArgumentException(string.Format(message, "initiateConnection"));

            m_historianDataListener.ID = Name;
            m_historianDataListener.InitializeData = false;
            m_historianDataListener.CacheData = false;
            m_historianDataListener.Server = server;
            m_historianDataListener.Port = int.Parse(port);
            m_historianDataListener.Protocol = (TransportProtocol)Enum.Parse(typeof(TransportProtocol), protocol, true);
            m_historianDataListener.ConnectToServer = outbound.ParseBoolean();
            m_historianDataListener.DataExtracted += HistorianDataListener_DataExtracted;
            m_historianDataListener.SocketConnecting += HistorianDataListener_SocketConnecting;
            m_historianDataListener.SocketConnected += HistorianDataListener_SocketConnected;
            m_historianDataListener.SocketDisconnected += HistorianDataListener_SocketDisconnected;
            m_historianDataListener.Parser.OutputTypeNotFound += HistorianDataListener_OutputTypeNotFound;
            m_historianDataListener.Parser.DataDiscarded += HistorianDataListener_DataDiscarded;
            m_historianDataListener.Parser.ParsingException += HistorianDataListener_ParsingException;
            m_historianDataListener.Initialize();
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="InputAdapter"/>.
        /// </summary>
        /// <param name="maxLength">Maximum length of the status message.</param>
        /// <returns>Text of the status message.</returns>
        public override string GetShortStatus(int maxLength)
        {
            return string.Format("Received {0} bytes in {1} packets.", m_historianDataListener.TotalBytesReceived, m_historianDataListener.TotalPacketsReceived).CenterText(maxLength);
        }

        /// <summary>
        /// Releases the unmanaged resources used by this <see cref="InputAdapter"/> and optionally releases the managed resources.
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
                        // This will be done only when the object is disposed by calling Dispose().
                        if (m_historianDataListener != null)
                        {
                            m_historianDataListener.DataExtracted -= HistorianDataListener_DataExtracted;
                            m_historianDataListener.SocketConnecting -= HistorianDataListener_SocketConnecting;
                            m_historianDataListener.SocketConnected -= HistorianDataListener_SocketConnected;
                            m_historianDataListener.SocketDisconnected -= HistorianDataListener_SocketDisconnected;
                            m_historianDataListener.Parser.OutputTypeNotFound -= HistorianDataListener_OutputTypeNotFound;
                            m_historianDataListener.Parser.DataDiscarded -= HistorianDataListener_DataDiscarded;
                            m_historianDataListener.Parser.ParsingException -= HistorianDataListener_ParsingException;
                            m_historianDataListener.Dispose();
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
        /// Attempts to connect to this <see cref="InputAdapter"/>.
        /// </summary>
        protected override void AttemptConnection()
        {
            m_historianDataListener.StartAsync();
        }

        /// <summary>
        /// Attempts to disconnect from this <see cref="InputAdapter"/>.
        /// </summary>
        protected override void AttemptDisconnection()
        {
            m_historianDataListener.Stop();
        }

        private void HistorianDataListener_DataExtracted(object sender, EventArgs<IList<IDataPoint>> e)
        {
            try
            {
                List<IMeasurement> measurements = new List<IMeasurement>();
                foreach (IDataPoint dataPoint in e.Argument)
                {
                    measurements.Add(new Measurement((uint)dataPoint.HistorianID, m_historianDataListener.ID, dataPoint.Value, dataPoint.Time));
                }
                OnNewMeasurements(measurements);
            }
            catch (Exception ex)
            {
                OnProcessException(ex);
            }
        }

        private void HistorianDataListener_SocketConnecting(object sender, EventArgs e)
        {
            OnStatusMessage("Attempting socket connection...");
        }

        private void HistorianDataListener_SocketConnected(object sender, EventArgs e)
        {
            OnConnected();
        }

        private void HistorianDataListener_SocketDisconnected(object sender, EventArgs e)
        {
            OnDisconnected();
        }

        private void HistorianDataListener_OutputTypeNotFound(object sender, EventArgs<short> e)
        {
            OnStatusMessage(string.Format("Unable to parse data for packet type {0}.", e.Argument));
        }

        private void HistorianDataListener_DataDiscarded(object sender, EventArgs<byte[]> e)
        {
            OnStatusMessage(string.Format("Unable to parse data: {0} bytes discarded.", e.Argument.Length));
        }

        private void HistorianDataListener_ParsingException(object sender, EventArgs<Exception> e)
        {
            OnProcessException(e.Argument);
        }

        #endregion
    }
}
