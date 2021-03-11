//******************************************************************************************************
//  DatabaseSetupScreen.xaml.cs - Gbtc
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
//  09/19/2010 - J. Ritchie Carroll
//       Added code to hide Access database option for 64-bit installations
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using GSF;
using GSF.Configuration;
using GSF.Data;
using Microsoft.Win32;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for DatabaseSetupScreen.xaml
    /// </summary>
    public partial class DatabaseSetupScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Fields
        private SqlServerDatabaseSetupScreen m_sqlServerDatabaseSetupScreen;
        private MySqlDatabaseSetupScreen m_mySqlDatabaseSetupScreen;
        private OracleDatabaseSetupScreen m_oracleDatabaseSetupScreen;
        private SqliteDatabaseSetupScreen m_sqliteDatabaseSetupScreen;
        private PostgresDatabaseSetupScreen m_postgresDatabaseSetupScreen;
        private Dictionary<string, object> m_state;
        private bool m_sampleScriptChanged;
        private bool m_enableAuditLogChanged;
        private string m_oldConnectionString;
        private string m_oldDataProviderString;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="DatabaseSetupScreen"/> class.
        /// </summary>
        public DatabaseSetupScreen()
        {
            InitializeComponent();
            InitializeNextScreens();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the screen to be displayed when the user clicks the "Next" button.
        /// </summary>
        public IScreen NextScreen
        {
            get
            {
                string databaseType = m_state["newDatabaseType"].ToString();

                if (databaseType == "SQLServer")
                    return m_sqlServerDatabaseSetupScreen;
                else if (databaseType == "MySQL")
                    return m_mySqlDatabaseSetupScreen;
                else if (databaseType == "Oracle")
                    return m_oracleDatabaseSetupScreen;
                else if (databaseType == "PostgreSQL")
                    return m_postgresDatabaseSetupScreen;
                else
                    return m_sqliteDatabaseSetupScreen;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the user can advance to
        /// the next screen from the current screen.
        /// </summary>
        public bool CanGoForward => true;

        /// <summary>
        /// Gets a boolean indicating whether the user can return to
        /// the previous screen from the current screen.
        /// </summary>
        public bool CanGoBack => true;

        /// <summary>
        /// Gets a boolean indicating whether the user can cancel the
        /// setup process from the current screen.
        /// </summary>
        public bool CanCancel => true;

        /// <summary>
        /// Gets a boolean indicating whether the user input is valid on the current page.
        /// </summary>
        public bool UserInputIsValid => true;

        /// <summary>
        /// Collection shared among screens that represents the state of the setup.
        /// </summary>
        public Dictionary<string, object> State
        {
            get => m_state;
            set
            {
                m_state = value;
                InitializeState();
            }
        }

        /// <summary>
        /// Allows the screen to update the navigation buttons after a change is made
        /// that would affect the user's ability to navigate to other screens.
        /// </summary>
        public Action UpdateNavigation
        {
            get;
            set;
        }

        #endregion

        #region [ Methods ]

        // Initializes the state keys to their default values.
        private void InitializeState()
        {
            if (m_state != null)
            {
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);
                Visibility existingVisibility = existing ? Visibility.Collapsed : Visibility.Visible;

                m_initialDataScriptCheckBox.Visibility = existingVisibility;
                m_sampleDataScriptCheckBox.Visibility = existingVisibility;

                // Show new database warning anytime user will be creating a new database
                if (m_state.TryGetValue("updateConfiguration", out object value))
                    m_newDatabaseWarning.Visibility = Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Collapsed;
                else
                    m_newDatabaseWarning.Visibility = existingVisibility;

                if (!m_state.ContainsKey("newDatabaseType"))
                    m_state.Add("newDatabaseType", "SQLServer");

                if (!m_state.ContainsKey("initialDataScript"))
                    m_state.Add("initialDataScript", true);

                if (!m_state.ContainsKey("sampleDataScript"))
                    m_state.Add("sampleDataScript", false);

                if (!m_state.ContainsKey("enableAuditLog"))
                    m_state.Add("enableAuditLog", false);

                if (m_enableAuditLogCheckBox != null)
                    ManageEnableAuditLogCheckBox();

                // If we are migrating to a new schema we need to check the old database if possible so we can determine if this is a
                // schema update from a non-security enabled database so that we can request admin credentials for the first time...
                if (migrate)
                {
                    object webManagerDir = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\openHistorianManagerServices", "Installation Path", null) ?? Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Wow6432Node\\openHistorianManagerServices", "Installation Path", null);
                    string configFile;

                    m_oldConnectionString = null;
                    m_oldDataProviderString = null;

                    // Attempt to use the openHistorian config file first
                    configFile = Directory.GetCurrentDirectory() + "\\" + App.ApplicationConfig;

                    if (File.Exists(configFile))
                    {
                        LoadOldConnectionStrings(configFile);
                    }
                    else
                    {
                        // Attempt to use the openHistorian Manager config file second
                        configFile = Directory.GetCurrentDirectory() + "\\" + App.ManagerConfig;

                        if (File.Exists(configFile))
                        {
                            LoadOldConnectionStrings(configFile);
                        }
                        else
                        {
                            // Attempt to use the web based openHistorian Manager config file as a last resort
                            if (webManagerDir != null)
                            {
                                configFile = webManagerDir.ToString() + "\\Web.config";

                                if (File.Exists(configFile))
                                    LoadOldConnectionStrings(configFile);
                            }
                        }
                    }

                    // Attempt to open existing database connection and see if "AuditLog" table exists
                    if (m_oldConnectionString != null && m_oldDataProviderString != null)
                    {
                        IDbConnection connection = null;

                        try
                        {
                            Dictionary<string, string> settings = m_oldConnectionString.ParseKeyValuePairs();
                            Dictionary<string, string> dataProviderSettings = m_oldDataProviderString.ParseKeyValuePairs();
                            string assemblyName = dataProviderSettings["AssemblyName"];
                            string connectionTypeName = dataProviderSettings["ConnectionType"];

                            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
                            Type connectionType = assembly.GetType(connectionTypeName);

                            connection = (IDbConnection)Activator.CreateInstance(connectionType);
                            connection.ConnectionString = m_oldConnectionString;
                            connection.Open();

                            try
                            {
                                if ((int)connection.ExecuteScalar("SELECT COUNT(*) FROM UserAccount") > 0)
                                    m_state["securityUpgrade"] = false;
                                else
                                    m_state["securityUpgrade"] = true;
                            }
                            catch
                            {
                                m_state["securityUpgrade"] = true;
                            }
                        }
                        catch
                        {
                            // Failure to open old database means we can't test if this is a non-security enabled schema
                        }
                        finally
                        {
                            connection?.Dispose();
                        }
                    }
                }
                if (!m_state.ContainsKey("securityUpgrade"))
                    m_state.Add("securityUpgrade", false);
            }
        }

        // Initializes the screens that can be used as the next screen based on user input.
        private void InitializeNextScreens()
        {
            m_sqlServerDatabaseSetupScreen = new SqlServerDatabaseSetupScreen();
            m_mySqlDatabaseSetupScreen = new MySqlDatabaseSetupScreen();
            m_oracleDatabaseSetupScreen = new OracleDatabaseSetupScreen();
            m_sqliteDatabaseSetupScreen = new SqliteDatabaseSetupScreen();
            m_postgresDatabaseSetupScreen = new PostgresDatabaseSetupScreen();
        }

        // Occurs when the user chooses to set up a SQL Server database.
        private void SqlServerDatabaseRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["newDatabaseType"] = "SQLServer";

            if (!m_sampleScriptChanged && m_sampleDataScriptCheckBox != null)
                m_sampleDataScriptCheckBox.IsChecked = false;

            if (m_enableAuditLogCheckBox != null)
            {
                //Make it visible for SQL Server database.
                ManageEnableAuditLogCheckBox();

                if (!m_enableAuditLogChanged)
                    m_enableAuditLogCheckBox.IsChecked = false;
            }
        }

        // Occurs when the user chooses to set up a MySQL database.
        private void MySqlRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["newDatabaseType"] = "MySQL";

            if (!m_sampleScriptChanged && m_sampleDataScriptCheckBox != null)
                m_sampleDataScriptCheckBox.IsChecked = false;

            if (m_enableAuditLogCheckBox != null)
            {
                // Make it visible for MySQL database
                ManageEnableAuditLogCheckBox();
                if (!m_enableAuditLogChanged)
                    m_enableAuditLogCheckBox.IsChecked = false;
            }
        }

        // Occurs when the user chooses to set up an Oracle database.
        private void OracleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["newDatabaseType"] = "Oracle";

            if (!m_sampleScriptChanged && m_sampleDataScriptCheckBox != null)
                m_sampleDataScriptCheckBox.IsChecked = false;

            if (m_enableAuditLogCheckBox != null)
            {
                // Make it visible for Oracle database
                ManageEnableAuditLogCheckBox();

                if (!m_enableAuditLogChanged)
                    m_enableAuditLogCheckBox.IsChecked = false;
            }
        }

        // Occurs when the user chooses to set up a SQLite database.
        private void SqliteRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["newDatabaseType"] = "SQLite";

            //if (!m_sampleScriptChanged && m_sampleDataScriptCheckBox != null)
            //    m_sampleDataScriptCheckBox.IsChecked = true;

            if (m_enableAuditLogCheckBox != null)
                ManageEnableAuditLogCheckBox();
        }

        // Occurs when the user chooses to set up a Oracle database.
        private void PostgresRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["newDatabaseType"] = "PostgreSQL";

            if (!m_sampleScriptChanged && m_sampleDataScriptCheckBox != null)
                m_sampleDataScriptCheckBox.IsChecked = false;

            if (m_enableAuditLogCheckBox != null)
                ManageEnableAuditLogCheckBox();
        }

        // Occurs when the user chooses to run the initial data script when setting up their database.
        private void InitialDataScriptCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["initialDataScript"] = true;
        }

        // Occurs when the user chooses to not run the initial data script when setting up their database.
        private void InitialDataScriptCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["initialDataScript"] = false;
        }

        // Occurs when the user explicitly changes the sample data script check box.
        private void SampleDataScriptCheckBox_Click(object sender, RoutedEventArgs e)
        {
            m_sampleScriptChanged = true;
        }

        // Occurs when the user chooses to run the sample data script when setting up their database.
        private void SampleDataScriptCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["sampleDataScript"] = true;
        }

        // Occurs when the user chooses to not run the sample data script when setting up their database.
        private void SampleDataScriptCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["sampleDataScript"] = false;
        }

        // Occurs when the user explicitly changes the enable audit log check box.
        private void EnableAuditLogCheckBox_Click(object sender, RoutedEventArgs e)
        {
            m_enableAuditLogChanged = true;
        }

        // Occurs when the user chooses to run the enable audit log when setting up their database.
        private void EnableAuditLogCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["enableAuditLog"] = true;
        }

        // Occurs when the user chooses to not run the enable audit log when setting up their database.
        private void EnableAuditLogCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["enableAuditLog"] = false;
        }

        private void ManageEnableAuditLogCheckBox()
        {
            bool existing = Convert.ToBoolean(m_state["existing"]);
            bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

            if (!existing || migrate)
            {
                m_enableAuditLogCheckBox.Visibility = Visibility.Visible;

                if (m_state.ContainsKey("newDatabaseType") && (m_state["newDatabaseType"].ToString() == "SQLite" || m_state["newDatabaseType"].ToString() == "PostgreSQL"))
                    m_enableAuditLogCheckBox.Visibility = Visibility.Collapsed;
            }
            else
                m_enableAuditLogCheckBox.Visibility = Visibility.Collapsed;
        }

        // Attempts to load old connection string parameters
        private void LoadOldConnectionStrings(string configFileName)
        {
            if ((object)m_oldConnectionString != null && (object)m_oldDataProviderString != null)
                return;

            ConfigurationFile configFile = ConfigurationFile.Open(configFileName);
            CategorizedSettingsSection categorizedSettings = configFile.Settings;
            CategorizedSettingsElementCollection systemSettings = categorizedSettings["systemSettings"];

            m_oldConnectionString = systemSettings["ConnectionString"]?.Value;
            m_oldDataProviderString = systemSettings["DataProviderString"]?.Value;
        }

        #endregion
    }
}