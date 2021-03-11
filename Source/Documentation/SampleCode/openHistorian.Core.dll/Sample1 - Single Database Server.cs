using System;
using System.IO;
using GSF.Diagnostics;
using GSF.Snap;
using GSF.Snap.Services;
using NUnit.Framework;
using GSF.Snap.Services.Reader;
using openHistorian.Net;
using openHistorian.Snap;

namespace SampleCode.openHistorian.Core.dll
{
    [TestFixture]
    public class Sample1
    {
        [Test]
        public void CreateScadaDatabase()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("DB", @"c:\temp\Scada\", true);
            using (HistorianServer server = new HistorianServer(settings))
            using (SnapClient client = SnapClient.Connect(server.Host))
            {
                ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>("db");
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
            using (HistorianServer server = new HistorianServer(new HistorianServerDatabaseConfig("DB", @"c:\temp\Scada\", false), 1234))
            {
                using (SnapClient client = SnapClient.Connect(server.Host))
                {
                    ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>("DB");
                    TreeStream<HistorianKey, HistorianValue> stream = database.Read(10, 800 - 1);
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