//******************************************************************************************************
//  SNRComputation.cs - Gbtc
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
//  10/02/2019 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

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
using GSF.Units;
using openHistorian.Model;
using PhasorProtocolAdapters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Measurement = GSF.TimeSeries.Measurement;
using MeasurementRecord = openHistorian.Model.Measurement;
using SignalType = GSF.Units.EE.SignalType;
using SignalTypeRecord = openHistorian.Model.SignalType;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Defines an Adapter that calculates Signal to Noise Ratios.
    /// </summary>
    [Description("SNR Computation: Computes Signal To Noise Ratios")]
    public class SNRComputation : CalculatedMeasurementBase
    {
        #region [ Members ]

        // Nested Types
        private class SNRDataWindow
        {
            public double  Sum;
            public double SquaredSum;
            public double Count;

            public void Reset() => Sum = SquaredSum = Count = 0;
        }

        /// <summary>
        /// Represents Set of input and output Measurement used for SNR computation
        /// </summary>
        public class SNRMapping
        {
            /// <summary>
            /// The Input Measurement Key
            /// </summary>
            public Guid InputKey { get; set; }
            /// <summary>
            /// The Output MeasurementKey
            /// </summary>
            public Guid OutputKey { get; set; }
        }

        /// <summary>
        /// Defines an entry for the SQL Database storing the Daily Summary
        /// Note that the DB takes care of aggregating through Triggers
        /// </summary>
        [TableName("SNR")]
        private class DailySummary
        {
            public DateTime Date { get; set; }
            public double SNR { get; set; }
            public Guid SignalID { get; set; }
        }

        // Constants

        private const string ReportSettingsCategory = "snrSQLReportingDB";

        /// <summary>
        /// Default value for <see cref="WindowLength"/>.
        /// </summary>
        public const int DefaultWindowLength = 30;

        /// <summary>
        /// Defines the default value for the <see cref="ParentDeviceAcronymTemplate"/>.
        /// </summary>
        public const string DefaultParentDeviceAcronymTemplate = "IAM!{0}";

        /// <summary>
        /// Defines the default value for the <see cref="SourceMeasurementTable"/>.
        /// </summary>
        public const string DefaultSourceMeasurementTable = "ActiveMeasurements";

        /// <summary>
        /// Defines the default value for the <see cref="CalculatedMeasurementBase.InputMeasurementKeys"/>.
        /// </summary>
        public const string DefaultInputMeasurementKeys = "FILTER ActiveMeasurements WHERE SignalType LIKE '%PH%' OR SignalType = 'FREQ'";

        /// <summary>
        /// Defines the default value for the <see cref="PointTagTemplate"/>.
        /// </summary>
        public const string DefaultPointTagTemplate = "SNR!{0}";

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

        // Fields
        private int m_numberOfFrames;
        private double m_lastSNR;

        //private bool m_countAlarms;
        private int m_sqlFailures;
        private int m_skipped;

        private ConcurrentDictionary<Guid, SNRDataWindow> m_dataWindow;
        private Dictionary<Guid, bool> m_isAngleMapping;
        private ConcurrentDictionary<Guid, MeasurementKey> m_outputMapping;
        private LongSynchronizedOperation m_sqlWritter;
        private IsolatedQueue<DailySummary> m_summaryQueue;
        private CancellationToken m_cancelationTokenSql;
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the reference for phase computation.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the reference angle for phase computation.")]
        [DefaultValue(null)]
        public MeasurementKey Reference { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of rows inserted into sql as single Transaction.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the maximum number of daily Summary Points inserted at once.")]
        [DefaultValue(DefaultMaxSQLRows)]
        public int MaxSQLRows { get; set; }

        /// <summary>
        /// Gets or sets the window length for the SNR computation.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the window length in frames used for SNR computation.")]
        [DefaultValue(DefaultWindowLength)]
        public int WindowLength { get; set; } = DefaultWindowLength;

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
        /// Gets or sets template for output measurement point tag names.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement point tag names, typically an expression like \"" + DefaultPointTagTemplate + "\" where \"{0}\" is substituted with this adapter name, a dash and then the PerAdapterOutputNames value for the current measurement. Note that \"{0}\" token is not required, property can be overridden to provide desired value.")]
        [DefaultValue(DefaultPointTagTemplate)]
        public virtual string PointTagTemplate { get; set; } = DefaultPointTagTemplate;

        /// <summary>
        /// Gets or sets template for output measurement signal reference names.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement signal reference names, typically an expression like \"" + DefaultSignalReferenceTemplate + "\" where \"{0}\" is substituted with this adapter name, a dash and then the PerAdapterOutputNames value for the current measurement. Note that \"{0}\" token is not required, property can be overridden to provide desired value.")]
        [DefaultValue(DefaultSignalReferenceTemplate)]
        public virtual string SignalReferenceTemplate { get; set; } = DefaultSignalReferenceTemplate;

        /// <summary>
        /// Gets or sets the source measurement table to use for configuration.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the source measurement table to use for configuration.")]
        [DefaultValue(DefaultSourceMeasurementTable)]
        public virtual string SourceMeasurementTable { get; set; } = DefaultSourceMeasurementTable;

        /// <summary>
        /// Gets or sets template for output measurement alternate tag names.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement alternate tag names, typically an expression where \"{0}\" is substituted with this adapter name, a dash and then the PerAdapterOutputNames value for the current measurement. Note that \"{0}\" token is not required, property can be overridden to provide desired value.")]
        [DefaultValue(DefaultAlternateTagTemplate)]
        public virtual string AlternateTagTemplate { get; set; } = DefaultAlternateTagTemplate;

        /// <summary>
        /// Gets or sets template for output measurement descriptions.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines template for output measurement descriptions, typically an expression like \"" + DefaultDescriptionTemplate + "\".")]
        [DefaultValue(DefaultDescriptionTemplate)]
        public virtual string DescriptionTemplate { get; set; } = DefaultDescriptionTemplate;

        /// <summary>
        /// Gets or sets the target historian acronym for output measurements.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the target historian acronym for output measurements.")]
        [DefaultValue(DefaultTargetHistorianAcronym)]
        public virtual string TargetHistorianAcronym { get; set; } = DefaultTargetHistorianAcronym;


        /// <summary>
        /// Gets or sets output measurements that the <see cref="SNRComputation"/> will produce, if any.
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
        /// Gets or sets the Voltage Threshold.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines The minimum Value (in V, A...) for computing SNR.")]
        [DefaultValue(0.1D)]
        public double NoiseThreshhold { get; set; }

        /// <summary>
        /// Gets a detailed status for this <see cref="SNRComputation"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.AppendFormat(" Calculation Window Length: {0:N0}", WindowLength);
                status.AppendLine();
                status.AppendFormat(" Last Calculated SNR Value: {0:N3}", m_lastSNR);
                status.AppendLine();
                status.AppendFormat("Configured Reference Angle: {0}", Reference?.ToString() ?? "Undefined");
                status.AppendLine();
                status.AppendFormat("Waiting to write Summaries to SQL: {0}", m_summaryQueue?.Count ?? 0);
                status.AppendLine();
                status.AppendFormat("Sequential SQL exceptions: {0}", m_sqlFailures);
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
        /// Initializes <see cref="SNRComputation"/>.
        /// </summary>
        public override void Initialize()
        {
            ParseConnectionString();

            base.Initialize();

            // Figure Output Measurement Keys
            m_dataWindow = new ConcurrentDictionary<Guid, SNRDataWindow>();
            m_outputMapping = new ConcurrentDictionary<Guid, MeasurementKey>();
            m_isAngleMapping = new Dictionary<Guid, bool>();

            m_isAngleMapping = InputMeasurementKeys
                .Zip(InputMeasurementKeyTypes, (key, signalType) => new { key, signalType })
                .ToDictionary(mapping => mapping.key.SignalID, mapping => mapping.signalType== SignalType.IPHA || mapping.signalType == SignalType.VPHA);

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

            // Create child adapter for provided inputs to the parent bulk collection-based adapter
            Parallel.For(0, InputMeasurementKeys.Length, i =>
            {
                Guid input;
                Guid output;

                // Adapter inputs are presumed to be grouped together
                input = InputMeasurementKeys[i].SignalID;

                string inputName = LookupPointTag(input, SourceMeasurementTable);
                string alternateTagPrefix = null;

                if (!string.IsNullOrWhiteSpace(AlternateTagTemplate))
                {
                    string deviceName = LookupDevice(input, SourceMeasurementTable);
                    string phasorLabel = LookupPhasorLabel(input, SourceMeasurementTable);
                    alternateTagPrefix = $"{deviceName}-{phasorLabel}";
                }

                // Setup output measurements for new child adapter

                // Check if an Output Model already exists
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    SNRMapping keys = new TableOperations<SNRMapping>(connection).QueryRecordWhere("InputKey = {0}", input);

                    if (keys is null)
                    {
                        string perAdapterOutputName = "RESULT";
                        string outputPrefix = $"{Name}!{inputName}-{perAdapterOutputName}";
                        string outputPointTag = string.Format(PointTagTemplate, outputPrefix);
                        string outputAlternateTag = string.Format(AlternateTagTemplate, alternateTagPrefix is null ? "" : $"{alternateTagPrefix}-{perAdapterOutputName}");
                        string signalReference = string.Format(SignalReferenceTemplate, outputPrefix);
                        string description = string.Format(DescriptionTemplate, outputPrefix, SignalType.CALC, Name, GetType().Name);

                        // Get output measurement record, creating a new one if needed
                        MeasurementRecord measurement = GetMeasurementRecord(currentDeviceID ?? CurrentDeviceID(i), outputPointTag, outputAlternateTag, signalReference, description, SignalType.CALC, TargetHistorianAcronym);

                           
                        output = measurement.SignalID;
                        
                        new TableOperations<SNRMapping>(connection).AddNewRecord(new SNRMapping() { InputKey = input, OutputKey = output });


                    }
                    else
                    {
                        output = keys.OutputKey;

                    }
                }
                // Add inputs and outputs to connection string settings for child adapter
                m_dataWindow.AddOrUpdate(input, new SNRDataWindow());
                m_dataWindow[input].Reset();
                m_outputMapping.AddOrUpdate(input, MeasurementKey.LookUpBySignalID(output));
            });

            m_numberOfFrames = 0;

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
            bool calcSNR = m_numberOfFrames >= WindowLength;

            List<IMeasurement> outputmeasurements = new List<IMeasurement>();

            double refAngle = 0.0D;

            if (Reference != null && frame.Measurements.TryGetValue(Reference, out IMeasurement referenceAngle))
            {
                refAngle = referenceAngle.AdjustedValue;
            }

            foreach (IMeasurement measurment in frame.Measurements.Values)
            {
                double value = measurment.AdjustedValue;
                Guid signalID = measurment.Key.SignalID;

                if (m_isAngleMapping[signalID])
                {
                    Angle[] angles = Angle.Unwrap(new Angle[] { value, refAngle }).ToArray();
                    value = angles[0] - angles[1];
                }

                m_dataWindow[signalID].Count += 1.0D;
                m_dataWindow[signalID].Sum += value;
                m_dataWindow[signalID].SquaredSum += value * value;

                if (calcSNR)
                {
                    double snr = CalculateSignalToNoise(m_dataWindow[signalID]);
                    if (m_dataWindow[signalID].Sum/ m_dataWindow[signalID].Count < NoiseThreshhold)
                    {
                        m_skipped++;
                        m_dataWindow[signalID].Reset();
                        continue;
                    }


                    m_lastSNR = snr;
                    m_dataWindow[signalID].Reset();
                    Measurement snrMeasurement = new Measurement { Metadata = m_outputMapping[signalID].Metadata };
                    outputmeasurements.Add(Measurement.Clone(snrMeasurement, snr, frame.Timestamp));

                    if (ReportSQL && !double.IsNaN(snr))
                        m_summaryQueue.Enqueue(new DailySummary() { SignalID = signalID, SNR = snr, Date = ((DateTime)frame.Timestamp).RoundDownToNearestDay() });
                }
            }

            if (!calcSNR)
                return;

            m_numberOfFrames = 0;
            OnNewMeasurements(outputmeasurements);
           
            
        }

        private double CalculateSignalToNoise(SNRDataWindow data)
        {
            if (data.Count < 1)
                return double.NaN;

            double sampleAverage = data.Sum / data.Count;
            double totalVariance = data.SquaredSum - 2.0D * sampleAverage * data.Sum + data.Count * sampleAverage * sampleAverage;
            double result = 10 * Math.Log10(Math.Abs(sampleAverage / Math.Sqrt(totalVariance / data.Count)));

            return double.IsInfinity(result) ? double.NaN : result;
        }

        private void SaveToSQL(CancellationToken CancellationToken)
        {
            while (!CancellationToken.IsCancelled)
            {
                DailySummary[] data = new DailySummary[MaxSQLRows];

                int n = m_summaryQueue.Dequeue(data, 0, MaxSQLRows);
                if ( n > 0)
                {
                    string sqlCmd = "INSERT INTO SNR (Date, SNR, SignalID) VALUES";

                    for (int i = 0; i < n; i++)
                        sqlCmd = sqlCmd + $"\n ('{data[i].Date}',{data[i].SNR},'{data[i].SignalID}'),";

                    sqlCmd = sqlCmd.Substring(0,sqlCmd.Length - 1);
                    using (AdoDataConnection connection = new AdoDataConnection(ReportSettingsCategory))
                        connection.ExecuteNonQuery(sqlCmd);
                }
                
            }
            
        }

        /// <summary>
        /// Gets associated device ID if any, for measurement generation.
        /// </summary>
        /// <param name="measurementIndex"> The measurement Index for which this device is</param>
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
                    DataRow record = DataSource.LookupMetadata(inputMeasurement.SignalID, SourceMeasurementTable);
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
        /// <param name="measurementTable">Measurement table name used for meta-data lookup.</param>
        /// <returns>Point tag name, if found; otherwise, string representation of provided signal ID.</returns>
        private string LookupPointTag(Guid signalID, string measurementTable = "ActiveMeasurements")
        {
            DataRow record = DataSource.LookupMetadata(signalID, measurementTable);
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
        /// <param name="measurementTable">Measurement table name used for meta-data lookup.</param>
        /// <returns>Device name, if found; otherwise, string representation of associated point tag.</returns>
        private string LookupDevice( Guid signalID, string measurementTable = "ActiveMeasurements")
        {
            DataRow record = DataSource.LookupMetadata(signalID, measurementTable);
            string device = null;

            if (record != null)
                device = record["Device"].ToString();

            if (string.IsNullOrWhiteSpace(device))
                device = LookupPointTag(signalID, measurementTable);

            return device.ToUpper();
        }

        /// <summary>
        /// Lookups up associated phasor label from provided <paramref name="signalID"/>.
        /// </summary>
        /// <param name="signalID"><see cref="Guid"/> signal ID to lookup.</param>
        /// <param name="measurementTable">Measurement table name used for meta-data lookup.</param>
        /// <returns>Phasor label name, if found; otherwise, string representation associated point tag.</returns>
        private string LookupPhasorLabel( Guid signalID, string measurementTable = "ActiveMeasurements")
        {
            DataRow record = DataSource.LookupMetadata(signalID, measurementTable);
            int phasorID = 0;

            if (record != null)
                phasorID = record.ConvertNullableField<int>("PhasorID") ?? 0;

            if (phasorID == 0)
                return LookupPointTag(signalID, measurementTable);

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<GSF.TimeSeries.Model.Phasor> phasorTable = new TableOperations<GSF.TimeSeries.Model.Phasor>(connection);
                GSF.TimeSeries.Model.Phasor phasorRecord = phasorTable.QueryRecordWhere("ID = {0}", phasorID);
                return phasorRecord is null ? LookupPointTag(signalID, measurementTable) : phasorRecord.Label.Trim().ToUpper();
            }
        }

        /// <summary>
        /// Parses connection string.
        /// </summary>
        private void ParseConnectionString()
        {
            Dictionary<string, string> settings = Settings;

            if (!settings.TryGetValue(nameof(InputMeasurementKeys), out string inputMeasurementKeys) || string.IsNullOrWhiteSpace(inputMeasurementKeys))
                settings[nameof(InputMeasurementKeys)] = DefaultInputMeasurementKeys;

            // Parse all properties marked with ConnectionStringParameterAttribute from provided ConnectionString value
            ConnectionStringParser parser = new ConnectionStringParser<ConnectionStringParameterAttribute>();
            parser.ParseConnectionString(ConnectionString, this);

            // Parse input measurement keys like class was a typical adapter
            if (Settings.TryGetValue(nameof(InputMeasurementKeys), out string setting))
                InputMeasurementKeys = AdapterBase.ParseInputMeasurementKeys(DataSource, true, setting, SourceMeasurementTable);

            // Parse output measurement keys like class was a typical adapter
            if (Settings.TryGetValue(nameof(OutputMeasurements), out setting))
                OutputMeasurements = AdapterBase.ParseOutputMeasurements(DataSource, true, setting, SourceMeasurementTable);

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
