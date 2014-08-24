using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GSF.IO;
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

            var net = new NetworkStreamSimulator();

            var sw = new Stopwatch();
            var sa = new ScramServer();
            sw.Start();
            sa.Users.AddUser("user1", "password1");
            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            ThreadPool.QueueUserWorkItem(Client1, net.ClientStream);
            var user = sa.AuthenticateAsServer(net.ServerStream);
            user = sa.AuthenticateAsServer(net.ServerStream);
            if (user == null)
                throw new Exception();

            Thread.Sleep(100);
        }

        void Client1(object state)
        {
            Stream client = (Stream)state;
            var sa = new ScramClient("user1","password1");
            sa.AuthenticateAsClient(client);
            m_sw.Start();
            var success = sa.AuthenticateAsClient(client);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);
            if (!success)
                throw new Exception();
        }

        [Test]
        public void TestMultiple()
        {
            Test1();
            Test1();
            Test1();
            Test1();
            Test1();
            Test1();
            Test1();
        }

    }
}
