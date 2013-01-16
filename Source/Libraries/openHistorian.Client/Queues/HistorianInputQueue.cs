//******************************************************************************************************
//  HistorianInputQueue.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  1/4/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GSF.Threading;
using openHistorian.Collections;

namespace openHistorian.Queues
{

    /// <summary>
    /// Serves as a local queue for getting data into a remote historian. 
    /// This queue will isolate the input from the volitality of a 
    /// remote historian. Data is also kept in this buffer until it has been committed
    /// to the disk subsystem. 
    /// </summary>
    public class HistorianInputQueue : IDisposable
    {
        struct PointData
        {
            public ulong Key1;
            public ulong Key2;
            public ulong Value1;
            public ulong Value2;
            public bool Load(IPointStream stream)
            {
                return stream.Read(out Key1, out Key2, out Value1, out Value2);
            }
        }

        StreamPoints m_pointStream;

        object m_syncWrite;

        IHistorianDatabase m_database;

        IsolatedQueue<PointData> m_blocks;

        ScheduledTask m_worker;

        Func<IHistorianDatabase> m_getDatabase;

        public HistorianInputQueue(Func<IHistorianDatabase> getDatabase)
        {
            m_syncWrite = new object();
            m_blocks = new IsolatedQueue<PointData>();
            m_pointStream = new StreamPoints(m_blocks, 1000);
            m_getDatabase = getDatabase;
            m_worker = new ScheduledTask(WorkerDoWork, WorkerCleanUp);
        }

        void WorkerCleanUp()
        {
            
        }

        /// <summary>
        /// Provides a thread safe way to enqueue points. 
        /// While points are streaming all other writes are blocked. Therefore,
        /// this point stream should be high speed.
        /// </summary>
        /// <param name="stream"></param>
        public void Enqueue(IPointStream stream)
        {
            lock (m_syncWrite)
            {
                PointData data = default(PointData);
                while (data.Load(stream))
                {
                    m_blocks.Enqueue(data);
                }
            }
            m_worker.Start();
        }

        /// <summary>
        /// Adds point data to the queue.
        /// </summary>
        public void Enqueue(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            lock (m_syncWrite)
            {
                PointData data = new PointData()
                {
                    Key1 = key1,
                    Key2 = key2,
                    Value1 = value1,
                    Value2 = value2
                };
                m_blocks.Enqueue(data);
            }
            m_worker.Start();
        }

        void WorkerDoWork()
        {
            m_pointStream.Reset();

            try
            {
                if (m_database == null)
                    m_database = m_getDatabase();
                m_database.Write(m_pointStream);
            }
            catch (Exception)
            {
                m_database = null;
                m_worker.Start(new TimeSpan(TimeSpan.TicksPerSecond * 1));
                return;
            }

            if (m_pointStream.QuitOnPointCount)
                m_worker.Start();
            else
                m_worker.Start(new TimeSpan(TimeSpan.TicksPerSecond * 1));
        }

        private class StreamPoints : IPointStream
        {
            IsolatedQueue<PointData> m_measurements;
            bool m_canceled = false;
            int m_maxPoints;
            int m_count;
            public StreamPoints(IsolatedQueue<PointData> measurements, int maxPointsPerStream)
            {
                m_measurements = measurements;
                m_maxPoints = maxPointsPerStream;
            }

            public void Reset()
            {
                m_canceled = false;
                m_count = 0;
            }

            public bool QuitOnPointCount
            {
                get
                {
                    return m_count >= m_maxPoints;
                }
            }

            public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
            {
                PointData data;
                if (!m_canceled && m_count < m_maxPoints && m_measurements.TryDequeue(out data))
                {
                    key1 = data.Key1;
                    key2 = data.Key2;
                    value1 = data.Value1;
                    value2 = data.Value2;
                    m_count++;
                    return true;
                }
                m_canceled = true;
                key1 = 0;
                key2 = 0;
                value1 = 0;
                value2 = 0;
                return false;
            }

            public void Cancel()
            {
                m_canceled = true;
            }
        }

        public void Dispose()
        {
            m_worker.Dispose();
        }
    }
}
