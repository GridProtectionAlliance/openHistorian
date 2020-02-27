//******************************************************************************************************
//  IndependentAdapterManagerHandlers.cs - Gbtc
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
//  02/16/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using GSF;
using GSF.Data;
using GSF.Diagnostics;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using ConnectionStringParser = GSF.Configuration.ConnectionStringParser<GSF.TimeSeries.Adapters.ConnectionStringParameterAttribute>;

namespace MAS
{
    // Common implementation extension handlers for independent adapter collection managers.
    internal static class IndependentAdapterManagerHandlers
    {
        /// <summary>
        /// Handles construction steps for a new <see cref="IIndependentAdapterManager"/> instance.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        public static void HandleConstruct(this IIndependentAdapterManager instance)
        {
            instance.OriginalDataMember = instance.DataMember;
            instance.DataMember = "[internal]";
            instance.Name = $"{instance.GetType().Name} Collection";

            instance.RoutingTables = new RoutingTables();

            // ReSharper disable SuspiciousTypeConversion.Global
            switch (instance)
            {
                case InputAdapterCollection inputAdapterCollection:
                    instance.RoutingTables.InputAdapters = inputAdapterCollection;
                    break;
                case ActionAdapterCollection actionAdapterCollection:
                    instance.RoutingTables.ActionAdapters = actionAdapterCollection;
                    break;
                case OutputAdapterCollection outputAdapterCollection:
                    instance.RoutingTables.OutputAdapters = outputAdapterCollection;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(instance));
            }
            // ReSharper restore SuspiciousTypeConversion.Global

            instance.RoutingTables.StatusMessage += RoutingTables_StatusMessage;
            instance.RoutingTables.ProcessException += RoutingTables_ProcessException;

            // Make sure routes are recalculated any time measurements are updated
            instance.InputMeasurementKeysUpdated += (sender, e) => instance.RoutingTables?.CalculateRoutingTables(null);

            instance.ConfigurationReloadedWaitHandle = new ManualResetEventSlim();
        }

        /// <summary>
        /// Disposes resources used by the <see cref="IIndependentAdapterManager"/> instance.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        public static void HandleDispose(this IIndependentAdapterManager instance)
        {
            if (instance.RoutingTables != null)
            {
                instance.RoutingTables.StatusMessage -= RoutingTables_StatusMessage;
                instance.RoutingTables.ProcessException -= RoutingTables_ProcessException;
                instance.RoutingTables.Dispose();
            }

            if (instance.ConfigurationReloadedWaitHandle != null)
            {
                instance.ConfigurationReloadedWaitHandle.Set();
                instance.ConfigurationReloadedWaitHandle.Dispose();
            }
        }

        /// <summary>
        /// Initializes the <see cref="IndependentAdapterManagerExtensions" />.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        public static void HandleInitialize(this IIndependentAdapterManager instance)
        {
            // We don't call base class initialize since it tries to auto-load adapters from the defined
            // data member - instead, the multi-adapter class implementation manages its own adapters
            instance.Initialized = false;

            instance.ParseConnectionString();

            if (instance.ConfigurationReloadWaitTimeout < 0)
                instance.ConfigurationReloadWaitTimeout = 0;

            if (instance.InputMeasurementIndexUsedForName < 0 || instance.InputMeasurementIndexUsedForName > instance.InputsPerAdapter - 1)
                instance.InputMeasurementIndexUsedForName = 0;

            instance.Initialized = true;
        }

        /// <summary>
        /// Sets <see cref="DataSet"/> based data source used to load each <see cref="IAdapter"/>. Updates to this
        /// property will cascade to all adapters in this <see cref="IndependentAdapterManagerExtensions"/> instance.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        public static void HandleUpdateDataSource(this IIndependentAdapterManager instance)
        {
            // Notify any waiting threads that configuration has been reloaded
            instance.ConfigurationReloadedWaitHandle.Set();

            // Update routes for configuration reload
            instance.RoutingTables?.CalculateRoutingTables(null);

            if (instance.AutoReparseConnectionString)
            {
                // Lookup adapter configuration record
                DataRow[] records = instance.DataSource.Tables[instance.OriginalDataMember].Select($"AdapterName = '{instance.Name}'");

                if (records.Length > 0)
                {
                    // Parsing connection string after any updates will parse input measurement keys causing
                    // a request to update measurements routed to adapter. Derived implementations can then
                    // use DataSourceChanged notification to add or remove child adapters as needed.
                    instance.ConnectionString = records[0]["ConnectionString"].ToNonNullString();
                    instance.ParseConnectionString();
                }
                else
                {
                    instance.OnStatusMessage(MessageLevel.Warning, $"Failed to find adapter \"{instance.Name}\" in \"{instance.OriginalDataMember}\" configuration table. Cannot reload connection string parameters.");
                }
            }

            instance.ConfigurationReloaded();
        }

