//******************************************************************************************************
//  IndependentActionAdapterManagerBase.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/13/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using GSF;
using GSF.Data;
using GSF.Diagnostics;
using GSF.Reflection;
using GSF.Threading;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.Units.EE;
using MeasurementRecord = MAS.Model.Measurement;
using static MAS.IndependentAdapterManagerExtensions;

namespace MAS
{
    /// <summary>
    /// Represents an adapter base class that provides functionality to manage and distribute measurements to a collection of action adapters.
    /// </summary>
    public abstract class IndependentActionAdapterManagerBase<TAdapter> : ActionAdapterCollection, IIndependentAdapterManager where TAdapter : IAdapter, new()
    {
        #region [ Members ]

        // Constants
        
        /// <summary>
        /// Defines the default value for the <see cref="FramesPerSecond"/>.
        /// </summary>
        public const int DefaultFramesPerSecond = 30;

        /// <summary>
        /// Defines the default value for the <see cref="LagTime"/>.
        /// </summary>
        public const double DefaultLagTime = 5.0D;

        /// <summary>
        /// Defines the default value for the <see cref="LeadTime"/>.
        /// </summary>
        public const double DefaultLeadTime = 5.0D;

        // Fields
        private ShortSynchronizedOperation m_manageChildAdapters;
        private readonly object m_adapterInputSync;
        private bool m_adapterInputReady;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="IndependentActionAdapterManagerBase{TAdapter}"/>.
        /// </summary>
        protected IndependentActionAdapterManagerBase()
        {
            this.HandleConstruct();
            m_adapterInputSync = new object();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets primary keys of input measurements for the <see cref="IndependentActionAdapterManagerBase{TAdapter}"/>.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines primary keys of input measurements the adapter expects; can be one of a filter expression, measurement key, point tag or Guid.")]
        [CustomConfigurationEditor("GSF.TimeSeries.UI.WPF.dll", "GSF.TimeSeries.UI.Editors.MeasurementEditor")]
        [DefaultValue(null)]
        public override MeasurementKey[] InputMeasurementKeys
        {
            get => base.InputMeasurementKeys;
            set
            {
                lock (m_adapterInputSync)
                    base.InputMeasurementKeys = value;
                
                InputMeasurementKeyTypes = DataSource.GetSignalTypes(value);
            }
        }

        /// <summary>
        /// Gets or sets output measurements that the <see cref="AdapterBase"/> will produce, if any.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines primary keys of output measurements the adapter expects; can be one of a filter expression, measurement key, point tag or Guid.")]
        [CustomConfigurationEditor("GSF.TimeSeries.UI.WPF.dll", "GSF.TimeSeries.UI.Editors.MeasurementEditor")]
        [DefaultValue(null)]
        public override IMeasurement[] OutputMeasurements
        {
            get => base.OutputMeasurements;
            set
            {
                base.OutputMeasurements = value;
                OutputMeasurementTypes = DataSource.GetSignalTypes(value);
            }
        }

        /// <summary>
        /// Gets or sets the number of frames per second applied to each adapter.
        /// </summary>
        /// <remarks>
        /// Valid frame rates for a <see cref="ConcentratorBase"/> are greater than 0 frames per second.
        /// </remarks>
        [ConnectionStringParameter]
        [Description("Defines the number of frames per second applied to each adapter.")]
        [DefaultValue(DefaultFramesPerSecond)]
        public virtual int FramesPerSecond { get; set; } = DefaultFramesPerSecond;

        /// <summary>
        /// Gets or sets the allowed past time deviation tolerance, in seconds (can be sub-second) applied to each adapter.
        /// </summary>
        /// <remarks>
        /// <para>Defines the time sensitivity to past measurement timestamps.</para>
        /// <para>The number of seconds allowed before assuming a measurement timestamp is too old.</para>
        /// <para>This becomes the amount of delay introduced by the concentrator to allow time for data to flow into the system.</para>
        /// </remarks>
        [ConnectionStringParameter]
        [Description("Defines the allowed past time deviation tolerance, in seconds (can be sub-second) applied to each adapter.")]
        [DefaultValue(DefaultLagTime)]
        public virtual double LagTime { get; set; } = DefaultLagTime;

        /// <summary>
        /// Gets or sets the allowed future time deviation tolerance, in seconds (can be sub-second) applied to each adapter.
        /// </summary>
        /// <remarks>
        /// <para>Defines the time sensitivity to future measurement timestamps.</para>
        /// <para>The number of seconds allowed before assuming a measurement timestamp is too advanced.</para>
        /// <para>This becomes the tolerated +/- accuracy of the local clock to real-time.</para>
        /// </remarks>
        [ConnectionStringParameter]
        [Description("Defines the allowed future time deviation tolerance, in seconds (can be sub-second) applied to each adapter.")]
        [DefaultValue(DefaultLeadTime)]
        public virtual double LeadTime { get; set; } = DefaultLeadTime;

        /// <summary>
        /// Gets or sets the wait timeout, in milliseconds, that system wait for system configuration reload to complete.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the wait timeout, in milliseconds, that system wait for system configuration reload to complete.")]
        [DefaultValue(DefaultConfigurationReloadWaitTimeout)]
        public int ConfigurationReloadWaitTimeout { get; set; } = DefaultConfigurationReloadWaitTimeout;

        /// <summary>
        /// Gets or sets the total number of attempts to wait for system configuration reloads when waiting for configuration updates to be available.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the total number of attempts to wait for system configuration reloads when waiting for configuration updates to be available.")]
        [DefaultValue(DefaultConfigurationReloadWaitAttempts)]
        public virtual int ConfigurationReloadWaitAttempts { get; set; } = DefaultConfigurationReloadWaitAttempts;

        /// <summary>
        /// Gets or sets the connection string used for database operations. Leave blank to use local configuration database defined in "systemSettings".
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the connection string used for database operations. Leave blank to use local configuration database defined in \"systemSettings\".")]
        [DefaultValue(DefaultDatabaseConnectionString)]
        public virtual string DatabaseConnnectionString { get; set; }

