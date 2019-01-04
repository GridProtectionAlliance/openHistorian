//******************************************************************************************************
//  eDNAGrafanaController.cs - Gbtc
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

using GrafanaAdapters;
using GSF;
using GSF.Diagnostics;
using GSF.TimeSeries;
using InStep.eDNA.EzDNAApiNet;
using Newtonsoft.Json;
using openHistorian.Adapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using eDNAMetaData = eDNAAdapters.Metadata;
namespace openHistorian.EdnaGrafanaController
{
    /// <summary>
    /// Represents a REST based API for a simple JSON based Grafana "phasor" based data source,
    /// accessible from Grafana data source as http://localhost:8180/api/ednagrafana
    /// </summary>
    public class EdnaGrafanaController: ApiController
    {
        #region [ Members ]

        // Nested Types

        /// <summary>
        /// Represents a historian data source for the Grafana adapter.
        /// </summary>
        protected class eDNADataSource : GrafanaDataSourceBase
        {

            /// <summary>
            /// Starts a query that will read data source values, given a set of point IDs and targets, over a time range.
            /// </summary>
            /// <param name="startTime">Start-time for query.</param>
            /// <param name="stopTime">Stop-time for query.</param>
            /// <param name="interval">Interval from Grafana request.</param>
            /// <param name="decimate">Flag that determines if data should be decimated over provided time range.</param>
            /// <param name="targetMap">Set of IDs with associated targets to query.</param>
            /// <returns>Queried data source data in terms of value and time.</returns>
            protected override IEnumerable<DataSourceValue> QueryDataSourceValues(DateTime startTime, DateTime stopTime, string interval, bool decimate, Dictionary<ulong, string> targetMap)
            {
                foreach (string point in targetMap.Values)
                {
                    int[] expectedResults =
                    {
                        (int)eDNAHistoryReturnStatus.END_OF_HISTORY,
                        (int)eDNAHistoryReturnStatus.NO_HISTORY_FOR_TIME
                    };

                    int result = History.DnaGetHistRaw(point, startTime.ToLocalTime(), stopTime.ToLocalTime(), out uint key);

                    while (result == 0)
                    {
                        result = History.DnaGetNextHist(key, out double value, out DateTime time, out string status);
                        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                        if (result == 0)
                            yield return new DataSourceValue() {
                                Target = point,
                                Time = time.Subtract(epoch).TotalMilliseconds,
                                Value = value,
                                Flags = MeasurementStateFlags.Normal
                            };
                    }

                    // Assume that unexpected return status indicates an error
                    // and therefore the analysis results should be trusted
                    if (expectedResults.Contains(result))
                    {
                        Log.Publish(MessageLevel.Error, "eDNA Error", $"Unexpected eDNA return code: {result}");
                        break;
                    }
                }
            }

            #region [ Methods ]

            #endregion  
        }


        // Fields
        private string m_defaultApiPath;

        #endregion

        #region [ Static ]
        /// <summary>
        /// Used to store the metadata from enda
        /// </summary>
        public static DataSet MetaData { get; private set; }
        //private static Dictionary<string, string> PointTagToShortID { get; set; }

        static EdnaGrafanaController()
        {
            //PointTagToShortID = new Dictionary<string, string>();
            InitializeMetaData();

            Task.Run(() => RefreshMetaData());
        }

        static private void InitializeMetaData() {
            DataTable dataTable = GetNewMetaDataTable();
            DataSet metaData = new DataSet();
            metaData.Tables.Add(dataTable);
            MetaData = metaData;
        }

        static private void RefreshMetaData()
        {

                DataTable dataTable = GetNewMetaDataTable();
                IEnumerable<eDNAMetaData> results = eDNAMetaData.Query(new eDNAMetaData() { Site = "TP", Service = "MAIN"});

//                dataTable.Rows.Add(
//                    Guid.NewGuid(), //node ID
//                    Guid.NewGuid(),  // source node
//                    "test:1",  // ID
//                    Guid.NewGuid(), // signal id 
//                    "test_1", // point tag
//                    "NULL", // alternate tag
//                    "NULL", // signal ref
//                    "true", // internal
//                    "false", // subscribed
//                    "NULL", // device
//                    "NULL", // deviceid
//                    1, // frames per sec
//                    "NULL", // protocol
//                    "NULL", // signal type
//                    "Vp;ts", // enineering units
//                    "NULL", // phasor id
//                    "NULL", // phasor type
//                    "NULL", // phase
//                    0, // adder
//                    1, // multiplier
//                    "NULL", // company
//                    0.0F, // long
//                    0.0F, // lat
//                    "", //desc 
//                    DateTime.UtcNow // updated on
//                );

//                dataTable.Rows.Add(
//    Guid.NewGuid(), //node ID
//    Guid.NewGuid(),  // source node
//    "test:2",  // ID
//    Guid.NewGuid(), // signal id 
//    "test_2", // point tag
//    "NULL", // alternate tag
//    "NULL", // signal ref
//    "true", // internal
//    "false", // subscribed
//    "NULL", // device
//    "NULL", // deviceid
//    1, // frames per sec
//    "NULL", // protocol
//    "NULL", // signal type
//    "Vp;ts", // enineering units
//    "NULL", // phasor id
//    "NULL", // phasor type
//    "NULL", // phase
//    0, // adder
//    1, // multiplier
//    "NULL", // company
//                    0.0F, // long
//                    0.0F, // lat
//    "", //desc 
//    DateTime.UtcNow // updated on
//);

                foreach (eDNAMetaData result in results)
                {
                    if (result.ExtendedDescription == string.Empty) continue;
    

                    string fqn = $"{result.Site}.{result.Service}.{result.ShortID}";
                    string id = $"edna_{result.Site}_{result.Service}:{result.ShortID}";

                    try
                    {
                        dataTable.Rows.Add(
                            Guid.NewGuid(), //node ID
                            Guid.NewGuid(),  // source node
                            id,  // ID
                            Guid.NewGuid(), // signal id 
                            fqn, // point tag
                            "NULL", // alternate tag
                            "NULL", // signal ref
                            "true", // internal
                            "false", // subscribed
                            "NULL", // device
                            "NULL", // deviceid
                            1, // frames per sec
                            "NULL", // protocol
                            "NULL", // signal type
                            result.Units, // enineering units
                            "NULL", // phasor id
                            "NULL", // phasor type
                            "NULL", // phase
                            0, // adder
                            1, // multiplier
                            "NULL", // company
                            0.0F, // long
                            0.0F, // lat
                            result.ExtendedDescription, //desc 
                            DateTime.UtcNow // updated on
                        );
                    }
                    catch (Exception ex)
                    {
                        Log.Publish(MessageLevel.Error, "eDNA Controller Metadata load error", exception: ex);
                    }
                }

                DataSet metaData = new DataSet();
                metaData.Tables.Add(dataTable);
                MetaData = metaData;
        }

