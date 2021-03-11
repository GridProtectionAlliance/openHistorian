using System;
using System.Collections.Generic;
using System.Text;
using GSF.Diagnostics;
using GSF.IO.Unmanaged.Test;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Snap.Storage;
using GSF.Snap.Tree;
using NUnit.Framework;
using openHistorian;
using openHistorian.Data.Query;
using openHistorian.Snap;

namespace GSF.Snap.Server
{
    [TestFixture]
    public class ArchiveReaderSequential_Test
    {
        [Test]
        public void TestOneFile()
        {
            HistorianKey key1 = new HistorianKey();
            HistorianKey key2 = new HistorianKey();
            HistorianValue value1 = new HistorianValue();
            HistorianValue value2 = new HistorianValue();
            Logger.Console.Verbose = VerboseLevel.All;
            MemoryPoolTest.TestMemoryLeak();
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>(null);

            SortedTreeTable<HistorianKey, HistorianValue> master = CreateTable();
            SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
            AddData(master, 100, 100, 100);
            AddData(table1, 100, 100, 100);
            using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
            {
                editor.Add(table1);
            }

            using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> masterRead = master.BeginRead())
            {
                SortedTreeScannerBase<HistorianKey, HistorianValue> masterScan = masterRead.GetTreeScanner();
                masterScan.SeekToStart();
                TreeStreamSequential<HistorianKey, HistorianValue> masterScanSequential = masterScan.TestSequential();

                using (SequentialReaderStream<HistorianKey, HistorianValue> sequencer = new SequentialReaderStream<HistorianKey, HistorianValue>(list))
                {
                    TreeStreamSequential<HistorianKey, HistorianValue> scanner = sequencer.TestSequential();

                    int count = 0;
                    while (scanner.Read(key1, value1))
                    {
                        count++;
                        if (!masterScanSequential.Read(key2, value2))
                            throw new Exception();

                        if (!key1.IsEqualTo(key2))
                            throw new Exception();

                        if (!value1.IsEqualTo(value2))
                            throw new Exception();
                    }
                    if (masterScan.Read(key2, value2))
                        throw new Exception();
                }

            }
            list.Dispose();
            master.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void TestTwoFiles()
        {
            MemoryPoolTest.TestMemoryLeak();
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>(null);

            HistorianKey key1 = new HistorianKey();
            HistorianKey key2 = new HistorianKey();
            HistorianValue value1 = new HistorianValue();
            HistorianValue value2 = new HistorianValue();

            SortedTreeTable<HistorianKey, HistorianValue> master = CreateTable();
            SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
            SortedTreeTable<HistorianKey, HistorianValue> table2 = CreateTable();
            AddData(master, 100, 100, 100);
            AddData(table1, 100, 100, 100);
            AddData(master, 101, 100, 100);
            AddData(table2, 101, 100, 100);
            using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
            {
                editor.Add(table1);
                editor.Add(table2);
            }

            using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> masterRead = master.BeginRead())
            {
                SortedTreeScannerBase<HistorianKey, HistorianValue> masterScan = masterRead.GetTreeScanner();
                masterScan.SeekToStart();
                TreeStreamSequential<HistorianKey, HistorianValue> masterScanSequential = masterScan.TestSequential();

                using (SequentialReaderStream<HistorianKey, HistorianValue> sequencer = new SequentialReaderStream<HistorianKey, HistorianValue>(list))
                {
                    TreeStreamSequential<HistorianKey, HistorianValue> scanner = sequencer.TestSequential();

                    while (scanner.Read(key1, value1))
                    {
                        if (!masterScanSequential.Read(key2, value2))
                            throw new Exception();

                        if (!key1.IsEqualTo(key2))
                            throw new Exception();

                        if (!value1.IsEqualTo(value2))
                            throw new Exception();
                    }
                    if (masterScan.Read(key2, value2))
                        throw new Exception();
                }
            }
            master.Dispose();
            list.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void TestTwoIdenticalFiles()
        {
            MemoryPoolTest.TestMemoryLeak();
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>(null);
            HistorianKey key1 = new HistorianKey();
            HistorianKey key2 = new HistorianKey();
            HistorianValue value1 = new HistorianValue();
            HistorianValue value2 = new HistorianValue();
            SortedTreeTable<HistorianKey, HistorianValue> master = CreateTable();
            SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
            SortedTreeTable<HistorianKey, HistorianValue> table2 = CreateTable();
            AddData(master, 100, 100, 100);
            AddData(table1, 100, 100, 100);
            AddData(table2, 100, 100, 100);
            using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
            {
                editor.Add(table1);
                editor.Add(table2);
            }

            using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> masterRead = master.BeginRead())
            {
                SortedTreeScannerBase<HistorianKey, HistorianValue> masterScan = masterRead.GetTreeScanner();
                masterScan.SeekToStart();
                TreeStreamSequential<HistorianKey, HistorianValue> masterScanSequential = masterScan.TestSequential();

                using (SequentialReaderStream<HistorianKey, HistorianValue> sequencer = new SequentialReaderStream<HistorianKey, HistorianValue>(list))
                {
                    TreeStreamSequential<HistorianKey, HistorianValue> scanner = sequencer.TestSequential();
                    while (scanner.Read(key1, value1))
                    {
                        if (!masterScanSequential.Read(key2, value2))
                            throw new Exception();

                        if (!key1.IsEqualTo(key2))
                            throw new Exception();

                        if (!value1.IsEqualTo(value2))
                            throw new Exception();
                    }
                    if (masterScan.Read(key2, value2))
                        throw new Exception();
                }
            }
            list.Dispose();
            master.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }


