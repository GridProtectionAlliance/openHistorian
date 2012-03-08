using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace openHistorian.Core.StorageSystem.File
{
    public class BenchmarkPageSizes
    {
        public static void Test()
        {
            //Create a file
            DiskIoMemoryStream stream = new DiskIoMemoryStream();
            FileAllocationTable fat = FileAllocationTable.CreateFileAllocationTable(stream);
            fat = FileAllocationTable.OpenHeader(stream);
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream, fat);
            ArchiveFileStream fs1 = trans.CreateFile(id, 1234);

            Stopwatch sw = new Stopwatch();
            //write 1GB of data to it.

            //byte[] data = new byte[ArchiveConstants.DataBlockDataLength];

            byte[][] data = new byte[256][];
            for (int x = 0; x < 256; x++)
            {
                data[(byte)x] = new byte[ArchiveConstants.DataBlockDataLength];
            }

            DiskIoBase.ChecksumCount = 0;
            sw.Start();
            const int SizeMB = 10;
            int y = 0;
            for (int x = 0; x < SizeMB * 1024 * 1024; x += data[0].Length, y++)
            {
                fs1.Write(data[(byte)y], 0, data[(byte)y].Length);
            }
            sw.Stop();
            System.Windows.Forms.MessageBox.Show((SizeMB / sw.Elapsed.TotalSeconds).ToString() + " " + ((DiskIoBase.ChecksumCount * 4.0 / 1024) / SizeMB).ToString());

            trans.Commit();

            fat = FileAllocationTable.OpenHeader(stream);
            trans = new TransactionalEdit(stream, fat);
            fs1 = trans.OpenFile(0);

            DiskIoBase.ChecksumCount = 0;
            sw.Start();
            for (int x = 0; x < SizeMB * 1024 * 1024; x += data[0].Length, y++)
            {
                fs1.Write(data[(byte)y], 0, data[(byte)y].Length);
            }
            sw.Stop();
            System.Windows.Forms.MessageBox.Show((SizeMB / sw.Elapsed.TotalSeconds).ToString() + " " + ((DiskIoBase.ChecksumCount * 4.0 / 1024) / SizeMB).ToString());

            sw.Reset();
            DiskIoBase.ChecksumCount = 0;

            sw.Start();
            fs1.Position = 0;
            for (int x = 0; x < SizeMB * 1024 * 1024; x += data[0].Length, y++)
            {
                fs1.Write(data[(byte)y], 0, data[(byte)y].Length);
            }
            sw.Stop();
            System.Windows.Forms.MessageBox.Show((SizeMB / sw.Elapsed.TotalSeconds).ToString() + " " + ((DiskIoBase.ChecksumCount * 4.0 / 1024) / SizeMB).ToString());
            DiskIoBase.ChecksumCount = 0;

            sw.Reset();
            sw.Start();
            fs1.Position = 0;
            for (int x = 0; x < SizeMB * 1024 * 1024; x += data[0].Length, y++)
            {
                fs1.Read(data[(byte)y], 0, data[(byte)y].Length);
            }
            sw.Stop();
            System.Windows.Forms.MessageBox.Show((SizeMB / sw.Elapsed.TotalSeconds).ToString() + " " + ((DiskIoBase.ChecksumCount * 4.0 / 1024) / SizeMB).ToString());
            DiskIoBase.ChecksumCount = 0;

            trans.Commit();
        }
    }
}
