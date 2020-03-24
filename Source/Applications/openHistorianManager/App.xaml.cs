//******************************************************************************************************
//  App.xaml.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  07/27/2011 - Mehulbhai P Thakkar
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows;
using GSF;
using GSF.Configuration;
using GSF.Data;
using GSF.Windows.ErrorManagement;
using GSF.Reflection;
using GSF.TimeSeries;
using GSF.TimeSeries.UI;
using openHistorianManager.Properties;

namespace openHistorianManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region [ Members ]

        // Fields
        private Guid m_nodeID;
        private readonly ErrorLogger m_errorLogger;
        private readonly Func<string> m_defaultErrorText;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an instance of <see cref="App"/> class.
        /// </summary>
        public App()
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            m_errorLogger = new ErrorLogger
            {
                ErrorTextMethod = ErrorText, 
                ExitOnUnhandledException = false, 
                HandleUnhandledException = true, 
                LogToEmail = false, 
                LogToEventLog = true, 
                LogToFile = true, 
                LogToScreenshot = true, 
                LogToUI = true
            };

            m_errorLogger.Initialize();

            m_defaultErrorText = m_errorLogger.ErrorTextMethod;

            Title = AssemblyInfo.EntryAssembly.Title;

            // Setup default cache for measurement keys and associated Guid based signal ID's
            AdoDataConnection database = null;

            try
            {
                database = new AdoDataConnection(CommonFunctions.DefaultSettingsCategory);

                if (!Environment.CommandLine.Contains("-elevated"))
                {
                    ConfigurationFile configurationFile = ConfigurationFile.Current;
                    CategorizedSettingsElementCollection systemSettings = configurationFile.Settings["SystemSettings"];
                    string elevateSetting = systemSettings["ElevateProcess"]?.Value;

                    bool elevateProcess = !string.IsNullOrEmpty(elevateSetting) ? elevateSetting.ParseBoolean() : database.IsSqlite;

                    if (elevateProcess)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = Environment.GetCommandLineArgs()[0],
                            Arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1)) + " -elevated",
                            UseShellExecute = true,
                            Verb = "runas"
                        };

                        using (Process.Start(startInfo)) { }
                        Environment.Exit(0);
                    }
                }

                MeasurementKey.EstablishDefaultCache(database.Connection, database.AdapterType);
            }
            catch (Exception ex)
            {
                // First attempt to display a modal dialog will fail to block this
                // thread -- modal dialog displayed by the error logger will block now
                MessageBox.Show(ex.Message);

                // Log and display error, then exit application - manager must connect to database to continue
                m_errorLogger.Log(new InvalidOperationException($"{Title} cannot connect to database: {ex.Message}", ex), true);
            }
            finally
            {
                database?.Dispose();
            }

            IsolatedStorageManager.WriteToIsolatedStorage("MirrorMode", false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the ID of the node user is currently connected to.
        /// </summary>
        public Guid NodeID
        {
            get => m_nodeID;
            set
            {
                m_nodeID = value;
                m_nodeID.SetAsCurrentNodeID();
            }
        }

        /// <summary>
        /// Gets title of the application window.
        /// </summary>
        public string Title { get; }

        #endregion

        #region [ Methods ]

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e) => Settings.Default.Save();

        private string ErrorText()
        {
            string errorMessage = m_defaultErrorText();
            Exception ex = m_errorLogger.LastException;

            if (ex != null)
            {
                if (string.Compare(ex.Message, "UnhandledException", StringComparison.OrdinalIgnoreCase) == 0 && ex.InnerException != null)
                    ex = ex.InnerException;

                errorMessage = $"{errorMessage}\r\n\r\nError details: {ex.Message}";
            }

            return errorMessage;
        }

        #endregion
    }
}