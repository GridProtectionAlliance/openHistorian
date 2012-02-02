//******************************************************************************************************
//  ReplicationProviderBase.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  11/04/2009 - Pinal C. Patel
//       Generated original version of source code.
//  11/17/2009 - Pinal C. Patel
//       Made Initialize(), SaveSettings() and LoadSettings() overridable in a derived class.
//  03/30/2010 - Pinal C. Patel
//       Corrected the usage of Enabled in Replicate().
//
//******************************************************************************************************

using System;
using System.Configuration;
using System.Threading;
using openHistorian.Archives;
using TVA;
using TVA.Adapters;
using TVA.Configuration;

namespace openHistorian.Replication
{
    /// <summary>
    /// Base class for a provider of replication mechanism for the <see cref="IDataArchive"/>.
    /// </summary>
    public abstract class ReplicationProviderBase : Adapter, IReplicationProvider
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Occurs when the process of replicating the <see cref="IDataArchive"/> is started.
        /// </summary>
        public event EventHandler ReplicationStart;

        /// <summary>
        /// Occurs when the process of replicating the <see cref="IDataArchive"/> is complete.
        /// </summary>
        public event EventHandler ReplicationComplete;

        /// <summary>
        /// Occurs when an <see cref="Exception"/> is encountered during the replication process of <see cref="IDataArchive"/>.
        /// </summary>
        public event EventHandler<EventArgs<Exception>> ReplicationException;

        /// <summary>
        /// Occurs when the <see cref="IDataArchive"/> is being replicated.
        /// </summary>
        public event EventHandler<EventArgs<ProcessProgress<int>>> ReplicationProgress;

        // Fields
        private string m_archiveLocation;
        private string m_replicaLocation;
        private int m_replicationInterval;
        private Thread m_replicationThread;
        private System.Timers.Timer m_replicationTimer;
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
        /// Gets or sets the primary location of the <see cref="IDataArchive"/>.
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
        /// Gets or sets the mirrored location of the <see cref="IDataArchive"/>.
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
        /// Gets or sets the interval in minutes at which the <see cref="IDataArchive"/> is to be replicated.
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

        #endregion

        #region [ Methods ]

        #region [ Abstract ]

        /// <summary>
        /// When overridden in a derived class, replicates the <see cref="IDataArchive"/>.
        /// </summary>
        protected abstract void ReplicateArchive();

        #endregion

        /// <summary>
        /// Initializes the replication provider.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            if (!m_initialized)
            {
                // Start timer for periodic replication.
                if (Enabled && m_replicationInterval > 0)
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
        /// Saves replication provider settings to the config file if the <see cref="Adapter.PersistSettings"/> property is set to true.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException"><see cref="Adapter.SettingsCategory"/> has a value of null or empty string.</exception>
        public override void SaveSettings()
        {
            if (PersistSettings)
            {
                // Ensure that settings category is specified.
                if (string.IsNullOrEmpty(SettingsCategory))
                    throw new ConfigurationErrorsException("SettingsCategory property has not been set");

                // Save settings under the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings["Enabled", true].Update(Enabled);
                settings["ArchiveLocation", true].Update(m_archiveLocation);
                settings["ReplicaLocation", true].Update(m_replicaLocation);
                settings["ReplicationInterval", true].Update(m_replicationInterval);
                config.Save();
            }
        }

        /// <summary>
        /// Loads saved replication provider settings from the config file if the <see cref="Adapter.PersistSettings"/> property is set to true.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException"><see cref="Adapter.SettingsCategory"/> has a value of null or empty string.</exception>
        public override void LoadSettings()
        {
            if (PersistSettings)
            {
                // Ensure that settings category is specified.
                if (string.IsNullOrEmpty(SettingsCategory))
                    throw new ConfigurationErrorsException("SettingsCategory property has not been set");

                // Load settings from the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings.Add("Enabled", Enabled, "True if this replication provider is enabled; otherwise False.");
                settings.Add("ArchiveLocation", m_archiveLocation, "Path to the primary location of time-series data archive.");
                settings.Add("ReplicaLocation", m_replicaLocation, "Path to the mirrored location of time-series data archive.");
                settings.Add("ReplicationInterval", m_replicationInterval, "Interval in minutes at which the time-series data archive is to be replicated.");
                Enabled = settings["Enabled"].ValueAs(Enabled);
                ArchiveLocation = settings["ArchiveLocation"].ValueAs(m_archiveLocation);
                ReplicaLocation = settings["ReplicaLocation"].ValueAs(m_replicaLocation);
                ReplicationInterval = settings["ReplicationInterval"].ValueAs(m_replicationInterval);
            }
        }

        /// <summary>
        /// Replicates the <see cref="IDataArchive"/>.
        /// </summary>
        /// <returns>true if the replication is successful; otherwise false.</returns>
        /// <exception cref="ArgumentNullException"><see cref="ArchiveLocation"/> or <see cref="ReplicaLocation"/> is null or empty string.</exception>
        public bool Replicate()
        {
            if (!Enabled || (m_replicationThread != null && m_replicationThread.IsAlive))
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
        protected override void Dispose(bool disposing)
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
