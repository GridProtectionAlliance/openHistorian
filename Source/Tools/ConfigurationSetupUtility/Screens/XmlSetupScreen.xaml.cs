//******************************************************************************************************
//  XmlSetupScreen.xaml.cs - Gbtc
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
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for XmlSetupScreen.xaml
    /// </summary>
    public partial class XmlSetupScreen : UserControl, IScreen
    {
        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="XmlSetupScreen"/> class.
        /// </summary>
        public XmlSetupScreen()
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
                string xmlFilePath = null;

                if (State.ContainsKey("xmlFilePath"))
                    xmlFilePath = State["xmlFilePath"] as string;

                if (!string.IsNullOrEmpty(xmlFilePath))
                    return true;
                else
                {
                    MessageBox.Show("Please enter the location of an XML configuration file.");
                    m_xmlFilePathTextBox.Focus();
                    return false;
                }
            }
        }

        /// <summary>
        /// Collection shared among screens that represents the state of the setup.
        /// </summary>
        public Dictionary<string, object> State { get; set; }

        /// <summary>
        /// Allows the screen to update the navigation buttons after a change is made
        /// that would affect the user's ability to navigate to other screens.
        /// </summary>
        public Action UpdateNavigation { get; set; }

        #endregion

        #region [ Methods ]

        // Occurs when the user enters a path to an XML configuration file.
        private void XmlFilePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            State["xmlFilePath"] = m_xmlFilePathTextBox.Text.Trim();
        }

        // Occurs when the user clicks the "Browse..." button.
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog browseDialog = new OpenFileDialog();

            browseDialog.Filter = "XML Files (*.xml)|*.xml|All Files|*.*";
            browseDialog.CheckFileExists = true;

            if (browseDialog.ShowDialog() == true)
                m_xmlFilePathTextBox.Text = browseDialog.FileName;
        }

        #endregion
    }
}