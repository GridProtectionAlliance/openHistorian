//******************************************************************************************************
//  AdvancedSettingsWindow.xaml.cs - Gbtc
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
//  09/10/2010 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Windows;

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// Interaction logic for AdvancedSettingsWindow.xaml
    /// </summary>
    public partial class AdvancedSettingsWindow : Window
    {
        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="AdvancedSettingsWindow"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be displayed initially in the connection string text box.</param>
        /// <param name="encrypt">Determines whether the encrypt check box is initially checked or unchecked.</param>
        public AdvancedSettingsWindow(string connectionString, string dataProviderString, bool encrypt)
        {
            InitializeComponent();

            m_connectionStringTextBox.Text = connectionString;
            m_dataProviderStringTextBox.Text = dataProviderString;
            m_encryptCheckBox.IsChecked = encrypt;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the connection string the user specified in the <see cref="AdvancedSettingsWindow"/>.
        /// </summary>
        public string ConnectionString => m_connectionStringTextBox.Text;

        /// <summary>
        /// Gets the data provider string the user specified in the <see cref="AdvancedSettingsWindow"/>.
        /// </summary>
        public string DataProviderString => m_dataProviderStringTextBox.Text;

        /// <summary>
        /// Gets a value to determine whether the user has selected to encrypt connection strings.
        /// </summary>
        public bool Encrypt => m_encryptCheckBox.IsChecked.Value;

        #endregion

        #region [ Methods ]

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        #endregion
    }
}