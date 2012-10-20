//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using openHistorian.V2.Server.Database.Archive;

//namespace openHistorian.V2.Server.Database
//{
    
    
//    /// <summary>
//    ///This is a test class for ArchiveInitializerTest and is intended
//    ///to contain all ArchiveInitializerTest Unit Tests
//    ///</summary>
//    [TestClass()]
//    public class ArchiveInitializerTest
//    {


//        private TestContext testContextInstance;

//        /// <summary>
//        ///Gets or sets the test context which provides
//        ///information about and functionality for the current test run.
//        ///</summary>
//        public TestContext TestContext
//        {
//            get
//            {
//                return testContextInstance;
//            }
//            set
//            {
//                testContextInstance = value;
//            }
//        }

//        #region Additional test attributes
//        // 
//        //You can use the following additional attributes as you write your tests:
//        //
//        //Use ClassInitialize to run code before running the first test in the class
//        //[ClassInitialize()]
//        //public static void MyClassInitialize(TestContext testContext)
//        //{
//        //}
//        //
//        //Use ClassCleanup to run code after all tests in a class have run
//        //[ClassCleanup()]
//        //public static void MyClassCleanup()
//        //{
//        //}
//        //
//        //Use TestInitialize to run code before running each test
//        //[TestInitialize()]
//        //public void MyTestInitialize()
//        //{
//        //}
//        //
//        //Use TestCleanup to run code after each test has run
//        //[TestCleanup()]
//        //public void MyTestCleanup()
//        //{
//        //}
//        //
//        #endregion

//        ArchiveInitializerSettings GetInMemorySettings()
//        {
//            ArchiveInitializerSettings settings = new ArchiveInitializerSettings();
//            settings.IsMemoryArchive = true;
//            settings.IsReadOnly = true;
//            return settings;
//        }

//        ArchiveInitializerSettings GetFileSettings()
//        {
//            ArchiveInitializerSettings settings = new ArchiveInitializerSettings();
//            settings.IsMemoryArchive = false;
//            settings.InitialSize = 1024 * 1024;
//            settings.AutoGrowthSize = 1024 * 1024;
//            settings.RequiredFreeSpaceForNewFile = 1024 * 1024 * 1024;
//            settings.RequiredFreeSpaceForAutoGrowth = 10 * 1024 * 1024;
//            settings.SavePaths.Folders.Add("C:\\temp");
//            settings.SavePaths.Folders.Add("R:\\temp");
//            settings.IsReadOnly = true;
//            return settings;
//        }

//        /// <summary>
//        ///A test for ArchiveInitializer Constructor
//        ///</summary>
//        [TestMethod()]
//        public void ArchiveInitializerConstructorTest()
//        {
//            ArchiveInitializer target = new ArchiveInitializer(GetInMemorySettings());
//            using (var archive = target.CreateArchiveFile())
//            {
//                archive.BeginEdit();
//                archive.AddPoint(1,1,1,1);
//                archive.CommitEdit();
//            }
//        }

//        /// <summary>
//        ///A test for CreateArchiveFile
//        ///</summary>
//        [TestMethod()]
//        public void CreateArchiveFileTest()
//        {
//            ArchiveInitializer target = new ArchiveInitializer(GetFileSettings());
//            using (var archive = target.CreateArchiveFile())
//            {
//                archive.BeginEdit();
//                archive.AddPoint(1, 1, 1, 1);
//                archive.CommitEdit();
//            }
//        }

//        ///// <summary>
//        /////A test for CreateArchiveName
//        /////</summary>
//        //[TestMethod()]
//        //[DeploymentItem("openHistorian.V2.dll")]
//        //public void CreateArchiveNameTest()
//        //{
//        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
//        //    ArchiveInitializer_Accessor target = new ArchiveInitializer_Accessor(param0); // TODO: Initialize to an appropriate value
//        //    string expected = string.Empty; // TODO: Initialize to an appropriate value
//        //    string actual;
//        //    actual = target.CreateArchiveName();
//        //    Assert.AreEqual(expected, actual);
//        //    Assert.Inconclusive("Verify the correctness of this test method.");
//        //}
//    }
//}
