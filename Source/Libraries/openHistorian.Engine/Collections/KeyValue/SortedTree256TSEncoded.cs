//******************************************************************************************************
//  SortedTree256TSEncoded.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  11/24/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.IO;

namespace openHistorian.Collections.KeyValue
{
    /// <summary>
    /// Represents a collection of 128-bit key/128-bit values pairs that is very similiar to a <see cref="SortedList{int128,int128}"/> 
    /// except it is optimal for storing millions to billions of entries and doing sequential scan of the data.
    /// </summary>
    public class SortedTree256TSEncoded : SortedTree256EncodedLeafNodeBase
    {
        // {88CC6EDA-7F05-449E-9943-97 38 BC FC CF 40}
        static Guid s_fileType = new Guid(0x88cc6eda, 0x7f05, 0x449e, 0x99, 0x43, 0x97, 0x38, 0xbc, 0xfc, 0xcf, 0x40);
        public static Guid GetFileType()
        {
            return s_fileType;
        }
        /// <summary>
        /// Loads an existing <see cref="SortedTree256"/>
        /// from the provided stream.
        /// </summary>
        /// <param name="stream">The stream to load from</param>
        public SortedTree256TSEncoded(BinaryStreamBase stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Creates an empty <see cref="SortedTree256"/> 
        /// and writes the data to the provided stream. 
        /// </summary>
        /// <param name="stream">The stream to use to store the tree.</param>
        /// <param name="blockSize">The size in bytes of a single block.</param>
        public SortedTree256TSEncoded(BinaryStreamBase stream, int blockSize)
            : base(stream, blockSize)
        {
        }

        protected override Guid FileType
        {
            get
            {
                return s_fileType;
            }
        }

        protected override unsafe int EncodeRecord(byte* buffer, ulong key1, ulong key2, ulong value1, ulong value2, ulong prevKey1, ulong prevKey2, ulong prevValue1, ulong prevValue2)
        {

            if (key1 == prevKey1 && key2 == prevKey2 + 1 && value1 == prevValue1)
            {
                //Bit7=0
                //Occurs when key1 and value1 have not changed. And key2 was incremented by 1.
                ulong delta;
                byte code;
                if ((value2 - prevValue2) <= (prevValue2 - value2))
                {
                    //Bit6=0
                    //Occurs when needing to add the delta
                    code = 0;
                    delta = (value2 - prevValue2);
                }
                else
                {
                    //Bit6=1
                    //Occurs when needing to subtract delta
                    code = (1 << 6);
                    delta = (prevValue2 - value2);
                }

                int bitCount = 64 - BitMath.CountLeadingZeros(delta);
                if (bitCount == 0)
                {
                    buffer[0] = 0;
                    return 1;
                }
                if (bitCount <= 6 * 8 + 3)
                {
                    uint extraBytes = (uint)(bitCount - 3 + 7) >> 3; //adding 7 will cause a round up instead of a round down.
                    buffer[0] = (byte)(code | (delta & 7) | (extraBytes << 3));
                    *(ulong*)(buffer + 1) = delta >> 3;
                    return 1 + (int)extraBytes;
                }
                else if (bitCount <= 7 * 8 + 2)
                {
                    buffer[0] = (byte)((14 << 2) | code);
                    *(ulong*)(buffer + 1) = delta >> 2;
                    return 8;
                }
                else
                {
                    buffer[0] = (byte)((15 << 2) | code);
                    *(ulong*)(buffer + 1) = delta;
                    return 9;
                }

            }
            else
            {
                //Bit7=1
                int size = 1;
                buffer[0] = 1 << 7;
                Compression.Write7Bit(buffer, ref size, key1 ^ prevKey1);
                Compression.Write7Bit(buffer, ref size, key2 ^ prevKey2);
                Compression.Write7Bit(buffer, ref size, value1 ^ prevValue1);
                Compression.Write7Bit(buffer, ref size, value2 ^ prevValue2);
                return size;
            }
        }

        protected override void DecodeNextRecord(ref ulong curKey1, ref ulong curKey2, ref ulong curValue1, ref ulong curValue2)
        {
            ulong tmpValue;
            byte code = Stream.ReadByte();
            if (code < 128)
            {
                curKey2++;
                if (code < 64)
                {
                    //Add Delta
                    switch (code >> 3)
                    {
                        case 0:
                            curValue2 = curValue2 + (code & 7u);
                            return;
                        case 1:
                            curValue2 = curValue2 + ((code & 7u) | ((ulong)Stream.ReadByte() << 3));
                            return;
                        case 2:
                            curValue2 = curValue2 + ((code & 7u) | ((ulong)Stream.ReadUInt16() << 3));
                            return;
                        case 3:
                            curValue2 = curValue2 + ((code & 7u) | ((ulong)Stream.ReadUInt24() << 3));
                            return;
                        case 4:
                            curValue2 = curValue2 + ((code & 7u) | ((ulong)Stream.ReadUInt32() << 3));
                            return;
                        case 5:
                            curValue2 = curValue2 + ((code & 7u) | ((ulong)Stream.ReadUInt40() << 3));
                            return;
                        case 6:
                            curValue2 = curValue2 + ((code & 7u) | ((ulong)Stream.ReadUInt48() << 3));
                            return;
                    }
                    if ((code & 4) == 0)
                    {
                        curValue2 = curValue2 + ((code & 3u) | ((ulong)Stream.ReadUInt56() << 2));
                        return;
                    }
                    else
                    {
                        curValue2 = curValue2 + (ulong)Stream.ReadUInt64();
                        return;
                    }
                }
                else
                {
                    code &= 63;
                    //Subtract Delta
                    switch (code >> 3)
                    {
                        case 0:
                            curValue2 = curValue2 - (code & 7u);
                            return;
                        case 1:
                            curValue2 = curValue2 - ((code & 7u) | ((ulong)Stream.ReadByte() << 3));
                            return;
                        case 2:
                            curValue2 = curValue2 - ((code & 7u) | ((ulong)Stream.ReadUInt16() << 3));
                            return;
                        case 3:
                            curValue2 = curValue2 - ((code & 7u) | ((ulong)Stream.ReadUInt24() << 3));
                            return;
                        case 4:
                            curValue2 = curValue2 - ((code & 7u) | ((ulong)Stream.ReadUInt32() << 3));
                            return;
                        case 5:
                            curValue2 = curValue2 - ((code & 7u) | ((ulong)Stream.ReadUInt40() << 3));
                            return;
                        case 6:
                            curValue2 = curValue2 - ((code & 7u) | ((ulong)Stream.ReadUInt48() << 3));
                            return;
                    }
                    if ((code & 4) == 0)
                    {
                        curValue2 = curValue2 - ((code & 3u) | ((ulong)Stream.ReadUInt56() << 2));
                        return;
                    }
                    else
                    {
                        curValue2 = curValue2 - (ulong)Stream.ReadUInt64();
                        return;
                    }
                }
            }
            else
            {
                curKey1 ^= Stream.Read7BitUInt64();
                curKey2 ^= Stream.Read7BitUInt64();
                curValue1 ^= Stream.Read7BitUInt64();
                curValue2 ^= Stream.Read7BitUInt64();
                return;
            }
        }
    }
}
