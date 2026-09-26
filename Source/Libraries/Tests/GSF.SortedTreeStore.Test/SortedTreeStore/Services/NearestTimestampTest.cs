//******************************************************************************************************
//  NearestTimestampTest.cs - Gbtc
//
//  Copyright © 2026, Grid Protection Alliance. All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using GSF.IO;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Configuration;
using GSF.Snap.Services.Reader;
using GSF.Snap.Services.Writer;
using GSF.Snap.Storage;
using openHistorian.Snap;
using openHistorian.Snap.Definitions;

namespace GSF.UnitTests.SortedTreeStore.Services
{
    [TestFixture]
    public class NearestTimestampTest
    {
        private sealed class Sample
        {
            public ulong Time;
            public ulong ID;
            public ulong Entry;
            public ulong Value;
            public override string ToString() => $"{Time}:{ID}:{Entry}:{Value}";
        }

        private static void AddArchive(ArchiveList<HistorianKey, HistorianValue> archives, IEnumerable<Sample> data, bool compressed)
        {
            SortedTreeFile file = SortedTreeFile.CreateInMemory();
            SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(
                compressed ? HistorianFileEncodingDefinition.TypeGuid : EncodingDefinition.FixedSizeCombinedEncoding);

            using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = table.BeginEdit())
            {
                foreach (Sample sample in data.OrderBy(s => s.Time).ThenBy(s => s.ID).ThenBy(s => s.Entry))
                    edit.AddPoint(new HistorianKey { Timestamp = sample.Time, PointID = sample.ID, EntryNumber = sample.Entry },
                        new HistorianValue { Value1 = sample.Value, Value2 = sample.Time, Value3 = sample.ID });
                edit.Commit();
            }

            using (ArchiveListEditor<HistorianKey, HistorianValue> editor = archives.AcquireEditLock())
                editor.Add(table);
        }

        private static List<Sample> Read(ArchiveList<HistorianKey, HistorianValue> archives,
            SeekFilterBase<HistorianKey> filter, MatchFilterBase<HistorianKey, HistorianValue> match = null,
            SortedTreeEngineReaderOptions options = null, bool verifyGeneratedPayload = true)
        {
            List<Sample> result = new List<Sample>();
            using SequentialReaderStream<HistorianKey, HistorianValue> reader = new SequentialReaderStream<HistorianKey, HistorianValue>(archives, options, filter, match);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            while (reader.Read(key, value))
            {
                if (verifyGeneratedPayload) Assert.AreEqual(key.Timestamp, value.Value2, "Timestamp/value pairing");
                if (verifyGeneratedPayload) Assert.AreEqual(key.PointID, value.Value3, "Quality/value pairing");
                result.Add(new Sample { Time = key.Timestamp, ID = key.PointID, Entry = key.EntryNumber, Value = value.Value1 });
            }

            return result;
        }

