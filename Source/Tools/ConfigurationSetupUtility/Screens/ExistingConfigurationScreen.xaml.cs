//******************************************************************************************************
//  ExistingConfigurationScreen.xaml.cs - Gbtc
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

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for ExistingConfigurationScreen.xaml
    /// </summary>
    public partial class ExistingConfigurationScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Fields
        private readonly IScreen m_nextScreen;
        private Dictionary<string, object> m_state;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="ExistingConfigurationScreen"/> class.
        /// </summary>
        public ExistingConfigurationScreen()
        {
            InitializeComponent();
            m_nextScreen = new ConfigurationTypeScreen();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the screen to be displayed when the user clicks the "Next" button.
        /// </summary>
        public IScreen NextScreen => m_nextScreen;

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

                if (!m_state.ContainsKey("existing"))
                    m_state.Add("existing", false);

                if (m_state.ContainsKey("updateConfiguration"))
                    m_state.Remove("updateConfiguration");
            }
        }

        /// <summary>
        /// Allows the screen to update the navigation buttons after a change is made
        /// that would affect the user's ability to navigate to other screens.
        /// </summary>
        public Action UpdateNavigation { get; set; }

        #endregion

        #region [ Methods ]

        // Occurs when the user chooses to set up using a new configuration.
        private void NewConfigurationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["existing"] = false;
        }

        // Occurs when the user chooses to set up using an existing configuration.
        private void ExistingConfigurationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (m_state != null)
                m_state["existing"] = true;
        }

        #endregion
    }
}