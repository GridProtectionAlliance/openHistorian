//******************************************************************************************************
//  BulkSingleInputOscillationDetector.cs - Gbtc
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
using System.Text;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.TimeSeries.Data;
using GSF.TimeSeries.Model;
using PowerCalculations;
using static MAS.SingleInputOscillationDetector;
using static MAS.OscillationDetector;

namespace MAS
{
    /// <summary>
    /// Represents an adapter that manages bulk detection of oscillations based on individual inputs.
    /// </summary>
    [Description("MAS Oscillation Detector [Bulk Single Input]: Manages bulk detection of oscillations based on individual inputs.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public class BulkSingleInputOscillationDetector : IndependentActionAdapterManagerBase<SingleInputOscillationDetector>
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Defines the default value for the <see cref="IndependentActionAdapterManagerBase{TAdapter}.InputMeasurementKeys"/>
        /// when <see cref="TargetCalculationType"/> is <see cref="CalculationType.VoltageMagnitude"/>.
        /// </summary>
        public const string DefaultVoltageMagnitudeInputMeasurementKeys = "FILTER ActiveMeasurements WHERE SignalType = 'VPHM' AND Phase='+'";

        /// <summary>
        /// Defines the default value for the <see cref="IndependentActionAdapterManagerBase{TAdapter}.InputMeasurementKeys"/>
        /// when <see cref="TargetCalculationType"/> is <see cref="CalculationType.Frequency"/>.
        /// </summary>
        public const string DefaultFrequencyInputMeasurementKeys = "FILTER ActiveMeasurements WHERE SignalType = 'VPHA' AND Phase='+'";

        /// <summary>
        /// Defines the default value for the <see cref="InputMeasurementIndexUsedForName"/>.
        /// </summary>
        public const int DefaultInputMeasurementIndexUsedForName = 0;

        // Fields
        private MeasurementKey[] m_operatingMeasurementKeys;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates new <see cref="BulkSingleInputOscillationDetector"/>.
        /// </summary>
        public BulkSingleInputOscillationDetector()
        {
            base.InputMeasurementIndexUsedForName = DefaultInputMeasurementIndexUsedForName;
            base.PointTagTemplate = DefaultPointTagTemplate;
            base.SignalReferenceTemplate = DefaultSignalReferenceTemplate;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the default strategy used to adjust voltage values for based on the nature of the voltage measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines default strategy used to adjust voltage values for based on the nature of the voltage measurements.")]
        [DefaultValue(typeof(VoltageAdjustmentStrategy), DefaultAdjustmentStrategy)]
        public VoltageAdjustmentStrategy AdjustmentStrategy { get; set; } = (VoltageAdjustmentStrategy)Enum.Parse(typeof(VoltageAdjustmentStrategy), DefaultAdjustmentStrategy);

        /// <summary>
        /// Gets or sets the target calculation type for the oscillation detector.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the target calculation type for the oscillation detector.")]
        [DefaultValue(typeof(CalculationType), DefaultCalculationType)]
        public CalculationType TargetCalculationType { get; set; } = (CalculationType)Enum.Parse(typeof(CalculationType), DefaultCalculationType);

        /// <summary>
        /// Gets or sets the index into the per adapter input measurements to use for target adapter name.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the index into the per adapter input measurements to use for target adapter name.")]
        [DefaultValue(DefaultInputMeasurementIndexUsedForName)]
        public override int InputMeasurementIndexUsedForName // Overriding to provide implementation specific default value
        {
            get => base.InputMeasurementIndexUsedForName;
            set => base.InputMeasurementIndexUsedForName = value;
        }

        /// <summary>
        /// Gets or sets template for output measurement point tag names.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement point tag names, typically an expression like \"" + DefaultPointTagTemplate + "\".")]
        [DefaultValue(DefaultPointTagTemplate)]
        public override string PointTagTemplate // Overriding to provide implementation specific default value
        { 
            get => base.PointTagTemplate; 
            set => base.PointTagTemplate = value;
        }

        /// <summary>
        /// Gets or sets template for local signal reference measurement name for source historian point.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement signal reference names, typically an expression like \"" + DefaultSignalReferenceTemplate + "\".")]
        [DefaultValue(DefaultSignalReferenceTemplate)]
        public override string SignalReferenceTemplate // Overriding to provide implementation specific default value
        { 
            get => base.SignalReferenceTemplate;
            set => base.SignalReferenceTemplate = value;
        }

        /// <summary>
        /// Gets or sets template for the parent device acronym used to group associated output measurements.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)] // Hiding parameter from manager - all outputs automatically associated with input device
        public override string ParentDeviceAcronymTemplate
        {
            get => null;
            set => base.ParentDeviceAcronymTemplate = value;
        }

        /// <summary>
        /// Gets associated device ID for <see cref="IndependentActionAdapterManagerBase{T}.CurrentAdapterIndex" />, if any, for measurement generation.
        /// If overridden to provide custom device ID, <see cref="IndependentActionAdapterManagerBase{T}.ParentDeviceAcronymTemplate" /> should be set to
        /// <c>null</c> so no parent device is created.
        /// </summary>
        public override int CurrentDeviceID
        {
            get
            {
                if (CurrentAdapterIndex > -1)
                {
                    MeasurementKey[] operatingMeasurementKeys = m_operatingMeasurementKeys ?? InputMeasurementKeys;

                    // Just pick first input measurement to find associated device ID
                    MeasurementKey inputMeasurement = operatingMeasurementKeys[CurrentAdapterIndex * PerAdapterInputCount];
                    DataRow record = DataSource.LookupMetadata(inputMeasurement.SignalID, SourceMeasurementTable);

                    if (!(record is null))
                    {
                        string deviceName = record["Device"].ToString();

                        if (!string.IsNullOrWhiteSpace(deviceName))
                        {
                            // Query the actual database record ID based on the known runtime ID for this device
                            using (AdoDataConnection connection = GetConfiguredConnection())
                            {
                                TableOperations<Device> deviceTable = new TableOperations<Device>(connection);
                                Device device = deviceTable.QueryRecordWhere("Acronym = {0}", deviceName);
                                return device.ID;
                            }
                        }

                        throw new InvalidOperationException($"Device name was blank for signal ID \"{inputMeasurement.SignalID}\" for current for adapter {CurrentAdapterIndex:N0}");
                    }

                    throw new InvalidOperationException($"Failed to find signal ID \"{inputMeasurement.SignalID}\" for current for adapter {CurrentAdapterIndex:N0}");
                }

                throw new IndexOutOfRangeException($"Current adapter index {CurrentAdapterIndex:N0} is invalid");
            }
        }

        /// <summary>
        /// Gets number of input measurement required by each adapter.
        /// </summary>
        public override int PerAdapterInputCount => 1;

        /// <summary>
        /// Gets output measurement names.
        /// </summary>
        public override ReadOnlyCollection<string> PerAdapterOutputNames => Array.AsReadOnly(Outputs.Select(output => $"{output}").ToArray());

        /// <summary>
        /// Gets or sets output measurements that the <see cref="AdapterBase"/> will produce, if any.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)] // Hiding parameter from manager - outputs managed automatically
        public override IMeasurement[] OutputMeasurements
        {
            get => base.OutputMeasurements;
            set => base.OutputMeasurements = value;
        }

        /// <summary>
        /// Returns the detailed status of the <see cref="BulkSingleInputOscillationDetector"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.AppendFormat("        Voltage Adjustment: {0}", AdjustmentStrategy);
                status.AppendLine();
                status.AppendFormat("   Target Calculation Type: {0}", TargetCalculationType);
                status.AppendLine();
                status.Append(base.Status);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Parses connection string.
        /// </summary>
        public override void ParseConnectionString()
        {
            Dictionary<string, string> settings = Settings;

            if (!settings.TryGetValue(nameof(InputMeasurementKeys), out string inputMeasurementKeys) || string.IsNullOrWhiteSpace(inputMeasurementKeys))
                settings[nameof(InputMeasurementKeys)] = TargetCalculationType == CalculationType.Frequency ?
                    DefaultFrequencyInputMeasurementKeys : DefaultVoltageMagnitudeInputMeasurementKeys;

            base.ParseConnectionString();

            m_operatingMeasurementKeys = InputMeasurementKeys;
            InitializeChildAdapterManagement(m_operatingMeasurementKeys);
        }

        #endregion
    }
}
