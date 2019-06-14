//******************************************************************************************************
//  ServiceHost.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
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
//  ----------------------------------------------------------------------------------------------------
//  09/02/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF;
using GSF.ComponentModel;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Security;
using GSF.Security.Model;
using GSF.ServiceProcess;
using GSF.TimeSeries;
using GSF.Web.Hosting;
using GSF.Web.Model;
using GSF.Web.Model.Handlers;
using GSF.Web.Security;
using GSF.Web.Shared;
using GSF.Web.Shared.Model;
using Microsoft.Owin.Hosting;
using openHistorian.Adapters;
using openHistorian.Model;
using openHistorian.Snap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Threading;

namespace openHistorian
{
    public class ServiceHost : ServiceHostBase
    {
        #region [ Members ]

        // Constants
        private const int DefaultMaximumDiagnosticLogSize = 10;

        // Events

        /// <summary>
        /// Raised when there is a new status message reported to service.
        /// </summary>
        public event EventHandler<EventArgs<Guid, string, UpdateType>> UpdatedStatus;

        /// <summary>
        /// Raised when there is a new exception logged to service.
        /// </summary>
        public event EventHandler<EventArgs<Exception>> LoggedException;

        // Fields
        private IDisposable m_webAppHost;
        private bool m_serviceStopping;
        private readonly LogSubscriber m_logSubscriber;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="ServiceHost"/> instance.
        /// </summary>
        public ServiceHost()
        {
            ServiceName = "openHistorian";

            m_logSubscriber = Logger.CreateSubscriber();
            m_logSubscriber.SubscribeToAssembly(typeof(Number).Assembly, VerboseLevel.High);
            m_logSubscriber.SubscribeToAssembly(typeof(HistorianKey).Assembly, VerboseLevel.High);
            m_logSubscriber.NewLogMessage += m_logSubscriber_Log;

            // This function needs to be called before establishing time-series IaonSession
            SetupGrafanaHostingAdapter();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the configured default web page for the application.
        /// </summary>
        public string DefaultWebPage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the model used for the application.
        /// </summary>
        public AppModel Model
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets current performance statistics.
        /// </summary>
        public string PerformanceStatistics => ServiceHelper?.PerformanceMonitor?.Status;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ServiceHost"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        m_webAppHost?.Dispose();
                        m_logSubscriber?.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Event handler for service starting operations.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments containing command line arguments passed into service at startup.</param>
        protected override void ServiceStartingHandler(object sender, EventArgs<string[]> e)
        {
            // Handle base class service starting procedures
            base.ServiceStartingHandler(sender, e);

            // Make sure openHistorian specific default service settings exist
            CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];
            CategorizedSettingsElementCollection securityProvider = ConfigurationFile.Current.Settings["securityProvider"];
            CategorizedSettingsElementCollection grafanaHosting = ConfigurationFile.Current.Settings["grafanaHosting"];

            // Define set of default anonymous web resources for this site
            const string DefaultAnonymousResourceExpression = "^/@|^/Scripts/|^/Content/|^/Images/|^/fonts/|^/favicon.ico$";
            const string DefaultAuthFailureRedirectResourceExpression = AuthenticationOptions.DefaultAuthFailureRedirectResourceExpression + "|^/grafana(?!/api/).*$";

            systemSettings.Add("CompanyName", "Grid Protection Alliance", "The name of the company who owns this instance of the openHistorian.");
            systemSettings.Add("CompanyAcronym", "GPA", "The acronym representing the company who owns this instance of the openHistorian.");
            systemSettings.Add("DiagnosticLogPath", FilePath.GetAbsolutePath(""), "Path for diagnostic logs.");
            systemSettings.Add("MaximumDiagnosticLogSize", DefaultMaximumDiagnosticLogSize, "The combined maximum size for the diagnostic logs in whole Megabytes; curtailment happens hourly. Set to zero for no limit.");
            systemSettings.Add("WebHostURL", "http://+:8180", "The web hosting URL for remote system management.");
            systemSettings.Add("WebRootPath", "wwwroot", "The root path for the hosted web server files. Location will be relative to install folder if full path is not specified.");
            systemSettings.Add("DefaultWebPage", "Index.cshtml", "The default web page for the hosted web server.");
            systemSettings.Add("DateFormat", "MM/dd/yyyy", "The default date format to use when rendering timestamps.");
            systemSettings.Add("TimeFormat", "HH:mm:ss.fff", "The default time format to use when rendering timestamps.");
            systemSettings.Add("BootstrapTheme", "Content/bootstrap.min.css", "Path to Bootstrap CSS to use for rendering styles.");
            systemSettings.Add("SubscriptionConnectionString", "server=localhost:6175; interface=0.0.0.0", "Connection string for data subscriptions to openHistorian server.");
            systemSettings.Add("AuthenticationSchemes", AuthenticationOptions.DefaultAuthenticationSchemes, "Comma separated list of authentication schemes to use for clients accessing the hosted web server, e.g., Basic or NTLM.");
            systemSettings.Add("AuthFailureRedirectResourceExpression", DefaultAuthFailureRedirectResourceExpression, "Expression that will match paths for the resources on the web server that should redirect to the LoginPage when authentication fails.");
            systemSettings.Add("AnonymousResourceExpression", DefaultAnonymousResourceExpression, "Expression that will match paths for the resources on the web server that can be provided without checking credentials.");
            systemSettings.Add("AuthenticationToken", SessionHandler.DefaultAuthenticationToken, "Defines the token used for identifying the authentication token in cookie headers.");
            systemSettings.Add("SessionToken", SessionHandler.DefaultSessionToken, "Defines the token used for identifying the session ID in cookie headers.");
            systemSettings.Add("RequestVerificationToken", AuthenticationOptions.DefaultRequestVerificationToken, "Defines the token used for anti-forgery verification in HTTP request headers.");
            systemSettings.Add("LoginPage", AuthenticationOptions.DefaultLoginPage, "Defines the login page used for redirects on authentication failure. Expects forward slash prefix.");
            systemSettings.Add("AuthTestPage", AuthenticationOptions.DefaultAuthTestPage, "Defines the page name for the web server to test if a user is authenticated. Expects forward slash prefix.");
            systemSettings.Add("Realm", "", "Case-sensitive identifier that defines the protection space for the web based authentication and is used to indicate a scope of protection.");
            systemSettings.Add("DefaultCorsOrigins", "", "Comma-separated list of allowed origins (including http:// prefix) that define the default CORS policy. Use '*' to allow all or empty string to disable CORS.");
            systemSettings.Add("DefaultCorsHeaders", "*", "Comma-separated list of supported headers that define the default CORS policy. Use '*' to allow all or empty string to allow none.");
            systemSettings.Add("DefaultCorsMethods", "*", "Comma-separated list of supported methods that define the default CORS policy. Use '*' to allow all or empty string to allow none.");
            systemSettings.Add("DefaultCorsSupportsCredentials", true, "Boolean flag for the default CORS policy indicating whether the resource supports user credentials in the request.");
            systemSettings.Add("NominalFrequency", 60, "Defines the nominal system frequency for this instance of the openHistorian");
            systemSettings.Add("DefaultCalculationLagTime", 6.0, "Defines the default lag-time value, in seconds, for template calculations");
            systemSettings.Add("DefaultCalculationLeadTime", 3.0, "Defines the default lead-time value, in seconds, for template calculations");
            systemSettings.Add("DefaultCalculationFramesPerSecond", 30, "Defines the default frames-per-second value for template calculations");

            DefaultWebPage = systemSettings["DefaultWebPage"].Value;

            Model = new AppModel();
            Model.Global.CompanyName = systemSettings["CompanyName"].Value;
            Model.Global.CompanyAcronym = systemSettings["CompanyAcronym"].Value;
            Model.Global.NodeID = Guid.Parse(systemSettings["NodeID"].Value);
            Model.Global.SubscriptionConnectionString = systemSettings["SubscriptionConnectionString"].Value;
            Model.Global.ApplicationName = "openHistorian";
            Model.Global.ApplicationDescription = "open Historian System";
            Model.Global.ApplicationKeywords = "open source, utility, software, time-series, archive";
            Model.Global.DateFormat = systemSettings["DateFormat"].Value;
            Model.Global.TimeFormat = systemSettings["TimeFormat"].Value;
            Model.Global.DateTimeFormat = $"{Model.Global.DateFormat} {Model.Global.TimeFormat}";
            Model.Global.PasswordRequirementsRegex = securityProvider["PasswordRequirementsRegex"].Value;
            Model.Global.PasswordRequirementsError = securityProvider["PasswordRequirementsError"].Value;
            Model.Global.BootstrapTheme = systemSettings["BootstrapTheme"].Value;
            Model.Global.WebRootPath = FilePath.GetAbsolutePath(systemSettings["WebRootPath"].Value);
            Model.Global.GrafanaServerPath = grafanaHosting["ServerPath"].Value;
            Model.Global.GrafanaServerInstalled = File.Exists(Model.Global.GrafanaServerPath);
            Model.Global.DefaultCorsOrigins = systemSettings["DefaultCorsOrigins"].Value;
            Model.Global.DefaultCorsHeaders = systemSettings["DefaultCorsHeaders"].Value;
            Model.Global.DefaultCorsMethods = systemSettings["DefaultCorsMethods"].Value;
            Model.Global.DefaultCorsSupportsCredentials = systemSettings["DefaultCorsSupportsCredentials"].ValueAsBoolean(true);
            Model.Global.NominalFrequency = systemSettings["NominalFrequency"].ValueAsInt32(60);
            Model.Global.DefaultCalculationLagTime = systemSettings["DefaultCalculationLagTime"].ValueAsDouble(6.0);
            Model.Global.DefaultCalculationLeadTime = systemSettings["DefaultCalculationLeadTime"].ValueAsDouble(3.0);
            Model.Global.DefaultCalculationFramesPerSecond = systemSettings["DefaultCalculationFramesPerSecond"].ValueAsInt32(30);

            // Register a symbolic reference to global settings for use by default value expressions
            ValueExpressionParser.DefaultTypeRegistry.RegisterSymbol("Global", Model.Global);

            // Parse configured authentication schemes
            if (!Enum.TryParse(systemSettings["AuthenticationSchemes"].ValueAs(AuthenticationOptions.DefaultAuthenticationSchemes.ToString()), true, out AuthenticationSchemes authenticationSchemes))
                authenticationSchemes = AuthenticationOptions.DefaultAuthenticationSchemes;

            // Initialize web startup configuration
            Startup.AuthenticationOptions.AuthenticationSchemes = authenticationSchemes;
            Startup.AuthenticationOptions.AuthFailureRedirectResourceExpression = systemSettings["AuthFailureRedirectResourceExpression"].ValueAs(DefaultAuthFailureRedirectResourceExpression);
            Startup.AuthenticationOptions.AnonymousResourceExpression = systemSettings["AnonymousResourceExpression"].ValueAs(DefaultAnonymousResourceExpression);
            Startup.AuthenticationOptions.AuthenticationToken = systemSettings["AuthenticationToken"].ValueAs(SessionHandler.DefaultAuthenticationToken);
            Startup.AuthenticationOptions.SessionToken = systemSettings["SessionToken"].ValueAs(SessionHandler.DefaultSessionToken);
            Startup.AuthenticationOptions.RequestVerificationToken = systemSettings["RequestVerificationToken"].ValueAs(AuthenticationOptions.DefaultRequestVerificationToken);
            Startup.AuthenticationOptions.LoginPage = systemSettings["LoginPage"].ValueAs(AuthenticationOptions.DefaultLoginPage);
            Startup.AuthenticationOptions.AuthTestPage = systemSettings["AuthTestPage"].ValueAs(AuthenticationOptions.DefaultAuthTestPage);
            Startup.AuthenticationOptions.Realm = systemSettings["Realm"].ValueAs("");
            Startup.AuthenticationOptions.LoginHeader = $"<h3><img src=\"/Images/{Model.Global.ApplicationName}.png\"/> {Model.Global.ApplicationName}</h3>";

            // Validate that configured authentication test page does not evaluate as an anonymous resource nor a authentication failure redirection resource
            string authTestPage = Startup.AuthenticationOptions.AuthTestPage;

            if (Startup.AuthenticationOptions.IsAnonymousResource(authTestPage))
                throw new SecurityException($"The configured authentication test page \"{authTestPage}\" evaluates as an anonymous resource. Modify \"AnonymousResourceExpression\" setting so that authorization test page is not a match.");

            if (Startup.AuthenticationOptions.IsAuthFailureRedirectResource(authTestPage))
                throw new SecurityException($"The configured authentication test page \"{authTestPage}\" evaluates as an authentication failure redirection resource. Modify \"AuthFailureRedirectResourceExpression\" setting so that authorization test page is not a match.");

            if (Startup.AuthenticationOptions.AuthenticationToken == Startup.AuthenticationOptions.SessionToken)
                throw new InvalidOperationException("Authentication token must be different from session token in order to differentiate the cookie values in the HTTP headers.");

            ServiceHelper.UpdatedStatus += UpdatedStatusHandler;
            ServiceHelper.LoggedException += LoggedExceptionHandler;
            GrafanaAuthProxyController.StatusMessage += GrafanaAuthProxyController_StatusMessage;

            // Define exception logger for CSV downloader
            CsvDownloadHandler.LogExceptionHandler = LogException;

            Thread startWebServer = new Thread(() =>
            {
                // Attach to default web server events
                WebServer webServer = WebServer.Default;
                webServer.StatusMessage += WebServer_StatusMessage;
                webServer.ExecutionException += LoggedExceptionHandler;

                // Define types for Razor pages - self-hosted web service does not use view controllers so
                // we must define configuration types for all paged view model based Razor views here:
                webServer.PagedViewModelTypes.TryAdd("TrendMeasurements.cshtml", new Tuple<Type, Type>(typeof(ActiveMeasurement), typeof(DataHub)));
                webServer.PagedViewModelTypes.TryAdd("Devices.cshtml", new Tuple<Type, Type>(typeof(Device), typeof(DataHub)));
                webServer.PagedViewModelTypes.TryAdd("Companies.cshtml", new Tuple<Type, Type>(typeof(Company), typeof(SharedHub)));
                webServer.PagedViewModelTypes.TryAdd("Vendors.cshtml", new Tuple<Type, Type>(typeof(Vendor), typeof(SharedHub)));
                webServer.PagedViewModelTypes.TryAdd("VendorDevices.cshtml", new Tuple<Type, Type>(typeof(VendorDevice), typeof(SharedHub)));
                webServer.PagedViewModelTypes.TryAdd("Users.cshtml", new Tuple<Type, Type>(typeof(UserAccount), typeof(SecurityHub)));
                webServer.PagedViewModelTypes.TryAdd("Groups.cshtml", new Tuple<Type, Type>(typeof(SecurityGroup), typeof(SecurityHub)));
            })
            {
                IsBackground = true
            };

            startWebServer.Start();

            new Thread(() =>
            {
                const int RetryDelay = 1000;
                const int SleepTime = 200;
                const int LoopCount = RetryDelay / SleepTime;

                startWebServer.Join(5000);

                while (!m_serviceStopping)
                {
                    if (TryStartWebHosting(systemSettings["WebHostURL"].Value))
                    {
                        try
                        {
                            // Initiate pre-compile of base templates
                            RazorEngine<CSharpEmbeddedResource>.Default.PreCompile(LogException);
                            RazorEngine<CSharpEmbeddedResource>.Default.PreCompile(LogException, "GSF.Web.Security.Views.");
                            RazorEngine<CSharpEmbeddedResource>.Default.PreCompile(LogException, "GSF.Web.Shared.Views.");
                            RazorEngine<CSharp>.Default.PreCompile(LogException);
                        }
                        catch (Exception ex)
                        {
                            LogException(new InvalidOperationException($"Failed to initiate pre-compile of razor templates: {ex.Message}", ex));
                        }

                        break;
                    }

                    for (int i = 0; i < LoopCount && !m_serviceStopping; i++)
                        Thread.Sleep(SleepTime);
                }
            })
            {
                IsBackground = true
            }
            .Start();
        }

