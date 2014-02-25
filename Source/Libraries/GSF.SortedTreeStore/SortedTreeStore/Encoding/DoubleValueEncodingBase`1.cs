//******************************************************************************************************
//  DoubleValueEncodingBase`1.cs - Gbtc
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
//  2/21/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using GSF.IO;
using GSF.IO.Unmanaged;

namespace GSF.SortedTreeStore.Encoding
{
    public abstract class DoubleValueEncodingBase<TKey, TValue>
    {
        public abstract bool UsesPreviousKey { get; }

        public abstract bool UsesPreviousValue { get; }

        public abstract int MaxCompressionSize { get; }

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
        public abstract bool ContainsEndOfStreamSymbol { get; }

        /// <summary>
        /// The byte code to use as the end of stream symbol.
        /// May throw NotSupportedException if <see cref="ContainsEndOfStreamSymbol"/> is false.
        /// </summary>
        public abstract byte EndOfStreamSymbol { get; }

        public unsafe virtual int Compress(byte* stream, TKey prevKey, TValue prevValue, TKey key, TValue value)
        {
            var bs = new BinaryStreamPointerWrapper(stream, MaxCompressionSize);
            Compress(bs, prevKey, prevValue, key, value);
            return (int)bs.Position;
        }

        public unsafe virtual int Decompress(byte* stream, TKey prevKey, TValue prevValue, TKey key, TValue value)
        {
            var bs = new BinaryStreamPointerWrapper(stream, MaxCompressionSize);
            Decompress(bs, prevKey, prevValue, key, value);
            return (int)bs.Position;
        }

        public abstract void Compress(BinaryStreamBase stream, TKey prevKey, TValue prevValue, TKey key, TValue value);

        public abstract void Decompress(BinaryStreamBase stream, TKey prevKey, TValue prevValue, TKey key, TValue value);

        public abstract DoubleValueEncodingBase<TKey, TValue> Clone();
    }
}
