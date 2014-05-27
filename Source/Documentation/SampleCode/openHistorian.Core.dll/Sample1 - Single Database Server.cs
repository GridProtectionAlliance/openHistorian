using System;
using System.IO;
using GSF.SortedTreeStore.Services;
using NUnit.Framework;
using openHistorian;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Services.Reader;

namespace SampleCode.openHistorian.Server.dll
{
    [TestFixture]
    public class Sample1
    {
        [Test]
        public void CreateScadaDatabase()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            var key = new HistorianKey();
            var value = new HistorianValue();

            using (var server = new HistorianServer(@"c:\temp\Scada\"))
            using (var client = Client.Connect(server.Host))
            {
                var database = client.GetDatabase<HistorianKey, HistorianValue>(string.Empty);
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
            throw new NotImplementedException();
            //var db = new HistorianDatabaseInstance();
            //db.InMemoryArchive = false;
            //db.ConnectionString = "port=1234";
            //db.Paths = new[] { @"c:\temp\Scada\" };

            //using (var server = new HistorianServer(db))
            //{
            //    var database = server.GetDefaultDatabase();
            //    var stream = database.Read(10, 800 - 1);
            //    while (stream.Read())
            //    {
            //        Console.WriteLine(stream.CurrentKey.Timestamp);
            //    }
            //}
        }
    }
}