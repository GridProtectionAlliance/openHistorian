using System;
using NUnit.Framework;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace SampleCode.openHistorian.Archive.dll
{
    public class Sample1
    {
        public void WriteDataToAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            using (ArchiveTable<HistorianKey, HistorianValue> file = ArchiveFile.CreateFile(fileName).OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                using (ArchiveTable<HistorianKey, HistorianValue>.Editor editor = file.BeginEdit())
                {
                    key.Timestamp = (ulong)DateTime.Now.Ticks;
                    key.PointID = 1ul;
                    value.Value1 = 0;
                    value.Value2 = 25;
                    editor.AddPoint(key, value);
                    editor.Commit();
                }
            }
        }

        public void ReadDataFromAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (ArchiveTable<HistorianKey, HistorianValue> file = ArchiveFile.OpenFile(fileName, isReadOnly: true).OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> readSnapshot = file.BeginRead())
                {
                    TreeScannerBase<HistorianKey, HistorianValue> scanner = readSnapshot.GetTreeScanner();

                    //Seeks to the beginning of the tree since {0,0} is the lowest possible value that can be stored in the tree.
                    scanner.SeekToStart();

                    ulong time, key, quality, value;
                    while (scanner.Read())
                    {
                        Console.WriteLine("{0} - {1} - {2} - {3}", scanner.CurrentKey.Timestamp, scanner.CurrentKey.PointID, scanner.CurrentValue.Value1, scanner.CurrentValue.Value2);
                    }
                }
            }
        }

        public void AppendDataToAnExistingFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (ArchiveTable<HistorianKey, HistorianValue> file = ArchiveFile.OpenFile(fileName, isReadOnly: false).OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                using (ArchiveTable<HistorianKey, HistorianValue>.Editor editor = file.BeginEdit())
                {
                    key.Timestamp = (ulong)DateTime.Now.Ticks;
                    key.PointID = 1ul;
                    value.Value1 = 0;
                    value.Value2 = 25;
                    editor.AddPoint(key, value); //2 x 64-bit keys and 2 x 64-bit values. All are UInt64 values.
                    editor.Commit();
                }
            }
        }

        public void RollbackChangesToAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (ArchiveTable<HistorianKey, HistorianValue> file = ArchiveFile.OpenFile(fileName, isReadOnly: false).OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                using (ArchiveTable<HistorianKey, HistorianValue>.Editor editor = file.BeginEdit())
                {
                    key.Timestamp = (ulong)DateTime.Now.Ticks;
                    key.PointID = 1ul;
                    value.Value1 = 0;
                    value.Value2 = 25;
                    editor.AddPoint(key, value); //2 x 64-bit keys and 2 x 64-bit values. All are UInt64 values.
                    editor.Commit();
                }

                using (ArchiveTable<HistorianKey, HistorianValue>.Editor editor = file.BeginEdit())
                {
                    key.Timestamp = 1;
                    key.PointID = 0;
                    value.Value1 = 2;
                    value.Value2 = 3;
                    for (uint x = 0; x < 1000; x++)
                    {
                        key.PointID = x;
                        editor.AddPoint(key, value);
                    }
                    editor.Rollback(); //These changes will not be written to the disk
                }
            }
        }

        public void ConcurrentReadingFromAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (ArchiveTable<HistorianKey, HistorianValue> file = ArchiveFile.OpenFile(fileName, isReadOnly: false).OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                ArchiveTableSnapshotInfo<HistorianKey, HistorianValue> snapshotInfo = file.AcquireReadSnapshot();

                using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> reader1 = snapshotInfo.CreateReadSnapshot())
                using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> reader2 = snapshotInfo.CreateReadSnapshot())
                {
                    TreeScannerBase<HistorianKey, HistorianValue> scanner1 = reader1.GetTreeScanner();
                    TreeScannerBase<HistorianKey, HistorianValue> scanner2 = reader2.GetTreeScanner();

                    //Seeks to the beginning of the tree since {0,0} is the lowest possible value that can be stored in the tree.
                    scanner1.SeekToStart();
                    scanner2.SeekToStart();

                    while (scanner1.Read() &
                           scanner2.Read())
                    {
                        Assert.AreEqual(scanner1.CurrentKey.Timestamp, scanner2.CurrentKey.Timestamp);
                        Assert.AreEqual(scanner1.CurrentKey.PointID, scanner2.CurrentKey.PointID);
                        Assert.AreEqual(scanner1.CurrentValue.Value1, scanner2.CurrentValue.Value1);
                        Assert.AreEqual(scanner1.CurrentValue.Value2, scanner2.CurrentValue.Value2);
                    }
                }
            }
        }

        public void ReadFromAFileWhileWritingToIt()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (ArchiveTable<HistorianKey, HistorianValue> file = ArchiveFile.OpenFile(fileName, isReadOnly: false).OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid))
            {
                using (ArchiveTable<HistorianKey, HistorianValue>.Editor writer = file.BeginEdit())
                using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> reader = file.BeginRead())
                {
                    TreeScannerBase<HistorianKey, HistorianValue> scanner = reader.GetTreeScanner();

                    //Seeks to the beginning of the tree since {0,0} is the lowest possible value that can be stored in the tree.
                    scanner.SeekToStart();

                    ulong time, key, quality, value;

                    while (scanner.Read())
                    {
                        scanner.CurrentKey.Timestamp++;
                        scanner.CurrentValue.Value2++;
                        writer.AddPoint(scanner.CurrentKey, scanner.CurrentValue);
                    }

                    writer.Commit();
                }
            }
        }
    }
}