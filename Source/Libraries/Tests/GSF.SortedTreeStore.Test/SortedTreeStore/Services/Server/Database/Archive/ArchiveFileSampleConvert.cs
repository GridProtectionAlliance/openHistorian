using System;
using System.IO;
using GSF.Snap;
using NUnit.Framework;
using GSF.Snap.Storage;
using GSF.Snap.Tree;
using openHistorian.Snap;

namespace openHistorian.UnitTests.Server.Database.Archive
{
    [TestFixture]
    public class ArchiveFileSampleConvert
    {
        [Test]
        public void WriteFile()
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            if (File.Exists("c:\\temp\\ArchiveTestFileBig.d2"))
                File.Delete("c:\\temp\\ArchiveTestFileBig.d2");
            //using (var af = ArchiveFile.CreateInMemory(CompressionMethod.TimeSeriesEncoded))
            using (SortedTreeFile af = SortedTreeFile.CreateFile("c:\\temp\\ArchiveTestFileBig.d2"))
            using (SortedTreeTable<HistorianKey, HistorianValue> af2 = af.OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                Random r = new Random(3);

                for (ulong v1 = 1; v1 < 36; v1++)
                {
                    using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = af2.BeginEdit())
                    {
                        for (ulong v2 = 1; v2 < 86000; v2++)
                        {
                            key.Timestamp = v1 * 2342523;
                            key.PointID = v2;
                            value.Value1 = (ulong)r.Next();
                            value.Value3 = 0;

                            edit.AddPoint(key, value);
                        }
                        edit.Commit();
                    }
                    af2.Count();
                }
                af2.Count();
            }
        }

        //[Test]
        //public void WriteFile2()
        //{
        //    using (var af = ArchiveFile.CreateInMemory(CompressionMethod.None))
        //    //using (var af = ArchiveFile.CreateInMemory(CompressionMethod.TimeSeriesEncoded))
        //    //using (var af = ArchiveFile.CreateFile("c:\\temp\\ArchiveTestFileBig.d2", CompressionMethod.TimeSeriesEncoded))
        //    {
        //        Random r = new Random(3);

        //        for (ulong v1 = 1; v1 < 360; v1++)
        //        {
        //            long cnt = af.Count();
        //            using (var edit = af.BeginEdit())
        //            {
        //                for (ulong v2 = 1; v2 < 28; v2++)
        //                {
        //                    Assert.AreEqual(cnt, af.Count());
        //                    if (v1 == 128)
        //                        v1 = v1;

        //                    edit.AddPoint(v1 * 2342523, v2, 0, (ulong)r.Next());
        //                    Assert.AreEqual(cnt, af.Count());
        //                    Assert.AreEqual(cnt + (long)v2, edit.GetRange().Count());
        //                }
        //                edit.Commit();
        //            }
        //            af.Count();
        //        }
        //        af.Count();
        //    }
        //}

        //[Test]
        //public void WriteFile2()
        //{
        //    if (File.Exists("c:\\temp\\ArchiveTestFileBig.d2"))
        //        File.Delete("c:\\temp\\ArchiveTestFileBig.d2");
        //    using (var af = ArchiveFile.CreateFile("c:\\temp\\ArchiveTestFileBig.d2", CompressionMethod.DeltaEncoded))
        //    {
        //        Random r = new Random(3);
        //        using (var edit = af.BeginEdit())
        //        {
        //            for (ulong v1 = 1; v1 < 36; v1++)
        //            {

        //                for (ulong v2 = 1; v2 < 86000; v2++)
        //                {
        //                    edit.AddPoint(v1 * 2342523, v2, 0, (ulong)r.Next());
        //                }

        //            }
        //            edit.Commit();
        //        }

        //    }
        //}

        [Test]
        public void ReadFile()
        {
            using (SortedTreeFile af = SortedTreeFile.OpenFile("c:\\temp\\ArchiveTestFileBig.d2", isReadOnly: true))
            using (SortedTreeTable<HistorianKey, HistorianValue> af2 = af.OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();
                Random r = new Random(3);

                SortedTreeScannerBase<HistorianKey, HistorianValue> scanner = af2.AcquireReadSnapshot().CreateReadSnapshot().GetTreeScanner();
                scanner.SeekToStart();
                for (ulong v1 = 1; v1 < 36; v1++)
                {
                    for (ulong v2 = 1; v2 < 86000; v2++)
                    {
                        Assert.IsTrue(scanner.Read(key, value));
                        Assert.AreEqual(key.Timestamp, v1 * 2342523);
                        Assert.AreEqual(key.PointID, v2);
                        Assert.AreEqual(value.Value3, 0ul);
                        Assert.AreEqual(value.Value1, (ulong)r.Next());
                    }
                }
            }
        }
    }
}