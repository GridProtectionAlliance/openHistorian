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
using System.Collections.Generic;
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
    public unsafe static class StreamExtensions
    {
        #region [ Object Read/Write ]

        private enum ObjectType : byte
        {
            Null = 0,
            DBNull = 1,
            BooleanTrue = 2,
            BooleanFalse = 3,
            SByte = 5,
            Byte = 6,
            Char = 4,
            Int16 = 7,
            UInt16 = 8,
            Int32 = 9,
            UInt32 = 10,
            Int64 = 11,
            UInt64 = 12,
            Single = 13,
            Double = 14,
            Decimal = 15,
            DateTime = 16,
            String = 17,
            ByteArray = 18,
            CharArray = 19,
            Guid = 20
        }

        public static void WriteObject(this Stream stream, object value)
        {
            if (value == null)
            {
                stream.Write((byte)ObjectType.Null);
                return;
            }
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Empty:
                    stream.Write((byte)ObjectType.Null);
                    break;
                case TypeCode.DBNull:
                    stream.Write((byte)ObjectType.DBNull);
                    break;
                case TypeCode.Boolean:
                    if ((bool)value)
                    {
                        stream.Write((byte)ObjectType.BooleanFalse);
                    }
                    else
                    {
                        stream.Write((byte)ObjectType.BooleanTrue);
                    }
                    break;
                case TypeCode.Char:
                    stream.Write((byte)ObjectType.Char);
                    stream.Write((char)value);
                    break;
                case TypeCode.SByte:
                    stream.Write((byte)ObjectType.SByte);
                    stream.Write((sbyte)value);
                    break;
                case TypeCode.Byte:
                    stream.Write((byte)ObjectType.Byte);
                    stream.Write((byte)value);
                    break;
                case TypeCode.Int16:
                    stream.Write((byte)ObjectType.Int16);
                    stream.Write((short)value);
                    break;
                case TypeCode.UInt16:
                    stream.Write((byte)ObjectType.UInt16);
                    stream.Write((ushort)value);
                    break;
                case TypeCode.Int32:
                    stream.Write((byte)ObjectType.Int32);
                    stream.Write((int)value);
                    break;
                case TypeCode.UInt32:
                    stream.Write((byte)ObjectType.UInt32);
                    stream.Write((uint)value);
                    break;
                case TypeCode.Int64:
                    stream.Write((byte)ObjectType.Int64);
                    stream.Write((long)value);
                    break;
                case TypeCode.UInt64:
                    stream.Write((byte)ObjectType.UInt64);
                    stream.Write((ulong)value);
                    break;
                case TypeCode.Single:
                    stream.Write((byte)ObjectType.Single);
                    stream.Write((float)value);
                    break;
                case TypeCode.Double:
                    stream.Write((byte)ObjectType.Double);
                    stream.Write((double)value);
                    break;
                case TypeCode.Decimal:
                    stream.Write((byte)ObjectType.Decimal);
                    stream.Write((Decimal)value);
                    break;
                case TypeCode.DateTime:
                    stream.Write((byte)ObjectType.DateTime);
                    stream.Write((DateTime)value);
                    break;
                case TypeCode.String:
                    stream.Write((byte)ObjectType.String);
                    stream.Write((String)value);
                    break;
                case TypeCode.Object:
                    if (value is byte[])
                    {
                        stream.Write((byte)ObjectType.ByteArray);
                        stream.WriteWithLength((byte[])value);
                    }
                    else if (value is char[])
                    {
                        stream.Write((byte)ObjectType.CharArray);
                        stream.Write(new string((char[])value));
                    }
                    else if (value is Guid)
                    {
                        stream.Write((byte)ObjectType.Guid);
                        stream.Write((Guid)value);
                    }
                    else
                    {
                        throw new NotSupportedException("This type cannot be serialized");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static object ReadObject(this Stream stream)
        {
            ObjectType type = (ObjectType)stream.ReadNextByte();
            switch (type)
            {
                case ObjectType.Null:
                    return null;
                case ObjectType.DBNull:
                    return DBNull.Value;
                case ObjectType.BooleanTrue:
                    return true;
                case ObjectType.BooleanFalse:
                    return false;
                case ObjectType.Char:
                    return stream.ReadChar();
                case ObjectType.SByte:
                    return stream.ReadSByte();
                case ObjectType.Byte:
                    return stream.ReadNextByte();
                case ObjectType.Int16:
                    return stream.ReadInt16();
                case ObjectType.UInt16:
                    return stream.ReadUInt16();
                case ObjectType.Int32:
                    return stream.ReadInt32();
                case ObjectType.UInt32:
                    return stream.ReadUInt32();
                case ObjectType.Int64:
                    return stream.ReadInt64();
                case ObjectType.UInt64:
                    return stream.ReadUInt64();
                case ObjectType.Single:
                    return stream.ReadSingle();
                case ObjectType.Double:
                    return stream.ReadDouble();
                case ObjectType.Decimal:
                    return stream.ReadDecimal();
                case ObjectType.DateTime:
                    return stream.ReadDateTime();
                case ObjectType.String:
                    return stream.ReadString();
                case ObjectType.ByteArray:
                    return stream.ReadBytes();
                case ObjectType.CharArray:
                    return stream.ReadString().ToCharArray();
                case ObjectType.Guid:
                    return stream.ReadGuid();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region [ 1 byte values ]

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
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static bool ReadBoolean(this Stream stream)
        {
            return stream.ReadNextByte() != 0;
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, bool value)
        {
            if (value)
                stream.Write((byte)1);
            else
                stream.Write((byte)0);
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, sbyte value)
        {
            stream.Write((byte)value);
        }

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static sbyte ReadSByte(this Stream stream)
        {
            return (sbyte)stream.ReadNextByte();
        }

        #endregion

        #region [ 2-byte values ]

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
        public static void Write(this Stream stream, ushort value)
        {
            Write(stream, (short)value);
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
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static short ReadInt16(this Stream stream)
        {
            byte[] data = stream.ReadBytes(2);
            return LittleEndian.ToInt16(data, 0);
        }

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static ushort ReadUInt16(this Stream stream)
        {
            return (ushort)stream.ReadInt16();
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

        #endregion


        #region [ 4-byte values ]

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
        public static void Write(this Stream stream, uint value)
        {
            stream.Write((int)value);
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, float value)
        {
            Write(stream, *(int*)&value);
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
        public static uint ReadUInt32(this Stream stream)
        {
            return (uint)stream.ReadInt32();
        }

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static float ReadSingle(this Stream stream)
        {
            var value = stream.ReadInt32();
            return *(float*)&value;
        }

        #endregion

        #region [ 8-byte values ]

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
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, ulong value)
        {
            stream.Write((long)value);
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, double value)
        {
            stream.Write(*(long*)&value);
        }

        /// <summary>
        /// Writes the supplied <see cref="value"/> to 
        /// <see cref="stream"/> in little endian format.
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, DateTime value)
        {
            stream.Write(value.Ticks);
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
        public static double ReadDouble(this Stream stream)
        {
            var value = stream.ReadInt64();
            return *(double*)&value;
        }

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static ulong ReadUInt64(this Stream stream)
        {
            return (ulong)stream.ReadInt64();
        }

        /// <summary>
        /// Reads the value from the stream in little endian format.
        /// </summary>
        /// <param name="stream">the stream to read from.</param>
        /// <returns>The value read</returns>
        public static DateTime ReadDateTime(this Stream stream)
        {
            return new DateTime(stream.ReadInt64());
        }

        #endregion


        #region [ 16-byte values ]

        /// <summary>
        /// Writes the supplied string to the <see cref="Stream"/> in UTF8 encoding.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void Write(this Stream stream, decimal value)
        {
            stream.Write(LittleEndian.GetBytes(value));
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
        /// Reads a decimal from the stream in Little Endian bytes.
        /// </summary>
        /// <param name="stream">the stream to read the decimal from.</param>
        /// <returns>the decimal value</returns>
        public static decimal ReadDecimal(this Stream stream)
        {
            return LittleEndian.ToDecimal(stream.ReadBytes(16), 0);
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

        #region [ byte array ]

        /// <summary>
        /// Writes the entire buffer to the <see cref="stream"/>
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        /// <param name="value">the value to write</param>
        public static void Write(this Stream stream, byte[] value)
        {
            if (value.Length > 0)
                stream.Write(value, 0, value.Length);
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
            if (value.Length > 0)
                stream.Write(value, 0, value.Length);
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
            if (length > 0)
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
            if (length > 0)
                stream.ReadAll(data, 0, data.Length);
            return data;
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

        #endregion


        #region [ Write ]



        /// <summary>
        /// Writes the supplied string to the <see cref="Stream"/> in UTF8 encoding.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void Write(this Stream stream, string value)
        {
            if (value.Length == 0)
            {
                stream.WriteByte(0);
                return;
            }
            WriteWithLength(stream, Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Writes the supplied Collection to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="collection"></param>
        public static void WriteCollection(this Stream stream, ICollection<string> collection)
        {
            if (collection == null)
            {
                stream.Write(false);
                return;
            }
            stream.Write(true);
            stream.Write(collection.Count);
            foreach (var item in collection)
            {
                stream.WriteNullable(item);
            }
        }
        /// <summary>
        /// Writes the supplied Collection to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="collection"></param>
        public static void WriteCollection(this Stream stream, ICollection<int> collection)
        {
            if (collection == null)
            {
                stream.Write(false);
                return;
            }
            stream.Write(true);
            stream.Write(collection.Count);
            foreach (var item in collection)
            {
                stream.Write(item);
            }
        }
        /// <summary>
        /// Writes the supplied string to the <see cref="Stream"/> 
        /// in UTF8 encoding with a prefix if the value is null
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void WriteNullable(this Stream stream, string value)
        {
            if (value == null)
            {
                Write(stream, false);
            }
            else
            {
                Write(stream, true);
                Write(stream, value);
            }
        }


        #endregion

        #region [ Read ]

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
        /// Reads a string from the <see cref="Stream"/> that was encoded in UTF8.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadString(this Stream stream)
        {
            byte[] data = stream.ReadBytes();
            if (data.Length > 0)
                return Encoding.UTF8.GetString(data);
            return string.Empty;
        }

        /// <summary>
        /// Reads a string from the <see cref="Stream"/> that was encoded in UTF8. 
        /// Value can be null and is prefixed with a boolean.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadNullableString(this Stream stream)
        {
            if (stream.ReadBoolean())
                return stream.ReadString();
            return null;
        }

        /// <summary>
        /// Writes the supplied string to the <see cref="Stream"/> in UTF8 encoding.
        /// </summary>
        /// <param name="stream"></param>
        public static string[] ReadStringCollection(this Stream stream)
        {
            if (!stream.ReadBoolean())
                return null;
            int value = stream.ReadInt32();
            var rv = new string[value];
            for (int x = 0; x < rv.Length; x++)
            {
                rv[x] = stream.ReadNullableString();
            }
            return rv;
        }

        /// <summary>
        /// Writes the supplied string to the <see cref="Stream"/> in UTF8 encoding.
        /// </summary>
        /// <param name="stream"></param>
        public static int[] ReadInt32Collection(this Stream stream)
        {
            if (!stream.ReadBoolean())
                return null;
            int value = stream.ReadInt32();
            var rv = new int[value];
            for (int x = 0; x < rv.Length; x++)
            {
                rv[x] = stream.ReadInt32();
            }
            return rv;
        }

        #endregion

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowEOS()
        {
            throw new EndOfStreamException("End of stream");
        }

    }
}
