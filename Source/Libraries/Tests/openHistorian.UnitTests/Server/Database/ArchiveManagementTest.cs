using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using openHistorian.Archive;
using openHistorian.Engine;
using openHistorian.Engine.ArchiveWriters;
using openHistorian.Engine.Configuration;

namespace openHistorian.Server.Database
{
    [TestFixture]
    public class ArchiveManagementTest
    {

        [Test]
        public void TestConstructor()
        {
            InitializeTiming();

            var sw = new DebugStopwatch();
            var settings = new ArchiveRolloverSettings();
            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
            settings.Initializer.IsMemoryArchive = true;
            settings.NewFileOnSize = long.MaxValue;

            var list = new ArchiveList();
            using (var writer = new ConcurrentArchiveMerger(settings, list, (file, x) => FinishArchive(list, file), x => x.Archive.Dispose()))
            {
                Thread.Sleep(150);
                sw.Start();
            }
            sw.Stop(2);
            using (var editor = list.AcquireEditLock())
            {
                Assert.AreEqual(editor.ArchiveFiles.Count, 0);
            }
        }

        [Test]
        public void TestCommitTiming()
        {
            InitializeTiming();

            var sw = new DebugStopwatch();
            var settings = new ArchiveRolloverSettings();
            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
            settings.Initializer.IsMemoryArchive = true;
            settings.NewFileOnSize = long.MaxValue;

            var list = new ArchiveList();
            using (var writer = new MergerPair(settings, list, (file, x) => FinishArchive(list, file), x => x.Archive.Dispose()))
            {

                Thread.Sleep(150);
                var archive = MakeArchive(0, 20, 1);
                using (var edit = list.AcquireEditLock())
                {
                    edit.Add(archive, false);
                }
                sw.Start();
                writer.ArchiveMerger.ProcessArchive(archive, 60);
                writer.WaitHandles.WaitForCommit(60, false);
            }
            sw.Stop(4);
            using (var editor = list.AcquireEditLock())
            {
                Assert.AreEqual(editor.ArchiveFiles.Count, 1);
            }
        }

        [Test]
        public void TestRolloverTiming()
        {
            InitializeTiming();

            var sw = new DebugStopwatch();
            var settings = new ArchiveRolloverSettings();
            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
            settings.Initializer.IsMemoryArchive = true;
            settings.NewFileOnSize = long.MaxValue;

            var list = new ArchiveList();
            using (var writer = new MergerPair(settings, list, (file, x) => FinishArchive(list, file), x => x.Archive.Dispose()))
            {
                Thread.Sleep(150);
                var archive = MakeArchive(0, 20, 1);
                using (var edit = list.AcquireEditLock())
                {
                    edit.Add(archive, false);
                }
                sw.Start();
                writer.ArchiveMerger.ProcessArchive(archive, 60);
                writer.WaitHandles.WaitForRollover(60, false);
            }
            sw.Stop(88, 112);
            using (var editor = list.AcquireEditLock())
            {
                Assert.AreEqual(editor.ArchiveFiles.Count, 1);
            }
        }
        [Test]
        public void TestForcedRolloverTiming()
        {
            InitializeTiming();

            var sw = new DebugStopwatch();
            var settings = new ArchiveRolloverSettings();
            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
            settings.Initializer.IsMemoryArchive = true;
            settings.NewFileOnSize = long.MaxValue;

            var list = new ArchiveList();
            using (var writer = new MergerPair(settings, list, (file, x) => FinishArchive(list, file), x => x.Archive.Dispose()))
            {
                Thread.Sleep(150);
                var archive = MakeArchive(0, 20, 1);
                using (var edit = list.AcquireEditLock())
                {
                    edit.Add(archive, false);
                }
                sw.Start();
                writer.ArchiveMerger.ProcessArchive(archive, 60);
                writer.WaitHandles.WaitForRollover(60, true);
            }
            sw.Stop(3);
            using (var editor = list.AcquireEditLock())
            {
                Assert.AreEqual(editor.ArchiveFiles.Count, 1);
            }
        }

