using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF.Snap.Storage;
using GSF.Snap.Tree;
using NUnit.Framework;
using openHistorian.Collections;
using openHistorian.Snap;

namespace GSF.Snap
{
    [TestFixture]
    public class VariousWritingSizes
    {
        [Test]
        public void TestSmall()
        {
            var key = new HistorianKey();
            var value = new HistorianValue();
            using (var af = SortedTreeFile.CreateInMemory())
            using (var file = af.OpenOrCreateTable<HistorianKey, HistorianValue>(SortedTree.FixedSizeNode))
            {
                using (var edit = file.BeginEdit())
                {
                  
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
                    while (scan.Read(key,value))
                    {
                        count++;
                    }
                    System.Console.WriteLine(count.ToString());
                }
            }
        }
    }
}
