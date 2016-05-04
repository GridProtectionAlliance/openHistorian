//******************************************************************************************************
//  PostgresDatabaseSetupScreen.xaml.cs - Gbtc
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
//  09/27/2011 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using GSF;
using GSF.Data;
using GSF.IO;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for PostgresDatabaseSetupScreen.xaml
    /// </summary>
    public partial class PostgresDatabaseSetupScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Fields
        private PostgresSetup m_postgresSetup;
        private Dictionary<string, object> m_state;
        private Button m_advancedButton;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="PostgresDatabaseSetupScreen"/> class.
        /// </summary>
        public PostgresDatabaseSetupScreen()
        {
            m_postgresSetup = new PostgresSetup();
            InitializeComponent();
            this.Loaded += PostgresDatabaseSetupScreen_Loaded;
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
        public bool CanGoForward
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the user can return to
        /// the previous screen from the current screen.
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the user can cancel the
        /// setup process from the current screen.
        /// </summary>
        public bool CanCancel
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the user input is valid on the current page.
        /// </summary>
        public bool UserInputIsValid
        {
            get
            {
                if (string.IsNullOrEmpty(m_hostNameTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid name for the host of the database.");
                    m_hostNameTextBox.Focus();
                    return false;
                }

                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                if (existing && !migrate)
                {
                    IDbConnection connection = null;

                    try
                    {
                        m_postgresSetup.OpenConnection(ref connection);

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
            get
            {
                return m_state;
            }
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
        private void PostgresDatabaseSetupScreen_Loaded(object sender, RoutedEventArgs e)
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
                Visibility newUserVisibility = (existing && !migrate) ? Visibility.Collapsed : Visibility.Visible;
                string newDatabaseMessage = "Please enter the needed information about the\r\nOracle database you would like to create.";
                string oldDatabaseMessage = "Please enter the needed information about\r\nyour existing Oracle database.";

                XDocument serviceConfig;
                string connectionString;
                string dataProviderString;

                m_state["postgresSetup"] = m_postgresSetup;
                m_postgresSetup.HostName = m_hostNameTextBox.Text;
                m_postgresSetup.Port = m_portTextBox.Text;
                m_postgresSetup.DatabaseName = m_databaseTextBox.Text;
                m_postgresSetup.AdminUserName = m_adminUserNameTextBox.Text;
                m_postgresSetup.AdminPassword = m_adminPasswordTextBox.SecurePassword;

                m_roleNameLabel.Visibility = newUserVisibility;
                m_rolePasswordLabel.Visibility = newUserVisibility;
                m_roleNameTextBox.Visibility = newUserVisibility;
                m_rolePasswordTextBox.Visibility = newUserVisibility;
                m_postgresDatabaseInstructionTextBlock.Text = (!existing || migrate) ? newDatabaseMessage : oldDatabaseMessage;

                // If connecting to existing database, user name and password need to be admin user:
                if (existing && !migrate)
                {
                    m_userNameLabel.Content = "User name:";
                    m_passwordLabel.Content = "Password:";
                    m_postgresSetup.RoleName = m_adminUserNameTextBox.Text;
                    m_postgresSetup.RolePassword = m_adminPasswordTextBox.SecurePassword;
                }
                else
                {
                    m_userNameLabel.Content = "Admin user name:";
                    m_passwordLabel.Content = "Admin password:";
                    m_postgresSetup.RoleName = m_roleNameTextBox.Text;
                    m_postgresSetup.RolePassword = m_rolePasswordTextBox.SecurePassword;
                }

                m_databaseTextBox.Text = migrate ? "openPDC" + App.DatabaseVersionSuffix : "openPDC";

                // When using an existing database as-is, read existing connection settings out of the configuration file
                string configFile = FilePath.GetAbsolutePath("openPDC.exe.config");

                if (!File.Exists(configFile))
                    configFile = FilePath.GetAbsolutePath("openPDCManager.exe.config");

                if (existing && !migrate && File.Exists(configFile))
                {
                    serviceConfig = XDocument.Load(configFile);

                    connectionString = serviceConfig
                        .Descendants("systemSettings")
                        .SelectMany(systemSettings => systemSettings.Elements("add"))
                        .Where(element => "ConnectionString".Equals((string)element.Attribute("name"), StringComparison.OrdinalIgnoreCase))
                        .Select(element => (string)element.Attribute("value"))
                        .FirstOrDefault();

                    dataProviderString = serviceConfig
                        .Descendants("systemSettings")
                        .SelectMany(systemSettings => systemSettings.Elements("add"))
                        .Where(element => "DataProviderString".Equals((string)element.Attribute("name"), StringComparison.OrdinalIgnoreCase))
                        .Select(element => (string)element.Attribute("value"))
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(connectionString) && PostgresSetup.DataProviderString.Equals(dataProviderString, StringComparison.OrdinalIgnoreCase))
                    {
                        m_postgresSetup.ConnectionString = connectionString;
                        m_hostNameTextBox.Text = m_postgresSetup.HostName;
                        m_portTextBox.Text = m_postgresSetup.Port;
                        m_databaseTextBox.Text = m_postgresSetup.DatabaseName;
                        m_adminUserNameTextBox.Text = m_postgresSetup.RoleName;
                        m_adminPasswordTextBox.Password = m_postgresSetup.RolePassword?.ToUnsecureString();
                    }
                }
            }
        }

        // Occurs when the screen is made visible or invisible.
        private void PostgresDatabaseSetupScreen_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (m_advancedButton == null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(this);
                Window mainWindow;

                while (parent != null && !(parent is Window))
                    parent = VisualTreeHelper.GetParent(parent);

                mainWindow = parent as Window;
                m_advancedButton = (mainWindow == null) ? null : mainWindow.FindName("m_advancedButton") as Button;
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

        // Occurs when the user changes the name of the PostgreSQL database host.
        private void HostNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_hostNameTextBox.Text = m_hostNameTextBox.Text.Trim();
            m_postgresSetup.HostName = m_hostNameTextBox.Text;
        }

        // Occurs when the user changes the port that the PostgreSQL database host is listening on.
        private void PortTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_portTextBox.Text = m_portTextBox.Text.Trim();
            m_postgresSetup.Port = m_portTextBox.Text;
        }

        // Occurs when the user changes the name of the PostgreSQL database.
        private void DatabaseTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_databaseTextBox.Text = m_databaseTextBox.Text.Trim();
            m_postgresSetup.DatabaseName = m_databaseTextBox.Text;
        }

        // Occurs when the user changes the administrator user name.
        private void AdminUserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((object)m_state != null)
            {
                string adminUserName = m_adminUserNameTextBox.Text;
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                m_postgresSetup.AdminUserName = adminUserName;

                if (existing && !migrate)
                    m_postgresSetup.RoleName = adminUserName;
            }
        }

        // Occurs when the user changes the administrator password.
        private void AdminPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            bool existing = Convert.ToBoolean(m_state["existing"]);
            bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

            m_postgresSetup.AdminPassword = m_adminPasswordTextBox.SecurePassword;

            if (existing && !migrate)
                m_postgresSetup.RolePassword = m_adminPasswordTextBox.SecurePassword;
        }

        // Occurs when the user chooses to test their database connection.
        private void TestConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            IDbConnection connection = null;
            string databaseName = m_postgresSetup.DatabaseName;

            try
            {
                m_postgresSetup.DatabaseName = null;
                m_postgresSetup.OpenAdminConnection(ref connection);

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
                m_postgresSetup.DatabaseName = databaseName;
                connection?.Dispose();
            }
        }

        // Occurs when the user changes the name of the new database role.
        private void RoleNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_postgresSetup.RoleName = m_roleNameTextBox.Text;
        }

        // Occurs when the user changes the password of the new database role.
        private void RolePasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            m_postgresSetup.RolePassword = m_rolePasswordTextBox.SecurePassword;
        }

        // Occurs when the user clicks the "Advanced..." button.
        private void AdvancedButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
            {
                SecureString password = m_postgresSetup.RolePassword;
                string dataProviderString = PostgresSetup.DataProviderString;
                bool encrypt = m_postgresSetup.EncryptConnectionString;
                string connectionString;
                AdvancedSettingsWindow advancedWindow;

                m_postgresSetup.RolePassword = null;
                connectionString = m_postgresSetup.ConnectionString;

                advancedWindow = new AdvancedSettingsWindow(connectionString, dataProviderString, encrypt);
                advancedWindow.Owner = Application.Current.MainWindow;
                (advancedWindow.FindName("m_dataProviderStringTextBox") as FrameworkElement)?.SetValue(IsEnabledProperty, false);

                if (advancedWindow.ShowDialog() == true)
                {
                    m_postgresSetup.ConnectionString = advancedWindow.ConnectionString;
                    m_postgresSetup.EncryptConnectionString = advancedWindow.Encrypt;
                }

                m_hostNameTextBox.Text = m_postgresSetup.HostName;
                m_portTextBox.Text = m_postgresSetup.Port;
                m_databaseTextBox.Text = m_postgresSetup.DatabaseName;
                m_roleNameTextBox.Text = m_postgresSetup.RoleName;

                if ((object)m_postgresSetup.RolePassword == null || m_postgresSetup.RolePassword.Length == 0)
                    m_postgresSetup.RolePassword = password;
                else
                    m_rolePasswordTextBox.Password = m_postgresSetup.RolePassword.ToUnsecureString();
            }
        }

        #endregion
    }
}
