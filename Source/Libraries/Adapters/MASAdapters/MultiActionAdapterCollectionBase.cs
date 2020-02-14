//******************************************************************************************************
//  MultiActionAdapterCollectionBase.cs - Gbtc
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
using System.Data;
using System.Linq;
using GSF;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.Units.EE;
using ConnectionStringParser = GSF.Configuration.ConnectionStringParser<GSF.TimeSeries.Adapters.ConnectionStringParameterAttribute>;
using MeasurementRecord = MAS.Model.Measurement;
using SignalTypeRecord = MAS.Model.SignalType;
using HistorianRecord = MAS.Model.Historian;

namespace MAS
{
    /// <summary>
    /// Represents an adapter base class that provides functionality to manage and distribute measurements to a collection of action adapters.
    /// </summary>
    public abstract class MultiActionAdapterCollectionBase : ActionAdapterCollection
    {
        #region [ Members ]

        // Fields
        private readonly RoutingTables m_routingTables;
        private readonly string m_originalDataMember;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="MultiActionAdapterCollectionBase"/>.
        /// </summary>
        protected MultiActionAdapterCollectionBase()
        {
            m_originalDataMember = base.DataMember;
            base.DataMember = "[internal]";
            base.Name = $"{GetType().Name} Collection";

            // Create a routing table to handle distribution of measurements to child adapters
            m_routingTables = new RoutingTables { ActionAdapters = this };
            m_routingTables.StatusMessage += RoutingTables_StatusMessage;
            m_routingTables.ProcessException += RoutingTables_ProcessException;

            // Make sure routes are recalculated any time measurements are updated
            InputMeasurementKeysUpdated += (sender, e) => m_routingTables?.CalculateRoutingTables(null);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets <see cref="DataSet"/> based data source used to load each <see cref="IAdapter"/>. Updates
        /// to this property will cascade to all adapters in this <see cref="MultiActionAdapterCollectionBase"/>.
        /// </summary>
        public override DataSet DataSource
        {
            get => base.DataSource;
            set
            {
                if (!DataSourceChanged(value))
                    return;

                base.DataSource = value;
                m_routingTables?.CalculateRoutingTables(null);

                if (AutoReparseConnectionString)
                {
                    // Lookup adapter configuration record
                    DataRow[] records = DataSource.Tables[m_originalDataMember].Select($"AdapterName = '{Name}'");

                    if (records.Length > 0)
                    {
                        // Parsing connection string after any updates will parse input measurement keys causing
                        // a request to update measurements routed to adapter. Derived implementations can then
                        // use DataSourceChanged notification to add or remove child adapters as needed.
                        ConnectionString = records[0]["ConnectionString"].ToNonNullString();
                        ParseConnectionString();
                    }
                    else
                    {
                        OnStatusMessage(MessageLevel.Warning, $"Failed to find adapter \"{Name}\" in \"{m_originalDataMember}\" configuration table. Cannot reload connection string parameters.");
                    }
                }

                DataSourceChanged();
            }
        }

        /// <summary>
        /// Gets or sets flag that determines if the <see cref="MultiActionAdapterCollectionBase"/> adapter
        /// <see cref="AdapterCollectionBase{T}.ConnectionString"/> should be automatically parsed every time
        /// the <see cref="DataSource"/> is updated without requiring adapter to be reinitialized. Defaults
        /// to <c>true</c> to allow child adapters to come and go based on updates to system configuration.
        /// </summary>
        protected bool AutoReparseConnectionString { get; set; } = true;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MultiActionAdapterCollectionBase"/> object and optionally releases the managed resources.
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

                if (m_routingTables == null)
                    return;

                m_routingTables.StatusMessage -= RoutingTables_StatusMessage;
                m_routingTables.ProcessException -= RoutingTables_ProcessException;
                m_routingTables.Dispose();
            }
            finally
            {
                m_disposed = true;          // Prevent duplicate dispose.
                base.Dispose(disposing);    // Call base class Dispose().
            }
        }

        /// <summary>
        /// Initializes the <see cref="MultiActionAdapterCollectionBase" />.
        /// </summary>
        public override void Initialize()
        {
            // We don't call base class initialize since it tries to auto-load adapters from the defined
            // data member - instead, the multi-adapter class implementation manages its own adapters
            Initialized = false;

            ParseConnectionString();

            Initialized = true;
        }

        /// <summary>
        /// Parses connection string. Derived classes should override for custom connection string parsing.
        /// </summary>
        protected virtual void ParseConnectionString()
        {
            // Parse all properties marked with ConnectionStringParameterAttribute from provided ConnectionString value
            ConnectionStringParser parser = new ConnectionStringParser();
            parser.ParseConnectionString(ConnectionString, this);
        }

        /// <summary>
        /// Notifies derived classes that data source has changed.
        /// </summary>
        protected virtual void DataSourceChanged()
        {
        }

        /// <summary>
        /// Recalculates routing tables.
        /// </summary>
        protected void RecalculateRoutingTables() => OnInputMeasurementKeysUpdated(); // Requests route recalculation by IonSession

