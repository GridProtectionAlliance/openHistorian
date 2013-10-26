using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace openHistorian.Engine
{
    [TestFixture]
    public class ArchiveReaderSequential_Test
    {
        [Test]
        public void TestOneFile()
        {
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>();

            var master = CreateTable();
            var table1 = CreateTable();
            AddData(master, 100, 100, 100);
            AddData(table1, 100, 100, 100);
            using (var editor = list.AcquireEditLock())
            {
                editor.Add(table1, false);
            }

            using (var masterRead = master.BeginRead())
            {
                var masterScan = masterRead.GetTreeScanner();
                masterScan.SeekToStart();
                var masterScanSequential = masterScan.TestSequential();

                var sequencer = new ArchiveReaderSequential<HistorianKey, HistorianValue>(list);
                var scanner = sequencer.Read().TestSequential();

                while (scanner.Read())
                {
                    if (!masterScanSequential.Read())
                        throw new Exception();

                    if (!scanner.CurrentKey.IsEqualTo(masterScan.CurrentKey))
                        throw new Exception();

                    if (!scanner.CurrentValue.IsEqualTo(masterScan.CurrentValue))
                        throw new Exception();
                }
                if (masterScan.Read())
                    throw new Exception();

            }
        }

        [Test]
        public void TestTwoFiles()
        {
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>();

            var master = CreateTable();
            var table1 = CreateTable();
            var table2 = CreateTable();
            AddData(master, 100, 100, 100);
            AddData(table1, 100, 100, 100);
            AddData(master, 101, 100, 100);
            AddData(table2, 101, 100, 100);
            using (var editor = list.AcquireEditLock())
            {
                editor.Add(table1, false);
                editor.Add(table2, false);
            }

            using (var masterRead = master.BeginRead())
            {
                var masterScan = masterRead.GetTreeScanner();
                masterScan.SeekToStart();
                var masterScanSequential = masterScan.TestSequential();

                var sequencer = new ArchiveReaderSequential<HistorianKey, HistorianValue>(list);
                var scanner = sequencer.Read().TestSequential();

                while (scanner.Read())
                {
                    if (!masterScanSequential.Read())
                        throw new Exception();

                    if (!scanner.CurrentKey.IsEqualTo(masterScan.CurrentKey))
                        throw new Exception();

                    if (!scanner.CurrentValue.IsEqualTo(masterScan.CurrentValue))
                        throw new Exception();
                }
                if (masterScan.Read())
                    throw new Exception();
            }
        }

        [Test]
        public void TestTwoIdenticalFiles()
        {
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>();

            var master = CreateTable();
            var table1 = CreateTable();
            var table2 = CreateTable();
            AddData(master, 100, 100, 100);
            AddData(table1, 100, 100, 100);
            AddData(table2, 100, 100, 100);
            using (var editor = list.AcquireEditLock())
            {
                editor.Add(table1, false);
                editor.Add(table2, false);
            }

            using (var masterRead = master.BeginRead())
            {
                var masterScan = masterRead.GetTreeScanner();
                masterScan.SeekToStart();
                var masterScanSequential = masterScan.TestSequential();

                var sequencer = new ArchiveReaderSequential<HistorianKey, HistorianValue>(list);
                var scanner = sequencer.Read().TestSequential();

                while (scanner.Read())
                {
                    if (!masterScanSequential.Read())
                        throw new Exception();

                    if (!scanner.CurrentKey.IsEqualTo(masterScan.CurrentKey))
                        throw new Exception();

                    if (!scanner.CurrentValue.IsEqualTo(masterScan.CurrentValue))
                        throw new Exception();
                }
                if (masterScan.Read())
                    throw new Exception();
            }
        }


        ArchiveTable<HistorianKey, HistorianValue> CreateTable()
        {
            var file = ArchiveFile.CreateInMemory();
            var table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(CreateFixedSizeNode.TypeGuid);
            return table;
        }

        void AddData(ArchiveTable<HistorianKey, HistorianValue> table, ulong start, ulong step, ulong count)
        {
            using (var edit = table.BeginEdit())
            {
                var key = new HistorianKey();
                var value = new HistorianValue();

                for (ulong v = start; v < start + step * count; v += step)
                {
                    key.SetMin();
                    key.PointID = v;
                    edit.AddPoint(key, value);
                }
                edit.Commit();
            }
        }
        void AddData(ArchiveTable<HistorianKey, HistorianValue> table, DateTime startTime, TimeSpan stepTime, int countTime, ulong startPoint, ulong stepPoint, ulong countPoint)
        {
            using (var edit = table.BeginEdit())
            {
                var key = new HistorianKey();
                var value = new HistorianValue();
                key.SetMin();
                var stepTimeTicks = (ulong)stepTime.Ticks;
                var stopTime = (ulong)(startTime.Ticks + countTime * stepTime.Ticks);
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

        [Test]
        public void BenchmarkRawFile()
        {
            const int Max = 1000000;

            var master = CreateTable();
            AddData(master, 100, 100, Max);


            DebugStopwatch sw = new DebugStopwatch();
            using (var masterRead = master.BeginRead())
            {
                double sec = sw.TimeEvent(() =>
                    {
                        var scanner = masterRead.GetTreeScanner();
                        scanner.SeekToStart();
                        while (scanner.Read())
                        {
                        }
                    });
                Console.WriteLine(Max / sec / 1000000);

            }
        }

        [Test]
        public void BenchmarkOneFile()
        {
            const int Max = 1000000;
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>();

            var table1 = CreateTable();
            AddData(table1, 100, 100, Max);
            using (var editor = list.AcquireEditLock())
            {
                editor.Add(table1, false);
            }

            var sequencer = new ArchiveReaderSequential<HistorianKey, HistorianValue>(list);

            DebugStopwatch sw = new DebugStopwatch();

            double sec = sw.TimeEvent(() =>
                {
                    var scanner = sequencer.Read();
                    while (scanner.Read())
                    {
                    }
                });
            Console.WriteLine(Max / sec / 1000000);
        }

        [Test]
        public void BenchmarkTwoFiles()
        {
            const int Max = 1000000;
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>();

            var table1 = CreateTable();
            var table2 = CreateTable();
            AddData(table1, 100, 100, Max / 2);
            AddData(table2, 101, 100, Max / 2);
            using (var editor = list.AcquireEditLock())
            {
                editor.Add(table1, false);
                editor.Add(table2, false);
            }

            var sequencer = new ArchiveReaderSequential<HistorianKey, HistorianValue>(list);

            DebugStopwatch sw = new DebugStopwatch();

            double sec = sw.TimeEvent(() =>
            {
                var scanner = sequencer.Read();
                while (scanner.Read())
                {
                }
            });
            Console.WriteLine(Max / sec / 1000000);
        }

        [Test]
        public void BenchmarkThreeFiles()
        {
            const int Max = 1000000;
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>();

            var table1 = CreateTable();
            var table2 = CreateTable();
            var table3 = CreateTable();
            AddData(table1, 100, 100, Max / 3);
            AddData(table2, 101, 100, Max / 3);
            AddData(table3, 102, 100, Max / 3);
            using (var editor = list.AcquireEditLock())
            {
                editor.Add(table1, false);
                editor.Add(table2, false);
                editor.Add(table3, false);
            }

            var sequencer = new ArchiveReaderSequential<HistorianKey, HistorianValue>(list);

            DebugStopwatch sw = new DebugStopwatch();

            double sec = sw.TimeEvent(() =>
            {
                var scanner = sequencer.Read();
                while (scanner.Read())
                {
                }
            });
            Console.WriteLine(Max / sec / 1000000);
        }

        [Test]
        public void BenchmarkRealisticSamples()
        {

            const int Max = 1000000;
            ArchiveList<HistorianKey, HistorianValue> list = new ArchiveList<HistorianKey, HistorianValue>();
            DateTime start = DateTime.Now.Date;

            for (int x = 0; x < 1000; x++)
            {
                var table1 = CreateTable();
                AddData(table1, start.AddMinutes(2 * x), new TimeSpan(TimeSpan.TicksPerSecond), 60, 100, 1, Max / 60 / 1000);
                using (var editor = list.AcquireEditLock())
                {
                    editor.Add(table1, false);
                }
            }
            
            var sequencer = new ArchiveReaderSequential<HistorianKey, HistorianValue>(list);

            DebugStopwatch sw = new DebugStopwatch();

            double sec = sw.TimeEvent(() =>
            {
                var scanner = sequencer.Read();
                while (scanner.Read())
                {
                }
            });
            Console.WriteLine(Max / sec / 1000000);
        }

    }
}
