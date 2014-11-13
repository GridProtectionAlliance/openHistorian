using System;
using System.IO;
using GSF.Snap.Services.Configuration;
using GSF.Snap.Services.Net;
using GSF.Snap.Net;
using NUnit.Framework;
using openHistorian;
using openHistorian.Collections;
using GSF.Snap.Tree;
using GSF.Snap.Services.Reader;

namespace SampleCode.openHistorian.Core.dll
{
    [TestFixture]
    public class Sample3
    {
        [Test]
        public void CreateScadaDatabase()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            var key = new HistorianKey();
            var value = new HistorianValue();

            var settings = new HistorianServerDatabaseConfig("DB", @"c:\temp\Scada\", true);
            using (var server = new HistorianServer(settings, 12345))
            {
                using (var client = new HistorianClient("127.0.0.1", 12345))
                using (var database = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
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
            var key = new HistorianKey();
            var value = new HistorianValue();

            var settings = new HistorianServerDatabaseConfig("DB", @"c:\temp\Scada\", true);
            using (HistorianServer server = new HistorianServer(settings, 12345))
            {
                using (var client = new HistorianClient("127.0.0.1", 12345))
                using (var database = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
                {
                    var stream = database.Read(0, 1000);
                    while (stream.Read(key, value))
                        Console.WriteLine(key.Timestamp);
                }
            }
        }
    }
}