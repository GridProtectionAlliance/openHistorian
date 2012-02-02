//******************************************************************************************************
//  NativeInputAdapter.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  04/05/2011 - Pinal C. Patel
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using openHistorian.Adapters.Native.Packets;
using openHistorian.Archives;
using TVA;
using TVA.Collections;
using TVA.Communication;

namespace openHistorian.Adapters.Native
{
    [XmlSerializerFormat()]
    public class NativeInputAdapter : DataAdapterBase
    {
        #region [ Members ]

        // Constants
        private const string DefaultSocketSettings = "Port=1004; PayloadAware=True";

        // Fields
        private string m_socketSettings;
        private Ticks m_lastStatusUpdate;
        private TcpServer m_socket;
        private PacketParser m_parser;
        private Dictionary<int, string> m_lookup;
        private IdentifiableItem<string, IDataArchive> m_archive;
        private ProcessQueue<IdentifiableItem<Guid, IList<IPacket>>> m_queue;
        private bool m_disposed;
        private bool m_initialized;

        #endregion

        #region [ Constructors ]

        public NativeInputAdapter()
        {
            m_socketSettings = DefaultSocketSettings;
        }

        #endregion

        #region [ Properties ]

        public string SocketSettings
        {
            get
            {
                return m_socketSettings;
            }
            set
            {
                m_socketSettings = value;
            }
        }

        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.Append(base.Status);
                status.Append(m_socket.Status);
                status.Append(m_parser.Status);
                status.Append(m_queue.Status);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        public override void Initialize()
        {
            base.Initialize();
            if (!m_initialized)
            {
                // Find the target archive for received data.
                IDictionary<string, IDataArchive> archives = FindTargetArchives(1, 1);
                m_archive = new IdentifiableItem<string, IDataArchive>(archives.First().Key, archives.First().Value);

                // Create lookup from mappings for runtime use.
                m_lookup = new Dictionary<int, string>();
                foreach (IDataMapping mapping in Mappings.Where(m => m.Source != "*"))
                {
                    m_lookup.Add(int.Parse(mapping.Source), mapping.Target);
                }

                // Initialize receiver socket.
                m_socket = new TcpServer();
                m_socket.ClientConnected += Socket_ClientConnected;
                m_socket.ClientDisconnected += Socket_ClientDisconnected;
                m_socket.ReceiveClientDataComplete += Socket_ReceiveClientDataComplete;
                m_socket.ReceiveClientDataException += Socket_ReceiveClientDataException;

                // Initialize raw data parser. 
                m_parser = new PacketParser();
                m_parser.DataParsed += Parser_DataParsed;
                m_parser.ParsingException += Parser_ParsingException;
                m_parser.OutputTypeNotFound += Parser_OutputTypeNotFound;

                // Create queue for processing parsed packets.
                m_queue = ProcessQueue<IdentifiableItem<Guid, IList<IPacket>>>.CreateRealTimeQueue(ProcessPackets, CanProcessPackets);

                m_initialized = true;
            }
        }

        public override void Start()
        {
            if (State == DataAdapterState.Stopped)
            {
                State = DataAdapterState.Starting;

                // Initialize if uninitialized.
                Initialize();

                // Start process queue.
                OnStatusUpdate(UpdateType.Information, "Starting process queue");
                m_queue.Start();
                OnStatusUpdate(UpdateType.Information, "Process queue started");

                // Start packet parser.
                OnStatusUpdate(UpdateType.Information, "Starting packet parser");
                m_parser.Start();
                OnStatusUpdate(UpdateType.Information, "Packet parser started");

                // Start receiver socket.
                OnStatusUpdate(UpdateType.Information, "Starting receiver socket");
                m_socket.ConfigurationString = m_socketSettings;
                m_socket.Start();
                OnStatusUpdate(UpdateType.Information, "Receiver socket started");

                State = DataAdapterState.Started;
            }
        }

        public override void Stop()
        {
            if (State == DataAdapterState.Started ||
                State == DataAdapterState.Starting)
            {
                State = DataAdapterState.Stopping;

                // Stop receiver socket.
                OnStatusUpdate(UpdateType.Information, "Stopping receiver socket");
                m_socket.Stop();
                OnStatusUpdate(UpdateType.Information, "Receiver socket socket");

                // Stop packet parser.
                OnStatusUpdate(UpdateType.Information, "Stopping packet parser");
                m_parser.Stop();
                OnStatusUpdate(UpdateType.Information, "Packet parser stopped");

                // Start process queue.
                OnStatusUpdate(UpdateType.Information, "Stopping process queue");
                m_queue.Stop();
                OnStatusUpdate(UpdateType.Information, "Process queue stopped");

                State = DataAdapterState.Stopped;
            }
        }

