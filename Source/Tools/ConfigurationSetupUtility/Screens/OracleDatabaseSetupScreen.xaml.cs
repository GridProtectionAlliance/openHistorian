//******************************************************************************************************
//  OracleDatabaseSetupScreen.xaml.cs - Gbtc
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
//  09/23/2011 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GSF.Configuration;
using GSF.Data;
using GSF.IO;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for OracleDatabaseSetupScreen.xaml
    /// </summary>
    public partial class OracleDatabaseSetupScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Fields
        private readonly OracleSetup m_oracleSetup;
        private Dictionary<string, object> m_state;
        private Button m_advancedButton;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="OracleDatabaseSetupScreen"/> class.
        /// </summary>
        public OracleDatabaseSetupScreen()
        {
            m_oracleSetup = new OracleSetup();
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(OracleDatabaseSetupScreen_Loaded);
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
                if (string.IsNullOrEmpty(m_tnsNameTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid name for the transparent network substrate (TNS).");
                    m_tnsNameTextBox.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(m_schemaUserNameTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid user name for the schema user.");
                    m_schemaUserNameTextBox.Focus();
                    return false;
                }

                if (m_schemaUserNameTextBox.Text.Length > 30)
                {
                    MessageBox.Show("Schema user name must be 30 characters or less.");
                    m_schemaUserNameTextBox.Focus();
                    return false;
                }

                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                if (existing && !migrate)
                {
                    IDbConnection connection = null;

                    try
                    {
                        m_oracleSetup.OpenConnection(ref connection);
                        if (Convert.ToInt32(connection.ExecuteScalar("SELECT COUNT(*) FROM UserAccount")) > 0)
                            m_state["securityUpgrade"] = false;
                        else
                            m_state["securityUpgrade"] = true;
                    }
                    catch (Exception ex)
                    {
                        string failMessage = "Database connection issue. " + ex.Message +
                                             " Check your username and password." +
                                             " Additionally, you may need to modify your connection under advanced settings.";

                        MessageBox.Show(failMessage);
                        m_adminUserNameTextBox.Focus();
                        return false;
                    }
                    finally
                    {
                        connection?.Dispose();
                    }
                }

                return true;
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

        // Set focus on the admin user name textbox onload.
        private void OracleDatabaseSetupScreen_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_adminUserNameTextBox.Text))
                m_adminUserNameTextBox.Focus();
            else if (string.IsNullOrEmpty(m_adminPasswordTextBox.Password))
                m_adminPasswordTextBox.Focus();
        }

        // Initialize the state keys to their default values.
        private void InitializeState()
        {
            if (m_state != null)
            {
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);
                Visibility newUserVisibility = existing && !migrate ? Visibility.Collapsed : Visibility.Visible;
                string newDatabaseMessage = "Please enter the needed information about the\r\nOracle database you would like to create.";
                string oldDatabaseMessage = "Please enter the needed information about\r\nyour existing Oracle database.";

                ConfigurationFile serviceConfig;
                string connectionString;
                string dataProviderString;

                m_state["oracleSetup"] = m_oracleSetup;
                m_oracleSetup.TnsName = m_tnsNameTextBox.Text;
                m_oracleSetup.AdminUserName = m_adminUserNameTextBox.Text;
                m_oracleSetup.AdminPassword = m_adminPasswordTextBox.Password;
                m_oracleSetup.CreateNewSchema = m_createNewSchemaCheckBox.IsChecked.HasValue && m_createNewSchemaCheckBox.IsChecked.Value;

                m_createNewSchemaCheckBox.Visibility = newUserVisibility;
                m_schemaUserNameLabel.Visibility = newUserVisibility;
                m_schemaUserPasswordLabel.Visibility = newUserVisibility;
                m_schemaUserNameTextBox.Visibility = newUserVisibility;
                m_schemaUserPasswordTextBox.Visibility = newUserVisibility;
                m_oracleDatabaseInstructionTextBlock.Text = !existing || migrate ? newDatabaseMessage : oldDatabaseMessage;

                // If connecting to existing database, user name and password need not be admin user:
                if (existing && !migrate)
                {
                    m_userNameLabel.Content = "User name:";
                    m_passwordLabel.Content = "Password:";
                    m_oracleSetup.SchemaUserName = m_adminUserNameTextBox.Text;
                    m_oracleSetup.SchemaPassword = m_adminPasswordTextBox.Password;
                }
                else
                {
                    m_userNameLabel.Content = "Admin user name:";
                    m_passwordLabel.Content = "Admin password:";
                    m_oracleSetup.SchemaUserName = m_schemaUserNameTextBox.Text;
                    m_oracleSetup.SchemaPassword = m_schemaUserPasswordTextBox.Password;
                }

                m_schemaUserNameTextBox.Text = migrate ? "openHistorian" + App.DatabaseVersionSuffix : "openHistorian";

                // When using an existing database as-is, read existing connection settings out of the configuration file
                string configFile = FilePath.GetAbsolutePath(App.ApplicationConfig);

                if (!File.Exists(configFile))
                    configFile = FilePath.GetAbsolutePath(App.ManagerConfig);

                if (existing && !migrate && File.Exists(configFile))
                {
                    serviceConfig = ConfigurationFile.Open(configFile);
                    connectionString = serviceConfig.Settings["systemSettings"]["ConnectionString"]?.Value;
                    dataProviderString = serviceConfig.Settings["systemSettings"]["DataProviderString"]?.Value;

                    if (!string.IsNullOrEmpty(connectionString) && m_oracleSetup.DataProviderString.Equals(dataProviderString, StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_oracleSetup.ConnectionString = connectionString;
                        m_tnsNameTextBox.Text = m_oracleSetup.TnsName;
                        m_adminUserNameTextBox.Text = m_oracleSetup.SchemaUserName;
                        m_adminPasswordTextBox.Password = m_oracleSetup.SchemaPassword;
                        m_oracleSetup.EncryptConnectionString = serviceConfig.Settings["systemSettings"]["ConnectionString"].Encrypted;
                    }
                }
            }
        }

        // Occurs when the screen is made visible or invisible.
        private void SqlServerDatabaseSetupScreen_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (m_advancedButton is null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(this);
                Window mainWindow;

                while (parent != null && !(parent is Window))
                    parent = VisualTreeHelper.GetParent(parent);

                mainWindow = parent as Window;
                m_advancedButton = mainWindow is null ? null : mainWindow.FindName("m_advancedButton") as Button;
            }

            if (m_advancedButton != null)
            {
                if (IsVisible)
                {
                    m_advancedButton.Visibility = Visibility.Visible;
                    m_advancedButton.Click += AdvancedButton_Click;
                }
                else
                {
                    m_advancedButton.Visibility = Visibility.Collapsed;
                    m_advancedButton.Click -= AdvancedButton_Click;
                }
            }
        }

        // Occurs when the user changes the host name of the SQL Server instance.
        private void TnsNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_tnsNameTextBox.Text = m_tnsNameTextBox.Text.Trim();
            m_oracleSetup.TnsName = m_tnsNameTextBox.Text;
        }

        // Occurs when the user changes the administrator user name.
        private void AdminUserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string adminUserName = m_adminUserNameTextBox.Text;
            bool existing = Convert.ToBoolean(m_state["existing"]);
            bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

            m_oracleSetup.AdminUserName = adminUserName;

            if (existing && !migrate)
                m_oracleSetup.SchemaUserName = adminUserName;
        }

        // Occurs when the user changes the administrator password.
        private void AdminPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string adminPassword = m_adminPasswordTextBox.Password;
            bool existing = Convert.ToBoolean(m_state["existing"]);
            bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

            m_oracleSetup.AdminPassword = adminPassword;

            if (existing && !migrate)
                m_oracleSetup.SchemaPassword = adminPassword;
        }

        // Occurs when the user chooses to test their database connection.
        private void TestConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            IDbConnection connection = null;

            try
            {
                m_oracleSetup.OpenAdminConnection(ref connection);
                MessageBox.Show("Database connection succeeded.");
            }
            catch
            {
                string failMessage = "Database connection failed."
                    + " Please check your username and password."
                    + " Additionally, you may need to modify your connection under advanced settings.";

                MessageBox.Show(failMessage);
            }
            finally
            {
                connection?.Dispose();
            }
        }

        // Occurs when the user chooses to create a new database user.
        private void CreateNewSchemaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            m_oracleSetup.CreateNewSchema = true;
        }

        // Occurs when the user chooses not to create a new database user.
        private void CreateNewSchemaCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            m_oracleSetup.CreateNewSchema = false;
        }

        // Occurs when the user changes the user name of the new database user.
        private void SchemaUserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_oracleSetup.SchemaUserName = m_schemaUserNameTextBox.Text;
        }

        // Occurs when the user changes the password of the new database user.
        private void SchemaUserPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            m_oracleSetup.SchemaPassword = m_schemaUserPasswordTextBox.Password;
        }

        // Occurs when the user clicks the "Advanced..." button.
        private void AdvancedButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
            {
                string password = m_oracleSetup.SchemaPassword;
                string dataProviderString = m_oracleSetup.DataProviderString;
                bool encrypt = m_oracleSetup.EncryptConnectionString;
                string connectionString;
                AdvancedSettingsWindow advancedWindow;

                m_oracleSetup.SchemaPassword = null;
                connectionString = m_oracleSetup.ConnectionString;

                advancedWindow = new AdvancedSettingsWindow(connectionString, dataProviderString, encrypt);
                advancedWindow.Owner = App.Current.MainWindow;

                if (advancedWindow.ShowDialog() == true)
                {
                    m_oracleSetup.ConnectionString = advancedWindow.ConnectionString;
                    m_oracleSetup.DataProviderString = advancedWindow.DataProviderString;
                    m_oracleSetup.EncryptConnectionString = advancedWindow.Encrypt;
                }

                if (string.IsNullOrEmpty(m_oracleSetup.SchemaPassword))
                    m_oracleSetup.SchemaPassword = password;

                m_tnsNameTextBox.Text = m_oracleSetup.TnsName;
                m_schemaUserNameTextBox.Text = m_oracleSetup.SchemaUserName;
                m_schemaUserPasswordTextBox.Password = m_oracleSetup.SchemaPassword;
            }
        }

        #endregion
    }
}