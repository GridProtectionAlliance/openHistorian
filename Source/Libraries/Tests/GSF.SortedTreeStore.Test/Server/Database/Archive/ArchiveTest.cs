using System.Diagnostics;
using NUnit.Framework;
using GSF.SortedTreeStore.Storage;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Tree.TreeNodes;

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

        private static SortedTreeTable<HistorianKey, HistorianValue>.Editor s_fileEditor;
        private static int s_points;

        public static void Test()
        {
            s_points = 0;
            //string file = "G:\\ArchiveTest.hfs";
            //if (File.Exists(file))
            //    File.Delete(file);
            //s_archive = new Archive(file);
            SortedTreeTable<HistorianKey, HistorianValue> sortedTreeFile = SortedTreeFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(SortedTree.FixedSizeNode);
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

            SortedTreeScannerBase<HistorianKey, HistorianValue> reader1 = sortedTreeFile.AcquireReadSnapshot().CreateReadSnapshot().GetTreeScanner();
            reader1.SeekToStart();

            while (reader1.Read())
            {
                cnt++;
            }

            sortedTreeFile.Dispose();
            sortedTreeFile = null;

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