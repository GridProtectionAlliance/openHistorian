using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GSF.IO.Unmanaged;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.Net
{
    [TestFixture]
    class NetworkBinaryStream_Test
    {
        [Test]
        public void Test1()
        {
            MemoryPoolTest.TestMemoryLeak();
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42134));
            listener.Start();
            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42134));
            TcpClient server = listener.AcceptTcpClient();

            var c = new NetworkBinaryStream(client.Client);
            var s = new NetworkBinaryStream(server.Client);

            Random r = new Random();
            int seed = r.Next();
            Random sr = new Random(seed);
            Random cr = new Random(seed);

            for (int x = 0; x < 2000; x++)
            {
                for (int y = 0; y < x; y++)
                {
                    int val = sr.Next();
                    c.Write(val);
                    c.Write((byte)val);
                }
                c.Flush();
                for (int y = 0; y < x; y++)
                {
                    int val = cr.Next();
                    if (val != s.ReadInt32())
                        throw new Exception("Error");
                    if ((byte)val != s.ReadUInt8())
                        throw new Exception("Error");
                }
            }
            
            server.Close();
            client.Close();
            listener.Stop();
            MemoryPoolTest.TestMemoryLeak();

        }
    }
}
