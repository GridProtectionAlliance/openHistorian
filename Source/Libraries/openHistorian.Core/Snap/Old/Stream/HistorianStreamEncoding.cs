////******************************************************************************************************
////  HistorianCompressedStream.cs - Gbtc
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
////  08/10/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using GSF;
//using GSF.IO;
//using GSF.Snap;
//using GSF.Snap.Encoding;
//using openHistorian.Snap.Definitions;

//namespace openHistorian.Snap.Stream
//{
//    /// <summary>
//    /// A stream based compression method that supports <see cref="HistorianKey"/>, <see cref="HistorianValue"/>
//    /// </summary>
//    public class HistorianStreamEncoding
//        : StreamEncodingBase<HistorianKey, HistorianValue>
//    {
//        ulong m_prevTimestamp;
//        ulong m_prevPointID;

//        /// <summary>
//        /// Gets if Encoding using byte arrays is supported.
//        /// </summary>
//        public override bool SupportsPointerSerialization
//        {
//            get
//            {
//                return true;
//            }
//        }

//        /// <summary>
//        /// Gets the maximum number of bytes needed to encode a single point.
//        /// </summary>
//        public override int MaxCompressedSize
//        {
//            get
//            {
//                return 55; //3 extra bytes just to be safe.
//            }
//        }

//        /// <summary>
//        /// Gets the definition of the encoding used.
//        /// </summary>
//        public override EncodingDefinition EncodingMethod
//        {
//            get
//            {
//                return HistorianStreamEncodingDefinition.TypeGuid;
//            }
//        }

//        /// <summary>
//        /// Writes the end of the stream symbol to the <see cref="stream"/>.
//        /// </summary>
//        /// <param name="stream">the stream to write to</param>
//        public override void WriteEndOfStream(BinaryStreamBase stream)
//        {
//            stream.Write((byte)255);
//        }

//        /// <summary>
//        /// Encodes the current key/value to the stream.
//        /// </summary>
//        /// <param name="stream">the stream to write to</param>
//        /// <param name="currentKey">the key to write</param>
//        /// <param name="currentValue">the value to write</param>
//        public override void Encode(BinaryStreamBase stream, HistorianKey currentKey, HistorianValue currentValue)
//        {
//            if (currentKey.Timestamp == m_prevTimestamp
//                && ((currentKey.PointID ^ m_prevPointID) < 64)
//                && currentKey.EntryNumber == 0
//                && currentValue.Value1 <= uint.MaxValue //must be a 32-bit value
//                && currentValue.Value2 == 0
//                && currentValue.Value3 == 0)
//            {
//                if (currentValue.Value1 == 0)
//                {
//                    stream.Write((byte)((currentKey.PointID ^ m_prevPointID)));
//                }
//                else
//                {
//                    stream.Write((byte)((currentKey.PointID ^ m_prevPointID) | 64));
//                    stream.Write((uint)currentValue.Value1);
//                }
//                m_prevTimestamp = currentKey.Timestamp;
//                m_prevPointID = currentKey.PointID;
//                return;
//            }

//            byte code = 128;

//            if (currentKey.Timestamp != m_prevTimestamp)
//                code |= 64;

//            if (currentKey.EntryNumber != 0)
//                code |= 32;

//            if (currentValue.Value1 > uint.MaxValue)
//                code |= 16;
//            else if (currentValue.Value1 > 0)
//                code |= 8;

//            if (currentValue.Value2 != 0)
//                code |= 4;

//            if (currentValue.Value3 > uint.MaxValue)
//                code |= 2;
//            else if (currentValue.Value3 > 0)
//                code |= 1;

//            stream.Write(code);

//            if (currentKey.Timestamp != m_prevTimestamp)
//                stream.Write7Bit(currentKey.Timestamp ^ m_prevTimestamp);

//            stream.Write7Bit(currentKey.PointID ^ m_prevPointID);

//            if (currentKey.EntryNumber != 0)
//                stream.Write7Bit(currentKey.EntryNumber);

//            if (currentValue.Value1 > uint.MaxValue)
//                stream.Write(currentValue.Value1);
//            else if (currentValue.Value1 > 0)
//                stream.Write((uint)currentValue.Value1);

//            if (currentValue.Value2 != 0)
//                stream.Write(currentValue.Value2);

//            if (currentValue.Value3 > uint.MaxValue)
//                stream.Write(currentValue.Value3);
//            else if (currentValue.Value3 > 0)
//                stream.Write((uint)currentValue.Value3);

//            m_prevTimestamp = currentKey.Timestamp;
//            m_prevPointID = currentKey.PointID;
//        }

//        /// <summary>
//        /// Encodes the current key/value to the stream.
//        /// </summary>
//        /// <param name="stream">the stream to write to</param>
//        /// <param name="currentKey">the key to write</param>
//        /// <param name="currentValue">the value to write</param>
//        /// <returns>the number of bytes advanced in the stream</returns>
//        public override unsafe int Encode(byte* stream, HistorianKey currentKey, HistorianValue currentValue)
//        {
//            int size;
//            if (currentKey.Timestamp == m_prevTimestamp
//             && ((currentKey.PointID ^ m_prevPointID) < 64)
//             && currentKey.EntryNumber == 0
//             && currentValue.Value1 <= uint.MaxValue //must be a 32-bit value
//             && currentValue.Value2 == 0
//             && currentValue.Value3 == 0)
//            {
//                if (currentValue.Value1 == 0)
//                {
//                    stream[0] = (byte)(currentKey.PointID ^ m_prevPointID);
//                    size = 1;
//                }
//                else
//                {
//                    stream[0] = ((byte)((currentKey.PointID ^ m_prevPointID) | 64));
//                    *(uint*)(stream + 1) = (uint)currentValue.Value1;
//                    size = 5;
//                }
//                m_prevTimestamp = currentKey.Timestamp;
//                m_prevPointID = currentKey.PointID;
//                return size;
//            }

