using System.Diagnostics;
using NUnit.Framework;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

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

        private static ArchiveTable<HistorianKey, HistorianValue>.Editor s_fileEditor;
        private static int s_points;

        public static void Test()
        {
            s_points = 0;
            //string file = "G:\\ArchiveTest.hfs";
            //if (File.Exists(file))
            //    File.Delete(file);
            //s_archive = new Archive(file);
            ArchiveTable<HistorianKey, HistorianValue> s_archiveFile = ArchiveFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid);
            int cnt = 0;


            Stopwatch sw;
            Stopwatch sw2;
            //using (ArchiveFile<HistorianKey, HistorianValue>.Editor fileEditor = s_archiveFile.BeginEdit())
            //{
            //    s_fileEditor = fileEditor;
            //    OldHistorianReader reader = new OldHistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
            //    reader.NewPoint += ReaderNewPoint;
            //    sw = new Stopwatch();
            //    sw2 = new Stopwatch();
            //    sw.Start();
            //    reader.Read();
            //    sw.Stop();

            //    sw2.Start();

            //    fileEditor.Commit();
            //}
            //long oldCount = Statistics.ChecksumCount;

            TreeScannerBase<HistorianKey, HistorianValue> reader1 = s_archiveFile.AcquireReadSnapshot().CreateReadSnapshot().GetTreeScanner();
            reader1.SeekToStart();

            while (reader1.Read())
            {
                cnt++;
            }

            s_archiveFile.Dispose();
            s_archiveFile = null;

            //MessageBox.Show(openHistorian.Collections.KeyValue.BasicTreeBase.PointsAdded + " " +
            //                openHistorian.Collections.KeyValue.BasicTreeBase.ShortcutsTaken);
            //MessageBox.Show(s_points + "points " + sw.Elapsed.TotalSeconds + "sec " + s_points / sw.Elapsed.TotalSeconds + " " + oldCount);
            //MessageBox.Show(s_points + "points " + sw2.Elapsed.TotalSeconds + "sec " + s_points / sw2.Elapsed.TotalSeconds + " cnt:" + cnt + " " + (FileStructure.DiskIo.ChecksumCount - oldCount));
        }

        //private static unsafe void ReaderNewPoint(OldHistorianReader.Points pt)
        //{
        //    //if (s_points % 10000 == 0)
        //    //    Clipboard.SetText(s_points.ToString());
        //    s_points++;
        //    //if (s_points % 10000 == 0)
        //    s_fileEditor.AddPoint((ulong)pt.Time.Ticks, (ulong)pt.PointID, (ulong)pt.flags, (ulong)*(int*)&pt.Value);
        //}
    }
}