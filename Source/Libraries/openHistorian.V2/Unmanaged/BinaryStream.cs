//******************************************************************************************************
//  BinaryStream.cs - Gbtc
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
//  4/6/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Runtime.InteropServices;

namespace openHistorian.V2.Unmanaged
{
    public unsafe class BinaryStream : IDisposable
    {

        #region [ Members ]

        bool m_disposed;
        /// <summary>
        /// The position that corresponds to the first byte in the buffer.
        /// </summary>
        long m_firstPosition;
        /// <summary>
        /// Contains the position for the last position
        /// </summary>
        long m_lastPosition;
        /// <summary>
        /// the current position data.
        /// </summary>
        byte* m_current;
        /// <summary>
        /// the first position of the block
        /// </summary>
        byte* m_first;
        /// <summary>
        /// one past the last address for reading
        /// </summary>
        byte* m_lastRead;
        /// <summary>
        /// one past the last address for writing
        /// </summary>
        byte* m_lastWrite;

        int m_mainIoSession;

        int m_secondaryIoSession;

        ISupportsBinaryStream m_stream;

        byte[] m_temp;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a <see cref="BinaryStream"/> that is at position 0 of the provided stream.
        /// </summary>
        /// <param name="stream">The base stream to use.</param>
        public BinaryStream(ISupportsBinaryStream stream)
        {
            m_temp = new byte[16];
            m_stream = stream;
            m_firstPosition = 0;
            m_current = null;
            m_first = null;
            m_lastRead = null;
            m_lastWrite = null;
            if (stream.RemainingSupportedIoSessions < 1)
                throw new Exception("Stream has run out of read sessions");
            m_mainIoSession = stream.GetNextIoSession();
            if (m_mainIoSession < 0)
                throw new Exception("Invalid Session ID");
            if (stream.RemainingSupportedIoSessions >= 1)
            {
                m_secondaryIoSession = stream.GetNextIoSession();
                if (m_secondaryIoSession < 0)
                    throw new Exception("Invalid Session ID");
            }
            m_stream.StreamDisposed += OnStreamDisposed;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="BinaryStream"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~BinaryStream()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if the binary stream can be cloned.  
        /// Since a base stream may only be able to support a definate 
        /// number of concurrent IO Sessions, this should be analized
        /// before cloning the stream.
        /// </summary>
        public bool SupportsAnotherClone
        {
            get
            {
                return m_stream.RemainingSupportedIoSessions > 0;
            }
        }

        /// <summary>
        /// Gets/Sets the current position for the stream.
        /// <remarks>It is important to use this to Get/Set the position from the underlying stream since 
        /// this class buffers the results of the query.  Setting this field does not gaurentee that
        /// the underlying stream will get set. Call FlushToUnderlyingStream to acomplish this.</remarks>
        /// </summary>
        public long Position
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

        /// <summary>
        /// Returns the number of bytes available at the end of the stream.
        /// </summary>
        long RemainingReadLength
        {
            get
            {
                return (m_lastRead - m_current);
            }
        }
        /// <summary>
        /// Returns the number of bytes available at the end of the stream for writing purposes.
        /// </summary>
        long RemainingWriteLength
        {
            get
            {
                return (m_lastWrite - m_current);
            }
        }
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Clones a binary stream if it is supported.  Check <see cref="SupportsAnotherClone"/> before calling this method.
        /// </summary>
        /// <returns></returns>
        public BinaryStream CloneStream()
        {
            if (!SupportsAnotherClone)
                throw new Exception("Base stream does not support additional clones");
            return new BinaryStream(m_stream);
        }


        /// <summary>
        /// Aquires the underlying block if direct read/write access is desired to increase speed.
        /// However, once the position of this stream has been moved, this block is no longer valid.
        /// So be careful.
        /// </summary>
        /// <param name="isWriting">determines if writing access is desired.</param>
        /// <param name="buffer">the pointer to the first byte of the structure</param>
        /// <param name="currentIndex">the index of the current position</param>
        /// <param name="validLength">the length of the block</param>
        public void GetRawDataBlock(bool isWriting, out byte* buffer, out int currentIndex, out int validLength)
        {
            if (isWriting)
            {
                if (RemainingWriteLength <= 0)
                    UpdateLocalBuffer(true);
                buffer = m_first;
                currentIndex = (int)(m_current - m_first);
                validLength = (int)RemainingWriteLength;
            }
            else
            {
                if (RemainingReadLength <= 0)
                    UpdateLocalBuffer(false);
                buffer = m_first;
                currentIndex = (int)(m_current - m_first);
                validLength = (int)RemainingReadLength;
            }
        }

        /// <summary>
        /// Updates the local buffer data.
        /// </summary>
        /// <param name="isWriting">hints to the stream if write access is desired.</param>
        public void UpdateLocalBuffer(bool isWriting)
        {
            //If the block block is already looked up, skip this step.
            if (isWriting && RemainingWriteLength > 0 || !isWriting && RemainingReadLength > 0)
                return;

            IntPtr buffer;
            long position = Position;
            bool supportsWrite;
            int validLength;

            m_stream.GetBlock(m_mainIoSession, position, isWriting, out buffer, out m_firstPosition, out validLength, out supportsWrite);
            m_first = (byte*)buffer;
            m_lastRead = m_first + validLength;
            m_current = m_first + (position - m_firstPosition);
            m_lastPosition = m_firstPosition + validLength;

            if (supportsWrite)
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
        public void Copy(long source, long destination, int length)
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

            bool sourceAndDesinationOverlap = (source < destination && source + length >= destination);
            bool containsSource = (length <= RemainingReadLength);
            bool containsDestination = (m_firstPosition <= destination && destination + length < m_lastPosition);

            if (containsSource && containsDestination)
            {
                UpdateLocalBuffer(true);

                byte* src = m_current;
                byte* dst = m_current + (destination - source);

                if (sourceAndDesinationOverlap)
                    Memory.CopyWinApi(src, dst, length);
                else
                    MemoryMethod.MemCpy.Invoke(dst,src,(uint)length);
                return;
            }

            if (m_secondaryIoSession >= 0)
            {
                IntPtr prt;
                long pos;
                int secLength;
                bool supportsWrite;
                m_stream.GetBlock(m_secondaryIoSession, destination, true, out prt, out pos, out secLength, out supportsWrite);
                containsDestination = (pos <= destination && destination + length < pos + secLength);

                if (containsSource && containsDestination)
                {
                    byte* src = m_current;
                    byte* dst = (byte*)prt + (destination - pos);

                    if (sourceAndDesinationOverlap)
                        Memory.CopyWinApi(src, dst, length);
                    else
                        MemoryMethod.MemCpy.Invoke(dst, src, (uint)length);
                    
                    return;
                }

            }

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
        public void InsertBytes(int numberOfBytes, int lengthOfValidDataToShift)
        {
            Copy(Position, Position + numberOfBytes, lengthOfValidDataToShift);
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
            Copy(Position + numberOfBytes, Position, lengthOfValidDataToShift);
        }

        //#region Helper Types

        //public void Write(long value1, long value2)
        //{
        //    const int size = 16;
        //    if (m_lastIndexWrite - m_currentIndex >= size - 1)
        //    {
        //        *(long*)(m_buffer + m_currentIndex) = value1;
        //        *(long*)(m_buffer + m_currentIndex + 8) = value2;
        //        m_currentIndex += size;
        //        return;
        //    }
        //    Write2(value1);
        //    Write2(value2);
        //}

        //#endregion

        #region Derived Types
        public void Write(sbyte value)
        {
            Write((byte)value);
        }
        public void Write(bool value)
        {
            if (value)
                Write((byte)1);
            else
                Write((byte)0);
        }
        public void Write(ushort value)
        {
            Write((short)value);
        }
        public void Write(uint value)
        {
            Write((int)value);
        }
        public void Write(ulong value)
        {
            Write((long)value);
        }
        #endregion

        #region Core Types

        public void Write(byte value)
        {
            const int size = sizeof(byte);
            byte* cur = m_current;
            if (cur < m_lastWrite)
            {
                *cur = value;
                m_current = cur + size;
                return;
            }
            Write2(value);
        }
        void Write2(byte value)
        {
            const int size = sizeof(byte);
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                *m_current = value;
                m_current += size;
                return;
            }
            m_temp[0] = value;
            Write(m_temp, 0, size);
        }
        public void Write(short value)
        {
            const int size = sizeof(short);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(short*)cur = value;
                m_current = cur + size;
                return;
            }
            Write2(value);
        }
        void Write2(short value)
        {
            const int size = sizeof(short);
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                *(short*)m_current = value;
                m_current += size;
                return;
            }
            m_temp[0] = (byte)value;
            m_temp[1] = (byte)(value >> 8);
            Write(m_temp, 0, size);
        }
        public void Write(int value)
        {
            const int size = sizeof(int);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(int*)cur = value;
                m_current = cur + size;
                return;
            }
            Write2(value);
        }
        void Write2(int value)
        {
            const int size = sizeof(int);
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                *(int*)m_current = value;
                m_current = m_current + size;
                return;
            }
            fixed (byte* lp = m_temp)
            {
                *(int*)lp = value;
            }
            Write(m_temp, 0, size);
        }
        public void Write(float value)
        {
            const int size = sizeof(float);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(float*)cur = value;
                m_current = cur + size;
                return;
            }
            Write2(value);
        }
        void Write2(float value)
        {
            const int size = sizeof(float);
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                *(float*)m_current = value;
                m_current = m_current + size;
                return;
            }
            fixed (byte* lp = m_temp)
            {
                *(float*)lp = value;
            }
            Write(m_temp, 0, size);
        }
        public void Write(long value)
        {
            const int size = sizeof(long);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(long*)cur = value;
                m_current = cur + size;
                return;
            }
            Write2(value);
        }
        void Write2(long value)
        {
            const int size = sizeof(long);
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                *(long*)m_current = value;
                m_current = m_current + size;
                return;
            }
            fixed (byte* lp = m_temp)
            {
                *(long*)lp = value;
            }
            Write(m_temp, 0, size);
        }
        public void Write(double value)
        {
            const int size = sizeof(double);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(double*)cur = value;
                m_current = cur + size;
                return;
            }
            Write2(value);
        }
        void Write2(double value)
        {
            const int size = sizeof(double);
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                *(double*)m_current = value;
                m_current = m_current + size;
                return;
            }
            fixed (byte* lp = m_temp)
            {
                *(double*)lp = value;
            }
            Write(m_temp, 0, size);
        }
        public void Write(DateTime value)
        {
            const int size = 8;
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(DateTime*)cur = value;
                m_current = cur + size;
                return;
            }
            Write2(value);
        }
        void Write2(DateTime value)
        {
            const int size = 8;
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                *(DateTime*)m_current = value;
                m_current = m_current + size;
                return;
            }
            fixed (byte* lp = m_temp)
            {
                *(DateTime*)lp = value;
            }
            Write(m_temp, 0, size);
        }
        public void Write(decimal value)
        {
            const int size = sizeof(decimal);
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(decimal*)cur = value;
                m_current = cur + size;
                return;
            }
            Write2(value);
        }
        void Write2(decimal value)
        {
            const int size = sizeof(decimal);
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                *(decimal*)m_current = value;
                m_current = m_current + size;
                return;
            }
            fixed (byte* lp = m_temp)
            {
                *(decimal*)lp = value;
            }
            Write(m_temp, 0, size);
        }
        public void Write(Guid value)
        {
            const int size = 16;
            byte* cur = m_current;
            if (cur + size <= m_lastWrite)
            {
                *(Guid*)cur = value;
                m_current = cur + size;
                return;
            }
            Write2(value);
        }
        void Write2(Guid value)
        {
            const int size = 16;
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                *(Guid*)m_current = value;
                m_current = m_current + size;
                return;
            }
            fixed (byte* lp = m_temp)
            {
                *(Guid*)lp = value;
            }
            Write(m_temp, 0, size);
        }
        public void Write7Bit(uint value)
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
            Write7Bit2(value);
        }
        void Write7Bit2(uint value)
        {
            int size = Compression.Get7BitSize(value);
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                int index = 0;
                Compression.Write7Bit(m_current, ref index, value);
                m_current += index;
                return;
            }
            int pos = 0;
            Compression.Write7Bit(m_temp, ref pos, value);
            Write(m_temp, 0, size);
        }
        public void Write7Bit(ulong value)
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
            Write7Bit2(value);
        }
        void Write7Bit2(ulong value)
        {
            int size = Compression.Get7BitSize(value);
            if (RemainingWriteLength <= 0)
                UpdateLocalBuffer(true);
            if (RemainingWriteLength >= size)
            {
                int index = 0;
                Compression.Write7Bit(m_current, ref index, value);
                m_current += index;
                return;
            }
            int pos = 0;
            Compression.Write7Bit(m_temp, ref pos, value);
            Write(m_temp, 0, size);
        }
        public void Write(byte[] value, int offset, int count)
        {
            if (m_current + count <= m_lastWrite)
            {
                Marshal.Copy(value, offset, (IntPtr)m_current, count);
                m_current += count;
                return;
            }
            Write2(value, offset, count);
        }
        void Write2(byte[] value, int offset, int count)
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

        //#region Helper Types
        //public void ReadInt128(out long value1, out long value2)
        //{
        //    const int size = 16;
        //    if (m_lastIndex - m_currentIndex >= size - 1)
        //    {
        //        value1 = *(long*)(m_buffer + m_currentIndex);
        //        value2 = *(long*)(m_buffer + m_currentIndex + 8);
        //        m_currentIndex += size;
        //        return;
        //    }
        //    value1 = ReadInt642();
        //    value2 = ReadInt642();
        //}
        //#endregion

        #region Derived Types

        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        public bool ReadBoolean()
        {
            return (ReadByte() != 0);
        }
        public ushort ReadUInt16()
        {
            return (ushort)ReadInt16();
        }
        public uint ReadUInt32()
        {
            return (uint)ReadInt32();
        }
        public ulong ReadUInt64()
        {
            return (ulong)ReadInt64();
        }
        #endregion

        #region Core Types
        public byte ReadByte()
        {
            const int size = sizeof(byte);
            if (m_current < m_lastRead)
            {
                byte value = *m_current;
                m_current += size;
                return value;
            }
            return ReadByte2();
        }
        byte ReadByte2()
        {
            const int size = sizeof(byte);
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength >= size)
            {
                byte value = *m_current;
                m_current += size;
                return value;
            }
            Read(m_temp, 0, size);
            return m_temp[0];
        }

        public short ReadInt16()
        {
            const int size = sizeof(short);
            if (m_current + size <= m_lastRead)
            {
                short value = *(short*)m_current;
                m_current += size;
                return value;
            }
            return ReadInt162();
        }
        short ReadInt162()
        {
            const int size = sizeof(short);
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength >= size)
            {
                short value = *(short*)m_current;
                m_current += size;
                return value;
            }
            Read(m_temp, 0, size);
            return (short)(m_temp[0] | (m_temp[1] << 8));
        }
        public int ReadInt32()
        {
            const int size = sizeof(int);
            if (m_current + size <= m_lastRead)
            {
                int value = *(int*)m_current;
                m_current += size;
                return value;
            }
            return ReadInt322();
        }
        int ReadInt322()
        {
            const int size = sizeof(int);
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength >= size)
            {
                int value = *(int*)m_current;
                m_current += size;
                return value;
            }
            Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(int*)(lp);
            }
        }
        public float ReadSingle()
        {
            const int size = sizeof(float);
            if (m_current + size <= m_lastRead)
            {
                float value = *(float*)m_current;
                m_current += size;
                return value;
            }
            return ReadSingle2();
        }
        float ReadSingle2()
        {
            const int size = sizeof(float);
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength >= size)
            {
                float value = *(float*)m_current;
                m_current += size;
                return value;
            }
            Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(float*)(lp);
            }
        }
        public long ReadInt64()
        {
            const int size = sizeof(long);
            if (m_current + size <= m_lastRead)
            {
                long value = *(long*)m_current;
                m_current += size;
                return value;
            }
            return ReadInt642();
        }
        long ReadInt642()
        {
            const int size = sizeof(long);
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength >= size)
            {
                long value = *(long*)m_current;
                m_current += size;
                return value;
            }
            Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(long*)(lp);
            }
        }
        public double ReadDouble()
        {
            const int size = sizeof(double);
            if (m_current + size <= m_lastRead)
            {
                double value = *(double*)m_current;
                m_current += size;
                return value;
            }
            return ReadDouble2();
        }
        double ReadDouble2()
        {
            const int size = sizeof(double);
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength >= size)
            {
                double value = *(double*)m_current;
                m_current += size;
                return value;
            }
            Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(double*)(lp);
            }
        }
        public DateTime ReadDateTime()
        {
            const int size = 8;
            if (m_current + size <= m_lastRead)
            {
                DateTime value = *(DateTime*)m_current;
                m_current += size;
                return value;
            }
            return ReadDateTime2();
        }
        DateTime ReadDateTime2()
        {
            const int size = 8;
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength >= size)
            {
                DateTime value = *(DateTime*)m_current;
                m_current += size;
                return value;
            }
            Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(DateTime*)(lp);
            }
        }
        public decimal ReadDecimal()
        {
            const int size = sizeof(decimal);
            if (m_current + size <= m_lastRead)
            {
                decimal value = *(decimal*)m_current;
                m_current += size;
                return value;
            }
            return ReadDecimal2();
        }
        decimal ReadDecimal2()
        {
            const int size = sizeof(decimal);
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength >= size)
            {
                decimal value = *(decimal*)m_current;
                m_current += size;
                return value;
            }
            Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(decimal*)(lp);
            }
        }
        public Guid ReadGuid()
        {
            const int size = 16;
            if (m_current + size <= m_lastRead)
            {
                Guid value = *(Guid*)m_current;
                m_current += size;
                return value;
            }
            return ReadGuid2();
        }
        Guid ReadGuid2()
        {
            const int size = 16;
            if (RemainingReadLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingReadLength >= size)
            {
                Guid value = *(Guid*)m_current;
                m_current += size;
                return value;
            }
            Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(Guid*)(lp);
            }
        }

        public uint Read7BitUInt32()
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
            return Read7BitUInt322();
        }
        uint Read7BitUInt322()
        {
            uint value11;
            value11 = ReadByte();
            if (value11 < 128)
            {
                return value11;
            }
            value11 ^= ((uint)ReadByte() << 7);
            if (value11 < 128 * 128)
            {
                return value11 ^ 0x80;
            }
            value11 ^= ((uint)ReadByte() << 14);
            if (value11 < 128 * 128 * 128)
            {
                return value11 ^ 0x4080;
            }
            value11 ^= ((uint)ReadByte() << 21);
            if (value11 < 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x204080;
            }
            value11 ^= ((uint)ReadByte() << 28) ^ 0x10204080;
            return value11;
        }

        public ulong Read7BitUInt64()
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
            return Read7BitUInt642();
        }
        ulong Read7BitUInt642()
        {
            ulong value11 = ReadByte();
            if (value11 < 128)
            {
                return value11;
            }
            value11 ^= ((ulong)ReadByte() << 7);
            if (value11 < 128 * 128)
            {
                return value11 ^ 0x80;
            }
            value11 ^= ((ulong)ReadByte() << 14);
            if (value11 < 128 * 128 * 128)
            {
                return value11 ^ 0x4080;
            }
            value11 ^= ((ulong)ReadByte() << 21);
            if (value11 < 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x204080;
            }
            value11 ^= ((ulong)ReadByte() << (7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x10204080L;
            }
            value11 ^= ((ulong)ReadByte() << (7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x810204080L;
            }
            value11 ^= ((ulong)ReadByte() << (7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x40810204080L;
            }
            value11 ^= ((ulong)ReadByte() << (7 + 7 + 7 + 7 + 7 + 7 + 7));
            if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
            {
                return value11 ^ 0x2040810204080L;
            }
            value11 ^= ((ulong)ReadByte() << (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
            return value11 ^ 0x102040810204080L;
        }

        public int Read(byte[] value, int offset, int count)
        {
            if (RemainingReadLength >= count)
            {
                Marshal.Copy((IntPtr)(m_current), value, offset, count);
                m_current += count;
                return count;
            }
            return Read2(value, offset, count);
        }
        int Read2(byte[] value, int offset, int count)
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="BinaryStream"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    m_firstPosition = 0;
                    m_lastPosition = 0;
                    m_current = null;
                    m_first = null;
                    m_lastRead = null;
                    m_lastWrite = null;

                    // This will be done regardless of whether the object is finalized or disposed.
                    if (m_mainIoSession >= 0)
                        m_stream.ReleaseIoSession(m_mainIoSession);
                    if (m_secondaryIoSession >= 0)
                        m_stream.ReleaseIoSession(m_secondaryIoSession);
                    m_mainIoSession = -1;
                    m_secondaryIoSession = -1;

                    m_stream.StreamDisposed -= OnStreamDisposed;

                    m_stream = null;

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

        void OnStreamDisposed(object sender, EventArgs e)
        {
            Dispose();
        }

        #endregion


    }
}
