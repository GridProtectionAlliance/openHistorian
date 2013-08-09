using NUnit.Framework;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

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
            using (ArchiveTable<HistorianKey, HistorianValue> target = ArchiveFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
            }
        }

        /// <summary>
        ///A test for AddPoint
        ///</summary>
        [Test()]
        public void AddPointTest()
        {

            using (ArchiveTable<HistorianKey, HistorianValue> target = ArchiveFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                using (ArchiveTable<HistorianKey, HistorianValue>.Editor fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(new HistorianKey(), new HistorianValue());
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
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            using (ArchiveTable<HistorianKey, HistorianValue> target = ArchiveFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                for (uint x = 0; x < 100; x++)
                {
                    using (ArchiveTable<HistorianKey, HistorianValue>.Editor fileEditor = target.BeginEdit())
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            key.Timestamp = x;
                            key.PointID = x;
                            value.Value1 = x;
                            value.Value3 = x;
                            fileEditor.AddPoint(key,value);
                            x++;
                        }
                        fileEditor.Commit();
                    }
                    Assert.AreEqual(target.FirstKey.Timestamp, (ulong)0);
                    Assert.AreEqual(target.LastKey.Timestamp, (ulong)(x - 1));
                }
            }
        }

        /// <summary>
        ///A test for CreateSnapshot
        ///</summary>
        [Test()]
        public void CreateSnapshotTest()
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            key.Timestamp = 1;
            key.PointID = 2;
            value.Value1 = 3;
            value.Value2 = 4;
            using (ArchiveTable<HistorianKey, HistorianValue> target = ArchiveFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                ulong date = 1;
                ulong pointId = 2;
                ulong value1 = 3;
                ulong value2 = 4;
                ArchiveTableSnapshotInfo<HistorianKey, HistorianValue> snap1;
                using (ArchiveTable<HistorianKey, HistorianValue>.Editor fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(key,value);
                    key.Timestamp++;
                    fileEditor.AddPoint(key,value);
                    snap1 = target.AcquireReadSnapshot();
                    fileEditor.Commit();
                }
                ArchiveTableSnapshotInfo<HistorianKey, HistorianValue> snap2 = target.AcquireReadSnapshot();

                using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> instance = snap1.CreateReadSnapshot())
                {
                    TreeScannerBase<HistorianKey, HistorianValue> scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(false, scanner.Read());
                }
                using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> instance = snap2.CreateReadSnapshot())
                {
                    TreeScannerBase<HistorianKey, HistorianValue> scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(true, scanner.Read());
                    Assert.AreEqual(1uL, scanner.CurrentKey.Timestamp);
                    Assert.AreEqual(2uL, scanner.CurrentKey.PointID);
                    Assert.AreEqual(3uL, scanner.CurrentValue.Value1);
                    Assert.AreEqual(4uL, scanner.CurrentValue.Value2);
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
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            key.Timestamp = 1;
            key.PointID = 2;
            value.Value1 = 3;
            value.Value2 = 4;

            using (ArchiveTable<HistorianKey, HistorianValue> target = ArchiveFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                ulong date = 1;
                ulong pointId = 2;
                ulong value1 = 3;
                ulong value2 = 4;
                ArchiveTableSnapshotInfo<HistorianKey, HistorianValue> snap1;
                using (ArchiveTable<HistorianKey, HistorianValue>.Editor fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(key, value);
                    snap1 = target.AcquireReadSnapshot();
                    fileEditor.Rollback();
                }
                ArchiveTableSnapshotInfo<HistorianKey, HistorianValue> snap2 = target.AcquireReadSnapshot();

                using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> instance = snap1.CreateReadSnapshot())
                {
                    TreeScannerBase<HistorianKey, HistorianValue> scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(false, scanner.Read());
                }
                using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> instance = snap2.CreateReadSnapshot())
                {
                    TreeScannerBase<HistorianKey, HistorianValue> scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(false, scanner.Read());
                }
            }
        }
    }
}