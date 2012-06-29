//******************************************************************************************************
//  RolloverEngine.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  5/30/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using openHistorian.V2.IO.Unmanaged;
using System.Diagnostics;
using System.Threading;
using openHistorian.V2.Server.Database.Partitions;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Manages the conditions required to rollover and combine an archive file.
    /// </summary>
    class RolloverEngine
    {
        ResourceEngine m_resources;
        InboundPointQueue m_newPointQueue;

        int m_generation0Count = 0;
        int m_generation1Count = 0;
        int m_generation2Count = 0;

        long m_generation0Size = 0;
        long m_generation1Size = 0;
        long m_generation2Size = 0;

        Stopwatch m_generation0Interval = new Stopwatch();
        Stopwatch m_generation1Interval = new Stopwatch();
        Stopwatch m_generation2Interval = new Stopwatch();

        int m_generation0CountLimit = 1000;
        int m_generation1CountLimit = 1000;
        int m_generation2CountLimit = 1000;

        long m_generation0SizeLimit = 100L * 1024 * 1024;
        long m_generation1SizeLimit = 1L * 1024 * 1024 * 1024;
        long m_generation2SizeLimit = 100L * 1024 * 1024 * 1024;

        TimeSpan m_generation0IntervalLimit = new TimeSpan(0, 0, 0, 10); //10 seconds
        TimeSpan m_generation1IntervalLimit = new TimeSpan(0, 0, 10); //10 minutes
        TimeSpan m_generation2IntervalLimit = new TimeSpan(0, 10, 0); //10 hours

        Thread m_processRolloverThread0;
        Thread m_processRolloverThread1;
        Thread m_processRolloverThread2;
        Thread m_insertThread;

        public RolloverEngine(ResourceEngine resources, InboundPointQueue pointQueue)
        {
            m_generation0Interval.Start();
            m_generation1Interval.Start();
            m_generation2Interval.Start();

            m_resources = resources;
            m_newPointQueue = pointQueue;

            m_processRolloverThread0 = new Thread(ProcessRolloverGen0);
            m_processRolloverThread0.Start();

            m_processRolloverThread1 = new Thread(ProcessRolloverGen1);
            m_processRolloverThread1.Start();

            m_processRolloverThread2 = new Thread(ProcessRolloverGen2);
            m_processRolloverThread2.Start();

            m_insertThread = new Thread(ProcessInsertingData);
            m_insertThread.Start();
        }

        /// <summary>
        /// This process fires 10 times per second and populates the must current archive file.
        /// </summary>
        void ProcessInsertingData()
        {
            while (true)
            {
                Thread.Sleep(100);
                BinaryStream stream;
                int pointCount;

                m_newPointQueue.GetPointBlock(out stream, out pointCount);
                if (pointCount > 0)
                {
                    m_resources.AcquireEditLock(0);
                    var currentArchive = m_resources.GetActivePartition(0);

                    currentArchive.PartitionFileFile.BeginEdit();
                    while (pointCount > 0)
                    {
                        pointCount--;

                        ulong time = stream.ReadUInt64();
                        ulong id = stream.ReadUInt64();
                        ulong flags = stream.ReadUInt64();
                        ulong value = stream.ReadUInt64();

                        currentArchive.PartitionFileFile.AddPoint(time, id, flags, value);
                    }
                    currentArchive.PartitionFileFile.CommitEdit();

                    m_resources.SetActivePartition(0, currentArchive);
                    m_resources.ReleaseEditLock(0);
                }
            }
        }

        void ProcessRolloverGen0()
        {
            while (true)
            {
                Thread.Sleep(100);
                if ((m_generation0Interval.Elapsed > m_generation0IntervalLimit) ||
                    (m_generation0Count > m_generation0CountLimit) ||
                    (m_generation0Size > m_generation0SizeLimit))
                {

                    m_resources.ReplaceActivePartition(0, new PartitionSummary());

                    m_resources.AcquireEditLock(1);
                    var destinationArchive = m_resources.GetActivePartition(1);
                    var sourceArchive = m_resources.GetProcessingPartition(0);

                    var source = sourceArchive.ActiveSnapshot.OpenInstance();
                    var dest = destinationArchive.PartitionFileFile;

                    dest.BeginEdit();

                    var reader = source.GetDataRange();
                    reader.SeekToKey(0, 0);

                    ulong value1, value2, key1, key2;
                    while (reader.GetNextKey(out key1, out key2, out value1, out value2))
                    {
                        dest.AddPoint(key1, key2, value1, value2);
                    }

                    dest.CommitEdit();

                    m_resources.CommittActivePartitionAndRemoveProcessing(1, 0);
                    m_resources.ReleaseEditLock(1);

                    m_generation0Count = 0;
                    m_generation0Interval.Restart();
                }
            }
        }

        void ProcessRolloverGen1()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if ((m_generation1Interval.Elapsed > m_generation1IntervalLimit) ||
                    (m_generation1Count > m_generation1CountLimit) ||
                    (m_generation1Size > m_generation1SizeLimit))
                {
                  
                    m_generation1Count = 0;
                    m_generation1Interval.Restart();
                }
            }
        }

        void ProcessRolloverGen2()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if ((m_generation2Interval.Elapsed > m_generation2IntervalLimit) ||
                    (m_generation2Count > m_generation2CountLimit) ||
                    (m_generation2Size > m_generation2SizeLimit))
                {

                    m_generation2Count = 0;
                    m_generation2Interval.Restart();
                }
            }
        }
    }
}
