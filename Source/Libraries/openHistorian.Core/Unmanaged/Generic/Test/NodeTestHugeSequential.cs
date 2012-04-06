using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using openHistorian.Core.Unmanaged.Generic.TimeKeyPair;

namespace openHistorian.Core.Unmanaged.Generic
{
    class NodeTestHugeSequential
    {
        internal static void Test()
        {
            TestArchive();
            //TestMultiLevelLong(10);
            //TestMultiLevelLongRandomSpecific(10);
            //TestMultiLevelLongRandomSpecificSingleStream(10);
            //TestScan(1);
        }

        static void TestMultiLevelLongRandomSpecificSingleStream(int hundredThousand)
        {
            const int countTime = 100 * 1000;
            int count = countTime * hundredThousand;

            //var ms = new MemoryStream();
            //ms.Position = 6000 * 4096;
            //ms.Write(new byte[] { 1 }, 0, 1);
            //var bs = new BinaryStream(ms);
            //var tree = new BPlusTree<TreeTypeLong, TreeTypeLong>(bs, 4096);

            var ms1 = new MemoryStream();
            ms1.Position = 6000 * 4096;
            ms1.Write(new byte[] { 1 }, 0, 1);
            var bs1 = new BinaryStream(ms1);
            var tree = new BPlusTreeLongLong(bs1, 4096);

            long key = 0;
            long data = 0;
            long data2;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int z = 0; z < hundredThousand; z++)
            {
                for (int x = 0; x < countTime; x++)
                {
                    key = key + 1;
                    tree.AddData(key, data);
                }
            }
            sw.Stop();

            long lookup1 = ms1.LookupCount;

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            key = 0;

            for (int z = 0; z < hundredThousand; z++)
            {
                for (int x = 0; x < countTime; x++)
                {
                    key = key + 1;
                    data2 = tree.GetData(key);
                    if (data2 != data)
                        throw new Exception();
                }
            }

            sw2.Stop();

            long lookup2 = ms1.LookupCount - lookup1;

            MessageBox.Show("Sequential Single Stream LongLong" + Environment.NewLine +
                (count / sw.Elapsed.TotalSeconds / 1000000).ToString() + " " + lookup1 + Environment.NewLine +
                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + " " + lookup2 + Environment.NewLine);
        }

        static void TestMultiLevelLongRandomSpecific(int hundredThousand)
        {
            const int countTime = 100 * 1000;
            int count = countTime * hundredThousand;

            //var ms = new MemoryStream();
            //ms.Position = 6000 * 4096;
            //ms.Write(new byte[] { 1 }, 0, 1);
            //var bs = new BinaryStream(ms);
            //var tree = new BPlusTree<TreeTypeLong, TreeTypeLong>(bs, 4096);

            var ms1 = new MemoryStream();
            ms1.Position = 6000 * 4096;
            ms1.Write(new byte[] { 1 }, 0, 1);
            var bs1 = new BinaryStream(ms1);
            var tree = new BPlusTreeLongLong(bs1, 4096);

            long key = 0;
            long data = 0;
            long data2;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int z = 0; z < hundredThousand; z++)
            {
                for (int x = 0; x < countTime; x++)
                {
                    key = key + 1;
                    tree.AddData(key, data);
                }
            }
            sw.Stop();

            long lookup1 = ms1.LookupCount;

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            key = 0;

            for (int z = 0; z < hundredThousand; z++)
            {
                for (int x = 0; x < countTime; x++)
                {
                    key = key + 1;
                    data2 = tree.GetData(key);
                    if (data2 != data)
                        throw new Exception();
                }
            }

            sw2.Stop();

            long lookup2 = ms1.LookupCount - lookup1;

