using System;
using System.Diagnostics;
using System.Windows.Forms;
using openHistorian.V2.Service.Instance.File;

namespace openHistorian.V2
{
    internal class ArchiveTest
    {
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

            HistorianReader reader = new HistorianReader("C:\\Unison\\GPA\\ArchiveFiles\\archive1.d");
            reader.NewPoint += ReaderNewPoint;
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sw.Start();
            reader.Read();
            sw.Stop();
            int cnt = 0;
            sw2.Start();

            foreach (var pts in s_archive.GetData(DateTime.MinValue, DateTime.MaxValue))
            {
                if (pts.Item2 == 102)
                {
                    cnt++;
                }
            }
            sw2.Stop();

            s_archive.Close();
            s_archive = null;

            MessageBox.Show(s_points + "points " + sw.Elapsed.TotalSeconds + "sec " + s_points / sw.Elapsed.TotalSeconds + " " + FileSystem.DiskIoEnhanced.ChecksumCount);
            MessageBox.Show(s_points + "points " + sw2.Elapsed.TotalSeconds + "sec " + s_points / sw2.Elapsed.TotalSeconds + " cnt:" + cnt);
        }

        static void ReaderNewPoint(HistorianReader.Points pt)
        {
            s_points++;
            //s_archive.AddPoint(pt.Time, pt.PointID, pt.flags, pt.Value);
        }
    }
}
