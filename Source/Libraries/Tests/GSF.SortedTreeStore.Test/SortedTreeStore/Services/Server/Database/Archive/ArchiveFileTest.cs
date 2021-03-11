using GSF.Snap;
using NUnit.Framework;
using GSF.Snap.Storage;
using GSF.Snap.Tree;
using openHistorian.Snap;

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
            using (SortedTreeTable<HistorianKey, HistorianValue> target = SortedTreeFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
            }
        }

        /// <summary>
        ///A test for AddPoint
        ///</summary>
        [Test()]
        public void AddPointTest()
        {

            using (SortedTreeTable<HistorianKey, HistorianValue> target = SortedTreeFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                using (SortedTreeTableEditor<HistorianKey, HistorianValue> fileEditor = target.BeginEdit())
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
            using (SortedTreeTable<HistorianKey, HistorianValue> target = SortedTreeFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                for (uint x = 0; x < 100; x++)
                {
                    using (SortedTreeTableEditor<HistorianKey, HistorianValue> fileEditor = target.BeginEdit())
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            key.Timestamp = x;
                            key.PointID = x;
                            value.Value1 = x;
                            value.Value3 = x;
                            fileEditor.AddPoint(key, value);
                            x++;
                        }
                        fileEditor.Commit();
                    }
                    Assert.AreEqual(target.FirstKey.Timestamp, 0);
                    Assert.AreEqual(target.LastKey.Timestamp, x - 1);
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
            using (SortedTreeTable<HistorianKey, HistorianValue> target = SortedTreeFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                ulong date = 1;
                ulong pointId = 2;
                ulong value1 = 3;
                ulong value2 = 4;
                SortedTreeTableSnapshotInfo<HistorianKey, HistorianValue> snap1;
                using (SortedTreeTableEditor<HistorianKey, HistorianValue> fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(key, value);
                    key.Timestamp++;
                    fileEditor.AddPoint(key, value);
                    snap1 = target.AcquireReadSnapshot();
                    fileEditor.Commit();
                }
                SortedTreeTableSnapshotInfo<HistorianKey, HistorianValue> snap2 = target.AcquireReadSnapshot();

                using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> instance = snap1.CreateReadSnapshot())
                {
                    SortedTreeScannerBase<HistorianKey, HistorianValue> scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(false, scanner.Read(key, value));
                }
                using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> instance = snap2.CreateReadSnapshot())
                {
                    SortedTreeScannerBase<HistorianKey, HistorianValue> scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(true, scanner.Read(key, value));
                    Assert.AreEqual(1uL, key.Timestamp);
                    Assert.AreEqual(2uL, key.PointID);
                    Assert.AreEqual(3uL, value.Value1);
                    Assert.AreEqual(4uL, value.Value2);
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

            using (SortedTreeTable<HistorianKey, HistorianValue> target = SortedTreeFile.CreateInMemory().OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                ulong date = 1;
                ulong pointId = 2;
                ulong value1 = 3;
                ulong value2 = 4;
                SortedTreeTableSnapshotInfo<HistorianKey, HistorianValue> snap1;
                using (SortedTreeTableEditor<HistorianKey, HistorianValue> fileEditor = target.BeginEdit())
                {
                    fileEditor.AddPoint(key, value);
                    snap1 = target.AcquireReadSnapshot();
                    fileEditor.Rollback();
                }
                SortedTreeTableSnapshotInfo<HistorianKey, HistorianValue> snap2 = target.AcquireReadSnapshot();

                using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> instance = snap1.CreateReadSnapshot())
                {
                    SortedTreeScannerBase<HistorianKey, HistorianValue> scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(false, scanner.Read(key, value));
                }
                using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> instance = snap2.CreateReadSnapshot())
                {
                    SortedTreeScannerBase<HistorianKey, HistorianValue> scanner = instance.GetTreeScanner();
                    scanner.SeekToStart();
                    Assert.AreEqual(false, scanner.Read(key, value));
                }
            }
        }
    }
}