        /// <summary>Event handler for service started operation.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// Time-series framework uses this handler to handle initialization of system objects.
        /// </remarks>
        protected override void ServiceStartedHandler(object sender, EventArgs e)
        {
            base.ServiceStartedHandler(sender, e);

            ServiceHelper.ClientRequestHandlers.Add(new ClientRequestHandler("eDNARefreshMetadata", "Refreshes eDNA metadata.", RefreshMetaDataHandler, new[] { "eDNARefresh", "RefresheDNAMetadata" }));

            if (!Model.Global.GrafanaServerInstalled)
                return;

            // Kick off a thread to monitor for when Grafana server has been properly
            // initialized so that initial user synchronization process can proceed
            new Thread(() =>
            {
                try
                {
                    const int DefaultInitializationTimeout = GrafanaAuthProxyController.DefaultInitializationTimeout;

                    // Access settings from "systemSettings" category in configuration file
                    CategorizedSettingsElementCollection grafanaHosting = ConfigurationFile.Current.Settings["grafanaHosting"];

                    // Make sure needed settings exist
                    grafanaHosting.Add("InitializationTimeout", DefaultInitializationTimeout, "Defines the timeout, in seconds, for the Grafana system to initialize.");

                    // Get settings as currently defined in configuration file
                    int initializationTimeout = grafanaHosting["InitializationTimeout"].ValueAs(DefaultInitializationTimeout);
                    DateTime startTime = DateTime.UtcNow;

                #if DEBUG                    
                    // Debugging adds run-time overhead, provide more time for initialization
                    initializationTimeout *= 3;                    
                #endif

                    // Give initialization - which includes starting Grafana server process - a chance to start
                    while (!GrafanaAuthProxyController.ServerIsResponding())
                    {
                        // Stop attempts after timeout has expired
                        if ((DateTime.UtcNow - startTime).TotalSeconds >= initializationTimeout)
                            break;

                        Thread.Sleep(500);
                    }
                }
                catch (Exception ex)
                {
                    LogException(new InvalidOperationException($"Failed while checking for Grafana server initialization: {ex.Message}", ex));
                }
                finally
                {
                    GrafanaAuthProxyController.InitializationComplete();
                }
            })
            {
                IsBackground = true
            }
            .Start();
        }

