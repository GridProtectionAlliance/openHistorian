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
using Microsoft.Win32;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomeScreen : IScreen
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
                m_welcomeMessageTextBlock.Text = "You now need to set up the openHistorian configuration.";
            else
                m_welcomeMessageTextBlock.Text = "";

            m_welcomeMessageTextBlock.Text += "\r\nThis wizard will walk you through the needed steps so you can easily set up your system configuration.";

            // The historian setup screen takes time to load because of DLL scanning, so we cache it at startup
            if (m_state != null)
                m_state["historianSetupScreen"] = new HistorianSetupScreen();

            UpdateInstallerVersionInfo();
        }

        private void m_installConnectionTester_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using Process installProcess = new Process();

            installProcess.StartInfo.FileName = "msiexec.exe";
            installProcess.StartInfo.Arguments = "-i Installers\\PMUConnectionTesterSetup.msi";
            installProcess.Start();
            installProcess.WaitForExit();

            UpdateInstallerVersionInfo();
        }

        private void m_installStreamSplitter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using Process installProcess = new Process();

            installProcess.StartInfo.FileName = "msiexec.exe";
            installProcess.StartInfo.Arguments = "-i Installers\\StreamSplitterSetup.msi";
            installProcess.Start();
            installProcess.WaitForExit();

            UpdateInstallerVersionInfo();
        }

        private void UpdateInstallerVersionInfo()
        {
            static string removeTrailingZeroRevision(string version) =>
                version.EndsWith(".0") ? version.Substring(0, version.Length - 2) : version;
            
            string connectionTesterVersion;

            try
            {
                connectionTesterVersion = File.ReadAllText("Installers\\PMUConnectionTesterVersion.txt").Trim();
            }
            catch
            {
                connectionTesterVersion = null;
            }            
            
            if (!string.IsNullOrWhiteSpace(connectionTesterVersion))
                m_installConnectionTester.Content = string.Format(m_installConnectionTester.Tag.ToString(), connectionTesterVersion);
            
            bool connectionTesterInstalled;

            try
            {
                object connectionTesterRevision = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Grid Protection Alliance\PMUConnectionTester", "Revision", null);

                if (connectionTesterRevision is null)
                {
                    connectionTesterInstalled = false;
                }
                else
                {
                    connectionTesterInstalled = File.Exists($@"{Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Grid Protection Alliance\PMUConnectionTester", "InstallPath", null)}\PmuConnectionTester.exe");
                    connectionTesterVersion = removeTrailingZeroRevision(connectionTesterRevision.ToString());
                }
            }
            catch
            {
                connectionTesterInstalled = false;
            }

            m_connectionTesterExisting.Content = connectionTesterInstalled ? 
                $"Currently installed PMU Connection Tester: v{connectionTesterVersion}" :
                "No current PMU Connection Tester installation detected";

            m_installConnectionTester.IsEnabled = m_connectionTesterExisting.IsEnabled = File.Exists("Installers\\PMUConnectionTesterSetup.msi");

            string streamSplitterVersion;

            try
            {
                streamSplitterVersion = File.ReadAllText("Installers\\StreamSplitterVersion.txt").Trim();
            }
            catch
            {
                streamSplitterVersion = null;
            }

            if (!string.IsNullOrWhiteSpace(streamSplitterVersion))
                m_installStreamSplitter.Content = string.Format(m_installStreamSplitter.Tag.ToString(), streamSplitterVersion);

            bool streamSplitterInstalled;

            try
            {
                object streamSplitterRevision = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Grid Protection Alliance\SynchrophasorStreamSplitter", "Revision", null);

                if (streamSplitterRevision is null)
                {
                    streamSplitterInstalled = false;
                }
                else
                {
                    streamSplitterInstalled = File.Exists($@"{Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Grid Protection Alliance\SynchrophasorStreamSplitter", "InstallPath", null)}\StreamSplitter.exe");
                    streamSplitterVersion = removeTrailingZeroRevision(streamSplitterRevision.ToString());
                }
            }
            catch
            {
                streamSplitterInstalled = false;
            }

            m_streamSplitterExisting.Content = streamSplitterInstalled ?
                $"Currently installed Stream Splitter: v{streamSplitterVersion}" :
                "No current Stream Splitter installation detected";

            m_installStreamSplitter.IsEnabled = m_streamSplitterExisting.IsEnabled = File.Exists("Installers\\StreamSplitterSetup.msi");
        }

        #endregion
    }
}