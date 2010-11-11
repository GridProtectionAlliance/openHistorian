//******************************************************************************************************
//  NotifierBase.cs - Gbtc
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
//  -----------------------------------------------------------------------------------------------------
//  05/26/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  08/06/2009 - Pinal C. Patel
//       Made Initialize() virtual so inheriting classes can override the default behavior.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  11/05/2009 - Pinal C. Patel
//       Modified to abort notify operation during dispose.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Configuration;
using System.Threading;
using TVA;
using TVA.Configuration;

namespace TimeSeriesArchiver.Notifiers
{
    /// <summary>
    /// Base class for a notifier that can process notification messages.
    /// </summary>
    /// <see cref="NotificationTypes"/>
    public abstract class NotifierBase : INotifier
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Occurs when a notification is being sent.
        /// </summary>
        public event EventHandler NotificationSendStart;

        /// <summary>
        /// Occurs when a notification has been sent.
        /// </summary>
        public event EventHandler NotificationSendComplete;

        /// <summary>
        /// Occurs when a timeout is encountered while sending a notification.
        /// </summary>
        public event EventHandler NotificationSendTimeout;

        /// <summary>
        /// Occurs when an <see cref="Exception"/> is encountered while sending a notification.
        /// </summary>
        /// <remarks>
        /// <see cref="EventArgs{T}.Argument"/> is the exception encountered while sending a notification.
        /// </remarks>
        public event EventHandler<EventArgs<Exception>> NotificationSendException;

        // Fields
        private int m_notifyTimeout;
        private NotificationTypes m_notifyOptions;
        private bool m_persistSettings;
        private string m_settingsCategory;
        private Thread m_notifyThread;
        private bool m_enabled;
        private bool m_disposed;
        private bool m_initialized;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the notifier.
        /// </summary>
        /// <param name="notifyOptions"><see cref="NotificationTypes"/> that can be processed by the notifier.</param>
        public NotifierBase(NotificationTypes notifyOptions)
        {
            m_notifyOptions = notifyOptions;
            m_notifyTimeout = 30;
            m_persistSettings = true;
            m_settingsCategory = this.GetType().Name;
        }

        /// <summary>
        /// Releases the unmanaged resources before the notifier is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~NotifierBase()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the number of seconds to wait for <see cref="Notify"/> to complete.
        /// </summary>
        /// <remarks>
        /// Set <see cref="NotifyTimeout"/> to -1 to wait indefinitely on <see cref="Notify"/>.
        /// </remarks>
        public int NotifyTimeout
        {
            get
            {
                return m_notifyTimeout;
            }
            set
            {
                if (value < 1)
                    m_notifyTimeout = -1;
                else
                    m_notifyTimeout = value;
            }
        }