        private void RefreshMetaDataHandler(ClientRequestInfo requestInfo) => eDNAGrafanaController.eDNAGrafanaController.RefreshAllMetaData();

        private bool TryStartWebHosting(string webHostURL)
        {
            try
            {
                // Create new web application hosting environment
                m_webAppHost = WebApp.Start<Startup>(webHostURL);
                return true;
            }
            catch (TargetInvocationException ex)
            {
                LogException(new InvalidOperationException($"Failed to initialize web hosting: {ex.InnerException?.Message ?? ex.Message}", ex.InnerException ?? ex));
                return false;
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Failed to initialize web hosting: {ex.Message}", ex));
                return false;
            }
        }

        protected override void ServiceStoppingHandler(object sender, EventArgs e)
        {
            m_serviceStopping = true;

            ServiceHelper helper = ServiceHelper;

            try
            {
                base.ServiceStoppingHandler(sender, e);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Service stopping handler exception: {ex.Message}", ex));
            }

            if ((object)helper != null)
            {
                helper.UpdatedStatus -= UpdatedStatusHandler;
                helper.LoggedException -= LoggedExceptionHandler;
            }
        }

        private void GrafanaAuthProxyController_StatusMessage(object sender, EventArgs<string> e)
        {
            LogStatusMessage($"[GRAFANA!AUTHPROXY] {e.Argument}");
        }

