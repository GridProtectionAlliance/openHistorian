using System;
using System.IO;
using GSF.IO;
using NUnit.Framework;
using openHistorian;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;
using openHistorian.Data.Query;

namespace SampleCode.openHistorian.Server.dll
{
    [TestFixture]
    public class SocketBenchmark
    {
        [Test]
        public void CreateScadaDatabase()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.IsNetworkHosted = false;
            db.InMemoryArchive = false;
            db.Paths = new[] { @"c:\temp\Scada\" };

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (HistorianServer server = new HistorianServer(db))
            {
                IHistorianDatabase<HistorianKey, HistorianValue> database = server.GetDefaultDatabase();

                for (ulong x = 0; x < 10000000; x++)
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
            db.ConnectionString = "port=12345";
            db.Paths = new[] { @"c:\temp\Scada\" };

            using (HistorianServer server = new HistorianServer(db))
            {
                HistorianClientOptions clientOptions = new HistorianClientOptions();
                clientOptions.IsReadOnly = true;
                clientOptions.NetworkPort = 12345;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                DebugStopwatch sw = new DebugStopwatch();
                double time = sw.TimeEvent(() =>
                    {
                        int count=0;
                        using (HistorianClient<HistorianKey, HistorianValue> client = new HistorianClient<HistorianKey, HistorianValue>(clientOptions))
                        {
                            IHistorianDatabase<HistorianKey, HistorianValue> database = client.GetDatabase();//.GetDatabase();
                            //IHistorianDatabase<HistorianKey, HistorianValue> database = server.GetDefaultDatabase();//.GetDatabase();
                            using (HistorianDataReaderBase<HistorianKey, HistorianValue> reader = database.OpenDataReader())
                            {
                                TreeStream<HistorianKey, HistorianValue> stream = reader.Read(0, 1000000000);
                                while (stream.Read())
                                {
                                    count++;
                                }
                            }
                            database.Disconnect();
                        }
                    });

                Console.WriteLine((10.0 / time).ToString() + " Million PPS");
            }

            //for (int x = 0; x < 15; x++)
            //{
            //    Console.WriteLine(BinaryStreamBase.CallMethods[x] + "\t" + ((BinaryStreamBase.Method)(x)).ToString());
            //}
        }

        [Test]
        public void TestReadDataFromArchive()
        {
            
            DebugStopwatch sw = new DebugStopwatch();

            string path = Directory.GetFiles(@"c:\temp\Scada\", "*.d2")[0];
            double time;

            using (ArchiveFile file = ArchiveFile.OpenFile(path, true))
            {
                var table = file.OpenTable<HistorianKey, HistorianValue>();

                time = sw.TimeEvent(() =>
                    {

                        using (var scan = table.BeginRead())
                        {
                            var t = scan.GetTreeScanner();
                            t.SeekToStart();
                            while (t.Read())
                            {

                            }
                        }
                    });
            }
            Console.WriteLine((1.0 / time).ToString() + " Million PPS");
        }
    }
}