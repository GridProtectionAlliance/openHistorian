//******************************************************************************************************
//  ServiceController.cs - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
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
//  02/05/2024 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;
using GSF.Security;
using GSF.Web.Security;
using Microsoft.Owin;

namespace openHistorian;

/// <summary>
/// Represents a REST based API for managing service-level activities for the openHistorian service.
/// </summary>
public class ServiceController : ApiController
{
    /// <summary>
    /// Sends a command to the host service.
    /// </summary>
    /// <param name="command">Command to send to the host service.</param>
    [HttpGet]
    [AuthorizeControllerRole("Administrator, Editor, Viewer")]
    public IHttpActionResult SendCommand(string command)
    {
        return SendCommand(command, false, 0);
    }

    /// <summary>
    /// Sends a command to the host service that expects a return value.
    /// </summary>
    /// <param name="command">Command to send to the host service.</param>
    /// <param name="returnValueTimeout">Timeout for return value response.</param>
    [HttpGet]
    [AuthorizeControllerRole("Administrator, Editor, Viewer")]
    public IHttpActionResult SendCommandWithReturnValue(string command, int returnValueTimeout = 5000)
    {
        return SendCommand(command, true, returnValueTimeout);
    }

    private IHttpActionResult SendCommand(string command, bool expectsReturnValue, int returnValueTimeout)
    {
        try
        {
            // Get authenticated user from OWIN context
            IOwinContext context = Request.GetOwinContext();

            if (context.Request.User is not SecurityPrincipal securityPrincipal)
                return Unauthorized();

            AuthorizationCache.UserIDs.TryGetValue(securityPrincipal.Identity.Name, out Guid clientID);

            // AuthorizeControllerRole verifies user is authenticated and
            // SendCommand verifies user has permission to send command:
            (HttpStatusCode statusCode, string response) = Program.Host.SendCommand(clientID, securityPrincipal, command, expectsReturnValue, returnValueTimeout);

            return Content(statusCode, response, new JsonMediaTypeFormatter());
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }
}