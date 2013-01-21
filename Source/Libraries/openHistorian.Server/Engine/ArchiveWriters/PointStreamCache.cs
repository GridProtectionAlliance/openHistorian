//******************************************************************************************************
//  PointStreamCache.cs - Gbtc
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
//  1/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************


using System;
using GSF;
using GSF.IO.Unmanaged;

namespace openHistorian.Engine.ArchiveWriters
{
    /// <summary>
    /// Serves as a local cache of a point stream. Points can be added or read from this class, however
    /// this cache will only operate in read or write mode at any given time. Change the operating mode by
    /// calling <see cref="ClearAndSetWriting"/> or <see cref="SetReadingFromBeginning"/>.
    /// </summary>
    internal class PointStreamCache : IStream256, IDisposable
    {
        //ToDo: Use a managed array of ulong instead of an unmanaged queue.
        //Jagged array of course.

        //ToDo: Automatically detect size and shrink the queue if need be. 
        //This might be acomplished by keeping the size over the past 10 times
        //it has been used, and not allowing it to grow beyond 2 times
        //the average size.

        BinaryStream m_queue;
        int m_pointCount;
        int m_remainingPoints;
        const int SizeOfData = 32;
        bool m_isReading;

        public PointStreamCache()
        {
            m_queue = new BinaryStream();
            m_isReading = false;
            m_pointCount = 0;
        }

        public bool IsReading
        {
            get
            {
                return m_isReading;
            }
        }

        /// <summary>
        /// Gets the number of points in the queue.
        /// </summary>
        public int Count
        {
            get
            {
                if (m_isReading)
                    return m_pointCount;
                return (int)(m_queue.Position / SizeOfData);
            }
        }

        public void Write(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            if (m_isReading)
                throw new Exception("Cannot write to a stream while it is being read.");
            m_queue.Write(key1);
            m_queue.Write(key2);
            m_queue.Write(value1);
            m_queue.Write(value2);
        }

        public void Write(IStream256 stream)
        {
            if (m_isReading)
                throw new Exception("Cannot write to a stream while it is being read.");

            ulong key1, key2, value1, value2;
            while (stream.Read(out key1, out key2, out value1, out value2))
            {
                m_queue.Write(key1);
                m_queue.Write(key2);
                m_queue.Write(value1);
                m_queue.Write(value2);
            }
        }

        public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
        {
            if (!m_isReading)
                throw new Exception("Cannot read from a stream while it is being written to.");

            if (m_remainingPoints > 0)
            {
                key1 = m_queue.ReadUInt64();
                key2 = m_queue.ReadUInt64();
                value1 = m_queue.ReadUInt64();
                value2 = m_queue.ReadUInt64();
                m_remainingPoints--;
                return true;
            }
            key1 = 0;
            key2 = 0;
            value1 = 0;
            value2 = 0;
            return false;

        }

        /// <summary>
        /// Changes the mode of this cache to writing mode
        /// and clears any of the data in the cache.
        /// </summary>
        public void ClearAndSetWriting()
        {
            m_queue.Position = 0;
            m_isReading = false;
        }

        /// <summary>
        /// Sets the mode of this cache to reading.
        /// Also sets the read position of this class to the
        /// beginning of the queue. This can also be used to reset the reader.
        /// </summary>
        public void SetReadingFromBeginning()
        {
            if (m_isReading)
            {
                m_queue.Position = 0;
                m_remainingPoints = m_pointCount;
            }
            else
            {
                m_isReading = true;
                m_pointCount = (int)(m_queue.Position / SizeOfData);
                m_remainingPoints = m_pointCount;
                m_queue.Position = 0;
            }
        }

        /// <summary>
        /// Disposes of internal resources used by this class.
        /// </summary>
        public void Dispose()
        {
            m_queue.Dispose();
        }
    }
}
