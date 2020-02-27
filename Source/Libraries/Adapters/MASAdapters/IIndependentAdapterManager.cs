//******************************************************************************************************
//  IIndependentAdapterManager.cs - Gbtc
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
using System.Collections.ObjectModel;
using System.Threading;
using GSF.Data;
using GSF.Diagnostics;
using GSF.TimeSeries.Adapters;
using GSF.Units.EE;

namespace MAS
{
    /// <summary>
    /// Represents the interface for implementations of independent adapter collection managers.
    /// </summary>
    public interface IIndependentAdapterManager : IAdapterCollection
    {
        /// <summary>
        /// Gets or sets the wait timeout, in milliseconds, that system wait for system configuration reload to complete.
        /// </summary>
        int ConfigurationReloadWaitTimeout { get; set; }

        /// <summary>
        /// Gets or sets the total number of attempts to wait for system configuration reloads when waiting for configuration updates to be available.
        /// </summary>
        int ConfigurationReloadWaitAttempts { get; set; }

        /// <summary>
        /// Gets or sets the connection string used for database operations. Leave blank to use local configuration database defined in "systemSettings".
        /// </summary>
        string DatabaseConnnectionString { get; set; }

        /// <summary>
        /// Gets or sets the provider string used for database operations. Defaults to a SQL Server provider string.
        /// </summary>
        string DatabaseProviderString { get; set; }

        /// <summary>
        /// Gets or sets template for output measurement point tag names.
        /// </summary>
        string PointTagTemplate { get; set; }

        /// <summary>
        /// Gets or sets template for local signal reference measurement name for source historian point.
        /// </summary>
        string SignalReferenceTemplate { get; set; }

        /// <summary>
        /// Gets or sets signal type for output measurements.
        /// </summary>
        SignalType SignalType { get; set; }

        /// <summary>
        /// Gets or sets the target historian acronym for output measurements.
        /// </summary>
        string TargetHistorianAcronym { get; set; }

        /// <summary>
        /// Gets routing tables used by<see cref="IIndependentAdapterManager"/> instance.
        /// </summary>
        RoutingTables RoutingTables { get; set; }

        /// <summary>
        /// Gets original data member setting for <see cref="IIndependentAdapterManager"/> instance.
        /// </summary>
        string OriginalDataMember { get; set; }

        /// <summary>
        /// Gets or sets flag that determines if the <see cref="IndependentActionAdapterManagerBase{TAdapter}"/> instance
        /// <see cref="AdapterCollectionBase{T}.ConnectionString"/> should be automatically parsed every time the
        /// <see cref="AdapterCollectionBase{T}.DataSource"/> is updated without requiring adapter to be reinitialized.
        /// </summary>
        bool AutoReparseConnectionString { get; set; }

        /// <summary>
        /// Gets input measurement <see cref="SignalType"/>'s for each of the <see cref="AdapterBase.InputMeasurementKeys"/>, if any.
        /// </summary>
        SignalType[] InputMeasurementKeyTypes { get; }
        
        /// <summary>
        /// Gets output measurement <see cref="SignalType"/>'s for each of the <see cref="AdapterBase.OutputMeasurements"/>, if any.
        /// </summary>
        SignalType[] OutputMeasurementTypes { get; }

        /// <summary>
        /// Gets or sets wait handle used by <see cref="IndependentActionAdapterManagerBase{TAdapter}"/> instance to
        /// manage waiting for changes to be loaded in system configuration.
        /// </summary>
        ManualResetEventSlim ConfigurationReloadedWaitHandle { get; set; }

        /// <summary>
        /// Gets number of input measurement required by each adapter.
        /// </summary>
        int InputsPerAdapter { get; }

        /// <summary>
        /// Gets or sets the index into the per adapter input measurements to use for target adapter name.
        /// </summary>
        int InputMeasurementIndexUsedForName { get; set; }

        /// <summary>
        /// Gets output measurement names.
        /// </summary>
        ReadOnlyCollection<string> OutputNames { get; }

        /// <summary>
        /// Gets or sets current adapter ID counter.
        /// </summary>
        uint AdapterIDCounter { get; set; }

        /// <summary>
        /// Parses connection string. Derived classes should override for custom connection string parsing.
        /// </summary>
        void ParseConnectionString();
        
        /// <summary>
        /// Notifies derived classes that configuration has been reloaded.
        /// </summary>
        void ConfigurationReloaded();

        /// <summary>
        /// Recalculates routing tables.
        /// </summary>
        void RecalculateRoutingTables();

        /// <summary>
        /// Gets configured database connection.
        /// </summary>
        /// <returns>New ADO data connection based on configured settings.</returns>
        AdoDataConnection GetConfiguredConnection();

        /// <summary>
        /// Enumerates child adapters
        /// </summary>
        void EnumerateAdapters();

        /// <summary>
        /// Gets subscriber information for specified client connection.
        /// </summary>
        /// <param name="adapterIndex">Enumerated index for child adapter.</param>
        /// <returns>Status for adapter with specified <paramref name="adapterIndex"/>.</returns>
        string GetAdapterStatus(int adapterIndex);

        /// <summary>
        /// Raises <see cref="AdapterCollectionBase{T}.ConfigurationChanged"/> event.
        /// </summary>
        void OnConfigurationChanged();

        /// <summary>
        /// Raises <see cref="AdapterCollectionBase{T}.InputMeasurementKeysUpdated"/> event.
        /// </summary>
        void OnInputMeasurementKeysUpdated();

        /// <summary>
        /// Raises the <see cref="AdapterCollectionBase{T}.StatusMessage"/> event and sends this data to the <see cref="Logger"/>.
        /// </summary>
        /// <param name="level">The <see cref="MessageLevel"/> to assign to this message</param>
        /// <param name="status">New status message.</param>
        /// <param name="eventName">A fixed string to classify this event; defaults to <c>null</c>.</param>
        /// <param name="flags"><see cref="MessageFlags"/> to use, if any; defaults to <see cref="MessageFlags.None"/>.</param>
        /// <remarks>
        /// <see pref="eventName"/> should be a constant string value associated with what type of message is being
        /// generated. In general, there should only be a few dozen distinct event names per class. Exceeding this
        /// threshold will cause the EventName to be replaced with a general warning that a usage issue has occurred.
        /// </remarks>
        void OnStatusMessage(MessageLevel level, string status, string eventName = null, MessageFlags flags = MessageFlags.None);

        /// <summary>
        /// Raises the <see cref="AdapterCollectionBase{T}.ProcessException"/> event.
        /// </summary>
        /// <param name="level">The <see cref="MessageLevel"/> to assign to this message</param>
        /// <param name="exception">Processing <see cref="Exception"/>.</param>
        /// <param name="eventName">A fixed string to classify this event; defaults to <c>null</c>.</param>
        /// <param name="flags"><see cref="MessageFlags"/> to use, if any; defaults to <see cref="MessageFlags.None"/>.</param>
        /// <remarks>
        /// <see pref="eventName"/> should be a constant string value associated with what type of message is being
        /// generated. In general, there should only be a few dozen distinct event names per class. Exceeding this
        /// threshold will cause the EventName to be replaced with a general warning that a usage issue has occurred.
        /// </remarks>
        void OnProcessException(MessageLevel level, Exception exception, string eventName = null, MessageFlags flags = MessageFlags.None);
    }
}