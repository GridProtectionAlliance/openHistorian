//******************************************************************************************************
//  RebroadcastExporter.cs - Gbtc
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
//  -----------------------------------------------------------------------------------------------------
//  07/06/2007 - Pinal C. Patel
//       Original version of source code generated.
//  04/17/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//  11/30/2011 - J. Ritchie Carroll
//       Modified to support buffer optimized ISupportBinaryImage.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.V1.Adapters;
using TVA.Collections;
using TVA.Communication;
using TVA.Parsing;

namespace openHistorian.V1.Exporters
{
    /// <summary>
    /// Represents an exporter that can export real-time time-series data using TCP or UDP to a listening <see cref="System.Net.Sockets.Socket"/>.
    /// </summary>
    /// <example>
    /// Definition of a sample <see cref="Export"/> that can be processed by <see cref="RebroadcastExporter"/>:
    /// <code>
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-16"?>
    /// <Export xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    ///   <Name>RebroadcastExport</Name>
    ///   <Type>RealTime</Type>
    ///   <Interval>0</Interval>
    ///   <Exporter>RebroadcastExporter</Exporter>
    ///   <Settings>
    ///     <ExportSetting>
    ///       <Name>CommunicationConfiguration</Name>
    ///       <Value>Protocol=TCP;Server=localhost:1002</Value>
    ///     </ExportSetting>
    ///     <ExportSetting>
    ///       <Name>LegacyMode</Name>
    ///       <Value>True</Value>
    ///     </ExportSetting>
    ///     <ExportSetting>
    ///       <Name>PacketSize</Name>
    ///       <Value>1100</Value>
    ///     </ExportSetting>
    ///   </Settings>
    ///   <Records>
    ///     <ExportRecord>
    ///       <Instance>P2</Instance>
    ///       <Identifier>1885</Identifier>
    ///     </ExportRecord>  
    ///     <ExportRecord>
    ///       <Instance>P2</Instance>
    ///       <Identifier>2711</Identifier>
    ///     </ExportRecord>
    ///   </Records>
    /// </Export>
    /// ]]>
    /// </code>
    /// <para>
    /// Description of custom settings required by <see cref="RebroadcastExporter"/> in an <see cref="Export"/>:
    /// <list type="table">
    ///     <listheader>
    ///         <term>Setting Name</term>
    ///         <description>Setting Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CommunicationConfiguration</term>
    ///         <description>
    ///         Connection information for connecting to a remote <see cref="System.Net.Sockets.Socket"/>.<br/><br/>
    ///         TCP example: Protocol=TCP;Server=localhost:1002<br/>
    ///         UDP example: Protocol=UDP;Server=localhost:1002<br/>
    ///         where the value of <b>Server</b> must be in the format of <b>[Remote IP or DNS Name]:[Remote Port]</b>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>LegacyMode (Optional)</term>
    ///         <description>True if export data is to be transmitted in <see cref="PacketType1"/> and False if export data is to be transmitted in <see cref="PacketType101"/>.</description>
    ///     </item>
    ///     <item>
    ///         <term>PacketSize (Optional)</term>
    ///         <description>Maximum size of the packet in which the export data is to be transmitted.</description>
    ///     </item>
    /// </list>
    /// </para>
    /// </example>
    /// <seealso cref="Export"/>
    public class RebroadcastExporter : ExporterBase
    {
        #region [ Members ]

        // Nested Types

        /// <summary>
        /// A class for storing runtime information of an <see cref="Export"/>.
        /// </summary>
        private class ExportContext
        {
            /// <summary>
            /// <see cref="IClient"/> used for transmitting the time-series data.
            /// </summary>
            public IClient Socket;

            /// <summary>
            /// Number of time-series data points to be transmitted in a single packet.
            /// </summary>
            public int DataPerPacket;

            /// <summary>
            /// <see cref="Delegate"/> to invoke for transmitting the time-series data.
            /// </summary>
            public Action<ExportContext, IList<IDataPoint>> TransmitHandler;
        }

