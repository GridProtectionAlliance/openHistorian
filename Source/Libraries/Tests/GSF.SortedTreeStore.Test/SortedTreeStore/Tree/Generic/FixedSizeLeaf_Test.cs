using System;
using System.Collections.Generic;
using GSF.Snap.Types;
using NUnit.Framework;

namespace GSF.Snap.Tree
{
    internal class SequentialTest
        : TreeNodeRandomizerBase<SnapUInt32, SnapUInt32>
    {
        private readonly SortedList<uint, uint> m_sortedItems = new SortedList<uint, uint>();
        private readonly List<KeyValuePair<uint, uint>> m_items = new List<KeyValuePair<uint, uint>>();

        private int m_maxCount;
        private uint m_current = 0;

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

        public override void GetRandom(int index, SnapUInt32 key, SnapUInt32 value)
        {
            KeyValuePair<uint, uint> kvp = m_items[index];
            key.Value = kvp.Key;
            value.Value = kvp.Value;
        }

        public override void GetInSequence(int index, SnapUInt32 key, SnapUInt32 value)
        {
            key.Value = m_sortedItems.Keys[index];
            value.Value = m_sortedItems.Values[index];
        }
    }

    internal class ReverseSequentialTest
        : TreeNodeRandomizerBase<SnapUInt32, SnapUInt32>
    {
        private readonly SortedList<uint, uint> m_sortedItems = new SortedList<uint, uint>();
        private readonly List<KeyValuePair<uint, uint>> m_items = new List<KeyValuePair<uint, uint>>();

        private int m_maxCount;
        private uint m_current = 0;

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

        public override void GetRandom(int index, SnapUInt32 key, SnapUInt32 value)
        {
            KeyValuePair<uint, uint> kvp = m_items[index];
            key.Value = kvp.Key;
            value.Value = kvp.Value;
        }

        public override void GetInSequence(int index, SnapUInt32 key, SnapUInt32 value)
        {
            key.Value = m_sortedItems.Keys[index];
            value.Value = m_sortedItems.Values[index];
        }
    }

    internal class RandomTest
        : TreeNodeRandomizerBase<SnapUInt32, SnapUInt32>
    {
        private readonly SortedList<uint, uint> m_sortedItems = new SortedList<uint, uint>();
        private readonly List<KeyValuePair<uint, uint>> m_items = new List<KeyValuePair<uint, uint>>();

        private Random r;

        public override void Reset(int maxCount)
        {
            r = new Random(1);
            m_sortedItems.Clear();
            m_items.Clear();
        }

        public override void Next()
        {
            uint rand = (uint)r.Next();
            m_sortedItems.Add(rand, rand * 2);
            m_items.Add(new KeyValuePair<uint, uint>(rand, rand * 2));
        }

        public override void GetRandom(int index, SnapUInt32 key, SnapUInt32 value)
        {
            KeyValuePair<uint, uint> kvp = m_items[index];
            key.Value = kvp.Key;
            value.Value = kvp.Value;
        }

        public override void GetInSequence(int index, SnapUInt32 key, SnapUInt32 value)
        {
            key.Value = m_sortedItems.Keys[index];
            value.Value = m_sortedItems.Values[index];
        }
    }

    [TestFixture]
    public class FixedSizeNodeTest
    {
        private const int Max = 1000000;

        [Test]
        public void TestSequently()
        {
            SortedTreeNodeBase<SnapUInt32, SnapUInt32> tree = Library.SortedTreeNodes.CreateTreeNode<SnapUInt32, SnapUInt32>(SortedTree.FixedSizeNode, 0);

            LeafNodeTest.TestNode(tree, new SequentialTest(), 5000);
        }

        [Test]
        public void TestReverseSequently()
        {
            SortedTreeNodeBase<SnapUInt32, SnapUInt32> tree = Library.SortedTreeNodes.CreateTreeNode<SnapUInt32, SnapUInt32>(SortedTree.FixedSizeNode, 0);

            LeafNodeTest.TestNode(tree, new ReverseSequentialTest(), 5000);
        }

        [Test]
        public void TestRandom()
        {
            SortedTreeNodeBase<SnapUInt32, SnapUInt32> tree = Library.SortedTreeNodes.CreateTreeNode<SnapUInt32, SnapUInt32>(SortedTree.FixedSizeNode, 0);

            LeafNodeTest.TestNode(tree, new RandomTest(), 2000);
        }

        [Test]
        public void BenchmarkSequently()
        {
            var tree = Library.SortedTreeNodes.GetTreeNodeInitializer<SnapUInt32, SnapUInt32>(SortedTree.FixedSizeNode);

            LeafNodeTest.TestSpeed(tree, new SequentialTest(), 500, 512);
        }

        [Test]
        public void BenchmarkReverseSequently()
        {
            var tree = Library.SortedTreeNodes.GetTreeNodeInitializer<SnapUInt32, SnapUInt32>(SortedTree.FixedSizeNode);

            LeafNodeTest.TestSpeed(tree, new ReverseSequentialTest(), 500, 512);
        }

        [Test]
        public void BenchmarkRandom()
        {
            var tree = Library.SortedTreeNodes.GetTreeNodeInitializer<SnapUInt32, SnapUInt32>(SortedTree.FixedSizeNode);

            LeafNodeTest.TestSpeed(tree, new RandomTest(), 500, 512);
        }

        [Test]
        public void BenchmarkBigRandom()
        {
            var tree = Library.SortedTreeNodes.GetTreeNodeInitializer<SnapUInt32, SnapUInt32>(SortedTree.FixedSizeNode);

            LeafNodeTest.TestSpeed(tree, new RandomTest(), 5000, 4096);
        }

       
    }
}