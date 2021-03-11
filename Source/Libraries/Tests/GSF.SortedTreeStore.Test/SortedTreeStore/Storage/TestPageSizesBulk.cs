using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using openHistorian;
using GSF.IO.FileStructure.Media;
using openHistorian.Snap;
using openHistorian.Snap.Definitions;

namespace GSF.Snap.Storage.Test
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

            System.Console.Write("Count\t");
            lst.ForEach((x) => System.Console.Write(x.Count.ToString() + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Size\t");
            lst.ForEach((x) => System.Console.Write(x.PageSize.ToString() + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Rate Write\t");
            lst.ForEach((x) => System.Console.Write(x.RateWrite.ToString("0.000") + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Rate Read\t");
            lst.ForEach((x) => System.Console.Write(x.RateRead.ToString("0.000") + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Read\t");
            lst.ForEach((x) => System.Console.Write(x.ReadCount.ToString() + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Write\t");
            lst.ForEach((x) => System.Console.Write(x.WriteCount.ToString() + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Checksum\t");
            lst.ForEach((x) => System.Console.Write(x.ChecksumCount.ToString() + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Lookups\t");
            lst.ForEach((x) => System.Console.Write(x.Lookups.ToString() + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Cached\t");
            lst.ForEach((x) => System.Console.Write(x.CachedLookups.ToString() + '\t'));
            System.Console.WriteLine();


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
            Stats.LookupKeys = 0;
            DiskIoSession.ReadCount = 0;
            DiskIoSession.WriteCount = 0;
            Stats.ChecksumCount = 0;
            DiskIoSession.Lookups = 0;
            DiskIoSession.CachedLookups = 0;
            long cnt;
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sw.Start();
            using (SortedTreeTable<HistorianKey, HistorianValue> af = SortedTreeFile.CreateInMemory(blockSize: pageSize).OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = af.BeginEdit())
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
                ChecksumCount = Stats.ChecksumCount,
                Lookups = DiskIoSession.Lookups,
                CachedLookups = DiskIoSession.CachedLookups
            };
        }

        [Test]
        public void TestBulkRolloverFile()
        {
            Stats.LookupKeys = 0;
            DiskIoSession.ReadCount = 0;
            DiskIoSession.WriteCount = 0;
            Stats.ChecksumCount = 0;
            DiskIoSession.Lookups = 0;
            DiskIoSession.CachedLookups = 0;
            long cnt;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //using (SortedTreeTable<HistorianKey, HistorianValue> af = SortedTreeFile.CreateInMemory(4096).OpenOrCreateTable<HistorianKey, HistorianValue>(SortedTree.FixedSizeNode))
            using (SortedTreeTable<HistorianKey, HistorianValue> af = SortedTreeFile.CreateInMemory(blockSize: 4096).OpenOrCreateTable<HistorianKey, HistorianValue>(HistorianFileEncodingDefinition.TypeGuid))
            {
                using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = af.BeginEdit())
                {
                    edit.AddPoints(new PointStreamSequentialPoints(1, 20000000));
                    edit.Commit();
                }
                sw.Stop();

                cnt = af.Count();
                System.Console.WriteLine(cnt);
            }

            System.Console.WriteLine((float)(20 / sw.Elapsed.TotalSeconds));
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
            using (SortedTreeTable<HistorianKey, HistorianValue> af = SortedTreeFile.CreateFile(fileName, blockSize: pageSize).OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = af.BeginEdit())
            {
                for (uint x = 0; x < 1000000; x++)
                {
                    key.PointID = x;
                    edit.AddPoint(key, value);
                }
                edit.Commit();
            }
            sw.Stop();
            System.Console.WriteLine("Size: " + pageSize + " Rate: " + (1 / sw.Elapsed.TotalSeconds).ToString());
        }
    }
}