            MessageBox.Show("Sequential Dual Stream LongLong" + Environment.NewLine +
                (count / sw.Elapsed.TotalSeconds / 1000000).ToString() + " " + lookup1 + Environment.NewLine +
                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + " " + lookup2 + Environment.NewLine);
        }

        static void TestMultiLevelLong(int hundredThousand)
        {
            const int countTime = 100 * 1000;
            int count = countTime * hundredThousand;

            //var ms = new MemoryStream();
            //ms.Position = 6000 * 4096;
            //ms.Write(new byte[] { 1 }, 0, 1);
            //var bs = new BinaryStream(ms);
            //var tree = new BPlusTree<TreeTypeLong, TreeTypeLong>(bs, 4096);

            var ms1 = new MemoryStream();
            ms1.Position = 6000 * 4096;
            ms1.Write(new byte[] { 1 }, 0, 1);
            var bs1 = new BinaryStream(ms1);
            var tree = new BPlusTree<TreeTypeLong, TreeTypeLong>(bs1, 4096);

            TreeTypeLong key = 0;
            long data = 0;
            long data2;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int z = 0; z < hundredThousand; z++)
            {
                for (int x = 0; x < countTime; x++)
                {
                    key = key.Value + 1;
                    tree.AddData(key, data);
                }
            }
            sw.Stop();

            long lookup1 = ms1.LookupCount;

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            key = 0;

            for (int z = 0; z < hundredThousand; z++)
            {
                for (int x = 0; x < countTime; x++)
                {
                    key = key.Value + 1;
                    data2 = tree.GetData(key).Value;
                    if (data2 != data)
                        throw new Exception();
                }
            }

            sw2.Stop();

            long lookup2 = ms1.LookupCount - lookup1;

            MessageBox.Show("Sequential Dual Stream BPlusTree<Long,Long>" + Environment.NewLine +
                (count / sw.Elapsed.TotalSeconds / 1000000).ToString() + " " + lookup1 + Environment.NewLine +
                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + " " + lookup2 + Environment.NewLine);
        }

        static void TestScan(int seed)
        {
            const int countTime = 1000;
            const int countKey = 1000;
            const int count = countTime * countKey;

            Random rand;
            rand = new Random(seed);

            var ms = new MemoryStream();
            ms.Position = 6000 * 4096;
            ms.Write(new byte[] { 1 }, 0, 1);
            var bs = new BinaryStream(ms);
            var tree = new BPlusTree<TimeKeyPair.KeyType, TreeTypeLong>(bs, 4096);

            TimeKeyPair.KeyType origKey;
            origKey.Time = new DateTime(DateTime.Now.Ticks);
            origKey.Key = seed;
            TimeKeyPair.KeyType key = origKey;
            long data;
            long data2;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < countTime; x++)
            {
                for (int y = 0; y < countKey; y++)
                {
                    key.Time = key.Time.AddTicks(rand.Next() + 1);
                    key.Key += rand.Next() + 1;
                    data = rand.Next();
                    tree.AddData(key, data);
                }
            }

            sw.Stop();

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            rand = new Random(seed);
            key = origKey;

            TimeKeyPair.KeyType start = default(TimeKeyPair.KeyType);
            TimeKeyPair.KeyType stop = default(TimeKeyPair.KeyType);
            start.Time = DateTime.MinValue;
            stop.Time = DateTime.MaxValue;

            int cnt = 0;
            var scan = tree.ExecuteScan(start, stop);
            while (scan.Next())
            {
                TimeKeyPair.KeyType keyResult = scan.GetKey();
                long valueResult = scan.GetValue().Value;

                key.Time = key.Time.AddTicks(rand.Next() + 1);
                key.Key += rand.Next() + 1;
                data = rand.Next();

                if (key.Time != keyResult.Time)
                    throw new Exception();

                if (key.Key != keyResult.Key)
                    throw new Exception();

                if (valueResult != data)
                    throw new Exception();

                cnt++;
                if (cnt == 86)
                    cnt = cnt;
            }

            sw2.Stop();
            MessageBox.Show((count / sw.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine +
                (count / sw2.Elapsed.TotalSeconds / 1000000).ToString() + Environment.NewLine);
        }

        public static void TestArchive()
        {
            s_points = 0;
            //string file = "G:\\ArchiveTest.hfs";
            //if (File.Exists(file))
            //    File.Delete(file);
            //s_archive = new Archive(file);

            MemoryStream ms1 = new MemoryStream();
            BinaryStream bs1 = new BinaryStream(ms1);


            m_tree = new BPlusTreeTSD(bs1, 4096);
            HistorianReader reader = new HistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
            reader.NewPoint += ReaderNewPoint;
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sw.Start();
            reader.Read();
            sw.Stop();
            int cnt = 0;
            sw2.Start();

            long lookup1 = ms1.LookupCount;


            foreach (var pts in GetData(DateTime.MinValue, DateTime.MaxValue))
            {
                if (pts.Item2 == 102)
                {
                    cnt++;
                }
            }
            sw2.Stop();

            m_tree = null;

            MessageBox.Show(s_points + "points " + sw.Elapsed.TotalSeconds + "sec " + s_points / sw.Elapsed.TotalSeconds + " " + lookup1);
            MessageBox.Show(s_points + "points " + sw2.Elapsed.TotalSeconds + "sec " + s_points / sw2.Elapsed.TotalSeconds + " cnt:" + cnt);

        }

        static BPlusTreeTSD m_tree;
        static int s_points;

        static void ReaderNewPoint(Points pt)
        {
            s_points++;
            AddPoint(pt.Time, pt.PointID, pt.flags, pt.Value);
        }

        static void AddPoint(DateTime date, long pointId, int flags, float data)
        {
            KeyType key = default(KeyType);
            key.Time = date;
            key.Key = pointId;

            TreeTypeIntFloat value = new TreeTypeIntFloat(flags, data);

            m_tree.AddData(key, value);
        }

        static IEnumerable<Tuple<DateTime, long, int, float>> GetData(long pointId, DateTime startDate, DateTime stopDate)
        {
            KeyType start = default(KeyType);
            KeyType end = default(KeyType);
            start.Time = startDate;
            start.Key = pointId;
            end.Time = stopDate;
            end.Key = pointId;

            var reader = m_tree.ExecuteScan(start, end);
            while (reader.Next())
            {
                KeyType key = reader.GetKey();
                if (reader.GetKey().Key == pointId)
                {
                    TreeTypeIntFloat value = reader.GetValue();
                    yield return new Tuple<DateTime, long, int, float>(key.Time, key.Key, value.Value1, value.Value2);
                }
            }
        }
        static IEnumerable<Tuple<DateTime, long, int, float>> GetData(DateTime startDate, DateTime stopDate)
        {
            KeyType start = default(KeyType);
            KeyType end = default(KeyType);
            start.Time = startDate;
            start.Key = long.MinValue;
            end.Time = stopDate;
            end.Key = long.MaxValue;

            var reader = m_tree.ExecuteScan(start, end);
            while (reader.Next())
            {
                KeyType key = reader.GetKey();
                TreeTypeIntFloat value = reader.GetValue();
                yield return new Tuple<DateTime, long, int, float>(key.Time, key.Key, value.Value1, value.Value2);
            }
        }
    }
}
