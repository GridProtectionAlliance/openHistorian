using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
                    writer.Write(count,1*count,2*count,3*count);
                    count++;
                    Thread.Sleep(1);
                }
            }
            Thread.Sleep(1000);

        }
        [Test]
        public void TestLongTermFast()
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
                    //Thread.Sleep(1);
                }
            }
            Thread.Sleep(1000);

        }
    }
}
