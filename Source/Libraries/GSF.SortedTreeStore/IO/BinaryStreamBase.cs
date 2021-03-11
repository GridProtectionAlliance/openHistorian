//******************************************************************************************************
//  BinaryStreamBase.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  5/1/2012 - Steven E. Chisholm
//       Generated original version of source code.
//  11/30/2012 - Steven E. Chisholm
//       Converted to a base class 
//       
//
//******************************************************************************************************

using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace GSF.IO
{
    /// <summary>
    /// An abstract class for reading/writing to a little endian stream.
    /// </summary>
    public abstract unsafe class BinaryStreamBase
        : IDisposable
    {
        /// <summary>
        /// A shared instance of UTF8 encoding.
        /// </summary>
        public readonly static Encoding Utf8 = Encoding.UTF8;

        /// <summary>
        /// A <see cref="Stream"/> implementation of this <see cref="BinaryStreamBase"/>
        /// </summary>
        public readonly BinaryStreamStream Stream;

        /// <summary>
        /// A temporary buffer where data is read/written to before it is serialized to the stream.
        /// </summary>
        private readonly byte[] m_buffer = new byte[16];

        private bool m_disposed;

        /// <summary>
        /// Creates a <see cref="BinaryStreamBase"/>
        /// </summary>
        protected BinaryStreamBase()
        {
            Stream = new BinaryStreamStream(this);
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>
        /// true if the stream supports writing; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>

        public abstract bool CanWrite { get; }
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>
        /// true if the stream supports reading; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>

        public abstract bool CanRead { get; }
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>
        /// true if the stream supports seeking; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public abstract bool CanSeek { get; }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
        public abstract long Length { get; }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception><exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
        public abstract long Position { get; set; }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream. </param><param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream. </param><param name="count">The number of bytes to be written to the current stream. </param><filterpriority>1</filterpriority>

        public abstract void Write(byte[] buffer, int offset, int count);
        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes. </param><exception cref="T:System.IO.IOException">An I/O error occurs. </exception><exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>2</filterpriority>
        public abstract void SetLength(long value);

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception><filterpriority>2</filterpriority>
        public abstract void Flush();

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source. </param><param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream. </param><param name="count">The maximum number of bytes to be read from the current stream. </param><exception cref="T:System.ArgumentException">The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length. </exception><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative. </exception><exception cref="T:System.IO.IOException">An I/O error occurs. </exception><exception cref="T:System.NotSupportedException">The stream does not support reading. </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
        public abstract int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter. </param><param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position. </param><exception cref="T:System.IO.IOException">An I/O error occurs. </exception><exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><filterpriority>1</filterpriority>
        public long Seek(long offset, SeekOrigin origin)
        {
            if (!CanSeek)
                throw new NotSupportedException();

            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
                default:
                    throw new InvalidEnumArgumentException("origin", (int)origin, typeof(SeekOrigin));
            }
            return Position;
        }

        /// <summary>
        /// Updates the local buffer data.
        /// </summary>
        /// <param name="isWriting">hints to the stream if write access is desired.</param>
        public virtual void UpdateLocalBuffer(bool isWriting)
        {

        }

        /// <summary>
        /// Copies a specified number of bytes to a new location
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="length"></param>
        public virtual void Copy(long source, long destination, int length)
        {
            byte[] data = new byte[length];
            long oldPos = Position;
            Position = source;
            ReadAll(data, 0, length);
            Position = destination;
            Write(data, 0, length);
            Position = oldPos;
        }

        /// <summary>
        /// Inserts a certain number of bytes into the stream, shifting valid data to the right.  The stream's position remains unchanged. 
        /// (ie. pointing to the beginning of the newly inserted bytes).
        /// </summary>
        /// <param name="numberOfBytes">The number of bytes to insert</param>
        /// <param name="lengthOfValidDataToShift">The number of bytes that will need to be shifted to perform this insert</param>
        /// <remarks>Internally this fuction merely acomplishes an Array.Copy(stream,position,stream,position+numberOfBytes,lengthOfValidDataToShift)
        /// However, it's much more complicated than this. So this is a pretty useful function.
        /// The newly created space is uninitialized. 
        /// </remarks>
        public void InsertBytes(int numberOfBytes, int lengthOfValidDataToShift)
        {
            long pos = Position;
            Copy(Position, Position + numberOfBytes, lengthOfValidDataToShift);
            Position = pos;
        }

        /// <summary>
        /// Removes a certain number of bytes from the stream, shifting valid data after this location to the left.  The stream's position remains unchanged. 
        /// (ie. pointing to where the data used to exist).
        /// </summary>
        /// <param name="numberOfBytes">The distance to shift.  Positive means shifting to the right (ie. inserting data)
        /// Negative means shift to the left (ie. deleteing data)</param>
        /// <param name="lengthOfValidDataToShift">The number of bytes that will need to be shifted to perform the remove. 
        /// This only includes the data that is valid after the shift is complete, and not the data that will be removed.</param>
        /// <remarks>Internally this fuction merely acomplishes an Array.Copy(stream,position+numberOfBytes,stream,position,lengthOfValidDataToShift)
        /// However, it's much more complicated than this. So this is a pretty useful function.
        /// The space at the end of the copy is uninitialized. 
        /// </remarks>
        public void RemoveBytes(int numberOfBytes, int lengthOfValidDataToShift)
        {
            long pos = Position;
            Copy(Position + numberOfBytes, Position, lengthOfValidDataToShift);
            Position = pos;
        }

        #region [ Write ]

        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(sbyte value)
        {
            Write((byte)value);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(bool value)
        {
            if (value)
                Write((byte)1);
            else
                Write((byte)0);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(ushort value)
        {
            Write((short)value);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(uint value)
        {
            Write((int)value);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(ulong value)
        {
            Write((long)value);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(float value)
        {
            Write(*(int*)&value);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(double value)
        {
            Write(*(long*)&value);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(DateTime value)
        {
            Write(value.Ticks);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public virtual void Write(byte value)
        {
            m_buffer[0] = value;
            Write(m_buffer, 0, 1);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public virtual void Write(short value)
        {
            m_buffer[0] = (byte)value;
            m_buffer[1] = (byte)(value >> 8);
            Write(m_buffer, 0, 2);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public virtual void Write(int value)
        {
            m_buffer[0] = (byte)value;
            m_buffer[1] = (byte)(value >> 8);
            m_buffer[2] = (byte)(value >> 16);
            m_buffer[3] = (byte)(value >> 24);
            Write(m_buffer, 0, 4);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public virtual void Write(long value)
        {
            m_buffer[0] = (byte)value;
            m_buffer[1] = (byte)(value >> 8);
            m_buffer[2] = (byte)(value >> 16);
            m_buffer[3] = (byte)(value >> 24);
            m_buffer[4] = (byte)(value >> 32);
            m_buffer[5] = (byte)(value >> 40);
            m_buffer[6] = (byte)(value >> 48);
            m_buffer[7] = (byte)(value >> 56);
            Write(m_buffer, 0, 8);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(decimal value)
        {
            if (BitConverter.IsLittleEndian)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(decimal*)lp = value;
                }
            }
            else
            {
                byte* ptr = (byte*)&value;
                m_buffer[0] = ptr[3];
                m_buffer[1] = ptr[2];
                m_buffer[2] = ptr[1];
                m_buffer[3] = ptr[0];
                m_buffer[4] = ptr[7];
                m_buffer[5] = ptr[6];
                m_buffer[6] = ptr[5];
                m_buffer[7] = ptr[4];
                m_buffer[8] = ptr[11];
                m_buffer[9] = ptr[10];
                m_buffer[10] = ptr[9];
                m_buffer[11] = ptr[8];
                m_buffer[12] = ptr[15];
                m_buffer[13] = ptr[14];
                m_buffer[14] = ptr[13];
                m_buffer[15] = ptr[12];
            }
            Write(m_buffer, 0, 16);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(Guid value)
        {
            byte* src = (byte*)&value;
            fixed (byte* dst = m_buffer)
            {
                if (BitConverter.IsLittleEndian)
                {
                    //just copy the data
                    *(long*)(dst + 0) = *(long*)(src + 0);
                    *(long*)(dst + 8) = *(long*)(src + 8);
                }
                else
                {
                    //Guid._a (int)  //swap endian
                    dst[0] = src[3];
                    dst[1] = src[2];
                    dst[2] = src[1];
                    dst[3] = src[0];
                    //Guid._b (short) //swap endian
                    dst[4] = src[5];
                    dst[5] = src[4];
                    //Guid._c (short) //swap endian
                    dst[6] = src[7];
                    dst[7] = src[6];
                    //Guid._d - Guid._k (8 bytes)
                    *(long*)(dst + 8) = *(long*)(src + 8);
                }
            }
            Write(m_buffer, 0, 16);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void WriteUInt24(uint value)
        {
            Write((ushort)value);
            Write((byte)(value >> 16));
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void WriteUInt40(ulong value)
        {
            Write((uint)value);
            Write((byte)(value >> 32));
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void WriteUInt48(ulong value)
        {
            Write((uint)value);
            Write((ushort)(value >> 32));
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void WriteUInt56(ulong value)
        {
            Write((uint)value);
            Write((ushort)(value >> 32));
            Write((byte)(value >> 48));
        }

        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        /// <param name="bytes">the number of bytes to write.</param>
        public void WriteUInt(ulong value, int bytes)
        {
            switch (bytes)
            {
                case 0:
                    return;
                case 1:
                    Write((byte)value);
                    return;
                case 2:
                    Write((ushort)value);
                    return;
                case 3:
                    WriteUInt24((uint)value);
                    return;
                case 4:
                    Write((uint)value);
                    return;
                case 5:
                    WriteUInt40(value);
                    return;
                case 6:
                    WriteUInt48(value);
                    return;
                case 7:
                    WriteUInt56(value);
                    return;
                case 8:
                    Write(value);
                    return;
            }
            throw new ArgumentOutOfRangeException("bytes", "must be between 0 and 8 inclusive.");
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public virtual void Write7Bit(uint value)
        {
            Encoding7Bit.Write(Write, value);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public virtual void Write7Bit(ulong value)
        {
            Encoding7Bit.Write(Write, value);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(string value)
        {
            WriteWithLength(Utf8.GetBytes(value));
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void Write(byte[] value)
        {
            Write(value, 0, value.Length);
        }
        /// <summary>
        /// Writes the specifed <see cref="value"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="value">the value to write</param>
        public void WriteWithLength(byte[] value)
        {
            Write7Bit((uint)value.Length);
            Write(value, 0, value.Length);
        }

        /// <summary>
        /// Writes the specifed <see cref="buffer"/> to the underlying stream in little-endian format.
        /// </summary>
        /// <param name="buffer">the pointer to the first byte</param>
        /// <param name="length">the number of bytes to write</param>
        public virtual void Write(byte* buffer, int length)
        {
            for (int x = 0; x < length; x++)
            {
                Write(buffer[x]);
            }
        }

        #endregion

        #region [ Read ]

        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public sbyte ReadInt8()
        {
            return (sbyte)ReadUInt8();
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public bool ReadBoolean()
        {
            return ReadUInt8() != 0;
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public ushort ReadUInt16()
        {
            return (ushort)ReadInt16();
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public uint ReadUInt24()
        {
            uint value = ReadUInt16();
            return value | ((uint)ReadUInt8() << 16);
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public uint ReadUInt32()
        {
            return (uint)ReadInt32();
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public ulong ReadUInt40()
        {
            ulong value = ReadUInt32();
            return value | ((ulong)ReadUInt8() << 32);
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public ulong ReadUInt48()
        {
            ulong value = ReadUInt32();
            return value | ((ulong)ReadUInt16() << 32);
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public ulong ReadUInt56()
        {
            ulong value = ReadUInt32();
            return value | ((ulong)ReadUInt24() << 32);
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public ulong ReadUInt64()
        {
            return (ulong)ReadInt64();
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <param name="bytes">the number of bytes in the value</param>
        /// <returns>The data read</returns>
        public ulong ReadUInt(int bytes)
        {
            switch (bytes)
            {
                case 0:
                    return 0;
                case 1:
                    return ReadUInt8();
                case 2:
                    return ReadUInt16();
                case 3:
                    return ReadUInt24();
                case 4:
                    return ReadUInt32();
                case 5:
                    return ReadUInt40();
                case 6:
                    return ReadUInt48();
                case 7:
                    return ReadUInt56();
                case 8:
                    return ReadUInt64();
            }
            throw new ArgumentOutOfRangeException("bytes", "must be between 0 and 8 inclusive.");
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public float ReadSingle()
        {
            int value = ReadInt32();
            return *(float*)&value;
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public double ReadDouble()
        {
            long value = ReadInt64();
            return *(double*)&value;
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public DateTime ReadDateTime()
        {
            return new DateTime(ReadInt64());
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public virtual byte ReadUInt8()
        {
            ReadAll(m_buffer, 0, 1);
            return m_buffer[0];
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public virtual short ReadInt16()
        {
            ReadAll(m_buffer, 0, 2);
            return (short)(m_buffer[0]
                | m_buffer[1] << 8);
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public virtual int ReadInt32()
        {
            ReadAll(m_buffer, 0, 4);
            return m_buffer[0]
                | m_buffer[1] << 8
                | m_buffer[2] << 16
                | m_buffer[3] << 24;
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public virtual long ReadInt64()
        {
            ReadAll(m_buffer, 0, 8);
            return m_buffer[0]
                   | (long)m_buffer[1] << 8
                   | (long)m_buffer[2] << 16
                   | (long)m_buffer[3] << 24
                   | (long)m_buffer[4] << 32
                   | (long)m_buffer[5] << 40
                   | (long)m_buffer[6] << 48
                   | (long)m_buffer[7] << 56;
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public decimal ReadDecimal()
        {
            ReadAll(m_buffer, 0, 16);

            if (BitConverter.IsLittleEndian)
            {
                fixed (byte* lp = m_buffer)
                {
                    return *(decimal*)lp;
                }
            }
            else
            {
                decimal rv;
                byte* ptr = (byte*)&rv;

                ptr[3] = m_buffer[0];
                ptr[2] = m_buffer[1];
                ptr[1] = m_buffer[2];
                ptr[0] = m_buffer[3];

                ptr[7] = m_buffer[4];
                ptr[6] = m_buffer[5];
                ptr[5] = m_buffer[6];
                ptr[4] = m_buffer[7];

                ptr[11] = m_buffer[8];
                ptr[10] = m_buffer[9];
                ptr[9] = m_buffer[10];
                ptr[8] = m_buffer[11];

                ptr[15] = m_buffer[12];
                ptr[14] = m_buffer[13];
                ptr[13] = m_buffer[14];
                ptr[12] = m_buffer[15];
                return rv;
            }
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public Guid ReadGuid()
        {
            ReadAll(m_buffer, 0, 16);

            Guid rv;
            byte* dst = (byte*)&rv;
            fixed (byte* src = m_buffer)
            {
                if (BitConverter.IsLittleEndian)
                {
                    //internal stucture is correct, just copy
                    *(long*)(dst + 0) = *(long*)(src + 0);
                    *(long*)(dst + 8) = *(long*)(src + 8);
                }
                else
                {
                    //Guid._a (int) //swap endian
                    dst[0] = src[3];
                    dst[1] = src[2];
                    dst[2] = src[1];
                    dst[3] = src[0];
                    //Guid._b (short) //swap endian
                    dst[4] = src[5];
                    dst[5] = src[4];
                    //Guid._c (short) //swap endian
                    dst[6] = src[7];
                    dst[7] = src[6];
                    //Guid._d - Guid._k (8 bytes)
                    *(long*)(dst + 8) = *(long*)(src + 8);
                }

                return rv;
            }
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public virtual uint Read7BitUInt32()
        {
            return Encoding7Bit.ReadUInt32(ReadUInt8);
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public virtual ulong Read7BitUInt64()
        {
            return Encoding7Bit.ReadUInt64(ReadUInt8);
        }
        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <param name="count">the number of bytes to read</param>
        /// <returns>The data read</returns>
        public byte[] ReadBytes(int count)
        {
            byte[] value = new byte[count];
            ReadAll(value, 0, count);
            return value;
        }

        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public byte[] ReadBytes()
        {
            return ReadBytes((int)Read7BitUInt32());
        }

        /// <summary>
        /// Reads a byte array from the provided stream. 
        /// If the size of the stream exceedes <see cref="maxLength"/>
        /// <see cref="value"/> is set to null and this function returns false.
        /// </summary>
        /// <param name="maxLength"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method can be used to limit the byte size returned.
        /// Since an untrusted source could claim that the length is 
        /// <see cref="int.MaxValue"/>, this prevents allocating 2GB
        /// of RAM to store the result.
        /// </remarks>
        public bool TryReadBytes(int maxLength, out byte[] value)
        {
            int length = (int)Read7BitUInt32();
            if (length < 0 || length > maxLength)
            {
                value = null;
                return false;
            }
            value = ReadBytes(length);
            return true;
        }

        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <returns>The data read</returns>
        public string ReadString()
        {
            return Utf8.GetString(ReadBytes());
        }

        /// <summary>
        /// Reads a string from the provided stream. 
        /// If the size of the string exceedes <see cref="maxLength"/>
        /// <see cref="value"/> is set to null and this function returns false.
        /// </summary>
        /// <param name="maxLength">The maximum number of characters in the string.</param>
        /// <param name="value">an output</param>
        /// <returns></returns>
        /// <remarks>
        /// This method can be used to limit the byte size returned.
        /// Since an untrusted source could claim that the length is 
        /// <see cref="int.MaxValue"/>, this prevents allocating 2GB
        /// of RAM to store the result.
        /// </remarks>
        public bool TryReadString(int maxLength, out string value)
        {
            if (!TryReadBytes(maxLength * 6, out byte[] data))
            {
                value = null;
                return false;
            }
            value = Utf8.GetString(data);
            if (value.Length > maxLength)
            {
                value = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads from the underlying stream in little endian format. Advancing the position.
        /// </summary>
        /// <param name="buffer">the pointer to write the data to</param>
        /// <param name="length">the number of bytes to read</param>
        /// <returns>The data read</returns>
        public void ReadAll(byte* buffer, int length)
        {
            for (int x = 0; x < length; x++)
            {
                buffer[x] = ReadUInt8();
            }
        }

        /// <summary>
        /// Reads all of the provided bytes. Will not return prematurely, 
        /// but continue to execute a <see cref="Read"/> command until the entire
        /// <see cref="length"/> has been read.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <exception cref="EndOfStreamException">occurs if the end of the stream has been reached.</exception>
        public void ReadAll(byte[] buffer, int position, int length)
        {
            buffer.ValidateParameters(position, length);
            while (length > 0)
            {
                int bytesRead = Read(buffer, position, length);
                if (bytesRead == 0)
                    throw new EndOfStreamException();
                length -= bytesRead;
                position += bytesRead;
            }
        }

        #endregion

        /// <summary>
        /// Releases all the resources used by the <see cref="BinaryStreamBase"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="BinaryStreamBase"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

    }
}