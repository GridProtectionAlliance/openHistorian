//******************************************************************************************************
//  ConvertArchiveFile.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
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
using System.IO;
using System.Threading;
using GSF.IO;
using GSF.Snap;
using GSF.Snap.Storage;
using openHistorian.Snap;

namespace openHistorian.Utility
{
    public static class ConvertArchiveFile
    {
        public static unsafe long ConvertVersion1FileHandleDuplicates(string oldFileName, string newFileName, EncodingDefinition compressionMethod, out long readTime, out long sortTime, out long writeTime)
        {
            if (!File.Exists(oldFileName))
                throw new ArgumentException("Old file does not exist", "oldFileName");

            if (File.Exists(newFileName))
                throw new ArgumentException("New file already exists", "newFileName");

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            OldHistorianReader hist = new OldHistorianReader(oldFileName);
            long startTime;
            long count = 0;

            // Derived SortedPointBuffer class increments EntryNumbers instead of removing duplicates
            SortedPointBuffer points = null;

            Func<OldHistorianReader.Points, bool> fillInMemoryTree = p =>
            {
                count++;

                if (count > long.MaxValue)
                    return false;

                key.Timestamp = (ulong)p.Time.Ticks;
                key.PointID = (ulong)p.PointID;

                value.Value3 = (ulong)p.Flags;
                value.Value1 = *(uint*)&p.Value;

                if (!points.TryEnqueue(key, value))
                    count--;

                return true;
            };

            startTime = DateTime.UtcNow.Ticks;
            hist.Read(fillInMemoryTree, capacity => points = new SortedPointBuffer(capacity, false));
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

            OldHistorianStream reader = new OldHistorianStream(oldFileName);

            SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.CreateNonSequential(Path.Combine(FilePath.GetDirectoryName(newFileName), FilePath.GetFileNameWithoutExtension(newFileName) + ".~d2i"), newFileName, 4096, null, compressionMethod, reader);

            return reader.PointsRead;
        }

        private class OldHistorianStream
            : TreeStream<HistorianKey, HistorianValue>
        {
            private readonly HistorianKey m_key;
            private readonly HistorianValue m_value;
            private readonly ManualResetEventSlim m_readNext;
            private readonly ManualResetEventSlim m_dataReady;
            private int m_pointCount;
            private int m_pointsRead;

            public unsafe OldHistorianStream(string oldFileName)
            {
                m_key = new HistorianKey();
                m_value = new HistorianValue();
                m_readNext = new ManualResetEventSlim();
                m_dataReady = new ManualResetEventSlim();

                Thread historianReader = new Thread(start =>
                {
                    OldHistorianReader hist = new OldHistorianReader(oldFileName);

                    hist.Read(p =>
                    {
                        m_readNext.Wait();
                        m_readNext.Reset();

                        m_key.Timestamp = (ulong)p.Time.Ticks;
                        m_key.PointID = (ulong)p.PointID;

                        m_value.Value3 = (ulong)p.Flags;
                        m_value.Value1 = *(uint*)&p.Value;

                        m_pointsRead++;
                        m_dataReady.Set();

                        return true;
                    },
                    c => m_pointCount = c);
                });

                historianReader.IsBackground = true;
                historianReader.Start();
            }

            public int PointsRead
            {
                get
                {
                    return m_pointsRead;
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

            protected override bool ReadNext(HistorianKey key, HistorianValue value)
            {
                m_readNext.Set();
                m_dataReady.Wait();
                m_dataReady.Reset();

                m_key.CopyTo(key);
                m_value.CopyTo(m_value);

                return m_pointsRead < m_pointCount;
            }
        }
    }
}

