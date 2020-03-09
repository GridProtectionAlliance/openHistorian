﻿//******************************************************************************************************
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
using System.Text;
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
// ReSharper disable AccessToDisposedClosure
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
                    SecurityPrincipal securityPrincipal = request.GetRequestContext().Principal as SecurityPrincipal;
                    await CopyModelAsCsvToStreamAsync(securityPrincipal, requestParameters, stream, cancellationToken);
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

#if MONO
            return Task.FromResult(false);
#else
            return Task.CompletedTask;
#endif
        }

        private async Task CopyModelAsCsvToStreamAsync(SecurityPrincipal securityPrincipal, NameValueCollection requestParameters, Stream responseStream, CancellationToken cancellationToken)
        {
            const double DefaultFrameRate = 30;
            const int DefaultTimestampSnap = 0;

            string dateTimeFormat = Program.Host.Model.Global.DateTimeFormat;

            // TODO: Improve operation for large point lists:
            // Pick-up "POST"ed parameters with a "genurl" param, then cache parameters
            // in a memory cache and return the unique URL (a string instead of a file)
            // with a "download" param and unique ID associated with cached parameters.
            // Then extract params based on unique ID and follow normal steps...

            // Note TSTolerance is in ms
            string pointIDsParam = requestParameters["PointIDs"];
            string startTimeParam = requestParameters["StartTime"];
            string endTimeParam = requestParameters["EndTime"];
            string timestampSnapParam = requestParameters["TSSnap"];
            string frameRateParam = requestParameters["FrameRate"];
            string alignTimestampsParam = requestParameters["AlignTimestamps"];
            string missingAsNaNParam = requestParameters["MissingAsNaN"];
            string fillMissingTimestampsParam = requestParameters["FillMissingTimestamps"];
            string instanceName = requestParameters["InstanceName"];
            string toleranceParam = requestParameters["TSTolerance"];
            ulong[] pointIDs;
            string headers;

            if (string.IsNullOrEmpty(pointIDsParam))
                throw new ArgumentNullException("PointIDs", "Cannot export data: no values were provided in \"PointIDs\" parameter.");

            try
            {
                pointIDs = pointIDsParam.Split(',').Select(ulong.Parse).ToArray();
                Array.Sort(pointIDs);
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("PointIDs", $"Cannot export data: failed to parse \"PointIDs\" parameter value \"{pointIDsParam}\": {ex.Message}");
            }

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

            using (DataContext dataContext = new DataContext())
            {
                // Validate current user has access to requested data
                if (!dataContext.UserIsInRole(securityPrincipal, s_minimumRequiredRoles))
                    throw new SecurityException($"Cannot export data: access is denied for user \"{Thread.CurrentPrincipal.Identity?.Name ?? "Undefined"}\", minimum required roles = {s_minimumRequiredRoles.ToDelimitedString(", ")}.");

                headers = GetHeaders(dataContext, pointIDs.Select(id => (int)id));
            }

            if (!double.TryParse(frameRateParam, out double frameRate))
                frameRate = DefaultFrameRate;
            if (!int.TryParse(timestampSnapParam, out int timestampSnap))
                timestampSnap = DefaultTimestampSnap;
            if (!double.TryParse(toleranceParam, out double tolerance))
                tolerance = 0.5;

            int toleranceTicks = (int)Math.Ceiling(tolerance * Ticks.PerMillisecond);
            bool alignTimestamps = alignTimestampsParam?.ParseBoolean() ?? true;
            bool missingAsNaN = missingAsNaNParam?.ParseBoolean() ?? true;
            bool fillMissingTimestamps = alignTimestamps && (fillMissingTimestampsParam?.ParseBoolean() ?? false);

            if (string.IsNullOrEmpty(instanceName))
                instanceName = TrendValueAPI.DefaultInstanceName;

            LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter adapter);
            HistorianServer serverInstance = adapter?.Server;

            if (serverInstance == null)
                throw new InvalidOperationException($"Cannot export data: failed to access internal historian server instance \"{instanceName}\".");

            const int TargetBufferSize = 524288;

            StringBuilder readBuffer = new StringBuilder(TargetBufferSize * 2);
            ManualResetEventSlim bufferReady = new ManualResetEventSlim(false);
            List<string> writeBuffer = new List<string>();
            object writeBufferLock = new object();
            bool readComplete = false;
            
            Task readTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    using (SnapClient connection = SnapClient.Connect(serverInstance.Host))
                    {
                        Dictionary<ulong, int> pointIDIndex = new Dictionary<ulong, int>(pointIDs.Length);
                        float[] values = new float[pointIDs.Length];

                        for (int i = 0; i < pointIDs.Length; i++)
                            pointIDIndex.Add(pointIDs[i], i);

                        for (int i = 0; i < values.Length; i++)
                            values[i] = float.NaN;

                        ulong interval;

                        if (Math.Abs(frameRate % 1) <= (double.Epsilon * 100))
                        {
                            Ticks[] subseconds = Ticks.SubsecondDistribution((int)frameRate);

                            interval = (ulong)(subseconds.Length > 1 ? subseconds[1].Value : Ticks.PerSecond);
                        }
                        else
                        {
                            interval = (ulong)(Math.Floor(1.0d / frameRate) * Ticks.PerSecond);
                        }
                        

                        ulong lastTimestamp = 0;

                        // Write data pages
                        SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, endTime);
                        MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(pointIDs);
                        HistorianKey historianKey = new HistorianKey();
                        HistorianValue historianValue = new HistorianValue();

                        // Write row values function
                        Action bufferValues = () =>
                        {
                            readBuffer.Append(missingAsNaN ? string.Join(",", values) : string.Join(",", values.Select(val => float.IsNaN(val) ? "" : $"{val}")));

                            if (readBuffer.Length < TargetBufferSize)
                                return;

                            lock (writeBufferLock)
                                writeBuffer.Add(readBuffer.ToString());

                            readBuffer.Clear();
                            bufferReady.Set();
                        };

                        using (ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName))
                        {
                            // Start stream reader for the provided time window and selected points
                            TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);
                            ulong timestamp = 0;

                            // Adjust timestamp to use first timestamp as base
                            bool adjustTimeStamp = true;
                            long baseTime = startTime.Ticks;

                            if (timestampSnap == 0)
                            {
                                adjustTimeStamp = false;
                                baseTime = Ticks.RoundToSecondDistribution(startTime.Ticks,frameRate,startTime.Ticks - startTime.Ticks % Ticks.PerSecond);

                            }
                            else if (timestampSnap == 1)
                            {
                                adjustTimeStamp = true;
                            }
                            else if (timestampSnap == 2)
                            {
                                adjustTimeStamp = false;
                                baseTime = startTime.Ticks;
                            }

                            while (stream.Read(historianKey, historianValue) && !cancellationToken.IsCancellationRequested)
                            {
                                if (alignTimestamps)
                                {
                                    if (adjustTimeStamp)
                                    {
                                        adjustTimeStamp = false;
                                        baseTime = (long)historianKey.Timestamp;
                                    }

                                    // Make sure the timestamp is actually close enough to the distribution
                                    Ticks ticks = Ticks.ToSecondDistribution((long)historianKey.Timestamp, frameRate, baseTime, toleranceTicks);
                                    if (ticks == Ticks.MinValue)
                                        continue;

                                    timestamp = (ulong)ticks.Value;
                                }
                                else
                                {
                                    timestamp = historianKey.Timestamp;
                                }

                                // Start a new row for each encountered new timestamp
                                if (timestamp != lastTimestamp)
                                {
                                    if (lastTimestamp > 0)
                                        bufferValues();

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
                                                interpolated = (ulong)Ticks.RoundToSecondDistribution((long)(interpolated + interval), frameRate, startTime.Ticks).Value;
                                                readBuffer.Append($"{Environment.NewLine}{new DateTime((long)interpolated, DateTimeKind.Utc).ToString(dateTimeFormat)},");
                                                bufferValues();
                                            }
                                        }
                                    }

                                    readBuffer.Append($"{Environment.NewLine}{new DateTime((long)timestamp, DateTimeKind.Utc).ToString(dateTimeFormat)},");
                                    lastTimestamp = timestamp;
                                }

                                // Save value to its column
                                values[pointIDIndex[historianKey.PointID]] = historianValue.AsSingle;
                            }

                            if (timestamp > 0)
                                bufferValues();

                            if (readBuffer.Length > 0)
                            {
                                lock (writeBufferLock)
                                    writeBuffer.Add(readBuffer.ToString());
                            }
                        }
                    }
                }
                finally
                {
                    readComplete = true;
                    bufferReady.Set();
                }
            }, cancellationToken);

            Task writeTask = Task.Factory.StartNew(() =>
            {
                using (StreamWriter writer = new StreamWriter(responseStream))
                {
                    //Ticks exportStart = DateTime.UtcNow.Ticks;
                    string[] localBuffer;

                    // Write column headers
                    writer.Write(headers);

                    while ((writeBuffer.Count > 0 || !readComplete) && !cancellationToken.IsCancellationRequested)
                    {
                        bufferReady.Wait(cancellationToken);
                        bufferReady.Reset();

                        lock (writeBufferLock)
                        {
                            localBuffer = writeBuffer.ToArray();
                            writeBuffer.Clear();
                        }

                        foreach (string buffer in localBuffer)
                            writer.Write(buffer);
                    }

                    // Flush stream
                    writer.Flush();
                    
                    //Debug.WriteLine("Export time: " + (DateTime.UtcNow.Ticks - exportStart).ToElapsedTimeString(3));
                }
            }, cancellationToken);

            await readTask;
            await writeTask;
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

                if (hub == null)
                    throw new SecurityException("Cannot export data: hub type \"DataHub\" is not a IRecordOperationsHub, access cannot be validated.");

                Tuple<string, string>[] recordOperations;

                try
                {
                    // Get any authorized query roles as defined in hub records operations for modeled table, default to read allowed for query
                    recordOperations = hub.RecordOperationsCache.GetRecordOperations(modelType);

                    if (recordOperations == null)
                        throw new NullReferenceException();
                }
                catch (KeyNotFoundException ex)
                {
                    throw new SecurityException("Cannot export data: hub type \"DataHub\" does not define record operations for \"Measurement\" table, access cannot be validated.", ex);
                }

                // Get record operation for querying records
                queryRecordsOperation = recordOperations[(int)RecordOperation.QueryRecords];

                if (queryRecordsOperation == null)
                    throw new NullReferenceException();

                // Get any defined role restrictions for record query operation - access to CSV download will based on these roles
                s_minimumRequiredRoles = string.IsNullOrEmpty(queryRecordsOperation.Item1) ? "*" : queryRecordsOperation.Item2 ?? "*";
            }
            catch (Exception ex)
            {
                throw new SecurityException("Cannot export data: failed to instantiate hub type \"DataHub\" or access record operations, access cannot be validated.", ex);
            }
        }

        #endregion
    }
}