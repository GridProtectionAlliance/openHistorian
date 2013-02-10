using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian.Archive;
using openHistorian.FileStructure;
using openHistorian.FileStructure.IO;

namespace openHistorian.UnitTests.Archive
{
    [TestFixture]
    public class TestPageSizes
    {

        [Test]
        public void Test4096()
        {
            Test(512);
            Test(1024);
            Test(2048);
            Test(4096);
            Test(4096 << 1);
            Test(4096 << 2);
            Test(4096 << 3);
            Test(4096 << 4);



            //string fileName = @"c:\temp\testFile.d2";
            //TestFile(1024, fileName);
            //TestFile(2048, fileName);
            //TestFile(4096, fileName);
            //TestFile(4096 << 1, fileName);
            //TestFile(4096 << 2, fileName);
            //TestFile(4096 << 3, fileName);
            //TestFile(4096 << 4, fileName);
        }

        void Test(int pageSize)
        {

            DiskIoSession.ReadCount = 0;
            DiskIoSession.WriteCount = 0;
            Footer.ChecksumCount = 0;

            var sw = new Stopwatch();
            sw.Start();
            using (var af = ArchiveFile.CreateInMemory(CompressionMethod.TimeSeriesEncoded, pageSize))
            using (var edit = af.BeginEdit())
            {
                for (uint x = 0; x < 1000000; x++)
                {
                    //if (x == 1000)
                    //    DiskIoSession.BreakOnIO = true;
                    edit.AddPoint(1, x, 0, 0);
                }

                edit.Commit();
            }
            sw.Stop();

            Console.WriteLine("Size: " + pageSize.ToString());
            Console.WriteLine("Rate: " + (1 / sw.Elapsed.TotalSeconds).ToString());
            Console.WriteLine("Read: " + (DiskIoSession.ReadCount).ToString());
            Console.WriteLine("Write: " + (DiskIoSession.WriteCount).ToString());
            Console.WriteLine("Checksums: " + (Footer.ChecksumCount).ToString());

        }

        void TestFile(int pageSize, string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            var sw = new Stopwatch();
            sw.Start();
            using (var af = ArchiveFile.CreateFile(fileName, CompressionMethod.TimeSeriesEncoded2, pageSize))
            using (var edit = af.BeginEdit())
            {
                for (uint x = 0; x < 1000000; x++)
                    edit.AddPoint(1, x, 0, 0);

                edit.Commit();
            }
            sw.Stop();
            Console.WriteLine("Size: " + pageSize + " Rate: " + (1 / sw.Elapsed.TotalSeconds).ToString());
        }


    }
}
