using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian.Archive;

namespace openHistorian.UnitTests.Server.Database.Archive
{
    [TestFixture]
    public class ArchiveFileSampleConvert
    {
        [Test]
        public void WriteFile()
        {
            if (File.Exists("c:\\temp\\ArchiveTestFileBig.d2"))
                File.Delete("c:\\temp\\ArchiveTestFileBig.d2");
            using (var af = ArchiveFile.CreateInMemory(CompressionMethod.TimeSeriesEncoded))
            //using (var af = ArchiveFile.CreateFile("c:\\temp\\ArchiveTestFileBig.d2", CompressionMethod.TimeSeriesEncoded))
            {
                Random r = new Random(3);

                for (ulong v1 = 1; v1 < 36; v1++)
                {
                    using (var edit = af.BeginEdit())
                    {
                        for (ulong v2 = 1; v2 < 86000; v2++)
                        {
                            edit.AddPoint(v1 * 2342523, v2, 0, (ulong)r.Next());
                        }
                        edit.Commit();
                    }
                    af.Count();
                }
                af.Count();
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
            using (var af = ArchiveFile.OpenFile("c:\\temp\\ArchiveTestFileBig.d2", AccessMode.ReadOnly))
            {
                Random r = new Random(3);

                ulong key1, key2, value1, value2;
                var scanner = af.AcquireReadSnapshot().CreateReadSnapshot().GetTreeScanner();
                scanner.SeekToKey(0, 0);
                for (ulong v1 = 1; v1 < 36; v1++)
                {
                    for (ulong v2 = 1; v2 < 86000; v2++)
                    {
                        Assert.IsTrue(scanner.Read(out key1, out key2, out value1, out value2));
                        Assert.AreEqual(key1, v1 * 2342523);
                        Assert.AreEqual(key2, v2);
                        Assert.AreEqual(value1, 0ul);
                        Assert.AreEqual(value2, (ulong)r.Next());
                    }
                }
            }
        }

    }
}
