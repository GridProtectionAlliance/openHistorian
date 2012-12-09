using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using openHistorian.Streaming.Client;
using openHistorian.Streaming.Server;
using System.Net;
namespace openHistorian.UnitTests.Streaming
{
    [TestFixture]
    class TestNetworkClients
    {
        [Test]
        public void TestClientServer()
        {
            SocketHistorian server = new SocketHistorian(43563);
            RemoteHistorian client = new RemoteHistorian(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 43563));
            var rw = client.ConnectToDatabase("Default");
            rw.Write(1,1,1,1);
            Thread.Sleep(1000);
            var reader = rw.Read(1);
            {
                ulong key1, key2, value1, value2;
                Assert.AreEqual(true, reader.Read(out key1, out key2, out value1, out value2));
                Assert.AreEqual(1ul, key1);
                Assert.AreEqual(1ul, key2);
                Assert.AreEqual(1ul, value1);
                Assert.AreEqual(1ul, value2);
            }
            rw.Disconnect();
            client.Dispose();
            server.Dispose();


        }



    }
}
