//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using GSF;
//using GSF.SortedTreeStore;
//using GSF.SortedTreeStore.Services.Net;
//using GSF.SortedTreeStore.Services;
//using GSF.SortedTreeStore.Net;
//using GSF.SortedTreeStore.Net.Compression;
//using GSF.SortedTreeStore.Services.Net;
//using GSF.SortedTreeStore.Tree.TreeNodes;
//using NUnit.Framework;
//using openHistorian.Collections;

//namespace openHistorian.PerformanceTests.SortedTreeStore.Engine
//{
//    [TestFixture]
//    public class BulkWritePoints
//    {
//        volatile bool Quit;
//        volatile int PointCount;
//        SortedList<double, int> PointSamples;


//        [Test]
//        public void VerifyDB()
//        {
//            using (var engine = new ServerDatabase<HistorianKey, HistorianValue>("DB", WriterMode.OnDisk, CreateHistorianCompressionTs.TypeGuid, "c:\\temp\\benchmark\\"))
//            using (var scan = engine.Read(null, null, null, null))
//            {
//                var key = new HistorianKey();
//                var value = new HistorianValue();
//                for (int x = 0; x < 100000000; x++)
//                {
//                    if (!scan.Read(key, value))
//                        throw new Exception("Missing points");
//                    if (key.PointID != (ulong)x)
//                        throw new Exception("Corrupt");
//                    if (key.Timestamp != 0)
//                        throw new Exception("Corrupt");
//                    if (key.EntryNumber != 0)
//                        throw new Exception("Corrupt");
//                    if (value.Value1 != 0)
//                        throw new Exception("Corrupt");
//                    if (value.Value1 != 0)
//                        throw new Exception("Corrupt");
//                    if (value.Value1 != 0)
//                        throw new Exception("Corrupt");
//                }
//                if (scan.Read())
//                    throw new Exception("too many points");
//            }
//        }

//        [Test]
//        public void TestWriteSpeedSocket()
//        {
//            Thread th = new Thread(WriteSpeed);
//            th.IsBackground = true;
//            th.Start();

//            Quit = false;
//            foreach (var file in Directory.GetFiles("c:\\temp\\benchmark\\"))
//                File.Delete(file);

//            PointCount = 0;
//            var collection = new Server();
//            using (var engine = new ServerDatabase<HistorianKey, HistorianValue>("DB", WriterMode.OnDisk, CreateHistorianCompressionTs.TypeGuid, "c:\\temp\\benchmark\\"))
//            using (var socket = new SocketListener(13141, collection))
//            {
//                collection.Add(engine);

//                var options = new RemoteClientOptions();
//                options.ServerNameOrIp = "127.0.0.1";
//                options.NetworkPort = 13141;

//                using (var client = new RemoteClient(options))
//                using (var db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
//                {
//                    db.SetEncodingMode(CreateHistorianCompressedStream.TypeGuid);

//                    engine.ProcessException += engine_Exception;
//                    Thread.Sleep(100);
//                    var key = new HistorianKey();
//                    var value = new HistorianValue();

//                    using (var writer = db.StartBulkWriting())
//                    {
//                        for (int x = 0; x < 100000000; x++)
//                        {
//                            key.PointID = (ulong)x;
//                            PointCount = x;
//                            writer.Write(key, value);
//                        }
//                    }

//                    Quit = true;
//                    th.Join();
//                }
//            }

//            Console.WriteLine("Time (sec)\tPoints");
//            foreach (var kvp in PointSamples)
//            {
//                Console.WriteLine(kvp.Key.ToString() + "\t" + kvp.Value.ToString());
//            }
//        }

//        [Test]
//        public void TestWriteSpeed()
//        {
//            Thread th = new Thread(WriteSpeed);
//            th.IsBackground = true;
//            th.Start();

//            Quit = false;
//            foreach (var file in Directory.GetFiles("c:\\temp\\benchmark\\"))
//                File.Delete(file);

//            PointCount = 0;
//            using (var engine = new ServerDatabase<HistorianKey, HistorianValue>("DB", WriterMode.OnDisk, CreateHistorianCompressionTs.TypeGuid, "c:\\temp\\benchmark\\"))
//            {
//                engine.ProcessException += engine_Exception;
//                Thread.Sleep(100);
//                var key = new HistorianKey();
//                var value = new HistorianValue();

//                for (int x = 0; x < 100000000; x++)
//                {
//                    key.PointID = (ulong)x;
//                    PointCount = x;
//                    engine.Write(key, value);
//                }
//                Quit = true;
//                th.Join();
//            }

//            Console.WriteLine("Time (sec)\tPoints");
//            foreach (var kvp in PointSamples)
//            {
//                Console.WriteLine(kvp.Key.ToString() + "\t" + kvp.Value.ToString());
//            }
//        }

//        [Test]
//        public void TestWriteSpeedRandom()
//        {
//            Thread th = new Thread(WriteSpeed);
//            Random r = new Random(1);

//            th.IsBackground = true;
//            th.Start();

//            Quit = false;
//            foreach (var file in Directory.GetFiles("c:\\temp\\benchmark\\"))
//                File.Delete(file);

//            PointCount = 0;
//            using (var engine = new ServerDatabase<HistorianKey, HistorianValue>("DB", WriterMode.OnDisk, CreateHistorianCompressionTs.TypeGuid, "c:\\temp\\benchmark\\"))
//            {
//                engine.ProcessException += engine_Exception;
//                Thread.Sleep(100);
//                var key = new HistorianKey();
//                var value = new HistorianValue();

//                for (int x = 0; x < 10000000; x++)
//                {
//                    key.Timestamp = (ulong)r.Next();
//                    key.PointID = (ulong)x;

//                    PointCount = x;
//                    engine.Write(key, value);
//                }
//                Quit = true;
//                th.Join();
//            }

//            Console.WriteLine("Time (sec)\tPoints");
//            foreach (var kvp in PointSamples)
//            {
//                Console.WriteLine(kvp.Key.ToString() + "\t" + kvp.Value.ToString());
//            }
//        }

//        void engine_Exception(object sender, EventArgs<Exception> e)
//        {
//            Console.WriteLine(e.Argument.ToString());
//        }

//        void WriteSpeed()
//        {
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            PointSamples = new SortedList<double, int>();

//            while (!Quit)
//            {
//                double elapsed = sw.Elapsed.TotalSeconds;
//                PointSamples.Add(elapsed, PointCount);
//                int sleepTime = (int)(elapsed * 1000) % 100;
//                sleepTime = 100 - sleepTime;
//                if (sleepTime < 50)
//                    sleepTime += 100;
//                Thread.Sleep(sleepTime);
//            }
//        }

//    }
//}
