//******************************************************************************************************
//  RemoteBinaryStream.cs - Gbtc
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
//  12/08/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using GSF.IO;
using GSF.Threading;

namespace GSF.Net
{
    /// <summary>
    /// A binary stream that works on some kind of piped stream.
    /// This means the reader of one stream 
    /// is connected to the writer of another stream.
    /// </summary>
    public class RemoteBinaryStream
        : BinaryStreamBase
    {
        private const int BufferSize = 1420;
        private int m_receivePosition;
        private int m_receiveLength;
        private int m_sendLength;
        private readonly byte[] m_receiveBuffer;
        private readonly byte[] m_sendBuffer;

        private readonly Stream m_stream;
        private readonly WorkerThreadSynchronization m_workerThreadSynchronization;

        /// <summary>
        /// Creates a <see cref="RemoteBinaryStream"/>
        /// </summary>
        /// <param name="stream">the underlying stream to wrap</param>
        /// <param name="workerThreadSynchronization">the synchronization object</param>
        public RemoteBinaryStream(Stream stream, WorkerThreadSynchronization workerThreadSynchronization = null)
        {
            if (!BitConverter.IsLittleEndian)
                throw new Exception("BigEndian processors are not supported");

            if (workerThreadSynchronization is null)
                workerThreadSynchronization = new WorkerThreadSynchronization();

            m_workerThreadSynchronization = workerThreadSynchronization;
            m_receiveBuffer = new byte[BufferSize];
            m_sendBuffer = new byte[BufferSize];
            m_sendLength = 0;
            m_receiveLength = 0;
            m_receivePosition = 0;
            m_stream = stream;
        }

        /// <summary>
        /// Gets the <see cref="WorkerThreadSynchronization"/>. 
        /// This context will be entered when communcating to the socket layer.
        /// </summary>
        public WorkerThreadSynchronization WorkerThreadSynchronization => m_workerThreadSynchronization;

        private int SendBufferFreeSpace => BufferSize - m_sendLength;

        protected int ReceiveBufferAvailable => m_receiveLength - m_receivePosition;

        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            if (m_sendLength <= 0)
                return;

            m_workerThreadSynchronization.BeginSafeToCallbackRegion();
            try
            {
                m_stream.Write(m_sendBuffer, 0, m_sendLength);
                m_stream.Flush();
            }
            finally
            {
                m_workerThreadSynchronization.EndSafeToCallbackRegion();
            }
            m_sendLength = 0;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count <= 0)
                return 0;

            int receiveBufferLength = ReceiveBufferAvailable;

            //if there is enough in the receive buffer to fulfill this request
            if (count <= receiveBufferLength)
            {
                Array.Copy(m_receiveBuffer, m_receivePosition, buffer, offset, count);
                m_receivePosition += count;
                return count;
            }
            int originalCount = count;

            //first empty the receive buffer.
            if (receiveBufferLength > 0)
            {
                Array.Copy(m_receiveBuffer, m_receivePosition, buffer, offset, receiveBufferLength);
                m_receivePosition = 0;
                m_receiveLength = 0;
                offset += receiveBufferLength;
                count -= receiveBufferLength;
            }

