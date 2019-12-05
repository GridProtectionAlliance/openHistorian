//******************************************************************************************************
//  TrendValueAPI.cs - Gbtc
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GSF.Collections;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.Identity;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.Units;
using PhasorProtocolAdapters;
using openHistorian.Model;
using Measurement = GSF.TimeSeries.Measurement;
using MeasurementRecord = openHistorian.Model.Measurement;
using SignalType = GSF.Units.EE.SignalType;
using SignalTypeRecord = openHistorian.Model.SignalType;

namespace openHistorian.Adapters
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    internal sealed class CalculatedMesaurementAttribute : Attribute
    {
    }

    /// <summary>
    /// Defines an Adapter that calculates Signal to Noise Ratios.
    /// </summary>
    [Description("SNR Computation: Computes Signal To Noise Ratios")]
    public class SNRComputation : CalculatedMeasurementBase
    {
        #region [ Members ]

        // Nested Types
        private class StatisticsCollection
        {
            public MeasurementKey Max;
            public MeasurementKey Min;
            public MeasurementKey Total;
            public MeasurementKey Sum;
            public MeasurementKey SqrD;
            public MeasurementKey Alert;

            public double Count;
            public double Maximum;
            public double Minimum;
            public double Summation;
            public double SquaredSummation;
            public int AlertCount;

            public void Reset()
            {
                Count = 0;
                Maximum = double.MinValue;
                Minimum = double.MaxValue;
                Summation = 0;
                SquaredSummation = 0;
                AlertCount = 0;
            }
        }

        // Constants

        /// <summary>
        /// Default value for <see cref="WindowLength"/>.
        /// </summary>
        public const int DefaultWindowLength = 30;

        /// <summary>
        /// Default value for <see cref="ReportingInterval"/>.
        /// </summary>
        public const int DefaultReportingInterval = 300;

        /// <summary>
        /// Default value for <see cref="ResultDeviceName"/>.
        /// </summary>
        public const string DefaultResultDeviceName = "SNR!SERVICE";

        /// <summary>
        /// Default value for <see cref="HistorianInstance"/>.
        /// </summary>
        public const string DefaultHistorian = "PPA";

        // Fields
        private int m_numberOfFrames;
        private Dictionary<Guid, List<double>> m_dataWindow;
        private List<double> m_refWindow;
        private Dictionary<Guid, MeasurementKey> m_outputMapping;
        private Dictionary<Guid, StatisticsCollection> m_statisticsMapping;
        private bool m_saveStats;
        private Guid m_nodeID;
        private double m_threshold;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the device acronym of the outputs
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurement]
        [Description("Defines Name of the Device used for output Measurements.")]
        [DefaultValue(DefaultResultDeviceName)]
        public string ResultDeviceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default historian instance used by the output measurements.
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurement]
        [Description("Defines the Historian Instance used by the output measurements. Specified by Acronym")]
        [DefaultValue(DefaultHistorian)]
        public string HistorianInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the flag to determine of aggregates are saved
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurement]
        [Description("Defines If aggregates are saved sepperately.")]
        [DefaultValue(false)]
        public bool SaveAggregates
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reporting Interval of the results
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurement]
        [Description("Defines the Reporting Interval of the Statistics.")]
        [DefaultValue(DefaultReportingInterval)]
        public int ReportingInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reference for Phase Computation
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurement]
        [Description("Defines Reference Angle for Phase SNRs.")]
        [DefaultValue(null)]
        public MeasurementKey Reference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default Device Acronym used if SNR measurements have to be generated
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurement]
        [Description("Defines the Windowlength in frames.")]
        [DefaultValue(DefaultWindowLength)]
        public int WindowLength
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a detailed status for this <see cref="SNRComputation"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.Append(base.Status);

                status.AppendFormat("Length of calculation window: {0}", WindowLength);
                status.AppendLine();
                status.AppendFormat("          Output Device Name: {0}", ResultDeviceName);
                status.AppendLine();

                if (m_saveStats)
                {
                    status.AppendFormat("          Reporting Interval: {0}", ReportingInterval);
                    status.AppendLine();
                }
                else
                {
                    status.AppendFormat("Statistics are not saved");
                    status.AppendLine();
                }
                
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
            new ConnectionStringParser<CalculatedMesaurementAttribute>().ParseConnectionString(ConnectionString, this);

            base.Initialize();

            // Figure Output Measurement Keys
            m_dataWindow = new Dictionary<Guid, List<double>>();
            m_refWindow = new List<double>();
            m_outputMapping = new Dictionary<Guid, MeasurementKey>();
            m_statisticsMapping = new Dictionary<Guid, StatisticsCollection>();
            m_saveStats = SaveAggregates;

            if (ReportingInterval % (double)WindowLength != 0.0D)
            {
                ReportingInterval = WindowLength * (ReportingInterval / WindowLength);
                OnStatusMessage(MessageLevel.Warning, $"Adjusting Reporting Interval to every {ReportingInterval} frames.");
            }

            CategorizedSettingsElementCollection reportSettings = ConfigurationFile.Current.Settings["reportSettings"];
            reportSettings.Add("DefaultSNRThreshold", "4.0", "Default SNR Alert threshold.");
            m_threshold = reportSettings["DefaultSNRThreshold"].ValueAs(m_threshold);

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<MeasurementRecord> measurementTable = new TableOperations<MeasurementRecord>(connection);
                TableOperations<Device> deviceTable = new TableOperations<Device>(connection);
                TableOperations<SignalTypeRecord> signalTable = new TableOperations<SignalTypeRecord>(connection);

                Device device = deviceTable.QueryRecordWhere("Acronym = {0}", ResultDeviceName);
                int historianID = Convert.ToInt32(connection.ExecuteScalar("SELECT ID FROM Historian WHERE Acronym = {0}", new object[] { HistorianInstance }));

                if (!InputMeasurementKeys.Any())
                    return;

                m_nodeID = deviceTable.QueryRecordWhere("Id={0}", measurementTable.QueryRecordWhere("SignalID = {0}", InputMeasurementKeys[0].SignalID).DeviceID).NodeID;

                if (device == null)
                {
                    device = CreateDefaultDevice(deviceTable);
                    OnStatusMessage(MessageLevel.Warning, $"Default Device for Output Measurments not found. Created Device {device.Acronym}");
                }

                foreach (MeasurementKey key in InputMeasurementKeys)
                {
                    m_dataWindow.Add(key.SignalID, new List<double>());

                    string outputReference = measurementTable.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR";

                    if (measurementTable.QueryRecordCountWhere("SignalReference = {0}", outputReference) > 0)
                    {
                        // Measurement Exists
                        m_outputMapping.Add(key.SignalID, MeasurementKey.LookUpBySignalID(
                            measurementTable.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID));
                    }
                    else
                    {
                        // Add Measurment to Database and make a statement
                        MeasurementRecord inMeasurement = measurementTable.QueryRecordWhere("SignalID = {0}", key.SignalID);
                        MeasurementRecord outMeasurement = new MeasurementRecord
                        {
                            HistorianID = historianID,
                            DeviceID = device.ID,
                            PointTag = inMeasurement.PointTag + "-SNR",
                            AlternateTag = inMeasurement.AlternateTag + "-SNR",
                            SignalTypeID = signalTable.QueryRecordWhere("Acronym = {0}", "CALC").ID,
                            SignalReference = outputReference,
                            Description = inMeasurement.Description + " SNR",
                            Enabled = true,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow,
                            CreatedBy = UserInfo.CurrentUserID,
                            UpdatedBy = UserInfo.CurrentUserID,
                            SignalID = Guid.NewGuid(),
                            Adder = 0.0D,
                            Multiplier = 1.0D
                        };

                        measurementTable.AddNewRecord(outMeasurement);

                        m_outputMapping.Add(key.SignalID, MeasurementKey.LookUpBySignalID(
                           measurementTable.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID));

                        OnStatusMessage(MessageLevel.Warning, $"Output measurment {outputReference} not found. Creating measurement");
                    }

                    if (m_saveStats)
                        m_statisticsMapping.Add(key.SignalID, CreateStatistics(measurementTable, key, device, historianID));
                }

                if (Reference == null)
                {
                    OnStatusMessage(MessageLevel.Warning, "No Reference Angle Specified");
                    
                    int refIndex = InputMeasurementKeyTypes.IndexOf(item => item == SignalType.IPHA || item == SignalType.VPHA);
                    
                    if (refIndex > -1)
                        Reference = InputMeasurementKeys[refIndex];
                }

                if (Reference != null)
                {
                    if (!InputMeasurementKeys.Contains(Reference))
                        InputMeasurementKeys.AddRange(new[] { Reference });
                }
            }
        }

        /// <summary>
        /// Publish frame of time-aligned collection of measurement values that arrived within the defined lag time.
        /// </summary>
        /// <param name="frame">Frame of measurements with the same timestamp that arrived within lag time that are ready for processing.</param>
        /// <param name="index">Index of frame within a second ranging from zero to frames per second - 1.</param>
        protected override void PublishFrame(IFrame frame, int index)
        {
            m_numberOfFrames++;

            MeasurementKey[] availableKeys = frame.Measurements.Values.MeasurementKeys();

            foreach (Guid key in m_dataWindow.Keys)
            {
                int keyIndex = availableKeys.IndexOf(item => item.SignalID == key);
                m_dataWindow[key].Add(keyIndex > -1 ? frame.Measurements[availableKeys[keyIndex]].Value : double.NaN);
            }

            if (Reference != null)
            {
                int keyIndex = availableKeys.IndexOf(item => item.SignalID == Reference.SignalID);
                m_refWindow.Add(keyIndex > -1 ? frame.Measurements[availableKeys[keyIndex]].Value : double.NaN);
            }

            int currentWindowLength = m_dataWindow.Keys.Select(key => m_dataWindow[key].Count).Max();

            if (currentWindowLength >= WindowLength)
            {
                List<IMeasurement> outputmeasurements = new List<IMeasurement>();

                List<Guid> Keys = m_dataWindow.Keys.ToList();
                foreach (Guid key in Keys)
                {
                    double snr = CalculateSignalToNoise(m_dataWindow[key], key);
                    Measurement outputMeasurement = new Measurement { Metadata = m_outputMapping[key].Metadata };

                    if (!double.IsNaN(snr) && m_saveStats)
                    {
                        m_statisticsMapping[key].Count++;

                        if (snr > m_statisticsMapping[key].Maximum)
                            m_statisticsMapping[key].Maximum = snr;
                        
                        if (snr < m_statisticsMapping[key].Minimum)
                            m_statisticsMapping[key].Minimum = snr;
                        
                        if (snr > m_threshold)
                            m_statisticsMapping[key].AlertCount++;
                        
                        m_statisticsMapping[key].Summation += snr;
                        m_statisticsMapping[key].SquaredSummation += snr * snr;
                    }

                    outputmeasurements.Add(Measurement.Clone(outputMeasurement, snr, frame.Timestamp));

                    m_dataWindow[key] = new List<double>();

                    if (!m_saveStats)
                        m_numberOfFrames = 0;
                }

                m_refWindow = new List<double>();

                // Reporting if neccesarry
                if (m_numberOfFrames >= ReportingInterval && m_saveStats)
                {
                    m_numberOfFrames = 0;

                    foreach (Guid key in Keys)
                    {
                        Measurement stateMeasurement = new Measurement { Metadata = m_statisticsMapping[key].Sum.Metadata };
                        outputmeasurements.Add(Measurement.Clone(stateMeasurement, m_statisticsMapping[key].Summation, frame.Timestamp));

                        stateMeasurement = new Measurement { Metadata = m_statisticsMapping[key].SqrD.Metadata };
                        outputmeasurements.Add(Measurement.Clone(stateMeasurement, m_statisticsMapping[key].SquaredSummation, frame.Timestamp));

                        stateMeasurement = new Measurement { Metadata = m_statisticsMapping[key].Min.Metadata };
                        outputmeasurements.Add(Measurement.Clone(stateMeasurement, m_statisticsMapping[key].Minimum, frame.Timestamp));

                        stateMeasurement = new Measurement { Metadata = m_statisticsMapping[key].Max.Metadata };
                        outputmeasurements.Add(Measurement.Clone(stateMeasurement, m_statisticsMapping[key].Maximum, frame.Timestamp));

                        stateMeasurement = new Measurement { Metadata = m_statisticsMapping[key].Total.Metadata };
                        outputmeasurements.Add(Measurement.Clone(stateMeasurement, m_statisticsMapping[key].Count, frame.Timestamp));

                        stateMeasurement = new Measurement { Metadata = m_statisticsMapping[key].Alert.Metadata };
                        outputmeasurements.Add(Measurement.Clone(stateMeasurement, m_statisticsMapping[key].AlertCount, frame.Timestamp));

                        m_statisticsMapping[key].Reset();
                    }
                }

                OnNewMeasurements(outputmeasurements);
            }
        }

        private double CalculateSignalToNoise(List<double> values, Guid key)
        {
            int sampleCount = values.Count;
            int keyIndex = InputMeasurementKeys.IndexOf(item => item.SignalID == key);
            SignalType keySignalType = InputMeasurementKeyTypes[keyIndex];

            // If its a Phase
            if ((keySignalType == SignalType.IPHA || keySignalType == SignalType.VPHA) && sampleCount == m_refWindow.Count && Reference != null)
                values = Angle.Unwrap(values.Select((item, index) => item - (Angle)m_refWindow[index]).ToArray()).Select(item => (double)item).ToList();

            values = values.Where(item => !double.IsNaN(item)).ToList();
            sampleCount = values.Count;

            if (sampleCount < 1)
                return double.NaN;

            double sampleAverage = values.Average();
            double totalVariance = values.Select(item => item - sampleAverage).Select(deviation => deviation * deviation).Sum();
            double result = 10 * Math.Log10(Math.Abs(sampleAverage / Math.Sqrt(totalVariance / sampleCount)));
            
            return double.IsInfinity(result) ? double.NaN : result;
        }

        private Device CreateDefaultDevice(TableOperations<Device> table)
        {
            int protocolID;

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<Protocol> protocolTable = new TableOperations<Protocol>(connection);
                protocolID = protocolTable.QueryRecordWhere("Acronym = {0}", "VirtualInput").ID;
            }

            Device result = new Device
            {
                Enabled = true,
                ProtocolID = protocolID,
                Name = "SNR Results",
                Acronym = ResultDeviceName,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                CreatedBy = UserInfo.CurrentUserID,
                UpdatedBy = UserInfo.CurrentUserID,
                UniqueID = Guid.NewGuid(),
                NodeID = m_nodeID
            };

            table.AddNewRecord(result);
            result = table.QueryRecordWhere("Acronym = {0}", ResultDeviceName);

            return result;
        }

        private StatisticsCollection CreateStatistics(TableOperations<MeasurementRecord> table, MeasurementKey key, Device device, int HistorianID)
        {
            MeasurementRecord inMeasurement = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            int signaltype;
            
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<SignalTypeRecord> signalTypeTable = new TableOperations<SignalTypeRecord>(connection);
                signaltype = signalTypeTable.QueryRecordWhere("Acronym = {0}", "CALC").ID;
            }

            string outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:SUM";

            // Sum
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-SNR:SUM",
                    AlternateTag = inMeasurement.AlternateTag + "-SNR:SUM",
                    SignalTypeID = signaltype,
                    SignalReference = outputReference,
                    Description = inMeasurement.Description + " Summ of SNR",
                    Enabled = true,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CreatedBy = UserInfo.CurrentUserID,
                    UpdatedBy = UserInfo.CurrentUserID,
                    SignalID = Guid.NewGuid(),
                    Adder = 0.0D,
                    Multiplier = 1.0D
                });
            }

            // sqrdSum
            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:SQR";
            
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-SNR:SQR",
                    AlternateTag = inMeasurement.AlternateTag + "-SNR:SQR",
                    SignalTypeID = signaltype,
                    SignalReference = outputReference,
                    Description = inMeasurement.Description + " Summ of Sqared SNR",
                    Enabled = true,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CreatedBy = UserInfo.CurrentUserID,
                    UpdatedBy = UserInfo.CurrentUserID,
                    SignalID = Guid.NewGuid(),
                    Adder = 0.0D,
                    Multiplier = 1.0D
                });
            }

            // Min
            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:MIN";
            
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-SNR:MIN",
                    AlternateTag = inMeasurement.AlternateTag + "-SNR:MIN",
                    SignalTypeID = signaltype,
                    SignalReference = outputReference,
                    Description = inMeasurement.Description + " Minimum SNR",
                    Enabled = true,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CreatedBy = UserInfo.CurrentUserID,
                    UpdatedBy = UserInfo.CurrentUserID,
                    SignalID = Guid.NewGuid(),
                    Adder = 0.0D,
                    Multiplier = 1.0D
                });
            }

            // Max
            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:MAX";
            
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-SNR:MAX",
                    AlternateTag = inMeasurement.AlternateTag + "-SNR:MAX",
                    SignalTypeID = signaltype,
                    SignalReference = outputReference,
                    Description = inMeasurement.Description + " Maximum SNR",
                    Enabled = true,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CreatedBy = UserInfo.CurrentUserID,
                    UpdatedBy = UserInfo.CurrentUserID,
                    SignalID = Guid.NewGuid(),
                    Adder = 0.0D,
                    Multiplier = 1.0D
                });
            }

            // Number of Points
            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:NUM";
            
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-SNR:NUM",
                    AlternateTag = inMeasurement.AlternateTag + "-SNR:NUM",
                    SignalTypeID = signaltype,
                    SignalReference = outputReference,
                    Description = inMeasurement.Description + " Number of Points",
                    Enabled = true,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CreatedBy = UserInfo.CurrentUserID,
                    UpdatedBy = UserInfo.CurrentUserID,
                    SignalID = Guid.NewGuid(),
                    Adder = 0.0D,
                    Multiplier = 1.0D
                });
            }

            // Number of Points above Alert 
            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:ALT";
            
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-SNR:ALT",
                    AlternateTag = inMeasurement.AlternateTag + "-SNR:ALT",
                    SignalTypeID = signaltype,
                    SignalReference = outputReference,
                    Description = inMeasurement.Description + " number of Alerts",
                    Enabled = true,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CreatedBy = UserInfo.CurrentUserID,
                    UpdatedBy = UserInfo.CurrentUserID,
                    SignalID = Guid.NewGuid(),
                    Adder = 0.0D,
                    Multiplier = 1.0D
                });
            }

            StatisticsCollection result = new StatisticsCollection();

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:SUM";
            result.Sum = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:SQR";
            result.SqrD = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:MIN";
            result.Min = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:MAX";
            result.Max = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:NUM";
            result.Total = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:ALT";
            result.Alert = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            result.Reset();
            return result;
        }

        #endregion
    }
}
