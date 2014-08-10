using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF.IO.FileStructure.Media;
using GSF.Net;
using NUnit.Framework;

namespace GSF.Security
{
    [TestFixture]
    public class Srp_Test
    {
        [Test]
        public void Test()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var srp = new Srp(SrpStrength.Bits8192);
            srp.DefaultIterations = 100;
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            sw.Restart();
            srp.AddUser("user","password");
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            sw.Restart();
            srp.AddUser("user2", "password");
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        Stopwatch m_sw = new Stopwatch();

        [Test]
        public void Test1()
        {
            m_sw.Reset();
            var listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42422));
            listener.Start();

            Thread.Sleep(100);
            ThreadPool.QueueUserWorkItem(Client);

            var client = listener.AcceptTcpClient();
            var ns = new NetworkBinaryStream(client.Client);

            var sa = new Srp(SrpStrength.Bits1024);
            sa.AddUser("user1", "password1");

            var user = sa.AuthenticateAsServer(ns);
            if (user == string.Empty)
                throw new Exception();
            ns.Dispose();
            listener.Stop();
            Thread.Sleep(100);
        }

        void Client(object state)
        {
            var client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42422));
            var ns = new NetworkBinaryStream(client.Client);
            var sa = new Srp(SrpStrength.Bits1024);
            m_sw.Start();
            var success = sa.AuthenticateAsClient(ns, "user1", "password1");
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);
            if (!success)
                throw new Exception();
            ns.Dispose();
        }

        [Test]
        public void Test2()
        {
            m_sw.Reset();
            var listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42422));
            listener.Start();

            Thread.Sleep(100);
            ThreadPool.QueueUserWorkItem(Client2);

            var client = listener.AcceptTcpClient();
            var ns = new NetworkBinaryStream(client.Client);

            data1 = new byte[128];
            ns.WriteWithLength(data1);

            ns.Read7BitUInt32();

            ns.Dispose();
            listener.Stop();
            Thread.Sleep(100);
        }

        private byte[] data1;
        private byte[] data2;

        void Client2(object state)
        {
            var client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 42422));
            var ns = new NetworkBinaryStream(client.Client);

            data2 = ns.ReadBytes();

            data1 = data2;


            ns.Dispose();
        }
    }
}