        /// <summary>
        /// Returns the detailed status for the <see cref="IIndependentAdapterManager"/> instance.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        public static string HandleStatus(this IIndependentAdapterManager instance)
        {
            const int MaxMeasurementsToShow = 10;

            StringBuilder status = new StringBuilder();

            status.AppendFormat("        Point Tag Template: {0}", instance.PointTagTemplate);
            status.AppendLine();
            status.AppendFormat(" Signal Reference Template: {0}", instance.SignalReferenceTemplate);
            status.AppendLine();
            status.AppendFormat("        Output Signal Type: {0}", instance.SignalType);
            status.AppendLine();
            status.AppendFormat("  Target Historian Acronym: {0}", instance.TargetHistorianAcronym);
            status.AppendLine();
            status.AppendFormat("        Inputs per Adapter: {0:N0}", instance.InputsPerAdapter);
            status.AppendLine();
            status.AppendFormat(" Input Index Used for Name: {0:N0}", instance.InputMeasurementIndexUsedForName);
            status.AppendLine();
            status.AppendFormat("              Output Names: {0}", string.Join(", ", instance.OutputNames.AsEnumerable() ?? new[] { "" }));
            status.AppendLine();
            status.AppendFormat("Re-parse Connection String: {0}", instance.AutoReparseConnectionString);
            status.AppendLine();
            status.AppendFormat("      Original Data Member: {0}", instance.OriginalDataMember);
            status.AppendLine();
            status.AppendFormat("     Config Reload Timeout: {0:N0} ms", instance.ConfigurationReloadWaitAttempts);
            status.AppendLine();
            status.AppendFormat("    Config Reload Attempts: {0:N0}", instance.ConfigurationReloadWaitTimeout);
            status.AppendLine();
            status.AppendFormat("Database Connection String: {0}", instance.DatabaseConnnectionString ?? "Using <systemSettings>");
            status.AppendLine();
            
            if (!string.IsNullOrWhiteSpace(instance.DatabaseConnnectionString))
            {
                status.AppendFormat("  Custom Database Provider: {0}", instance.DatabaseProviderString ?? "");
                status.AppendLine();
            }

            if (instance.OutputMeasurements != null && instance.OutputMeasurements.Length > instance.OutputMeasurements.Count(m => m.Key == MeasurementKey.Undefined))
            {
                status.AppendFormat("       Output measurements: {0:N0} defined measurements", instance.OutputMeasurements.Length);
                status.AppendLine();
                status.AppendLine();

                for (int i = 0; i < Math.Min(instance.OutputMeasurements.Length, MaxMeasurementsToShow); i++)
                {
                    status.Append(instance.OutputMeasurements[i].ToString().TruncateRight(40).PadLeft(40));
                    status.Append(" ");
                    status.AppendLine(instance.OutputMeasurements[i].ID.ToString());
                }

                if (instance.OutputMeasurements.Length > MaxMeasurementsToShow)
                    status.AppendLine("...".PadLeft(26));

                status.AppendLine();
            }
            if (instance.InputMeasurementKeys != null && instance.InputMeasurementKeys.Length > instance.InputMeasurementKeys.Count(k => k == MeasurementKey.Undefined))
            {
                status.AppendFormat("        Input measurements: {0:N0} defined measurements", instance.InputMeasurementKeys.Length);
                status.AppendLine();
                status.AppendLine();

                for (int i = 0; i < Math.Min(instance.InputMeasurementKeys.Length, MaxMeasurementsToShow); i++)
                    status.AppendLine(instance.InputMeasurementKeys[i].ToString().TruncateRight(25).CenterText(50));

                if (instance.InputMeasurementKeys.Length > MaxMeasurementsToShow)
                    status.AppendLine("...".CenterText(50));

                status.AppendLine();
            }

            return status.ToString();
        }

        /// <summary>
        /// Parses connection string. Derived classes should override for custom connection string parsing.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        public static void HandleParseConnectionString(this IIndependentAdapterManager instance)
        {
            // Parse all properties marked with ConnectionStringParameterAttribute from provided ConnectionString value
            ConnectionStringParser parser = new ConnectionStringParser();
            parser.ParseConnectionString(instance.ConnectionString, instance);

            // Parse input measurement keys like class was a typical adapter
            if (instance.Settings.TryGetValue(nameof(instance.InputMeasurementKeys), out string setting))
                instance.InputMeasurementKeys = AdapterBase.ParseInputMeasurementKeys(instance.DataSource, true, setting);

            // Parse output measurement keys like class was a typical adapter
            if (instance.Settings.TryGetValue(nameof(instance.OutputMeasurements), out setting))
                instance.OutputMeasurements = AdapterBase.ParseOutputMeasurements(instance.DataSource, true, setting);
        }

