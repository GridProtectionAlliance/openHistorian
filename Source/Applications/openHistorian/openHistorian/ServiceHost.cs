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

using System;
using System.Net;
using System.Reflection;
using System.Security;
using System.Threading;
using GSF;
using GSF.ComponentModel;
using GSF.Configuration;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Reflection;
using GSF.Security;
using GSF.Security.Model;
using GSF.ServiceProcess;
using GSF.TimeSeries;
using GSF.Web.Hosting;
using GSF.Web.Model;
using GSF.Web.Model.Handlers;
using GSF.Web.Security;
using Microsoft.Owin.Hosting;
using openHistorian.Model;
using openHistorian.Snap;

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

            // Define set of default anonymous web resources for this site
            const string DefaultAnonymousResourceExpression = "^/@|^/Scripts/|^/Content/|^/Images/|^/fonts/|^/api/(?!importedmeasurements)|^/instance/|^/favicon.ico$";

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
            systemSettings.Add("AuthFailureRedirectResourceExpression", AuthenticationOptions.DefaultAuthFailureRedirectResourceExpression, "Expression that will match paths for the resources on the web server that should redirect to the LoginPage when authentication fails.");
            systemSettings.Add("AnonymousResourceExpression", DefaultAnonymousResourceExpression, "Expression that will match paths for the resources on the web server that can be provided without checking credentials.");
            systemSettings.Add("PassThroughAuthSupportedBrowserExpression", AuthenticationOptions.DefaultPassThroughAuthSupportedBrowserExpression, "Expression that will match user-agent header string for browser clients that can support NTLM based pass-through authentication.");
            systemSettings.Add("AuthenticationToken", SessionHandler.DefaultAuthenticationToken, "Defines the token used for identifying the authentication token in cookie headers.");
            systemSettings.Add("SessionToken", SessionHandler.DefaultSessionToken, "Defines the token used for identifying the session ID in cookie headers.");
            systemSettings.Add("LoginPage", AuthenticationOptions.DefaultLoginPage, "Defines the login page used for redirects on authentication failure. Expects forward slash prefix.");
            systemSettings.Add("AuthTestPage", AuthenticationOptions.DefaultAuthTestPage, "Defines the page name for the web server to test if a user is authenticated. Expects forward slash prefix.");
            systemSettings.Add("Realm", "", "Case-sensitive identifier that defines the protection space for the web based authentication and is used to indicate a scope of protection.");

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
            Model.Global.WebRootPath = systemSettings["WebRootPath"].Value;

            AuthenticationSchemes authenticationSchemes;

            // Parse configured authentication schemes
            if (!Enum.TryParse(systemSettings["AuthenticationSchemes"].ValueAs(AuthenticationOptions.DefaultAuthenticationSchemes.ToString()), true, out authenticationSchemes))
                authenticationSchemes = AuthenticationOptions.DefaultAuthenticationSchemes;

            // Initialize web startup configuration
            Startup.AuthenticationOptions.AuthenticationSchemes = authenticationSchemes;
            Startup.AuthenticationOptions.AuthFailureRedirectResourceExpression = systemSettings["AuthFailureRedirectResourceExpression"].ValueAs(AuthenticationOptions.DefaultAuthFailureRedirectResourceExpression);
            Startup.AuthenticationOptions.AnonymousResourceExpression = systemSettings["AnonymousResourceExpression"].ValueAs(DefaultAnonymousResourceExpression);
            Startup.AuthenticationOptions.PassThroughAuthSupportedBrowserExpression = systemSettings["PassThroughAuthSupportedBrowserExpression"].ValueAs(AuthenticationOptions.DefaultPassThroughAuthSupportedBrowserExpression);
            Startup.AuthenticationOptions.AuthenticationToken = systemSettings["AuthenticationToken"].ValueAs(SessionHandler.DefaultAuthenticationToken);
            Startup.AuthenticationOptions.SessionToken = systemSettings["SessionToken"].ValueAs(SessionHandler.DefaultSessionToken);
            Startup.AuthenticationOptions.LoginPage = systemSettings["LoginPage"].ValueAs(AuthenticationOptions.DefaultLoginPage);
            Startup.AuthenticationOptions.AuthTestPage = systemSettings["AuthTestPage"].ValueAs(AuthenticationOptions.DefaultAuthTestPage);
            Startup.AuthenticationOptions.Realm = systemSettings["Realm"].ValueAs("");
            Startup.AuthenticationOptions.LoginHeader = $"<h2><img src=\"/Images/{Model.Global.ApplicationName}.png\"/> {Model.Global.ApplicationName}</h2>";

            // Validate that configured authentication test page does not evaluate as an anonymous resource nor a authentication failure redirection resource
            string authTestPage = Startup.AuthenticationOptions.AuthTestPage;

            if (Startup.AuthenticationOptions.IsAnonymousResource(authTestPage))
                throw new SecurityException($"The configured authentication test page \"{authTestPage}\" evaluates as an anonymous resource. Modify \"AnonymousResourceExpression\" setting so that authorization test page is not a match.");

            if (Startup.AuthenticationOptions.IsAuthFailureRedirectResource(authTestPage))
                throw new SecurityException($"The configured authentication test page \"{authTestPage}\" evaluates as an authentication failure redirection resource. Modify \"AuthFailureRedirectResourceExpression\" setting so that authorization test page is not a match.");

            if (Startup.AuthenticationOptions.AuthenticationToken == Startup.AuthenticationOptions.SessionToken)
                throw new InvalidOperationException("Authentication token must be different from session token in order to differentiate the cookie values in the HTTP headers.");

            // Register a symbolic reference to global settings for use by default value expressions
            ValueExpressionParser.DefaultTypeRegistry.RegisterSymbol("Global", Program.Host.Model.Global);

            ServiceHelper.UpdatedStatus += UpdatedStatusHandler;
            ServiceHelper.LoggedException += LoggedExceptionHandler;

            // Attach to default web server events
            WebServer webServer = WebServer.Default;
            webServer.StatusMessage += WebServer_StatusMessage;
            webServer.ExecutionException += LoggedExceptionHandler;

            // Define types for Razor pages - self-hosted web service does not use view controllers so
            // we must define configuration types for all paged view model based Razor views here:
            webServer.PagedViewModelTypes.TryAdd("TrendMeasurements.cshtml", new Tuple<Type, Type>(typeof(ActiveMeasurement), typeof(DataHub)));
            webServer.PagedViewModelTypes.TryAdd("Companies.cshtml", new Tuple<Type, Type>(typeof(Company), typeof(DataHub)));
            webServer.PagedViewModelTypes.TryAdd("Devices.cshtml", new Tuple<Type, Type>(typeof(Device), typeof(DataHub)));
            webServer.PagedViewModelTypes.TryAdd("Vendors.cshtml", new Tuple<Type, Type>(typeof(Vendor), typeof(DataHub)));
            webServer.PagedViewModelTypes.TryAdd("VendorDevices.cshtml", new Tuple<Type, Type>(typeof(VendorDevice), typeof(DataHub)));
            webServer.PagedViewModelTypes.TryAdd("Users.cshtml", new Tuple<Type, Type>(typeof(UserAccount), typeof(SecurityHub)));
            webServer.PagedViewModelTypes.TryAdd("Groups.cshtml", new Tuple<Type, Type>(typeof(SecurityGroup), typeof(SecurityHub)));

            // Define exception logger for CSV downloader
            CsvDownloadHandler.LogExceptionHandler = LogException;

            new Thread(() =>
            {
                const int RetryDelay = 1000;
                const int SleepTime = 200;
                const int LoopCount = RetryDelay / SleepTime;

                while (!m_serviceStopping)
                {
                    if (TryStartWebHosting(systemSettings["WebHostURL"].Value))
                    {
                        try
                        {
                            // Initiate pre-compile of base templates
                            if (AssemblyInfo.EntryAssembly.Debuggable)
                            {
                                RazorEngine<CSharpDebug>.Default.PreCompile(LogException);
                                RazorEngine<VisualBasicDebug>.Default.PreCompile(LogException);
                            }
                            else
                            {
                                RazorEngine<CSharp>.Default.PreCompile(LogException);
                                RazorEngine<VisualBasic>.Default.PreCompile(LogException);
                            }
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

        private void WebServer_StatusMessage(object sender, EventArgs<string> e)
        {
            LogWebHostStatusMessage(e.Argument);
        }

        protected override void ServiceStoppingHandler(object sender, EventArgs e)
        {
            m_serviceStopping = true;

            ServiceHelper helper = ServiceHelper;

            base.ServiceStoppingHandler(sender, e);

            if ((object)helper != null)
            {
                helper.UpdatedStatus -= UpdatedStatusHandler;
                helper.LoggedException -= LoggedExceptionHandler;
            }
        }

        public void LogWebHostStatusMessage(string message, UpdateType type = UpdateType.Information)
        {
            LogStatusMessage($"[WebHost] {message}", type);
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
        /// <param name="userInput">Request string.</param>
        public void SendRequest(Guid clientID, string userInput)
        {
            ClientRequest request = ClientRequest.Parse(userInput);

            if ((object)request != null)
            {
                ClientRequestHandler requestHandler = ServiceHelper.FindClientRequestHandler(request.Command);

                if (SecurityProviderUtility.IsResourceSecurable(request.Command) && !SecurityProviderUtility.IsResourceAccessible(request.Command, Thread.CurrentPrincipal))
                {
                    ServiceHelper.UpdateStatus(clientID, UpdateType.Alarm, $"Access to \"{request.Command}\" is denied.\r\n\r\n");
                    return;
                }

                if ((object)requestHandler != null)
                    requestHandler.HandlerMethod(new ClientRequestInfo(new ClientInfo { ClientID = clientID }, request));
                else
                    ServiceHelper.UpdateStatus(clientID, UpdateType.Alarm, $"Command \"{request.Command}\" is not supported.\r\n\r\n");
            }
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
                        DisplayStatusMessage($"[SnapEngine] WARNING: {logMessage.Message}", UpdateType.Warning, false);
                    break;
                case MessageLevel.Debug:
                    break;
                default:
                    if (!string.IsNullOrWhiteSpace(logMessage.Message))
                        DisplayStatusMessage($"[SnapEngine] {logMessage.Message}", UpdateType.Information, false);
                    break;
            }
        }

        #endregion
    }
}