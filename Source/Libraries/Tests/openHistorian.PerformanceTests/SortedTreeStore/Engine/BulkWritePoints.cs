using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GSF;
using GSF.Diagnostics;
using GSF.IO.Unmanaged;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Tree.TreeNodes;
using NUnit.Framework;
using openHistorian.Collections;

namespace openHistorian.PerformanceTests.SortedTreeStore.Engine
{
    [TestFixture]
    public class BulkWritePoints
    {
        volatile bool Quit;
        volatile int PointCount;
        SortedList<double, int> PointSamples;

        [Test]
        public void VerifyDB()
        {
            Logger.ReportToConsole(VerboseLevel.All ^ VerboseLevel.DebugLow);
            //Logger.ConsoleSubscriber.AddIgnored(Logger.LookupType("GSF.SortedTreeStore"));
            Globals.MemoryPool.SetMaximumBufferSize(1000 * 1024 * 1024);
            Globals.MemoryPool.SetTargetUtilizationLevel(TargetUtilizationLevels.Low);

            var config = new ServerDatabaseConfig()
            {
                ArchiveEncodingMethod = CreateHistorianCompressionTs.TypeGuid,
                DatabaseName = "DB",
                KeyType = new HistorianKey().GenericTypeGuid,
                ValueType = new HistorianValue().GenericTypeGuid,
                MainPath = "c:\\temp\\benchmark\\",
                WriterMode = WriterMode.OnDisk
            };
            var serverConfig = new ServerConfig();
            serverConfig.Databases.Add(config);

            using (var engine = new Server(serverConfig.ToServerSettings()))
            using (var client = Client.Connect(engine))
            using (var db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
            using (var scan = db.Read(null, null, null))
            {
                var key = new HistorianKey();
                var value = new HistorianValue();
                for (int x = 0; x < 100000000; x++)
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
            Logger.ReportToConsole(VerboseLevel.All ^ VerboseLevel.DebugLow);

            Globals.MemoryPool.SetMaximumBufferSize(4000 * 1024 * 1024L);

            Thread th = new Thread(WriteSpeed);
            th.IsBackground = true;
            th.Start();

            Quit = false;
            foreach (var file in Directory.GetFiles("c:\\temp\\benchmark\\", "*.*", SearchOption.AllDirectories))
                File.Delete(file);

            PointCount = 0;

            var config = new ServerDatabaseConfig()
            {
                ArchiveEncodingMethod = CreateHistorianCompressionTs.TypeGuid,
                DatabaseName = "DB",
                KeyType = new HistorianKey().GenericTypeGuid,
                ValueType = new HistorianValue().GenericTypeGuid,
                MainPath = "c:\\temp\\benchmark\\",
                WriterMode = WriterMode.OnDisk
            };
            var serverConfig = new ServerConfig();
            serverConfig.Databases.Add(config);

            using (var engine = new Server(serverConfig.ToServerSettings()))
            using (var client = Client.Connect(engine))
            using (var db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
            {
                Thread.Sleep(100);
                var key = new HistorianKey();
                var value = new HistorianValue();
                //for (int x = 0; x < 10000000; x++)
                for (int x = 0; x < 100000000; x++)
                {
                    key.PointID = (ulong)x;
                    PointCount = x;
                    db.Write(key, value);
                }
                Quit = true;
                th.Join();
            }

            Console.WriteLine("Time (sec)\tPoints");
            foreach (var kvp in PointSamples)
            {
                Console.WriteLine(kvp.Key.ToString() + "\t" + kvp.Value.ToString());
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100);
        }

        [Test]
        public void TestWriteSpeedRandom()
        {

            Logger.ReportToConsole(VerboseLevel.All ^ VerboseLevel.DebugLow);

            Random r = new Random(1);
            Thread th = new Thread(WriteSpeed);
            th.IsBackground = true;
            th.Start();

            Quit = false;
            foreach (var file in Directory.GetFiles("c:\\temp\\benchmark\\"))
                File.Delete(file);

            PointCount = 0;

            var config = new ServerDatabaseConfig()
            {
                ArchiveEncodingMethod = CreateHistorianCompressionTs.TypeGuid,
                DatabaseName = "DB",
                KeyType = new HistorianKey().GenericTypeGuid,
                ValueType = new HistorianValue().GenericTypeGuid,
                MainPath = "c:\\temp\\benchmark\\",
                WriterMode = WriterMode.OnDisk
            };
            var serverConfig = new ServerConfig();
            serverConfig.Databases.Add(config);

            using (var engine = new Server(serverConfig.ToServerSettings()))
            using (var client = Client.Connect(engine))
            using (var db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
            {
                Thread.Sleep(100);
                var key = new HistorianKey();
                var value = new HistorianValue();
                for (int x = 0; x < 10000000; x++)
                {
                    key.Timestamp = (ulong)r.Next();
                    key.PointID = (ulong)x;
                    PointCount = x;
                    db.Write(key, value);
                }
                Quit = true;
                th.Join();
            }

            Console.WriteLine("Time (sec)\tPoints");
            foreach (var kvp in PointSamples)
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
            Logger.ReportToConsole(VerboseLevel.All ^ VerboseLevel.DebugLow);

            Globals.MemoryPool.SetMaximumBufferSize(4000 * 1024 * 1024L);

            foreach (var file in Directory.GetFiles("c:\\temp\\Test\\", "*.*", SearchOption.AllDirectories))
                File.Delete(file);

            PointCount = 0;

            var config = new ServerDatabaseConfig()
            {
                ArchiveEncodingMethod = CreateHistorianCompressionTs.TypeGuid,
                DatabaseName = "DB",
                KeyType = new HistorianKey().GenericTypeGuid,
                ValueType = new HistorianValue().GenericTypeGuid,
                MainPath = "c:\\temp\\Test\\Main\\",
                WriterMode = WriterMode.OnDisk
            };
            config.FinalWritePaths.Add("c:\\temp\\Test\\Rollover\\");
            var serverConfig = new ServerConfig();
            serverConfig.Databases.Add(config);

            ulong time = (ulong)DateTime.Now.Ticks;

            using (var engine = new Server(serverConfig.ToServerSettings()))
            using (var client = Client.Connect(engine))
            using (var db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
            {
                Thread.Sleep(100);
                var key = new HistorianKey();
                var value = new HistorianValue();
                for (int x = 0; x < 100000000; x++)
                {
                    if (x % 100 == 0)
                        Thread.Sleep(10);
                    key.Timestamp = (ulong)time;
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
