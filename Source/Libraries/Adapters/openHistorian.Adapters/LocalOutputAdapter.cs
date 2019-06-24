//******************************************************************************************************
//  LocalOutputAdapter.cs - Gbtc
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
//  ----------------------------------------------------------------------------------------------------
//  07/25/2013 - Ritchie
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using GrafanaAdapters;
using GSF;
using GSF.Collections;
using GSF.Configuration;
using GSF.Data;
using GSF.Diagnostics;
using GSF.Historian.DataServices;
using GSF.Historian.Replication;
using GSF.IO;
using GSF.IO.Unmanaged;
using GSF.Snap.Services;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.Units;
using openHistorian.Net;
using openHistorian.Snap;
using Timer = System.Timers.Timer;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents an output adapter that archives measurements to a local archive.
    /// </summary>
    [Description("openHistorian 2.0 (Local): archives measurements to a local in-process openHistorian.")]
    public class LocalOutputAdapter : OutputAdapterBase
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Defines the default listening port for the historian.
        /// </summary>
        public const int DefaultPort = 38402;

        /// <summary>
        /// Defines default vlaue for <see cref="WatchAttachedPaths"/>.
        /// </summary>
        public const bool DefaultWatchAttachedPaths = false;

        /// <summary>
        /// Defines default value for <see cref="DataChannel"/>.
        /// </summary>
        public const string DefaultDataChannel = "port=38402";

        /// <summary>
        /// Defines the default value for <see cref="TargetFileSize"/>.
        /// </summary>
        public const double DefaultTargetFileSize = 2.0D;

        /// <summary>
        /// Defines the default value for <see cref="MaximumArchiveDays"/>.
        /// </summary>
        public const int DefaultMaximumArchiveDays = 0;

        /// <summary>
        /// Defines the default value for <see cref="EnableTimeReasonabilityCheck"/>.
        /// </summary>
        public const bool DefaultEnableTimeReasonabilityCheck = false;

        /// <summary>
        /// Defines the default value for <see cref="PastTimeReasonabilityLimit"/>.
        /// </summary>
        public const double DefaultPastTimeReasonabilityLimit = 43200.0D;

        /// <summary>
        /// Defines the default value for <see cref="FutureTimeReasonabilityLimit"/>.
        /// </summary>
        public const double DefaultFutureTimeReasonabilityLimit = 43200.0D;

        /// <summary>
        /// Defines the default value for <see cref="SwingingDoorCompressionEnabled"/>.
        /// </summary>
        public const bool DefaultSwingingDoorCompressionEnabled = false;

        /// <summary>
        /// Defines the default value for <see cref="DirectoryNamingMode"/>.
        /// </summary>
        public const ArchiveDirectoryMethod DefaultDirectoryNamingMode = ArchiveDirectoryMethod.YearThenMonth;

        /// <summary>
        /// Defines the default value for <see cref="ArchiveCurtailmentInterval"/>.
        /// </summary>
        public const int DefaultArchiveCurtailmentInterval = Time.SecondsPerDay;

        // Fields
        private HistorianIArchive m_archive;
        private HistorianServerDatabaseConfig m_archiveInfo;
        private string m_instanceName;
        private string m_workingDirectory;
        private string[] m_archiveDirectories;
        private string[] m_attachedPaths;
        private bool m_watchAttachedPaths;
        private string m_dataChannel;
        private double m_targetFileSize;
        private int m_maximumArchiveDays;
        private bool m_enableTimeReasonabilityCheck;
        private long m_pastTimeReasonabilityLimit;
        private long m_futureTimeReasonabilityLimit;
        private bool m_swingingDoorCompressionEnabled;
        private ArchiveDirectoryMethod m_directoryNamingMode;
        private DataServices m_dataServices;
        private ReplicationProviders m_replicationProviders;
        private long m_archivedMeasurements;
        private HistorianServer m_server;
        private readonly HistorianKey m_key;
        private readonly HistorianValue m_value;
        private Dictionary<ulong, DataRow> m_measurements;
        private Dictionary<ulong, Tuple<int, int, double>> m_compressionSettings;
        private Dictionary<ulong, Tuple<IMeasurement, IMeasurement, double, double>> m_swingingDoorStates;
        private Timer m_archiveCurtailmentTimer;
        private SafeFileWatcher[] m_attachedPathWatchers;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalOutputAdapter"/> class.
        /// </summary>
        public LocalOutputAdapter()
        {
            m_key = new HistorianKey();
            m_value = new HistorianValue();
            m_swingingDoorStates = new Dictionary<ulong, Tuple<IMeasurement, IMeasurement, double, double>>();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets instance name defined for this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the instance name for the historian. Leave this value blank to default to the adapter name."),
        DefaultValue("")]
        public string InstanceName
        {
            get
            {
                if (string.IsNullOrEmpty(m_instanceName))
                    return Name.ToLower();

                return m_instanceName;
            }
            set
            {
                m_instanceName = value;
            }
        }

        /// <summary>
        /// Gets or sets TCP server based connection string to use for the historian data channel.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines TCP server based connection string to use for the historian data channel."),
        DefaultValue(DefaultDataChannel)]
        public string DataChannel
        {
            get
            {
                return m_dataChannel;
            }
            set
            {
                m_dataChannel = value;
            }
        }

        /// <summary>
        /// Gets or sets the working directory which is used to write working data before it is moved into its permanent file.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the working directory used for working data and intermediate files and before moving data to its permanent location (see ArchiveDirectories). Leave blank to default to \".\\Archive\\\"."),
        DefaultValue("")]
        public string WorkingDirectory
        {
            get
            {
                return m_workingDirectory;
            }
            set
            {
                string localPath = FilePath.GetAbsolutePath(value);

                if (!Directory.Exists(localPath))
                {
                    try
                    {
                        Directory.CreateDirectory(localPath);
                    }
                    catch (Exception ex)
                    {
                        OnProcessException(MessageLevel.Error, new InvalidOperationException($"Failed to create working directory \"{localPath}\": {ex.Message}", ex));
                    }
                }

                m_workingDirectory = localPath;
            }
        }

        /// <summary>
        /// Gets or sets the write directories for the historian.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the write directories for this historian instance. Leave empty to default to WorkingDirectory. Separate multiple directories with a semi-colon."),
        DefaultValue("")]
        public string ArchiveDirectories
        {
            get
            {
                if ((object)m_archiveDirectories != null)
                    return string.Join(";", m_archiveDirectories);

                return "";
            }
            set
            {
                if ((object)value != null)
                {
                    List<string> archivePaths = new List<string>();

                    foreach (string archivePath in value.Split(';'))
                    {
                        string localPath = FilePath.GetAbsolutePath(archivePath.Trim());

                        if (!Directory.Exists(localPath))
                        {
                            try
                            {
                                Directory.CreateDirectory(localPath);
                            }
                            catch (Exception ex)
                            {
                                OnProcessException(MessageLevel.Error, new InvalidOperationException($"Failed to create archive directory \"{localPath}\": {ex.Message}", ex));
                            }
                        }

                        archivePaths.Add(localPath);
                    }

                    m_archiveDirectories = archivePaths.ToArray();
                }
                else
                {
                    m_archiveDirectories = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets directory naming mode for archive directory files.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the directory naming mode for archive directory files."),
        DefaultValue(DefaultDirectoryNamingMode)]
        public ArchiveDirectoryMethod DirectoryNamingMode
        {
            get
            {
                return m_directoryNamingMode;
            }
            set
            {
                m_directoryNamingMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the default interval, in seconds, over which the archive curtailment will operate. Set to zero to disable.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the default interval, in seconds, over which the archive curtailment will operate. Set to zero to disable."),
        DefaultValue(DefaultArchiveCurtailmentInterval)]
        public int ArchiveCurtailmentInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the directories and/or individual files to attach to the historian.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the directories and/or individual files to attach to this historian instance. Separate multiple paths with a semi-colon."),
        DefaultValue("")]
        public string AttachedPaths
        {
            get
            {
                if ((object)m_attachedPaths != null)
                    return string.Join(";", m_attachedPaths);

                return "";
            }
            set
            {
                if ((object)value != null)
                {
                    List<string> attachedPaths = new List<string>();

                    foreach (string archivePath in value.Split(';'))
                    {
                        string localPath = FilePath.GetAbsolutePath(archivePath);

                        if (Directory.Exists(localPath) || File.Exists(localPath))
                            attachedPaths.Add(localPath);
                        else
                            OnProcessException(MessageLevel.Error, new InvalidOperationException($"Failed to locate \"{localPath}\""));
                    }

                    m_attachedPaths = attachedPaths.ToArray();
                }
                else
                {
                    m_attachedPaths = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the flag which determines whether to set up file watchers to monitor the attached paths.
        /// </summary>
        [ConnectionStringParameter,
        Description("Determines whether to set up file watchers to monitor the attached paths."),
        DefaultValue(DefaultWatchAttachedPaths)]
        public bool WatchAttachedPaths
        {
            get
            {
                return m_watchAttachedPaths;
            }
            set
            {
                m_watchAttachedPaths = value;
            }
        }

        /// <summary>
        /// Gets or sets target file size, in GigaBytes.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define desired target file size in GigaBytes."),
        DefaultValue(DefaultTargetFileSize)]
        public double TargetFileSize
        {
            get
            {
                return m_targetFileSize;
            }
            set
            {
                if (value < 0.1D || value > SI2.Tera)
                    throw new ArgumentOutOfRangeException(nameof(value));

                m_targetFileSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of days of data to maintain in the archive.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the maximum number of days of data to maintain, i.e., any archives files with data older than current date minus value will be deleted daily. Defaults to zero meaning no maximum."),
        DefaultValue(DefaultMaximumArchiveDays)]
        public int MaximumArchiveDays
        {
            get
            {
                return m_maximumArchiveDays;
            }
            set
            {
                m_maximumArchiveDays = value;
            }
        }

        /// <summary>
        /// Gets or sets flag that indicates if incoming timestamps to the historian should be validated for reasonability.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the flag that indicates if incoming timestamps to the historian should be validated for reasonability."),
        DefaultValue(DefaultEnableTimeReasonabilityCheck)]
        public bool EnableTimeReasonabilityCheck
        {
            get
            {
                return m_enableTimeReasonabilityCheck;
            }
            set
            {
                m_enableTimeReasonabilityCheck = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of seconds that a past timestamp, as compared to local clock, will be considered valid.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the maximum number of seconds that a past timestamp, as compared to local clock, will be considered valid."),
        DefaultValue(DefaultPastTimeReasonabilityLimit)]
        public double PastTimeReasonabilityLimit
        {
            get
            {
                return new Ticks(m_pastTimeReasonabilityLimit).ToSeconds();
            }
            set
            {
                m_pastTimeReasonabilityLimit = Ticks.FromSeconds(Math.Abs(value));
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of seconds that a future timestamp, as compared to local clock, will be considered valid.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the maximum number of seconds that a future timestamp, as compared to local clock, will be considered valid."),
        DefaultValue(DefaultFutureTimeReasonabilityLimit)]
        public double FutureTimeReasonabilityLimit
        {
            get
            {
                return new Ticks(m_futureTimeReasonabilityLimit).ToSeconds();
            }
            set
            {
                m_futureTimeReasonabilityLimit = Ticks.FromSeconds(Math.Abs(value));
            }
        }

        /// <summary>
        /// Gets or sets the flag that determines if swinging door compression is enabled for this historian instance.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the flag that determines if swinging door compression is enabled for this historian instance."),
        DefaultValue(DefaultSwingingDoorCompressionEnabled)]
        public bool SwingingDoorCompressionEnabled
        {
            get
            {
                return m_swingingDoorCompressionEnabled;
            }
            set
            {
                m_swingingDoorCompressionEnabled = value;
            }
        }

        /// <summary>
        /// Returns a flag that determines if measurements sent to this <see cref="LocalOutputAdapter"/> are destined for archival.
        /// </summary>
        public override bool OutputIsForArchive => true;

        /// <summary>
        /// Gets flag that determines if this <see cref="LocalOutputAdapter"/> uses an asynchronous connection.
        /// </summary>
        protected override bool UseAsyncConnect => true;

        /// <summary>
        /// Gets or sets <see cref="DataSet" /> based data source available to this <see cref="LocalOutputAdapter" />.
        /// </summary>
        public override DataSet DataSource
        {
            get
            {
                return base.DataSource;
            }

            set
            {
                base.DataSource = value;

                if ((object)value == null)
                    return;

                Dictionary<ulong, DataRow> measurements = new Dictionary<ulong, DataRow>();
                string instanceName = InstanceName;

                // Create dictionary of metadata for this server instance
                foreach (DataRow row in value.Tables["ActiveMeasurements"].Rows)
                {
                    MeasurementKey key;

                    if (MeasurementKey.TryParse(row["ID"].ToString(), out key) && (key.Source?.Equals(instanceName, StringComparison.OrdinalIgnoreCase) ?? false))
                        measurements[key.ID] = row;
                }

                Dictionary<ulong, Tuple<int, int, double>> compressionSettings = new Dictionary<ulong, Tuple<int, int, double>>();

                if (value.Tables.Contains("CompressionSettings"))
                {
                    // Extract compression settings for defined measurements
                    foreach (DataRow row in value.Tables["CompressionSettings"].Rows)
                    {
                        uint pointID = row.ConvertField<uint>("PointID");

                        if (InputMeasurementKeys.Any(key => key.ID == pointID))
                        {
                            // Get compression settings
                            int compressionMinTime = row.ConvertField<int>("CompressionMinTime");
                            int compressionMaxTime = row.ConvertField<int>("CompressionMaxTime");
                            double compressionLimit = row.ConvertField<double>("CompressionLimit");

                            compressionSettings[pointID] = new Tuple<int, int, double>(compressionMinTime, compressionMaxTime, compressionLimit);
                        }
                    }
                }

                Interlocked.Exchange(ref m_measurements, measurements);
                Interlocked.Exchange(ref m_compressionSettings, compressionSettings);

                // When metadata is updated for an output adapter, reset sliding memory caches for Grafana data sources
                TargetCaches.ResetAll();
            }
        }

        /// <summary>
        /// Returns the detailed status of the data output source.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.Append(base.Status);
                status.AppendLine();

                status.AppendFormat("   Historian instance name: {0}\r\n", InstanceName);
                status.AppendFormat("         Working directory: {0}\r\n", FilePath.TrimFileName(WorkingDirectory, 51));
                status.AppendFormat("      Network data channel: {0}\r\n", DataChannel.ToNonNullString(DefaultDataChannel));
                status.AppendFormat("          Target file size: {0:N4}GB\r\n", TargetFileSize);
                status.AppendFormat("     Directory naming mode: {0}\r\n", DirectoryNamingMode);
                status.AppendFormat("       Disk flush interval: {0:N0}ms\r\n", m_archiveInfo.DiskFlushInterval);
                status.AppendFormat("      Cache flush interval: {0:N0}ms\r\n", m_archiveInfo.CacheFlushInterval);
                status.AppendFormat("             Staging count: {0:N0}\r\n", m_archiveInfo.StagingCount);
                status.AppendFormat("          Memory pool size: {0:N4}GB\r\n", Globals.MemoryPool.MaximumPoolSize / SI2.Giga);
                status.AppendFormat("      Maximum archive days: {0}\r\n", MaximumArchiveDays < 1 ? "No limit" : MaximumArchiveDays.ToString("N0"));
                status.AppendFormat("  Time reasonability check: {0}\r\n", EnableTimeReasonabilityCheck ? "Enabled" : "Not Enabled");
                status.AppendFormat(" Archive curtailment timer: {0}\r\n", Time.ToElapsedTimeString(ArchiveCurtailmentInterval, 0));

                if (EnableTimeReasonabilityCheck)
                {
                    status.AppendFormat("   Maximum past time limit: {0:N4}s, i.e., {1}\r\n", PastTimeReasonabilityLimit, new Ticks(m_pastTimeReasonabilityLimit).ToElapsedTimeString(4));
                    status.AppendFormat(" Maximum future time limit: {0:N4}s, i.e., {1}\r\n", FutureTimeReasonabilityLimit, new Ticks(m_futureTimeReasonabilityLimit).ToElapsedTimeString(4));
                }

                if ((object)m_dataServices != null)
                    status.Append(m_dataServices.Status);

                if ((object)m_replicationProviders != null)
                    status.Append(m_replicationProviders.Status);

                if ((object)m_server != null && (object)m_server.Host != null)
                    m_server.Host?.GetFullStatus(status);

                return status.ToString();
            }
        }

        /// <summary>
        /// Historian server instance.
        /// </summary>
        public HistorianServer Server => m_server;

        /// <summary>
        /// Active measurement metadata dictionary.
        /// </summary>
        public Dictionary<ulong, DataRow> Measurements => m_measurements;

        private SafeFileWatcher[] AttachedPathWatchers
        {
            get
            {
                return m_attachedPathWatchers;
            }
            set
            {
                if ((object)m_attachedPathWatchers != null)
                {
                    foreach (SafeFileWatcher watcher in m_attachedPathWatchers)
                    {
                        watcher.EnableRaisingEvents = false;
                        watcher.Dispose();
                    }
                }

                m_attachedPathWatchers = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by this <see cref="LocalOutputAdapter"/> and optionally releases the managed resources.
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
                        AttachedPathWatchers = null;

                        if ((object)m_dataServices != null)
                        {
                            m_dataServices.AdapterCreated -= DataServices_AdapterCreated;
                            m_dataServices.AdapterLoaded -= DataServices_AdapterLoaded;
                            m_dataServices.AdapterUnloaded -= DataServices_AdapterUnloaded;
                            m_dataServices.AdapterLoadException -= AdapterLoader_AdapterLoadException;
                            m_dataServices.Dispose();
                        }

                        if ((object)m_replicationProviders != null)
                        {
                            m_replicationProviders.AdapterCreated -= ReplicationProviders_AdapterCreated;
                            m_replicationProviders.AdapterLoaded -= ReplicationProviders_AdapterLoaded;
                            m_replicationProviders.AdapterUnloaded -= ReplicationProviders_AdapterUnloaded;
                            m_replicationProviders.AdapterLoadException -= AdapterLoader_AdapterLoadException;
                            m_replicationProviders.Dispose();
                        }

                        if ((object)m_archiveCurtailmentTimer != null)
                        {
                            m_archiveCurtailmentTimer.Stop();
                            m_archiveCurtailmentTimer.Elapsed -= m_archiveCurtailmentTimerElapsed;
                            m_archiveCurtailmentTimer.Dispose();
                        }
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Initializes this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //const string errorMessage = "{0} is missing from Settings - Example: instanceName=default; ArchiveDirectories={{c:\\Archive1\\;d:\\Backups2\\}}; dataChannel={{port=9591; interface=0.0.0.0}}";
            Dictionary<string, string> settings = Settings;
            string setting;
            double value;

            // Validate settings.
            if (!settings.TryGetValue("instanceName", out m_instanceName) || string.IsNullOrWhiteSpace(m_instanceName))
                m_instanceName = Name;

            // Track instance in static dictionary
            Instances[InstanceName] = this;

            if (!settings.TryGetValue("WorkingDirectory", out setting) || string.IsNullOrEmpty(setting))
                setting = "Archive";

            WorkingDirectory = setting;

            if (settings.TryGetValue("ArchiveDirectories", out setting))
                ArchiveDirectories = setting;

            if (settings.TryGetValue("ArchiveCurtailmentInterval", out setting))
                ArchiveCurtailmentInterval = int.Parse(setting);
            else
                ArchiveCurtailmentInterval = DefaultArchiveCurtailmentInterval;

            if (settings.TryGetValue("AttachedPaths", out setting))
                AttachedPaths = setting;

            if (settings.TryGetValue("WatchAttachedPaths", out setting))
                m_watchAttachedPaths = setting.ParseBoolean();
            else
                m_watchAttachedPaths = DefaultWatchAttachedPaths;

            if (!settings.TryGetValue("DataChannel", out m_dataChannel))
                m_dataChannel = DefaultDataChannel;

            double targetFileSize;

            if (!settings.TryGetValue("TargetFileSize", out setting) || !double.TryParse(setting, out targetFileSize))
                targetFileSize = DefaultTargetFileSize;

            if (targetFileSize < 0.1D || targetFileSize > SI2.Tera)
                targetFileSize = DefaultTargetFileSize;

            if (!settings.TryGetValue("MaximumArchiveDays", out setting) || !int.TryParse(setting, out m_maximumArchiveDays))
                m_maximumArchiveDays = DefaultMaximumArchiveDays;

            if (settings.TryGetValue("EnableTimeReasonabilityCheck", out setting))
                m_enableTimeReasonabilityCheck = setting.ParseBoolean();
            else
                m_enableTimeReasonabilityCheck = DefaultEnableTimeReasonabilityCheck;

            if (settings.TryGetValue("PastTimeReasonabilityLimit", out setting) && double.TryParse(setting, out value))
                PastTimeReasonabilityLimit = value;
            else
                PastTimeReasonabilityLimit = DefaultPastTimeReasonabilityLimit;

            if (settings.TryGetValue("FutureTimeReasonabilityLimit", out setting) && double.TryParse(setting, out value))
                FutureTimeReasonabilityLimit = value;
            else
                FutureTimeReasonabilityLimit = DefaultFutureTimeReasonabilityLimit;

            if (settings.TryGetValue("SwingingDoorCompressionEnabled", out setting))
                SwingingDoorCompressionEnabled = setting.ParseBoolean();
            else
                SwingingDoorCompressionEnabled = DefaultSwingingDoorCompressionEnabled;

            if (!settings.TryGetValue("DirectoryNamingMode", out setting) || !Enum.TryParse(setting, true, out m_directoryNamingMode))
                DirectoryNamingMode = DefaultDirectoryNamingMode;

            // Handle advanced settings - there are hidden but available from manual entry into connection string
            int stagingCount, diskFlushInterval, cacheFlushInterval;

            if (!settings.TryGetValue("StagingCount", out setting) || !int.TryParse(setting, out stagingCount))
                stagingCount = 3;

            if (!settings.TryGetValue("DiskFlushInterval", out setting) || !int.TryParse(setting, out diskFlushInterval))
                diskFlushInterval = 10000;

            if (!settings.TryGetValue("CacheFlushInterval", out setting) || !int.TryParse(setting, out cacheFlushInterval))
                cacheFlushInterval = 100;

            // Establish archive information for this historian instance
            m_archiveInfo = new HistorianServerDatabaseConfig(InstanceName, WorkingDirectory, true);

            if ((object)m_archiveDirectories != null)
                m_archiveInfo.FinalWritePaths.AddRange(m_archiveDirectories);

            if ((object)m_attachedPaths != null)
                m_archiveInfo.ImportPaths.AddRange(m_attachedPaths);

            m_archiveInfo.ImportAttachedPathsAtStartup = false;
            m_archiveInfo.TargetFileSize = (long)(targetFileSize * SI.Giga);
            m_archiveInfo.DirectoryMethod = DirectoryNamingMode;
            m_archiveInfo.StagingCount = stagingCount;
            m_archiveInfo.DiskFlushInterval = diskFlushInterval;
            m_archiveInfo.CacheFlushInterval = cacheFlushInterval;

            // Provide web service support
            m_dataServices = new DataServices();
            m_dataServices.AdapterCreated += DataServices_AdapterCreated;
            m_dataServices.AdapterLoaded += DataServices_AdapterLoaded;
            m_dataServices.AdapterUnloaded += DataServices_AdapterUnloaded;
            m_dataServices.AdapterLoadException += AdapterLoader_AdapterLoadException;

            // Provide archive replication support
            m_replicationProviders = new ReplicationProviders();
            m_replicationProviders.AdapterCreated += ReplicationProviders_AdapterCreated;
            m_replicationProviders.AdapterLoaded += ReplicationProviders_AdapterLoaded;
            m_replicationProviders.AdapterUnloaded += ReplicationProviders_AdapterUnloaded;
            m_replicationProviders.AdapterLoadException += AdapterLoader_AdapterLoadException;

            if (MaximumArchiveDays > 0)
            {
                m_archiveCurtailmentTimer = new Timer(ArchiveCurtailmentInterval * 1000.0D);
                m_archiveCurtailmentTimer.AutoReset = true;
                m_archiveCurtailmentTimer.Elapsed += m_archiveCurtailmentTimerElapsed;
                m_archiveCurtailmentTimer.Enabled = true;
            }

            // Initialize the file watchers for attached paths
            if (m_watchAttachedPaths)
                AttachedPathWatchers = m_attachedPaths?.Select(WatchPath).ToArray();
            else
                AttachedPathWatchers = null;
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        /// <param name="maxLength">Maximum length of the status message.</param>
        /// <returns>Text of the status message.</returns>
        public override string GetShortStatus(int maxLength)
        {
            return $"Archived {m_archivedMeasurements} measurements.".CenterText(maxLength);
        }

        /// <summary>
        /// Detaches an archive file from the historian.
        /// </summary>
        /// <param name="fileName">Archive file name to detach.</param>
        [AdapterCommand("Detaches an archive file from the historian. Wild cards are allowed in file name and folders to handle multiple files.", "Administrator", "Editor")]
        public void DetachFile(string fileName)
        {
            ClientDatabaseBase<HistorianKey, HistorianValue> database = GetClientDatabase();

            if (fileName.Contains('*'))
                ExecuteWildCardFileOperation(database, Path.GetFullPath(fileName), database.DetatchFiles);
            else
                ExecuteFileOperation(database, Path.GetFullPath(fileName), database.DetatchFiles);
        }

        /// <summary>
        /// Deletes an archive file from the historian.
        /// </summary>
        /// <param name="fileName">Archive file name to delete.</param>
        [AdapterCommand("Deletes an archive file from the historian. Wild cards are allowed in file name and folders to handle multiple files.", "Administrator", "Editor")]
        public void DeleteFile(string fileName)
        {
            ClientDatabaseBase<HistorianKey, HistorianValue> database = GetClientDatabase();

            if (fileName.Contains('*'))
                ExecuteWildCardFileOperation(database, Path.GetFullPath(fileName), database.DeleteFiles);
            else
                ExecuteFileOperation(database, Path.GetFullPath(fileName), database.DeleteFiles);
        }

        /// <summary>
        /// Detaches files in an archive folder from the historian.
        /// </summary>
        /// <param name="folderName">Archive folder name to detach.</param>
        [AdapterCommand("Detaches all archive files in a specified folder from the historian.", "Administrator", "Editor")]
        public void DetachFolder(string folderName)
        {
            ClientDatabaseBase<HistorianKey, HistorianValue> database = GetClientDatabase();
            ExecuteFolderOperation(database, FilePath.GetDirectoryName(Path.GetFullPath(folderName)), database.DetatchFiles);
        }

        /// <summary>
        /// Deletes files in an archive folder from the historian.
        /// </summary>
        /// <param name="folderName">Archive folder name to delete.</param>
        [AdapterCommand("Deletes all archive files in a specified folder from the historian.", "Administrator", "Editor")]
        public void DeleteFolder(string folderName)
        {
            ClientDatabaseBase<HistorianKey, HistorianValue> database = GetClientDatabase();
            ExecuteFolderOperation(database, FilePath.GetDirectoryName(Path.GetFullPath(folderName)), database.DeleteFiles);
        }

        /// <summary>
        /// Initiates archive file curtailment based on defined maximum archive days.
        /// </summary>
        [AdapterCommand("Initiates archive file curtailment based on defined maximum archive days.", "Administrator", "Editor")]
        public void CurtailArchiveFiles()
        {
            if (MaximumArchiveDays < 1)
            {
                OnStatusMessage(MessageLevel.Info, "Maximum archive days not set, cannot initiate archive file curtailment.");
                return;
            }

            try
            {
                OnStatusMessage(MessageLevel.Info, "Attempting to curtail archive files based on defined maximum archive days...");

                ClientDatabaseBase<HistorianKey, HistorianValue> database = GetClientDatabase();

                // Get list of files that have both a start time and an end time that are greater than the maximum archive days. We check both start and end times
                // since PMUs can provide bad time (not currently being filtered) and you don't want to accidentally delete a file with otherwise in-range data.
                ArchiveDetails[] filesToDelete = database.GetAllAttachedFiles().Where(file => (DateTime.UtcNow - file.StartTime).TotalDays > MaximumArchiveDays && (DateTime.UtcNow - file.EndTime).TotalDays > MaximumArchiveDays).ToArray();
                database.DeleteFiles(filesToDelete.Select(file => file.Id).ToList());
                OnStatusMessage(MessageLevel.Info, $"Deleted the following old archive files:\r\n    {filesToDelete.Select(file => FilePath.TrimFileName(file.FileName, 75)).ToDelimitedString(Environment.NewLine + "    ")}");
            }
            catch (Exception ex)
            {
                OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to limit maximum archive size: {ex.Message}", ex));
            }
        }

        private void ExecuteWildCardFileOperation(ClientDatabaseBase<HistorianKey, HistorianValue> database, string fileName, Action<List<Guid>> fileOperation)
        {
            HashSet<string> sourceFiles = new HashSet<string>(FilePath.GetFileList(fileName).Select(Path.GetFullPath), StringComparer.OrdinalIgnoreCase);
            List<Guid> files = database.GetAllAttachedFiles().Where(file => sourceFiles.Contains(Path.GetFullPath(file.FileName))).Select(file => file.Id).ToList();
            fileOperation(files);
        }

        private void ExecuteFileOperation(ClientDatabaseBase<HistorianKey, HistorianValue> database, string fileName, Action<List<Guid>> fileOperation)
        {
            List<Guid> files = database.GetAllAttachedFiles().Where(file => Path.GetFullPath(file.FileName).Equals(fileName, StringComparison.OrdinalIgnoreCase)).Select(file => file.Id).ToList();
            fileOperation(files);
        }

        private void ExecuteFolderOperation(ClientDatabaseBase<HistorianKey, HistorianValue> database, string folderName, Action<List<Guid>> folderOperation)
        {
            List<Guid> files = database.GetAllAttachedFiles().Where(file => Path.GetFullPath(file.FileName).StartsWith(folderName, StringComparison.OrdinalIgnoreCase)).Select(file => file.Id).ToList();
            folderOperation(files);
        }

        private ClientDatabaseBase<HistorianKey, HistorianValue> GetClientDatabase()
        {
            if ((object)m_archive != null && (object)m_archive.ClientDatabase != null)
                return m_archive.ClientDatabase;

            throw new InvalidOperationException("Cannot execute historian operation, archive database is not open.");
        }

        private void m_archiveCurtailmentTimerElapsed(object sender, ElapsedEventArgs e)
        {
            CurtailArchiveFiles();
        }

        /// <summary>
        /// Attempts to connect to this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        protected override void AttemptConnection()
        {
            // Open archive files
            Dictionary<string, string> settings = m_dataChannel.ParseKeyValuePairs();
            string setting;
            int port;

            if (!settings.TryGetValue("port", out setting) || !int.TryParse(setting, out port))
                port = DefaultPort;

            m_server = new HistorianServer(m_archiveInfo, port);
            m_archive = m_server[InstanceName];

            // Initialization of services needs to occur after files are open
            m_dataServices.Initialize();
            m_replicationProviders.Initialize();

            OnConnected();

            if ((object)m_attachedPathWatchers != null)
            {
                foreach (SafeFileWatcher fileWatcher in m_attachedPathWatchers)
                    fileWatcher.EnableRaisingEvents = true;
            }

            new Thread(AttachArchiveDirectoriesAndAttachedPaths).Start();
        }

        /// <summary>
        /// Attempts to disconnect from this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        protected override void AttemptDisconnection()
        {
            if ((object)m_attachedPathWatchers != null)
            {
                foreach (SafeFileWatcher fileWatcher in m_attachedPathWatchers)
                    fileWatcher.EnableRaisingEvents = false;
            }

            m_archive = null;
            m_server.Dispose();
            m_server = null;

            OnDisconnected();
            m_archivedMeasurements = 0;
        }

        /// <summary>
        /// Archives <paramref name="measurements"/> locally.
        /// </summary>
        /// <param name="measurements">Measurements to be archived.</param>
        /// <exception cref="InvalidOperationException">Local archive is closed.</exception>
        protected override void ProcessMeasurements(IMeasurement[] measurements)
        {
            foreach (IMeasurement measurement in measurements)
            {
                // Validate timestamp reasonability as compared to local clock, when enabled
                if (m_enableTimeReasonabilityCheck)
                {
                    long deviation = DateTime.UtcNow.Ticks - measurement.Timestamp.Value;

                    if (deviation < -m_futureTimeReasonabilityLimit || deviation > m_pastTimeReasonabilityLimit)
                        continue;
                }

                m_key.Timestamp = (ulong)measurement.Timestamp.Value;
                m_key.PointID = measurement.Key.ID;

                // Since current time-series measurements are basically all floats - values fit into first value,
                // this will change as value types for time-series framework expands
                m_value.Value1 = BitConvert.ToUInt64((float)measurement.AdjustedValue);
                m_value.Value3 = (ulong)measurement.StateFlags;

                // Check to see if swinging door compression is enabled
                if (m_swingingDoorCompressionEnabled)
                {
                    Tuple<int, int, double> settings = null;

                    // Attempt to lookup compression settings for this measurement
                    if ((m_compressionSettings?.TryGetValue(m_key.PointID, out settings) ?? false) && (object)settings != null)
                    {
                        // Get compression settings
                        int compressionMinTime = settings.Item1;
                        int compressionMaxTime = settings.Item2;
                        double compressionLimit = settings.Item3;

                        // Get current swinging door compression state, creating state if needed
                        Tuple<IMeasurement, IMeasurement, double, double> state = m_swingingDoorStates.GetOrAdd(m_key.PointID, id => new Tuple<IMeasurement, IMeasurement, double, double>(measurement, measurement, double.MinValue, double.MaxValue));
                        IMeasurement currentData = measurement;
                        IMeasurement archivedData = state.Item1;
                        IMeasurement previousData = state.Item2;
                        double lastHighSlope = state.Item3;
                        double lastLowSlope = state.Item4;
                        double highSlope = 0.0D;
                        double lowSlope = 0.0D;
                        bool archiveData;

                        // Data is to be compressed
                        if (compressionMinTime > 0 && currentData.Timestamp - archivedData.Timestamp < compressionMinTime)
                        {
                            // CompressionMinTime is in effect
                            archiveData = false;
                        }
                        else if (currentData.StateFlags != archivedData.StateFlags || currentData.StateFlags != previousData.StateFlags || (compressionMaxTime > 0 && previousData.Value - archivedData.Timestamp > compressionMaxTime))
                        {
                            // Quality changed or CompressionMaxTime is exceeded
                            archiveData = true;
                        }
                        else
                        {
                            // Perform a compression test
                            highSlope = (currentData.Value - (archivedData.Value + compressionLimit)) / (currentData.Timestamp - archivedData.Timestamp);
                            lowSlope = (currentData.Value - (archivedData.Value - compressionLimit)) / (currentData.Timestamp - archivedData.Timestamp);
                            double slope = (currentData.Value - archivedData.Value) / (currentData.Timestamp - archivedData.Timestamp);

                            if (highSlope >= lastHighSlope)
                                lastHighSlope = highSlope;

                            if (lowSlope <= lastLowSlope)
                                lastLowSlope = lowSlope;

                            archiveData = slope <= lastHighSlope || slope >= lastLowSlope;
                        }

                        // Update swinging door compression state
                        m_swingingDoorStates[m_key.PointID] = new Tuple<IMeasurement, IMeasurement, double, double>
                        (
                            archiveData ? currentData : archivedData,
                            currentData,
                            archiveData ? highSlope: lastHighSlope,
                            archiveData ? lowSlope : lastLowSlope
                        );

                        // Continue to next point if this point does not need to be archived
                        if (!archiveData)
                            continue;
                    }
                }

                m_archive.Write(m_key, m_value);
            }

            m_archivedMeasurements += measurements.Length;
        }

        private void AttachArchiveDirectoriesAndAttachedPaths()
        {
            ClientDatabaseBase<HistorianKey, HistorianValue> clientDatabase = GetClientDatabase();

            string[] archivedirectories = m_archiveDirectories ?? new string[0];
            string[] attachedPaths = m_attachedPaths ?? new string[0];

            List<string> files = archivedirectories
                .Concat(attachedPaths)
                .SelectMany(dir => FilePath.EnumerateFiles(dir))
                .ToList();

            string[] filePtr = new string[1];

            foreach (string file in files)
            {
                if (!Enabled)
                    return;

                filePtr[0] = file;
                clientDatabase.AttachFilesOrPaths(filePtr);
            }
        }

        private SafeFileWatcher WatchPath(string path)
        {
            SafeFileWatcher fileWatcher = new SafeFileWatcher(path, "*.d2*");

            fileWatcher.Created += (sender, args) => ThreadPool.QueueUserWorkItem(state =>
            {
                ClientDatabaseBase<HistorianKey, HistorianValue> clientDatabase = GetClientDatabase();
                string[] filePtr = { (string)state };
                clientDatabase.AttachFilesOrPaths(filePtr);
            }, args.FullPath);

            fileWatcher.InternalBufferSize = 65536;

            return fileWatcher;
        }

        private void DataServices_AdapterCreated(object sender, EventArgs<IDataService> e)
        {
            e.Argument.SettingsCategory = InstanceName.ToLowerInvariant() + e.Argument.SettingsCategory;
        }

        private void DataServices_AdapterLoaded(object sender, EventArgs<IDataService> e)
        {
            e.Argument.Archive = m_archive;
            e.Argument.ServiceProcessException += DataServices_ServiceProcessException;
            OnStatusMessage(MessageLevel.Info, $"{e.Argument.GetType().Name} has been loaded.");
        }

        private void DataServices_AdapterUnloaded(object sender, EventArgs<IDataService> e)
        {
            e.Argument.Archive = null;
            e.Argument.ServiceProcessException -= DataServices_ServiceProcessException;
            OnStatusMessage(MessageLevel.Info, $"{e.Argument.GetType().Name} has been unloaded.");
        }

        private void ReplicationProviders_AdapterCreated(object sender, EventArgs<IReplicationProvider> e)
        {
            e.Argument.SettingsCategory = InstanceName.ToLowerInvariant() + e.Argument.SettingsCategory;
        }

        private void ReplicationProviders_AdapterLoaded(object sender, EventArgs<IReplicationProvider> e)
        {
            e.Argument.ReplicationStart += ReplicationProvider_ReplicationStart;
            e.Argument.ReplicationComplete += ReplicationProvider_ReplicationComplete;
            e.Argument.ReplicationProgress += ReplicationProvider_ReplicationProgress;
            e.Argument.ReplicationException += ReplicationProvider_ReplicationException;
            OnStatusMessage(MessageLevel.Info, $"{e.Argument.GetType().Name} has been loaded.");
        }

        private void ReplicationProviders_AdapterUnloaded(object sender, EventArgs<IReplicationProvider> e)
        {
            e.Argument.ReplicationStart -= ReplicationProvider_ReplicationStart;
            e.Argument.ReplicationComplete -= ReplicationProvider_ReplicationComplete;
            e.Argument.ReplicationProgress -= ReplicationProvider_ReplicationProgress;
            e.Argument.ReplicationException -= ReplicationProvider_ReplicationException;
            OnStatusMessage(MessageLevel.Info, $"{e.Argument.GetType().Name} has been unloaded.");
        }

        private void AdapterLoader_AdapterLoadException(object sender, EventArgs<Exception> e)
        {
            OnProcessException(MessageLevel.Warning, e.Argument);
        }

        private void DataServices_ServiceProcessException(object sender, EventArgs<Exception> e)
        {
            OnProcessException(MessageLevel.Warning, e.Argument);
        }

        private void ReplicationProvider_ReplicationStart(object sender, EventArgs e)
        {
            OnStatusMessage(MessageLevel.Info, $"{sender.GetType().Name} has started archive replication...");
        }

        private void ReplicationProvider_ReplicationComplete(object sender, EventArgs e)
        {
            OnStatusMessage(MessageLevel.Info, $"{sender.GetType().Name} has finished archive replication.");
        }

        private void ReplicationProvider_ReplicationProgress(object sender, EventArgs<ProcessProgress<int>> e)
        {
            OnStatusMessage(MessageLevel.Info, $"{sender.GetType().Name} has replicated archive file {e.Argument.ProgressMessage}.");
        }

        private void ReplicationProvider_ReplicationException(object sender, EventArgs<Exception> e)
        {
            OnProcessException(MessageLevel.Warning, e.Argument);
        }

        #endregion

        #region [ Static ]

        /// <summary>
        /// Accesses local output adapter instances (normally only one).
        /// </summary>
        public static readonly ConcurrentDictionary<string, LocalOutputAdapter> Instances = new ConcurrentDictionary<string, LocalOutputAdapter>(StringComparer.OrdinalIgnoreCase);

        // Static Constructor

        static LocalOutputAdapter()
        {
            CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];

            systemSettings.Add("MemoryPoolSize", "0.0", "The fixed memory pool size in Gigabytes. Leave at zero for dynamically calculated setting.");
            systemSettings.Add("MemoryPoolTargetUtilization", "Low", "The target utilization level for the memory pool. One of 'Low', 'Medium', or 'High'.");

            // Set maximum buffer size
            double memoryPoolSize = systemSettings["MemoryPoolSize"].ValueAs(0.0D);

            if (memoryPoolSize > 0.0D)
                Globals.MemoryPool.SetMaximumBufferSize((long)(memoryPoolSize * SI2.Giga));

            TargetUtilizationLevels targetLevel;

            if (!Enum.TryParse(systemSettings["MemoryPoolTargetUtilization"].Value, false, out targetLevel))
                targetLevel = TargetUtilizationLevels.High;

            Globals.MemoryPool.SetTargetUtilizationLevel(targetLevel);
        }

        // Static Methods

        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local
        private static void OptimizeLocalHistorianSettings(AdoDataConnection connection, string nodeIDQueryString, ulong trackingVersion, string arguments, Action<string> statusMessage, Action<Exception> processException)
        {
            // Make sure setting exists to allow user to by-pass local historian optimizations at startup
            ConfigurationFile configFile = ConfigurationFile.Current;
            CategorizedSettingsElementCollection settings = configFile.Settings["systemSettings"];
            settings.Add("OptimizeLocalHistorianSettings", true, "Determines if the defined local historians will have their settings optimized at startup");

            // See if this node should optimize local historian settings
            if (settings["OptimizeLocalHistorianSettings"].ValueAsBoolean())
            {
                statusMessage("Optimizing settings for local historians...");

                // Load the defined local system historians
                IEnumerable<DataRow> historians = connection.RetrieveData($"SELECT AdapterName FROM RuntimeHistorian WHERE NodeID = {nodeIDQueryString} AND TypeName = 'openHistorian.Adapters.LocalOutputAdapter'").AsEnumerable();

                List<string> validHistorians = new List<string>();
                string name, acronym;

                // Apply settings optimizations to local historians
                foreach (DataRow row in historians)
                {
                    acronym = row.Field<string>("AdapterName").ToLower();
                    validHistorians.Add(acronym);
                }

                // Local statics historian is valid regardless of historian type
                if (!validHistorians.Contains("stat"))
                    validHistorians.Add("stat");

                // Sort valid historians for binary search
                validHistorians.Sort();

                // Create a list to track categories to remove
                HashSet<string> categoriesToRemove = new HashSet<string>();

                // Search for unused settings categories
                foreach (PropertyInformation info in configFile.Settings.ElementInformation.Properties)
                {
                    name = info.Name;

                    if (name.EndsWith("AdoMetadataProvider") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("AdoMetadataProvider", StringComparison.Ordinal))) < 0)
                        categoriesToRemove.Add(name);

                    if (name.EndsWith("OleDbMetadataProvider") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("OleDbMetadataProvider", StringComparison.Ordinal))) < 0)
                        categoriesToRemove.Add(name);

                    if (name.EndsWith("RestWebServiceMetadataProvider") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("RestWebServiceMetadataProvider", StringComparison.Ordinal))) < 0)
                        categoriesToRemove.Add(name);

                    if (name.EndsWith("MetadataService") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("MetadataService", StringComparison.Ordinal))) < 0)
                        categoriesToRemove.Add(name);

                    if (name.EndsWith("TimeSeriesDataService") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("TimeSeriesDataService", StringComparison.Ordinal))) < 0)
                        categoriesToRemove.Add(name);

                    if (name.EndsWith("HadoopReplicationProvider") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("HadoopReplicationProvider", StringComparison.Ordinal))) < 0)
                        categoriesToRemove.Add(name);
                }

                if (categoriesToRemove.Count > 0)
                {
                    statusMessage("Removing unused local historian configuration settings...");

                    // Remove any unused settings categories
                    foreach (string category in categoriesToRemove)
                    {
                        configFile.Settings.Remove(category);
                    }
                }

                // Save any applied changes
                configFile.Save();
            }
        }

        #endregion

        #region [ Future Code ]

        // This code can be used to refresh the local metadata repository when/if this gets implemented

        // >> members:

        //private MetadataProviders m_metadataProviders;
        //private readonly object m_queuedMetadataRefreshPending;
        //private AutoResetEvent m_metadataRefreshComplete;
        //private bool m_autoRefreshMetadata;

        // >> ctor:

        //m_autoRefreshMetadata = true;
        //m_queuedMetadataRefreshPending = new object();
        //m_metadataRefreshComplete = new AutoResetEvent(true);

        // >> Status:

        //status.Append(m_metadataProviders.Status);
        //status.AppendLine();

        // >> Initialize():

        //// Provide metadata sync support.
        //m_metadataProviders = new MetadataProviders();
        //m_metadataProviders.AdapterCreated += MetadataProviders_AdapterCreated;
        //m_metadataProviders.AdapterLoaded += MetadataProviders_AdapterLoaded;
        //m_metadataProviders.AdapterUnloaded += MetadataProviders_AdapterUnloaded;
        //m_metadataProviders.AdapterLoadException += AdapterLoader_AdapterLoadException;

        // >> Dispose():

        //if (m_metadataProviders != null)
        //{
        //    m_metadataProviders.AdapterCreated -= MetadataProviders_AdapterCreated;
        //    m_metadataProviders.AdapterLoaded -= MetadataProviders_AdapterLoaded;
        //    m_metadataProviders.AdapterUnloaded -= MetadataProviders_AdapterUnloaded;
        //    m_metadataProviders.AdapterLoadException -= AdapterLoader_AdapterLoadException;
        //    m_metadataProviders.Dispose();
        //}

        // >> AttemptConnection():

        //m_metadataProviders.Initialize();

        //int waitCount = 0;

        //// Wait for adapter initialization to complete, up to 2 seconds
        //while (waitCount < 20 && m_adapterLoadedCount != m_dataServices.Adapters.Count + m_metadataProviders.Adapters.Count + m_replicationProviders.Adapters.Count)
        //{
        //    Thread.Sleep(100);
        //    waitCount++;
        //}

        //// Kick off a meta-data refresh...
        //if (m_autoRefreshMetadata)
        //{
        //    RefreshMetadata();
        //    m_autoRefreshMetadata = false;
        //}

        ///// <summary>
        ///// Gets or sets a boolean indicating whether or not metadata is
        ///// refreshed when the adapter attempts to connect to the archive.
        ///// </summary>
        //[ConnectionStringParameter,
        //Description("Define a boolean indicating whether to refresh metadata on connect."),
        //DefaultValue(true)]
        //public bool AutoRefreshMetadata
        //{
        //    get
        //    {
        //        return m_autoRefreshMetadata;
        //    }
        //    set
        //    {
        //        m_autoRefreshMetadata = value;
        //    }
        //}

        //private void MetadataProviders_AdapterCreated(object sender, EventArgs<IMetadataProvider> e)
        //{
        //    e.Argument.SettingsCategory = InstanceName + e.Argument.SettingsCategory;

        //    if (e.Argument.GetType() == typeof(AdoMetadataProvider))
        //    {
        //        // Populate the default configuration for AdoMetadataProvider.
        //        AdoMetadataProvider provider = e.Argument as AdoMetadataProvider;

        //        provider.Enabled = true;
        //        provider.SelectString = string.Format("SELECT * FROM HistorianMetadata WHERE PlantCode='{0}'", Name);

        //        // The following connection information is now provided via configuration Eval mappings
        //        //    provider.DataProviderString = config.Settings["SystemSettings"]["DataProviderString"].Value;

        //        //    ConfigurationFile config = ConfigurationFile.Current;
        //        //    string connectionString = config.Settings["SystemSettings"]["ConnectionString"].Value;
        //        //    Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
        //        //    string setting;

        //        //    if (settings.TryGetValue("Provider", out setting))
        //        //    {
        //        //        // Check if provider is for Access
        //        //        if (setting.StartsWith("Microsoft.Jet.OLEDB", StringComparison.OrdinalIgnoreCase))
        //        //        {
        //        //            // Make sure path to Access database is fully qualified
        //        //            if (settings.TryGetValue("Data Source", out setting))
        //        //            {
        //        //                settings["Data Source"] = FilePath.GetAbsolutePath(setting);
        //        //                connectionString = settings.JoinKeyValuePairs();
        //        //            }
        //        //        }
        //        //    }

        //        //    provider.ConnectionString = connectionString;
        //    }
        //}

        //private void MetadataProviders_AdapterLoaded(object sender, EventArgs<IMetadataProvider> e)
        //{
        //    e.Argument.Metadata = m_archive.MetadataFile;

        //    e.Argument.MetadataRefreshStart += MetadataProviders_MetadataRefreshStart;
        //    e.Argument.MetadataRefreshComplete += MetadataProviders_MetadataRefreshComplete;
        //    e.Argument.MetadataRefreshTimeout += MetadataProviders_MetadataRefreshTimeout;
        //    e.Argument.MetadataRefreshException += MetadataProviders_MetadataRefreshException;
        //    OnStatusMessage("{0} has been loaded.", e.Argument.GetType().Name);

        //    m_adapterLoadedCount++;
        //}

        //private void MetadataProviders_AdapterUnloaded(object sender, EventArgs<IMetadataProvider> e)
        //{
        //    e.Argument.Metadata = null;
        //    e.Argument.MetadataRefreshStart -= MetadataProviders_MetadataRefreshStart;
        //    e.Argument.MetadataRefreshComplete -= MetadataProviders_MetadataRefreshComplete;
        //    e.Argument.MetadataRefreshTimeout -= MetadataProviders_MetadataRefreshTimeout;
        //    e.Argument.MetadataRefreshException -= MetadataProviders_MetadataRefreshException;
        //    OnStatusMessage("{0} has been unloaded.", e.Argument.GetType().Name);
        //}

        //private void MetadataProviders_MetadataRefreshStart(object sender, EventArgs e)
        //{
        //    OnStatusMessage("{0} has started metadata refresh...", sender.GetType().Name);
        //}

        //private void MetadataProviders_MetadataRefreshComplete(object sender, EventArgs e)
        //{
        //    OnStatusMessage("{0} has finished metadata refresh.", sender.GetType().Name);
        //}

        //private void MetadataProviders_MetadataRefreshTimeout(object sender, EventArgs e)
        //{
        //    OnStatusMessage("{0} has timed-out on metadata refresh.", sender.GetType().Name);
        //}

        //private void MetadataProviders_MetadataRefreshException(object sender, EventArgs<Exception> e)
        //{
        //    OnProcessException(e.Argument);
        //}
        #endregion
    }
}
