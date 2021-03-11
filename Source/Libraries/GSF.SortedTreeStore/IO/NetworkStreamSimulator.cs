//******************************************************************************************************
//  NetworkStreamSimulator.cs - Gbtc
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
//  9/2/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using GSF.Collections;

namespace GSF.IO
{
    /// <summary>
    /// Provides a stream that functions like a network stream
    /// except it cuts out the socket layer.
    /// </summary>
    public class NetworkStreamSimulator
    {
        private class InternalStreams : Stream
        {
            private readonly IsolatedQueue<byte> m_sendQueue;
            private readonly IsolatedQueue<byte> m_receiveQueue;

            public InternalStreams(IsolatedQueue<byte> sendQueue, IsolatedQueue<byte> receiveQueue)
            {
                m_sendQueue = sendQueue;
                m_receiveQueue = receiveQueue;
            }

            public override void Flush()
            {

            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int bytesRead = 0;
                while (bytesRead < count)
                {
                    bytesRead += m_receiveQueue.Dequeue(buffer, offset + bytesRead, count - bytesRead);
                }
                return bytesRead;
            }

            public override int ReadByte()
            {
                byte value;
                while (!m_receiveQueue.TryDequeue(out value))
                    ;
                return value;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                m_sendQueue.Enqueue(buffer, offset, count);
            }

            public override void WriteByte(byte value)
            {
                m_sendQueue.Enqueue(value);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }
        }

        private readonly IsolatedQueue<byte> m_queueA;
        private readonly IsolatedQueue<byte> m_queueB;

        /// <summary>
        /// The client's stream
        /// </summary>
        public readonly Stream ClientStream;
        /// <summary>
        /// The server's stream
        /// </summary>
        public readonly Stream ServerStream;

        /// <summary>
        /// Creates a new <see cref="NetworkStreamSimulator"/>
        /// </summary>
        public NetworkStreamSimulator()
        {
            m_queueA = new IsolatedQueue<byte>();
            m_queueB = new IsolatedQueue<byte>();

            ClientStream = new InternalStreams(m_queueA, m_queueB);
            ServerStream = new InternalStreams(m_queueB, m_queueA);
        }


    }
}
