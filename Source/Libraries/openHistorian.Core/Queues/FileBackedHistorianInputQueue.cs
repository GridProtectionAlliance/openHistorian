////******************************************************************************************************
////  FileBackedHistorianInputQueue.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://opensource.org/licenses/MIT
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  1/18/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////
////******************************************************************************************************

//using System;
//using System.IO;
//using GSF;
//using GSF.Collections;
//using GSF.SortedTreeStore;
//using GSF.SortedTreeStore.Services;
//using GSF.Threading;
//using openHistorian.Collections;

//namespace openHistorian.Queues
//{
//    /// <summary>
//    /// Serves as a local queue for getting data into a remote historian. 
//    /// This queue will isolate the input from the volitality of a 
//    /// remote historian. Data is also kept in this buffer until it has been committed
//    /// to the disk subsystem. 
//    /// </summary>
//    public class FileBackedHistorianInputQueue : IDisposable
//    {
//        private struct PointData : ILoadable
//        {
//            public ulong Key1;
//            public ulong Key2;
//            public ulong Value1;
//            public ulong Value2;

//            public bool Load(TreeStream<HistorianKey, HistorianValue> stream)
//            {
//                HistorianKey key = new HistorianKey();
//                HistorianValue value = new HistorianValue();

//                if (stream.Read(key,value))
//                {
//                    Key1 = key.Timestamp;
//                    Key2 = key.PointID;
//                    Value1 = value.Value3;
//                    Value2 = value.Value1;
//                    return true;
//                }
//                return false;
//            }

//            public unsafe int InMemorySize
//            {
//                get
//                {
//                    return sizeof(PointData);
//                }
//            }

//            public int OnDiskSize
//            {
//                get
//                {
//                    return 32;
//                }
//            }

//            public void Save(BinaryWriter writer)
//            {
//                writer.Write(Key1);
//                writer.Write(Key2);
//                writer.Write(Value1);
//                writer.Write(Value2);
//            }

//            public void Load(BinaryReader reader)
//            {
//                Key1 = reader.ReadUInt64();
//                Key2 = reader.ReadUInt64();
//                Value1 = reader.ReadUInt64();
//                Value2 = reader.ReadUInt64();
//            }
//        }

//        private readonly StreamPoints m_pointStream;

//        private readonly object m_syncWrite;

//        private ClientDatabaseBase<HistorianKey, HistorianValue> m_database;

//        private readonly IsolatedQueueFileBacked<PointData> m_blocks;

//        private readonly ScheduledTask m_worker;

//        private readonly Func<ClientDatabaseBase<HistorianKey, HistorianValue>> m_getDatabase;

//        /// <summary>
//        /// Creates a new <see cref="FileBackedHistorianInputQueue"/>. 
//        /// </summary>
//        /// <param name="getDatabase">A lamda expression for connecting to a database</param>
//        /// <param name="path">The disk path to use to save the state of this queue to. 
//        /// It is critical that this path and file prefix is unique to the instance of this class.</param>
//        /// <param name="filePrefix">The prefix string to add to the beginning of every file in this directory.</param>
//        /// <param name="maxInMemorySize">The maximum desired in-memory size before switching to a file storage method.</param>
//        /// <param name="individualFileSize">The desired size of each file.</param>
//        /// <remarks>The total memory used by this class will be approximately the sum of <see cref="maxInMemorySize"/> and
//        /// <see cref="individualFileSize"/> while operating in file mode.</remarks>
//        public FileBackedHistorianInputQueue(Func<ClientDatabaseBase<HistorianKey, HistorianValue>> getDatabase, string path, string filePrefix, int maxInMemorySize, int individualFileSize)
//        {
//            m_syncWrite = new object();
//            m_blocks = new IsolatedQueueFileBacked<PointData>(path, filePrefix, maxInMemorySize, individualFileSize);
//            m_pointStream = new StreamPoints(m_blocks, 1000);
//            m_getDatabase = getDatabase;
//            m_worker = new ScheduledTask(ThreadingMode.DedicatedForeground);
//            m_worker.Running += WorkerDoWork;
//        }

//        /// <summary>
//        /// Provides a thread safe way to enqueue points. 
//        /// While points are streaming all other writes are blocked. Therefore,
//        /// this point stream should be high speed.
//        /// </summary>
//        /// <param name="stream"></param>
//        public void Enqueue(TreeStream<HistorianKey, HistorianValue> stream)
//        {
//            lock (m_syncWrite)
//            {
//                PointData data = default(PointData);
//                while (data.Load(stream))
//                {
//                    m_blocks.Enqueue(data);
//                }
//            }
//            m_worker.Start();
//        }

//        /// <summary>
//        /// Adds point data to the queue.
//        /// </summary>
//        public void Enqueue(HistorianKey key, HistorianValue value)
//        {
//            lock (m_syncWrite)
//            {
//                PointData data = new PointData()
//                {
//                    Key1 = key.Timestamp,
//                    Key2 = key.PointID,
//                    Value1 = value.Value3,
//                    Value2 = value.Value1
//                };
//                m_blocks.Enqueue(data);
//            }
//            m_worker.Start();
//        }

//        private void WorkerDoWork(object sender, EventArgs<ScheduledTaskRunningReason> eventArgs)
//        {
//            m_pointStream.Reset();

//            try
//            {
//                if (m_database is null)
//                    m_database = m_getDatabase();
//                m_database.Write(m_pointStream);
//            }
//            catch (Exception)
//            {
//                m_database = null;
//                m_worker.Start(1000);
//                return;
//            }

//            if (m_pointStream.QuitOnPointCount)
//                m_worker.Start();
//            else
//                m_worker.Start(1000);
//        }


//        private class StreamPoints
//            : TreeStream<HistorianKey, HistorianValue>
//        {
//            private readonly IsolatedQueueFileBacked<PointData> m_measurements;
//            private readonly int m_maxPoints;
//            private int m_count;

//            public StreamPoints(IsolatedQueueFileBacked<PointData> measurements, int maxPointsPerStream)
//            {
//                m_measurements = measurements;
//                m_maxPoints = maxPointsPerStream;
//            }

//            public void Reset()
//            {
//                m_count = 0;
//                SetEos(false);
//            }

//            protected override void EndOfStreamReached()
//            {
//                SetEos(true);
//            }

//            public bool QuitOnPointCount
//            {
//                get
//                {
//                    return m_count >= m_maxPoints;
//                }
//            }


//            protected override bool ReadNext(HistorianKey key, HistorianValue value)
//            {
//                PointData data;
//                if (m_count < m_maxPoints && m_measurements.TryDequeue(out data))
//                {
//                    key.Timestamp = data.Key1;
//                    key.PointID = data.Key2;
//                    value.Value3 = data.Value1;
//                    value.Value1 = data.Value2;
//                    m_count++;
//                    return true;
//                }
//                key.Timestamp = 0;
//                key.PointID = 0;
//                value.Value3 = 0;
//                value.Value1 = 0;
//                return false;
//            }
            
//        }

//        public void Dispose()
//        {
//            m_worker.Dispose();
//        }
//    }
//}