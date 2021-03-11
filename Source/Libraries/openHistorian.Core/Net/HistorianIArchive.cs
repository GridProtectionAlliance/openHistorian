//******************************************************************************************************
//  HistorianIArchive.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  07/26/2013 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GSF;
using GSF.Data;
using GSF.Historian;
using GSF.Historian.Files;
using GSF.Parsing;
using GSF.Snap;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.TimeSeries;
using openHistorian.Snap;
using DataType = GSF.Historian.Files.DataType;

namespace openHistorian.Net
{
    /// <summary>
    /// An <see cref="IArchive"/> wrapper around a SortedTreeStore.
    /// </summary>
    /// <remarks>
    /// This class implements the 1.0 historian <see cref="IArchive"/> to automatically bring in historian providers (e.g., web services).
    /// </remarks>
    public class HistorianIArchive : IArchive
    {
        #region [ Members ]

        // Events
        public event EventHandler MetadataUpdated;

        // Fields
        private readonly HistorianServer m_server;
        private readonly SnapClient m_client;
        private readonly ClientDatabaseBase<HistorianKey, HistorianValue> m_clientDatabase;

        #endregion


        #region [ Constructors ]

        public HistorianIArchive(HistorianServer server, string databaseName)
        {
            m_server = server;
            m_client = SnapClient.Connect(m_server.Host);
            m_clientDatabase = m_client.GetDatabase<HistorianKey, HistorianValue>(databaseName);
        }

        #endregion

        #region [ Properties ]

        public HistorianServer Server => m_server;

        public SnapClient Client => m_client;

        public ClientDatabaseBase<HistorianKey, HistorianValue> ClientDatabase => m_clientDatabase;

    #endregion

        #region [ Methods ]

        // Implementing IArchive for automatic integration with time-series web services, metadata sync providers and replication support
        void IArchive.Open()
        {
            // Archive integration components should not be opening the archive (assumed to open already)
        }

        void IArchive.Close()
        {
            // Archive integration components should not be closing the archive
        }

        IEnumerable<IDataPoint> IArchive.ReadData(IEnumerable<int> historianIDs, string startTime, string endTime, bool timeSorted)
        {
            ulong startTimestamp = (ulong)TimeTag.Parse(startTime).ToDateTime().Ticks;
            ulong endTimestamp = (ulong)TimeTag.Parse(endTime).ToDateTime().Ticks;

            return ReadDataStream(m_clientDatabase.Read(startTimestamp, endTimestamp, historianIDs.Select(pointID => (ulong)pointID)));
        }

        IEnumerable<IDataPoint> IArchive.ReadData(IEnumerable<int> historianIDs, DateTime startTime, DateTime endTime, bool timeSorted)
        {
            return ReadDataStream(m_clientDatabase.Read(startTime, endTime, historianIDs.Select(pointID => (ulong)pointID)));
        }

        IEnumerable<IDataPoint> IArchive.ReadData(int historianID, string startTime, string endTime, bool timeSorted)
        {
            ulong startTimestamp = (ulong)TimeTag.Parse(startTime).ToDateTime().Ticks;
            ulong endTimestamp = (ulong)TimeTag.Parse(endTime).ToDateTime().Ticks;
            return ReadDataStream(m_clientDatabase.Read(startTimestamp, endTimestamp, new[] { (ulong)historianID }));
        }

        IEnumerable<IDataPoint> IArchive.ReadData(int historianID, DateTime startTime, DateTime endTime, bool timeSorted)
        {
            return ReadDataStream(m_clientDatabase.Read(startTime, endTime, new[] { (ulong)historianID }));
        }

        private IEnumerable<IDataPoint> ReadDataStream(TreeStream<HistorianKey, HistorianValue> stream)
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            List<ArchiveDataPoint> queriedData = new List<ArchiveDataPoint>();
            ArchiveDataPoint point;
            MeasurementStateFlags stateFlags;

            while (stream.Read(key, value))
            {
                point = new ArchiveDataPoint((int)key.PointID);
                point.Time = new TimeTag(new DateTime((long)key.Timestamp));
                point.Value = BitConvert.ToSingle(value.Value1);

                stateFlags = (MeasurementStateFlags)value.Value3;

                if ((stateFlags & MeasurementStateFlags.BadData) == 0)
                {
                    if ((stateFlags & MeasurementStateFlags.BadTime) == 0)
                        point.Quality = Quality.Good;
                    else
                        point.Quality = Quality.Old;
                }
                else
                {
                    point.Quality = Quality.SuspectData;
                }

                queriedData.Add(point);
            }

            return queriedData;
        }

        void IArchive.WriteData(IDataPoint dataPoint)
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            key.PointID = (ulong)dataPoint.HistorianID;
            key.Timestamp = (ulong)dataPoint.Time.ToDateTime().Ticks;

