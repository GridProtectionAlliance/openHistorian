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
using System.Data;
using System.Linq;
using System.Text;
using GSF;
using GSF.Collections;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Threading;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using PhasorProtocolAdapters;
using openHistorian.Model;
using ConnectionStringParser = GSF.Configuration.ConnectionStringParser<GSF.TimeSeries.Adapters.ConnectionStringParameterAttribute>;
using GSF.Identity;

namespace openHistorian.Adapters
{
        /// <summary>
        /// Defines an Adapter that calculates Unbalance.
        /// </summary>
        [Description("Unbalanced Computation: Computes Unbalance of the voltage and currents")]
    public class UnBalancedCalculation : CalculatedMeasurementBase
    {

        #region[InternalClasses]

        private class StatisticsCollection
        {
            public MeasurementKey max;
            public MeasurementKey min;
            public MeasurementKey total;
            public MeasurementKey sum;
            public MeasurementKey sqrd;
            public MeasurementKey alert;

            public double count;
            public double maximum;
            public double minimum;
            public double summation;
            public double squaredsummation;
            public int alertcount;

            public void Reset()
            {
                this.count = 0;
                this.maximum = double.MinValue;
                this.minimum = double.MaxValue;
                this.summation = 0;
                this.squaredsummation = 0;
                this.alertcount = 0;
            }
        }

        private class ThreePhaseSet
        {
            public MeasurementKey positive;
            public MeasurementKey negative;
            public MeasurementKey zero;
            public MeasurementKey outMapping;

            public double threshold;
            public ThreePhaseSet(MeasurementKey pos, MeasurementKey zero, MeasurementKey neg, MeasurementKey unbalance, double threshold)
            {
                this.positive = pos;
                this.negative = neg;
                this.zero = zero;
                this.outMapping = unbalance;
                this.threshold = threshold;

            }
        }

        #endregion[InternalClasses]


        #region [ Members ]

        /// <summary>
        /// Default value for <see cref="ReportingIntervall"/>.
        /// </summary>
        public const int DefaultReportingIntervall = 300;

        /// <summary>
        /// Default value for <see cref="ResultDeviceName"/>.
        /// </summary>
        public const string DefaultResultDeviceName = "UBAl!SERVICE";

		/// <summary>
		/// Default value for <see cref="HistorianInstance"/>.
		/// </summary>
		public const string DefaultHistorian = "PPA";

		private int numberOfFrames;
        private Guid nodeID;
        private List<ThreePhaseSet> threePhaseComponent;
        private Dictionary<Guid, StatisticsCollection> statisticsMapping;
        private bool saveStats;


        #endregion [ Members ]

        #region[Properties]

