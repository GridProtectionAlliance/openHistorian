//******************************************************************************************************
//  HadoopDataLoader.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/01/2020 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using GSF;
using GSF.Collections;
using GSF.Configuration;
using GSF.Data;
using GSF.Diagnostics;
using GSF.IO;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using Timer = System.Timers.Timer;
using System.Threading;
using CancellationToken = GSF.Threading.CancellationToken;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents an output adapter that publishes measurements from a HADOOP Database to openHistorian for archival.
    /// </summary>
    [Description("Hadoop Data Loader: loads measurements from HADOOP DataBase")]
    public class HadoopDataLoader : InputAdapterBase
    {
        #region [ Members ]
        // Constants
        private const string DefaultConnectionString = "DRIVER={Cloudera ODBC Driver for Apache Hive};HOST=myserver;PORT=10000;DB=default;";
        private const string DefaultTableName = "telemetry";
        private const string DefaultMappingFile = "";
        private const string DefaultTimeStampField = "source_tz";
        private const string DefaultValueField = "value";
        //private const string DefaultTagQuery = "device = '{1}' AND device_type = '{2}'";
        private const string DefaultTagQuery = "device = 'a' AND device_type = 'b'";
        private const string DefaultTicks = "";

        private const string TimeStampUpdatefile = "./HadoopTS.bin";

        // Fields
        private Timer m_timer;
        private string m_query;
        private Dictionary<Guid, List<string>> m_queryParameter;
        private readonly DateTime m_oldestTimestamp = new DateTime(1990, 1, 1, 0, 0, 0);
        private int m_num;
        private int m_nTags;
        private int m_currNum;
        private DateTime m_lastConnected;
        private bool m_disposed;
        private CancellationToken m_cancelationToken;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the ConnectionString to connect to Hadoop.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the connection string to the HADOOP Database")]
        [DefaultValue(DefaultConnectionString)]
        public string HadoopConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the Hadoop Table to query data.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the Table used to query measurements")]
        [DefaultValue(DefaultTableName)]
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the Field used to generate the Timestamp.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the Field used to create the Timestamp")]
        [DefaultValue(DefaultTimeStampField)]
        public string TimeStampField { get; set; }

        /// <summary>
        /// Gets or sets the field used to pull data.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the Field to pull Data")]
        [DefaultValue(DefaultValueField)]
        public string ValueField { get; set; }

        /// <summary>
        /// Gets or sets the Mapping File.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the mapping File use to identify measurements")]
        [DefaultValue(DefaultMappingFile)]
        [CustomConfigurationEditor("GSF.TimeSeries.UI.WPF.dll", "GSF.TimeSeries.UI.Editors.FileDialogEditor", "type=open; checkFileExists=true; defaultExt=.csv; filter=CSV files|*.csv|AllFiles|*.*")]
        public string MappingFile { get; set; }

        /// <summary>
        /// Gets or sets the Tag Query.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the requirements used to match measurements to historian Tags")]
        [DefaultValue(DefaultTagQuery)]
        public string TagQuery { get; set; }

        /// <summary>
        /// Gets or sets the Field that is used to Order Signals.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the Order of Points if multiple Points have the same Timestamp")]
        [DefaultValue("")]
        public string OrderField { get; set; }
        
        /// <summary>
         /// Gets or sets the Field that is used to Order Signals.
         /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the Field that contains sub-second TS (in Ticks)")]
        [DefaultValue(DefaultTicks)]
        public string SubSecondField { get; set; }


        /// <summary>
        /// Gets or sets the interval in which the Database is updated.
        /// </summary>
        [ConnectionStringParameter]
        [Description("Defines the interval at which HADOOP is queried in milliseconds.")]
        [DefaultValue(30000)]
        public int UpdateInterval { get; set; }

        /// <summary>
        /// Gets the flag indicating if this adapter supports temporal processing.
        /// </summary>
        public override bool SupportsTemporalProcessing => false;

        /// <summary>
        /// Gets flag that determines if the data input connects asynchronously.
        /// </summary>
        protected override bool UseAsyncConnect => false;

        /// <summary>
        /// Returns the detailed status of the data input source.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.Append(base.Status);

                status.AppendLine($"         Connection String: {HadoopConnectionString}");
                status.AppendLine($"                Data Query: {m_query}");
                status.AppendLine($"              Last connect: {m_lastConnected:dd/MM/yyyy hh:mm:ss}");
                status.AppendLine($"Processed Messages on Last connect: {m_num}");
                status.AppendLine($"OpenHistorian Tags Found  : {m_nTags}");
                status.AppendLine($"Currently Processing      : {m_currNum}");

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HadoopDataLoader"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if (m_timer != null)
                        {
                            m_timer.Elapsed -= m_timer_Elapsed;
                            m_timer.Dispose();
                            m_timer = null;
                        }
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Initializes this <see cref="HadoopDataLoader"/>.
        /// </summary>
        public override void Initialize()
        {
            Dictionary<string, string> settings = Settings;
            m_nTags = 0;
            m_num = 0;
            m_currNum = 0;

            // Handle misspelled property so previously configured adapters will still apply proper value
            if (settings.TryGetValue("UpdateIntervall", out string setting) && int.TryParse(setting, out _))
                settings["UpdateInterval"] = setting;

            new ConnectionStringParser<ConnectionStringParameterAttribute>().ParseConnectionString(ConnectionString, this);

            base.Initialize();

            //Generate Query
            m_query = $"SELECT {ValueField} AS V, {TimeStampField} AS T";

            if (!string.IsNullOrEmpty(SubSecondField))
                m_query = m_query + $", {SubSecondField} AS Ticks";

            m_query = m_query + $" FROM {TableName} WHERE {TimeStampField} > '{{0}}' AND {TagQuery}";

            if (!string.IsNullOrEmpty(OrderField))
                m_query = $" ORDER BY {OrderField}";


            //Create Mapping Dictionary
            List<string> pointTags = OutputMeasurements.Select(item => item.Metadata.TagName).ToList();
            m_queryParameter = new Dictionary<Guid, List<string>>();

            // Read Mapping File
            using (StreamReader reader = new StreamReader(FilePath.GetAbsolutePath(MappingFile)))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                { 
                    List<string> entries = line.Split(',').Select(item => item.Trim()).ToList();
                    string pointTag = entries[0];

                    int index = pointTags.FindIndex(item => item == pointTag);
                    if (index > -1)
                        m_queryParameter.Add(OutputMeasurements[index].Key.SignalID, entries.Skip(1).ToList());
                }
            }

            m_nTags = m_queryParameter.Count;

            m_cancelationToken = new CancellationToken();

            //start Timer
            m_timer = new Timer();
            m_timer.Interval = UpdateInterval;
            m_timer.AutoReset = true;
            m_timer.Elapsed += m_timer_Elapsed;
        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="HadoopDataLoader"/>.
        /// </summary>
        /// <param name="maxLength">Maximum length of the status message.</param>
        /// <returns>Text of the status message.</returns>
        public override string GetShortStatus(int maxLength)
        {
            //if (Enabled && m_publicationTime > 0)
            //return $"Publishing data for {(Ticks)m_publicationTime:yyyy-MM-dd HH:mm:ss.fff}...".CenterText(maxLength);
            if (m_lastConnected != null)
                return $"Last Successful Connection at {m_lastConnected:dd/MM/yyyy hh:mm:ss} returned {m_num} Points".CenterText(maxLength);

            return "Not currently publishing data".CenterText(maxLength);
        }

        /// <summary>
        /// Attempts to connect to this <see cref="HadoopDataLoader"/>.
        /// </summary>
        protected override void AttemptConnection()
        {
           
            m_timer.Start();
        }

        /// <summary>
        /// Attempts to disconnect from this <see cref="HadoopDataLoader"/>.
        /// </summary>
        protected override void AttemptDisconnection()
        {
            if (m_cancelationToken != null)
                m_cancelationToken.Cancel();
            if (m_timer != null)
            {
                m_timer.Enabled = false;

                lock (m_timer)
                    m_timer = null;
            }
          
        }

       

        // Run Query in Haoop
        private void m_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<IMeasurement> measurements = new List<IMeasurement>();

            OnStatusMessage(MessageLevel.Info, "Connecting to Hadoop DB for update");
            
            if (Monitor.TryEnter(m_timer))
            {
                try 
                {
                    bool addTicks = !string.IsNullOrEmpty(SubSecondField);
                        //Connect to DataBase
                    using (OdbcConnection connection = new OdbcConnection(HadoopConnectionString))
                    {
                        m_currNum = 0;
                        int nPoints = 0;
                        foreach( Guid guid in m_queryParameter.Keys)
                        {
                            Ticks newerThan;
                            m_currNum++;
                            nPoints = 0;

                            lock (s_TimeStampUpdateLock)
                            {
                                using (FileBackedDictionary<Guid, Ticks> dictionary = new FileBackedDictionary<Guid, Ticks>(TimeStampUpdatefile))
                                {
                                    if (!dictionary.TryGetValue(guid, out newerThan))
                                        newerThan = m_oldestTimestamp;
                                }
                            }

                            object[] param = { newerThan.ToString("yyyy-MM-dd hh:mm:ss") };

                            

                            param = param.Concat(m_queryParameter[guid]).ToArray();

                            DataTable table = connection.RetrieveData(string.Format(m_query, param));
                            

                            foreach (DataRow row in table.Rows)
                            {
                                Measurement measurement = new Measurement { Metadata = MeasurementKey.LookUpOrCreate(guid, "").Metadata };
                                measurement.Value = row.AsDouble(0)?? double.NaN;
                                measurement.Timestamp = DateTime.Parse(row.AsString(1));

                                // This is only down to Seconds accuracy so we do make sure we are only keeping the seconds here
                                measurement.Timestamp = measurement.Timestamp - measurement.Timestamp.DistanceBeyondSecond();

                                if (addTicks)
                                    measurement.Timestamp = measurement.Timestamp + row.AsInt64(2)?? 0;

                                if (measurement.Timestamp <= newerThan)
                                    continue;

                                measurements.Add(measurement);
                                nPoints++;
                                if (measurement.Timestamp > newerThan)
                                    newerThan = measurement.Timestamp;
                            }

                            lock (s_TimeStampUpdateLock)
                            {
                                using (FileBackedDictionary<Guid, Ticks> dictionary = new FileBackedDictionary<Guid, Ticks>(TimeStampUpdatefile))
                                {
                                    if (dictionary.Keys.Contains(guid))
                                        dictionary[guid] = newerThan;
                                    else
                                        dictionary.Add(guid, newerThan);
                                }
                            }

                            m_lastConnected = DateTime.UtcNow;
                            if (m_currNum%20 == 0)
                            {
                                OnStatusMessage(MessageLevel.Info, $"Got Measurements for {m_currNum} out of {m_nTags} Tags");
                                OnStatusMessage(MessageLevel.Info, $"Obtained {nPoints} Points For Tag {guid} up to {newerThan:dd/MM/yyyy hh:mm:ss}");
                            }
                            if (m_cancelationToken.IsCancelled)
                                break;
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    // Pooled timer thread executed after last read, verify timer has stopped
                    m_timer.Enabled = false;
                    OnProcessException(MessageLevel.Warning, ex);
                }
                catch (Exception ex)
                {
                    OnProcessException(MessageLevel.Error, ex);
                }
                finally
                {
                    System.Threading.Monitor.Exit(m_timer);
                }
            }

            // Publish all measurements for this time interval
            m_num = measurements.Count;
            OnStatusMessage(MessageLevel.Info, $"Disconnected from Hadoop with a total of {m_num} Points");
            if (measurements.Count > 0)
                OnNewMeasurements(measurements);

        }

        #endregion

        #region [ static ]
        private static readonly object s_TimeStampUpdateLock = new object();
        #endregion
    }
}
