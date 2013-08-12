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

            //Console.WriteLine("KeyMethodsBase calls");
            //for (int x = 0; x < 23; x++)
            //{
            //    Console.WriteLine(TreeKeyMethodsBase<HistorianKey>.CallMethods[x] + "\t" + ((TreeKeyMethodsBase<HistorianKey>.Method)(x)).ToString());
            //}
            //Console.WriteLine("ValueMethodsBase calls");
            //for (int x = 0; x < 5; x++)
            //{
            //    Console.WriteLine(TreeValueMethodsBase<HistorianValue>.CallMethods[x] + "\t" + ((TreeValueMethodsBase<HistorianValue>.Method)(x)).ToString());
            //}
        }

        [Test]
        public void BenchmarkWriteSpeed()
        {
            DebugStopwatch sw = new DebugStopwatch();

            double time;
            double count = 0;

            using (ArchiveFile file = ArchiveFile.CreateInMemory())
            {
                var table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(CreateHistorianCompressionTs.TypeGuid);
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();

                time = sw.TimeEvent(() =>
                {
                    //TreeKeyMethodsBase<HistorianKey>.ClearStats();
                    //TreeValueMethodsBase<HistorianKey>.ClearStats();
                    count = 0;
                    using (var scan = table.BeginEdit())
                    {
                        for (uint x = 0; x < 10000000; x++)
                        {
                            key.PointID = x;
                            scan.AddPoint(key, value);
                            count++;
                        }
                        scan.Rollback();
                    }
                });
            }

            Console.WriteLine((count / 1000000 / time).ToString() + " Million PPS");

            //Console.WriteLine("KeyMethodsBase calls");
            //for (int x = 0; x < 23; x++)
            //{
            //    Console.WriteLine(TreeKeyMethodsBase<HistorianKey>.CallMethods[x] + "\t" + ((TreeKeyMethodsBase<HistorianKey>.Method)(x)).ToString());
            //}
            //Console.WriteLine("ValueMethodsBase calls");
            //for (int x = 0; x < 5; x++)
            //{
            //    Console.WriteLine(TreeValueMethodsBase<HistorianValue>.CallMethods[x] + "\t" + ((TreeValueMethodsBase<HistorianValue>.Method)(x)).ToString());
            //}
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
                double count = 0;

                DebugStopwatch sw = new DebugStopwatch();
                double time = sw.TimeEvent(() =>
                    {
                        count = 0;
                        using (HistorianClient<HistorianKey, HistorianValue> client = new HistorianClient<HistorianKey, HistorianValue>(clientOptions))
                        {
                            IHistorianDatabase<HistorianKey, HistorianValue> database = client.GetDatabase();//.GetDatabase();
                            //IHistorianDatabase<HistorianKey, HistorianValue> database = server.GetDefaultDatabase();//.GetDatabase();
                            using (HistorianDataReaderBase<HistorianKey, HistorianValue> reader = database.OpenDataReader())
                            {
                                //TreeStream<HistorianKey, HistorianValue> stream = reader.Read(0, ulong.MaxValue, new ulong[] { 2 });
                                TreeStream<HistorianKey, HistorianValue> stream = reader.Read(0, ulong.MaxValue);
                                while (stream.Read())
                                {
                                    count++;
                                }
                            }
                            database.Disconnect();
                        }
                    });

                Console.WriteLine((count / 1000000 / time).ToString() + " Million PPS");
            }

            //Console.WriteLine("KeyMethodsBase calls");
            //for (int x = 0; x < 23; x++)
            //{
            //    Console.WriteLine(TreeKeyMethodsBase<HistorianKey>.CallMethods[x] + "\t" + ((TreeKeyMethodsBase<HistorianKey>.Method)(x)).ToString());
            //}
            //Console.WriteLine("ValueMethodsBase calls");
            //for (int x = 0; x < 5; x++)
            //{
            //    Console.WriteLine(TreeValueMethodsBase<HistorianValue>.CallMethods[x] + "\t" + ((TreeValueMethodsBase<HistorianValue>.Method)(x)).ToString());
            //}
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
            double count = 0;

            using (ArchiveFile file = ArchiveFile.OpenFile(path, true))
            {
                var table = file.OpenTable<HistorianKey, HistorianValue>();

                time = sw.TimeEvent(() =>
                    {
                        count = 0;
                        using (var scan = table.BeginRead())
                        {
                            var t = scan.GetTreeScanner();
                            t.SeekToStart();
                            while (t.Read())
                            {
                                count++;
                            }
                        }
                    });
            }
            Console.WriteLine((count / 1000000 / time).ToString() + " Million PPS");

            //Console.WriteLine("KeyMethodsBase calls");
            //for (int x = 0; x < 23; x++)
            //{
            //    Console.WriteLine(TreeKeyMethodsBase<HistorianKey>.CallMethods[x] + "\t" + ((TreeKeyMethodsBase<HistorianKey>.Method)(x)).ToString());
            //}
            //Console.WriteLine("ValueMethodsBase calls");
            //for (int x = 0; x < 5; x++)
            //{
            //    Console.WriteLine(TreeValueMethodsBase<HistorianValue>.CallMethods[x] + "\t" + ((TreeValueMethodsBase<HistorianValue>.Method)(x)).ToString());
            //}
        }
    }
}