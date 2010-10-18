//*******************************************************************************************************
//  InputAdapter.cs
//  Copyright © 2009 - TVA, all rights reserved - Gbtc
//
//  Build Environment: C#, Visual Studio 2008
//  Primary Developer: James R. Carroll
//      Office: PSO PCS, CHATTANOOGA - MR BK-C
//       Phone: 423/751-4165
//       Email: jrcarrol@tva.gov
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  06/01/2006 - James R. Carroll
//       Generated original version of source code.
//
//*******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using TVA.Communication;
using TVA.Historian.Packets;
using TVA.Measurements;
using TVA.Measurements.Routing;

namespace TVA.Historian
{
    /// <summary>
    /// Represents the class for parsing incoming measurements from a TVA historian data stream.
    /// </summary>
    [CLSCompliant(false)]
    public class InputAdapter : InputAdapterBase
	{
        #region [ Members ]

        // Fields
        private string m_historianID;
        private int m_archiverPort;
        private UdpClient m_client;
        private PacketParser m_parser;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="InputAdapter"/>.
        /// </summary>
        public InputAdapter()
        {
            m_archiverPort = 2001;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets flag that determines if the data input connects asynchronously.
        /// </summary>
        protected override bool UseAsyncConnect
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the name of this <see cref="InputAdapter"/>.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Archive Listener " + m_historianID + ":" + m_archiverPort;
            }
        }

        /// <summary>
        /// Returns the detailed status of the data input source.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.Append("    Historian parser state: ");

                if (m_parser == null)
                    status.Append("Inactive - not parsing");
                else
                    status.Append("Active");

                status.AppendLine();

                if (m_parser != null)
                    status.Append(m_parser.Status);

                if (m_client != null)
                    status.Append(m_client.Status);

                status.Append(base.Status);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="InputAdapter"/> object and optionally releases the managed resources.
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
                        if (m_client != null)
                        {
                            m_client.ConnectionEstablished -= m_client_ConnectionEstablished;
                            m_client.ConnectionTerminated -= m_client_ConnectionTerminated;
                            m_client.ConnectionException -= m_client_ConnectionException;
                            m_client.Dispose();
                        }
                        m_client = null;

                        if (m_parser != null)
                        {
                            m_parser.DataParsed -= m_parser_DataParsed;
                            m_parser.Dispose();
                        }
                        m_parser = null;
                    }
                }
                finally
                {
                    base.Dispose(disposing);    // Call base class Dispose().
                    m_disposed = true;          // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Intializes <see cref="InputAdapter"/>.
        /// </summary>
        public override void Initialize()
        {
            Dictionary<string, string> settings = Settings;
            string value;

            // Example connection string:
            // Port=1003; ServerID=P3
            if (settings.TryGetValue("port", out value))
                m_archiverPort = int.Parse(value);

            if (settings.TryGetValue("historianid", out value))
                m_historianID = value.Trim().ToUpper();

            // Create new data parser
            m_parser = new PacketParser();
            m_parser.DataParsed += m_parser_DataParsed;

            // Create UDP client to listen for messages
            m_client = new UdpClient("localport=" + m_archiverPort);
            m_client.ConnectionEstablished += m_client_ConnectionEstablished;
            m_client.ConnectionTerminated += m_client_ConnectionTerminated;
            m_client.ConnectionException += m_client_ConnectionException;
            m_client.Handshake = false;

            // Send data received over UDP port directly to packet parser
            m_client.ReceiveDataHandler = (buffer, offset, count) => m_parser.Parse(Guid.Empty, buffer, offset, count);
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="AdapterBase"/>.
        /// </summary>
        /// <param name="maxLength">Maximum number of available characters for display.</param>
        /// <returns>A short one-line summary of the current status of this <see cref="AdapterBase"/>.</returns>
        public override string GetShortStatus(int maxLength)
        {
            return ("Total received measurements " + ReceivedMeasurements.ToString("N0")).PadLeft(maxLength, '\xA0');
        }

        /// <summary>
        /// Attempts to connect to data input source.
        /// </summary>
        protected override void AttemptConnection()
        {
            // Starts historian packet parser
            m_parser.Start();

            // Starts listening for UDP data
            m_client.ConnectAsync();
        }

        /// <summary>
        /// Attempts to disconnect from data input source.
        /// </summary>
        protected override void AttemptDisconnection()
        {
            // Stops listening for UDP data
            if (m_client != null)
                m_client.Disconnect();

            // Stops historian packet parser
            if (m_parser != null)
                m_parser.Stop();
        }

        // UDP client connection established handler
        private void m_client_ConnectionEstablished(object sender, EventArgs e)
        {
            OnConnected();
        }

        // UDP client connection terminated handler
        private void m_client_ConnectionTerminated(object sender, EventArgs e)
        {
            OnDisconnected();
        }

        // UDP client connection exception handler
        private void m_client_ConnectionException(object sender, EventArgs<Exception> e)
        {
            OnProcessException(e.Argument);
        }

        // Historian packet parser data parsed handler
        private void m_parser_DataParsed(object sender, EventArgs<Guid, IList<IPacket>> e)
        {
            List<IMeasurement> measurements = new List<IMeasurement>();

            // We have new historian packets, convert all PacketType1's to measurements
            foreach (PacketType1 packet in e.Argument2)
            {
                if (packet != null)
                    measurements.Add(new Measurement((uint)packet.HistorianID, m_historianID, packet.Value, packet.Time));
            }

            // Publish new measurements to consumer...
            if (measurements.Count > 0)
                this.OnNewMeasurements(measurements);
        }

        #endregion
    }	
}
