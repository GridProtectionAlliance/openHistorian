//******************************************************************************************************
//  Encoding7Bit.cs - Gbtc
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
//  3/16/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.CompilerServices;
using GSF.IO;

namespace GSF
{
    /// <summary>
    /// Contains 7 bit encoding functions
    /// </summary>
    public static class Encoding7Bit
    {
        #region [ 32 bit ]

        /// <summary>
        /// Gets the number of bytes required to write the provided value.
        /// </summary>
        /// <param name="value1">the value to measure</param>
        /// <returns></returns>
        public static int GetSize(uint value1)
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

        /// <summary>
        /// Gets the number of bytes for the supplied value in the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public unsafe static int MeasureUInt32(byte* stream)
        {
            if (stream[0] < 128)
                return 1;
            if (stream[1] < 128)
                return 2;
            if (stream[2] < 128)
                return 3;
            if (stream[3] < 128)
                return 4;
            return 5;
        }

        /// <summary>
        /// Gets the number of bytes for the supplied value in the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public unsafe static int MeasureUInt32(byte* stream, int position)
        {
            return MeasureUInt32(stream + position);
        }

        /// <summary>
        /// Gets the number of bytes for the supplied value in the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static int MeasureUInt32(byte[] stream, int position)
        {
            if (stream[position + 0] < 128)
                return 1;
            if (stream[position + 1] < 128)
                return 2;
            if (stream[position + 2] < 128)
                return 3;
            if (stream[position + 3] < 128)
                return 4;
            return 5;
        }

        #region [ Write ]

        /// <summary>
        /// Writes the 7-bit encoded value to the provided stream.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="value1">the value to write</param>
        /// <returns>The number of bytes required to store the value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int Write(byte* stream, uint value1)
        {
            if (value1 < 128)
            {
                stream[0] = (byte)value1;
                return 1;
            }
            stream[0] = (byte)(value1 | 128);
            if (value1 < 128 * 128)
            {
                stream[1] = (byte)(value1 >> 7);
                return 2;
            }
            stream[1] = (byte)((value1 >> 7) | 128);
            if (value1 < 128 * 128 * 128)
            {
                stream[2] = (byte)(value1 >> 14);
                return 3;
            }
            stream[2] = (byte)((value1 >> 14) | 128);
            if (value1 < 128 * 128 * 128 * 128)
            {
                stream[3] = (byte)(value1 >> 21);
                return 4;
            }
            stream[3] = (byte)((value1 >> 21) | 128);
            stream[4] = (byte)(value1 >> 28);
            return 5;
        }

        /// <summary>
        /// Writes the 7-bit encoded value to the provided stream.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="position">a reference parameter to the starting position. 
        /// This field will be updated. when the function returns</param>
        /// <param name="value1">the value to write</param>
        public static unsafe void Write(byte* stream, ref int position, uint value1)
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

        /// <summary>
        /// Writes the 7-bit encoded value to the provided stream.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="position">a reference parameter to the starting position. 
        /// This field will be updated. when the function returns</param>
        /// <param name="value1">the value to write</param>
        public static void Write(byte[] stream, ref int position, uint value1)
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

        /// <summary>
        /// Writes the 7-bit encoded value to the provided stream.
        /// </summary>
        /// <param name="stream">a delegate to a write byte method</param>
        /// <param name="value1">the value to write</param>
        public static void Write(Action<byte> stream, uint value1)
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

        #endregion

        #region [ Read ]

        /// <summary>
        /// Reads a 7-bit encoded uint.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="position">the position in the stream. Position will be updated after reading</param>
        /// <returns>The value</returns>
        public unsafe static uint ReadUInt32(byte* stream, ref int position)
        {
            stream += position;
            uint value11;
            value11 = stream[0];
            if (value11 < 128)
            {
                position += 1;
                return value11;
            }
            value11 ^= ((uint)stream[1] << 7);
            if (value11 < 128 * 128)
            {
                position += 2;
                return value11 ^ 0x80;
            }
            value11 ^= ((uint)stream[2] << 14);
            if (value11 < 128 * 128 * 128)
            {
                position += 3;
                return value11 ^ 0x4080;
            }
            value11 ^= ((uint)stream[3] << 21);
            if (value11 < 128 * 128 * 128 * 128)
            {
                position += 4;
                return value11 ^ 0x204080;
            }
            value11 ^= ((uint)stream[4] << 28) ^ 0x10204080;
            position += 5;
            return value11;
        }