        /// <summary>
        /// Gets or sets the provider string used for database operations. Defaults to a SQL Server provider string.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the provider string used for database operations. Defaults to a SQL Server provider string.")]
        [DefaultValue(DefaultDatabaseProviderString)]
        public virtual string DatabaseProviderString { get; set; }

        /// <summary>
        /// Gets or sets template for output measurement point tag names.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement point tag names, typically an expression like \"" + DefaultPointTagTemplate + "\".")]
        [DefaultValue(DefaultPointTagTemplate)]
        public virtual string PointTagTemplate { get; set; } = DefaultPointTagTemplate;

        /// <summary>
        /// Gets or sets template for local signal reference measurement name for source historian point.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement signal reference names, typically an expression like \"" + DefaultSignalReferenceTemplate + "\".")]
        [DefaultValue(DefaultSignalReferenceTemplate)]
        public virtual string SignalReferenceTemplate { get; set; } = DefaultSignalReferenceTemplate;

        /// <summary>
        /// Gets or sets signal type for output measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the signal type for output measurements.")]
        [DefaultValue(typeof(SignalType), DefaultSignalType)]
        public virtual SignalType SignalType { get; set; } = (SignalType)Enum.Parse(typeof(SignalType), DefaultSignalType);

        /// <summary>
        /// Gets or sets the target historian acronym for output measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the target historian acronym for output measurements.")]
        [DefaultValue(DefaultTargetHistorianAcronym)]
        public virtual string TargetHistorianAcronym { get; set; } = DefaultTargetHistorianAcronym;

        /// <summary>
        /// Gets or sets <see cref="DataSet"/> based data source used to load each <see cref="IAdapter"/>. Updates
        /// to this property will cascade to all adapters in this <see cref="IndependentActionAdapterManagerBase{TAdapter}"/>.
        /// </summary>
        public override DataSet DataSource
        {
            get => base.DataSource;
            set
            {
                if (!this.DataSourceChanged(value))
                    return;

                base.DataSource = value;
                this.HandleUpdateDataSource();
            }
        }

