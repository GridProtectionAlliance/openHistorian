//******************************************************************************************************
//  ReplicationProviderBase.cs - Gbtc
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
//  11/04/2009 - Pinal C. Patel
//       Generated original version of source code.
//  11/17/2009 - Pinal C. Patel
//       Made Initialize(), SaveSettings() and LoadSettings() overridable in a derived class.
//  03/30/2010 - Pinal C. Patel
//       Corrected the usage of Enabled in Replicate().
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Configuration;
using System.Threading;
using TVA;
using TVA.Configuration;

namespace TimeSeriesArchiver.Replication
{
    /// <summary>
    /// Base class for a provider of replication mechanism for the <see cref="IArchive"/>.
    /// </summary>
    public abstract class ReplicationProviderBase : IReplicationProvider
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Occurs when the process of replicating the <see cref="IArchive"/> is started.
        /// </summary>
        public event EventHandler ReplicationStart;

        /// <summary>
        /// Occurs when the process of replicating the <see cref="IArchive"/> is complete.
        /// </summary>
        public event EventHandler ReplicationComplete;

        /// <summary>
        /// Occurs when an <see cref="Exception"/> is encountered during the replication process of <see cref="IArchive"/>.
        /// </summary>
        public event EventHandler<EventArgs<Exception>> ReplicationException;

        /// <summary>
        /// Occurs when the <see cref="IArchive"/> is being replicated.
        /// </summary>
        public event EventHandler<EventArgs<ProcessProgress<int>>> ReplicationProgress;

        // Fields
        private string m_archiveLocation;
        private string m_replicaLocation;
        private int m_replicationInterval;
        private bool m_persistSettings;
        private string m_settingsCategory;
        private Thread m_replicationThread;
        private System.Timers.Timer m_replicationTimer;
        private bool m_enabled;
        private bool m_disposed;
        private bool m_initialized;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the replication provider.
        /// </summary>
        protected ReplicationProviderBase()
        {
            m_replicationInterval = -1;
            m_persistSettings = true;
            m_settingsCategory = this.GetType().Name;
        }