        /// <summary>
        /// Gets or sets the flag to determine of aggregates are saved
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurementAttribute]
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
		[CalculatedMesaurementAttribute]
		[Description("Defines the Historian Instance used by the output measurements. Specified by Acronym")]
		[DefaultValue(DefaultHistorian)]
		public string HistorianInstance
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the reporting Intervall of the results
		/// </summary>
		[ConnectionStringParameter]
        [CalculatedMesaurementAttribute]
        [Description("Defines the Reporting Intervall of the Statistics.")]
        [DefaultValue(DefaultReportingIntervall)]
        public int ReportingIntervall
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the default Device Acronym used if SNR measurements have to be generated
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurementAttribute]
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
                if (this.saveStats)
                {
                    status.AppendFormat("         Reporting Intervall: {0}", ReportingIntervall);
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

        #endregion[Properties]

        #region[Method]

        /// <summary>
        /// Initializes <see cref="UnBalancedCalculation"/>.
        /// </summary>
        public override void Initialize()
        {
            new GSF.Configuration.ConnectionStringParser<CalculatedMesaurementAttribute>().ParseConnectionString(ConnectionString, this);

            
            base.Initialize();

           
            this.threePhaseComponent = new List<ThreePhaseSet>();
            this.statisticsMapping = new Dictionary<Guid, StatisticsCollection>();
            this.saveStats = this.SaveAggregates;

            CategorizedSettingsElementCollection reportSettings = ConfigurationFile.Current.Settings["reportSettings"];
            reportSettings.Add("IUnbalanceThreshold", "4.0", "Current Unbalance Alert threshold.");
            reportSettings.Add("VUnbalanceThreshold", "4.0", "Voltage Unbalance Alert threshold.");
            double Ithreshold = reportSettings["IUnbalanceThreshold"].ValueAs(1.0D);
            double Vthreshold = reportSettings["VUnbalanceThreshold"].ValueAs(1.0D);

            List<Guid> processed = new List<Guid>();

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<openHistorian.Model.Measurement> measTable = new GSF.Data.Model.TableOperations<openHistorian.Model.Measurement>(connection);
                GSF.Data.Model.TableOperations<openHistorian.Model.Device> deviceTable = new GSF.Data.Model.TableOperations<openHistorian.Model.Device>(connection);
                GSF.Data.Model.TableOperations<openHistorian.Model.SignalType> signalTable = new GSF.Data.Model.TableOperations<openHistorian.Model.SignalType>(connection);

                openHistorian.Model.Device device = deviceTable.QueryRecordWhere("Acronym = {0}", ResultDeviceName);

				int HistorianID = Convert.ToInt32(connection.ExecuteScalar("SELECT ID FROM Historian WHERE Acronym = {0}", new object[1] { this.HistorianInstance }));

				// Take Care of the Device
				if ( this.InputMeasurementKeys.Count() < 1)
                {
                    return;
                }

                this.nodeID = deviceTable.QueryRecordWhere("Id={0}", measTable.QueryRecordWhere("SignalID = {0}", this.InputMeasurementKeys[0].SignalID).DeviceID).NodeID;

                if (device == null)
                {
                    device = CreateDefaultDevice(deviceTable);
                    OnStatusMessage(GSF.Diagnostics.MessageLevel.Warning, String.Format("Default Device for Output Measurments not found. Created Device {0}",device.Acronym));
                }

                foreach (MeasurementKey key in this.InputMeasurementKeys)
                {
                    if (processed.Contains(key.SignalID))
                        continue;

                    string describtion = GetDescribtion(key, new GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement>(connection));

                    MeasurementKey neg = this.InputMeasurementKeys.Where(item =>
                        GetDescribtion(item, new GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement>(connection)) == describtion
                        && SearchNegative(item, new GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement>(connection))).FirstOrDefault();

                    MeasurementKey pos = this.InputMeasurementKeys.Where(item =>
                       GetDescribtion(item, new GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement>(connection)) == describtion
                       && SearchPositive(item, new GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement>(connection))).FirstOrDefault();

                    MeasurementKey zero = this.InputMeasurementKeys.Where(item =>
                       GetDescribtion(item, new GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement>(connection)) == describtion
                       && SearchZero(item, new GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement>(connection))).FirstOrDefault();


                    if ((neg != null && zero != null && pos != null))
                    {
                        MeasurementKey unBalance;
                        string outputRefference = measTable.QueryRecordWhere("SignalID = {0}", pos.SignalID).SignalReference + "-UBAL";

                        if (measTable.QueryRecordCountWhere("SignalReference = {0}", outputRefference) > 0)
                        {
                            //Measurement Exists
                            unBalance = MeasurementKey.LookUpBySignalID(measTable.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID); 
                        }
                        else
                        {
                            //Add Measurment to Database and make a statement
                            openHistorian.Model.Measurement inMeas = measTable.QueryRecordWhere("SignalID = {0}", pos.SignalID);
                            openHistorian.Model.Measurement outMeas = new openHistorian.Model.Measurement()
                            {
                                HistorianID = HistorianID,
                                DeviceID = device.ID,
                                PointTag = inMeas.PointTag + "-UBAL",
                                AlternateTag = inMeas.AlternateTag + "-UBAL",
                                SignalTypeID = signalTable.QueryRecordWhere("Acronym = {0}", "CALC").ID,
                                SignalReference = outputRefference,
                                Description = GetDescribtion(pos, new GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement>(connection)) + " UnBalanced",
                                Enabled = true,
                                CreatedOn = DateTime.UtcNow,
                                UpdatedOn = DateTime.UtcNow,
                                CreatedBy = UserInfo.CurrentUserID,
                                UpdatedBy = UserInfo.CurrentUserID,
                                SignalID = Guid.NewGuid(),
                                Adder = 0.0D,
                                Multiplier = 1.0D
                            };
                            measTable.AddNewRecord(outMeas);
                            unBalance = MeasurementKey.LookUpBySignalID(measTable.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

                            OnStatusMessage(GSF.Diagnostics.MessageLevel.Warning, String.Format("Output measurment {0} not found. Creating measurement", outputRefference));
                        }

                        double threshold = 0;
                        if (this.InputMeasurementKeyTypes[this.InputMeasurementKeys.IndexOf(item => item == key)] == GSF.Units.EE.SignalType.IPHM)
                            threshold = Ithreshold;
                        else
                            threshold = Vthreshold;

                        this.threePhaseComponent.Add(new ThreePhaseSet(pos, zero, neg, unBalance, threshold));
                        processed.Add(pos.SignalID);
                        processed.Add(neg.SignalID);
                        processed.Add(zero.SignalID);

                        if (this.saveStats)
                            this.statisticsMapping.Add(key.SignalID, CreateStatistics(measTable, key, device, HistorianID));

                    }

                    else
                    {
                        if (pos != null)
                        {
                            processed.Add(pos.SignalID);
                        }
                        if (neg != null)
                        {
                            processed.Add(neg.SignalID);
                        }
                        if (zero != null)
                        {
                            processed.Add(zero.SignalID);
                        }
                    }

                }

                if (this.threePhaseComponent.Count() == 0)
                {
                    OnStatusMessage(GSF.Diagnostics.MessageLevel.Error, "No case with all 3 sequemnces was found");
                    return;
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
            IMeasurement[] available = frame.Measurements.Values.ToArray();

            List<Guid> availableGuids = available.Select(item => item.Key.SignalID).ToList();

            List<IMeasurement> outputmeasurements = new List<IMeasurement>();

            foreach (ThreePhaseSet set in this.threePhaseComponent)
            {
                bool hasP=availableGuids.Contains(set.positive.SignalID);
                bool hasN = availableGuids.Contains(set.negative.SignalID);
                bool hasZ = availableGuids.Contains(set.zero.SignalID);

                if (hasP && hasN)
                {
                    double v1p = available.Where(item => item.Key.SignalID == set.positive.SignalID).FirstOrDefault().Value;
                    double v2n = available.Where(item => item.Key.SignalID == set.negative.SignalID).FirstOrDefault().Value;
                    if (v1p != 0.0F)
                    {
                        double unbalanced = v2n / v1p;
                        GSF.TimeSeries.Measurement outmeas = new GSF.TimeSeries.Measurement();
                        outmeas.Metadata = set.outMapping.Metadata;

                        if (this.saveStats)
                        {
                            this.statisticsMapping[set.positive.SignalID].count++;
                            if (unbalanced > this.statisticsMapping[set.positive.SignalID].maximum)
                                this.statisticsMapping[set.positive.SignalID].maximum = unbalanced;
                            if (unbalanced < this.statisticsMapping[set.positive.SignalID].minimum)
                                this.statisticsMapping[set.positive.SignalID].minimum = unbalanced;
                            if (unbalanced > set.threshold)
                                this.statisticsMapping[set.positive.SignalID].alertcount++;
                            this.statisticsMapping[set.positive.SignalID].summation += unbalanced;
                            this.statisticsMapping[set.positive.SignalID].squaredsummation += (unbalanced * unbalanced);
                        }



                        outputmeasurements.Add(GSF.TimeSeries.Measurement.Clone(outmeas, unbalanced*100.0D, frame.Timestamp));
                    }
                }
                if (!this.saveStats)
                    numberOfFrames = 0;
            }

            
            // Reporting if neccesarry
            if ((numberOfFrames >= this.ReportingIntervall) && (this.saveStats))
            {
                foreach (ThreePhaseSet set in this.threePhaseComponent)
                {
                    Guid key = set.positive.SignalID;
                       
                    GSF.TimeSeries.Measurement statmeas = new GSF.TimeSeries.Measurement();
                    statmeas.Metadata = this.statisticsMapping[key].sum.Metadata;
                    outputmeasurements.Add(GSF.TimeSeries.Measurement.Clone(statmeas, this.statisticsMapping[key].summation, frame.Timestamp));

                    statmeas = new GSF.TimeSeries.Measurement();
                    statmeas.Metadata = this.statisticsMapping[key].sqrd.Metadata;
                    outputmeasurements.Add(GSF.TimeSeries.Measurement.Clone(statmeas, this.statisticsMapping[key].squaredsummation, frame.Timestamp));

                    statmeas = new GSF.TimeSeries.Measurement();
                    statmeas.Metadata = this.statisticsMapping[key].min.Metadata;
                    outputmeasurements.Add(GSF.TimeSeries.Measurement.Clone(statmeas, this.statisticsMapping[key].minimum, frame.Timestamp));

                    statmeas = new GSF.TimeSeries.Measurement();
                    statmeas.Metadata = this.statisticsMapping[key].max.Metadata;
                    outputmeasurements.Add(GSF.TimeSeries.Measurement.Clone(statmeas, this.statisticsMapping[key].maximum, frame.Timestamp));

                    statmeas = new GSF.TimeSeries.Measurement();
                    statmeas.Metadata = this.statisticsMapping[key].total.Metadata;
                    outputmeasurements.Add(GSF.TimeSeries.Measurement.Clone(statmeas, this.statisticsMapping[key].count, frame.Timestamp));

                    statmeas = new GSF.TimeSeries.Measurement();
                    statmeas.Metadata = this.statisticsMapping[key].alert.Metadata;
                    outputmeasurements.Add(GSF.TimeSeries.Measurement.Clone(statmeas, this.statisticsMapping[key].alertcount, frame.Timestamp));

                    this.statisticsMapping[key].Reset();
                    numberOfFrames = 0;
                }

            }

            OnNewMeasurements(outputmeasurements);
            
        }

        private openHistorian.Model.Device CreateDefaultDevice(GSF.Data.Model.TableOperations<openHistorian.Model.Device> table)
        {
            int protocolId = 0;

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<openHistorian.Model.Protocol> protocolTable = new GSF.Data.Model.TableOperations<openHistorian.Model.Protocol>(connection);
                protocolId = protocolTable.QueryRecordWhere("Acronym = {0}", "VirtualInput").ID;
            }

            openHistorian.Model.Device result = new Device()
            {
                Enabled = true,
                ProtocolID = protocolId,
                Name = "Unbalanced Results",
                Acronym = this.ResultDeviceName,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                CreatedBy = UserInfo.CurrentUserID,
                UpdatedBy = UserInfo.CurrentUserID,
                UniqueID = Guid.NewGuid(),
                NodeID = this.nodeID
            };

            table.AddNewRecord(result);
            result = table.QueryRecordWhere("Acronym = {0}", ResultDeviceName);
            return result;

        }

        private string GetDescribtion(MeasurementKey key, GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement> table)
        {
            ActiveMeasurement measurement = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            string text = measurement.Description;
            string stopAt;
            if (measurement.PhasorType == 'I')
                stopAt = "-I";
            else
                stopAt = "-V";
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }

        private bool SearchNegative(MeasurementKey key, GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement> table)
        {
            ActiveMeasurement measurement = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            return (measurement.Phase == '-');
        }

        private bool SearchPositive(MeasurementKey key, GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement> table)
        {
            ActiveMeasurement measurement = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            return (measurement.Phase == '+');
        }

        private bool SearchZero(MeasurementKey key, GSF.Data.Model.TableOperations<openHistorian.Model.ActiveMeasurement> table)
        {
            ActiveMeasurement measurement = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            return (measurement.Phase == '0');
        }

        private StatisticsCollection CreateStatistics(GSF.Data.Model.TableOperations<openHistorian.Model.Measurement> table, MeasurementKey key, Device device, int HistorianID)
        {
            openHistorian.Model.Measurement inMeas = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            int signaltype = 0;
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<openHistorian.Model.SignalType> signalTypeTable = new GSF.Data.Model.TableOperations<openHistorian.Model.SignalType>(connection);
                signaltype = signalTypeTable.QueryRecordWhere("Acronym = {0}", "CALC").ID;
            }

            string outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:SUM";


            //Sum
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {

                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-UBAL:SUM",
                    AlternateTag = inMeas.AlternateTag + "-UBAL:SUM",
                    SignalTypeID = signaltype,
                    SignalReference = outputRefference,
                    Description = inMeas.Description + " Summ of UBAL",
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

            //sqrdSum
            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:SQR";
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-UBAL:SQR",
                    AlternateTag = inMeas.AlternateTag + "-UBAL:SQR",
                    SignalTypeID = signaltype,
                    SignalReference = outputRefference,
                    Description = inMeas.Description + " Summ of Sqared UBAL",
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

            //Min
            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:MIN";
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-UBAL:MIN",
                    AlternateTag = inMeas.AlternateTag + "-UBAL:MIN",
                    SignalTypeID = signaltype,
                    SignalReference = outputRefference,
                    Description = inMeas.Description + " Minimum UBAL",
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
            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:MAX";
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-UBAL:MAX",
                    AlternateTag = inMeas.AlternateTag + "-UBAL:MAX",
                    SignalTypeID = signaltype,
                    SignalReference = outputRefference,
                    Description = inMeas.Description + " Maximum UBAL",
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
            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:NUM";
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-UBAL:NUM",
                    AlternateTag = inMeas.AlternateTag + "-UBAL:NUM",
                    SignalTypeID = signaltype,
                    SignalReference = outputRefference,
                    Description = inMeas.Description + " Number of Points",
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
            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:ALT";
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-UBAL:ALT",
                    AlternateTag = inMeas.AlternateTag + "-UBAL:ALT",
                    SignalTypeID = signaltype,
                    SignalReference = outputRefference,
                    Description = inMeas.Description + " number of Alerts",
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

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:SUM";
            result.sum = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:SQR";
            result.sqrd = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:MIN";
            result.min = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:MAX";
            result.max = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:NUM";
            result.total = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-UBAL:ALT";
            result.alert = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            result.Reset();
            return result;
        }

        #endregion[Method]

    }
}
