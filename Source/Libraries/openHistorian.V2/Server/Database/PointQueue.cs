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
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.Server.Database
{
    /// <summary>
    /// Provides a way for points to be synchronously queued and processed in bulk.
    /// </summary>
    class PointQueue : IDisposable
    {
        const int SizeOfData = 32;

        //ToDo: Keep statistics on the size of the queue.  It's possible that the underlying memory stream may get very large and will never reduce in size.
        Action m_queueFullCallback;

        bool m_disposed;
        BinaryStream m_processingQueue;
        BinaryStream m_activeQueue;
        object m_syncRoot;
        int m_autoCommitQueueSize;
        int m_activePointCount;

        /// <summary>
        /// Creates a new inbound queue to buffer points.
        /// </summary>
        /// <param name="autoCommitQueueSize">After the queue becomes this size, a <see cref="Action"/> 
        /// delegate will be called once.</param>
        /// <param name="queueFullCallback">The optional callback function once the queue becomes exactly autoCommitQueueSize in size.</param>
        public PointQueue(int autoCommitQueueSize = -1, Action queueFullCallback = null)
        {
            m_queueFullCallback = queueFullCallback;
            m_autoCommitQueueSize = autoCommitQueueSize;
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
        public void WriteData(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            bool isQueueFull;
            lock (m_syncRoot)
            {
                m_activePointCount++;
                isQueueFull = (m_autoCommitQueueSize == m_activePointCount);
                m_activeQueue.Write(key1);
                m_activeQueue.Write(key2);
                m_activeQueue.Write(value1);
                m_activeQueue.Write(value2);
            }
            if (isQueueFull && m_queueFullCallback != null)
                m_queueFullCallback.Invoke();
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
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            lock (m_syncRoot)
            {
                stream = m_activeQueue;
                pointCount = (int)(stream.Position / SizeOfData);
                stream.Position = 0;

                m_activeQueue = m_processingQueue;
                m_activeQueue.Position = 0;
                m_activePointCount = 0;

                m_processingQueue = stream;
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
                    m_queueFullCallback = null;
                    m_disposed = true;
                    m_activeQueue = null;
                    m_processingQueue = null;
                }
            }

        }
    }
}
