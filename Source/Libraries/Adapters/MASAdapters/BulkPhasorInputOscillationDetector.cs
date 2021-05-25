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

        // Fields
        private MeasurementKey[] m_operatingMeasurementKeys;

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
                try
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

                            OnProcessException(MessageLevel.Error, new InvalidOperationException($"Device name was blank for signal ID \"{inputMeasurement.SignalID}\" for current for adapter {CurrentAdapterIndex:N0}"));
                        }

                        OnProcessException(MessageLevel.Error, new InvalidOperationException($"Failed to find signal ID \"{inputMeasurement.SignalID}\" for current for adapter {CurrentAdapterIndex:N0}"));
                    }

                    OnProcessException(MessageLevel.Error, new IndexOutOfRangeException($"Current adapter index {CurrentAdapterIndex:N0} is invalid"));
                }
                catch (Exception ex)
                {
                    OnProcessException(MessageLevel.Error, new InvalidOperationException($"Failed to lookup current device ID for adapter {CurrentAdapterIndex:N0}: {ex.Message}", ex));
                }

                return base.CurrentDeviceID;
            }
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

            if (!settings.TryGetValue(nameof(InputMeasurementKeys), out string setting) || string.IsNullOrWhiteSpace(setting))
                settings[nameof(InputMeasurementKeys)] = DefaultInputMeasurementKeys;

            base.ParseConnectionString();

            // Get a local copy of the input keys as these will change often during initialization
            MeasurementKey[] inputMeasurementKeys = InputMeasurementKeys;
            SignalType[] inputMeasurementKeyTypes = InputMeasurementKeyTypes;

            if (inputMeasurementKeys.Length == 0)
            {
                OnStatusMessage(MessageLevel.Error, "No inputs were configured. Cannot initialize adapter.");
                return;
            }

            if (inputMeasurementKeys.Length != inputMeasurementKeyTypes.Length)
            {
                OnStatusMessage(MessageLevel.Error, "Parallel input measurement keys and type array lengths do not match. Cannot initialize adapter.");
                return;
            }

            List<MeasurementKey> voltageMagnitudeKeys = new List<MeasurementKey>();
            List<MeasurementKey> voltageAngleKeys = new List<MeasurementKey>();
            List<MeasurementKey> currentMagnitudeKeys = new List<MeasurementKey>();
            List<MeasurementKey> currentAngleKeys = new List<MeasurementKey>();

            for (int i = 0; i < inputMeasurementKeys.Length; i++)
            {
                MeasurementKey key = inputMeasurementKeys[i];
                SignalType signalType = inputMeasurementKeyTypes[i];

                switch (signalType)
                {
                    case SignalType.VPHM:
                        voltageMagnitudeKeys.Add(key);
                        break;
                    case SignalType.VPHA:
                        voltageAngleKeys.Add(key);
                        break;
                    case SignalType.IPHM:
                        currentMagnitudeKeys.Add(key);
                        break;
                    case SignalType.IPHA:
                        currentAngleKeys.Add(key);
                        break;
                    default:
                        OnStatusMessage(MessageLevel.Warning, $"Unexpected input type \"{signalType}\" encountered in input for {nameof(BulkPhasorInputOscillationDetector)}. Expected one of \"{SignalType.VPHM}\", \"{SignalType.VPHA}\", \"{SignalType.IPHM}\", or \"{SignalType.IPHA}\".");
                        break;
                }
            }

            if (voltageMagnitudeKeys.Count != voltageAngleKeys.Count)
                OnStatusMessage(MessageLevel.Warning, $"Uneven number of voltage magnitude ({voltageMagnitudeKeys.Count:N0}) and voltage angle ({voltageAngleKeys.Count:N0}) inputs provided, verify input configuration.");

            if (currentMagnitudeKeys.Count != currentAngleKeys.Count)
                OnStatusMessage(MessageLevel.Warning, $"Uneven number of current magnitude ({currentMagnitudeKeys.Count:N0}) and current angle ({currentAngleKeys.Count:N0}) inputs provided, verify input configuration.");

            // Build proper set of inputs where voltages and associated currents are grouped together
            List<MeasurementKey> inputs = new List<MeasurementKey>();
            int unassociatedCount = 0;

            using (AdoDataConnection connection = GetConfiguredConnection())
            {
                TableOperations<MeasurementRecord> measurementTable = new TableOperations<MeasurementRecord>(connection);
                TableOperations<PhasorRecord> phasorTable = new TableOperations<PhasorRecord>(connection);
                TableOperations<SignalTypeRecord> signalTypeTable = new TableOperations<SignalTypeRecord>(connection);

                // Lookup needed signal type IDs
                int currentAngleSignalTypeID = signalTypeTable.QueryRecordWhere("Acronym = {0}", $"{SignalType.IPHA}")?.ID ?? 2;
                int voltageMagnitudeSignalTypeID = signalTypeTable.QueryRecordWhere("Acronym = {0}", $"{SignalType.VPHM}")?.ID ?? 3;
                int voltageAngleSignalTypeID = signalTypeTable.QueryRecordWhere("Acronym = {0}", $"{SignalType.VPHA}")?.ID ?? 4;

                foreach (MeasurementKey currentMagnitude in currentMagnitudeKeys)
                {
                    if (currentMagnitude is null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    // Lookup current magnitude measurement
                    MeasurementRecord currentMagnitudeMeasurement = measurementTable.QueryRecordWhere("SignalID = {0}", currentMagnitude.SignalID);

                    // If current magnitude measurement is not found, skip configuration
                    if (currentMagnitudeMeasurement is null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    // Lookup current phasor (as associated with current magnitude)
                    PhasorRecord currentPhasor = phasorTable.QueryRecordWhere("DeviceID = {0} AND SourceIndex = {1}", currentMagnitudeMeasurement.DeviceID, currentMagnitudeMeasurement.PhasorSourceIndex);

                    // If no associated voltage phasor is assigned, skip configuration
                    if (currentPhasor?.DestinationPhasorID is null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    // Lookup associated current angle measurement
                    MeasurementRecord currentAngleMeasurement = measurementTable.QueryRecordWhere("DeviceID = {0} AND PhasorSourceIndex = {1} AND SignalTypeID = {2}", currentPhasor.DeviceID, currentPhasor.SourceIndex, currentAngleSignalTypeID);

                    // If current angle measurement is not found, skip configuration
                    if (currentAngleMeasurement is null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    // Lookup associated current angle in the inputs to this adapter
                    MeasurementKey currentAngle = currentAngleKeys.FirstOrDefault(angleKey => angleKey.SignalID == currentAngleMeasurement.SignalID);

                    // If current angle is not found as an input, skip configuration
                    if (currentAngle is null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    // Lookup destination voltage phasor (as mapped to the current phasor)
                    PhasorRecord voltagePhasor = phasorTable.QueryRecordWhere("ID = {0}", currentPhasor.DestinationPhasorID);

                    // If associated voltage phasor is not found, skip configuration
                    if (voltagePhasor is null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    // Lookup associated voltage magnitude measurement
                    MeasurementRecord voltageMagnitudeMeasurement = measurementTable.QueryRecordWhere("DeviceID = {0} AND PhasorSourceIndex = {1} AND SignalTypeID = {2}", voltagePhasor.DeviceID, voltagePhasor.SourceIndex, voltageMagnitudeSignalTypeID);

                    // If voltage magnitude measurement is not found, skip configuration
                    if (voltageMagnitudeMeasurement is null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    // Lookup associated voltage magnitude in the inputs to this adapter
                    MeasurementKey voltageMagnitude = voltageMagnitudeKeys.FirstOrDefault(magnitudeKey => magnitudeKey.SignalID == voltageMagnitudeMeasurement.SignalID);

                    // If voltage magnitude is not found as an input, skip configuration
                    if (voltageMagnitude is null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    // Lookup associated voltage angle measurement
                    MeasurementRecord voltageAngleMeasurement = measurementTable.QueryRecordWhere("DeviceID = {0} AND PhasorSourceIndex = {1} AND SignalTypeID = {2}", voltagePhasor.DeviceID, voltagePhasor.SourceIndex, voltageAngleSignalTypeID);

                    // If voltage angle measurement is not found, skip configuration
                    if (voltageAngleMeasurement is null)
                    {
                        unassociatedCount++;
                        continue;
                    }

                    // Lookup associated voltage angle in the inputs to this adapter
                    MeasurementKey voltageAngle = voltageAngleKeys.FirstOrDefault(angleKey => angleKey.SignalID == voltageAngleMeasurement.SignalID);

                    // If voltage angle is not found as an input, skip configuration
                    if (voltageAngle is null)
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
                OnStatusMessage(MessageLevel.Warning, $"{unassociatedCount:N0} of the specified input current phasors had no associated voltage phasors and were excluded as inputs.");

            if (inputs.Count % PerAdapterInputCount != 0)
                OnStatusMessage(MessageLevel.Warning, $"Unexpected number of inputs ({inputs.Count:N0}) for {PerAdapterInputCount:N0} inputs per adapter.");

            if (inputs.Count == 0)
            {
                OnStatusMessage(MessageLevel.Warning, "No valid inputs were defined. Cannot initialize adapter.");
                return;
            }

            // Define properly ordered and associated set of inputs
            m_operatingMeasurementKeys = inputs.ToArray();

            // Setup child adapters
            InitializeChildAdapterManagement(m_operatingMeasurementKeys);

            // Update external routing tables to only needed inputs
            InputMeasurementKeys = m_operatingMeasurementKeys;
        }

        #endregion
    }
}