        private void WebServer_StatusMessage(object sender, EventArgs<string> e)
        {
            LogWebHostStatusMessage(e.Argument);
        }

        public void LogWebHostStatusMessage(string message, UpdateType type = UpdateType.Information)
        {
            LogStatusMessage($"[WEBHOST] {message}", type);
        }

        /// <summary>
        /// Logs a status message to connected clients.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="type">Type of message to log.</param>
        public void LogStatusMessage(string message, UpdateType type = UpdateType.Information)
        {
            DisplayStatusMessage(message, type);
        }

        /// <summary>
        /// Logs an exception to the service.
        /// </summary>
        /// <param name="ex">Exception to log.</param>
        public new void LogException(Exception ex)
        {
            base.LogException(ex);
            DisplayStatusMessage($"{ex.Message}", UpdateType.Alarm);
        }

        /// <summary>
        /// Sends a command request to the service.
        /// </summary>
        /// <param name="clientID">Client ID of sender.</param>
        /// <param name="principal">The principal used for role-based security.</param>
        /// <param name="userInput">Request string.</param>
        public void SendRequest(Guid clientID, IPrincipal principal, string userInput)
        {
            ClientRequest request = ClientRequest.Parse(userInput);

            if ((object)request == null)
                return;

            if (SecurityProviderUtility.IsResourceSecurable(request.Command) && !SecurityProviderUtility.IsResourceAccessible(request.Command, principal))
            {
                ServiceHelper.UpdateStatus(clientID, UpdateType.Alarm, $"Access to \"{request.Command}\" is denied.\r\n\r\n");
                return;
            }

            ClientRequestHandler requestHandler = ServiceHelper.FindClientRequestHandler(request.Command);

            if ((object)requestHandler == null)
            {
                ServiceHelper.UpdateStatus(clientID, UpdateType.Alarm, $"Command \"{request.Command}\" is not supported.\r\n\r\n");
                return;
            }

            ClientInfo clientInfo = new ClientInfo();
            clientInfo.ClientID = clientID;
            clientInfo.SetClientUser(principal);

            ClientRequestInfo requestInfo = new ClientRequestInfo(clientInfo, request);
            requestHandler.HandlerMethod(requestInfo);
        }

