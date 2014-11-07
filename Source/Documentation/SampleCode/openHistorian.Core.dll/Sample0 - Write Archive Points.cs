using System;
using System.IO;
using NUnit.Framework;
using GSF.SortedTreeStore.Storage;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Tree.TreeNodes;

namespace SampleCode.openHistorian.Core.dll
{
    [TestFixture]
    public class Sample0
    {
        [Test]
        public void WriteDataToAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";
            if (File.Exists(fileName))
                File.Delete(fileName);
            var key = new HistorianKey();
            var value = new HistorianValue();
            using (var file = SortedTreeFile.CreateFile(fileName))
            using (var table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(SortedTree.FixedSizeNode))
            using (var editor = table.BeginEdit())
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

            using (var file = SortedTreeFile.OpenFile(fileName, isReadOnly: true))
            using (var table = file.OpenTable<HistorianKey, HistorianValue>())
            using (var snapshot = table.BeginRead())
            {
                var scanner = snapshot.GetTreeScanner();
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
            var key = new HistorianKey();
            var value = new HistorianValue();
            using (var file = SortedTreeFile.OpenFile(fileName, isReadOnly: false))
            using (var table = file.OpenTable<HistorianKey, HistorianValue>())
            using (var editor = table.BeginEdit())
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
            var key = new HistorianKey();
            var value = new HistorianValue();
            using (var file = SortedTreeFile.OpenFile(fileName, isReadOnly: false))
            using (var table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(SortedTree.FixedSizeNode))
            {
                using (var editor = table.BeginEdit())
                {
                    key.TimestampAsDate = DateTime.Now.AddDays(-1);
                    key.PointID = 234;
                    value.AsString = "Add Me";
                    editor.AddPoint(key, value);
                    editor.Commit();
                }

                using (var editor = table.BeginEdit())
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

            using (var file = SortedTreeFile.OpenFile(fileName, isReadOnly: true))
            using (var table = file.OpenTable<HistorianKey, HistorianValue>())
            {
                var snapshotInfo = table.AcquireReadSnapshot();

                
                using (var reader1 = snapshotInfo.CreateReadSnapshot())
                using (var reader2 = snapshotInfo.CreateReadSnapshot())
                {
                    var scanner1 = reader1.GetTreeScanner();
                    var scanner2 = reader2.GetTreeScanner();

                    scanner1.SeekToStart();
                    scanner2.SeekToStart();

                    while (scanner1.Read(key1,value1) & scanner2.Read(key2,value2))
                    {
                        Assert.AreEqual(key1.Timestamp, key2.Timestamp);
                        Assert.AreEqual(key1.PointID, key2.PointID);
                        Assert.AreEqual(key1.EntryNumber, key2.EntryNumber);
                        Assert.AreEqual(value1.Value1, value2.Value1);
                        Assert.AreEqual(value1.Value2, value2.Value2);
                        Assert.AreEqual(value1.Value3, value2.Value3);
                    }
                }
            }
            ReadDataFromAFile();

        }

        [Test]
        public void ReadFromAFileWhileWritingToIt()
        {
            var key = new HistorianKey();
            var value = new HistorianValue();
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (var file = SortedTreeFile.OpenFile(fileName, isReadOnly: false))
            using (var table = file.OpenTable<HistorianKey, HistorianValue>())
            {
                using (var writer = table.BeginEdit())
                using (var reader = table.BeginRead())
                {
                    var scanner = reader.GetTreeScanner();
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