        /// <summary>
        /// Releases the unmanaged resources before the replication provider is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~ReplicationProviderBase()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the primary location of the <see cref="IArchive"/>.
        /// </summary>
        public string ArchiveLocation
        {
            get
            {
                return m_archiveLocation;
            }
            set
            {
                m_archiveLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the mirrored location of the <see cref="IArchive"/>.
        /// </summary>
        public string ReplicaLocation
        {
            get
            {
                return m_replicaLocation;
            }
            set
            {
                m_replicaLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the interval in minutes at which the <see cref="IArchive"/> is to be replicated.
        /// </summary>
        public int ReplicationInterval
        {
            get
            {
                return m_replicationInterval;
            }
            set
            {
                if (value < 0)
                    m_replicationInterval = -1;
                else
                    m_replicationInterval = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the replication provider settings are to be saved to the config file.
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
        /// Gets or sets the category under which the replication provider settings are to be saved to the config file if the <see cref="PersistSettings"/> property is set to true.
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

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the replication provider is currently enabled.
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

        #endregion

        #region [ Methods ]

        #region [ Abstract ]

        /// <summary>
        /// When overridden in a derived class, replicates the <see cref="IArchive"/>.
        /// </summary>
        protected abstract void ReplicateArchive();

        #endregion

        /// <summary>
        /// Releases all the resources used by the replication provider.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the replication provider.
        /// </summary>
        public virtual void Initialize()
        {
            if (!m_initialized)
            {
                // Load settings from the config file.
                LoadSettings();
                // Start timer for periodic replication.
                if (m_enabled && m_replicationInterval > 0)
                {
                    m_replicationTimer = new System.Timers.Timer(m_replicationInterval * 60000);
                    m_replicationTimer.Elapsed += ReplicationTimer_Elapsed;
                    m_replicationTimer.Start();
                }
                // Initialize only once.
                m_initialized = true;
            }
        }

        /// <summary>
        /// Saves replication provider settings to the config file if the <see cref="PersistSettings"/> property is set to true.
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
                settings["ArchiveLocation", true].Update(m_archiveLocation);
                settings["ReplicaLocation", true].Update(m_replicaLocation);
                settings["ReplicationInterval", true].Update(m_replicationInterval);
                config.Save();
            }
        }

        /// <summary>
        /// Loads saved replication provider settings from the config file if the <see cref="PersistSettings"/> property is set to true.
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
                settings.Add("Enabled", m_enabled, "True if this replication provider is enabled; otherwise False.");
                settings.Add("ArchiveLocation", m_archiveLocation, "Path to the primary location of time-series data archive.");
                settings.Add("ReplicaLocation", m_replicaLocation, "Path to the mirrored location of time-series data archive.");
                settings.Add("ReplicationInterval", m_replicationInterval, "Interval in minutes at which the time-series data archive is to be replicated.");
                Enabled = settings["Enabled"].ValueAs(m_enabled);
                ArchiveLocation = settings["ArchiveLocation"].ValueAs(m_archiveLocation);
                ReplicaLocation = settings["ReplicaLocation"].ValueAs(m_replicaLocation);
                ReplicationInterval = settings["ReplicationInterval"].ValueAs(m_replicationInterval);
            }
        }

        /// <summary>
        /// Replicates the <see cref="IArchive"/>.
        /// </summary>
        /// <returns>true if the replication is successful; otherwise false.</returns>
        /// <exception cref="ArgumentNullException"><see cref="ArchiveLocation"/> or <see cref="ReplicaLocation"/> is null or empty string.</exception>
        public bool Replicate()
        {
            if (!m_enabled || (m_replicationThread != null && m_replicationThread.IsAlive))
                return false;

            if (string.IsNullOrEmpty(m_archiveLocation))
                throw new ArgumentNullException("ArchiveLocation");

            if (string.IsNullOrEmpty(m_replicaLocation))
                throw new ArgumentNullException("ReplicaLocation");

            m_replicationThread = new Thread(ReplicateInternal);
            m_replicationThread.Start();
            m_replicationThread.Join();

            return true;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the replication provider and optionally releases the managed resources.
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

                        if (m_replicationThread != null)
                            m_replicationThread.Abort();

                        if (m_replicationTimer != null)
                        {
                            m_replicationTimer.Elapsed -= ReplicationTimer_Elapsed;
                            m_replicationTimer.Dispose();
                        }
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ReplicationStart"/> event.
        /// </summary>
        protected virtual void OnReplicationStart()
        {
            if (ReplicationStart != null)
                ReplicationStart(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="ReplicationComplete"/> event.
        /// </summary>
        protected virtual void OnReplicationComplete()
        {
            if (ReplicationComplete != null)
                ReplicationComplete(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="ReplicationException"/> event.
        /// </summary>
        /// <param name="ex"><see cref="Exception"/> to send to <see cref="ReplicationException"/> event.</param>
        protected virtual void OnReplicationException(Exception ex)
        {
            if (ReplicationException != null)
                ReplicationException(this, new EventArgs<Exception>(ex));
        }

        /// <summary>
        /// Raises the <see cref="ReplicationProgress"/> event.
        /// </summary>
        /// <param name="replicationProgress"><see cref="ProcessProgress{T}"/> to send to <see cref="ReplicationProgress"/> event.</param>
        protected virtual void OnReplicationProgress(ProcessProgress<int> replicationProgress)
        {
            if (ReplicationProgress != null)
                ReplicationProgress(this, new EventArgs<ProcessProgress<int>>(replicationProgress));
        }

        private void ReplicateInternal()
        {
            try
            {
                OnReplicationStart();
                ReplicateArchive();
                OnReplicationComplete();
            }
            catch (Exception ex)
            {
                OnReplicationException(ex);
            }
        }

        private void ReplicationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Replicate();
        }

        #endregion
    }
}
