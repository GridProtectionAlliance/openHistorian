using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.IO.Unmanaged;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;
using NUnit.Framework;
using openHistorian.Scada.AMI;
using openHistorian.SortedTreeStore.Types.CustomCompression.Ts;

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
            using (var af = SortedTreeFile.CreateInMemory())
            using (var table = af.OpenOrCreateTable<AmiKey, AmiKey>(SortedTree.FixedSizeNode))
            {
                using (var edit = table.BeginEdit())
                {
                    sw.Start();
                    var key = new AmiKey();
                    var value = new AmiKey();

                    for (int x = 0; x < count; x++)
                    {
                        key.Timestamp = (uint)x;
                        edit.AddPoint(key, value);
                    }

                    edit.Commit();
                    sw.Stop();

                    Console.WriteLine(af.FileSize / 1024.0 / 1024.0);
                    Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);
                }

                using (var read = table.BeginRead())
                using (var scan = read.GetTreeScanner())
                {
                    scan.SeekToStart();
                    while (scan.Read())
                        count--;
                }
                Console.WriteLine(count);
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
            using (var af = SortedTreeFile.CreateInMemory())
            using (var table = af.OpenOrCreateTable<AmiKey, AmiValue>(CreateAmiCombinedEncoding.TypeGuid))
            {
                using (var edit = table.BeginEdit())
                {
                    sw.Start();
                    var key = new AmiKey();
                    var value = new AmiValue();

                    for (int x = 0; x < count; x++)
                    {
                        key.Timestamp = (uint)x;
                        edit.AddPoint(key, value);
                    }

                    edit.Commit();
                    sw.Stop();

                    Console.WriteLine(af.FileSize / 1024.0 / 1024.0);
                    Console.WriteLine(count / sw.Elapsed.TotalSeconds / 1000000);
                }
            }
        }
    }
}
