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
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using PhasorProtocolAdapters;
using openHistorian.Model;
using GSF.Identity;
using Measurement = GSF.TimeSeries.Measurement;
using MeasurementRecord = openHistorian.Model.Measurement;
using SignalType = GSF.Units.EE.SignalType;
using SignalTypeRecord = openHistorian.Model.SignalType;

// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
namespace openHistorian.Adapters
{
    /// <summary>
    /// Defines an Adapter that calculates Unbalance.
    /// </summary>
    [Description("Unbalanced Computation: Computes Unbalance of the voltage and currents")]
    public class UnBalancedCalculation : CalculatedMeasurementBase
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

        private class ThreePhaseSet
        {
            public readonly MeasurementKey Positive;
            public readonly MeasurementKey Negative;
            public readonly MeasurementKey Zero;
            public readonly MeasurementKey OutMapping;
            public readonly double Threshold;

            public ThreePhaseSet(MeasurementKey pos, MeasurementKey zero, MeasurementKey neg, MeasurementKey unbalance, double threshold)
            {
                Positive = pos;
                Negative = neg;
                Zero = zero;
                OutMapping = unbalance;
                Threshold = threshold;
            }
        }

        // Constants

        /// <summary>
        /// Default value for <see cref="ReportingInterval"/>.
        /// </summary>
        public const int DefaultReportingInterval = 300;

        /// <summary>
        /// Default value for <see cref="ResultDeviceName"/>.
        /// </summary>
        public const string DefaultResultDeviceName = "UBAl!SERVICE";

        /// <summary>
        /// Default value for <see cref="HistorianInstance"/>.
        /// </summary>
        public const string DefaultHistorian = "PPA";

