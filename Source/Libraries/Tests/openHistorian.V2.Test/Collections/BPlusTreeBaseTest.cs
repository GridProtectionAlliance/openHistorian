using System.Diagnostics;
using openHistorian.V2.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using openHistorian.V2.Collections.Specialized;
using openHistorian.V2.IO;
using openHistorian.V2.IO.Unmanaged;
using System.Linq;

namespace openHistorian.V2.Test
{


    /// <summary>
    ///This is a test class for BPlusTreeBaseTest and is intended
    ///to contain all BPlusTreeBaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BPlusTreeBaseTest
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

        [TestMethod()]
        public void AddSequential()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryStream stream = new BinaryStream(memoryStream))
                {
                    BPlusTreeLongLong tree = new BPlusTreeLongLong(stream, 4096);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    for (long x = 1; x < 100000; x++)
                    {
                        tree.Add(x, x + 1);
                        if (tree.Get(x) != x + 1) throw new Exception();
                    }
                    sw.Stop();

                }
            }
        }

        [TestMethod()]
        public void AddReverseSequential()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryStream stream = new BinaryStream(memoryStream))
                {
                    BPlusTreeLongLong tree = new BPlusTreeLongLong(stream, 4096);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    for (long x = 10000; x > 0; x--)
                    {
                        tree.Add(x, x + 1);
                        if (tree.Get(x) != x + 1) throw new Exception();
                    }
                    sw.Stop();

                }
            }
        }

        [TestMethod()]
        public void AddRandom()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryStream stream = new BinaryStream(memoryStream))
                {
                    Random r = new Random();
                    int seed = r.Next();
                    seed = 1688042167;
                    r = new Random(seed);

                    BPlusTreeLongLong tree = new BPlusTreeLongLong(stream, 4096);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    for (long x = 1; x < 100000; x++)
                    {
                        if (x == 85299)
                            x = x;

                        long key = (long)r.Next() << 31 | r.Next();
                        tree.Add(key, x + 1);
                        if (tree.Get(key) != x + 1) throw new Exception();
                    }
                    sw.Stop();

                }
            }
        }

        [TestMethod()]
        public void AddSequentialGetRange()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryStream stream = new BinaryStream(memoryStream))
                {
                    BPlusTreeLongLong tree = new BPlusTreeLongLong(stream, 4096);


                    for (long x = 0; x < 100000; x++)
                    {
                        tree.Add(x, x + 1);
                        //if (tree.Get(x) != x + 1) throw new Exception();
                    }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    Assert.AreEqual(100000, tree.GetRange().Count());
                    Assert.AreEqual(1000, tree.GetRange(100000 - 1000).Count());
                    Assert.AreEqual(19000, tree.GetRange(1000, 20000).Count());

                    Assert.AreEqual(50000, tree.GetRange(x => (x % 2) == 1).Count());
                    Assert.AreEqual(500, tree.GetRange(100000 - 1000, x => (x % 2) == 1).Count());
                    Assert.AreEqual(9500, tree.GetRange(1000, 20000, x => (x % 2) == 1).Count());

                    sw.Stop();
                    System.Windows.Forms.MessageBox.Show(sw.Elapsed.TotalMilliseconds.ToString());
                }
            }
        }

        public void BenchmarkGetRange()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryStream stream = new BinaryStream(memoryStream))
                {
                    BPlusTreeLongLong tree = new BPlusTreeLongLong(stream, 4096);


                    for (long x = 0; x < 10000000; x++)
                    {
                        tree.Add(x, x + 1);
                        //if (tree.Get(x) != x + 1) throw new Exception();
                    }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    Assert.AreEqual(10000000, tree.GetRange(0, 100000000, x => (x % 1) == 0).Count());

                    sw.Stop();
                    System.Windows.Forms.MessageBox.Show(sw.Elapsed.TotalMilliseconds.ToString());
                }
            }
        }

        //[TestMethod()]
        //public void GetTest()
        //{
        //    GetTestHelper<GenericParameterHelper, GenericParameterHelper>();
        //}



    }
}
