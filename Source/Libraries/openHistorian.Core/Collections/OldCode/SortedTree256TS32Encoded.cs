////******************************************************************************************************
////  SortedTree256TS32Encoded.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  1/2/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using GSF;
//using GSF.IO;

//namespace openHistorian.Collections
//{
//    /// <summary>
//    /// Represents a collection of 128-bit key/128-bit values pairs that is very similiar to a <see cref="SortedList{int128,int128}"/> 
//    /// except it is optimal for storing millions to billions of entries and doing sequential scan of the data.
//    /// </summary>
//    internal class SortedTree256TS32Encoded : SortedTree256EncodedLeafNodeBase
//    {
//        // {61B742F3-90C7-4737-B96A-24404A17D734}
//        static Guid s_fileType = new Guid(0x61b742f3, 0x90c7, 0x4737, 0xb9, 0x6a, 0x24, 0x40, 0x4a, 0x17, 0xd7, 0x34);

//        public static Guid GetFileType()
//        {
//            return s_fileType;
//        }
//        /// <summary>
//        /// Loads an existing <see cref="SortedTree256"/>
//        /// from the provided stream.
//        /// </summary>
//        /// <param name="stream">The stream to load from</param>
//        public SortedTree256TS32Encoded(BinaryStreamBase stream)
//            : base(stream, stream)
//        {
//        }

//        /// <summary>
//        /// Loads an existing <see cref="SortedTree256"/>
//        /// from the provided stream.
//        /// </summary>
//        /// <param name="stream">The stream to load from</param>
//        public SortedTree256TS32Encoded(BinaryStreamBase stream1, BinaryStreamBase stream2)
//            : base(stream1, stream2)
//        {
//        }

//        /// <summary>
//        /// Creates an empty <see cref="SortedTree256"/> 
//        /// and writes the data to the provided stream. 
//        /// </summary>
//        /// <param name="stream">The stream to use to store the tree.</param>
//        /// <param name="blockSize">The size in bytes of a single block.</param>
//        public SortedTree256TS32Encoded(BinaryStreamBase stream1, BinaryStreamBase stream2, int blockSize)
//            : base(stream1, stream2, blockSize)
//        {
//        }

//        /// <summary>
//        /// Creates an empty <see cref="SortedTree256"/> 
//        /// and writes the data to the provided stream. 
//        /// </summary>
//        /// <param name="stream">The stream to use to store the tree.</param>
//        /// <param name="blockSize">The size in bytes of a single block.</param>
//        public SortedTree256TS32Encoded(BinaryStreamBase stream, int blockSize)
//            : base(stream, stream, blockSize)
//        {
//        }

//        protected override Guid FileType
//        {
//            get
//            {
//                return s_fileType;
//            }
//        }

//        protected override TreeScanner256Base LeafNodeGetScanner()
//        {
//            throw new NotImplementedException();
//        }

//        protected override unsafe int EncodeRecord(byte* buffer, KeyValuePair256 currentKey, KeyValuePair256 previousKey)
//        {
//            throw new NotImplementedException();
//        }

//        protected override void DecodeNextRecord(KeyValuePair256 currentKey)
//        {
//            throw new NotImplementedException();
//        }

//        protected override int MaximumEncodingSize
//        {
//            get
//            {
//                return 50;
//            }
//        }

//        protected unsafe int EncodeRecord(byte* buffer, ulong key1, ulong key2, ulong value1, ulong value2, ulong prevKey1, ulong prevKey2, ulong prevValue1, ulong prevValue2)
//        {
//            int bytePos = 1;
//            byte qualityBit = 0;
//            if (value1 != 0)
//            {
//                qualityBit = 1;
//                Compression.Write7Bit(buffer, ref bytePos, value1);
//            }

//            if (key1 == prevKey1) //Valid type between 1-9
//            {
//                byte keyBits = 0;
//                ulong keyDiff = (key2 - prevKey2);
//                if (keyDiff < 4 && keyDiff > 0)
//                {
//                    keyBits = (byte)(keyDiff << 1);
//                }
//                else
//                {
//                    Compression.Write7Bit(buffer, ref bytePos, keyDiff);
//                }

//                ulong delta;
//                byte addBits = 0;
//                if ((value2 - prevValue2) <= (prevValue2 - value2))
//                {
//                    //Bit6=0
//                    //Occurs when needing to add the delta
//                    addBits = 1 << 3;
//                    delta = (value2 - prevValue2);
//                }
//                else
//                {
//                    //Bit6=1
//                    //Occurs when needing to subtract delta
//                    delta = (prevValue2 - value2);
//                }

//                int bitCount = 64 - BitMath.CountLeadingZeros(delta);
//                if (bitCount == 0)
//                {
//                    buffer[0] = (byte)(qualityBit | keyBits | 192);
//                    return bytePos;
//                }
//                if (bitCount <= 10)
//                {
//                    buffer[0] = (byte)(qualityBit | keyBits | addBits | 0 | (((byte)delta & 3) << 4));
//                    *(ulong*)(buffer + bytePos) = delta >> 2;
//                    return bytePos + 1;
//                }
//                if (bitCount <= 18)
//                {
//                    buffer[0] = (byte)(qualityBit | keyBits | addBits | 64 | (((byte)delta & 3) << 4));
//                    *(ulong*)(buffer + bytePos) = delta >> 2;
//                    return bytePos + 2;
//                }
//                if (bitCount <= 26)
//                {
//                    buffer[0] = (byte)(qualityBit | keyBits | addBits | 128 | (((byte)delta & 3) << 4));
//                    *(ulong*)(buffer + bytePos) = delta >> 2;
//                    return bytePos + 3;
//                }

