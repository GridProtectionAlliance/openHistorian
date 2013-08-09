using System;
using System.IO;
using NUnit.Framework;
using openHistorian;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace SampleCode.openHistorian.Server.dll
{
    [TestFixture]
    public class Sample1
    {
        [Test]
        public void CreateScadaDatabase()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.IsNetworkHosted = false;
            db.InMemoryArchive = false;
            db.Paths = new[] { @"c:\temp\Scada\" };

            using (HistorianServer server = new HistorianServer(db))
            {
                IHistorianDatabase<HistorianKey, HistorianValue> database = server.GetDefaultDatabase();

                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();

                for (ulong x = 0; x < 10000; x++)
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
            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.InMemoryArchive = true;
            db.ConnectionString = "port=1234";
            db.Paths = new[] { @"c:\temp\Scada\" };

            using (HistorianServer server = new HistorianServer(db))
            {
                IHistorianDatabase<HistorianKey, HistorianValue> database = server.GetDefaultDatabase();

                using (HistorianDataReaderBase<HistorianKey, HistorianValue> reader = database.OpenDataReader())
                {
                    TreeStream<HistorianKey, HistorianValue> stream = reader.Read(0, 100);
                    stream.Cancel();
                }
            }
        }
    }
}