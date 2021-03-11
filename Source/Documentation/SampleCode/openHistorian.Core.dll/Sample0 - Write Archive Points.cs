using System;
using System.IO;
using GSF.Diagnostics;
using GSF.Snap;
using NUnit.Framework;
using GSF.Snap.Storage;
using GSF.Snap.Tree;
using openHistorian.Snap;

namespace SampleCode.openHistorian.Core.dll
{
    [TestFixture]
    public class Sample0
    {
        [Test]
        public void WriteDataToAFile()
        {
            Logger.Console.Verbose = VerboseLevel.All;
            string fileName = @"C:\Temp\ArchiveFile.d2";
            if (File.Exists(fileName))
                File.Delete(fileName);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            using (SortedTreeFile file = SortedTreeFile.CreateFile(fileName))
            using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            using (SortedTreeTableEditor<HistorianKey, HistorianValue> editor = table.BeginEdit())
            {
                key.TimestampAsDate = DateTime.Now;
                key.PointID = 1;
                value.AsString = "Test Write";
                editor.AddPoint(key, value);
                editor.Commit();
            }
        }

        [Test]
        public void ReadDataFromAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (SortedTreeFile file = SortedTreeFile.OpenFile(fileName, isReadOnly: true))
            using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>())
            using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> snapshot = table.BeginRead())
            {
                SortedTreeScannerBase<HistorianKey, HistorianValue> scanner = snapshot.GetTreeScanner();
                scanner.SeekToStart();
                while (scanner.Read(key,value))
                {
                    Console.WriteLine("{0}, {1}, {2}",
                        key.TimestampAsDate.ToString(), key.PointID, value.AsString);
                }
            }
        }

        [Test]
        public void AppendDataToAnExistingFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            using (SortedTreeFile file = SortedTreeFile.OpenFile(fileName, isReadOnly: false))
            using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>())
            using (SortedTreeTableEditor<HistorianKey, HistorianValue> editor = table.BeginEdit())
            {
                key.TimestampAsDate = DateTime.Now;
                key.PointID = 2;
                value.AsString = "Test Append";
                editor.AddPoint(key, value);
                editor.Commit();
            }
            ReadDataFromAFile();
        }

        [Test]
        public void RollbackChangesToAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            using (SortedTreeFile file = SortedTreeFile.OpenFile(fileName, isReadOnly: false))
            using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                using (SortedTreeTableEditor<HistorianKey, HistorianValue> editor = table.BeginEdit())
                {
                    key.TimestampAsDate = DateTime.Now.AddDays(-1);
                    key.PointID = 234;
                    value.AsString = "Add Me";
                    editor.AddPoint(key, value);
                    editor.Commit();
                }

                using (SortedTreeTableEditor<HistorianKey, HistorianValue> editor = table.BeginEdit())
                {
                    key.Timestamp = 31;
                    value.AsString = "But Not Me";
                    editor.AddPoint(key, value);
                    editor.Rollback(); //These changes will not be written to the disk
                }
            }
            ReadDataFromAFile();

        }

        [Test]
        public void ConcurrentReadingFromAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            HistorianKey key1 = new HistorianKey();
            HistorianKey key2 = new HistorianKey();
            HistorianValue value1 = new HistorianValue();
            HistorianValue value2 = new HistorianValue();

            using (SortedTreeFile file = SortedTreeFile.OpenFile(fileName, isReadOnly: true))
            using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>())
            {
                SortedTreeTableSnapshotInfo<HistorianKey, HistorianValue> snapshotInfo = table.AcquireReadSnapshot();

                
                using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> reader1 = snapshotInfo.CreateReadSnapshot())
                using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> reader2 = snapshotInfo.CreateReadSnapshot())
                {
                    SortedTreeScannerBase<HistorianKey, HistorianValue> scanner1 = reader1.GetTreeScanner();
                    SortedTreeScannerBase<HistorianKey, HistorianValue> scanner2 = reader2.GetTreeScanner();

                    scanner1.SeekToStart();
                    scanner2.SeekToStart();

                    bool scanner1Read = scanner1.Read(key1, value1);
                    bool scanner2Read = scanner2.Read(key2, value2);

                    while (scanner1Read && scanner2Read)
                    {
                        Assert.AreEqual(key1.Timestamp, key2.Timestamp);
                        Assert.AreEqual(key1.PointID, key2.PointID);
                        Assert.AreEqual(key1.EntryNumber, key2.EntryNumber);
                        Assert.AreEqual(value1.Value1, value2.Value1);
                        Assert.AreEqual(value1.Value2, value2.Value2);
                        Assert.AreEqual(value1.Value3, value2.Value3);

                        scanner1Read = scanner1.Read(key1, value1);
                        scanner2Read = scanner2.Read(key2, value2);
                    }
                }
            }
            ReadDataFromAFile();

        }

        [Test]
        public void ReadFromAFileWhileWritingToIt()
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (SortedTreeFile file = SortedTreeFile.OpenFile(fileName, isReadOnly: false))
            using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>())
            {
                using (SortedTreeTableEditor<HistorianKey, HistorianValue> writer = table.BeginEdit())
                using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> reader = table.BeginRead())
                {
                    SortedTreeScannerBase<HistorianKey, HistorianValue> scanner = reader.GetTreeScanner();
                    scanner.SeekToStart();
                    while (scanner.Read(key,value))
                    {
                        key.Timestamp++;
                        value.Value1++;
                        writer.AddPoint(key, value);
                    }

                    writer.Commit();
                }
            }
            ReadDataFromAFile();

        }
    }
}