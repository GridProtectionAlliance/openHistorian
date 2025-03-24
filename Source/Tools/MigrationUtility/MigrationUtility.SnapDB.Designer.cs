//******************************************************************************************************
//  MigrationUtility.SnapDBWriter.Designer.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  11/21/2014 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GSF.Diagnostics;
using GSF.Historian;
using GSF.Historian.Files;
using GSF.Snap;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Units;
using openHistorian.Net;
using openHistorian.Snap;

namespace ComparisonUtility
{
    // SnapDB Engine Code
    partial class MigrationUtility
    {
        private class SnapDBEngine : IDisposable
        {
            private MigrationUtility m_parent;
            private HistorianServer m_server;
            private LogSubscriber m_logSubscriber;
            private bool m_disposed;

            public SnapDBEngine(MigrationUtility parent, string instanceName, string destinationFilesLocation, string targetFileSize, string directoryNamingMethod, bool readOnly = false)
            {
                m_parent = parent;

                m_logSubscriber = Logger.CreateSubscriber(VerboseLevel.High);
                m_logSubscriber.NewLogMessage += m_logSubscriber_Log;

                if (string.IsNullOrEmpty(instanceName))
                    instanceName = "PPA";
                else
                    instanceName = instanceName.Trim();

                // Establish archive information for this historian instance
                HistorianServerDatabaseConfig archiveInfo = new HistorianServerDatabaseConfig(instanceName, destinationFilesLocation, !readOnly);

                double targetSize;

                if (!double.TryParse(targetFileSize, out targetSize))
                    targetSize = 1.5D;

                archiveInfo.TargetFileSize = (long)(targetSize * SI.Giga);

                int methodIndex;

                if (!int.TryParse(directoryNamingMethod, out methodIndex) || !Enum.IsDefined(typeof(ArchiveDirectoryMethod), methodIndex))
                    methodIndex = (int)ArchiveDirectoryMethod.YearThenMonth;

                archiveInfo.DirectoryMethod = (ArchiveDirectoryMethod)methodIndex;

                m_server = new HistorianServer(archiveInfo);
                m_parent.ShowUpdateMessage("[SnapDB] Engine initialized");
            }

            /// <summary>
            /// Releases the unmanaged resources before the <see cref="SnapDBEngine"/> object is reclaimed by <see cref="GC"/>.
            /// </summary>
            ~SnapDBEngine()
            {
                Dispose(false);
            }

            public SnapServer ServerHost
            {
                get
                {
                    return m_server.Host;
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    try
                    {
                        if ((object)m_server != null)
                        {
                            m_server.Dispose();
                            m_server = null;
                        }

                        if ((object)m_logSubscriber != null)
                        {
                            m_logSubscriber.NewLogMessage -= m_logSubscriber_Log;
                            m_logSubscriber = null;
                        }

                        m_parent.ShowUpdateMessage("[SnapDB] Engine terminated");
                        {
                        }
                    }
                    finally
                    {
                        m_disposed = true;  // Prevent duplicate dispose.
                    }
                }
            }

            // Expose SnapDB log messages via Adapter status and exception event raisers
            private void m_logSubscriber_Log(LogMessage logMessage)
            {
                if ((object)logMessage.Exception != null)
                    m_parent.ShowUpdateMessage("[SnapDB] Exception during {0}: {1}", logMessage.EventPublisherDetails.EventName, logMessage.GetMessage());
                else
                    m_parent.ShowUpdateMessage("[SnapDB] {0}: {1}", logMessage.Level, logMessage.GetMessage());
            }
        }

        private class SnapDBClient : IDisposable
        {
            private readonly SnapClient m_client;
            private readonly ClientDatabaseBase<HistorianKey, HistorianValue> m_database;
            private TreeStream<HistorianKey, HistorianValue> m_stream;
            private readonly HistorianKey m_key;
            private readonly HistorianValue m_value;
            private HistorianKey m_lastKey;
            private bool m_disposed;

