using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;

namespace openHistorian.PerformanceTests
{
    [TestFixture]
    public class BenchmarkSockets
    {
        const int Loop = 10000;
        
        [Test]
        public void Run()
        {
            Thread read = new Thread(Reader);
            read.IsBackground = true;

            TcpListener listen = new TcpListener(IPAddress.Parse("127.0.0.1"), 36345);
            listen.Start();

            Thread.Sleep(100);
            read.Start();

            TcpClient client = listen.AcceptTcpClient();

            byte[] data = new byte[154600];
            Stopwatch sw = new Stopwatch();

            NetworkStream stream = client.GetStream();

            sw.Start();
            for (int x = 0; x < Loop; x++)
                stream.Write(data, 0, data.Length);
            sw.Stop();
            stream.Close();

            Console.WriteLine("Write: " + (Loop * data.Length / sw.Elapsed.TotalSeconds / 1000000).ToString());
            Thread.Sleep(1000);

            read.Join();
        }


        void Reader()
        {

            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 36345);

            Stopwatch sw = new Stopwatch();
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[154600];
            sw.Start();
            while (stream.Read(data, 0, data.Length) > 0)
                ;
            sw.Stop();

            Console.WriteLine("Read: " + (Loop * data.Length / sw.Elapsed.TotalSeconds / 1000000).ToString());
        }
    }
}