        static private DataTable GetNewMetaDataTable()
        {
            DataTable dataTable = new DataTable("ActiveMeasurements");
            dataTable.Columns.Add("NodeID", typeof(Guid));
            dataTable.Columns.Add("SourceNodeID", typeof(Guid));
            dataTable.Columns.Add("ID", typeof(string));
            dataTable.Columns.Add("SignalID", typeof(Guid));
            dataTable.Columns.Add("PointTag", typeof(string));
            dataTable.Columns.Add("AlternateTag", typeof(string));
            dataTable.Columns.Add("SignalReference", typeof(string));
            dataTable.Columns.Add("Internal", typeof(string));
            dataTable.Columns.Add("Subscribed", typeof(string));
            dataTable.Columns.Add("Device", typeof(string));
            dataTable.Columns.Add("DeviceID", typeof(string));
            dataTable.Columns.Add("FramesPerSecond", typeof(int));
            dataTable.Columns.Add("Protocol", typeof(string));
            dataTable.Columns.Add("SignalType", typeof(string));
            dataTable.Columns.Add("EngineeringUnits", typeof(string));
            dataTable.Columns.Add("PhasorID", typeof(string));
            dataTable.Columns.Add("PhasorType", typeof(string));
            dataTable.Columns.Add("Phase", typeof(string));
            dataTable.Columns.Add("Adder", typeof(int));
            dataTable.Columns.Add("Multiplier", typeof(int));
            dataTable.Columns.Add("Company", typeof(string));
            dataTable.Columns.Add("Longitude", typeof(float));
            dataTable.Columns.Add("Latitude", typeof(float));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("UpdatedOn", typeof(DateTime));

            return dataTable;
        }
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
        /// Gets historian data source for this Grafana adapter.
        /// </summary>
        protected eDNADataSource DataSource
        {
            get
            {
                return new eDNADataSource
                {
                    InstanceName = "eDNA",
                    Metadata = MetaData
                };
            }
        }

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
        /// Queries openHistorian as a Grafana data source.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<List<TimeSeriesValues>> Query(QueryRequest request, CancellationToken cancellationToken)
        {
            if (request.targets.FirstOrDefault()?.target == null)
                return Task.FromResult(new List<TimeSeriesValues>());

            return DataSource?.Query(request, cancellationToken) ?? Task.FromResult(new List<TimeSeriesValues>());
        }

        /// <summary>
        /// Queries openHistorian as a Grafana Metadata source.
        /// </summary>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<string> GetMetadata(Target request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(request.target))
                    return string.Empty;

                DataTable table = new DataTable();
                DataRow[] rows = DataSource?.Metadata.Tables["ActiveMeasurements"].Select($"PointTag IN ({request.target})") ?? new DataRow[0];

                if (rows.Length > 0)
                    table = rows.CopyToDataTable();

                return JsonConvert.SerializeObject(table);
            },
            cancellationToken);
        }

        /// <summary>
        /// Search openHistorian for a target.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public string[] Search(Target request)
        {
            return DataSource.Metadata.Tables["ActiveMeasurements"].Select($"PointTag LIKE '%{request.target}%'").Take(DataSource.MaximumSearchTargetsPerRequest).Select(row => $"{row["PointTag"]}").ToArray();

        }

        /// <summary>
        /// Search openHistorian for a field.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public string[] SearchFields(Target request)
        {
            return DataSource.Metadata.Tables["ActiveMeasurements"].Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
        }

        /// <summary>
        /// Search openHistorian for a table.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public string[] SearchFilters(Target request)
        {
            return new List<string>() { "ActiveMeasurements"}.ToArray();
        }

        /// <summary>
        /// Search openHistorian for a field.
        /// </summary>
        /// <param name="request">Search target.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public string[] SearchOrderBys(Target request)
        {
            return DataSource.Metadata.Tables["ActiveMeasurements"].Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
        }

        /// <summary>
        /// Queries openHistorian for annotations in a time-range (e.g., Alarms).
        /// </summary>
        /// <param name="request">Annotation request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public List<AnnotationResponse> Annotations(AnnotationRequest request, CancellationToken cancellationToken)
        {
            return new List<AnnotationResponse>();
        }

        #endregion

        #region [ static ]
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(EdnaGrafanaController), MessageClass.Component);
        #endregion
    }
}
