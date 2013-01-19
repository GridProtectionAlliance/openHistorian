using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.Collections.KeyValue;
using GSF.IO.Unmanaged;

namespace openHistorian.FileStructure
{
    [TestFixture]
    public class SubFileStreamTest
    {
        [Test]
        public void TestRandomWriteAmplification()
        {
            double size;

            using (TransactionalFileStructure file = TransactionalFileStructure.CreateInMemory(4096))
            using (var edit = file.BeginEdit())
            using (var stream = edit.CreateFile(Guid.NewGuid(), 12))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                DiskIo.ChecksumCount = 0;
                DiskIoSession.WriteCount = 0;
                DiskIoSession.ReadCount = 0;

                bs.Position = 8 * 1000000 - 8;
                bs.Write(0);
                Random r = new Random(2425);
                
                //Write 8 million
                for (long s = 0; s < 1000000; s++)
                {
                    bs.Position = r.Next(8000000 - 8);
                    bs.Write((byte)1);
                }
                size = 1000000;
            }

            Console.WriteLine("Read: " + (DiskIoSession.ReadCount / size).ToString("0.000"));
            Console.WriteLine("Write: " + (DiskIoSession.WriteCount / size).ToString("0.000"));
            Console.WriteLine("Checksums: " + (DiskIo.ChecksumCount / size).ToString("0.000"));
        }


        [Test]
        public void TestSequentialWriteAmplification()
        {
            double size;
            DiskIo.ChecksumCount = 0;
            DiskIoSession.WriteCount = 0;
            DiskIoSession.ReadCount = 0;

            using (TransactionalFileStructure file = TransactionalFileStructure.CreateInMemory(4096))
            using (var edit = file.BeginEdit())
            using (var stream = edit.CreateFile(Guid.NewGuid(), 12))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                DiskIo.ChecksumCount = 0;
                DiskIoSession.WriteCount = 0;
                DiskIoSession.ReadCount = 0;

                //Write 8 million
                for (long s = 0; s < 1000000; s++)
                {
                    bs.Write(s);
                }
                size = bs.Position/4096.0;
            }

            Console.WriteLine("Read: " + (DiskIoSession.ReadCount / size).ToString("0.0"));
            Console.WriteLine("Write: " + (DiskIoSession.WriteCount / size).ToString("0.0"));
            Console.WriteLine("Checksums: " + (DiskIo.ChecksumCount / size).ToString("0.0"));
        }

        [Test]
        public void TestSequentialReWriteAmplification()
        {
            double size;
            DiskIo.ChecksumCount = 0;
            DiskIoSession.WriteCount = 0;
            DiskIoSession.ReadCount = 0;

            using (TransactionalFileStructure file = TransactionalFileStructure.CreateInMemory(4096))
            using (var edit = file.BeginEdit())
            using (var stream = edit.CreateFile(Guid.NewGuid(), 12))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                DiskIo.ChecksumCount = 0;
                DiskIoSession.WriteCount = 0;
                DiskIoSession.ReadCount = 0;

                //Write 8 million
                for (long s = 0; s < 1000000; s++)
                {
                    bs.Write(s);
                }
                size = bs.Position / 4096.0;
                bs.Position = 0;

                DiskIo.ChecksumCount = 0;
                DiskIoSession.WriteCount = 0;
                DiskIoSession.ReadCount = 0;

                for (long s = 0; s < 1000000; s++)
                {
                    bs.Write(s);
                }
            }

            Console.WriteLine("Read: " + (DiskIoSession.ReadCount / size).ToString("0.0"));
            Console.WriteLine("Write: " + (DiskIoSession.WriteCount / size).ToString("0.0"));
            Console.WriteLine("Checksums: " + (DiskIo.ChecksumCount / size).ToString("0.0"));
        }

        [Test]
        public void TestSequentialReadAmplification()
        {
            double size;
            DiskIo.ChecksumCount = 0;
            DiskIoSession.WriteCount = 0;
            DiskIoSession.ReadCount = 0;

            using (TransactionalFileStructure file = TransactionalFileStructure.CreateInMemory(4096))
            using (var edit = file.BeginEdit())
            using (var stream = edit.CreateFile(Guid.NewGuid(), 12))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                //Write 8 million
                for (long s = 0; s < 1000000; s++)
                {
                    bs.Write(s);
                }
                size = bs.Position / 4096.0;
                bs.Position = 0;

                DiskIo.ChecksumCount = 0;
                DiskIoSession.WriteCount = 0;
                DiskIoSession.ReadCount = 0;

                for (long s = 0; s < 1000000; s++)
                {
                    bs.ReadInt64();
                }
            }

            Console.WriteLine("Read: " + (DiskIoSession.ReadCount / size).ToString("0.0"));
            Console.WriteLine("Write: " + (DiskIoSession.WriteCount / size).ToString("0.0"));
            Console.WriteLine("Checksums: " + (DiskIo.ChecksumCount / size).ToString("0.0"));

        }

    }
}
