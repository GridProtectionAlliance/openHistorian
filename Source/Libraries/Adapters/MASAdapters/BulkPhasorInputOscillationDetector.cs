//******************************************************************************************************
//  BulkPhasorInputOscillationDetector.cs - Gbtc
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
using System.Linq;
using System.Text;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using PowerCalculations;
using MeasurementRecord = GSF.TimeSeries.Model.Measurement;
using PhasorRecord = GSF.TimeSeries.Model.Phasor;
using SignalTypeRecord = GSF.TimeSeries.Model.SignalType;
using SignalType = GSF.Units.EE.SignalType;
using static MAS.PhasorInputOscillationDetector;
using static MAS.OscillationDetector;

namespace MAS
{
    /// <summary>
    /// Represents an adapter that manages bulk detection of oscillations based on individual inputs.
    /// </summary>
    [Description("MAS Oscillation Detector [Bulk Phasor Input]: Manages bulk detection of oscillations based on voltage and current phasor data inputs.")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public class BulkPhasorInputOscillationDetector : IndependentActionAdapterManagerBase<PhasorInputOscillationDetector>
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Defines the default value for the <see cref="IndependentActionAdapterManagerBase{TAdapter}.InputMeasurementKeys"/>.
        /// </summary>
        public const string DefaultInputMeasurementKeys = "FILTER ActiveMeasurements WHERE SignalType LIKE '%PH%' AND Phase='+' ORDER BY PhasorID";

        /// <summary>
        /// Defines the default value for the <see cref="InputMeasurementIndexUsedForName"/>.
        /// </summary>
        public const int DefaultInputMeasurementIndexUsedForName = 2;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates new <see cref="BulkPhasorInputOscillationDetector"/>.
        /// </summary>
        public BulkPhasorInputOscillationDetector()
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
        /// Gets number of input measurement required by each adapter.
        /// </summary>
        public override int PerAdapterInputCount => 4;

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
        /// Returns the detailed status of the <see cref="BulkPhasorInputOscillationDetector"/>.
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
                settings[nameof(InputMeasurementKeys)] = DefaultInputMeasurementKeys;

            base.ParseConnectionString();

            List<MeasurementKey> voltageMagnitudes = new List<MeasurementKey>();
            List<MeasurementKey> voltageAngles = new List<MeasurementKey>();
            List<MeasurementKey> currentMagnitudes = new List<MeasurementKey>();
            List<MeasurementKey> currentAngles = new List<MeasurementKey>();

            for (int i = 0; i < InputMeasurementKeys.Length; i++)
            {
                MeasurementKey key = InputMeasurementKeys[i];
                SignalType signalType = InputMeasurementKeyTypes[i];

                switch (signalType)
                {
                    case SignalType.VPHM:
                        voltageMagnitudes.Add(key);
                        break;
                    case SignalType.VPHA:
                        voltageAngles.Add(key);
                        break;
                    case SignalType.IPHM:
                        currentMagnitudes.Add(key);
                        break;
                    case SignalType.IPHA:
                        currentAngles.Add(key);
                        break;
                    default:
                        OnStatusMessage(MessageLevel.Warning, $"Unexpected input type \"{signalType}\" encountered in input for {nameof(BulkPhasorInputOscillationDetector)}. Expected one of \"{SignalType.VPHM}\", \"{SignalType.VPHA}\", \"{SignalType.IPHM}\", or \"{SignalType.IPHA}\".");
                        break;
                }
            }

            if (currentMagnitudes.Count != currentAngles.Count)
                throw new InvalidOperationException("Uneven number of current magnitude and angle inputs provided. Cannot initialize adapter.");

            // Build proper set of inputs where voltages and associated currents are grouped together
            List<MeasurementKey> inputs = new List<MeasurementKey>();
            int unassociatedCount = 0;

            using (AdoDataConnection connection = GetConfiguredConnection())
            {
                TableOperations<MeasurementRecord> measurementTable = new TableOperations<MeasurementRecord>(connection);
                TableOperations<PhasorRecord> phasorTable = new TableOperations<PhasorRecord>(connection);
                TableOperations<SignalTypeRecord> signalTypeTable = new TableOperations<SignalTypeRecord>(connection);

                // Lookup signal type ID for voltage magnitude
                int voltageMagnitudeSignalTypeID = signalTypeTable.QueryRecordWhere("Acronym = {0}", $"{SignalType.VPHM}")?.ID ?? 3;

                for (int i = 0; i < currentMagnitudes.Count; i++)
                {
                    MeasurementKey currentMagnitude = currentMagnitudes[i];
                    MeasurementKey currentAngle = currentAngles[i];

                    if (currentMagnitude == null || currentAngle == null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    MeasurementRecord currentMeasurement = measurementTable.QueryRecordWhere("SignalID = {0}", currentMagnitude.SignalID);
                    PhasorRecord currentPhasor = phasorTable.QueryRecordWhere("DeviceID = {0} AND SourceIndex = {1}", currentMeasurement.DeviceID, currentMeasurement.PhasorSourceIndex);

                    // If no associated voltage is assigned, skip configuration
                    if (currentPhasor.DestinationPhasorID == null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    PhasorRecord voltagePhasor = phasorTable.QueryRecordWhere("ID = {0}", currentPhasor.DestinationPhasorID);
                    MeasurementRecord voltageMeasurement = measurementTable.QueryRecordWhere("DeviceID = {0} AND PhasorSourceIndex = {1} AND SignalTypeID = {2}", voltagePhasor.DeviceID, voltagePhasor.SourceIndex, voltageMagnitudeSignalTypeID);
                    MeasurementKey voltageMagnitude = null;
                    MeasurementKey voltageAngle = null;

                    for (int j = 0; j < voltageMagnitudes.Count; j++)
                    {
                        if (voltageMagnitudes[j].SignalID == voltageMeasurement.SignalID)
                        {
                            voltageMagnitude = voltageMagnitudes[j];
                            voltageAngle = voltageAngles[j];
                            break;
                        }
                    }

                    if (voltageMagnitude == null || voltageAngle == null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    // Add associated voltage and current input measurement keys in desired order
                    inputs.Add(voltageMagnitude);
                    inputs.Add(voltageAngle);
                    inputs.Add(currentMagnitude);
                    inputs.Add(currentAngle);
                }
            }

            if (unassociatedCount > 0)
                OnStatusMessage(MessageLevel.Warning, $"{unassociatedCount:N0} of the specified input currents had no associated voltages and were excluded as inputs.");

            if (inputs.Count % PerAdapterInputCount != 0)
                OnStatusMessage(MessageLevel.Warning, $"Unexpected number of input {inputs.Count:N0} for {PerAdapterInputCount:N0} inputs per adapter.");

            // Define properly ordered and associated set of inputs
            InputMeasurementKeys = inputs.ToArray();
            
            InitializeChildAdapterManagement();
        }

        #endregion
    }
}
