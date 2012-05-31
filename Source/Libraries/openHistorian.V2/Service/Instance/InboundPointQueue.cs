//******************************************************************************************************
//  InboundPointQueue.cs - Gbtc
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
//  5/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.Service.Instance
{
    /// <summary>
    /// The first layer of data insertion. This is a holding location for point data
    /// so committing points occurs at a slower interval.
    /// </summary>
    class InboundPointQueue
    {
        //ToDO: make the queue be an in memory b+ tree.  This may speed up the inserting 
        //to the main database.

        MemoryStream m_actriveMemoryStream;
        MemoryStream m_processingMemoryStream;

        BinaryStream m_processingQueue;
        BinaryStream m_activeQueue;
        const int SizeOfData = 32;

        public InboundPointQueue()
        {
            m_actriveMemoryStream = new MemoryStream();
            m_activeQueue = new BinaryStream(m_actriveMemoryStream);
            m_processingMemoryStream = new MemoryStream();
            m_activeQueue = new BinaryStream(m_processingMemoryStream);

        }

        /// <summary>
        /// Provides the synchronization object for lock free method calls.
        /// </summary>
        /// <remarks>Since this class does its own internal synchronization on this object,
        /// the user must still synchronize on this object for lock free methods of this class.</remarks>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Writes the following data to the queue without locks. 
        /// User must lock on <see cref="SyncRoot"/> 
        /// in order to use this function.
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        public void WriteDataWithoutLocks(long key1, long key2, long value1, long value2)
        {
            m_activeQueue.Write(key1);
            m_activeQueue.Write(key2);
            m_activeQueue.Write(value1);
            m_activeQueue.Write(value2);
        }

        /// <summary>
        /// Writes the following data to the queue.
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <remarks>This call locks on <see cref="SyncRoot"/>.  
        /// To prevent repeated locks, lock <see cref="SyncRoot"/> 
        /// and call <see cref="WriteDataWithoutLocks"/>.</remarks>
        public void WriteData(long key1, long key2, long value1, long value2)
        {
            lock (this)
            {
                m_activeQueue.Write(key1);
                m_activeQueue.Write(key2);
                m_activeQueue.Write(value1);
                m_activeQueue.Write(value2);
            }
        }

        /// <summary>
        /// Outputs the most recent active binary stream.
        /// </summary>
        /// <param name="stream">The input queue stream</param>
        /// <param name="pointCount">the number of points in the stream</param>
        /// <remarks>This call will dequeue all points currently in the point queue.
        /// Since this class swaps an active and a processing queue, reads do not have to be
        /// synchronized with active inputs.</remarks>
        public void GetPointBlock(out BinaryStream stream, out int pointCount)
        {
            lock (this)
            {
                stream = m_activeQueue;
                pointCount = (int)(m_activeQueue.Position / SizeOfData);
                m_activeQueue.Position = 0;

                m_activeQueue = m_processingQueue;
                m_activeQueue.Position = 0;
                m_processingQueue = m_activeQueue;
            }
        }
    }
}
