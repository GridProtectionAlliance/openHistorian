using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.Snap.Storage;
using GSF.Snap.Tree;
using NUnit.Framework;
using openHistorian.Collections;

namespace GSF.Snap.Services.Reader
{
    [TestFixture]
    public class UnionReaderTest
    {
        private int seed = 1;

        [Test]
        public void Test200()
        {
            Test(200);
        }

        [Test]
        public void Test()
        {
            for (int x = 1; x < 1000; x *= 2)
            {
                Test(x);
            }
        }

        public void Test(int count)
        {
            var lst = new List<SortedTreeTable<HistorianKey, HistorianValue>>();
            for (int x = 0; x < count; x++)
            {
                lst.Add(CreateTable());
            }

            using (var reader = new UnionTreeStream<HistorianKey, HistorianValue>(lst.Select(x => new ArchiveTreeStreamWrapper<HistorianKey, HistorianValue>(x)), true))
            {
                var key = new HistorianKey();
                var value = new HistorianValue();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (reader.Read(key, value))
                    ;
                sw.Stop();
                System.Console.Write("{0}\t{1}\t{2}", count, sw.Elapsed.TotalSeconds, sw.Elapsed.TotalSeconds / count);
                System.Console.WriteLine();
            }

            lst.ForEach(x => x.Dispose());
        }


        SortedTreeTable<HistorianKey, HistorianValue> CreateTable()
        {
            var r = new Random(seed++);
            var key = new HistorianKey();
            var value = new HistorianValue();
            var file = SortedTreeFile.CreateInMemory();
            var table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(SortedTree.FixedSizeNode);

            using (var edit = table.BeginEdit())
            {
                for (int x = 0; x < 1000; x++)
                {
                    key.Timestamp = (ulong)r.Next();
                    key.PointID = (ulong)r.Next();
                    key.EntryNumber = (ulong)r.Next();
                    edit.AddPoint(key, value);
                }
                edit.Commit();
            }




            return table;
        }




    }
}
