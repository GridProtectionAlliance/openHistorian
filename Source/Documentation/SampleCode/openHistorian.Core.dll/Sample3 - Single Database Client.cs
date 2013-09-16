using System;
using System.IO;
using NUnit.Framework;
using openHistorian;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

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
                HistorianClientOptions clientOptions = new HistorianClientOptions();
                clientOptions.NetworkPort = 12345;
                clientOptions.ServerNameOrIp = "127.0.0.1";
                
                using (var client = new HistorianClient<HistorianKey, HistorianValue>(clientOptions))
                {
                    var database = client.GetDefaultDatabase();

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
                HistorianClientOptions clientOptions = new HistorianClientOptions();
                clientOptions.NetworkPort = 12345;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                using (var client = new HistorianClient<HistorianKey, HistorianValue>(clientOptions))
                {
                    var database = client.GetDefaultDatabase();
                    using (var reader = database.OpenDataReader())
                    {
                        var stream = reader.Read(0, 1000);
                        while (stream.Read())
                            Console.WriteLine(stream.CurrentKey.Timestamp);

                    }
                    database.Disconnect();
                }
            }
        }
    }
}