using NUnit.Framework;
using openHistorian.Archive;
using System;
using openHistorian;
using openHistorian.Collections;

namespace openHistorian.Test
{
    /// <summary>
    ///This is a test class for PartitionFileTest and is intended
    ///to contain all PartitionFileTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class ArchiveFileTest
    {
        /// <summary>
        ///A test for PartitionFile Constructor
        ///</summary>
        [Test()]
        public void PartitionFileConstructorTest()
        {
            using (var target = HistorianArchiveFile.CreateInMemory())
            {
            }
        }

        /// <summary>
        ///A test for AddPoint
        ///</summary>
        [Test()]
        public void AddPointTest()
        {
            using (var target = HistorianArchiveFile.CreateInMemory())
            {
                ulong date = 0;
                ulong pointId = 0;
                ulong value1 = 0;
                ulong value2 = 0;
                using (var fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(date, pointId, value1, value2);
                    fileEditor.Commit();
                }
            }
        }

        /// <summary>
        ///A test for AddPoint
        ///</summary>
        [Test()]
        public void EnduranceTest()
        {
            using (var target = HistorianArchiveFile.CreateInMemory())
            {
                for (uint x = 0; x < 100; x++)
                {
                    using (var fileEditor = target.BeginEdit())
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            fileEditor.AddPoint(x, x, x, x);
                            x++;
                        }
                        fileEditor.Commit();
                    }
                    Assert.AreEqual(target.FirstKey.Timestamp, (ulong)0);
                    Assert.AreEqual(target.LastKey.Timestamp, (ulong)(x-1));
                }
                
            }
        }

        /// <summary>
        ///A test for CreateSnapshot
        ///</summary>
        [Test()]
        public void CreateSnapshotTest()
        {
            using (var target = HistorianArchiveFile.CreateInMemory())
            {
                ulong date = 1;
                ulong pointId = 2;
                ulong value1 = 3;
                ulong value2 = 4;
                ArchiveFileSnapshotInfo<HistorianKey,HistorianValue> snap1;
                using (var fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(date, pointId, value1, value2);
                    fileEditor.AddPoint(date + 1, pointId, value1, value2);
                    snap1 = target.AcquireReadSnapshot();
                    fileEditor.Commit();
                }
                var snap2 = target.AcquireReadSnapshot();

                using (var instance = snap1.CreateReadSnapshot())
                {
                    var scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(false, scanner.Read(out date, out pointId, out value1, out value2));
                }
                using (var instance = snap2.CreateReadSnapshot())
                {
                    var scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(true, scanner.Read(out date, out pointId, out value1, out value2));
                    Assert.AreEqual(1uL, date);
                    Assert.AreEqual(2uL, pointId);
                    Assert.AreEqual(3uL, value1);
                    Assert.AreEqual(4uL, value2);
                }
                Assert.AreEqual(1uL, target.FirstKey.Timestamp);
                Assert.AreEqual(2uL, target.LastKey.Timestamp);

            }
        }

        /// <summary>
        ///A test for RollbackEdit
        ///</summary>
        [Test()]
        public void RollbackEditTest()
        {
            using (var target = HistorianArchiveFile.CreateInMemory())
            {
                ulong date = 1;
                ulong pointId = 2;
                ulong value1 = 3;
                ulong value2 = 4;
                ArchiveFileSnapshotInfo<HistorianKey, HistorianValue> snap1;
                using (var fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(date, pointId, value1, value2);
                    snap1 = target.AcquireReadSnapshot();
                    fileEditor.Rollback();
                }
                var snap2 = target.AcquireReadSnapshot();

                using (var instance = snap1.CreateReadSnapshot())
                {
                    var scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(false, scanner.Read(out date, out pointId, out value1, out value2));
                }
                using (var instance = snap2.CreateReadSnapshot())
                {
                    var scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(false, scanner.Read(out date, out pointId, out value1, out value2));
                }
            }
        }
    }
}