            public SnapDBClient(SnapDBEngine engine, string instanceName)
            {
                m_client = SnapClient.Connect(engine.ServerHost);
                m_database = m_client.GetDatabase<HistorianKey, HistorianValue>(instanceName);
                m_key = new HistorianKey();
                m_value = new HistorianValue();
                m_lastKey = new HistorianKey();
            }

            ~SnapDBClient()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    try
                    {
                        if (disposing)
                        {
                            if ((object)m_stream != null)
                                m_stream.Dispose();

                            if ((object)m_database != null)
                                m_database.Dispose();

                            if ((object)m_client != null)
                                m_client.Dispose();
                        }
                    }
                    finally
                    {
                        m_disposed = true;  // Prevent duplicate dispose.
                    }
                }
            }

            public void WriteSnapDBData(DataPoint dataPoint, bool ignoreDuplicates)
            {
                // Copy data point to key and value
                m_key.Timestamp = dataPoint.Timestamp;
                m_key.PointID = dataPoint.PointID;
                m_value.Value1 = dataPoint.Value;
                m_value.Value3 = dataPoint.Flags;

                if (!ignoreDuplicates)
                {
                    if (m_lastKey.Timestamp == m_key.Timestamp && m_lastKey.PointID == m_key.PointID)
                    {
                        // Duplicate key encountered, increment entry number
                        m_lastKey.EntryNumber++;
                        m_key.EntryNumber = m_lastKey.EntryNumber;
                    }
                    else
                    {
                        m_key.EntryNumber = 0;
                        m_key.CopyTo(m_lastKey);
                    }
                }

                // Write key/value pair to SnapDB engine
                m_database.Write(m_key, m_value);
            }

            public bool ReadNextSnapDBPoint(DataPoint point)
            {
                if ((object)m_stream is null)
                    throw new NullReferenceException("Stream is not initialized");

                if (m_stream.Read(m_key, m_value))
                {
                    point.Timestamp = m_key.Timestamp;
                    point.PointID = m_key.PointID;
                    point.Value = m_value.Value1;
                    point.Flags = m_value.Value3;

                    return true;
                }

                return false;
            }

            public bool FindSnapDBPoint(DataPoint point)
            {
                using (TreeStream<HistorianKey, HistorianValue> stream = m_database.ReadSingleValue(point.Timestamp, point.PointID))
                {
                    if (stream.Read(m_key, m_value))
                        return m_key.MillisecondTimestamp == point.Timestamp && m_key.PointID == point.PointID;
                }

                return false;
            }

            public bool ScanToSnapDBPoint(ulong timestamp, ulong pointID, DataPoint point)
            {
                if ((object)m_stream != null)
                    m_stream.Dispose();

                // Start read at provided timestamp
                m_stream = m_database.Read(timestamp, ulong.MaxValue);

                bool success = true;

                // Scan to desired point
                do
                {
                    if (!m_stream.Read(m_key, m_value))
                    {
                        success = false;
                        break;
                    }
                }
                while (m_key.PointID != pointID);

                point.Timestamp = m_key.Timestamp;
                point.PointID = m_key.PointID;
                point.Value = m_value.Value1;
                point.Flags = m_value.Value3;

                return success;
            }