        /// <summary>
        /// Reads a 7-bit encoded uint.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="position">the position in the stream. Position will be updated after reading</param>
        /// <returns>The value</returns>
        public static uint ReadUInt32(byte[] stream, ref int position)
        {
            int pos = position;
            uint value11;
            value11 = stream[pos];
            if (value11 < 128)
            {
                position = pos + 1;
                return value11;
            }
            value11 ^= ((uint)stream[pos + 1] << 7);
            if (value11 < 128 * 128)
            {
                position = pos + 2;
                return value11 ^ 0x80;
            }
            value11 ^= ((uint)stream[pos + 2] << 14);
            if (value11 < 128 * 128 * 128)
            {
                position = pos + 3;
                return value11 ^ 0x4080;
            }
            value11 ^= ((uint)stream[pos + 3] << 21);
            if (value11 < 128 * 128 * 128 * 128)
            {
                position = pos + 4;
                return value11 ^ 0x204080;
            }
            value11 ^= ((uint)stream[pos + 4] << 28) ^ 0x10204080;
            position = pos + 5;
            return value11;
        }

        /// <summary>
        /// Reads a 7-bit encoded uint.
        /// </summary>
        /// <param name="stream">A stream to read from.</param>
        /// <returns>the value</returns>
        /// <remarks>
        /// This method will check for the end of the stream
        /// </remarks>
        /// <exception cref="EndOfStreamException">Occurs if the end of the stream was reached.</exception>
        public static uint ReadUInt32(Stream stream)
        {
            uint value11;
            value11 = stream.ReadNextByte();
            if (value11 < 128)
            {
                return value11;
            }
            value11 ^= ((uint)stream.ReadNextByte() << 7);
            if (value11 < 128 * 128)
            {
                return value11 ^ 0x80;
            }
            value11 ^= ((uint)stream.ReadNextByte() << 14);
            if (value11 < 128 * 128 * 128)
            {
                return value11 ^ 0x4080;
            }
            value11 ^= ((uint)stream.ReadNextByte() << 21);
            if (value11 < 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x204080;
            }
            value11 ^= ((uint)stream.ReadNextByte() << 28) ^ 0x10204080;
            return value11;
        }

        /// <summary>
        /// Reads a 7-bit encoded uint.
        /// </summary>
        /// <param name="stream">A delegate where to read the next byte</param>
        /// <returns>the value</returns>
        public static uint ReadUInt32(Func<byte> stream)
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

        #endregion

        #endregion

        #region [ 64 bit ]

        /// <summary>
        /// Gets the number of bytes required to write the provided value.
        /// </summary>
        /// <param name="value1">the value to measure</param>
        /// <returns>The number of bytes needed to store the provided value.</returns>
        public static int GetSize(ulong value1)
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

        /// <summary>
        /// Gets the number of bytes for the supplied value in the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public unsafe static int MeasureUInt64(byte* stream)
        {
            if (stream[0] < 128)
                return 1;
            if (stream[1] < 128)
                return 2;
            if (stream[2] < 128)
                return 3;
            if (stream[3] < 128)
                return 4;
            if (stream[4] < 128)
                return 5;
            if (stream[5] < 128)
                return 6;
            if (stream[6] < 128)
                return 7;
            if (stream[7] < 128)
                return 8;
            return 9;
        }

        /// <summary>
        /// Gets the number of bytes for the supplied value in the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public unsafe static int MeasureUInt64(byte* stream, int position)
        {
            return MeasureUInt64(stream + position);
        }

