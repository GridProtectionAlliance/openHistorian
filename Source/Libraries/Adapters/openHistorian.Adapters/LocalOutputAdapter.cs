//******************************************************************************************************
//  LocalOutputAdapter.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  07/25/2013 - J. Ritchie Carroll
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
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.Historian.DataServices;
using GSF.Historian.Replication;
using GSF.IO;
using GSF.IO.Unmanaged;
using GSF.Snap.Services;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.Units;
using openHistorian.Model;
using openHistorian.Net;
using openHistorian.Snap;
using DeviceGroup = openHistorian.Model.DeviceGroup;
using Measurement = openHistorian.Model.Measurement;
using Timer = System.Timers.Timer;
using GSFDataPublisher = GSF.TimeSeries.Transport.DataPublisher;
using STTPDataPublisher = sttp.DataPublisher;

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
        /// Defines default value for <see cref="WatchAttachedPaths"/>.
        /// </summary>
        public const bool DefaultWatchAttachedPaths = false;

        /// <summary>
        /// Defines default value for <see cref="DataChannel"/>.
        /// </summary>
        public const string DefaultDataChannel = "port=38402; interface=::0";

        /// <summary>
        /// Defines the default value for <see cref="TargetFileSize"/>.
        /// </summary>
        public const double DefaultTargetFileSize = 2.0D;

        /// <summary>
        /// Defines the default value for <see cref="DesiredRemainingSpace"/>.
        /// </summary>
        public const double DefaultDesiredRemainingSpace = 100.0D;

        /// <summary>
        /// Defines the default value for <see cref="MaximumArchiveDays"/>.
        /// </summary>
        public const int DefaultMaximumArchiveDays = 0;

        /// <summary>
        /// Defines the default value for <see cref="AutoRemoveOldestFilesBeforeFull"/>.
        /// </summary>
        public const bool DefaultAutoRemoveOldestFilesBeforeFull = true;

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
        private double m_targetFileSize;
        private double m_desiredRemainingSpace;
        private long m_pastTimeReasonabilityLimit;
        private long m_futureTimeReasonabilityLimit;
        private DataServices m_dataServices;
        private ReplicationProviders m_replicationProviders;
        private long m_archivedMeasurements;
        private readonly HistorianKey m_key;
        private readonly HistorianValue m_value;
        private Dictionary<ulong, DataRow> m_measurements;
        private Dictionary<ulong, Tuple<int, int, double>> m_compressionSettings;
        private readonly Dictionary<ulong, Tuple<IMeasurement, IMeasurement, double, double>> m_swingingDoorStates;
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
        [ConnectionStringParameter]
        [Description("Define the instance name for the historian. Leave this value blank to default to the adapter name.")]
        [DefaultValue("")]
        public string InstanceName
        {
            get => string.IsNullOrEmpty(m_instanceName) ? Name.ToLower() : m_instanceName;
            set => m_instanceName = value;
        }

        /// <summary>
        /// Gets or sets TCP server based connection string to use for the historian data channel.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines TCP server based connection string to use for the historian data channel.")]
        [DefaultValue(DefaultDataChannel)]
        public string DataChannel { get; set; }

        /// <summary>
        /// Gets or sets the working directory which is used to write working data before it is moved into its permanent file.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define the working directory used for working data and intermediate files and before moving data to its permanent location (see ArchiveDirectories). Leave blank to default to \".\\Archive\\\".")]
        [DefaultValue("")]
        public string WorkingDirectory
        {
            get => m_workingDirectory;
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
        [ConnectionStringParameter]
        [Description("Define the write directories for this historian instance. Leave empty to default to WorkingDirectory. Separate multiple directories with a semi-colon.")]
        [DefaultValue("")]
        public string ArchiveDirectories
        {
            get => m_archiveDirectories is null ? "" : string.Join(";", m_archiveDirectories);
            set
            {
                if (value is null)
                {
                    m_archiveDirectories = null;
                }
                else
                {
                    List<string> archivePaths = new();

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
            }
        }

        /// <summary>
        /// Gets or sets directory naming mode for archive directory files.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define the directory naming mode for archive directory files.")]
        [DefaultValue(DefaultDirectoryNamingMode)]
        public ArchiveDirectoryMethod DirectoryNamingMode { get; set; }

        /// <summary>
        /// Gets or sets the default interval, in seconds, over which the archive curtailment will operate. Set to zero to disable.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define the default interval, in seconds, over which the archive curtailment will operate. Set to zero to disable.")]
        [DefaultValue(DefaultArchiveCurtailmentInterval)]
        public int ArchiveCurtailmentInterval { get; set; }

        /// <summary>
        /// Gets or sets the directories and/or individual files to attach to the historian.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define the directories and/or individual files to attach to this historian instance. Separate multiple paths with a semi-colon.")]
        [DefaultValue("")]
        public string AttachedPaths
        {
            get => m_attachedPaths is null ? "" : string.Join(";", m_attachedPaths);
            set
            {
                if (value is null)
                {
                    m_attachedPaths = null;
                }
                else
                {
                    List<string> attachedPaths = new();

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
            }
        }

        /// <summary>
        /// Gets or sets the flag which determines whether to set up file watchers to monitor the attached paths.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Determines whether to set up file watchers to monitor the attached paths.")]
        [DefaultValue(DefaultWatchAttachedPaths)]
        public bool WatchAttachedPaths { get; set; }

        /// <summary>
        /// Gets or sets target file size, in GigaBytes.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define desired target file size in GigaBytes.")]
        [DefaultValue(DefaultTargetFileSize)]
        public double TargetFileSize
        {
            get => m_targetFileSize;
            set
            {
                if (value < 0.1D || value > SI2.Tera)
                    throw new ArgumentOutOfRangeException(nameof(value));

                m_targetFileSize = value;
            }
        }

        /// <summary>
        /// Gets or sets desired remaining space, in GigaBytes.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define desired remaining disk space in GigaBytes.")]
        [DefaultValue(DefaultDesiredRemainingSpace)]
        public double DesiredRemainingSpace
        {
            get => m_desiredRemainingSpace;
            set
            {
                if (value < 0.1D || value > SI2.Tera)
                    throw new ArgumentOutOfRangeException(nameof(value));

                m_desiredRemainingSpace = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of days of data to maintain in the archive.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define the maximum number of days of data to maintain, i.e., any archives files with data older than current date minus value will be deleted daily. Defaults to zero meaning no maximum.")]
        [DefaultValue(DefaultMaximumArchiveDays)]
        public int MaximumArchiveDays { get; set; } = DefaultMaximumArchiveDays;

        /// <summary>
        /// Gets or sets the flag that determines if oldest archive files should be removed before running out of archive space.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define the flag that determines if oldest archive files should be removed before running out of archive space.")]
        [DefaultValue(DefaultAutoRemoveOldestFilesBeforeFull)]
        public bool AutoRemoveOldestFilesBeforeFull { get; set; } = DefaultAutoRemoveOldestFilesBeforeFull;

        /// <summary>
        /// Gets or sets flag that indicates if incoming timestamps to the historian should be validated for reasonability.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define the flag that indicates if incoming timestamps to the historian should be validated for reasonability.")]
        [DefaultValue(DefaultEnableTimeReasonabilityCheck)]
        public bool EnableTimeReasonabilityCheck { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of seconds that a past timestamp, as compared to local clock, will be considered valid.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define the maximum number of seconds that a past timestamp, as compared to local clock, will be considered valid.")]
        [DefaultValue(DefaultPastTimeReasonabilityLimit)]
        public double PastTimeReasonabilityLimit
        {
            get => new Ticks(m_pastTimeReasonabilityLimit).ToSeconds();
            set => m_pastTimeReasonabilityLimit = Ticks.FromSeconds(Math.Abs(value));
        }

        /// <summary>
        /// Gets or sets the maximum number of seconds that a future timestamp, as compared to local clock, will be considered valid.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define the maximum number of seconds that a future timestamp, as compared to local clock, will be considered valid.")]
        [DefaultValue(DefaultFutureTimeReasonabilityLimit)]
        public double FutureTimeReasonabilityLimit
        {
            get => new Ticks(m_futureTimeReasonabilityLimit).ToSeconds();
            set => m_futureTimeReasonabilityLimit = Ticks.FromSeconds(Math.Abs(value));
        }

        /// <summary>
        /// Gets or sets the flag that determines if swinging door compression is enabled for this historian instance.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Define the flag that determines if swinging door compression is enabled for this historian instance.")]
        [DefaultValue(DefaultSwingingDoorCompressionEnabled)]
        public bool SwingingDoorCompressionEnabled { get; set; }

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
            get => base.DataSource;
            set
            {
                base.DataSource = value;

                if (value is null)
                    return;

                Dictionary<ulong, DataRow> measurements = new();
                string instanceName = InstanceName;

                // Create dictionary of metadata for this server instance
                foreach (DataRow row in value.Tables["ActiveMeasurements"].Rows)
                {
                    if (MeasurementKey.TryParse(row["ID"].ToString(), out MeasurementKey key) && (key.Source?.Equals(instanceName, StringComparison.OrdinalIgnoreCase) ?? false))
                        measurements[key.ID] = row;
                }

                Dictionary<ulong, Tuple<int, int, double>> compressionSettings = new();

                if (value.Tables.Contains("CompressionSettings"))
                {
                    // Extract compression settings for defined measurements
                    foreach (DataRow row in value.Tables["CompressionSettings"].Rows)
                    {
                        uint pointID = row.ConvertField<uint>("PointID");

                        if (InputMeasurementKeys.All(key => key.ID != pointID))
                            continue;

                        // Get compression settings
                        int compressionMinTime = row.ConvertField<int>("CompressionMinTime");
                        int compressionMaxTime = row.ConvertField<int>("CompressionMaxTime");
                        double compressionLimit = row.ConvertField<double>("CompressionLimit");

                        compressionSettings[pointID] = new Tuple<int, int, double>(compressionMinTime, compressionMaxTime, compressionLimit);
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
                StringBuilder status = new();

                status.Append(base.Status);
                status.AppendLine();

                status.AppendLine($"   Historian instance name: {InstanceName}");
                status.AppendLine($"         Working directory: {FilePath.TrimFileName(WorkingDirectory, 51)}");
                status.AppendLine($"      Network data channel: {DataChannel.ToNonNullString(DefaultDataChannel)}");
                status.AppendLine($"          Target file size: {TargetFileSize:N4}GB");
                status.AppendLine($"   Desired remaining space: {DesiredRemainingSpace:N4}GB");
                status.AppendLine($"     Directory naming mode: {DirectoryNamingMode}");
                status.AppendLine($"       Disk flush interval: {m_archiveInfo.DiskFlushInterval:N0}ms");
                status.AppendLine($"      Cache flush interval: {m_archiveInfo.CacheFlushInterval:N0}ms");
                status.AppendLine($"             Staging count: {m_archiveInfo.StagingCount:N0}");
                status.AppendLine($"          Memory pool size: {Globals.MemoryPool.MaximumPoolSize / SI2.Giga:N4}GB");
                status.AppendLine($"      Maximum archive days: {(MaximumArchiveDays < 1 ? "No limit" : MaximumArchiveDays.ToString("N0"))}");
                status.AppendLine($"  Auto-remove old archives: {AutoRemoveOldestFilesBeforeFull}");
                status.AppendLine($"  Time reasonability check: {(EnableTimeReasonabilityCheck ? "Enabled" : "Not Enabled")}");
                status.AppendLine($" Archive curtailment timer: {Time.ToElapsedTimeString(ArchiveCurtailmentInterval, 0)}");

                if (EnableTimeReasonabilityCheck)
                {
                    status.AppendLine($"   Maximum past time limit: {PastTimeReasonabilityLimit:N4}s, i.e., {new Ticks(m_pastTimeReasonabilityLimit).ToElapsedTimeString(4)}");
                    status.AppendLine($" Maximum future time limit: {FutureTimeReasonabilityLimit:N4}s, i.e., {new Ticks(m_futureTimeReasonabilityLimit).ToElapsedTimeString(4)}");
                }

                if (m_dataServices is not null)
                    status.Append(m_dataServices.Status);

                if (m_replicationProviders is not null)
                    status.Append(m_replicationProviders.Status);

                Server?.Host?.GetFullStatus(status, 25);

                return status.ToString();
            }
        }

        /// <summary>
        /// Historian server instance.
        /// </summary>
        public HistorianServer Server { get; private set; }

        /// <summary>
        /// Active measurement metadata dictionary.
        /// </summary>
        public Dictionary<ulong, DataRow> Measurements => m_measurements;

        private SafeFileWatcher[] AttachedPathWatchers
        {
            set
            {
                if (m_attachedPathWatchers is not null)
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
            if (m_disposed)
                return;

            try
            {
                if (!disposing)
                    return;

                AttachedPathWatchers = null;

                if (m_dataServices is not null)
                {
                    m_dataServices.AdapterCreated -= DataServices_AdapterCreated;
                    m_dataServices.AdapterLoaded -= DataServices_AdapterLoaded;
                    m_dataServices.AdapterUnloaded -= DataServices_AdapterUnloaded;
                    m_dataServices.AdapterLoadException -= AdapterLoader_AdapterLoadException;
                    m_dataServices.Dispose();
                }

                if (m_replicationProviders is not null)
                {
                    m_replicationProviders.AdapterCreated -= ReplicationProviders_AdapterCreated;
                    m_replicationProviders.AdapterLoaded -= ReplicationProviders_AdapterLoaded;
                    m_replicationProviders.AdapterUnloaded -= ReplicationProviders_AdapterUnloaded;
                    m_replicationProviders.AdapterLoadException -= AdapterLoader_AdapterLoadException;
                    m_replicationProviders.Dispose();
                }

                if (m_archiveCurtailmentTimer is not null)
                {
                    m_archiveCurtailmentTimer.Stop();
                    m_archiveCurtailmentTimer.Elapsed -= ArchiveCurtailmentTimerElapsed;
                    m_archiveCurtailmentTimer.Dispose();
                }
            }
            finally
            {
                m_disposed = true;          // Prevent duplicate dispose.
                base.Dispose(disposing);    // Call base class Dispose().
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

            // Validate settings.
            if (!settings.TryGetValue(nameof(InstanceName), out m_instanceName) || string.IsNullOrWhiteSpace(m_instanceName))
                m_instanceName = Name;

            // Track instance in static dictionary
            Instances[InstanceName] = this;

            if (!settings.TryGetValue(nameof(WorkingDirectory), out string setting) || string.IsNullOrEmpty(setting))
                setting = "Archive";

            WorkingDirectory = setting;

            if (settings.TryGetValue(nameof(ArchiveDirectories), out setting))
                ArchiveDirectories = setting;

            ArchiveCurtailmentInterval = settings.TryGetValue(nameof(ArchiveCurtailmentInterval), out setting) ? int.Parse(setting) : DefaultArchiveCurtailmentInterval;

            if (settings.TryGetValue(nameof(AttachedPaths), out setting))
                AttachedPaths = setting;

            WatchAttachedPaths = settings.TryGetValue(nameof(WatchAttachedPaths), out setting) && setting.ParseBoolean();

            DataChannel = settings.TryGetValue(nameof(DataChannel), out setting) && !string.IsNullOrWhiteSpace(setting) ? setting : DefaultDataChannel;

            if (!settings.TryGetValue(nameof(TargetFileSize), out setting) || !double.TryParse(setting, out double targetFileSize))
                targetFileSize = DefaultTargetFileSize;

            if (targetFileSize < 0.1D || targetFileSize > SI2.Tera)
                targetFileSize = DefaultTargetFileSize;

            if (!settings.TryGetValue(nameof(DesiredRemainingSpace), out setting) || !double.TryParse(setting, out double desiredRemainingSpace))
                desiredRemainingSpace = DefaultDesiredRemainingSpace;

            if (desiredRemainingSpace < 0.1D || desiredRemainingSpace > SI2.Tera)
                desiredRemainingSpace = DefaultDesiredRemainingSpace;

            if (settings.TryGetValue(nameof(MaximumArchiveDays), out setting) && int.TryParse(setting, out int maximumArchiveDays))
                MaximumArchiveDays = maximumArchiveDays;

            if (settings.TryGetValue(nameof(AutoRemoveOldestFilesBeforeFull), out setting))
                AutoRemoveOldestFilesBeforeFull = setting.ParseBoolean();

            EnableTimeReasonabilityCheck = settings.TryGetValue(nameof(EnableTimeReasonabilityCheck), out setting) && setting.ParseBoolean();

            if (settings.TryGetValue(nameof(PastTimeReasonabilityLimit), out setting) && double.TryParse(setting, out double value))
                PastTimeReasonabilityLimit = value;
            else
                PastTimeReasonabilityLimit = DefaultPastTimeReasonabilityLimit;

            if (settings.TryGetValue(nameof(FutureTimeReasonabilityLimit), out setting) && double.TryParse(setting, out value))
                FutureTimeReasonabilityLimit = value;
            else
                FutureTimeReasonabilityLimit = DefaultFutureTimeReasonabilityLimit;

            SwingingDoorCompressionEnabled = settings.TryGetValue(nameof(SwingingDoorCompressionEnabled), out setting) && setting.ParseBoolean();

            if (settings.TryGetValue(nameof(DirectoryNamingMode), out setting) && Enum.TryParse(setting, true, out ArchiveDirectoryMethod directoryNamingMode))
                DirectoryNamingMode = directoryNamingMode;
            else
                DirectoryNamingMode = DefaultDirectoryNamingMode;

            // Handle advanced settings - there are hidden but available from manual entry into connection string
            if (!settings.TryGetValue("StagingCount", out setting) || !int.TryParse(setting, out int stagingCount))
                stagingCount = 3;

            if (!settings.TryGetValue("DiskFlushInterval", out setting) || !int.TryParse(setting, out int diskFlushInterval))
                diskFlushInterval = 10000;

            if (!settings.TryGetValue("CacheFlushInterval", out setting) || !int.TryParse(setting, out int cacheFlushInterval))
                cacheFlushInterval = 100;

            // Establish archive information for this historian instance
            m_archiveInfo = new HistorianServerDatabaseConfig(InstanceName, WorkingDirectory, true);

            if (m_archiveDirectories is not null)
                m_archiveInfo.FinalWritePaths.AddRange(m_archiveDirectories);

            if (m_attachedPaths is not null)
                m_archiveInfo.ImportPaths.AddRange(m_attachedPaths);

            m_archiveInfo.ImportAttachedPathsAtStartup = false;
            m_archiveInfo.TargetFileSize = (long)(targetFileSize * SI.Giga);
            m_archiveInfo.DesiredRemainingSpace = (long)(desiredRemainingSpace * SI.Giga);
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
                m_archiveCurtailmentTimer.Elapsed += ArchiveCurtailmentTimerElapsed;
                m_archiveCurtailmentTimer.Enabled = true;
            }

            // Initialize the file watchers for attached paths
            AttachedPathWatchers = WatchAttachedPaths ? m_attachedPaths?.Select(WatchPath).ToArray() : null;
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        /// <param name="maxLength">Maximum length of the status message.</param>
        /// <returns>Text of the status message.</returns>
        public override string GetShortStatus(int maxLength) =>
            $"Archived {m_archivedMeasurements} measurements.".CenterText(maxLength);

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
            HashSet<string> sourceFiles = new(FilePath.GetFileList(fileName).Select(Path.GetFullPath), StringComparer.OrdinalIgnoreCase);
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
            if (m_archive?.ClientDatabase is not null)
                return m_archive.ClientDatabase;

            throw new InvalidOperationException("Cannot execute historian operation, archive database is not open.");
        }

        private void ArchiveCurtailmentTimerElapsed(object sender, ElapsedEventArgs e) => CurtailArchiveFiles();

        /// <summary>
        /// Attempts to connect to this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        protected override void AttemptConnection()
        {
            // Open archive files
            Dictionary<string, string> settings = (DataChannel ?? DefaultDataChannel).ParseKeyValuePairs();

            if (!settings.TryGetValue("port", out string setting) || !int.TryParse(setting, out int port))
                port = DefaultPort;

            if (!settings.TryGetValue("interface", out string networkInterfaceIP))
                networkInterfaceIP = "::0";

            Server = new HistorianServer(m_archiveInfo, port, networkInterfaceIP);
            m_archive = Server[InstanceName];

            // Initialization of services needs to occur after files are open
            m_dataServices.Initialize();
            m_replicationProviders.Initialize();

            OnConnected();

            if (m_attachedPathWatchers is not null)
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
            if (m_attachedPathWatchers is not null)
            {
                foreach (SafeFileWatcher fileWatcher in m_attachedPathWatchers)
                    fileWatcher.EnableRaisingEvents = false;
            }

            m_archive = null;
            Server?.Dispose();
            Server = null;

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
                if (EnableTimeReasonabilityCheck)
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
                if (SwingingDoorCompressionEnabled)
                {
                    // Attempt to lookup compression settings for this measurement
                    if ((m_compressionSettings?.TryGetValue(m_key.PointID, out Tuple<int, int, double> settings) ?? false) && settings is not null)
                    {
                        // Get compression settings
                        int compressionMinTime = settings.Item1;
                        int compressionMaxTime = settings.Item2;
                        double compressionLimit = settings.Item3;

                        // Get current swinging door compression state, creating state if needed
                        Tuple<IMeasurement, IMeasurement, double, double> state = m_swingingDoorStates.GetOrAdd(m_key.PointID, _ => new Tuple<IMeasurement, IMeasurement, double, double>(measurement, measurement, double.MinValue, double.MaxValue));
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
                        else if (currentData.StateFlags != archivedData.StateFlags || currentData.StateFlags != previousData.StateFlags || compressionMaxTime > 0 && previousData.Value - archivedData.Timestamp > compressionMaxTime)
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
            try
            {
                ClientDatabaseBase<HistorianKey, HistorianValue> clientDatabase = GetClientDatabase();

                string[] archivedirectories = m_archiveDirectories ?? Array.Empty<string>();
                string[] attachedPaths = m_attachedPaths ?? Array.Empty<string>();

                List<string> files = archivedirectories
                    .Concat(attachedPaths)
                    .SelectMany(dir => FilePath.EnumerateFiles(dir, exceptionHandler: ex => 
                        OnProcessException(MessageLevel.Error, ex, nameof(AttachArchiveDirectoriesAndAttachedPaths))))
                    .ToList();

                string[] filePtr = new string[1];

                foreach (string file in files)
                {
                    if (!Enabled)
                        return;

                    filePtr[0] = file;

                    try
                    {
                        clientDatabase.AttachFilesOrPaths(filePtr);
                    }
                    catch (Exception ex)
                    {
                        OnProcessException(MessageLevel.Error, new InvalidOperationException(
                            $"{Name} failed while attempting to attach to file \"{file}\": {ex.Message}", ex),
                            nameof(AttachArchiveDirectoriesAndAttachedPaths));
                    }
                }
            }
            catch (Exception ex)
            {
                OnProcessException(MessageLevel.Critical, new InvalidOperationException($"{Name} failed while attempting to attach to existing archives: {ex.Message}", ex));
            }
        }

        private SafeFileWatcher WatchPath(string path)
        {
            SafeFileWatcher fileWatcher = new(path, "*.d2*");

            fileWatcher.Created += (_, args) => ThreadPool.QueueUserWorkItem(state =>
            {
                ClientDatabaseBase<HistorianKey, HistorianValue> clientDatabase = GetClientDatabase();
                string[] filePtr = { (string)state };
                clientDatabase.AttachFilesOrPaths(filePtr);
            },
            args.FullPath);

            fileWatcher.InternalBufferSize = 65536;

            return fileWatcher;
        }

        private void DataServices_AdapterCreated(object sender, EventArgs<IDataService> e) =>
            e.Argument.SettingsCategory = $"{InstanceName.ToLowerInvariant()}{e.Argument.SettingsCategory}";

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

        private void ReplicationProviders_AdapterCreated(object sender, EventArgs<IReplicationProvider> e) =>
            e.Argument.SettingsCategory = InstanceName.ToLowerInvariant() + e.Argument.SettingsCategory;

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

        private void AdapterLoader_AdapterLoadException(object sender, EventArgs<Exception> e) => 
            OnProcessException(MessageLevel.Warning, e.Argument);

        private void DataServices_ServiceProcessException(object sender, EventArgs<Exception> e) => 
            OnProcessException(MessageLevel.Warning, e.Argument);

        private void ReplicationProvider_ReplicationStart(object sender, EventArgs e) => 
            OnStatusMessage(MessageLevel.Info, $"{sender.GetType().Name} has started archive replication...");

        private void ReplicationProvider_ReplicationComplete(object sender, EventArgs e) => 
            OnStatusMessage(MessageLevel.Info, $"{sender.GetType().Name} has finished archive replication.");

        private void ReplicationProvider_ReplicationProgress(object sender, EventArgs<ProcessProgress<int>> e) => 
            OnStatusMessage(MessageLevel.Info, $"{sender.GetType().Name} has replicated archive file {e.Argument.ProgressMessage}.");

        private void ReplicationProvider_ReplicationException(object sender, EventArgs<Exception> e) => 
            OnProcessException(MessageLevel.Warning, e.Argument);

        #endregion

        #region [ Static ]

        // Static Fields

        /// <summary>
        /// Accesses local output adapter instances (normally only one).
        /// </summary>
        public static readonly ConcurrentDictionary<string, LocalOutputAdapter> Instances = new(StringComparer.OrdinalIgnoreCase);

        private static int s_virtualProtocolID;

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

            if (!Enum.TryParse(systemSettings["MemoryPoolTargetUtilization"].Value, false, out TargetUtilizationLevels targetLevel))
                targetLevel = TargetUtilizationLevels.High;

            Globals.MemoryPool.SetTargetUtilizationLevel(targetLevel);
        }

        // Static Methods

        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local
        private static void OptimizeLocalHistorianSettings(AdoDataConnection connection, string nodeIDQueryString, ulong trackingVersion, string arguments, Action<string> statusMessage, Action<Exception> processException)
        {
            TableOperations<CustomInputAdapter> inputAdapterTable = new(connection);
            TableOperations<CustomActionAdapter> actionAdapterTable = new(connection);
            TableOperations<CustomOutputAdapter> outputAdapterTable = new(connection);
            TableOperations<Historian> historianTable = new(connection);
            TableOperations<Measurement> measurementTable = new(connection);
            TableOperations<DeviceGroupClass> deviceGroupClassTable = new(connection);

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

                List<string> validHistorians = new();

                // Apply settings optimizations to local historians
                foreach (DataRow row in historians)
                {
                    string acronym = row.Field<string>("AdapterName").ToLower();
                    validHistorians.Add(acronym);
                }

                // Local statics historian is valid regardless of historian type
                if (!validHistorians.Contains("stat"))
                    validHistorians.Add("stat");

                // Sort valid historians for binary search
                validHistorians.Sort();

                // Create a list to track categories to remove
                HashSet<string> categoriesToRemove = new();

                // Search for unused settings categories
                foreach (PropertyInformation info in configFile.Settings.ElementInformation.Properties)
                {
                    string name = info.Name;

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
                        configFile.Settings.Remove(category);
                }

                // Save any applied changes
                configFile.Save();

                // Convert valid historian list to proper acronym casing (and remove stat historian)
                IEnumerable<string> historianAcronyms = validHistorians.Where(value => !value.Equals("stat")).Select(value => value.ToUpperInvariant());

                foreach (string historianAcronym in historianAcronyms)
                {
                    Historian historianAdapter = historianTable.QueryRecordWhere("Acronym = {0}", historianAcronym);
                    CustomOutputAdapter outputAdapter = null;

                    if (historianAdapter is null)
                    {
                        outputAdapter = outputAdapterTable.QueryRecordWhere("AdapterName = {0}", historianAcronym);

                        if (outputAdapter is null)
                        {
                            statusMessage($"WARNING: Could not check for associated historian input reader after failing to find historian adapter \"{historianAcronym}\" in either \"Historian\" or \"CustomOutputAdapter\" table. Record was just found in the \"RuntimeHistorian\" view, verify schema integrity.");
                            continue;
                        }
                    }

                    // Parse connection string to get adapter settings for historian
                    Dictionary<string, string> adapterSettings = (historianAdapter?.ConnectionString ?? outputAdapter?.ConnectionString ?? "").ParseKeyValuePairs();

                    // Get instance name for historian adapter
                    if (!adapterSettings.TryGetValue("instanceName", out string instanceName) || string.IsNullOrWhiteSpace(instanceName))
                        instanceName = historianAcronym;

                    // Reader name will always be associated with instance name of historian (this often matches historian adapter name)
                    string readerName = $"{instanceName}READER";
                    CustomInputAdapter inputAdapter = inputAdapterTable.QueryRecordWhere("AdapterName = {0}", readerName);

                    if (inputAdapter is not null)
                        continue;

                    ushort? port = null;

                    // Check if historian has defined a custom data channel, if so attempt to parse port number
                    if (adapterSettings.ContainsKey(nameof(DataChannel)))
                    {
                        Dictionary<string, string> configSettings = adapterSettings[nameof(DataChannel)].ParseKeyValuePairs();

                        if (configSettings.ContainsKey("port") && ushort.TryParse(configSettings["port"], out ushort value))
                            port = value;
                    }

                    string serverConnection = port is null ? "127.0.0.1" : $"127.0.0.1:{port.Value}";

                    // Add new associated historian input adapter reader to handle temporal queries
                    inputAdapter = inputAdapterTable.NewRecord();
                    inputAdapter.NodeID = new Guid(nodeIDQueryString.RemoveCharacter('\''));
                    inputAdapter.AdapterName = $"{historianAcronym}READER";
                    inputAdapter.AssemblyName = "openHistorian.Adapters.dll";
                    inputAdapter.TypeName = "openHistorian.Adapters.LocalInputAdapter";
                    inputAdapter.ConnectionString = $"{nameof(LocalInputAdapter.InstanceName)}={instanceName}; {nameof(LocalInputAdapter.HistorianServer)}={serverConnection}; ConnectOnDemand=true";
                    inputAdapter.LoadOrder = 0;
                    inputAdapter.Enabled = true;
                    inputAdapterTable.AddNewRecord(inputAdapter);
                }
            }

            // Make sure gateway protocol data publishers filter out device groups from metadata since groups reference local device IDs
            const string DefaultDeviceFilter = "FROM DeviceDetail WHERE IsConcentrator = 0;";
            const string GSFDataPublisherTypeName = nameof(GSF) + "." + nameof(GSF.TimeSeries) + "." + nameof(GSF.TimeSeries.Transport) + "." + nameof(GSF.TimeSeries.Transport.DataPublisher);
            const string GSFMetadataTables = nameof(GSFDataPublisher.MetadataTables);
            const string STTPDataPublisherTypeName = nameof(sttp) + "." + nameof(sttp.DataPublisher);
            const string STTPMetadataTables = nameof(STTPDataPublisher.MetadataTables);
            int virtualProtocolID = s_virtualProtocolID != 0 ? s_virtualProtocolID : s_virtualProtocolID = connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='VirtualInput'");
            string deviceGroupFilter = $"FROM DeviceDetail WHERE IsConcentrator = 0 AND NOT (ProtocolID = {virtualProtocolID} AND AccessID = {DeviceGroup.DefaultAccessID});";

            IEnumerable<CustomActionAdapter> dataPublisherAdapters = actionAdapterTable.QueryRecordsWhere($"NodeID = {nodeIDQueryString} AND TypeName LIKE '%.DataPublisher'");

            foreach (CustomActionAdapter actionAdapter in dataPublisherAdapters)
            {
                if (actionAdapter is null)
                    continue;

                Dictionary<string, string> connectionString = actionAdapter.ConnectionString?.ParseKeyValuePairs() ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                switch (actionAdapter.TypeName)
                {
                    case GSFDataPublisherTypeName:
                        if (connectionString.ContainsKey(GSFMetadataTables))
                            continue;
                        
                        connectionString[GSFMetadataTables] = GSFDataPublisher.DefaultMetadataTables.Replace(DefaultDeviceFilter, deviceGroupFilter);
                        break;
                    case STTPDataPublisherTypeName:
                        // For down-stream identity matched metadata replication, check option to allow device groups replicate - this will OK when device IDs match, e.g.,
                        // when STTP data subscribers enable the "useIdentityInsertsForMetadata" option
                        if (connectionString.TryGetValue("allowDeviceGroupReplication", out string setting) && setting.ParseBoolean())
                        {
                            connectionString.Remove(STTPMetadataTables);
                        }
                        else
                        {
                            if (connectionString.ContainsKey(STTPMetadataTables))
                                continue;

                            connectionString[STTPMetadataTables] = STTPDataPublisher.DefaultMetadataTables.Replace(DefaultDeviceFilter, deviceGroupFilter);
                        }
                        break;
                    default:
                        continue;
                }

                actionAdapter.ConnectionString = connectionString.JoinKeyValuePairs();
                actionAdapterTable.UpdateRecord(actionAdapter);
            }

            // Make sure default device group classes are defined
            int count = deviceGroupClassTable.QueryRecordCountWhere("Longitude = {0} and Latitude = {0}", DeviceGroupClass.DefaultGeoID);
            
            if (count == 0)
            {
                // Add default device group classes
                List<string> classOptions = new()
                {
                    "Region",
                    "Substation",
                    "Generation",
                    "Other"
                };

                foreach (string classOption in classOptions)
                {
                    DeviceGroupClass deviceGroupClass = deviceGroupClassTable.NewRecord();
                    deviceGroupClass.Acronym = classOption.ToUpperInvariant();
                    deviceGroupClass.Name = classOption;
                    deviceGroupClassTable.AddNewRecord(deviceGroupClass);
                }
            }

            IEnumerable<CustomActionAdapter> dynamicCalculators = actionAdapterTable.QueryRecordsWhere("TypeName = 'DynamicCalculator.DynamicCalculator'");

            foreach (CustomActionAdapter dynamicCalculator in dynamicCalculators)
            {
                if (dynamicCalculator is null)
                    continue;

                Dictionary<string, string> connectionString = dynamicCalculator.ConnectionString?.ParseKeyValuePairs() ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                
                // variableList={FREQ=bef9f097-d6f8-4b4f-9ce4-97ffa30a08e9}; expressionText=FREQ-60; framesPerSecond=30; lagTime=6; leadTime=3; outputMeasurements=5966846f-156d-4a2d-a181-08c4875447a1; useLatestValues=false
                // variableList={VPOSA=62eb881c-4423-47bc-87bd-e3acfd5c7430; VAA=01564cc5-2451-4501-ba51-4fcea7173333}; expressionText=VPOSA-VAA; framesPerSecond=30; lagTime=6; leadTime=3; outputMeasurements=d59389a2-3816-4e84-a593-75f07369e582; useLatestValues=false

                if (!connectionString.ContainsKey("variableList"))
                    continue;

                string variableList = connectionString["variableList"];

                if (string.IsNullOrWhiteSpace(variableList))
                    continue;

                Dictionary<string, string> variables = variableList.ParseKeyValuePairs();
                HashSet<Guid> signalIDs = new(variables.Values.Select(id => Guid.TryParse(id, out Guid signalID) ? signalID : Guid.Empty).Where(signalID => signalID != Guid.Empty));

                if (signalIDs.Count == 0)
                    continue;

                int inputCount = connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM Measurement WHERE SignalID IN ({string.Join(",", signalIDs.Select(id => $"'{id}'"))})");
                
                if (inputCount == signalIDs.Count)
                    continue;

                // Remove any calculations that reference non-existent input measurements
                int affectedRows = actionAdapterTable.DeleteRecord(dynamicCalculator);

                if (affectedRows > 0)
                    statusMessage($"Removed dynamic calculation adapter \"{dynamicCalculator.AdapterName}\" that referenced non-existent input measurements.");

                // Remove result calculation output measurement
                if (!connectionString.ContainsKey("outputMeasurements"))
                    continue;

                string outputMeasurement = connectionString["outputMeasurements"];

                if (string.IsNullOrWhiteSpace(outputMeasurement))
                    continue;

                if (Guid.TryParse(outputMeasurement, out Guid outputMeasurementID))
                    measurementTable.DeleteRecordWhere($"SignalID = '{outputMeasurementID}'");
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
