//******************************************************************************************************
//  MySqlDatabaseSetupScreen.xaml.cs - Gbtc
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
//  09/09/2010 - Stephen C. Wills
//       Generated original version of source code.
//  09/19/2010 - J. Ritchie Carroll
//       Added security warning message for non-local MySql host addresses.
//  09/26/2010 - J. Ritchie Carroll
//       Added typical versions of the MySQL Connector/NET so the data provider string could be
//       automatically defined.
//  01/21/2011 - J. Ritchie Carroll
//       Modified next page to be admin user account credentials setup.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GSF.Configuration;
using GSF.Data;
using GSF.IO;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for MySqlDatabaseSetupScreen.xaml
    /// </summary>
    public partial class MySqlDatabaseSetupScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Fields
        private readonly MySqlSetup m_mySqlSetup;
        private Dictionary<string, object> m_state;
        private Button m_advancedButton;
        private readonly string m_dataProviderString;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="MySqlDatabaseSetupScreen"/> class.
        /// </summary>
        public MySqlDatabaseSetupScreen()
        {
            m_mySqlSetup = new MySqlSetup();
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MySqlDatabaseSetupScreen_Loaded);

            string[] mySQLConnectorNetVersions = { "6.7.9.0", "6.7.8.0", "6.7.7.0", "6.7.6.0", "6.7.5.0", "6.7.4.0", "6.7.3.0", "6.7.2.0", "6.7.1.0", "6.7.0.0",
                                                   "6.6.9.0", "6.6.8.0", "6.6.7.0", "6.6.6.0", "6.6.5.0", "6.6.4.0", "6.6.3.0", "6.6.2.0", "6.6.1.0", "6.6.0.0",
                                                   "6.5.9.0", "6.5.8.0", "6.5.7.0", "6.5.6.0", "6.5.5.0", "6.5.4.0", "6.5.3.0", "6.5.2.0", "6.5.1.0", "6.5.0.0",
                                                   "6.4.9.0", "6.4.8.0", "6.4.7.0", "6.4.6.0", "6.4.5.0", "6.4.4.0", "6.4.3.0", "6.4.2.0", "6.4.1.0", "6.4.0.0",
                                                   "6.3.9.0", "6.3.8.0", "6.3.7.0", "6.3.6.0", "6.3.5.0", "6.3.4.0", "6.3.3.0", "6.3.2.0", "6.3.1.0", "6.3.0.0",
                                                   "6.2.9.0", "6.2.8.0", "6.2.7.0", "6.2.6.0", "6.2.5.0", "6.2.4.0", "6.2.3.0", "6.2.2.0", "6.2.1.0", "6.2.0.0",
                                                   "6.1.9.0", "6.1.8.0", "6.1.7.0", "6.1.6.0", "6.1.5.0", "6.1.4.0", "6.1.3.0", "6.1.2.0", "6.1.1.0", "6.1.0.0" };

            string assemblyNamePrefix = "MySql.Data, Version=";
            string assemblyNameSuffix = ", Culture=neutral, PublicKeyToken=c5687fc88969c44d";
            string assemblyName;

            m_dataProviderString = null;

            // Attempt to load latest version of the MySQL connector net to creator the proper data provider string
            foreach (string connectorNetVersion in mySQLConnectorNetVersions)
            {
                try
                {
                    // Create an assembly name based on this version of the MySQL Connector/NET
                    assemblyName = assemblyNamePrefix + connectorNetVersion + assemblyNameSuffix;

                    // See if this version of the MySQL Connector/NET can be loaded
                    Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));

                    // If assembly load succeeded, create a valid data provider string
                    m_dataProviderString = "AssemblyName={" + assemblyName + "}; ConnectionType=MySql.Data.MySqlClient.MySqlConnection; AdapterType=MySql.Data.MySqlClient.MySqlDataAdapter";
                }
                catch
                {
                    // Nothing to do but try next version
                }
            }

            if (string.IsNullOrEmpty(m_dataProviderString))
                m_dataProviderString = "AssemblyName={MySql.Data, Version=6.5.4.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d}; ConnectionType=MySql.Data.MySqlClient.MySqlConnection; AdapterType=MySql.Data.MySqlClient.MySqlDataAdapter";

            m_mySqlSetup.DataProviderString = m_dataProviderString;
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
                if (string.IsNullOrEmpty(m_hostNameTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid host name for the MySQL instance.");
                    m_hostNameTextBox.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(m_databaseNameTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid database name.");
                    m_databaseNameTextBox.Focus();
                    return false;
                }

                if (m_createNewUserCheckBox.IsChecked.Value && string.IsNullOrEmpty(m_newUserNameTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid user name for the new user.");
                    m_newUserNameTextBox.Focus();
                    return false;
                }

                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                if (existing && !migrate)
                {
                    IDbConnection connection = null;

                    try
                    {
                        m_mySqlSetup.OpenConnection(ref connection);

                        if (Convert.ToInt32(connection.ExecuteScalar("SELECT COUNT(*) FROM UserAccount")) > 0)
                            m_state["securityUpgrade"] = false;
                        else
                            m_state["securityUpgrade"] = true;
                    }
                    catch (Exception ex)
                    {
                        string failMessage = "Database connection failed."
                            + " Please check your username and password."
                            + " Additionally, you may need to modify your connection under advanced settings."
                            + Environment.NewLine + Environment.NewLine
                            + "Error: " + ex.Message;

                        MessageBox.Show(failMessage);
                        m_newUserNameTextBox.Focus();
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
        private void MySqlDatabaseSetupScreen_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_adminUserNameTextBox.Text))
                m_adminUserNameTextBox.Focus();
            else if (string.IsNullOrEmpty(m_adminPasswordTextBox.Password))
                m_adminPasswordTextBox.Focus();
        }

        // Initializes the state keys to their default values.
        private void InitializeState()
        {
            if (m_state != null)
            {
                const string NewDatabaseMessage = "Please enter the needed information about the\r\nMySQL database you would like to create.";
                const string OldDatabaseMessage = "Please enter the needed information about\r\nyour existing MySQL database.";

                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);
                Visibility newUserVisibility = existing && !migrate ? Visibility.Collapsed : Visibility.Visible;

                ConfigurationFile serviceConfig;
                string connectionString;
                string dataProviderString;

                m_state["mySqlSetup"] = m_mySqlSetup;
                m_mySqlSetup.HostName = m_hostNameTextBox.Text;
                m_mySqlSetup.DatabaseName = m_databaseNameTextBox.Text;
                m_createNewUserCheckBox.Visibility = newUserVisibility;
                m_newUserNameLabel.Visibility = newUserVisibility;
                m_newUserPasswordLabel.Visibility = newUserVisibility;
                m_newUserNameTextBox.Visibility = newUserVisibility;
                m_newUserPasswordTextBox.Visibility = newUserVisibility;
                m_mySqlDatabaseInstructionTextBlock.Text = !existing || migrate ? NewDatabaseMessage : OldDatabaseMessage;

                // If connecting to existing database, user name and password need not be admin user:
                if (existing && !migrate)
                {
                    m_userNameLabel.Content = "User name:";
                    m_passwordLabel.Content = "Password:";
                }
                else
                {
                    m_userNameLabel.Content = "Admin user name:";
                    m_passwordLabel.Content = "Admin password:";
                }

                if (!m_state.ContainsKey("createNewMySqlUser"))
                    m_state.Add("createNewMySqlUser", m_createNewUserCheckBox.IsChecked.Value);

                if (!m_state.ContainsKey("newMySqlUserName"))
                    m_state.Add("newMySqlUserName", m_newUserNameTextBox.Text);

                if (!m_state.ContainsKey("newMySqlUserPassword"))
                    m_state.Add("newMySqlUserPassword", m_newUserPasswordTextBox.Password);

                if (!m_state.ContainsKey("encryptMySqlConnectionStrings"))
                    m_state.Add("encryptMySqlConnectionStrings", false);

                m_databaseNameTextBox.Text = migrate ? "openHistorian" + App.DatabaseVersionSuffix : "openHistorian";

                // When using an existing database as-is, read existing connection settings out of the configuration file
                string configFile = FilePath.GetAbsolutePath(App.ApplicationConfig); //"openHistorian.exe.config"

                if (!File.Exists(configFile))
                    configFile = FilePath.GetAbsolutePath(App.ManagerConfig); //"openHistorianManager.exe.config"

                if (existing && !migrate && File.Exists(configFile))
                {
                    serviceConfig = ConfigurationFile.Open(configFile);
                    connectionString = serviceConfig.Settings["systemSettings"]["ConnectionString"]?.Value;
                    dataProviderString = serviceConfig.Settings["systemSettings"]["DataProviderString"]?.Value;

                    if (!string.IsNullOrEmpty(connectionString) && m_mySqlSetup.DataProviderString.Equals(dataProviderString, StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_mySqlSetup.ConnectionString = connectionString;
                        m_hostNameTextBox.Text = m_mySqlSetup.HostName;
                        m_databaseNameTextBox.Text = m_mySqlSetup.DatabaseName;
                        m_adminUserNameTextBox.Text = m_mySqlSetup.UserName;
                        m_adminPasswordTextBox.Password = m_mySqlSetup.Password;
                        m_state["encryptMySqlConnectionStrings"] = serviceConfig.Settings["systemSettings"]["ConnectionString"].Encrypted;
                    }
                }
            }
        }

        // Occurs when the screen is made visible or invisible.
        private void MySqlDatabaseSetupScreen_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        // Occurs when the user changes the host name of the MySQL instance.
        private void HostNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_hostNameTextBox.Text = m_hostNameTextBox.Text.Trim();
            m_mySqlSetup.HostName = m_hostNameTextBox.Text;
        }

        // Occurs when the user leaves the host name field
        private void m_hostNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress[] hostIPs = Dns.GetHostAddresses(m_hostNameTextBox.Text);
                IEnumerable<IPAddress> localIPs = Dns.GetHostAddresses("localhost").Concat(Dns.GetHostAddresses(Dns.GetHostName()));

                // Check to see if entered host name corresponds to a local IP address
                if (!hostIPs.Any(localIPs.Contains))
                    MessageBox.Show("You have entered a non-local host name for your MySql instance. By default remote access to MySQL database server is disabled for security reasons. If you have trouble connecting, check the security settings on the remote MySQL database server.", "MySql Security", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch
            {
                MessageBox.Show("The configuration utility could not determine if you entered a non-local host name for your MySql instance. Keep in mind that remote access to MySQL database server is disabled by default for security reasons. If you have trouble connecting, check the security settings on the remote MySQL database server.", "MySql Security", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Occurs when the user changes the database name.
        private void DatabaseNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_mySqlSetup.DatabaseName = m_databaseNameTextBox.Text;
        }

        // Removes invalid characters from database name
        private void DatabaseNameTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            bool existing = Convert.ToBoolean(m_state["existing"]);
            bool correctDatabaseName = !(existing && !Convert.ToBoolean(m_state["updateConfiguration"]));
            if (correctDatabaseName)
            {
                m_databaseNameTextBox.Text = Regex.Replace(m_databaseNameTextBox.Text, @"[\W]", "");
            }
        }

        // Occurs when the user changes the administrator user name.
        private void AdminUserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string adminUserName = m_adminUserNameTextBox.Text;
            m_mySqlSetup.UserName = adminUserName;
        }

        // Occurs when the user changes the administrator password.
        private void AdminPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string adminPassword = m_adminPasswordTextBox.Password;
            m_mySqlSetup.Password = adminPassword;
        }

        // Occurs when the user chooses to test their database connection.
        private void TestConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            IDbConnection connection = null;
            string databaseName = null;

            try
            {
                databaseName = m_mySqlSetup.DatabaseName;
                m_mySqlSetup.DatabaseName = null;
                m_mySqlSetup.OpenConnection(ref connection);
                MessageBox.Show("Database connection succeeded.");
            }
            catch
            {
                const string FailMessage = "Database connection failed."
                    + " Please check your username and password."
                    + " Additionally, you may need to modify your connection under advanced settings.";

                MessageBox.Show(FailMessage);
            }
            finally
            {
                connection?.Dispose();

                if (databaseName != null)
                    m_mySqlSetup.DatabaseName = databaseName;
            }
        }

        // Occurs when the user chooses to create a new database user.
        private void CreateNewUserCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["createNewMySqlUser"] = true;
        }

        // Occurs when the user chooses not to create a new database user.
        private void CreateNewUserCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["createNewMySqlUser"] = false;
        }

        // Occurs when the user changes the user name of the new MySQL database user.
        private void NewUserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_state != null)
                m_state["newMySqlUserName"] = m_newUserNameTextBox.Text;
        }

        // Occurs when the user changes the password of the new MySQL database user.
        private void NewUserPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["newMySqlUserPassword"] = m_newUserPasswordTextBox.Password;
        }

        // Occurs when the user clicks the "Advanced..." button.
        private void AdvancedButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
            {
                string password = m_mySqlSetup.Password;
                string dataProviderString = m_mySqlSetup.DataProviderString;
                bool encrypt = Convert.ToBoolean(m_state["encryptMySqlConnectionStrings"]);
                string connectionString;
                AdvancedSettingsWindow advancedWindow;

                m_mySqlSetup.Password = null;
                connectionString = m_mySqlSetup.ConnectionString;
                advancedWindow = new AdvancedSettingsWindow(connectionString, dataProviderString, encrypt);
                advancedWindow.Owner = Application.Current.MainWindow;

                if (advancedWindow.ShowDialog() == true)
                {
                    m_mySqlSetup.ConnectionString = advancedWindow.ConnectionString;
                    m_mySqlSetup.DataProviderString = advancedWindow.DataProviderString;
                    m_state["encryptMySqlConnectionStrings"] = advancedWindow.Encrypt;
                }

                if (string.IsNullOrEmpty(m_mySqlSetup.Password))
                    m_mySqlSetup.Password = password;

                m_hostNameTextBox.Text = m_mySqlSetup.HostName;
                m_databaseNameTextBox.Text = m_mySqlSetup.DatabaseName;
                m_adminUserNameTextBox.Text = m_mySqlSetup.UserName;
                m_adminPasswordTextBox.Password = m_mySqlSetup.Password;
            }
        }

        #endregion
    }
}