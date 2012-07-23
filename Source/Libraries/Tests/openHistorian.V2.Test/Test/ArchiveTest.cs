using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2
{
    
    [TestClass()]
    public class ArchiveTest
    {
        [TestMethod()]
        public void TestMethod()
        {
            Test();
        }

        static ArchiveFile s_archiveFile;
        static int s_points;

        public static void Test()
        {
            s_points = 0;
            //string file = "G:\\ArchiveTest.hfs";
            //if (File.Exists(file))
            //    File.Delete(file);
            //s_archive = new Archive(file);
            s_archiveFile = new ArchiveFile();

            s_archiveFile.BeginEdit();

            HistorianReader reader = new HistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
            reader.NewPoint += ReaderNewPoint;
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sw.Start();
            reader.Read();
            sw.Stop();
            int cnt = 0;
            sw2.Start();

            s_archiveFile.CommitEdit();

            long oldCount = FileStructure.DiskIoSession.ChecksumCount;

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

            //MessageBox.Show(openHistorian.V2.Collections.KeyValue.BasicTreeBase.PointsAdded + " " +
            //                openHistorian.V2.Collections.KeyValue.BasicTreeBase.ShortcutsTaken);
            MessageBox.Show(s_points + "points " + sw.Elapsed.TotalSeconds + "sec " + s_points / sw.Elapsed.TotalSeconds + " " + oldCount);
            MessageBox.Show(s_points + "points " + sw2.Elapsed.TotalSeconds + "sec " + s_points / sw2.Elapsed.TotalSeconds + " cnt:" + cnt + " " + (FileStructure.DiskIoSession.ChecksumCount - oldCount));
        }

        unsafe static void ReaderNewPoint(HistorianReader.Points pt)
        {
            //if (s_points % 10000 == 0)
            //    Clipboard.SetText(s_points.ToString());
            s_points++;
            //if (s_points % 10000 == 0)
            s_archiveFile.AddPoint((ulong)pt.Time.Ticks, (ulong)pt.PointID, (ulong)pt.flags, (ulong)*(int*)&pt.Value);
        }
    }
}
