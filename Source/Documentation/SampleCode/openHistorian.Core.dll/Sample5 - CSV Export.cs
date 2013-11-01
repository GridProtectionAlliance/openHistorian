using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using openHistorian;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace SampleCode.openHistorian.Server.dll
{
    [TestFixture]
    public class Sample5
    {
        [Test]
        public void TestReadData()
        {
            List<HistorianDatabaseInstance> serverDatabases = new List<HistorianDatabaseInstance>();

            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.DatabaseName = "PPA";
            db.InMemoryArchive = true;
            db.Paths = new[] { @"c:\temp\Scada\" };
            db.ConnectionString = "port=38409";

            serverDatabases.Add(db);

            db = new HistorianDatabaseInstance();
            db.DatabaseName = "Synchrophasor";
            db.InMemoryArchive = true;
            db.Paths = new[] { @"c:\temp\Synchrophasor\" };
            db.ConnectionString = "port=12345";

            serverDatabases.Add(db);

            using (HistorianServer server = new HistorianServer(serverDatabases))
            {
                HistorianClientOptions clientOptions = new HistorianClientOptions();
                clientOptions.DefaultDatabase = "PPA";
                clientOptions.NetworkPort = 38409;
                clientOptions.ServerNameOrIp = "127.0.0.1"; //IP address of server.

                using (HistorianClient<HistorianKey, HistorianValue> client = new HistorianClient<HistorianKey, HistorianValue>(clientOptions))
                {
                    var database = client.GetDefaultDatabase();
                    using (var reader = database.OpenDataReader())
                    {
                        using (var csvStream = new StreamWriter("C:\\temp\\file.csv"))
                        {
                            csvStream.Write("Timestamp,PointID,Value,Quality");
                            KeyValueStream<HistorianKey, HistorianValue> stream = reader.Read(DateTime.MinValue, DateTime.MaxValue, new ulong[] { 1, 2, 3 });
                            while (stream.Read())
                            {
                                csvStream.WriteLine("{0},{1},{2},{3}", stream.CurrentKey.TimestampAsDate, stream.CurrentKey.PointID, stream.CurrentValue.AsSingle, stream.CurrentValue.Value3);
                            }
                            csvStream.Flush();
                        }
                    }
                    database.Disconnect();
                }
            }
        }
    }
}