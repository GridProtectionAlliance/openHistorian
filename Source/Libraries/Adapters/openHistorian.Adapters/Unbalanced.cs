//******************************************************************************************************
//  Unbalanced.cs - Gbtc
//
//  Copyright © 2019, Grid Protection Alliance.  All Rights Reserved.
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
//  10/03/2019 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF;
using GSF.Collections;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Threading;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.TimeSeries.Data;
using openHistorian.Model;
using PhasorProtocolAdapters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Measurement = GSF.TimeSeries.Measurement;
using MeasurementRecord = openHistorian.Model.Measurement;
using SignalType = GSF.Units.EE.SignalType;
using SignalTypeRecord = openHistorian.Model.SignalType;

// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
namespace openHistorian.Adapters
{
    /// <summary>
    /// Defines an Adapter that calculates Unbalance Factors.
    /// </summary>
    [Description("Unbalanced Computation: Computes unbalance factors for voltage and currents")]
    public class UnBalancedCalculation : CalculatedMeasurementBase
    {
        #region [ Members ]

        private class DailySummary
        {
            public DateTime Date { get; set; }
            //public double S0S1 { get; set; }
            public double S0S1 { get; set; }
            public Guid PositivePhaseSignalID { get; set; }
            public int Alarm { get; set; }
            public int PercentAlarm { get; set; }
            public string SignalType { get; set; }

        }

        /// <summary>
        ///  Represents Set of input and output measurement used for Unbalance computation
        /// </summary>
        public class ThreePhaseSet
        {
        #pragma warning disable 1591
            public Guid PositiveSequence { get; set; }
            public Guid NegativeSequence { get; set; }
            public Guid ZeroSequence { get; set; }
            public Guid S0S1 { get; set; }
            public Guid S2S1 { get; set; }

            public string SignalType { get; set; }

            public double count;
            public double sum;
            public void Reset() => count = sum = 0;

            public bool activeAlarm;
            public bool acknowlegedAlarm;
            public int countExceeding;
            public double BaseVoltage;

        #pragma warning restore 1591
        }


        // Constants      

        private const string ReportSettingsCategory = "snrSQLReportingDB";

        /// <summary>
        /// Default value for <see cref="MappingFilePath"/>.
        /// </summary>
        public const string DefaultMappingFile = "";

        /// <summary>
        /// Defines the default value for the <see cref="ParentDeviceAcronymTemplate"/>.
        /// </summary>
        public const string DefaultParentDeviceAcronymTemplate = "IAM!{0}";

        /// <summary>
        /// Defines the default value for the <see cref="PointTagTemplate"/>.
        /// </summary>
        public const string DefaultPointTagTemplate = "UBAL!{0}";

        /// <summary>
        /// Defines the default value for the <see cref="SignalReferenceTemplate"/>.
        /// </summary>
        public const string DefaultSignalReferenceTemplate = DefaultPointTagTemplate + "-CV";

        /// <summary>
        /// Defines the default value for the <see cref="AlternateTagTemplate"/>.
        /// </summary>
        public const string DefaultAlternateTagTemplate = "";

        /// <summary>
        /// Defines the default value for the <see cref="DescriptionTemplate"/>.
        /// </summary>
        public const string DefaultDescriptionTemplate = "{0} [{1}] measurement created for {2} [{3}].";

        /// <summary>
        /// Defines the default value for the <see cref="TargetHistorianAcronym"/>.
        /// </summary>
        public const string DefaultTargetHistorianAcronym = "PPA";

        /// <summary>
        /// Defines the default value for the <see cref="MaxSQLRows"/>.
        /// </summary>
        public const int DefaultMaxSQLRows = 999;

        /// <summary>
        /// Defines the default value for the <see cref="AggregationWindow"/>.
        /// </summary>
        public const int DefaultWindowSize = 30;


        /// <summary>
        /// Defines the default value for the <see cref="AlarmSetPoint"/>.
        /// </summary>
        public const double DefaultAlarmThreshhold = 0.03;

        /// <summary>
        /// Defines the default value for the <see cref="AlarmDelay"/>.
        /// </summary>
        public const int DefaultAlarmDelay = 5;

