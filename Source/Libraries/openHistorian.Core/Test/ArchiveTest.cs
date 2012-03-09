using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace openHistorian.Core
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
            sw.Start();
            reader.Read();
            s_archive.Close();
            s_archive = null;
            sw.Stop();
            MessageBox.Show(s_points + "points " + sw.Elapsed.TotalSeconds + "sec " + s_points / sw.Elapsed.TotalSeconds + " " + openHistorian.Core.StorageSystem.File.DiskIoBase.ChecksumCount);

        }

        static void ReaderNewPoint(Points pt)
        {
            s_points++;
            s_archive.AddPoint(pt.Time,pt.PointID,pt.flags, pt.Value);
        }
    }
}
