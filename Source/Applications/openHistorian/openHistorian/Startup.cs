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

using GSF;
using GSF.IO;
using GSF.Web;
using GSF.Web.Hosting;
using GSF.Web.Security;
using GSF.Web.Shared;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Json;
using ModbusAdapters;
using Newtonsoft.Json;
using openHistorian.Model;
using Owin;
using System;
using System.Security;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;

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
            AppModel model = Program.Host.Model;

            // Load security hub into application domain before establishing SignalR hub configuration, initializing default status and exception handlers
            try
            {
                using (new SecurityHub(
                    (message, updateType) => Program.Host.LogWebHostStatusMessage(message, updateType),
                    ex => Program.Host.LogException(ex)
                )) { }
            }
            catch (Exception ex)
            {
                Program.Host.LogException(new SecurityException($"Failed to load Security Hub, validate database connection string in configuration file: {ex.Message}", ex));
            }

            // Load shared hub into application domain, initializing default status and exception handlers
            try
            {
                using (new SharedHub(
                    (message, updateType) => Program.Host.LogWebHostStatusMessage(message, updateType),
                    ex => Program.Host.LogException(ex)
                )) { }
            }
            catch (Exception ex)
            {
                Program.Host.LogException(new SecurityException($"Failed to load Shared Hub: {ex.Message}", ex));
            }

            Load_ModbusAssembly();
            Load_OSIPIGrafanaController();
            Load_eDNAGrafanaController();

            // Configure Windows Authentication for self-hosted web service
            HubConfiguration hubConfig = new HubConfiguration();
            HttpConfiguration httpConfig = new HttpConfiguration();

            // Make sure any hosted exceptions get propagated to service error handling
            httpConfig.Services.Replace(typeof(IExceptionHandler), new HostedExceptionHandler());

            // Enabled detailed client errors
            hubConfig.EnableDetailedErrors = true;

            // Enable GSF session management
            httpConfig.EnableSessions(AuthenticationOptions);

            // Enable GSF role-based security authentication
            app.UseAuthentication(AuthenticationOptions);

            // Enable cross-domain scripting default policy - controllers can manually
            // apply "EnableCors" attribute to class or an action to override default
            // policy configured here
            try
            {
                if (!string.IsNullOrWhiteSpace(model.Global.DefaultCorsOrigins))
                    httpConfig.EnableCors(new EnableCorsAttribute(model.Global.DefaultCorsOrigins, model.Global.DefaultCorsHeaders, model.Global.DefaultCorsMethods) { SupportsCredentials = model.Global.DefaultCorsSupportsCredentials });
            }
            catch (Exception ex)
            {
                Program.Host.LogException(new InvalidOperationException($"Failed to establish default CORS policy: {ex.Message}", ex));
            }

            // Load ServiceHub SignalR class
            app.MapSignalR(hubConfig);

            // Map specific historian instance API controllers
            try
            {
                httpConfig.Routes.MapHttpRoute(
                    name: "InstanceAPIs",
                    routeTemplate: "instance/{instanceName}/{controller}/{action}/{id}",
                    defaults: new { action = "Index", id = RouteParameter.Optional }
                );
            }
            catch (Exception ex)
            {
                Program.Host.LogException(new InvalidOperationException($"Failed to initialize instance API controllers: {ex.Message}", ex));
            }

            // Map OSI-PI Grafana controller
            try
            {
                httpConfig.Routes.MapHttpRoute(
                    name: "OsiPiGrafana",
                    routeTemplate: "api/pigrafana/{instanceName}/{serverName}/{action}",
                    defaults: new { action = "Index", controller = "OsiPiGrafana" }
                );
            }
            catch (Exception ex)
            {
                Program.Host.LogStatusMessage($"WARNING: Failed to initialize OSI-PI Grafana controller routes {ex.Message}", UpdateType.Warning);
            }

            // Map eDNA Grafana controller
            try
            {
                httpConfig.Routes.MapHttpRoute(
                    name: "eDNAGrafana",
                    routeTemplate: "api/ednagrafana/{site}/{service}/{action}",
                    defaults: new { action = "Index", controller = "eDNAGrafana" }
                    );
            }
            catch (Exception ex)
            {
                Program.Host.LogStatusMessage($"WARNING: Failed to initialize eDNA Grafana controller routes: {ex.Message}", UpdateType.Warning);
            }

            // Map custom API controllers
            try
            {
                httpConfig.Routes.MapHttpRoute(
                    name: "CustomAPIs",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { action = "Index", id = RouteParameter.Optional }
                );
            }
            catch (Exception ex)
            {
                Program.Host.LogException(new InvalidOperationException($"Failed to initialize custom API controllers: {ex.Message}", ex));
            }

            // Map Grafana authenticated proxy controller
            try
            {
                httpConfig.Routes.MapHttpRoute(
                    name: "GrafanaAuthProxy",
                    routeTemplate: "grafana/{*url}",
                    defaults: new { controller = "GrafanaAuthProxy", url = RouteParameter.Optional }
                    );
            }
            catch (Exception ex)
            {
                Program.Host.LogException(new InvalidOperationException($"Failed to initialize Grafana authenticated proxy controller: {ex.Message}", ex));
            }

            // Set configuration to use reflection to setup routes
            httpConfig.MapHttpAttributeRoutes();

            // Load the WebPageController class and assign its routes
            app.UseWebApi(httpConfig);

            // Setup resolver for web page controller instances
            app.UseWebPageController(WebServer.Default, Program.Host.DefaultWebPage, model, typeof(AppModel), AuthenticationOptions);

            // Check for configuration issues before first request
            httpConfig.EnsureInitialized();
        }

        private void Load_ModbusAssembly()
        {
            try
            {
                // Wrap class reference in lambda function to force
                // assembly load errors to occur within the try-catch
                new Action(() =>
                {
                    // Make embedded resources of Modbus poller available to web server
                    using (ModbusPoller poller = new ModbusPoller())
                        WebExtensions.AddEmbeddedResourceAssembly(poller.GetType().Assembly);

                    ModbusPoller.RestoreConfigurations(FilePath.GetAbsolutePath("ModbusConfigs"));
                })();
            }
            catch (Exception ex)
            {
                Program.Host.LogException(new InvalidOperationException($"Failed to load Modbus assembly: {ex.Message}", ex));
            }
        }

        private void Load_OSIPIGrafanaController()
        {
            // Load external OSIPIGrafanaController so route map can find it
            try
            {
                // Wrap class reference in lambda function to force
                // assembly load errors to occur within the try-catch
                new Action(() =>
                {
                    using (new OSIPIGrafanaController.OSIPIGrafanaController()) { }
                })();
            }
            catch (Exception ex)
            {
                Program.Host.LogStatusMessage($"WARNING: Failed to load OSI-PI Grafana controller, validate OSI-PI AFSDK is installed: {ex.Message}", UpdateType.Warning);
            }
        }

        private void Load_eDNAGrafanaController()
        {
            // Load external eDNAGrafanaController so route map can find it
            try
            {
                // Wrap class reference in lambda function to force
                // assembly load errors to occur within the try-catch
                new Action(() =>
                {
                    using (new eDNAGrafanaController.eDNAGrafanaController()) { }
                })();
            }
            catch (Exception ex)
            {
                Program.Host.LogStatusMessage($"WARNING: Failed to load eDNA Grafana controller, validate eDNA DLL's exists in program directory: {ex.Message}", UpdateType.Warning);
            }
        }

        // Static Properties

        /// <summary>
        /// Gets the authentication options used for the hosted web server.
        /// </summary>
        public static AuthenticationOptions AuthenticationOptions { get; } = new AuthenticationOptions();

        #region [ Old Code ]

        //Dictionary<string, string> replacements = new Dictionary<string, string>() { { "{Namespace}", "openHistorian" } };

        //// Extract and update local Modbus configuration screens
        //string webRootPath = FilePath.GetAbsolutePath(FilePath.AddPathSuffix(Program.Host.Model.Global.WebRootPath));
        //            ExtractTextResource("ModbusAdapters.ModbusConfig.cshtml", $"{webRootPath}ModbusConfig.cshtml", replacements);
        //            ExtractTextResource("ModbusAdapters.Status.cshtml", $"{webRootPath}Status.cshtml", replacements);

        //private static void ExtractTextResource(string resourceName, string fileName, IEnumerable<KeyValuePair<string, string>> replacements)
        //{
        //    Stream stream = WebExtensions.OpenEmbeddedResourceStream(resourceName);

        //    if ((object)stream != null)
        //    {
        //        using (StreamReader reader = new StreamReader(stream))
        //        {
        //            string resourceData = reader.ReadToEnd();

        //            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
        //            {
        //                foreach (KeyValuePair<string, string> replacement in replacements)
        //                    resourceData = resourceData.Replace(replacement.Key, replacement.Value);

        //                writer.Write(resourceData);
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}
