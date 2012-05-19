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
        Thread m_rolloverThread;

        InboundPointQueue m_newPointQueue;
        LookupTable m_lookupClass;
        Archive m_currentArchive;
        IDatabase m_database;

        public Engine()
        {

        }

        public long LookupPointId(Guid pointId)
        {
            return -1;
            }

        public void Open()
        {
            m_newPointQueue = new InboundPointQueue();
            m_lookupClass = new LookupTable();
            m_insertThread = new Thread(ProcessInsertingData);
            m_insertThread.Start();
            m_rolloverThread = new Thread(ProcessRolloverData);
            m_rolloverThread.Start();
        }

        public void Close()
        {

        }

        void ProcessRolloverData()
        {
            while (true)
            {
                Thread.Sleep(10000);

                var oldFiles = m_lookupClass.GetLatestSnapshot().Clone();
                //determine what files need to be recombined.
                //recombine them
                //update the snapshot library
                //post update.
            }
        }

        /// <summary>
        /// This process fires 10 times per second and populates the 
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
                    while (pointCount > 0)
                    {
                        pointCount--;

                        long time = stream.ReadInt64();
                        long id = stream.ReadInt64();
                        long flags = stream.ReadInt64();
                        long value = stream.ReadInt64();

                        m_currentArchive.AddPoint(time, id, flags, value);
                    }
                }
            }
        }

        public void WriteData(long key1, long key2, long value1, long value2)
        {
            m_newPointQueue.WriteData(key1, key2, value1, value2);

        }
     
    }
}