        /// <summary>
        /// Defines the default value for the <see cref="AlarmHysterisis"/>.
        /// </summary>
        public const double DefaultAlarmHysterisis = 0;




        // Fields
        private int m_numberOfFrames;
        private double m_lastS0S1;
        private double m_lastS2S1;
        private bool m_countAlarms;
        private int m_sqlFailures;
        private int m_skipped;

        private LongSynchronizedOperation m_sqlWritter;
        private IsolatedQueue<DailySummary> m_summaryQueue;
        private CancellationToken m_cancelationTokenSql;

        private ConcurrentBag<ThreePhaseSet> m_threePhaseComponent;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the File used for mapping 
        /// </summary>
        [ConnectionStringParameter]
        [Description("Sets the Path to the Mapping File to get sets of sequence Voltages - Order of the Entries is +,-,0.")]
        [DefaultValue(DefaultMappingFile)]
        [CustomConfigurationEditor("GSF.TimeSeries.UI.WPF.dll", "GSF.TimeSeries.UI.Editors.FileDialogEditor", "type=open; checkFileExists=true; defaultExt=.csv; filter=CSV files|*.csv|AllFiles|*.*")]
        public string MappingFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Size of the Window used for aggregating to avoid Incorrect Results 
        /// </summary>
        [ConnectionStringParameter]
        [Description("Sets the Size of the window used to aggregate results to avoid spikes due to bad data.")]
        [DefaultValue(DefaultWindowSize)]
        public int AggregationWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum number of rows inserted into sql as single Transaction.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the maximum number of daily Summary Points inserted at once.")]
        [DefaultValue(DefaultMaxSQLRows)]
        public int MaxSQLRows { get; set; }

        /// <summary>
        /// Gets or sets the Set point for Alarming.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines Threshold for alarming on unbalance. If it is 0 there are no Alarms")]
        [DefaultValue(DefaultAlarmThreshhold)]
        public double AlarmSetPoint { get; set; }

        /// <summary>
        /// Gets or sets the Delay for Alarming.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the delay for alarming on unbalance in sec.")]
        [DefaultValue(DefaultAlarmDelay)]
        public int AlarmDelay { get; set; }

        /// <summary>
        /// Gets or sets the Hysteresis for Alarming.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the hysteresis for alarming on unbalance")]
        [DefaultValue(DefaultAlarmHysterisis)]
        public double AlarmHysterisis { get; set; }

