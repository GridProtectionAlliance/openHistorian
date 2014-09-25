//******************************************************************************************************
//  BufferedTreeStream'2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  9/23/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services.Reader
{
    /// <summary>
    /// A wrapper around a <see cref="TreeStream{TKey,TValue}"/> that primarily supports peaking
    /// a value from a stream.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class BufferedTreeStream<TKey, TValue>
        : IDisposable
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        TreeStream<TKey, TValue> m_table;

        /// <summary>
        /// An index value that is used to disassociate the archive file. Passed to this class from the <see cref="SortedTreeEngineReaderSequential{TKey,TValue}"/>
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Creates the table reader.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="table"></param>
        public BufferedTreeStream(int index, TreeStream<TKey, TValue> table)
        {
            if (!table.IsAlwaysSequential)
                throw new ArgumentException("Stream must gaurentee sequential data access");
            if (!table.NeverContainsDuplicates)
                table = new DistinctTreeStream<TKey, TValue>(table);

            Index = index;
            m_table = table;
        }

        public bool CacheIsValid = false;
        public TKey CacheKey = new TKey();
        public TValue CacheValue = new TValue();

        /// <summary>
        /// Makes sure that the cache value is valid.
        /// </summary>
        public void EnsureCache()
        {
            if (!CacheIsValid)
                ReadToCache();
        }

        /// <summary>
        /// Reads the next value of the stream.
        /// </summary>
        public void ReadToCache()
        {
            CacheIsValid = m_table.Read(CacheKey, CacheValue);
        }

        /// <summary>
        /// Reads the next available value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Read(TKey key, TValue value)
        {
            if (CacheIsValid)
            {
                CacheIsValid = false;
                CacheKey.CopyTo(key);
                CacheValue.CopyTo(value);
                return true;
            }
            return m_table.Read(key, value);
        }

        /// <summary>
        /// Writes this value back to the point buffer.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteToCache(TKey key, TValue value)
        {
            CacheIsValid = true;
            key.CopyTo(CacheKey);
            value.CopyTo(CacheValue);
        }

        public void Dispose()
        {
            if (m_table != null)
            {
                m_table.Dispose();
                m_table = null;
            }
        }
    }
}
