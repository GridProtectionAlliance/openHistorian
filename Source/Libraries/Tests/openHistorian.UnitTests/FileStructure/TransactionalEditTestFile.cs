using System;
using System.IO;
using GSF;
using NUnit.Framework;
using openHistorian.FileStructure.IO;

namespace openHistorian.FileStructure.Test
{
    [TestFixture()]
    public class TransactionalEditTestFile
    {
        static int BlockSize = 4096;
        static int BlockDataLength = BlockSize - FileStructureConstants.BlockFooterLength;


        [Test()]
        public void Test()
        {
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);

            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                using (BufferedFile bfs = new BufferedFile(fs, true, OpenMode.Create))
                using (DiskIo stream = new DiskIo(BlockSize, bfs, 0))
                {
                    FileHeaderBlock fat = stream.Header;
                    //obtain a readonly copy of the file allocation table.
                    fat = stream.Header;
                    TestCreateNewFile(stream, fat);
                    fat = stream.Header;
                    TestOpenExistingFile(stream, fat);
                    fat = stream.Header;
                    TestRollback(stream, fat);
                    fat = stream.Header;
                    TestVerifyRollback(stream, fat);
                    Assert.IsTrue(true);
                }
            }
            finally
            {
                File.Delete(fileName);
            }

            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
        }
        static void TestCreateNewFile(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(BlockSize, stream, fat);
            //create 3 files

            SubFileStream fs1 = trans.CreateFile(id, 1234);
            SubFileStream fs2 = trans.CreateFile(id, 1234);
            SubFileStream fs3 = trans.CreateFile(id, 1234);
            if (fs1.SubFile.FileExtension != id)
                throw new Exception();
            if (fs1.SubFile.FileFlags != 1234)
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

        static void TestOpenExistingFile(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(BlockSize, stream, fat);
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

            //verify origional still in tact.
            SubFileStream fs11 = trans.OpenOrigionalFile(0);
            SubFileStream fs12 = trans.OpenOrigionalFile(1);
            SubFileStream fs13 = trans.OpenOrigionalFile(2);

            SubFileStreamTest.TestSingleByteRead(fs11);
            SubFileStreamTest.TestCustomSizeRead(fs12, 5);
            SubFileStreamTest.TestCustomSizeRead(fs13, BlockDataLength + 20);

            fs1.Dispose();
            fs2.Dispose();
            fs3.Dispose();

            fs11.Dispose();
            fs12.Dispose();
            fs13.Dispose();

            trans.CommitAndDispose();
        }

        static void TestRollback(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(BlockSize, stream, fat);

            //create 3 files additional files
            SubFileStream fs21 = trans.CreateFile(id, 1234);
            SubFileStream fs22 = trans.CreateFile(id, 1234);
            SubFileStream fs23 = trans.CreateFile(id, 1234);

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

        static void TestVerifyRollback(DiskIo stream, FileHeaderBlock fat)
        {
            Guid id = Guid.NewGuid();
            TransactionalEdit trans = new TransactionalEdit(BlockSize, stream, fat);

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
