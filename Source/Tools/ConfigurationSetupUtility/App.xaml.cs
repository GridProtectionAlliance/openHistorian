//******************************************************************************************************
//  App.xaml.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/07/2010 - Stephen C. Wills
//       Generated original version of source code.
//  03/02/2011 - J. Ritchie Carroll
//       Added unhandled exception logger with dialog for better end user problem diagnosis.
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using GSF.IO;
using GSF.Security.Cryptography;
using GSF.Windows.ErrorManagement;

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region [ Members ]

        // Constants
        public const CipherStrength CryptoStrength = CipherStrength.Aes256;
        public const string CipherLookupKey = "0679d9ae-aca5-4702-a3f5-604415096987";
        public const string ApplicationExe = "openHistorian.exe";
        public const string ApplicationConfig = "openHistorian.exe.config";
        public const string Manager = "openHistorianManager";
        public const string ManagerExe = "openHistorianManager.exe";
        public const string ManagerConfig = "openHistorianManager.exe.config";
        public const string BaseSqliteConfig = "openHistorian.db";
        public readonly static string SqliteConfigv2 = "openHistorian" + DatabaseVersionSuffix + ".db";
        public const string SqliteSampleData = "openHistorian-SampleDataSet.db";
        public const string SqliteInitialData = "openHistorian-InitialDataSet.db";
        private readonly ErrorLogger m_errorLogger;
        private readonly Func<string> m_defaultErrorText;

        #endregion

        #region [ Constructors ]

        public App()
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            m_errorLogger = new ErrorLogger();
            m_defaultErrorText = m_errorLogger.ErrorTextMethod;
            m_errorLogger.ErrorTextMethod = ErrorText;
            m_errorLogger.ExitOnUnhandledException = false;
            m_errorLogger.HandleUnhandledException = true;
            m_errorLogger.LogToEmail = false;
            m_errorLogger.LogToEventLog = true;
            m_errorLogger.LogToFile = true;
            m_errorLogger.LogToScreenshot = false;
            m_errorLogger.LogToUI = true;
            m_errorLogger.Initialize();

            // When run from the installer the current directory may not be the directory where this application is running
            Directory.SetCurrentDirectory(FilePath.GetAbsolutePath(""));

            // Attempt to create an event log source for the openHistorian Manager for authentication logging. This needs to be done
            // here since the CSU runs with administrative privileges and the openHistorian Manager normally does not; also there is
            // a short system delay that exists before you can write to a new event log source after it is first created.
            try
            {
                string applicationName = "openHistorian";

                // Create the event log source based on defined application name for openHistorian if it does not already exist
                if (!EventLog.SourceExists(applicationName))
                    EventLog.CreateEventSource(applicationName, "Application");

                applicationName = "openHistorian Manager";

                // Create the event log source based on defined application name for openHistorian Manager if it does not already exist
                if (!EventLog.SourceExists(applicationName))
                    EventLog.CreateEventSource(applicationName, "Application");
            }
            catch (Exception ex)
            {
                m_errorLogger.Log(new InvalidOperationException($"Warning: failed to create or validate the event log source for the openHistorian Manager: {ex.Message}", ex), false);
            }

            try
            {
                using (Process util = new Process())
                {
                    util.StartInfo.FileName = "NoInetFixUtil.exe";
                    util.StartInfo.Arguments = " --checkall";
                    util.StartInfo.UseShellExecute = true;
                    util.Start();
                }
            }
            catch (Exception ex)
            {
                if (!m_errorLogger.ErrorLog.IsOpen)
                    m_errorLogger.ErrorLog.Open();

                m_errorLogger.ErrorLog.WriteLine($"Warning: failed to run NoInetUtil: {ex.Message}");
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets reference to global error logger.
        /// </summary>
        public ErrorLogger ErrorLogger => m_errorLogger;

        #endregion

        #region [ Methods ]

        private string ErrorText()
        {
            string errorMessage = m_defaultErrorText();
            Exception ex = m_errorLogger.LastException;

            if (ex != null)
            {
                if (string.Compare(ex.Message, "UnhandledException", true) == 0 && ex.InnerException != null)
                    ex = ex.InnerException;

                errorMessage = $"{errorMessage}\r\n\r\nError details: {ex.Message}";
            }

            return errorMessage;
        }

        #endregion

        #region [ Static ]

        private static string s_currentVersionLabel;

        /// <summary>
        /// Gets database name suffix for current application version, e.g., "v21" for version 2.1
        /// </summary>
        public static string DatabaseVersionSuffix
        {
            get
            {
                if (!string.IsNullOrEmpty(s_currentVersionLabel))
                    return s_currentVersionLabel;

                try
                {
                    Version version = Assembly.GetEntryAssembly().GetName().Version;
                    s_currentVersionLabel = $"v{version.Major}{version.Minor}";
                }
                catch
                {
                    s_currentVersionLabel = "v2";
                }

                return s_currentVersionLabel;
            }
        }

        #endregion
    }
}