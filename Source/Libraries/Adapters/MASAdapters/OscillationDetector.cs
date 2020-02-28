//******************************************************************************************************
//  OscillationDetector.cs - Gbtc
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
//  02/12/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GSF;
using GSF.Collections;
using GSF.Diagnostics;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.Units.EE;

#pragma warning disable CA1819

namespace MAS
{
    /// <summary>
    /// Defines the processing algorithm for 1-second data windows.
    /// </summary>
    public class OscillationDetector
    {
        #region [ Members ]

        // Nested Types

        /// <summary>
        /// Defines the output measurements for an adapter that uses the <see cref="OscillationDetector"/>.
        /// </summary>
        /// <remarks>
        /// One output measurement should be defined for each enumeration value, in order:
        /// </remarks>
        public enum Output
        {
            /// <summary>
            /// Band 1 oscillation energy for input.
            /// </summary>
            /// <remarks>
            /// Band 1, with a pass-band of 0.01-Hz to 0.15 Hz, monitors very slow oscillations that typically involve speed-governor controllers.
            /// The response time is 200 sec. or less.
            /// </remarks>
            Band1Energy,

            /// <summary>
            /// Band 2 oscillation energy for input.
            /// </summary>
            /// <remarks>
            /// Band 2, with a pass-band of 0.15-Hz to 1.0-Hz, is tuned to oscillations typically observed in the electromechanical oscillation range.
            /// The response time is 12 sec. or less.
            /// </remarks>
            Band2Energy,

            /// <summary>
            /// Band 3 oscillation energy for input.
            /// </summary>
            /// <remarks>
            /// Band 3, with a pass-band of 1.0-Hz to 5.0-Hz, is typically associated with local electromechanical modes and generator controls.
            /// The response time is 6 sec. or less.
            /// </remarks>
            Band3Energy,

            /// <summary>
            /// Band 4 oscillation energy for input.
            /// </summary>
            /// <remarks>
            /// Band 4, with a pass-band of 5.0-Hz to Nyquist, may be associated with torsional dynamics of a generator, for example,
            /// or may be related to voltage or other relatively high-speed controllers.
            /// </remarks>
            Band4Energy
        }

        // Constants

        /// <summary>
        /// Defines the default value for the <see cref="IIndependentAdapterManager.PointTagTemplate"/>.
        /// </summary>
        public const string DefaultPointTagTemplate = "MAS!{0}";

        /// <summary>
        /// Defines the default value for the <see cref="IIndependentAdapterManager.SignalReferenceTemplate"/>.
        /// </summary>
        public const string DefaultSignalReferenceTemplate = DefaultPointTagTemplate + "-CV";

