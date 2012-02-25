using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace openHistorian.Core.StorageSystem
{
    public unsafe class BinaryStream
    {
        int m_currentIndex;
        int m_lastIndex;
        int m_lastIndexWrite;
        int m_firstIndex;
        int m_origionalIndex;
        long m_currentPosition;

        //GCHandle handle;
        //byte* f_buffer;
        internal byte[] m_buffer;
        ISupportsBinaryStream m_stream;
        byte[] m_temp;

        public BinaryStream(ISupportsBinaryStream stream)
        {
            m_temp = new byte[16];
            m_stream = stream;
            Clear();
        }
        void Clear()
        {
            m_buffer = null;
            m_firstIndex = 0;
            m_lastIndex = -1;
            m_lastIndexWrite = -1;
            m_currentIndex = 0;
            m_origionalIndex = 0;
            m_currentPosition = -1;
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
                if (m_currentPosition >= 0)
                    return m_currentPosition + (m_currentIndex - m_origionalIndex);
                else
                    return m_stream.Position + (m_currentIndex - m_origionalIndex);
            }
            set
            {
                if (m_currentPosition >= 0)
                {
                    if (m_currentPosition != m_stream.Position)
                        throw new Exception();

                    long newCurrentIndex = m_origionalIndex + (value - m_currentPosition);
                    if (newCurrentIndex >= m_firstIndex && newCurrentIndex <= m_lastIndex)
                    {
                        m_currentIndex = (int)newCurrentIndex;

                        if (Position != value)
                            throw new Exception();

                        return;
                    }
                }
                //ToDo: Determine if flushing is required, if not, make adjustments.
                //long pos = m_stream.Position;
                FlushToUnderlyingStream();
                m_stream.Position = value;
            }
        }

        /// <summary>
        /// Returns the number of bytes available at the end of the stream.
        /// </summary>
        int RemainingLength
        {
            get
            {
                return (m_lastIndex - m_currentIndex) + 1;
            }
        }
        /// <summary>
        /// Returns the number of bytes available at the end of the stream for writing purposes.
        /// </summary>
        int RemainingLengthWrite
        {
            get
            {
                return (m_lastIndexWrite - m_currentIndex) + 1;
            }
        }

        /// <summary>
        /// Since all read/write operations are cached, calling this will update the underlying stream's position.
        /// </summary>
        public void FlushToUnderlyingStream()
        {
            m_stream.Position = Position;
            //ToDo: Consider not modifing any of the local buffer parameters.
            Clear();
        }

        /// <summary>
        /// Updates the local buffer data.
        /// </summary>
        /// <param name="isWriting"></param>
        /// <remarks>This is called when there is not enough bytes in the buffer to store the available data.</remarks>
        internal void UpdateLocalBuffer(bool isWriting)
        {
            FlushToUnderlyingStream();
            //if (m_buffer != null)
            //    handle.Free();
            m_stream.GetCurrentBlock(isWriting, out m_buffer, out m_firstIndex, out m_lastIndex, out m_currentIndex);
            m_currentPosition = m_stream.Position;
            m_origionalIndex = m_currentIndex;
            
            //handle = GCHandle.Alloc(m_buffer, GCHandleType.Pinned);
            //f_buffer = (byte*)handle.AddrOfPinnedObject().ToPointer();

            if (isWriting)
                m_lastIndexWrite = m_lastIndex;
            else
                m_lastIndexWrite = -1;
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
            byte[] buffer1, buffer2;
            int firstIndex1, firstIndex2, lastIndex1, lastIndex2, currentIndex1, currentIndex2;

            long origionalPosition = Position;
            Position = source;
            FlushToUnderlyingStream();
            m_stream.GetCurrentBlock(false, out buffer1, out firstIndex1, out lastIndex1, out currentIndex1);
            Position = destination;
            FlushToUnderlyingStream();
            m_stream.GetCurrentBlock(true, out buffer2, out firstIndex2, out lastIndex2, out currentIndex2);

            if (lastIndex1 - currentIndex1 + 1 >= length && lastIndex1 - currentIndex1 + 1 >= length) //both source and destination are within the same buffer
            {
                Array.Copy(buffer1, currentIndex1, buffer2, currentIndex2, length);
            }
            else if (lastIndex1 - currentIndex1 + 1 >= length) //only the source is within the same buffer
            {
                Position = destination;
                Write(buffer1, currentIndex1, length);
            }
            else
            {
                //manually perform the copy
                byte[] data = new byte[length];
                Position = source;
                Read(data, 0, data.Length);
                Position = destination;
                Write(data, 0, data.Length);
            }
            Position = origionalPosition;
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
            //Future Improvements: Add more functions for popular moves, like shift by 8 or shift by 12...
            //   Also if on a boundry, do something other than copy the entire contents into an array.
            //   However, all inserts will likely on be on the same page and therefore this last speed optimization may just complecate the code.

            if (numberOfBytes < 0)
                throw new ArgumentException("numberOfBytes", "value cannot be less than zero");
            if (lengthOfValidDataToShift < 0)
                throw new ArgumentException("lengthOfValidDataToShift", "value cannot be less than zero");

            if (numberOfBytes == 0 || lengthOfValidDataToShift == 0)
                return;

            if (m_lastIndexWrite < 0) //Make sure we are in write mode.
                UpdateLocalBuffer(true);

            if (RemainingLengthWrite >= numberOfBytes + lengthOfValidDataToShift)
            {
                Array.Copy(m_buffer, m_currentIndex, m_buffer, m_currentIndex + numberOfBytes, lengthOfValidDataToShift);
            }
            else
            {
                byte[] data = new byte[lengthOfValidDataToShift];
                Read(data, 0, lengthOfValidDataToShift);
                Position -= lengthOfValidDataToShift - numberOfBytes;
                Write(data, 0, lengthOfValidDataToShift);
                Position -= lengthOfValidDataToShift + numberOfBytes;
            }
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
            //Future Improvements: Add more functions for popular moves, like shift by 8 or shift by 12...
            //   Also if on a boundry, do something other than copy the entire contents into an array.
            //   However, all inserts will likely on be on the same page and therefore this last speed optimization may just complecate the code.

            if (numberOfBytes < 0)
                throw new ArgumentException("numberOfBytes", "value cannot be less than zero");
            if (lengthOfValidDataToShift < 0)
                throw new ArgumentException("lengthOfValidDataToShift", "value cannot be less than zero");

            if (numberOfBytes == 0 || lengthOfValidDataToShift == 0)
                return;

            if (m_lastIndexWrite < 0) //Make sure we are in write mode.
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= numberOfBytes + lengthOfValidDataToShift)
            {
                Array.Copy(m_buffer, m_currentIndex + numberOfBytes, m_buffer, m_currentIndex, lengthOfValidDataToShift);
            }
            else
            {
                byte[] data = new byte[lengthOfValidDataToShift];
                Position += numberOfBytes;
                Read(data, 0, lengthOfValidDataToShift);
                Position -= lengthOfValidDataToShift + numberOfBytes;
                Write(data, 0, lengthOfValidDataToShift);
                Position -= lengthOfValidDataToShift;
            }
        }

        #region Helper Types

        public void Write(long value1, long value2)
        {
            const int size = 16;
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(long*)(lp + m_currentIndex) = value1;
                    *(long*)(lp + m_currentIndex + 8) = value2;
                }
                m_currentIndex += size;
                return;
            }
            Write2(value1);
            Write2(value2);
        }

        #endregion

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
            if (m_lastIndexWrite >= m_currentIndex) //RemainingLenghtWrite >= size.
            {
                m_buffer[m_currentIndex] = value;
                m_currentIndex += size;
                return;
            }
            Write2(value);
        }
        void Write2(byte value)
        {
            const int size = sizeof(byte);
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                m_buffer[m_currentIndex] = value;
                m_currentIndex += size;
                return;
            }
            FlushToUnderlyingStream();
            m_temp[0] = value;
            m_stream.Write(m_temp, 0, size);
        }
        public void Write(short value)
        {
            const int size = sizeof(short);
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                m_buffer[m_currentIndex] = (byte)value;
                m_buffer[m_currentIndex + 1] = (byte)(value >> 8);
                m_currentIndex += size;
                return;
            }
            Write2(value);
        }
        void Write2(short value)
        {
            const int size = sizeof(short);
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                m_buffer[m_currentIndex] = (byte)value;
                m_buffer[m_currentIndex + 1] = (byte)(value >> 8);
                m_currentIndex += size;
                return;
            }
            FlushToUnderlyingStream();
            m_temp[0] = (byte)value;
            m_temp[1] = (byte)(value >> 8);
            m_stream.Write(m_temp, 0, size);
        }
        public void Write(int value)
        {
            const int size = sizeof(int);
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(int*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            Write2(value);
        }
        void Write2(int value)
        {
            const int size = sizeof(int);
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(int*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            FlushToUnderlyingStream();
            fixed (byte* lp = m_temp)
            {
                *(int*)lp = value;
            }
            m_stream.Write(m_temp, 0, size);
        }
        public void Write(float value)
        {
            const int size = sizeof(float);
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(float*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            Write2(value);
        }
        void Write2(float value)
        {
            const int size = sizeof(float);
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(float*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            FlushToUnderlyingStream();
            fixed (byte* lp = m_temp)
            {
                *(float*)lp = value;
            }
            m_stream.Write(m_temp, 0, size);
        }
        public void Write(long value)
        {
            const int size = sizeof(long);
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(long*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            Write2(value);
        }
        void Write2(long value)
        {
            const int size = sizeof(long);
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(long*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            FlushToUnderlyingStream();
            fixed (byte* lp = m_temp)
            {
                *(long*)lp = value;
            }
            m_stream.Write(m_temp, 0, size);
        }
        public void Write(double value)
        {
            const int size = sizeof(double);
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(double*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            Write2(value);
        }
        void Write2(double value)
        {
            const int size = sizeof(double);
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(double*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            FlushToUnderlyingStream();
            fixed (byte* lp = m_temp)
            {
                *(double*)lp = value;
            }
            m_stream.Write(m_temp, 0, size);
        }
        public void Write(DateTime value)
        {
            const int size = 8;
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(DateTime*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            Write2(value);
        }
        void Write2(DateTime value)
        {
            const int size = 8;
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(DateTime*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            FlushToUnderlyingStream();
            fixed (byte* lp = m_temp)
            {
                *(DateTime*)lp = value;
            }
            m_stream.Write(m_temp, 0, size);
        }
        public void Write(decimal value)
        {
            const int size = sizeof(decimal);
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(decimal*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            Write2(value);
        }
        void Write2(decimal value)
        {
            const int size = sizeof(decimal);
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(decimal*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            FlushToUnderlyingStream();
            fixed (byte* lp = m_temp)
            {
                *(decimal*)lp = value;
            }
            m_stream.Write(m_temp, 0, size);
        }
        public void Write(Guid value)
        {
            const int size = 16;
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(Guid*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            Write2(value);
        }
        void Write2(Guid value)
        {
            const int size = 16;
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                fixed (byte* lp = m_buffer)
                {
                    *(Guid*)(lp + m_currentIndex) = value;
                }
                m_currentIndex += size;
                return;
            }
            FlushToUnderlyingStream();
            fixed (byte* lp = m_temp)
            {
                *(Guid*)lp = value;
            }
            m_stream.Write(m_temp, 0, size);
        }
        public void Write7Bit(uint value)
        {
            const int size = 5;
            byte[] stream = m_buffer;
            int position = m_currentIndex;
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                if (value < 128)
                {
                    stream[position] = (byte)value;
                    m_currentIndex += 1;
                    return;
                }
                stream[position] = (byte)(value | 128);
                if (value < 128 * 128)
                {
                    stream[position + 1] = (byte)(value >> 7);
                    m_currentIndex += 2;
                    return;
                }
                stream[position + 1] = (byte)((value >> 7) | 128);
                if (value < 128 * 128 * 128)
                {
                    stream[position + 2] = (byte)(value >> 14);
                    m_currentIndex += 3;
                    return;
                }
                stream[position + 2] = (byte)((value >> 14) | 128);
                if (value < 128 * 128 * 128 * 128)
                {
                    stream[position + 3] = (byte)(value >> 21);
                    m_currentIndex += 4;
                    return;
                }
                stream[position + 3] = (byte)((value >> 21) | 128);
                stream[position + 4] = (byte)(value >> 28);
                m_currentIndex += 5;
                return;
            }
            Write7Bit2(value);
        }
        void Write7Bit2(uint value)
        {
            int size = Compression.Get7BitSize(value);
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                Compression.Write7Bit(m_buffer, ref m_currentIndex, value);
                return;
            }
            FlushToUnderlyingStream();
            int pos = 0;
            Compression.Write7Bit(m_temp, ref pos, value);
            m_stream.Write(m_temp, 0, size);
        }

        public void Write7Bit(ulong value)
        {
            const int size = 9;
            byte[] stream = m_buffer;
            int position = m_currentIndex;
            if (m_lastIndexWrite - m_currentIndex >= size - 1)
            {
                if (value < 128)
                {
                    stream[position] = (byte)value;
                    m_currentIndex += 1;
                    return;
                }
                stream[position] = (byte)(value | 128);
                if (value < 128 * 128)
                {
                    stream[position + 1] = (byte)(value >> 7);
                    m_currentIndex += 2;
                    return;
                }
                stream[position + 1] = (byte)((value >> 7) | 128);
                if (value < 128 * 128 * 128)
                {
                    stream[position + 2] = (byte)(value >> 14);
                    m_currentIndex += 3;
                    return;
                }
                stream[position + 2] = (byte)((value >> 14) | 128);
                if (value < 128 * 128 * 128 * 128)
                {
                    stream[position + 3] = (byte)(value >> 21);
                    m_currentIndex += 4;
                    return;
                }
                stream[position + 3] = (byte)((value >> (7 + 7 + 7)) | 128);
                if (value < 128L * 128 * 128 * 128 * 128)
                {
                    stream[position + 4] = (byte)(value >> (7 + 7 + 7 + 7));
                    m_currentIndex += 5;
                    return;
                }
                stream[position + 4] = (byte)((value >> (7 + 7 + 7 + 7)) | 128);
                if (value < 128L * 128 * 128 * 128 * 128 * 128)
                {
                    stream[position + 5] = (byte)(value >> (7 + 7 + 7 + 7 + 7));
                    m_currentIndex += 6;
                    return;
                }
                stream[position + 5] = (byte)((value >> (7 + 7 + 7 + 7 + 7)) | 128);
                if (value < 128L * 128 * 128 * 128 * 128 * 128 * 128)
                {
                    stream[position + 6] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7));
                    m_currentIndex += 7;
                    return;
                }
                stream[position + 6] = (byte)((value >> (7 + 7 + 7 + 7 + 7 + 7)) | 128);
                if (value < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
                {
                    stream[position + 7] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7 + 7));
                    m_currentIndex += 8;
                    return;
                }
                stream[position + 7] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7 + 7) | 128);
                stream[position + 8] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
                m_currentIndex += 9;
                return;
            }
            Write7Bit2(value);
        }
        void Write7Bit2(ulong value)
        {
            int size = Compression.Get7BitSize(value);
            if (RemainingLengthWrite <= 0)
                UpdateLocalBuffer(true);
            if (RemainingLengthWrite >= size)
            {
                Compression.Write7Bit(m_buffer, ref m_currentIndex, value);
                return;
            }
            FlushToUnderlyingStream();
            int pos = 0;
            Compression.Write7Bit(m_temp, ref pos, value);
            m_stream.Write(m_temp, 0, size);
        }

        public void Write(byte[] value, int offset, int count)
        {
            if (m_lastIndexWrite - m_currentIndex >= count - 1)
            {
                Array.Copy(value, offset, m_buffer, m_currentIndex, count);
                m_currentIndex += count;
                return;
            }
            Write2(value, offset, count);
        }
        void Write2(byte[] value, int offset, int count)
        {
            FlushToUnderlyingStream();
            m_stream.Write(value, offset, count);
        }

        #endregion

        #endregion

        #region Reading

        #region Helper Types
        public void ReadInt128(out long value1, out long value2)
        {
            const int size = 16;
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                fixed (byte* lp = m_buffer)
                {
                    value1 = *(long*)(lp + m_currentIndex);
                    value2 = *(long*)(lp + m_currentIndex + 8);
                }
                m_currentIndex += size;
                return;
            }
            value1 = ReadInt642();
            value2 = ReadInt642();
        }
        #endregion

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
            if (m_lastIndex >= m_currentIndex) //RemainingLength >= size;
            {
                byte value = m_buffer[m_currentIndex];
                m_currentIndex += size;
                return value;
            }
            return ReadByte2();
        }
        byte ReadByte2()
        {
            const int size = sizeof(byte);
            if (RemainingLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingLength >= size)
            {
                byte value = m_buffer[m_currentIndex];
                m_currentIndex += size;
                return value;
            }
            FlushToUnderlyingStream();
            m_stream.Read(m_temp, 0, size);
            return m_temp[0];
        }
        public short ReadInt16()
        {
            const int size = sizeof(short);
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                short value = (short)(m_buffer[m_currentIndex] | (m_buffer[m_currentIndex + 1] << 8));
                m_currentIndex += size;
                return value;
            }
            return ReadInt162();
        }
        short ReadInt162()
        {
            const int size = sizeof(short);
            if (RemainingLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingLength >= size)
            {
                short value = (short)(m_buffer[m_currentIndex] | (m_buffer[m_currentIndex + 1] << 8));
                m_currentIndex += size;
                return value;
            }
            FlushToUnderlyingStream();
            m_stream.Read(m_temp, 0, size);
            return (short)(m_temp[0] | (m_temp[1] << 8));
        }
        public int ReadInt32()
        {
            const int size = sizeof(int);
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                int value;
                fixed (byte* lp = m_buffer)
                {
                    value = *(int*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            return ReadInt322();
        }
        int ReadInt322()
        {
            const int size = sizeof(int);
            if (RemainingLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingLength >= size)
            {
                int value;
                fixed (byte* lp = m_buffer)
                {
                    value = *(int*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            FlushToUnderlyingStream();
            m_stream.Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(int*)(lp);
            }
        }
        public float ReadSingle()
        {
            const int size = sizeof(float);
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                float value;
                fixed (byte* lp = m_buffer)
                {
                    value = *(float*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            return ReadSingle2();
        }
        float ReadSingle2()
        {
            const int size = sizeof(float);
            if (RemainingLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingLength >= size)
            {
                float value;
                fixed (byte* lp = m_buffer)
                {
                    value = *(float*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            FlushToUnderlyingStream();
            m_stream.Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(float*)(lp);
            }
        }
        public long ReadInt64()
        {
            const int size = sizeof(long);
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                long value;

                fixed (byte* lp = m_buffer)
                {
                    value = *(long*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            return ReadInt642();
        }
        long ReadInt642()
        {
            const int size = sizeof(long);
            if (RemainingLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingLength >= size)
            {
                long value;
                fixed (byte* lp = m_buffer)
                {
                    value = *(long*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            FlushToUnderlyingStream();
            m_stream.Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(long*)(lp);
            }
        }
        public double ReadDouble()
        {
            const int size = sizeof(double);
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                double value;

                fixed (byte* lp = m_buffer)
                {
                    value = *(double*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            return ReadDouble2();
        }
        double ReadDouble2()
        {
            const int size = sizeof(double);
            if (RemainingLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingLength >= size)
            {
                double value;
                fixed (byte* lp = m_buffer)
                {
                    value = *(double*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            FlushToUnderlyingStream();
            m_stream.Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(double*)(lp);
            }
        }
        public DateTime ReadDateTime()
        {
            const int size = 8;
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                DateTime value;
                fixed (byte* lp = m_buffer)
                {
                    value = *(DateTime*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            return ReadDateTime2();
        }
        DateTime ReadDateTime2()
        {
            const int size = 8;
            if (RemainingLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingLength >= size)
            {
                DateTime value;
                fixed (byte* lp = m_buffer)
                {
                    value = *(DateTime*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            FlushToUnderlyingStream();
            m_stream.Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(DateTime*)(lp);
            }
        }
        public decimal ReadDecimal()
        {
            const int size = sizeof(decimal);
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                m_currentIndex += size;
                fixed (byte* lp = m_buffer)
                {
                    return *(decimal*)(lp + m_currentIndex - size);
                }
            }
            return ReadDecimal2();
        }
        decimal ReadDecimal2()
        {
            const int size = sizeof(decimal);
            if (RemainingLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingLength >= size)
            {
                decimal value;
                fixed (byte* lp = m_buffer)
                {
                    value = *(decimal*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            FlushToUnderlyingStream();
            m_stream.Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(decimal*)(lp);
            }
        }
        public Guid ReadGuid()
        {
            const int size = 16;
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                m_currentIndex += size;
                fixed (byte* lp = m_buffer)
                {
                    return *(Guid*)(lp + m_currentIndex - size);
                }
            }
            return ReadGuid2();
        }
        Guid ReadGuid2()
        {
            const int size = 16;
            if (RemainingLength <= 0)
                UpdateLocalBuffer(false);
            if (RemainingLength >= size)
            {
                Guid value;
                fixed (byte* lp = m_buffer)
                {
                    value = *(Guid*)(lp + m_currentIndex);
                }
                m_currentIndex += size;
                return value;
            }
            FlushToUnderlyingStream();
            m_stream.Read(m_temp, 0, size);
            fixed (byte* lp = m_temp)
            {
                return *(Guid*)(lp);
            }
        }

        public uint Read7BitUInt32()
        {
            const int size = 5;
            byte[] stream = m_buffer;
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                int pos = m_currentIndex;
                uint value11;
                value11 = stream[pos];
                if (value11 < 128)
                {
                    m_currentIndex += 1;
                    return value11;
                }
                value11 ^= ((uint)stream[pos + 1] << 7);
                if (value11 < 128 * 128)
                {
                    m_currentIndex += 2;
                    return value11 ^ 0x80;
                }
                value11 ^= ((uint)stream[pos + 2] << 14);
                if (value11 < 128 * 128 * 128)
                {
                    m_currentIndex += 3;
                    return value11 ^ 0x4080;
                }
                value11 ^= ((uint)stream[pos + 3] << 21);
                if (value11 < 128 * 128 * 128 * 128)
                {
                    m_currentIndex += 4;
                    return value11 ^ 0x204080;
                }
                value11 ^= ((uint)stream[pos + 4] << 28) ^ 0x10204080;
                m_currentIndex += 5;
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
            byte[] stream = m_buffer;
            if (m_lastIndex - m_currentIndex >= size - 1)
            {
                int pos = m_currentIndex;
                ulong value11;
                value11 = stream[pos];
                if (value11 < 128)
                {
                    m_currentIndex += 1;
                    return value11;
                }
                value11 ^= ((ulong)stream[pos + 1] << (7));
                if (value11 < 128 * 128)
                {
                    m_currentIndex += 2;
                    return value11 ^ 0x80;
                }
                value11 ^= ((ulong)stream[pos + 2] << (7 + 7));
                if (value11 < 128 * 128 * 128)
                {
                    m_currentIndex += 3;
                    return value11 ^ 0x4080;
                }
                value11 ^= ((ulong)stream[pos + 3] << (7 + 7 + 7));
                if (value11 < 128 * 128 * 128 * 128)
                {
                    m_currentIndex += 4;
                    return value11 ^ 0x204080;
                }
                value11 ^= ((ulong)stream[pos + 4] << (7 + 7 + 7 + 7));
                if (value11 < 128L * 128 * 128 * 128 * 128)
                {
                    m_currentIndex += 5;
                    return value11 ^ 0x10204080L;
                }
                value11 ^= ((ulong)stream[pos + 5] << (7 + 7 + 7 + 7 + 7));
                if (value11 < 128L * 128 * 128 * 128 * 128 * 128)
                {
                    m_currentIndex += 6;
                    return value11 ^ 0x810204080L;
                }
                value11 ^= ((ulong)stream[pos + 6] << (7 + 7 + 7 + 7 + 7 + 7));
                if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
                {
                    m_currentIndex += 7;
                    return value11 ^ 0x40810204080L;
                }
                value11 ^= ((ulong)stream[pos + 7] << (7 + 7 + 7 + 7 + 7 + 7 + 7));
                if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
                {
                    m_currentIndex += 8;
                    return value11 ^ 0x2040810204080L;
                }
                value11 ^= ((ulong)stream[pos + 8] << (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
                m_currentIndex += 9;
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
            if (m_lastIndex - m_currentIndex >= count - 1)
            {
                Array.Copy(m_buffer, m_currentIndex, value, offset, count);
                m_currentIndex += count;
                return count;
            }
            return Read2(value, offset, count);
        }
        int Read2(byte[] value, int offset, int count)
        {
            FlushToUnderlyingStream();
            return m_stream.Read(value, offset, count);
        }
        #endregion

        #endregion

    }
}
