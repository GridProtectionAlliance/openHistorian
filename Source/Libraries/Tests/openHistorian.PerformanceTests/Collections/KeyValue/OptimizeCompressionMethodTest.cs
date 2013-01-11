using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using openHistorian.IO.Unmanaged;

namespace openHistorian.Collections.KeyValue
{
    [TestFixture]
    public class OptimizeCompressionMethodTest
    {

        public unsafe static void Run()
        {
            var lst = new Dictionary<uint, uint>();
     
            var bs0 = new BinaryStream();
            var tree0 = new SortedTree256(bs0, 4096);

            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
            var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d");

            Action<OldHistorianReader.Points> del = (x) => tree0.Add((ulong)x.Time.Ticks, (ulong)x.PointID, x.flags, *(uint*)&x.Value);
            hist.Read(del);

            //tree0 = SortPoints(tree0);

            long pointCount = 0;
            var scan0 = tree0.GetTreeScanner();
            scan0.SeekToKey(0, 0);
            ulong key1, key2, value1, value2;

            int[] bitsUsed = new int[65];

            ulong prevKey1 = 0;
            ulong prevKey2 = 0;
            ulong prevValue1 = 0;
            ulong prevValue2 = 0;
            ulong oldPrevValue = 0;

            while (scan0.GetNextKey(out key1, out key2, out value1, out value2))
            {
                //int bits = 64 - BitMath.CountLeadingZeros(key1 - prevKey1);
                int bits = 64 - BitMath.CountLeadingZeros(key2 - prevKey2);
                //int bits = 64 - BitMath.CountLeadingZeros(value1 ^ prevValue1);
                //int bits = 64 - BitMath.CountLeadingZeros(value2 ^ prevValue2);
                //int bits = 64 - LowestCompression(value2, prevValue2, oldPrevValue, lst);
                bitsUsed[bits]++;

                prevKey1 = key1;
                prevKey2 = key2;
                prevValue1 = value1;
                oldPrevValue = prevValue2;
                prevValue2 = value2;
            }

            StringBuilder sb = new StringBuilder();
            for (int x = 0; x < 65; x++)
                sb.AppendLine(x + "\t" + bitsUsed[x]);

            Clipboard.SetText(sb.ToString());
        }

        static int LowestCompression(ulong current, ulong previous, ulong evenOlder, Dictionary<uint, uint> integerValues)
        {
            int bits = BitMath.CountLeadingZeros(current - previous);
            bits = Math.Max(bits, BitMath.CountLeadingZeros(previous - current));
            //bits = Math.Max(bits, BitMath.CountLeadingZeros(current ^ previous));

            //bits = Math.Max(bits, BitMath.CountLeadingZeros(current));

            //bits = Math.Max(bits, BitMath.CountTrailingZeros(current - previous));
            //bits = Math.Max(bits, BitMath.CountTrailingZeros(previous - current));
            //bits = Math.Max(bits, BitMath.CountTrailingZeros(current ^ previous));
            //bits = Math.Max(bits, BitMath.CountTrailingZeros(current));

            //bits = Math.Max(bits, BitMath.CountLeadingOnes(current - previous));
            //bits = Math.Max(bits, BitMath.CountLeadingOnes(previous - current));
            //bits = Math.Max(bits, BitMath.CountLeadingOnes(current ^ previous));
            //bits = Math.Max(bits, BitMath.CountLeadingOnes(current));

            //bits = Math.Max(bits, BitMath.CountTrailingOnes(current - previous));
            //bits = Math.Max(bits, BitMath.CountTrailingOnes(previous - current));
            //bits = Math.Max(bits, BitMath.CountTrailingOnes(current ^ previous));
            //bits = Math.Max(bits, BitMath.CountTrailingOnes(current));

            //if (BitMath.CountBitsSet(current) == 1)
            //{
            //    bits = Math.Max(bits, 64 - 4);
            //}
            //if (BitMath.CountBitsSet(current) == 2)
            //{
            //    bits = Math.Max(bits, 64 - 8);
            //}

            //bits = Math.Max(bits, BitMath.CountLeadingZeros(current - evenOlder));
            //bits = Math.Max(bits, BitMath.CountLeadingZeros(evenOlder - current));
            //bits = Math.Max(bits, BitMath.CountLeadingZeros(current ^ evenOlder));

            //bits = Math.Max(bits, BitMath.CountLeadingZeros(current - (previous + (previous - evenOlder))));
            //bits = Math.Max(bits, BitMath.CountLeadingZeros(current - (previous + (previous - evenOlder) + (previous - evenOlder))));
            //bits = Math.Max(bits, BitMath.CountLeadingZeros(current - (previous + (previous - evenOlder) + (previous - evenOlder) + (previous - evenOlder))));


            ////Determine if they are integer stored as float.
            //if (current <= 0xFFFFFFFF && current>0)
            //{
            //    uint value1;
            //    uint value2;
            //    if (integerValues.TryGetValue((uint)previous, out value2))
            //    {

            //    }
            //    if (integerValues.TryGetValue((uint)current, out value1))
            //    {
            //        bits = Math.Max(bits, BitMath.CountLeadingZeros(value1));
            //        bits = Math.Max(bits, BitMath.CountLeadingZeros(value1 - value2));
            //        bits = Math.Max(bits, BitMath.CountLeadingZeros(value2 - value1));
            //        bits = Math.Max(bits, BitMath.CountLeadingZeros(value2 ^ value1));
            //    }


            //}

            return bits;
        }


        static SortedTree256 SortPoints(SortedTree256 tree)
        {
            ulong maxPointId = 0;
            var scan = tree.GetTreeScanner();
            ulong key1, key2, value1, value2;
            scan.SeekToKey(0, 0);
            while (scan.GetNextKey(out key1, out key2, out value1, out value2))
            {
                maxPointId = Math.Max(key2, maxPointId);
            }

            var map = new PointValue[(int)maxPointId + 1];

            scan.SeekToKey(0, 0);
            while (scan.GetNextKey(out key1, out key2, out value1, out value2))
            {
                if (map[(int)key2] == null)
                    map[(int)key2] = new PointValue();
                map[(int)key2].Value = value2;
            }

            var list = new List<PointValue>();
            foreach (var pv in map)
            {
                if (pv != null)
                    list.Add(pv);
            }
            list.Sort();

            for (uint x = 0; x < list.Count; x++)
            {
                list[(int)x].NewPointId = x;
            }

            var tree2 = new SortedTree256(new BinaryStream(), 4096);
            scan.SeekToKey(0, 0);
            while (scan.GetNextKey(out key1, out key2, out value1, out value2))
            {
                tree2.Add(key1, map[(int)key2].NewPointId, value1, value2);
            }

            return tree2;
        }

        class PointValue : IComparable<PointValue>
        {
            public ulong NewPointId;
            public ulong Value;

            /// <summary>
            /// Compares the current object with another object of the same type.
            /// </summary>
            /// <returns>
            /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public int CompareTo(PointValue other)
            {
                return Value.CompareTo(other.Value);
            }
        }

    }
}
