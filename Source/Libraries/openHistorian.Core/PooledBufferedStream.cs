//******************************************************************************************************
//  PooledBufferedStream.cs - Gbtc
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
//  1/26/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//*****************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;

namespace openHistorian.Core
{
    public unsafe class PooledBufferStream
    {
        /// <summary>
        /// The number of bits in the page size.
        /// </summary>
        const int ShiftLength = 12;
        /// <summary>
        /// The mask that can be used to Logical AND the position to get the relative position within the page.
        /// </summary>
        const int OffsetMask = Length - 1;
        /// <summary>
        /// The size of each page.
        /// </summary>
        const int Length = 4096;

        /// <summary>
        /// A temporary buffer that will be used if there is not enough space at the end of a page
        /// to read/write the ValueType.  The current size is 16 bytes.
        /// </summary>
        private byte[] m_buffer;

        /// <summary>
        /// The byte position in the stream
        /// </summary>
        private long m_position;

        /// <summary>
        /// The largest position represented in this buffer stream.
        /// </summary>
        private long m_HeadPosition;

        /// <summary>
        /// The lowest position that can still be accessed that hasn't been flushed to the underlying stream.
        /// </summary>
        private long m_TailPosition;

        /// <summary>
        /// The list of all pages in the stream
        /// </summary>
        private ContinuousBuffer<byte[]> m_stream;

        /// <summary>
        /// The stream to write the data to upon a flush.
        /// </summary>
        private Stream m_BaseStream;

        /// <summary>
        /// The number of bytes in the queue before an autoflush occurs.  -1 means never autoflush.
        /// </summary>
        private int m_AutoFlushBytes;

        /// <summary>
        /// Create a new <see cref="PooledBufferStream"/>
        /// </summary>
        public PooledBufferStream()
        {
            m_buffer = new byte[16];
            m_position = 0;
            m_HeadPosition = 0;
            m_TailPosition = 0;
            m_stream = new ContinuousBuffer<byte[]>();
        }

        /// <summary>
        /// Returns the page that corresponds to the absolute position.  
        /// This function will also autogrow the stream.
        /// </summary>
        /// <param name="position">The position to use to calculate the page to retrieve</param>
        /// <returns></returns>
        byte[] GetPage(long position)
        {
            if (position < m_TailPosition)
                throw new Exception("The position being accessed has already been flushed to the base stream and cannot be reaccessed");
            
            long page = position >> ShiftLength;

            //If there are not enough pages in the stream, add enough.
            while (page > m_stream.LastIndex)
            {
                m_stream.Add(new byte[Length]);
            }
            return m_stream[page];
        }

        /// <summary>
        /// This calculates the number of bytes remain at the end of the current page.
        /// </summary>
        /// <param name="position">The position to use to calculate the remaining bytes.</param>
        /// <returns></returns>
        int RemainingLenght(long position)
        {
            return Length - CalculateOffset(position);
        }

        /// <summary>
        /// Returns the relative offset within the page where the start of the position exists.
        /// </summary>
        /// <param name="position">The position to use to calculate the offset.</param>
        /// <returns></returns>
        int CalculateOffset(long position)
        {
            return (int)(position & OffsetMask);
        }

        /// <summary>
        /// Gets/Sets the cursor position within the stream
        /// </summary>
        public long Position
        {
            get
            {
                return m_position;
            }
            set
            {
                if (value < m_TailPosition)
                    throw new Exception("The position being accessed has already been flushed to the base stream and cannot be reaccessed");
                if (value > m_HeadPosition)
                    m_HeadPosition = value;
                m_position = value;
            }
        }

        /// <summary>
        /// Flushes all of the current data in the buffer to the underlying stream.
        /// </summary>
        public void Flush()
        {

        }
        /// <summary>
        /// Flushes all of the data to the base stream up to and including the position passed to this funciton.
        /// </summary>
        /// <param name="PositionToFlushTo">Writes all of the bytes before this position to the buffer stream.</param>
        public void Flush(long PositionToFlushTo)
        {

        }

