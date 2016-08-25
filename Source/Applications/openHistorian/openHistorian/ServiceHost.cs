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
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using GSF;
using GSF.Configuration;
using GSF.IO;
using GSF.IO.Unmanaged;
using GSF.Security.Model;
using GSF.ServiceProcess;
using GSF.TimeSeries;
using GSF.Units;
using GSF.Web.Hosting;
using GSF.Web.Security;
using Microsoft.Owin.Hosting;
using openHistorian.Model;
using Measurement = openHistorian.Model.Measurement;
using Timer = System.Timers.Timer;

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
        private string m_diagnosticLogPath;
        private long m_maximumDiagnosticLogSize;
        private Timer m_logCurtailmentTimer;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="ServiceHost"/> instance.
        /// </summary>
        public ServiceHost()
        {
            ServiceName = "openHistorian";
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

                        if ((object)m_logCurtailmentTimer != null)
                        {
                            m_logCurtailmentTimer.Stop();
                            m_logCurtailmentTimer.Elapsed -= m_logCurtailmentTimer_Elapsed;
                            m_logCurtailmentTimer.Dispose();
                        }
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
            systemSettings.Add("MemoryPoolSize", "0.0", "The fixed memory pool size in Gigabytes. Leave at zero for dynamically calculated setting.");
            systemSettings.Add("MemoryPoolTargetUtilization", "Low", "The target utilization level for the memory pool. One of 'Low', 'Medium', or 'High'.");
            systemSettings.Add("DiagnosticLogPath", FilePath.GetAbsolutePath(""), "Path for diagnostic logs.");
            systemSettings.Add("MaximumDiagnosticLogSize", DefaultMaximumDiagnosticLogSize, "The combined maximum size for the diagnostic logs in whole Megabytes; curtailment happens hourly. Set to zero for no limit.");
            systemSettings.Add("WebHostURL", "http://localhost:8180", "The web hosting URL for remote system management.");
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

            ServiceHelper.UpdatedStatus += UpdatedStatusHandler;
            ServiceHelper.LoggedException += LoggedExceptionHandler;

            try
            {
                // Attach to default web server events
                WebServer webServer = WebServer.Default;
                webServer.StatusMessage += WebServer_StatusMessage;
                webServer.ExecutionException += LoggedExceptionHandler;

                // Define types for Razor pages - self-hosted web service does not use view controllers so
                // we must define configuration types for all paged view model based Razor views here:
                webServer.PagedViewModelTypes.TryAdd("TrendMeasurements.cshtml", new Tuple<Type, Type>(typeof(Measurement), typeof(DataHub)));
                webServer.PagedViewModelTypes.TryAdd("Companies.cshtml", new Tuple<Type, Type>(typeof(Company), typeof(DataHub)));
                webServer.PagedViewModelTypes.TryAdd("Vendors.cshtml", new Tuple<Type, Type>(typeof(Vendor), typeof(DataHub)));
                webServer.PagedViewModelTypes.TryAdd("VendorDevices.cshtml", new Tuple<Type, Type>(typeof(VendorDevice), typeof(DataHub)));
                webServer.PagedViewModelTypes.TryAdd("Users.cshtml", new Tuple<Type, Type>(typeof(UserAccount), typeof(SecurityHub)));
                webServer.PagedViewModelTypes.TryAdd("Groups.cshtml", new Tuple<Type, Type>(typeof(SecurityGroup), typeof(SecurityHub)));

                // TODO: Pre-compiling is interfering with Hub role authorizations - so skipping for now...
                //// Initiate pre-compile of base templates
                //if (AssemblyInfo.EntryAssembly.Debuggable)
                //{
                //    RazorEngine<CSharpDebug>.Default.PreCompile(LogException);
                //    RazorEngine<VisualBasicDebug>.Default.PreCompile(LogException);
                //}
                //else
                //{
                //    RazorEngine<CSharp>.Default.PreCompile(LogException);
                //    RazorEngine<VisualBasic>.Default.PreCompile(LogException);
                //}

                // Create new web application hosting environment
                m_webAppHost = WebApp.Start<Startup>(systemSettings["WebHostURL"].Value);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Failed to initialize web hosting: {ex.Message}", ex));
            }

            // Set maximum buffer size
            double memoryPoolSize = systemSettings["MemoryPoolSize"].ValueAs(0.0D);

            if (memoryPoolSize > 0.0D)
                Globals.MemoryPool.SetMaximumBufferSize((long)(memoryPoolSize * SI2.Giga));

            TargetUtilizationLevels targetLevel;

            if (!Enum.TryParse(systemSettings["MemoryPoolTargetUtilization"].Value, false, out targetLevel))
                targetLevel = TargetUtilizationLevels.High;

            Globals.MemoryPool.SetTargetUtilizationLevel(targetLevel);

            // Set default logging path
            m_diagnosticLogPath = systemSettings["DiagnosticLogPath"].Value;

            if (string.IsNullOrWhiteSpace(m_diagnosticLogPath) || !Directory.Exists(m_diagnosticLogPath))
                m_diagnosticLogPath = FilePath.GetAbsolutePath("");

            GSF.Diagnostics.Logger.SetLoggingPath(m_diagnosticLogPath);

            // Get maximum diagnostic log size
            m_maximumDiagnosticLogSize = SI2.Mega * systemSettings["MaximumDiagnosticLogSize"].ValueAs(DefaultMaximumDiagnosticLogSize);

            if (m_maximumDiagnosticLogSize > 0)
            {
                m_logCurtailmentTimer = new Timer(Time.SecondsPerHour * 1000.0D);
                m_logCurtailmentTimer.AutoReset = true;
                m_logCurtailmentTimer.Elapsed += m_logCurtailmentTimer_Elapsed;
                m_logCurtailmentTimer.Enabled = true;
            }
        }

        private void WebServer_StatusMessage(object sender, EventArgs<string> e)
        {
            DisplayStatusMessage(e.Argument, UpdateType.Information);
        }

        protected override void ServiceStoppingHandler(object sender, EventArgs e)
        {
            base.ServiceStoppingHandler(sender, e);

            ServiceHelper.UpdatedStatus -= UpdatedStatusHandler;
            ServiceHelper.LoggedException -= LoggedExceptionHandler;
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

                if ((object)requestHandler != null)
                    requestHandler.HandlerMethod(new ClientRequestInfo(new ClientInfo() { ClientID = clientID }, request));
                else
                    DisplayStatusMessage($"Command \"{request.Command}\" is not supported\r\n\r\n", UpdateType.Alarm);
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

        private void m_logCurtailmentTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(m_logCurtailmentTimer))
                return;

            try
            {
                long totalBytes = 0L;
                int curtailmentStartIndex = -1;

                FileInfo[] logFiles =
                    FilePath.GetFileList(Path.Combine(m_diagnosticLogPath, "*.logbin")).
                    OrderByDescending(file => Convert.ToInt64(FilePath.GetFileNameWithoutExtension(file))).
                    Select(file => new FileInfo(file)).
                    ToArray();

                for (int i = 1; i < logFiles.Length; i++)
                {
                    totalBytes += logFiles[i].Length;

                    if (totalBytes > m_maximumDiagnosticLogSize)
                    {
                        curtailmentStartIndex = i - 1;
                        break;
                    }
                }

                if (curtailmentStartIndex > -1)
                    for (int i = curtailmentStartIndex; i < logFiles.Length; i++)
                        logFiles[i].Delete();
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Failed to curtail diagnostic logs due to an exception: {ex.Message}", ex));
            }
            finally
            {
                Monitor.Exit(m_logCurtailmentTimer);
            }
        }

        #endregion
    }
}