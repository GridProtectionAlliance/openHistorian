//******************************************************************************************************
//  Startup.cs - Gbtc
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using GSF.Web.Hosting;
using GSF.Web.Security;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using openHistorian.Model;
using Owin;
using ModbusAdapters;
using GSF.Web;
using GSF.IO;

namespace openHistorian
{
    public class HostedExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            Program.Host.LogException(context.Exception);
            base.Handle(context);
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Modify the JSON serializer to serialize dates as UTC - otherwise, timezone will not be appended
            // to date strings and browsers will select whatever timezone suits them
            JsonSerializerSettings settings = JsonUtility.CreateDefaultSerializerSettings();
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            JsonSerializer serializer = JsonSerializer.Create(settings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);
            
            // Load security hub in application domain before establishing SignalR hub configuration
            try
            {
                using (new SecurityHub()) { }
            }
            catch (Exception ex)
            {
                throw new SecurityException($"Failed to load Security Hub, validate database connection string in configuration file: {ex.Message}", ex);
            }

            // Load Modbus assembly
            try
            {
                using (ModbusPoller poller = new ModbusPoller())
                {
                    // Make embedded resources of Modbus poller available to web server
                    WebExtensions.AddEmbeddedResourceAssembly(poller.GetType().Assembly);

                    Dictionary<string, string> replacements = new Dictionary<string, string>()
                        { { "{Namespace}", "openHistorian" } };

                    // Extract and update local Modbus configuration screens
                    string webRootPath = FilePath.GetAbsolutePath($"{FilePath.AddPathSuffix(Program.Host.Model.Global.WebRootPath)}");
                    ExtractTextResource("ModbusAdapters.ModbusConfig.cshtml", $"{webRootPath}ModbusConfig.cshtml", replacements);
                    ExtractTextResource("ModbusAdapters.Status.cshtml", $"{webRootPath}Status.cshtml", replacements);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load Modbus assembly: {ex.Message}", ex);
            }

            // Configure Windows Authentication for self-hosted web service
            HttpListener listener = (HttpListener)app.Properties["System.Net.HttpListener"];
            listener.AuthenticationSchemeSelectorDelegate = AuthenticationSchemeForClient;

            HubConfiguration hubConfig = new HubConfiguration();
            HttpConfiguration httpConfig = new HttpConfiguration();

            // Setup resolver for web page controller instances
            httpConfig.DependencyResolver = WebPageController.GetDependencyResolver(WebServer.Default, Program.Host.DefaultWebPage, new AppModel(), typeof(AppModel));

            // Make sure any hosted exceptions get propagated to service error handling
            httpConfig.Services.Replace(typeof(IExceptionHandler), new HostedExceptionHandler());

            // Enabled detailed client errors
            hubConfig.EnableDetailedErrors = true;

            // Enable cross-domain scripting
            app.UseCors(CorsOptions.AllowAll);

            // Load ServiceHub SignalR class
            app.MapSignalR(hubConfig);

            // Map specific historian instance API controllers
            httpConfig.Routes.MapHttpRoute(
                name: "InstanceAPIs",
                routeTemplate: "instance/{instanceName}/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = RouteParameter.Optional }
            );

            // Map custom API controllers
            httpConfig.Routes.MapHttpRoute(
                name: "CustomAPIs",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = RouteParameter.Optional }
            );

            // Set configuration to use reflection to setup routes
            httpConfig.MapHttpAttributeRoutes();

            // Load the WebPageController class and assign its routes
            app.UseWebApi(httpConfig);

            // Check for configuration issues before first request
            httpConfig.EnsureInitialized();
        }

        private static AuthenticationSchemes AuthenticationSchemeForClient(HttpListenerRequest request)
        {
            string urlPath = request.Url.PathAndQuery;

            if (urlPath.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) || urlPath.StartsWith("/instance/", StringComparison.OrdinalIgnoreCase))
                return AuthenticationSchemes.Anonymous;

            // Explicitly select NTLM, since Negotiate seems to fail
            // when accessing the page using the system's domain name
            // while the application is running as a domain account
            return AuthenticationSchemes.Ntlm;
        }

        private static void ExtractTextResource(string resourceName, string fileName, IEnumerable<KeyValuePair<string, string>> replacements)
        {
            Stream stream = WebExtensions.OpenEmbeddedResourceStream(resourceName);

            if ((object)stream != null)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string resourceData = reader.ReadToEnd();

                    using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
                    {
                        foreach (KeyValuePair<string, string> replacement in replacements)
                            resourceData = resourceData.Replace(replacement.Key, replacement.Value);

                        writer.Write(resourceData);
                    }
                }
            }
        }
    }
}
