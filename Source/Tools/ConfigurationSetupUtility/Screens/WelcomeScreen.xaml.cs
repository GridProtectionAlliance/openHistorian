//******************************************************************************************************
//  WelcomePage.xaml.cs - Gbtc
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
//       Added code to cache 64-bit installation state when passed as a command line argument
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomeScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Fields

        private readonly IScreen m_nextPage;
        private Dictionary<string, object> m_state;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="WelcomeScreen"/> class.
        /// </summary>
        public WelcomeScreen()
        {
            InitializeComponent();
            m_nextPage = new ExistingConfigurationScreen();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the screen to be displayed when the user clicks the "Next" button.
        /// </summary>
        public IScreen NextScreen => m_nextPage;

        /// <summary>
        /// Gets a boolean indicating whether the user can advance to
        /// the next screen from the current screen.
        /// </summary>
        public bool CanGoForward => true;

        /// <summary>
        /// Gets a boolean indicating whether the user can return to
        /// the previous screen from the current screen.
        /// </summary>
        public bool CanGoBack => false;

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
                InitializeWelcomeMessage();
            }
        }

        /// <summary>
        /// Allows the screen to update the navigation buttons after a change is made
        /// that would affect the user's ability to navigate to other screens.
        /// </summary>
        public Action UpdateNavigation { get; set; }

        #endregion

        #region [ Methods ]

        // Initializes the welcome message based on the existence of the -install flag.
        private void InitializeWelcomeMessage()
        {
            string[] args = Environment.GetCommandLineArgs();
            bool installFlag = args.Contains("-install", StringComparer.CurrentCultureIgnoreCase);

            if (m_state != null)
                m_state["64bit"] = args.Contains("-64bit", StringComparer.CurrentCultureIgnoreCase);

            if (installFlag)
                m_welcomeMessageTextBlock.Text = "You now need to set up the openHistorian configuration.\r\n";
            else
                m_welcomeMessageTextBlock.Text = "";

            m_welcomeMessageTextBlock.Text += "\r\nThis wizard will walk you through the needed steps so you can easily set up your system configuration.";

            // The historian setup screen takes time to load because of DLL scanning, so we cache it at startup
            if (m_state != null)
                m_state["historianSetupScreen"] = new HistorianSetupScreen();

            m_installConnectionTester.IsEnabled = File.Exists("Installers\\PMUConnectionTesterSetup.msi");
            m_installStreamSplitter.IsEnabled = File.Exists("Installers\\StreamSplitterSetup.msi");
        }

        private void m_installConnectionTester_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (Process installProcess = new Process())
            {
                installProcess.StartInfo.FileName = "msiexec.exe";
                installProcess.StartInfo.Arguments = "-i Installers\\PMUConnectionTesterSetup.msi";
                installProcess.Start();
                installProcess.WaitForExit();
            }
        }

        private void m_installStreamSplitter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (Process installProcess = new Process())
            {
                installProcess.StartInfo.FileName = "msiexec.exe";
                installProcess.StartInfo.Arguments = "-i Installers\\StreamSplitterSetup.msi";
                installProcess.Start();
                installProcess.WaitForExit();
            }
        }

        #endregion
    }
}