using openHistorian.V2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace openHistorian.V2.Test
{
    
    
    /// <summary>
    ///This is a test class for WinApiTest and is intended
    ///to contain all WinApiTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WinApiTest
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
        ///A test for GetAvailableFreeSpace
        ///</summary>
        [TestMethod()]
        public void GetAvailableFreeSpaceTest()
        {
            long freeSpace = 0; 
            long totalSize = 0; 
            bool actual;

            actual = WinApi.GetAvailableFreeSpace("C:\\", out freeSpace, out totalSize);
            Assert.AreEqual(true, freeSpace>0);
            Assert.AreEqual(true, totalSize>0);
            Assert.AreEqual(true, actual);

            actual = WinApi.GetAvailableFreeSpace("C:\\windows", out freeSpace, out totalSize);
            Assert.AreEqual(true, freeSpace > 0);
            Assert.AreEqual(true, totalSize > 0);
            Assert.AreEqual(true, actual);

            actual = WinApi.GetAvailableFreeSpace("\\\\htpc\\h", out freeSpace, out totalSize);
            Assert.AreEqual(true, freeSpace > 0);
            Assert.AreEqual(true, totalSize > 0);
            Assert.AreEqual(true, actual);

            actual = WinApi.GetAvailableFreeSpace("G:\\Steam\\steamapps\\common", out freeSpace, out totalSize);
            Assert.AreEqual(true, freeSpace > 0);
            Assert.AreEqual(true, totalSize > 0);
            Assert.AreEqual(true, actual);

            actual = WinApi.GetAvailableFreeSpace("G:\\Steam\\steamapps\\common\\portal 2", out freeSpace, out totalSize); //ntfs symbolic link
            Assert.AreEqual(true, freeSpace > 0);
            Assert.AreEqual(true, totalSize > 0);
            Assert.AreEqual(true, actual);

            actual = WinApi.GetAvailableFreeSpace("P:\\", out freeSpace, out totalSize);
            Assert.AreEqual(true, freeSpace > 0);
            Assert.AreEqual(true, totalSize > 0);
            Assert.AreEqual(true, actual);

            actual = WinApi.GetAvailableFreeSpace("P:\\R Drive", out freeSpace, out totalSize); //mount point
            Assert.AreEqual(true, freeSpace > 0);
            Assert.AreEqual(true, totalSize > 0);
            Assert.AreEqual(true, actual);

            actual = WinApi.GetAvailableFreeSpace("L:\\", out freeSpace, out totalSize); //Bad Location
            Assert.AreEqual(false, freeSpace > 0);
            Assert.AreEqual(false, totalSize > 0);
            Assert.AreEqual(false, actual);
        }
    }
}
