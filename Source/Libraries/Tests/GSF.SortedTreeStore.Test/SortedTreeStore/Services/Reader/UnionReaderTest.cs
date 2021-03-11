using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GSF.Snap.Storage;
using NUnit.Framework;
using openHistorian.Snap;

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
            List<SortedTreeTable<HistorianKey, HistorianValue>> lst = new List<SortedTreeTable<HistorianKey, HistorianValue>>();
            for (int x = 0; x < count; x++)
            {
                lst.Add(CreateTable());
            }

            using (UnionTreeStream<HistorianKey, HistorianValue> reader = new UnionTreeStream<HistorianKey, HistorianValue>(lst.Select(x => new ArchiveTreeStreamWrapper<HistorianKey, HistorianValue>(x)), true))
            {
                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();
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
            Random r = new Random(seed++);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            SortedTreeFile file = SortedTreeFile.CreateInMemory();
            SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding);

            using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = table.BeginEdit())
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
