//******************************************************************************************************
//  TrenDAPController.cs - Gbtc
//
//  Copyright © 2018, Grid Protection Alliance.  All Rights Reserved.
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
//  12/20/2018 - Billy Ernest
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF;
using GSF.Collections;
using GSF.Configuration;
using GSF.Data;
using GSF.Diagnostics;
using GSF.Security;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.TimeSeries;
using Newtonsoft.Json;
using openHistorian.Adapters;
using openHistorian.Snap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

// ReSharper disable VirtualMemberCallInConstructor
namespace openHistorian.TrenDAPController
{
    /// <summary>
    /// Represents a REST based API for a simple JSON based Grafana data source for TrenDAP,
    /// accessible from Grafana data source as http://localhost:8180/api/TrenDAP
    /// </summary>
    public class TrenDAPController : ApiController
    {
        #region [ Members ]


        #endregion

        #region [ Static ]


        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Validates that openHistorian Grafana data source is responding as expected.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Index()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Returns ActiveMeasurement table.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage GetMetaData()
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(connection.RetrieveData("SELECT * FROM ActiveMeasurement WHERE SignalType !='STAT' AND Device IS NOT NULL")));
                return response;

            }
        }

        /// <summary>
        /// Returns historian instances.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage GetInstances()
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(connection.RetrieveData("SELECT * FROM Historian WHERE Acronym !='STAT'")));
                return response;

            }
        }


        /// <summary>
        /// Body of post queries from TrenDAP
        /// </summary>
        public class Post
        {
            /// <summary>
            /// The acronym of the historian instance to query from
            /// </summary>
            public string Instance { get; set; }
            /// <summary>
            /// How the result is aggregated 1s,1m,1d,1w
            /// </summary>
            public string Aggregate { get; set; }
            /// <summary>
            /// List of device names to include in query results
            /// </summary>
            public string[] Devices { get; set; }
            /// <summary>
            /// List of Phases to include in query results
            /// </summary>
            public string[] Phases { get; set; }
            /// <summary>
            /// List of SignalTypes to include in query results
            /// </summary>
            public string[] Types { get; set; }
            /// <summary>
            /// Bit flag of valid Hours of the day to include in query results
            /// </summary>
            public ulong Hours { get; set; }
            /// <summary>
            /// Bit flag of valid Days of the week to include in query results
            /// </summary>
            public ulong Days { get; set; }
            /// <summary>
            /// Bit flag of valid Weeks of the year to include in query results
            /// </summary>
            public ulong Weeks { get; set; }
            /// <summary>
            /// Bit flag of valid Months of the year to include in query results
            /// </summary>
            public ulong Months { get; set; }
            /// <summary>
            /// Start time of query results
            /// </summary>
            public DateTime StartTime { get; set; }
            /// <summary>
            /// End time of query results
            /// </summary>
            public DateTime EndTime { get; set; }

        }

        /// <summary>
        /// Returns specific channel metadata associated with query.
        /// </summary>
        [HttpPost]
        public HttpResponseMessage QueryMetaData([FromBody] Post post)
        {
            DataTable table = GetTable(post);
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, table);
            stream.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

        private DataTable GetTable(Post post)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {

                string sql = @$"
                    SELECT * 
                    FROM ActiveMeasurement 
                    WHERE 
                        Device IN ({string.Join(",", post.Devices.Select(d => "'" + d + "'"))}) AND
                        Phase IN ({string.Join(",", post.Phases.Select(d => "'" + d + "'"))}) AND
                        SignalType IN ({string.Join(",", post.Types.Select(d => "'" + d + "'"))})
                    ";
                return connection.RetrieveData(sql);
            }
        }

        public class HistorianPoint { 
            public ulong PointID { get; set; }
            public float Value { get; set; }
            public DateTime Time { get; set; }
            public ulong Flags { get; set; }
        }

        [Serializable]
        public class HistorianAggregatePoint
        {
            public string Tag { get; set; }
            public string Timestamp { get; set; }
            public double Minimum { get; set; }
            public double Average { get; set; }
            public double Maximum { get; set; }
            public int Count { get; set; }
            public ulong QualityFlags { get; set; }
        }


        /// <summary>
        /// Returns specific channel metadata associated with query.
        /// </summary>
        [HttpPost]
        public HttpResponseMessage Query([FromBody] Post post)
        {
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

            DataTable measurements = GetTable(post);
            if (post.Instance == "None") return new HttpResponseMessage(HttpStatusCode.BadRequest);
            SnapServer server = GetAdapterInstance(post.Instance)?.Server?.Host;
            if (server == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            using (SnapClient connection = SnapClient.Connect(server))
            using (ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(post.Instance))
            {
                if (database == null)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);

                var timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(post.StartTime, post.EndTime);
                var pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(measurements.Select().Select(row => ulong.Parse(row["ID"].ToString().Split(':')[1])));

                using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter))
                {
                    HistorianKey key = new HistorianKey();
                    HistorianValue value = new HistorianValue();
                    List<HistorianPoint> points = new List<HistorianPoint>();
                    Dictionary<Tuple<ulong, string>, HistorianAggregatePoint> dict = new Dictionary<Tuple<ulong, string>, HistorianAggregatePoint>();

                    while (stream.Read(key, value))
                    {
                        ulong pointID = key.PointID;
                        ulong timestamp = key.Timestamp;
                        float pointValue = value.AsSingle;

                        if (((value.Value3 / 1) & 1) != 0) continue; // Bad Data
                        if (((value.Value3 / 65536) & 1) != 0) continue; // Bad Time
                        if (((value.Value3 / 4194304) & 1) != 0) continue; // Discarded Value
                        if (((value.Value3 / 2147483648) & 1) != 0) continue; // MeasurementError


                        DateTime timeStamp = epoch.AddMilliseconds((timestamp - (ulong)UnixTimeTag.BaseTicks.Value) / (double)Ticks.PerMillisecond);
                        int week = cultureInfo.Calendar.GetWeekOfYear(timeStamp, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

                        if (((ulong)(post.Hours / Math.Pow(2, (int)timeStamp.Hour)) & 1) == 0) continue;
                        if (((ulong)(post.Days / Math.Pow(2, (int)timeStamp.DayOfWeek)) & 1) == 0) continue;
                        if (((ulong)(post.Weeks / Math.Pow(2, week)) & 1) == 0) continue;
                        if (((ulong)(post.Months / Math.Pow(2, timeStamp.Month - 1)) & 1) == 0) continue;

                        Tuple<ulong, string> dictKey;
                        if (post.Aggregate == "1m")
                            dictKey = new Tuple<ulong, string>(pointID, timeStamp.ToString("yyyy-MM-ddTHH:mm:00Z"));
                        else if (post.Aggregate == "1h")
                            dictKey = new Tuple<ulong, string>(pointID, timeStamp.ToString("yyyy-MM-ddTHH:00:00Z"));
                        else if (post.Aggregate == "1d")
                            dictKey = new Tuple<ulong, string>(pointID, timeStamp.ToString("yyyy-MM-ddT00:00:00Z"));
                        else
                            dictKey = new Tuple<ulong, string>(pointID, timeStamp.ToString("yyyy-MM-01T00:00:00Z"));


                        if (dict.ContainsKey(dictKey)){
                            HistorianAggregatePoint point = dict[dictKey];
                            point.Maximum = point.Maximum < pointValue ? pointValue : point.Maximum;
                            point.Minimum = point.Minimum > pointValue ? pointValue : point.Minimum;
                            point.Average = point.Average * point.Count + pointValue;
                            point.QualityFlags = point.QualityFlags | value.Value3;
                            point.Count += 1;
                            point.Average = point.Average / point.Count;
                            dict[dictKey] = point;
                        }
                        else {
                            HistorianAggregatePoint point = new HistorianAggregatePoint()
                            {
                                Tag = $"{post.Instance}:{pointID}",
                                Timestamp = dictKey.Item2,
                                Maximum = pointValue,
                                Minimum = pointValue,
                                Average = pointValue,
                                Count = 1,
                                QualityFlags = value.Value3
                            };
                            dict.Add(dictKey, point);
                        }
                    }

                    List<HistorianAggregatePoint> results = dict.Values.ToList();

                    HttpResponseMessage response = new HttpResponseMessage();
                    response.StatusCode = HttpStatusCode.OK;
                    response.Content = new StringContent(JsonConvert.SerializeObject(results));
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    return response;

                }



            }

        }


        

        private static LocalOutputAdapter GetAdapterInstance(string instanceName)
        {
            if (!string.IsNullOrWhiteSpace(instanceName))
            {
                if (LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter adapterInstance))
                    return adapterInstance;
            }

            return null;
        }


        #endregion
    }
}
