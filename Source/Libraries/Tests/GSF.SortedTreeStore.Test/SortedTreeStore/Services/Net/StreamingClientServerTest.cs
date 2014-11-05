using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Security;
using GSF.SortedTreeStore.Services.Configuration;
using NUnit.Framework;
using openHistorian;

namespace GSF.SortedTreeStore.Services.Net
{
    [TestFixture]
    public class StreamingClientServerTest
    {
        [Test]
        public void Test1()
        {
            Logger.ReportToConsole(VerboseLevel.All);

            var netStream = new NetworkStreamSimulator();

            var dbcfg = new HistorianServerDatabaseConfig("DB", @"C:\Temp\Historian", true);
            var server = new HistorianServer(dbcfg);
            var auth = new SecureStreamServer<SocketUserPermissions>();
            auth.SetDefaultUser(true, new SocketUserPermissions()
            {
                CanRead = true,
                CanWrite = true,
                IsAdmin = true
            });

            var netServer = new StreamingServer(auth, netStream.ServerStream, server.Host, null);

            ThreadPool.QueueUserWorkItem(ProcessClient, netServer);

            var client = new StreamingClient(netStream.ClientStream, new SecureStreamClientDefault(), true);

            var db  = client.GetDatabase("DB");

            client.Dispose();
            server.Dispose();

        }

        private void ProcessClient(object netServer)
        {
            ((StreamingServer)netServer).ProcessClient();
        }

    }
}