        /// <summary>
        /// Gets the number of bytes for the supplied value in the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static int MeasureUInt64(byte[] stream, int position)
        {
            if (stream[position + 0] < 128)
                return 1;
            if (stream[position + 1] < 128)
                return 2;
            if (stream[position + 2] < 128)
                return 3;
            if (stream[position + 3] < 128)
                return 4;
            if (stream[position + 4] < 128)
                return 5;
            if (stream[position + 5] < 128)
                return 6;
            if (stream[position + 6] < 128)
                return 7;
            if (stream[position + 7] < 128)
                return 8;
            return 9;
        }

        #region [ Write ]

        /// <summary>
        /// Writes the 7-bit encoded value to the provided stream.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="value1">the value to write</param>
        /// <returns>The number of bytes required to store the value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe public static int Write(byte* stream, ulong value1)
        {
            if (value1 < 128)
            {
                stream[0] = (byte)value1;
                return 1;
            }
            stream[0] = (byte)(value1 | 128);
            if (value1 < 128 * 128)
            {
                stream[1] = (byte)(value1 >> 7);
                return 2;
            }
            stream[1] = (byte)((value1 >> 7) | 128);
            if (value1 < 128 * 128 * 128)
            {
                stream[2] = (byte)(value1 >> (7 + 7));
                return 3;
            }
            stream[2] = (byte)((value1 >> (7 + 7)) | 128);
            if (value1 < 128 * 128 * 128 * 128)
            {
                stream[3] = (byte)(value1 >> (7 + 7 + 7));
                return 4;
            }
            stream[3] = (byte)((value1 >> (7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128)
            {
                stream[4] = (byte)(value1 >> (7 + 7 + 7 + 7));
                return 5;
            }
            stream[4] = (byte)((value1 >> (7 + 7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128)
            {
                stream[5] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7));
                return 6;
            }
            stream[5] = (byte)((value1 >> (7 + 7 + 7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
            {
                stream[6] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7));
                return 7;
            }
            stream[6] = (byte)((value1 >> (7 + 7 + 7 + 7 + 7 + 7)) | 128);
            if (value1 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
            {
                stream[7] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7));
                return 8;
            }
            stream[7] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7) | 128);
            stream[8] = (byte)(value1 >> (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
            return 9;
        }

        /// <summary>
        /// Writes the 7-bit encoded value to the provided stream.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="position">a reference parameter to the starting position. 
        /// This field will be updated. when the function returns</param>
        /// <param name="value1">the value to write</param>
        unsafe public static void Write(byte* stream, ref int position, ulong value1)
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

        /// <summary>
        /// Writes the 7-bit encoded value to the provided stream.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="position">a reference parameter to the starting position. 
        /// This field will be updated. when the function returns</param>
        /// <param name="value1">the value to write</param>
        public static void Write(byte[] stream, ref int position, ulong value1)
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

        /// <summary>
        /// Writes the 7-bit encoded value to the provided stream.
        /// </summary>
        /// <param name="stream">a delegate to a write byte method</param>
        /// <param name="value1">the value to write</param>
        public static void Write(Action<byte> stream, ulong value1)
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

        #endregion

        #region [ Read ]

        /// <summary>
        /// Reads a 7-bit encoded ulong.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="position">the position in the stream. Position will be updated after reading</param>
        /// <returns>The value</returns>
        public unsafe static ulong ReadUInt64(byte* stream, ref int position)
        {
            stream += position;
            ulong value11;
            value11 = stream[0];
            if (value11 < 128)
            {
                position += 1;
                return value11;
            }
            value11 ^= ((ulong)stream[1] << (7));
            if (value11 < 128 * 128)
            {
                position += 2;
                return value11 ^ 0x80;
            }
            value11 ^= ((ulong)stream[2] << (7 + 7));
            if (value11 < 128 * 128 * 128)
            {
                position += 3;
                return value11 ^ 0x4080;
            }
            value11 ^= ((ulong)stream[3] << (7 + 7 + 7));
            if (value11 < 128 * 128 * 128 * 128)
            {
                position += 4;
                return value11 ^ 0x204080;
            }
            value11 ^= ((ulong)stream[4] << (7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128)
            {
                position += 5;
                return value11 ^ 0x10204080L;
            }
            value11 ^= ((ulong)stream[5] << (7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128)
            {
                position += 6;
                return value11 ^ 0x810204080L;
            }
            value11 ^= ((ulong)stream[6] << (7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
            {
                position += 7;
                return value11 ^ 0x40810204080L;
            }
            value11 ^= ((ulong)stream[7] << (7 + 7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
            {
                position += 8;
                return value11 ^ 0x2040810204080L;
            }
            value11 ^= ((ulong)stream[8] << (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
            position += 9;
            return value11 ^ 0x102040810204080L;
        }

        /// <summary>
        /// Reads a 7-bit encoded ulong.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="position">the position in the stream. Position will be updated after reading</param>
        /// <returns>The value</returns>
        public static ulong ReadUInt64(byte[] stream, ref int position)
        {
            int pos = position;
            ulong value11;
            value11 = stream[pos];
            if (value11 < 128)
            {
                position += 1;
                return value11;
            }
            value11 ^= ((ulong)stream[pos + 1] << (7));
            if (value11 < 128 * 128)
            {
                position += 2;
                return value11 ^ 0x80;
            }
            value11 ^= ((ulong)stream[pos + 2] << (7 + 7));
            if (value11 < 128 * 128 * 128)
            {
                position += 3;
                return value11 ^ 0x4080;
            }
            value11 ^= ((ulong)stream[pos + 3] << (7 + 7 + 7));
            if (value11 < 128 * 128 * 128 * 128)
            {
                position += 4;
                return value11 ^ 0x204080;
            }
            value11 ^= ((ulong)stream[pos + 4] << (7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128)
            {
                position += 5;
                return value11 ^ 0x10204080L;
            }
            value11 ^= ((ulong)stream[pos + 5] << (7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128)
            {
                position += 6;
                return value11 ^ 0x810204080L;
            }
            value11 ^= ((ulong)stream[pos + 6] << (7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
            {
                position += 7;
                return value11 ^ 0x40810204080L;
            }
            value11 ^= ((ulong)stream[pos + 7] << (7 + 7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
            {
                position += 8;
                return value11 ^ 0x2040810204080L;
            }
            value11 ^= ((ulong)stream[pos + 8] << (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
            position += 9;
            return value11 ^ 0x102040810204080L;
        }

        /// <summary>
        /// Reads a 7-bit encoded ulong.
        /// </summary>
        /// <param name="stream">A stream to read from.</param>
        /// <returns>the value</returns>
        /// <remarks>
        /// This method will check for the end of the stream
        /// </remarks>
        /// <exception cref="EndOfStreamException">Occurs if the end of the stream was reached.</exception>
        public static ulong ReadUInt64(Stream stream)
        {
            ulong value11;
            value11 = stream.ReadNextByte();
            if (value11 < 128)
            {
                return value11;
            }
            value11 ^= ((ulong)stream.ReadNextByte() << (7));
            if (value11 < 128 * 128)
            {
                return value11 ^ 0x80;
            }
            value11 ^= ((ulong)stream.ReadNextByte() << (7 + 7));
            if (value11 < 128 * 128 * 128)
            {
                return value11 ^ 0x4080;
            }
            value11 ^= ((ulong)stream.ReadNextByte() << (7 + 7 + 7));
            if (value11 < 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x204080;
            }
            value11 ^= ((ulong)stream.ReadNextByte() << (7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x10204080L;
            }
            value11 ^= ((ulong)stream.ReadNextByte() << (7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x810204080L;
            }
            value11 ^= ((ulong)stream.ReadNextByte() << (7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x40810204080L;
            }
            value11 ^= ((ulong)stream.ReadNextByte() << (7 + 7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x2040810204080L;
            }
            value11 ^= ((ulong)stream.ReadNextByte() << (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
            return value11 ^ 0x102040810204080L;
        }

        /// <summary>
        /// Reads a 7-bit encoded uint.
        /// </summary>
        /// <param name="stream">A delegate where to read the next byte</param>
        /// <returns>the value</returns>
        public static ulong ReadUInt64(Func<byte> stream)
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

        #endregion

        #endregion












    }
}
