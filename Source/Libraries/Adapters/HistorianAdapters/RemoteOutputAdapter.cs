//******************************************************************************************************
//  RemoteOutputAdapter.cs - Gbtc
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
//  06/01/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/22/2009 - Pinal C. Patel
//       Re-wrote the adapter to utilize new components.
//  09/23/2009 - Pinal C. Patel
//       Fixed the handling of socket disconnect.
//  03/04/2010 - Pinal C. Patel
//       Added outputIsForArchive and throttleTransmission setting parameters for more control over 
//       the adapter.
//       Switched to ManualResetEvent for waiting on historian acknowledgement for efficiency.
//  11/03/2010 - Mihir Brahmbhatt
//       Updated openHistorian Reference
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TimeSeriesFramework;
using TimeSeriesFramework.Adapters;
using TVA;
using TVA.Communication;
using TimeSeriesArchiver.Packets;

namespace HistorianAdapters
{
    /// <summary>
    /// Represents an output adapter that publishes measurements to TVA Historian for archival.
    /// </summary>
    public class RemoteOutputAdapter : OutputAdapterBase
    {
        #region [ Members ]

        // Constants
        private const int DefaultHistorianPort = 1003;
        private const bool DefaultPayloadAware = true;
        private const bool DefaultConserveBandwidth = true;
        private const bool DefaultOutputIsForArchive = true;
        private const bool DefaultThrottleTransmission = true;
        private const int DefaultSamplesPerTransmission = 100000;
        private const int PubliserWaitTime = 5000;

        // Fields
        private bool m_outputIsForArchive;
        private bool m_throttleTransmission;
        private int m_samplesPerTransmission;
        private TcpClient m_historianPublisher;
        private byte[] m_publisherBuffer;
        private ManualResetEvent m_publisherWaitHandle;
        private Action<IMeasurement[], int, int> m_publisherDelegate;
        private bool m_publisherDisconnecting;
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
            m_historianPublisher = new TcpClient();
            m_publisherWaitHandle = new ManualResetEvent(false);
        }

        #endregion

        #region [ Properties ]

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
                status.Append(m_historianPublisher.Status);

