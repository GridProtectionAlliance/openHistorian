using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.Collections.KeyValue
{
    [TestClass]
    public class SortedTree256CompareTest
    {
        public static unsafe void RunBenchmark()
        {
            var bs0 = new BinaryStream();
            var bs1 = new BinaryStream();
            var bs2 = new BinaryStream();
            var bs3 = new BinaryStream();
            var tree0 = new SortedTree256(bs0, 4096);
            var tree1 = new SortedTree256(bs1, 4096);
            var tree2 = new SortedTree256Compressed(bs2, 4096);
            var tree3 = new SortedTree256Coded(bs3, 4096);

            //var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d");
            var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
            long ptCnt = 0;
            Action<OldHistorianReader.Points> del = (x) =>
            {
                tree0.Add((ulong)x.Time.Ticks, (ulong)x.PointID, x.flags, *(uint*)&x.Value);
                ptCnt++;
            };
            hist.Read(del);

            tree0 = SortPoints(tree0);

            var scan0 = tree0.GetDataRange();
            scan0.SeekToKey(0, 0);
            ulong key1, key2, value1, value2;
            while (scan0.GetNextKey(out key1, out key2, out value1, out value2))
            {
                tree1.Add(key1, key2, value1, value2);
                tree2.Add(key1, key2, value1, value2);
                tree3.Add(key1, key2, value1, value2);
            }

            long size0 = ((MemoryStream)bs0.BaseStream).FileSize;
            long size1 = ((MemoryStream)bs1.BaseStream).FileSize;
            long size2 = ((MemoryStream)bs2.BaseStream).FileSize;
            long size3 = ((MemoryStream)bs3.BaseStream).FileSize;

            float size0Mb = size0 / (float)ptCnt;
            float size1Mb = size1 / (float)ptCnt;
            float size2Mb = size2 / (float)ptCnt;
            float size3Mb = size3 / (float)ptCnt;

            //these will never raise an exception, but are there so the compilier will not simplify out these variables.

            StringBuilder SB = new StringBuilder();
            SB.AppendLine(size0Mb.ToString());
            SB.AppendLine(size1Mb.ToString());
            SB.AppendLine(size2Mb.ToString());
            SB.AppendLine(size3Mb.ToString());

            Clipboard.SetText(SB.ToString());
        }


        [TestMethod]
        public unsafe void CompareSize()
        {
            var bs0 = new BinaryStream();
            var bs1 = new BinaryStream();
            var bs2 = new BinaryStream();
            var tree0 = new SortedTree256(bs0, 4096);
            var tree1 = new SortedTree256(bs1, 4096);
            var tree2 = new SortedTree256Compressed(bs2, 4096);

            var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");

            Action<OldHistorianReader.Points> del = (x) => tree0.Add((ulong)x.Time.Ticks, (ulong)x.PointID, x.flags, *(uint*)&x.Value);
            hist.Read(del);

            long pointCount = 0;
            var scan0 = tree0.GetDataRange();
            scan0.SeekToKey(0, 0);
            ulong key1, key2, value1, value2;
            while (scan0.GetNextKey(out key1, out key2, out value1, out value2))
            {
                pointCount++;
                tree1.Add(key1, key2, value1, value2);
                tree2.Add(key1, key2, value1, value2);
            }

            long size0 = ((MemoryStream)bs0.BaseStream).FileSize;
            long size1 = ((MemoryStream)bs1.BaseStream).FileSize;
            long size2 = ((MemoryStream)bs2.BaseStream).FileSize;

            float size0Mb = size0 / 1024.0f / 1024.0f;
            float size1Mb = size1 / 1024.0f / 1024.0f;
            float size2Mb = size2 / 1024.0f / 1024.0f;

            //these will never raise an exception, but are there so the compilier will not simplify out these variables.
            Assert.AreNotEqual(0f, size0Mb);
            Assert.AreNotEqual(0f, size1Mb);
            Assert.AreNotEqual(0f, size2Mb);

        }

        [TestMethod]
        public unsafe void CompareSize2()
        {
            var bs0 = new BinaryStream();
            var bs1 = new BinaryStream();
            var bs2 = new BinaryStream();
            var tree0 = new SortedTree256(bs0, 4096);
            var tree1 = new SortedTree256(bs1, 4096);
            var tree2 = new SortedTree256Compressed(bs2, 4096);

            var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");

            Action<OldHistorianReader.Points> del = (x) => tree0.Add((ulong)x.Time.Ticks, (ulong)x.PointID, x.flags, *(uint*)&x.Value);
            hist.Read(del);

            tree0 = SortPoints(tree0);

            long pointCount = 0;
            var scan0 = tree0.GetDataRange();
            scan0.SeekToKey(0, 0);
            ulong key1, key2, value1, value2;
            while (scan0.GetNextKey(out key1, out key2, out value1, out value2))
            {
                pointCount++;
                tree1.Add(key1, key2, value1, value2);
                tree2.Add(key1, key2, value1, value2);
            }

            long size0 = ((MemoryStream)bs0.BaseStream).FileSize;
            long size1 = ((MemoryStream)bs1.BaseStream).FileSize;
            long size2 = ((MemoryStream)bs2.BaseStream).FileSize;

            float size0Mb = size0 / 1024.0f / 1024.0f;
            float size1Mb = size1 / 1024.0f / 1024.0f;
            float size2Mb = size2 / 1024.0f / 1024.0f;

            //these will never raise an exception, but are there so the compilier will not simplify out these variables.
            Assert.AreNotEqual(0f, size0Mb);
            Assert.AreNotEqual(0f, size1Mb);
            Assert.AreNotEqual(0f, size2Mb);

        }

        [TestMethod]
        public unsafe void CompareSize3()
        {
            var bs0 = new BinaryStream();
            var bs1 = new BinaryStream();
            var bs2 = new BinaryStream();
            var tree0 = new SortedTree256(bs0, 4096);
            var tree1 = new SortedTree256(bs1, 4096);
            var tree2 = new SortedTree256Coded(bs2, 4096);
            //var tree2 = new SortedTree256Compressed(bs2, 4096);

            var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
            long ptCnt = 0;
            Action<OldHistorianReader.Points> del = (x) =>
                {
                    if (ptCnt < 100000)
                        tree0.Add((ulong)x.Time.Ticks, (ulong)x.PointID, x.flags, *(uint*)&x.Value);
                    ptCnt++;
                };
            hist.Read(del);

            tree0 = SortPoints(tree0);

            long pointCount = 0;
            var scan0 = tree0.GetDataRange();
            scan0.SeekToKey(0, 0);
            ulong key1, key2, value1, value2;
            while (scan0.GetNextKey(out key1, out key2, out value1, out value2))
            {
                pointCount++;
                tree1.Add(key1, key2, value1, value2);
                tree2.Add(key1, key2, value1, value2);
            }

            long size0 = ((MemoryStream)bs0.BaseStream).FileSize;
            long size1 = ((MemoryStream)bs1.BaseStream).FileSize;
            long size2 = ((MemoryStream)bs2.BaseStream).FileSize;

            float size0Mb = size0 / 1024.0f / 1024.0f;
            float size1Mb = size1 / 1024.0f / 1024.0f;
            float size2Mb = size2 / 1024.0f / 1024.0f;

            //these will never raise an exception, but are there so the compilier will not simplify out these variables.
            Assert.AreNotEqual(0f, size0Mb);
            Assert.AreNotEqual(0f, size1Mb);
            SortedTree256Coded.SizeByNoComp++;
            Assert.AreNotEqual(0f, size2Mb);
        }

        public static SortedTree256 SortPoints(SortedTree256 tree)
        {
            ulong maxPointId = 0;
            var scan = tree.GetDataRange();
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
