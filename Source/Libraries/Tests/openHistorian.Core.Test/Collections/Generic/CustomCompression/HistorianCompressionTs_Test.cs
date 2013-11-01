using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.IO.Unmanaged;
using NUnit.Framework;
using openHistorian.Collections.Generic.TreeNodes;
using openHistorian.FileStructure;

namespace openHistorian.Collections.Generic.CustomCompression
{

    [TestFixture]
    class HistorianCompressionTs
    {
        private const int Max = 1000000;

        [Test]
        public void TestCompressCases()
        {
            using (var bs = new BinaryStream())
            {
                
                var tree = SortedTree<HistorianKey, HistorianValue>.Create(bs, 4096, CreateHistorianCompressionTs.TypeGuid);
                HistorianKey key = new HistorianKey();
                HistorianKey key1 = new HistorianKey();
                HistorianValue value = new HistorianValue();

                key.Timestamp = 0;
                key.PointID = 0;
                key.EntryNumber = 0;

                value.Value1 = 0;
                value.Value2 = 0;
                value.Value3 = 0;

                tree.Add(key, value);
                tree.Get(key, value);
                Assert.AreEqual(0ul, value.Value1);
                Assert.AreEqual(0ul, value.Value2);
                Assert.AreEqual(0ul, value.Value3);

                key.PointID = 1;
                tree.Add(key, value);
                tree.Get(key1, value);
                tree.Get(key, value);
                Assert.AreEqual(0ul, value.Value1);
                Assert.AreEqual(0ul, value.Value2);
                Assert.AreEqual(0ul, value.Value3);

                key.PointID = 2;
                value.Value1 = 1;
                tree.Add(key, value);
                tree.Get(key1, value);
                tree.Get(key, value);
                Assert.AreEqual(1ul, value.Value1);
                Assert.AreEqual(0ul, value.Value2);
                Assert.AreEqual(0ul, value.Value3);

                key.PointID = 3;
                value.Value1 = 561230651435234523ul;
                tree.Add(key, value);
                tree.Get(key1, value);
                tree.Get(key, value);
                Assert.AreEqual(561230651435234523ul, value.Value1);
                Assert.AreEqual(0ul, value.Value2);
                Assert.AreEqual(0ul, value.Value3);


                key.PointID = 35602353232;
                value.Value1 = 561230651435234523ul;
                tree.Add(key, value);
                tree.Get(key1, value);
                tree.Get(key, value);
                Assert.AreEqual(561230651435234523ul, value.Value1);
                Assert.AreEqual(0ul, value.Value2);
                Assert.AreEqual(0ul, value.Value3);


                key.PointID++;
                value.Value1 = 561230651435234523ul;
                value.Value2 = 561230651435234524ul;
                value.Value3 = 561230651435234525ul;
                tree.Add(key, value);
                tree.Get(key1, value);
                tree.Get(key, value);
                Assert.AreEqual(561230651435234523ul, value.Value1);
                Assert.AreEqual(561230651435234524ul, value.Value2);
                Assert.AreEqual(561230651435234525ul, value.Value3);

                key.EntryNumber = 1;
                value.Value1 = 561230651435234523ul;
                value.Value2 = 561230651435234524ul;
                value.Value3 = 561230651435234525ul;
                tree.Add(key, value);
                tree.Get(key1, value);
                tree.Get(key, value);
                Assert.AreEqual(561230651435234523ul, value.Value1);
                Assert.AreEqual(561230651435234524ul, value.Value2);
                Assert.AreEqual(561230651435234525ul, value.Value3);

                key.PointID++;
                key.EntryNumber = 0;
                value.AsSingle = 60.1f;
                value.Value2 = 0;
                value.Value3 = 0;
                tree.Add(key, value);
                tree.Get(key1, value);
                tree.Get(key, value);
                Assert.AreEqual(60.1f, value.AsSingle);
                Assert.AreEqual(0ul, value.Value2);
                Assert.AreEqual(0ul, value.Value3);


                key.PointID++;
                key.EntryNumber = 0;
                value.AsSingle = -60.1f;
                value.Value2 = 0;
                value.Value3 = 0;
                tree.Add(key, value);
                tree.Get(key1, value);
                tree.Get(key, value);
                Assert.AreEqual(-60.1f, value.AsSingle);
                Assert.AreEqual(0ul, value.Value2);
                Assert.AreEqual(0ul, value.Value3);

                key.Timestamp++;
                key.EntryNumber = 0;
                value.Value1 = 0;
                value.Value2 = 0;
                value.Value3 = 0;
                tree.Add(key, value);
                tree.Get(key1, value);
                tree.Get(key, value);
                Assert.AreEqual(0ul, value.Value1);
                Assert.AreEqual(0ul, value.Value2);
                Assert.AreEqual(0ul, value.Value3);

            }
         
        }

        [Test]
        public void TestSequently()
        {
            var tree = TreeNodeInitializer.CreateTreeNode<HistorianKey, HistorianValue>(CreateHistorianCompressionTs.TypeGuid, 0);

            LeafNodeTest.TestNode(tree, new SequentialTest(), 5000);
        }

        [Test]
        public void TestReverseSequently()
        {
            var tree = TreeNodeInitializer.CreateTreeNode<HistorianKey, HistorianValue>(CreateHistorianCompressionTs.TypeGuid, 0);

            LeafNodeTest.TestNode(tree, new ReverseSequentialTest(), 5000);
        }

        [Test]
        public void TestRandom()
        {
            var tree = TreeNodeInitializer.CreateTreeNode<HistorianKey, HistorianValue>(CreateHistorianCompressionTs.TypeGuid, 0);

            LeafNodeTest.TestNode(tree, new RandomTest(), 2000);
        }

        [Test]
        public void BenchmarkSequently()
        {
            var tree = TreeNodeInitializer.GetTreeNodeInitializer<HistorianKey, HistorianValue>(CreateHistorianCompressionTs.TypeGuid);

            LeafNodeTest.TestSpeed(tree, new SequentialTest(), 500, 512);
        }

        [Test]
        public void BenchmarkReverseSequently()
        {
            var tree = TreeNodeInitializer.GetTreeNodeInitializer<HistorianKey, HistorianValue>(CreateHistorianCompressionTs.TypeGuid);

            LeafNodeTest.TestSpeed(tree, new ReverseSequentialTest(), 500, 512);
        }

        [Test]
        public void BenchmarkRandom()
        {
            var tree = TreeNodeInitializer.GetTreeNodeInitializer<HistorianKey, HistorianValue>(CreateHistorianCompressionTs.TypeGuid);

            LeafNodeTest.TestSpeed(tree, new RandomTest(), 500, 512);
        }
    }
}
