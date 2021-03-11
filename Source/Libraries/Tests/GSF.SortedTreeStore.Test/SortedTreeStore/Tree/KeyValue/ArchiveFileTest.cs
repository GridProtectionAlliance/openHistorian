//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Windows.Forms;
//using NUnit.Framework;
//using GSF.IO.Unmanaged;
//using openHistorian.Archive;

//namespace openHistorian.Collections
//{
//    [TestFixture]
//    internal class ArchiveFileTest
//    {

//        [Test]
//        public static unsafe void ReadFiles()
//        {
//            string path1 = "c:\\temp\\ArchiveFileTest1.d2";
//            string path2 = "c:\\temp\\ArchiveFileTest2.d2";
//            string path3 = "c:\\temp\\ArchiveFileTest3.d2";

//            using (var file1 = HistorianArchiveFile.OpenFile(path1, isReadOnly:true))
//            using (var file2 = HistorianArchiveFile.OpenFile(path2, isReadOnly: true))
//            using (var file3 = HistorianArchiveFile.OpenFile(path3, isReadOnly: true))
//            {

//                var scan1 = file1.AcquireReadSnapshot().CreateReadSnapshot().GetTreeScanner();
//                var scan2 = file2.AcquireReadSnapshot().CreateReadSnapshot().GetTreeScanner();
//                var scan3 = file3.AcquireReadSnapshot().CreateReadSnapshot().GetTreeScanner();
//                scan1.SeekToStart();
//                scan2.SeekToStart();
//                scan3.SeekToStart();
//                ulong key1, key2, value1, value2;
//                ulong key11, key22, value11, value22;
//                while (scan1.Read(out key1, out key2, out value1, out value2))
//                {
//                    Assert.IsTrue(scan2.Read(out key11, out key22, out value11, out value22));
//                    Assert.AreEqual(key1, key11);
//                    Assert.AreEqual(key2, key22);
//                    Assert.AreEqual(value1, value11);
//                    Assert.AreEqual(value2, value22);
//                    Assert.IsTrue(scan3.Read(out key11, out key22, out value11, out value22));
//                    Assert.AreEqual(key1, key11);
//                    Assert.AreEqual(key2, key22);
//                    Assert.AreEqual(value1, value11);
//                    Assert.AreEqual(value2, value22);
//                }
//                Assert.IsFalse(scan2.Read(out key1, out key2, out value1, out value2));
//                Assert.IsFalse(scan3.Read(out key1, out key2, out value1, out value2));

//            }
//        }

//        [Test]
//        public static unsafe void RunBenchmark()
//        {
//            string path1 = "c:\\temp\\ArchiveFileTest1.d2";
//            string path2 = "c:\\temp\\ArchiveFileTest2.d2";
//            string path3 = "c:\\temp\\ArchiveFileTest3.d2";

//            var bs0 = new BinaryStream();
//            var tree0 = SortedTree256.Create(bs0, 4096);

//            if (File.Exists(path1))
//                File.Delete(path1);
//            if (File.Exists(path2))
//                File.Delete(path2);
//            if (File.Exists(path3))
//                File.Delete(path3);

//            using (var file1 = HistorianArchiveFile.CreateFile(path1, CompressionMethod.None))
//            using (var file2 = HistorianArchiveFile.CreateFile(path2, CompressionMethod.DeltaEncoded))
//            using (var file3 = HistorianArchiveFile.CreateFile(path3, CompressionMethod.TimeSeriesEncoded))
//            using (var edit1 = file1.BeginEdit())
//            using (var edit2 = file2.BeginEdit())
//            using (var edit3 = file3.BeginEdit())
//            {
//                var hist = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d");
//                //var hist = new OldHistorianReader(@"D:\Projects\Applications\openPDC\Synchrophasor\Current Version\Build\Output\Debug\Applications\openPDC\Archive\ppa_archive_2012-11-06 16!00!51.233_to_2012-11-06 16!07!16.933.d");
//                Action<OldHistorianReader.Points> del = (x) =>
//                    {
//                        tree0.Add((ulong)x.Time.Ticks, (ulong)x.PointID, x.flags, *(uint*)&x.Value);
//                    };
//                hist.Read(del);

//                //tree0 = SortPoints(tree0);

//                var scan0 = tree0.GetTreeScanner();
//                scan0.SeekToKey(0, 0);
//                ulong key1, key2, value1, value2;
//                while (scan0.Read(out key1, out key2, out value1, out value2))
//                {
//                    edit1.AddPoint(key1, key2, value1, value2);
//                    edit2.AddPoint(key1, key2, value1, value2);
//                    edit3.AddPoint(key1, key2, value1, value2);
//                }

//                edit1.Commit();
//                edit2.Commit();
//                edit3.Commit();
//            }
//        }


//        public static SortedTree256 SortPoints(SortedTree256 tree)
//        {
//            ulong maxPointId = 0;
//            var scan = tree.GetTreeScanner();
//            ulong key1, key2, value1, value2;
//            scan.SeekToKey(0, 0);
//            while (scan.Read(out key1, out key2, out value1, out value2))
//            {
//                maxPointId = Math.Max(key2, maxPointId);
//            }

//            var map = new PointValue[(int)maxPointId + 1];

//            scan.SeekToKey(0, 0);
//            while (scan.Read(out key1, out key2, out value1, out value2))
//            {
//                if (map[(int)key2] is null)
//                    map[(int)key2] = new PointValue();
//                map[(int)key2].Value = value2;
//            }

//            var list = new List<PointValue>();
//            foreach (var pv in map)
//            {
//                if (pv != null)
//                    list.Add(pv);
//            }
//            list.Sort();

//            for (uint x = 0; x < list.Count; x++)
//            {
//                list[(int)x].NewPointId = x;
//            }

//            var tree2 = SortedTree256.Create(new BinaryStream(), 4096);
//            scan.SeekToKey(0, 0);
//            while (scan.Read(out key1, out key2, out value1, out value2))
//            {
//                tree2.Add(key1, map[(int)key2].NewPointId, value1, value2);
//            }

//            return tree2;
//        }

//        class PointValue : IComparable<PointValue>
//        {
//            public ulong NewPointId;
//            public ulong Value;

//            /// <summary>
//            /// Compares the current object with another object of the same type.
//            /// </summary>
//            /// <returns>
//            /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
//            /// </returns>
//            /// <param name="other">An object to compare with this object.</param>
//            public int CompareTo(PointValue other)
//            {
//                return Value.CompareTo(other.Value);
//            }
//        }

//    }
//}

