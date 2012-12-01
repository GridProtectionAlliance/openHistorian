using openHistorian.V2.Collections.KeyValue;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using openHistorian.V2.IO;
using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.Collections.KeyValue
{


    /// <summary>
    ///This is a test class for BasicTreeTest and is intended
    ///to contain all BasicTreeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SortedTree256TSEncodedTest
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
        ///A test for BasicTree Constructor
        ///</summary>
        [TestMethod()]
        public void Test()
        {
            //TestPageSize(1000);
            Random r = new Random();

            for (int x = 256; x < 65536; x = (int)(x + x * (r.Next(500) / 100f / 100)))
                TestSingleLevel(x, 1);//r.Next());

            for (int x = 256; x < 65536; x = (int)(x + x * (r.Next(10000) / 100f / 100)))
                TestDoubleLevel(x, r.Next());

            TestMulitLevel(256, r.Next());

        }


        /// <summary>
        ///A test for BasicTree Constructor
        ///</summary>
        [TestMethod()]
        public void TestSequential()
        {

            using (var stream = new MemoryStream())
            using (var binaryStream = stream.CreateBinaryStream())
            {
                SortedTree256TSEncoded tree = new SortedTree256TSEncoded(binaryStream, 4096);
                for (uint x = 0; x < 10000; x++)
                {
                    ulong k1 = x;
                    ulong k2 = x * x;
                    ulong v1 = x * x * x;
                    ulong v2 = x * x * x * x;
                    ulong vv1, vv2;
                    tree.Add(k1, k2, v1, v2);
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();
                }

                for (uint x = 0; x < 10000; x++)
                {
                    ulong k1 = x;
                    ulong k2 = x * x;
                    ulong v1 = x * x * x;
                    ulong v2 = x * x * x * x;
                    ulong vv1, vv2;
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();
                }

            }
        }


        /// <summary>
        ///A test for BasicTree Constructor
        ///</summary>
        [TestMethod()]
        public void TestReverseSequential()
        {

            using (var stream = new MemoryStream())
            using (var binaryStream = stream.CreateBinaryStream())
            {
                SortedTree256TSEncoded tree = new SortedTree256TSEncoded(binaryStream, 4096);
                for (uint x = 5000; x >= 1; x--)
                {
                    ulong k1 = x;
                    ulong k2 = x * x;
                    ulong v1 = x * x * x;
                    ulong v2 = x * x * x * x;
                    ulong vv1, vv2;
                    tree.Add(k1, k2, v1, v2);
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();
                }
                for (uint x = 5000; x >= 1; x--)
                {
                    ulong k1 = x;
                    ulong k2 = x * x;
                    ulong v1 = x * x * x;
                    ulong v2 = x * x * x * x;
                    ulong vv1, vv2;
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();
                }
            }
        }

        /// <summary>
        ///A test for BasicTree Constructor
        ///</summary>
        [TestMethod()]
        public void TestUpDown()
        {

            using (var stream = new MemoryStream())
            using (var binaryStream = stream.CreateBinaryStream())
            {
                SortedTree256TSEncoded tree = new SortedTree256TSEncoded(binaryStream, 4096);
                for (uint x = 1; x < 5000; x++)
                {
                    ulong k1 = 10000 + x;
                    ulong k2 = x * x;
                    ulong v1 = x * x * x;
                    ulong v2 = x * x * x * x;
                    ulong vv1, vv2;
                    tree.Add(k1, k2, v1, v2);
                    if (x == 3)
                        x = x;
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();

                    k1 = 10000 - x;
                    tree.Add(k1, k2, v1, v2);
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();

                }

                for (uint x = 1; x < 5000; x++)
                {
                    ulong k1 = 10000 + x;
                    ulong k2 = x * x;
                    ulong v1 = x * x * x;
                    ulong v2 = x * x * x * x;
                    ulong vv1, vv2;
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();

                    k1 = 10000 - x;
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();

                }
            }
        }

        /// <summary>
        ///A test for BasicTree Constructor
        ///</summary>
        [TestMethod()]
        public void TestUpDownInner()
        {

            using (var stream = new MemoryStream())
            using (var binaryStream = stream.CreateBinaryStream())
            {
                SortedTree256TSEncoded tree = new SortedTree256TSEncoded(binaryStream, 4096);
                for (uint x = 5000; x >= 1; x--)
                {
                    ulong k1 = 10000 + x;
                    ulong k2 = x * x;
                    ulong v1 = x * x * x;
                    ulong v2 = x * x * x * x;
                    ulong vv1, vv2;
                    tree.Add(k1, k2, v1, v2);
                    if (x == 3)
                        x = x;
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();

                    k1 = 10000 - x;
                    tree.Add(k1, k2, v1, v2);
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();

                }

                for (uint x = 5000; x >= 1; x--)
                {
                    ulong k1 = 10000 + x;
                    ulong k2 = x * x;
                    ulong v1 = x * x * x;
                    ulong v2 = x * x * x * x;
                    ulong vv1, vv2;
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();

                    k1 = 10000 - x;
                    tree.Get(k1, k2, out vv1, out vv2);
                    if (v1 != vv1 | v2 != vv2)
                        throw new Exception();

                }
            }
        }

        public void TestSingleLevel(int blockSize, int seed)
        {
            int maxItems = blockSize / 32 - 1;
            //try
            {
                using (var stream = new MemoryStream())
                using (var binaryStream = stream.CreateBinaryStream())
                {
                    SortedTree256TSEncoded tree = new SortedTree256TSEncoded(binaryStream, blockSize);

                    Random r = new Random(seed);
                    for (int x = 0; x < maxItems; x++)
                    {
                        ulong k1 = (ulong)r.Next();
                        ulong k2 = (ulong)r.Next();
                        ulong v1 = (ulong)r.Next();
                        ulong v2 = (ulong)r.Next();
                        ulong vv1, vv2;
                        if (x == 4)
                            x = x;
                        tree.Add(k1, k2, v1, v2);
                        tree.Get(k1, k2, out vv1, out vv2);
                        if (v1 != vv1 | v2 != vv2)
                            throw new Exception();
                    }

                    r = new Random(seed);
                    for (int x = 0; x < maxItems; x++)
                    {
                        ulong k1 = (ulong)r.Next();
                        ulong k2 = (ulong)r.Next();
                        ulong v1 = (ulong)r.Next();
                        ulong v2 = (ulong)r.Next();
                        ulong vv1, vv2;
                        if (x == 1)
                            x = x;
                        tree.Get(k1, k2, out vv1, out vv2);
                        if (v1 != vv1 | v2 != vv2)
                            throw new Exception();
                    }

                }
            }
            //catch (Exception ex)
            {
                //throw new Exception("Error At " + seed, ex);
            }
        }

        public void TestDoubleLevel(int blockSize, int seed)
        {
            int maxItems = Math.Min(blockSize, (blockSize / 32 - 1) * (blockSize / 32 - 1));
            //try
            {
                using (var stream = new MemoryStream())
                using (var binaryStream = stream.CreateBinaryStream())
                {
                    SortedTree256TSEncoded tree = new SortedTree256TSEncoded(binaryStream, blockSize);

                    Random r = new Random(seed);
                    for (int x = 0; x < maxItems; x++)
                    {
                        if (x == 7)
                            x = x;
                        ulong k1 = (ulong)r.Next();
                        ulong k2 = (ulong)r.Next();
                        ulong v1 = (ulong)r.Next();
                        ulong v2 = (ulong)r.Next();
                        ulong vv1, vv2;
                        tree.Add(k1, k2, v1, v2);
                        tree.Get(k1, k2, out vv1, out vv2);
                        if (v1 != vv1 | v2 != vv2)
                            throw new Exception();
                    }

                    r = new Random(seed);
                    for (int x = 0; x < maxItems; x++)
                    {
                        ulong k1 = (ulong)r.Next();
                        ulong k2 = (ulong)r.Next();
                        ulong v1 = (ulong)r.Next();
                        ulong v2 = (ulong)r.Next();
                        ulong vv1, vv2;
                        tree.Get(k1, k2, out vv1, out vv2);
                        if (v1 != vv1 | v2 != vv2)
                            throw new Exception();
                    }

                }
            }
            //catch (Exception ex)
            {
                //throw new Exception("Error At " + seed, ex);
            }
        }

        public void TestMulitLevel(int blockSize, int seed)
        {
            int maxItems = (int)Math.Pow(blockSize / 32 - 1, 6);
            //try
            {
                using (var stream = new MemoryStream())
                using (var binaryStream = stream.CreateBinaryStream())
                {
                    SortedTree256TSEncoded tree = new SortedTree256TSEncoded(binaryStream, blockSize);

                    Random r = new Random(seed);
                    for (int x = 0; x < maxItems; x++)
                    {
                        if (x == 7)
                            x = x;
                        ulong k1 = (ulong)r.Next();
                        ulong k2 = (ulong)r.Next();
                        ulong v1 = (ulong)r.Next();
                        ulong v2 = (ulong)r.Next();
                        ulong vv1, vv2;
                        tree.Add(k1, k2, v1, v2);
                        tree.Get(k1, k2, out vv1, out vv2);
                        if (v1 != vv1 | v2 != vv2)
                            throw new Exception();
                    }

                    r = new Random(seed);
                    for (int x = 0; x < maxItems; x++)
                    {
                        ulong k1 = (ulong)r.Next();
                        ulong k2 = (ulong)r.Next();
                        ulong v1 = (ulong)r.Next();
                        ulong v2 = (ulong)r.Next();
                        ulong vv1, vv2;
                        tree.Get(k1, k2, out vv1, out vv2);
                        if (v1 != vv1 | v2 != vv2)
                            throw new Exception();
                    }

                }
            }
            //catch (Exception ex)
            {
                //throw new Exception("Error At " + seed, ex);
            }
        }

    }
}
