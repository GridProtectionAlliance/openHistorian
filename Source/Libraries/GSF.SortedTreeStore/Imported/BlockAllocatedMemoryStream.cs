//******************************************************************************************************
//  BlockAllocatedMemoryStream.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  06/14/2013 - J. Ritchie Carroll
//       Adapted from the "MemoryTributary" class written by Sebastian Friston:
//          Source Code: http://memorytributary.codeplex.com/
//          Article: http://www.codeproject.com/Articles/348590/A-replacement-for-MemoryStream
//
//******************************************************************************************************

#region [ Contributor License Agreements ]

/******************************************************************************\
   Copyright (c) 2012 Sebastian Friston
  
   Permission is hereby granted, free of charge, to any person obtaining a
   copy of this software and associated documentation files (the "Software"),
   to deal in the Software without restriction, including without limitation
   the rights to use, copy, modify, merge, publish, distribute, sublicense,
   and/or sell copies of the Software, and to permit persons to whom the
   Software is furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in
   all copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
   OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
   FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
   DEALINGS IN THE SOFTWARE.
  
\******************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.IO;

namespace GSF.IO
{
    /// <summary>
    /// Defines a stream whose backing store is memory. Externally this class operates similar to a <see cref="MemoryStream"/>,
    /// internally it uses dynamically allocated buffer blocks instead of one large contiguous array of data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="BlockAllocatedMemoryStream"/> has two primary benefits over a normal <see cref="MemoryStream"/>, first, the
    /// allocation of a large contiguous array of data in <see cref="MemoryStream"/> can fail when the requested amount of contiguous
    /// memory is unavailable - the <see cref="BlockAllocatedMemoryStream"/> prevents this; second, a <see cref="MemoryStream"/> will
    /// constantly reallocate the buffer size as the stream grows and shrinks and then copy all the data from the old buffer to the
    /// new - the <see cref="BlockAllocatedMemoryStream"/> maintains its blocks over its life cycle, unless manually cleared, thus
    /// eliminating unnecessary allocations and garbage collections when growing and reusing a stream.
    /// </para>
    /// <para>
    /// Important: Unlike <see cref="MemoryStream"/>, the <see cref="BlockAllocatedMemoryStream"/> will not use a user provided buffer
    /// as its backing buffer. Any user provided buffers used to instantiate the class will be copied into internally managed reusable
    /// memory buffers. Subsequently, the <see cref="BlockAllocatedMemoryStream"/> does not support the notion of a non-expandable
    /// stream. If you are using a <see cref="MemoryStream"/> with your own buffer, the <see cref="BlockAllocatedMemoryStream"/> will
    /// not provide any immediate benefit.
    /// </para>
    /// <para>
    /// Note that the <see cref="BlockAllocatedMemoryStream"/> will maintain all allocated blocks for stream use until the
    /// <see cref="Clear"/> method is called or the class is disposed.
    /// </para>
    /// <para>
    /// No members in the <see cref="BlockAllocatedMemoryStream"/> are guaranteed to be thread safe. Make sure any calls are
    /// synchronized when simultaneously accessed from different threads.
    /// </para>
    /// </remarks>
    public class BlockAllocatedMemoryStream : Stream
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Default value for <see cref="BlockSize"/> property.
        /// </summary>
        public const int DefaultBlockSize = 65536;

        // Fields
        private long m_length;
        private long m_position;
        private readonly int m_blockSize;
        private readonly List<byte[]> m_blocks = new List<byte[]>();
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of <see cref="BlockAllocatedMemoryStream"/>.
        /// </summary>
        public BlockAllocatedMemoryStream()
        {
            m_blockSize = DefaultBlockSize;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BlockAllocatedMemoryStream"/> from specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Initial buffer to copy into stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <remarks>
        /// Unlike <see cref="MemoryStream"/>, the <see cref="BlockAllocatedMemoryStream"/> will not use the provided
        /// <paramref name="buffer"/> as its backing buffer. The buffer will be copied into internally managed reusable
        /// memory buffers. Subsequently, the notion of a non-expandable stream is not supported.
        /// </remarks>
        public BlockAllocatedMemoryStream(byte[] buffer)
            : this(buffer, 0, (object)buffer == null ? 0 : buffer.Length, DefaultBlockSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BlockAllocatedMemoryStream"/> from specified <paramref name="buffer"/>
        /// and desired block size.
        /// </summary>
        /// <param name="buffer">Initial buffer to copy into stream.</param>
        /// <param name="blockSize">Desired size of memory blocks.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Block size must be greater than zero.</exception>
        /// <remarks>
        /// Unlike <see cref="MemoryStream"/>, the <see cref="BlockAllocatedMemoryStream"/> will not use the provided
        /// <paramref name="buffer"/> as its backing buffer. The buffer will be copied into internally managed reusable
        /// memory buffers. Subsequently, the notion of a non-expandable stream is not supported.
        /// </remarks>
        public BlockAllocatedMemoryStream(byte[] buffer, int blockSize)
            : this(buffer, 0, (object)buffer == null ? 0 : buffer.Length, blockSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BlockAllocatedMemoryStream"/> from specified region of <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Initial buffer to copy into stream.</param>
        /// <param name="startIndex">0-based start index into the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes within <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <paramref name="length"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        /// <remarks>
        /// Unlike <see cref="MemoryStream"/>, the <see cref="BlockAllocatedMemoryStream"/> will not use the provided
        /// <paramref name="buffer"/> as its backing buffer. The buffer will be copied into internally managed reusable
        /// memory buffers. Subsequently, the notion of a non-expandable stream is not supported.
        /// </remarks>
        public BlockAllocatedMemoryStream(byte[] buffer, int startIndex, int length)
            : this(buffer, startIndex, length, DefaultBlockSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BlockAllocatedMemoryStream"/> from specified region of <paramref name="buffer"/>
        /// and desired block size.
        /// </summary>
        /// <param name="buffer">Initial buffer to copy into stream.</param>
        /// <param name="startIndex">0-based start index into the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes within <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <param name="blockSize">Desired size of memory blocks.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <paramref name="length"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Block size must be greater than zero.</exception>
        /// <remarks>
        /// Unlike <see cref="MemoryStream"/>, the <see cref="BlockAllocatedMemoryStream"/> will not use the provided
        /// <paramref name="buffer"/> as its backing buffer. The buffer will be copied into internally managed reusable
        /// memory buffers. Subsequently, the notion of a non-expandable stream is not supported.
        /// </remarks>
        public BlockAllocatedMemoryStream(byte[] buffer, int startIndex, int length, int blockSize)
        {
            buffer.ValidateParameters(startIndex, length);

            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException("blockSize", "Block size must be greater than zero.");

            m_blockSize = blockSize;
            Write(buffer, startIndex, length);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BlockAllocatedMemoryStream"/> for specified <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">Initial length of the stream.</param>
        public BlockAllocatedMemoryStream(int capacity)
            : this(capacity, DefaultBlockSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BlockAllocatedMemoryStream"/> for specified <paramref name="capacity"/>
        /// and desired block size.
        /// </summary>
        /// <param name="capacity">Initial length of the stream.</param>
        /// <param name="blockSize">Desired size of memory blocks.</param>
        /// <exception cref="ArgumentOutOfRangeException">Block size must be greater than zero.</exception>
        public BlockAllocatedMemoryStream(int capacity, int blockSize)
        {
            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException("blockSize", "Block size must be greater than zero.");

            m_blockSize = blockSize;

            SetLength(capacity);

            // Pre-allocate memory at desired capacity
            while (m_blocks.Count <= (int)(capacity / blockSize))
                m_blocks.Add(new byte[blockSize]);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a value that indicates whether the <see cref="BlockAllocatedMemoryStream"/> object supports reading.
        /// This is always <c>true</c>.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="BlockAllocatedMemoryStream"/> object supports seeking.
        /// This is always <c>true</c>.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="BlockAllocatedMemoryStream"/> object supports writing.
        /// This is always <c>true</c>.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets current block size defined for this <see cref="BlockAllocatedMemoryStream"/> instance.
        /// </summary>
        public int BlockSize
        {
            get
            {
                return m_blockSize;
            }
        }

        /// <summary>
        /// Gets current stream length for this <see cref="BlockAllocatedMemoryStream"/> instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        public override long Length
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

                return m_length;
            }
        }

        /// <summary>
        /// Gets current stream position for this <see cref="BlockAllocatedMemoryStream"/> instance.
        /// </summary>
        /// <exception cref="IOException">Seeking was attempted before the beginning of the stream.</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        public override long Position
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

                return m_position;
            }
            set
            {
                if (m_disposed)
                    throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

                if (value < 0L)
                    throw new IOException("Seek was attempted before the beginning of the stream.");

                m_position = value;
            }
        }

        /// <summary>
        /// Gets the block of memory currently addressed by <see cref="Position"/>.
        /// </summary>
        /// <remarks>
        /// The buffer returned by the property is owned and managed by this <see cref="BlockAllocatedMemoryStream"/>,
        /// make sure direct access to this buffer is maintained within this class and not exposed externally.
        /// </remarks>
        protected byte[] Block
        {
            get
            {
                while (m_blocks.Count <= BlockIndex)
                    m_blocks.Add(new byte[m_blockSize]);

                return m_blocks[BlockIndex];
            }
        }

        /// <summary>
        /// Gets the index of the block currently addressed by <see cref="Position"/>.
        /// </summary>
        protected int BlockIndex
        {
            get
            {
                return (int)(m_position / m_blockSize);
            }
        }

        /// <summary>
        /// Gets the offset of the byte currently addressed by <see cref="Position"/> relative to the block that contains it.
        /// </summary>
        protected int BlockOffset
        {
            get
            {
                return (int)(m_position % m_blockSize);
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="BlockAllocatedMemoryStream"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        // Make sure buffer blocks get returned to the pool
                        Clear();
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Clears the entire <see cref="BlockAllocatedMemoryStream"/> contents and releases any allocated memory blocks.
        /// </summary>
        public void Clear()
        {
            m_position = 0;
            m_length = 0;
            m_blocks.Clear();
        }

        /// <summary>
        /// Sets the <see cref="Position"/> within the current stream to the specified value relative the <paramref name="origin"/>.
        /// </summary>
        /// <returns>
        /// The new position within the stream, calculated by combining the initial reference point and the offset.
        /// </returns>
        /// <param name="offset">The new position within the stream. This is relative to the <paramref name="origin"/> parameter, and can be positive or negative.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/>, which acts as the seek reference point.</param>
        /// <exception cref="IOException">Seeking was attempted before the beginning of the stream.</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (m_disposed)
                throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset < 0L)
                        throw new IOException("Seek was attempted before the beginning of the stream.");

                    m_position = offset;
                    break;
                case SeekOrigin.Current:
                    if (m_position + offset < 0L)
                        throw new IOException("Seek was attempted before the beginning of the stream.");

                    m_position += offset;
                    break;
                case SeekOrigin.End:
                    if (m_length + offset < 0L)
                        throw new IOException("Seek was attempted before the beginning of the stream.");

                    // Note change from original code which forced negative offset - following code
                    // matches normal MemoryStream operation for end of stream origin:
                    m_position = m_length + offset;
                    break;
            }

            return m_position;
        }

        /// <summary>
        /// Sets the length of the current stream to the specified value.
        /// </summary>
        /// <param name="value">The value at which to set the length.</param>
        public override void SetLength(long value)
        {
            m_length = value;

            // Note change from original code which did not perform the following operation - code
            // matches MemoryStream operation when position exceeds length for this function:
            if (m_position > m_length)
                m_position = m_length;
        }

        /// <summary>
        /// Reads a block of bytes from the current stream and writes the data to <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified byte array with the values between <paramref name="startIndex"/> and (<paramref name="startIndex"/> + <paramref name="length"/> - 1) replaced by the characters read from the current stream.</param>
        /// <param name="startIndex">The byte offset in <paramref name="buffer"/> at which to begin reading.</param>
        /// <param name="length">The maximum number of bytes to read.</param>
        /// <returns>
        /// The total number of bytes written into the buffer. This can be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached before any bytes are read.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <paramref name="length"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        public override int Read(byte[] buffer, int startIndex, int length)
        {
            if (m_disposed)
                throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

            buffer.ValidateParameters(startIndex, length);

            // Only read as far as end of buffer regardless of how much data was requested
            int remaining = (int)(m_length - m_position);

            if (remaining <= 0)
                return 0;

            if (length > remaining)
                length = remaining;

            int read = 0;
            int count;

            do
            {
                count = Math.Min(length, m_blockSize - BlockOffset);

                Buffer.BlockCopy(Block, BlockOffset, buffer, startIndex, count);

                length -= count;
                startIndex += count;
                read += count;
                m_position += count;
            }
            while (length > 0);

            return read;
        }

        /// <summary>
        /// Reads a byte from the current stream.
        /// </summary>
        /// <returns>
        /// The current byte cast to an <see cref="Int32"/>, or -1 if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        public override int ReadByte()
        {
            if (m_disposed)
                throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

            if (m_position >= m_length)
                return -1;

            byte value = Block[BlockOffset];
            m_position++;

            return value;
        }

        /// <summary>
        /// Writes a block of bytes to the current stream using data read from <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to write data from.</param>
        /// <param name="startIndex">The byte offset in <paramref name="buffer"/> at which to begin writing from.</param>
        /// <param name="length">The maximum number of bytes to write.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <paramref name="length"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        public override void Write(byte[] buffer, int startIndex, int length)
        {
            if (m_disposed)
                throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

            buffer.ValidateParameters(startIndex, length);

            if (length == 0)
                return;

            long originalPosition = m_position;
            int count;

            try
            {
                do
                {
                    count = Math.Min(length, m_blockSize - BlockOffset);
                    EnsureCapacity(m_position + count);

                    Buffer.BlockCopy(buffer, startIndex, Block, BlockOffset, count);

                    length -= count;
                    startIndex += count;
                    m_position += count;
                }
                while (length > 0);
            }
            catch (Exception)
            {
                m_position = originalPosition;
                throw;
            }
        }

        /// <summary>
        /// Writes a byte to the current stream at the current position.
        /// </summary>
        /// <param name="value">The byte to write.</param>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        public override void WriteByte(byte value)
        {
            if (m_disposed)
                throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

            EnsureCapacity(m_position + 1);
            Block[BlockOffset] = value;
            m_position++;
        }

        /// <summary>
        /// Writes the stream contents to a byte array, regardless of the <see cref="Position"/> property.
        /// </summary>
        /// <returns>A <see cref="byte"/>[] containing the current data in the stream</returns>
        /// <remarks>
        /// This may fail if there is not enough contiguous memory available to hold current size of stream.
        /// When possible use methods which operate on streams directly instead.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Cannot create a byte array with more than 2,147,483,591 elements.</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        public byte[] ToArray()
        {
            if (m_disposed)
                throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

            if (m_length > 0x7FFFFFC7L)
                throw new InvalidOperationException("Cannot create a byte array of size " + m_length);

            byte[] destination = new byte[m_length];
            long originalPosition = m_position;

            m_position = 0;
            Read(destination, 0, (int)m_length);
            m_position = originalPosition;

            return destination;
        }

        /// <summary>
        /// Reads specified number of bytes from source stream into this <see cref="BlockAllocatedMemoryStream"/>
        /// starting at the current position.
        /// </summary>
        /// <param name="source">The stream containing the data to copy</param>
        /// <param name="length">The number of bytes to copy</param>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        public void ReadFrom(Stream source, long length)
        {
            if (m_disposed)
                throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

            const int bufferSize = 8192;

            byte[] buffer = new byte[bufferSize];
            int read;

            do
            {
                read = source.Read(buffer, 0, (int)Math.Min(bufferSize, length));
                length -= read;
                Write(buffer, 0, read);
            }
            while (length > 0);
        }

        /// <summary>
        /// Writes the entire stream into destination, regardless of <see cref="Position"/>, which remains unchanged.
        /// </summary>
        /// <param name="destination">The stream onto which to write the current contents.</param>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        public void WriteTo(Stream destination)
        {
            if (m_disposed)
                throw new ObjectDisposedException("BlockAllocatedMemoryStream", "The stream is closed.");

            long originalPosition = m_position;
            m_position = 0;

            CopyTo(destination);

            m_position = originalPosition;
        }

        /// <summary>
        /// Overrides the <see cref="Stream.Flush"/> method so that no action is performed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method overrides the <see cref="Stream.Flush"/> method.
        /// </para>
        /// <para>
        /// Because any data written to a <see cref="BlockAllocatedMemoryStream"/> object is
        /// written into RAM, this method is superfluous.
        /// </para>
        /// </remarks>
        public override void Flush()
        {
            // Nothing to flush...
        }

        /// <summary>
        /// Makes sure desired <paramref name="length"/> can be accommodated by future data accesses.
        /// </summary>
        /// <param name="length">Minimum desired stream capacity.</param>
        protected void EnsureCapacity(long length)
        {
            if (length > m_length)
                m_length = length;
        }

        #endregion
    }
}