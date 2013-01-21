

using System;
using NUnit.Framework;
using openHistorian;
using openHistorian.Archive;
using openHistorian.Collections.KeyValue;

namespace SampleCode.openHistorian.Archive.dll
{
    public class Sample1
    {
        public void WriteDataToAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";
            CompressionMethod method = CompressionMethod.None;

            using (ArchiveFile file = ArchiveFile.CreateFile(fileName, method))
            {
                using (ArchiveFile.Editor editor = file.BeginEdit())
                {
                    ulong time = (ulong)DateTime.Now.Ticks;
                    ulong key = 1ul;
                    ulong quality = 0;
                    ulong value = 25;
                    editor.AddPoint(time, key, quality, value); //2 x 64-bit keys and 2 x 64-bit values. All are UInt64 values.
                    editor.Commit();
                }
            }
        }

        public void ReadDataFromAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (ArchiveFile file = ArchiveFile.OpenFile(fileName, AccessMode.ReadOnly))
            {
                using (ArchiveFileReadSnapshot readSnapshot = file.BeginRead())
                {
                    ITreeScanner256 scanner = readSnapshot.GetTreeScanner();

                    //Seeks to the beginning of the tree since {0,0} is the lowest possible value that can be stored in the tree.
                    scanner.SeekToKey(0ul, 0ul);

                    ulong time, key, quality, value;
                    while (scanner.Read(out time, out key, out quality, out value))
                    {
                        Console.WriteLine("{0} - {1} - {2} - {3}", time, key, quality, value);
                    }
                }
            }
        }

        public void AppendDataToAnExistingFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (ArchiveFile file = ArchiveFile.OpenFile(fileName, AccessMode.ReadWrite))
            {
                using (ArchiveFile.Editor editor = file.BeginEdit())
                {
                    ulong time = (ulong)DateTime.Now.Ticks;
                    ulong key = 1ul;
                    ulong quality = 0;
                    ulong value = 25;
                    editor.AddPoint(time, key, quality, value); //2 x 64-bit keys and 2 x 64-bit values. All are UInt64 values.
                    editor.Commit();
                }
            }
        }

        public void RollbackChangesToAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (ArchiveFile file = ArchiveFile.OpenFile(fileName, AccessMode.ReadWrite))
            {
                using (ArchiveFile.Editor editor = file.BeginEdit())
                {
                    ulong time = (ulong)DateTime.Now.Ticks;
                    ulong key = 1ul;
                    ulong quality = 0;
                    ulong value = 25;
                    editor.AddPoint(time, key, quality, value); //2 x 64-bit keys and 2 x 64-bit values. All are UInt64 values.
                    editor.Commit();
                }

                using (ArchiveFile.Editor editor = file.BeginEdit())
                {
                    for (uint x = 0; x < 1000; x++)
                    {
                        editor.AddPoint(1, x, 2, 3);
                    }
                    editor.Rollback(); //These changes will not be written to the disk
                }
            }
        }

        public void ConcurrentReadingFromAFile()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (ArchiveFile file = ArchiveFile.OpenFile(fileName, AccessMode.ReadWrite))
            {
                ArchiveFileSnapshotInfo snapshotInfo = file.AcquireReadSnapshot();

                using (ArchiveFileReadSnapshot reader1 = snapshotInfo.CreateReadSnapshot())
                using (ArchiveFileReadSnapshot reader2 = snapshotInfo.CreateReadSnapshot())
                {
                    ITreeScanner256 scanner1 = reader1.GetTreeScanner();
                    ITreeScanner256 scanner2 = reader2.GetTreeScanner();

                    //Seeks to the beginning of the tree since {0,0} is the lowest possible value that can be stored in the tree.
                    scanner1.SeekToKey(0ul, 0ul);
                    scanner2.SeekToKey(0ul, 0ul);

                    ulong time1, key1, quality1, value1;
                    ulong time2, key2, quality2, value2;

                    while (scanner1.Read(out time1, out key1, out quality1, out value1) &
                       scanner2.Read(out time2, out key2, out quality2, out value2))
                    {
                        Assert.AreEqual(time1, time2);
                        Assert.AreEqual(key1, key2);
                        Assert.AreEqual(quality1, quality2);
                        Assert.AreEqual(value1, value2);
                    }
                }
            }
        }

        public void ReadFromAFileWhileWritingToIt()
        {
            string fileName = @"C:\Temp\ArchiveFile.d2";

            using (ArchiveFile file = ArchiveFile.OpenFile(fileName, AccessMode.ReadWrite))
            {
                using (ArchiveFile.Editor writer = file.BeginEdit())
                using (ArchiveFileReadSnapshot reader = file.BeginRead())
                {
                    ITreeScanner256 scanner = reader.GetTreeScanner();

                    //Seeks to the beginning of the tree since {0,0} is the lowest possible value that can be stored in the tree.
                    scanner.SeekToKey(0ul, 0ul);

                    ulong time, key, quality, value;

                    while (scanner.Read(out time, out key, out quality, out value))
                    {
                        writer.AddPoint(time + 1, key, quality, value + 1);
                    }

                    writer.Commit();
                }
            }
        }


    }
}
