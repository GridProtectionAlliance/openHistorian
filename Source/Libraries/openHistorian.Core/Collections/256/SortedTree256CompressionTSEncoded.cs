////******************************************************************************************************
////  SortedTree256CompressionTSEncoded.cs - Gbtc
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
////  3/22/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using GSF;
//using GSF.IO;
//using openHistorian.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Collections
//{
//    public struct SortedTree256CompressionTSEncoded
//        //: ISortedTreeCompressionMethods<KeyValue256>
//    {
//        // {88CC6EDA-7F05-449E-9943-97 38 BC FC CF 40}
//        static Guid s_fileType = new Guid(0x88cc6eda, 0x7f05, 0x449e, 0x99, 0x43, 0x97, 0x38, 0xbc, 0xfc, 0xcf, 0x40);

//        public Guid FileType
//        {
//            get
//            {
//                return s_fileType;
//            }
//        }

//        public bool IsFixedSize
//        {
//            get
//            {
//                return false;

//            }
//        }

//        public unsafe int EncodeRecord(byte* buffer, KeyValue256 currentKey, KeyValue256 previousKey)
//        {

//            if (currentKey.Key1 == previousKey.Key1 && currentKey.Key2 == previousKey.Key2 + 1 && currentKey.Value2 == previousKey.Value1)
//            {
//                //Bit7=0
//                //Occurs when key1 and value1 have not changed. And key2 was incremented by 1.
//                ulong delta;
//                byte code;
//                if ((currentKey.Value2 - previousKey.Value2) <= (previousKey.Value2 - currentKey.Value2))
//                {
//                    //Bit6=0
//                    //Occurs when needing to add the delta
//                    code = 0;
//                    delta = (currentKey.Value2 - previousKey.Value2);
//                }
//                else
//                {
//                    //Bit6=1
//                    //Occurs when needing to subtract delta
//                    code = (1 << 6);
//                    delta = (previousKey.Value2 - currentKey.Value2);
//                }

//                int bitCount = 64 - BitMath.CountLeadingZeros(delta);
//                if (bitCount == 0)
//                {
//                    buffer[0] = 0;
//                    return 1;
//                }
//                if (bitCount <= 6 * 8 + 3)
//                {
//                    uint extraBytes = (uint)(bitCount - 3 + 7) >> 3; //adding 7 will cause a round up instead of a round down.
//                    buffer[0] = (byte)(code | (delta & 7) | (extraBytes << 3));
//                    *(ulong*)(buffer + 1) = delta >> 3;
//                    return 1 + (int)extraBytes;
//                }
//                else if (bitCount <= 7 * 8 + 2)
//                {
//                    buffer[0] = (byte)(code | (delta & 3) | (14 << 2));
//                    *(ulong*)(buffer + 1) = delta >> 2;
//                    return 8;
//                }
//                else
//                {
//                    buffer[0] = (byte)((15 << 2) | code);
//                    *(ulong*)(buffer + 1) = currentKey.Value2;
//                    return 9;
//                }

//            }
//            else
//            {
//                //Bit7=1
//                int size = 1;
//                buffer[0] = 1 << 7;
//                Compression.Write7Bit(buffer, ref size, currentKey.Key1 ^ previousKey.Key1);
//                Compression.Write7Bit(buffer, ref size, currentKey.Key2 ^ previousKey.Key2);
//                Compression.Write7Bit(buffer, ref size, currentKey.Value2 ^ previousKey.Value1);
//                Compression.Write7Bit(buffer, ref size, currentKey.Value2 ^ previousKey.Value2);
//                return size;
//            }
//        }

//        public void DecodeNextRecord(BinaryStreamBase stream, KeyValue256 currentKey)
//        {
//            ulong tmpValue;
//            byte code = stream.ReadByte();
//            if (code < 128)
//            {
//                currentKey.Key2++;
//                if (code < 64)
//                {
//                    //Add Delta
//                    switch (code >> 3)
//                    {
//                        case 0:
//                            currentKey.Value2 = currentKey.Value2 + (code & 7u);
//                            return;
//                        case 1:
//                            currentKey.Value2 = currentKey.Value2 + ((code & 7u) | ((ulong)stream.ReadByte() << 3));
//                            return;
//                        case 2:
//                            currentKey.Value2 = currentKey.Value2 + ((code & 7u) | ((ulong)stream.ReadUInt16() << 3));
//                            return;
//                        case 3:
//                            currentKey.Value2 = currentKey.Value2 + ((code & 7u) | ((ulong)stream.ReadUInt24() << 3));
//                            return;
//                        case 4:
//                            currentKey.Value2 = currentKey.Value2 + ((code & 7u) | ((ulong)stream.ReadUInt32() << 3));
//                            return;
//                        case 5:
//                            currentKey.Value2 = currentKey.Value2 + ((code & 7u) | ((ulong)stream.ReadUInt40() << 3));
//                            return;
//                        case 6:
//                            currentKey.Value2 = currentKey.Value2 + ((code & 7u) | ((ulong)stream.ReadUInt48() << 3));
//                            return;
//                    }
//                    if ((code & 4) == 0)
//                    {
//                        currentKey.Value2 = currentKey.Value2 + ((code & 3u) | ((ulong)stream.ReadUInt56() << 2));
//                        return;
//                    }
//                    else
//                    {
//                        currentKey.Value2 = (ulong)stream.ReadUInt64();
//                        return;
//                    }
//                }
//                else
//                {
//                    code &= 63;
//                    //Subtract Delta
//                    switch (code >> 3)
//                    {
//                        case 0:
//                            currentKey.Value2 = currentKey.Value2 - (code & 7u);
//                            return;
//                        case 1:
//                            currentKey.Value2 = currentKey.Value2 - ((code & 7u) | ((ulong)stream.ReadByte() << 3));
//                            return;
//                        case 2:
//                            currentKey.Value2 = currentKey.Value2 - ((code & 7u) | ((ulong)stream.ReadUInt16() << 3));
//                            return;
//                        case 3:
//                            currentKey.Value2 = currentKey.Value2 - ((code & 7u) | ((ulong)stream.ReadUInt24() << 3));
//                            return;
//                        case 4:
//                            currentKey.Value2 = currentKey.Value2 - ((code & 7u) | ((ulong)stream.ReadUInt32() << 3));
//                            return;
//                        case 5:
//                            currentKey.Value2 = currentKey.Value2 - ((code & 7u) | ((ulong)stream.ReadUInt40() << 3));
//                            return;
//                        case 6:
//                            currentKey.Value2 = currentKey.Value2 - ((code & 7u) | ((ulong)stream.ReadUInt48() << 3));
//                            return;
//                    }
//                    if ((code & 4) == 0)
//                    {
//                        currentKey.Value2 = currentKey.Value2 - ((code & 3u) | ((ulong)stream.ReadUInt56() << 2));
//                        return;
//                    }
//                    else
//                    {
//                        currentKey.Value2 = (ulong)stream.ReadUInt64();
//                        return;
//                    }
//                }
//            }
//            else
//            {
//                currentKey.Key1 ^= stream.Read7BitUInt64();
//                currentKey.Key2 ^= stream.Read7BitUInt64();
//                currentKey.Value1 ^= stream.Read7BitUInt64();
//                currentKey.Value2 ^= stream.Read7BitUInt64();
//                return;
//            }
//        }
//    }
//}