            //if still asking for more than 100 bytes, skip the receive buffer 
            //and copy directly to the destination
            if (count > 100)
            {
                //Loop since ReceiveFromSocket can return parial results.
                while (count > 0)
                {
                    m_workerThreadSynchronization.BeginSafeToCallbackRegion();
                    try
                    {
                        receiveBufferLength = m_stream.Read(buffer, offset, count);
                    }
                    finally
                    {
                        m_workerThreadSynchronization.EndSafeToCallbackRegion();
                    }


                    if (receiveBufferLength == 0)
                        throw new EndOfStreamException();
                    offset += receiveBufferLength;
                    count -= receiveBufferLength;
                }
                return originalCount;
            }
            else
            {
                //With fewer than 100 bytes requested, 
                //first fill up the receive buffer, 
                //then copy this to the destination.
                int prebufferLength = m_receiveBuffer.Length;
                m_receiveLength = 0;
                while (m_receiveLength < count)
                {
                    m_workerThreadSynchronization.BeginSafeToCallbackRegion();
                    try
                    {
                        receiveBufferLength = m_stream.Read(m_receiveBuffer, m_receiveLength, prebufferLength);
                    }
                    finally
                    {
                        m_workerThreadSynchronization.EndSafeToCallbackRegion();
                    }
                    if (receiveBufferLength == 0)
                        throw new EndOfStreamException();
                    m_receiveLength += receiveBufferLength;
                    prebufferLength -= receiveBufferLength;
                }
                Array.Copy(m_receiveBuffer, 0, buffer, offset, count);
                m_receivePosition = count;
                return originalCount;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                WorkerThreadSynchronization.BeginSafeToCallbackRegion();
            }
            base.Dispose(disposing);
        }

        public override void Write(byte value)
        {
            if (m_sendLength < BufferSize)
            {
                m_sendBuffer[m_sendLength] = value;
                m_sendLength++;
                return;
            }
            base.Write(value);
        }

        public unsafe override void Write(long value)
        {
            if (m_sendLength <= BufferSize - 8)
            {
                fixed (byte* lp = m_sendBuffer)
                {
                    *(long*)(lp + m_sendLength) = value;
                    m_sendLength += 8;
                    return;
                }
            }
            base.Write(value);
        }

        public unsafe override void Write(int value)
        {
            if (m_sendLength <= BufferSize - 4)
            {
                fixed (byte* lp = m_sendBuffer)
                {
                    *(int*)(lp + m_sendLength) = value;
                    m_sendLength += 4;
                    return;
                }
            }
            base.Write(value);
        }

        public unsafe override void Write7Bit(ulong value)
        {
            const int size = 9;
            if (m_sendLength <= BufferSize - 9)
            {
                fixed (byte* lp = m_sendBuffer)
                {
                    byte* stream = lp + m_sendLength;
                    if (value < 128)
                    {
                        stream[0] = (byte)value;
                        m_sendLength += 1;
                        return;
                    }
                    stream[0] = (byte)(value | 128);
                    if (value < 128 * 128)
                    {
                        stream[1] = (byte)(value >> 7);
                        m_sendLength += 2;
                        return;
                    }
                    stream[1] = (byte)((value >> 7) | 128);
                    if (value < 128 * 128 * 128)
                    {
                        stream[2] = (byte)(value >> 14);
                        m_sendLength += 3;
                        return;
                    }
                    stream[2] = (byte)((value >> 14) | 128);
                    if (value < 128 * 128 * 128 * 128)
                    {
                        stream[3] = (byte)(value >> 21);
                        m_sendLength += 4;
                        return;
                    }
                    stream[3] = (byte)((value >> (7 + 7 + 7)) | 128);
                    if (value < 128L * 128 * 128 * 128 * 128)
                    {
                        stream[4] = (byte)(value >> (7 + 7 + 7 + 7));
                        m_sendLength += 5;
                        return;
                    }
                    stream[4] = (byte)((value >> (7 + 7 + 7 + 7)) | 128);
                    if (value < 128L * 128 * 128 * 128 * 128 * 128)
                    {
                        stream[5] = (byte)(value >> (7 + 7 + 7 + 7 + 7));
                        m_sendLength += 6;
                        return;
                    }
                    stream[5] = (byte)((value >> (7 + 7 + 7 + 7 + 7)) | 128);
                    if (value < 128L * 128 * 128 * 128 * 128 * 128 * 128)
                    {
                        stream[6] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7));
                        m_sendLength += 7;
                        return;
                    }
                    stream[6] = (byte)((value >> (7 + 7 + 7 + 7 + 7 + 7)) | 128);
                    if (value < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
                    {
                        stream[7] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7 + 7));
                        m_sendLength += 8;
                        return;
                    }
                    stream[7] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7 + 7) | 128);
                    stream[8] = (byte)(value >> (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7));
                    m_sendLength += 9;
                    return;
                }

            }
            base.Write7Bit(value);
        }

        public override byte ReadUInt8()
        {
            if (m_receivePosition < m_receiveLength)
            {
                byte value = m_receiveBuffer[m_receivePosition];
                m_receivePosition++;
                return value;
            }
            return base.ReadUInt8();
        }

        public unsafe override int ReadInt32()
        {
            if (m_receivePosition <= m_receiveLength - 4)
            {
                fixed (byte* lp = m_receiveBuffer)
                {
                    int value = *(int*)(lp + m_receivePosition);
                    m_receivePosition += 4;
                    return value;
                }
            }
            return base.ReadInt32();
        }

        public unsafe override long ReadInt64()
        {
            if (m_receivePosition <= m_receiveLength - 8)
            {
                fixed (byte* lp = m_receiveBuffer)
                {
                    long value = *(long*)(lp + m_receivePosition);
                    m_receivePosition += 8;
                    return value;
                }
            }
            return base.ReadInt64();
        }

        public unsafe override ulong Read7BitUInt64()
        {
            if (m_receivePosition <= m_receiveLength - 9)
            {
                fixed (byte* lp = m_receiveBuffer)
                {
                    byte* stream = lp + m_receivePosition;
                    ulong value11;
                    value11 = stream[0];
                    if (value11 < 128)
                    {
                        m_receivePosition += 1;
                        return value11;
                    }
                    value11 ^= (ulong)stream[1] << 7;
                    if (value11 < 128 * 128)
                    {
                        m_receivePosition += 2;
                        return value11 ^ 0x80;
                    }
                    value11 ^= (ulong)stream[2] << (7 + 7);
                    if (value11 < 128 * 128 * 128)
                    {
                        m_receivePosition += 3;
                        return value11 ^ 0x4080;
                    }
                    value11 ^= (ulong)stream[3] << (7 + 7 + 7);
                    if (value11 < 128 * 128 * 128 * 128)
                    {
                        m_receivePosition += 4;
                        return value11 ^ 0x204080;
                    }
                    value11 ^= (ulong)stream[4] << (7 + 7 + 7 + 7);
                    if (value11 < 128L * 128 * 128 * 128 * 128)
                    {
                        m_receivePosition += 5;
                        return value11 ^ 0x10204080L;
                    }
                    value11 ^= (ulong)stream[5] << (7 + 7 + 7 + 7 + 7);
                    if (value11 < 128L * 128 * 128 * 128 * 128 * 128)
                    {
                        m_receivePosition += 6;
                        return value11 ^ 0x810204080L;
                    }
                    value11 ^= (ulong)stream[6] << (7 + 7 + 7 + 7 + 7 + 7);
                    if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128)
                    {
                        m_receivePosition += 7;
                        return value11 ^ 0x40810204080L;
                    }
                    value11 ^= (ulong)stream[7] << (7 + 7 + 7 + 7 + 7 + 7 + 7);
                    if (value11 < 128L * 128 * 128 * 128 * 128 * 128 * 128 * 128)
                    {
                        m_receivePosition += 8;
                        return value11 ^ 0x2040810204080L;
                    }
                    value11 ^= (ulong)stream[8] << (7 + 7 + 7 + 7 + 7 + 7 + 7 + 7);
                    m_receivePosition += 9;
                    return value11 ^ 0x102040810204080L;
                }
            }
            return base.Read7BitUInt64();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (SendBufferFreeSpace < count)
                Flush();

            if (count > 100)
            {
                Flush();
                m_workerThreadSynchronization.BeginSafeToCallbackRegion();
                try
                {
                    m_stream.Write(buffer, offset, count);
                }
                finally
                {
                    m_workerThreadSynchronization.EndSafeToCallbackRegion();
                }
            }
            else
            {
                Array.Copy(buffer, offset, m_sendBuffer, m_sendLength, count);
                m_sendLength += count;
            }
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;
    }
}