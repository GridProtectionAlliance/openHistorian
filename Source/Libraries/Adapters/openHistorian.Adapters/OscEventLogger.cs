//******************************************************************************************************
//  OscEventLogger.cs - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
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
//  11/8/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using GSF;
using GSF.Collections;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.Threading;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.TimeSeries.Data;
using openHistorian.Model;

// ReSharper disable UnusedMember.Local
namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents an action adapter that logs oscillation event records.
    /// </summary>
    [Description("Oscillation Logger: Logs oscillation event records")]
    public class OscEventLogger : ActionAdapterBase
    {
        #region [ Members ]

        // Nested Types
        private enum OscOutputs
        {
            Band1Energy,
            Band2Energy,
            Band3Energy,
            Band4Energy,
            Band1DominantFrequency, 
            Band2DominantFrequency, 
            Band3DominantFrequency, 
            Band4DominantFrequency,
            Band1Alarm,
            Band2Alarm,
            Band3Alarm,
            Band4Alarm,
            SummaryAlarm
        }

        private class AlarmInputs
        {
            public readonly string Source;
            public readonly MeasurementKey[] MeasurementKeys; // One per each OscOutputs

            public AlarmInputs(string source)
            {
                if (string.IsNullOrWhiteSpace(source))
                    throw new ArgumentNullException(nameof(source));

                Source = source;
                MeasurementKeys = new MeasurementKey[Enum.GetValues(typeof(OscOutputs)).Length];
            }

            public override int GetHashCode() => Source.GetHashCode();
        }

        private class AlarmDetails
        {
            public readonly AlarmInputs Inputs;
            public DateTime StartTime;
            public DateTime StopTime;
            public int OscEventsID;
            public readonly double[] MaxMagnitude;          // One for each band, 0-3
            public readonly double[] DominantFrequencySum;  // One for each band, 0-3
            public readonly int[] DominantFrequencyCount;   // One for each band, 0-3

            public AlarmDetails(AlarmInputs inputs)
            {
                Inputs = inputs;
                StartTime = DateTime.MinValue;
                StopTime = DateTime.MinValue;
                OscEventsID = 0;
                MaxMagnitude = new[] { double.NaN, double.NaN, double.NaN, double.NaN };
                DominantFrequencySum = new[] { double.NaN, double.NaN, double.NaN, double.NaN };
                DominantFrequencyCount = new[] { -1, -1, -1, -1 };
            }

            public bool IsStarted => StartTime > DateTime.MinValue;

            public bool IsStopped => StopTime > DateTime.MinValue;
        }

        private class AlarmSummary
        {
            private readonly Dictionary<AlarmInputs, AlarmDetails> m_alarms = new();
            private readonly OscEventLogger m_parent;

            public AlarmSummary(OscEventLogger parent)
            {
                m_parent = parent ?? throw new ArgumentNullException(nameof(parent));
            }

            private AlarmDetails FirstAlarm => m_alarms.Values.FirstOrDefault();

            private bool AllAlarmsStopped => m_alarms.Values.All(alarm => alarm.IsStopped);

            private AlarmDetails TryGetAlarmDetails(AlarmInputs inputs)
            {
                m_alarms.TryGetValue(inputs, out AlarmDetails alarm);
                return alarm;
            }
            
            public void CheckAlarmState(AlarmInputs inputs, IMeasurement[] measurements)
            {
                Debug.Assert(inputs.MeasurementKeys.Length == measurements.Length);

                m_parent.m_totalAlarmTests++;

                AlarmDetails alarm = TryGetAlarmDetails(inputs);

                // Check for summary alarm value
                if (measurements[(int)OscOutputs.SummaryAlarm].AdjustedValue == 0.0D)
                {
                    // Alarm is not triggered, check for stopped state
                    if (alarm is null || !alarm.IsStarted)
                        return;

                    MarkAlarmStopped(alarm);

                    if (AllAlarmsStopped)
                        IdentifyInitiatingEvent();
                }
                else
                {
                    // Alarm is triggered, check for continued or started state
                    if (alarm is null)
                        MarkAlarmStarted(inputs);
                    else if (alarm.IsStarted)
                        UpdateStats(alarm, measurements);
                }
            }

            private void MarkAlarmStarted(AlarmInputs inputs)
            {
                // Initially, just mark first alarm as parent
                int? parentID = FirstAlarm?.OscEventsID;

                AlarmDetails alarm = m_alarms.GetOrAdd(inputs, _ => new AlarmDetails(inputs));
                alarm.StartTime = DateTime.UtcNow;

                m_parent.OnStatusMessage(MessageLevel.Info, $"New oscillation event detected for \"{inputs.Source}\" at {alarm.StartTime:yyyy-MM-dd HH:mm:ss.fff}.");

                // Write new event record
                using AdoDataConnection connection = new("systemSettings");
                TableOperations<OscEvents> oscEventsTable = new(connection);
                OscEvents oscEvents = oscEventsTable.NewRecord();

                oscEvents.ParentID = parentID;
                oscEvents.Source = inputs.Source;
                oscEvents.StartTime = alarm.StartTime;

                oscEventsTable.AddNewRecord(oscEvents);

                // Query newly added record to get auto-inc identity
                oscEvents = oscEventsTable.QueryRecordWhere("Source = {0} AND StartTime = {1}", inputs.Source, alarm.StartTime);

                if (oscEvents is null)
                    m_parent.OnStatusMessage(MessageLevel.Warning, $"Failed to lookup newly added oscillation event record for \"{inputs.Source}\" at {alarm.StartTime:yyyy-MM-dd HH:mm:ss.fff} - event log updates may fail.");
                else
                    alarm.OscEventsID = oscEvents.ID;

                m_parent.m_totalDetectedEvents++;
            }

            private void MarkAlarmStopped(AlarmDetails alarm)
            {
                alarm.StopTime = DateTime.UtcNow;

                m_parent.OnStatusMessage(MessageLevel.Info, $"End of oscillation event detected for \"{alarm.Inputs.Source}\" at {alarm.StopTime:yyyy-MM-dd HH:mm:ss.fff}.");

                // Update event record
                using AdoDataConnection connection = new("systemSettings");
                TableOperations<OscEvents> oscEventsTable = new(connection);
                OscEvents oscEvents = oscEventsTable.QueryRecordWhere("ID = {0}", alarm.OscEventsID);

                oscEvents.StopTime = alarm.StopTime;

                if (!double.IsNaN(alarm.MaxMagnitude[0]))
                    oscEvents.MagnitudeBand1 = alarm.MaxMagnitude[0];

                if (!double.IsNaN(alarm.DominantFrequencySum[0]) && alarm.DominantFrequencyCount[0] > 0)
                    oscEvents.FrequencyBand1 = alarm.DominantFrequencySum[0] / alarm.DominantFrequencyCount[0];

                if (!double.IsNaN(alarm.MaxMagnitude[1]))
                    oscEvents.MagnitudeBand2 = alarm.MaxMagnitude[1];

                if (!double.IsNaN(alarm.DominantFrequencySum[1]) && alarm.DominantFrequencyCount[1] > 0)
                    oscEvents.FrequencyBand2 = alarm.DominantFrequencySum[1] / alarm.DominantFrequencyCount[1];

                if (!double.IsNaN(alarm.MaxMagnitude[2]))
                    oscEvents.MagnitudeBand3 = alarm.MaxMagnitude[2];

                if (!double.IsNaN(alarm.DominantFrequencySum[2]) && alarm.DominantFrequencyCount[2] > 0)
                    oscEvents.FrequencyBand3 = alarm.DominantFrequencySum[2] / alarm.DominantFrequencyCount[2];

                if (!double.IsNaN(alarm.MaxMagnitude[3]))
                    oscEvents.MagnitudeBand4 = alarm.MaxMagnitude[3];

                if (!double.IsNaN(alarm.DominantFrequencySum[3]) && alarm.DominantFrequencyCount[3] > 0)
                    oscEvents.FrequencyBand4 = alarm.DominantFrequencySum[3] / alarm.DominantFrequencyCount[3];

                oscEventsTable.UpdateRecord(oscEvents);
            }

            private void UpdateStats(AlarmDetails alarm, IMeasurement[] measurements)
            {
                // Maintain updated event stats
                if (measurements[(int)OscOutputs.Band1Alarm].AdjustedValue != 0.0D)
                {
                    alarm.MaxMagnitude[0] = Max(alarm.MaxMagnitude[0], measurements[(int)OscOutputs.Band1Energy].AdjustedValue);
                    alarm.DominantFrequencySum[0] = Sum(alarm.DominantFrequencySum[0], measurements[(int)OscOutputs.Band1DominantFrequency].AdjustedValue);
                    alarm.DominantFrequencyCount[0] = Inc(alarm.DominantFrequencyCount[0]);
                    m_parent.DebugMessage($"Updated \"{alarm.Inputs.Source}\" band 1 alarm statistics");
                }

                if (measurements[(int)OscOutputs.Band2Alarm].AdjustedValue != 0.0D)
                {
                    alarm.MaxMagnitude[1] = Max(alarm.MaxMagnitude[1], measurements[(int)OscOutputs.Band2Energy].AdjustedValue);
                    alarm.DominantFrequencySum[1] = Sum(alarm.DominantFrequencySum[1], measurements[(int)OscOutputs.Band2DominantFrequency].AdjustedValue);
                    alarm.DominantFrequencyCount[1] = Inc(alarm.DominantFrequencyCount[1]);
                    m_parent.DebugMessage($"Updated \"{alarm.Inputs.Source}\" band 2 alarm statistics");
                }

                if (measurements[(int)OscOutputs.Band3Alarm].AdjustedValue != 0.0D)
                {
                    alarm.MaxMagnitude[2] = Max(alarm.MaxMagnitude[2], measurements[(int)OscOutputs.Band3Energy].AdjustedValue);
                    alarm.DominantFrequencySum[2] = Sum(alarm.DominantFrequencySum[2], measurements[(int)OscOutputs.Band3DominantFrequency].AdjustedValue);
                    alarm.DominantFrequencyCount[2] = Inc(alarm.DominantFrequencyCount[2]);
                    m_parent.DebugMessage($"Updated \"{alarm.Inputs.Source}\" band 3 alarm statistics");
                }

                if (measurements[(int)OscOutputs.Band4Alarm].AdjustedValue != 0.0D)
                {
                    alarm.MaxMagnitude[3] = Max(alarm.MaxMagnitude[3], measurements[(int)OscOutputs.Band4Energy].AdjustedValue);
                    alarm.DominantFrequencySum[3] = Sum(alarm.DominantFrequencySum[3], measurements[(int)OscOutputs.Band4DominantFrequency].AdjustedValue);
                    alarm.DominantFrequencyCount[3] = Inc(alarm.DominantFrequencyCount[3]);
                    m_parent.DebugMessage($"Updated \"{alarm.Inputs.Source}\" band 4 alarm statistics");
                }
            }

            private void IdentifyInitiatingEvent()
            {
                if (m_alarms.Count > 1)
                {
                    AlarmDetails initiatingEvent = m_alarms.Values.MaxBy(alarm => Max(Max(Max(alarm.MaxMagnitude[0], alarm.MaxMagnitude[1]), alarm.MaxMagnitude[2]), alarm.MaxMagnitude[3]));

                    if (initiatingEvent is not null && !ReferenceEquals(initiatingEvent, FirstAlarm))
                    {
                        using AdoDataConnection connection = new("systemSettings");
                        TableOperations<OscEvents> oscEventsTable = new(connection);

                        // Update parent ID of associated records to properly identify initiating event
                        foreach (AlarmDetails alarm in m_alarms.Values)
                        {
                            OscEvents oscEvents = oscEventsTable.QueryRecordWhere("ID = {0}", alarm.OscEventsID);

                            if (ReferenceEquals(alarm, initiatingEvent))
                            {
                                int associatedCount = m_alarms.Count - 1;
                                oscEvents.ParentID = null; // Null parentID represents initiating event record
                                oscEvents.Notes = $"There {(associatedCount == 1 ? "was" : "were")} {associatedCount:N0} other location{(associatedCount == 1 ? "" : "s")} that detected event";
                            }
                            else
                            {
                                oscEvents.ParentID = initiatingEvent.OscEventsID;
                                oscEvents.Notes = $"\"{initiatingEvent.Inputs.Source}\" was identified as the source of the event";
                            }

                            oscEventsTable.UpdateRecord(oscEvents);
                        }
                    }
                }

                // Clear alarms
                m_alarms.Clear();

                m_parent.m_totalCompletedEvents++;
            }

            private static double Max(double v1, double v2) =>
                double.IsNaN(v1) ? v2 : double.IsNaN(v2) ? v1 : Math.Max(v1, v2);

            private static double Sum(double v1, double v2) =>
                double.IsNaN(v1) ? v2 : double.IsNaN(v2) ? v1 : v1 + v2;

            private static int Inc(int v) =>
                v < 0 ? 1 : v + 1;
        }

        // Constants
        private const int DefaultFramesPerSecond = 1;
        private const double DefaultLagTime = 1.5D;
        private const double DefaultLeadTime = 5.0D;
        private const bool DefaultEnableDebugMessages = false;

        // Fields
        private readonly AlarmSummary m_alarmSummary;
        private readonly ShortSynchronizedOperation m_updateAlarmInputs;
        private Dictionary<string, AlarmInputs> m_alarmInputs;
        private long m_totalAlarmTests;
        private long m_totalDetectedEvents;
        private long m_totalCompletedEvents;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="OscEventLogger"/>.
        /// </summary>
        public OscEventLogger()
        {
            m_alarmSummary = new AlarmSummary(this);
            m_updateAlarmInputs = new ShortSynchronizedOperation(UpdateAlarmInputs, ex => OnProcessException(MessageLevel.Error, ex));
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets primary keys of input measurements the action adapter expects.
        /// </summary>
        /// <remarks>
        /// If your adapter needs to receive all measurements, you must explicitly set InputMeasurementKeys to null.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override MeasurementKey[] InputMeasurementKeys // Redeclared to hide property from TSL host manager - property is automatically managed by this adapter
        {
            get => base.InputMeasurementKeys;
            set
            {
                // When input measurements have been updated, we need to reload alarms inputs
                HashSet<MeasurementKey> inputMeasurements = new(base.InputMeasurementKeys);

                if (inputMeasurements.SetEquals(value))
                    return;

                base.InputMeasurementKeys = value;
                m_updateAlarmInputs.RunOnceAsync();
            }
        }

        /// <summary>
        /// Gets or sets output measurements that the action adapter will produce, if any.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override IMeasurement[] OutputMeasurements // Redeclared to hide property from TSL host manager - property is is not used by this adapter
        {
            get => base.OutputMeasurements;
            set => base.OutputMeasurements = value;
        }

        /// <summary>
        /// Gets or sets the number of frames per second.
        /// </summary>
        /// <remarks>
        /// Valid frame rates for a <see cref="T:GSF.TimeSeries.ConcentratorBase" /> are greater than 0 frames per second.
        /// </remarks>
        [ConnectionStringParameter]
        [Description("Defines the number of frames per second expected by the adapter.")]
        [DefaultValue(DefaultFramesPerSecond)]
        public new int FramesPerSecond // Redeclared to set default value
        {
            get => base.FramesPerSecond;
            set => base.FramesPerSecond = value;
        }

        /// <summary>
        /// Gets or sets the allowed past time deviation tolerance, in seconds (can be sub-second).
        /// </summary>
        /// <remarks>
        /// <para>Defines the time sensitivity to past measurement timestamps.</para>
        /// <para>The number of seconds allowed before assuming a measurement timestamp is too old.</para>
        /// <para>This becomes the amount of delay introduced by the concentrator to allow time for data to flow into the system.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">LagTime must be greater than zero, but it can be less than one.</exception>
        [ConnectionStringParameter]
        [Description("Defines the allowed past time deviation tolerance, in seconds (can be sub-second).")]
        [DefaultValue(DefaultLagTime)]
        public new double LagTime // Redeclared to set default value
        {
            get => base.LagTime;
            set => base.LagTime = value;
        }

        /// <summary>
        /// Gets or sets the allowed future time deviation tolerance, in seconds (can be sub-second).
        /// </summary>
        /// <remarks>
        /// <para>Defines the time sensitivity to future measurement timestamps.</para>
        /// <para>The number of seconds allowed before assuming a measurement timestamp is too advanced.</para>
        /// <para>This becomes the tolerated +/- accuracy of the local clock to real-time.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">LeadTime must be greater than zero, but it can be less than one.</exception>
        [ConnectionStringParameter]
        [Description("Defines the allowed future time deviation tolerance, in seconds (can be sub-second).")]
        [DefaultValue(DefaultLeadTime)]
        public new double LeadTime // Redeclared to set default value
        {
            get => base.LeadTime;
            set => base.LeadTime = value;
        }

        /// <summary>
        /// Gets or sets the flag that enables verbose debug messages.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the flag that enables verbose debug messages.")]
        [DefaultValue(DefaultEnableDebugMessages)]
        public bool EnableDebugMessages { get; set; } = DefaultEnableDebugMessages;

        /// <summary>
        /// Gets the flag indicating if this adapter supports temporal processing.
        /// </summary>
        public override bool SupportsTemporalProcessing => false;

        /// <summary>
        /// Returns the detailed status of this <see cref="OscEventLogger"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new();

                status.Append(base.Status);

                //                  012345678901234567890123456
                status.AppendLine($"         Total Alarm Tests: {m_totalAlarmTests:N0}");
                status.AppendLine($"     Total Detected Events: {m_totalDetectedEvents:N0}");
                status.AppendLine($"    Total Completed Events: {m_totalCompletedEvents:N0}");
                status.AppendLine($"    Verbose Debug Messages: {(EnableDebugMessages ? "Enabled" : "Disabled")}");

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="OscEventLogger"/>.
        /// </summary>
        public override void Initialize()
        {
            Dictionary<string, string> settings = Settings;
            ConnectionStringParser<ConnectionStringParameterAttribute> parser = new();
            parser.ParseConnectionString(ConnectionString, this);

            settings[nameof(InputMeasurementKeys)] = $"FILTER ActiveMeasurements WHERE {string.Join(" OR ", s_oscOutputNames.Select(suffix => $"PointTag LIKE '%{suffix}'"))}";

            if (!settings.ContainsKey(nameof(FramesPerSecond)))
                settings[nameof(FramesPerSecond)] = DefaultFramesPerSecond.ToString();

            if (!settings.ContainsKey(nameof(LagTime)))
                settings[nameof(LagTime)] = DefaultLagTime.ToString(CultureInfo.InvariantCulture);

            if (!settings.ContainsKey(nameof(LeadTime)))
                settings[nameof(LeadTime)] = DefaultLeadTime.ToString(CultureInfo.InvariantCulture);

            ConnectionString = settings.JoinKeyValuePairs();

            base.Initialize();
        }

        private void UpdateAlarmInputs()
        {
            Dictionary<string, AlarmInputs> alarmInputs = new(StringComparer.OrdinalIgnoreCase);
            string[] pointTags = InputMeasurementKeys.Select(key => key.SignalID).Select(LookupPointTag).ToArray();

            DebugMessage($"Loaded {pointTags.Length:N0} configured points to monitor for oscillation alarms");

            for (int i = 0; i < pointTags.Length; i++)
            {
                string pointTag = pointTags[i];
                string source = RemoveSuffix(pointTag);
                AlarmInputs inputs = alarmInputs.GetOrAdd(source, _ =>
                {
                    DebugMessage($"Created new alarm oscillation group for \"{source}\"");
                    return new AlarmInputs(source);
                });

                for (int j = 0; j < s_oscOutputNames.Length; j++)
                {
                    string outputName = s_oscOutputNames[j];
                    
                    if (!pointTag.EndsWith(outputName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    inputs.MeasurementKeys[j] = InputMeasurementKeys[i].Metadata.Key;
                    DebugMessage($"Added \"{outputName}\" tag to alarm oscillation group \"{source}\", count = {inputs.MeasurementKeys.Count(key => key is not null):N0}");
                    break;
                }
            }

            DebugMessage($"Established {alarmInputs.Count:N0} alarm alarm oscillation groups");

            foreach (KeyValuePair<string, AlarmInputs> kvp in alarmInputs.ToArray())
            {
                string source = kvp.Key;
                AlarmInputs inputs = kvp.Value;
                int undefinedKeys = inputs.MeasurementKeys.Count(key => key is null);

                if (undefinedKeys == 0)
                    continue;

                alarmInputs.Remove(source);
                OnStatusMessage(MessageLevel.Warning, $"Oscillation source \"{source}\" had {undefinedKeys:N0} undefined measurements. Alarm inputs removed from event log monitoring.");
            }

            lock (m_alarmSummary)
                m_alarmInputs = alarmInputs;
        }

        /// <summary>
        /// Publish <see cref="IFrame" /> of time-aligned collection of <see cref="IMeasurement" /> values that arrived within the
        /// concentrator's defined <see cref="ConcentratorBase.LagTime" />.
        /// </summary>
        /// <param name="frame"><see cref="IFrame" /> of measurements with the same timestamp that arrived within <see cref="ConcentratorBase.LagTime" /> that are ready for processing.</param>
        /// <param name="index">Index of <see cref="IFrame" /> within a second ranging from zero to <c><see cref="ConcentratorBase.FramesPerSecond" /> - 1</c>.</param>
        protected override void PublishFrame(IFrame frame, int index)
        {
            Dictionary<string, AlarmInputs> alarmInputs;

            lock (m_alarmSummary)
                alarmInputs = m_alarmInputs;

            if (alarmInputs is null)
                return;

            foreach (AlarmInputs inputs in alarmInputs.Values)
            {
                IMeasurement[] measurements = new IMeasurement[inputs.MeasurementKeys.Length];
                bool foundAll = true;

                for (int i = 0; i < inputs.MeasurementKeys.Length; i++)
                {
                    if (!frame.Measurements.TryGetValue(inputs.MeasurementKeys[i], out IMeasurement measurement))
                    {
                        foundAll = false;
                        break;
                    }

                    measurements[i] = measurement;
                }

                if (!foundAll)
                {
                    OnStatusMessage(MessageLevel.Warning, $"Failed to find all oscillation outputs for \"{inputs.Source}\" - event log updates may fail.");
                    continue;
                }

                m_alarmSummary.CheckAlarmState(inputs, measurements);
            }
        }

        private string LookupPointTag(Guid signalID)
        {
            DataRow record = DataSource.LookupMetadata(signalID);
            string pointTag = null;

            // Try alternate tag first, it has a better format
            if (record is not null)
                pointTag = record["AlternateTag"].ToString();

            if (string.IsNullOrWhiteSpace(pointTag) && record is not null)
                pointTag = record["PointTag"].ToString();

            if (string.IsNullOrWhiteSpace(pointTag))
                pointTag = signalID.ToString();

            return pointTag.ToUpper();
        }

        private void DebugMessage(string message)
        {
            if (EnableDebugMessages)
                OnStatusMessage(MessageLevel.Debug, message);
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly string[] s_oscOutputNames = Enum.GetNames(typeof(OscOutputs));

        // Static Methods
        private static string RemoveSuffix(string pointTag)
        {
            int lastDashIndex = pointTag.LastIndexOf("-", StringComparison.Ordinal);
            return (lastDashIndex > 0 ? pointTag.Substring(0, lastDashIndex) : pointTag).Trim();
        }

        #endregion
    }
}
