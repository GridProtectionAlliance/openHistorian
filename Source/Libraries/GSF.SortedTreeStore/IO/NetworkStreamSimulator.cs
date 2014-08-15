using System;
using System.IO;
using GSF.Collections;

namespace GSF.IO
{
    public class NetworkStreamSimulator
    {
        private class InternalStreams : Stream
        {
            private IsolatedQueue2<byte> m_sendQueue;
            private IsolatedQueue2<byte> m_receiveQueue;

            public InternalStreams(IsolatedQueue2<byte> sendQueue, IsolatedQueue2<byte> receiveQueue)
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

            public override bool CanRead
            {
                get
                {
                    return true;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    return false;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    return true;
                }
            }

            public override long Length
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

            public override long Position
            {
                get
                {
                    throw new NotSupportedException();
                }
                set
                {
                    throw new NotSupportedException();
                }
            }
        }

        private IsolatedQueue2<byte> m_queueA;
        private IsolatedQueue2<byte> m_queueB;

        public readonly Stream ClientStream;
        public readonly Stream ServerStream;

        public NetworkStreamSimulator()
        {
            m_queueA = new IsolatedQueue2<byte>();
            m_queueB = new IsolatedQueue2<byte>();

            ClientStream = new InternalStreams(m_queueA, m_queueB);
            ServerStream = new InternalStreams(m_queueB, m_queueA);
        }


    }
}
