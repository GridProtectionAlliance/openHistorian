using System;
using System.Diagnostics;
using NUnit.Framework;
using openHistorian.Server.Database.Archive;

namespace openHistorian
{
    
    [TestFixture()]
    public class ArchiveTest
    {
        [Test()]
        public void TestMethod()
        {
            Test();
        }

        static ArchiveFile.ArchiveFileEditor s_fileEditor;
        static int s_points;

        public static void Test()
        {
            s_points = 0;
            //string file = "G:\\ArchiveTest.hfs";
            //if (File.Exists(file))
            //    File.Delete(file);
            //s_archive = new Archive(file);
            ArchiveFile s_archiveFile = ArchiveFile.CreateInMemory();
            int cnt = 0;


            Stopwatch sw;
            Stopwatch sw2;
            using (var fileEditor = s_archiveFile.BeginEdit())
            {
                s_fileEditor = fileEditor;
                OldHistorianReader reader = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
                reader.NewPoint += ReaderNewPoint;
                sw = new Stopwatch();
                sw2 = new Stopwatch();
                sw.Start();
                reader.Read();
                sw.Stop();
                
                sw2.Start();

                fileEditor.Commit();
            }
            long oldCount = FileStructure.DiskIo.ChecksumCount;

            var reader1 = s_archiveFile.CreateSnapshot().OpenInstance().GetDataRange();
            reader1.SeekToKey(0,0);

            ulong value1, value2, key1, key2;
            while (reader1.GetNextKey(out key1, out key2, out value1, out value2))
            {
                cnt++;
           
                //dest.AddPoint(key1, key2, value1, value2);
            }

            //foreach (var pts in s_archive.GetData(DateTime.MinValue, DateTime.MaxValue))
            //{
            //    if (pts.Item2 == 102)
            //    {
            //        cnt++;
            //    }
            //}
            sw2.Stop();

            s_archiveFile.Dispose();
            s_archiveFile = null;

            //MessageBox.Show(openHistorian.Collections.KeyValue.BasicTreeBase.PointsAdded + " " +
            //                openHistorian.Collections.KeyValue.BasicTreeBase.ShortcutsTaken);
            //MessageBox.Show(s_points + "points " + sw.Elapsed.TotalSeconds + "sec " + s_points / sw.Elapsed.TotalSeconds + " " + oldCount);
            //MessageBox.Show(s_points + "points " + sw2.Elapsed.TotalSeconds + "sec " + s_points / sw2.Elapsed.TotalSeconds + " cnt:" + cnt + " " + (FileStructure.DiskIo.ChecksumCount - oldCount));
        }

        unsafe static void ReaderNewPoint(OldHistorianReader.Points pt)
        {
            //if (s_points % 10000 == 0)
            //    Clipboard.SetText(s_points.ToString());
            s_points++;
            //if (s_points % 10000 == 0)
            s_fileEditor.AddPoint((ulong)pt.Time.Ticks, (ulong)pt.PointID, (ulong)pt.flags, (ulong)*(int*)&pt.Value);
        }
    }
}
