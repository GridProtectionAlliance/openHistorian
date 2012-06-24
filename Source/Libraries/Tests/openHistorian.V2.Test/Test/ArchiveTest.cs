using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using openHistorian.V2.Service.Instance.File;

namespace openHistorian.V2
{
    
    //[TestClass()]
    public class ArchiveTest
    {
        //[TestMethod()]
        public void TestMethod()
        {
            Test();
        }

        static Archive s_archive;
        static int s_points;

        public static void Test()
        {
            s_points = 0;
            //string file = "G:\\ArchiveTest.hfs";
            //if (File.Exists(file))
            //    File.Delete(file);
            //s_archive = new Archive(file);
            s_archive = new Archive();

            s_archive.BeginEdit();

            HistorianReader reader = new HistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
            reader.NewPoint += ReaderNewPoint;
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sw.Start();
            reader.Read();
            sw.Stop();
            int cnt = 0;
            sw2.Start();

            s_archive.CommitEdit();

            long oldCount = FileSystem.DiskIoSession.ChecksumCount;

            var reader1 = s_archive.CreateSnapshot().OpenInstance().GetDataRange();
            reader1.SeekToKey(long.MinValue, long.MinValue);

            long value1, value2, key1, key2;
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

            s_archive.Close();
            s_archive = null;


            MessageBox.Show(s_points + "points " + sw.Elapsed.TotalSeconds + "sec " + s_points / sw.Elapsed.TotalSeconds + " " + oldCount);
            MessageBox.Show(s_points + "points " + sw2.Elapsed.TotalSeconds + "sec " + s_points / sw2.Elapsed.TotalSeconds + " cnt:" + cnt + " " + (FileSystem.DiskIoSession.ChecksumCount - oldCount));
        }

        unsafe static void ReaderNewPoint(HistorianReader.Points pt)
        {
            //if (s_points % 10000 == 0)
            //    Clipboard.SetText(s_points.ToString());
            s_points++;
            //if (s_points % 10000 == 0)
                s_archive.AddPoint(pt.Time.Ticks, pt.PointID, pt.flags, *(int*)&pt.Value);
        }
    }
}
