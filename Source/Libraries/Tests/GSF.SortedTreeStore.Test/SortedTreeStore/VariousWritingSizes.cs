using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;
using NUnit.Framework;
using openHistorian.Collections;

namespace GSF.SortedTreeStore
{
    [TestFixture]
    public class VariousWritingSizes
    {
        [Test]
        public void TestSmall()
        {
            using (var af = SortedTreeFile.CreateInMemory())
            using (var file = af.OpenOrCreateTable<HistorianKey, HistorianValue>(SortedTree.FixedSizeNode))
            {
                using (var edit = file.BeginEdit())
                {
                    var key = new HistorianKey();
                    var value = new HistorianValue();
                    for (int x = 0; x < 10000000; x++)
                    {
                        key.Timestamp = (ulong)x;
                        edit.AddPoint(key, value);
                    }
                    edit.Commit();
                }

                using (var read = file.BeginRead())
                using (var scan = read.GetTreeScanner())
                {
                    int count = 0;
                    scan.SeekToStart();
                    while (scan.Read())
                    {
                        count++;
                    }
                    System.Console.WriteLine(count.ToString());
                }
            }
        }
    }
}
