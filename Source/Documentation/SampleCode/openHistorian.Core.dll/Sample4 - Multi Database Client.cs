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
    public class Sample4
    {
        [Test]
        public void CreateAllDatabases()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);
            Array.ForEach(Directory.GetFiles(@"c:\temp\Synchrophasor\", "*.d2", SearchOption.AllDirectories), File.Delete);

            HistorianServerDatabaseConfig config1 = new HistorianServerDatabaseConfig("Scada", @"c:\temp\Scada\", true);
            HistorianServerDatabaseConfig config2 = new HistorianServerDatabaseConfig("Synchrophasor", @"c:\temp\Synchrophasor\", true);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (HistorianServer server = new HistorianServer())
            {
                server.AddDatabase(config1);
                server.AddDatabase(config2);

                using (SnapClient client = SnapClient.Connect(server.Host))
                {
                    ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>("Scada");

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
            HistorianServerDatabaseConfig config1 = new HistorianServerDatabaseConfig("Scada", @"c:\temp\Scada\", true);
            HistorianServerDatabaseConfig config2 = new HistorianServerDatabaseConfig("Synchrophasor", @"c:\temp\Synchrophasor\", true);

            using (HistorianServer server = new HistorianServer(12345))
            {
                server.AddDatabase(config1);
                server.AddDatabase(config2);

                using (HistorianClient client = new HistorianClient("127.0.0.1", 12345))
                {
                    ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>("Scada");
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