        /// <summary>
        /// Writes the following data to the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="value">The data to write</param>
        public void Write(byte value)
        {
            Write(Position, value);
            Position++;
        }
        /// <summary>
        /// Writes the following data to the stream at the provided postion. The internal position will remain uneffected.
        /// </summary>
        /// <param name="position">The position to reference</param>
        /// <param name="value">The data to write</param>
        public void Write(long position, byte value)
        {
            int offset = CalculateOffset(position);
            fixed (byte* lp = GetPage(position))
            {
                byte* lpp = (byte*)(lp + offset);
                *lpp = value;
            }
        }
        /// <summary>
        /// Writes the following data to the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="value">The data to write</param>
        public void Write(sbyte value)
        {
            Write(Position, value);
            Position++;
        }
        /// <summary>
        /// Writes the following data to the stream at the provided postion. The internal position will remain uneffected.
        /// </summary>
        /// <param name="position">The position to reference</param>
        /// <param name="value">The data to write</param>
        public void Write(long position, sbyte value)
        {
            int offset = CalculateOffset(position);
            fixed (byte* lp = GetPage(position))
            {
                sbyte* lpp = (sbyte*)(lp + offset);
                *lpp = value;
            }
        }
        /// <summary>
        /// Writes the following data to the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="value">The data to write</param>
        public void Write(uint value)
        {
            Write(Position, value);
            Position += 4;
        }
        /// <summary>
        /// Writes the following data to the stream at the provided postion. The internal position will remain uneffected.
        /// </summary>
        /// <param name="position">The position to reference</param>
        /// <param name="value">The data to write</param>
        public void Write(long position, uint value)
        {
            if (RemainingLenght(position) < 4)
            {
                fixed (byte* lp = m_buffer)
                {
                    uint* lpp = (uint*)(lp);
                    *lpp = value;
                }
                Write(position, m_buffer, 0, 4);
            }
            else
            {
                int offset = CalculateOffset(position);
                fixed (byte* lp = GetPage(position))
                {
                    uint* lpp = (uint*)(lp + offset);
                    *lpp = value;
                }
            }
        }
        /// <summary>
        /// Writes the following data to the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="value">The data to write</param>
        public void Write(DateTime value)
        {
            Write(Position, value);
            Position += 8;
        }
        /// <summary>
        /// Writes the following data to the stream at the provided postion. The internal position will remain uneffected.
        /// </summary>
        /// <param name="position">The position to reference</param>
        /// <param name="value">The data to write</param>
        public void Write(long position, DateTime value)
        {
            if (RemainingLenght(position) < 8)
            {
                fixed (byte* lp = m_buffer)
                {
                    DateTime* lpp = (DateTime*)(lp);
                    *lpp = value;
                }
                Write(position, m_buffer, 0, 8);
            }
            else
            {
                int offset = CalculateOffset(position);
                fixed (byte* lp = GetPage(position))
                {
                    DateTime* lpp = (DateTime*)(lp + offset);
                    *lpp = value;
                }
            }
        }
        /// <summary>
        /// Writes the following data to the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="value">The data to write</param>
        public void Write(float value)
        {
            Write(Position, value);
            Position += 4;
        }
        /// <summary>
        /// Writes the following data to the stream at the provided postion. The internal position will remain uneffected.
        /// </summary>
        /// <param name="position">The position to reference</param>
        /// <param name="value">The data to write</param>
        public void Write(long position, float value)
        {
            if (RemainingLenght(position) < 4)
            {
                fixed (byte* lp = m_buffer)
                {
                    float* lpp = (float*)(lp);
                    *lpp = value;
                }
                Write(position, m_buffer, 0, 4);
            }
            else
            {
                int offset = CalculateOffset(position);
                fixed (byte* lp = GetPage(position))
                {
                    float* lpp = (float*)(lp + offset);
                    *lpp = value;
                }
            }
        }
        /// <summary>
        /// Writes the following data to the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="value">The data to write</param>
        public void Write(long value)
        {
            Write(Position, value);
            Position += 8;
        }
        /// <summary>
        /// Writes the following data to the stream at the provided postion. The internal position will remain uneffected.
        /// </summary>
        /// <param name="position">The position to reference</param>
        /// <param name="value">The data to write</param>
        public void Write(long position, long value)
        {
            if (RemainingLenght(position) < 8)
            {
                fixed (byte* lp = m_buffer)
                {
                    long* lpp = (long*)(lp);
                    *lpp = value;
                }
                Write(position, m_buffer, 0, 8);
            }
            else
            {
                int offset = CalculateOffset(position);
                fixed (byte* lp = GetPage(position))
                {
                    long* lpp = (long*)(lp + offset);
                    *lpp = value;
                }
            }
        }
        /// <summary>
        /// Writes the following data to the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="value">The data to write</param>
        public void Write(Guid value)
        {
            Write(Position, value);
            Position += 16;
        }
        /// <summary>
        /// Writes the following data to the stream at the provided postion. The internal position will remain uneffected.
        /// </summary>
        /// <param name="position">The position to reference</param>
        /// <param name="value">The data to write</param>
        public void Write(long position, Guid value)
        {
            if (RemainingLenght(position) < 16)
            {
                fixed (byte* lp = m_buffer)
                {
                    Guid* lpp = (Guid*)(lp);
                    *lpp = value;
                }
                Write(position, m_buffer, 0, 16);
            }
            else
            {
                int offset = CalculateOffset(position);
                fixed (byte* lp = GetPage(position))
                {
                    Guid* lpp = (Guid*)(lp + offset);
                    *lpp = value;
                }
            }
        }
        /// <summary>
        /// Writes the following data to the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="data">The data to write</param>
        /// <param name="offset">The position to start the write</param>
        /// <param name="count">The number of bytes to write</param>
        public void Write(byte[] data, int offset, int count)
        {
            Write(Position, data, offset, count);
            Position += count;
        }

