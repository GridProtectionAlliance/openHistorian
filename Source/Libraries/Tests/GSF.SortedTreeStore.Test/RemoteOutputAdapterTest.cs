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
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (HistorianServer server = new HistorianServer(@"c:\temp\historian\"))
            using (var client = server.Host.CreateClientHost())
            {
                using (HistorianInputQueue queue = new HistorianInputQueue(() => client.GetDatabase<HistorianKey, HistorianValue>()))
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