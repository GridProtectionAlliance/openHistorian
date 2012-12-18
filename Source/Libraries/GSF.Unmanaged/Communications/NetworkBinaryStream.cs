using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace openHistorian.Communications
{

    public class NetworkBinaryStream : Stream
    {
        CircularBuffer m_receiveBuffer = new CircularBuffer(2000);
        LinearBuffer m_sendBuffer = new LinearBuffer(2000);
        Socket m_socket;
        byte[] m_tmpBuffer = new byte[1500];

        public NetworkBinaryStream(Socket socket, int timeout = -1)
            : base()
        {
            m_socket = socket;
            Timeout = timeout;
            m_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
        }

        public int Timeout
        {
            get
            {
                return m_socket.ReceiveTimeout;
            }
            set
            {
                m_socket.ReceiveTimeout = value;
                m_socket.SendTimeout = value;
            }
        }

        public bool Connected
        {
            get
            {
                return (m_socket != null) && m_socket.Connected;
            }
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

        public int AvailableReadBytes
        {
            get
            {
                return m_receiveBuffer.DataAvailable + m_socket.Available;
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

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            Send(m_sendBuffer.InternalBuffer, 0, m_sendBuffer.DataAvailable);
            m_sendBuffer.Clear();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count <= 0)
                return 0;

            int length = m_receiveBuffer.Read(buffer, offset, count);
            if (length == count)
                return count;

            int origionalCount = count;

            count -= length;
            offset += length;

            if (count > 100)
            {
                while (count > 0)
                {
                    length = Receive(buffer, offset, count);
                    offset += length;
                    count -= length;
                }
            }
            else
            {
                while (m_receiveBuffer.DataAvailable < count)
                {
                    length = Receive(m_tmpBuffer, 0, 1500);
                    m_receiveBuffer.Write(m_tmpBuffer, 0, length);
                }
                m_receiveBuffer.Read(buffer, offset, count);
            }

            return origionalCount;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (m_sendBuffer.FreeSpace < count)
                Flush();
            if (count > 100)
            {
                Send(buffer, offset, count);
            }
            else
            {
                m_sendBuffer.Write(buffer, offset, count);
            }
        }

        /// <summary>
        /// Disconnects the socket.  Does not throw an exception.
        /// </summary>
        /// <remarks></remarks>
        public void Disconnect()
        {
            Socket socket = Interlocked.Exchange(ref m_socket, null);
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                try
                {
                    socket.Close();
                }
                catch
                {
                }
            }

        }

        void Send(byte[] buffer, int offset, int count)
        {
            if (!Connected)
                throw new Exception("Not Connected");
            try
            {
                if (m_socket.Send(buffer, offset, count, SocketFlags.None) != count)
                    throw new Exception("Something isn't right");
            }
            catch
            {
                Disconnect();
                throw;
            }
        }

        int Receive(byte[] buffer, int offset, int count)
        {
            if (!Connected)
                throw new Exception("Not Connected");
            try
            {
                int rec = m_socket.Receive(buffer, offset, count, SocketFlags.None);
                if (rec == 0)
                {
                    Disconnect();
                    return 0;
                }
                return rec;
            }
            catch
            {
                Disconnect();
                throw;
            }
        }
    }

}
