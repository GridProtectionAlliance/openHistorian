using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Historian.PointTypes;
using Historian.StorageSystem.File;
using System.IO;

namespace Historian.DataWriter
{
    public class TestWritingPoints
    {
        internal static void Test()
        {
            PointWriter points = CreateAPointWriter();
            WriteToDisk(points);
        }
        static void WriteToDisk(PointWriter points)
        {
            ////Create a file
            //DiskIOUnbuffered stream = DiskIOUnbuffered.CreateFile("R:\\ArchiveFile.OHFS");
            DiskIOMemoryStream stream = new DiskIOMemoryStream();

            FileAllocationTable fat = FileAllocationTable.CreateFileAllocationTable(stream);
            fat = FileAllocationTable.OpenHeader(stream);
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream, fat);
            ArchiveFileStream fs1 = trans.CreateFile(id, 1234);
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            points.CommitWritingPoints(fs1);
            sw.Stop();
            System.Windows.Forms.MessageBox.Show((3000000 / sw.Elapsed.TotalSeconds).ToString());
            fs1.Dispose();
        }
        static PointWriter CreateAPointWriter()
        {
            PointWriter writer = new PointWriter();
            DateTime time = DateTime.UtcNow;
            float value = 0.0f;
            uint quality = 483u;

            for (int k = 0; k < 10000; k++)
            {
                SinglePrecisionFloatingPointQueue queue = new SinglePrecisionFloatingPointQueue(Guid.NewGuid());
                writer.AddPointDefinition(queue);
                for (int x = 0; x < 300; x++)
                {
                    queue.m_points.Add(new SinglePrecisionFloatingPoint(time, quality, value));
                    time = time.AddMilliseconds(33);
                    value = value + 12.35f;
                }
            }
            return writer;
        }
    }
}
