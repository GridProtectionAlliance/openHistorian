//******************************************************************************************************
//  ExportDataHandler.ashx.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  07/29/2016 - Billy Ernest
//       Generated original version of source code.
//  08/10/2016 - J. Ritchie Carroll
//       Combined ASP.NET and self-hosted handlers into a single shared embedded resource.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using GSF;
using GSF.Collections;
using GSF.Data.Model;
using GSF.Security;
using GSF.TimeSeries;
using GSF.Web.Hosting;
using GSF.Web.Hubs;
using GSF.Web.Model;
using openHistorian.Adapters;
using Measurement = openHistorian.Model.Measurement;

// ReSharper disable once CheckNamespace
// ReSharper disable NotResolvedInText
namespace openHistorian
{
    /// <summary>
    /// Handles downloading of exported historian data.
    /// </summary>
    public class ExportDataHandler : IHostedHttpHandler
    {
        #region [ Members ]

        // Constants
        private const string CsvContentType = "text/csv";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if client cache should be enabled for rendered handler content.
        /// </summary>
        /// <remarks>
        /// If rendered handler content does not change often, the server and client will use the
        /// <see cref="IHostedHttpHandler.GetContentHash"/> to determine if the client needs to refresh the content.
        /// </remarks>
        public bool UseClientCache => false;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets hash of response content based on any <paramref name="request"/> parameters.
        /// </summary>
        /// <param name="request">HTTP request message.</param>
        /// <remarks>
        /// Value is only used when <see cref="IHostedHttpHandler.UseClientCache"/> is <c>true</c>.
        /// </remarks>
        public long GetContentHash(HttpRequestMessage request) => 0;

        /// <summary>
        /// Enables processing of HTTP web requests by a custom handler that implements the <see cref="IHostedHttpHandler"/> interface.
        /// </summary>
        /// <param name="request">HTTP request message.</param>
        /// <param name="response">HTTP response message.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        public Task ProcessRequestAsync(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            NameValueCollection requestParameters = request.RequestUri.ParseQueryString();

            response.Content = new PushStreamContent((stream, content, context) => 
            {
                try
                {
                    CopyModelAsCsvToStream(requestParameters, stream, cancellationToken);
                }
                finally
                {
                    stream.Close();
                }
            }, new MediaTypeHeaderValue(CsvContentType));

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = requestParameters["FileName"] ?? "Export.csv"
            };