            value.Value1 = BitConvert.ToUInt64(dataPoint.Value);
            value.Value3 = (ulong)dataPoint.Quality.MeasurementQuality();

            m_clientDatabase.Write(key, value);
        }

        byte[] IArchive.ReadMetaData(int historianID)
        {
            MetadataRecord record = ReadMetadataRecord(historianID);

            if (record != null)
                return record.BinaryImage();

            return null;
        }

        byte[] IArchive.ReadMetaDataSummary(int historianID)
        {
            MetadataRecord record = ReadMetadataRecord(historianID);

            if (record != null)
                return record.Summary.BinaryImage();

            return null;
        }

        // openHistorian 2.0 currently uses the database for its metadata repository - so we just update the database using the
        // provided metadata record as received from the metadata web service interface
        void IArchive.WriteMetaData(int historianID, byte[] metadata)
        {
            MetadataRecord record = new MetadataRecord(historianID, MetadataFileLegacyMode.Enabled, metadata, 0, metadata.Length);

            using (AdoDataConnection database = new AdoDataConnection("systemSettings"))
            {
                IDbConnection connection = database.Connection;

                // TODO: To enable update signal type, associated device, destination historian, etc. - look up key ID values in advance of measurement record update

                // Only updating readily available fields - to make this more useful in the future, will need to update (or add a new) web service metadata format...
                string query = database.ParameterizedQueryString("UPDATE Measurement SET PointTag={0}, SignalReference={1}, AlternateTag={2}, Enabled={3} WHERE HistorianID = {4}", "pointTag", "signalReference", "alternateTag", "enabled", "historianID");

                // Update metadata fields
                connection.ExecuteNonQuery(query, record.Name, record.Synonym1, record.Synonym3, record.GeneralFlags.Enabled, historianID);
            }
        }

        // openHistorian 2.0 currently uses the database for its metadata repository - so we just expose the metadata record
        // as read from the database for exposure via metadata web service interface
        private MetadataRecord ReadMetadataRecord(int historianID)
        {
            MetadataRecord record = new MetadataRecord(historianID, MetadataFileLegacyMode.Enabled);

            using (AdoDataConnection database = new AdoDataConnection("systemSettings"))
            {
                IDbConnection connection = database.Connection;

                string query = string.Format("SELECT * FROM HistorianMetadata WHERE HistorianID = {0}", historianID);

                DataRow row = connection.RetrieveRow(database.AdapterType, query);

                // Make sure record exists for this point ID
                if (Convert.IsDBNull(row[0]) || Convert.ToInt32(row[0]) != historianID)
                    return null;

                if (!Convert.IsDBNull(row[1]))
                    record.GeneralFlags.DataType = (DataType)Convert.ToInt32(row[1]);

                if (!Convert.IsDBNull(row[2]))
                    record.Name = Convert.ToString(row[2]);

                if (!Convert.IsDBNull(row[3]))
                    record.Synonym1 = Convert.ToString(row[3]);

                if (!Convert.IsDBNull(row[4]))
                    record.Synonym2 = Convert.ToString(row[4]);

                if (!Convert.IsDBNull(row[5]))
                    record.Synonym3 = Convert.ToString(row[5]);

                if (!Convert.IsDBNull(row[6]))
                    record.Description = Convert.ToString(row[6]);

                if (!Convert.IsDBNull(row[7]))
                    record.HardwareInfo = Convert.ToString(row[7]);

                if (!Convert.IsDBNull(row[8]))
                    record.Remarks = Convert.ToString(row[8]);

                if (!Convert.IsDBNull(row[9]))
                    record.PlantCode = Convert.ToString(row[9]);

                if (!Convert.IsDBNull(row[10]))
                    record.UnitNumber = Convert.ToInt32(row[10]);

                if (!Convert.IsDBNull(row[11]))
                    record.SystemName = Convert.ToString(row[11]);

                if (!Convert.IsDBNull(row[12]))
                    record.SourceID = Convert.ToInt32(row[12]);

                if (!Convert.IsDBNull(row[13]))
                    record.GeneralFlags.Enabled = Convert.ToBoolean(row[13]);

                if (!Convert.IsDBNull(row[14]))
                    record.ScanRate = Convert.ToSingle(row[14]);

                if (!Convert.IsDBNull(row[15]))
                    record.CompressionMinTime = Convert.ToInt32(row[15]);

                if (!Convert.IsDBNull(row[16]))
                    record.CompressionMaxTime = Convert.ToInt32(row[16]);

                if (!Convert.IsDBNull(row[30]))
                    record.SecurityFlags.ChangeSecurity = Convert.ToInt32(row[30]);

                if (!Convert.IsDBNull(row[31]))
                    record.SecurityFlags.AccessSecurity = Convert.ToInt32(row[31]);

                if (!Convert.IsDBNull(row[32]))
                    record.GeneralFlags.StepCheck = Convert.ToBoolean(row[32]);

                if (!Convert.IsDBNull(row[33]))
                    record.GeneralFlags.AlarmEnabled = Convert.ToBoolean(row[33]);

                if (!Convert.IsDBNull(row[34]))
                    record.AlarmFlags.Value = Convert.ToInt32(row[34]);

                if (!Convert.IsDBNull(row[36]))
                    record.GeneralFlags.AlarmToFile = Convert.ToBoolean(row[36]);

                if (!Convert.IsDBNull(row[37]))
                    record.GeneralFlags.AlarmByEmail = Convert.ToBoolean(row[37]);

                if (!Convert.IsDBNull(row[38]))
                    record.GeneralFlags.AlarmByPager = Convert.ToBoolean(row[38]);

                if (!Convert.IsDBNull(row[39]))
                    record.GeneralFlags.AlarmByPhone = Convert.ToBoolean(row[39]);

                if (!Convert.IsDBNull(row[40]))
                    record.AlarmEmails = Convert.ToString(row[40]);

                if (!Convert.IsDBNull(row[41]))
                    record.AlarmPagers = Convert.ToString(row[41]);

                if (!Convert.IsDBNull(row[42]))
                    record.AlarmPhones = Convert.ToString(row[42]);

                if (record.GeneralFlags.DataType == DataType.Analog)
                {
                    if (!Convert.IsDBNull(row[17]))
                        record.AnalogFields.EngineeringUnits = Convert.ToString(row[17]);

                    if (!Convert.IsDBNull(row[18]))
                        record.AnalogFields.LowWarning = Convert.ToSingle(row[18]);

                    if (!Convert.IsDBNull(row[19]))
                        record.AnalogFields.HighWarning = Convert.ToSingle(row[19]);

                    if (!Convert.IsDBNull(row[20]))
                        record.AnalogFields.LowAlarm = Convert.ToSingle(row[20]);

                    if (!Convert.IsDBNull(row[21]))
                        record.AnalogFields.HighAlarm = Convert.ToSingle(row[21]);

                    if (!Convert.IsDBNull(row[22]))
                        record.AnalogFields.LowRange = Convert.ToSingle(row[22]);

                    if (!Convert.IsDBNull(row[23]))
                        record.AnalogFields.HighRange = Convert.ToSingle(row[23]);

                    if (!Convert.IsDBNull(row[24]))
                        record.AnalogFields.CompressionLimit = Convert.ToSingle(row[24]);

                    if (!Convert.IsDBNull(row[25]))
                        record.AnalogFields.ExceptionLimit = Convert.ToSingle(row[25]);

                    if (!Convert.IsDBNull(row[26]))
                        record.AnalogFields.DisplayDigits = Convert.ToInt32(row[26]);

                    if (!Convert.IsDBNull(row[35]))
                        record.AnalogFields.AlarmDelay = Convert.ToSingle(row[35]);
                }
                else if (record.GeneralFlags.DataType == DataType.Digital)
                {
                    if (!Convert.IsDBNull(row[27]))
                        record.DigitalFields.SetDescription = Convert.ToString(row[27]);

                    if (!Convert.IsDBNull(row[28]))
                        record.DigitalFields.ClearDescription = Convert.ToString(row[28]);

                    if (!Convert.IsDBNull(row[29]))
                        record.DigitalFields.AlarmState = Convert.ToInt32(row[29]);

                    if (!Convert.IsDBNull(row[35]))
                        record.DigitalFields.AlarmDelay = Convert.ToSingle(row[35]);
                }
            }

            return record;
        }

        byte[] IArchive.ReadStateData(int historianID)
        {
            return ReadStateDataRecord(historianID).BinaryImage();
        }

        byte[] IArchive.ReadStateDataSummary(int historianID)
        {
            return ReadStateDataRecord(historianID).Summary.BinaryImage();
        }

        private StateRecord ReadStateDataRecord(int historianID)
        {
            StateRecord record = new StateRecord(historianID);

            // TODO: Just doing this to get latest value until API supports this
            IEnumerable<IDataPoint> dataPoints;
            Ticks stopTime = DateTime.UtcNow.Ticks;
            Ticks startTime = stopTime - Ticks.PerSecond * 2;

            dataPoints = ReadDataStream(m_clientDatabase.Read((ulong)(long)startTime, (ulong)(long)stopTime, new[] { (ulong)historianID }));

            StateRecordDataPoint dataPoint = new StateRecordDataPoint(dataPoints.Last());

            record.CurrentData = dataPoint;
            record.PreviousData = dataPoint;
            record.ArchivedData = dataPoint;

            return record;
        }

        void IArchive.WriteStateData(int historianID, byte[] stateData)
        {
            // Archive integration components should not be updating state data
        }

        public void Write(HistorianKey key, HistorianValue value)
        {
            m_clientDatabase.Write(key, value);

        }

        #endregion
    }
}
