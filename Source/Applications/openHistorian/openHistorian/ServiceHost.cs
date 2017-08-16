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
using System.Reflection;
using System.Threading;
using GSF;
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
using GSF.ComponentModel;

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
        private Thread m_startEngineThread;
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

            // Register a symbolic reference to global settings for use by default value expressions
            ValueExpressionParser.DefaultTypeRegistry.RegisterSymbol("Global", Program.Host.Model.Global);

            ServiceHelper.UpdatedStatus += UpdatedStatusHandler;
            ServiceHelper.LoggedException += LoggedExceptionHandler;

            m_startEngineThread = new Thread(() =>
            {
                const int RetryDelay = 1000;
                const int SleepTime = 200;
                const int LoopCount = RetryDelay / SleepTime;

                bool webUIStarted = false;

                while (true)
                {
                    webUIStarted = webUIStarted || TryStartWebUI();

                    if (webUIStarted)
                        break;

                    for (int i = 0; i < LoopCount; i++)
                    {
                        if (m_serviceStopping)
                            return;

                        Thread.Sleep(SleepTime);
                    }
                }
            });

            m_startEngineThread.Start();

        }

        private bool TryStartWebUI()
        {
            CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];

            try
            {
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

                // Create new web application hosting environment
                m_webAppHost = WebApp.Start<Startup>(systemSettings["WebHostURL"].Value);

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

            base.ServiceStoppingHandler(sender, e);

            ServiceHelper.UpdatedStatus -= UpdatedStatusHandler;
            ServiceHelper.LoggedException -= LoggedExceptionHandler;

            m_startEngineThread.Join();
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

                SecurityProviderCache.ValidateCurrentProvider();

                if (SecurityProviderUtility.IsResourceSecurable(request.Command) && !SecurityProviderUtility.IsResourceAccessible(request.Command))
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