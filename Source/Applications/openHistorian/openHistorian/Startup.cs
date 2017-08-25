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
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Security;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using GSF.Web;
using GSF.Web.Hosting;
using GSF.Web.Security;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Owin;
using ModbusAdapters;
using openHistorian.Model;

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

            // Load security hub into application domain before establishing SignalR hub configuration
            try
            {
                using (new SecurityHub()) { }
            }
            catch (Exception ex)
            {
                Program.Host.LogException(new SecurityException($"Failed to load Security Hub, validate database connection string in configuration file: {ex.Message}", ex));
            }

            // Load Modbus assembly
            try
            {
                // Make embedded resources of Modbus poller available to web server
                using (ModbusPoller poller = new ModbusPoller())
                    WebExtensions.AddEmbeddedResourceAssembly(poller.GetType().Assembly);
            }
            catch (Exception ex)
            {
                Program.Host.LogException(new InvalidOperationException($"Failed to load Modbus assembly: {ex.Message}", ex));
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

            // Setup session handling for API controller instances
            httpConfig.MessageHandlers.Add(new SessionHandler(SessionToken));

            // Enabled detailed client errors
            hubConfig.EnableDetailedErrors = true;

            // Enable cross-domain scripting
            app.UseCors(CorsOptions.AllowAll);

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

            // Set configuration to use reflection to setup routes
            httpConfig.MapHttpAttributeRoutes();

            // Load the WebPageController class and assign its routes
            app.UseWebApi(httpConfig);

            // Check for configuration issues before first request
            httpConfig.EnsureInitialized();
        }

        // Static Fields
        private static string[] s_anonymousResources;
        private static readonly ConcurrentDictionary<string, bool> s_anonymousResourceCache;

        // Static Constructor
        static Startup()
        {
            s_anonymousResourceCache = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        }

        // Static Properties
    
        /// <summary>
        /// Gets the authentication schemes to use for clients accessing the hosted web server.
        /// </summary>
        public static AuthenticationSchemes AuthenticationSchemes { get; internal set; }

        /// <summary>
        /// Gets web resources that should be allowed anonymous access.
        /// </summary>
        public static string[] AnonymousResources
        {
            get
            {
                return s_anonymousResources;
            }
            internal set
            {
                s_anonymousResources = value;
                s_anonymousResourceCache.Clear();
            }
        }

        /// <summary>
        /// Gets or sets the token used for identifying the session ID in cookie headers.
        /// </summary>
        public static string SessionToken { get; internal set; }

        // Static Methods
        private static AuthenticationSchemes AuthenticationSchemeForClient(HttpListenerRequest request)
        {
            string urlPath = request.Url.PathAndQuery;

            if (s_anonymousResourceCache?.GetOrAdd(urlPath, path => path == "/" || AnonymousResources.Any(resource => path.StartsWith(resource, StringComparison.OrdinalIgnoreCase))) ?? false)
                return AuthenticationSchemes.Anonymous;

            return AuthenticationSchemes;
        }

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
