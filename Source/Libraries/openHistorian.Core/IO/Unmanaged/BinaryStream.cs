//******************************************************************************************************
//  BinaryStream.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  4/6/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Runtime.InteropServices;
using GSF.UnmanagedMemory;

namespace GSF.IO.Unmanaged
{
    public unsafe class BinaryStream 
        : BinaryStreamBase
    {
        #region [ Members ]

        private readonly BlockArguments m_args;

        /// <summary>
        /// Determines if this class owns the underlying stream, thus when Dispose() is called
        /// the dispose of the underlying stream will also be called.
        /// </summary>
        private readonly bool m_leaveOpen;

        private bool m_disposed;

        /// <summary>
        /// The position that corresponds to the first byte in the buffer.
        /// </summary>
        private long m_firstPosition;

        /// <summary>
        /// Contains the position for the last position
        /// </summary>
        private long m_lastPosition;

        /// <summary>
        /// the current position data.
        /// </summary>
        private byte* m_current;

        /// <summary>
        /// the first position of the block
        /// </summary>
        private byte* m_first;

        /// <summary>
        /// one past the last address for reading
        /// </summary>
        private byte* m_lastRead;

        /// <summary>
        /// one past the last address for writing
        /// </summary>
        private byte* m_lastWrite;

        private BinaryStreamIoSessionBase m_mainIoSession;

        private BinaryStreamIoSessionBase m_secondaryIoSession;

        private ISupportsBinaryStream m_stream;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a <see cref="BinaryStream"/> that is in memory only.
        /// </summary>
        public BinaryStream()
            : this(new MemoryPoolStream(), false)
        {
        }

        /// <summary>
        /// Creates a <see cref="BinaryStream"/> that is in memory only.
        /// </summary>
        public BinaryStream(MemoryPool pool)
            : this(new MemoryPoolStream(pool), false)
        {
        }

        /// <summary>
        /// Creates a <see cref="BinaryStream"/> that is in memory only.
        /// </summary>
        /// <param name="allocatesOwnMemory">true to allowcate its own memory rather than using the <see cref="MemoryPool"/>.</param>
        public BinaryStream(bool allocatesOwnMemory)
            : this(CreatePool(allocatesOwnMemory))
        {

        }

        static ISupportsBinaryStream CreatePool(bool allocatesOwnMemory)
        {
            if (allocatesOwnMemory)
                return new UnmanagedMemoryStream();
            else
                return new MemoryPoolStream();
        }

        /// <summary>
        /// Creates a <see cref="BinaryStream"/> that is at position 0 of the provided stream.
        /// </summary>
        /// <param name="stream">The base stream to use.</param>
        /// <param name="leaveOpen">Determines if the underlying stream will automatically be 
        /// disposed of when this class has it's dispose method called.</param>
        public BinaryStream(ISupportsBinaryStream stream, bool leaveOpen = true)
            : base()
        {
            m_args = new BlockArguments();
            m_leaveOpen = leaveOpen;
            m_stream = stream;
            m_firstPosition = 0;
            m_current = null;
            m_first = null;
            m_lastRead = null;
            m_lastWrite = null;
            if (stream.RemainingSupportedIoSessions < 1)
                throw new Exception("Stream has run out of read sessions");
            m_mainIoSession = stream.CreateIoSession();
            //if (stream.RemainingSupportedIoSessions >= 1)
            //    m_secondaryIoSession = stream.CreateIoSession();
        }

        #endregion

        #region [ Properties ]

        public ISupportsBinaryStream BaseStream
        {
            get
            {
                return m_stream;
            }
        }

        /// <summary>
        /// Determines if the binary stream can be cloned.  
        /// Since a base stream may only be able to support a definate 
        /// number of concurrent IO Sessions, this should be analized
        /// before cloning the stream.
        /// </summary>
        public override bool SupportsAnotherClone
        {
            get
            {
                return m_stream.RemainingSupportedIoSessions > 0 || m_secondaryIoSession != null;
            }
        }

        /// <summary>
        /// Gets/Sets the current position for the stream.
        /// <remarks>It is important to use this to Get/Set the position from the underlying stream since 
        /// this class buffers the results of the query.  Setting this field does not gaurentee that
        /// the underlying stream will get set. Call FlushToUnderlyingStream to acomplish this.</remarks>
        /// </summary>
        public override long Position
        {
            get
            {
                return m_firstPosition + (m_current - m_first);
            }
            set
            {
                if (m_firstPosition <= value && value < m_lastPosition)
                {
                    m_current = m_first + (value - m_firstPosition);
                }
                else
                {
                    m_firstPosition = value;
                    m_lastPosition = value;
                    m_current = null;
                    m_first = null;
                    m_lastRead = null;
                    m_lastWrite = null;
                }
            }
        }

        private long GetPosition()
        {
            return m_firstPosition + (m_current - m_first);
        }

        private void SetPosition(long value)
        {
            if (m_firstPosition <= value && value < m_lastPosition)
            {
                m_current = m_first + (value - m_firstPosition);
            }
            else
            {
                m_firstPosition = value;
                m_lastPosition = value;
                m_current = null;
                m_first = null;
                m_lastRead = null;
                m_lastWrite = null;
            }
        }

        /// <summary>
        /// Returns the number of bytes available at the end of the stream.
        /// </summary>
        private long RemainingReadLength
        {
            get
            {
                return (m_lastRead - m_current);
            }
        }

        /// <summary>
        /// Returns the number of bytes available at the end of the stream for writing purposes.
        /// </summary>
        private long RemainingWriteLength
        {
            get
            {
                return (m_lastWrite - m_current);
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// When accessing the underlying stream, a lock is placed on the data. Calling this method clears that lock.
        /// </summary>
        public void ClearLocks()
        {
            m_firstPosition = Position;
            m_lastPosition = m_firstPosition;
            m_current = null;
            m_first = null;
            m_lastRead = null;
            m_lastWrite = null;

            if (m_mainIoSession != null)
                m_mainIoSession.Clear();
            if (m_secondaryIoSession != null)
                m_secondaryIoSession.Clear();
        }

        /// <summary>
        /// Clones a binary stream if it is supported.  Check <see cref="SupportsAnotherClone"/> before calling this method.
        /// </summary>
        /// <returns></returns>
        public override BinaryStreamBase CloneStream()
        {
            if (!SupportsAnotherClone)
                throw new Exception("Base stream does not support additional clones");

            return new BinaryStream(m_stream, false);
        }

        public override byte* GetWritePointer(long position, int length)
        {
            Position = position;
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength < length)
                throw new Exception("Cannot get the provided length.");
            return m_current;
        }

        public override byte* GetReadPointer(long position, int length)
        {
            SetPosition(position);
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength < length)
                throw new Exception("Cannot get the provided length.");
            return m_current;
        }

        public override byte* GetReadPointer(long position, int length, out bool supportsWriting)
        {
            Position = position;
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength < length)
                throw new Exception("Cannot get the provided length.");
            supportsWriting = (RemainingWriteLength >= length);
            return m_current;
        }

