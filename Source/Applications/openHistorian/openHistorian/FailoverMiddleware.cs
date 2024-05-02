//******************************************************************************************************
//  FailoverMiddleware.cs - Gbtc
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
//  04/16/2024 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using GSF.Diagnostics;
using Microsoft.Owin;
using Owin;

namespace openHistorian
{
    public class FailoverMiddleware(OwinMiddleware next) : OwinMiddleware(next)
    {
        public override Task Invoke(IOwinContext context)
        {
            if (!IsAuthorized(context.Request))
            {
                string user = context.Request.User?.Identity.Name ?? "Unknown User";
                string message = $"Rejecting unauthorized attempt to fail over by user \"{user}\"";
                Log.Publish(MessageLevel.Info, nameof(Invoke), message);

                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ReasonPhrase = "Forbidden";
                return Task.CompletedTask;
            }

            string host = context.Request.Host.Value;
            if (string.IsNullOrEmpty(host))
                host = context.Request.RemoteIpAddress;

            if (!Program.Host.FailOver(host))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ReasonPhrase = "Bad Request";

                byte[] errorMessage = Encoding.ASCII.GetBytes("Unable to fail over at this time. Check server logs for more details.");
                context.Response.ContentType = "text/plain";
                context.Response.ContentLength = errorMessage.Length;
                context.Response.Body.Write(errorMessage, 0, errorMessage.Length);
                return Task.CompletedTask;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ReasonPhrase = "OK";
            return Task.CompletedTask;
        }

        private bool IsAuthorized(IOwinRequest request) =>
            request.User?.Identity is WindowsIdentity identity &&
            identity.User == WindowsIdentity.GetCurrent().User;

        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(FailoverMiddleware), MessageClass.Application);
    }

    public static partial class AppBuilderExtensions
    {
        public static IAppBuilder UseFailover(this IAppBuilder app, string path)
        {
            void RegisterAuthenticationSchemeSelector(IAppBuilder deferredApp) =>
                deferredApp.RegisterFailoverAuthenticationSchemeSelector(path);

            return app
                .UseFailoverMiddleware(path)
                .Defer(RegisterAuthenticationSchemeSelector);
        }

        private static IAppBuilder UseFailoverMiddleware(this IAppBuilder app, string path)
        {
            PathString pathString = new(path);

            bool IsFailOver(IOwinContext context) =>
                context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                context.Request.Path.StartsWithSegments(pathString);

            return app.MapWhen(IsFailOver, branch => branch.Use<FailoverMiddleware>());
        }

        private static void RegisterFailoverAuthenticationSchemeSelector(this IAppBuilder app, string path)
        {
            bool IsFailOver(HttpListenerRequest request) =>
                request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                request.Url.LocalPath.Equals(path, StringComparison.OrdinalIgnoreCase);

            HttpListener listener = (HttpListener)app.Properties["System.Net.HttpListener"];
            AuthenticationSchemeSelector oldSelector = listener.AuthenticationSchemeSelectorDelegate;

            listener.AuthenticationSchemeSelectorDelegate = request =>
            {
                if (IsFailOver(request))
                    return AuthenticationSchemes.Ntlm;

                return oldSelector(request);
            };
        }
    }
}