            return Task.CompletedTask;
        }

        private void CopyModelAsCsvToStream(NameValueCollection requestParameters, Stream responseStream, CancellationToken cancellationToken)
        {
            const int DefaultFrameRate = 30;

            SecurityProviderCache.ValidateCurrentProvider();
            string dateTimeFormat = Program.Host.Model.Global.DateTimeFormat;

            string pointIDs = requestParameters["PointIDs"];
            string startTime = requestParameters["StartTime"];
            string endTime = requestParameters["EndTime"];
            string frameRate = requestParameters["FrameRate"];

            if (string.IsNullOrEmpty(pointIDs))
                throw new ArgumentNullException("PointIDs", "Cannot export data: no values were provided in \"PointIDs\" parameter.");

            if (string.IsNullOrEmpty(startTime))
                throw new ArgumentNullException("StartTime", "Cannot export data: no \"StartTime\" parameter value was specified.");

            if (string.IsNullOrEmpty(pointIDs))
                throw new ArgumentNullException("EndTime", "Cannot export data: no \"EndTime\" parameter value was specified.");

            DateTime exportStartTime, exportEndTime;

            try
            {
                exportStartTime = DateTime.ParseExact(startTime, dateTimeFormat, null, DateTimeStyles.AdjustToUniversal);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Cannot export data: failed to parse \"StartTime\" parameter value \"{startTime}\". Expected format is \"{dateTimeFormat}\". Error message: {ex.Message}", "StartTime", ex);
            }

            try
            {
                exportEndTime = DateTime.ParseExact(endTime, dateTimeFormat, null, DateTimeStyles.AdjustToUniversal);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Cannot export data: failed to parse \"EndTime\" parameter value \"{endTime}\". Expected format is \"{dateTimeFormat}\". Error message: {ex.Message}", "EndTime", ex);
            }

            if (exportStartTime > exportEndTime)
                throw new ArgumentOutOfRangeException("StartTime", "Cannot export data: start time exceeds end time.");

            int samplesPerSecond;

            if (!int.TryParse(frameRate, out samplesPerSecond))
                samplesPerSecond = DefaultFrameRate;


            using (Connection connection = new Connection($"127.0.0.1:{HistorianQueryHubClient.PortNumber}", HistorianQueryHubClient.InstanceName))
            using (DataContext dataContext = new DataContext())
            using (StreamWriter writer = new StreamWriter(responseStream))
            {
                // Validate current user has access to requested data
                if (!dataContext.UserIsInRole(s_minimumRequiredRoles))
                    throw new SecurityException($"Cannot export data: access is denied for user \"{Thread.CurrentPrincipal.Identity?.Name ?? "Undefined"}\", minimum required roles = {s_minimumRequiredRoles.ToDelimitedString(", ")}.");

                int[] idValues = pointIDs.Split(',').Select(int.Parse).ToArray();
                Array.Sort(idValues);

                // Write column headers
                writer.Write(GetHeaders(dataContext, idValues));

                long lastTimestamp = 0;
                int columnIndex = 0;

                // Write data pages
                foreach (IMeasurement measurement in MeasurementAPI.GetHistorianData(connection, exportStartTime, exportEndTime, pointIDs))
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    Ticks timestamp = Ticks.RoundToSubsecondDistribution(measurement.Timestamp, samplesPerSecond);

                    // Start a new row for each encountered new timestamp
                    if (timestamp != lastTimestamp)
                    {
                        writer.Write($"{Environment.NewLine}\"{DateTime.SpecifyKind(timestamp, DateTimeKind.Utc).ToString(dateTimeFormat)}\"");
                        columnIndex = 0;
                        lastTimestamp = timestamp;
                    }

                    // Sync to current column
                    while (idValues[columnIndex] != measurement.Key.ID)
                    {
                        writer.Write(',');
                        columnIndex++;

                        if (columnIndex >= idValues.Length)
                        {
                            columnIndex = 0;
                            break;
                        }
                    }

                    writer.Write($",\"{measurement.AdjustedValue}\"");
                    columnIndex++;

                    if (columnIndex >= idValues.Length)
                        columnIndex = 0;
                }
            }
        }

        private string GetHeaders(DataContext dataContext, IEnumerable<int> idValues)
        {
            object[] parameters = idValues.Cast<object>().ToArray();
            string[] parameterNames = parameters.Select((parameter, index) => "p" + index).ToArray();
            string parameterizedQueryString = $"PointID IN ({string.Join(",", parameters.Select((parameter, index) => $"{{{index}}}"))})";
            RecordRestriction pointIDRestriction = new RecordRestriction(parameterizedQueryString, parameters);

            return "\"Timestamp\"," + string.Join(",", dataContext.Table<Measurement>().
                QueryRecords(restriction: pointIDRestriction).
                Select(measurement => $"\"[{measurement.PointID}] {measurement.PointTag}\""));
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly string s_minimumRequiredRoles;

        // Static Constructor
        static ExportDataHandler()
        {
            Type modelType = typeof(Measurement);
            Type hubType = typeof(DataHub);
            IRecordOperationsHub hub;

            // Record operation tuple defines method name and allowed roles
            Tuple<string, string> queryRecordsOperation;

            try
            {
                hub = Activator.CreateInstance(hubType) as IRecordOperationsHub;

                if ((object)hub == null)
                    throw new SecurityException($"Cannot export data: hub type \"DataHub\" is not a IRecordOperationsHub, access cannot be validated.");

                Tuple<string, string>[] recordOperations;

                try
                {
                    // Get any authorized query roles as defined in hub records operations for modeled table, default to read allowed for query
                    recordOperations = hub.RecordOperationsCache.GetRecordOperations(modelType);

                    if ((object)recordOperations == null)
                        throw new NullReferenceException();
                }
                catch (KeyNotFoundException ex)
                {
                    throw new SecurityException($"Cannot export data: hub type \"DataHub\" does not define record operations for \"Measurement\" table, access cannot be validated.", ex);
                }

                // Get record operation for querying records
                queryRecordsOperation = recordOperations[(int)RecordOperation.QueryRecords];

                if ((object)queryRecordsOperation == null)
                    throw new NullReferenceException();

                // Get any defined role restrictions for record query operation - access to CSV download will based on these roles
                s_minimumRequiredRoles = string.IsNullOrEmpty(queryRecordsOperation.Item1) ? "*" : queryRecordsOperation.Item2 ?? "*";
            }
            catch (Exception ex)
            {
                throw new SecurityException($"Cannot export data: failed to instantiate hub type \"DataHub\" or access record operations, access cannot be validated.", ex);
            }
        }

        #endregion
    }
}