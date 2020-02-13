//******************************************************************************************************
//  PhasorInputOscillationDetector.cs - Gbtc
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
//  02/11/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GSF;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.Units;
using GSF.Units.EE;
using PowerCalculations;
using static MAS.OscillationDetector;

namespace MAS
{
    /// <summary>
    /// Represents an adapter that detects an oscillation based on a single set of voltage and current phasor data.
    /// </summary>
    [Description("MAS Oscillation Detector [Phasor Input]: Detects oscillations based on a single set of voltage and current phasor data inputs.")]
    public class PhasorInputOscillationDetector : OneSecondDataWindowAdapterBase
    {
        #region [ Members ]

        // Constants
        private const double SqrtOf3 = 1.7320508075688772935274463415059D;
        private const int VoltageMagnitude = 0;
        private const int VoltageAngle = 1;
        private const int CurrentMagnitude = 2;
        private const int CurrentAngle = 3;

        /// <summary>
        /// Defines the default value for the <see cref="AdjustmentStrategy"/>.
        /// </summary>
        public const string DefaultAdjustmentStrategy = "LineToNeutral";

        // Fields
        private readonly OscillationDetector m_detector = new OscillationDetector();
        private MeasurementKey m_voltageMagnitude;
        private MeasurementKey m_voltageAngle;
        private MeasurementKey m_currentMagnitude;
        private MeasurementKey m_currentAngle;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the triggering threshold for band 1 oscillation energy.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the triggering threshold for band 1 oscillation energy.")]
        [DefaultValue(DefaultBand1TriggerThreshold)]
        public double Band1TriggerThreshold { get; set; } = DefaultBand1TriggerThreshold;

        /// <summary>
        /// Gets or sets the triggering threshold for band 2 oscillation energy.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the triggering threshold for band 2 oscillation energy.")]
        [DefaultValue(DefaultBand2TriggerThreshold)]
        public double Band2TriggerThreshold { get; set; } = DefaultBand2TriggerThreshold;

        /// <summary>
        /// Gets or sets the triggering threshold for band 3 oscillation energy.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the triggering threshold for band 3 oscillation energy.")]
        [DefaultValue(DefaultBand3TriggerThreshold)]
        public double Band3TriggerThreshold { get; set; } = DefaultBand3TriggerThreshold;

        /// <summary>
        /// Gets or sets the triggering threshold for band 4 oscillation energy.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the triggering threshold for band 4 oscillation energy.")]
        [DefaultValue(DefaultBand4TriggerThreshold)]
        public double Band4TriggerThreshold { get; set; } = DefaultBand4TriggerThreshold;

        /// <summary>
        /// Gets or sets the default strategy used to adjust voltage values for based on the nature of the voltage measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines default strategy used to adjust voltage values for based on the nature of the voltage measurements.")]
        [DefaultValue(typeof(VoltageAdjustmentStrategy), DefaultAdjustmentStrategy)]
        public VoltageAdjustmentStrategy AdjustmentStrategy { get; set; } = (VoltageAdjustmentStrategy)Enum.Parse(typeof(VoltageAdjustmentStrategy), DefaultAdjustmentStrategy);

        /// <summary>
        /// Returns the detailed status of the <see cref="PhasorInputOscillationDetector"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.Append(m_detector.Status);
                status.Append(base.Status);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="PhasorInputOscillationDetector" />.
        /// </summary>
        public override void Initialize()
        {
            InputCount = 4;
            OutputCount = Outputs.Length;

            base.Initialize();

            MeasurementKey getMeasurementKey(SignalType signalType) => InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes[index] == signalType).FirstOrDefault();