        /// <summary>
        /// Gets or set <see cref="NotificationTypes"/> that can be processed by the notifier.
        /// </summary>
        public NotificationTypes NotifyOptions
        {
            get
            {
                return m_notifyOptions;
            }
            set
            {
                m_notifyOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the notifier is currently enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the notifier settings are to be saved to the config file.
        /// </summary>
        public bool PersistSettings
        {
            get
            {
                return m_persistSettings;
            }
            set
            {
                m_persistSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets the category under which the notifier settings are to be saved to the config file if the <see cref="PersistSettings"/> property is set to true.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is a null or empty string.</exception>
        public string SettingsCategory
        {
            get
            {
                return m_settingsCategory;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                m_settingsCategory = value;
            }
        }

        #endregion

        #region [ Methods ]

        #region [ Abstract ]

        /// <summary>
        /// When overridden in a derived class, processes a <see cref="NotificationTypes.Alarm"/> notification.
        /// </summary>
        /// <param name="subject">Subject matter for the notification.</param>
        /// <param name="message">Brief message for the notification.</param>
        /// <param name="details">Detailed message for the notification.</param>
        protected abstract void NotifyAlarm(string subject, string message, string details);

        /// <summary>
        /// When overridden in a derived class, processes a <see cref="NotificationTypes.Warning"/> notification.
        /// </summary>
        /// <param name="subject">Subject matter for the notification.</param>
        /// <param name="message">Brief message for the notification.</param>
        /// <param name="details">Detailed message for the notification.</param>
        protected abstract void NotifyWarning(string subject, string message, string details);

        /// <summary>
        /// When overridden in a derived class, processes a <see cref="NotificationTypes.Information"/> notification.
        /// </summary>
        /// <param name="subject">Subject matter for the notification.</param>
        /// <param name="message">Brief message for the notification.</param>
        /// <param name="details">Detailed message for the notification.</param>
        protected abstract void NotifyInformation(string subject, string message, string details);

        /// <summary>
        /// When overridden in a derived class, processes a <see cref="NotificationTypes.Heartbeat"/> notification.
        /// </summary>
        /// <param name="subject">Subject matter for the notification.</param>
        /// <param name="message">Brief message for the notification.</param>
        /// <param name="details">Detailed message for the notification.</param>
        protected abstract void NotifyHeartbeat(string subject, string message, string details);

        #endregion

        /// <summary>
        /// Releases all the resources used by the notifier.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the notifier.
        /// </summary>
        public virtual void Initialize()
        {
            if (!m_initialized)
            {
                LoadSettings();         // Load settings from the config file.
                m_initialized = true;   // Initialize only once.
            }
        }

        /// <summary>
        /// Saves notifier settings to the config file if the <see cref="PersistSettings"/> property is set to true.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException"><see cref="SettingsCategory"/> has a value of null or empty string.</exception>
        public virtual void SaveSettings()
        {
            if (m_persistSettings)
            {
                // Ensure that settings category is specified.
                if (string.IsNullOrEmpty(m_settingsCategory))
                    throw new ConfigurationErrorsException("SettingsCategory property has not been set");

                // Save settings under the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[m_settingsCategory];
                settings["Enabled", true].Update(m_enabled);
                settings["NotifyTimeout", true].Update(m_notifyTimeout);
                settings["NotifyOptions", true].Update(m_notifyOptions);
                config.Save();
            }
        }

        /// <summary>
        /// Loads saved notifier settings from the config file if the <see cref="PersistSettings"/> property is set to true.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException"><see cref="SettingsCategory"/> has a value of null or empty string.</exception>
        public virtual void LoadSettings()
        {
            if (m_persistSettings)
            {
                // Ensure that settings category is specified.
                if (string.IsNullOrEmpty(m_settingsCategory))
                    throw new ConfigurationErrorsException("SettingsCategory property has not been set");

                // Load settings from the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[m_settingsCategory];
                settings.Add("Enabled", m_enabled, "True if this notifier is enabled; otherwise False.");
                settings.Add("NotifyTimeout", m_notifyTimeout, "Number of seconds to wait for notification processing to complete.");
                settings.Add("NotifyOptions", m_notifyOptions, "Types of notifications (Information; Warning; Alarm; Heartbeat) to be processed by this notifier.");
                Enabled = settings["Enabled"].ValueAs(m_enabled);
                NotifyTimeout = settings["NotifyTimeout"].ValueAs(m_notifyTimeout);
                NotifyOptions = settings["NotifyOptions"].ValueAs(m_notifyOptions);
            }
        }

        /// <summary>
        /// Process a notification.
        /// </summary>
        /// <param name="subject">Subject matter for the notification.</param>
        /// <param name="message">Brief message for the notification.</param>
        /// <param name="details">Detailed message for the notification.</param>
        /// <param name="notificationType">One of the <see cref="NotificationTypes"/> values.</param>
        /// <returns>true if notification is processed successfully; otherwise false.</returns>
        public bool Notify(string subject, string message, string details, NotificationTypes notificationType)
        {
            if (!m_enabled || (m_notifyThread != null && m_notifyThread.IsAlive))
                return false;

            // Start notification thread with appropriate parameters.
            m_notifyThread = new Thread(NotifyInternal);
            if ((notificationType & NotificationTypes.Alarm) == NotificationTypes.Alarm &&
                (m_notifyOptions & NotificationTypes.Alarm) == NotificationTypes.Alarm)
                // Alarm notifications are supported.
                m_notifyThread.Start(new object[] { new Action<string, string, string>(NotifyAlarm) , subject, message, details});
            else if ((notificationType & NotificationTypes.Warning) == NotificationTypes.Warning && 
                     (m_notifyOptions & NotificationTypes.Warning) == NotificationTypes.Warning)
                // Warning notifications are supported.
                m_notifyThread.Start(new object[] { new Action<string, string, string>(NotifyWarning), subject, message, details });
            else if ((notificationType & NotificationTypes.Information) == NotificationTypes.Information && 
                     (m_notifyOptions & NotificationTypes.Information) == NotificationTypes.Information)
                // Information notifications are supported.
                m_notifyThread.Start(new object[] { new Action<string, string, string>(NotifyInformation), subject, message, details });
            else if ((notificationType & NotificationTypes.Heartbeat) == NotificationTypes.Heartbeat && 
                     (m_notifyOptions & NotificationTypes.Heartbeat) == NotificationTypes.Heartbeat)
                // Heartbeat notifications are supported.
                m_notifyThread.Start(new object[] { new Action<string, string, string>(NotifyHeartbeat), subject, message, details });
            else
                // Specified notification type is not supported.
                return false;

            if (m_notifyTimeout < 1)
            {
                // Wait indefinetely on the refresh.
                m_notifyThread.Join(Timeout.Infinite);
            }
            else
            {
                // Wait for the specified time on refresh.
                if (!m_notifyThread.Join(m_notifyTimeout * 1000))
                {
                    m_notifyThread.Abort();

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the notifier and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                        SaveSettings();

                        if (m_notifyThread != null)
                            m_notifyThread.Abort();
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="NotificationSendStart"/> event.
        /// </summary>
        protected virtual void OnNotificationSendStart()
        {
            if (NotificationSendStart != null)
                NotificationSendStart(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="NotificationSendComplete"/> event.
        /// </summary>
        protected virtual void OnNotificationSendComplete()
        {
            if (NotificationSendComplete != null)
                NotificationSendComplete(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="NotificationSendTimeout"/> event.
        /// </summary>
        protected virtual void OnNotificationSendTimeout()
        {
            if (NotificationSendTimeout != null)
                NotificationSendTimeout(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="NotificationSendException"/> event.
        /// </summary>
        /// <param name="exception"><see cref="Exception"/> to send to <see cref="NotificationSendException"/> event.</param>
        protected virtual void OnNotificationSendException(Exception exception)
        {
            if (NotificationSendException != null)
                NotificationSendException(this, new EventArgs<Exception>(exception));
        }

        private void NotifyInternal(object state)
        {
            try
            {
                // Unpackage the parameters.
                object[] args = (object[])state;
                string subject = args[1].ToString();
                string message = args[2].ToString();
                string details = args[3].ToString();
                Action<string, string, string> target = (Action<string, string, string>)args[0];

                OnNotificationSendStart();
                target(subject, message, details);
                OnNotificationSendComplete();
            }
            catch (ThreadAbortException)
            {
                OnNotificationSendTimeout();
            }
            catch (Exception ex)
            {
                OnNotificationSendException(ex);
            }
        }

        #endregion
    }
}