        /// <summary>
        /// Gets or sets template for the parent device acronym used to group associated output measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for the parent device acronym used to group associated output measurements, typically an expression like \"" + DefaultParentDeviceAcronymTemplate + "\" where \"{0}\" is substituted with this adapter name. Set to blank value to create no parent device associated output measurements. Note that \"{0}\" token is not required, you can simply use a specific device acronym.")]
        [DefaultValue(DefaultParentDeviceAcronymTemplate)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ParentDeviceAcronymTemplate { get; set; } = DefaultParentDeviceAcronymTemplate;

        /// <summary>
        /// Gets or sets the ReportSQL flag.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines whether the daily summary is saved to a SQL Database.")]
        [DefaultValue(false)]
        public bool ReportSQL { get; set; }

        /// <summary>
        /// Gets or sets the UseNominalThreshhold flag.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines whether the Voltage Threshold is in p.u. or Volts.")]
        [DefaultValue(true)]
        public bool UseNominalThreshhold { get; set; }

        /// <summary>
        /// Gets or sets the Voltage Threshhold.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines The minimum Voltage (in p.u. or V) for computing Unbalanced.")]
        [DefaultValue(0.5D)]
        public double NoiseThreshhold { get; set; }

        /// <summary>
        /// Gets or sets template for output measurement point tag names.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement point tag names, typically an expression like \"" + DefaultPointTagTemplate + "\" where \"{0}\" is substituted with this adapter name, a dash and then the PerAdapterOutputNames value for the current measurement. Note that \"{0}\" token is not required, property can be overridden to provide desired value.")]
        [DefaultValue(DefaultPointTagTemplate)]
        public string PointTagTemplate { get; set; } = DefaultPointTagTemplate;

        /// <summary>
        /// Gets or sets template for output measurement signal reference names.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement signal reference names, typically an expression like \"" + DefaultSignalReferenceTemplate + "\" where \"{0}\" is substituted with this adapter name, a dash and then the PerAdapterOutputNames value for the current measurement. Note that \"{0}\" token is not required, property can be overridden to provide desired value.")]
        [DefaultValue(DefaultSignalReferenceTemplate)]
        public string SignalReferenceTemplate { get; set; } = DefaultSignalReferenceTemplate;

        /// <summary>
        /// Gets or sets template for output measurement alternate tag names.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement alternate tag names, typically an expression where \"{0}\" is substituted with this adapter name, a dash and then the PerAdapterOutputNames value for the current measurement. Note that \"{0}\" token is not required, property can be overridden to provide desired value.")]
        [DefaultValue(DefaultAlternateTagTemplate)]
        public string AlternateTagTemplate { get; set; } = DefaultAlternateTagTemplate;

        /// <summary>
        /// Gets or sets template for output measurement descriptions.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement descriptions, typically an expression like \"" + DefaultDescriptionTemplate + "\".")]
        [DefaultValue(DefaultDescriptionTemplate)]
        public string DescriptionTemplate { get; set; } = DefaultDescriptionTemplate;

        /// <summary>
        /// Gets or sets the target historian acronym for output measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the target historian acronym for output measurements.")]
        [DefaultValue(DefaultTargetHistorianAcronym)]
        public string TargetHistorianAcronym { get; set; } = DefaultTargetHistorianAcronym;


        /// <summary>
        /// Gets or sets output measurements that the <see cref="UnBalancedCalculation"/> will produce, if any.
        /// </summary>
        /// [ConnectionStringParameter]
        [Description("Defines the list of output measurements.")]
        [DefaultValue(null)]
        [EditorBrowsable(EditorBrowsableState.Never)] // Hiding parameter from manager - outputs managed automatically
        public override IMeasurement[] OutputMeasurements
        {
            get => base.OutputMeasurements;
            set => base.OutputMeasurements = value;
        }

        /// <summary>
        /// Gets or sets primary keys of input measurements the calculated measurement expects.
        /// </summary>
        [DefaultValue(null)]
        [Description("Defines primary keys of input measurements the action adapter expects; can be one of a filter expression, measurement key, point tag or Guid.")]
        [EditorBrowsable(EditorBrowsableState.Never)] // Hiding parameter from manager - inputs managed automatically based on Mapping file
        public override MeasurementKey[] InputMeasurementKeys
        {
            get => base.InputMeasurementKeys;
            set => base.InputMeasurementKeys = value;
        }


        /// <summary>
        /// Gets a detailed status for this <see cref="UnBalancedCalculation"/>.
        /// </summary>
        public override string Status
        {
            get
            {

                StringBuilder status = new StringBuilder();

                status.AppendFormat("          Mapping File: {0}", MappingFilePath);
                status.AppendLine();
                status.AppendFormat(" Number of 3Phase Sets: {0}", m_threePhaseComponent?.Count ?? 0) ;
                status.AppendLine();
                status.AppendFormat("       Last S0S1 Value: {0:N3}", m_lastS0S1);
                status.AppendLine();
                status.AppendFormat("       Last S2S1 Value: {0:N3}", m_lastS2S1);
                status.AppendLine();
                status.AppendFormat("Waiting to write Summaries to SQL: {0}", m_summaryQueue?.Count ?? 0);
                status.AppendLine();
                status.AppendFormat("Sequential SQL exceptions: {0}", m_sqlFailures);
                status.AppendLine();
                status.AppendFormat("Use Nominal V for Threshold: {0}", UseNominalThreshhold);
                status.AppendLine();
                status.AppendFormat("Noise threshold: {0}", NoiseThreshhold);
                status.AppendLine();
                status.AppendFormat("Skipped Measurements: {0}", m_skipped);
                status.AppendLine();
                status.Append(base.Status);

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="UnBalancedCalculation"/>.
        /// </summary>
        public override void Initialize()
        {
            ParseConnectionString();

            base.Initialize();

            //Ensure AggregationWindow and Alarming Delay do not Conflict
            if (AggregationWindow > AlarmDelay * FramesPerSecond)
                throw new ArgumentException("Aggregation Window (in frames) needs to be smaller than AlarmDelay (in seconds)");

            m_skipped = 0;
            m_threePhaseComponent = new ConcurrentBag<ThreePhaseSet>();
           
            List<string> entries = new List<string>();
            string line;
            // Read Mapping File
            using (StreamReader reader = new StreamReader(FilePath.GetAbsolutePath(MappingFilePath)))
                while ((line = reader.ReadLine()) != null)
                {
                    entries.Add(line);
                }
                
            if (entries.Count == 0)
            {
                OnStatusMessage(MessageLevel.Warning, "No 3 Phase mappings specified.");
                return;
            }

            int? currentDeviceID = null;

            // Create associated parent device for output measurements if 
            if (!string.IsNullOrWhiteSpace(ParentDeviceAcronymTemplate))
            {

                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    TableOperations<Device> deviceTable = new TableOperations<Device>(connection);
                    string deviceAcronym = string.Format(ParentDeviceAcronymTemplate, Name);

                    Device device = deviceTable.QueryRecordWhere("Acronym = {0}", deviceAcronym) ?? deviceTable.NewRecord();
                    int protocolID = connection.ExecuteScalar<int?>("SELECT ID FROM Protocol WHERE Acronym = 'VirtualInput'") ?? 15;

                    device.Acronym = deviceAcronym;
                    device.Name = deviceAcronym;
                    device.ProtocolID = protocolID;
                    device.Enabled = true;

                    deviceTable.AddNewOrUpdateRecord(device);
                    currentDeviceID = deviceTable.QueryRecordWhere("Acronym = {0}", deviceAcronym)?.ID;
                }
            }

            ConcurrentBag<MeasurementKey> inputs = new ConcurrentBag<MeasurementKey>();

            Parallel.For(0, entries.Count, (i) =>
            {
                List<string> pointTags = entries[i].Split(',').Select(item => item.Trim()).ToList();
                if (pointTags.Count != 3)
                {
                    OnStatusMessage(MessageLevel.Warning, $"Skipping Line {i} in mapping file.");
                    return;
                }
                Guid signalId1;
                Guid signalId2;
                Guid signalId3;

                try
                {
                    using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                    {
                        signalId1 = connection.ExecuteScalar<Guid>("SELECT SignalID FROM ActiveMeasurement WHERE PointTag = {0}", pointTags[0].Trim());
                        signalId2 = connection.ExecuteScalar<Guid>("SELECT SignalID FROM ActiveMeasurement WHERE PointTag = {0}", pointTags[1].Trim());
                        signalId3 = connection.ExecuteScalar<Guid>("SELECT SignalID FROM ActiveMeasurement WHERE PointTag = {0}", pointTags[2].Trim());

                        if (signalId1 == Guid.Empty || signalId2 == Guid.Empty || signalId3 == Guid.Empty)
                        {
                            OnStatusMessage(MessageLevel.Warning, $"Skipping Line {i} in mapping file.");
                            return;
                        }
                    }
                    MeasurementKey input1 = MeasurementKey.LookUpBySignalID(signalId1);
                    MeasurementKey input2 = MeasurementKey.LookUpBySignalID(signalId2);
                    MeasurementKey input3 = MeasurementKey.LookUpBySignalID(signalId3);

                    // We Assume they are in order... +, -, 0 This is important
                    Guid output;

                    // Check if an Output Model already exists
                    using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                    {
                        ThreePhaseSet keys = new TableOperations<ThreePhaseSet>(connection).QueryRecordWhere("PositiveSequence = {0} AND NegativeSequence = {1} AND ZeroSequence = {2}", input1.SignalID, input2.SignalID, input3.SignalID);

                        if (keys is null)
                        {
                            string inputName = LookupPointTag(input1.SignalID);
                            string alternateTagPrefix = null;

                            if (!string.IsNullOrWhiteSpace(AlternateTagTemplate))
                            {
                                string deviceName = LookupDevice(input1.SignalID);
                                string phasorLabel = LookupPhasorLabel(input1.SignalID);
                                alternateTagPrefix = $"{deviceName}-{phasorLabel}";
                            }
                            string perAdapterOutputName = "RESULT";
                            string outputPrefix = $"{Name}!{inputName}-{perAdapterOutputName}";
                            string outputPointTag = string.Format(PointTagTemplate, outputPrefix);
                            string outputAlternateTag = string.Format(AlternateTagTemplate, alternateTagPrefix is null ? "" : $"{alternateTagPrefix}-{perAdapterOutputName}");
                            string signalReference = string.Format(SignalReferenceTemplate, outputPrefix);
                            string description = string.Format(DescriptionTemplate, outputPrefix, SignalType.CALC, Name, GetType().Name);

                            // Get output measurement record, creating a new one if needed
                            MeasurementRecord measurement = GetMeasurementRecord(currentDeviceID ?? CurrentDeviceID(i), outputPointTag, outputAlternateTag, signalReference, description, SignalType.CALC, TargetHistorianAcronym);

                            MeasurementRecord posSeq = new TableOperations<MeasurementRecord>(connection).QueryRecordWhere("SignalId = {0}", input1.SignalID);
                            string unbalancetype = "X";

                            if (posSeq != null && posSeq.SignalTypeID == (int)SignalType.IPHM)
                                unbalancetype = "I";
                            else if (posSeq != null && posSeq.SignalTypeID == (int)SignalType.VPHM)
                                unbalancetype = "V";

                            output = measurement.SignalID;

                            keys = new ThreePhaseSet() { PositiveSequence = input1.SignalID, NegativeSequence = input2.SignalID, ZeroSequence = input3.SignalID, S0S1 = output, S2S1 = output };
                            keys.SignalType = unbalancetype;
                            new TableOperations<ThreePhaseSet>(connection).AddNewRecord(keys);


                        }

                        keys.Reset();
                        keys.activeAlarm = false;
                        keys.countExceeding = 0;
                        keys.acknowlegedAlarm = false;
                        keys.BaseVoltage = connection.ExecuteScalar<int>("SELECT COALESCE((SELECT BaseKV FROM Phasor WHERE ID = (SELECT PhasorID FROM ActiveMeasurement WHERE SignalID = {0})), 0)", keys.PositiveSequence);
                        m_threePhaseComponent.Add(keys);

                    }

                    // Need to add Measurements to inputMeasuremetnKeys
                    inputs.Add(input1);
                    inputs.Add(input2);
                    inputs.Add(input3);
                }
                catch (Exception ex)
                {
                    OnStatusMessage(MessageLevel.Error, $"An Error Occurred while Processing Line {i}: {ex.Message}.");
                }
            });

            InputMeasurementKeys = inputs.ToArray();

            if (m_threePhaseComponent.Count == 0)
                OnStatusMessage(MessageLevel.Error, "No case with all 3 sequences was found");

            m_countAlarms = AlarmSetPoint != 0;

            m_numberOfFrames = 0;
            m_sqlFailures = 0;

            m_cancelationTokenSql = new CancellationToken();

            m_sqlWritter = new LongSynchronizedOperation(() => { SaveToSQL(m_cancelationTokenSql); }, ex => {
                m_sqlFailures++;
                OnStatusMessage(MessageLevel.Error, $"Unable to save summary data to SQL: {ex.Message}");

                if (m_sqlFailures < 10)
                    m_sqlWritter.RunOnceAsync();
                else
                    ReportSQL = false;
            });

            m_summaryQueue = new IsolatedQueue<DailySummary>();

            if (ReportSQL)
                m_sqlWritter.RunOnceAsync();
        }

        /// <summary>
        /// Publish frame of time-aligned collection of measurement values that arrived within the defined lag time.
        /// </summary>
        /// <param name="frame">Frame of measurements with the same timestamp that arrived within lag time that are ready for processing.</param>
        /// <param name="index">Index of frame within a second ranging from zero to frames per second - 1.</param>
        protected override void PublishFrame(IFrame frame, int index)
        {

            m_numberOfFrames++;
            m_skipped = 0;
            List<IMeasurement> outputmeasurements = new List<IMeasurement>();
            bool report = m_numberOfFrames > AggregationWindow;
            
            foreach (ThreePhaseSet set in m_threePhaseComponent)
            {
                bool hasP = frame.Measurements.TryGetValue(MeasurementKey.LookUpBySignalID(set.PositiveSequence), out IMeasurement positiveSeq);
                bool hasN = frame.Measurements.TryGetValue(MeasurementKey.LookUpBySignalID(set.NegativeSequence), out IMeasurement negativeSeq);
                bool hasZ = frame.Measurements.TryGetValue(MeasurementKey.LookUpBySignalID(set.ZeroSequence), out IMeasurement zeroSeq);

                double s0s1 = double.NaN;
                double s2s1 = double.NaN;

                double v1P = double.NaN;

                if (hasP && hasN)
                {
                    v1P = positiveSeq.AdjustedValue;
                    double v2N = negativeSeq.AdjustedValue;

                    if (v1P != 0.0D)
                    {
                        s2s1 = v2N / v1P;
                        m_lastS2S1 = s2s1;
                    }
                }
                if (hasP && hasZ)
                {
                    v1P = positiveSeq.AdjustedValue;
                    double v0Z = zeroSeq.AdjustedValue;

                    if (v1P != 0.0D)
                    {
                        s0s1 = v0Z / v1P;
                        m_lastS0S1 = s0s1;
                    }
                }

                // Noise Threshold
                if (!hasP)
                    continue;

                double threshold = NoiseThreshhold * (UseNominalThreshhold ? set.BaseVoltage: 1.0D);
                if (v1P < threshold)
                {
                    m_skipped++;
                    continue;
                }
                    

                Measurement s2s1Measurement = new Measurement { Metadata = MeasurementKey.LookUpBySignalID(set.S0S1).Metadata };
                outputmeasurements.Add(Measurement.Clone(s2s1Measurement, s2s1, frame.Timestamp));

                // Logic to count Alarms
                if (m_countAlarms)
                {

                    if (s0s1 > AlarmSetPoint)
                        set.countExceeding++;

                    if (set.countExceeding > AlarmDelay * FramesPerSecond)
                        set.activeAlarm = true;


                    if (s0s1 < AlarmSetPoint - AlarmHysterisis)
                    {
                        set.countExceeding = 0;
                        set.acknowlegedAlarm = false;
                        set.activeAlarm = false;
                    }
                }

                if (ReportSQL && !double.IsNaN(s0s1))
                {
                    set.sum += s0s1;
                    set.count += 1.0;

                    if (report)
                    {
                        DailySummary summary = new DailySummary()
                        {
                            PositivePhaseSignalID = set.PositiveSequence,
                            S0S1 = set.sum / set.count,
                            Date = ((DateTime)frame.Timestamp).RoundDownToNearestDay(),
                            Alarm = 0,
                            PercentAlarm = 0,
                            SignalType = set.SignalType
                        };

                        if (m_countAlarms && set.activeAlarm && !set.acknowlegedAlarm)
                        {
                            summary.Alarm = 1;
                            set.acknowlegedAlarm = true;
                        }

                        if (set.activeAlarm)
                            summary.PercentAlarm = 1;

                        m_summaryQueue.Enqueue(summary);
                        set.Reset();
                    }
                }
            }

            if (report)
                m_numberOfFrames = 0;

            OnNewMeasurements(outputmeasurements);

        }

       

        private void SaveToSQL(CancellationToken CancellationToken)
        {
            while (!CancellationToken.IsCancelled)
            {
                DailySummary[] data = new DailySummary[MaxSQLRows];

                int n = m_summaryQueue.Dequeue(data, 0, MaxSQLRows);
                if (n > 0)
                {
                    string sqlCmd = "INSERT INTO Unbalance (Date, S0S1, PositivePhaseSignalID, SignalType, Alarm, ActiveAlarm) VALUES";

                    for (int i = 0; i < n; i++)
                        sqlCmd = sqlCmd + $"\n ('{data[i].Date}',{data[i].S0S1},'{data[i].PositivePhaseSignalID}','{data[i].SignalType}',{data[i].Alarm},{data[i].PercentAlarm}),";

                    sqlCmd = sqlCmd.Substring(0, sqlCmd.Length - 1);
                    using (AdoDataConnection connection = new AdoDataConnection(ReportSettingsCategory))
                        connection.ExecuteNonQuery(sqlCmd);
                }

                m_sqlFailures = 0;
            }

        }

        /// <summary>
        /// Gets associated device ID if any, for measurement generation.
        /// </summary>
        /// <param name="measurementIndex"> The imeasurement Index for which this device is</param>
        /// <remarks>
        /// If overridden to provide custom device ID, <see cref="ParentDeviceAcronymTemplate" /> should be set
        /// to <c>null</c> so no parent device is created.
        /// </remarks>
        private int CurrentDeviceID(int measurementIndex)
        {
            // Idea here is to associate each new output measurement with device associated with input.
            // When this value is not specified and the ParentDeviceAcronymTemplate is defined, system
            // will create a single new device for "all" output measurements to be associated with, this
            // way the values will still have a parent and can be transported via protocols like STTP.

            try
            {
                if (measurementIndex > -1)
                {
                    // Just pick first input measurement to find associated device ID
                    MeasurementKey inputMeasurement = InputMeasurementKeys[measurementIndex];
                    DataRow record = DataSource.LookupMetadata(inputMeasurement.SignalID, "ActiveMeasurements");
                    int runtimeID = record?.ConvertNullableField<int>("DeviceID") ?? throw new Exception($"Failed to find associated runtime device ID for input measurement {inputMeasurement.SignalID}");

                    // Query the actual database record ID based on the known runtime ID for this device
                    using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                    {
                        TableOperations<Runtime> runtimeTable = new TableOperations<Runtime>(connection);
                        Runtime runtimeRecord = runtimeTable.QueryRecordWhere("ID = {0} AND SourceTable='Device'", runtimeID);
                        return runtimeRecord.SourceID;
                    }
                }
            }
            catch (Exception ex)
            {
                OnProcessException(MessageLevel.Error, new InvalidOperationException($"Failed to lookup current device ID for adapter {measurementIndex:N0}: {ex.Message}", ex));
            }

            return 0;
        }

        /// <summary>
        /// Lookups up point tag name from provided <paramref name="signalID"/>.
        /// </summary>
        /// 
        /// <param name="signalID"><see cref="Guid"/> signal ID to lookup.</param>
        /// <returns>Point tag name, if found; otherwise, string representation of provided signal ID.</returns>
        private string LookupPointTag(Guid signalID)
        {
            DataRow record = DataSource.LookupMetadata(signalID, "ActiveMeasurements");
            string pointTag = null;

            if (record != null)
                pointTag = record["PointTag"].ToString();

            if (string.IsNullOrWhiteSpace(pointTag))
                pointTag = signalID.ToString();

            return pointTag.ToUpper();
        }

        /// <summary>
        /// Lookups up associated device name from provided <paramref name="signalID"/>.
        /// </summary>
        /// <param name="signalID"><see cref="Guid"/> signal ID to lookup.</param>
        /// <returns>Device name, if found; otherwise, string representation of associated point tag.</returns>
        private string LookupDevice(Guid signalID)
        {
            DataRow record = DataSource.LookupMetadata(signalID, "ActiveMeasurements");
            string device = null;

            if (record != null)
                device = record["Device"].ToString();

            if (string.IsNullOrWhiteSpace(device))
                device = LookupPointTag(signalID);

            return device.ToUpper();
        }

        /// <summary>
        /// Lookups up associated phasor label from provided <paramref name="signalID"/>.
        /// </summary>
        /// <param name="signalID"><see cref="Guid"/> signal ID to lookup.</param>
        /// <returns>Phasor label name, if found; otherwise, string representation associated point tag.</returns>
        private string LookupPhasorLabel(Guid signalID)
        {
            DataRow record = DataSource.LookupMetadata(signalID, "ActiveMeasurements");
            int phasorID = 0;

            if (record != null)
                phasorID = record.ConvertNullableField<int>("PhasorID") ?? 0;

            if (phasorID == 0)
                return LookupPointTag(signalID);

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<GSF.TimeSeries.Model.Phasor> phasorTable = new TableOperations<GSF.TimeSeries.Model.Phasor>(connection);
                GSF.TimeSeries.Model.Phasor phasorRecord = phasorTable.QueryRecordWhere("ID = {0}", phasorID);
                return phasorRecord is null ? LookupPointTag(signalID) : phasorRecord.Label.Trim().ToUpper();
            }
        }

        /// <summary>
        /// Parses connection string.
        /// </summary>
        private void ParseConnectionString()
        {
            //Dictionary<string, string> settings = Settings;

            // Parse all properties marked with ConnectionStringParameterAttribute from provided ConnectionString value
            ConnectionStringParser parser = new ConnectionStringParser<ConnectionStringParameterAttribute>();
            parser.ParseConnectionString(ConnectionString, this);
        }

        /// <summary>
        /// Gets measurement record, creating it if needed.
        /// </summary>
        /// <param name="currentDeviceID">Device ID associated with current adapter, or zero if none.</param>
        /// <param name="pointTag">Point tag of measurement.</param>
        /// <param name="alternateTag">Alternate tag of measurement.</param>
        /// <param name="signalReference">Signal reference of measurement.</param>
        /// <param name="description">Description of measurement.</param>
        /// <param name="signalType">Signal type of measurement.</param>
        /// <param name="targetHistorianAcronym">Acronym of target historian for measurement.</param>
        /// <returns>Measurement record.</returns>
        private MeasurementRecord GetMeasurementRecord(int currentDeviceID, string pointTag, string alternateTag, string signalReference, string description, SignalType signalType = SignalType.CALC, string targetHistorianAcronym = "PPA")
        {
            // Open database connection as defined in configuration file "systemSettings" category
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<Device> deviceTable = new TableOperations<Device>(connection);
                TableOperations<MeasurementRecord> measurementTable = new TableOperations<MeasurementRecord>(connection);
                TableOperations<Historian> historianTable = new TableOperations<Historian>(connection);
                TableOperations<SignalTypeRecord> signalTypeTable = new TableOperations<SignalTypeRecord>(connection);

                // Lookup target device ID
                int? deviceID = currentDeviceID > 0 ? currentDeviceID : deviceTable.QueryRecordWhere("Acronym = {0}", Name)?.ID;

                // Lookup target historian ID
                int? historianID = historianTable.QueryRecordWhere("Acronym = {0}", targetHistorianAcronym)?.ID;

                // Lookup signal type ID
                int signalTypeID = signalTypeTable.QueryRecordWhere("Acronym = {0}", signalType.ToString())?.ID ?? 1;

                // Lookup measurement record by point tag, creating a new record if one does not exist
                MeasurementRecord measurement = measurementTable.QueryRecordWhere("SignalReference = {0}", signalReference) ?? measurementTable.NewRecord();

                // Update record fields
                measurement.DeviceID = deviceID;
                measurement.HistorianID = historianID;
                measurement.PointTag = pointTag;
                measurement.AlternateTag = alternateTag;
                measurement.SignalReference = signalReference;
                measurement.SignalTypeID = signalTypeID;
                measurement.Description = description;

                // Save record updates
                measurementTable.AddNewOrUpdateRecord(measurement);

                // Re-query new records to get any database assigned information, e.g., unique Guid-based signal ID
                if (measurement.PointID == 0)
                    measurement = measurementTable.QueryRecordWhere("SignalReference = {0}", signalReference);

                // Notify host system of configuration changes
                OnConfigurationChanged();

                return measurement;
            }
        }

        /// <summary>
        /// Stops the Synchroniced Operation to write to SQL and calls <see cref="ActionAdapterBase.Stop"/>
        /// </summary>
        public override void Stop()
        {
            m_cancelationTokenSql.Cancel();
            base.Stop();

        }


        #endregion
    }
}
