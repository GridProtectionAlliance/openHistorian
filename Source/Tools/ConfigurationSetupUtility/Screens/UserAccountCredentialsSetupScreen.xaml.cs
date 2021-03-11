//******************************************************************************************************
//  UserAccountCredentialsSetupScreen.xaml.cs - Gbtc
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
//  01/23/2011 - J. Ritchie Carroll
//       Generated original version of source code.
//  02/28/2011 - Mehulbhai P Thakkar
//       Added a checkbox to allow pass-through authentication.
//       Added SetFocus() method to set intial focus for better user experience.
//       Added TextBox_GotFocus() event for all textboxes to highlight current value in the textbox.
//  03/02/2011 - J. Ritchie Carroll
//       Improved text box focusing after message box display.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using GSF.Identity;
using Microsoft.Win32;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for UserAccountCredentialsSetupScreen.xaml
    /// </summary>
    public partial class UserAccountCredentialsSetupScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Fields
        private Dictionary<string, object> m_state;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="UserAccountCredentialsSetupScreen"/> class.
        /// </summary>
        public UserAccountCredentialsSetupScreen()
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
                IScreen applyChangesScreen;

                if (!State.ContainsKey("applyChangesScreen"))
                    State.Add("applyChangesScreen", new ApplyConfigurationChangesScreen());

                applyChangesScreen = State["applyChangesScreen"] as IScreen;

                return applyChangesScreen;
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
                if (!CheckCurrentUserAuthentication())
                {
                    if (RadioButtonWindowsAuthentication.IsChecked == true)
                    {
                        string errorMessage = string.Empty;

                        try
                        {
                            string[] userData = ToLoginID(WindowsUserNameTextBox.Text.Trim()).Split('\\');

                            if (userData.Length == 2)
                            {
                                if (UserInfo.AuthenticateUser(userData[0], userData[1], WindowsUserPasswordTextBox.Password, out errorMessage) is null)
                                {
                                    MessageBox.Show("Authentication failed. Please verify your username and password.\r\n\r\n" + errorMessage, "Windows Authentication User Setup Error");
                                    WindowsUserPasswordTextBox.Focus();
                                    return false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Username format is invalid: for Windows authentication please provide a username formatted like \"domain\\username\".\r\nUse the machine name \"" + Environment.MachineName + "\" as the domain name if the system is not on a domain or you want to use a local account.", "Windows Authentication User Setup Error");
                                WindowsUserNameTextBox.Focus();
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + Environment.NewLine + errorMessage, "Windows Authentication User Setup Error");
                            WindowsUserPasswordTextBox.Focus();
                            return false;
                        }
                    }
                    else
                    {
                        // Define default values for database passwords, but attempt to load custom settings from openHistorian or manager config files
                        const string DefaultPasswordRequirementRegex = "^.*(?=.{8,})(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).*$";
                        const string DefaultPasswordRequirementError = "Invalid Password: Password must be at least 8 characters and must contain at least 1 number, 1 upper case letter and 1 lower case letter";

                        string configFile, passwordRequirementsRegex = null, passwordRequirementsError = null;

                        // Attempt to use the openHistorian config file first
                        configFile = Directory.GetCurrentDirectory() + "\\" + App.ApplicationConfig;

                        if (!File.Exists(configFile) || !TryLoadPasswordRequirements(configFile, out passwordRequirementsRegex, out passwordRequirementsError))
                        {
                            // Attempt to use the openHistorian Manager config file second
                            configFile = Directory.GetCurrentDirectory() + "\\" + App.ManagerConfig;

                            if (File.Exists(configFile))
                                TryLoadPasswordRequirements(configFile, out passwordRequirementsRegex, out passwordRequirementsError);
                        }

                        // Use defaults if no config file settings could be loaded
                        if (string.IsNullOrEmpty(passwordRequirementsRegex))
                            passwordRequirementsRegex = DefaultPasswordRequirementRegex;

                        if (string.IsNullOrEmpty(passwordRequirementsError))
                            passwordRequirementsError = DefaultPasswordRequirementError;

                        string userName = DbUserNameTextBox.Text.Trim();
                        string password = DbUserPasswordTextBox.Password.Trim();
                        string confirmPassword = DbUserConfirmPasswordTextBox.Password.Trim();

                        if (string.IsNullOrEmpty(userName))
                        {
                            MessageBox.Show("Please provide administrative user account name.", "Database Authentication User Setup Error");
                            DbUserNameTextBox.Focus();
                            return false;
                        }

                        if (userName.Contains("\\"))
                        {
                            MessageBox.Show("User name being used for database authentication appears to have a domain name prefix.\r\n\r\nAvoid using a \"\\\" in the user name or switch to Windows authentication mode.", "Database Authentication User Setup Error");
                            DbUserNameTextBox.Focus();
                            return false;
                        }

                        if (string.IsNullOrEmpty(password) || !Regex.IsMatch(password, passwordRequirementsRegex))
                        {
                            MessageBox.Show("Please provide valid password for administrative user." + Environment.NewLine + passwordRequirementsError, "Database Authentication User Setup Error");
                            DbUserPasswordTextBox.Focus();
                            return false;
                        }

                        if (password != confirmPassword)
                        {
                            MessageBox.Show("Password does not match the confirm password", "Database Authentication User Setup Error");
                            DbUserConfirmPasswordTextBox.Focus();
                            return false;
                        }

                        if (string.IsNullOrEmpty(DbUserFirstNameTextBox.Text.Trim()))
                        {
                            MessageBox.Show("Please provide first name for administrative user", "Database Authentication User Setup Error");
                            DbUserFirstNameTextBox.Focus();
                            return false;
                        }

                        if (string.IsNullOrEmpty(DbUserLastNameTextBox.Text.Trim()))
                        {
                            MessageBox.Show("Please provide last name for administrative user", "Database Authentication User Setup Error");
                            DbUserLastNameTextBox.Focus();
                            return false;
                        }
                    }
                }

                // Update state values to the latest entered on the form.
                InitializeState();

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

        // Initializes the state keys to their default values.
        private void InitializeState()
        {
            if (m_state != null)
            {
                m_state["authenticationType"] = RadioButtonWindowsAuthentication.IsChecked == true ? "windows" : "database";
                m_state["adminUserName"] = RadioButtonWindowsAuthentication.IsChecked == true ? ToLoginID(WindowsUserNameTextBox.Text.Trim()) : DbUserNameTextBox.Text.Trim();
                m_state["adminPassword"] = DbUserPasswordTextBox.Password.Trim();
                m_state["adminUserFirstName"] = DbUserFirstNameTextBox.Text.Trim();
                m_state["adminUserLastName"] = DbUserLastNameTextBox.Text.Trim();
                m_state["allowPassThroughAuthentication"] = CheckBoxPassThroughAuthentication.IsChecked == true ? "True" : "False";
            }
        }

        private void RadioButtonWindowsAuthentication_Checked(object sender, RoutedEventArgs e)
        {
            // Windows Authentication Selected.            
            MessageTextBlock.Text = "Please enter current credentials for the Windows authenticated user setup to be the administrator for openHistorian. Credentials validated by operating system.";
            UserAccountHeaderTextBlock.Text = "Windows Authentication";
            WindowsUserPasswordTextBox.IsEnabled = !CheckCurrentUserAuthentication();
            WindowsInfoGrid.Visibility = Visibility.Visible;
            DbInfoGrid.Visibility = Visibility.Collapsed;
            SetFocus();
        }

        private void RadioButtonWindowsAuthentication_Unchecked(object sender, RoutedEventArgs e)
        {
            // Database Authentication Selected.
            MessageTextBlock.Text = "Please provide the desired credentials for database user setup to be the administrator for openHistorian. Password complexity rules apply.";
            UserAccountHeaderTextBlock.Text = "Database Authentication";
            WindowsInfoGrid.Visibility = Visibility.Collapsed;
            DbInfoGrid.Visibility = Visibility.Visible;
            SetFocus();
        }

        private void UserAccountCredentialsSetupScreen_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButtonWindowsAuthentication.IsChecked = true;

            if (string.IsNullOrEmpty(WindowsUserNameTextBox.Text))
                WindowsUserNameTextBox.Text = Thread.CurrentPrincipal.Identity.Name;

            SetFocus();
        }

        private void WindowsUserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WindowsUserPasswordTextBox.IsEnabled = !CheckCurrentUserAuthentication();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox;
            PasswordBox passwordBox;

            textBox = sender as TextBox;

            if (textBox != null)
            {
                textBox.SelectAll();
            }
            else
            {
                passwordBox = sender as PasswordBox;

                passwordBox?.SelectAll();
            }
        }

        private bool CheckCurrentUserAuthentication()
        {
            string userName;
            string loginID;

            try
            {
                userName = WindowsUserNameTextBox.Text.Trim();

                if (RadioButtonWindowsAuthentication.IsChecked == true && !string.IsNullOrEmpty(userName))
                {
                    loginID = ToLoginID(userName);

                    if (string.Compare(Thread.CurrentPrincipal.Identity.Name, loginID, StringComparison.OrdinalIgnoreCase) == 0 && Thread.CurrentPrincipal.Identity.IsAuthenticated)
                        return true;
                }

                return false;
            }
            catch
            {
                // If an error occurs, assume
                // the user is not authenticated
                return false;
            }
        }

        private void SetFocus()
        {
            TextBox userNameTextBox = RadioButtonWindowsAuthentication.IsChecked == true ? WindowsUserNameTextBox : DbUserNameTextBox;
            PasswordBox userPasswordBox = RadioButtonWindowsAuthentication.IsChecked == true ? WindowsUserPasswordTextBox : DbUserPasswordTextBox;

            if (!string.IsNullOrEmpty(userNameTextBox.Text))
                userPasswordBox.Focus();
            else
                userNameTextBox.Focus();
        }

        #endregion

        #region [ Static ]

        private static string ToLoginID(string userID)
        {
            const string LogonDomainRegistryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";
            const string LogonDomainRegistryValue = "DefaultDomainName";

            string domain;
            string userName;
            string[] splitID = userID.Trim().Split('\\');

            if (splitID.Length == 2)
            {
                domain = splitID[0];
                userName = splitID[1];
            }
            else
            {
                userName = userID.Trim();

                // Attempt to use the default logon domain of the host machine. Note that this key will not exist on machines
                // that do not connect to a domain and the Environment.UserDomainName property will return the machine name.
                domain = Registry.GetValue(LogonDomainRegistryKey, LogonDomainRegistryValue, Environment.UserDomainName).ToString();

                // Set the domain as the local machine if one is not specified
                if (string.IsNullOrEmpty(domain))
                    domain = Environment.MachineName;

                // Handle special case - '.' is an alias for local system
                if (domain == ".")
                    domain = Environment.MachineName;
            }

            return domain + "\\" + userName;
        }

        private static SecureString ConvertToSecureString(string value)
        {
            SecureString secureString = new SecureString();

            foreach (char c in value)
                secureString.AppendChar(c);

            secureString.MakeReadOnly();

            return secureString;
        }

        // Attempts to load password requirement parameters
        private bool TryLoadPasswordRequirements(string configFileName, out string passwordRequirementsRegex, out string passwordRequirementsError)
        {
            bool loadedRequirements = false;

            // Load existing system settings
            XmlDocument configFile = new XmlDocument();
            configFile.Load(configFileName);

            passwordRequirementsRegex = null;
            passwordRequirementsError = null;

            XmlNode systemSettings = configFile.SelectSingleNode("configuration/categorizedSettings/securityProvider");

            if (systemSettings != null)
            {
                foreach (XmlNode child in systemSettings.ChildNodes)
                {
                    if (child.Attributes != null && child.Attributes["name"] != null)
                    {
                        switch (child.Attributes["name"].Value.ToLower())
                        {
                            case "passwordrequirementsregex":
                                passwordRequirementsRegex = child.Attributes["value"].Value;
                                break;
                            case "passwordrequirementserror":
                                passwordRequirementsError = child.Attributes["value"].Value;
                                break;
                        }
                    }

                    // Stop checking section node attributes once desired settings have been loaded
                    if (!string.IsNullOrEmpty(passwordRequirementsRegex) && !string.IsNullOrEmpty(passwordRequirementsError))
                    {
                        loadedRequirements = true;
                        break;
                    }
                }
            }

            return loadedRequirements;
        }

        #endregion
    }
}