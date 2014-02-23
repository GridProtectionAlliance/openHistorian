//******************************************************************************************************
//  PointStreamCache`2.cs - Gbtc
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
using GSF.IO.Unmanaged;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Writer
{
    /// <summary>
    /// Serves as a local cache of a point stream. Points can be added or read from this class, however
    /// this cache will only operate in read or write mode at any given time. Change the operating mode by
    /// calling <see cref="ClearAndSetWriting"/> or <see cref="SetReadingFromBeginning"/>.
    /// </summary>
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    public class PointStreamCache<TKey, TValue>
        : TreeStream<TKey, TValue>, IDisposable
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        //ToDo: Automatically detect size and shrink the queue if need be. 
        //This might be acomplished by keeping the size over the past 10 times
        //it has been used, and not allowing it to grow beyond 2 times
        //the average size.

        private readonly BinaryStream m_queue;
        private int m_pointCount;
        private int m_remainingPoints;
        private bool m_isReading;
        private readonly SortedTreeTypeMethods<TKey> m_keyMethods;
        private readonly SortedTreeTypeMethods<TValue> m_valueMethods;

        /// <summary>
        /// Creates a point stream cache
        /// </summary>
        public PointStreamCache()
        {
            m_keyMethods = new TKey().CreateValueMethods();
            m_valueMethods = new TValue().CreateValueMethods();
            m_queue = new BinaryStream(allocatesOwnMemory: true);
            m_isReading = false;
            m_pointCount = 0;
        }

        /// <summary>
        /// Gets the number of points in the queue.
        /// </summary>
        public int Count
        {
            get
            {
                return m_pointCount;
            }
        }

        /// <summary>
        /// Writes the provided key and value to the cache
        /// </summary>
        /// <param name="key">the key to write</param>
        /// <param name="value">the value to write</param>
        public void Write(TKey key, TValue value)
        {
            if (m_isReading)
                throw new Exception("Cannot write to a stream while it is being read.");

            m_pointCount++;
            key.Write(m_queue);
            value.Write(m_queue);
        }

        /// <summary>
        /// Writes the provided stream to the cache
        /// </summary>
        /// <param name="stream">the stream to write</param>
        public void Write(TreeStream<TKey, TValue> stream)
        {
            TKey key = new TKey();
            TValue value = new TValue();

            if (m_isReading)
                throw new Exception("Cannot write to a stream while it is being read.");

            while (stream.Read(key, value))
            {
                m_pointCount++;
                key.Write(m_queue);
                value.Write(m_queue);
            }
        }

        /// <summary>
        /// Advances to the next entry
        /// </summary>
        /// <returns>
        /// Returns true if the next value is valid. Returns false if the end of the stream has been encountered.
        /// </returns>
        public override bool Read(TKey key, TValue value)
        {
            if (!m_isReading)
                throw new Exception("Cannot read from a stream while it is being written to.");

            if (m_remainingPoints > 0)
            {
                key.Read(m_queue);
                value.Read(m_queue);
                m_remainingPoints--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Changes the mode of this cache to writing mode
        /// and clears any of the data in the cache.
        /// </summary>
        public void ClearAndSetWriting()
        {
            m_pointCount = 0;
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
            m_isReading = true;
            m_remainingPoints = m_pointCount;
            m_queue.Position = 0;
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