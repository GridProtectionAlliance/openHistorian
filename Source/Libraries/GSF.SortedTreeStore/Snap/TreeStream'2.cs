//******************************************************************************************************
//  TreeStream'2.cs - Gbtc
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
//  09/15/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Threading.Tasks;

namespace GSF.Snap
{
    /// <summary>
    /// Represents a stream of KeyValues
    /// </summary>
    /// <typeparam name="TKey">The key associated with the point</typeparam>
    /// <typeparam name="TValue">The value associated with the point</typeparam>
    public abstract class TreeStream<TKey, TValue> : IDisposable
        where TKey : class, new()
        where TValue : class, new()
    {
        private bool m_eos;
        private bool m_disposed;

        /// <summary>
        /// Boolean indicating that the end of the stream has been read or class has been disposed.
        /// </summary>
        public bool EOS => m_eos;

        /// <summary>
        /// Gets if the stream is always in sequential order. Do not return true unless it is Guaranteed that 
        /// the data read from this stream is sequential.
        /// </summary>
        public virtual bool IsAlwaysSequential => false;

        /// <summary>
        /// Gets if the stream will never return duplicate keys. Do not return true unless it is Guaranteed that 
        /// the data read from this stream will never contain duplicates.
        /// </summary>
        public virtual bool NeverContainsDuplicates => false;

        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        public bool Read(TKey key, TValue value)
        {
            if (m_eos || !ReadNext(key, value))
            {
                EndOfStreamReached();
                return false;
            }
            return true;
        }

    #if !SQLCLR
        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        public async ValueTask<bool> ReadAsync(TKey key, TValue value)
        {
            if (m_eos || !await new ValueTask<bool>(ReadNext(key, value)))
            {
                EndOfStreamReached();
                return false;
            }

            return true;
        }
    #endif

        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        public async ValueTask<bool> ReadAsync(TKey key, TValue value)
        {
            if (m_eos || !await new ValueTask<bool>(ReadNext(key, value)))
            {
                EndOfStreamReached();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Occurs when the end of the stream has been reached. The default behavior is to call <see cref="Dispose"/>
        /// </summary>
        protected virtual void EndOfStreamReached()
        {
            Dispose();
        }

        /// <summary>
        /// Allow rolling back the EOS
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if <see cref="value"/> is <see cref="Boolean.False"/> and this class
        /// has already been disposed</exception>
        protected void SetEos(bool value)
        {
            if (m_disposed && !value)
                throw new ObjectDisposedException(GetType().FullName);
            m_eos = value;
        }

        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        protected abstract bool ReadNext(TKey key, TValue value);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    Dispose(true);
                }
                finally
                {
                    m_eos = true;
                    m_disposed = true;
                }
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="TreeStream{TKey,TValue}"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            m_eos = true;
        }
    }
}