//******************************************************************************************************
//  SqliteDatabaseSetupScreen.xaml.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  07/18/2011 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using GSF;
using GSF.Data;
using GSF.IO;
using GSF.Configuration;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for SqliteDatabaseSetupScreen.xaml
    /// </summary>
    public partial class SqliteDatabaseSetupScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Constants
        private const string DataProviderString = "AssemblyName={System.Data.SQLite, Version=1.0.109.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139}; ConnectionType=System.Data.SQLite.SQLiteConnection; AdapterType=System.Data.SQLite.SQLiteDataAdapter";

        // Fields
        private Dictionary<string, object> m_state;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="SqliteDatabaseSetupScreen"/> class.
        /// </summary>
        public SqliteDatabaseSetupScreen()
        {
            InitializeComponent();
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
                IScreen nextScreen;
                bool securityUpgrade = false;

                if (m_state.ContainsKey("securityUpgrade"))
                    securityUpgrade = Convert.ToBoolean(m_state["securityUpgrade"]);

                if (Convert.ToBoolean(m_state["existing"]) && !securityUpgrade)
                {
                    if (!m_state.ContainsKey("applyChangesScreen"))
                        m_state.Add("applyChangesScreen", new ApplyConfigurationChangesScreen());

                    nextScreen = m_state["applyChangesScreen"] as IScreen;
                }
                else
                {
                    if (!m_state.ContainsKey("userAccountSetupScreen"))
                        m_state.Add("userAccountSetupScreen", new UserAccountCredentialsSetupScreen());

                    nextScreen = m_state["userAccountSetupScreen"] as IScreen;
                }

                return nextScreen;
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
        public bool UserInputIsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(m_sqliteDatabaseFilePathTextBox.Text))
                {
                    bool existing = Convert.ToBoolean(m_state["existing"]);
                    bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                    if ((!existing || migrate) && File.Exists(m_sqliteDatabaseFilePathTextBox.Text))
                        return MessageBox.Show("A SQLite database already exists at the selected location. Are you sure you want to override the existing configuration?", "Configuration Already Exists", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes;

                    if (existing && !migrate)
                    {
                        //check if UserAccount table has any rows.
                        IDbConnection connection = null;
                        try
                        {
                            string destination = m_state["sqliteDatabaseFilePath"].ToString();
                            string connectionString = "Data Source=" + destination + "; Version=3";

                            Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
                            Dictionary<string, string> dataProviderSettings = DataProviderString.ParseKeyValuePairs();
                            string assemblyName = dataProviderSettings["AssemblyName"];
                            string connectionTypeName = dataProviderSettings["ConnectionType"];

                            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
                            Type connectionType = assembly.GetType(connectionTypeName);

                            if (settings.TryGetValue("Data Source", out string connectionSetting))
                            {
                                settings["Data Source"] = FilePath.GetAbsolutePath(connectionSetting);
                                connectionString = settings.JoinKeyValuePairs();
                            }

                            connection = (IDbConnection)Activator.CreateInstance(connectionType);
                            connection.ConnectionString = connectionString;
                            connection.Open();

                            if (Convert.ToInt32(connection.ExecuteScalar("SELECT COUNT(*) FROM UserAccount")) > 0)
                                m_state["securityUpgrade"] = false;
                            else
                                m_state["securityUpgrade"] = true;
                        }
                        catch (Exception ex)
                        {
                            string failMessage = "Database connection issue. " + ex.Message;
                            MessageBox.Show(failMessage);
                            m_sqliteDatabaseFilePathTextBox.Focus();
                            return false;
                        }
                        finally
                        {
                            connection?.Dispose();
                        }
                    }

                    return true;
                }
                else
                {
                    MessageBox.Show("Please enter a location for the SQLite database file.");
                    m_sqliteDatabaseFilePathTextBox.Focus();
                    return false;
                }
            }
        }

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
            bool existing = Convert.ToBoolean(m_state["existing"]);
            bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);
            string newDatabaseMessage = "Please select the location in which to save the new database file.";
            string oldDatabaseMessage = "Please select the location of your existing database file.";

            ConfigurationFile serviceConfig;
            string connectionString;
            string dataProviderString;

            Dictionary<string, string> settings;

            m_sqliteDatabaseInstructionTextBlock.Text = !existing || migrate ? newDatabaseMessage : oldDatabaseMessage;

            try
            {
                // Set a default path for SQLite database that will allow non-restrictive read/write access
                string sqliteDatabaseFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "openHistorian\\");

                // Make sure path exists

                if (!Directory.Exists(sqliteDatabaseFilePath))
                    Directory.CreateDirectory(sqliteDatabaseFilePath);

                m_sqliteDatabaseFilePathTextBox.Text = Path.Combine(sqliteDatabaseFilePath, migrate ? App.SqliteConfigv2 : App.BaseSqliteConfig);
            }
            catch
            {
                m_sqliteDatabaseFilePathTextBox.Text = migrate ? App.SqliteConfigv2 : App.BaseSqliteConfig;
            }

            if (!m_state.ContainsKey("sqliteDatabaseFilePath"))
                m_state.Add("sqliteDatabaseFilePath", m_sqliteDatabaseFilePathTextBox.Text);

            // When using an existing database as-is, read existing connection settings out of the configuration file
            string configFile = FilePath.GetAbsolutePath(App.ApplicationConfig);

            if (!File.Exists(configFile))
                configFile = FilePath.GetAbsolutePath(App.ManagerConfig);

            if (existing && !migrate && File.Exists(configFile))
            {
                serviceConfig = ConfigurationFile.Open(configFile);
                connectionString = serviceConfig.Settings["systemSettings"]["ConnectionString"]?.Value;
                dataProviderString = serviceConfig.Settings["systemSettings"]["DataProviderString"]?.Value;

                if (!string.IsNullOrEmpty(connectionString) && DataProviderString.Equals(dataProviderString, StringComparison.InvariantCultureIgnoreCase))
                {
                    settings = connectionString.ParseKeyValuePairs();

                    if (settings.TryGetValue("Data Source", out string setting) && File.Exists(setting))
                        m_sqliteDatabaseFilePathTextBox.Text = setting;
                }
            }
        }

        // Occurs when the user changes the path name of the SQLite database file.
        private void SqliteDatabaseFilePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_state != null)
                m_state["sqliteDatabaseFilePath"] = m_sqliteDatabaseFilePathTextBox.Text;
        }

        // Occurs when the user clicks the "Browse..." button.
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            FileDialog browseDialog;
            bool existing = Convert.ToBoolean(m_state["existing"]);
            bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

            if (existing && !migrate)
            {
                browseDialog = new OpenFileDialog();
                browseDialog.CheckFileExists = true;
            }
            else
            {
                browseDialog = new SaveFileDialog();
                browseDialog.AddExtension = true;
                browseDialog.CheckPathExists = true;
                browseDialog.DefaultExt = "db";
            }

            browseDialog.Filter = "DB Files (*.db)|*.db|All Files|*.*";

            if (browseDialog.ShowDialog() == true)
                m_sqliteDatabaseFilePathTextBox.Text = browseDialog.FileName;
        }

        #endregion
    }
}