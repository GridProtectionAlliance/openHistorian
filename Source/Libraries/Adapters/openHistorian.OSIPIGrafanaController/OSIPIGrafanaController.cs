//******************************************************************************************************
//  OSIPIGrafanaController.cs - Gbtc
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
//  01/17/2019 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GrafanaAdapters;
using GrafanaAdapters.DataSources;
using GSF;
using GSF.TimeSeries;
using Newtonsoft.Json;
using OSIsoft.AF.Asset;
using OSIsoft.AF.PI;
using OSIsoft.AF.Time;
using PIAdapters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using GrafanaAdapters.Functions;
using GrafanaAdapters.Model.Annotations;
using GrafanaAdapters.Model.Common;

namespace openHistorian.OSIPIGrafanaController
{
    /// <summary>
    /// Represents a REST based API for a simple JSON based Grafana data source for OSIsoft PI,
    /// accessible from Grafana data source as http://localhost:8180/api/pigrafana/{instance}/{serverName}
    /// </summary>
    /// <remarks>
    /// <para>
    /// This adapter assumes that a PIOutputAdapter is being used to synchronize metadata and send data
    /// to PI, this way the adapter does not query the PI database for its metadata - which can be slow.
    /// Instead the adapter uses the locally accessible cached metadata, which is synchronized with PI,
    /// for Grafana queries. Because of this, the OSIPIGrafanaController is linked to its parent
    /// PIOutputAdapter instance by the "ServerName" connection string parameter, which becomes a query
    /// parameter of the OSIPIGrafanaController URL route template.
    /// </para>
    /// <para>
    /// One benefit of this pass-through architecture means that the openHistorian Grafana interface will
    /// be utilized which includes all the time-series functions provided by the GrafanaDataSourceBase:
    /// https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md
    /// </para>
    /// <para>
    /// Future versions of this adapter could try querying OSI-PI metadata directly so that an independent,
    /// i.e., a PI instance that is not being fed by PIOutputAdapter, could be used. However, the OSI-PI
    /// Grafana adapter that uses PIWebAPI is already available for this purpose.
    /// </para>
    /// </remarks>
    public class OSIPIGrafanaController : ApiController
    {
        #region [ Members ]

        // Nested Types

        /// <summary>
        /// Represents an OSI-PI data source for the Grafana adapter.
        /// </summary>
        [Serializable]
        protected class OSIPIDataSource : GrafanaDataSourceBase
        {
            #region [ Members ]

            private readonly long m_baseTicks = UnixTimeTag.BaseTicks.Value;

            /// <summary>
            /// KeyName for data source cache.
            /// </summary>
            public string KeyName;

            /// <summary>
            /// Tag name prefix remove count.
            /// </summary>
            public int PrefixRemoveCount;

            /// <summary>
            /// Current OSI-PI connection.
            /// </summary>
            public PIConnection Connection;

            #endregion

            #region [ Methods ]

            /// <summary>
            /// Starts a query that will read data source values, given a set of point IDs and targets, over a time range.
            /// </summary>
            /// <param name="queryParameters">Parameters that define the query.</param>
            /// <param name="targetMap">Set of IDs with associated targets to query.</param>
            /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
            /// <returns>Queried data source data in terms of value and time.</returns>
            protected override async IAsyncEnumerable<DataSourceValue> QueryDataSourceValues(QueryParameters queryParameters, Dictionary<ulong, string> targetMap, CancellationToken cancellationToken)
            {
                Dictionary<int, ulong> idMap = new();
                PIPointList points = new();

                foreach (KeyValuePair<ulong, string> target in targetMap)
                {
                    ulong metadataID = target.Key;
                    string pointTag = target.Value;

                    if (!MetadataIDToPIPoint.TryGetValue(metadataID, out PIPoint point))
                        if (TryFindPIPoint(Connection, Metadata, GetPITagName(pointTag, PrefixRemoveCount), out point))
                            MetadataIDToPIPoint[metadataID] = point;

                    if (point is not null)
                    {
                        points.Add(point);
                        idMap[point.ID] = metadataID;
                    }
                }

                // Start data read from historian
                await foreach (AFValue currentPoint in ReadData(queryParameters.StartTime, queryParameters.StopTime, points).ToAsyncEnumerable().WithCancellation(cancellationToken))
                {
                    if (currentPoint is null)
                        continue;

                    yield return new DataSourceValue
                    {
                        Target = targetMap[idMap[currentPoint.PIPoint.ID]],
                        Time = (currentPoint.Timestamp.UtcTime.Ticks - m_baseTicks) / (double)Ticks.PerMillisecond,
                        Value = Convert.ToDouble(currentPoint.Value),
                        Flags = ConvertStatusFlags(currentPoint.Status)
                    };
                }
            }

