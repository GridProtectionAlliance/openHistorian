using System;
using GSF.IO.FileStructure.Media;
using GSF.IO.Unmanaged;
using NUnit.Framework;
using openHistorian;

namespace GSF.IO.FileStructure
{
    [TestFixture]
    public class SubFileStreamTest
    {
        [Test]
        public void TestRandomWriteAmplification()
        {
            MemoryPoolTest.TestMemoryLeak();
            double size;

            using (TransactionalFileStructure file = TransactionalFileStructure.CreateInMemory(4096))
            using (TransactionalEdit edit = file.BeginEdit())
            using (SubFileStream stream = edit.CreateFile(SubFileName.CreateRandom()))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                Stats.ChecksumCount = 0;
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
            Console.WriteLine("Checksums: " + (Stats.ChecksumCount / size).ToString("0.000"));
            MemoryPoolTest.TestMemoryLeak();
        }


        [Test]
        public void TestSequentialWriteAmplification()
        {
            MemoryPoolTest.TestMemoryLeak();
            double size;
            Stats.ChecksumCount = 0;
            DiskIoSession.WriteCount = 0;
            DiskIoSession.ReadCount = 0;

            using (TransactionalFileStructure file = TransactionalFileStructure.CreateInMemory(4096))
            using (TransactionalEdit edit = file.BeginEdit())
            using (SubFileStream stream = edit.CreateFile(SubFileName.CreateRandom()))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                Stats.ChecksumCount = 0;
                DiskIoSession.WriteCount = 0;
                DiskIoSession.ReadCount = 0;

                //Write 8 million
                for (long s = 0; s < 1000000; s++)
                {
                    bs.Write(s);
                }
                size = bs.Position / 4096.0;
            }

            Console.WriteLine("Read: " + (DiskIoSession.ReadCount / size).ToString("0.0"));
            Console.WriteLine("Write: " + (DiskIoSession.WriteCount / size).ToString("0.0"));
            Console.WriteLine("Checksums: " + (Stats.ChecksumCount / size).ToString("0.0"));
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void TestSequentialReWriteAmplification()
        {
            MemoryPoolTest.TestMemoryLeak();
            double size;
            Stats.ChecksumCount = 0;
            DiskIoSession.WriteCount = 0;
            DiskIoSession.ReadCount = 0;

            using (TransactionalFileStructure file = TransactionalFileStructure.CreateInMemory(4096))
            using (TransactionalEdit edit = file.BeginEdit())
            using (SubFileStream stream = edit.CreateFile(SubFileName.CreateRandom()))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                Stats.ChecksumCount = 0;
                DiskIoSession.WriteCount = 0;
                DiskIoSession.ReadCount = 0;

                //Write 8 million
                for (long s = 0; s < 1000000; s++)
                {
                    bs.Write(s);
                }
                size = bs.Position / 4096.0;
                bs.Position = 0;

                Stats.ChecksumCount = 0;
                DiskIoSession.WriteCount = 0;
                DiskIoSession.ReadCount = 0;

                for (long s = 0; s < 1000000; s++)
                {
                    bs.Write(s);
                }
            }

            Console.WriteLine("Read: " + (DiskIoSession.ReadCount / size).ToString("0.0"));
            Console.WriteLine("Write: " + (DiskIoSession.WriteCount / size).ToString("0.0"));
            Console.WriteLine("Checksums: " + (Stats.ChecksumCount / size).ToString("0.0"));
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void TestSequentialReadAmplification()
        {
            MemoryPoolTest.TestMemoryLeak();
            double size;
            Stats.ChecksumCount = 0;
            DiskIoSession.WriteCount = 0;
            DiskIoSession.ReadCount = 0;

            using (TransactionalFileStructure file = TransactionalFileStructure.CreateInMemory(4096))
            using (TransactionalEdit edit = file.BeginEdit())
            using (SubFileStream stream = edit.CreateFile(SubFileName.CreateRandom()))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                //Write 8 million
                for (long s = 0; s < 1000000; s++)
                {
                    bs.Write(s);
                }
                size = bs.Position / 4096.0;
                bs.Position = 0;

                Stats.ChecksumCount = 0;
                DiskIoSession.WriteCount = 0;
                DiskIoSession.ReadCount = 0;

                for (long s = 0; s < 1000000; s++)
                {
                    bs.ReadInt64();
                }
            }

            Console.WriteLine("Read: " + (DiskIoSession.ReadCount / size).ToString("0.0"));
            Console.WriteLine("Write: " + (DiskIoSession.WriteCount / size).ToString("0.0"));
            Console.WriteLine("Checksums: " + (Stats.ChecksumCount / size).ToString("0.0"));
            MemoryPoolTest.TestMemoryLeak();
        }
    }
}