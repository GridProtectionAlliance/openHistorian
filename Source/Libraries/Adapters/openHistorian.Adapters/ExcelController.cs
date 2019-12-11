//******************************************************************************************************
//  ExcelController.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  12/05/2019 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using GrafanaAdapters;
using GSF;
using GSF.Data;
using GSF.Data.Model;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.TimeSeries;
using Newtonsoft.Json;
using openHistorian.Model;
using openHistorian.Snap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CancellationToken = System.Threading.CancellationToken;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a REST based API for a simple JSON based Excel data source.
    /// </summary>
    public class ExcelController : ApiController
    {
        #region [ Members ]

        // Nested Types

        /// <summary>
        /// Represents a DataPoint for the Excel Add-in.
        /// </summary>
        protected class DataPoint
        {
            /// <summary>
            /// Value of the point.
            /// </summary>
            public double Value;

            /// <summary>
            /// PointTag.
            /// </summary>
            public string PointTag;

            /// <summary>
            /// TimeStamp in Ticks.
            /// </summary>
            public ulong TimeStamp;
        }

        /// <summary>
        /// Represents a request for Data from the Excel Add-in.
        /// Note that a similar class exists in the Excel Add-in.
        /// </summary>
        protected class DataRequest
        {
            public DateTime StartTime;
            public DateTime EndTime;
            public List<string> PointTags;
        }

        /// <summary>
        /// Represents a historian data source for the Excel Add-in.
        /// Note that Data is returned as a 2D List to write to Excel
        /// </summary>
        protected class HistorianDataSource
        {
            private readonly ulong m_baseTicks = (ulong)UnixTimeTag.BaseTicks.Value;

            /// <summary>
            /// Gets or sets instance name for this ExcelController.HistorianDataSource implementation.
            /// </summary>
            public string InstanceName { get; set; }

            /// <summary>
            /// Starts a query that will read data source values, given a set of point IDs and targets, over a time range.
            /// </summary>
            /// <param name="startTime">Start-time for query.</param>
            /// <param name="stopTime">Stop-time for query.</param>
            /// <param name="points">Set of IDs with associated targets to query.</param>
            /// <returns>Queried data source data as a List of Dictionaries with TS and pointTag.</returns>
            public IEnumerable<List<DataPoint>> QueryDataSourceValues(DateTime startTime, DateTime stopTime,  Dictionary<ulong,string> points)
            {
                SnapServer server = GetAdapterInstance(InstanceName)?.Server?.Host;

                if (server == null)
                    yield break;

                using (SnapClient connection = SnapClient.Connect(server))
                using (ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(InstanceName))
                {
                    if (database == null)
                        yield break;

                    

                    // Set data scan resolution
                    SeekFilterBase<HistorianKey>  timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime);

                    // Setup point ID selections
                    MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(points.Keys);

                    // Start stream reader for the provided time window and selected points
                    using (TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter))
                    {
                        HistorianKey key = new HistorianKey();
                        HistorianValue value = new HistorianValue();
                        stream.Read(key, value);
                        ulong currentTS = key.Timestamp - m_baseTicks;
                        
                        List<List<DataPoint>> values = new List<List<DataPoint>>();
                        if (points.Keys.Contains(key.PointID))
                        {
                            values.Add(new List<DataPoint>() {
                            new DataPoint
                            {
                                TimeStamp = currentTS,
                                PointTag = points[key.PointID],
                                Value = value.AsSingle
                            }
                        });
                        }
                        else
                        {
                            values.Add(new List<DataPoint>());
                        }

                        int index = 0;

                        while (stream.Read(key, value))
                        {
                            if (!points.Keys.Contains(key.PointID))
                                continue;

                            if ((key.Timestamp - m_baseTicks) - currentTS > 10)
                            {
                                currentTS = (key.Timestamp - m_baseTicks);

                                values.Add(new List<DataPoint>() {
                                    new DataPoint
                                    {
                                        TimeStamp = currentTS,
                                        PointTag = points[key.PointID],
                                        Value = value.AsSingle
                                    }
                                });

                                yield return values[index];

                                index++;
                            }
                            else
                            {
                                values[index].Add(
                                    new DataPoint
                                    {
                                        TimeStamp = currentTS,
                                        PointTag = points[key.PointID],
                                        Value = value.AsSingle
                                    });
                            }
                          
                        }
                    }
                }
            }
        }




        private string m_defaultApiPath;
        private HistorianDataSource m_dataSource;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the default API path string for this controller.
        /// </summary>
        protected virtual string DefaultApiPath
        {
            get
            {
                if (!string.IsNullOrEmpty(m_defaultApiPath))
                    return m_defaultApiPath;

                string controllerName = GetType().Name.ToLowerInvariant();

                if (controllerName.EndsWith("controller") && controllerName.Length > 10)
                    controllerName = controllerName.Substring(0, controllerName.Length - 10);

                m_defaultApiPath = $"/api/{controllerName}";

                return m_defaultApiPath;
            }
        }

        /// <summary>
        /// Gets historian data source for this Excel-addin adapter.
        /// </summary>
        protected HistorianDataSource DataSource
        {
            get
            {
                if (m_dataSource != null)
                    return m_dataSource;

                string uriPath = Request.RequestUri.PathAndQuery;
                string instanceName;

                if (uriPath.StartsWith(DefaultApiPath, StringComparison.OrdinalIgnoreCase))
                {
                    // No instance provided in URL, use default instance name
                    instanceName = TrendValueAPI.DefaultInstanceName;
                }
                else
                {
                    string[] pathElements = uriPath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                    if (pathElements.Length > 2)
                        instanceName = pathElements[1].Trim();
                    else
                        throw new InvalidOperationException($"Unexpected API URL route destination encountered: {Request.RequestUri}");
                }

                if (!string.IsNullOrWhiteSpace(instanceName))
                {
                    LocalOutputAdapter adapterInstance = GetAdapterInstance(instanceName);

                    if (adapterInstance != null)
                    {
                        m_dataSource = new HistorianDataSource
                        {
                            InstanceName = instanceName,
                        };
                    }
                }

                return m_dataSource;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Validates that openHistorian Excel data source is responding as expected.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Index()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Validates that this is a valid Endpoint for the Excel Addin.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public virtual Task<bool> TestConnection(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                return true;
            },
            cancellationToken);
        }

        /// <summary>
        /// Queries openHistorian as an Excel data source for PointTags.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public virtual Task<List<string>> QueryPointTags(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                string filter = request.Content.ReadAsStringAsync().Result;
                List<string> result = new List<string>();
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    TableOperations<ActiveMeasurement> measurementTable = new TableOperations<ActiveMeasurement>(connection);
                    if (filter == "")
                        result = measurementTable.QueryRecords().Select(item => item.PointTag).ToList();
                    else
                        result = measurementTable.QueryRecordsWhere("PointTag Like ('%' + {0} + '%')", filter).Select(item => item.PointTag).ToList();
                }
                return result;
            },
            cancellationToken);
        }

        /// <summary>
        /// Queries openHistorian as an Excel data source for Points at a single TS.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public virtual Task<List<double>> QueryValues(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                
                JsonSerializer serializer = JsonSerializer.CreateDefault();
                DataRequest requestData = serializer.Deserialize<DataRequest>(new JsonTextReader(new StreamReader(request.Content.ReadAsStreamAsync().Result)));


                List<double> result = new List<double>();
                Dictionary<ulong, string> keys = new Dictionary<ulong, string>();

                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    foreach (string point in requestData.PointTags)
                    {
                        TableOperations<ActiveMeasurement> measurementTable = new TableOperations<ActiveMeasurement>(connection);
                        if (measurementTable.QueryRecordCountWhere("PointTag LIKE {0}", point) > 0)
                            keys.Add(measurementTable.QueryRecordWhere("PointTag LIKE {0}", point).PointID, point);
                    }
                }

                List<DataPoint> data = DataSource.QueryDataSourceValues(requestData.StartTime, requestData.EndTime, keys).ToList()[0];

                foreach (string point in requestData.PointTags)
                {
                    if (data.Where(item => item.PointTag == point).Count() == 1)
                        result.Add(data.Where(item => item.PointTag == point).First().Value);
                    else
                        result.Add(double.NaN);
                }
                return result;
            },
            cancellationToken);
        }

        /// <summary>
        /// Queries openHistorian as an Excel data source for Points in a time range.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        public virtual Task<List<List<double>>> QuerySeries(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {

                JsonSerializer serializer = JsonSerializer.CreateDefault();
                DataRequest requestData = serializer.Deserialize<DataRequest>(new JsonTextReader(new StreamReader(request.Content.ReadAsStreamAsync().Result)));


                List<List<double>> result = new List<List<double>>();
                Dictionary<ulong, string> keys = new Dictionary<ulong, string>();

                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    foreach (string point in requestData.PointTags)
                    {
                        TableOperations<ActiveMeasurement> measurementTable = new TableOperations<ActiveMeasurement>(connection);
                        if (measurementTable.QueryRecordCountWhere("PointTag LIKE {0}", point) > 0)
                            keys.Add(measurementTable.QueryRecordWhere("PointTag LIKE {0}", point).PointID, point);
                    }
                }

                List<List<DataPoint>> data = DataSource.QueryDataSourceValues(requestData.StartTime, requestData.EndTime, keys).ToList();

                int index = 0;
                foreach (List<DataPoint> dataSlice in data)
                {
                    result.Add(new List<double>() { (double)dataSlice[0].TimeStamp / Ticks.PerDay });
                    foreach (string point in requestData.PointTags)
                    {
                        if (dataSlice.Where(item => item.PointTag == point).Count() == 1)
                            result[index].Add(dataSlice.Where(item => item.PointTag == point).First().Value);
                        else
                            result[index].Add(double.NaN);
                    }
                    index++;
                }
                return result;
            },
            cancellationToken);
        }


        #endregion

        #region [ Static ]

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
