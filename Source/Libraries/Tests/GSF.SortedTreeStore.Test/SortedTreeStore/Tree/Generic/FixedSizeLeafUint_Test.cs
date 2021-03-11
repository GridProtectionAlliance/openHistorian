using System;
using System.Collections.Generic;
using GSF.Snap.Types;
using NUnit.Framework;

namespace GSF.Snap.Tree
{
    internal class SequentialTestInt
        : TreeNodeRandomizerBase<SnapInt32, SnapInt32>
    {
        private readonly SortedList<int, int> m_sortedItems = new SortedList<int, int>();
        private readonly List<KeyValuePair<int, int>> m_items = new List<KeyValuePair<int, int>>();

        private int m_maxCount;
        private int m_current;

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
            m_items.Add(new KeyValuePair<int, int>(m_current, m_current * 2));
            m_current++;
        }

        public override void GetRandom(int index, SnapInt32 key, SnapInt32 value)
        {
            KeyValuePair<int, int> kvp = m_items[index];
            key.Value = kvp.Key;
            value.Value = kvp.Value;
        }

        public override void GetInSequence(int index, SnapInt32 key, SnapInt32 value)
        {
            key.Value = m_sortedItems.Keys[index];
            value.Value = m_sortedItems.Values[index];
        }
    }

    internal class ReverseSequentialTestInt
        : TreeNodeRandomizerBase<SnapInt32, SnapInt32>
    {
        private readonly SortedList<int, int> m_sortedItems = new SortedList<int, int>();
        private readonly List<KeyValuePair<int, int>> m_items = new List<KeyValuePair<int, int>>();

        private int m_maxCount;
        private int m_current;

        public override void Reset(int maxCount)
        {
            m_sortedItems.Clear();
            m_items.Clear();
            m_maxCount = maxCount;
            m_current = maxCount;
        }

        public override void Next()
        {
            m_sortedItems.Add(m_current, m_current * 2);
            m_items.Add(new KeyValuePair<int, int>(m_current, m_current * 2));
            m_current--;
        }

        public override void GetRandom(int index, SnapInt32 key, SnapInt32 value)
        {
            KeyValuePair<int, int> kvp = m_items[index];
            key.Value = kvp.Key;
            value.Value = kvp.Value;
        }

        public override void GetInSequence(int index, SnapInt32 key, SnapInt32 value)
        {
            key.Value = m_sortedItems.Keys[index];
            value.Value = m_sortedItems.Values[index];
        }
    }

    internal class RandomTestInt
        : TreeNodeRandomizerBase<SnapInt32, SnapInt32>
    {
        private readonly SortedList<int, int> m_sortedItems = new SortedList<int, int>();
        private readonly List<KeyValuePair<int, int>> m_items = new List<KeyValuePair<int, int>>();

        private Random r;

        public override void Reset(int maxCount)
        {
            r = new Random(1);
            m_sortedItems.Clear();
            m_items.Clear();
        }

        public override void Next()
        {
            int rand = r.Next();
            m_sortedItems.Add(rand, rand * 2);
            m_items.Add(new KeyValuePair<int, int>(rand, rand * 2));
        }

        public override void GetRandom(int index, SnapInt32 key, SnapInt32 value)
        {
            KeyValuePair<int, int> kvp = m_items[index];
            key.Value = kvp.Key;
            value.Value = kvp.Value;
        }

        public override void GetInSequence(int index, SnapInt32 key, SnapInt32 value)
        {
            key.Value = m_sortedItems.Keys[index];
            value.Value = m_sortedItems.Values[index];
        }
    }

    [TestFixture]
    public class FixedSizeNodeTestInt
    {
        private const int Max = 1000000;

        [Test]
        public void TestSequently()
        {
            SortedTreeNodeBase<SnapInt32, SnapInt32> tree = Library.CreateTreeNode<SnapInt32, SnapInt32>(EncodingDefinition.FixedSizeCombinedEncoding, 0);

            LeafNodeTest.TestNode(tree, new SequentialTestInt(), 5000);
        }

        [Test]
        public void TestReverseSequently()
        {
            SortedTreeNodeBase<SnapInt32, SnapInt32> tree = Library.CreateTreeNode<SnapInt32, SnapInt32>(EncodingDefinition.FixedSizeCombinedEncoding, 0);

            LeafNodeTest.TestNode(tree, new ReverseSequentialTestInt(), 5000);
        }

        [Test]
        public void TestRandom()
        {
            SortedTreeNodeBase<SnapInt32, SnapInt32> tree = Library.CreateTreeNode<SnapInt32, SnapInt32>(EncodingDefinition.FixedSizeCombinedEncoding, 0);

            LeafNodeTest.TestNode(tree, new RandomTestInt(), 2000);
        }

        [Test]
        public void BenchmarkSequently()
        {
            SortedTreeNodeBase<SnapInt32, SnapInt32> tree = Library.CreateTreeNode<SnapInt32, SnapInt32>(EncodingDefinition.FixedSizeCombinedEncoding, 0);

            LeafNodeTest.TestSpeed(tree, new SequentialTestInt(), 500, 512);
        }

        [Test]
        public void BenchmarkReverseSequently()
        {
            SortedTreeNodeBase<SnapInt32, SnapInt32> tree = Library.CreateTreeNode<SnapInt32, SnapInt32>(EncodingDefinition.FixedSizeCombinedEncoding, 0);

            LeafNodeTest.TestSpeed(tree, new ReverseSequentialTestInt(), 500, 512);
        }

        [Test]
        public void BenchmarkRandom()
        {
            SortedTreeNodeBase<SnapInt32, SnapInt32> tree = Library.CreateTreeNode<SnapInt32, SnapInt32>(EncodingDefinition.FixedSizeCombinedEncoding, 0);

            LeafNodeTest.TestSpeed(tree, new RandomTestInt(), 500, 512);
        }

        [Test]
        public void BenchmarkBigRandom()
        {
            SortedTreeNodeBase<SnapInt32, SnapInt32> tree = Library.CreateTreeNode<SnapInt32, SnapInt32>(EncodingDefinition.FixedSizeCombinedEncoding, 0);

            LeafNodeTest.TestSpeed(tree, new RandomTestInt(), 5000, 4096);
        }

        //        [Test]
        //        public void TestReverseSequently()
        //        {

        //            uint rootKey = 0;
        //            byte rootLevel = 0;

        //            uint nextKeyIndex = 0;
        //            bool hasChanged = false;
        //            Func<uint> getNextKey = () =>
        //            {
        //                nextKeyIndex++;
        //                return nextKeyIndex;
        //            };

        //            Stopwatch swWrite = new Stopwatch();
        //            Stopwatch swRead = new Stopwatch();
        //            using (var bs = new BinaryStream())
        //            {
        //                bs.Write(0);
        //                SparseIndexBase<KeyValue256> ln = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionNone>.SparseIndex(bs, 125 * 10, getNextKey);
        //                var k = new KeyValue256();
        //                ln.Initialize(0, 0);
        //                long index;

        //                swWrite.Start();
        //                for (uint x = 0; x < Max; x++)
        //                {
        //#if !SkipAssert
        //                    hasChanged = !(rootKey == ln.RootNodeIndexAddress && rootLevel == ln.RootNodeLevel);
        //                    Assert.AreEqual(hasChanged, ln.HasChanged);
        //                    if (hasChanged)
        //                    {
        //                        ln.Save(out rootLevel, out rootKey);
        //                        hasChanged = false;
        //                        Assert.AreEqual(hasChanged, ln.HasChanged);
        //                    }

        //#endif
        //                    k.Key1 = (2 * Max) - x;
        //                    ln.Add(k, x + 1);
        //                }
        //                swWrite.Stop();

        //                swRead.Start();
        //                for (uint x = 0; x < Max; x++)
        //                {
        //                    k.Key1 = (2 * Max) - x;
        //                    index = ln.Get(k);
        //#if !SkipAssert
        //                    Assert.AreEqual((long)x + 1, index);
        //#endif
        //                }
        //                swRead.Stop();

        //                ln = ln;
        //            }

        //            Console.WriteLine((Max / swRead.Elapsed.TotalSeconds / 1000000).ToString("0.000 MPS Read"));
        //            Console.WriteLine((Max / swWrite.Elapsed.TotalSeconds / 1000000).ToString("0.000 MPS Write"));
        //        }

        //        [Test]
        //        public void TestRandom()
        //        {
        //            uint rootKey = 0;
        //            byte rootLevel = 0;

        //            uint nextKeyIndex = 0;
        //            bool hasChanged = false;
        //            Func<uint> getNextKey = () =>
        //            {
        //                nextKeyIndex++;
        //                return nextKeyIndex;
        //            };

        //            Stopwatch swWrite = new Stopwatch();
        //            Stopwatch swRead = new Stopwatch();
        //            using (var bs = new BinaryStream())
        //            {
        //                bs.Write(0);
        //                SparseIndexBase<KeyValue256> ln = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionNone>.SparseIndex(bs, 125 * 10, getNextKey);
        //                var k = new KeyValue256();
        //                ln.Initialize(0, 0);
        //                long index;

        //                swWrite.Start();
        //                for (uint x = 0; x < Max; x++)
        //                {
        //#if !SkipAssert
        //                    hasChanged = !(rootKey == ln.RootNodeIndexAddress && rootLevel == ln.RootNodeLevel);
        //                    Assert.AreEqual(hasChanged, ln.HasChanged);
        //                    if (hasChanged)
        //                    {
        //                        ln.Save(out rootLevel, out rootKey);
        //                        hasChanged = false;
        //                        Assert.AreEqual(hasChanged, ln.HasChanged);
        //                    }

        //#endif
        //                    if ((x & 1) == 0)
        //                        k.Key1 = (2 * Max) - x;
        //                    else
        //                        k.Key1 = (2 * Max) + x;
        //                    ln.Add(k, x + 1);
        //                }
        //                swWrite.Stop();

        //                swRead.Start();
        //                for (uint x = 0; x < Max; x++)
        //                {
        //                    if ((x & 1) == 0)
        //                        k.Key1 = (2 * Max) - x;
        //                    else
        //                        k.Key1 = (2 * Max) + x;
        //                    index = ln.Get(k);
        //#if !SkipAssert
        //                    Assert.AreEqual((long)x + 1, index);
        //#endif
        //                }
        //                swRead.Stop();

        //                ln = ln;
        //            }

        //            Console.WriteLine((Max / swRead.Elapsed.TotalSeconds / 1000000).ToString("0.000 MPS Read"));
        //            Console.WriteLine((Max / swWrite.Elapsed.TotalSeconds / 1000000).ToString("0.000 MPS Write"));
        //        }


        //        [Test]
        //        public void TestTrueRandom()
        //        {
        //            int seed = 6;
        //            Random r = new Random(seed);
        //            uint rootKey = 0;
        //            byte rootLevel = 0;

        //            uint nextKeyIndex = 0;
        //            bool hasChanged = false;
        //            Func<uint> getNextKey = () =>
        //            {
        //                nextKeyIndex++;
        //                return nextKeyIndex;
        //            };

        //            Stopwatch swWrite = new Stopwatch();
        //            Stopwatch swRead = new Stopwatch();
        //            using (var bs = new BinaryStream())
        //            {
        //                bs.Write(0);
        //                SparseIndexBase<KeyValue256> ln = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionNone>.SparseIndex(bs, 125 * 10, getNextKey);
        //                var k = new KeyValue256();
        //                ln.Initialize(0, 0);
        //                long index;

        //                swWrite.Start();
        //                for (uint x = 0; x < Max; x++)
        //                {
        //#if !SkipAssert
        //                    hasChanged = !(rootKey == ln.RootNodeIndexAddress && rootLevel == ln.RootNodeLevel);
        //                    Assert.AreEqual(hasChanged, ln.HasChanged);
        //                    if (hasChanged)
        //                    {
        //                        ln.Save(out rootLevel, out rootKey);
        //                        hasChanged = false;
        //                        Assert.AreEqual(hasChanged, ln.HasChanged);
        //                    }
        //#endif

        //                    k.Key1 = ((ulong)r.Next()) << 32 | (ulong)r.Next();
        //                    k.Key2 = ((ulong)r.Next()) << 32 | (ulong)r.Next();
        //                    ln.Add(k, x + 1);
        //                }
        //                swWrite.Stop();

        //                r = new Random(seed);

        //                swRead.Start();
        //                for (uint x = 0; x < Max; x++)
        //                {
        //                    k.Key1 = ((ulong)r.Next()) << 32 | (ulong)r.Next();
        //                    k.Key2 = ((ulong)r.Next()) << 32 | (ulong)r.Next();
        //                    index = ln.Get(k);
        //#if !SkipAssert
        //                    Assert.AreEqual((long)x + 1, index);
        //#endif
        //                }
        //                swRead.Stop();

        //                ln = ln;
        //            }

        //            Console.WriteLine((Max / swRead.Elapsed.TotalSeconds / 1000000).ToString("0.000 MPS Read"));
        //            Console.WriteLine((Max / swWrite.Elapsed.TotalSeconds / 1000000).ToString("0.000 MPS Write"));
        //        }
    }
}