        /// <summary>
        /// Updates the local buffer data.
        /// </summary>
        /// <param name="isWriting">hints to the stream if write access is desired.</param>
        public override void UpdateLocalBuffer(bool isWriting)
        {
            //If the block block is already looked up, skip this step.
            if (isWriting && RemainingWriteLength > 0 || !isWriting && RemainingReadLength > 0)
                return;

            long position = GetPosition();
            m_args.Position = position;
            m_args.IsWriting = isWriting;
            PointerVersion++;
            m_mainIoSession.GetBlock(m_args);
            m_firstPosition = m_args.FirstPosition;
            m_first = (byte*)m_args.FirstPointer;
            m_lastRead = m_first + m_args.Length;
            m_current = m_first + (position - m_firstPosition);
            m_lastPosition = m_firstPosition + m_args.Length;

            if (m_args.SupportsWriting)
                m_lastWrite = m_lastRead;
            else
                m_lastWrite = m_first;
        }

        #region Writing

        /// <summary>
        /// Copies a specified number of bytes to a new location
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="length"></param>
        public override void Copy(long source, long destination, int length)
        {
            if (source < 0)
                throw new ArgumentException("value cannot be less than zero", "source");
            if (destination < 0)
                throw new ArgumentException("value cannot be less than zero", "destination");
            if (length < 0)
                throw new ArgumentException("value cannot be less than zero", "length");

            if (length == 0 || source == destination)
                return;

            Position = source;
            UpdateLocalBuffer(false);

            bool containsSource = (length <= RemainingReadLength);
            bool containsDestination = (m_firstPosition <= destination && destination + length < m_lastPosition);

            if (containsSource && containsDestination)
            {
                UpdateLocalBuffer(true);

                byte* src = m_current;
                byte* dst = m_current + (destination - source);

                Memory.Copy(src, dst, length);
                return;
            }

            //if (m_secondaryIoSession != null)
            //{
            //    IntPtr prt;
            //    long pos;
            //    int secLength;
            //    bool supportsWrite;
            //    m_secondaryIoSession.GetBlock(destination, true, out prt, out pos, out secLength, out supportsWrite);
            //    containsDestination = (pos <= destination && destination + length < pos + secLength);

            //    if (containsSource && containsDestination)
            //    {
            //        byte* src = m_current;
            //        byte* dst = (byte*)prt + (destination - pos);

            //        Memory.Copy(src, dst, length);

            //        return;
            //    }

            //}

            //manually perform the copy
            byte[] data = new byte[length];
            Position = source;
            Read(data, 0, data.Length);
            Position = destination;
            Write(data, 0, data.Length);
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
        public override void InsertBytes(int numberOfBytes, int lengthOfValidDataToShift)
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
        public override void RemoveBytes(int numberOfBytes, int lengthOfValidDataToShift)
        {
            long pos = Position;
            Copy(Position + numberOfBytes, Position, lengthOfValidDataToShift);
            Position = pos;
        }

        #region Core Types

        public override void Write(byte value)
        {
            const int size = sizeof(byte);
            byte* cur = m_current;
            if (cur < m_lastWrite)
            {
                *cur = value;
                m_current = cur + size;
                return;
            }
            base.Write(value);
        }

        public override void Write(short value)
        {
            const int size = sizeof(short);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(short*)cur = value;
                m_current = cur + size;
                return;
            }
            base.Write(value);
        }

