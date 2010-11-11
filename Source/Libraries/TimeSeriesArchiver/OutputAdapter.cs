//*******************************************************************************************************
//  OutputAdapter.cs
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
//  06/01/2009 - James R. Carroll
//       Generated original version of source code.
//
//*******************************************************************************************************

namespace TVA.Historian
{
//    [CLSCompliant(false)]
//    public class OutputAdapter : OuputAdapterBase
//    {
		
//        private string m_archiverIP;
//        private string m_archiverHostName;
//        private int m_archiverPort;
//        private int m_maximumEvents;
//        private int m_connectionTimeout;
//        private int m_bufferSize;
//        private byte[] m_buffer;
//        private Exception m_connectionException;
//        private TcpClient m_connection;
//        private bool m_disposed;
		
//        public OutputAdapter()
//        {
			
			
//            m_archiverIP = "127.0.0.1";
//            m_archiverPort = 1003;
//            m_maximumEvents = 100000;
//            m_connectionTimeout = 2000;
			
//        }
		
//        public override void Initialize(string connectionString)
//        {
			
//            string value;
			
//            // Example connection string:
//            // IP=localhost; Port=1003; MaxEvents=100000
//            object with_1 = ParseKeyValuePairs(connectionString);
//            if (with_1.TryGetValue("ip", value))
//            {
//                m_archiverIP = value;
//            }
//            if (with_1.TryGetValue("port", value))
//            {
//                m_archiverPort = Convert.ToInt32(value);
//            }
//            if (with_1.TryGetValue("maxevents", value))
//            {
//                m_maximumEvents = Convert.ToInt32(value);
//            }
//            if (with_1.TryGetValue("timeout", value))
//            {
//                m_connectionTimeout = Convert.ToInt32(value);
//            }
			
//            if (string.IsNullOrEmpty(m_archiverIP))
//            {
//                throw (new InvalidOperationException("Cannot start TCP stream listener connection to historian without specifing a host IP"));
//            }
			
//            m_bufferSize = PacketType1.Size * m_maximumEvents;
//            m_buffer = CreateArray<byte>(m_bufferSize);
			
//            // Attempt to lookup DNS host name for given IP
//            try
//            {
//                m_archiverHostName = Dns.GetHostEntry(m_archiverIP).HostName;
//                if (string.IsNullOrEmpty(m_archiverHostName))
//                {
//                    m_archiverHostName = m_archiverIP;
//                }
//            }
//            catch
//            {
//                m_archiverHostName = m_archiverIP;
//            }
			
//        }
		
//        protected override void AttemptConnection()
//        {
			
//            // Connect to archiver using TCP
//            m_connection = new TcpClient("server=" + m_archiverIP + "; port=" + m_archiverPort);
//            m_connection.ConnectingCancelled += new System.EventHandler(this.m_connection_ConnectingCancelled);
//            m_connection.ConnectingException += new System.EventHandler(this.m_connection_ConnectingException);
//            m_connection.Disconnected += new System.EventHandler(this.m_connection_Disconnected);
//            m_connection.MaximumConnectionAttempts = 1;
//            m_connection.PayloadAware = true;
//            m_connection.Handshake = false;
			
//            m_connectionException = null;
			
//            m_connection.Connect();
			
//            // Block calling thread until connection succeeds or fails
//            if (! m_connection.WaitForConnection(m_connectionTimeout))
//            {
//                if (m_connectionException == null)
//                {
//                    // Failed to connect for unknown reason - restart connection cycle
//                    throw (new InvalidOperationException("Failed to connect"));
//                }
//            }
			
//            // If there was a connection exception, re-throw to restart connect cycle
//            if (m_connectionException != null)
//            {
//                throw (m_connectionException);
//            }
			
//        }
		
//        protected override void AttemptDisconnection()
//        {
			
//            if (m_connection != null)
//            {
//                m_connection.Dispose();
//            }
//            m_connection = null;
//            m_connection.ConnectingCancelled += new System.EventHandler(this.m_connection_ConnectingCancelled);
//            m_connection.ConnectingException += new System.EventHandler(this.m_connection_ConnectingException);
//            m_connection.Disconnected += new System.EventHandler(this.m_connection_Disconnected);
			
//        }
		
//        public override string Name
//        {
//            get
//            {
//                return "Archiver " + m_archiverHostName + ":" + m_archiverPort;
//            }
//        }
		
//        public override string Status
//        {
//            get
//            {
//                System.Text.StringBuilder with_1 = new StringBuilder;
//                if (m_connection != null)
//                {
//                    with_1.Append(m_connection.Status);
//                }
//                with_1.Append(base.Status);
//                return with_1.ToString();
//            }
//        }
		
//        protected override void ArchiveMeasurements(DatAWareHistorianAdapter.OutputAdapter.ArchiveMeasurements[] measurements)
//        {
			
//            if ((m_connection != null)&& m_connection.IsConnected)
//            {
//                IMeasurement measurement;
//                int remainingPoints = measurements.Length;
//                int pointsToArchive;
//                int arrayIndex;
//                int bufferIndex;
//                int x;
				
//                while (remainingPoints > 0)
//                {
//                    pointsToArchive = Minimum(m_maximumEvents, remainingPoints);
//                    remainingPoints -= pointsToArchive;
					
//                    // Load binary standard event images into local buffer
//                    bufferIndex = 0;
					
//                    for (x = arrayIndex; x <= arrayIndex + pointsToArchive - 1; x++)
//                    {
//                        measurement = measurements[x];
//                        if (measurement.Ticks > 0)
//                        {
//                            Buffer.BlockCopy((new PacketType1(measurement)).BinaryImage, 0, m_buffer, bufferIndex * PacketType1.Size, PacketType1.Size);
//                            bufferIndex++;
//                        }
//                    }
					
//                    arrayIndex += pointsToArchive;
					
//                    // Post data to TCP stream
//                    if (bufferIndex > 0)
//                    {
//                        m_connection.Send(m_buffer, 0, bufferIndex * PacketType1.Size);
//                    }
//                }
//            }
			
//        }
		
//        private void m_connection_ConnectingCancelled(object sender, System.EventArgs e)
//        {
			
//            // Signal time-out of connection attempt
//            m_connectionException = new InvalidOperationException("Timed-out waiting for connection");
			
//        }
		
//        private void m_connection_ConnectingException(object sender, DatAWareHistorianAdapter.OutputAdapter.m_connection_ConnectingException.GenericEventArgs<Exception> e)
//        {
			
//            // Take note of connection exception
//            m_connectionException = e.Argument;
			
//        }
		
//        private void m_connection_Disconnected(object sender, System.EventArgs e)
//        {
			
//            // Make sure connection cycle gets restarted when we get disconnected from archiver...
//            Connect();
			
//        }
		
//    }	
}
