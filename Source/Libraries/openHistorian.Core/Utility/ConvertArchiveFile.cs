//******************************************************************************************************
//  ConvertArchiveFile.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//  11/24/2014 - J. Ritchie Carroll
//       Updated to support simplified file format.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using GSF;
using GSF.IO;
using GSF.Snap;
using GSF.Snap.Storage;
using openHistorian.Snap;

namespace openHistorian.Utility
{
    /// <summary>
    /// openHistorian 1.0 Archive Conversion Functions.
    /// </summary>
    public static class ConvertArchiveFile
    {
        public static long ConvertVersion1FileHandleDuplicates(string oldFileName, string newFileName, EncodingDefinition compressionMethod, out long readTime, out long sortTime, out long writeTime)
        {
            if (!File.Exists(oldFileName))
                throw new ArgumentException("Old file does not exist", "oldFileName");

            if (File.Exists(newFileName))
                throw new ArgumentException("New file already exists", "newFileName");

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            long startTime;
            int count;

            // Derived SortedPointBuffer class increments EntryNumbers instead of removing duplicates
            SortedPointBuffer points;

            startTime = DateTime.UtcNow.Ticks;

            using (OldHistorianReader archiveFile = new OldHistorianReader())
            {
                archiveFile.Open(oldFileName);

                count = archiveFile.PointsArchived;
                points = new SortedPointBuffer(count, false);

                foreach (OldHistorianReader.DataPoint point in archiveFile.Read())
                {
                    key.Timestamp = (ulong)point.Timestamp.Ticks;
                    key.PointID = (ulong)point.PointID;

                    value.Value1 = BitConvert.ToUInt64(point.Value);
                    value.Value3 = (ulong)point.Flags;

                    points.TryEnqueue(key, value);
                }
            }

            readTime = DateTime.UtcNow.Ticks - startTime;

            startTime = DateTime.UtcNow.Ticks;
            points.IsReadingMode = true;
            sortTime = DateTime.UtcNow.Ticks - startTime;

            startTime = DateTime.UtcNow.Ticks;
            SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.Create(Path.Combine(FilePath.GetDirectoryName(newFileName), FilePath.GetFileNameWithoutExtension(newFileName) + ".~d2i"), newFileName, 4096, null, compressionMethod, points);
            writeTime = DateTime.UtcNow.Ticks - startTime;

            return count;
        }

        public static long ConvertVersion1FileIgnoreDuplicates(string oldFileName, string newFileName, EncodingDefinition compressionMethod)
        {
            if (!File.Exists(oldFileName))
                throw new ArgumentException("Old file does not exist", "oldFileName");

            if (File.Exists(newFileName))
                throw new ArgumentException("New file already exists", "newFileName");

            using (OldHistorianStream reader = new OldHistorianStream(oldFileName))
            {
                SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.CreateNonSequential(Path.Combine(FilePath.GetDirectoryName(newFileName), FilePath.GetFileNameWithoutExtension(newFileName) + ".~d2i"), newFileName, 4096, null, compressionMethod, reader);
                return reader.PointCount;
            }
        }

        private class OldHistorianStream
            : TreeStream<HistorianKey, HistorianValue>
        {
            private readonly HistorianKey m_key;
            private readonly HistorianValue m_value;
            private readonly OldHistorianReader m_archiveFile;
            private readonly IEnumerator<OldHistorianReader.DataPoint> m_enumerator;
            private bool m_disposed;

            public OldHistorianStream(string oldFileName)
            {
                m_key = new HistorianKey();
                m_value = new HistorianValue();

                m_archiveFile = new OldHistorianReader();
                m_archiveFile.Open(oldFileName);
                m_enumerator = m_archiveFile.Read().GetEnumerator();
            }

            protected override void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    try
                    {
                        if (disposing)
                        {
                            if (m_archiveFile != null)
                                m_archiveFile.Dispose();
                        }
                    }
                    finally
                    {
                        m_disposed = true;          // Prevent duplicate dispose.
                        base.Dispose(disposing);    // Call base class Dispose().
                    }
                }
            }

            public int PointCount => m_archiveFile.PointsArchived;

            public override bool IsAlwaysSequential => true;

            public override bool NeverContainsDuplicates => true;

            protected override unsafe bool ReadNext(HistorianKey key, HistorianValue value)
            {
                if (m_enumerator.MoveNext())
                {
                    OldHistorianReader.DataPoint point = m_enumerator.Current;

                    key.MillisecondTimestamp = (ulong)point.Timestamp.Ticks;
                    key.PointID = (ulong)point.PointID;

                    value.Value1 = *(uint*)&point.Value;
                    value.Value3 = (ulong)point.Flags;

                    return true;
                }

                return false;
            }
        }
    }
}

