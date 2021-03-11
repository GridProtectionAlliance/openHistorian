//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Windows.Forms;
//using GSF;
//using NUnit.Framework;
//using GSF.IO.Unmanaged;
//using GSF.SortedTreeStore.Tree;

//namespace openHistorian.Collections
//{
//    public class OptimizeCompressionMethodTest
//    {
//        public unsafe static void Run()
//        {
//            const float max = 10000000;
//            var lst = new Dictionary<uint, uint>();

//            var key = new HistorianKey();
//            var value = new HistorianValue();

//            var bs0 = new BinaryStream();
//            var tree0 = SortedTree<HistorianKey, HistorianValue>.Create(bs0, 4096);


//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p1_archive_2012-04-14 09!21!08.800_to_2012-04-14 13!29!47.400.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p2_archive_2012-04-14 11!05!54.833_to_2012-04-14 14!03!45.200.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p3_archive_2012-04-14 07!22!43.900_to_2012-04-14 14!43!39.300.d");
//            var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p3_archive_2012-04-14 14!43!39.300_to_2012-04-14 22!05!01.900.d");

//            float count = 0;
//            Func<OldHistorianReader.Points, bool> del = (x) =>
//                {
//                    //if (*(uint*)&x.Value == 0)
//                    //    return true;
//                    //if (*(uint*)&x.Value > (1<<30))
//                    //    return true;

//                    count++;
//                    if (count > max)
//                        return false;
//                    key.Timestamp = (ulong)x.Time.Ticks;
//                    key.PointID = (ulong)x.PointID;
//                    value.Value3 = x.flags;
//                    value.Value1 = *(uint*)&x.Value;
//                    if (!tree0.TryAdd(key, value))
//                        count--;
//                    return true;
//                };

//            hist.Read(del);

//            //tree0 = SortPoints(tree0);

//            long pointCount = 0;
//            var scan0 = tree0.CreateTreeScanner();
//            scan0.SeekToStart();

//            int[] bitsUsed1 = new int[65];
//            int[] bitsUsed2 = new int[65];
//            int[] bitsUsed3 = new int[65];
//            int[] bitsUsed4 = new int[65];

//            ulong prevKey1 = 0;
//            ulong prevKey2 = 0;
//            ulong prevValue1 = 0;
//            ulong prevValue2 = 0;

//            while (scan0.Read())
//            {
//                int bits1 = 64 - BitMath.CountLeadingZeros(scan0.CurrentKey.Timestamp - prevKey1);
//                int bits2 = 64 - BitMath.CountLeadingZeros(scan0.CurrentKey.PointID - prevKey2);
//                int bits3 = 64 - BitMath.CountLeadingZeros(scan0.CurrentValue.Value3);
//                int bits4 = 64 - BitMath.CountLeadingZeros(scan0.CurrentValue.Value1);

//                if (scan0.CurrentValue.Value1 >= (3u << 30) && scan0.CurrentValue.Value1 < (4ul << 30))
//                {
//                    bitsUsed1[bits1]++;
//                    bitsUsed2[bits2]++;
//                    bitsUsed3[bits3]++;
//                    bitsUsed4[bits4]++;
//                }
//                else
//                {
//                    count--;
//                }


//                prevKey1 = scan0.CurrentKey.Timestamp;
//                prevKey2 = scan0.CurrentKey.PointID;
//                prevValue1 = scan0.CurrentValue.Value3;
//                prevValue2 = scan0.CurrentValue.Value1;
//            }

//            StringBuilder sb = new StringBuilder();
//            sb.AppendLine("Bits to store\tTimestamp\tPointId\tQuality\tValue");
//            for (int x = 0; x < 65; x++)
//                sb.AppendLine(x + "\t" + (bitsUsed1[x] / count).ToString("0.0%") + "\t" + (bitsUsed2[x] / count).ToString("0.0%") + "\t" + (bitsUsed3[x] / count).ToString("0.0%") + "\t" + (bitsUsed4[x] / count).ToString("0.0%"));
//            sb.AppendLine(count.ToString());
//            Clipboard.SetText(sb.ToString());
//        }

//        public unsafe static void Run2()
//        {
//            const int max = 9000000;
//            var lst = new Dictionary<uint, uint>();

