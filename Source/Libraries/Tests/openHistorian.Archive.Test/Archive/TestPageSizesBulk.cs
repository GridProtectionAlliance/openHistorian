using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;
using openHistorian.FileStructure.IO;

namespace openHistorian.UnitTests.Archive
{
    [TestFixture]
    public class TestPageSizesBulk
    {
        [Test]
        public void Test4096()
        {
            List<TestResults> lst = new List<TestResults>();
            Test(512);
            lst.Add(Test(512));
            lst.Add(Test(1024));
            lst.Add(Test(2048));
            lst.Add(Test(4096));
            lst.Add(Test(4096 << 1));
            lst.Add(Test(4096 << 2));
            lst.Add(Test(4096 << 3));
            lst.Add(Test(4096 << 4));

            Console.Write("Count\t");
            lst.ForEach((x) => Console.Write(x.Count.ToString() + '\t'));
            Console.WriteLine();
            Console.Write("Size\t");
            lst.ForEach((x) => Console.Write(x.PageSize.ToString() + '\t'));
            Console.WriteLine();
            Console.Write("Rate Write\t");
            lst.ForEach((x) => Console.Write(x.RateWrite.ToString("0.000") + '\t'));
            Console.WriteLine();
            Console.Write("Rate Read\t");
            lst.ForEach((x) => Console.Write(x.RateRead.ToString("0.000") + '\t'));
            Console.WriteLine();
            Console.Write("Read\t");
            lst.ForEach((x) => Console.Write(x.ReadCount.ToString() + '\t'));
            Console.WriteLine();
            Console.Write("Write\t");
            lst.ForEach((x) => Console.Write(x.WriteCount.ToString() + '\t'));
            Console.WriteLine();
            Console.Write("Checksum\t");
            lst.ForEach((x) => Console.Write(x.ChecksumCount.ToString() + '\t'));
            Console.WriteLine();
            Console.Write("Lookups\t");
            lst.ForEach((x) => Console.Write(x.Lookups.ToString() + '\t'));
            Console.WriteLine();
            Console.Write("Cached\t");
            lst.ForEach((x) => Console.Write(x.CachedLookups.ToString() + '\t'));
            Console.WriteLine();


            //string fileName = @"c:\temp\testFile.d2";
            //TestFile(1024, fileName);
            //TestFile(2048, fileName);
            //TestFile(4096, fileName);
            //TestFile(4096 << 1, fileName);
            //TestFile(4096 << 2, fileName);
            //TestFile(4096 << 3, fileName);
            //TestFile(4096 << 4, fileName);
        }

        private TestResults Test(int pageSize)
        {
            openHistorian.Stats.LookupKeys = 0;
            DiskIoSession.ReadCount = 0;
            DiskIoSession.WriteCount = 0;
            Statistics.ChecksumCount = 0;
            DiskIoSession.Lookups = 0;
            DiskIoSession.CachedLookups = 0;
            long cnt;
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sw.Start();
            using (ArchiveTable<HistorianKey, HistorianValue> af = ArchiveFile.CreateInMemory(pageSize).OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                using (ArchiveTable<HistorianKey, HistorianValue>.Editor edit = af.BeginEdit())
                {
                    for (int x = 0; x < 100; x++)
                    {
                        edit.AddPoints(new PointStreamSequential(x * 10000, 10000));
                    }
                    edit.Commit();
                }
                sw.Stop();
                sw2.Start();
                cnt = af.Count();
                sw2.Stop();
            }
            return new TestResults()
            {
                Count = cnt,
                PageSize = pageSize,
                RateWrite = (float)(1 / sw.Elapsed.TotalSeconds),
                RateRead = (float)(1 / sw2.Elapsed.TotalSeconds),
                ReadCount = DiskIoSession.ReadCount,
                WriteCount = DiskIoSession.WriteCount,
                ChecksumCount = Statistics.ChecksumCount,
                Lookups = DiskIoSession.Lookups,
                CachedLookups = DiskIoSession.CachedLookups
            };
        }

        //TestResults Test(int pageSize)
        //{
        //    GSF.Stats.LookupKeys = 0;
        //    DiskIoSession.ReadCount = 0;
        //    DiskIoSession.WriteCount = 0;
        //    Statistics.ChecksumCount = 0;
        //    DiskMediumIoSession.Lookups = 0;
        //    DiskMediumIoSession.CachedLookups = 0;
        //    long cnt;
        //    var sw = new Stopwatch();
        //    var sw2 = new Stopwatch();
        //    sw.Start();
        //    using (var af = ArchiveFile.CreateInMemory(CompressionMethod.TimeSeriesEncoded2, pageSize))
        //    {
        //        using (var edit = af.BeginEdit())
        //        {
        //            for (int x = 0; x < 10; x++)
        //            {
        //                edit.AddPoints(new PointStreamSequential(x * 10000, 10000));
        //            }
        //            edit.Commit();
        //        }
        //        sw.Stop();
        //        sw2.Start();
        //        cnt = af.Count();
        //        sw2.Stop();
        //    }
        //    return new TestResults()
        //        {
        //            Count = cnt,
        //            PageSize = pageSize,
        //            RateWrite = (float)(1 / sw.Elapsed.TotalSeconds),
        //            RateRead = (float)(1 / sw2.Elapsed.TotalSeconds),
        //            ReadCount = DiskIoSession.ReadCount,
        //            WriteCount = DiskIoSession.WriteCount,
        //            ChecksumCount = Statistics.ChecksumCount,
        //            Lookups = DiskMediumIoSession.Lookups,
        //            CachedLookups = DiskMediumIoSession.CachedLookups
        //        };
        //}

        public class TestResults
        {
            public long Count;
            public int PageSize;
            public float RateWrite;
            public float RateRead;
            public long ReadCount;
            public long WriteCount;
            public long ChecksumCount;
            public long Lookups;
            public long CachedLookups;
        }

        private void TestFile(int pageSize, string fileName)
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            key.Timestamp = 1;

            if (File.Exists(fileName))
                File.Delete(fileName);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (ArchiveTable<HistorianKey, HistorianValue> af = ArchiveFile.CreateFile(fileName, pageSize).OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            using (ArchiveTable<HistorianKey, HistorianValue>.Editor edit = af.BeginEdit())
            {
                for (uint x = 0; x < 1000000; x++)
                {
                    key.PointID = x;
                    edit.AddPoint(key,value);
                }
                edit.Commit();
            }
            sw.Stop();
            Console.WriteLine("Size: " + pageSize + " Rate: " + (1 / sw.Elapsed.TotalSeconds).ToString());
        }
    }
}