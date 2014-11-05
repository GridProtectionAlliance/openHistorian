using System.Diagnostics;
using System.IO;
using System.Threading;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Security.Authentication;
using NUnit.Framework;

namespace GSF.Security
{
    [TestFixture]
    public class SecureStream_Test
    {
        public NullToken T;
        Stopwatch m_sw = new Stopwatch();

        //[Test]
        //public void Test1()
        //{
        //    m_sw.Reset();

        //    var net = new NetworkStreamSimulator();

        //    var sa = new SecureStreamServer<NullToken>();
        //    sa.Srp.Users.AddUser("user1","password");
        //    ThreadPool.QueueUserWorkItem(Client1, net.ClientStream);

        //    SecureStream stream;
        //    sa.TryAuthenticateAsServer(net.ServerStream, out stream, out T);
        //    sa.TryAuthenticateAsServer(net.ServerStream, out stream, out T);

        //    Thread.Sleep(100);
        //}

        //void Client1(object state)
        //{
        //    Stream client = (Stream)state;
        //    var sa = new SecureStreamClientSrp("user1", "password");
        //    m_sw.Start();

        //    sa.TryAuthenticate(client);

        //    m_sw.Stop();
        //    System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);
        //    m_sw.Restart();
        //    sa.TryAuthenticate(client);

        //    m_sw.Stop();
        //    System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);
        //}

        //[Test]
        //public void TestRepeat()
        //{
        //    for (int x = 0; x < 5; x++)
        //        Test1();

        //}

        [Test]
        public void Default()
        {
            for (int x = 0; x < 5; x++)
                TestDefault();

        }
        [Test]
        public void TestDefault()
        {
            Logger.ReportToConsole(VerboseLevel.All);
            m_sw.Reset();

            var net = new NetworkStreamSimulator();

            var sa = new SecureStreamServer<NullToken>();
            sa.SetDefaultUser(true, new NullToken());
            ThreadPool.QueueUserWorkItem(ClientDefault, net.ClientStream);

            Stream stream;
            sa.TryAuthenticateAsServer(net.ServerStream, false, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, false, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, false, out stream, out T);

            Thread.Sleep(100);
        }

        void ClientDefault(object state)
        {
            Stream client = (Stream)state;
            var sa = new SecureStreamClientDefault();
            m_sw.Start();
            sa.TryAuthenticate(client,false);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);

            m_sw.Restart();
            sa.TryAuthenticate(client);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);

            m_sw.Restart();
            sa.TryAuthenticate(client, false);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);

            m_sw.Restart();
            sa.TryAuthenticate(client);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);

            m_sw.Restart();
            sa.TryAuthenticate(client, false);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);
        }

        [Test]
        public void TestIntegrated()
        {
            Logger.ReportToConsole(VerboseLevel.All);
            m_sw.Reset();

            var net = new NetworkStreamSimulator();

            var sa = new SecureStreamServer<NullToken>();
            sa.AddUserIntegratedSecurity("Zthe\\steven", new NullToken());
            ThreadPool.QueueUserWorkItem(ClientIntegrated, net.ClientStream);

            Stream stream;
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);

            Thread.Sleep(100);
        }

        void ClientIntegrated(object state)
        {
            Stream client = (Stream)state;
            var sa = new SecureStreamClientIntegratedSecurity();
            m_sw.Start();
            sa.TryAuthenticate(client);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);

            m_sw.Restart();
            sa.TryAuthenticate(client);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);

            m_sw.Restart();
            sa.TryAuthenticate(client);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);

            m_sw.Restart();
            sa.TryAuthenticate(client);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);

            m_sw.Restart();
            sa.TryAuthenticate(client);
            m_sw.Stop();
            System.Console.WriteLine(m_sw.Elapsed.TotalMilliseconds);
        }

        [Test]
        public void TestRepeatIntegrated()
        {
            for (int x = 0; x < 5; x++)
                TestIntegrated();

        }
    }
}