//            var key = new HistorianKey();
//            var value = new HistorianValue();

//            var bs0 = new BinaryStream();
//            var tree0 = SortedTree<HistorianKey, HistorianValue>.Create(bs0, 4096);
//            int globalCount = 0;
//            float count = 0;
//            Func<OldHistorianReader.Points, bool> del = (x) =>
//            {
//                //if (*(uint*)&x.Value == 0)
//                //    return true;
//                //if (*(uint*)&x.Value > (1<<30))
//                //    return true;

//                count++;
//                if (count > max)
//                    return false;
//                key.Timestamp = (ulong)x.Time.Ticks;
//                key.PointID = (ulong)x.PointID;
//                value.Value3 = x.flags;
//                value.Value1 = *(uint*)&x.Value;
//                if (!tree0.TryAdd(key, value))
//                    count--;
//                else
//                    globalCount++;
//                return true;
//            };

//            var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p1_archive_2012-04-14 09!21!08.800_to_2012-04-14 13!29!47.400.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p2_archive_2012-04-14 11!05!54.833_to_2012-04-14 14!03!45.200.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p3_archive_2012-04-14 07!22!43.900_to_2012-04-14 14!43!39.300.d");
//            //var 
//            count = 0;
//            hist.Read(del);

//            //hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p1_archive_2012-04-14 09!21!08.800_to_2012-04-14 13!29!47.400.d");
//            //count = 0;
//            //hist.Read(del);

//            //hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p2_archive_2012-04-14 11!05!54.833_to_2012-04-14 14!03!45.200.d");
//            //count = 0;
//            //hist.Read(del);

//            //hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p3_archive_2012-04-14 07!22!43.900_to_2012-04-14 14!43!39.300.d");
//            //count = 0;
//            //hist.Read(del);

//            //hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p3_archive_2012-04-14 14!43!39.300_to_2012-04-14 22!05!01.900.d");
//            //count = 0;
//            //hist.Read(del);


//            int bitsPrefix = 8;
//            StringBuilder sb = new StringBuilder();
//            sb.AppendLine("Distinct Bits\tPrefix\tCount\tLower\tUpper\tLower\tUpper");


//            for (int bits = 8; bits < 27; bits++)
//                CompressPrefixBits(tree0, bits, sb, globalCount);


//            sb.AppendLine(globalCount.ToString());


//            Clipboard.SetText(sb.ToString());
//        }

//        static unsafe void CompressPrefixBits(SortedTree<HistorianKey, HistorianValue> tree0, int bitsPrefix, StringBuilder sb, int maxCount)
//        {
//            var scan0 = tree0.CreateTreeScanner();
//            scan0.SeekToStart();

//            int[] prefixes = new int[2 << bitsPrefix];

//            while (scan0.Read())
//            {
//                prefixes[scan0.CurrentValue.Value1 >> (32 - bitsPrefix)]++;
//            }

//            for (uint x = 0; x < prefixes.Length; x++)
//            {
//                if (prefixes[x]*100.0/maxCount > 0.25)
//                {
//                    uint lower = x << (32 - bitsPrefix);
//                    uint upper = (x + 1) << (32 - bitsPrefix);
//                    float lowerF = *(float*)&lower;
//                    float upperF = *(float*)&upper;

//                    sb.AppendLine((32 - bitsPrefix) + "\t" + x + "\t" + (prefixes[x]*100.0/maxCount).ToString() + "\t" + (lowerF).ToString() + "\t" + (upperF).ToString() +
//                                  "\t" + (lowerF * 1.732).ToString() + "\t" + (upperF * 1.732).ToString());
//                }
//            }
//        }

//        public unsafe static void Run3()
//        {
//            const int max = 9000000;
//            var lst = new Dictionary<uint, uint>();

//            var key = new HistorianKey();
//            var value = new HistorianValue();

//            var bs0 = new BinaryStream();
//            var tree0 = SortedTree<HistorianKey, HistorianValue>.Create(bs0, 4096);
//            int globalCount = 0;
//            float count = 0;
//            Func<OldHistorianReader.Points, bool> del = (x) =>
//            {
//                //if (*(uint*)&x.Value == 0)
//                //    return true;
//                //if (*(uint*)&x.Value > (1<<30))
//                //    return true;