            private IEnumerable<AFValue> ReadData(AFTime startTime, AFTime endTime, PIPointList points)
            {
                try
                {
                    return new TimeSortedValueScanner
                    {
                        Points = points,
                        StartTime = startTime,
                        EndTime = endTime
                        //DataReadExceptionHandler = ex => OnProcessException(MessageLevel.Warning, ex)
                    }
                    .Read();
                }
                catch
                {
                    // Removed cached data source on read failure
                    if (DataSources.TryRemove(KeyName, out _))
                        MetadataIDToPIPoint.Clear();

                    throw;
                }
            }

            private MeasurementStateFlags ConvertStatusFlags(AFValueStatus status)
            {
                MeasurementStateFlags flags = MeasurementStateFlags.Normal;

                if ((status & AFValueStatus.Bad) > 0)
                    flags |= MeasurementStateFlags.BadData;

                if ((status & AFValueStatus.Questionable) > 0)
                    flags |= MeasurementStateFlags.SuspectData;

                if ((status & AFValueStatus.BadSubstituteValue) > 0)
                    flags |= MeasurementStateFlags.CalculationError | MeasurementStateFlags.BadData;

                if ((status & AFValueStatus.UncertainSubstituteValue) > 0)
                    flags |= MeasurementStateFlags.CalculationError | MeasurementStateFlags.SuspectData;

                if ((status & AFValueStatus.Substituted) > 0)
                    flags |= MeasurementStateFlags.CalculatedValue;

                return flags;
            }

            #endregion
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Validates that openHistorian Grafana data source is responding as expected.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Index(string instanceName, string serverName)
        {
            if (DataSource(instanceName, serverName) is null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Queries OSI-PI as a Grafana data source.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="serverName">OSI-PI server name.</param>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<IEnumerable<TimeSeriesValues>> Query(string instanceName, string serverName, QueryRequest request, CancellationToken cancellationToken)
        {
            if (request.targets.FirstOrDefault()?.target is null)
                return Task.FromResult(Enumerable.Empty<TimeSeriesValues>());

            return DataSource(instanceName, serverName)?.Query(request, cancellationToken) ?? Task.FromResult(Enumerable.Empty<TimeSeriesValues>());
        }

        /// <summary>
        /// Queries OSI-PI as a Grafana Metadata source.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="serverName">OSI-PI server name.</param>
        /// <param name="request">Query request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public virtual Task<string> GetMetadata(string instanceName, string serverName, Target request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(request.target))
                    return string.Empty;

                DataTable table = new();
                DataRow[] rows = DataSource(instanceName, serverName)?.Metadata.Tables["ActiveMeasurements"].Select($"PointTag IN ({request.target})") ?? new DataRow[0];

                if (rows.Length > 0)
                    table = rows.CopyToDataTable();

                return JsonConvert.SerializeObject(table);
            },
            cancellationToken);
        }

