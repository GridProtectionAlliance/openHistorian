using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GSF.Communications
{
    [TestFixture]
    class NetworkBinaryStream_Test
    {
        [Test]
        public void Test1()
        {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42134));
            listener.Start();
            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42134));
            TcpClient server = listener.AcceptTcpClient();

            var c = new NetworkBinaryStream2(client.Client);
            var s = new NetworkBinaryStream2(server.Client);

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
                    if ((byte)val != s.ReadByte())
                        throw new Exception("Error");
                }
            }
            
            server.Close();
            client.Close();
            listener.Stop();
        }
    }
}
