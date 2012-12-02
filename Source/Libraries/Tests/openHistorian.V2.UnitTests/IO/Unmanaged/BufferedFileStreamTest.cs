using NUnit.Framework;
using openHistorian.V2.IO.Unmanaged;
using System;
using System.IO;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.IO.Unmanaged.Test
{

    /// <summary>
    ///This is a test class for BufferedFileStreamTest and is intended
    ///to contain all BufferedFileStreamTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class BufferedFileStreamTest
    {
        
        /// <summary>
        ///A test for BufferedFileStream Constructor
        ///</summary>
        [Test()]
        public void BufferedFileStreamConstructorTest()
        {
            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    using (BufferedFileStream bfs = new BufferedFileStream(fs))
                    {
                        BinaryStream bs = new BinaryStream(bfs);
                        bs.Write(1L);
                        bs.ClearLocks();
                    }
                }
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        /// <summary>
        ///A test for Flush
        ///</summary>
        [Test()]
        public void FlushTest()
        {
            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    using (BufferedFileStream bfs = new BufferedFileStream(fs))
                    {
                        Globals.BufferPool.SetMaximumBufferSize(1 * 1000 * 1000);

                        BinaryStream bs = new BinaryStream(bfs);
                        bs.Write(1L);
                        bs.ClearLocks();
                        using (BufferedFileStream bfs2 = new BufferedFileStream(fs))
                        {
                            BinaryStream bs2 = new BinaryStream(bfs2);
                            Assert.AreEqual(0L, bs2.ReadInt64());
                        }
                        bfs.Flush();
                        using (BufferedFileStream bfs2 = new BufferedFileStream(fs))
                        {
                            BinaryStream bs2 = new BinaryStream(bfs2);
                            Assert.AreEqual(1L, bs2.ReadInt64());
                        }
                    }

                }
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        /// <summary>
        ///Tests to verify that the exact same memory buffer is used with two different binary streams.
        ///</summary>
        [Test()]
        public void TestConcurrent()
        {
            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    using (BufferedFileStream bfs = new BufferedFileStream(fs))
                    {
                        BinaryStream bs = new BinaryStream(bfs);
                        BinaryStream bs2 = new BinaryStream(bfs);
                        bs.Write(0L);
                        bs2.Write(0L);
                        bs.Write(1L);
                        Assert.AreEqual(1L, bs2.ReadInt64());
                        bs2.Write(2L);
                        Assert.AreEqual(2L, bs.ReadInt64());
                    }
                }
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        /// <summary>
        ///Tests to verify that the exact same memory buffer is used with two different binary streams.
        ///</summary>
        [Test()]
        public void TestLargeFile()
        {

            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    using (BufferPool pool = new BufferPool(65536))
                    {
                        pool.SetMaximumBufferSize(10 * 1024 * 1024);
                        using (BufferedFileStream bfs = new BufferedFileStream(fs, pool, 4096))
                        {
                            BinaryStream bs = new BinaryStream(bfs);
                            for (long x = 0; x < 1000 * 1000 * 10; x++) //80 MB written
                            {
                                bs.Write(x);
                            }
                            bs.Position = 0;
                            for (long x = 0; x < 1000 * 1000 * 10; x++) //80 MB written
                            {
                                Assert.AreEqual(x, bs.ReadInt64());
                            }
                            bs.ClearLocks();
                            bfs.Flush();
                        }
                    }
                }
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    using (BufferPool pool = new BufferPool(65536))
                    {
                        pool.SetMaximumBufferSize(10 * 1024 * 1024);
                        using (BufferedFileStream bfs = new BufferedFileStream(fs, pool, 4096))
                        {
                            BinaryStream bs = new BinaryStream(bfs);
                            for (long x = 0; x < 1000 * 1000 * 10; x++) //80 MB written
                            {
                                Assert.AreEqual(x, bs.ReadInt64());
                            }
                        }
                    }
                    //Clipboard.SetText(fs.Length.ToString());
                }

            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
