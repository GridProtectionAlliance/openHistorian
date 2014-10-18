using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.IO.FileStructure;
using GSF.IO.Unmanaged;
using GSF.SortedTreeStore.Collection;
using GSF.SortedTreeStore.Tree;
using NUnit.Framework;
using openHistorian.Collections;

namespace GSF.SortedTreeStore.Storage
{
    [TestFixture]
    public class SortedTreeFileSimpleWriterTest
    {
        [Test]
        public void Test()
        {
            for (int x = 1; x < 10000; x += 10)
            {
                Test(x);
                System.Console.WriteLine(x);
            }

        }

        public void Test(int pointCount)
        {
            SortedPointBuffer<HistorianKey, HistorianValue> points = new SortedPointBuffer<HistorianKey, HistorianValue>(pointCount, true);

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            for (int x = 0; x < pointCount; x++)
            {
                key.PointID = (ulong)x;
                points.TryEnqueue(key, value);
            }

            points.IsReadingMode = true;

            File.Delete(@"C:\Temp\fileTemp.~d2i");
            File.Delete(@"C:\Temp\fileTemp.d2i");

            SortedTreeFileSimpleWriter<HistorianKey, HistorianValue>.Create(@"C:\Temp\fileTemp.~d2i", @"C:\Temp\fileTemp.d2i", 4096, SortedTree.FixedSizeNode, points);

            using (var file = SortedTreeFile.OpenFile(@"C:\Temp\fileTemp.d2i", true))
            using (var table = file.OpenTable<HistorianKey, HistorianValue>())
            using (var read = table.AcquireReadSnapshot().CreateReadSnapshot())
            using (var scanner = read.GetTreeScanner())
            {
                scanner.SeekToStart();
                int cnt = 0;
                while (scanner.Read(key, value))
                {
                    if (key.PointID != (ulong)cnt)
                        throw new Exception();
                    cnt++;

                }
                if (cnt != pointCount)
                    throw new Exception();


            }
        }
    }
}