        /// <summary>
        /// Search OSI-PI for a target.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="serverName">OSI-PI server name.</param>
        /// <param name="request">Search target.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public Task<string[]> Search(string instanceName, string serverName, Target request, CancellationToken cancellationToken)
        {
            return DataSource(instanceName, serverName)?.Search(request, cancellationToken) ?? Task.FromResult(new string[0]);
        }

      
        /// <summary>
        /// Queries OSI-PI for annotations in a time-range (e.g., Alarms).
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="serverName">OSI-PI server name.</param>
        /// <param name="request">Annotation request.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        [HttpPost]
        [SuppressMessage("Security", "SG0016", Justification = "Current operation dictated by Grafana. CSRF exposure limited to data access.")]
        public Task<List<AnnotationResponse>> Annotations(string instanceName, string serverName, AnnotationRequest request, CancellationToken cancellationToken)
        {
            return DataSource(instanceName, serverName)?.Annotations(request, cancellationToken) ?? Task.FromResult(new List<AnnotationResponse>());
        }

     
        /// <summary>
        /// Gets OSI-PI data source for this Grafana adapter.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="serverName">OSI-PI server name.</param>
        private OSIPIDataSource DataSource(string instanceName, string serverName)
        {
            string keyName = $"{instanceName}.{serverName}";

            // Don't GetOrAdd here - don't want a possible cached Null value
            if (DataSources.TryGetValue(keyName, out OSIPIDataSource dataSource))
                return dataSource;

            // If PI is not connected, keep trying to connect by creating a new data source
            dataSource = CreateNewDataSource(keyName);

            if (dataSource != null)
                DataSources[keyName] = dataSource;

            return dataSource;
        }

        private OSIPIDataSource CreateNewDataSource(string keyName)
        {
            string[] parts = keyName.Split('.');
            string instanceName = parts[0];
            string serverName = parts[1];

            if (!PIOutputAdapter.Instances.TryGetValue(serverName, out PIOutputAdapter adapterInstance) || !adapterInstance.Initialized || adapterInstance.IsDisposed)
                return null;

            DataSet metadata = adapterInstance.DataSource;

            OSIPIDataSource dataSource = new()
            {
                InstanceName = instanceName,
                Metadata = metadata,
                KeyName = keyName,
                PrefixRemoveCount = adapterInstance.TagNamePrefixRemoveCount,
                Connection = new PIConnection()
            };

            dataSource.Connection.ServerName = serverName;
            dataSource.Connection.UserName = adapterInstance.UserName;
            dataSource.Connection.Password = adapterInstance.Password;
            dataSource.Connection.ConnectTimeout = adapterInstance.ConnectTimeout;
            dataSource.Connection.Open();

            // On successful connection, kick off a thread to start meta-data ID to OSI-PI point ID mapping
            new Thread(_ =>
            {
                foreach (DataRow row in metadata.Tables["ActiveMeasurements"].Rows)
                {
                    string[] idParts = row["ID"].ToString().Split(':');

                    if (idParts.Length != 2)
                        continue;

                    ulong metadataID = ulong.Parse(idParts[1]);

                    if (!MetadataIDToPIPoint.TryGetValue(metadataID, out PIPoint point))
                        if (TryFindPIPoint(dataSource.Connection, GetPITagName(row["PointTag"].ToString(), dataSource.PrefixRemoveCount), row["AlternateTag"].ToString(), out point))
                            MetadataIDToPIPoint[metadataID] = point;
                }
            })
            .Start();

            return dataSource;
        }

        #endregion

        #region [ Static ]

        private static readonly ConcurrentDictionary<string, OSIPIDataSource> DataSources = new();
        private static readonly ConcurrentDictionary<ulong, PIPoint> MetadataIDToPIPoint = new();

        // Static Methods
        private static bool TryFindPIPoint(PIConnection connection, DataSet metadata, string pointTag, out PIPoint point)
        {
            DataRow[] rows = metadata.Tables["ActiveMeasurements"].Select($"PointTag = '{pointTag}'");

            if (rows.Length <= 0)
            {
                point = null;
                return false;
            }

            return TryFindPIPoint(connection, pointTag, rows[0]["AlternateTag"].ToString(), out point);
        }

        private static bool TryFindPIPoint(PIConnection connection, string pointTag, string alternateTag, out PIPoint point)
        {
            return PIPoint.TryFindPIPoint(connection.Server, string.IsNullOrWhiteSpace(alternateTag) ? pointTag : alternateTag, out point);
        }

        private static string GetPITagName(string tagName, int prefixRemoveCount)
        {
            if (prefixRemoveCount < 1)
                return tagName;

            for (int i = 0; i < prefixRemoveCount; i++)
            {
                int prefixIndex = tagName.IndexOf('!');

                if (prefixIndex > -1 && prefixIndex + 1 < tagName.Length)
                    tagName = tagName.Substring(prefixIndex + 1);
                else
                    break;
            }

            return tagName;
        }

        #endregion
    }
}