//******************************************************************************************************
//  PointQueue.cs - Gbtc
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

using System;
using openHistorian.IO.Unmanaged;

namespace openHistorian.Engine.ArchiveWriters
{
    /// <summary>
    /// Provides a way for points to be synchronously queued and processed in bulk.
    /// </summary>
    internal class PointQueue : IDisposable
    {
        const int SizeOfData = 32;

        //ToDo: Keep statistics on the size of the queue.  It's possible that the underlying memory stream may get very large and will never reduce in size.

        bool m_disposed;
        bool m_stopped;
        BinaryStream m_processingQueue;
        BinaryStream m_activeQueue;
        object m_syncRoot;
        long m_sequenceId;

        /// <summary>
        /// Creates a new inbound queue to buffer points.
        /// </summary>
        public PointQueue()
        {
            m_sequenceId = 0;
            m_syncRoot = new object();
            m_activeQueue = new BinaryStream();
            m_processingQueue = new BinaryStream();
        }

        /// <summary>
        /// Writes the following data to the queue.
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        public long WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            lock (m_syncRoot)
            {
                if (m_stopped)
                    throw new Exception("No new points can be added. Point queue has been stopped.");
                m_sequenceId++;
                m_activeQueue.Write(key1);
                m_activeQueue.Write(key2);
                m_activeQueue.Write(value1);
                m_activeQueue.Write(value2);
                return m_sequenceId;
            }
        }

        public long SequenceId
        {
            get
            {
                lock (m_syncRoot)
                {
                    return m_sequenceId;
                }
            }
        }

        /// <summary>
        /// Outputs the most recent active binary stream.
        /// </summary>
        /// <param name="stream">The input queue stream</param>
        /// <param name="pointCount">the number of points in the stream</param>
        /// <param name="sequenceId">the sequence number that represents all of these points</param>
        /// <param name="stopInputs">set if class is getting ready to dispose and therefore all writes should terminate</param>
        /// <remarks>This call will dequeue all points currently in the point queue.
        /// Since this class swaps an active and a processing queue, reads do not have to be
        /// synchronized with active inputs.</remarks>
        public void GetPointBlock(out BinaryStream stream, out int pointCount, out long sequenceId, bool stopInputs)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            lock (m_syncRoot)
            {
                if (stopInputs)
                    m_stopped = true;

                stream = m_activeQueue;
                pointCount = (int)(stream.Position / SizeOfData);
                stream.Position = 0;

                m_activeQueue = m_processingQueue;
                m_activeQueue.Position = 0;

                m_processingQueue = stream;
                sequenceId = m_sequenceId;
            }
        }

        /// <summary>
        /// Disposes the underlying queues contained in this class. 
        /// This method is not thread safe.
        /// </summary>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    if (m_processingQueue != null)
                        m_processingQueue.Dispose();
                    if (m_activeQueue != null)
                        m_activeQueue.Dispose();
                }
                finally
                {
                    m_disposed = true;
                    m_activeQueue = null;
                    m_processingQueue = null;
                }
            }
        }

        public long Stop()
        {
            lock (m_syncRoot)
            {
                m_stopped = true;
                return m_sequenceId;
            }
        }
    }
}