        // Independent full-resolution oracle: assign each sample to its closest grid target
        // (shared midpoint goes to earlier target), then choose the nearest sample per ID.
        private static List<Sample> Expected(List<Sample> samples, ulong first, ulong last, ulong step, ulong radius, ulong[] ids, bool atOrAfter = false)
        {
            List<Sample> result = new List<Sample>();

            for (ulong target = first; target <= last; target += step)
            {
                foreach (ulong id in ids.Distinct())
                {
                    Sample sample = samples.Where(s => s.ID == id && s.Time >= first && s.Time <= last)
                        .Where(s => !atOrAfter || s.Time >= target)
                        .Where(s => (s.Time >= target ? s.Time - target : target - s.Time) <= radius)
                        .Where(s => target == first || s.Time > target - step / 2 || step % 2 != 0)
                        .OrderBy(s => s.Time >= target ? s.Time - target : target - s.Time)
                        .ThenBy(s => s.Time).ThenBy(s => s.Entry).FirstOrDefault();

                    if (sample != null)
                        result.Add(sample);
                }

                if (last - target < step)
                    break;
            }

            return result.OrderBy(s => s.Time).ThenBy(s => s.ID).ThenBy(s => s.Entry).ToList();
        }

        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void SparseOverlappingArchivesMatchFullResolutionOracle(bool compressed, bool atOrAfter)
        {
            Random random = new Random(47291);
            List<Sample> samples = new List<Sample>();

            for (ulong t = 7; t < 20000; t += (ulong)random.Next(1, 14))
            {
                for (ulong id = 1; id <= 6; id++)
                {
                    if (random.Next(4) != 0 && !(id == 3 && t > 5000 && t < 15000))
                        samples.Add(new Sample { Time = t, ID = id, Value = t * 10 + id });
                }
            }

            samples.Add(new Sample { Time = 9950, ID = 8, Value = 11 });
            samples.Add(new Sample { Time = 10050, ID = 8, Value = 12 });
            samples.Add(new Sample { Time = 9950, ID = 8, Entry = 1, Value = 13 });

            ulong[] ids = { 1, 2, 3, 6, 8, 999, 1 };

            using ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>();

            AddArchive(archives, samples.Where((s, i) => i % 2 == 0), compressed);
            AddArchive(archives, samples.Where((s, i) => i % 2 != 0 || i % 11 == 0), compressed);

            NearestTimestampSeekFilter<HistorianKey> filter = new NearestTimestampSeekFilter<HistorianKey>(0, 20000, 100, ids,
                searchMode: atOrAfter ? TimestampSearchMode.AtOrAfter : TimestampSearchMode.Nearest);

            List<Sample> expected = Expected(samples, 0, 20000, 100, 50, ids, atOrAfter);
            CollectionAssert.AreEqual(expected.Select(s => s.ToString()).ToArray(), Read(archives, filter).Select(s => s.ToString()).ToArray());

            // A filter can be reused and a scanner can seek again after reaching file boundaries.
            CollectionAssert.AreEqual(expected.Select(s => s.ToString()).ToArray(), Read(archives, filter).Select(s => s.ToString()).ToArray());
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ExactMatchesUseOneSeekPerTargetAndStillResolveArchiveTies(bool compressed)
        {
            using ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>();
            List<Sample> data = new List<Sample>();

            for (ulong time = 0; time <= 20; time += 10)
                for (ulong id = 1; id <= 2; id++)
                    data.Add(new Sample { Time = time, ID = id, Entry = 1, Value = time + id });

            AddArchive(archives, data, compressed);
            NearestTimestampSeekFilter<HistorianKey> filter = new NearestTimestampSeekFilter<HistorianKey>(0, 20, 10, new ulong[] { 1, 2 });

            long seeks = Stats.SeeksRequested;

            Assert.AreEqual(6, Read(archives, filter).Count);
            Assert.AreEqual(3L, Stats.SeeksRequested - seeks, "Exact timestamps must not trigger backward seeks.");

            AddArchive(archives, data.Select(s => new Sample { Time = s.Time, ID = s.ID, Entry = 0, Value = s.Value + 100 }), compressed);

            List<Sample> result = Read(archives, filter);

            Assert.AreEqual(6, result.Count);
            Assert.IsTrue(result.All(s => s.Entry == 0 && s.Value >= 100), "Other archives still participate after an exact match.");
        }

        [Test]
        public void PartialExactMatchesStillSearchBackwardsAndForwardOnlyDoesNot()
        {
            using ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>();

            AddArchive(archives, new[] {
                new Sample { Time = 90, ID = 1 }, new Sample { Time = 110, ID = 1 }, new Sample { Time = 130, ID = 1 },
                new Sample { Time = 109, ID = 2 }, new Sample { Time = 112, ID = 2 }, new Sample { Time = 129, ID = 2 }
            }, true);

            NearestTimestampSeekFilter<HistorianKey> nearest = new NearestTimestampSeekFilter<HistorianKey>(90, 130, 20, new ulong[] { 1, 2 });

            CollectionAssert.AreEqual(new ulong[] { 109, 129 }, Read(archives, nearest).Where(s => s.ID == 2).Select(s => s.Time).ToArray());

            NearestTimestampSeekFilter<HistorianKey> forward = new NearestTimestampSeekFilter<HistorianKey>(90, 130, 20, new ulong[] { 1, 2 }, searchMode: TimestampSearchMode.AtOrAfter);
            long seeks = Stats.SeeksRequested;

            CollectionAssert.AreEqual(new ulong[] { 112 }, Read(archives, forward).Where(s => s.ID == 2).Select(s => s.Time).ToArray());
            Assert.AreEqual(3L, Stats.SeeksRequested - seeks, "Forward-only performs no backward seeks, even when IDs are missing.");
            Assert.AreEqual(0, Read(archives, TimestampPointIDSeekFilter.FindNearestKey<HistorianKey>(130, 2, 10, TimestampSearchMode.AtOrAfter)).Count);
            Assert.AreEqual(0, Read(archives, TimestampPointIDSeekFilter.FindNearestKey<HistorianKey>(110, 2, 1, TimestampSearchMode.AtOrAfter)).Count);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void SearchModeRoundTripsWithoutChangingVersionOneNearestPayloads(bool singleTarget)
        {
            foreach (TimestampSearchMode mode in new[] { TimestampSearchMode.Nearest, TimestampSearchMode.AtOrAfter })
            {
                NearestTimestampSeekFilter<HistorianKey> filter = singleTarget
                    ? new NearestTimestampSeekFilter<HistorianKey>(ulong.MaxValue - 10, 7, 15, mode)
                    : new NearestTimestampSeekFilter<HistorianKey>(ulong.MaxValue - 20, ulong.MaxValue, 10, new ulong[] { 7 }, searchMode: mode);
                using MemoryStream memory = new MemoryStream();
                using BinaryStreamWrapper stream = new BinaryStreamWrapper(memory, false);

                filter.Save(stream);

                byte[] bytes = memory.ToArray();

                Assert.AreEqual(mode == TimestampSearchMode.Nearest ? 1 : 2, bytes[0]);

                if (mode == TimestampSearchMode.Nearest)
                {
                    Assert.AreEqual(singleTarget ? 26 : 46, bytes.Length, "Version-one layout remains unchanged.");
                    Assert.AreEqual(singleTarget ? ulong.MaxValue - 10 : ulong.MaxValue - 20, BitConverter.ToUInt64(bytes, 2));
                }

                memory.Position = 0;
                NearestTimestampSeekFilter<HistorianKey> copy = (NearestTimestampSeekFilter<HistorianKey>)Library.Filters.GetSeekFilter<HistorianKey>(filter.FilterType, stream);

                Assert.AreEqual(mode, copy.SearchMode);

                while (filter.NextWindow())
                {
                    Assert.IsTrue(copy.NextWindow());
                    Assert.AreEqual(filter.TargetTimestamp, copy.TargetTimestamp);
                    Assert.AreEqual(filter.StartOfFrame.Timestamp, copy.StartOfFrame.Timestamp);
                    Assert.AreEqual(filter.EndOfFrame.Timestamp, copy.EndOfFrame.Timestamp);

                    if (mode == TimestampSearchMode.AtOrAfter)
                        Assert.AreEqual(copy.TargetTimestamp, copy.StartOfFrame.Timestamp);
                }

                Assert.IsFalse(copy.NextWindow());

                copy.Reset();

                Assert.IsTrue(copy.NextWindow());
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => new NearestTimestampSeekFilter<HistorianKey>(0, 10, 10, new ulong[] { 1 }, searchMode: (TimestampSearchMode)99));

            using (MemoryStream memory = new MemoryStream(new byte[] { 2, 0, 99 }))
            using (BinaryStreamWrapper stream = new BinaryStreamWrapper(memory, false))
                Assert.Throws<InvalidDataException>(() => NearestTimestampSeekFilter<HistorianKey>.Load(stream));
        }
        [Test]
        public void BoundariesTiesAndDuplicateEntries()
        {
            List<Sample> data = new List<Sample>
            {
                new Sample { Time = 0, ID = 1, Value = 10 },
                new Sample { Time = 9, ID = 1, Value = 11 },
                new Sample { Time = 11, ID = 1, Value = 12 },
                new Sample { Time = 9, ID = 1, Entry = 1, Value = 13 },
                new Sample { Time = 15, ID = 2, Value = 14 },
                new Sample { Time = 20, ID = 1, Value = 15 }
            };

            using ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>();

            AddArchive(archives, data, true);

            List<Sample> actual = Read(archives, new NearestTimestampSeekFilter<HistorianKey>(0, 20, 10, new ulong[] { 1, 2 }));
            CollectionAssert.AreEqual(new ulong[] { 0, 9, 15, 20 }, actual.Select(s => s.Time).ToArray());

            Assert.AreEqual(0UL, actual[1].Entry);
            Assert.AreEqual(11UL, actual[1].Value);
            Assert.AreEqual(3, Read(archives, new NearestTimestampSeekFilter<HistorianKey>(0, 20, 10, new ulong[] { 1, 2 }), new OnlyIDOne()).Count);
            Assert.AreEqual(2, Read(archives, new NearestTimestampSeekFilter<HistorianKey>(0, 20, 10, new ulong[] { 1 }, 0)).Count);
            Assert.AreEqual(0, Read(archives, new NearestTimestampSeekFilter<HistorianKey>(0, 20, 10, new ulong[0])).Count);
            Assert.AreEqual(0, Read(archives, new NearestTimestampSeekFilter<HistorianKey>(30, 40, 10, new ulong[] { 1 })).Count);
        }

        private sealed class OnlyIDOne : MatchFilterBase<HistorianKey, HistorianValue>
        {
            public override Guid FilterType => Guid.Empty;
            public override bool Contains(HistorianKey key, HistorianValue value) => key.PointID == 1;
            public override void Save(BinaryStreamBase stream) { throw new NotSupportedException(); }
        }

        [Test]
        public void SerializationAndUnsignedLimits()
        {
            NearestTimestampSeekFilter<HistorianKey> filter = new NearestTimestampSeekFilter<HistorianKey>(ulong.MaxValue - 20, ulong.MaxValue, 10, new ulong[] { 2, 1, 1 });

            using (MemoryStream memory = new MemoryStream())
            using (BinaryStreamWrapper stream = new BinaryStreamWrapper(memory, false))
            {
                filter.Save(stream);
                memory.Position = 0;

                SeekFilterBase<HistorianKey> copy = new NearestTimestampSeekFilterDefinition().Create<HistorianKey>(stream);

                for (int i = 0; i < 3; i++)
                {
                    Assert.IsTrue(filter.NextWindow());
                    Assert.IsTrue(copy.NextWindow());
                    Assert.AreEqual(filter.StartOfFrame.Timestamp, copy.StartOfFrame.Timestamp);
                    Assert.AreEqual(filter.EndOfFrame.Timestamp, copy.EndOfFrame.Timestamp);
                }

                Assert.IsFalse(copy.NextWindow());
                Assert.IsFalse(copy.NextWindow());

                copy.Reset();

                Assert.IsTrue(copy.NextWindow());
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => new NearestTimestampSeekFilter<HistorianKey>(0, 1, 0, new ulong[] { 1 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => new NearestTimestampSeekFilter<HistorianKey>(1, 0, 1, new ulong[] { 1 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => new NearestTimestampSeekFilter<HistorianKey>(0, 10, 10, new ulong[] { 1 }, 6));
        }

        [Test]
        public void LimitsAndCancellationDoNotEmitPartiallySearchedWindows()
        {
            using ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>();

            AddArchive(archives, Enumerable.Range(0, 1000).Select(i => new Sample { Time = (ulong)i, ID = 1 }), false);

            NearestTimestampSeekFilter<HistorianKey> filter() => new NearestTimestampSeekFilter<HistorianKey>(0, 1000, 100, new ulong[] { 1 });

            Assert.AreEqual(2, Read(archives, filter(), null, new SortedTreeEngineReaderOptions(maxReturnedCount: 2)).Count);
            Assert.AreEqual(0, Read(archives, filter(), null, new SortedTreeEngineReaderOptions(maxScanCount: 1)).Count);
            Assert.AreEqual(1, Read(archives, filter(), null, new SortedTreeEngineReaderOptions(maxSeekCount: 1)).Count);
            Assert.AreEqual(0, Read(archives, TimestampPointIDSeekFilter.FindNearestKey<HistorianKey>(1001, 1, 10),
                null, new SortedTreeEngineReaderOptions(maxSeekCount: 1)).Count);

            using SequentialReaderStream<HistorianKey, HistorianValue> reader = new SequentialReaderStream<HistorianKey, HistorianValue>(archives, null, filter());

            reader.CancelReader();

            Assert.IsFalse(reader.Read(new HistorianKey(), new HistorianValue()));
        }

        [Test]
        public void SingleTargetSearchesBeforeAndAfterAndClampsUnsignedLimits()
        {
            using ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>();

            AddArchive(archives, new[] {
                new Sample { Time = 90, ID = 1, Value = 1 },
                new Sample { Time = 108, ID = 1, Value = 2 },
                new Sample { Time = 112, ID = 1, Value = 3 },
                new Sample { Time = ulong.MaxValue - 1, ID = 2, Value = 4 }
            }, false);

            Assert.AreEqual(90UL, Read(archives, TimestampPointIDSeekFilter.FindNearestKey<HistorianKey>(95, 1, 10)).Single().Time);
            Assert.AreEqual(108UL, Read(archives, TimestampPointIDSeekFilter.FindNearestKey<HistorianKey>(100, 1, 10)).Single().Time);
            Assert.AreEqual(108UL, Read(archives, TimestampPointIDSeekFilter.FindNearestKey<HistorianKey>(110, 1, 10)).Single().Time);
            Assert.AreEqual(0, Read(archives, TimestampPointIDSeekFilter.FindNearestKey<HistorianKey>(100, 1, 7)).Count);
            Assert.AreEqual(0, Read(archives, TimestampPointIDSeekFilter.FindNearestKey<HistorianKey>(0, 1, 10)).Count);
            Assert.AreEqual(ulong.MaxValue - 1, Read(archives, TimestampPointIDSeekFilter.FindNearestKey<HistorianKey>(ulong.MaxValue, 2, ulong.MaxValue)).Single().Time);

            using MemoryStream memory = new MemoryStream();
            using BinaryStreamWrapper stream = new BinaryStreamWrapper(memory, false);

            TimestampPointIDSeekFilter.FindNearestKey<HistorianKey>(95, 1, 10).Save(stream);

            memory.Position = 0;

            SeekFilterBase<HistorianKey> restored = Library.Filters.GetSeekFilter<HistorianKey>(NearestTimestampSeekFilterDefinition.FilterGuid, stream);

            Assert.AreEqual(90UL, Read(archives, restored).Single().Time);
        }

        [Test]
        [Category("ArchiveIntegration")]
        public void OptionalReadOnlyArchiveMatchesFullResolution()
        {
            string path = Environment.GetEnvironmentVariable("SNAPDB_TEST_ARCHIVE");
            string selection = Environment.GetEnvironmentVariable("SNAPDB_TEST_POINTIDS");

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(selection))
                Assert.Ignore("Set SNAPDB_TEST_ARCHIVE and SNAPDB_TEST_POINTIDS for a read-only archive comparison.");

            ulong[] ids = selection.Split(',').Select(ulong.Parse).ToArray();

            using SortedTreeFile file = SortedTreeFile.OpenFile(path, true);
            using ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>();

            SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>();
            ulong first = table.FirstKey.Timestamp;
            ulong last = table.LastKey.Timestamp;
            ulong interval = Math.Max(2UL, (last - first) / 60);

            using (ArchiveListEditor<HistorianKey, HistorianValue> editor = archives.AcquireEditLock())
                editor.Add(table);

            SelectedIDs match = new SelectedIDs(ids);
            Stopwatch timer = Stopwatch.StartNew();
            List<Sample> full = Read(archives, TimestampSeekFilter.CreateFromRange<HistorianKey>(first, last), match, null, false);

            timer.Stop();

            long fullMilliseconds = timer.ElapsedMilliseconds;

            Assert.Greater(full.Count, 0, "The selected archive must contain the specified measurement IDs.");

            timer.Restart();

            List<Sample> nearest = Read(archives, new NearestTimestampSeekFilter<HistorianKey>(first, last, interval, ids), null, null, false);

            timer.Stop();

            CollectionAssert.AreEqual(Expected(full, first, last, interval, interval / 2, ids).Select(s => s.ToString()).ToArray(), nearest.Select(s => s.ToString()).ToArray());

            System.Console.WriteLine("Read-only archive: selected source={0}, returned={1}, full={2} ms, nearest={3} ms", full.Count, nearest.Count, fullMilliseconds, timer.ElapsedMilliseconds);

            ulong[] presentIDs = full.Select(s => s.ID).Distinct().ToArray();

            System.Console.WriteLine("Present IDs: " + string.Join(",", presentIDs));

            timer.Restart();
            List<Sample> present = Read(archives, new NearestTimestampSeekFilter<HistorianKey>(first, last, interval, presentIDs), null, null, false);

            timer.Stop();

            CollectionAssert.AreEqual(nearest.Select(s => s.ToString()).ToArray(), present.Select(s => s.ToString()).ToArray());

            System.Console.WriteLine("Read-only archive (present IDs only): {0} ms", timer.ElapsedMilliseconds);
        }



        private sealed class DiskSamples : TreeStream<HistorianKey, HistorianValue>
        {
            private readonly IEnumerator<Sample> m_samples;
            internal DiskSamples(IEnumerable<Sample> samples) { m_samples = samples.GetEnumerator(); }
            public override bool IsAlwaysSequential => true;
            public override bool NeverContainsDuplicates => true;

            protected override bool ReadNext(HistorianKey key, HistorianValue value)
            {
                if (!m_samples.MoveNext()) return false;
                Sample sample = m_samples.Current;
                key.Timestamp = sample.Time; key.PointID = sample.ID; key.EntryNumber = sample.Entry;
                value.Value1 = sample.Value; value.Value2 = sample.Time; value.Value3 = sample.ID;
                return true;
            }
            protected override void Dispose(bool disposing) { if (disposing) m_samples.Dispose(); base.Dispose(disposing); }
        }

        private static void WriteIndexedArchive(string path, IEnumerable<Sample> samples)
        {
            using DiskSamples stream = new DiskSamples(samples);
            SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.Create(path + ".pending", path, 4096, null, HistorianFileEncodingDefinition.TypeGuid, stream);
        }

        private static List<Sample> ReadDiskArchives(string[] paths, out long visits, SortedTreeEngineReaderOptions options = null)
        {
            using ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>();

            foreach (string path in paths)
            {
                SortedTreeFile file = SortedTreeFile.OpenFile(path, true);
                using ArchiveListEditor<HistorianKey, HistorianValue> editor = archives.AcquireEditLock();

                editor.Add(file.OpenTable<HistorianKey, HistorianValue>());
            }

            long before = Stats.PointsScanned;
            List<Sample> result = Read(archives, new NearestTimestampSeekFilter<HistorianKey>(0, 999, 100, new ulong[] { 1, 2, 999 }), null, options);

            visits = Stats.PointsScanned - before;

            return result;
        }

        [Test]
        public void FileIDIndexPrunesMixedRequestsAndFailsOpen()
        {
            string folder = Path.Combine(Path.GetTempPath(), "nearest-index-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            string first = Path.Combine(folder, "one.d2");
            string second = Path.Combine(folder, "two.d2i");

            try
            {
                WriteIndexedArchive(first, Enumerable.Range(0, 1000).Select(t => new Sample { Time = (ulong)t, ID = 1 }));
                WriteIndexedArchive(second, Enumerable.Range(0, 1000).Select(t => new Sample { Time = (ulong)t, ID = 2 }));

                Assert.IsTrue(File.Exists(first + ".pidx"));
                Assert.IsFalse(File.Exists(first + ".bidx"), "Bloom generation is opt-in.");

                byte[] index = File.ReadAllBytes(first + ".pidx");

                List<Sample> indexed = ReadDiskArchives(new[] { first, second }, out long indexedVisits);

                Assert.AreEqual(20, indexed.Count, "An ID absent in one file remains eligible in another.");

                List<Sample> bypass = ReadDiskArchives(new[] { first, second }, out long bypassVisits, new SortedTreeEngineReaderOptions(false, false));

                CollectionAssert.AreEqual(indexed.Select(sample => sample.ToString()).ToArray(), bypass.Select(sample => sample.ToString()).ToArray());
                Assert.Greater(bypassVisits, indexedVisits * 10);

                File.Delete(first + ".pidx");
                File.Delete(second + ".pidx");

                List<Sample> fallback = ReadDiskArchives(new[] { first, second }, out long fallbackVisits);

                CollectionAssert.AreEqual(fallback.Select(s => s.ToString()).ToArray(), indexed.Select(s => s.ToString()).ToArray());
                Assert.Less(indexedVisits * 10, fallbackVisits);

                System.Console.WriteLine("File presence index: indexed visits={0}, fallback visits={1}, one-ID sidecar={2} bytes", indexedVisits, fallbackVisits, index.Length);

                // A valid index belonging to a different archive cannot exclude that file's IDs.
                File.WriteAllBytes(second + ".pidx", index);

                CollectionAssert.AreEqual(fallback.Select(s => s.ToString()).ToArray(), ReadDiskArchives(new[] { first, second }, out fallbackVisits).Select(s => s.ToString()).ToArray());

                index[0] ^= 1; // Digest corruption must fall back, never return false negatives.
                File.WriteAllBytes(first + ".pidx", index);

                CollectionAssert.AreEqual(fallback.Select(s => s.ToString()).ToArray(), ReadDiskArchives(new[] { first, second }, out fallbackVisits).Select(s => s.ToString()).ToArray());

                File.WriteAllBytes(first + ".pidx", Enumerable.Repeat((byte)255, 128).ToArray());

                Assert.AreEqual(20, ReadDiskArchives(new[] { first, second }, out fallbackVisits).Count);

                File.WriteAllBytes(first + ".pidx", new byte[] { 1, 2, 3 });

                Assert.AreEqual(20, ReadDiskArchives(new[] { first, second }, out fallbackVisits).Count);
            }
            finally
            {
                foreach (string path in Directory.GetFiles(folder)) File.Delete(path);
                Directory.Delete(folder);
            }
        }

        [Test]
        public void FileIDIndexValidatesSnapshotAndFollowsArchiveLifecycle()
        {
            string folder = Path.Combine(Path.GetTempPath(), "nearest-index-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, "one.d2i");

            try
            {
                WriteIndexedArchive(path, Enumerable.Range(0, 10000).Select(id => new Sample { Time = 1, ID = (ulong)id }));

                using (SortedTreeFile file = SortedTreeFile.OpenFile(path, true))
                {
                    Guid archive = file.Snapshot.Header.ArchiveId;
                    uint sequence = file.Snapshot.Header.SnapshotSequenceNumber;
                    Guid key = new HistorianKey().GenericTypeGuid;
                    Guid value = new HistorianValue().GenericTypeGuid;

                    Assert.AreEqual(10000, ArchivePointIDIndex.Load(path, archive, sequence, key, value).Length);
                    Assert.IsNull(ArchivePointIDIndex.Load(path, archive, sequence + 1, key, value));
                    Assert.IsNull(ArchivePointIDIndex.Load(path, archive, sequence, Guid.NewGuid(), value));

                    long bytes = new FileInfo(path + ".pidx").Length;

                    Assert.Less(bytes, 80000);

                    System.Console.WriteLine("10,000-ID compressed sidecar: {0} bytes", bytes);

                    file.ChangeExtension(".d2", true, true);

                    Assert.IsFalse(File.Exists(path + ".pidx"));
                    Assert.IsTrue(File.Exists(file.FilePath + ".pidx"));
                    Assert.AreEqual(10000, ArchivePointIDIndex.Load(file.FilePath, archive, sequence, key, value).Length);

                    path = file.FilePath;
                    file.Delete();

                    Assert.IsFalse(File.Exists(path));
                    Assert.IsFalse(File.Exists(path + ".pidx"));
                }

                // A sidecar publication conflict must not invalidate a successfully written archive.
                Directory.CreateDirectory(path + ".pidx");

                WriteIndexedArchive(path, new[] { new Sample { Time = 1, ID = 1 } });

                using (SortedTreeFile file = SortedTreeFile.OpenFile(path, true))
                    Assert.AreEqual(1UL, file.OpenTable<HistorianKey, HistorianValue>().FirstKey.PointID);

                Directory.Delete(path + ".pidx");
            }
            finally
            {
                foreach (string entry in Directory.GetFiles(folder)) File.Delete(entry);
                foreach (string entry in Directory.GetDirectories(folder)) Directory.Delete(entry);
                Directory.Delete(folder);
            }
        }


        [Test]
        public void BloomMembershipHasNoFalseNegativesAndBoundsFalsePositives()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeBucketBloomFilterSettings(true, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeBucketBloomFilterSettings(true, 900, double.NaN));

            ArchiveTimeBucketBloomIndex.Builder builder = new ArchiveTimeBucketBloomIndex.Builder(new TimeBucketBloomFilterSettings(true, 1));

            const ulong Second = TimeSpan.TicksPerSecond;

            for (ulong id = 0; id < 4000; id++)
                builder.Add(Second, id * 7919);

            builder.Add(ulong.MaxValue, ulong.MaxValue);

            ArchiveTimeBucketBloomIndex index = builder.Complete();

            for (ulong id = 0; id < 4000; id++)
                Assert.IsTrue(index.MayContain(id * 7919, Second, Second));

            Assert.IsTrue(index.MayContain(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue));
            Assert.IsFalse(index.MayContain(0, 0, Second - 1));
            Assert.IsFalse(index.MayContain(0, 2 * Second, 3 * Second));

            int positives = 0;

            for (ulong id = 1; id <= 10000; id++)
            {
                if (index.MayContain(id * 7919 + 1, Second, Second))
                    positives++;
            }

            Assert.Less(positives, 300, "False-positive performance regression (not a correctness guarantee).");

            System.Console.WriteLine("Bloom absent-ID probes: {0}/10000 false positives", positives);

            // A construction limit must drop the whole synopsis, not omit populated buckets.
            builder = new ArchiveTimeBucketBloomIndex.Builder(new TimeBucketBloomFilterSettings(true, 1));

            for (ulong bucket = 0; bucket <= ArchiveTimeBucketBloomIndex.MaximumBuckets; bucket++)
                builder.Add(bucket * Second, 1);

            Assert.IsNull(builder.Complete());
        }

        [TestCase(false)]
        [TestCase(true)]
        public void BloomSidecarPrunesInternalGapsAndFailsOpen(bool forwardOnly)
        {
            string folder = Path.Combine(Path.GetTempPath(), "nearest-bloom-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, "gap.d2");
            const ulong Second = TimeSpan.TicksPerSecond;

            try
            {
                List<Sample> samples = Enumerable.Range(0, 10000).Select(t => new Sample
                {
                    Time = (ulong)t * Second,
                    ID = 1
                }).ToList();

                samples.Add(new Sample
                {
                    Time = 0,
                    ID = 2
                });
                samples.Add(new Sample
                {
                    Time = 9999 * Second,
                    ID = 2
                });
                samples = samples.OrderBy(s => s.Time).ThenBy(s => s.ID).ToList();

                using (DiskSamples stream = new DiskSamples(samples))
                    SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.CreateWithBloomFilter(path + ".pending", path, 4096, null, HistorianFileEncodingDefinition.TypeGuid, stream, new TimeBucketBloomFilterSettings(true, 10));

                Assert.IsTrue(File.Exists(path + ".bidx"));

                byte[] bytes = File.ReadAllBytes(path + ".bidx");
                long[] visits = new long[6];

                for (int pass = 0; pass < 6; pass++)
                {
                    switch (pass)
                    {
                        case 1:
                            File.Delete(path + ".bidx");
                            break;
                        case 2:
                            File.WriteAllBytes(path + ".bidx", Enumerable.Repeat((byte)255, 128).ToArray());
                            break;
                        case 3:
                        case 4:
                            bytes[0] ^= 1;
                            File.WriteAllBytes(path + ".bidx", bytes);
                            break;
                    }

                    using SortedTreeFile file = SortedTreeFile.OpenFile(path, true);
                    using ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>();
                    using (ArchiveListEditor<HistorianKey, HistorianValue> editor = archives.AcquireEditLock())
                        editor.Add(file.OpenTable<HistorianKey, HistorianValue>());

                    long before = Stats.PointsScanned;

                    List<Sample> result = Read(archives, new NearestTimestampSeekFilter<HistorianKey>(5000 * Second, 5100 * Second, 100 * Second, new ulong[] { 1, 2 }, null, forwardOnly ? TimestampSearchMode.AtOrAfter : TimestampSearchMode.Nearest), null, new SortedTreeEngineReaderOptions(pass != 5, pass != 4));

                    visits[pass] = Stats.PointsScanned - before;

                    Assert.AreEqual(2, result.Count);
                    Assert.IsTrue(result.All(sample => sample.ID == 1));
                }

                Assert.Less(visits[0] * 10, visits[1]);
                Assert.AreEqual(visits[1], visits[2]);
                Assert.AreEqual(visits[1], visits[3]);
                Assert.AreEqual(visits[1], visits[4], "Bloom bypass uses ordinary scanning despite a valid sidecar.");
                Assert.AreEqual(visits[0], visits[5], "Bloom remains usable when exact-ID indexing is bypassed.");

                System.Console.WriteLine("Bloom internal gap ({0}): indexed visits={1}, fallback={2}, sidecar={3} bytes", forwardOnly, visits[0], visits[1], bytes.Length);
            }
            finally
            {
                foreach (string entry in Directory.GetFiles(folder))
                    File.Delete(entry);

                Directory.Delete(folder);
            }
        }

        [Test]
        public void BloomDiskOracleIdentityAndLifecycle()
        {
            string folder = Path.Combine(Path.GetTempPath(), "nearest-bloom-oracle-" + Guid.NewGuid().ToString("N"));

            Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, "oracle.d2i");
            const ulong Second = TimeSpan.TicksPerSecond;

            List<Sample> data = new List<Sample>();
            Random random = new Random(8182);

            for (ulong time = 0; time < 200 * Second; time += 3 * Second + 1)
            {
                for (ulong id = 0; id < 10; id++)
                {
                    if (random.Next(3) == 0)
                        data.Add(new Sample { Time = time, ID = id });
                }
            }

            data.Add(new Sample { Time = 200 * Second, ID = 9 });

            try
            {
                using (DiskSamples stream = new DiskSamples(data))
                    SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.CreateWithBloomFilter(path + ".pending", path, 4096, null, HistorianFileEncodingDefinition.TypeGuid, stream, new TimeBucketBloomFilterSettings(true, 10));

                using (SortedTreeFile file = SortedTreeFile.OpenFile(path, true))
                using (ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>())
                {
                    using (ArchiveListEditor<HistorianKey, HistorianValue> editor = archives.AcquireEditLock()) editor.Add(file.OpenTable<HistorianKey, HistorianValue>());
                    ulong[] ids = Enumerable.Range(0, 11).Select(id => (ulong)id).ToArray();

                    foreach (bool forward in new[] { false, true })
                    {
                        List<Sample> actual = Read(archives, new NearestTimestampSeekFilter<HistorianKey>(0, 200 * Second, 20 * Second, ids, null, forward ? TimestampSearchMode.AtOrAfter : TimestampSearchMode.Nearest));
                        CollectionAssert.AreEqual(Expected(data, 0, 200 * Second, 20 * Second, 10 * Second, ids, forward).Select(s => s.ToString()).ToArray(), actual.Select(s => s.ToString()).ToArray());
                    }

                    Guid archive = file.Snapshot.Header.ArchiveId;
                    uint seq = file.Snapshot.Header.SnapshotSequenceNumber;
                    Guid key = new HistorianKey().GenericTypeGuid, value = new HistorianValue().GenericTypeGuid;

                    Assert.IsNull(ArchiveTimeBucketBloomIndex.Load(path, Guid.NewGuid(), seq, key, value));
                    Assert.IsNull(ArchiveTimeBucketBloomIndex.Load(path, archive, seq + 1, key, value));
                    Assert.IsNull(ArchiveTimeBucketBloomIndex.Load(path, archive, seq, Guid.NewGuid(), value));

                    ArchiveTimeBucketBloomIndex index = ArchiveTimeBucketBloomIndex.Load(path, archive, seq, key, value);

                    foreach (Sample sample in data)
                        Assert.IsTrue(index.MayContain(sample.ID, sample.Time, sample.Time));
                }

                using (SortedTreeFile file = SortedTreeFile.OpenFile(path, true))
                {
                    file.ChangeExtension(".d2", true, true);
                    Assert.IsFalse(File.Exists(path + ".bidx"));

                    path = file.FilePath;
                    Assert.IsTrue(File.Exists(path + ".bidx"));

                    file.Delete();
                    Assert.IsFalse(File.Exists(path + ".bidx"));
                }
            }
            finally { foreach (string entry in Directory.GetFiles(folder)) File.Delete(entry); Directory.Delete(folder); }
        }

        [Test]
        public void BloomSettingsRoundTripAndReachEveryRollover()
        {
            AdvancedServerDatabaseConfig<HistorianKey, HistorianValue> config = new AdvancedServerDatabaseConfig<HistorianKey, HistorianValue>("test", Path.GetTempPath(), true)
            {
                BloomFilter = new TimeBucketBloomFilterSettings(true, 120, 0.001)
            };

            ServerDatabaseSettings server = config.ToServerDatabaseSettings();
            SimplifiedArchiveInitializerSettings settings = server.WriteProcessor.FirstStageWriter.FinalSettings;

            Assert.IsTrue(settings.BloomFilter.Enabled);

            foreach (CombineFilesSettings rollover in server.WriteProcessor.StagingRollovers)
                Assert.AreEqual(120, rollover.ArchiveSettings.BloomFilter.BucketDurationSeconds);

            using MemoryStream stream = new MemoryStream();

            settings.Save(stream);
            stream.Position = 0;

            SimplifiedArchiveInitializerSettings restored = new SimplifiedArchiveInitializerSettings();
            restored.Load(stream);

            Assert.IsTrue(restored.BloomFilter.Enabled);
            Assert.AreEqual(120, restored.BloomFilter.BucketDurationSeconds);
            Assert.AreEqual(0.001, restored.BloomFilter.FalsePositiveProbability);
            Assert.AreEqual(stream.Length, stream.Position);

            stream.SetLength(0);
            settings.BloomFilter = new TimeBucketBloomFilterSettings();
            settings.Save(stream);
            stream.Position = 0;
            restored.Load(stream);

            Assert.IsFalse(restored.BloomFilter.Enabled);
            Assert.AreEqual(stream.Length, stream.Position);
        }

        [Test]
        public void BloomSettingsPreserveLegacyVersionsAndDirectoryFillMode()
        {
            SimplifiedArchiveInitializerSettings settings = new SimplifiedArchiveInitializerSettings { FillMethod = ArchiveDirectoryFillMethod.RoundRobin };

            using MemoryStream stream = new MemoryStream();

            settings.Save(stream);

            byte[] versionTwo = stream.ToArray();
            Assert.AreEqual(2, versionTwo[0]);

            SimplifiedArchiveInitializerSettings loaded = new SimplifiedArchiveInitializerSettings();

            stream.Position = 0;
            loaded.Load(stream);

            Assert.AreEqual(ArchiveDirectoryFillMethod.RoundRobin, loaded.FillMethod);
            Assert.IsFalse(loaded.BloomFilter.Enabled);

            byte[] versionOne = versionTwo.Take(5).Concat(versionTwo.Skip(9)).ToArray();

            versionOne[0] = 1;

            using (MemoryStream old = new MemoryStream(versionOne)) loaded.Load(old);

            Assert.AreEqual(ArchiveDirectoryFillMethod.Sequential, loaded.FillMethod);
            Assert.IsFalse(loaded.BloomFilter.Enabled);

            stream.SetLength(0);

            settings.BloomFilter = new TimeBucketBloomFilterSettings(true);
            settings.Save(stream);

            Assert.AreEqual(3, stream.ToArray()[0]);
            stream.Position = 0;
            loaded.Load(stream);

            Assert.AreEqual(ArchiveDirectoryFillMethod.RoundRobin, loaded.FillMethod);
            Assert.IsTrue(loaded.BloomFilter.Enabled);
        }

        [Test]
        public void IndexControlsRoundTripAndBudgetIsEnforced()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeBucketBloomFilterSettings(maximumIndexSizeMiB: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeBucketBloomFilterSettings(maximumIndexSizeMiB: 65));
            
            foreach (bool exact in new[] { false, true })
            {
                foreach (bool bloom in new[] { false, true })
                {
                    using MemoryStream memory = new MemoryStream();
                    using BinaryStreamWrapper stream = new BinaryStreamWrapper(memory, false);
                    SortedTreeEngineReaderOptions original = new SortedTreeEngineReaderOptions(exact, bloom, TimeSpan.FromSeconds(2), 3, 4, 5);
                    
                    original.Save(stream);
                    
                    Assert.AreEqual(exact && bloom ? 0 : 1, memory.ToArray()[0]);
                    memory.Position = 0;
                    
                    SortedTreeEngineReaderOptions restored = new SortedTreeEngineReaderOptions(stream);
                    
                    Assert.AreEqual(exact, restored.UsePointIDIndex);
                    Assert.AreEqual(bloom, restored.UseTimeBucketBloomFilter);
                    Assert.AreEqual(original.Timeout, restored.Timeout);
                    Assert.AreEqual(3, restored.MaxReturnedCount);
                    Assert.AreEqual(4, restored.MaxScanCount);
                    Assert.AreEqual(5, restored.MaxSeekCount);
                    Assert.AreEqual(memory.Length, memory.Position);
                }
            }

            AdvancedServerDatabaseConfig<HistorianKey, HistorianValue> config = new AdvancedServerDatabaseConfig<HistorianKey, HistorianValue>("test", Path.GetTempPath(), true)
            {
                EnablePointIDIndex = false,
                BloomFilter = new TimeBucketBloomFilterSettings(true, 1, 0.01, 1)
            };

            ServerDatabaseSettings server = config.ToServerDatabaseSettings();
            SimplifiedArchiveInitializerSettings settings = server.WriteProcessor.FirstStageWriter.FinalSettings;
            
            Assert.IsFalse(settings.EnablePointIDIndex);
            
            foreach (CombineFilesSettings rollover in server.WriteProcessor.StagingRollovers)
            {
                Assert.IsFalse(rollover.ArchiveSettings.EnablePointIDIndex);
                Assert.AreEqual(1, rollover.ArchiveSettings.BloomFilter.MaximumIndexSizeMiB);
            }
            
            using (MemoryStream memory = new MemoryStream())
            {
                settings.Save(memory);
                memory.Position = 0;
                
                SimplifiedArchiveInitializerSettings restored = new SimplifiedArchiveInitializerSettings();
                
                restored.Load(memory);
                
                Assert.IsFalse(restored.EnablePointIDIndex);
                Assert.AreEqual(1, restored.BloomFilter.MaximumIndexSizeMiB);
                Assert.IsTrue(restored.BloomFilter.Enabled);
                Assert.AreEqual(memory.Length, memory.Position);
            }
            
            ArchiveTimeBucketBloomIndex.Builder small = new ArchiveTimeBucketBloomIndex.Builder(new TimeBucketBloomFilterSettings(true, 1, 0.01, 1));
            ArchiveTimeBucketBloomIndex.Builder larger = new ArchiveTimeBucketBloomIndex.Builder(new TimeBucketBloomFilterSettings(true, 1, 0.01, 2));
            
            for (ulong bucket = 0; bucket < 600; bucket++)
            {
                for (ulong id = 0; id < 1000; id++)
                {
                    small.Add(bucket * TimeSpan.TicksPerSecond, id);
                    larger.Add(bucket * TimeSpan.TicksPerSecond, id);
                }
            }

            Assert.IsNull(small.Complete(), "An exhausted budget must drop the complete index.");
            Assert.IsNotNull(larger.Complete());
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void IndexGenerationSwitchesAreIndependent(bool exact, bool bloom)
        {
            string folder = Path.Combine(Path.GetTempPath(), "index-controls-" + Guid.NewGuid().ToString("N"));
            
            Directory.CreateDirectory(folder);

            try
            {
                SimplifiedArchiveInitializerSettings settings = new SimplifiedArchiveInitializerSettings();

                settings.ConfigureOnDisk(new[] { folder }, 100 * 1024 * 1024, ArchiveDirectoryMethod.TopDirectoryOnly, HistorianFileEncodingDefinition.TypeGuid, "test", ".pending", ".d2");
                settings.EnablePointIDIndex = exact;
                settings.BloomFilter = new TimeBucketBloomFilterSettings(bloom, maximumIndexSizeMiB: 1);

                SimplifiedArchiveInitializer<HistorianKey, HistorianValue> initializer = new SimplifiedArchiveInitializer<HistorianKey, HistorianValue>(settings);
                using DiskSamples stream = new DiskSamples(new[]
                {
                    new Sample
                    {
                        Time = 20,
                        ID = 42
                    }
                });
                SortedTreeTable<HistorianKey, HistorianValue> table = initializer.CreateArchiveFile(new HistorianKey { Timestamp = 20 }, new HistorianKey { Timestamp = 20 }, 0, stream, null);
                using SortedTreeFile file = table.BaseFile;

                Assert.AreEqual(exact, File.Exists(file.FilePath + ".pidx"));
                Assert.AreEqual(bloom, File.Exists(file.FilePath + ".bidx"));
                Assert.AreEqual(42UL, table.FirstKey.PointID);
            }
            finally
            {
                foreach (string entry in Directory.GetFiles(folder)) 
                    File.Delete(entry); Directory.Delete(folder);
            }
        }

        private sealed class SelectedIDs : MatchFilterBase<HistorianKey, HistorianValue>
        {
            private readonly HashSet<ulong> m_ids;
            public SelectedIDs(IEnumerable<ulong> ids) { m_ids = new HashSet<ulong>(ids); }
            public override Guid FilterType => Guid.Empty;
            public override bool Contains(HistorianKey key, HistorianValue value) => m_ids.Contains(key.PointID);
            public override void Save(BinaryStreamBase stream) { throw new NotSupportedException(); }
        }

        [Test]
        public void GeneratedYearSkipsInterveningSamples()
        {
            const ulong Minute = TimeSpan.TicksPerMinute;
            const ulong Interval = 240 * Minute;
            const ulong End = 365 * 24 * 60 * Minute;
            
            List<Sample> data = new List<Sample>();
            
            for (ulong t = 0; t < End; t += 5 * Minute)
            {
                for (ulong id = 1; id <= 3; id++)
                    data.Add(new Sample { Time = t + id * 7 * TimeSpan.TicksPerSecond, ID = id, Value = t + id });
            }

            using ArchiveList<HistorianKey, HistorianValue> archives = new ArchiveList<HistorianKey, HistorianValue>();
            AddArchive(archives, data, true);
            
            // Reproduce the original Grafana failure with a 1 ms interval tolerance.
            Assert.AreEqual(0, Read(archives, TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(Interval, End - 1, Interval, TimeSpan.TicksPerMillisecond)).Count);
            
            long before = Stats.PointsScanned;
            Stopwatch timer = Stopwatch.StartNew();
            List<Sample> result = Read(archives, new NearestTimestampSeekFilter<HistorianKey>(0, End - 1, Interval, new ulong[] { 1, 2, 3 }));
            
            timer.Stop();
            
            long visited = Stats.PointsScanned - before;
            
            Assert.AreEqual(2190 * 3, result.Count);
            Assert.Less(visited, data.Count / 5, "Must skip the data between targets, not perform a full scan.");
            
            foreach (Sample sample in result)
                Assert.AreEqual(sample.ID * 7 * TimeSpan.TicksPerSecond, sample.Time % Interval);
            
            System.Console.WriteLine("Nearest year: source={0}, returned={1}, visited={2}, elapsed={3} ms", data.Count, result.Count, visited, timer.ElapsedMilliseconds);

            if (Environment.GetEnvironmentVariable("SNAPDB_NEAREST_BENCHMARK") != "1")
                return;
            
            // Keep test-framework assertions and result materialization out of the timed
            // section; NUnit 2 and NUnit 3 have different per-assertion overheads.
            Diagnostics.Logger.Console.Verbose = Diagnostics.VerboseLevel.None;
            double[] milliseconds = new double[100];
            
            for (int iteration = 0; iteration < milliseconds.Length; iteration++)
            {
                int count = 0;
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();
                timer.Restart();
                
                using (SequentialReaderStream<HistorianKey, HistorianValue> reader = new SequentialReaderStream<HistorianKey, HistorianValue>(archives, null, new NearestTimestampSeekFilter<HistorianKey>(0, End - 1, Interval, new ulong[] { 1, 2, 3 })))
                    while (reader.Read(key, value)) count++;
                
                timer.Stop();
                milliseconds[iteration] = timer.Elapsed.TotalMilliseconds;
                
                Assert.AreEqual(6570, count);
            }
            
            System.Console.WriteLine("Engine-only query milliseconds (100 runs): " + string.Join(", ", milliseconds.Select(ms => ms.ToString("F2"))));
            System.Console.WriteLine("Engine-only final-10 median: {0:F2} ms; 64-bit={1}; CLR={2}", milliseconds.Skip(milliseconds.Length - 10).OrderBy(ms => ms).ElementAt(5), Environment.Is64BitProcess, Environment.Version);
        }
    }
}
