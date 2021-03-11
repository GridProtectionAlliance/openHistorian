using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GSF.IO.Unmanaged;
using GSF.Snap.Tree;
using NUnit.Framework;
using GSF.IO.FileStructure.Media;
using openHistorian.Snap;
using openHistorian.Snap.Definitions;

namespace GSF.Snap.Storage.Test
{
    [TestFixture]
    public class TestPageSizes
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

            System.Console.Write("Size\t");
            lst.ForEach((x) => System.Console.Write(x.PageSize.ToString() + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Rate\t");
            lst.ForEach((x) => System.Console.Write(x.Rate.ToString("0.000") + '\t'));
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


        }

        [Test]
        public void TestWriteFile()
        {
            string fileName = @"c:\temp\testFile.d2";
            TestFile(4096, fileName);
        }

        [Test]
        public void Test4096RandomHuge()
        {
            const uint count = 200000;
            List<TestResults> lst = new List<TestResults>();
            //TestRandom(4096, count);
            //lst.Add(TestRandom(4096, count));
            lst.Add(TestRandom(4096, count));

            System.Console.Write("Size\t");
            lst.ForEach((x) => System.Console.Write(x.PageSize.ToString() + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Rate\t");
            lst.ForEach((x) => System.Console.Write(x.Rate.ToString("0.000") + '\t'));
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

        [Test]
        public void Test4096Random()
        {
            const uint count = 100000;
            List<TestResults> lst = new List<TestResults>();
            TestRandom(512, count);
            lst.Add(TestRandom(512, count));
            lst.Add(TestRandom(1024, count));
            lst.Add(TestRandom(2048, count));
            lst.Add(TestRandom(4096, count));
            lst.Add(TestRandom(4096 << 1, count));
            lst.Add(TestRandom(4096 << 2, count));
            lst.Add(TestRandom(4096 << 3, count));
            lst.Add(TestRandom(4096 << 4, count));

            System.Console.Write("Size\t");
            lst.ForEach((x) => System.Console.Write(x.PageSize.ToString() + '\t'));
            System.Console.WriteLine();
            System.Console.Write("Rate\t");
            lst.ForEach((x) => System.Console.Write(x.Rate.ToString("0.000") + '\t'));
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
            //StringBuilder sb = new StringBuilder();
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            DiskIoSession.ReadCount = 0;
            DiskIoSession.WriteCount = 0;
            Stats.ChecksumCount = 0;
            DiskIoSession.Lookups = 0;
            DiskIoSession.CachedLookups = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (SortedTreeFile af = SortedTreeFile.CreateInMemory(blockSize: pageSize))
            using (SortedTreeTable<HistorianKey, HistorianValue> af2 = af.OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = af2.BeginEdit())
                {
                    for (ulong x = 0; x < 1000000; x++)
                    {
                        key.Timestamp = x;
                        key.PointID = 2 * x;
                        value.Value3 = 3 * x;
                        value.Value1 = 4 * x;
                        //if ((x % 100) == 0)
                        //    sb.AppendLine(x + "," + DiskIoSession.ReadCount + "," + DiskIoSession.WriteCount);
                        //if (x == 1000)
                        //    DiskIoSession.BreakOnIO = true;
                        edit.AddPoint(key, value);
                        //edit.AddPoint(uint.MaxValue - x, 2 * x, 3 * x, 4 * x);
                    }
                    edit.Commit();
                }
                //long cnt = af.Count();
            }

            sw.Stop();

            //File.WriteAllText(@"C:\temp\" + pageSize + ".csv",sb.ToString());


            return new TestResults()
            {
                PageSize = pageSize,
                Rate = (float)(1 / sw.Elapsed.TotalSeconds),
                ReadCount = DiskIoSession.ReadCount,
                WriteCount = DiskIoSession.WriteCount,
                ChecksumCount = Stats.ChecksumCount,
                Lookups = DiskIoSession.Lookups,
                CachedLookups = DiskIoSession.CachedLookups
            };
        }

        private TestResults TestRandom(int pageSize, uint count)
        {
            //StringBuilder sb = new StringBuilder();
            Random R = new Random(1);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            DiskIoSession.ReadCount = 0;
            DiskIoSession.WriteCount = 0;
            Stats.ChecksumCount = 0;
            DiskIoSession.Lookups = 0;
            DiskIoSession.CachedLookups = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (SortedTreeFile af = SortedTreeFile.CreateInMemory(blockSize: pageSize))
            using (SortedTreeTable<HistorianKey, HistorianValue> af2 = af.OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                uint pointPairs = count / 5000;
                for (uint i = 0; i < pointPairs; i++)
                {
                    uint max = i * 5000 + 5000;
                    using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = af2.BeginEdit())
                    {
                        for (ulong x = i * 5000; x < max; x++)
                        {
                            key.Timestamp = (uint)R.Next();
                            key.PointID = 2 * x;
                            value.Value3 = 3 * x;
                            value.Value1 = 4 * x;
                            //if ((x % 100) == 0)
                            //    sb.AppendLine(x + "," + DiskIoSession.ReadCount + "," + DiskIoSession.WriteCount);
                            //if (x == 1000)
                            //    DiskIoSession.BreakOnIO = true;
                            edit.AddPoint(key, value);
                            //edit.AddPoint(uint.MaxValue - x, 2 * x, 3 * x, 4 * x);
                        }
                        edit.Commit();
                    }
                }
                //long cnt = af.Count();
            }

            sw.Stop();

            //File.WriteAllText(@"C:\temp\" + pageSize + ".csv",sb.ToString());


            return new TestResults()
            {
                PageSize = pageSize,
                Rate = (float)(count / sw.Elapsed.TotalSeconds / 1000000),
                ReadCount = DiskIoSession.ReadCount,
                WriteCount = DiskIoSession.WriteCount,
                ChecksumCount = Stats.ChecksumCount,
                Lookups = DiskIoSession.Lookups,
                CachedLookups = DiskIoSession.CachedLookups
            };
        }

        private TestResults TestRandomBinaryStream(int pageSize, uint count)
        {
            //StringBuilder sb = new StringBuilder();
            Random R = new Random(1);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            DiskIoSession.ReadCount = 0;
            DiskIoSession.WriteCount = 0;
            Stats.ChecksumCount = 0;
            DiskIoSession.Lookups = 0;
            DiskIoSession.CachedLookups = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            using (BinaryStream bs = new BinaryStream(allocatesOwnMemory: true))
            {
                SortedTree<HistorianKey, HistorianValue> table = SortedTree<HistorianKey, HistorianValue>.Create(bs, 4096);
                for (ulong x = 0; x < count; x++)
                {
                    key.Timestamp = (uint)R.Next();
                    key.PointID = 2 * x;
                    value.Value3 = 3 * x;
                    value.Value1 = 4 * x;
                    //if ((x % 100) == 0)
                    //    sb.AppendLine(x + "," + DiskIoSession.ReadCount + "," + DiskIoSession.WriteCount);
                    //if (x == 1000)
                    //    DiskIoSession.BreakOnIO = true;
                    table.TryAdd(key, value);
                    //edit.AddPoint(uint.MaxValue - x, 2 * x, 3 * x, 4 * x);
                }

            }

            sw.Stop();

            //File.WriteAllText(@"C:\temp\" + pageSize + ".csv",sb.ToString());


            return new TestResults()
            {
                PageSize = pageSize,
                Rate = (float)(count / sw.Elapsed.TotalSeconds / 1000000),
                ReadCount = DiskIoSession.ReadCount,
                WriteCount = DiskIoSession.WriteCount,
                ChecksumCount = Stats.ChecksumCount,
                Lookups = DiskIoSession.Lookups,
                CachedLookups = DiskIoSession.CachedLookups
            };
        }

        public class TestResults
        {
            public int PageSize;
            public float Rate;
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

            value.Value3 = 0;
            value.AsSingle = 65.20f;

            if (File.Exists(fileName))
                File.Delete(fileName);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (SortedTreeFile af = SortedTreeFile.CreateFile(fileName, blockSize: pageSize))
            using (SortedTreeTable<HistorianKey, HistorianValue> af2 = af.OpenOrCreateTable<HistorianKey, HistorianValue>(HistorianFileEncodingDefinition.TypeGuid))
            using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = af2.BeginEdit())
            {
                for (uint x = 0; x < 10000000; x++)
                {
                    key.Timestamp = 1;
                    key.PointID = x;
                    edit.AddPoint(key, value);
                }
                edit.Commit();
            }
            sw.Stop();
            System.Console.WriteLine("Size: " + pageSize + " Rate: " + (10 / sw.Elapsed.TotalSeconds).ToString());
        }
    }
}