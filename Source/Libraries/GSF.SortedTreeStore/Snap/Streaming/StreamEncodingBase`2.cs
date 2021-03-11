//******************************************************************************************************
//  StreamEncodingBase`2.cs - Gbtc
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
//  08/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.IO;
using GSF.Snap.Encoding;

namespace GSF.Snap.Streaming
{
    /// <summary>
    /// Encoding that is stream based. This encoding is similar to <see cref="PairEncodingBase{TKey,TValue}"/>
    /// except it contains end of stream data.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class StreamEncodingBase<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {

        /// <summary>
        /// Gets the definition of the encoding used.
        /// </summary>
        public abstract EncodingDefinition EncodingMethod { get; }

        /// <summary>
        /// Writes the end of the stream symbol to the <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public abstract void WriteEndOfStream(BinaryStreamBase stream);

        /// <summary>
        /// Encodes the current key/value to the stream.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="currentKey">the key to write</param>
        /// <param name="currentValue">the value to write</param>
        public abstract void Encode(BinaryStreamBase stream, TKey currentKey, TValue currentValue);

        /// <summary>
        /// Attempts to read the next point from the stream. 
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="key">the key to store the value to</param>
        /// <param name="value">the value to store to</param>
        /// <returns>True if successful. False if end of the stream has been reached.</returns>
        public abstract bool TryDecode(BinaryStreamBase stream, TKey key, TValue value);

        /// <summary>
        /// Resets the encoder. Some encoders maintain streaming state data that should
        /// be reset when reading from a new stream.
        /// </summary>
        public abstract void ResetEncoder();

    }
}