        public void DisconnectClient(Guid clientID)
        {
            ServiceHelper.DisconnectClient(clientID);
        }

        private void UpdatedStatusHandler(object sender, EventArgs<Guid, string, UpdateType> e)
        {
            if ((object)UpdatedStatus != null)
                UpdatedStatus(sender, new EventArgs<Guid, string, UpdateType>(e.Argument1, e.Argument2, e.Argument3));
        }

        private void LoggedExceptionHandler(object sender, EventArgs<Exception> e)
        {
            if ((object)LoggedException != null)
                LoggedException(sender, new EventArgs<Exception>(e.Argument));
        }

        private void m_logSubscriber_Log(LogMessage logMessage)
        {
            switch (logMessage.Level)
            {
                case MessageLevel.Critical:
                case MessageLevel.Error:
                    ServiceHelper?.ErrorLogger?.Log(logMessage.Exception ?? new InvalidOperationException(logMessage.GetMessage()));
                    break;
                case MessageLevel.Warning:
                    if (!string.IsNullOrWhiteSpace(logMessage.Message))
                        DisplayStatusMessage($"[SNAPENGINE] WARNING: {logMessage.Message}", UpdateType.Warning, false);
                    break;
                case MessageLevel.Debug:
                    break;
                default:
                    if (!string.IsNullOrWhiteSpace(logMessage.Message))
                        DisplayStatusMessage($"[SNAPENGINE] {logMessage.Message}", UpdateType.Information, false);
                    break;
            }
        }