        /// <summary>
        /// Writes the following data to the stream at the provided postion. The internal position will remain uneffected.
        /// </summary>
        /// <param name="position">The position to reference</param>
        /// <param name="data">The data to write</param>
        /// <param name="offset">The position to start the write</param>
        /// <param name="count">The number of bytes to write</param>
        public void Write(long position, byte[] data, int offset, int count)
        {
            int availableLength = RemainingLenght(position);
            int destOffset = CalculateOffset(position);
            byte[] block = GetPage(position);

            if (availableLength >= count)
            {
                Array.Copy(data, offset, block, destOffset, count);
            }
            else
            {
                Array.Copy(data, offset, block, destOffset, availableLength);
                Write(position + availableLength, data, offset + availableLength, count - availableLength);
            }
        }

        /// <summary>
        /// Reads data from the stream at the current position, advancing the position.
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            byte value = ReadByte(Position);
            Position++;
            return value;
        }

        /// <summary>
        /// Reads data from the stream at the provided position, the stream's current position is not effected.
        /// </summary>
        /// <param name="position">The position from the stream to read from.</param>
        /// <returns></returns>
        public byte ReadByte(long position)
        {
            int offset = CalculateOffset(position);
            fixed (byte* lp = GetPage(position))
            {
                byte* lpp = (byte*)(lp + offset);
                return *lpp;
            }
        }

        /// <summary>
        /// Reads data from the stream at the current position, advancing the position.
        /// </summary>
        /// <returns></returns>
        public Guid ReadGuid()
        {
            Guid value = ReadGuid(Position);
            Position += 16;
            return value;
        }

        /// <summary>
        /// Reads data from the stream at the provided position, the stream's current position is not effected.
        /// </summary>
        /// <param name="position">The position from the stream to read from.</param>
        /// <returns></returns>
        public Guid ReadGuid(long position)
        {
            if (RemainingLenght(position) < 16)
            {
                Read(position, m_buffer, 0, 16);
                fixed (byte* lp = m_buffer)
                {
                    Guid* lpp = (Guid*)(lp);
                    return *lpp;
                }
            }
            else
            {
                int offset = CalculateOffset(position);
                fixed (byte* lp = GetPage(position))
                {
                    Guid* lpp = (Guid*)(lp + offset);
                    return *lpp;
                }
            }
        }

        /// <summary>
        /// Reads data from the stream at the current position, advancing the position.
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            uint value = ReadUInt32(Position);
            Position += 4;
            return value;
        }

        /// <summary>
        /// Reads data from the stream at the provided position, the stream's current position is not effected.
        /// </summary>
        /// <param name="position">The position from the stream to read from.</param>
        /// <returns></returns>
        public uint ReadUInt32(long position)
        {
            if (RemainingLenght(position) < 4)
            {
                Read(position, m_buffer, 0, 4);
                fixed (byte* lp = m_buffer)
                {
                    uint* lpp = (uint*)(lp);
                    return *lpp;
                }
            }
            else
            {
                int offset = CalculateOffset(position);
                fixed (byte* lp = GetPage(position))
                {
                    uint* lpp = (uint*)(lp + offset);
                    return *lpp;
                }
            }
        }

        /// <summary>
        /// Reads data from the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="data">The data to read</param>
        /// <param name="offset">The position to start the read</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The number of bytes read from the stream. This will always be equal to count.</returns>
        public int Read(byte[] data, int offset, int count)
        {
            int bytesRead = Read(Position, data, offset, count);
            Position += count;
            return bytesRead;
        }

        /// <summary>
        /// Reads data from the stream at the provided position, the stream's current position is not effected.
        /// </summary>
        /// <param name="position">The position from the stream to read from.</param>
        /// <param name="data">The data to read</param>
        /// <param name="offset">The position to start the read</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The number of bytes read from the stream. This will always be equal to count.</returns>
        public int Read(long position, byte[] data, int offset, int count)
        {
            int availableLength = RemainingLenght(position);
            int destOffset = CalculateOffset(position);
            byte[] block = GetPage(position);

            if (availableLength >= count)
            {
                Array.Copy(block, destOffset, data, offset, count);
            }
            else
            {
                Array.Copy(block, destOffset, data, offset, availableLength);
                Read(position + availableLength, data, offset + availableLength, count - availableLength);
            }
            return count;
        }

    }
}
