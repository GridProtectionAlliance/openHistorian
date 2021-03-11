using System;
using System.Collections.Generic;
using GSF.IO.Unmanaged;
using NUnit.Framework;
using openHistorian.Snap;
using openHistorian.Snap.Definitions;

namespace GSF.Snap.Tree.CustomCompression
{
    internal class SequentialTest
     : TreeNodeRandomizerBase<HistorianKey, HistorianValue>
    {
        private readonly SortedList<uint, uint> m_sortedItems = new SortedList<uint, uint>();
        private readonly List<KeyValuePair<uint, uint>> m_items = new List<KeyValuePair<uint, uint>>();

        private int m_maxCount;
        private uint m_current;

        public override void Reset(int maxCount)
        {
            m_sortedItems.Clear();
            m_items.Clear();
            m_maxCount = maxCount;
            m_current = 1;
        }

        public override void Next()
        {
            m_sortedItems.Add(m_current, m_current * 2);
            m_items.Add(new KeyValuePair<uint, uint>(m_current, m_current * 2));
            m_current++;
        }

        public override void GetRandom(int index, HistorianKey key, HistorianValue value)
        {
            KeyValuePair<uint, uint> kvp = m_items[index];
            key.Timestamp = kvp.Key;
            value.Value1 = kvp.Value;
        }

        public override void GetInSequence(int index, HistorianKey key, HistorianValue value)
        {
            key.Timestamp = m_sortedItems.Keys[index];
            value.Value1 = m_sortedItems.Values[index];
        }
    }

    internal class ReverseSequentialTest
        : TreeNodeRandomizerBase<HistorianKey, HistorianValue>
    {
        private readonly SortedList<uint, uint> m_sortedItems = new SortedList<uint, uint>();
        private readonly List<KeyValuePair<uint, uint>> m_items = new List<KeyValuePair<uint, uint>>();

        private int m_maxCount;
        private uint m_current;

        public override void Reset(int maxCount)
        {
            m_sortedItems.Clear();
            m_items.Clear();
            m_maxCount = maxCount;
            m_current = (uint)maxCount;
        }

        public override void Next()
        {
            m_sortedItems.Add(m_current, m_current * 2);
            m_items.Add(new KeyValuePair<uint, uint>(m_current, m_current * 2));
            m_current--;
        }

        public override void GetRandom(int index, HistorianKey key, HistorianValue value)
        {
            KeyValuePair<uint, uint> kvp = m_items[index];
            key.Timestamp = kvp.Key;
            value.Value1 = kvp.Value;
        }

        public override void GetInSequence(int index, HistorianKey key, HistorianValue value)
        {
            key.Timestamp = m_sortedItems.Keys[index];
            value.Value1 = m_sortedItems.Values[index];
        }
    }

    internal class RandomTest
        : TreeNodeRandomizerBase<HistorianKey, HistorianValue>
    {
        private readonly SortedList<ulong, ulong> m_sortedItems = new SortedList<ulong, ulong>();
        private readonly List<KeyValuePair<ulong, ulong>> m_items = new List<KeyValuePair<ulong, ulong>>();

        private Random r;

        public override void Reset(int maxCount)
        {
            r = new Random(1);
            m_sortedItems.Clear();
            m_items.Clear();
        }

        public override void Next()
        {
            ulong rand = ((ulong)r.Next() << 33) | (uint)r.Next();
            m_sortedItems.Add(rand, rand * 2);
            m_items.Add(new KeyValuePair<ulong, ulong>(rand, rand * 2));
        }

        public override void GetRandom(int index, HistorianKey key, HistorianValue value)
        {
            KeyValuePair<ulong, ulong> kvp = m_items[index];
            key.Timestamp = kvp.Key;
            value.Value1 = kvp.Value;
        }

        public override void GetInSequence(int index, HistorianKey key, HistorianValue value)
        {
            key.Timestamp = m_sortedItems.Keys[index];
            value.Value1 = m_sortedItems.Values[index];
        }
    }

    [TestFixture]
    class HistorianCompressionTs
    {
        private const int Max = 1000000;

        [Test]
        public void TestCompressCases()
        {
            using (BinaryStream bs = new BinaryStream())
            {

                SortedTree<HistorianKey, HistorianValue> tree = SortedTree<HistorianKey, HistorianValue>.Create(bs, 4096, HistorianFileEncodingDefinition.TypeGuid);
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
            SortedTreeNodeBase<HistorianKey, HistorianValue> tree = Library.CreateTreeNode<HistorianKey, HistorianValue>(HistorianFileEncodingDefinition.TypeGuid, 0);

            LeafNodeTest.TestNode(tree, new SequentialTest(), 5000);
        }

        [Test]
        public void TestReverseSequently()
        {
            SortedTreeNodeBase<HistorianKey, HistorianValue> tree = Library.CreateTreeNode<HistorianKey, HistorianValue>(HistorianFileEncodingDefinition.TypeGuid, 0);

            LeafNodeTest.TestNode(tree, new ReverseSequentialTest(), 5000);
        }

        [Test]
        public void TestRandom()
        {
            SortedTreeNodeBase<HistorianKey, HistorianValue> tree = Library.CreateTreeNode<HistorianKey, HistorianValue>(HistorianFileEncodingDefinition.TypeGuid, 0);

            LeafNodeTest.TestNode(tree, new RandomTest(), 2000);
        }

        [Test]
        public void BenchmarkSequently()
        {
            SortedTreeNodeBase<HistorianKey, HistorianValue> tree = Library.CreateTreeNode<HistorianKey, HistorianValue>(HistorianFileEncodingDefinition.TypeGuid, 0);

            LeafNodeTest.TestSpeed(tree, new SequentialTest(), 500, 512);
        }

        [Test]
        public void BenchmarkReverseSequently()
        {
            SortedTreeNodeBase<HistorianKey, HistorianValue> tree = Library.CreateTreeNode<HistorianKey, HistorianValue>(HistorianFileEncodingDefinition.TypeGuid, 0);

            LeafNodeTest.TestSpeed(tree, new ReverseSequentialTest(), 500, 512);
        }

        [Test]
        public void BenchmarkRandom()
        {
            SortedTreeNodeBase<HistorianKey, HistorianValue> tree = Library.CreateTreeNode<HistorianKey, HistorianValue>(HistorianFileEncodingDefinition.TypeGuid, 0);

            LeafNodeTest.TestSpeed(tree, new RandomTest(), 500, 512);
        }


    }
}