        public override void Write(int value)
        {
            const int size = sizeof(int);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(int*)cur = value;
                m_current = cur + size;
                return;
            }
            base.Write(value);
        }

        public override void Write(float value)
        {
            const int size = sizeof(float);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(float*)cur = value;
                m_current = cur + size;
                return;
            }
            base.Write(value);
        }

        public override void Write(long value)
        {
            const int size = sizeof(long);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(long*)cur = value;
                m_current = cur + size;
                return;
            }
            base.Write(value);
        }

        public override void Write(double value)
        {
            const int size = sizeof(double);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(double*)cur = value;
                m_current = cur + size;
                return;
            }
            base.Write(value);
        }

        public override void Write(decimal value)
        {
            const int size = sizeof(decimal);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(decimal*)cur = value;
                m_current = cur + size;
                return;
            }
            base.Write(value);
        }

        public override void Write(Guid value)
        {
            const int size = 16;
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(Guid*)cur = value;
                m_current = cur + size;
                return;
            }
            base.Write(value);
        }

        public override void Write7Bit(uint value)
        {
            const int size = 5;
            byte* stream = m_current;
            if (stream + size <= m_lastWrite)
            {
                if (value < 128)
                {
                    stream[0] = (byte)value;
                    m_current += 1;
                    return;
                }
                stream[0] = (byte)(value | 128);
                if (value < 128 * 128)
                {
                    stream[1] = (byte)(value >> 7);
                    m_current += 2;
                    return;
                }
                stream[1] = (byte)((value >> 7) | 128);
                if (value < 128 * 128 * 128)
                {
                    stream[2] = (byte)(value >> 14);
                    m_current += 3;
                    return;
                }
                stream[2] = (byte)((value >> 14) | 128);
                if (value < 128 * 128 * 128 * 128)
                {
                    stream[3] = (byte)(value >> 21);
                    m_current += 4;
                    return;
                }
                stream[3] = (byte)((value >> 21) | 128);
                stream[4] = (byte)(value >> 28);
                m_current += 5;
                return;
            }
            base.Write7Bit(value);
        }

        public override void Write7Bit(ulong value)
        {
            const int size = 9;
            byte* stream = m_current;
            if (stream + size <= m_lastWrite)
            {
                if (value < 128)
                {
                    stream[0] = (byte)value;
                    m_current += 1;
                    return;
                }
                stream[0] = (byte)(value | 128);
                if (value < 128 * 128)
                {
                    stream[1] = (byte)(value >> 7);
                    m_current += 2;
                    return;
                }
                stream[1] = (byte)((value >> 7) | 128);
                if (value < 128 * 128 * 128)
                {
                    stream[2] = (byte)(value >> 14);
                    m_current += 3;
                    return;
                }
                stream[2] = (byte)((value >> 14) | 128);
                if (value < 128 * 128 * 128 * 128)
                {
                    stream[3] = (byte)(value >> 21);
                    m_current += 4;
                    return;
                }
                stream[3] = (byte)((value >> (7 + 7 + 7)) | 128);
                if (value < 128L * 128 * 128 * 128 * 128)
                {
                    stream[4] = (byte)(value >> (7 + 7 + 7 + 7));
                    m_current += 5;
                    return;
                }
                stream[4] = (byte)((value >> (7 + 7 + 7 + 7)) | 128);
                if (value < 128L * 128 * 128 * 128 * 128 * 128)
                {
                    stream[5] = (byte)(value >> (7 + 7 + 7 + 7 + 7));
                    m_current += 6;
                    return;
                }
                stream[5] = (byte)((value >> (7 + 7 + 7 + 7 + 7)) | 128);
                if (value < 128L * 128 * 128 * 128 * 128 * 128 * 128)
                {
                    stream[6] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7));
                    m_current += 7;
                    return;
                }
                stream[6] = (byte)((value >> (7 + 7 + 7 + 7 + 7 + 7)) | 128);
                if (value < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
                {
                    stream[7] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7 + 7));
                    m_current += 8;
                    return;
                }
                stream[7] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7 + 7) | 128);
                stream[8] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
                m_current += 9;
                return;
            }
            base.Write7Bit(value);
        }

        public override void Write(byte* buffer, int length)
        {
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength < length)
            {
                base.Write(buffer, length);
                return;
            }

            int pos = 0;
            while (pos + 8 <= length)
            {
                *(long*)(m_current + pos) = *(long*)(buffer + pos);
                pos += 8;
            }
            if (pos + 4 <= length)
            {
                *(int*)(m_current + pos) = *(int*)(buffer + pos);
                pos += 4;
            }
            if (pos + 2 <= length)
            {
                *(short*)(m_current + pos) = *(short*)(buffer + pos);
                pos += 2;
            }
            if (pos + 1 <= length)
            {
                *(m_current + pos) = *(buffer + pos);
            }
            m_current += length;
        }

        public override void Write(byte[] value, int offset, int count)
        {
            if (m_current + count <= m_lastWrite)
            {
                Marshal.Copy(value, offset, (IntPtr)m_current, count);
                m_current += count;
                return;
            }
            Write2(value, offset, count);
        }

        private void Write2(byte[] value, int offset, int count)
        {
            while (count > 0)
            {
                if (RemainingWriteLength <= 0)
                    UpdateLocalBuffer(true);
                int availableLength = Math.Min((int)RemainingWriteLength, count);

                Marshal.Copy(value, offset, (IntPtr)m_current, availableLength);
                m_current += availableLength;

                count -= availableLength;
                offset += availableLength;
            }
        }

        #endregion

        #endregion

        #region Reading

        #region Derived Types

        public override uint ReadUInt24()
        {
            uint value;
            if (m_current + 4 <= m_lastRead)
            {
                value = (*(uint*)m_current) & 0xFFFFFFu;
                m_current += 3;
                return value;
            }
            value = ReadUInt16();
            return value | ((uint)ReadByte() << 16);
        }

        public override ulong ReadUInt40()
        {
            ulong value;
            if (m_current + 8 <= m_lastRead)
            {
                value = (*(ulong*)m_current) & 0xFFFFFFFFFFul;
                m_current += 5;
                return value;
            }
            value = ReadUInt32();
            return value | ((ulong)ReadByte() << 32);
        }

        public override ulong ReadUInt48()
        {
            ulong value;
            if (m_current + 8 <= m_lastRead)
            {
                value = (*(ulong*)m_current) & 0xFFFFFFFFFFFFul;
                m_current += 6;
                return value;
            }
            value = ReadUInt32();
            return value | ((ulong)ReadUInt16() << 32);
        }

        public override ulong ReadUInt56()
        {
            ulong value;
            if (m_current + 8 <= m_lastRead)
            {
                value = (*(ulong*)m_current) & 0xFFFFFFFFFFFFFFul;
                m_current += 7;
                return value;
            }
            value = ReadUInt32();
            return value | ((ulong)ReadUInt24() << 32);
        }

        #endregion

        #region Core Types

        public override byte ReadByte()
        {
            const int size = sizeof(byte);
            if (m_current < m_lastRead)
            {
                byte value = *m_current;
                m_current += size;
                return value;
            }
            return base.ReadByte();
        }

        public override short ReadInt16()
        {
            const int size = sizeof(short);
            if (m_current + size <= m_lastRead)
            {
                short value = *(short*)m_current;
                m_current += size;
                return value;
            }
            return base.ReadInt16();
        }

        public override int ReadInt32()
        {
            const int size = sizeof(int);
            if (m_current + size <= m_lastRead)
            {
                int value = *(int*)m_current;
                m_current += size;
                return value;
            }
            return base.ReadInt32();
        }

        public override float ReadSingle()
        {
            const int size = sizeof(float);
            if (m_current + size <= m_lastRead)
            {
                float value = *(float*)m_current;
                m_current += size;
                return value;
            }
            return base.ReadSingle();
        }

        public override long ReadInt64()
        {
            const int size = sizeof(long);
            if (m_current + size <= m_lastRead)
            {
                long value = *(long*)m_current;
                m_current += size;
                return value;
            }
            return base.ReadInt64();
        }

        public override double ReadDouble()
        {
            const int size = sizeof(double);
            if (m_current + size <= m_lastRead)
            {
                double value = *(double*)m_current;
                m_current += size;
                return value;
            }
            return base.ReadDouble();
        }

        public override decimal ReadDecimal()
        {
            const int size = sizeof(decimal);
            if (m_current + size <= m_lastRead)
            {
                decimal value = *(decimal*)m_current;
                m_current += size;
                return value;
            }
            return base.ReadDecimal();
        }

        public override Guid ReadGuid()
        {
            const int size = 16;
            if (m_current + size <= m_lastRead)
            {
                Guid value = *(Guid*)m_current;
                m_current += size;
                return value;
            }
            return base.ReadGuid();
        }

        public override uint Read7BitUInt32()
        {
            const int size = 5;
            byte* stream = m_current;
            if (stream + size <= m_lastRead)
            {
                uint value11;
                value11 = stream[0];
                if (value11 < 128)
                {
                    m_current += 1;
                    return value11;
                }
                value11 ^= ((uint)stream[1] << 7);
                if (value11 < 128 * 128)
                {
                    m_current += 2;
                    return value11 ^ 0x80;
                }
                value11 ^= ((uint)stream[2] << 14);
                if (value11 < 128 * 128 * 128)
                {
                    m_current += 3;
                    return value11 ^ 0x4080;
                }
                value11 ^= ((uint)stream[3] << 21);
                if (value11 < 128 * 128 * 128 * 128)
                {
                    m_current += 4;
                    return value11 ^ 0x204080;
                }
                value11 ^= ((uint)stream[4] << 28) ^ 0x10204080;
                m_current += 5;
                return value11;
            }
            return base.Read7BitUInt32();
        }

        public override ulong Read7BitUInt64()
        {
            const int size = 9;
            byte* stream = m_current;
            if (stream + size <= m_lastRead)
            {
                ulong value11;
                value11 = stream[0];
                if (value11 < 128)
                {
                    m_current += 1;
                    return value11;
                }
                value11 ^= ((ulong)stream[1] << (7));
                if (value11 < 128 * 128)
                {
                    m_current += 2;
                    return value11 ^ 0x80;
                }
                value11 ^= ((ulong)stream[2] << (7 + 7));
                if (value11 < 128 * 128 * 128)
                {
                    m_current += 3;
                    return value11 ^ 0x4080;
                }
                value11 ^= ((ulong)stream[3] << (7 + 7 + 7));
                if (value11 < 128 * 128 * 128 * 128)
                {
                    m_current += 4;
                    return value11 ^ 0x204080;
                }
                value11 ^= ((ulong)stream[4] << (7 + 7 + 7 + 7));
                if (value11 < 128L * 128 * 128 * 128 * 128)
                {
                    m_current += 5;
                    return value11 ^ 0x10204080L;
                }
                value11 ^= ((ulong)stream[5] << (7 + 7 + 7 + 7 + 7));
                if (value11 < 128L * 128 * 128 * 128 * 128 * 128)
                {
                    m_current += 6;
                    return value11 ^ 0x810204080L;
                }
                value11 ^= ((ulong)stream[6] << (7 + 7 + 7 + 7 + 7 + 7));
                if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
                {
                    m_current += 7;
                    return value11 ^ 0x40810204080L;
                }
                value11 ^= ((ulong)stream[7] << (7 + 7 + 7 + 7 + 7 + 7 + 7));
                if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
                {
                    m_current += 8;
                    return value11 ^ 0x2040810204080L;
                }
                value11 ^= ((ulong)stream[8] << (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
                m_current += 9;
                return value11 ^ 0x102040810204080L;
            }
            return base.Read7BitUInt64();
        }

        public override int Read(byte[] value, int offset, int count)
        {
            if (RemainingReadLength >= count)
            {
                Marshal.Copy((IntPtr)(m_current), value, offset, count);
                m_current += count;
                return count;
            }
            return Read2(value, offset, count);
        }

        private int Read2(byte[] value, int offset, int count)
        {
            int origionalCount = count;
            while (count > 0)
            {
                if (RemainingReadLength <= 0)
                    UpdateLocalBuffer(false);
                int availableLength = Math.Min((int)RemainingReadLength, count);

                Marshal.Copy((IntPtr)m_current, value, offset, availableLength);

                m_current += availableLength;
                count -= availableLength;
                offset += availableLength;
            }
            return origionalCount;
        }

        #endregion

        #endregion

        /// <summary>
        /// Releases all the resources used by the <see cref="BinaryStream"/> object.
        /// </summary>
        public override void Dispose()
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (m_mainIoSession != null)
                        m_mainIoSession.Dispose();
                    if (m_secondaryIoSession != null)
                        m_secondaryIoSession.Dispose();
                    if (!m_leaveOpen && m_stream != null)
                        m_stream.Dispose();
                }
                finally
                {
                    m_firstPosition = 0;
                    m_lastPosition = 0;
                    m_current = null;
                    m_first = null;
                    m_lastRead = null;
                    m_lastWrite = null;
                    m_mainIoSession = null;
                    m_secondaryIoSession = null;
                    m_stream = null;
                    m_disposed = true;
                }
            }
        }

        #endregion
    }
}