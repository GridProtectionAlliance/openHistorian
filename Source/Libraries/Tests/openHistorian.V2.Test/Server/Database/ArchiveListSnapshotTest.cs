using openHistorian.V2.Server.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace openHistorian.V2.Server.Database
{


    /// <summary>
    ///This is a test class for ArchiveListSnapshotTest and is intended
    ///to contain all ArchiveListSnapshotTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArchiveListSnapshotTest
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
        ///A test for ArchiveListSnapshot Constructor
        ///</summary>
        [TestMethod()]
        public void ArchiveListSnapshotConstructorTest()
        {
            Action<ArchiveListSnapshot> onDisposed = null;
            Action<ArchiveListSnapshot> acquireResources = null;
            ArchiveListSnapshot target = new ArchiveListSnapshot(onDisposed, acquireResources);
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            bool disposed = false;
            bool isSelf = false;
            ArchiveListSnapshot target = null;

            Action<ArchiveListSnapshot> onDisposed = ((x) =>
            {
                disposed = true;
                isSelf = (target == x);
            });
            Action<ArchiveListSnapshot> acquireResources = null;
            target = new ArchiveListSnapshot(onDisposed, acquireResources);
            target.Dispose();
            Assert.AreEqual(true, disposed);
            Assert.AreEqual(true, isSelf);
        }

        /// <summary>
        ///A test for UpdateSnapshot
        ///</summary>
        [TestMethod()]
        public void UpdateSnapshotTest()
        {

            bool updated = false;
            bool isSelf = false;
            ArchiveListSnapshot target = null;

            Action<ArchiveListSnapshot> acquireResources = ((x) =>
            {
                updated = true;
                isSelf = (target == x);
            });

            Action<ArchiveListSnapshot> onDisposed = null;

            target = new ArchiveListSnapshot(onDisposed, acquireResources);
            target.UpdateSnapshot();

            Assert.AreEqual(true, updated);
            Assert.AreEqual(true, isSelf);
        }

        /// <summary>
        ///A test for IsDisposed
        ///</summary>
        [TestMethod()]
        public void IsDisposedTest()
        {
            object obj;
            Action<ArchiveListSnapshot> onDisposed = null;
            Action<ArchiveListSnapshot> acquireResources = null;
            ArchiveListSnapshot target = new ArchiveListSnapshot(onDisposed, acquireResources);

            Assert.AreEqual(false, target.IsDisposed);
            target.Dispose();
            Assert.AreEqual(true, target.IsDisposed);

            HelperFunctions.ExpectError(() => target.Tables = null);
            HelperFunctions.ExpectError(() => obj = target.Tables);
            HelperFunctions.ExpectError(() => target.UpdateSnapshot());
        }

    }
}
