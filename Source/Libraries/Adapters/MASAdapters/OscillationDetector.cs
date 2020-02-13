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
using System.Numerics;
using System.Text;
using GSF;
using GSF.TimeSeries;
using GSF.Units.EE;
using MathNet.Filtering;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.Interpolation;
using Range = GSF.Range<double>;

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
            Band4Energy,

            /// <summary>
            /// Band alarm summary value.
            /// </summary>
            /// <remarks>
            /// This measurement value provides a bit-wise representation that represents if any bands are currently in alarm,
            /// useful for driving displays.
            /// <list type="table">
            /// <listheader><term>Bit Index</term> <term>Alarm Target (when set)</term></listheader>
            /// <item><term>Bit 0</term> <term>Band 1 alarm triggered.</term></item>
            /// <item><term>Bit 1</term> <term>Band 2 alarm triggered.</term></item>
            /// <item><term>Bit 2</term> <term>Band 3 alarm triggered.</term></item>
            /// <item><term>Bit 3</term> <term>Band 4 alarm triggered.</term></item>
            /// </list>
            /// </remarks>
            BandAlarms
        }

        // Constants

        /// <summary>
        /// Defines the default value for the <see cref="Band1TriggerThreshold"/>.
        /// </summary>
        public const double DefaultBand1TriggerThreshold = 10.0D;
        
        /// <summary>
        /// Defines the default value for the <see cref="Band2TriggerThreshold"/>.
        /// </summary>
        public const double DefaultBand2TriggerThreshold = 10.0D;
        
        /// <summary>
        /// Defines the default value for the <see cref="Band3TriggerThreshold"/>.
        /// </summary>
        public const double DefaultBand3TriggerThreshold = 10.0D;

        /// <summary>
        /// Defines the default value for the <see cref="Band4TriggerThreshold"/>.
        /// </summary>
        public const double DefaultBand4TriggerThreshold = 10.0D;

        /// <summary>
        /// Defines the default point tag template for output measurements.
        /// </summary>
        public const string DefaultPointTagTemplate = nameof(MAS) + ".OD!{0}";

        /// <summary>
        /// Defines the default signal reference template for output measurements.
        /// </summary>
        public const string DefaultSignalReferenceTemplate = DefaultPointTagTemplate + "-CV";

        /// <summary>
        /// Defines the default signal type for output measurements.
        /// </summary>
        public const string DefaultSignalType = "CALC";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets output measurements that algorithm will produce.
        /// </summary>
        public IMeasurement[] OutputMeasurements { get; set; }

        /// <summary>
        /// Gets or sets the frames per second for the incoming data.
        /// </summary>
        public int FramesPerSecond { get; set; }

        /// <summary>
        /// Gets or sets the triggering threshold for band 1 oscillation energy.
        /// </summary>
        public double Band1TriggerThreshold { get; set; } = DefaultBand1TriggerThreshold;

        /// <summary>
        /// Gets or sets the triggering threshold for band 2 oscillation energy.
        /// </summary>
        public double Band2TriggerThreshold { get; set; } = DefaultBand2TriggerThreshold;

        /// <summary>
        /// Gets or sets the triggering threshold for band 3 oscillation energy.
        /// </summary>
        public double Band3TriggerThreshold { get; set; } = DefaultBand3TriggerThreshold;

        /// <summary>
        /// Gets or sets the triggering threshold for band 4 oscillation energy.
        /// </summary>
        public double Band4TriggerThreshold { get; set; } = DefaultBand4TriggerThreshold;

        /// <summary>
        /// Returns the detailed status of the <see cref="OscillationDetector"/>.
        /// </summary>
        public string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.AppendFormat("  Band 1 Trigger Threshold: {0:N3}", Band1TriggerThreshold);
                status.AppendLine();
                status.AppendFormat("  Band 2 Trigger Threshold: {0:N3}", Band2TriggerThreshold);
                status.AppendLine();
                status.AppendFormat("  Band 3 Trigger Threshold: {0:N3}", Band3TriggerThreshold);
                status.AppendLine();
                status.AppendFormat("  Band 4 Trigger Threshold: {0:N3}", Band4TriggerThreshold);
                status.AppendLine();

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Processes 1-second data window consisting of measurements.
        /// </summary>
        /// <param name="timestamp">Top of second window timestamp.</param>
        /// <param name="dataWindow">1-second data window for single value.</param>
        /// <returns>New result measurements ready for publication.</returns>
        public Measurement[] ProcessDataWindow(Ticks timestamp, IMeasurement[] dataWindow)
        {
            Debug.Assert(dataWindow.Length == FramesPerSecond, $"Expected {FramesPerSecond} data window inputs, received {dataWindow.Length}.");

            // Break measurement data window into parallel value, time and quality vectors
            double[] values = dataWindow.Select(measurement => measurement.AdjustedValue).ToArray();
            DateTime[] times = dataWindow.Select(measurement => (DateTime)measurement.Timestamp).ToArray();
            bool[] valueQualities = dataWindow.Select(measurement => measurement.ValueQualityIsGood()).ToArray();
            bool[] timeQualities = dataWindow.Select(measurement => measurement.TimestampQualityIsGood()).ToArray();

            // Process vector based data window
            (double, bool, bool)[] results = ProcessDataWindow(values, times, valueQualities, timeQualities);
            
            Debug.Assert(results.Length == Outputs.Length, $"Expected {Outputs.Length} data window processing results, received {results.Length}.");
            
            // Turn results into output measurements
            Measurement[] measurements = new Measurement[Outputs.Length];

            for (int i = 0; i < Outputs.Length; i++)
            {
                (double value, bool valueQualityIsGood, bool timeQualityIsGood) = results[i];
                measurements[i] = Measurement.Clone(OutputMeasurements[i], value, timestamp);
                measurements[i].StateFlags = Common.DerivedQualityFlags(valueQualityIsGood, timeQualityIsGood);
            }

            return measurements;
        }

        // Process 1-second data window consisting of parallel value, time and quality vectors
        private (double, bool, bool)[] ProcessDataWindow(double[] values, DateTime[] times, bool[] valueQualities, bool[] timeQualities)
        {
            // Create results array which consists of a tuple of result value, value quality flag and time quality flag
            (double value, bool, bool)[] results = new (double, bool, bool)[Outputs.Length];

            bool areGood(bool state) => state;
            bool areNotNaN(double value) => !double.IsNaN(value);

            // Derive result quality state based on all incoming values and states
            bool valueQualityIsGood = valueQualities.All(areGood) && values.All(areNotNaN);
            bool timeQualityIsGood = timeQualities.All(areGood);

            InterpolateMissingValues(values);
            
            foreach (Output output in Outputs)
            {
                double value;
                
                switch (output)
                {
                    case Output.Band1Energy:
                        value = CalculateOscillationEnergy(Band1Range, FramesPerSecond, values, times);
                        break;
                    case Output.Band2Energy:
                        value = CalculateOscillationEnergy(Band2Range, FramesPerSecond, values, times);
                        break;
                    case Output.Band3Energy:
                        value = CalculateOscillationEnergy(Band3Range, FramesPerSecond, values, times);
                        break;
                    case Output.Band4Energy:
                        value = CalculateOscillationEnergy(Band4Range, FramesPerSecond, values, times);
                        break;
                    case Output.BandAlarms:
                        value = CalculateBandAlarms(results.Select(result => result.value).ToArray());
                        break;
                    default:
                        value = double.NaN;
                        break;
                }

                results[(int)output] = (value, valueQualityIsGood, timeQualityIsGood);
            }

            return results;
        }

        private double CalculateBandAlarms(double[] results)
        {
            uint bandAlarmBits = 0;

            foreach (Output output in Outputs)
            {
                int index = (int)output;
                double result = results[index];
                bool alarmTriggered = false;

                switch (output)
                {
                    case Output.Band1Energy:
                        alarmTriggered = result > Band1TriggerThreshold;
                        break;
                    case Output.Band2Energy:
                        alarmTriggered = result > Band2TriggerThreshold;
                        break;
                    case Output.Band3Energy:
                        alarmTriggered = result > Band3TriggerThreshold;
                        break;
                    case Output.Band4Energy:
                        alarmTriggered = result > Band4TriggerThreshold;
                        break;
                }

                if (alarmTriggered)
                    bandAlarmBits |= (uint)BitExtensions.BitVal(index);
            }

            return bandAlarmBits;
        }

        #endregion

        #region [ Static ]

        // Static Fields

        /// <summary>
        /// Array of <see cref="Output"/> enumeration values.
        /// </summary>
        public static readonly Output[] Outputs = Enum.GetValues(typeof(Output)) as Output[];

        /// <summary>
        /// Band 1 pass-band limits.
        /// </summary>
        public static readonly Range Band1Range = new Range(0.01D, 0.15D);

        /// <summary>
        /// Band 2 pass-band limits.
        /// </summary>
        public static readonly Range Band2Range = new Range(0.15D, 1.0D);

        /// <summary>
        /// Band 3 pass-band limits.
        /// </summary>
        public static readonly Range Band3Range = new Range(1.0D, 5.0D);

        /// <summary>
        /// Band 4 pass-band limits.
        /// </summary>
        public static readonly Range Band4Range = new Range(5.0D, 120.0D);

        // Static Methods

        private static void InterpolateMissingValues(double[] values)
        {
            // Create an x/y data set of all non-NAN values
            List<double> yValues = new List<double>();
            List<double> xValues = new List<double>();

            for (int i = 0; i < values.Length; i++)
            {
                if (double.IsNaN(values[i]))
                    continue;

                xValues.Add(i);
                yValues.Add(values[i]);
            }

            if (xValues.Count < 2)
                return;

            CubicSpline spline = CubicSpline.InterpolateAkimaSorted(xValues.ToArray(), yValues.ToArray());

            for (int i = 0; i < values.Length; i++)
            {
                if (double.IsNaN(values[i]))
                    values[i] = spline.Interpolate(i);
            }
        }

        // TODO: This function simply produces a value, math is not intended to be accurate
        private static double CalculateOscillationEnergy(Range range, int framesPerSecond, double[] values, DateTime[] times)
        {
            OnlineFilter bandpass = OnlineFilter.CreateBandpass(ImpulseResponse.Finite, framesPerSecond, range.Start, range.End);
            
            Complex[] data = bandpass.ProcessSamples(values).Select(value => new Complex(value, 0.0D)).ToArray();

            Fourier.Inverse(data, FourierOptions.AsymmetricScaling);

            return data.Select(value => value.Magnitude).Take(data.Length / 2).Average();
        }

        #endregion
    }
}
