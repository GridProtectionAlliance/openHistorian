using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using OpenHistorian.Adapters;
using openHistorian.Queues;

namespace openHistorian.Adapters
{
    [TestFixture]
    class RemoteOutputAdapterTest
    {
        [Test]
        public void TestRemoteAdapter()
        {
            var serverSettings = new HistorianServerOptions();
            serverSettings.IsNetworkHosted = false;
            serverSettings.Paths.Add(@"c:\temp\historian\");
            serverSettings.IsReadOnly = false;
            using (var server = new HistorianServer(serverSettings))
            {
                using (var queue = new HistorianInputQueue(() => server.GetDatabase()))
                {
                    for (uint x = 0; x < 100000; x++)
                    {
                        queue.Enqueue(0,x,0,0);
                    }
                    Thread.Sleep(100);
                }
                Thread.Sleep(100);
            }

            //Thread.Sleep(100);
        }


    }
}