//                delta = prevValue2 ^ value2;
//                bitCount = 64 - BitMath.CountLeadingZeros(delta);

//                uint extraBytes = (uint)(bitCount + 7) >> 3; //adding 7 will cause a round up instead of a round down.
//                buffer[0] = (byte)(qualityBit | keyBits | 192 | ((extraBytes - 3) << 3));
//                *(ulong*)(buffer + bytePos) = delta;
//                return bytePos + (int)extraBytes;

//            }
//            else //valid types 10-17
//            {
//                Compression.Write7Bit(buffer, ref bytePos, key2);
//                Compression.Write7Bit(buffer, ref bytePos, key1 - prevKey1);

//                int bitCount = 64 - BitMath.CountLeadingZeros(value2 ^ prevValue2);

//                if (bitCount <= 48)
//                {
//                    uint extraBytes = (uint)(bitCount + 7) >> 3; //adding 7 will cause a round up instead of a round down.
//                    buffer[0] = (byte)(qualityBit | 240 | (extraBytes << 1));
//                    *(ulong*)(buffer + bytePos) = value2 ^ prevValue2;
//                    return bytePos + (int)extraBytes;
//                }
//                buffer[0] = (byte)(qualityBit | 254);
//                *(ulong*)(buffer + bytePos) = value2 ^ prevValue2;
//                bytePos += 6;
//                Compression.Write7Bit(buffer, ref bytePos, (value2 ^ prevValue2) >> 48);
//                return bytePos;
//            }


//        }

//        protected void DecodeNextRecord(ref ulong curKey1, ref ulong curKey2, ref ulong curValue1, ref ulong curValue2)
//        {
//            uint code = StreamLeaf.ReadByte();
//            bool qualityNonZero = (code & 1) != 0;

//            //Compute the quality value (curValue1)
//            if (qualityNonZero)
//            {
//                curValue1 = StreamLeaf.Read7BitUInt64();
//            }
//            else
//            {
//                curValue1 = 0;
//            }

//            //Determine if the time has changed           
//            if (code < 0xF0) //The time is the same
//            {
//                uint keyCode = ((code >> 1) & 3);
//                //Compute the key change (curKey2)
//                if (keyCode != 0)
//                {
//                    curKey2 += keyCode;
//                }
//                else
//                {
//                    curKey2 += StreamLeaf.Read7BitUInt64();
//                }

//                if (code < 192) //value has extra bits and is zero 1-3 bytes in size
//                {
//                    bool addValue = (code & 8) != 0;
//                    ulong extraBits = ((code >> 4) & 3);

//                    if (code < 64)
//                    {
//                        extraBits |= (ulong)StreamLeaf.ReadByte() << 2;
//                    }
//                    else if (code < 128)
//                    {
//                        extraBits |= (ulong)StreamLeaf.ReadUInt16() << 2;
//                    }
//                    else
//                    {
//                        extraBits |= (ulong)StreamLeaf.ReadUInt24() << 2;
//                    }

//                    if (addValue)
//                    {
//                        curValue2 += extraBits;
//                    }
//                    else
//                    {
//                        curValue2 -= extraBits;
//                    }
//                }
//                else //value is 0 or 4-8 bytes in size
//                {
//                    switch ((code >> 3) & 7)
//                    {
//                        case 0:
//                            break;
//                        case 1:
//                            curValue2 ^= StreamLeaf.ReadUInt32();
//                            break;
//                        case 2:
//                            curValue2 ^= StreamLeaf.ReadUInt40();
//                            break;
//                        case 3:
//                            curValue2 ^= StreamLeaf.ReadUInt48();
//                            break;
//                        case 4:
//                            curValue2 ^= StreamLeaf.ReadUInt56();
//                            break;
//                        case 5:
//                            curValue2 ^= StreamLeaf.ReadUInt64();
//                            break;
//                    }
//                }
//            }
//            else //The time has changed.
//            {
//                curKey2 = StreamLeaf.Read7BitUInt64();
//                curKey1 = curKey1 + StreamLeaf.Read7BitUInt64();

//                switch ((code >> 1) & 7)
//                {
//                    case 0:
//                        break;
//                    case 1:
//                        curValue2 ^= StreamLeaf.ReadByte();
//                        break;
//                    case 2:
//                        curValue2 ^= StreamLeaf.ReadUInt16();
//                        break;
//                    case 3:
//                        curValue2 ^= StreamLeaf.ReadUInt24();
//                        break;
//                    case 4:
//                        curValue2 ^= StreamLeaf.ReadUInt32();
//                        break;
//                    case 5:
//                        curValue2 ^= StreamLeaf.ReadUInt40();
//                        break;
//                    case 6:
//                        curValue2 ^= StreamLeaf.ReadUInt48();
//                        break;
//                    case 7:
//                        curValue2 ^= StreamLeaf.ReadUInt48();
//                        curValue2 ^= (StreamLeaf.Read7BitUInt64() << 48);
//                        break;
//                }
//            }
//        }
//    }
//}

