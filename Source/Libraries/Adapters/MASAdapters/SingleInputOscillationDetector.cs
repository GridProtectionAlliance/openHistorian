//******************************************************************************************************
//  SingleInputOscillationDetector.cs - Gbtc
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
using System.Text;
using GSF;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using PowerCalculations;
using static MAS.OscillationDetector;

namespace MAS
{
    /// <summary>
    /// Represents an adapter that detects an oscillation based on an individual input.
    /// </summary>
    [Description("MAS Oscillation Detector [Single Input]: Detects oscillations based on an individual input.")]
    public class SingleInputOscillationDetector : OneSecondDataWindowAdapterBase
    {
        #region [ Members ]

        // Nested Types

        /// <summary>
        /// Defines possible calculation types for the <see cref="SingleInputOscillationDetector"/>.
        /// </summary>
        public enum CalculationType
        {
            /// <summary>
            /// Megavars.
            /// </summary>
            Frequency,
            /// <summary>
            /// Voltage Magnitude.
            /// </summary>
            VoltageMagnitude
        }

        // Constants

        /// <summary>
        /// Defines the default value for the <see cref="AdjustmentStrategy"/>.
        /// </summary>
        public const string DefaultAdjustmentStrategy = "LineToNeutral";

        /// <summary>
        /// Defaults the default value for the <see cref="TargetCalculationType" />
        /// </summary>
        public const string DefaultCalculationType = "Frequency";

        // Fields
        private readonly OscillationDetector m_detector;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="SingleInputOscillationDetector"/>.
        /// </summary>
        public SingleInputOscillationDetector()
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
        /// Returns the detailed status of the <see cref="SingleInputOscillationDetector"/>.
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
        /// Initializes <see cref="SingleInputOscillationDetector" />.
        /// </summary>
        public override void Initialize()
        {
            InputCount = 1;
            OutputCount = Outputs.Length;

            base.Initialize();

            // Provide algorithm with parameters as configured by adapter
            PseudoConfiguration configuration = new PseudoConfiguration
            {
                FramesPerSecond = FramesPerSecond
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
