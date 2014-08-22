//******************************************************************************************************
//  StreamExtensions.cs - Gbtc
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
//  8/15/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace GSF.IO
{
    /// <summary>
    /// Extends <see cref="Stream"/> to allow standard read/write methods.
    /// </summary>
    /// <remarks>
    /// Everything is written in little endian format
    /// </remarks>
    public static class StreamExtensions
    {
        #region [ Write ]


        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, byte value)
        {
            stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, short value)
        {
            stream.WriteByte((byte)value);
            stream.WriteByte((byte)(value >> 8));
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, int value)
        {
            stream.WriteByte((byte)value);
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)(value >> 16));
            stream.WriteByte((byte)(value >> 24));
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, long value)
        {
            stream.WriteByte((byte)value);
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)(value >> 16));
            stream.WriteByte((byte)(value >> 24));
            stream.WriteByte((byte)(value >> 32));
            stream.WriteByte((byte)(value >> 40));
            stream.WriteByte((byte)(value >> 48));
            stream.WriteByte((byte)(value >> 56));
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> along with prefixing the length 
        /// so it can be properly read as a unit.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void WriteWithLength(this Stream stream, byte[] value)
        {
            Encoding7Bit.Write(stream.WriteByte, (uint)value.Length);
            stream.Write(value, 0, value.Length);
        }

        #endregion

        #region [ Read ]

        /// <summary>
        /// Reads a byte array from the <see cref="Stream"/>. 
        /// The number of bytes should be prefixed in the stream.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        /// <returns>A new array containing the bytes.</returns>
        public static byte[] ReadBytes(this Stream stream)
        {
            int length = (int)stream.Read7BitUInt32();
            if (length < 0)
                throw new Exception("Invalid length");
            byte[] data = new byte[length];
            stream.Read(data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static int ReadInt32(this Stream stream)
        {
            //Little endian encoded integer
            byte b1 = stream.ReadNextByte();
            byte b2 = stream.ReadNextByte();
            byte b3 = stream.ReadNextByte();
            byte b4 = stream.ReadNextByte();
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
        }

        /// <summary>
        /// Read a byte from the stream. 
        /// Will throw an exception if the end of the stream has been reached.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>the value read</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadNextByte(this Stream stream)
        {
            int value = stream.ReadByte();
            if (value < 0)
                ThrowEOS();
            return (byte)value;
        }
        
        /// <summary>
        /// Reads the 7-bit encoded value from the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static uint Read7BitUInt32(this Stream stream)
        {
            return Encoding7Bit.ReadUInt32(stream);
        }

        #endregion

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowEOS()
        {
            throw new EndOfStreamException("End of stream");
        }

    }
}
