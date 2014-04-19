using System;
using System.IO;
using GSF.SortedTreeStore.Net;
using NUnit.Framework;
using openHistorian;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Server.Reader;

namespace SampleCode.openHistorian.Server.dll
{
    [TestFixture]
    public class Sample3
    {
        [Test]
        public void CreateScadaDatabase()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.IsNetworkHosted = false;
            db.InMemoryArchive = false;
            db.ConnectionString = "port=12345";
            db.Paths = new[] { @"c:\temp\Scada\" };

            var key = new HistorianKey();
            var value = new HistorianValue();

            using (var server = new HistorianServer(db))
            {
                SortedTreeClientOptions clientOptions = new SortedTreeClientOptions();
                clientOptions.NetworkPort = 12345;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                using (var client = new HistorianClient(clientOptions))
                {
                    var database = client.GetDefaultDatabase<HistorianKey, HistorianValue>();

                    for (ulong x = 0; x < 1000; x++)
                    {
                        key.Timestamp = x;
                        database.Write(key, value);
                    }

                    database.HardCommit();
                    System.Threading.Thread.Sleep(1200);
                    database.Disconnect();
                }


            }
        }

        [Test]
        public void TestReadData()
        {
            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.InMemoryArchive = true;
            db.ConnectionString = "port=12345";
            db.Paths = new[] { @"c:\temp\Scada\" };

            using (HistorianServer server = new HistorianServer(db))
            {
                SortedTreeClientOptions clientOptions = new SortedTreeClientOptions();
                clientOptions.NetworkPort = 12345;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                using (var client = new HistorianClient(clientOptions))
                {
                    var database = client.GetDefaultDatabase<HistorianKey, HistorianValue>();
                    var stream = database.Read(0, 1000);
                    while (stream.Read())
                        Console.WriteLine(stream.CurrentKey.Timestamp);

                    database.Disconnect();
                }
            }
        }
    }
}