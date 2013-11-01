//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using NUnit.Framework;
//using openHistorian.Archive;
//using openHistorian.FileStructure;
//using GSF.IO.Unmanaged;

//namespace openHistorian.Collections
//{
//    [TestFixture]
//    internal class SortedTree256Test1
//    {
//        const uint Count = 8600;

//        [Test]
//        public void TestDeltaEncoded()
//        {
//            using (BinaryStream bs = new BinaryStream())
//            {
//                var tree = SortedTree256.Create(bs, 4096, CompressionMethod.TimeSeriesEncoded);
//                SortedTree256ArchiveFileDelta(tree);
//            }

//        }


//        public void SortedTree256ArchiveFileDelta(SortedTree256 tree)
//        {
//            Random r = new Random(3);

//            for (ulong v1 = 1; v1 < 36; v1++)
//                for (ulong v2 = 1; v2 < Count; v2++)
//                    tree.Add(v1 * 2342523, v2, 0, (ulong)r.Next());

//            r = new Random(3);

//            for (ulong v1 = 1; v1 < 36; v1++)
//                for (ulong v2 = 1; v2 < Count; v2++)
//                {
//                    if (v2 == 4)
//                        v2 = v2;
//                    ulong vv1, vv2;
//                    tree.Get(v1 * 2342523, v2, out vv1, out vv2);
//                    Assert.AreEqual(vv1, 0ul);
//                    Assert.AreEqual(vv2, (ulong)r.Next());
//                }

//            r = new Random(3);

//            ulong key1, key2, value1, value2;
//            var scanner = tree.GetTreeScanner();
//            scanner.SeekToKey(0, 0);
//            for (ulong v1 = 1; v1 < 36; v1++)
//            {
//                for (ulong v2 = 1; v2 < Count; v2++)
//                {
//                    Assert.IsTrue(scanner.Read(out key1, out key2, out value1, out value2));
//                    Assert.AreEqual(key1, v1 * 2342523);
//                    Assert.AreEqual(key2, v2);
//                    Assert.AreEqual(value1, 0ul);
//                    Assert.AreEqual(value2, (ulong)r.Next());
//                }
//            }
//            Assert.IsFalse(scanner.Read(out key1, out key2, out value1, out value2));


//        }

//    }
//}