        // Fields
        private int m_numberOfFrames;
        private Guid m_nodeID;
        private List<ThreePhaseSet> m_threePhaseComponent;
        private Dictionary<Guid, StatisticsCollection> m_statisticsMapping;
        private bool m_saveStats;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the flag to determine of aggregates are saved
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurement]
        [Description("Defines if aggregates are saved sepperately.")]
        [DefaultValue(false)]
        public bool SaveAggregates
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default historian instance used by the output measurements
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
        /// Gets or sets the default Device Acronym used if SNR measurements have to be generated
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurement]
        [Description("Defines the Device Name Acronym.")]
        [DefaultValue(DefaultResultDeviceName)]
        public string ResultDeviceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a detailed status for this <see cref="UnBalancedCalculation"/>.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.Append(base.Status);

                status.AppendFormat("  Default Output Device Name: {0}", ResultDeviceName);
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
        /// Initializes <see cref="UnBalancedCalculation"/>.
        /// </summary>
        public override void Initialize()
        {
            new ConnectionStringParser<CalculatedMesaurementAttribute>().ParseConnectionString(ConnectionString, this);

            base.Initialize();

            m_threePhaseComponent = new List<ThreePhaseSet>();
            m_statisticsMapping = new Dictionary<Guid, StatisticsCollection>();
            m_saveStats = SaveAggregates;

            CategorizedSettingsElementCollection reportSettings = ConfigurationFile.Current.Settings["reportSettings"];
            reportSettings.Add("IUnbalanceThreshold", "4.0", "Current Unbalance Alert threshold.");
            reportSettings.Add("VUnbalanceThreshold", "4.0", "Voltage Unbalance Alert threshold.");
            double i_threshold = reportSettings["IUnbalanceThreshold"].ValueAs(1.0D);
            double v_threshold = reportSettings["VUnbalanceThreshold"].ValueAs(1.0D);
            List<Guid> processed = new List<Guid>();

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<MeasurementRecord> measurementTable = new TableOperations<MeasurementRecord>(connection);
                TableOperations<Device> deviceTable = new TableOperations<Device>(connection);
                TableOperations<SignalTypeRecord> signalTable = new TableOperations<SignalTypeRecord>(connection);

                Device device = deviceTable.QueryRecordWhere("Acronym = {0}", ResultDeviceName);
                int historianID = Convert.ToInt32(connection.ExecuteScalar("SELECT ID FROM Historian WHERE Acronym = {0}", new object[] { HistorianInstance }));

                // Take Care of the Device
                if (InputMeasurementKeys.Length < 1)
                    return;

                m_nodeID = deviceTable.QueryRecordWhere("Id={0}", measurementTable.QueryRecordWhere("SignalID = {0}", InputMeasurementKeys[0].SignalID).DeviceID).NodeID;

                if (device == null)
                {
                    device = CreateDefaultDevice(deviceTable);
                    OnStatusMessage(MessageLevel.Warning, $"Default Device for Output Measurments not found. Created Device {device.Acronym}");
                }

                foreach (MeasurementKey key in InputMeasurementKeys)
                {
                    if (processed.Contains(key.SignalID))
                        continue;

                    string description = GetDescription(key, new TableOperations<ActiveMeasurement>(connection));

                    MeasurementKey neg = InputMeasurementKeys.FirstOrDefault(item => GetDescription(item, new TableOperations<ActiveMeasurement>(connection)) == description && SearchNegative(item, new TableOperations<ActiveMeasurement>(connection)));
                    MeasurementKey pos = InputMeasurementKeys.FirstOrDefault(item => GetDescription(item, new TableOperations<ActiveMeasurement>(connection)) == description && SearchPositive(item, new TableOperations<ActiveMeasurement>(connection)));
                    MeasurementKey zero = InputMeasurementKeys.FirstOrDefault(item => GetDescription(item, new TableOperations<ActiveMeasurement>(connection)) == description && SearchZero(item, new TableOperations<ActiveMeasurement>(connection)));

                    if (neg != null && zero != null && pos != null)
                    {
                        MeasurementKey unBalance;
                        string outputReference = measurementTable.QueryRecordWhere("SignalID = {0}", pos.SignalID).SignalReference + "-UBAL";

                        if (measurementTable.QueryRecordCountWhere("SignalReference = {0}", outputReference) > 0)
                        {
                            // Measurement Exists
                            unBalance = MeasurementKey.LookUpBySignalID(measurementTable.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);
                        }
                        else
                        {
                            // Add Measurment to Database and make a statement
                            MeasurementRecord inMeasurement = measurementTable.QueryRecordWhere("SignalID = {0}", pos.SignalID);

                            MeasurementRecord outMeasurement = new MeasurementRecord
                            {
                                HistorianID = historianID,
                                DeviceID = device.ID,
                                PointTag = inMeasurement.PointTag + "-UBAL",
                                AlternateTag = inMeasurement.AlternateTag + "-UBAL",
                                SignalTypeID = signalTable.QueryRecordWhere("Acronym = {0}", "CALC").ID,
                                SignalReference = outputReference,
                                Description = GetDescription(pos, new TableOperations<ActiveMeasurement>(connection)) + " UnBalanced",
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
                            unBalance = MeasurementKey.LookUpBySignalID(measurementTable.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

                            OnStatusMessage(MessageLevel.Warning, $"Output measurment {outputReference} not found. Creating measurement");
                        }

                        double threshold;

                        if (InputMeasurementKeyTypes[InputMeasurementKeys.IndexOf(item => item == key)] == SignalType.IPHM)
                            threshold = i_threshold;
                        else
                            threshold = v_threshold;

                        m_threePhaseComponent.Add(new ThreePhaseSet(pos, zero, neg, unBalance, threshold));

                        processed.Add(pos.SignalID);
                        processed.Add(neg.SignalID);
                        processed.Add(zero.SignalID);

                        if (m_saveStats)
                            m_statisticsMapping.Add(key.SignalID, CreateStatistics(measurementTable, key, device, historianID));
                    }

                    else
                    {
                        if (pos != null)
                            processed.Add(pos.SignalID);

                        if (neg != null)
                            processed.Add(neg.SignalID);

                        if (zero != null)
                            processed.Add(zero.SignalID);
                    }
                }

                if (m_threePhaseComponent.Count == 0)
                    OnStatusMessage(MessageLevel.Error, "No case with all 3 sequemnces was found");
            }
        }

        /// <summary>
        /// Publish frame of time-aligned collection of measurement values that arrived within the defined lag time.
        /// </summary>
        /// <param name="frame">Frame of measurements with the same timestamp that arrived within lag time that are ready for processing.</param>
        /// <param name="index">Index of frame within a second ranging from zero to frames per second - 1.</param>
        protected override void PublishFrame(IFrame frame, int index)
        {
            IMeasurement[] available = frame.Measurements.Values.ToArray();
            List<Guid> availableGuids = available.Select(item => item.Key.SignalID).ToList();
            List<IMeasurement> outputmeasurements = new List<IMeasurement>();

            foreach (ThreePhaseSet set in m_threePhaseComponent)
            {
                bool hasP = availableGuids.Contains(set.Positive.SignalID);
                bool hasN = availableGuids.Contains(set.Negative.SignalID);
                //bool hasZ = availableGuids.Contains(set.Zero.SignalID);

                if (hasP && hasN)
                {
                    double v1p = available.FirstOrDefault(item => (item.Key?.SignalID ?? Guid.Empty) == set.Positive.SignalID)?.Value ?? 0.0D;
                    double v2n = available.FirstOrDefault(item => (item.Key?.SignalID ?? Guid.Empty) == set.Negative.SignalID)?.Value ?? 0.0D;
                    
                    if (v1p != 0.0D)
                    {
                        double unbalanced = v2n / v1p;
                        Measurement outputMeasurement = new Measurement { Metadata = set.OutMapping.Metadata };

                        if (m_saveStats)
                        {
                            m_statisticsMapping[set.Positive.SignalID].Count++;

                            if (unbalanced > m_statisticsMapping[set.Positive.SignalID].Maximum)
                                m_statisticsMapping[set.Positive.SignalID].Maximum = unbalanced;

                            if (unbalanced < m_statisticsMapping[set.Positive.SignalID].Minimum)
                                m_statisticsMapping[set.Positive.SignalID].Minimum = unbalanced;

                            if (unbalanced > set.Threshold)
                                m_statisticsMapping[set.Positive.SignalID].AlertCount++;

                            m_statisticsMapping[set.Positive.SignalID].Summation += unbalanced;
                            m_statisticsMapping[set.Positive.SignalID].SquaredSummation += unbalanced * unbalanced;
                        }

                        outputmeasurements.Add(Measurement.Clone(outputMeasurement, unbalanced * 100.0D, frame.Timestamp));
                    }
                }

                if (!m_saveStats)
                    m_numberOfFrames = 0;
            }

            // Reporting if neccesarry
            if (m_numberOfFrames >= ReportingInterval && m_saveStats)
            {
                foreach (ThreePhaseSet set in m_threePhaseComponent)
                {
                    Guid key = set.Positive.SignalID;

                    Measurement statisticMeasurement = new Measurement { Metadata = m_statisticsMapping[key].Sum.Metadata };
                    outputmeasurements.Add(Measurement.Clone(statisticMeasurement, m_statisticsMapping[key].Summation, frame.Timestamp));

                    statisticMeasurement = new Measurement { Metadata = m_statisticsMapping[key].SqrD.Metadata };
                    outputmeasurements.Add(Measurement.Clone(statisticMeasurement, m_statisticsMapping[key].SquaredSummation, frame.Timestamp));

                    statisticMeasurement = new Measurement { Metadata = m_statisticsMapping[key].Min.Metadata };
                    outputmeasurements.Add(Measurement.Clone(statisticMeasurement, m_statisticsMapping[key].Minimum, frame.Timestamp));

                    statisticMeasurement = new Measurement { Metadata = m_statisticsMapping[key].Max.Metadata };
                    outputmeasurements.Add(Measurement.Clone(statisticMeasurement, m_statisticsMapping[key].Maximum, frame.Timestamp));

                    statisticMeasurement = new Measurement { Metadata = m_statisticsMapping[key].Total.Metadata };
                    outputmeasurements.Add(Measurement.Clone(statisticMeasurement, m_statisticsMapping[key].Count, frame.Timestamp));

                    statisticMeasurement = new Measurement { Metadata = m_statisticsMapping[key].Alert.Metadata };
                    outputmeasurements.Add(Measurement.Clone(statisticMeasurement, m_statisticsMapping[key].AlertCount, frame.Timestamp));

                    m_statisticsMapping[key].Reset();
                    m_numberOfFrames = 0;
                }
            }

            OnNewMeasurements(outputmeasurements);
        }

        private Device CreateDefaultDevice(TableOperations<Device> table)
        {
            int protocolId;

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<Protocol> protocolTable = new TableOperations<Protocol>(connection);
                protocolId = protocolTable.QueryRecordWhere("Acronym = {0}", "VirtualInput").ID;
            }

            Device result = new Device
            {
                Enabled = true,
                ProtocolID = protocolId,
                Name = "Unbalanced Results",
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

        private string GetDescription(MeasurementKey key, TableOperations<ActiveMeasurement> table)
        {
            ActiveMeasurement measurement = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            string text = measurement.Description;
            string stopAt = measurement.PhasorType == 'I' ? "-I" : "-V";

            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                    return text.Substring(0, charLocation);
            }

            return string.Empty;
        }

        private bool SearchNegative(MeasurementKey key, TableOperations<ActiveMeasurement> table)
        {
            ActiveMeasurement measurement = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            return measurement.Phase == '-';
        }

        private bool SearchPositive(MeasurementKey key, TableOperations<ActiveMeasurement> table)
        {
            ActiveMeasurement measurement = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            return measurement.Phase == '+';
        }

        private bool SearchZero(MeasurementKey key, TableOperations<ActiveMeasurement> table)
        {
            ActiveMeasurement measurement = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            return measurement.Phase == '0';
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

            string outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:SUM";

            // Sum
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-UBAL:SUM",
                    AlternateTag = inMeasurement.AlternateTag + "-UBAL:SUM",
                    SignalTypeID = signaltype,
                    SignalReference = outputReference,
                    Description = inMeasurement.Description + " Summ of UBAL",
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
            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:SQR";

            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-UBAL:SQR",
                    AlternateTag = inMeasurement.AlternateTag + "-UBAL:SQR",
                    SignalTypeID = signaltype,
                    SignalReference = outputReference,
                    Description = inMeasurement.Description + " Summ of Sqared UBAL",
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
            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:MIN";

            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-UBAL:MIN",
                    AlternateTag = inMeasurement.AlternateTag + "-UBAL:MIN",
                    SignalTypeID = signaltype,
                    SignalReference = outputReference,
                    Description = inMeasurement.Description + " Minimum UBAL",
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
            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:MAX";

            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-UBAL:MAX",
                    AlternateTag = inMeasurement.AlternateTag + "-UBAL:MAX",
                    SignalTypeID = signaltype,
                    SignalReference = outputReference,
                    Description = inMeasurement.Description + " Maximum UBAL",
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
            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:NUM";

            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-UBAL:NUM",
                    AlternateTag = inMeasurement.AlternateTag + "-UBAL:NUM",
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
            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:ALT";

            if (table.QueryRecordCountWhere("SignalReference = {0}", outputReference) < 1)
            {
                table.AddNewRecord(new MeasurementRecord
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeasurement.PointTag + "-UBAL:ALT",
                    AlternateTag = inMeasurement.AlternateTag + "-UBAL:ALT",
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

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:SUM";
            result.Sum = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:SQR";
            result.SqrD = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:MIN";
            result.Min = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:MAX";
            result.Max = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:NUM";
            result.Total = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            outputReference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:ALT";
            result.Alert = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputReference).SignalID);

            result.Reset();
            return result;
        }

        #endregion
    }
}
