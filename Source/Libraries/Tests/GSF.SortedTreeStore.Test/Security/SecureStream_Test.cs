using System;
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
        readonly Stopwatch m_sw = new Stopwatch();

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

        //[Test]
        //public void Default()
        //{
        //    for (int x = 0; x < 5; x++)
        //        TestDefault();

        //}

        [Test]
        public void TestDefault()
        {
            Logger.Console.Verbose = VerboseLevel.All;
            m_sw.Reset();

            NetworkStreamSimulator net = new NetworkStreamSimulator();
            SecureStreamServer<NullToken> sa = new SecureStreamServer<NullToken>();
            sa.SetDefaultUser(true, new NullToken());
            ThreadPool.QueueUserWorkItem(ClientDefault, net.ClientStream);

            if (!sa.TryAuthenticateAsServer(net.ServerStream, true, out Stream stream, out T))
            {
                throw new Exception();
            }
            
            stream.Write("Message");
            stream.Flush();
            if (stream.ReadString() != "Response")
                throw new Exception();
            stream.Dispose();
            
            Thread.Sleep(100);
        }

        void ClientDefault(object state)
        {
            Stream client = (Stream)state;
            SecureStreamClientDefault sa = new SecureStreamClientDefault();
            if (!sa.TryAuthenticate(client, true, out Stream stream))
            {
                throw new Exception();
            }

            if (stream.ReadString() != "Message")
                throw new Exception();
            stream.Write("Response");
            stream.Flush();
            stream.Dispose();
        }

        [Test]
        public void BenchmarkDefault()
        {
            for (int x = 0; x < 5; x++)
                TestBenchmarkDefault();

        }
        [Test]
        public void TestBenchmarkDefault()
        {
            Logger.Console.Verbose = VerboseLevel.All;
            m_sw.Reset();

            NetworkStreamSimulator net = new NetworkStreamSimulator();

            SecureStreamServer<NullToken> sa = new SecureStreamServer<NullToken>();
            sa.SetDefaultUser(true, new NullToken());
            ThreadPool.QueueUserWorkItem(ClientBenchmarkDefault, net.ClientStream);

            sa.TryAuthenticateAsServer(net.ServerStream, false, out Stream stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, false, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, false, out stream, out T);

            Thread.Sleep(100);
        }

        void ClientBenchmarkDefault(object state)
        {
            Stream client = (Stream)state;
            SecureStreamClientDefault sa = new SecureStreamClientDefault();
            m_sw.Start();
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
        public void TestBenchmarkIntegrated()
        {
            return;
            Logger.Console.Verbose = VerboseLevel.All;
            m_sw.Reset();

            NetworkStreamSimulator net = new NetworkStreamSimulator();

            SecureStreamServer<NullToken> sa = new SecureStreamServer<NullToken>();
            sa.AddUserIntegratedSecurity("Zthe\\steven", new NullToken());
            ThreadPool.QueueUserWorkItem(ClientBenchmarkIntegrated, net.ClientStream);

            Stream stream;
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);
            sa.TryAuthenticateAsServer(net.ServerStream, true, out stream, out T);

            Thread.Sleep(100);
        }

        void ClientBenchmarkIntegrated(object state)
        {
            Stream client = (Stream)state;
            SecureStreamClientIntegratedSecurity sa = new SecureStreamClientIntegratedSecurity();
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
                TestBenchmarkIntegrated();

        }
    }
}