            // Validate input contains voltage and current phase angle / magnitude measurement keys
            m_voltageMagnitude = getMeasurementKey(SignalType.VPHM) ?? throw new InvalidOperationException("One voltage magnitude input measurement is required. Cannot initialize adapter.");
            m_voltageAngle = getMeasurementKey(SignalType.VPHA) ?? throw new InvalidOperationException("One voltage angle input measurement is required. Cannot initialize adapter.");
            m_currentMagnitude = getMeasurementKey(SignalType.IPHM) ?? throw new InvalidOperationException("One current magnitude input measurement is required. Cannot initialize adapter.");
            m_currentAngle = getMeasurementKey(SignalType.IPHA) ?? throw new InvalidOperationException("One current angle input measurement is required. Cannot initialize adapter.");

            // Make sure input measurement keys are in desired order
            InputMeasurementKeys = new[] { m_voltageMagnitude, m_voltageAngle, m_currentMagnitude, m_currentAngle };

            // Provide algorithm with parameters as configured by adapter
            m_detector.OutputMeasurements = OutputMeasurements;
            m_detector.FramesPerSecond = FramesPerSecond;
            m_detector.Band1TriggerThreshold = Band1TriggerThreshold;
            m_detector.Band2TriggerThreshold = Band2TriggerThreshold;
            m_detector.Band3TriggerThreshold = Band3TriggerThreshold;
            m_detector.Band4TriggerThreshold = Band4TriggerThreshold;
        }

        /// <summary>
        /// Processes 1-second data window.
        /// </summary>
        /// <param name="timestamp">Top of second window timestamp.</param>
        /// <param name="dataWindow">1-second data window.</param>
        protected override void ProcessDataWindow(Ticks timestamp, IMeasurement[,] dataWindow)
        {
            IMeasurement[] results = new IMeasurement[FramesPerSecond];

            void addResult(double value, int index, MeasurementStateFlags flags)
            {
                results[index] = new Measurement
                {
                    Metadata = MeasurementMetadata.Undefined,
                    Value = value,
                    Timestamp = timestamp + SubsecondOffsets[index],
                    StateFlags = flags
                };
            }

            for (int i = 0; i < FramesPerSecond; i++)
            {
                IMeasurement voltageMagnitudeMeasurement = dataWindow[VoltageMagnitude, i];
                IMeasurement voltageAngleMeasurement = dataWindow[VoltageAngle, i];
                IMeasurement currentMagnitudeMeasurement = dataWindow[CurrentMagnitude, i];
                IMeasurement currentAngleMeasurement = dataWindow[CurrentAngle, i];

                if (voltageMagnitudeMeasurement == null || voltageAngleMeasurement == null || currentMagnitudeMeasurement == null || currentAngleMeasurement == null)
                {
                    addResult(double.NaN, i, MeasurementStateFlags.BadData);
                    continue;
                }

                Voltage voltageMagnitude = voltageMagnitudeMeasurement.AdjustedValue;
                Angle voltageAngle = Angle.FromDegrees(voltageAngleMeasurement.AdjustedValue);
                Current currentMagnitude = currentMagnitudeMeasurement.AdjustedValue;
                Angle currentAngle = Angle.FromDegrees(currentAngleMeasurement.AdjustedValue);

                switch (AdjustmentStrategy)
                {
                    case VoltageAdjustmentStrategy.LineToNeutral:
                        voltageMagnitude *= 3;
                        break;

                    case VoltageAdjustmentStrategy.LineToLine:
                        voltageMagnitude *= SqrtOf3;
                        break;
                }

                Phasor voltage = new Phasor(PhasorType.Voltage, new ComplexNumber(voltageAngle, voltageMagnitude));
                Phasor current = new Phasor(PhasorType.Current, new ComplexNumber(currentAngle, currentMagnitude));
                Power activePower = Phasor.CalculateActivePower(voltage, current);

                addResult(activePower, i, Common.DerivedQualityFlags(new[] { voltageMagnitudeMeasurement, voltageAngleMeasurement, currentMagnitudeMeasurement, currentAngleMeasurement }));
            }
            
            // Process detection algorithm against calculated power results
            Measurement[] measurements = m_detector.ProcessDataWindow(timestamp, results);

            // Publish new result measurements
            OnNewMeasurements(measurements);
        }

        #endregion
    }
}
