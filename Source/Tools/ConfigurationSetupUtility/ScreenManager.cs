//******************************************************************************************************
//  ScreenManager.cs - Gbtc
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

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// Manages the screens displayed to the user in the Configuration Setup Utility.
    /// </summary>
    public class ScreenManager
    {
        #region [ Members ]

        // Fields
        private readonly Window m_mainWindow;
        private IScreen m_currentScreen;
        private readonly Stack<IScreen> m_history;
        private readonly Dictionary<string, object> m_state;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="ScreenManager"/> class.
        /// </summary>
        public ScreenManager(Window mainWindow, IScreen startScreen)
        {
            m_mainWindow = mainWindow;
            m_currentScreen = startScreen;
            m_history = new Stack<IScreen>();
            m_state = new Dictionary<string, object>();

            m_state.Add("screenManager", this);
            m_currentScreen.UpdateNavigation = UpdateNavigation;
            m_currentScreen.State = m_state;

            UpdateScreenPanel();
            UpdateNavigation();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the screen currently displayed in the setup window.
        /// </summary>
        public IScreen CurrentScreen => m_currentScreen;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Advances the setup window to the next screen, storing the current screen in the history.
        /// </summary>
        public void GoToNextScreen()
        {
            GoToNextScreen(true);
        }

        /// <summary>
        /// Advances the setup window to the next screen.
        /// </summary>
        /// <param name="storeHistory">Determines whether the <see cref="ScreenManager"/> should store the current screen in history.</param>
        public void GoToNextScreen(bool storeHistory)
        {
            if (m_currentScreen.NextScreen != null && m_currentScreen.CanGoForward && m_currentScreen.UserInputIsValid)
            {
                if (storeHistory)
                    m_history.Push(m_currentScreen);

                m_currentScreen = m_currentScreen.NextScreen;
                m_currentScreen.UpdateNavigation = UpdateNavigation;
                m_currentScreen.State = m_state;
                UpdateScreenPanel();
                UpdateNavigation();
            }
        }

        /// <summary>
        /// Returns the setup window to the previous screen.
        /// </summary>
        public void GoToPreviousScreen()
        {
            if (m_currentScreen.CanGoBack && m_history.Count > 0)
            {
                m_currentScreen = m_history.Pop();
                m_currentScreen.State = m_state;
                UpdateScreenPanel();
                UpdateNavigation();
            }
        }

        /// <summary>
        /// Updates the screen panel to display the current page.
        /// </summary>
        public void UpdateScreenPanel()
        {
            if (m_mainWindow.FindName("m_screenPanel") is Panel screenPanel)
            {
                screenPanel.Children.Clear();

                if (m_currentScreen is UIElement currentPage)
                    screenPanel.Children.Add(currentPage);
            }
        }

        /// <summary>
        /// Updates the navigation buttons on the main window based on the
        /// navigation settings of the current screen.
        /// </summary>
        public void UpdateNavigation()
        {
            if (!m_mainWindow.Dispatcher.CheckAccess())
                m_mainWindow.Dispatcher.Invoke(new Action(UpdateNavigation), null);
            else
            {
                if (m_mainWindow.FindName("m_nextButton") is Button nextButton)
                {
                    nextButton.IsEnabled = m_currentScreen.CanGoForward;
                    nextButton.Content = m_currentScreen.NextScreen is null ? "Finish" : "Next >";
                }

                if (m_mainWindow.FindName("m_backButton") is Button backButton)
                    backButton.IsEnabled = m_currentScreen.CanGoBack && m_history.Count > 0;

                if (m_mainWindow.FindName("m_cancelButton") is Button cancelButton)
                    cancelButton.IsEnabled = m_currentScreen.CanCancel;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if final shut-down operations (via user input validation) succeeded.
        /// </summary>
        public bool PerformShutdownOperations()
        {
            return m_currentScreen.UserInputIsValid;
        }

        /// <summary>
        /// Attempts to bring main window into focus.
        /// </summary>
        public void Activate()
        {
            m_mainWindow.Activate();
        }

        #endregion
    }
}