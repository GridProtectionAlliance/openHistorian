using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using GSF.IO;
using GSF.Net;
using NUnit.Framework;
using openHistorian;
using Org.BouncyCastle.Crypto.Digests;

namespace GSF.Security
{
    [TestFixture]
    public class Srp_Test
    {
        [Test]
        public void TestDHKeyExchangeTime()
        {
            var c = SrpConstants.Lookup(SrpStrength.Bits1024);
            c.g.ModPow(c.N, c.N);

            DebugStopwatch sw = new DebugStopwatch();
            var time = sw.TimeEvent(() => Hash<Sha1Digest>.Compute(c.Nb));
            System.Console.WriteLine(time);



        }

        [Test]
        public void Test()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var srp = new SrpServer();
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            sw.Restart();
            srp.Users.AddUser("user", "password");
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds);

            sw.Restart();
            srp.Users.AddUser("user2", "password");
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        Stopwatch m_sw = new Stopwatch();

        [Test]
        public void Test1()
        {
            m_sw.Reset();

            var net = new NetworkStreamSimulator();

            var sa = new SrpServer();
            sa.Users.AddUser("user1", "password1", SrpStrength.Bits1024, 1, 1);

            ThreadPool.QueueUserWorkItem(Client1, net.ClientStream);
            var user = sa.AuthenticateAsServer(net.ServerStream);
            if (user == null)
                throw new Exception();

            Thread.Sleep(100);
        }

        void Client1(object state)
        {
            Stream client = (Stream)state;
            var sa = new SrpClient();
            m_sw.Start();
            var success = sa.AuthenticateAsClient(client, "user1", "password1");
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);
            if (!success)
                throw new Exception();
        }

        [Test]
        public void TestRepeat()
        {
            for (int x = 0; x < 5; x++)
                Test1();

        }
    }
}
