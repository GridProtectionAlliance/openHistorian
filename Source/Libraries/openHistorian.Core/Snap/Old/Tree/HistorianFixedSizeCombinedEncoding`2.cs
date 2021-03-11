////******************************************************************************************************
////  HistorianFixedSizeCombinedEncoding.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://opensource.org/licenses/MIT
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  02/21/2014 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using GSF.IO;
//using GSF.Snap;
//using GSF.Snap.Encoding;

//namespace openHistorian.Snap.Encoding
//{
//    /// <summary>
//    /// An encoding method that is fixed in size and calls the native read/write functions of the specified type.
//    /// </summary>
//    /// <remarks>
//    /// This class overrides the default fixed size encoding method for speed improvements.
//    /// </remarks>
//    public class HistorianFixedSizeCombinedEncoding
//        : CombinedEncodingBase<HistorianKey, HistorianValue>
//    {
//        int m_keySize;
//        int m_valueSize;

//        /// <summary>
//        /// Creates a new class
//        /// </summary>
//        public HistorianFixedSizeCombinedEncoding()
//        {
//            m_keySize = 24;
//            m_valueSize = 24;
//        }

//        /// <summary>
//        /// Gets the encoding method that this class implements.
//        /// </summary>
//        public override EncodingDefinition EncodingMethod
//        {
//            get
//            {
//                return CombinedEncodingDefinitionFixedSize.TypeGuid;
//            }
//        }

//        /// <summary>
//        /// Gets if the previous key will need to be presented to the encoding algorithms to
//        /// property encode the next sample. Returning false will cause nulls to be passed
//        /// in a parameters to the encoding.
//        /// </summary>
//        public override bool UsesPreviousKey
//        {
//            get
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Gets if the previous value will need to be presented to the encoding algorithms to
//        /// property encode the next sample. Returning false will cause nulls to be passed
//        /// in a parameters to the encoding.
//        /// </summary>
//        public override bool UsesPreviousValue
//        {
//            get
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Gets the maximum amount of space that is required for the compression algorithm. This
//        /// prevents lower levels from having overflows on the underlying streams. It is critical
//        /// that this value be correct. Error on the side of too large of a value as a value
//        /// too small will corrupt data and be next to impossible to track down the point of corruption
//        /// </summary>
//        public override int MaxCompressionSize
//        {
//            get
//            {
//                return m_keySize + m_valueSize;
//            }
//        }

//        /// <summary>
//        /// Gets if the stream supports a symbol that 
//        /// represents that the end of the stream has been encountered.
//        /// </summary>
//        /// <remarks>
//        /// An example of a symbol would be the byte code 0xFF.
//        /// In this case, if the first byte of the
//        /// word is 0xFF, the encoding has specifically
//        /// designated this as the end of the stream. Therefore, calls to
//        /// Decompress will result in an end of stream exception.
//        /// 
//        /// Failing to reserve a code as the end of stream will mean that
//        /// streaming points will include its own symbol to represent the end of the
//        /// stream, taking 1 extra byte per point encoded.
//        /// </remarks>
//        public override bool ContainsEndOfStreamSymbol
//        {
//            get
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// The byte code to use as the end of stream symbol.
//        /// May throw NotSupportedException if <see cref="CombinedEncodingBase{TKey,TValue}.ContainsEndOfStreamSymbol"/> is false.
//        /// </summary>
//        public override byte EndOfStreamSymbol
//        {
//            get
//            {
//                throw new NotSupportedException();
//            }
//        }

//        /// <summary>
//        /// Encodes <see cref="key"/> and <see cref="value"/> to the provided <see cref="stream"/>.
//        /// </summary>
//        /// <param name="stream">where to write the data</param>
//        /// <param name="prevKey">the previous key if required by <see cref="CombinedEncodingBase{TKey,TValue}.UsesPreviousKey"/>. Otherwise null.</param>
//        /// <param name="prevValue">the previous value if required by <see cref="CombinedEncodingBase{TKey,TValue}.UsesPreviousValue"/>. Otherwise null.</param>
//        /// <param name="key">the key to encode</param>
//        /// <param name="value">the value to encode</param>
//        /// <returns>the number of bytes necessary to encode this key/value.</returns>
//        public override void Encode(BinaryStreamBase stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey key, HistorianValue value)
//        {
//            stream.Write(key.Timestamp);
//            stream.Write(key.PointID);
//            stream.Write(key.EntryNumber);
//            stream.Write(value.Value1);
//            stream.Write(value.Value2);
//            stream.Write(value.Value3);
//        }

//        /// <summary>
//        /// Decodes <see cref="key"/> and <see cref="value"/> from the provided <see cref="stream"/>.
//        /// </summary>
//        /// <param name="stream">where to read the data</param>
//        /// <param name="prevKey">the previous key if required by <see cref="CombinedEncodingBase{TKey,TValue}.UsesPreviousKey"/>. Otherwise null.</param>
//        /// <param name="prevValue">the previous value if required by <see cref="CombinedEncodingBase{TKey,TValue}.UsesPreviousValue"/>. Otherwise null.</param>
//        /// <param name="key">the place to store the decoded key</param>
//        /// <param name="value">the place to store the decoded value</param>
//        /// <param name="isEndOfStream">outputs true if the end of the stream symbol is detected. Not all encoding methods have an end of stream symbol and therefore will always return false.</param>
//        /// <returns>the number of bytes necessary to decode the next key/value.</returns>
//        public override void Decode(BinaryStreamBase stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey key, HistorianValue value, out bool isEndOfStream)
//        {
//            isEndOfStream = false;
//            key.Timestamp = stream.ReadUInt64();
//            key.PointID = stream.ReadUInt64();
//            key.EntryNumber = stream.ReadUInt64();
//            value.Value1 = stream.ReadUInt64();
//            value.Value2 = stream.ReadUInt64();
//            value.Value3 = stream.ReadUInt64();
//        }

//        /// <summary>
//        /// Decodes <see cref="key"/> and <see cref="value"/> from the provided <see cref="stream"/>.
//        /// </summary>
//        /// <param name="stream">where to read the data</param>
//        /// <param name="prevKey">the previous key if required by <see cref="CombinedEncodingBase{TKey,TValue}.UsesPreviousKey"/>. Otherwise null.</param>
//        /// <param name="prevValue">the previous value if required by <see cref="CombinedEncodingBase{TKey,TValue}.UsesPreviousValue"/>. Otherwise null.</param>
//        /// <param name="key">the place to store the decoded key</param>
//        /// <param name="value">the place to store the decoded value</param>
//        /// <param name="isEndOfStream">outputs true if the end of the stream symbol is detected. Not all encoding methods have an end of stream symbol and therefore will always return false.</param>
//        /// <returns>the number of bytes necessary to decode the next key/value.</returns>
//        public override unsafe int Decode(byte* stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey key, HistorianValue value, out bool isEndOfStream)
//        {
//            isEndOfStream = false;
//            key.Timestamp = *(ulong*)stream;
//            key.PointID = *(ulong*)(stream + 8);
//            key.EntryNumber = *(ulong*)(stream + 16);
//            value.Value1 = *(ulong*)(stream + 24);
//            value.Value2 = *(ulong*)(stream + 32);
//            value.Value3 = *(ulong*)(stream + 40);
//            return 48;
//        }

//        /// <summary>
//        /// Encodes <see cref="key"/> and <see cref="value"/> to the provided <see cref="stream"/>.
//        /// </summary>
//        /// <param name="stream">where to write the data</param>
//        /// <param name="prevKey">the previous key if required by <see cref="CombinedEncodingBase{TKey,TValue}.UsesPreviousKey"/>. Otherwise null.</param>
//        /// <param name="prevValue">the previous value if required by <see cref="CombinedEncodingBase{TKey,TValue}.UsesPreviousValue"/>. Otherwise null.</param>
//        /// <param name="key">the key to encode</param>
//        /// <param name="value">the value to encode</param>
//        /// <returns>the number of bytes necessary to encode this key/value.</returns>
//        public override unsafe int Encode(byte* stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey key, HistorianValue value)
//        {
//            *(ulong*)stream = key.Timestamp;
//            *(ulong*)(stream + 8) = key.PointID;
//            *(ulong*)(stream + 16) = key.EntryNumber;
//            *(ulong*)(stream + 24) = value.Value1;
//            *(ulong*)(stream + 32) = value.Value2;
//            *(ulong*)(stream + 40) = value.Value3;
//            return 48;
//        }

//        /// <summary>
//        /// Clones this encoding method.
//        /// </summary>
//        /// <returns>A clone</returns>
//        public override CombinedEncodingBase<HistorianKey, HistorianValue> Clone()
//        {
//            return new HistorianFixedSizeCombinedEncoding();
//        }
//    }
//}