//                count++;
//                if (count > max)
//                    return false;
//                key.Timestamp = (ulong)x.Time.Ticks;
//                key.PointID = (ulong)x.PointID;
//                value.Value3 = x.flags;
//                value.Value1 = *(uint*)&x.Value;
//                if (!tree0.TryAdd(key, value))
//                    count--;
//                else
//                    globalCount++;
//                return true;
//            };

//            var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p1_archive_2012-04-14 09!21!08.800_to_2012-04-14 13!29!47.400.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p2_archive_2012-04-14 11!05!54.833_to_2012-04-14 14!03!45.200.d");
//            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p3_archive_2012-04-14 07!22!43.900_to_2012-04-14 14!43!39.300.d");
//            //var 
//            count = 0;
//            hist.Read(del);

//            //hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p1_archive_2012-04-14 09!21!08.800_to_2012-04-14 13!29!47.400.d");
//            //count = 0;
//            //hist.Read(del);

//            //hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p2_archive_2012-04-14 11!05!54.833_to_2012-04-14 14!03!45.200.d");
//            //count = 0;
//            //hist.Read(del);

//            //hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p3_archive_2012-04-14 07!22!43.900_to_2012-04-14 14!43!39.300.d");
//            //count = 0;
//            //hist.Read(del);

//            //hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\p3_archive_2012-04-14 14!43!39.300_to_2012-04-14 22!05!01.900.d");
//            //count = 0;
//            //hist.Read(del);


//            int bitsPrefix = 8;
//            StringBuilder sb = new StringBuilder();
//            sb.AppendLine("Bits To Store\tPostfix\tCount");


//            for (int bits = 8; bits < 27; bits++)
//                CompressTrailingBits(tree0, bits, sb, globalCount);


//            sb.AppendLine(globalCount.ToString());


//            Clipboard.SetText(sb.ToString());
//        }

//        static unsafe void CompressTrailingBits(SortedTree<HistorianKey, HistorianValue> tree0, int bitsPrefix, StringBuilder sb, int maxCount)
//        {
//            var scan0 = tree0.CreateTreeScanner();
//            scan0.SeekToStart();

//            int[] prefixes = new int[2 << bitsPrefix];

//            ulong mask = ((1u << bitsPrefix) - 1);

//            while (scan0.Read())
//            {
//                prefixes[scan0.CurrentValue.Value1 & mask]++;
//            }

//            for (uint x = 0; x < prefixes.Length; x++)
//            {
//                if (prefixes[x] * 100.0 / maxCount > 0.25)
//                {
//                    sb.AppendLine((32-bitsPrefix) + "\t" + x + "\t" + (prefixes[x] * 100.0 / maxCount).ToString() + "\t");
//                }
//            }
//        }

//        //static int LowestCompression(ulong current, ulong previous, ulong evenOlder, Dictionary<uint, uint> integerValues)
//        //{
//        //    int bits = BitMath.CountLeadingZeros(current - previous);
//        //    bits = Math.Max(bits, BitMath.CountLeadingZeros(previous - current));
//        //    //bits = Math.Max(bits, BitMath.CountLeadingZeros(current ^ previous));

//        //    //bits = Math.Max(bits, BitMath.CountLeadingZeros(current));

//        //    //bits = Math.Max(bits, BitMath.CountTrailingZeros(current - previous));
//        //    //bits = Math.Max(bits, BitMath.CountTrailingZeros(previous - current));
//        //    //bits = Math.Max(bits, BitMath.CountTrailingZeros(current ^ previous));
//        //    //bits = Math.Max(bits, BitMath.CountTrailingZeros(current));

//        //    //bits = Math.Max(bits, BitMath.CountLeadingOnes(current - previous));
//        //    //bits = Math.Max(bits, BitMath.CountLeadingOnes(previous - current));
//        //    //bits = Math.Max(bits, BitMath.CountLeadingOnes(current ^ previous));
//        //    //bits = Math.Max(bits, BitMath.CountLeadingOnes(current));

//        //    //bits = Math.Max(bits, BitMath.CountTrailingOnes(current - previous));
//        //    //bits = Math.Max(bits, BitMath.CountTrailingOnes(previous - current));
//        //    //bits = Math.Max(bits, BitMath.CountTrailingOnes(current ^ previous));
//        //    //bits = Math.Max(bits, BitMath.CountTrailingOnes(current));

