//******************************************************************************************************
//  ServiceConnection.cs - Gbtc
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
//  01/15/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GSF;
using GSF.Console;
using GSF.ServiceProcess;
using GSF.TimeSeries.Data;
using GSF.Web.Hubs;

namespace openHistorian
{
    /// <summary>
    /// Represents a client instance of a <see cref="ServiceHub"/> for a remote console connection.
    /// </summary>
    public class ServiceConnectionHubClient : HubClientBase
    {
        #region [ Members ]

        // Fields
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="ServiceConnectionHubClient"/> instance.
        /// </summary>
        public ServiceConnectionHubClient()
        {
            Program.Host.UpdatedStatus += m_serviceHost_UpdatedStatus;
            Program.Host.SendingClientResponse += m_service_SendingClientResponse;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ServiceConnectionHubClient"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                if (!disposing)
                    return;

                Program.Host.SendingClientResponse -= m_service_SendingClientResponse;
                Program.Host.UpdatedStatus -= m_serviceHost_UpdatedStatus;

                if (Guid.TryParse(ConnectionID, out Guid clientID))
                    Program.Host.DisconnectClient(clientID);
            }
            finally
            {
                m_disposed = true;          // Prevent duplicate dispose.
                base.Dispose(disposing);    // Call base class Dispose().
            }
        }

        /// <summary>
        /// Sends a service command.
        /// </summary>
        /// <param name="command">Command string.</param>
        public void SendCommand(string command)
        {
            // Note that rights of current thread principle will be used to determine service command rights...
            if (Guid.TryParse(ConnectionID, out Guid clientID))
                Program.Host.SendCommand(clientID, HubInstance.Context.User, command);
        }

        private void m_serviceHost_UpdatedStatus(object sender, EventArgs<Guid, string, UpdateType> e)
        {
            // Only show broadcast messages or those destined to this client
            if (!Guid.TryParse(ConnectionID, out Guid clientID) || e.Argument1 != Guid.Empty && e.Argument1 != clientID)
                return;

            string color = e.Argument3 switch
            {
                UpdateType.Alarm => "red",
                UpdateType.Warning => "yellow",
                _ => "white"
            };

            BroadcastMessage(e.Argument2, color);
        }

        private void BroadcastMessage(string message, string color)
        {
            if (string.IsNullOrEmpty(color))
                color = "white";
            
            ClientScript.broadcastMessage(message, color);
        }

        private void m_service_SendingClientResponse(object sender, EventArgs<Guid, ServiceResponse, bool> e)
        {
            Guid responseClientID = e.Argument1;
            ServiceResponse response = e.Argument2;

            if (!Guid.TryParse(ConnectionID, out Guid clientID) || !clientID.Equals(responseClientID) ||
                !ClientHelper.TryParseActionableResponse(response, out string sourceCommand, out _))
                return;

            // If actionable client response is successful and targeted for this hub client,
            // inform service helper that it does not need to broadcast a response
            e.Argument3 = false;

            Guid[] parseSignalIDs(out string sourceAdapter)
            {
                sourceAdapter = null;

                try
                {
                    List<object> attachments = response.Attachments;

                    if (attachments is null || attachments.Count < 2 ||
                        !(attachments[0] is byte[][] signalIDs) ||
                        !(attachments[1] is Arguments arguments) || 
                        !arguments.Exists("OrderedArg1"))
                        return Array.Empty<Guid>();
                    
                    sourceAdapter = arguments["OrderedArg1"];
                    return signalIDs.Select(bytes => new Guid(bytes)).Where(id => id != Guid.Empty).ToArray();
                }
                catch (Exception ex)
                {
                    Program.Host.LogException(new InvalidOperationException($"Failed to parse actionable service response with Guid buffers: {ex.Message}", ex));
                    return Array.Empty<Guid>();
                }
            }

            string command = sourceCommand.ToLower().Trim();

            if (command.Equals("getinputmeasurements"))
            {
                Guid[] signalIDs = parseSignalIDs(out string sourceAdapter);

                if (!string.IsNullOrWhiteSpace(sourceAdapter))
                    ClientScript?.parsedInputMeasurements(sourceAdapter, signalIDs.Select(LookupPointTag).Where(tag => !string.IsNullOrWhiteSpace(tag)));
            }
            else if (command.Equals("getoutputmeasurements"))
            {
                Guid[] signalIDs = parseSignalIDs(out string sourceAdapter);

                if (!string.IsNullOrWhiteSpace(sourceAdapter))
                    ClientScript?.parsedOutputMeasurements(sourceAdapter, signalIDs.Select(LookupPointTag).Where(tag => !string.IsNullOrWhiteSpace(tag)));
            }
        }

        private static string LookupPointTag(Guid signalID)
        {
            DataSet metadata = Program.Host.Metadata;

            if (metadata is null)
                return null;

            DataRow record = metadata.LookupMetadata(signalID, "ActiveMeasurements");

            if (record is null || record["SignalType"].ToString().Equals("STAT", StringComparison.OrdinalIgnoreCase))
                return null;

            string pointTag = record["PointTag"].ToString();
            return string.IsNullOrWhiteSpace(pointTag) ? null : pointTag;
        }

        #endregion
    }
}