//            byte code = 128;

//            if (currentKey.Timestamp != m_prevTimestamp)
//                code |= 64;

//            if (currentKey.EntryNumber != 0)
//                code |= 32;

//            if (currentValue.Value1 > uint.MaxValue)
//                code |= 16;
//            else if (currentValue.Value1 > 0)
//                code |= 8;

//            if (currentValue.Value2 != 0)
//                code |= 4;

//            if (currentValue.Value3 > uint.MaxValue)
//                code |= 2;
//            else if (currentValue.Value3 > 0)
//                code |= 1;

//            stream[0] = code;
//            size = 1;

//            if (currentKey.Timestamp != m_prevTimestamp)
//                Encoding7Bit.Write(stream, ref size, currentKey.Timestamp ^ m_prevTimestamp);

//            Encoding7Bit.Write(stream, ref size, currentKey.PointID ^ m_prevPointID);

//            if (currentKey.EntryNumber != 0)
//                Encoding7Bit.Write(stream, ref size, currentKey.EntryNumber);

//            if (currentValue.Value1 > uint.MaxValue)
//            {
//                *(ulong*)(stream + size) = currentValue.Value1;
//                size += 8;
//            }
//            else if (currentValue.Value1 > 0)
//            {
//                *(uint*)(stream + size) = (uint)currentValue.Value1;
//                size += 4;
//            }

//            if (currentValue.Value2 != 0)
//            {
//                *(ulong*)(stream + size) = currentValue.Value2;
//                size += 8;
//            }

//            if (currentValue.Value3 > uint.MaxValue)
//            {
//                *(ulong*)(stream + size) = currentValue.Value3;
//                size += 8;
//            }
//            else if (currentValue.Value3 > 0)
//            {
//                *(uint*)(stream + size) = (uint)currentValue.Value3;
//                size += 4;
//            }

//            m_prevTimestamp = currentKey.Timestamp;
//            m_prevPointID = currentKey.PointID;
//            return size;
//        }

//        /// <summary>
//        /// Attempts to read the next point from the stream. 
//        /// </summary>
//        /// <param name="stream">The stream to read from</param>
//        /// <param name="key">the key to store the value to</param>
//        /// <param name="value">the value to store to</param>
//        /// <returns>True if successful. False if end of the stream has been reached.</returns>
//        public override bool TryDecode(BinaryStreamBase stream, HistorianKey key, HistorianValue value)
//        {
//            byte code = stream.ReadUInt8();
//            if (code == 255)
//                return false;

//            if (code < 128)
//            {
//                if (code < 64)
//                {
//                    key.Timestamp = m_prevTimestamp;
//                    key.PointID = m_prevPointID ^ code;
//                    key.EntryNumber = 0;
//                    value.Value1 = 0;
//                    value.Value2 = 0;
//                    value.Value3 = 0;
//                }
//                else
//                {
//                    key.Timestamp = m_prevTimestamp;
//                    key.PointID = m_prevPointID ^ code ^ 64;
//                    key.EntryNumber = 0;
//                    value.Value1 = stream.ReadUInt32();
//                    value.Value2 = 0;
//                    value.Value3 = 0;
//                }
//                m_prevTimestamp = key.Timestamp;
//                m_prevPointID = key.PointID;
//                return true;
//            }

//            if ((code & 64) != 0) //T is set
//                key.Timestamp = m_prevTimestamp ^ stream.Read7BitUInt64();
//            else
//                key.Timestamp = m_prevTimestamp;

//            key.PointID = m_prevPointID ^ stream.Read7BitUInt64();

//            if ((code & 32) != 0) //E is set)
//                key.EntryNumber = stream.Read7BitUInt64();
//            else
//                key.EntryNumber = 0;

//            if ((code & 16) != 0) //V1 High is set)
//                value.Value1 = stream.ReadUInt64();
//            else if ((code & 8) != 0) //V1 low is set)
//                value.Value1 = stream.ReadUInt32();
//            else
//                value.Value1 = 0;

//            if ((code & 4) != 0) //V2 is set)
//                value.Value2 = stream.ReadUInt64();
//            else
//                value.Value2 = 0;

//            if ((code & 2) != 0) //V1 High is set)
//                value.Value3 = stream.ReadUInt64();
//            else if ((code & 1) != 0) //V1 low is set)
//                value.Value3 = stream.ReadUInt32();
//            else
//                value.Value3 = 0;
//            m_prevTimestamp = key.Timestamp;
//            m_prevPointID = key.PointID;

//            return true;
//        }

//        /// <summary>
//        /// Resets the encoder. Some encoders maintain streaming state data that should
//        /// be reset when reading from a new stream.
//        /// </summary>
//        public override void ResetEncoder()
//        {
//            m_prevTimestamp = 0;
//            m_prevPointID = 0;
//        }
//    }
//}