        protected override void OnArchiveAdded(IDataArchive archive)
        {
            if (string.Compare(m_archive.ID, archive.Name) == 0)
            {
                // Save reference to the added archive.
                m_archive.Item = archive;
                OnStatusUpdate(UpdateType.Information, "Saved reference to \"{0}\"", m_archive.ID);
            }
        }

        protected override void OnArchiveRemoved(IDataArchive archive)
        {
            if (string.Compare(m_archive.ID, archive.Name) == 0)
            {
                // Remove reference of the removed archive.
                m_archive = null;
                OnStatusUpdate(UpdateType.Information, "Removed reference to \"{0}\"", m_archive.ID);
            }
        }

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
                        Stop();

                        if (m_socket != null)
                        {
                            m_socket.ClientConnected -= Socket_ClientConnected;
                            m_socket.ClientDisconnected -= Socket_ClientDisconnected;
                            m_socket.ReceiveClientDataComplete -= Socket_ReceiveClientDataComplete;
                            m_socket.Dispose();
                        }
                        m_socket = null;

                        if (m_parser != null)
                        {
                            m_parser.DataParsed -= Parser_DataParsed;
                            m_parser.OutputTypeNotFound -= Parser_OutputTypeNotFound;
                            m_parser.ParsingException -= Parser_ParsingException;
                            m_parser.Dispose();
                        }
                        m_parser = null;

                        if (m_queue != null)
                        {
                            m_queue.Dispose();
                        }
                        m_queue = null;
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        private bool CanProcessPackets(IdentifiableItem<Guid, IList<IPacket>> item)
        {
            return (m_archive.Item != null && m_archive.Item.State == DataArchiveState.Open);
        }

        private void ProcessPackets(IdentifiableItem<Guid, IList<IPacket>>[] items)
        {
            IEnumerable<byte[]> replyData;
            foreach (IdentifiableItem<Guid, IList<IPacket>> item in items)
            {
                foreach (IPacket packet in item.Item)
                {
                    // Process packet.
                    try
                    {
                        if (packet.ProcessHandler != null)
                        {
                            replyData = packet.ProcessHandler();
                            if (replyData != null)
                            {
                                foreach (byte[] data in replyData)
                                {
                                    m_socket.SendToAsync(item.ID, data, 0, data.Length);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OnExecutionException("Error encountered during processing", ex);
                    }
                }
            }
        }

        private void Socket_ClientConnected(object sender, EventArgs<Guid> e)
        {
            OnStatusUpdate(UpdateType.Information, "Client connected");
        }

        private void Socket_ClientDisconnected(object sender, EventArgs<Guid> e)
        {
            OnStatusUpdate(UpdateType.Information, "Client disconnected");
        }

        private void Socket_ReceiveClientDataComplete(object sender, EventArgs<Guid, byte[], int> e)
        {
            // Queue data for parsing.
            m_parser.Parse(e.Argument1, e.Argument2, 0, e.Argument3);

            // Show periodic status update.
            if (m_queue.TotalProcessedItems > 0 && (DateTime.Now.Ticks - m_lastStatusUpdate).ToSeconds() >= 30)
            {
                m_lastStatusUpdate = DateTime.Now.Ticks;
                OnStatusUpdate(UpdateType.Information, "{0} of {1} packets processed so far", m_queue.TotalProcessedItems, m_queue.TotalProcessedItems + m_queue.ItemsBeingProcessed + m_queue.Count);
            }
        }

        private void Socket_ReceiveClientDataException(object sender, EventArgs<Guid, Exception> e)
        {
            OnExecutionException("Error encountered in receiving data", e.Argument2);
        }

        private void Parser_DataParsed(object sender, EventArgs<Guid, IList<IPacket>> e)
        {
            string key;
            IData dataPoint;
            IEnumerable<byte[]> replyData;
            foreach (IPacket packet in e.Argument2)
            {
                // Pre-process packet.
                try
                {
                    // Perform mapping for data.
                    dataPoint = packet as IData;
                    if (dataPoint != null && m_lookup.TryGetValue(dataPoint.Key.Id, out key))
                        dataPoint.Key = key;

                    packet.Archive = m_archive.Item;
                    if (packet.PreProcessHandler != null)
                    {
                        replyData = packet.PreProcessHandler();
                        if (replyData != null)
                        {
                            foreach (byte[] data in replyData)
                            {
                                m_socket.SendToAsync(e.Argument1, data, 0, data.Length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnExecutionException("Error encountered during pre-processing", ex);
                }
            }

            m_queue.Add(new IdentifiableItem<Guid, IList<IPacket>>(e.Argument1, e.Argument2));
        }

        private void Parser_OutputTypeNotFound(object sender, EventArgs<short> e)
        {
            OnStatusUpdate(UpdateType.Warning, string.Format("Packet ID {0} is not supported", e.Argument));
        }

        private void Parser_ParsingException(object sender, EventArgs<Exception> e)
        {
            OnExecutionException("Error encountered during parsing", e.Argument);
        }

        #endregion
    }
}
