using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GSF;
using GSF.Diagnostics;
using GSF.IO.Unmanaged;
using GSF.Snap;
using GSF.Snap.Services;
using NUnit.Framework;
using openHistorian.Net;
using openHistorian.Snap;

namespace openHistorian.PerformanceTests.SortedTreeStore.Engine
{
    [TestFixture]
    public class BulkWritePoints
    {
        private const int PointsToArchive = 100000000;

        volatile bool Quit;
        volatile int PointCount;
        SortedList<double, int> PointSamples;

        [Test]
        public void VerifyDB()
        {
            //Logger.ReportToConsole(VerboseLevel.All ^ VerboseLevel.DebugLow);
            //Logger.ConsoleSubscriber.AddIgnored(Logger.LookupType("GSF.SortedTreeStore"));
            Globals.MemoryPool.SetMaximumBufferSize(1000 * 1024 * 1024);
            Globals.MemoryPool.SetTargetUtilizationLevel(TargetUtilizationLevels.Low);

            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("DB", "c:\\temp\\benchmark\\", true);
            using (SnapServer engine = new SnapServer(settings))
            using (SnapClient client = SnapClient.Connect(engine))
            using (ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
            using (TreeStream<HistorianKey, HistorianValue> scan = db.Read(null, null, null))
            {
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();

                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int x = 0; x < PointsToArchive; x++)
                {
                    if (!scan.Read(key, value))
                        throw new Exception("Missing points");
                    if (key.PointID != (ulong)x)
                        throw new Exception("Corrupt");
                    if (key.Timestamp != 0)
                        throw new Exception("Corrupt");
                    if (key.EntryNumber != 0)
                        throw new Exception("Corrupt");
                    if (value.Value1 != 0)
                        throw new Exception("Corrupt");
                    if (value.Value1 != 0)
                        throw new Exception("Corrupt");
                    if (value.Value1 != 0)
                        throw new Exception("Corrupt");
                }

                double totalTime = sw.Elapsed.TotalSeconds;
                Console.WriteLine("Completed read test in {0:#,##0.00} seconds at {1:#,##0.00} points per second", totalTime, PointsToArchive / totalTime);

                if (scan.Read(key, value))
                    throw new Exception("too many points");
            }
        }

        //[Test]
        //public void TestWriteSpeedSocket()
        //{
        //    Thread th = new Thread(WriteSpeed);
        //    th.IsBackground = true;
        //    th.Start();

        //    Quit = false;
        //    foreach (var file in Directory.GetFiles("c:\\temp\\benchmark\\"))
        //        File.Delete(file);

        //    PointCount = 0;
        //    var collection = new Server();
        //    using (var engine = new ServerDatabase<HistorianKey, HistorianValue>("DB", WriterMode.OnDisk, CreateHistorianCompressionTs.TypeGuid, "c:\\temp\\benchmark\\"))
        //    using (var socket = new SocketListener(13141, collection))
        //    {
        //        collection.Add(engine);

        //        var options = new RemoteClientOptions();
        //        options.ServerNameOrIp = "127.0.0.1";
        //        options.NetworkPort = 13141;

        //        using (var client = new RemoteClient(options))
        //        using (var db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
        //        {
        //            db.SetEncodingMode(CreateHistorianCompressedStream.TypeGuid);

        //            engine.ProcessException += engine_Exception;
        //            Thread.Sleep(100);
        //            var key = new HistorianKey();
        //            var value = new HistorianValue();

        //            using (var writer = db.StartBulkWriting())
        //            {
        //                for (int x = 0; x < 100000000; x++)
        //                {
        //                    key.PointID = (ulong)x;
        //                    PointCount = x;
        //                    writer.Write(key, value);
        //                }
        //            }

        //            Quit = true;
        //            th.Join();
        //        }
        //    }

        //    Console.WriteLine("Time (sec)\tPoints");
        //    foreach (var kvp in PointSamples)
        //    {
        //        Console.WriteLine(kvp.Key.ToString() + "\t" + kvp.Value.ToString());
        //    }
        //}

