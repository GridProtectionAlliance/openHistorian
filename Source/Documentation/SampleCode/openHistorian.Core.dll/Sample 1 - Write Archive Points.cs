using System;
using System.IO;
using NUnit.Framework;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace SampleCode.openHistorian.Archive.dll
{
    [TestFixture]
    public class Sample1
    {
        [Test]
        public void WriteDataToAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";
            if (File.Exists(fileName))
                File.Delete(fileName);
            var key = new HistorianKey();
            var value = new HistorianValue();
            using (var file = ArchiveFile.CreateFile(fileName))
            using (var table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
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

            using (var file = ArchiveFile.OpenFile(fileName, isReadOnly: true))
            using (var table = file.OpenTable<HistorianKey, HistorianValue>())
            using (var snapshot = table.BeginRead())
            {
                var scanner = snapshot.GetTreeScanner();
                scanner.SeekToStart();
                while (scanner.Read())
                {
                    var key = scanner.CurrentKey;
                    var value = scanner.CurrentValue;
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
            using (var file = ArchiveFile.OpenFile(fileName, isReadOnly: false))
            using (var table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
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
            using (var file = ArchiveFile.OpenFile(fileName, isReadOnly: false))
            using (var table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
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

            using (var file = ArchiveFile.OpenFile(fileName, isReadOnly: true))
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

                    while (scanner1.Read() & scanner2.Read())
                    {
                        Assert.AreEqual(scanner1.CurrentKey.Timestamp, scanner2.CurrentKey.Timestamp);
                        Assert.AreEqual(scanner1.CurrentKey.PointID, scanner2.CurrentKey.PointID);
                        Assert.AreEqual(scanner1.CurrentKey.EntryNumber, scanner2.CurrentKey.EntryNumber);
                        Assert.AreEqual(scanner1.CurrentValue.Value1, scanner2.CurrentValue.Value1);
                        Assert.AreEqual(scanner1.CurrentValue.Value2, scanner2.CurrentValue.Value2);
                        Assert.AreEqual(scanner1.CurrentValue.Value3, scanner2.CurrentValue.Value3);
                    }
                }
            }
            ReadDataFromAFile();

        }

        [Test]
        public void ReadFromAFileWhileWritingToIt()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (var file = ArchiveFile.OpenFile(fileName, isReadOnly: false))
            using (var table = file.OpenTable<HistorianKey, HistorianValue>())
            {
                using (ArchiveTable<HistorianKey, HistorianValue>.Editor writer = table.BeginEdit())
                using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> reader = table.BeginRead())
                {
                    var scanner = reader.GetTreeScanner();
                    scanner.SeekToStart();
                    while (scanner.Read())
                    {
                        scanner.CurrentKey.Timestamp++;
                        scanner.CurrentValue.Value1++;
                        writer.AddPoint(scanner.CurrentKey, scanner.CurrentValue);
                    }

                    writer.Commit();
                }
            }
            ReadDataFromAFile();

        }
    }
}