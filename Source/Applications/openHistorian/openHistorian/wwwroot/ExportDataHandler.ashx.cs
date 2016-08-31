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
//  08/22/2016 - J. Ritchie Carroll
//       Generated original version of source code.
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
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Web.Hosting;
using GSF.Web.Hubs;
using GSF.Web.Model;
using openHistorian.Adapters;
using openHistorian.Net;
using openHistorian.Snap;
using Measurement = openHistorian.Model.Measurement;

// ReSharper disable once CheckNamespace
// ReSharper disable NotResolvedInText
// ReSharper disable once AccessToDisposedClosure
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

            response.Content = new PushStreamContent(async (stream, content, context) =>
            {
                try
                {
                    await CopyModelAsCsvToStreamAsync(requestParameters, stream, cancellationToken);
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

        private async Task CopyModelAsCsvToStreamAsync(NameValueCollection requestParameters, Stream responseStream, CancellationToken cancellationToken)
        {
            const int DefaultFrameRate = 30;

            SecurityProviderCache.ValidateCurrentProvider();
            string dateTimeFormat = Program.Host.Model.Global.DateTimeFormat;

            // TODO: Improve operation for large point lists:
            // Pick-up "POST"ed parameters with a "genurl" param, then cache parameters
            // in a memory cache and return the unique URL (a string instead of a file)
            // with a "download" param and unique ID associated with cached parameters.
            // Then extract params based on unique ID and follow normal steps...

            string pointIDsParam = requestParameters["PointIDs"];
            string startTimeParam = requestParameters["StartTime"];
            string endTimeParam = requestParameters["EndTime"];
            string frameRateParam = requestParameters["FrameRate"];
            string alignTimestampsParam = requestParameters["AlignTimestamps"];
            string missingAsNaNParam = requestParameters["MissingAsNaN"];
            string fillMissingTimestampsParam = requestParameters["FillMissingTimestamps"];

            if (string.IsNullOrEmpty(pointIDsParam))
                throw new ArgumentNullException("PointIDs", "Cannot export data: no values were provided in \"PointIDs\" parameter.");

            if (string.IsNullOrEmpty(startTimeParam))
                throw new ArgumentNullException("StartTime", "Cannot export data: no \"StartTime\" parameter value was specified.");

            if (string.IsNullOrEmpty(pointIDsParam))
                throw new ArgumentNullException("EndTime", "Cannot export data: no \"EndTime\" parameter value was specified.");

            DateTime startTime, endTime;

            try
            {
                startTime = DateTime.ParseExact(startTimeParam, dateTimeFormat, null, DateTimeStyles.AdjustToUniversal);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Cannot export data: failed to parse \"StartTime\" parameter value \"{startTimeParam}\". Expected format is \"{dateTimeFormat}\". Error message: {ex.Message}", "StartTime", ex);
            }

            try
            {
                endTime = DateTime.ParseExact(endTimeParam, dateTimeFormat, null, DateTimeStyles.AdjustToUniversal);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Cannot export data: failed to parse \"EndTime\" parameter value \"{endTimeParam}\". Expected format is \"{dateTimeFormat}\". Error message: {ex.Message}", "EndTime", ex);
            }

            if (startTime > endTime)
                throw new ArgumentOutOfRangeException("StartTime", "Cannot export data: start time exceeds end time.");

            int frameRate;

            if (!int.TryParse(frameRateParam, out frameRate))
                frameRate = DefaultFrameRate;

            bool alignTimestamps = alignTimestampsParam?.ParseBoolean() ?? true;
            bool missingAsNaN = missingAsNaNParam?.ParseBoolean() ?? true;
            bool fillMissingTimestamps = alignTimestamps && (fillMissingTimestampsParam?.ParseBoolean() ?? false);

            HistorianServer serverInstance = LocalOutputAdapter.ServerIntances.Values.FirstOrDefault();

            if ((object)serverInstance == null)
                throw new InvalidOperationException("Cannot export data: failed to access internal historian server instance.");

            await Task.Factory.StartNew(async () =>
            {
                using (SnapClient connection = SnapClient.Connect(serverInstance.Host))
                using (DataContext dataContext = new DataContext())
                using (StreamWriter writer = new StreamWriter(responseStream))
                {
                    // Validate current user has access to requested data
                    if (!dataContext.UserIsInRole(s_minimumRequiredRoles))
                        throw new SecurityException($"Cannot export data: access is denied for user \"{Thread.CurrentPrincipal.Identity?.Name ?? "Undefined"}\", minimum required roles = {s_minimumRequiredRoles.ToDelimitedString(", ")}.");

                    ulong[] pointIDs = pointIDsParam.Split(',').Select(ulong.Parse).ToArray();
                    Dictionary<ulong, int> pointIDIndex = new Dictionary<ulong, int>(pointIDs.Length);
                    float[] values = new float[pointIDs.Length];

                    Array.Sort(pointIDs);

                    for (int i = 0; i < pointIDs.Length; i++)
                        pointIDIndex.Add(pointIDs[i], i);

                    for (int i = 0; i < values.Length; i++)
                        values[i] = float.NaN;

                    // Write column headers
                    await writer.WriteAsync(GetHeaders(dataContext, pointIDs.Select(id => (int)id)));

                    Ticks[] subseconds = Ticks.SubsecondDistribution(frameRate);
                    ulong interval = (ulong)(subseconds.Length > 1 ? subseconds[1].Value : Ticks.PerSecond);

                    ulong lastTimestamp = 0;

                    // Write data pages
                    SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, endTime);
                    MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(pointIDs);
                    HistorianKey key = new HistorianKey();
                    HistorianValue value = new HistorianValue();

                    // Write row values function
                    Func<Task> writeValuesAsync = async () => await writer.WriteAsync(missingAsNaN ? string.Join(",", values) : string.Join(",", values.Select(val => float.IsNaN(val) ? "" : $"{val}")));

                    // Start stream reader for the provided time window and selected points
                    using (ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(HistorianQueryHubClient.InstanceName))
                    {
                        TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);
                        ulong timestamp = 0;

                        while (stream.Read(key, value))
                        {
                            if (cancellationToken.IsCancellationRequested)
                                break;

                            if (alignTimestamps)
                                timestamp = (ulong)Ticks.RoundToSubsecondDistribution((long)key.Timestamp, frameRate).Value;
                            else
                                timestamp = key.Timestamp;

                            // Start a new row for each encountered new timestamp
                            if (timestamp != lastTimestamp)
                            {
                                if (lastTimestamp > 0)
                                    await writeValuesAsync();

                                for (int i = 0; i < values.Length; i++)
                                    values[i] = float.NaN;

                                if (fillMissingTimestamps && lastTimestamp > 0 && timestamp > lastTimestamp)
                                {
                                    ulong difference = timestamp - lastTimestamp;

                                    if (difference > interval)
                                    {
                                        ulong interpolated = lastTimestamp;

                                        for (ulong i = 1; i < difference / interval; i++)
                                        {
                                            interpolated = (ulong)Ticks.RoundToSubsecondDistribution((long)(interpolated + interval), frameRate).Value;
                                            await writer.WriteAsync($"{Environment.NewLine}{new DateTime((long)interpolated, DateTimeKind.Utc).ToString(dateTimeFormat)},");
                                            await writeValuesAsync();
                                        }
                                    }
                                }

                                await writer.WriteAsync($"{Environment.NewLine}{new DateTime((long)timestamp, DateTimeKind.Utc).ToString(dateTimeFormat)},");
                                lastTimestamp = timestamp;
                            }

                            // Save value to its column
                            values[pointIDIndex[key.PointID]] = value.AsSingle;
                        }

                        if (timestamp > 0)
                            await writeValuesAsync();
                    }
                }
            },
            cancellationToken);
        }

        private string GetHeaders(DataContext dataContext, IEnumerable<int> pointIDs)
        {
            object[] parameters = pointIDs.Cast<object>().ToArray();
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