        SortedTreeTable<HistorianKey, HistorianValue> CreateTable()
        {
            SortedTreeFile file = SortedTreeFile.CreateInMemory();
            SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding);
            return table;
        }

        void AddData(SortedTreeTable<HistorianKey, HistorianValue> table, ulong start, ulong step, ulong count)
        {
            using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = table.BeginEdit())
            {
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();

                for (ulong v = start; v < start + step * count; v += step)
                {
                    key.SetMin();
                    key.PointID = v;
                    edit.AddPoint(key, value);
                }
                edit.Commit();
            }
        }
        void AddData(SortedTreeTable<HistorianKey, HistorianValue> table, DateTime startTime, TimeSpan stepTime, int countTime, ulong startPoint, ulong stepPoint, ulong countPoint)
        {
            using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = table.BeginEdit())
            {
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();
                key.SetMin();
                ulong stepTimeTicks = (ulong)stepTime.Ticks;
                ulong stopTime = (ulong)(startTime.Ticks + countTime * stepTime.Ticks);
                for (ulong t = (ulong)startTime.Ticks; t < stopTime; t += stepTimeTicks)
                {
                    for (ulong v = startPoint; v < startPoint + stepPoint * countPoint; v += stepPoint)
                    {
                        key.Timestamp = t;
                        key.PointID = v;
                        edit.AddPoint(key, value);
                    }
                }
                edit.Commit();
            }
        }
        void AddDataTerminal(SortedTreeTable<HistorianKey, HistorianValue> table, ulong pointID, DateTime startTime, TimeSpan stepTime, ulong startValue, ulong stepValue, int count)
        {
            using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = table.BeginEdit())
            {
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();
                key.SetMin();
                ulong t = (ulong)startTime.Ticks;
                ulong v = startValue;

                while (count > 0)
                {
                    count--;
                    key.Timestamp = t;
                    key.PointID = pointID;
                    value.Value1 = v;

                    edit.AddPoint(key, value);
                    t += (ulong)stepTime.Ticks;
                    v += stepValue;
                }

                edit.Commit();
            }
        }

        [Test]
        public void BenchmarkRawFile()
        {
            MemoryPoolTest.TestMemoryLeak();
            const int Max = 1000000;

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            SortedTreeTable<HistorianKey, HistorianValue> master = CreateTable();
            AddData(master, 100, 100, Max);


            DebugStopwatch sw = new DebugStopwatch();
            using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> masterRead = master.BeginRead())
            {
                double sec = sw.TimeEvent(() =>
                    {
                        SortedTreeScannerBase<HistorianKey, HistorianValue> scanner = masterRead.GetTreeScanner();
                        scanner.SeekToStart();
                        while (scanner.Read(key, value))
                        {
                        }
                    });
                System.Console.WriteLine(Max / sec / 1000000);

            }
            master.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void BenchmarkOneFile()
        {
            MemoryPoolTest.TestMemoryLeak();
            const int Max = 1000000;
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>(null);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
            AddData(table1, 100, 100, Max);
            using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
            {
                editor.Add(table1);
            }

            SequentialReaderStream<HistorianKey, HistorianValue> sequencer = new SequentialReaderStream<HistorianKey, HistorianValue>(list);

            DebugStopwatch sw = new DebugStopwatch();

            double sec = sw.TimeEvent(() =>
                {
                    SequentialReaderStream<HistorianKey, HistorianValue> scanner = sequencer;
                    while (scanner.Read(key,value))
                    {
                    }
                });
            System.Console.WriteLine(Max / sec / 1000000);
            table1.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void BenchmarkTwoFiles()
        {
            MemoryPoolTest.TestMemoryLeak();
            const int Max = 1000000;
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>(null);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
            SortedTreeTable<HistorianKey, HistorianValue> table2 = CreateTable();
            AddData(table1, 100, 100, Max / 2);
            AddData(table2, 101, 100, Max / 2);
            using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
            {
                editor.Add(table1);
                editor.Add(table2);
            }

            SequentialReaderStream<HistorianKey, HistorianValue> sequencer = new SequentialReaderStream<HistorianKey, HistorianValue>(list);

            DebugStopwatch sw = new DebugStopwatch();

            double sec = sw.TimeEvent(() =>
            {
                SequentialReaderStream<HistorianKey, HistorianValue> scanner = sequencer;
                while (scanner.Read(key,value))
                {
                }
            });
            System.Console.WriteLine(Max / sec / 1000000);
            list.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void BenchmarkThreeFiles()
        {
            MemoryPoolTest.TestMemoryLeak();
            const int Max = 1000000;
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>(null);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
            SortedTreeTable<HistorianKey, HistorianValue> table2 = CreateTable();
            SortedTreeTable<HistorianKey, HistorianValue> table3 = CreateTable();
            AddData(table1, 100, 100, Max / 3);
            AddData(table2, 101, 100, Max / 3);
            AddData(table3, 102, 100, Max / 3);
            using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
            {
                editor.Add(table1);
                editor.Add(table2);
                editor.Add(table3);
            }

            SequentialReaderStream<HistorianKey, HistorianValue> sequencer = new SequentialReaderStream<HistorianKey, HistorianValue>(list);

            DebugStopwatch sw = new DebugStopwatch();

            double sec = sw.TimeEvent(() =>
            {
                SequentialReaderStream<HistorianKey, HistorianValue> scanner = sequencer;
                while (scanner.Read(key,value))
                {
                }
            });
            System.Console.WriteLine(Max / sec / 1000000);
            list.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void BenchmarkRealisticSamples()
        {

            MemoryPoolTest.TestMemoryLeak();
            const int Max = 1000000;
            const int FileCount = 1000;
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>(null);
            DateTime start = DateTime.Now.Date;
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            for (int x = 0; x < FileCount; x++)
            {
                SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
                AddData(table1, start.AddMinutes(2 * x), new TimeSpan(TimeSpan.TicksPerSecond), 60, 100, 1, Max / 60 / FileCount);
                using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
                {
                    editor.Add(table1);
                }
            }

            SeekFilterBase<HistorianKey> filter = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(start, start.AddMinutes(2 * FileCount), new TimeSpan(TimeSpan.TicksPerSecond * 2), new TimeSpan(TimeSpan.TicksPerMillisecond));
            SequentialReaderStream<HistorianKey, HistorianValue> sequencer = new SequentialReaderStream<HistorianKey, HistorianValue>(list, null, filter);

            DebugStopwatch sw = new DebugStopwatch();
            int xi = 0;
            double sec = sw.TimeEvent(() =>
            {
                SequentialReaderStream<HistorianKey, HistorianValue> scanner = sequencer;
                while (scanner.Read(key,value))
                {
                    xi++;
                }
            });
            System.Console.WriteLine(Max / sec / 1000000);
            //TreeKeyMethodsBase<HistorianKey>.WriteToConsole();
            //TreeValueMethodsBase<HistorianValue>.WriteToConsole();

            //Console.WriteLine("KeyMethodsBase calls");
            //for (int x = 0; x < 23; x++)
            //{
            //    Console.WriteLine(TreeKeyMethodsBase<HistorianKey>.CallMethods[x] + "\t" + ((TreeKeyMethodsBase<HistorianKey>.Method)(x)).ToString());
            //}
            //Console.WriteLine("ValueMethodsBase calls");
            //for (int x = 0; x < 5; x++)
            //{
            //    Console.WriteLine(TreeValueMethodsBase<HistorianValue>.CallMethods[x] + "\t" + ((TreeValueMethodsBase<HistorianValue>.Method)(x)).ToString());
            //}
            list.Dispose();

            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void ConsoleTest1()
        {
            MemoryPoolTest.TestMemoryLeak();
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>(null);
            DateTime start = DateTime.Now.Date;

            for (int x = 0; x < 3; x++)
            {
                SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
                AddDataTerminal(table1, (ulong)x, start, new TimeSpan(TimeSpan.TicksPerSecond), (ulong)(1000 * x), 1, 60 * 60);
                using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
                {
                    editor.Add(table1);
                }
            }

            SeekFilterBase<HistorianKey> filter = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(start, start.AddMinutes(10), new TimeSpan(TimeSpan.TicksPerSecond * 1), new TimeSpan(TimeSpan.TicksPerMillisecond));
            SequentialReaderStream<HistorianKey, HistorianValue> sequencer = new SequentialReaderStream<HistorianKey, HistorianValue>(list, null, filter);
            SortedList<DateTime, FrameData> frames = sequencer.GetFrames();
            WriteToConsole(frames);
            list.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void ConsoleTest2()
        {
            MemoryPoolTest.TestMemoryLeak();
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>(null);
            DateTime start = DateTime.Now.Date;

            for (int x = 0; x < 3; x++)
            {
                SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
                AddDataTerminal(table1, (ulong)x, start, new TimeSpan(TimeSpan.TicksPerSecond), (ulong)(1000 * x), 1, 60 * 60);
                using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
                {
                    editor.Add(table1);
                }
            }

            SeekFilterBase<HistorianKey> filter = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(start.AddMinutes(-100), start.AddMinutes(10), new TimeSpan(TimeSpan.TicksPerSecond * 60), new TimeSpan(TimeSpan.TicksPerSecond));
            SequentialReaderStream<HistorianKey, HistorianValue> sequencer = new SequentialReaderStream<HistorianKey, HistorianValue>(list, null, filter);
            SortedList<DateTime, FrameData> frames = sequencer.GetFrames();
            WriteToConsole(frames);
            list.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void ConsoleTest3()
        {
            MemoryPoolTest.TestMemoryLeak();
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>(null);
            DateTime start = DateTime.Now.Date;

            for (int x = 0; x < 3; x++)
            {
                SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
                AddDataTerminal(table1, (ulong)x, start, new TimeSpan(TimeSpan.TicksPerSecond), (ulong)(1000 * x), 1, 60 * 60);
                using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
                {
                    editor.Add(table1);
                }
            }
            for (int x = 0; x < 3; x++)
            {
                SortedTreeTable<HistorianKey, HistorianValue> table1 = CreateTable();
                AddDataTerminal(table1, (ulong)x, start, new TimeSpan(TimeSpan.TicksPerSecond), (ulong)(1000 * x), 1, 60 * 60);
                using (ArchiveListEditor<HistorianKey, HistorianValue> editor = list.AcquireEditLock())
                {
                    editor.Add(table1);
                }
            }

            SeekFilterBase<HistorianKey> filter = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(start, start.AddMinutes(10), new TimeSpan(TimeSpan.TicksPerSecond * 60), new TimeSpan(TimeSpan.TicksPerSecond));
            SequentialReaderStream<HistorianKey, HistorianValue> sequencer = new SequentialReaderStream<HistorianKey, HistorianValue>(list, null, filter);
            SortedList<DateTime, FrameData> frames = sequencer.GetFrames();
            WriteToConsole(frames);
            list.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }

        void WriteToConsole(SortedList<DateTime, FrameData> frames)
        {
            StringBuilder sb = new StringBuilder();

            ulong?[] data = new ulong?[10];

            foreach (KeyValuePair<DateTime, FrameData> frame in frames)
            {
                Array.Clear(data, 0, data.Length);
                foreach (KeyValuePair<ulong, HistorianValueStruct> sample in frame.Value.Points)
                {
                    data[sample.Key] = sample.Value.Value1;
                }

                sb.Append(frame.Key.ToString());
                sb.Append('\t');
                foreach (ulong? value in data)
                {
                    if (value.HasValue)
                    {
                        sb.Append(value.Value);
                    }
                    sb.Append('\t');
                }

                System.Console.WriteLine(sb.ToString());
                sb.Clear();
            }
        }


    }
}
