using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using GSF.IO;
using GSF.Net;
using GSF.Security.Authentication;
using NUnit.Framework;
using openHistorian;

namespace GSF.Security
{
    [TestFixture]
    public class IntegratedSecurity_Test
    {

        Stopwatch m_sw = new Stopwatch();

        [Test]
        public void Test1()
        {
            Guid token;
            m_sw.Reset();

            var net = new NetworkStreamSimulator();

            var sa = new IntegratedSecurityServer();
            sa.Users.AddUser("zthe\\steven");

            ThreadPool.QueueUserWorkItem(Client1, net.ClientStream);
            var user = sa.TryAuthenticateAsServer(net.ServerStream, out token);
            user = sa.TryAuthenticateAsServer(net.ServerStream, out token);
            if (user == null)
                throw new Exception();

            Thread.Sleep(100);
        }

        void Client1(object state)
        {
            Stream client = (Stream)state;
            var sa = new IntegratedSecurityClient();
            m_sw.Start();
            var success = sa.TryAuthenticateAsClient(client);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);
            m_sw.Restart();
            success = sa.TryAuthenticateAsClient(client);
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
