using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GSF.Net;
using NUnit.Framework;

namespace GSF.Security
{
    [TestFixture]
    public class SimpleAuthentication_Test
    {
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

            var sa = new SimpleAuthentication();
            sa.AddUser("user1","password1");

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
            var sa = new SimpleAuthentication();
            m_sw.Start();
            var success = sa.AuthenticateAsClient(ns, "user1", "password1");
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);
            if (!success)
                throw new Exception();
            ns.Dispose();
        }

    }
}
