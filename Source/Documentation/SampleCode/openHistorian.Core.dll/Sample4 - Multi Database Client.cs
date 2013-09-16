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
    public class Sample4
    {
        [Test]
        public void CreateAllDatabases()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);
            Array.ForEach(Directory.GetFiles(@"c:\temp\Synchrophasor\", "*.d2", SearchOption.AllDirectories), File.Delete);

            List<HistorianDatabaseInstance> serverDatabases = new List<HistorianDatabaseInstance>();

            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.DatabaseName = "Scada";
            db.InMemoryArchive = false;
            db.Paths = new[] { @"c:\temp\Scada\" };

            serverDatabases.Add(db);

            db = new HistorianDatabaseInstance();
            db.DatabaseName = "Synchrophasor";
            db.InMemoryArchive = false;
            db.Paths = new[] { @"c:\temp\Synchrophasor\" };

            serverDatabases.Add(db);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (HistorianServer server = new HistorianServer(serverDatabases))
            {
                HistorianDatabaseBase<HistorianKey, HistorianValue> database = server["Scada"];

                for (ulong x = 0; x < 10000; x++)
                {
                    key.Timestamp = x;
                    database.Write(key, value);
                }
                database.HardCommit();
                database.Disconnect();

                database = server["Synchrophasor"];
                for (ulong x = 0; x < 10000; x++)
                {
                    key.Timestamp = x;
                    database.Write(key, value);
                }
                database.HardCommit();
                database.Disconnect();
            }
        }

        [Test]
        public void TestReadData()
        {
            List<HistorianDatabaseInstance> serverDatabases = new List<HistorianDatabaseInstance>();

            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.DatabaseName = "Scada";
            db.InMemoryArchive = true;
            db.Paths = new[] { @"c:\temp\Scada\" };
            db.ConnectionString = "port=12345";

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
                clientOptions.IsReadOnly = true;
                clientOptions.NetworkPort = 12345;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                using (HistorianClient<HistorianKey, HistorianValue> client = new HistorianClient<HistorianKey, HistorianValue>(clientOptions))
                {
                    HistorianDatabaseBase<HistorianKey, HistorianValue> database = client["Scada"];
                    using (HistorianDataReaderBase<HistorianKey, HistorianValue> reader = database.OpenDataReader())
                    {
                        KeyValueStream<HistorianKey, HistorianValue> stream = reader.Read(0, 100);
                        stream.Cancel();
                    }
                    database.Disconnect();

                    database = client["Synchrophasor"];

                    using (HistorianDataReaderBase<HistorianKey, HistorianValue> reader = database.OpenDataReader())
                    {
                        KeyValueStream<HistorianKey, HistorianValue> stream = reader.Read(0, 100);
                        stream.Cancel();
                    }
                    database.Disconnect();
                }
            }
        }
    }
}