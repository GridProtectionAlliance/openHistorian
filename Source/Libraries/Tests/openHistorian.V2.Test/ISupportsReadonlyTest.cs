using openHistorian.V2.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace openHistorian.V2.Test
{


    /// <summary>
    ///This is a test class for ISupportsReadonlyTest and is intended
    ///to contain all ISupportsReadonlyTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ISupportsReadonlyTest
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


        public static void Test<T>(ISupportsReadonly<T> obj)
        {
            bool origional = obj.IsReadOnly;

            ISupportsReadonly<T> ro = (ISupportsReadonly<T>)obj.ReadonlyClone();
            Assert.AreEqual(true, ro.IsReadOnly);
            Assert.AreEqual(origional,obj.IsReadOnly);

            ISupportsReadonly<T> rw = (ISupportsReadonly<T>)obj.EditableClone();
            Assert.AreEqual(false, rw.IsReadOnly);
            Assert.AreEqual(origional, obj.IsReadOnly);
            rw.IsReadOnly = true;
            Assert.AreEqual(origional, obj.IsReadOnly);

            Assert.AreEqual(true, rw.IsReadOnly);
            HelperFunctions.ExpectError(() => rw.IsReadOnly = false);
            Assert.AreEqual(origional, obj.IsReadOnly);

            HelperFunctions.ExpectError(() => ro.IsReadOnly = false);

        }
    }
}