        /// <summary>
        /// Gets number of input measurement required by each adapter.
        /// </summary>
        public abstract int InputsPerAdapter { get; }

        /// <summary>
        /// Gets or sets the index into the per adapter input measurements to use for target adapter name.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the index into the per adapter input measurements to use for target adapter name.")]
        public virtual int InputMeasurementIndexUsedForName { get; set; }

        /// <summary>
        /// Gets output measurement names.
        /// </summary>
        public abstract ReadOnlyCollection<string> OutputNames { get; }

        /// <summary>
        /// Gets or sets flag that determines if the <see cref="IndependentActionAdapterManagerBase{TAdapter}"/> adapter
        /// <see cref="AdapterCollectionBase{T}.ConnectionString"/> should be automatically parsed every time
        /// the <see cref="DataSource"/> is updated without requiring adapter to be reinitialized. Defaults
        /// to <c>true</c> to allow child adapters to come and go based on updates to system configuration.
        /// </summary>
        protected virtual bool AutoReparseConnectionString { get; set; } = true;

        /// <summary>
        /// Gets input measurement <see cref="SignalType"/>'s for each of the <see cref="ActionAdapterBase.InputMeasurementKeys"/>, if any.
        /// </summary>
        public virtual SignalType[] InputMeasurementKeyTypes { get; private set; }

        /// <summary>
        /// Gets output measurement <see cref="SignalType"/>'s for each of the <see cref="ActionAdapterBase.OutputMeasurements"/>, if any.
        /// </summary>
        public virtual SignalType[] OutputMeasurementTypes { get; private set; }

        /// <summary>
        /// Gets or sets current adapter ID counter.
        /// </summary>
        public uint AdapterIDCounter { get; set; }
        
        /// <summary>
        /// Returns the detailed status of the <see cref="IndependentActionAdapterManagerBase{TAdapter}"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.AppendFormat("         Frames Per Second: {0:N0}", FramesPerSecond);
                status.AppendLine();
                status.AppendFormat("      Lag Time / Lead Time: {0:N3} / {1:N3}", LagTime, LeadTime);
                status.AppendLine();
                status.Append(this.HandleStatus());
                status.Append(base.Status);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="IndependentActionAdapterManagerBase{TAdapter}"/> object and optionally releases the managed resources.
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

