using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GSF.IO;
using GSF.Security.Authentication;
using NUnit.Framework;

namespace GSF.Security
{
    [TestFixture]
    public class IntegratedSecurity_Test
    {
        readonly Stopwatch m_sw = new Stopwatch();

        [Test]
        public void Test1()
        {
            m_sw.Reset();

            NetworkStreamSimulator net = new NetworkStreamSimulator();

            IntegratedSecurityServer sa = new IntegratedSecurityServer();
            sa.Users.AddUser("zthe\\steven");

            ThreadPool.QueueUserWorkItem(Client1, net.ClientStream);
            bool user = sa.TryAuthenticateAsServer(net.ServerStream, out Guid token);
            user = sa.TryAuthenticateAsServer(net.ServerStream, out token);
            //if (user is null)
            //    throw new Exception();

            Thread.Sleep(100);
        }

        void Client1(object state)
        {
            Stream client = (Stream)state;
            IntegratedSecurityClient sa = new IntegratedSecurityClient();
            m_sw.Start();
            _ = sa.TryAuthenticateAsClient(client);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);
            m_sw.Restart();
            bool success = sa.TryAuthenticateAsClient(client);
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
