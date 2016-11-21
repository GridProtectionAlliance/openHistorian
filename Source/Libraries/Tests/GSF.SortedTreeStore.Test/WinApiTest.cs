using GSF.IO;
using NUnit.Framework;

namespace GSF.Test
{
    /// <summary>
    ///This is a test class for WinApiTest and is intended
    ///to contain all WinApiTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class WinApiTest
    {
        /// <summary>
        ///A test for GetAvailableFreeSpace
        ///</summary>
        [Test()]
        public void GetAvailableFreeSpaceTest()
        {
            long freeSpace = 0;
            long totalSize = 0;
            bool actual;

            actual = FilePath.GetAvailableFreeSpace("C:\\", out freeSpace, out totalSize);
            Assert.AreEqual(true, freeSpace > 0);
            Assert.AreEqual(true, totalSize > 0);
            Assert.AreEqual(true, actual);

            actual = FilePath.GetAvailableFreeSpace("C:\\windows", out freeSpace, out totalSize);
            Assert.AreEqual(true, freeSpace > 0);
            Assert.AreEqual(true, totalSize > 0);
            Assert.AreEqual(true, actual);

            actual = FilePath.GetAvailableFreeSpace("\\\\htpc\\h", out freeSpace, out totalSize);
            Assert.AreEqual(true, freeSpace > 0);
            Assert.AreEqual(true, totalSize > 0);
            Assert.AreEqual(true, actual);

            //actual = WinApi.GetAvailableFreeSpace("G:\\Steam\\steamapps\\common", out freeSpace, out totalSize);
            //Assert.AreEqual(true, freeSpace > 0);
            //Assert.AreEqual(true, totalSize > 0);
            //Assert.AreEqual(true, actual);

            //actual = WinApi.GetAvailableFreeSpace("G:\\Steam\\steamapps\\common\\portal 2", out freeSpace, out totalSize); //ntfs symbolic link
            //Assert.AreEqual(true, freeSpace > 0);
            //Assert.AreEqual(true, totalSize > 0);
            //Assert.AreEqual(true, actual);

            //actual = WinApi.GetAvailableFreeSpace("P:\\", out freeSpace, out totalSize);
            //Assert.AreEqual(true, freeSpace > 0);
            //Assert.AreEqual(true, totalSize > 0);
            //Assert.AreEqual(true, actual);

            //actual = WinApi.GetAvailableFreeSpace("P:\\R Drive", out freeSpace, out totalSize); //mount point
            //Assert.AreEqual(true, freeSpace > 0);
            //Assert.AreEqual(true, totalSize > 0);
            //Assert.AreEqual(true, actual);

            actual = FilePath.GetAvailableFreeSpace("L:\\", out freeSpace, out totalSize); //Bad Location
            Assert.AreEqual(false, freeSpace > 0);
            Assert.AreEqual(false, totalSize > 0);
            Assert.AreEqual(false, actual);
        }
    }
}