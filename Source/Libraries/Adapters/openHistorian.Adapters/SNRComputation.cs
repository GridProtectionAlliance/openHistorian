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
using System.Diagnostics;

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
                this.maximum = double.MaxValue;
                this.minimum = double.MinValue;
                this.summation = 0;
                this.squaredsummation = 0;
                this.alertcount = 0;
            }
        }

        #endregion[InternalClasses]

        #region [ Members ]

        /// <summary>
        /// Default value for <see cref="WindowLength"/>.
        /// </summary>
        public const int DefaultWindowLength = 30;

        /// <summary>
        /// Default value for <see cref="ReportingIntervall"/>.
        /// </summary>
        public const int DefaultReportingIntervall = 300;


        /// <summary>
        /// Default value for <see cref="ResultDeviceName"/>.
        /// </summary>
        public const string DefaultResultDeviceName = "SNR!SERVICE";

        private int numberOfFrames;
        private Dictionary<Guid, List<double>> dataWindow;
        private List<double> refWindow;
        private Dictionary<Guid, MeasurementKey> outputMapping;
        private Dictionary<Guid, StatisticsCollection> statisticsMapping;
        private bool saveStats;
        private Guid nodeID;
        private double threshold;

        #endregion [ Members ]

        #region[Properties]

        /// <summary>
        /// Gets or sets the device acronym of the outputs
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurementAttribute]
        [Description("Defines Name of the Device used for output Measurements.")]
        [DefaultValue(DefaultResultDeviceName)]
        public string ResultDeviceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the flag to determine of aggregates are saved
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurementAttribute]
        [Description("Defines If aggregates are saved sepperately.")]
        [DefaultValue(false)]
        public bool SaveAggregates
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
        /// Gets or sets the reference for Phase Computation
        /// </summary>
        [ConnectionStringParameter]
        [CalculatedMesaurementAttribute]
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
        [CalculatedMesaurementAttribute]
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
        /// Initializes <see cref="SNRComputation"/>.
        /// </summary>
        public override void Initialize()
        {
            new GSF.Configuration.ConnectionStringParser<CalculatedMesaurementAttribute>().ParseConnectionString(ConnectionString, this);

            
            base.Initialize();

            //Figure OutOutput Measurement Keys

            this.dataWindow = new Dictionary<Guid, List<double>>();
            this.refWindow = new List<double>();
            this.outputMapping = new Dictionary<Guid, MeasurementKey>();
            this.statisticsMapping = new Dictionary<Guid, StatisticsCollection>();
            this.saveStats = this.SaveAggregates;

            if( (double)this.ReportingIntervall % (double)this.WindowLength != 0)
            {
                this.ReportingIntervall = this.WindowLength * (this.ReportingIntervall / this.WindowLength);
                OnStatusMessage(GSF.Diagnostics.MessageLevel.Warning, String.Format("Adjusting Reporting Interval to every {0} frames.", this.ReportingIntervall));
            }

            
            CategorizedSettingsElementCollection reportSettings = ConfigurationFile.Current.Settings["reportSettings"];
            reportSettings.Add("DefaultSNRThreshold", "4.0", "Default SNR Alert threshold.");
            this.threshold = reportSettings["DefaultSNRThreshold"].ValueAs(this.threshold);


            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<openHistorian.Model.Measurement> measTable = new GSF.Data.Model.TableOperations<openHistorian.Model.Measurement>(connection);
                GSF.Data.Model.TableOperations<openHistorian.Model.Device> deviceTable = new GSF.Data.Model.TableOperations<openHistorian.Model.Device>(connection);
                GSF.Data.Model.TableOperations<openHistorian.Model.SignalType> signalTable = new GSF.Data.Model.TableOperations<openHistorian.Model.SignalType>(connection);

                openHistorian.Model.Device device = deviceTable.QueryRecordWhere("Acronym = {0}", ResultDeviceName);

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
                    this.dataWindow.Add(key.SignalID, new List<double>());

                    string outputRefference = measTable.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR";

                    if (measTable.QueryRecordCountWhere("SignalReference = {0}", outputRefference) > 0)
                    {
                        //Measurement Exists
                        this.outputMapping.Add(key.SignalID, MeasurementKey.LookUpBySignalID(
                            measTable.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID));

                    }
                    else
                    {
                        //Add Measurment to Database and make a statement
                        openHistorian.Model.Measurement inMeas = measTable.QueryRecordWhere("SignalID = {0}", key.SignalID);
                        openHistorian.Model.Measurement outMeas = new openHistorian.Model.Measurement()
                        {
                            HistorianID = inMeas.HistorianID,
                            DeviceID = device.ID,
                            PointTag = inMeas.PointTag + "-SNR",
                            AlternateTag = inMeas.AlternateTag + "-SNR",
                            SignalTypeID = signalTable.QueryRecordWhere("Acronym = {0}","CALC").ID,
                            SignalReference = outputRefference,
                            Description = inMeas.Description + " SNR",
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
                        this.outputMapping.Add(key.SignalID, MeasurementKey.LookUpBySignalID(
                           measTable.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID));

                        OnStatusMessage(GSF.Diagnostics.MessageLevel.Warning, String.Format("Output measurment {0} not found. Creating measurement", outputRefference));
                    }

                    if (this.saveStats)
                        this.statisticsMapping.Add(key.SignalID, CreateStatistics(measTable, key, device));
                }

                if (this.Reference == null)
                {
                    OnStatusMessage(GSF.Diagnostics.MessageLevel.Warning, "No Reference Angle Specified");
                    int refIndex = this.InputMeasurementKeyTypes.IndexOf(item => (item == GSF.Units.EE.SignalType.IPHA || item == GSF.Units.EE.SignalType.VPHA));
                    if (refIndex > -1)
                    {
                        this.Reference = this.InputMeasurementKeys[refIndex];
                    }
                }

                if (this.Reference != null)
                {
                    if (!this.InputMeasurementKeys.Contains(this.Reference))
                    {
                        this.InputMeasurementKeys.AddRange(new MeasurementKey[1] { this.Reference });
                    }
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
            numberOfFrames++;

            MeasurementKey[] availableKeys = frame.Measurements.Values.MeasurementKeys();
            
            foreach (Guid key in this.dataWindow.Keys)
            {
                int keyIndex = availableKeys.IndexOf(item => item.SignalID == key);
                if (keyIndex > -1)
                {
                    this.dataWindow[key].Add(frame.Measurements[availableKeys[keyIndex]].Value);
                }
                else
                {
                    this.dataWindow[key].Add(double.NaN);
                }
            }

            if (this.Reference != null)
            {
                int keyIndex = availableKeys.IndexOf(item => item.SignalID == Reference.SignalID);
                if (keyIndex > -1)
                {
                    this.refWindow.Add(frame.Measurements[availableKeys[keyIndex]].Value);
                }
                else
                {
                    this.refWindow.Add(double.NaN);
                }
            }
            
            
            int currentWindowLength = this.dataWindow.Keys.Select(key => this.dataWindow[key].Count).Max();

            if (currentWindowLength >= WindowLength)
            {
                List<IMeasurement> outputmeasurements = new List<IMeasurement>();

                List<Guid> Keys = this.dataWindow.Keys.ToList();
                foreach (Guid key in Keys)
                {
                    double SNR = CalculateSignalToNoise(this.dataWindow[key],key);
                    GSF.TimeSeries.Measurement outmeas = new GSF.TimeSeries.Measurement();
                    outmeas.Metadata = this.outputMapping[key].Metadata;

                    if ((SNR != double.NaN) && (this.saveStats))
                    {
                        this.statisticsMapping[key].count++;
                        if (SNR > this.statisticsMapping[key].maximum)
                            this.statisticsMapping[key].maximum = SNR;
                        if (SNR < this.statisticsMapping[key].minimum)
                            this.statisticsMapping[key].minimum = SNR;
                        if (SNR > this.threshold)
                            this.statisticsMapping[key].alertcount++;
                        this.statisticsMapping[key].summation += SNR;
                        this.statisticsMapping[key].squaredsummation += (SNR * SNR);
                    }

                    outputmeasurements.Add(GSF.TimeSeries.Measurement.Clone(outmeas,SNR, frame.Timestamp));
                    dataWindow[key] = new List<double>();
                    if (!this.saveStats)
                        numberOfFrames = 0;
                }
                refWindow = new List<double>();

                // Reporting if neccesarry
                if ((numberOfFrames >= this.ReportingIntervall) && (this.saveStats))
                {
                    numberOfFrames = 0;
                    foreach (Guid key in Keys)
                    {
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
                    }

                }

                OnNewMeasurements(outputmeasurements);
            }
        }

        private double CalculateSignalToNoise(List<double> values, Guid key)
        {
            int sampleCount = values.Count;
            int keyIndex = this.InputMeasurementKeys.IndexOf(item => item.SignalID == key);

            //If its a Phase
            if ((this.InputMeasurementKeyTypes[keyIndex] == GSF.Units.EE.SignalType.IPHA ||
                this.InputMeasurementKeyTypes[keyIndex] == GSF.Units.EE.SignalType.IPHA) && 
                    (sampleCount == this.refWindow.Count()) && (this.Reference != null))
            {
                values = GSF.Units.Angle.Unwrap(values.Select((item, index) => (GSF.Units.Angle)item - (GSF.Units.Angle)this.refWindow[index]).ToArray()).Select(item => (double)item).ToList();
            }

            values = values.Where(item => !double.IsNaN(item)).ToList();
            sampleCount = values.Count;

            if (sampleCount < 1)
                return double.NaN;

            double sampleAverage = values.Average();
            double totalVariance = values.Select(item => item - sampleAverage).Select(deviation => deviation * deviation).Sum();

            return (10*Math.Log10(Math.Abs(sampleAverage / Math.Sqrt(totalVariance / sampleCount))));
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
                Name = "SNR Results",
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

        private StatisticsCollection CreateStatistics(GSF.Data.Model.TableOperations<openHistorian.Model.Measurement> table, MeasurementKey key, Device device)
        {
            openHistorian.Model.Measurement inMeas = table.QueryRecordWhere("SignalID = {0}", key.SignalID);
            int signaltype = 0;
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<openHistorian.Model.SignalType> signalTypeTable = new GSF.Data.Model.TableOperations<openHistorian.Model.SignalType>(connection);
                signaltype = signalTypeTable.QueryRecordWhere("Acronym = {0}", "CALC").ID;
            }

            string outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:SUM";


            //Sum
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = inMeas.HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-SNR:SUM",
                    AlternateTag = inMeas.AlternateTag + "-SNR:SUM",
                    SignalTypeID = signaltype,
                    SignalReference = outputRefference,
                    Description = inMeas.Description + " Summ of SNR",
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
            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:SQR";
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = inMeas.HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-SNR:SQR",
                    AlternateTag = inMeas.AlternateTag + "-SNR:SQR",
                    SignalTypeID = signaltype,
                    SignalReference = outputRefference,
                    Description = inMeas.Description + " Summ of Sqared SNR",
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
            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:MIN";
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = inMeas.HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-SNR:MIN",
                    AlternateTag = inMeas.AlternateTag + "-SNR:MIN",
                    SignalTypeID = signaltype,
                    SignalReference = outputRefference,
                    Description = inMeas.Description + " Minimum SNR",
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
            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:MAX";
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = inMeas.HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-SNR:MAX",
                    AlternateTag = inMeas.AlternateTag + "-SNR:MAX",
                    SignalTypeID = signaltype,
                    SignalReference = outputRefference,
                    Description = inMeas.Description + " Maximum SNR",
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
            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:NUM";
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = inMeas.HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-SNR:NUM",
                    AlternateTag = inMeas.AlternateTag + "-SNR:NUM",
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
            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:ALT";
            if (table.QueryRecordCountWhere("SignalReference = {0}", outputRefference) < 1)
            {
                table.AddNewRecord(new openHistorian.Model.Measurement()
                {
                    HistorianID = inMeas.HistorianID,
                    DeviceID = device.ID,
                    PointTag = inMeas.PointTag + "-SNR:ALT",
                    AlternateTag = inMeas.AlternateTag + "-SNR:ALT",
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

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:SUM";
            result.sum = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:SQR";
            result.sqrd = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:MIN";
            result.min = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:MAX";
            result.max = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:NUM";
            result.total = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            outputRefference = table.QueryRecordWhere("SignalID = {0}", key.SignalID).SignalReference + "-SNR:ALT";
            result.alert = MeasurementKey.LookUpBySignalID(table.QueryRecordWhere("SignalReference = {0}", outputRefference).SignalID);

            result.Reset();
            return result;          
        }

        #endregion[Method]

    }
}
