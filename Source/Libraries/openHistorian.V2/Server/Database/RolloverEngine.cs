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
using System.Threading;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Manages the conditions required to rollover and combine an archive file.
    /// </summary>
    class RolloverEngine : IDisposable
    {
        volatile bool m_disposed;

        ResourceEngine m_resources;
        InboundPointQueue m_newPointQueue;

        Thread m_insertThread;

        ManualResetEvent m_manualResetEventThreadInitial;

        public RolloverEngine(ResourceEngine resources, InboundPointQueue pointQueue)
        {
            pointQueue.SetQueueCallback(SignalInitialInsert);

            m_resources = resources;
            m_newPointQueue = pointQueue;

            m_manualResetEventThreadInitial = new ManualResetEvent(false);

            m_insertThread = new Thread(ProcessInsertingData);
            m_insertThread.Start();
        }

        /// <summary>
        /// This process fires 10 times per second and populates the must current archive file.
        /// </summary>
        void ProcessInsertingData()
        {
            while (!m_disposed)
            {
                m_manualResetEventThreadInitial.WaitOne(100);
                m_manualResetEventThreadInitial.Reset();
                
                BinaryStream stream;
                int pointCount;

                m_newPointQueue.GetPointBlock(out stream, out pointCount);
                if (pointCount > 0)
                {
                    using (var insertMode = m_resources.StartPartitionInsertMode(0))
                    {
                        insertMode.Partition.PartitionFileFile.BeginEdit();
                        while (pointCount > 0)
                        {
                            pointCount--;

                            ulong time = stream.ReadUInt64();
                            ulong id = stream.ReadUInt64();
                            ulong flags = stream.ReadUInt64();
                            ulong value = stream.ReadUInt64();

                            insertMode.Partition.PartitionFileFile.AddPoint(time, id, flags, value);
                        }
                        insertMode.Partition.PartitionFileFile.CommitEdit();

                        insertMode.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Moves data from the queue and inserts it into Generation 0's Archive.
        /// </summary>
        public void SignalInitialInsert()
        {
            m_manualResetEventThreadInitial.Set();
        }
   
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                SignalInitialInsert();
            }
        }
        
    }
}
