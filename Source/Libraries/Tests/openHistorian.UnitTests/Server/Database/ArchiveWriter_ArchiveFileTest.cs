using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using NUnit.Framework;
using openHistorian.IO.Unmanaged;
using openHistorian.Server.Database.Archive;
using openHistorian.Server.Database.ArchiveWriters;

namespace openHistorian.Server.Database
{
    [TestFixture]
    public class ArchiveWriterArchiveFileTest
    {
        [Test]
        public void TestEditing()
        {
            var stream = new BinaryStream();
            int fileCount = 0;
            var list = new ArchiveList();
            var archive = new ConcurrentWriterAutoCommit.ActiveFile(list, (file,x) => fileCount++);
            Assert.AreEqual(0, CountFiles(list));
            archive.CreateIfNotExists();
            Assert.AreEqual(1, CountFiles(list));
            archive.CreateIfNotExists();
            Assert.AreEqual(1, CountFiles(list));
            Assert.AreEqual(0, GetLatestValue(list));
            stream.Write(1ul);
            stream.Write(0ul);
            stream.Write(0ul);
            stream.Write(0ul);
            stream.Position = 0;
            archive.Append(stream, 1);
            Assert.AreEqual(0, GetLatestValue(list));
            archive.RefreshSnapshot();
            Assert.AreEqual(1, GetLatestValue(list));
            stream.Position = 0;
            stream.Write(10ul);
            stream.Position = 0;
            archive.Append(stream, 1);
            Assert.AreEqual(1, GetLatestValue(list));
            archive.RefreshSnapshot();
            Assert.AreEqual(10, GetLatestValue(list));
            stream.Position = 0;
            stream.Write(12ul);
            stream.Position = 0;
            archive.Append(stream, 1);
            Assert.AreEqual(0, fileCount);
            archive.RefreshAndRolloverFile(0);
            Assert.AreEqual(1, fileCount);
            Assert.AreEqual(12, GetLatestValue(list));
            archive.CreateIfNotExists();
            Assert.AreEqual(0, GetLatestValue(list));
            archive.RefreshAndRolloverFile(0);
            Assert.AreEqual(2, fileCount);
        }
        [Test]
        public void TestTiming()
        {
            var stream = new BinaryStream();
            int fileCount = 0;
            var list = new ArchiveList();
            var archive = new ConcurrentWriterAutoCommit.ActiveFile(list, (file, x) => fileCount++);

            GCSettings.LatencyMode = GCLatencyMode.Batch;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsTrue(archive.CommitAge.Ticks == 0);
            Assert.IsTrue(archive.FileAge.Ticks == 0);

            archive.CreateIfNotExists();
            Thread.Sleep(10);
            Assert.IsTrue(Math.Abs(10 - archive.CommitAge.TotalMilliseconds) < 1);
            Assert.IsTrue(Math.Abs(10 - archive.FileAge.TotalMilliseconds) < 1);
            archive.RefreshSnapshot();
            Assert.IsTrue(Math.Abs(0 - archive.CommitAge.TotalMilliseconds) < 1);
            Assert.IsTrue(Math.Abs(10 - archive.FileAge.TotalMilliseconds) < 2);
            Thread.Sleep(10);
            Assert.IsTrue(Math.Abs(10 - archive.CommitAge.TotalMilliseconds) < 1);
            Assert.IsTrue(Math.Abs(20 - archive.FileAge.TotalMilliseconds) < 2);
            archive.RefreshAndRolloverFile(0);
            Assert.IsTrue(archive.CommitAge.Ticks == 0);
            Assert.IsTrue(archive.FileAge.Ticks == 0);
        }

        int CountFiles(ArchiveList list)
        {
            using (var editor = list.AcquireEditLock())
            {
                return editor.ArchiveFiles.Count;
            }
        }

        int GetLatestValue(ArchiveList list)
        {
            using (var editor = list.AcquireEditLock())
            {
                return (int)editor.ArchiveFiles.Last().LastKeyValue;
            }
        }

        void FinishArchive(ArchiveList list, ArchiveFile file)
        {
            using (var editor = list.AcquireEditLock())
            {
                editor.ReleaseEditLock(file);
            }
        }

    }
}
