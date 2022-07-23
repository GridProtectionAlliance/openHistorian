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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GSF;
using GSF.Collections;
using GSF.COMTRADE;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Security;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Web.Hosting;
using GSF.Web.Hubs;
using GSF.Web.Model;
using openHistorian.Adapters;
using openHistorian.Model;
using openHistorian.Net;
using openHistorian.Snap;
using Measurement = openHistorian.Model.Measurement;
using EESignalType = GSF.Units.EE.SignalType;

// ReSharper disable LocalizableElement
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

        // Nested Types
        private class PointMetadata
        {
            public ulong[] PointIDs;
            public Measurement[] Measurements;
            public string StationName;
            public string DeviceID;
            public string TargetDeviceName;
            public int TargetQualityFlagsID;
        }

        private class HeaderData
        {
            public string FileImage;
            private string m_fileName;

            public string FileName
            {
                get => m_fileName;
                set => m_fileName = $"{Path.GetFileNameWithoutExtension(value)}.cfg";
            }
        }

        // Constants
        private const string CsvContentType = "text/csv";
        private const string TextContentType = "text/plain";
        private const string BinaryContentType = "application/octet-stream";
        private const int TargetBufferSize = 524288;

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
        public async Task ProcessRequestAsync(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            NameValueCollection requestParameters = request.RequestUri.ParseQueryString();
            SecurityPrincipal securityPrincipal = request.GetRequestContext().Principal as SecurityPrincipal;

            // Initial post request assumed to be all point IDs to query, to be cached by key on server. This operation
            // allows for very large point ID selection posts that could otherwise exceed URI parameter string limits.
            if (request.Method == HttpMethod.Post)
            {
                string content = await request.Content.ReadAsStringAsync();
                ulong[] pointIDs = content.Split(',').Select(ulong.Parse).ToArray();
                Array.Sort(pointIDs);

                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent(CachePointMetadata(securityPrincipal, pointIDs), Encoding.UTF8, TextContentType);
            }
            else
            {
                if (requestParameters["ExportCachedHeader"]?.ParseBoolean() ?? false)
                {
                    // Header export, e.g., cached CFG file
                    HeaderData header = GetCachedHeaderData(requestParameters["HeaderCacheID"]);

                    if (header is not null)
                    {
                        string fileName = requestParameters["FileName"] ?? header.FileName ?? "Export.cfg";
                        response.Content = new StringContent(header.FileImage, Encoding.UTF8, "text/plain");
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileName };
                    }
                }
                else
                {
                    // Data export
                    string cacheIDParam = requestParameters["CacheID"];
                    string pointIDsParam = requestParameters["PointIDs"];
                    PointMetadata metadata;

                    if (string.IsNullOrEmpty(cacheIDParam) && string.IsNullOrEmpty(pointIDsParam))
                        throw new ArgumentException("Cannot export data: no point ID values can be accessed. Neither the \"CacheID\" nor the \"PointIDs\" parameter was provided.");

                    if (string.IsNullOrEmpty(pointIDsParam))
                    {
                        metadata = GetCachedPointMetadata(cacheIDParam);

                        if (metadata is null)
                            throw new ArgumentNullException("CacheID", $"Cannot export data: failed to load cached point metadata referenced by \"CacheID\" parameter value \"{cacheIDParam}\".");
                    }
                    else
                    {
                        try
                        {
                            ulong[] pointIDs = pointIDsParam.Split(',').Select(ulong.Parse).ToArray();
                            Array.Sort(pointIDs);
                            metadata = CreatePointMetadata(securityPrincipal, pointIDs);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentNullException("PointIDs", $"Cannot export data: failed to parse \"PointIDs\" parameter value \"{pointIDsParam}\": {ex.Message}");
                        }
                    }

                    if (!int.TryParse(requestParameters["FileFormat"], out int fileFormat))
                        fileFormat = -1; // Default to CSV

                    bool useCFF = requestParameters["UseCFF"]?.ParseBoolean() ?? false;
                    string fileName = requestParameters["FileName"] ?? $"{metadata.TargetDeviceName ?? "Export"}.{(fileFormat < 0 ? "csv" : useCFF ? "cff" : "dat")}";

                    response.Content = new PushStreamContent(async (stream, _, _) =>
                    {
                        try
                        {
                            await ExportToStreamAsync(fileFormat, useCFF, fileName, metadata, requestParameters, stream, cancellationToken);
                        }
                        finally
                        {
                            stream.Close();
                        }
                    },
                    new MediaTypeHeaderValue(fileFormat switch
                    {
                        -1 => CsvContentType,   // CSV
                        0 => TextContentType,   // COMTRADE ASCII
                        _ => BinaryContentType  // COMTRADE Binary
                    }));

                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileName };
                }
            }
        }

        private static async Task ExportToStreamAsync(int fileFormat, bool useCFF, string fileName, PointMetadata metadata, NameValueCollection requestParameters, Stream responseStream, CancellationToken cancellationToken)
        {
            // See if operation state for this export can be found
            string connectionID = requestParameters["ConnectionID"];
            string operationHandleParam = requestParameters["OperationHandle"];

            HistorianOperationState operationState = null;
            Action completeHistorianOperation = null;

            if (!string.IsNullOrEmpty(connectionID) && HistorianOperations.TryGetHubClient(connectionID, out HistorianOperationsHubClient hubClient) && uint.TryParse(operationHandleParam, out uint operationaHandle))
            {
                operationState = hubClient.GetHistorianOperationState(operationaHandle);

                if (operationState.CancellationToken.IsCancelled)
                {
                    operationState = null;
                }
                else
                {
                    operationState.StartTime = DateTime.UtcNow.Ticks;
                    operationState.TargetExportName = fileName;

                    completeHistorianOperation = () =>
                    {
                        operationState.Completed = !operationState.CancellationToken.IsCancelled;
                        operationState.StopTime = DateTime.UtcNow.Ticks;
                        hubClient.CancelHistorianOperation(operationaHandle);
                    };
                }
            }

            try
            {
                const double DefaultFrameRate = 30D;
                const int DefaultTimestampSnap = 0;
                const double DefaultTolerance = 0.5D;

                string dateTimeFormat = Program.Host.Model.Global.DateTimeFormat;
                string startTimeParam = requestParameters["StartTime"];
                string endTimeParam = requestParameters["EndTime"];
                string frameRateParam = requestParameters["FrameRate"];
                string alignTimestampsParam = requestParameters["AlignTimestamps"];
                string missingAsNaNParam = requestParameters["MissingAsNaN"];
                string fillMissingTimestampsParam = requestParameters["FillMissingTimestamps"];
                string instanceName = requestParameters["InstanceName"];
                string timestampSnapParam = requestParameters["TimestampSnap"];
                string toleranceParam = requestParameters["Tolerance"]; // In milliseconds

                if (string.IsNullOrEmpty(startTimeParam))
                    throw new ArgumentNullException("StartTime", "Cannot export data: no \"StartTime\" parameter value was specified.");

                if (string.IsNullOrEmpty(endTimeParam))
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

                FileType? fileType = null;

                if (fileFormat > -1)
                    fileType = (FileType)fileFormat;

                Dictionary<ulong, int> pointIDIndex = new(metadata.PointIDs.Length);
                byte[] headers = GetHeaders(fileType, useCFF, metadata, pointIDIndex, startTime, out Schema schema);

                if (!double.TryParse(frameRateParam, out double frameRate))
                    frameRate = DefaultFrameRate;

                if (!int.TryParse(timestampSnapParam, out int timestampSnap))
                    timestampSnap = DefaultTimestampSnap;

                if (!double.TryParse(toleranceParam, out double tolerance))
                    tolerance = DefaultTolerance;

                int toleranceTicks = (int)Math.Ceiling(tolerance * Ticks.PerMillisecond);
                bool alignTimestamps = alignTimestampsParam?.ParseBoolean() ?? true;
                bool missingAsNaN = missingAsNaNParam?.ParseBoolean() ?? true;
                bool fillMissingTimestamps = alignTimestamps && (fillMissingTimestampsParam?.ParseBoolean() ?? false);

                if (string.IsNullOrEmpty(instanceName))
                    instanceName = TrendValueAPI.DefaultInstanceName;

                LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter adapter);
                HistorianServer serverInstance = adapter?.Server;

                if (serverInstance is null)
                    throw new InvalidOperationException($"Cannot export data: failed to access internal historian server instance \"{instanceName}\".");

                ManualResetEventSlim bufferReady = new(false);
                BlockAllocatedMemoryStream writeBuffer = new();
                bool[] readComplete = { false };

                Task readTask = ReadTask(fileType, schema, serverInstance, instanceName, metadata, pointIDIndex, startTime, endTime, writeBuffer, bufferReady, frameRate, missingAsNaN, timestampSnap, alignTimestamps, toleranceTicks, fillMissingTimestamps, dateTimeFormat, readComplete, operationState, cancellationToken);
                Task writeTask = WriteTask(responseStream, headers, writeBuffer, bufferReady, readComplete, operationState, completeHistorianOperation, cancellationToken);

                await Task.WhenAll(writeTask, readTask);

                if (fileFormat > -1 && !useCFF)
                    CacheHeaderData(requestParameters["HeaderCacheID"], schema.FileImage, fileName, operationState.EndSampleCount);
            }
            catch (Exception ex)
            {
                if (operationState is not null)
                {
                    operationState.Failed = true;
                    operationState.FailedReason = ex.Message;
                }

                throw;
            }
        }

        private static Task ReadTask(FileType? fileType, Schema schema, HistorianServer serverInstance, string instanceName, PointMetadata metadata, Dictionary<ulong, int> pointIDIndex, DateTime startTime, DateTime endTime, BlockAllocatedMemoryStream writeBuffer, ManualResetEventSlim bufferReady, double frameRate, bool missingAsNaN, int timestampSnap, bool alignTimestamps, int toleranceTicks, bool fillMissingTimestamps, string dateTimeFormat, bool[] readComplete, HistorianOperationState operationState, CancellationToken cancellationToken) =>
            Task.Factory.StartNew(() =>
            {
                uint sample = 0U;

                try
                {
                    using SnapClient connection = SnapClient.Connect(serverInstance.Host);
                    BlockAllocatedMemoryStream readBuffer = new();
                    StreamWriter readBufferWriter = new(readBuffer) { NewLine = Writer.CRLF };
                    int valueCount = metadata.PointIDs.Length;

                    if (fileType is not null && metadata.TargetQualityFlagsID > 0)
                        valueCount--;

                    double[] values = new double[valueCount];

                    for (int i = 0; i < values.Length; i++)
                        values[i] = double.NaN;

                    ulong interval;

                    if (Math.Abs(frameRate % 1) <= double.Epsilon * 100)
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
                    MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIdMatchFilter.CreateFromList<HistorianKey, HistorianValue>(metadata.PointIDs);
                    HistorianKey historianKey = new();
                    HistorianValue historianValue = new();
                    ushort fracSecValue = 0;

                    // Write row values function
                    void bufferValues(DateTime recordTimestamp)
                    {
                        switch (fileType)
                        {
                            case FileType.Ascii:
                                Writer.WriteNextRecordAscii(readBufferWriter, schema, recordTimestamp, values, sample++, true, fracSecValue);
                                break;
                            case FileType.Binary:
                                Writer.WriteNextRecordBinary(readBuffer, schema, recordTimestamp, values, sample++, true, fracSecValue);
                                break;
                            case FileType.Binary32:
                                Writer.WriteNextRecordBinary32(readBuffer, schema, recordTimestamp, values, sample++, true, fracSecValue);
                                break;
                            case FileType.Float32:
                                Writer.WriteNextRecordFloat32(readBuffer, schema, recordTimestamp, values, sample++, true, fracSecValue);
                                break;
                            case null:
                                readBufferWriter.Write($"{Environment.NewLine}{recordTimestamp.ToString(dateTimeFormat)},");
                                readBufferWriter.Write(missingAsNaN ? string.Join(",", values) : string.Join(",", values.Select(val => double.IsNaN(val) ? "" : $"{val}")));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
                        }

                        // Update progress based on time
                        if (operationState is not null)
                            operationState.Progress = recordTimestamp.Ticks - startTime.Ticks;

                        if (readBuffer.Length < TargetBufferSize)
                            return;

                        lock (writeBuffer)
                            readBuffer.WriteTo(writeBuffer);

                        readBuffer.Clear();
                        bufferReady.Set();
                    }

                    // Start stream reader for the provided time window and selected points
                    using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName);
                    TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);

                    // Adjust timestamp to use first timestamp as base
                    bool adjustTimeStamp = timestampSnap switch
                    {
                        0 => false,
                        1 => true,
                        2 => false,
                        _ => true
                    };

                    long baseTime = timestampSnap switch
                    {
                        0 => Ticks.RoundToSecondDistribution(startTime.Ticks, frameRate, startTime.Ticks - startTime.Ticks % Ticks.PerSecond),
                        _ => startTime.Ticks
                    };

                    while (stream.Read(historianKey, historianValue) && !cancellationToken.IsCancellationRequested && !(operationState?.CancellationToken.IsCancelled ?? false))
                    {
                        ulong timestamp;

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
                            if (lastTimestamp > 0UL)
                                bufferValues(new DateTime((long)lastTimestamp));

                            for (int i = 0; i < values.Length; i++)
                                values[i] = double.NaN;

                            if (fillMissingTimestamps && lastTimestamp > 0UL && timestamp > lastTimestamp)
                            {
                                ulong difference = timestamp - lastTimestamp;

                                if (difference > interval)
                                {
                                    ulong interpolated = lastTimestamp;

                                    for (ulong i = 1; i < difference / interval; i++)
                                    {
                                        interpolated = (ulong)Ticks.RoundToSecondDistribution((long)(interpolated + interval), frameRate, startTime.Ticks).Value;
                                        bufferValues(new DateTime((long)interpolated, DateTimeKind.Utc));
                                    }
                                }
                            }

                            lastTimestamp = timestamp;
                        }

                        // Save value to its column
                        if (pointIDIndex.TryGetValue(historianKey.PointID, out int index))
                            values[index] = historianValue.AsSingle;
                        else if (historianKey.PointID == (ulong)metadata.TargetQualityFlagsID)
                            fracSecValue = (ushort)historianValue.AsSingle;
                    }

                    if (lastTimestamp > 0UL)
                    {
                        bufferValues(new DateTime((long)lastTimestamp));
                    }
                    else
                    {
                        // No data queried, interpolate blank rows if requested
                        if (fillMissingTimestamps)
                        {
                            ulong difference = (ulong)(endTime.Ticks - startTime.Ticks);

                            if (difference > interval)
                            {
                                for (int i = 0; i < values.Length; i++)
                                    values[i] = double.NaN;

                                ulong interpolated = (ulong)startTime.Ticks;

                                for (ulong i = 1; i < difference / interval; i++)
                                {
                                    interpolated = (ulong)Ticks.RoundToSecondDistribution((long)(interpolated + interval), frameRate, startTime.Ticks).Value;
                                    bufferValues(new DateTime((long)interpolated, DateTimeKind.Utc));
                                }
                            }
                        }
                    }

                    readBufferWriter.Flush();

                    if (readBuffer.Length > 0)
                    {
                        lock (writeBuffer)
                            readBuffer.WriteTo(writeBuffer);
                    }

                    if (operationState is not null)
                        operationState.Progress = operationState.Total;
                }
                catch (Exception ex)
                {
                    if (operationState is not null)
                    {
                        operationState.Failed = true;
                        operationState.FailedReason = ex.Message;
                    }

                    throw;
                }
                finally
                {
                    if (operationState is not null && sample > 0U)
                        operationState.EndSampleCount = sample - 1U;

                    readComplete[0] = true;
                    bufferReady.Set();
                }
            },
            cancellationToken);

        private static Task WriteTask(Stream responseStream, byte[] headers, BlockAllocatedMemoryStream writeBuffer, ManualResetEventSlim bufferReady, bool[] readComplete, HistorianOperationState operationState, Action completeHistorianOperation, CancellationToken cancellationToken) =>
            Task.Factory.StartNew(() =>
            {
                long binaryByteCount = 0L;

                try
                {
                    // Write headers, e.g., CSV header row or CFF schema
                    if (headers is not null)
                        responseStream.Write(headers, 0, headers.Length);

                    while ((writeBuffer.Length > 0 || !readComplete[0]) && !cancellationToken.IsCancellationRequested && !(operationState?.CancellationToken.IsCancelled ?? false))
                    {
                        byte[] bytes;

                        bufferReady.Wait(cancellationToken);
                        bufferReady.Reset();

                        lock (writeBuffer)
                        {
                            bytes = writeBuffer.ToArray();
                            writeBuffer.Clear();
                        }

                        responseStream.Write(bytes, 0, bytes.Length);
                        binaryByteCount += bytes.Length;
                    }

                    // Flush stream
                    responseStream.Flush();
                }
                catch (Exception ex)
                {
                    if (operationState is not null)
                    {
                        operationState.Failed = true;
                        operationState.FailedReason = ex.Message;
                    }

                    throw;
                }
                finally
                {
                    if (operationState is not null)
                        operationState.BinaryByteCount = binaryByteCount;

                    completeHistorianOperation?.Invoke();
                }
            },
            cancellationToken);

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly string s_minimumRequiredRoles;
        private static readonly MemoryCache s_pointMetadataCache;
        private static readonly MemoryCache s_headerCache;

        // Static Constructor
        static ExportDataHandler()
        {
            Type modelType = typeof(Measurement);
            Type hubType = typeof(DataHub);

            try
            {
                if (Activator.CreateInstance(hubType) is not IRecordOperationsHub hub)
                    throw new SecurityException($"Cannot export data: hub type \"{nameof(DataHub)}\" is not an {nameof(IRecordOperationsHub)}, access cannot be validated.");

                Tuple<string, string>[] recordOperations;

                try
                {
                    // Get any authorized query roles as defined in hub records operations for modeled table, default to read allowed for query
                    recordOperations = hub.RecordOperationsCache.GetRecordOperations(modelType);

                    if (recordOperations is null)
                        throw new NullReferenceException();
                }
                catch (KeyNotFoundException ex)
                {
                    throw new SecurityException($"Cannot export data: hub type \"{nameof(DataHub)}\" does not define record operations for \"{nameof(Measurement)}\" table, access cannot be validated.", ex);
                }

                // Get record operation for querying records. Record operation tuple defines method name and allowed roles:
                Tuple<string, string> queryRecordsOperation = recordOperations[(int)RecordOperation.QueryRecords];

                if (queryRecordsOperation is null)
                    throw new NullReferenceException();

                // Get any defined role restrictions for record query operation - access to CSV download will based on these roles
                s_minimumRequiredRoles = string.IsNullOrEmpty(queryRecordsOperation.Item1) ? "*" : queryRecordsOperation.Item2 ?? "*";
            }
            catch (Exception ex)
            {
                throw new SecurityException($"Cannot export data: failed to instantiate hub type \"{nameof(DataHub)}\" or access record operations, access cannot be validated.", ex);
            }

            s_pointMetadataCache = new MemoryCache($"{nameof(ExportDataHandler)}-{nameof(PointMetadata)}Cache");
            s_headerCache = new MemoryCache($"{nameof(ExportDataHandler)}-HeaderCache");
        }

        // Static Methods
        private static byte[] GetHeaders(FileType? fileType, bool useCFF, PointMetadata metadata, Dictionary<ulong, int> pointIDIndex, DateTime startTime, out Schema schema)
        {
            using DataContext dataContext = new();
            TableOperations<Device> deviceTable = dataContext.Table<Device>();
            Dictionary<int, Device> deviceIDMap = new();
            byte[] bytes = null;

            Device lookupDevice(int? id) =>
                id is null ? null : deviceIDMap.GetOrAdd(id.Value, _ => deviceTable.QueryRecordWhere("ID = {0}", id));

            if (fileType is null)
            {
                // Create CSV header
                StringBuilder headers = new("\"Timestamp\"");

                if (metadata.Measurements.Length > 0)
                {
                    headers.Append(',');
                    headers.Append(string.Join(",", metadata.Measurements.Select(measurement => $"\"[{measurement.PointID}] {measurement.PointTag}\"")));
                }
                //Add row with Description
                headers.Append(Environment.NewLine);
                if (metadata.Measurements.Length > 0)
                {
                    headers.Append(',');
                    headers.Append(string.Join(",", metadata.Measurements.Select(measurement => $"\"{measurement.Description}\"")));
                }

                for (int i = 0; i < metadata.PointIDs.Length; i++)
                    pointIDIndex.Add(metadata.PointIDs[i], i);

                bytes = new UTF8Encoding(false).GetBytes(headers.ToString());
                schema = null;
            }
            else
            {
                // Create COMTRADE header
                TableOperations<SignalType> signalTypeTable = dataContext.Table<SignalType>();
                SignalType[] signalTypes = signalTypeTable.QueryRecords().ToArray();
                Dictionary<int, EESignalType> signalTypeAcronyms = signalTypes.ToDictionary(key => key.ID, value => Enum.TryParse(value.Acronym, true, out EESignalType signalType) ? signalType : EESignalType.NONE);
                Dictionary<int, string> signalTypeUnits = signalTypes.ToDictionary(key => key.ID, value => value.EngineeringUnits);
                string[] digitalSignalTypes = { "FLAG", "DIGI", "QUAL" };
                int[] digitalSignalTypeIDs = signalTypes.Where(signalType => digitalSignalTypes.Contains(signalType.Acronym)).Select(signalType => signalType.ID).ToArray();
                Measurement[] analogs = metadata.Measurements.Where(measurement => !digitalSignalTypeIDs.Contains(measurement.SignalTypeID)).ToArray();
                Measurement[] digitals = metadata.Measurements.Where(measurement => digitalSignalTypeIDs.Contains(measurement.SignalTypeID) && measurement.PointID != metadata.TargetQualityFlagsID).ToArray();
                Dictionary<int, Measurement> digitalIDMap = digitals.ToDictionary(key => key.PointID);
                Dictionary<ChannelMetadata, Measurement> analogChannelMeasurementMap = new();
                Dictionary<ChannelMetadata, Measurement> digitalChannelMeasurementMap = new();

                ChannelMetadata getAnalogChannel(Measurement analogMeasurement)
                {
                    ChannelMetadata analogChannel = new()
                    {
                        Name = analogMeasurement.PointTag,
                        SignalType = signalTypeAcronyms[analogMeasurement.SignalTypeID],
                        IsDigital = false,
                        Units = signalTypeUnits[analogMeasurement.SignalTypeID],
                        CircuitComponent = lookupDevice(analogMeasurement.DeviceID)?.Acronym
                    };

                    analogChannelMeasurementMap[analogChannel] = analogMeasurement;
                    return analogChannel;
                }

                ChannelMetadata getDigitalChannel(Measurement digitalMeasurement)
                {
                    EESignalType signalType = signalTypeAcronyms[digitalMeasurement.SignalTypeID];
                    string deviceAcronym = lookupDevice(digitalMeasurement.DeviceID)?.Acronym;

                    ChannelMetadata digitalChannel = new()
                    {
                        Name = signalType == EESignalType.FLAG ? deviceAcronym ?? digitalMeasurement.PointTag.Replace(":", "_") : digitalMeasurement.PointID.ToString(),
                        SignalType = signalType,
                        IsDigital = true,
                        CircuitComponent = deviceAcronym
                    };

                    digitalChannelMeasurementMap[digitalChannel] = digitalMeasurement;
                    return digitalChannel;
                }

                // Create channel metadata
                List<ChannelMetadata> channels = new(analogs.Select(getAnalogChannel).Concat(digitals.Select(getDigitalChannel)));
                channels.Sort(ChannelMetadataSorter.Default);

                // Create schema
                schema = Writer.CreateSchema(channels, metadata.StationName, metadata.DeviceID, startTime.Ticks, Writer.MaxEndSample, 2013, fileType.Value, 1.0D, 0.0D, Program.Host.Model.Global?.NominalFrequency ?? 60);

                // Load indexed digital labels
                Dictionary<int, string[]> digitalLabels = new();

                foreach (DigitalChannel digital in schema.DigitalChannels)
                {
                    if (!int.TryParse(digital.Name, out int pointID) || !digitalIDMap.TryGetValue(pointID, out Measurement digitalMeasurement))
                        continue;

                    string phaseID = digital.PhaseID?.Trim();

                    if (string.IsNullOrEmpty(phaseID) || phaseID.Length < 2 || !int.TryParse(phaseID.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int index))
                        continue;

                    if (index < 0)
                        index = 0;

                    if (index > 15)
                        index = 15;

                    string[] labels = digitalLabels.GetOrAdd(pointID, _ => ParseDigitalLabels(digitalMeasurement?.AlternateTag ?? ""));
                    digital.Name = string.IsNullOrWhiteSpace(labels[index]) ? $"{digitalMeasurement.PointTag} ({index})" : labels[index];
                }

                if (useCFF)
                {
                    // Create COMTRADE CFF header
                    using BlockAllocatedMemoryStream stream = new();
                    Writer.CreateCFFStream(stream, schema);
                    bytes = stream.ToArray();
                }

                // Create properly ordered point ID index
                for (int i = 0; i < channels.Count; i++)
                {
                    ChannelMetadata channel = channels[i];

                    if (analogChannelMeasurementMap.TryGetValue(channel, out Measurement analog))
                        pointIDIndex.Add((ulong)analog.PointID, i);
                    else if (digitalChannelMeasurementMap.TryGetValue(channel, out Measurement digital))
                        pointIDIndex.Add((ulong)digital.PointID, i);
                }
            }

            Debug.Assert(pointIDIndex.Count + (fileType is null || metadata.TargetQualityFlagsID == 0 ? 0 : 1) == metadata.PointIDs.Length);
            return bytes;
        }

        private static string[] ParseDigitalLabels(string digitalLabels)
        {
            string[] labels = new string[16];

            if (digitalLabels.Contains("|"))
            {
                string[] parts = digitalLabels.Split('|');

                for (int i = 0; i < parts.Length && i < 16; i++)
                    labels[i] = parts[i].Trim();
            }
            else
            {
                int i = 0;

                for (int j = 0; j < digitalLabels.Length && i < 16; j += 16)
                {
                    int length = 16;
                    int remaining = digitalLabels.Length - j;

                    if (remaining <= 0)
                        break;

                    if (remaining < 16)
                        length = remaining;

                    labels[i++] = digitalLabels.Substring(j, length).Trim();
                }
            }

            return labels;
        }

        private static PointMetadata CreatePointMetadata(SecurityPrincipal securityPrincipal, ulong[] pointIDs)
        {
            const int MaxSqlParams = 50;

            using DataContext dataContext = new();

            // Validate current user has access to requested data
            if (!dataContext.UserIsInRole(securityPrincipal, s_minimumRequiredRoles))
                throw new SecurityException($"Cannot export data: access is denied for user \"{Thread.CurrentPrincipal.Identity?.Name ?? "Undefined"}\", minimum required roles = {s_minimumRequiredRoles.ToDelimitedString(", ")}.");

            PointMetadata metadata = new()
            {
                PointIDs = pointIDs,
                StationName = nameof(openHistorian),
            };

            TableOperations<Measurement> measurementTable = dataContext.Table<Measurement>();
            List<Measurement> measurements = new();

            for (int i = 0; i < pointIDs.Length; i += MaxSqlParams)
            {
                object[] parameters = pointIDs.Skip(i).Take(MaxSqlParams).Select(id => (object)(int)id).ToArray();
                string parameterizedQueryString = $"PointID IN ({string.Join(",", parameters.Select((_, index) => $"{{{index}}}"))})";
                RecordRestriction pointIDRestriction = new(parameterizedQueryString, parameters);
                measurements.AddRange(measurementTable.QueryRecords(pointIDRestriction));
            }

            metadata.Measurements = measurements.ToArray();
            metadata.DeviceID = $"Export for {pointIDs.Length} measurements";

            try
            {
                int? firstDeviceID = metadata.Measurements.First()?.DeviceID;

                // If all data is from a single device, can pick up device name and acronym for station name and device ID, respectively
                if (firstDeviceID is not null && metadata.Measurements.All(measurement => measurement.DeviceID == firstDeviceID))
                {
                    TableOperations<Device> deviceTable = dataContext.Table<Device>();
                    Device device = deviceTable.QueryRecordWhere("ID = {0}", firstDeviceID);

                    if (device is not null)
                    {
                        metadata.TargetDeviceName = device.Acronym;
                        metadata.StationName = device.Name ?? metadata.TargetDeviceName;
                        metadata.DeviceID = $"{metadata.TargetDeviceName} (ID {device.ID})";

                        Measurement qualityFlags = measurementTable.QueryRecordWhere($"SignalReference = '{device.Acronym}-QF'");

                        if (qualityFlags?.PointID > 0 && measurements.Any(measurement => measurement.PointID == qualityFlags.PointID))
                            metadata.TargetQualityFlagsID = qualityFlags.PointID;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SwallowException(ex);
            }

            return metadata;
        }

        private static string CachePointMetadata(SecurityPrincipal securityPrincipal, ulong[] pointIDs)
        {
            string pointCacheID = Guid.NewGuid().ToString();
            s_pointMetadataCache.Add(pointCacheID, CreatePointMetadata(securityPrincipal, pointIDs), new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(30.0D) });
            return pointCacheID;
        }

        private static PointMetadata GetCachedPointMetadata(string pointCacheID) =>
            string.IsNullOrEmpty(pointCacheID) ? null : s_pointMetadataCache.Get(pointCacheID) as PointMetadata;

        private static void CacheHeaderData(string headerCacheID, string fileImage, string fileName, long endSample)
        {
            if (string.IsNullOrEmpty(headerCacheID))
                return;

            // Update CFG end sample value
            string defaultEndSample = $"0,{Writer.MaxEndSample}";
            int endSampleIndex = fileImage.LastIndexOf(defaultEndSample, StringComparison.Ordinal);

            if (endSampleIndex > -1)
                fileImage = $"{fileImage.Substring(0, endSampleIndex)}0,{endSample}{fileImage.Substring(endSampleIndex + defaultEndSample.Length)}";

            s_headerCache.Add(headerCacheID, new HeaderData { FileImage = fileImage, FileName = fileName }, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(30.0D) });
        }

        private static HeaderData GetCachedHeaderData(string headerCacheID) =>
            string.IsNullOrEmpty(headerCacheID) ? null : s_headerCache.Get(headerCacheID) as HeaderData;

        #endregion
    }
}