//        //    //if (BitMath.CountBitsSet(current) == 1)
//        //    //{
//        //    //    bits = Math.Max(bits, 64 - 4);
//        //    //}
//        //    //if (BitMath.CountBitsSet(current) == 2)
//        //    //{
//        //    //    bits = Math.Max(bits, 64 - 8);
//        //    //}

//        //    //bits = Math.Max(bits, BitMath.CountLeadingZeros(current - evenOlder));
//        //    //bits = Math.Max(bits, BitMath.CountLeadingZeros(evenOlder - current));
//        //    //bits = Math.Max(bits, BitMath.CountLeadingZeros(current ^ evenOlder));

//        //    //bits = Math.Max(bits, BitMath.CountLeadingZeros(current - (previous + (previous - evenOlder))));
//        //    //bits = Math.Max(bits, BitMath.CountLeadingZeros(current - (previous + (previous - evenOlder) + (previous - evenOlder))));
//        //    //bits = Math.Max(bits, BitMath.CountLeadingZeros(current - (previous + (previous - evenOlder) + (previous - evenOlder) + (previous - evenOlder))));


//        //    ////Determine if they are integer stored as float.
//        //    //if (current <= 0xFFFFFFFF && current>0)
//        //    //{
//        //    //    uint value1;
//        //    //    uint value2;
//        //    //    if (integerValues.TryGetValue((uint)previous, out value2))
//        //    //    {

//        //    //    }
//        //    //    if (integerValues.TryGetValue((uint)current, out value1))
//        //    //    {
//        //    //        bits = Math.Max(bits, BitMath.CountLeadingZeros(value1));
//        //    //        bits = Math.Max(bits, BitMath.CountLeadingZeros(value1 - value2));
//        //    //        bits = Math.Max(bits, BitMath.CountLeadingZeros(value2 - value1));
//        //    //        bits = Math.Max(bits, BitMath.CountLeadingZeros(value2 ^ value1));
//        //    //    }


//        //    //}

//        //    return bits;
//        //}


//        //static SortedTree256 SortPoints(SortedTree256 tree)
//        //{
//        //    ulong maxPointId = 0;
//        //    var scan = tree.GetTreeScanner();
//        //    ulong key1, key2, value1, value2;
//        //    scan.SeekToKey(0, 0);
//        //    while (scan.Read(out key1, out key2, out value1, out value2))
//        //    {
//        //        maxPointId = Math.Max(key2, maxPointId);
//        //    }

//        //    var map = new PointValue[(int)maxPointId + 1];

//        //    scan.SeekToKey(0, 0);
//        //    while (scan.Read(out key1, out key2, out value1, out value2))
//        //    {
//        //        if (map[(int)key2] is null)
//        //            map[(int)key2] = new PointValue();
//        //        map[(int)key2].Value = value2;
//        //    }

//        //    var list = new List<PointValue>();
//        //    foreach (var pv in map)
//        //    {
//        //        if (pv != null)
//        //            list.Add(pv);
//        //    }
//        //    list.Sort();

//        //    for (uint x = 0; x < list.Count; x++)
//        //    {
//        //        list[(int)x].NewPointId = x;
//        //    }

//        //    var tree2 = SortedTree256.Create(new BinaryStream(), 4096);
//        //    scan.SeekToKey(0, 0);
//        //    while (scan.Read(out key1, out key2, out value1, out value2))
//        //    {
//        //        tree2.Add(key1, map[(int)key2].NewPointId, value1, value2);
//        //    }

//        //    return tree2;
//        //}

//        //class PointValue : IComparable<PointValue>
//        //{
//        //    public ulong NewPointId;
//        //    public ulong Value;

//        //    /// <summary>
//        //    /// Compares the current object with another object of the same type.
//        //    /// </summary>
//        //    /// <returns>
//        //    /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
//        //    /// </returns>
//        //    /// <param name="other">An object to compare with this object.</param>
//        //    public int CompareTo(PointValue other)
//        //    {
//        //        return Value.CompareTo(other.Value);
//        //    }
//        //}

//    }
//}

