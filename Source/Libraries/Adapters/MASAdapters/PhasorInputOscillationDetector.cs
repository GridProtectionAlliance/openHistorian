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

        // Nested Types
        
        /// <summary>
        /// Defines possible calculation types for the <see cref="PhasorInputOscillationDetector"/>.
        /// </summary>
        public enum CalculationType
        {
            /// <summary>
            /// Megawatts.
            /// </summary>
            Megawatts,
            /// <summary>
            /// Megavars.
            /// </summary>
            Megavars
        }

        /// <summary>
        /// Defines the default value for the <see cref="AdjustmentStrategy"/>.
        /// </summary>
        public const string DefaultAdjustmentStrategy = "LineToNeutral";

        /// <summary>
        /// Defaults the default value for the <see cref="TargetCalculationType" />
        /// </summary>
        public const string DefaultCalculationType = "Megawatts";

        // Fields
        private readonly OscillationDetector m_detector;
        private MeasurementKey m_voltageMagnitude;
        private MeasurementKey m_voltageAngle;
        private MeasurementKey m_currentMagnitude;
        private MeasurementKey m_currentAngle;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="SingleInputOscillationDetector"/>.
        /// </summary>
        public PhasorInputOscillationDetector()
        {
            m_detector = new OscillationDetector(
                (level, message) => OnStatusMessage(level, message),
                (level, ex) => OnProcessException(level, ex),
                OnNewMeasurements);
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
        /// Returns the detailed status of the <see cref="PhasorInputOscillationDetector"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.Append(m_detector.Status);
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
            PseudoConfiguration configuration = new PseudoConfiguration
            {
                FramesPerSecond = FramesPerSecond,
                IsLineToNeutral = AdjustmentStrategy == VoltageAdjustmentStrategy.LineToNeutral
            };

            m_detector.DetectorAPI = new PseudoDetectorAPI
            {
                Configuration = configuration
            };

            m_detector.OutputMeasurements = OutputMeasurements;
            m_detector.FramesPerSecond = FramesPerSecond;
            m_detector.InputTypes = InputMeasurementKeyTypes;
            m_detector.Initialize(Name);
        }

        /// <summary>
        /// Processes 1-second data window.
        /// </summary>
        /// <param name="timestamp">Top of second window timestamp.</param>
        /// <param name="dataWindow">1-second data window.</param>
        protected override void ProcessDataWindow(Ticks timestamp, IMeasurement[,] dataWindow)
        {
            // Process detection algorithm against single input window
            m_detector.ProcessDataWindow(timestamp, dataWindow);
        }

        #endregion
    }
}