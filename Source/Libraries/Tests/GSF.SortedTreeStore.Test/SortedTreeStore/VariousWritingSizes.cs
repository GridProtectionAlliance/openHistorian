using GSF.Snap.Storage;
using GSF.Snap.Tree;
using NUnit.Framework;
using openHistorian.Snap;

namespace GSF.Snap
{
    [TestFixture]
    public class VariousWritingSizes
    {
        [Test]
        public void TestSmall()
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            using (SortedTreeFile af = SortedTreeFile.CreateInMemory())
            using (SortedTreeTable<HistorianKey, HistorianValue> file = af.OpenOrCreateTable<HistorianKey, HistorianValue>(EncodingDefinition.FixedSizeCombinedEncoding))
            {
                using (SortedTreeTableEditor<HistorianKey, HistorianValue> edit = file.BeginEdit())
                {
                  
                    for (int x = 0; x < 10000000; x++)
                    {
                        key.Timestamp = (ulong)x;
                        edit.AddPoint(key, value);
                    }
                    edit.Commit();
                }

                using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> read = file.BeginRead())
                using (SortedTreeScannerBase<HistorianKey, HistorianValue> scan = read.GetTreeScanner())
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