                this.HandleDispose();
            }
            finally
            {
                m_disposed = true;          // Prevent duplicate dispose.
                base.Dispose(disposing);    // Call base class Dispose().
            }
        }

        /// <summary>
        /// Initializes the <see cref="IndependentActionAdapterManagerBase{TAdapter}" />.
        /// </summary>
        public override void Initialize() => this.HandleInitialize();

        /// <summary>
        /// Initializes management operations for child adapters.
        /// </summary>
        protected virtual void InitializeChildAdapterManagement()
        {
            lock (m_adapterInputSync)
                m_adapterInputReady = true;

            if (m_manageChildAdapters != null)
                return;

            if (InputsPerAdapter <= 0 || OutputNames?.Count <= 0 || InputMeasurementKeys.Length <= 0)
                return;

            // Define a synchronized operation to manage bulk collection of child adapters
            m_manageChildAdapters = new ShortSynchronizedOperation(ManageChildAdapters, ex => OnProcessException(MessageLevel.Warning, ex));

            // Kick off initial child adapter management operations
            m_manageChildAdapters.RunOnceAsync();
        }

        /// <summary>
        /// Validates that an even number of inputs are provided for specified <see cref="InputsPerAdapter"/>.
        /// </summary>
        protected void ValidateEvenInputCount()
        {
            int remainder = InputMeasurementKeys.Length % InputsPerAdapter;

            if (remainder == 0)
                return;

            int adjustedCount = InputMeasurementKeys.Length - remainder;
            OnStatusMessage(MessageLevel.Warning, $"Uneven number of inputs provided, adjusting total number of inputs to {adjustedCount:N0}. Expected {InputsPerAdapter:N0} per adapter, received {InputMeasurementKeys.Length:N0} total measurements, leaving {InputsPerAdapter - remainder:N0} needed.");
            InputMeasurementKeys = InputMeasurementKeys.Take(adjustedCount).ToArray();
        }

        /// <summary>
        /// Parses connection string. Derived classes should override for custom connection string parsing.
        /// </summary>
        public virtual void ParseConnectionString()
        {
            lock (m_adapterInputSync)
            {
                m_adapterInputReady = false;
                this.HandleParseConnectionString();
            }

            if (FramesPerSecond < 1)
                FramesPerSecond = DefaultFramesPerSecond;

            if (LagTime < 0.0D)
                LagTime = DefaultLagTime;

            if (LeadTime < 0.0D)
                LeadTime = DefaultLeadTime;
        }

        /// <summary>
        /// Notifies derived classes that configuration has been reloaded
        /// </summary>
        public virtual void ConfigurationReloaded()
        {
            lock (m_adapterInputSync)
            {
                if (m_adapterInputReady)
                    m_manageChildAdapters?.RunOnceAsync();
            }
        }

        /// <summary>
        /// Recalculates routing tables.
        /// </summary>
        public virtual void RecalculateRoutingTables() => this.HandleRecalculateRoutingTables();

        /// <summary>
        /// Queues a collection of measurements for processing to each <see cref="IActionAdapter"/> connected to this <see cref="IndependentActionAdapterManagerBase{TAdapter}"/>.
        /// </summary>
        /// <param name="measurements">Measurements to queue for processing.</param>
        public override void QueueMeasurementsForProcessing(IEnumerable<IMeasurement> measurements) => this.HandleQueueMeasurementsForProcessing(measurements);

        /// <summary>
        /// Gets a short one-line status of this <see cref="IndependentActionAdapterManagerBase{TAdapter}"/>.
        /// </summary>
        /// <param name="maxLength">Maximum number of available characters for display.</param>
        /// <returns>A short one-line summary of the current status of the <see cref="IndependentActionAdapterManagerBase{TAdapter}"/>.</returns>
        public override string GetShortStatus(int maxLength) => this.HandleGetShortStatus(maxLength);

        /// <summary>
        /// Enumerates child adapters.
        /// </summary>
        [AdapterCommand("Enumerates child adapters.")]
        public virtual void EnumerateAdapters() => this.HandleEnumerateAdapters();

        /// <summary>
        /// Gets subscriber information for specified client connection.
        /// </summary>
        /// <param name="adapterIndex">Enumerated index for child adapter.</param>
        /// <returns>Status for adapter with specified <paramref name="adapterIndex"/>.</returns>
        [AdapterCommand("Gets subscriber information for specified client connection.")]
        public virtual string GetAdapterStatus(int adapterIndex) => this.HandleGetAdapterStatus(adapterIndex);
        
        /// <summary>
        /// Gets configured database connection.
        /// </summary>
        /// <returns>New ADO data connection based on configured settings.</returns>
        public AdoDataConnection GetConfiguredConnection() => this.HandleGetConfiguredConnection();

        private void ManageChildAdapters()
        {
            MeasurementKey[] measurementKeys;

            lock (m_adapterInputSync)
            {
                if (!m_adapterInputReady)
                    return;

                measurementKeys = InputMeasurementKeys;
            }

            HashSet<string> activeAdapterNames = new HashSet<string>(StringComparer.Ordinal);
            List<TAdapter> adapters = new List<TAdapter>();
            HashSet<Guid> signalIDs = new HashSet<Guid>();

            // Create settings dictionary for connection string to use with primary child adapters
            Dictionary<string, string> settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (PropertyInfo property in GetType().GetProperties())
            {
                if (property.AttributeExists<PropertyInfo, ConnectionStringParameterAttribute>())
                    settings[property.Name] = $"{property.GetValue(this)}";
            }

            int inputsPerAdapter = InputsPerAdapter;
            int outputsPerAdapter = OutputNames.Count;
            int nameIndex = InputMeasurementIndexUsedForName;

            // Create child adapter for provided inputs to the parent bulk collection-based adapter
            for (int i = 0; i < measurementKeys.Length; i += inputsPerAdapter)
            {
                Guid[] inputs = new Guid[inputsPerAdapter];
                Guid[] outputs = new Guid[outputsPerAdapter];

                // Adapter inputs are presumed to be grouped together
                for (int j = 0; j < inputsPerAdapter && i + j < measurementKeys.Length; j++)
                    inputs[j] = measurementKeys[i + j].SignalID;

                string inputName = this.LookupPointTag(inputs[nameIndex]);
                string adapterName = $"{Name}!{inputName}";

                // Track active adapter names so that adapters that no longer have sources can be removed
                activeAdapterNames.Add(adapterName);

                // See if child adapter already exists
                if (this.FindAdapter(adapterName) != null)
                    continue;

                // Setup output measurements for new child adapter
                for (int j = 0; j < outputsPerAdapter; j++)
                {
                    string outputID = $"{adapterName}-{OutputNames[j].ToUpper()}";
                    string outputPointTag = string.Format(PointTagTemplate, outputID);
                    string signalReference = string.Format(SignalReferenceTemplate, outputID);

                    // Get output measurement record, creating a new one if needed
                    MeasurementRecord measurement = this.GetMeasurementRecord(outputPointTag, signalReference, SignalType, TargetHistorianAcronym);

                    // Track output signal IDs
                    signalIDs.Add(measurement.SignalID);
                    outputs[j] = measurement.SignalID;
                }

                // Add inputs and outputs to connection string settings for child adapter
                settings[nameof(InputMeasurementKeys)] = string.Join(";", inputs);
                settings[nameof(OutputMeasurements)] = string.Join(";", outputs);

                adapters.Add(new TAdapter
                {
                    Name = adapterName,
                    ID = AdapterIDCounter++,
                    ConnectionString = settings.JoinKeyValuePairs(),
                    DataSource = DataSource
                });
            }

            // Check for adapters that are no longer referenced and need to be removed
            IEnumerable<IActionAdapter> adaptersToRemove = this.Where<IActionAdapter>(adapter => !activeAdapterNames.Contains(adapter.Name));

            foreach (IActionAdapter adapter in adaptersToRemove)
                Remove(adapter);

            // Host system was notified about configuration changes, i.e., new or updated output measurements.
            // Before initializing child adapters, we wait for this process to complete.
            this.WaitForSignalsToLoad(signalIDs.ToArray());

            // Add new adapters to parent bulk adapter collection, this will auto-initialize each child adapter
            foreach (TAdapter adapter in adapters)
                Add(adapter as IActionAdapter);

            RecalculateRoutingTables();
        }

        #endregion

        #region [ IIndependentAdapterManager Implementation ]

        RoutingTables IIndependentAdapterManager.RoutingTables { get; set; }

        string IIndependentAdapterManager.OriginalDataMember { get; set; }

        ManualResetEventSlim IIndependentAdapterManager.ConfigurationReloadedWaitHandle { get; set; }

        bool IIndependentAdapterManager.AutoReparseConnectionString { get => AutoReparseConnectionString; set => AutoReparseConnectionString = value; }

        void IIndependentAdapterManager.OnConfigurationChanged() => OnConfigurationChanged();

        void IIndependentAdapterManager.OnInputMeasurementKeysUpdated() => OnInputMeasurementKeysUpdated();

        void IIndependentAdapterManager.OnStatusMessage(MessageLevel level, string status, string eventName, MessageFlags flags) => OnStatusMessage(level, status, eventName, flags);

        void IIndependentAdapterManager.OnProcessException(MessageLevel level, Exception exception, string eventName, MessageFlags flags) => OnProcessException(level, exception, eventName, flags);

        #endregion
    }
}
