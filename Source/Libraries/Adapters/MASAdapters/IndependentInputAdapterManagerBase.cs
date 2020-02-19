//******************************************************************************************************
//  IndependentInputAdapterManagerBase.cs - Gbtc
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using GSF.Data;
using GSF.Diagnostics;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.Units.EE;
using static MAS.IndependentAdapterManagerExtensions;

namespace MAS
{
    /// <summary>
    /// Represents an adapter base class that provides functionality to manage and distribute measurements to a collection of input adapters.
    /// </summary>
    public abstract class IndependentInputAdapterManagerBase : InputAdapterCollection, IIndependentAdapterManager
    {
        #region [ Members ]

        // Fields
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="IndependentInputAdapterManagerBase"/>.
        /// </summary>
        protected IndependentInputAdapterManagerBase() => this.HandleConstruct();

        #endregion

        #region [ Properties ]

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
        public int ConfigurationReloadWaitAttempts { get; set; } = DefaultConfigurationReloadWaitAttempts;

        /// <summary>
        /// Gets or sets the connection string used for database operations. Leave blank to use local configuration database defined in "systemSettings".
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the connection string used for database operations. Leave blank to use local configuration database defined in \"systemSettings\".")]
        [DefaultValue(DefaultDatabaseConnectionString)]
        public string DatabaseConnnectionString { get; set; }

        /// <summary>
        /// Gets or sets the provider string used for database operations. Defaults to a SQL Server provider string.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the provider string used for database operations. Defaults to a SQL Server provider string.")]
        [DefaultValue(DefaultDatabaseProviderString)]
        public string DatabaseProviderString { get; set; }

        /// <summary>
        /// Gets or sets template for output measurement point tag names.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement point tag names, typically an expression like \"" + DefaultPointTagTemplate + "\".")]
        [DefaultValue(DefaultPointTagTemplate)]
        public string PointTagTemplate { get; set; } = DefaultPointTagTemplate;

        /// <summary>
        /// Gets or sets template for local signal reference measurement name for source historian point.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement signal reference names, typically an expression like \"" + DefaultSignalReferenceTemplate + "\".")]
        [DefaultValue(DefaultSignalReferenceTemplate)]
        public string SignalReferenceTemplate { get; set; } = DefaultSignalReferenceTemplate;

        /// <summary>
        /// Gets or sets signal type for output measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the signal type for output measurements.")]
        [DefaultValue(typeof(SignalType), DefaultSignalType)]
        public SignalType SignalType { get; set; } = (SignalType)Enum.Parse(typeof(SignalType), DefaultSignalType);

        /// <summary>
        /// Gets or sets the target historian acronym for output measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the target historian acronym for output measurements.")]
        [DefaultValue(DefaultTargetHistorianAcronym)]
        public string TargetHistorianAcronym { get; set; } = DefaultTargetHistorianAcronym;

        /// <summary>
        /// Gets or sets <see cref="DataSet"/> based data source used to load each <see cref="IAdapter"/>. Updates
        /// to this property will cascade to all adapters in this <see cref="IndependentInputAdapterManagerBase"/>.
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
        public int InputsPerAdapter { get; } = 0;

        /// <summary>
        /// Gets or sets the the index into the per adapter input measurements to use for target adapter name.
        /// </summary>
        public int InputMeasurementUsedForName { get; set; } = 0;

        /// <summary>
        /// Gets output measurement names.
        /// </summary>
        public abstract ReadOnlyCollection<string> OutputNames { get; }

        /// <summary>
        /// Gets or sets flag that determines if the <see cref="IndependentInputAdapterManagerBase"/> adapter
        /// <see cref="AdapterCollectionBase{T}.ConnectionString"/> should be automatically parsed every time
        /// the <see cref="DataSource"/> is updated without requiring adapter to be reinitialized. Defaults
        /// to <c>true</c> to allow child adapters to come and go based on updates to system configuration.
        /// </summary>
        protected bool AutoReparseConnectionString { get; set; } = true;

        SignalType[] IIndependentAdapterManager.InputMeasurementKeyTypes { get; } = null;

        /// <summary>
        /// Gets output measurement <see cref="SignalType"/>'s for each of the <see cref="AdapterBase.OutputMeasurements"/>, if any.
        /// </summary>
        public virtual SignalType[] OutputMeasurementTypes { get; private set; }

        /// <summary>
        /// Returns the detailed status of the <see cref="IndependentInputAdapterManagerBase"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.Append(this.HandleStatus());
                status.Append(base.Status);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="IndependentInputAdapterManagerBase"/> object and optionally releases the managed resources.
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
        /// Initializes the <see cref="IndependentInputAdapterManagerBase" />.
        /// </summary>
        public override void Initialize() => this.HandleInitialize();

        /// <summary>
        /// Parses connection string. Derived classes should override for custom connection string parsing.
        /// </summary>
        public virtual void ParseConnectionString() => this.HandleParseConnectionString();

        /// <summary>
        /// Notifies derived classes that configuration has been reloaded
        /// </summary>
        public virtual void ConfigurationReloaded() { }

        /// <summary>
        /// Recalculates routing tables.
        /// </summary>
        public void RecalculateRoutingTables() => this.HandleRecalculateRoutingTables();

        /// <summary>
        /// Gets a short one-line status of this <see cref="IndependentInputAdapterManagerBase"/>.
        /// </summary>
        /// <param name="maxLength">Maximum number of available characters for display.</param>
        /// <returns>A short one-line summary of the current status of the <see cref="IndependentInputAdapterManagerBase"/>.</returns>
        public override string GetShortStatus(int maxLength) => this.HandleGetShortStatus(maxLength);

        /// <summary>
        /// Gets configured database connection.
        /// </summary>
        /// <returns>New ADO data connection based on configured settings.</returns>
        public AdoDataConnection GetConfiguredConnection() => this.HandleGetConfiguredConnection();

        #endregion

        #region [ IIndependentAdapterManager Implementation ]

        RoutingTables IIndependentAdapterManager.RoutingTables { get; set; }

        string IIndependentAdapterManager.OriginalDataMember { get; set; }

        uint IIndependentAdapterManager.AdapterIDCounter { get; set; }

        ManualResetEventSlim IIndependentAdapterManager.ConfigurationReloadedWaitHandle { get; set; }

        bool IIndependentAdapterManager.AutoReparseConnectionString { get => AutoReparseConnectionString; set => AutoReparseConnectionString = value; }

        void IIndependentAdapterManager.OnConfigurationChanged() => OnConfigurationChanged();

        void IIndependentAdapterManager.OnInputMeasurementKeysUpdated() => OnInputMeasurementKeysUpdated();

        void IIndependentAdapterManager.OnStatusMessage(MessageLevel level, string status, string eventName, MessageFlags flags) => OnStatusMessage(level, status, eventName, flags);

        void IIndependentAdapterManager.OnProcessException(MessageLevel level, Exception exception, string eventName, MessageFlags flags) => OnProcessException(level, exception, eventName, flags);

        #endregion
    }
}