        /// <summary>
        /// Finds child adapter with specified <paramref name="adapterName"/>.
        /// </summary>
        /// <param name="adapterName">Adapter name to find.</param>
        /// <returns><see cref="IActionAdapter"/> instance with <paramref name="adapterName"/>, if found; otherwise, <c>null</c>.</returns>
        protected IActionAdapter FindAdapter(string adapterName) => this.FirstOrDefault<IActionAdapter>(adapter => adapterName.Equals(adapter.Name));

        /// <summary>
        /// Lookups up point tag name from provided <see cref="MeasurementKey"/>.
        /// </summary>
        /// <param name="key">Key to lookup.</param>
        /// <param name="measurementTable">Measurement table name used for meta-data lookup.</param>
        /// <returns>Point tag name, if found; otherwise, string representation of provided measurement key.</returns>
        protected string LookupPointTag(MeasurementKey key, string measurementTable = "ActiveMeasurements")
        {
            DataRow[] records = DataSource.Tables[measurementTable].Select($"SignalID = '{key.SignalID}'");
            string pointTag = null;

            if (records.Length > 0)
                pointTag = records[0]["PointTag"].ToString();

            if (string.IsNullOrWhiteSpace(pointTag))
                pointTag = key.ToString();

            return pointTag.ToUpper();
        }

        /// <summary>
        /// Gets measurement record, creating it if needed.
        /// </summary>
        /// <param name="pointTag">Point tag of measurement.</param>
        /// <param name="signalReference">Signal reference of measurement.</param>
        /// <param name="signalType">Signal type of measurement.</param>
        /// <param name="targetHistorianAcronym">Acronym of target historian for measurement.</param>
        /// <returns>Measurement record.</returns>
        protected MeasurementRecord GetMeasurementRecord(string pointTag, string signalReference, SignalType signalType, string targetHistorianAcronym)
        {
            // Open database connection as defined in configuration file "systemSettings" category
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<MeasurementRecord> measurementTable = new TableOperations<MeasurementRecord>(connection);
                TableOperations<HistorianRecord> historianTable = new TableOperations<HistorianRecord>(connection);
                TableOperations<SignalTypeRecord> signalTypeTable = new TableOperations<SignalTypeRecord>(connection);

                // Lookup target historian ID
                int? historianID = historianTable.QueryRecordWhere("Acronym = {0}", targetHistorianAcronym)?.ID;

                // Lookup signal type ID
                int signalTypeID = signalTypeTable.QueryRecordWhere("Acronym = {0}", signalType.ToString())?.ID ?? 1;

                // Lookup measurement record by point tag, creating a new record if one does not exist
                MeasurementRecord measurement = measurementTable.QueryRecordWhere("PointTag = {0}", pointTag) ?? measurementTable.NewRecord();

                // Update record fields
                measurement.HistorianID = historianID;
                measurement.PointTag = pointTag;
                measurement.SignalReference = signalReference;
                measurement.SignalTypeID = signalTypeID;
                measurement.Description = $"{signalType} measurement created for {Name} [{GetType().Name}].";

                // Save record updates
                measurementTable.AddNewOrUpdateRecord(measurement);

                // Re-query new records to get any database assigned information, e.g., unique Guid-based signal ID
                if (measurement.PointID == 0)
                    measurement = measurementTable.QueryRecordWhere("PointTag = {0}", pointTag);

                return measurement;
            }
        }

        /// <summary>
        /// Queues a collection of measurements for processing to each <see cref="IActionAdapter"/> connected to this <see cref="MultiActionAdapterCollectionBase"/>.
        /// </summary>
        /// <param name="measurements">Measurements to queue for processing.</param>
        public override void QueueMeasurementsForProcessing(IEnumerable<IMeasurement> measurements)
        {
            // Pass measurements coming into parent collection adapter to routing tables for individual child adapter distribution
            IList<IMeasurement> measurementList = measurements as IList<IMeasurement> ?? measurements.ToList();
            m_routingTables.InjectMeasurements(this, new EventArgs<ICollection<IMeasurement>>(measurementList));
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="MultiActionAdapterCollectionBase"/>.
        /// </summary>
        /// <param name="maxLength">Maximum number of available characters for display.</param>
        /// <returns>A short one-line summary of the current status of the <see cref="MultiActionAdapterCollectionBase"/>.</returns>
        public override string GetShortStatus(int maxLength)
        {
            if (Enabled)
                return $"Processing enabled for {Count:N0} adapters.".CenterText(maxLength);

            return "Processing not enabled".CenterText(maxLength);
        }

        // Determines whether the data in the data source has actually changed when receiving a new data source.
        private bool DataSourceChanged(DataSet newDataSource)
        {
            try
            {
                return !DataSetEqualityComparer.Default.Equals(DataSource, newDataSource);
            }
            catch
            {
                // Function is for optimization, reason for failure is irrelevant
                return true;
            }
        }

        // Make sure to expose any routing table messages
        private void RoutingTables_StatusMessage(object sender, EventArgs<string> e) => OnStatusMessage(MessageLevel.Info, e.Argument);

        // Make sure to expose any routing table exceptions
        private void RoutingTables_ProcessException(object sender, EventArgs<Exception> e) => OnProcessException(MessageLevel.Warning, e.Argument);

        #endregion
    }
}
