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
using System.Text;

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
        public static void Write(this Stream stream, bool value)
        {
            if (value)
                stream.WriteByte(1);
            else
                stream.WriteByte(0);
        }

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
            Write(stream, LittleEndian.GetBytes(value));
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, char value)
        {
            Write(stream, (short)value);
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, int value)
        {
            Write(stream, LittleEndian.GetBytes(value));
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, long value)
        {
            Write(stream, LittleEndian.GetBytes(value));
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

        /// <summary>
        /// Writes the entire buffer to the <see cref="stream"/>
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, byte[] value)
        {
            stream.Write(value, 0, value.Length);
        }

        /// <summary>
        /// Writes a guid in little endian bytes to the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void Write(this Stream stream, Guid value)
        {
            Write(stream, GuidExtensions.ToLittleEndianBytes(value));
        }

       

        /// <summary>
        /// Writes the supplied string to the <see cref="Stream"/> in UTF8 encoding.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void Write(this Stream stream, string value)
        {
            WriteWithLength(stream, Encoding.UTF8.GetBytes(value));
        }

        #endregion

        #region [ Read ]

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static bool ReadBoolean(this Stream stream)
        {
            return stream.ReadNextByte() != 0;
        }

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
            stream.ReadAll(data, 0, data.Length);
            return data;
        }


        /// <summary>
        /// Reads a byte array from the <see cref="Stream"/>. 
        /// The number of bytes should be prefixed in the stream.
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        /// <param name="length">gets the number of bytes to read.</param>
        /// <returns>A new array containing the bytes.</returns>
        public static byte[] ReadBytes(this Stream stream, int length)
        {
            if (length < 0)
                throw new Exception("Invalid length");
            byte[] data = new byte[length];
            stream.ReadAll(data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static int ReadInt32(this Stream stream)
        {
            byte[] data = stream.ReadBytes(4);
            return LittleEndian.ToInt32(data, 0);
        }

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static long ReadInt64(this Stream stream)
        {
            byte[] data = stream.ReadBytes(8);
            return LittleEndian.ToInt64(data, 0);
        }

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static int ReadInt16(this Stream stream)
        {
            byte[] data = stream.ReadBytes(2);
            return LittleEndian.ToInt16(data, 0);
        }

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static char ReadChar(this Stream stream)
        {
            return (char)stream.ReadInt16();
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

        /// <summary>
        /// Reads all of the provided bytes. Will not return prematurely, 
        /// but continue to execute a <see cref="Stream.Read"/> command until the entire
        /// <see cref="length"/> has been read.
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="buffer">The buffer to write to</param>
        /// <param name="position">the start position in the <see cref="buffer"/></param>
        /// <param name="length">the number of bytes to read</param>
        /// <exception cref="EndOfStreamException">occurs if the end of the stream has been reached.</exception>
        public static void ReadAll(this Stream stream, byte[] buffer, int position, int length)
        {
            buffer.ValidateParameters(position, length);
            while (length > 0)
            {
                int bytesRead = stream.Read(buffer, position, length);
                if (bytesRead == 0)
                    throw new EndOfStreamException();
                length -= bytesRead;
                position += bytesRead;
            }
        }

        /// <summary>
        /// Reads a string from the <see cref="Stream"/> that was encoded in UTF8.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadString(this Stream stream)
        {
            byte[] data = stream.ReadBytes();
            return Encoding.UTF8.GetString(data);
        }


        /// <summary>
        /// Reads a Guid from the stream in Little Endian bytes.
        /// </summary>
        /// <param name="stream">the stream to read the guid from.</param>
        /// <returns>the guid value</returns>
        public static Guid ReadGuid(this Stream stream)
        {
            return GuidExtensions.ToLittleEndianGuid(stream.ReadBytes(16));
        }

        #endregion

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowEOS()
        {
            throw new EndOfStreamException("End of stream");
        }

    }
}
