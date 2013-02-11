using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using GSF;
using NUnit.Framework;
using openHistorian.Engine;

namespace openHistorian.Engine.ArchiveWriters
{
    [TestFixture]
    public class WriteProcessorTest
    {
        [Test]
        public void TestLongTerm()
        {
            List<string> paths = new List<string>();
            paths.Add(@"C:\Temp\Historian");

            using (var list = new ArchiveList())
            using (var files = list.CreateNewClientResources())
            using (var writer = new WriteProcessor(WriteProcessorSettings.CreateOnDisk(list, paths), list))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                ulong count = 0;
                while (sw.Elapsed.TotalMinutes < 10)
                {
                    writer.Write(count, 1 * count, 2 * count, 3 * count);
                    count++;
                    Thread.Sleep(1);
                }
            }
            Thread.Sleep(1000);

        }
        [Test]
        public void TestLongTermFast()
        {
            //Globals.BufferPool.SetMaximumBufferSize(1024 * 1024 * 1500); //Lower to 100MB so more GC occurs.
            List<string> paths = new List<string>();
            paths.Add(@"C:\Temp\Historian");

            using (var list = new ArchiveList())
            using (var files = list.CreateNewClientResources())
            using (var writer = new WriteProcessor(WriteProcessorSettings.CreateOnDisk(list, paths), list))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                ulong count = 0;
                while (sw.Elapsed.TotalMinutes < 0.5)
                {
                    writer.Write(count, 1 * count, 2 * count, 3 * count);
                    count++;
                    //Thread.Sleep(1);
                }
                Console.WriteLine(count);
            }
            Thread.Sleep(1000);
        }

        [Test]
        public void CountPointsWritten()
        {
            long count;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var serverOptions = new HistorianServerOptions();
            serverOptions.IsNetworkHosted = false;
            serverOptions.IsReadOnly = true;
            serverOptions.Paths.Add(@"C:\Temp\Historian\temp5");

            using (var server = new HistorianServer(serverOptions))
            {
                var database = server.GetDatabase();
                using (var reader = database.OpenDataReader())
                {
                    var stream = reader.Read(0, ulong.MaxValue);
                    count = stream.Count();
                }
            }
            sw.Stop();
            Console.WriteLine(count.ToString());
            Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());

        }


    }
}
