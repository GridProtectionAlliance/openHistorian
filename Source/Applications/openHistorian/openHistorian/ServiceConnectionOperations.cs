//******************************************************************************************************
//  ServiceConnectionOperations.cs - Gbtc
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
//  10/14/2016 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF;
using GSF.Web.Hubs;
using Microsoft.AspNet.SignalR.Hubs;

namespace openHistorian
{
    /// <summary>
    /// Defines an interface for using <see cref="ServiceConnectionOperations"/> within a SignalR hub.
    /// </summary>
    /// <remarks>
    /// This interface makes sure all hub methods needed by RemoteConsole.cshtml get properly defined.
    /// </remarks>
    public interface IServiceConnectionOperations
    {
        /// <summary>
        /// Sends a service command.
        /// </summary>
        /// <param name="command">Command string.</param>
        void SendCommand(string command);
    }

    /// <summary>
    /// Represents hub operations for using <see cref="ServiceConnectionHubClient"/> instances.
    /// </summary>
    /// <remarks>
    /// This hub client operations class makes sure a service connection is created per SignalR session and only created when needed.
    /// </remarks>
    public class ServiceConnectionOperations : HubClientOperationsBase<ServiceConnectionHubClient>, IServiceConnectionOperations
    {
        /// <summary>
        /// Creates a new <see cref="ServiceConnectionOperations"/> instance.
        /// </summary>
        /// <param name="hub">Parent hub.</param>
        /// <param name="logStatusMessageFunction">Delegate to use to log status messages, if any.</param>
        /// <param name="logExceptionFunction">Delegate to use to log exceptions, if any.</param>
        public ServiceConnectionOperations(IHub hub, Action<string, UpdateType> logStatusMessageFunction = null, Action<Exception> logExceptionFunction = null) :
            base(hub, logStatusMessageFunction, logExceptionFunction)
        {
        }

        /// <summary>
        /// Sends a service command.
        /// </summary>
        /// <param name="command">Command string.</param>
        public void SendCommand(string command) =>
            HubClient.SendCommand(command);
    }
}