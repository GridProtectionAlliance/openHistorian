using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using openHistorian.V2.Collections;

namespace openHistorian.V2.Server.Configuration
{
    
    
    /// <summary>
    ///This is a test class for FolderListSettingsTest and is intended
    ///to contain all FolderListSettingsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FolderListSettingsTest
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
        ///A test for FolderListSettings Constructor
        ///</summary>
        [TestMethod()]
        public void FolderListSettingsConstructorTest()
        {
            FolderListSettings target = new FolderListSettings();
            target.Folders.Add("1");
            target.Folders.Add("2");
            ISupportsReadonlyTest.Test(target);
            target.Folders.Add("3");

            target.IsReadOnly = true;
            ISupportsReadonlyTest.Test(target);
            Assert.AreEqual(true,target.Folders.IsReadOnly);
            var editable = target.CloneEditable();
            editable.Folders.Add("4");
            Assert.AreEqual(3,target.Folders.Count);
            editable.IsReadOnly = true;
            editable = editable.CloneEditable();
            editable.Folders.Add("5");
            Assert.AreEqual(5,editable.Folders.Count);
        }
    }
}
