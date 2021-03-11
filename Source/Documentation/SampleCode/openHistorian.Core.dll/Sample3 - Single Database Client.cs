using System;
using System.IO;
using GSF.Snap;
using GSF.Snap.Services;
using NUnit.Framework;
using GSF.Snap.Services.Reader;
using openHistorian.Net;
using openHistorian.Snap;

namespace SampleCode.openHistorian.Core.dll
{
    [TestFixture]
    public class Sample3
    {
        [Test]
        public void CreateScadaDatabase()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("DB", @"c:\temp\Scada\", true);
            using (HistorianServer server = new HistorianServer(settings, 12345))
            {
                using (HistorianClient client = new HistorianClient("127.0.0.1", 12345))
                using (ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
                {
                    for (ulong x = 0; x < 1000; x++)
                    {
                        key.Timestamp = x;
                        database.Write(key, value);
                    }

                    database.HardCommit();
                    System.Threading.Thread.Sleep(1200);
                }
            }
        }

        [Test]
        public void TestReadData()
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("DB", @"c:\temp\Scada\", true);
            using (HistorianServer server = new HistorianServer(settings, 12345))
            {
                using (HistorianClient client = new HistorianClient("127.0.0.1", 12345))
                using (ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
                {
                    TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, 1000);
                    while (stream.Read(key, value))
                        Console.WriteLine(key.Timestamp);
                }
            }
        }
    }
}