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
using System.Security.Principal;
using System.Windows;
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
        private readonly string m_title;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates an instance of <see cref="App"/> class.
        /// </summary>
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
            m_errorLogger.LogToScreenshot = true;
            m_errorLogger.LogToUI = true;
            m_errorLogger.Initialize();

            m_title = AssemblyInfo.EntryAssembly.Title;

            // Setup default cache for measurement keys and associated Guid based signal ID's
            AdoDataConnection database = null;

            try
            {
                database = new AdoDataConnection(CommonFunctions.DefaultSettingsCategory);
                MeasurementKey.EstablishDefaultCache(database.Connection, database.AdapterType);
            }
            catch (Exception ex)
            {
                // Log and display error, then exit application - manager must connect to database to continue
                m_errorLogger.Log(new InvalidOperationException(string.Format("{0} cannot connect to database: {1}", m_title, ex.Message), ex), true);
            }
            finally
            {
                if (database != null)
                    database.Dispose();
            }

            IsolatedStorageManager.WriteToIsolatedStorage("MirrorMode", true);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the ID of the node user is currently connected to.
        /// </summary>
        public Guid NodeID
        {
            get
            {
                return m_nodeID;
            }
            set
            {
                m_nodeID = value;
                m_nodeID.SetAsCurrentNodeID();
            }
        }

        /// <summary>
        /// Gets title of the window.
        /// </summary>
        public string Title
        {
            get
            {
                return m_title;
            }
        }

        #endregion

        #region [ Methods ]

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            Settings.Default.Save();
        }

        private string ErrorText()
        {
            string errorMessage = m_defaultErrorText();
            Exception ex = m_errorLogger.LastException;

            if (ex != null)
            {
                if (string.Compare(ex.Message, "UnhandledException", true) == 0 && ex.InnerException != null)
                    ex = ex.InnerException;

                errorMessage = string.Format("{0}\r\n\r\nError details: {1}", errorMessage, ex.Message);
            }

            return errorMessage;
        }

        #endregion
    }
}