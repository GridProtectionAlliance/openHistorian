using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeplexIsDown
{
    public interface IMuxStream
    {
        void Write();
        void Read();
    }

    public class StreamMux
    {
        public class ClientStream : Stream
        {
            ManualResetEvent m_waitHandle;
            MemoryStream m_tempWriteBuffer;
            MemoryStream m_tempReadBuffer;
            int m_availableReadBytes;
            int m_availableWriteBytes;
            StreamMux m_baseClass;
            int m_clientId;
            public ClientStream(StreamMux baseClass, int clientId)
            {
                m_baseClass = baseClass;
                m_clientId = clientId;
                m_availableReadBytes = 0;
                m_availableWriteBytes = 0;
                m_tempWriteBuffer = new MemoryStream();
                m_tempReadBuffer = new MemoryStream();
                m_waitHandle = new ManualResetEvent(false);
            }

            public override void Flush()
            {

            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (m_availableReadBytes > count)
                    ;
                //m_tempBuffer.Read()
                return 1;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {

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

            public override long Position { get; set; }
        }

        object m_syncRoot;

        IMuxStream m_stream;

        ClientStream[] m_clients;
        int m_clientCount;

        public StreamMux(IMuxStream baseStream)
        {
            m_syncRoot = new object();
            m_clients = new ClientStream[4];
            m_stream = baseStream;
        }

        public ClientStream CreateStream()
        {
            lock (m_syncRoot)
            {
                for (int x = 0; x < m_clients.Length; x++)
                {
                    if (m_clients[x] == null)
                    {
                        m_clients[x] = new ClientStream(this, x);
                        return m_clients[x];
                    }
                }

                ClientStream[] newArray = new ClientStream[m_clients.Length * 2];
                m_clients.CopyTo(newArray, 0);
                int clientId = m_clients.Length + 1;
                m_clients[clientId] = new ClientStream(this, clientId);
                return m_clients[clientId];
            }
        }

        void SendData(int clientId)
        {

        }
        void DataReceived()
        {

        }


    }
}