        [Test]
        public void TestWriteSpeed()
        {
            //Logger.ReportToConsole(VerboseLevel.All ^ VerboseLevel.DebugLow);
            //Logger.SetLoggingPath("c:\\temp\\");

            Globals.MemoryPool.SetMaximumBufferSize(4000 * 1024 * 1024L);

            //Thread th = new Thread(WriteSpeed);
            //th.IsBackground = true;
            //th.Start();

            //Quit = false;
            foreach (string file in Directory.GetFiles("c:\\temp\\benchmark\\", "*.*", SearchOption.AllDirectories))
                File.Delete(file);

            //PointCount = 0;

            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("DB", "c:\\temp\\benchmark\\", true);

            using (SnapServer engine = new SnapServer(settings))
            using (SnapClient client = SnapClient.Connect(engine))
            using (ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
            {
                Thread.Sleep(100);
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();

                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int x = 0; x < PointsToArchive; x++)
                {
                    key.PointID = (ulong)x;
                    //PointCount = x;
                    db.Write(key, value);
                }

                double totalTime = sw.Elapsed.TotalSeconds;
                Console.WriteLine("Completed write test in {0:#,##0.00} seconds at {1:#,##0.00} points per second", totalTime, PointsToArchive / totalTime);
            }
            //Quit = true;
            //th.Join();

            //Console.WriteLine("Time (sec)\tPoints");
            //foreach (var kvp in PointSamples)
            //{
            //    Console.WriteLine(kvp.Key.ToString() + "\t" + kvp.Value.ToString());
            //}

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100);
        }

        [Test]
        public void TestWriteSpeedRandom()
        {

            Logger.Console.Verbose = VerboseLevel.All;

            Random r = new Random(1);
            Thread th = new Thread(WriteSpeed);
            th.IsBackground = true;
            th.Start();

            Quit = false;
            foreach (string file in Directory.GetFiles("c:\\temp\\benchmark\\"))
                File.Delete(file);

            PointCount = 0;

            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("DB", "c:\\temp\\benchmark\\", true);

            using (SnapServer engine = new SnapServer(settings))
            using (SnapClient client = SnapClient.Connect(engine))
            using (ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
            {
                Thread.Sleep(100);
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();
                for (int x = 0; x < 10000000; x++)
                {
                    key.Timestamp = (ulong)r.Next();
                    key.PointID = (ulong)x;
                    PointCount = x;
                    db.Write(key, value);
                }
            }
            Quit = true;
            th.Join();
            Console.WriteLine("Time (sec)\tPoints");
            foreach (KeyValuePair<double, int> kvp in PointSamples)
            {
                Console.WriteLine(kvp.Key.ToString() + "\t" + kvp.Value.ToString());
            }
        }

        void WriteSpeed()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            PointSamples = new SortedList<double, int>();

            while (!Quit)
            {
                double elapsed = sw.Elapsed.TotalSeconds;
                PointSamples.Add(elapsed, PointCount);
                int sleepTime = (int)(elapsed * 1000) % 100;
                sleepTime = 100 - sleepTime;
                if (sleepTime < 50)
                    sleepTime += 100;
                Thread.Sleep(sleepTime);
            }
        }


        [Test]
        public void TestRollover()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            Globals.MemoryPool.SetMaximumBufferSize(4000 * 1024 * 1024L);

            foreach (string file in Directory.GetFiles("c:\\temp\\Test\\", "*.*", SearchOption.AllDirectories))
                File.Delete(file);

            PointCount = 0;

            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("DB", "c:\\temp\\Test\\Main\\", true);
            settings.FinalWritePaths.Add("c:\\temp\\Test\\Rollover\\");

            ulong time = (ulong)DateTime.Now.Ticks;

            using (SnapServer engine = new SnapServer(settings))
            using (SnapClient client = SnapClient.Connect(engine))
            using (ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
            {
                Thread.Sleep(100);
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();
                for (int x = 0; x < 100000000; x++)
                {
                    if (x % 100 == 0)
                        Thread.Sleep(10);
                    key.Timestamp = time;
                    time += TimeSpan.TicksPerMinute;
                    db.Write(key, value);
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100);
        }





    }
}
