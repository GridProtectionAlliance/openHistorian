//******************************************************************************************************
//  CombinedEncodingFixedSize`2.cs - Gbtc
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
//  02/21/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace GSF.Snap.Encoding
{
    /// <summary>
    /// An encoding method that is fixed in size and calls the native read/write functions of the specified type.
    /// </summary>
    /// <typeparam name="TKey">The type to use as the key</typeparam>
    /// <typeparam name="TValue">The type to use as the value</typeparam>
    internal class PairEncodingFixedSize<TKey, TValue>
        : PairEncodingBase<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        private readonly int m_keySize;
        private readonly int m_valueSize;

        /// <summary>
        /// Creates a new class
        /// </summary>
        public PairEncodingFixedSize()
        {
            m_keySize = new TKey().Size;
            m_valueSize = new TValue().Size;
        }

        /// <summary>
        /// Gets the encoding method that this class implements.
        /// </summary>
        public override EncodingDefinition EncodingMethod => EncodingDefinition.FixedSizeCombinedEncoding;

        /// <summary>
        /// Gets if the previous key will need to be presented to the encoding algorithms to
        /// property encode the next sample. Returning false will cause nulls to be passed
        /// in a parameters to the encoding.
        /// </summary>
        public override bool UsesPreviousKey => false;

        /// <summary>
        /// Gets if the previous value will need to be presented to the encoding algorithms to
        /// property encode the next sample. Returning false will cause nulls to be passed
        /// in a parameters to the encoding.
        /// </summary>
        public override bool UsesPreviousValue => false;

        /// <summary>
        /// Gets the maximum amount of space that is required for the compression algorithm. This
        /// prevents lower levels from having overflows on the underlying streams. It is critical
        /// that this value be correct. Error on the side of too large of a value as a value
        /// too small will corrupt data and be next to impossible to track down the point of corruption
        /// </summary>
        public override int MaxCompressionSize => m_keySize + m_valueSize;

        /// <summary>
        /// Gets if the stream supports a symbol that 
        /// represents that the end of the stream has been encountered.
        /// </summary>
        /// <remarks>
        /// An example of a symbol would be the byte code 0xFF.
        /// In this case, if the first byte of the
        /// word is 0xFF, the encoding has specifically
        /// designated this as the end of the stream. Therefore, calls to
        /// Decompress will result in an end of stream exception.
        /// 
        /// Failing to reserve a code as the end of stream will mean that
        /// streaming points will include its own symbol to represent the end of the
        /// stream, taking 1 extra byte per point encoded.
        /// </remarks>
        public override bool ContainsEndOfStreamSymbol => false;

        /// <summary>
        /// The byte code to use as the end of stream symbol.
        /// May throw NotSupportedException if <see cref="PairEncodingBase{TKey,TValue}.ContainsEndOfStreamSymbol"/> is false.
        /// </summary>
        public override byte EndOfStreamSymbol => throw new NotSupportedException();

        /// <summary>
        /// Encodes <see cref="key"/> and <see cref="value"/> to the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">where to write the data</param>
        /// <param name="prevKey">the previous key if required by <see cref="PairEncodingBase{TKey,TValue}.UsesPreviousKey"/>. Otherwise null.</param>
        /// <param name="prevValue">the previous value if required by <see cref="PairEncodingBase{TKey,TValue}.UsesPreviousValue"/>. Otherwise null.</param>
        /// <param name="key">the key to encode</param>
        /// <param name="value">the value to encode</param>
        /// <returns>the number of bytes necessary to encode this key/value.</returns>
        public override void Encode(BinaryStreamBase stream, TKey prevKey, TValue prevValue, TKey key, TValue value)
        {
            key.Write(stream);
            value.Write(stream);
        }

        /// <summary>
        /// Decodes <see cref="key"/> and <see cref="value"/> from the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">where to read the data</param>
        /// <param name="prevKey">the previous key if required by <see cref="PairEncodingBase{TKey,TValue}.UsesPreviousKey"/>. Otherwise null.</param>
        /// <param name="prevValue">the previous value if required by <see cref="PairEncodingBase{TKey,TValue}.UsesPreviousValue"/>. Otherwise null.</param>
        /// <param name="key">the place to store the decoded key</param>
        /// <param name="value">the place to store the decoded value</param>
        /// <param name="isEndOfStream">outputs true if the end of the stream symbol is detected. Not all encoding methods have an end of stream symbol and therefore will always return false.</param>
        /// <returns>the number of bytes necessary to decode the next key/value.</returns>
        public override void Decode(BinaryStreamBase stream, TKey prevKey, TValue prevValue, TKey key, TValue value, out bool isEndOfStream)
        {
            isEndOfStream = false;
            key.Read(stream);
            value.Read(stream);
        }

        /// <summary>
        /// Decodes <see cref="key"/> and <see cref="value"/> from the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">where to read the data</param>
        /// <param name="prevKey">the previous key if required by <see cref="PairEncodingBase{TKey,TValue}.UsesPreviousKey"/>. Otherwise null.</param>
        /// <param name="prevValue">the previous value if required by <see cref="PairEncodingBase{TKey,TValue}.UsesPreviousValue"/>. Otherwise null.</param>
        /// <param name="key">the place to store the decoded key</param>
        /// <param name="value">the place to store the decoded value</param>
        /// <param name="isEndOfStream">outputs true if the end of the stream symbol is detected. Not all encoding methods have an end of stream symbol and therefore will always return false.</param>
        /// <returns>the number of bytes necessary to decode the next key/value.</returns>
        public override unsafe int Decode(byte* stream, TKey prevKey, TValue prevValue, TKey key, TValue value, out bool isEndOfStream)
        {
            isEndOfStream = false;
            key.Read(stream);
            value.Read(stream + m_keySize);
            return m_keySize + m_valueSize;
        }

        /// <summary>
        /// Encodes <see cref="key"/> and <see cref="value"/> to the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">where to write the data</param>
        /// <param name="prevKey">the previous key if required by <see cref="PairEncodingBase{TKey,TValue}.UsesPreviousKey"/>. Otherwise null.</param>
        /// <param name="prevValue">the previous value if required by <see cref="PairEncodingBase{TKey,TValue}.UsesPreviousValue"/>. Otherwise null.</param>
        /// <param name="key">the key to encode</param>
        /// <param name="value">the value to encode</param>
        /// <returns>the number of bytes necessary to encode this key/value.</returns>
        public override unsafe int Encode(byte* stream, TKey prevKey, TValue prevValue, TKey key, TValue value)
        {
            key.Write(stream);
            value.Write(stream + m_keySize);
            return m_keySize + m_valueSize;
        }

        /// <summary>
        /// Clones this encoding method.
        /// </summary>
        /// <returns>A clone</returns>
        public override PairEncodingBase<TKey, TValue> Clone()
        {
            return new PairEncodingFixedSize<TKey, TValue>();
        }
    }
}
