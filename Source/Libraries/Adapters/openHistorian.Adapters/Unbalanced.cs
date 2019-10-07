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
        #region [ Members ]

        
        /// <summary>
        /// Default value for <see cref="ResultDeviceName"/>.
        /// </summary>
        public const string DefaultResultDeviceName = "SNR!SERVICE";

        private int numberOfFrames;
        private Guid nodeID;
        private List<ThreePhaseSet> threePhaseComponent;

        #endregion [ Members ]

        private class ThreePhaseSet
        {
            public MeasurementKey Positive;
            public MeasurementKey Negative;
            public MeasurementKey Zero;
            public MeasurementKey outMapping;

            public ThreePhaseSet(MeasurementKey pos, MeasurementKey zero, MeasurementKey neg, MeasurementKey unbalance)
            {
                this.Positive = pos;
                this.Negative = neg;
                this.Zero = zero;
                this.outMapping = unbalance;

            }
        }
        
        #region[Properties]

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

            //Figure OutOutput Measurement Keys
            this.threePhaseComponent = new List<ThreePhaseSet>();

            List<Guid> processed = new List<Guid>();

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                GSF.Data.Model.TableOperations<openHistorian.Model.Measurement> measTable = new GSF.Data.Model.TableOperations<openHistorian.Model.Measurement>(connection);
                GSF.Data.Model.TableOperations<openHistorian.Model.Device> deviceTable = new GSF.Data.Model.TableOperations<openHistorian.Model.Device>(connection);
                GSF.Data.Model.TableOperations<openHistorian.Model.SignalType> signalTable = new GSF.Data.Model.TableOperations<openHistorian.Model.SignalType>(connection);

                openHistorian.Model.Device device = deviceTable.QueryRecordWhere("Acronym = {0}", ResultDeviceName);

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
                                HistorianID = inMeas.HistorianID,
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

                        this.threePhaseComponent.Add(new ThreePhaseSet(pos, zero, neg, unBalance));
                        processed.Add(pos.SignalID);
                        processed.Add(neg.SignalID);
                        processed.Add(zero.SignalID);

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
            numberOfFrames++;

            if (numberOfFrames > 3000)
            {
                numberOfFrames = 0;
            }

            IMeasurement[] available = frame.Measurements.Values.ToArray();

            List<Guid> availableGuids = available.Select(item => item.Key.SignalID).ToList();

            List<IMeasurement> outputmeasurements = new List<IMeasurement>();

            foreach (ThreePhaseSet set in this.threePhaseComponent)
            {
                bool hasP=availableGuids.Contains(set.Positive.SignalID);
                bool hasN = availableGuids.Contains(set.Negative.SignalID);
                bool hasZ = availableGuids.Contains(set.Zero.SignalID);

                if (hasP && hasN)
                {
                    double v1p = available.Where(item => item.Key.SignalID == set.Positive.SignalID).FirstOrDefault().Value;
                    double v2n = available.Where(item => item.Key.SignalID == set.Negative.SignalID).FirstOrDefault().Value;
                    if (v1p != 0.0F)
                    {
                        double unbalanced = v2n / v1p;
                        GSF.TimeSeries.Measurement outmeas = new GSF.TimeSeries.Measurement();
                        outmeas.Metadata = set.outMapping.Metadata;

                        outputmeasurements.Add(GSF.TimeSeries.Measurement.Clone(outmeas, unbalanced, frame.Timestamp));
                    }
                }
            }


                OnNewMeasurements(outputmeasurements);
            
            

            /// So that the number of frames does not continue on forever
            /// The number is arbitrary, but it needs to rollover like this
            if (numberOfFrames % 30 == 0)
            {
                //OnStatusMessage(measurement.Key.ToString() + ": " + measurement.Value.ToString());
            }
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
        #endregion[Method]

    }
}