        /// <summary>
        /// Recalculates routing tables.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        public static void HandleRecalculateRoutingTables(this IIndependentAdapterManager instance) => instance.OnInputMeasurementKeysUpdated(); // Requests route recalculation by IonSession

        /// <summary>
        /// Queues a collection of measurements for processing to each <see cref="IAdapter"/> connected to this <see cref="IndependentAdapterManagerExtensions"/>.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        /// <param name="measurements">Measurements to queue for processing.</param>
        public static void HandleQueueMeasurementsForProcessing(this IIndependentAdapterManager instance, IEnumerable<IMeasurement> measurements)
        {
            // Pass measurements coming into parent collection adapter to routing tables for individual child adapter distribution
            IList<IMeasurement> measurementList = measurements as IList<IMeasurement> ?? measurements.ToList();
            instance.RoutingTables.InjectMeasurements(instance, new EventArgs<ICollection<IMeasurement>>(measurementList));
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="IndependentAdapterManagerExtensions"/>.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        /// <param name="maxLength">Maximum number of available characters for display.</param>
        /// <returns>A short one-line summary of the current status of the <see cref="IndependentAdapterManagerExtensions"/>.</returns>
        public static string HandleGetShortStatus(this IIndependentAdapterManager instance, int maxLength)
        {
            if (instance.Enabled)
                return $"Processing enabled for {instance.Count:N0} adapters.".CenterText(maxLength);

            return "Processing not enabled".CenterText(maxLength);
        }

        /// <summary>
        /// Enumerates child adapters.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        public static void HandleEnumerateAdapters(this IIndependentAdapterManager instance)
        {
            StringBuilder enumeratedAdapters = new StringBuilder();
            IAdapter[] adapters = instance.ToArray();

            enumeratedAdapters.AppendLine($"{instance.Name} Indexed Adapter Enumeration - {adapters.Length:N0} Total:\r\n");

            for (int i = 0; i < adapters.Length; i++)
                enumeratedAdapters.AppendLine($"{i.ToString("N0").PadLeft(5)}: {adapters[i].Name}".TrimWithEllipsisMiddle(79));

            instance.OnStatusMessage(MessageLevel.Info, enumeratedAdapters.ToString());
        }

        /// <summary>
        /// Gets subscriber information for specified client connection.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        /// <param name="adapterIndex">Enumerated index for child adapter.</param>
        public static string HandleGetAdapterStatus(this IIndependentAdapterManager instance, int adapterIndex) => instance[adapterIndex].Status;

        /// <summary>
        /// Gets configured database connection.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        /// <returns>New ADO data connection based on configured settings.</returns>
        public static AdoDataConnection HandleGetConfiguredConnection(this IIndependentAdapterManager instance) => string.IsNullOrWhiteSpace(instance.DatabaseConnnectionString) ?
            new AdoDataConnection("systemSettings") :
            new AdoDataConnection(instance.DatabaseConnnectionString, instance.DatabaseProviderString);

        /// <summary>
        /// Determines whether the data in the data source has actually changed when receiving a new data source.
        /// </summary>
        /// <param name="instance">Target <see cref="IIndependentAdapterManager"/> instance.</param>
        /// <param name="newDataSource">New data source to check.</param>
        /// <returns><c>true</c> if data source has changed; otherwise, <c>false</c>.</returns>
        public static bool DataSourceChanged(this IIndependentAdapterManager instance, DataSet newDataSource)
        {
            try
            {
                return !DataSetEqualityComparer.Default.Equals(instance.DataSource, newDataSource);
            }
            catch
            {
                // Function is for optimization, reason for failure is irrelevant
                return true;
            }
        }

        // Make sure to expose any routing table messages
        private static void RoutingTables_StatusMessage(object sender, EventArgs<string> e)
        {
            if (sender is IIndependentAdapterManager instance)
                instance.OnStatusMessage(MessageLevel.Info, e.Argument);
        }

        // Make sure to expose any routing table exceptions
        private static void RoutingTables_ProcessException(object sender, EventArgs<Exception> e)
        {
            if (sender is IIndependentAdapterManager instance)
                instance.OnProcessException(MessageLevel.Warning, e.Argument);
        }
    }
}