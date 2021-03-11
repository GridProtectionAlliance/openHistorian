//******************************************************************************************************
//  ConfigurationTypeScreen.xaml.cs - Gbtc
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
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for ConfigurationTypeScreen.xaml
    /// </summary>
    public partial class ConfigurationTypeScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Fields

        private DatabaseSetupScreen m_databaseSetupScreen;
        private XmlSetupScreen m_xmlSetupScreen;
        private WebServiceSetupScreen m_webServiceSetupScreen;
        private UpdateConfigurationScreen m_updateConfigurationScreen;
        private WarningMessageScreen m_warningMessageScreen;
        private Dictionary<string, object> m_state;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="ConfigurationTypeScreen"/> class.
        /// </summary>
        public ConfigurationTypeScreen()
        {
            InitializeComponent();
            InitializeNextScreens();

            try
            {
                // Determine if the openHistorian is installed in this folder (could be stand-alone debug application or service), we check exe
                // instead of config file since config file can remain between installations. Since only the openHistorian can be setup with a
                // XML or web service configuration, we disable these radio buttons for openHistorian Manager only installations.
                string exeFileName = Directory.GetCurrentDirectory() + "\\" + App.ApplicationExe;

                if (!File.Exists(exeFileName))
                {
                    // If the openHistorian is not be installed user may have chosen to only install the openHistorian Manager on this system,
                    // so we disable the non-database options...
                    m_xmlRadioButton.IsEnabled = false;
                    m_webServiceRadioButton.IsEnabled = false;
                }
            }
            catch
            {
                // Not failing if we cannot determine if openHistorian is available...
            }
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
                string configurationType = m_state["configurationType"] as string;
                bool existing = Convert.ToBoolean(m_state["existing"]);

                if (configurationType == "database")
                {
                    if (existing)
                        return m_updateConfigurationScreen;
                    else
                        return m_databaseSetupScreen;
                }
                else if (configurationType == "xml")
                {
                    if (existing)
                        return m_xmlSetupScreen;
                    else
                    {
                        m_warningMessageScreen.NextScreen = m_xmlSetupScreen;
                        return m_warningMessageScreen;
                    }
                }
                else
                {
                    if (existing)
                        return m_webServiceSetupScreen;
                    else
                    {
                        m_warningMessageScreen.NextScreen = m_webServiceSetupScreen;
                        return m_warningMessageScreen;
                    }
                }
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

                if (!m_state.ContainsKey("configurationType"))
                    m_state.Add("configurationType", "database");

                m_state["databaseSetupScreen"] = m_databaseSetupScreen;
                UpdateDatabaseRadioButtonContent();
            }
        }

        /// <summary>
        /// Allows the screen to update the navigation buttons after a change is made
        /// that would affect the user's ability to navigate to other screens.
        /// </summary>
        public Action UpdateNavigation { get; set; }

        #endregion

        #region [ Methods ]

        // Initializes the screens that can be used as the next screen based on user input.
        private void InitializeNextScreens()
        {
            m_databaseSetupScreen = new DatabaseSetupScreen();
            m_xmlSetupScreen = new XmlSetupScreen();
            m_webServiceSetupScreen = new WebServiceSetupScreen();
            m_updateConfigurationScreen = new UpdateConfigurationScreen();
            m_warningMessageScreen = new WarningMessageScreen();
        }

        // Updates the text displayed for the database radio button.
        private void UpdateDatabaseRadioButtonContent()
        {
            if (m_state != null && Convert.ToBoolean(m_state["existing"]))
                m_databaseRadioButton.Content = "Database";
            else
                m_databaseRadioButton.Content = "Database (suggested)";
        }

        // Occurs when the user chooses to use a database configuration.
        private void DatabaseRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["configurationType"] = "database";
        }

        // Occurs when the user chooses to use an XML configuration.
        private void XmlRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["configurationType"] = "xml";
        }

        // Occurs when the user chooses to use a web service configuration.
        private void WebServiceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["configurationType"] = "web service";
        }

        #endregion
    }
}