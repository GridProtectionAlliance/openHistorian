using openHistorian.V2.IO.Unmanaged;
using openHistorian.V2.Unmanaged;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.Test
{

    /// <summary>
    ///This is a test class for BufferedFileStreamTest and is intended
    ///to contain all BufferedFileStreamTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BufferedFileStreamTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for BufferedFileStream Constructor
        ///</summary>
        [TestMethod()]
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
        [TestMethod()]
        public void FlushTest()
        {
            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    using (BufferedFileStream bfs = new BufferedFileStream(fs))
                    {
                        Globals.BufferPool.MaximumBufferSize = (1 * 1000 * 1000);

                        BinaryStream bs = new BinaryStream(bfs);
                        bs.Write(1L);
                        bs.ClearLocks();
                        using (BufferedFileStream bfs2 = new BufferedFileStream(fs))
                        {
                            BinaryStream bs2 = new BinaryStream(bfs2);
                            Assert.AreEqual(0L, bs2.ReadInt64());
                        }
                        bfs.Flush(true, false);
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
        [TestMethod()]
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
    }
}
