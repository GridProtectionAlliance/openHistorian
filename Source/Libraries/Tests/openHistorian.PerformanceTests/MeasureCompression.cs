using System;
using System.Text;
using GSF.Snap;
using GSF.Snap.Storage;
using GSF.Snap.Tree;
using NUnit.Framework;
using openHistorian.Snap;

namespace openHistorian.PerformanceTests
{
    [TestFixture]
    public class MeasureCompression
    {
        [Test]
        public void Test()
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            using (SortedTreeFile file = SortedTreeFile.OpenFile(@"C:\Unison\GPA\Codeplex\openHistorian\Main\Build\Output\Release\Applications\openHistorian\Archive\635293583194231435-Stage2-0ef36dcc-4264-498f-b194-01b2043a9231.d2", true))
            using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>())
            using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> reader = table.BeginRead())
            using (SortedTreeScannerBase<HistorianKey, HistorianValue> scan = reader.GetTreeScanner())
            {
                scan.SeekToStart();
                while (scan.Read(key,value))
                    ;
            }
        }

        [Test]
        public void GetBits()
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Higher Bits, Bucket Number, Count, FloatValue");
            using (SortedTreeFile file = SortedTreeFile.OpenFile(@"C:\Archive\635184227258021940-Stage2-8b835d6a-8299-45bb-9624-d4a470e4abe1.d2", true))
            using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>())
            using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> reader = table.BeginRead())
            using (SortedTreeScannerBase<HistorianKey, HistorianValue> scan = reader.GetTreeScanner())
            {
                int count = 0;
                scan.SeekToStart();
                while (scan.Read(key,value))
                    count++;

                for (int x = 1; x < 24; x++)
                {
                    scan.SeekToStart();
                    int[] bucket = MeasureBits(scan, x);
                    Write(sb, bucket, x, count);
                }
            }

            Console.WriteLine(sb.ToString());
        }

        public int[] MeasureBits(TreeStream<HistorianKey, HistorianValue> stream, int higherBits)
        {
            HistorianKey hkey = new HistorianKey();
            HistorianValue hvalue = new HistorianValue();
            int[] bucket = new int[1 << higherBits];
            int shiftBits = 32 - higherBits;
            while (stream.Read(hkey,hvalue))
            {
                uint value = (uint)hvalue.Value1 >> shiftBits;
                bucket[value]++;
            }
            return bucket;
        }

        public unsafe void Write(StringBuilder sb, int[] buckets, int higherBits, int count)
        {
            int shift = 32 - higherBits;
            for (uint x = 0; x < buckets.Length; x++)
            {
                uint value = x << shift;
                float valuef = *(float*)&value;
                double percent = buckets[x] / (double)count * 100.0;
                if (percent > 0.01)
                    sb.AppendLine(higherBits.ToString() + "," + x.ToString() + "," + (buckets[x] / (double)count * 100.0).ToString("0.00") + "," + valuef.ToString());
            }
        }


        [Test]
        public void GetDifference()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Bucket Number, Count");
            using (SortedTreeFile file = SortedTreeFile.OpenFile(@"C:\Unison\GPA\Codeplex\openHistorian\Main\Build\Output\Release\Applications\openHistorian\Archive\635293583194231435-Stage2-0ef36dcc-4264-498f-b194-01b2043a9231.d2", true))
            using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>())
            using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> reader = table.BeginRead())
            using (SortedTreeScannerBase<HistorianKey, HistorianValue> scan = reader.GetTreeScanner())
            {

                HistorianKey key1 = new HistorianKey();
                HistorianKey key2 = new HistorianKey();
                HistorianValue value = new HistorianValue();

                int count = 0;
                scan.SeekToStart();
                while (scan.Read(key1,value))
                    count++;

              

                int[] bucket = new int[130];
                scan.SeekToStart();

                while (true)
                {
                    if (!scan.Read(key1, value))
                        break;

                    if (key1.Timestamp == key2.Timestamp)
                    {
                        int diff = Math.Abs((int)(key1.PointID - key2.PointID));
                        diff = Math.Min(129, diff);
                        bucket[diff]++;
                    }

                    if (!scan.Read(key2, value))
                        break;

                    if (key1.Timestamp == key2.Timestamp)
                    {
                        int diff = Math.Abs((int)(key1.PointID - key2.PointID));
                        diff = Math.Min(129, diff);
                        bucket[diff]++;
                    }
                }

                for (uint x = 0; x < bucket.Length; x++)
                {
                    sb.AppendLine(x.ToString() + "," + (bucket[x] / (double)count * 100.0).ToString("0.00"));
                }
            }
            Console.WriteLine(sb.ToString());
        }



    }
}