                return status.ToString();
            }
        }

        /// <summary>
        /// Returns a flag that determines if measurements sent to this <see cref="RemoteOutputAdapter"/> are destined for archival.
        /// </summary>
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
                return true;
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

            string server;
            string port;
            string payloadAware;
            string conserveBandwidth;
            string outputIsForArchive;
            string throttleTransmission;
            string samplesPerTransmission;
            string errorMessage = "{0} is missing from Settings - Example: server=localhost;port=1003;payloadAware=True;conserveBandwidth=True;outputIsForArchive=True;throttleTransmission=True;samplesPerTransmission=100000";
            Dictionary<string, string> settings = Settings;

            // Validate settings.
            if (!settings.TryGetValue("server", out server))
                throw new ArgumentException(string.Format(errorMessage, "server"));

            if (!settings.TryGetValue("port", out port))
                port = DefaultHistorianPort.ToString();

            if (!settings.TryGetValue("payloadaware", out payloadAware))
                payloadAware = DefaultPayloadAware.ToString();

            if (!settings.TryGetValue("conservebandwidth", out conserveBandwidth))
                conserveBandwidth = DefaultConserveBandwidth.ToString();

            if (!settings.TryGetValue("outputisforarchive", out outputIsForArchive))
                outputIsForArchive = DefaultOutputIsForArchive.ToString();

            if (!settings.TryGetValue("throttletransmission", out throttleTransmission))
                throttleTransmission = DefaultThrottleTransmission.ToString();

            if (!settings.TryGetValue("samplespertransmission", out samplesPerTransmission))
                samplesPerTransmission = DefaultSamplesPerTransmission.ToString();

            // Initialize member variables.
            m_outputIsForArchive = outputIsForArchive.ParseBoolean();
            m_throttleTransmission = throttleTransmission.ParseBoolean();
            m_samplesPerTransmission = int.Parse(samplesPerTransmission);

            // Initialize publisher delegates.
            if (conserveBandwidth.ParseBoolean())
            {
                m_publisherDelegate = TransmitPacketType101;
            }
            else
            {
                m_publisherDelegate = TransmitPacketType1;
                m_publisherBuffer = new byte[m_samplesPerTransmission * PacketType1.ByteCount];
            }
            
            // Initialize publiser socket.
            m_historianPublisher.ConnectionString = string.Format("Server={0}:{1}", server, port);
            m_historianPublisher.PayloadAware = payloadAware.ParseBoolean();
            m_historianPublisher.ConnectionAttempt += HistorianPublisher_ConnectionAttempt;
            m_historianPublisher.ConnectionEstablished += HistorianPublisher_ConnectionEstablished;
            m_historianPublisher.ConnectionTerminated += HistorianPublisher_ConnectionTerminated;
            m_historianPublisher.SendDataException += HistorianPublisher_SendDataException;
            m_historianPublisher.ReceiveDataComplete += HistorianPublisher_ReceiveDataComplete;
            m_historianPublisher.ReceiveDataException += HistorianPublisher_ReceiveDataException;
            m_historianPublisher.Initialize();
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
            else
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
                        // This will be done only when the object is disposed by calling Dispose().
                        if (m_historianPublisher != null)
                        {
                            m_historianPublisher.ConnectionAttempt -= HistorianPublisher_ConnectionAttempt;
                            m_historianPublisher.ConnectionEstablished -= HistorianPublisher_ConnectionEstablished;
                            m_historianPublisher.ConnectionTerminated -= HistorianPublisher_ConnectionTerminated;
                            m_historianPublisher.SendDataException -= HistorianPublisher_SendDataException;
                            m_historianPublisher.ReceiveDataComplete -= HistorianPublisher_ReceiveDataComplete;
                            m_historianPublisher.ReceiveDataException -= HistorianPublisher_ReceiveDataException;
                            m_historianPublisher.Dispose();
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
        /// Attempts to connect to this <see cref="RemoteOutputAdapter"/>.
        /// </summary>
        protected override void AttemptConnection()
        {           
            m_publisherDisconnecting = false;
            m_historianPublisher.ConnectAsync();
        }

        /// <summary>
        /// Attempts to disconnect from this <see cref="RemoteOutputAdapter"/>.
        /// </summary>
        protected override void AttemptDisconnection()
        {
            m_publisherDisconnecting = true;
            m_historianPublisher.Disconnect();
        }

        /// <summary>
        /// Publishes <paramref name="measurements"/> for archival.
        /// </summary>
        /// <param name="measurements">Measurements to be archived.</param>
        /// <exception cref="OperationCanceledException">Acknowledgement is not received from historian for published data.</exception>
        protected override void ProcessMeasurements(IMeasurement[] measurements)
        {
            if (m_historianPublisher.CurrentState != ClientState.Connected)
                throw new InvalidOperationException("Historian publisher socket is not connected");

            try
            {
                for (int i = 0; i < measurements.Length; i += m_samplesPerTransmission)
                {
                    // Wait for historian acknowledgement.
                    if (m_throttleTransmission)
                    {
                        if (!m_publisherWaitHandle.WaitOne(PubliserWaitTime))
                            throw new OperationCanceledException("Timeout waiting for acknowledgement from historian");
                    }

                    // Publish measurements to historian.
                    m_publisherWaitHandle.Reset();
                    m_publisherDelegate(measurements, i, (measurements.Length - i < m_samplesPerTransmission ? measurements.Length : i + m_samplesPerTransmission) - 1);
                }
                m_measurementsPublished += measurements.Length;
            }
            catch
            {
                m_publisherWaitHandle.Set();
                throw;
            }
        }

        private void HistorianPublisher_ConnectionAttempt(object sender, EventArgs e)
        {
            OnStatusMessage("Attempting socket connection...");
        }

        private void HistorianPublisher_ConnectionEstablished(object sender, EventArgs e)
        {
            OnConnected();
            m_publisherWaitHandle.Set();
        }

        private void HistorianPublisher_ConnectionTerminated(object sender, EventArgs e)
        {
            m_measurementsPublished = 0;
            m_publisherWaitHandle.Reset();

            if (!m_publisherDisconnecting)
                Start();
        }

        private void HistorianPublisher_SendDataException(object sender, EventArgs<Exception> e)
        {
            m_publisherWaitHandle.Set();
            OnProcessException(e.Argument);
        }

        private void HistorianPublisher_ReceiveDataComplete(object sender, EventArgs<byte[], int> e)
        {
            // Check for acknowledgement from historian.
            string reply = Encoding.ASCII.GetString(e.Argument1, 0, e.Argument2);
            if (reply == "ACK")
                m_publisherWaitHandle.Set();
        }

        private void HistorianPublisher_ReceiveDataException(object sender, EventArgs<Exception> e)
        {
            m_publisherWaitHandle.Set();
            OnProcessException(e.Argument);
        }

        private void TransmitPacketType1(IMeasurement[] measurements, int startIndex, int endIndex)
        {
            int bufferIndex = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                Buffer.BlockCopy(new PacketType1(measurements[i]).BinaryImage, 0, m_publisherBuffer, bufferIndex, PacketType1.ByteCount);
                bufferIndex += PacketType1.ByteCount;
            }
            m_historianPublisher.SendAsync(m_publisherBuffer, 0, bufferIndex);
        }

        private void TransmitPacketType101(IMeasurement[] measurements, int startIndex, int endIndex)
        {
            PacketType101 packet = new PacketType101();
            for (int i = startIndex; i <= endIndex; i++)
            {
                packet.Data.Add(new PacketType101DataPoint(measurements[i]));
            }

            m_historianPublisher.SendAsync(packet.BinaryImage);
        }

        #endregion
    }
}
