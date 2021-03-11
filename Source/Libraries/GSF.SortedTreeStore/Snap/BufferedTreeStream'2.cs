//******************************************************************************************************
//  BufferedTreeStream'2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/23/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Snap
{
    public partial class UnionTreeStream<TKey, TValue>
    {
        /// <summary>
        /// A wrapper around a <see cref="TreeStream{TKey,TValue}"/> that primarily supports peaking
        /// a value from a stream.
        /// </summary>
        private class BufferedTreeStream
            : IDisposable
        {
            public TreeStream<TKey, TValue> Stream;
            public bool IsValid;
            public readonly TKey CacheKey = new TKey();
            public readonly TValue CacheValue = new TValue();

            /// <summary>
            /// Creates the table reader.
            /// </summary>
            /// <param name="stream"></param>
            public BufferedTreeStream(TreeStream<TKey, TValue> stream)
            {
                if (!stream.IsAlwaysSequential)
                    throw new ArgumentException("Stream must gaurentee sequential data access");
                if (!stream.NeverContainsDuplicates)
                    stream = new DistinctTreeStream<TKey, TValue>(stream);

                Stream = stream;
                EnsureCache();
            }
          
            /// <summary>
            /// Makes sure that the cache value is valid.
            /// </summary>
            public void EnsureCache()
            {
                if (!IsValid)
                    ReadToCache();
            }

            /// <summary>
            /// Reads the next value of the stream.
            /// </summary>
            public void ReadToCache()
            {
                IsValid = Stream.Read(CacheKey, CacheValue);
            }

            /// <summary>
            /// Reads the next available value.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public void Read(TKey key, TValue value)
            {
                if (IsValid)
                {
                    IsValid = false;
                    CacheKey.CopyTo(key);
                    CacheValue.CopyTo(value);
                    return;
                }
                throw new Exception("Cache is not valid. Programming Error.");
            }

            /// <summary>
            /// Writes this value back to the point buffer.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void WriteToCache(TKey key, TValue value)
            {
                IsValid = true;
                key.CopyTo(CacheKey);
                value.CopyTo(CacheValue);
            }

            public void Dispose()
            {
                if (Stream != null)
                {
                    Stream.Dispose();
                    Stream = null;
                }

            }
        }

    }
}