        // Fields
        private Dictionary<string, ExportContext> m_contexts;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RebroadcastExporter"/> class.
        /// </summary>
        public RebroadcastExporter()
            : this("RebroadcastExporter")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RebroadcastExporter"/> class.
        /// </summary>
        /// <param name="name"><see cref="ExporterBase.Name"/> of the exporter.</param>
        protected RebroadcastExporter(string name)
            : base(name)
        {
            m_contexts = new Dictionary<string, ExportContext>();

            // Register handlers.
            ExportAddedHandler = CreateContext;
            ExportRemovedHandler = RemoveContext;
            ExportUpdatedHandler = UpdateContext;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Processes the <paramref name="export"/> using the current <see cref="DataListener.Data"/>.
        /// </summary>
        /// <param name="export"><see cref="Export"/> to be processed.</param>
        protected override void ProcessExport(Export export)
        {
            List<IDataPoint> dataToTransmit = new List<IDataPoint>();
            Dictionary<string, IList<IDataPoint>> exportData = GetExportData(export);

            // Gather the current data to be transmitted for the export.
            foreach (string listenerName in exportData.Keys)
            {
                dataToTransmit.AddRange(exportData[listenerName]);
            }

            // Transmit the prepared current data.
            TransmitData(export, dataToTransmit);
        }

        /// <summary>
        /// Processes the <paramref name="export"/> using the real-time <paramref name="data"/>.
        /// </summary>
        /// <param name="export"><see cref="Export"/> to be processed.</param>
        /// <param name="listener"><see cref="DataListener"/> that provided the <paramref name="data"/>.</param>
        /// <param name="data">Real-time time-series data received by the <paramref name="listener"/>.</param>
        protected override void ProcessRealTimeExport(Export export, DataListener listener, IList<IDataPoint> data)
        {
            // In case of real-time export, immediately transmit the received data.
            TransmitData(export, data);
        }

        /// <summary>
        /// Performs the transmission of time-series data for the <paramref name="export"/>.
        /// </summary>
        /// <param name="export"><see cref="Export"/> whose time-series data os to be transmitted.</param>
        /// <param name="dataToTransmit">Collection of time-series data to be transmitted.</param>
        protected virtual void TransmitData(Export export, IList<IDataPoint> dataToTransmit)
        {
            // Retrieve the export context.
            ExportContext context;
            lock (m_contexts)
            {
                m_contexts.TryGetValue(export.Name, out context);
            }

            if (context != null)
            {
                // Context for the export exists.
                if (context.Socket.CurrentState == ClientState.Disconnected)
                {
                    // Attempt to connect the socket.
                    context.Socket.Connect();
                    if (context.Socket.CurrentState != ClientState.Connected)
                        return;
                }

                // Socket is connected, so transmit the export data.
                context.TransmitHandler(context, dataToTransmit);
            }
        }

        /// <summary>
        /// Transmits export data in <see cref="PacketType1"/>
        /// </summary>
        private void TransmitPacketType1(ExportContext context, IList<IDataPoint> dataToTransmit)
        {
            byte[] dataBuffer = new byte[context.DataPerPacket * PacketType1.FixedLength];
            for (int i = 0; i < dataToTransmit.Count; i += context.DataPerPacket)
            {
                // Transmit the data at the maximum allowed rate.
                int dataCount = 0;
                for (int j = i; j < (i + (dataToTransmit.Count - i < context.DataPerPacket ? dataToTransmit.Count - i : context.DataPerPacket)); j++)
                {
                    // Prepare binary image of the data.
                    new PacketType1(dataToTransmit[j]).GenerateBinaryImage(dataBuffer, dataCount * PacketType1.FixedLength);
                    dataCount++;
                }
                // Transmit the prepared binary image.
                context.Socket.SendAsync(dataBuffer, 0, dataCount * PacketType1.FixedLength);
            }
        }

        /// <summary>
        /// Transmits export data in <see cref="PacketType101"/>
        /// </summary>
        private void TransmitPacketType101(ExportContext context, IList<IDataPoint> dataToTransmit)
        {
            PacketType101 packet;
            if (dataToTransmit.Count <= context.DataPerPacket)
            {
                // Transmit all the data.
                packet = new PacketType101(dataToTransmit);
                context.Socket.SendAsync(packet.BinaryImage(), 0, packet.BinaryLength);
            }
            else
            {
                // Transmit data at the maximum allowed rate.
                for (int i = 0; i < dataToTransmit.Count; i += context.DataPerPacket)
                {
                    packet = new PacketType101(dataToTransmit.GetRange(i, dataToTransmit.Count - i < context.DataPerPacket ? dataToTransmit.Count - i : context.DataPerPacket));
                    context.Socket.SendAsync(packet.BinaryImage(), 0, packet.BinaryLength);
                }
            }
        }

        private void CreateContext(Export export)
        {
            ExportContext context;
            lock (m_contexts)
            {
                if (!m_contexts.TryGetValue(export.Name, out context))
                {
                    // Context for the export does not exist, so we create one.
                    ExportSetting clientConfigSetting = export.FindSetting("CommunicationConfiguration");
                    ExportSetting legacyModeSetting = export.FindSetting("LegacyMode");
                    ExportSetting packetSizeSetting = export.FindSetting("PacketSize");
                    if ((clientConfigSetting != null) && !string.IsNullOrEmpty(clientConfigSetting.Value))
                    {
                        // Create the client socket for the export context.
                        context = new ExportContext();
                        context.Socket = ClientBase.Create(clientConfigSetting.Value + "; Port=0");
                        context.Socket.Handshake = false;           // Use regular TCP/UDP socket communication.
                        context.Socket.MaxConnectionAttempts = 1;   // Try connection attempt no more than 1 time.

                        // Determine the format of transmission.
                        if (legacyModeSetting != null && Convert.ToBoolean(legacyModeSetting.Value))
                        {
                            // Data is to be transmitted using PacketType1.
                            context.TransmitHandler = TransmitPacketType1;
                            if (packetSizeSetting != null)
                                // Custom packet size is specified, so determine the maximum number of data points in a packet.
                                context.DataPerPacket = Convert.ToInt32(packetSizeSetting.Value) / PacketType1.FixedLength;
                            else
                                // Custom packet size is not specified, so used the defaults for the transmission protocol.
                                context.DataPerPacket = (context.Socket.TransportProtocol == TransportProtocol.Tcp ? 50 : 1400);
                        }
                        else
                        {
                            // Data is to be transmitted using PacketType101.
                            context.TransmitHandler = TransmitPacketType101;
                            if (packetSizeSetting != null)
                                // Custom packet size is specified, so determine the maximum number of data points in a packet.
                                context.DataPerPacket = Convert.ToInt32(packetSizeSetting.Value) / PacketType101DataPoint.FixedLength;
                            else
                                // Custom packet size is not specified, so used the defaults for the transmission protocol.
                                context.DataPerPacket = (context.Socket.TransportProtocol == TransportProtocol.Tcp ? 1000000 : 2200);
                        }

                        // Save export context.
                        m_contexts.Add(export.Name, context);

                        // Provide a status update.
                        OnStatusUpdate(string.Format("{0} based client created for export {1}", context.Socket.TransportProtocol.ToString().ToUpper(), export.Name));
                    }
                }
            }
        }

        private void RemoveContext(Export export)
        {
            ExportContext context;
            lock (m_contexts)
            {
                if (m_contexts.TryGetValue(export.Name, out context))
                {
                    // Context for the export exists, so remove it from the list.
                    context.Socket.Dispose();
                    m_contexts.Remove(export.Name);

                    // Provide a status update.
                    OnStatusUpdate(string.Format("{0} based client of export {1} removed", context.Socket.TransportProtocol.ToString().ToUpper(), export.Name));
                }
            }
        }

        private void UpdateContext(Export export)
        {
            RemoveContext(export);   // Remove context.
            CreateContext(export);   // Create context.
        }

        #endregion
    }
}
