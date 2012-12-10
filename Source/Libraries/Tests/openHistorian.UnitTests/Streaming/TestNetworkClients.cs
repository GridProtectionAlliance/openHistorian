using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using openHistorian.Communications;
using openHistorian.Streaming.Server;
using System.Net;
namespace openHistorian.UnitTests.Streaming
{
    [TestFixture]
    class TestNetworkClients
    {
        [Test]
        public void ClientServer()
        {
            SocketHistorian server = new SocketHistorian(43563);
            RemoteHistorian client = new RemoteHistorian(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 43563));
            var rw = client.ConnectToDatabase("Default");
            rw.Write(1, 1, 1, 1);
            Thread.Sleep(1000);
            var rw2 = rw.OpenDataReader();
            var reader = rw2.Read(1);
            {
                ulong key1, key2, value1, value2;
                Assert.AreEqual(true, reader.Read(out key1, out key2, out value1, out value2));
                Assert.AreEqual(1ul, key1);
                Assert.AreEqual(1ul, key2);
                Assert.AreEqual(1ul, value1);
                Assert.AreEqual(1ul, value2);
            }
            rw2.Close();
            client.Disconnect();
            client.Dispose();
            server.Dispose();
        }

        [Test]
        public void WriteManyPoints()
        {
            SocketHistorian server = new SocketHistorian(43563);
            RemoteHistorian client = new RemoteHistorian(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 43563));
            var rw = client.ConnectToDatabase("Default");

            for (ulong x = 0; x < 10000; x++)
            {
                rw.Write(x, x * x, x * x * x, x * x * x * x);
            }

            Thread.Sleep(1000);
            var rw2 = rw.OpenDataReader();
            var reader = rw2.Read(0,10000);
            {
                ulong key1, key2, value1, value2;
                for (ulong x = 0; x < 10000; x++)
                {
                    Assert.AreEqual(true, reader.Read(out key1, out key2, out value1, out value2));
                    Assert.AreEqual(x, key1);
                    Assert.AreEqual(x * x, key2);
                    Assert.AreEqual(x * x * x, value1);
                    Assert.AreEqual(x * x * x * x, value2);
                }
                Assert.AreEqual(false, reader.Read(out key1, out key2, out value1, out value2));
            }
            rw2.Close();
            client.Disconnect();
            client.Dispose();
            server.Dispose();
        }

        class WriteSomePoints : IPointStream
        {
            ulong x = 0;
            public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
            {
                if (x < 10000)
                {
                    key1 = x;
                    key2 = x * x;
                    value1 = x * x * x;
                    value2 = x * x * x * x;
                    x++;
                    return true;
                }
                key1 = 0;
                key2 = 0;
                value1 = 0;
                value2 = 0;
                return false;
            }

            public void Cancel()
            {
                x = 10000;
            }
        }

        [Test]
        public void WriteManyPointsStreaming()
        {
            SocketHistorian server = new SocketHistorian(43563);
            RemoteHistorian client = new RemoteHistorian(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 43563));
            var rw = client.ConnectToDatabase("Default");

            rw.Write(new WriteSomePoints());

            Thread.Sleep(1000);
            var rw2 = rw.OpenDataReader();
            var reader = rw2.Read(0, 10000);
            {
                ulong key1, key2, value1, value2;
                for (ulong x = 0; x < 10000; x++)
                {
                    Assert.AreEqual(true, reader.Read(out key1, out key2, out value1, out value2));
                    Assert.AreEqual(x, key1);
                    Assert.AreEqual(x * x, key2);
                    Assert.AreEqual(x * x * x, value1);
                    Assert.AreEqual(x * x * x * x, value2);
                }
                Assert.AreEqual(false, reader.Read(out key1, out key2, out value1, out value2));
            }
            rw2.Close();
            client.Disconnect();
            client.Dispose();
            server.Dispose();
        }
    }
}
