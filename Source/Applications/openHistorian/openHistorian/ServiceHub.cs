//******************************************************************************************************
//  ConsoleHub.cs - Gbtc
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
//  01/12/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace openHistorian
{
    public class ServiceHub : Hub
    {
        #region [ Members ]

        // Fields
        private readonly ServiceConnection m_serviceConnection;

        #endregion

        #region [ Constructors ]

        public ServiceHub()
        {
            m_serviceConnection = ServiceConnection.Default;
        }

        #endregion

        #region [ Methods ]

        public override Task OnConnected()
        {
            s_connectCount++;
            Program.Host.LogStatusMessage($"ServiceHub connect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {s_connectCount}");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                s_connectCount--;
                m_serviceConnection.Disconnect(Context.ConnectionId);
                Program.Host.LogStatusMessage($"ServiceHub disconnect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {s_connectCount}");
            }

            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Gets the current server time.
        /// </summary>
        /// <returns>Current server time.</returns>
        public DateTime GetServerTime() => DateTime.UtcNow;

        /// <summary>
        /// Gets current performance statistics for service.
        /// </summary>
        /// <returns>Current performance statistics for service.</returns>
        public string GetPerformanceStatistics() => Program.Host.PerformanceStatistics;

        /// <summary>
        /// Sends a service command.
        /// </summary>
        /// <param name="command">Command string.</param>
        public void SendCommand(string command)
        {
            m_serviceConnection.SendCommand(Context.ConnectionId, command);
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static volatile int s_connectCount;

        #endregion
    }
}
