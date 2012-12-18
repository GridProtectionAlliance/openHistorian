using NUnit.Framework;
using openHistorian.Engine;
using openHistorian.Server.Database;
using System;

namespace openHistorian.Server.Database
{


    /// <summary>
    ///This is a test class for ArchiveListSnapshotTest and is intended
    ///to contain all ArchiveListSnapshotTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class ArchiveListSnapshotTest
    {
        
        
        /// <summary>
        ///A test for ArchiveListSnapshot Constructor
        ///</summary>
        [Test()]
        public void ArchiveListSnapshotConstructorTest()
        {
            Action<ArchiveListSnapshot> onDisposed = null;
            Action<ArchiveListSnapshot> acquireResources = null;
            ArchiveListSnapshot target = new ArchiveListSnapshot(onDisposed, acquireResources);
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [Test()]
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
        [Test()]
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
        [Test()]
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
