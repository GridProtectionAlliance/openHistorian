using System;
using System.Collections.Generic;
using System.IO;
using GSF.SortedTreeStore;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Services.Reader;
using NUnit.Framework;
using openHistorian;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;

namespace SampleCode.openHistorian.Server.dll
{
    [TestFixture]
    public class Sample2
    {
        [Test]
        public void CreateAllDatabases()
        {
            throw new NotImplementedException();

            //Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);
            //Array.ForEach(Directory.GetFiles(@"c:\temp\Synchrophasor\", "*.d2", SearchOption.AllDirectories), File.Delete);

            //List<HistorianDatabaseInstance> serverDatabases = new List<HistorianDatabaseInstance>();

            //HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            //db.DatabaseName = "Scada";
            //db.InMemoryArchive = false;
            //db.Paths = new[] { @"c:\temp\Scada\" };

            //serverDatabases.Add(db);

            //db = new HistorianDatabaseInstance();
            //db.DatabaseName = "Synchrophasor";
            //db.InMemoryArchive = false;
            //db.Paths = new[] { @"c:\temp\Synchrophasor\" };

            //serverDatabases.Add(db);

            //HistorianKey key = new HistorianKey();
            //HistorianValue value = new HistorianValue();

            //using (HistorianServer server = new HistorianServer(serverDatabases))
            //{
            //    ServerDatabaseBase database = server["Scada"];

            //    for (ulong x = 0; x < 10000; x++)
            //    {
            //        key.Timestamp = x;
            //        database.Write(key, value);
            //    }
            //    database.HardCommit();

            //    database = server["Synchrophasor"];
            //    for (ulong x = 0; x < 10000; x++)
            //    {
            //        key.Timestamp = x;
            //        database.Write(key, value);
            //    }
            //    database.HardCommit();
            //}
        }

        [Test]
        public void TestReadData()
        {
            throw new NotImplementedException();

            //List<HistorianDatabaseInstance> serverDatabases = new List<HistorianDatabaseInstance>();

            //HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            //db.DatabaseName = "Scada";
            //db.InMemoryArchive = true;
            //db.Paths = new[] { @"c:\temp\Scada\" };

            //serverDatabases.Add(db);

            //db = new HistorianDatabaseInstance();
            //db.DatabaseName = "Synchrophasor";
            //db.InMemoryArchive = true;
            //db.Paths = new[] { @"c:\temp\Synchrophasor\" };

            //serverDatabases.Add(db);

            //using (HistorianServer server = new HistorianServer(serverDatabases))
            //{
            //    ServerDatabaseBase database = server["Scada"];
            //    TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, 100);
            //    stream.Cancel();

            //    database = server["Synchrophasor"];
            //    stream = database.Read(0, 100);
            //    stream.Cancel();
            //}
        }
    }
}