        // Fields
        private readonly Action<MessageLevel, string> m_statusMessage;
        private readonly Action<MessageLevel, Exception> m_processException;
        private readonly Action<ICollection<IMeasurement>> m_publishMeasurements;
        private bool m_timeQualityIsGood;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="OscillationDetector"/>.
        /// </summary>
        /// <param name="statusMessage">Status message callback.</param>
        /// <param name="processException">Process exception callback.</param>
        /// <param name="publishMeasurements">Publish measurements callback.</param>
        public OscillationDetector(
            Action<MessageLevel, string> statusMessage,
            Action<MessageLevel, Exception> processException,
            Action<ICollection<IMeasurement>> publishMeasurements)
        {
            m_statusMessage = statusMessage;
            m_processException = processException;
            m_publishMeasurements = publishMeasurements;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets detector API.
        /// </summary>
        public PseudoDetectorAPI DetectorAPI { get; set; } // TODO: Change "dynamic" to proper API type
        
        /// <summary>
        /// Gets or sets input types.
        /// </summary>
        public SignalType[] InputTypes { get; set; }

        /// <summary>
        /// Gets or sets output measurements that algorithm will produce.
        /// </summary>
        public IMeasurement[] OutputMeasurements { get; set; }

        /// <summary>
        /// Gets or sets the frames per second for the incoming data.
        /// </summary>
        public int FramesPerSecond { get; set; }

        /// <summary>
        /// Gets adapter name associated with the algorithm.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets oscillation detector configuration.
        /// </summary>
        public PseudoConfiguration Configuration { get; set; }

        /// <summary>
        /// Returns the detailed status of the <see cref="OscillationDetector"/>.
        /// </summary>
        public string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.AppendFormat("         Frames Per Second: {0:N0}", FramesPerSecond);
                status.AppendLine();

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="OscillationDetector"/>.
        /// </summary>
        /// <param name="name">Adapter name.</param>
        public void Initialize(string name)
        {
            Name = name;
            
            DetectorAPI.ReportCallback = PublishResults;

            m_statusMessage(MessageLevel.Info, $"Oscillation detector \"{Name}\" using \"{string.Join(", ", InputTypes)}\" inputs initialized.");
        }

        /// <summary>
        /// Processes 1-second data window consisting of measurements.
        /// </summary>
        /// <param name="timestamp">Top of second window timestamp.</param>
        /// <param name="dataWindow">1-second data window for input values.</param>
        /// <returns>New result measurements ready for publication.</returns>
        public void ProcessDataWindow(Ticks timestamp, IMeasurement[,] dataWindow)
        {
            Debug.Assert(dataWindow.GetLength(1) == FramesPerSecond, $"Expected {FramesPerSecond} data window inputs, received {dataWindow.Length}.");

            int inputCount = InputTypes.Length;

            // Break measurement data window into parallel value, time and quality vectors
            double[,] values = new double[inputCount, FramesPerSecond];
            bool[,] valueQualities = new bool[inputCount, FramesPerSecond];
            bool[,] timeQualities = new bool[inputCount, FramesPerSecond];

            for (int i = 0; i < inputCount; i++)
            {
                switch(InputTypes[i])
                {
                    case SignalType.VPHM:
                        for (int j = 0; j < FramesPerSecond; j++)
                        {
                            values[i, j] = dataWindow[i, j].AdjustedValue;
                            valueQualities[i, j] = dataWindow[i, j].ValueQualityIsGood();
                            timeQualities[i, j] = dataWindow[i, j].TimestampQualityIsGood();
                        }
                        break;
                    case SignalType.VPHA:
                        for (int j = 0; j < FramesPerSecond; j++)
                        {
                            values[i, j] = dataWindow[i, j].AdjustedValue;
                            valueQualities[i, j] = dataWindow[i, j].ValueQualityIsGood();
                            timeQualities[i, j] = dataWindow[i, j].TimestampQualityIsGood();
                        }
                        break;
                    case SignalType.IPHM:
                        for (int j = 0; j < FramesPerSecond; j++)
                        {
                            values[i, j] = dataWindow[i, j].AdjustedValue;
                            valueQualities[i, j] = dataWindow[i, j].ValueQualityIsGood();
                            timeQualities[i, j] = dataWindow[i, j].TimestampQualityIsGood();
                        }
                        break;
                    case SignalType.IPHA:
                        for (int j = 0; j < FramesPerSecond; j++)
                        {
                            values[i, j] = dataWindow[i, j].AdjustedValue;
                            valueQualities[i, j] = dataWindow[i, j].ValueQualityIsGood();
                            timeQualities[i, j] = dataWindow[i, j].TimestampQualityIsGood();
                        }
                        break;
                    default:
                        for (int j = 0; j < FramesPerSecond; j++)
                        {
                            values[i, j] = dataWindow[i, j].AdjustedValue;
                            valueQualities[i, j] = dataWindow[i, j].ValueQualityIsGood();
                            timeQualities[i, j] = dataWindow[i, j].TimestampQualityIsGood();
                        }
                        break;
                }
            }

            // Derive overall time result quality state based on all incoming states
            bool[] allQualities = new bool[inputCount * FramesPerSecond];

            for (int i = 0; i < inputCount; i++)
                for (int j = 0; j < FramesPerSecond; j++)
                    allQualities[i * FramesPerSecond + j] = timeQualities[i, j];

            m_timeQualityIsGood = allQualities.All(state => state);

            // Process vector based data window
            DetectorAPI.Load(timestamp, values, valueQualities);
        }

        private void PublishResults(PseudoResult result)
        {
            // Turn results into output measurements
            Measurement[] measurements = new Measurement[Outputs.Length];

            foreach (Output output in Outputs)
            {
                int index = (int)output;

                switch (output)
                {
                    case Output.Band1Energy:
                        measurements[index] = Measurement.Clone(OutputMeasurements[index], result.Band1Energy, result.Timestamp);
                        measurements[index].StateFlags = DeriveQualityFlags.From(result.Band1EnergyQualityIsGood, m_timeQualityIsGood);
                        break;
                    case Output.Band2Energy:
                        measurements[index] = Measurement.Clone(OutputMeasurements[index], result.Band2Energy, result.Timestamp);
                        measurements[index].StateFlags = DeriveQualityFlags.From(result.Band2EnergyQualityIsGood, m_timeQualityIsGood);
                        break;
                    case Output.Band3Energy:
                        measurements[index] = Measurement.Clone(OutputMeasurements[index], result.Band3Energy, result.Timestamp);
                        measurements[index].StateFlags = DeriveQualityFlags.From(result.Band3EnergyQualityIsGood, m_timeQualityIsGood);
                        break;
                    case Output.Band4Energy:
                        measurements[index] = Measurement.Clone(OutputMeasurements[index], result.Band4Energy, result.Timestamp);
                        measurements[index].StateFlags = DeriveQualityFlags.From(result.Band4EnergyQualityIsGood, m_timeQualityIsGood);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            m_publishMeasurements(measurements);
        }

        #endregion

        #region [ Static ]

        // Static Fields

        /// <summary>
        /// Array of <see cref="Output"/> enumeration values.
        /// </summary>
        public static readonly Output[] Outputs = Enum.GetValues(typeof(Output)) as Output[];

        #endregion
    }

    // TODO: Delete these psuedo classes
    #pragma warning disable 1591

    public class PseudoConfiguration
    {
        public int FramesPerSecond { get; set; }
        public bool IsLineToNeutral { get; set; }
    }

    public class PseudoResult
    {
        public DateTime Timestamp { get; set; }
        public double Band1Energy { get; set; }
        public bool Band1EnergyQualityIsGood { get; set; }
        public double Band2Energy { get; set; }
        public bool Band2EnergyQualityIsGood { get; set; }
        public double Band3Energy { get; set; }
        public bool Band3EnergyQualityIsGood { get; set; }
        public double Band4Energy { get; set; }
        public bool Band4EnergyQualityIsGood { get; set; }
    }

    public class PseudoDetectorAPI
    {
        public PseudoConfiguration Configuration { get; set; }

        public void Load(DateTime timestamp, double[,] values, bool[,] qualities)
        {
            PseudoResult result = new PseudoResult { Timestamp = timestamp };

            for (int i = 0; i < values.GetLength(0); i++)
            {
                switch ((OscillationDetector.Output)i)
                {
                    case OscillationDetector.Output.Band1Energy:
                        result.Band1Energy = values.GetColumn(i).Average();
                        result.Band1EnergyQualityIsGood = qualities.GetColumn(i).All(state => state);
                        break;
                    case OscillationDetector.Output.Band2Energy:
                        result.Band2Energy = values.GetColumn(i).Average();
                        result.Band2EnergyQualityIsGood = qualities.GetColumn(i).All(state => state);
                        break;
                    case OscillationDetector.Output.Band3Energy:
                        result.Band3Energy = values.GetColumn(i).Average();
                        result.Band3EnergyQualityIsGood = qualities.GetColumn(i).All(state => state);
                        break;
                    case OscillationDetector.Output.Band4Energy:
                        result.Band4Energy = values.GetColumn(i).Average();
                        result.Band4EnergyQualityIsGood = qualities.GetColumn(i).All(state => state);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            ReportCallback(result);
        }

        public Action<PseudoResult> ReportCallback { get; set; }
    }
}
