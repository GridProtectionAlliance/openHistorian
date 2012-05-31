//******************************************************************************************************
//  Engine.cs - Gbtc
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
using openHistorian.V2.Service.Instance.Database;
using openHistorian.V2.Service.Instance.File;

namespace openHistorian.V2.Service.Instance
{
    /// <summary>
    /// Represents a single self contained historian that is referenced by an instance name. 
    /// </summary>
    public class Engine
    {
        Thread m_insertThread;
        RolloverEngine m_rolloverEngine;
        ResourceSharingEngine m_resourceSharingEngine;

        InboundPointQueue m_newPointQueue;
        IDatabase m_database;
        object m_snapshotLock;

        public Engine()
        {

        }

        public long LookupPointId(Guid pointId)
        {
            return -1;
        }

        public void Open()
        {
            m_snapshotLock = new object();
            m_newPointQueue = new InboundPointQueue();
            m_insertThread = new Thread(ProcessInsertingData);
            m_insertThread.Start();
            m_resourceSharingEngine = new ResourceSharingEngine();
            m_rolloverEngine = new RolloverEngine(m_resourceSharingEngine);
        }

        public void Close()
        {

        }

        void ProcessRolloverData()
        {
            while (true)
            {
                Thread.Sleep(10000);

                lock (m_snapshotLock)
                {
                    Archive newArchive = new Archive();
                    TableSummaryInfo newTableInfo = new TableSummaryInfo();
                    newTableInfo.ArchiveFile = newArchive;
                    newTableInfo.ActiveSnapshot = newArchive.CreateSnapshot();
                    newTableInfo.FirstTime = DateTime.MinValue;
                    newTableInfo.LastTime = DateTime.MaxValue;
                    newTableInfo.TimeMatchMode = TableSummaryInfo.MatchMode.UniverseEntry;
                    newTableInfo.IsReadOnly = true;

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
                    Action<Archive> callback = (currentArchive) =>
                    {
                        currentArchive.BeginEdit();
                        while (pointCount > 0)
                        {
                            pointCount--;

                            long time = stream.ReadInt64();
                            long id = stream.ReadInt64();
                            long flags = stream.ReadInt64();
                            long value = stream.ReadInt64();

                            currentArchive.AddPoint(time, id, flags, value);
                        }
                        currentArchive.CommitEdit();
                    };

                    m_resourceSharingEngine.RequestInsertIntoGeneration0(callback);
                }
            }
        }

        public void WriteData(long key1, long key2, long value1, long value2)
        {
            m_newPointQueue.WriteData(key1, key2, value1, value2);
        }

    }
}
