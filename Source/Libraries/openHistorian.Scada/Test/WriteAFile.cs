using System;
using System.Diagnostics;
using GSF.Snap;
using GSF.Snap.Services;
using GSF.Snap.Services.Configuration;
using GSF.Snap.Storage;
using GSF.Snap.Tree;
using NUnit.Framework;
using openHistorian.Scada.AMI;

namespace openHistorian.Scada.Test
{
    [TestFixture]
    public class WriteAFile
    {

        [Test]
        public void TestFixed()
        {
            int count = 10000000;
            Stopwatch sw = new Stopwatch();
            using (SortedTreeFile af = SortedTreeFile.CreateInMemory())
            using (SortedTreeTable<AmiKey, AmiKey> table = af.OpenOrCreateTable<AmiKey, AmiKey>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                AmiKey key = new AmiKey();
                AmiKey value = new AmiKey();
                using (SortedTreeTableEditor<AmiKey, AmiKey> edit = table.BeginEdit())
                {
                    sw.Start();


                    for (int x = 0; x < count; x++)
                    {
                        key.Timestamp = (uint)x;
                        edit.AddPoint(key, value);
                    }

                    edit.Commit();
                    sw.Stop();

                    Console.WriteLine(af.ArchiveSize / 1024.0 / 1024.0);
                    Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);
                }

                using (SortedTreeTableReadSnapshot<AmiKey, AmiKey> read = table.BeginRead())
                using (SortedTreeScannerBase<AmiKey, AmiKey> scan = read.GetTreeScanner())
                {
                    scan.SeekToStart();
                    while (scan.Read(key, value))
                        count--;
                }
                Console.WriteLine(count);
            }
        }

        [Test]
        public void TestRollover()
        {
            int count = 1000000;
            Stopwatch sw = new Stopwatch();
            using (SortedTreeFile af = SortedTreeFile.CreateInMemory())
            using (SortedTreeTable<AmiKey, AmiKey> table = af.OpenOrCreateTable<AmiKey, AmiKey>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                using (SortedTreeTableEditor<AmiKey, AmiKey> edit = table.BeginEdit())
                {
                    sw.Start();
                    AmiKey key = new AmiKey();
                    AmiKey value = new AmiKey();

                    for (int x = 0; x < count; x++)
                    {
                        key.Timestamp = (uint)x;
                        edit.AddPoint(key, value);
                    }

                    edit.Commit();
                    sw.Stop();

                    Console.WriteLine(af.ArchiveSize / 1024.0 / 1024.0);
                    Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);
                }

                using (SortedTreeFile af2 = SortedTreeFile.CreateInMemory())
                using (SortedTreeTable<AmiKey, AmiKey> table2 = af2.OpenOrCreateTable<AmiKey, AmiKey>(new EncodingDefinition(EncodingDefinition.FixedSizeCombinedEncoding.KeyValueEncodingMethod, EncodingDefinition.FixedSizeCombinedEncoding.KeyValueEncodingMethod)))
                using (SortedTreeTableEditor<AmiKey, AmiKey> edit = table2.BeginEdit())
                {
                    using (SortedTreeTableReadSnapshot<AmiKey, AmiKey> read = table.BeginRead())
                    using (SortedTreeScannerBase<AmiKey, AmiKey> scan = read.GetTreeScanner())
                    {
                        scan.SeekToStart();
                        edit.AddPoints(scan);
                    }
                }

                Console.WriteLine(count);
            }
        }

        [Test]
        public void TestArchiveWriter()
        {
            Random r = new Random(3);
            _ = new EncodingDefinition(EncodingDefinition.FixedSizeIndividualGuid, EncodingDefinition.FixedSizeIndividualGuid);
            AdvancedServerDatabaseConfig<AmiKey, AmiKey> config = new AdvancedServerDatabaseConfig<AmiKey, AmiKey>("KV2CPQ", "C:\\Temp\\AMI", true);
            using (SnapServer server = new SnapServer(config))
            {
                using (SnapClient client = SnapClient.Connect(server))
                using (ClientDatabaseBase<AmiKey, AmiKey> db = client.GetDatabase<AmiKey, AmiKey>("KV2CPQ"))
                {
                    int count = 10000000;
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    AmiKey key = new AmiKey();
                    AmiKey value = new AmiKey();

                    for (int x = count; x >= 0; x--)
                    {
                        key.Timestamp = (ulong)r.Next();
                        key.TableId = r.Next();
                        db.Write(key, value);
                    }

                    sw.Stop();

                    Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);

                    Console.WriteLine(count);
                }
            }
        }

        //[Test]
        //public void TestFixed()
        //{
        //    int count = 10000000;
        //    Stopwatch sw = new Stopwatch();
        //    using (var af = SortedTreeFile.CreateInMemory())
        //    using (var table = af.OpenOrCreateTable<AmiKey, AmiKey>(SortedTree.FixedSizeNode))
        //    {
        //        using (var edit = table.BeginEdit())
        //        {
        //            sw.Start();
        //            var key = new AmiKey();
        //            var value = new AmiKey();

        //            for (int x = 0; x < count; x++)
        //            {
        //                key.Timestamp = (uint)x;
        //                edit.AddPoint(key, value);
        //            }

        //            edit.Commit();
        //            sw.Stop();

        //            Console.WriteLine(af.FileSize / 1024.0 / 1024.0);
        //            Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);
        //        }

        //        using (var read = table.BeginRead())
        //        using (var scan = read.GetTreeScanner())
        //        {
        //            scan.SeekToStart();
        //            while (scan.Read())
        //                count--;
        //        }
        //        Console.WriteLine(count);
        //    }
        //}
        [Test]
        public void TestCompressed()
        {
            int count = 10000000;
            Stopwatch sw = new Stopwatch();
            using (SortedTreeFile af = SortedTreeFile.CreateInMemory())
            using (SortedTreeTable<AmiKey, AmiValue> table = af.OpenOrCreateTable<AmiKey, AmiValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                using (SortedTreeTableEditor<AmiKey, AmiValue> edit = table.BeginEdit())
                {
                    sw.Start();
                    AmiKey key = new AmiKey();
                    AmiValue value = new AmiValue();

                    for (int x = 0; x < count; x++)
                    {
                        key.Timestamp = (uint)x;
                        edit.AddPoint(key, value);
                    }

                    edit.Commit();
                    sw.Stop();

                    Console.WriteLine(af.ArchiveSize / 1024.0 / 1024.0);
                    Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);
                }
            }
        }
    }
}
