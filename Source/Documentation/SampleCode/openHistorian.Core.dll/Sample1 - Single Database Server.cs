using System;
using System.IO;
using GSF.Diagnostics;
using GSF.SortedTreeStore.Net;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Services.Configuration;
using GSF.SortedTreeStore.Services.Net;
using NUnit.Framework;
using openHistorian;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Services.Reader;

namespace SampleCode.openHistorian.Core.dll
{
    [TestFixture]
    public class Sample1
    {
        [Test]
        public void CreateScadaDatabase()
        {
            Logger.ReportToConsole(VerboseLevel.All);

            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            var key = new HistorianKey();
            var value = new HistorianValue();

            var settings = new HistorianServerDatabaseConfig("DB", @"c:\temp\Scada\", true);
            using (var server = new HistorianServer(settings))
            using (var client = Client.Connect(server.Host))
            {
                var database = client.GetDatabase<HistorianKey, HistorianValue>("db");
                for (ulong x = 0; x < 1000; x++)
                {
                    key.Timestamp = x;
                    database.Write(key, value);
                }

                database.HardCommit();
            }
        }

        [Test]
        public void TestReadData()
        {
            using (var server = new HistorianServer(new HistorianServerDatabaseConfig("DB", @"c:\temp\Scada\", false), 1234))
            {
                using (var client = Client.Connect(server.Host))
                {
                    var database = client.GetDatabase<HistorianKey, HistorianValue>("DB");
                    var stream = database.Read(10, 800 - 1);
                    HistorianKey key = new HistorianKey();
                    HistorianValue value = new HistorianValue();
                    while (stream.Read(key, value))
                    {
                        Console.WriteLine(key.Timestamp);
                    }
                }

            }
        }
    }
}