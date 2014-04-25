using System.Threading;
using NUnit.Framework;
using openHistorian.Collections;
using openHistorian.Queues;

namespace openHistorian.Adapters
{
    [TestFixture]
    internal class RemoteOutputAdapterTest
    {
        [Test]
        public void TestRemoteAdapter()
        {
            HistorianDatabaseInstance serverSettings = new HistorianDatabaseInstance();
            serverSettings.IsNetworkHosted = false;
            serverSettings.Paths = new[] { @"c:\temp\historian\" };
            serverSettings.InMemoryArchive = false;

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (HistorianServer server = new HistorianServer(serverSettings))
            using (var client = server.CreateClient())
            {
                using (HistorianInputQueue queue = new HistorianInputQueue(() => client.GetDatabase<HistorianKey,HistorianValue>(serverSettings.DatabaseName)))
                {
                    for (uint x = 0; x < 100000; x++)
                    {
                        key.PointID = x;
                        queue.Enqueue(key, value);
                    }
                    Thread.Sleep(100);
                }
                Thread.Sleep(100);
            }
            //Thread.Sleep(100);
        }
    }
}