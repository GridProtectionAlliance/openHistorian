//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Threading;
//using GSF;
//using NUnit.Framework;
//using openHistorian.Archive;
//using openHistorian.Engine;
//using openHistorian.Engine.ArchiveWriters;
//using openHistorian.Engine.Configuration;

//namespace openHistorian.Server.Database
//{
//    [TestFixture]
//    public class ArchiveWriterTest
//    {
//        ManualResetEvent m_waitHandle;
//        ManualResetEvent m_waitHandle2;
//        Stopwatch m_sw3;
//        Stopwatch m_sw;
//        Stopwatch m_sw2;

//        [Test]
//        public void TestThreadSynchronizationTiming()
//        {
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            m_waitHandle = new ManualResetEvent(false);
//            m_waitHandle.Set();
//            m_waitHandle.WaitOne();
//            sw.Stop();
//            m_waitHandle = new ManualResetEvent(false);
//            m_waitHandle2 = new ManualResetEvent(false);
//            m_sw = new Stopwatch();
//            m_sw2 = new Stopwatch();
//            m_sw3 = new Stopwatch();
//            var th = new Thread(Process);
//            m_sw3.Start();
//            th.Start();
//            Thread.Sleep(100);
//            m_sw.Start();
//            m_waitHandle.Set();
//            m_waitHandle2.WaitOne(10);
//            m_sw2.Stop();

//            m_sw = m_sw;
//        }
//        void Process()
//        {
//            m_sw3.Stop();
//            m_waitHandle.WaitOne();
//            m_sw.Stop();
//            m_sw2.Start();
//            m_waitHandle2.Set();
//        }

//        [Test]
//        public void TestConstructor()
//        {
//            InitializeTiming();

//            var sw = new DebugStopwatch();
//            var settings = new ArchiveWriterSettings();
//            settings.CommitOnInterval = new TimeSpan(0, 0, 0, 0, 10);
//            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
//            var list = new ArchiveList();
//            using (var writer = new ConcurrentWriterAutoCommit(settings, list, (file, x) => FinishArchive(list, file)))
//            {
//                Thread.Sleep(150);
//                sw.Start();
//            }
//            sw.Stop(2);
//            using (var editor = list.AcquireEditLock())
//            {
//                Assert.AreEqual(editor.ArchiveFiles.Count, 0);
//            }
//        }

//        [Test]
//        public void TestForcedRolloverTiming()
//        {
//            InitializeTiming();

//            var sw = new DebugStopwatch();
//            var settings = new ArchiveWriterSettings();
//            settings.CommitOnInterval = new TimeSpan(0, 0, 0, 0, 10);
//            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
//            var list = new ArchiveList();
//            using (var writer = new ConcurrentWriterAutoCommit(settings, list, (file, x) => FinishArchive(list, file)))
//            {
//                Thread.Sleep(15);
//                sw.Start();
//                long sequenceId = writer.WriteData(0ul, 0ul, 0ul, 0ul);
//                writer.CommitAndRollover();
//                sw.Stop(1.5);
//            }
//            using (var editor = list.AcquireEditLock())
//            {
//                Assert.AreEqual(editor.ArchiveFiles.Count, 1);
//            }
//        }

//        [Test]
//        public void TestCommitTiming()
//        {
//            InitializeTiming();

//            var sw = new DebugStopwatch();
//            var settings = new ArchiveWriterSettings();
//            settings.CommitOnInterval = new TimeSpan(0, 0, 0, 0, 10);
//            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
//            var list = new ArchiveList();
//            using (var writer = new ConcurrentWriterAutoCommit(settings, list, (file, x) => FinishArchive(list, file)))
//            {
//                sw.DoGC();
//                long sequenceId = writer.WriteData(0ul, 0ul, 0ul, 0ul);
//                writer.Commit();
//                sw.Start();
//                sequenceId = writer.WriteData(1ul, 0ul, 0ul, 0ul);
//                writer.WaitForCommit(sequenceId, false);
//                sw.Stop(8.5,13);
//            }
//            using (var editor = list.AcquireEditLock())
//            {
//                Assert.AreEqual(editor.ArchiveFiles.Count, 1);
//            }
//        }
//        [Test]
//        public void TestRolloverTiming()
//        {
//            InitializeTiming();

//            var sw = new DebugStopwatch();
//            var settings = new ArchiveWriterSettings();
//            settings.CommitOnInterval = new TimeSpan(0, 0, 0, 0, 20);
//            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 110);
//            var list = new ArchiveList();
//            using (var writer = new ConcurrentWriterAutoCommit(settings, list, (file, x) => FinishArchive(list, file)))
//            {
//                sw.DoGC();
//                long sequenceId = writer.WriteData(0ul, 0ul, 0ul, 0ul);
//                writer.CommitAndRollover();

//                sw.Start(true);
//                sequenceId = writer.WriteData(1ul, 0ul, 0ul, 0ul);
//                writer.WaitForRollover(sequenceId, false);
//                sw.Stop(104, 112);
//            }
//            using (var editor = list.AcquireEditLock())
//            {
//                Assert.AreEqual(editor.ArchiveFiles.Count, 2);
//            }
//        }

//        void FinishArchive(ArchiveList list, ArchiveFile file)
//        {
//            using (var editor = list.AcquireEditLock())
//            {
//                editor.ReleaseEditLock(file);
//            }
//        }

//        void InitializeTiming()
//        {
//            List<int> allocations = new List<int>(100);
//            int page = 0;
//            IntPtr ptr;
//            while (allocations.Count < 100)
//            {
//                Globals.BufferPool.AllocatePage(out page, out ptr);
//                allocations.Add(page);
//            }
//            Globals.BufferPool.ReleasePages(allocations);

//            var settings = new ArchiveWriterSettings();
//            settings.CommitOnInterval = new TimeSpan(0, 0, 0, 0, 10);
//            settings.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
//            var list = new ArchiveList();
//            using (var writer = new ConcurrentWriterAutoCommit(settings, list, (file, x) => FinishArchive(list, file)))
//            {
//                writer.WriteData(0ul, 0ul, 0ul, 0ul);
//                writer.CommitAndRollover();
//            }
//        }

//    }
//}

