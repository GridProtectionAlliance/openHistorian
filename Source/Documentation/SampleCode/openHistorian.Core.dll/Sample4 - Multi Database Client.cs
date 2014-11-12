using System;
using System.Collections.Generic;
using System.IO;
using GSF.Diagnostics;
using GSF.SortedTreeStore;
using GSF.SortedTreeStore.Net;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Services.Configuration;
using NUnit.Framework;
using openHistorian;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Services.Reader;

namespace SampleCode.openHistorian.Core.dll
{
    [TestFixture]
    public class Sample4
    {
        [Test]
        public void CreateAllDatabases()
        {
            Logger.ReportToConsole(VerboseLevel.All);

            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);
            Array.ForEach(Directory.GetFiles(@"c:\temp\Synchrophasor\", "*.d2", SearchOption.AllDirectories), File.Delete);

            var config1 = new HistorianServerDatabaseConfig("Scada", @"c:\temp\Scada\", true);
            var config2 = new HistorianServerDatabaseConfig("Synchrophasor", @"c:\temp\Synchrophasor\", true);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (HistorianServer server = new HistorianServer())
            {
                server.AddDatabase(config1);
                server.AddDatabase(config2);

                using (var client = Client.Connect(server.Host))
                {
                    var database = client.GetDatabase<HistorianKey, HistorianValue>("Scada");

                    for (ulong x = 0; x < 10000; x++)
                    {
                        key.Timestamp = x;
                        database.Write(key, value);
                    }
                    database.HardCommit();

                    database = client.GetDatabase<HistorianKey, HistorianValue>("Synchrophasor");

                    for (ulong x = 0; x < 10000; x++)
                    {
                        key.Timestamp = x;
                        database.Write(key, value);
                    }
                    database.HardCommit();
                }
            }
        }

        [Test]
        public void TestReadData()
        {
            var config1 = new HistorianServerDatabaseConfig("Scada", @"c:\temp\Scada\", true);
            var config2 = new HistorianServerDatabaseConfig("Synchrophasor", @"c:\temp\Synchrophasor\", true);

            using (HistorianServer server = new HistorianServer(12345))
            {
                server.AddDatabase(config1);
                server.AddDatabase(config2);

                using (var client = new HistorianClient("127.0.0.1", 12345))
                {
                    var database = client.GetDatabase<HistorianKey, HistorianValue>("Scada");
                    TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, 100);
                    stream.Dispose();
                    database.Dispose();

                    database = client.GetDatabase<HistorianKey, HistorianValue>("Synchrophasor");
                    stream = database.Read(0, 100);
                    stream.Dispose();
                    database.Dispose();
                }
            }
        }
    }
}