        private void SetupGrafanaHostingAdapter()
        {
            try
            {
                const string GrafanaProcessAdapterName = "GRAFANA!PROCESS";
                const string DefaultGrafanaServerPath = GrafanaAuthProxyController.DefaultServerPath;

                const string GrafanaAdminRoleName = GrafanaAuthProxyController.GrafanaAdminRoleName;
                const string GrafanaAdminRoleDescription = "Grafana Administrator Role";

                // Access needed settings from specified categories in configuration file
                CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];
                CategorizedSettingsElementCollection grafanaHosting = ConfigurationFile.Current.Settings["grafanaHosting"];
                string newNodeID = Guid.NewGuid().ToString();

                // Make sure needed settings exist
                systemSettings.Add("NodeID", newNodeID, "Unique Node ID");
                grafanaHosting.Add("ServerPath", DefaultGrafanaServerPath, "Defines the path to the Grafana server to host - set to empty string to disable hosting.");

                // Get settings as currently defined in configuration file
                Guid nodeID = Guid.Parse(systemSettings["NodeID"].ValueAs(newNodeID));
                string grafanaServerPath = grafanaHosting["ServerPath"].ValueAs(DefaultGrafanaServerPath);

                // Only enable adapter if file path to configured Grafana server executable is accessible
                bool enabled = File.Exists(FilePath.GetAbsolutePath(grafanaServerPath));

                // Open database connection as defined in configuration file "systemSettings" category
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    // Make sure Grafana process adapter exists
                    TableOperations<CustomActionAdapter> actionAdapterTable = new TableOperations<CustomActionAdapter>(connection);
                    CustomActionAdapter actionAdapter = actionAdapterTable.QueryRecordWhere("AdapterName = {0}", GrafanaProcessAdapterName) ?? actionAdapterTable.NewRecord();

                    // Update record fields
                    actionAdapter.NodeID = nodeID;
                    actionAdapter.AdapterName = GrafanaProcessAdapterName;
                    actionAdapter.AssemblyName = "FileAdapters.dll";
                    actionAdapter.TypeName = "FileAdapters.ProcessLauncher";
                    actionAdapter.Enabled = enabled;

                    // Define default adapter connection string if none is defined
                    if (string.IsNullOrWhiteSpace(actionAdapter.ConnectionString))
                        actionAdapter.ConnectionString =
                            $"FileName={DefaultGrafanaServerPath}; " +
                            $"WorkingDirectory={FilePath.GetAbsolutePath("Grafana")}; " +
                            "ForceKillOnDispose=True; " +
                            "ProcessOutputAsLogMessages=True; " +
                            "LogMessageTextExpression={(?<=.*msg\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*file\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*file\\s*\\=\\s*)[^\\s]*(?=s|$)|(?<=.*path\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*path\\s*\\=\\s*)[^\\s]*(?=s|$)|(?<=.*error\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*reason\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*id\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*version\\s*\\=\\s*)[^\\s]*(?=\\s|$)|(?<=.*dbtype\\s*\\=\\s*)[^\\s]*(?=\\s|$)|(?<=.*)commit\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*)compiled\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*)address\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*)protocol\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*)subUrl\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*)code\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*name\\s*\\=\\s*)[^\\s]*(?=\\s|$)}; " +
                            "LogMessageLevelExpression={(?<=.*lvl\\s*\\=\\s*)[^\\s]*(?=\\s|$)}; " +
                            "LogMessageLevelMappings={info=Info; warn=Waning; error=Error; critical=Critical; debug=Debug}";

                    // Preserve connection string on existing records except for Grafana server executable path that comes from configuration file
                    Dictionary<string, string> settings = actionAdapter.ConnectionString.ParseKeyValuePairs();
                    settings["FileName"] = grafanaServerPath;
                    actionAdapter.ConnectionString = settings.JoinKeyValuePairs();

                    // Save record updates
                    actionAdapterTable.AddNewOrUpdateRecord(actionAdapter);

                    // Make sure Grafana admin role exists
                    TableOperations<ApplicationRole> applicationRoleTable = new TableOperations<ApplicationRole>(connection);
                    ApplicationRole applicationRole = applicationRoleTable.QueryRecordWhere("Name = {0} AND NodeID = {1}", GrafanaAdminRoleName, nodeID);

                    if ((object)applicationRole == null)
                    {
                        applicationRole = applicationRoleTable.NewRecord();
                        applicationRole.NodeID = nodeID;
                        applicationRole.Name = GrafanaAdminRoleName;
                        applicationRole.Description = GrafanaAdminRoleDescription;
                        applicationRoleTable.AddNewRecord(applicationRole);
                    }
                }
            }
            catch (Exception ex)
            {
                LogPublisher log = Logger.CreatePublisher(typeof(ServiceHost), MessageClass.Application);
                log.Publish(MessageLevel.Error, "Error Message", "Failed to setup Grafana hosting adapter", null, ex);
            }
        }

        #endregion
    }
}