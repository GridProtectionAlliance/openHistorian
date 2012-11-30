//******************************************************************************************************
//  Compression.cs - Gbtc
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
//  3/16/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.V2
{
    /// <summary>
    /// Contains 7 bit encoding functions
    /// </summary>
    public static class Compression
    {
        public static int Get7BitSize(uint value1)
        {
            if (value1 < 128)
                return 1;
            if (value1 < 128 * 128)
                return 2;
            if (value1 < 128 * 128 * 128)
                return 3;
            if (value1 < 128 * 128 * 128 * 128)
                return 4;
            return 5;
        }

        public static int Get7BitSize(ulong value1)
        {
            if (value1 < 128)
                return 1;
            if (value1 < 128 * 128)
                return 2;
            if (value1 < 128 * 128 * 128)
                return 3;
            if (value1 < 128 * 128 * 128 * 128)
                return 4;
            if (value1 < 128L * 128 * 128 * 128 * 128)
                return 5;
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128)
                return 6;
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
                return 7;
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
                return 8;
            return 9;
        }

        unsafe public static void Write7Bit(byte* stream, ref int position, uint value1)
        {
            if (value1 < 128)
            {
                stream[position] = (byte)value1;
                position += 1;
                return;
            }
            stream[position] = (byte)(value1 | 128);
            if (value1 < 128 * 128)
            {
                stream[position + 1] = (byte)(value1 >> 7);
                position += 2;
                return;
            }
            stream[position + 1] = (byte)((value1 >> 7) | 128);
            if (value1 < 128 * 128 * 128)
            {
                stream[position + 2] = (byte)(value1 >> 14);
                position += 3;
                return;
            }
            stream[position + 2] = (byte)((value1 >> 14) | 128);
            if (value1 < 128 * 128 * 128 * 128)
            {
                stream[position + 3] = (byte)(value1 >> 21);
                position += 4;
                return;
            }
            stream[position + 3] = (byte)((value1 >> 21) | 128);
            stream[position + 4] = (byte)(value1 >> 28);
            position += 5;
            return;
        }

        public static void Write7Bit(byte[] stream, ref int position, uint value1)
        {
            if (value1 < 128)
            {
                stream[position] = (byte)value1;
                position += 1;
                return;
            }
            stream[position] = (byte)(value1 | 128);
            if (value1 < 128 * 128)
            {
                stream[position + 1] = (byte)(value1 >> 7);
                position += 2;
                return;
            }
            stream[position + 1] = (byte)((value1 >> 7) | 128);
            if (value1 < 128 * 128 * 128)
            {
                stream[position + 2] = (byte)(value1 >> 14);
                position += 3;
                return;
            }
            stream[position + 2] = (byte)((value1 >> 14) | 128);
            if (value1 < 128 * 128 * 128 * 128)
            {
                stream[position + 3] = (byte)(value1 >> 21);
                position += 4;
                return;
            }
            stream[position + 3] = (byte)((value1 >> 21) | 128);
            stream[position + 4] = (byte)(value1 >> 28);
            position += 5;
            return;
        }

        public static void Write7Bit(Action<byte> stream, uint value1)
        {
            if (value1 < 128)
            {
                stream((byte)value1);
                return;
            }
            stream((byte)(value1 | 128));
            if (value1 < 128 * 128)
            {
                stream((byte)(value1 >> 7));
                return;
            }
            stream((byte)((value1 >> 7) | 128));
            if (value1 < 128 * 128 * 128)
            {
                stream((byte)(value1 >> 14));
                return;
            }
            stream((byte)((value1 >> 14) | 128));
            if (value1 < 128 * 128 * 128 * 128)
            {
                stream((byte)(value1 >> 21));
                return;
            }
            stream((byte)((value1 >> 21) | 128));
            stream((byte)(value1 >> 28));
        }

        public static void Read7BitUInt32(byte[] stream, ref int position, out uint value1)
        {
            int pos = position;
            uint value11;
            value11 = stream[pos];
            if (value11 < 128)
            {
                position = pos + 1;
                value1 = value11;
                return;
            }
            value11 ^= ((uint)stream[pos + 1] << 7);
            if (value11 < 128 * 128)
            {
                position = pos + 2;
                value1 = value11 ^ 0x80;
                return;
            }
            value11 ^= ((uint)stream[pos + 2] << 14);
            if (value11 < 128 * 128 * 128)
            {
                position = pos + 3;
                value1 = value11 ^ 0x4080;
                return;
            }
            value11 ^= ((uint)stream[pos + 3] << 21);
            if (value11 < 128 * 128 * 128 * 128)
            {
                position = pos + 4;
                value1 = value11 ^ 0x204080;
                return;
            }
            value11 ^= ((uint)stream[pos + 4] << 28) ^ 0x10204080;
            position = pos + 5;
            value1 = value11;
            return;
        }

        public static uint Read7BitUInt32(Func<byte> stream)
        {
            uint value11;
            value11 = stream();
            if (value11 < 128)
            {
                return value11;
            }
            value11 ^= ((uint)stream() << 7);
            if (value11 < 128 * 128)
            {
                return value11 ^ 0x80;
            }
            value11 ^= ((uint)stream() << 14);
            if (value11 < 128 * 128 * 128)
            {
                return value11 ^ 0x4080;
            }
            value11 ^= ((uint)stream() << 21);
            if (value11 < 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x204080;
            }
            value11 ^= ((uint)stream() << 28) ^ 0x10204080;
            return value11;
        }

        public static ulong Read7BitUInt64(Func<byte> stream)
        {
            ulong value11;
            value11 = stream();
            if (value11 < 128)
            {
                return value11;
            }
            value11 ^= ((ulong)stream() << (7));
            if (value11 < 128 * 128)
            {
                return value11 ^ 0x80;
            }
            value11 ^= ((ulong)stream() << (7 + 7));
            if (value11 < 128 * 128 * 128)
            {
                return value11 ^ 0x4080;
            }
            value11 ^= ((ulong)stream() << (7 + 7 + 7));
            if (value11 < 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x204080;
            }
            value11 ^= ((ulong)stream() << (7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x10204080L;
            }
            value11 ^= ((ulong)stream() << (7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x810204080L;
            }
            value11 ^= ((ulong)stream() << (7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x40810204080L;
            }
            value11 ^= ((ulong)stream() << (7 + 7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x2040810204080L;
            }
            value11 ^= ((ulong)stream() << (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
            return value11 ^ 0x102040810204080L;
        }


        public static void Read7BitUInt64(byte[] stream, ref int position, out ulong value1)
        {
            int pos = position;
            ulong value11;
            value11 = stream[pos];
            if (value11 < 128)
            {
                position += 1;
                value1 = value11;
                return;
            }
            value11 ^= ((ulong)stream[pos + 1] << (7));
            if (value11 < 128 * 128)
            {
                position += 2;
                value1 = value11 ^ 0x80;
                return;
            }
            value11 ^= ((ulong)stream[pos + 2] << (7 + 7));
            if (value11 < 128 * 128 * 128)
            {
                position += 3;
                value1 = value11 ^ 0x4080;
                return;
            }
            value11 ^= ((ulong)stream[pos + 3] << (7 + 7 + 7));
            if (value11 < 128 * 128 * 128 * 128)
            {
                position += 4;
                value1 = value11 ^ 0x204080;
                return;
            }
            value11 ^= ((ulong)stream[pos + 4] << (7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128)
            {
                position += 5;
                value1 = value11 ^ 0x10204080L;
                return;
            }
            value11 ^= ((ulong)stream[pos + 5] << (7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128)
            {
                position += 6;
                value1 = value11 ^ 0x810204080L;
                return;
            }
            value11 ^= ((ulong)stream[pos + 6] << (7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
            {
                position += 7;
                value1 = value11 ^ 0x40810204080L;
                return;
            }
            value11 ^= ((ulong)stream[pos + 7] << (7 + 7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
            {
                position += 8;
                value1 = value11 ^ 0x2040810204080L;
                return;
            }
            value11 ^= ((ulong)stream[pos + 8] << (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
            position += 9;
            value1 = value11 ^ 0x102040810204080L;
            return;
        }

        unsafe public static void Write7Bit(byte* stream, ref int position, ulong value1)
        {
            if (value1 < 128)
            {
                stream[position] = (byte)value1;
                position += 1;
                return;
            }
            stream[position] = (byte)(value1 | 128);
            if (value1 < 128 * 128)
            {
                stream[position + 1] = (byte)(value1 >> 7);
                position += 2;
                return;
            }
            stream[position + 1] = (byte)((value1 >> 7) | 128);
            if (value1 < 128 * 128 * 128)
            {
                stream[position + 2] = (byte)(value1 >> (7 + 7));
                position += 3;
                return;
            }
            stream[position + 2] = (byte)((value1 >> (7 + 7)) | 128);
            if (value1 < 128 * 128 * 128 * 128)
            {
                stream[position + 3] = (byte)(value1 >> (7 + 7 + 7));
                position += 4;
                return;
            }
            stream[position + 3] = (byte)((value1 >> (7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128)
            {
                stream[position + 4] = (byte)(value1 >> (7 + 7 + 7 + 7));
                position += 5;
                return;
            }
            stream[position + 4] = (byte)((value1 >> (7 + 7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128)
            {
                stream[position + 5] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7));
                position += 6;
                return;
            }
            stream[position + 5] = (byte)((value1 >> (7 + 7 + 7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
            {
                stream[position + 6] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7));
                position += 7;
                return;
            }
            stream[position + 6] = (byte)((value1 >> (7 + 7 + 7 + 7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
            {
                stream[position + 7] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7));
                position += 8;
                return;
            }
            stream[position + 7] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7) | 128);
            stream[position + 8] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
            position += 9;
            return;
        }

        public static void Write7Bit(byte[] stream, ref int position, ulong value1)
        {
            if (value1 < 128)
            {
                stream[position] = (byte)value1;
                position += 1;
                return;
            }
            stream[position] = (byte)(value1 | 128);
            if (value1 < 128 * 128)
            {
                stream[position + 1] = (byte)(value1 >> 7);
                position += 2;
                return;
            }
            stream[position + 1] = (byte)((value1 >> 7) | 128);
            if (value1 < 128 * 128 * 128)
            {
                stream[position + 2] = (byte)(value1 >> (7 + 7));
                position += 3;
                return;
            }
            stream[position + 2] = (byte)((value1 >> (7 + 7)) | 128);
            if (value1 < 128 * 128 * 128 * 128)
            {
                stream[position + 3] = (byte)(value1 >> (7 + 7 + 7));
                position += 4;
                return;
            }
            stream[position + 3] = (byte)((value1 >> (7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128)
            {
                stream[position + 4] = (byte)(value1 >> (7 + 7 + 7 + 7));
                position += 5;
                return;
            }
            stream[position + 4] = (byte)((value1 >> (7 + 7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128)
            {
                stream[position + 5] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7));
                position += 6;
                return;
            }
            stream[position + 5] = (byte)((value1 >> (7 + 7 + 7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
            {
                stream[position + 6] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7));
                position += 7;
                return;
            }
            stream[position + 6] = (byte)((value1 >> (7 + 7 + 7 + 7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
            {
                stream[position + 7] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7));
                position += 8;
                return;
            }
            stream[position + 7] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7) | 128);
            stream[position + 8] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
            position += 9;
            return;
        }
        public static void Write7Bit(Action<byte> stream, ulong value1)
        {
            if (value1 < 128)
            {
                stream((byte)value1);
                return;
            }
            stream((byte)(value1 | 128));
            if (value1 < 128 * 128)
            {
                stream((byte)(value1 >> 7));
                return;
            }
            stream((byte)((value1 >> 7) | 128));
            if (value1 < 128 * 128 * 128)
            {
                stream((byte)(value1 >> (7 + 7)));
                return;
            }
            stream((byte)((value1 >> (7 + 7)) | 128));
            if (value1 < 128 * 128 * 128 * 128)
            {
                stream((byte)(value1 >> (7 + 7 + 7)));
                return;
            }
            stream((byte)((value1 >> (7 + 7 + 7)) | 128));
            if (value1 < 128L * 128 * 128 * 128 * 128)
            {
                stream((byte)(value1 >> (7 + 7 + 7 + 7)));
                return;
            }
            stream((byte)((value1 >> (7 + 7 + 7 + 7)) | 128));
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128)
            {
                stream((byte)(value1 >> (7 + 7 + 7 + 7 + 7)));
                return;
            }
            stream((byte)((value1 >> (7 + 7 + 7 + 7 + 7)) | 128));
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
            {
                stream((byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7)));
                return;
            }
            stream((byte)((value1 >> (7 + 7 + 7 + 7 + 7 + 7)) | 128));
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
            {
                stream((byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7)));
                return;
            }
            stream((byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7) | 128));
            stream((byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7)));
            return;
        }
        //public unsafe static void Write(byte[] stream1, ref int position, uint value1, uint value2, uint value3, uint value4)
        //{
        //    fixed (byte* stream = stream1)
        //    {
        //        int pos = position + 1;
        //        int prefix = 0;
        //        if (value1 <= 0xFF)
        //        {
        //            stream[pos] = (byte)value1;
        //            pos += 1;
        //        }
        //        else if (value1 <= 0xFFFF)
        //        {
        //            *(ushort*)(stream + pos) = (ushort)value1;
        //            pos += 2;
        //            prefix = 1 << 6;
        //        }
        //        else if (value1 <= 0xFFFFFF)
        //        {
        //            *(uint*)(stream + pos) = value1;
        //            pos += 3;
        //            prefix = 2 << 6;
        //        }
        //        else
        //        {
        //            *(uint*)(stream + pos) = value1;
        //            pos += 4;
        //            prefix = 3 << 6;
        //        }

        //        if (value2 <= 0xFF)
        //        {
        //            stream[pos] = (byte)value2;
        //            pos += 1;
        //        }
        //        else if (value2 <= 0xFFFF)
        //        {
        //            *(ushort*)(stream + pos) = (ushort)value2;
        //            pos += 2;
        //            prefix |= 1 << 4;
        //        }
        //        else if (value2 <= 0xFFFFFF)
        //        {
        //            *(uint*)(stream + pos) = value2;
        //            pos += 3;
        //            prefix |= 2 << 4;
        //        }
        //        else
        //        {
        //            *(uint*)(stream + pos) = value2;
        //            pos += 4;
        //            prefix |= 3 << 4;
        //        }

        //        if (value3 <= 0xFF)
        //        {
        //            stream[pos] = (byte)value3;
        //            pos += 1;
        //        }
        //        else if (value3 <= 0xFFFF)
        //        {
        //            *(ushort*)(stream + pos) = (ushort)value3;
        //            pos += 2;
        //            prefix |= 1 << 2;
        //        }
        //        else if (value3 <= 0xFFFFFF)
        //        {
        //            *(uint*)(stream + pos) = value3;
        //            pos += 3;
        //            prefix |= 2 << 2;
        //        }
        //        else
        //        {
        //            *(uint*)(stream + pos) = value3;
        //            pos += 4;
        //            prefix |= 3 << 2;
        //        }

        //        if (value4 <= 0xFF)
        //        {
        //            stream[pos] = (byte)value4;
        //            pos += 1;
        //        }
        //        else if (value4 <= 0xFFFF)
        //        {
        //            *(ushort*)(stream + pos) = (ushort)value4;
        //            pos += 2;
        //            prefix |= 1;
        //        }
        //        else if (value4 <= 0xFFFFFF)
        //        {
        //            *(uint*)(stream + pos) = value4;
        //            pos += 3;
        //            prefix |= 2;
        //        }
        //        else
        //        {
        //            *(uint*)(stream + pos) = value4;
        //            pos += 4;
        //            prefix |= 3;
        //        }

        //        stream[position] = (byte)prefix;
        //        position = pos;
        //    }
        //}

        //public unsafe static void Write(byte[] stream1, ref int position, uint value1, uint value2, uint value3, uint value4)
        //{
        //    fixed (byte* stream = stream1)
        //    {
        //        int pos = position + 1;
        //        int prefix = 0;
        //        if (value1 <= 0xFF)
        //        {
        //            stream[pos] = (byte)value1;
        //            pos += 1;
        //        }
        //        else if (value1 <= 0xFFFF)
        //        {
        //            *(ushort*)(stream + pos) = (ushort)value1;
        //            pos += 2;
        //            prefix = 1 << 6;
        //        }
        //        else if (value1 <= 0xFFFFFF)
        //        {
        //            *(uint*)(stream + pos) = value1;
        //            pos += 3;
        //            prefix = 2 << 6;
        //        }
        //        else
        //        {
        //            *(uint*)(stream + pos) = value1;
        //            pos += 4;
        //            prefix = 3 << 6;
        //        }

        //        if (value2 <= 0xFF)
        //        {
        //            stream[pos] = (byte)value2;
        //            pos += 1;
        //        }
        //        else if (value2 <= 0xFFFF)
        //        {
        //            *(ushort*)(stream + pos) = (ushort)value2;
        //            pos += 2;
        //            prefix |= 1 << 4;
        //        }
        //        else if (value2 <= 0xFFFFFF)
        //        {
        //            *(uint*)(stream + pos) = value2;
        //            pos += 3;
        //            prefix |= 2 << 4;
        //        }
        //        else
        //        {
        //            *(uint*)(stream + pos) = value2;
        //            pos += 4;
        //            prefix |= 3 << 4;
        //        }

        //        if (value3 <= 0xFF)
        //        {
        //            stream[pos] = (byte)value3;
        //            pos += 1;
        //        }
        //        else if (value3 <= 0xFFFF)
        //        {
        //            *(ushort*)(stream + pos) = (ushort)value3;
        //            pos += 2;
        //            prefix |= 1 << 2;
        //        }
        //        else if (value3 <= 0xFFFFFF)
        //        {
        //            *(uint*)(stream + pos) = value3;
        //            pos += 3;
        //            prefix |= 2 << 2;
        //        }
        //        else
        //        {
        //            *(uint*)(stream + pos) = value3;
        //            pos += 4;
        //            prefix |= 3 << 2;
        //        }

        //        if (value4 <= 0xFF)
        //        {
        //            stream[pos] = (byte)value4;
        //            pos += 1;
        //        }
        //        else if (value4 <= 0xFFFF)
        //        {
        //            *(ushort*)(stream + pos) = (ushort)value4;
        //            pos += 2;
        //            prefix |= 1;
        //        }
        //        else if (value4 <= 0xFFFFFF)
        //        {
        //            *(uint*)(stream + pos) = value4;
        //            pos += 3;
        //            prefix |= 2;
        //        }
        //        else
        //        {
        //            *(uint*)(stream + pos) = value4;
        //            pos += 4;
        //            prefix |= 3;
        //        }

        //        stream[position] = (byte)prefix;
        //        position = pos;
        //    }

        //}


        //public unsafe static void Write(byte[] stream, ref int position, uint value1, uint value2, uint value3, uint value4)
        //{
        //    int pos = position+1;
        //    int prefix = 0;
        //    if (value1 <= 0xFF)
        //    {
        //        stream[pos] = (byte)value1;
        //        pos += 1;
        //    }
        //    else if (value1 <= 0xFFFF)
        //    {
        //        stream[pos] = (byte)value1;
        //        stream[pos+1] = (byte)(value1>>8);
        //        pos += 2;
        //        prefix = 1 << 6;
        //    }
        //    else if (value1 <= 0xFFFFFF)
        //    {
        //        stream[pos] = (byte)value1;
        //        stream[pos + 1] = (byte)(value1 >> 8);
        //        stream[pos + 1] = (byte)(value1 >> 16);
        //        pos += 3;
        //        prefix = 2 << 6;
        //    }
        //    else
        //    {
        //        stream[pos] = (byte)value1;
        //        stream[pos + 1] = (byte)(value1 >> 8);
        //        stream[pos + 1] = (byte)(value1 >> 16);
        //        stream[pos + 1] = (byte)(value1 >> 24);
        //        pos += 4;
        //        prefix = 3 << 6;
        //    }

        //    if (value2 <= 0xFF)
        //    {
        //        stream[pos] = (byte)value2;
        //        pos += 1;
        //    }
        //    else if (value2 <= 0xFFFF)
        //    {
        //        stream[pos] = (byte)value2;
        //        stream[pos + 1] = (byte)(value2 >> 8);
        //        pos += 2;
        //        prefix |= 1 << 4;
        //    }
        //    else if (value2 <= 0xFFFFFF)
        //    {
        //        stream[pos] = (byte)value2;
        //        stream[pos + 1] = (byte)(value2 >> 8);
        //        stream[pos + 1] = (byte)(value2 >> 16);
        //        pos += 3;
        //        prefix |= 2 << 4;
        //    }
        //    else
        //    {
        //        stream[pos] = (byte)value2;
        //        stream[pos + 1] = (byte)(value2 >> 8);
        //        stream[pos + 1] = (byte)(value2 >> 16);
        //        stream[pos + 1] = (byte)(value2 >> 24);
        //        pos += 4;
        //        prefix |= 3 << 4;
        //    }

        //    if (value3 <= 0xFF)
        //    {
        //        stream[pos] = (byte)value3;
        //        pos += 1;
        //    }
        //    else if (value3 <= 0xFFFF)
        //    {
        //        stream[pos] = (byte)value3;
        //        stream[pos + 1] = (byte)(value3 >> 8);
        //        pos += 2;
        //        prefix |= 1 << 2;
        //    }
        //    else if (value3 <= 0xFFFFFF)
        //    {
        //        stream[pos] = (byte)value3;
        //        stream[pos + 1] = (byte)(value3 >> 8);
        //        stream[pos + 1] = (byte)(value3 >> 16);
        //        pos += 3;
        //        prefix |= 2 << 2;
        //    }
        //    else
        //    {
        //        stream[pos] = (byte)value3;
        //        stream[pos + 1] = (byte)(value3 >> 8);
        //        stream[pos + 1] = (byte)(value3 >> 16);
        //        stream[pos + 1] = (byte)(value3 >> 24);
        //        pos += 4;
        //        prefix |= 3 << 2;
        //    }

        //    if (value4 <= 0xFF)
        //    {
        //        stream[pos] = (byte)value4;
        //        pos += 1;
        //    }
        //    else if (value4 <= 0xFFFF)
        //    {
        //        stream[pos] = (byte)value4;
        //        stream[pos + 1] = (byte)(value4 >> 8);
        //        pos += 2;
        //        prefix |= 1;
        //    }
        //    else if (value4 <= 0xFFFFFF)
        //    {
        //        stream[pos] = (byte)value4;
        //        stream[pos + 1] = (byte)(value4 >> 8);
        //        stream[pos + 1] = (byte)(value4 >> 16);
        //        pos += 3;
        //        prefix |= 2;
        //    }
        //    else
        //    {
        //        stream[pos] = (byte)value4;
        //        stream[pos + 1] = (byte)(value4 >> 8);
        //        stream[pos + 1] = (byte)(value4 >> 16);
        //        stream[pos + 1] = (byte)(value4 >> 24);
        //        pos += 4;
        //        prefix |= 3;
        //    }

        //    stream[position] = (byte)prefix;
        //    position = pos;

        //}

        static byte GetLength(uint value)
        {
            if (value <= 0xFF)
                return 0;
            if (value <= 0xFFFF)
                return 1;
            if (value <= 0xFFFFFF)
                return 2;
            return 3;
        }
    }
}
