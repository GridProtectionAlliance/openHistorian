using System.Threading;
using GSF.Snap.Services;
using NUnit.Framework;
using openHistorian.Net;
using openHistorian.Queues;
using openHistorian.Snap;

namespace openHistorian.Adapters
{
    [TestFixture]
    internal class RemoteOutputAdapterTest
    {
        [Test]
        public void TestRemoteAdapter()
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("PPA", @"c:\temp\historian\", true);

            using (HistorianServer server = new HistorianServer(settings))
            using (SnapClient client = SnapClient.Connect(server.Host))
            {
                using (HistorianInputQueue queue = new HistorianInputQueue(() => client.GetDatabase<HistorianKey, HistorianValue>(string.Empty)))
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