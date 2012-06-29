//******************************************************************************************************
//  DatabaseEngine.cs - Gbtc
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
//  5/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading;
using openHistorian.V2.IO.Unmanaged;
using openHistorian.V2.Server.Database.Partitions;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public class DatabaseEngine
    {
        Thread m_insertThread;
        RolloverEngine m_rolloverEngine;
        ResourceSharingEngine m_resourceSharingEngine;
        InboundPointQueue m_newPointQueue;
        object m_snapshotLock;

        public DatabaseEngine()
        {
            m_snapshotLock = new object();
            m_newPointQueue = new InboundPointQueue();
            m_insertThread = new Thread(ProcessInsertingData);
            m_insertThread.Start();
            m_resourceSharingEngine = new ResourceSharingEngine();
            m_rolloverEngine = new RolloverEngine(m_resourceSharingEngine);
        }

        public long LookupPointId(Guid pointId)
        {
            return -1;
        }

        void ProcessRolloverData()
        {
            while (true)
            {
                Thread.Sleep(10000);

                lock (m_snapshotLock)
                {
                    PartitionFile newPartitionFile = new PartitionFile();
                    PartitionSummary newPartition = new PartitionSummary();
                    newPartition.PartitionFileFile = newPartitionFile;
                    newPartition.ActiveSnapshot = newPartitionFile.CreateSnapshot();
                    newPartition.FirstKeyValue = ulong.MinValue;
                    newPartition.LastKeyValue = ulong.MaxValue;
                    newPartition.KeyMatchMode = PartitionSummary.MatchMode.UniverseEntry;
                    newPartition.IsReadOnly = true;

                    //LookupTable currentLookupTable = m_lookupTable.CloneEditableCopy();

                    //TableSummaryInfo existingTableInfo = currentLookupTable.GetGeneration(0);
                    //currentLookupTable.SetGeneration(0, newTableInfo);
                    //currentLookupTable.SetGeneration(1, existingTableInfo);


                    //var oldFiles = m_lookupTable.GetLatestSnapshot().Clone();
                }

                //determine what files need to be recombined.
                //recombine them
                //update the snapshot library
                //post update.
            }
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
                    Action<PartitionFile> callback = (currentArchive) =>
                    {
                        currentArchive.BeginEdit();
                        while (pointCount > 0)
                        {
                            pointCount--;

                            ulong time = stream.ReadUInt64();
                            ulong id = stream.ReadUInt64();
                            ulong flags = stream.ReadUInt64();
                            ulong value = stream.ReadUInt64();

                            currentArchive.AddPoint(time, id, flags, value);
                        }
                        currentArchive.CommitEdit();
                    };

                    m_resourceSharingEngine.RequestInsertIntoGeneration0(callback);
                }
            }
        }

        public void WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            m_newPointQueue.WriteData(key1, key2, value1, value2);
        }

    }
}