        [Test]
        public void TestForcedBigCombine()
        {
            var settings = new ArchiveRolloverSettings();
            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
            settings.Initializer.IsMemoryArchive = true;
            settings.NewFileOnSize = long.MaxValue;

            var list = new ArchiveList();
            using (var writer = new MergerPair(settings, list, (file, x) => FinishArchive(list, file), x => x.Archive.Dispose()))
            {
                var archive1 = MakeArchive(2, 100, 2);
                var archive2 = MakeArchive(1, 81, 2);
                var archive3 = MakeArchive(83, 101, 2);
                using (var edit = list.AcquireEditLock())
                {
                    edit.Add(archive1, false);
                    edit.Add(archive2, false);
                    edit.Add(archive3, false);
                }
                writer.ArchiveMerger.ProcessArchive(archive1, 1);
                writer.WaitHandles.Commit();
                using (var edit = list.AcquireEditLock())
                {
                    using (var rdr = edit.ArchiveFiles.Last().ActiveSnapshotInfo.CreateReadSnapshot())
                    {
                        Assert.IsTrue(rdr.FirstKey == 2);
                        Assert.IsTrue(rdr.LastKey == 100);
                    }
                }
                writer.ArchiveMerger.ProcessArchive(archive2, 2);
                writer.WaitHandles.Commit();
                using (var edit = list.AcquireEditLock())
                {
                    using (var rdr = edit.ArchiveFiles.Last().ActiveSnapshotInfo.CreateReadSnapshot())
                    {
                        Assert.IsTrue(rdr.FirstKey == 1);
                        Assert.IsTrue(rdr.LastKey == 100);
                    }
                }
                writer.ArchiveMerger.ProcessArchive(archive3, 3);
                writer.WaitHandles.Commit();
                using (var edit = list.AcquireEditLock())
                {
                    using (var rdr = edit.ArchiveFiles.Last().ActiveSnapshotInfo.CreateReadSnapshot())
                    {
                        Assert.IsTrue(rdr.FirstKey == 1);
                        Assert.IsTrue(rdr.LastKey == 101);

                        var scan = rdr.GetTreeScanner();
                        scan.SeekToKey(0, 0);

                        ulong value1, value2, value3, value4;

                        for (uint x = 1; x < 102; x++)
                        {
                            scan.GetNextKey(out value1, out value2, out value3, out value4);
                            Assert.IsTrue(value1 == x);
                        }
                        Assert.IsFalse(scan.GetNextKey(out value1, out value2, out value3, out value4));
                    }
                }
            }
            using (var editor = list.AcquireEditLock())
            {
                Assert.AreEqual(editor.ArchiveFiles.Count, 1);
            }
        }

        void FinishArchive(ArchiveList list, ArchiveFile file)
        {
            using (var editor = list.AcquireEditLock())
            {
                editor.ReleaseEditLock(file);
            }
        }

        void InitializeTiming()
        {
            var settings = new ArchiveRolloverSettings();
            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
            settings.Initializer.IsMemoryArchive = true;
            settings.NewFileOnSize = long.MaxValue;

            var list = new ArchiveList();
            using (var writer = new MergerPair(settings, list, (file, x) => FinishArchive(list, file), x => x.Archive.Dispose()))
            {
                Thread.Sleep(150);
                var archive = MakeArchive(0, 200, 1);
                using (var edit = list.AcquireEditLock())
                {
                    edit.Add(archive, false);
                }
                writer.ArchiveMerger.ProcessArchive(archive, 60);
                writer.WaitHandles.WaitForCommit(60, false);
            }
            using (var editor = list.AcquireEditLock())
            {
                Assert.AreEqual(editor.ArchiveFiles.Count, 1);
            }
        }

        ArchiveFile MakeArchive(uint start, uint stop, uint step)
        {
            var file = ArchiveFile.CreateInMemory();
            using (var edit = file.BeginEdit())
            {
                for (ulong value = start; value <= stop; value += step)
                {
                    edit.AddPoint(value, 0, 0, 0);
                }
                edit.Commit();
            }
            return file;
        }

    }
}
