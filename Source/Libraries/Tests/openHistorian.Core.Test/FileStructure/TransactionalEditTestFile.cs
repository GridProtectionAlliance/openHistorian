using System;
using System.IO;
using GSF;
using GSF.IO.FileStructure.Media;
using NUnit.Framework;

namespace GSF.IO.FileStructure.Test
{
    [TestFixture()]
    public class TransactionalEditTestFile
    {
        private static int BlockSize = 4096;
        private static readonly int BlockDataLength = BlockSize - FileStructureConstants.BlockFooterLength;


        [Test()]
        public void Test()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);

            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            try
            {
                using (DiskIo stream = DiskIo.CreateFile(fileName, Globals.MemoryPool, BlockSize))
                {
                    FileHeaderBlock fat = stream.LastCommittedHeader;
                    //obtain a readonly copy of the file allocation table.
                    fat = stream.LastCommittedHeader;
                    TestCreateNewFile(stream, fat);
                    fat = stream.LastCommittedHeader;
                    TestOpenExistingFile(stream, fat);
                    fat = stream.LastCommittedHeader;
                    TestRollback(stream, fat);
                    fat = stream.LastCommittedHeader;
                    TestVerifyRollback(stream, fat);
                    Assert.IsTrue(true);
                }
            }
            finally
            {
                File.Delete(fileName);
            }

            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
        }

        private static void TestCreateNewFile(DiskIo stream, FileHeaderBlock fat)
        {
            SubFileName id1 = SubFileName.CreateRandom();
            SubFileName id2 = SubFileName.CreateRandom();
            SubFileName id3 = SubFileName.CreateRandom();
            TransactionalEdit trans = new TransactionalEdit(stream);
            //create 3 files

            SubFileStream fs1 = trans.CreateFile(id1);
            SubFileStream fs2 = trans.CreateFile(id2);
            SubFileStream fs3 = trans.CreateFile(id3);
            if (fs1.SubFile.FileName != id1)
                throw new Exception();
            //write to the three files
            SubFileStreamTest.TestSingleByteWrite(fs1);
            SubFileStreamTest.TestCustomSizeWrite(fs2, 5);
            SubFileStreamTest.TestCustomSizeWrite(fs3, BlockDataLength + 20);

            //read from them and verify content.
            SubFileStreamTest.TestCustomSizeRead(fs3, BlockDataLength + 20);
            SubFileStreamTest.TestCustomSizeRead(fs2, 5);
            SubFileStreamTest.TestSingleByteRead(fs1);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            trans.CommitAndDispose();
        }

        private static void TestOpenExistingFile(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream);
            //create 3 files

            SubFileStream fs1 = trans.OpenFile(0);
            SubFileStream fs2 = trans.OpenFile(1);
            SubFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            SubFileStreamTest.TestSingleByteRead(fs1);
            SubFileStreamTest.TestCustomSizeRead(fs2, 5);
            SubFileStreamTest.TestCustomSizeRead(fs3, BlockDataLength + 20);

            //rewrite bad data.
            SubFileStreamTest.TestSingleByteWrite(fs2);
            SubFileStreamTest.TestCustomSizeWrite(fs3, 5);
            SubFileStreamTest.TestCustomSizeWrite(fs1, BlockDataLength + 20);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            trans.CommitAndDispose();
        }

        private static void TestRollback(DiskIo stream, FileHeaderBlock fat)
        {
            SubFileName id1 = SubFileName.CreateRandom();
            SubFileName id2 = SubFileName.CreateRandom();
            SubFileName id3 = SubFileName.CreateRandom();
            TransactionalEdit trans = new TransactionalEdit(stream);

            //create 3 files additional files
            SubFileStream fs21 = trans.CreateFile(id1);
            SubFileStream fs22 = trans.CreateFile(id2);
            SubFileStream fs23 = trans.CreateFile(id3);

            //open files
            SubFileStream fs1 = trans.OpenFile(0);
            SubFileStream fs2 = trans.OpenFile(1);
            SubFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            SubFileStreamTest.TestSingleByteRead(fs2);
            SubFileStreamTest.TestCustomSizeRead(fs3, 5);
            SubFileStreamTest.TestCustomSizeRead(fs1, BlockDataLength + 20);

            //rewrite bad data.
            SubFileStreamTest.TestSingleByteWrite(fs3);
            SubFileStreamTest.TestCustomSizeWrite(fs1, 5);
            SubFileStreamTest.TestCustomSizeWrite(fs2, BlockDataLength + 20);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            fs21.Dispose();
            fs22.Dispose();
            fs23.Dispose();

            trans.RollbackAndDispose();
        }

        private static void TestVerifyRollback(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(stream);

            if (trans.Files.Count != 3)
                throw new Exception();

            //open files
            SubFileStream fs1 = trans.OpenFile(0);
            SubFileStream fs2 = trans.OpenFile(1);
            SubFileStream fs3 = trans.OpenFile(2);

            //read from them and verify content.
            SubFileStreamTest.TestSingleByteRead(fs2);
            SubFileStreamTest.TestCustomSizeRead(fs3, 5);
            SubFileStreamTest.TestCustomSizeRead(fs1, BlockDataLength + 20);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            trans.Dispose();
        }
    }
}