            public void FlushSnapDB()
            {
                m_database.HardCommit();
            }
        }

        private static void CopyDataPointToKeyValue(IDataPoint dataPoint, HistorianKey key, HistorianValue value)
        {
            // Write key information
            key.MillisecondTimestamp = (ulong)dataPoint.Time.ToDateTime().Ticks;
            key.PointID = (ulong)dataPoint.HistorianID;

            // Note that third ulong in key can be used for storing duplicate timestamps
            // such as those duplicates that may come in during leap-seconds. IData will
            // need to expose this indication such that entry number could be updated
            key.EntryNumber = 0;

            // Since current time-series measurements are basically all floats - values fit into
            // first ulong, this will change as value types accepted by framework expands
            value.AsSingle = dataPoint.Value;

            // This value will be used when host framework expands to support multiple data types
            value.Value2 = 0;

            // While leaving second ulong available for expanded data type storage, we store
            // quality in third ulong for future consistency  - note that this still leaves
            // 32-bits of space available for future use
            value.Value3 = (ulong)dataPoint.Quality;
        }

        private class GSFHistorianStream
            : TreeStream<HistorianKey, HistorianValue>
        {
            private ArchiveFile m_file;
            private IEnumerator<IDataPoint> m_enumerator;
            private HistorianValue m_value;
            private HistorianKey m_key;
            private HistorianKey m_lastKey;
            private long m_pointCount;
            private readonly string m_instanceName;
            private readonly StreamWriter m_duplicateDataOutput;
            private bool m_disposed;

            public GSFHistorianStream(MigrationUtility parent, string sourceFileName, string instanceName, StreamWriter duplicateDataOutput = null)
            {
                m_file = OpenArchiveFile(sourceFileName, ref instanceName);
                m_instanceName = instanceName;
                m_duplicateDataOutput = duplicateDataOutput;

                // Find maximum point ID
                int maxPointID = FindMaximumPointID(m_file.MetadataFile);

                // Create new time-sorted data point scanner to read points in this file in sorted order
                TimeSortedArchiveFileScanner scanner = new TimeSortedArchiveFileScanner(false);

                // Get start and end times from file data and validate
                TimeTag startTime, endTime;

                startTime = m_file.Fat.FileStartTime;

                if (startTime == TimeTag.MaxValue)
                    startTime = TimeTag.MinValue;

                endTime = m_file.Fat.FileEndTime;

                if (endTime == TimeTag.MinValue)
                    endTime = TimeTag.MaxValue;

                scanner.FileAllocationTable = m_file.Fat;
                scanner.HistorianIDs = Enumerable.Range(1, maxPointID);
                scanner.StartTime = startTime;
                scanner.EndTime = endTime;
                scanner.ResumeFrom = null;
                scanner.DataReadExceptionHandler = (sender, e) => parent.ShowUpdateMessage("[GSFHistorian] Exception encountered during data read: {0}", e.Argument.Message);

                m_enumerator = scanner.Read().GetEnumerator();

                m_value = new HistorianValue();
                m_key = new HistorianKey();
                m_lastKey = new HistorianKey();
            }

            public ArchiveFile ArchiveFile
            {
                get
                {
                    return m_file;
                }
            }
            public string InstanceName
            {
                get
                {
                    return m_instanceName;
                }
            }

            public long PointCount
            {
                get
                {
                    return m_pointCount;
                }
            }

            public override bool IsAlwaysSequential
            {
                get
                {
                    return true;
                }
            }

            public override bool NeverContainsDuplicates
            {
                get
                {
                    return true;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    try
                    {
                        if (disposing)
                        {
                            if ((object)m_file != null)
                                m_file.Dispose();
                        }
                    }
                    finally
                    {
                        m_disposed = true;
                        base.Dispose(disposing);
                    }
                }
            }

            public bool ReadNext(DataPoint point)
            {
                bool result = ReadNext(m_key, m_value);

                if (result)
                {
                    point.Timestamp = m_key.Timestamp;
                    point.PointID = m_key.PointID;
                    point.Value = m_value.Value1;
                    point.Flags = m_value.Value3;
                }

                return result;
            }

            protected override bool ReadNext(HistorianKey key, HistorianValue value)
            {
                if (m_enumerator.MoveNext())
                {
                    CopyDataPointToKeyValue(m_enumerator.Current, key, value);

                    if (m_lastKey.Timestamp == key.Timestamp && m_lastKey.PointID == key.PointID)
                    {
                        // Duplicate key encountered, increment entry number
                        m_lastKey.EntryNumber++;
                        key.EntryNumber = m_lastKey.EntryNumber;

                        if ((object)m_duplicateDataOutput != null)
                            lock (m_duplicateDataOutput)
                                m_duplicateDataOutput.WriteLine("[{0:00000}@{1:yyyy-MM-dd HH:mm:ss.fff}] = {2}", key.PointID, key.TimestampAsDate, key.EntryNumber);
                    }
                    else
                    {
                        key.CopyTo(m_lastKey);
                    }

                    m_pointCount++;
                    return true;
                }

                return false;
            }
        }
    }
}
