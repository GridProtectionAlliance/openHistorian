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
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using GSF;
using GSF.Configuration;
using GSF.Data;
using GSF.Diagnostics;
using GSF.Historian;
using GSF.Historian.DataServices;
using GSF.Historian.Replication;
using GSF.IO;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Services.Configuration;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using openHistorian.Collections;

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
        /// Defines default value for <see cref="DataChannel"/>.
        /// </summary>
        public const string DefaultDataChannel = "port=38402";

        // Fields
        private HistorianIArchive m_archive;
        private HistorianDatabaseConfig m_archiveInfo;
        private string m_instanceName;
        private string[] m_archivePaths;
        private string m_dataChannel;
        private bool m_inMemoryArchive;
        private readonly HistorianKey m_key;
        private readonly HistorianValue m_value;
        private DataServices m_dataServices;
        private ReplicationProviders m_replicationProviders;
        private bool m_useNamespaceReservation;
        private long m_archivedMeasurements;
        private volatile int m_adapterLoadedCount;
        private bool m_disposed;
        private LogSubscriber m_logSubscriber;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalOutputAdapter"/> class.
        /// </summary>
        public LocalOutputAdapter()
        {
            m_key = new HistorianKey();
            m_value = new HistorianValue();
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
        /// Gets or sets flag that determines if historian data will be archived to memory (temporary archive) or to disk (permanent archive).
        /// </summary>
        [ConnectionStringParameter,
        Description("Determines if historian data will be archived to memory, i.e., set to \"true\" for temporary memory backed archive or set to \"false\" for permanent disk backed archive."),
        DefaultValue(false)]
        public bool InMemoryArchive
        {
            get
            {
                return m_inMemoryArchive;
            }
            set
            {
                m_inMemoryArchive = value;
            }
        }

        /// <summary>
        /// Gets or sets the data paths for the historian.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the data paths for this historian instance. Separate multiple paths with a semi-colon. Leave blank to default to \".\\Archive\\\"."),
        DefaultValue("")]
        public string ArchivePaths
        {
            get
            {
                if ((object)m_archivePaths != null)
                    return string.Join(";", m_archivePaths);

                return "";
            }
            set
            {
                if ((object)value != null)
                {
                    List<string> archivePaths = new List<string>();

                    foreach (string archivePath in value.Split(';'))
                    {
                        string localPath = FilePath.GetAbsolutePath(archivePath);

                        if (!Directory.Exists(localPath))
                        {
                            try
                            {
                                Directory.CreateDirectory(localPath);
                            }
                            catch (Exception ex)
                            {
                                OnProcessException(new InvalidOperationException(string.Format("Failed to create local archive path \"{0}\": {1}", localPath, ex.Message), ex));
                            }
                        }

                        archivePaths.Add(FilePath.GetAbsolutePath(archivePath));
                    }

                    m_archivePaths = archivePaths.ToArray();
                }
                else
                {
                    m_archivePaths = null;
                }
            }
        }

        /// <summary>
        /// Returns a flag that determines if measurements sent to this <see cref="LocalOutputAdapter"/> are destined for archival.
        /// </summary>
        public override bool OutputIsForArchive
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets flag that determines if this <see cref="LocalOutputAdapter"/> uses an asynchronous connection.
        /// </summary>
        protected override bool UseAsyncConnect
        {
            get
            {
                return true;
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
                status.AppendFormat("  Primary archive location: {0}\r\n", FilePath.TrimFileName(ArchivePaths.Split(';')[0], 51));
                status.AppendFormat("         In memory archive: {0}\r\n", InMemoryArchive);
                status.AppendFormat("      Network data channel: {0}\r\n", DataChannel.ToNonNullString(DefaultDataChannel));
                status.Append(m_dataServices.Status);
                status.AppendLine();
                status.Append(m_replicationProviders.Status);
                Common.HistorianServer.Host.GetFullStatus(status);
                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><b>InstanceName</b> is missing from the <see cref="AdapterBase.Settings"/>.</exception>
        public override void Initialize()
        {
            base.Initialize();

            //string refreshMetadata;

            string errorMessage = "{0} is missing from Settings - Example: instanceName=default; archivePaths={{c:\\Archive\\;d:\\Backups\\}}; dataChannel={{port=9591; interface=0.0.0.0}}";
            Dictionary<string, string> settings = Settings;
            string setting;

            // Validate settings.
            if (!settings.TryGetValue("instanceName", out m_instanceName))
                throw new ArgumentException(string.Format(errorMessage, "instanceName"));

            if (!settings.TryGetValue("archivePaths", out setting))
                setting = FilePath.GetAbsolutePath("Archive");

            if (!settings.TryGetValue("dataChannel", out m_dataChannel))
                m_dataChannel = DefaultDataChannel;

            ArchivePaths = setting;

            // Establish archive information for this historian instance

            m_archiveInfo = new HistorianDatabaseConfig(InstanceName, m_archivePaths.First(), true);
            m_archiveInfo.ImportPaths.AddRange(m_archivePaths.Skip(1));
            if (m_inMemoryArchive)
                throw new NotImplementedException("In Memory Mode Not Yet Supported");

            // TODO: Determine where these parameters (or similar) are definable and expose through historian instance or elsewhere so that they can be configured by adapter parameters
            //m_archive.FileSize = 100;
            //m_archive.CompressData = false;
            //m_archive.PersistSettings = true;
            //m_archive.SettingsCategory = m_instanceName + m_archive.SettingsCategory;
            //m_archive.RolloverStart += Archive_RolloverStart;
            //m_archive.RolloverComplete += Archive_RolloverComplete;
            //m_archive.RolloverException += Archive_RolloverException;
            //m_archive.Initialize();

            if (settings.TryGetValue("useNamespaceReservation", out setting))
                m_useNamespaceReservation = setting.ParseBoolean();
            else
                m_useNamespaceReservation = false;

            // Provide web service support.
            m_dataServices = new DataServices();
            m_dataServices.AdapterCreated += DataServices_AdapterCreated;
            m_dataServices.AdapterLoaded += DataServices_AdapterLoaded;
            m_dataServices.AdapterUnloaded += DataServices_AdapterUnloaded;
            m_dataServices.AdapterLoadException += AdapterLoader_AdapterLoadException;

            // Provide archive replication support.
            m_replicationProviders = new ReplicationProviders();
            m_replicationProviders.AdapterCreated += ReplicationProviders_AdapterCreated;
            m_replicationProviders.AdapterLoaded += ReplicationProviders_AdapterLoaded;
            m_replicationProviders.AdapterUnloaded += ReplicationProviders_AdapterUnloaded;
            m_replicationProviders.AdapterLoadException += AdapterLoader_AdapterLoadException;
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        /// <param name="maxLength">Maximum length of the status message.</param>
        /// <returns>Text of the status message.</returns>
        public override string GetShortStatus(int maxLength)
        {
            return string.Format("Archived {0} measurements {1}.", m_archivedMeasurements, "to disk").CenterText(maxLength);
        }

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
                        if (m_dataServices != null)
                        {
                            m_dataServices.AdapterCreated -= DataServices_AdapterCreated;
                            m_dataServices.AdapterLoaded -= DataServices_AdapterLoaded;
                            m_dataServices.AdapterUnloaded -= DataServices_AdapterUnloaded;
                            m_dataServices.AdapterLoadException -= AdapterLoader_AdapterLoadException;
                            m_dataServices.Dispose();
                        }

                        if (m_replicationProviders != null)
                        {
                            m_replicationProviders.AdapterCreated -= ReplicationProviders_AdapterCreated;
                            m_replicationProviders.AdapterLoaded -= ReplicationProviders_AdapterLoaded;
                            m_replicationProviders.AdapterUnloaded -= ReplicationProviders_AdapterUnloaded;
                            m_replicationProviders.AdapterLoadException -= AdapterLoader_AdapterLoadException;
                            m_replicationProviders.Dispose();
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
        /// Attempts to connect to this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        protected override void AttemptConnection()
        {
            m_logSubscriber = Logger.CreateSubscriber();
            m_logSubscriber.Verbose = VerboseLevel.NonDebug;
            m_logSubscriber.Log += m_logSubscriber_Log;
            // Open archive files
            Common.HistorianServer.Host.AddDatabase(m_archiveInfo);
            m_archive = Common.HistorianServer[InstanceName];

            m_adapterLoadedCount = 0;

            // Initialization of services needs to occur after files are open
            m_dataServices.Initialize();
            m_replicationProviders.Initialize();

            OnConnected();
        }

        void m_logSubscriber_Log(LogMessage logMessage)
        {
            OnStatusMessage(logMessage.ToString());
        }

        /// <summary>
        /// Attempts to disconnect from this <see cref="LocalOutputAdapter"/>.
        /// </summary>
        protected override void AttemptDisconnection()
        {
            m_archive = null;
            Common.HistorianServer.Host.RemoveDatabase(m_archiveInfo.DatabaseName);

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
                // 
                m_key.Timestamp = (ulong)(long)measurement.Timestamp;
                m_key.PointID = measurement.Key.ID;

                // Since current time-series measurements are basically all floats - values fit into first value,
                // this will change as value types for time-series framework expands
                m_value.Value1 = BitMath.ConvertToUInt64((float)measurement.AdjustedValue);
                m_value.Value3 = (ulong)measurement.StateFlags;

                m_archive.Write(m_key, m_value);
            }

            m_archivedMeasurements += measurements.Length;
        }

        // TODO: Need to get historian to bubble-up these kinds of messages / events
        //private void Archive_RolloverStart(object sender, EventArgs e)
        //{
        //    OnStatusMessage("Archive is being rolled over...");
        //}

        //private void Archive_RolloverComplete(object sender, EventArgs e)
        //{
        //    OnStatusMessage("Archive rollover is complete.");
        //}

        //private void Archive_RolloverException(object sender, EventArgs<Exception> e)
        //{
        //    OnProcessException(e.Argument);
        //    OnStatusMessage("Archive rollover failed - {0}", e.Argument.Message);
        //}

        private void DataServices_AdapterCreated(object sender, EventArgs<IDataService> e)
        {
            e.Argument.SettingsCategory = InstanceName.ToLowerInvariant() + e.Argument.SettingsCategory;
        }

        private void DataServices_AdapterLoaded(object sender, EventArgs<IDataService> e)
        {
            e.Argument.Archive = m_archive;
            e.Argument.ServiceProcessException += DataServices_ServiceProcessException;
            OnStatusMessage("{0} has been loaded.", e.Argument.GetType().Name);

            m_adapterLoadedCount++;
        }

        private void DataServices_AdapterUnloaded(object sender, EventArgs<IDataService> e)
        {
            e.Argument.Archive = null;
            e.Argument.ServiceProcessException -= DataServices_ServiceProcessException;
            OnStatusMessage("{0} has been unloaded.", e.Argument.GetType().Name);
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
            OnStatusMessage("{0} has been loaded.", e.Argument.GetType().Name);

            m_adapterLoadedCount++;
        }

        private void ReplicationProviders_AdapterUnloaded(object sender, EventArgs<IReplicationProvider> e)
        {
            e.Argument.ReplicationStart -= ReplicationProvider_ReplicationStart;
            e.Argument.ReplicationComplete -= ReplicationProvider_ReplicationComplete;
            e.Argument.ReplicationProgress -= ReplicationProvider_ReplicationProgress;
            e.Argument.ReplicationException -= ReplicationProvider_ReplicationException;
            OnStatusMessage("{0} has been unloaded.", e.Argument.GetType().Name);
        }

        private void AdapterLoader_AdapterLoadException(object sender, EventArgs<Exception> e)
        {
            OnProcessException(e.Argument);
        }

        private void DataServices_ServiceProcessException(object sender, EventArgs<Exception> e)
        {
            OnProcessException(e.Argument);
        }

        private void ReplicationProvider_ReplicationStart(object sender, EventArgs e)
        {
            OnStatusMessage("{0} has started archive replication...", sender.GetType().Name);
        }

        private void ReplicationProvider_ReplicationComplete(object sender, EventArgs e)
        {
            OnStatusMessage("{0} has finished archive replication.", sender.GetType().Name);
        }

        private void ReplicationProvider_ReplicationProgress(object sender, EventArgs<ProcessProgress<int>> e)
        {
            OnStatusMessage("{0} has replicated archive file {1}.", sender.GetType().Name, e.Argument.ProgressMessage);
        }

        private void ReplicationProvider_ReplicationException(object sender, EventArgs<Exception> e)
        {
            OnProcessException(e.Argument);
        }

        #endregion

        #region [ Static ]

        // Static Methods

        // Apply historian configuration optimizations at start-up
        private static void OptimizeLocalHistorianSettings(IDbConnection connection, Type adapterType, string nodeIDQueryString, string arguments, Action<object, EventArgs<string>> statusMessage, Action<object, EventArgs<Exception>> processException)
        {
            // Make sure setting exists to allow user to by-pass local historian optimizations at startup
            ConfigurationFile configFile = ConfigurationFile.Current;
            CategorizedSettingsElementCollection settings = configFile.Settings["systemSettings"];
            settings.Add("OptimizeLocalHistorianSettings", true, "Determines if the defined local historians will have their settings optimized at startup");

            // See if this node should optimize local historian settings
            if (settings["OptimizeLocalHistorianSettings"].ValueAsBoolean())
            {
                statusMessage("LocalOutputAdapter", new EventArgs<string>("Optimizing settings for local historians..."));

                // Load the defined local system historians
                IEnumerable<DataRow> historians = connection.RetrieveData(adapterType, string.Format("SELECT AdapterName FROM RuntimeHistorian WHERE NodeID = {0} AND TypeName = 'openHistorian.Adapters.LocalOutputAdapter'", nodeIDQueryString)).AsEnumerable();

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

                    if (name.EndsWith("AdoMetadataProvider") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("AdoMetadataProvider"))) < 0)
                        categoriesToRemove.Add(name);

                    if (name.EndsWith("OleDbMetadataProvider") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("OleDbMetadataProvider"))) < 0)
                        categoriesToRemove.Add(name);

                    if (name.EndsWith("RestWebServiceMetadataProvider") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("RestWebServiceMetadataProvider"))) < 0)
                        categoriesToRemove.Add(name);

                    if (name.EndsWith("MetadataService") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("MetadataService"))) < 0)
                        categoriesToRemove.Add(name);

                    if (name.EndsWith("TimeSeriesDataService") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("TimeSeriesDataService"))) < 0)
                        categoriesToRemove.Add(name);

                    if (name.EndsWith("HadoopReplicationProvider") && validHistorians.BinarySearch(name.Substring(0, name.IndexOf("HadoopReplicationProvider"))) < 0)
                        categoriesToRemove.Add(name);
                }

                if (categoriesToRemove.Count > 0)
                {
                    statusMessage("LocalOutputAdapter", new EventArgs<string>("Removing unused local historian configuration settings..."));

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
