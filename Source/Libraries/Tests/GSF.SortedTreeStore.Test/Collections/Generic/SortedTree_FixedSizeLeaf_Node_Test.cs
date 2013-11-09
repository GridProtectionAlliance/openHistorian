//using System;
//using System.Diagnostics;
//using GSF.IO.Unmanaged;
//using NUnit.Framework;

//namespace openHistorian.Collections.Generic
//{
//    [TestFixture]
//    public class SortedTree_FixedSizeLeaf_Node_Test
//    {
//        KeyValue256Methods m_methods;
//        const int Max = 1000000;

//        [Test]
//        public void Test_CreateRootNode_Get()
//        {
//            uint rootKey = 0;
//            byte rootLevel = 0;

//            uint nextKeyIndex = 0;
//            Func<uint> getNextKey = () =>
//            {
//                nextKeyIndex++;
//                return nextKeyIndex - 1;
//            };

//            Stopwatch swWrite = new Stopwatch();
//            Stopwatch swRead = new Stopwatch();
//            using (var bs = new BinaryStream())
//            {
//                var node = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionNone>.FixedSizeLeaf.Node(bs, 1024);
//                var tmp = new KeyValue256();
//                var k1 = new KeyValue256();
//                var k2 = new KeyValue256();
//                var k3 = new KeyValue256();
//                var k4 = new KeyValue256();
//                k1.Key1 = 1000;
//                k2.Key1 = 2000;
//                k3.Key1 = 3000;
//                k4.Key1 = 4000;

//                node.CreateEmptyNode(0);

//                Assert.AreEqual(0u, node.NodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);

//                node.LoadNode(0);

//                Assert.AreEqual(0u, node.NodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);
//            }
//        }

//        [Test]
//        public void Test_Insert()
//        {
//            uint rootKey = 0;
//            byte rootLevel = 0;

//            uint nextKeyIndex = 0;
//            Func<uint> getNextKey = () =>
//            {
//                nextKeyIndex++;
//                return nextKeyIndex - 1;
//            };

//            Stopwatch swWrite = new Stopwatch();
//            Stopwatch swRead = new Stopwatch();
//            using (var bs = new BinaryStream())
//            {
//                var r = new Random();
//                Func<ulong> GetLong = () => ((ulong)r.Next()) << 32 | (ulong)r.Next();


//                var node = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionNone>.FixedSizeLeaf.Node(bs, 1024);
//                var tmp = new KeyValue256();
//                var k1 = new KeyValue256(1000ul, GetLong(), GetLong(), GetLong());
//                var k2 = new KeyValue256(2000ul, GetLong(), GetLong(), GetLong());
//                var k3 = new KeyValue256(3000ul, GetLong(), GetLong(), GetLong());
//                var k4 = new KeyValue256(4000ul, GetLong(), GetLong(), GetLong());

//                node.CreateEmptyNode(0);
//                node.Insert(k1);
//                node.Insert(k3);
//                node.Insert(k4);

//                Assert.AreEqual((ushort)3, node.RecordCount);
//                node.GetFirstValue(tmp);
//                Assert.IsTrue(m_methods.IsExact(k1, tmp));
//                node.GetLastValue(tmp);
//                Assert.IsTrue(m_methods.IsExact(k4, tmp));

//                Assert.IsTrue(node.TryGet(k1, tmp));
//                Assert.IsTrue(m_methods.IsExact(k1, tmp));

//                Assert.IsFalse(node.TryGet(k2, tmp));

//                Assert.IsTrue(node.TryGet(k3, tmp));
//                Assert.IsTrue(m_methods.IsExact(k3, tmp));

//                Assert.IsTrue(node.TryGet(k4, tmp));
//                Assert.IsTrue(m_methods.IsExact(k4, tmp));

//                node.LoadNode(0);

//                Assert.AreEqual((ushort)3, node.RecordCount);
//                node.GetFirstValue(tmp);
//                Assert.IsTrue(m_methods.IsExact(k1, tmp));
//                node.GetLastValue(tmp);
//                Assert.IsTrue(m_methods.IsExact(k4, tmp));

//                Assert.IsTrue(node.TryGet(k1, tmp));
//                Assert.IsTrue(m_methods.IsExact(k1, tmp));

//                Assert.IsFalse(node.TryGet(k2, tmp));

//                Assert.IsTrue(node.TryGet(k3, tmp));
//                Assert.IsTrue(m_methods.IsExact(k3, tmp));

//                Assert.IsTrue(node.TryGet(k4, tmp));
//                Assert.IsTrue(m_methods.IsExact(k4, tmp));

//            }
//        }

//        [Test]
//        public void Test_Split()
//        {
//            uint rootKey = 0;
//            byte rootLevel = 0;

//            uint nextKeyIndex = 0;
//            Func<uint> getNextKey = () =>
//            {
//                nextKeyIndex++;
//                return nextKeyIndex - 1;
//            };

//            Stopwatch swWrite = new Stopwatch();
//            Stopwatch swRead = new Stopwatch();
//            using (var bs = new BinaryStream())
//            {
//                var r = new Random();
//                Func<ulong> GetLong = () => ((ulong)r.Next()) << 32 | (ulong)r.Next();


//                var node = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionNone>.FixedSizeLeaf.Node(bs, 1024);
//                var tmp = new KeyValue256();
//                var k1 = new KeyValue256(1000ul, GetLong(), GetLong(), GetLong());
//                var k2 = new KeyValue256(2000ul, GetLong(), GetLong(), GetLong());
//                var k3 = new KeyValue256(3000ul, GetLong(), GetLong(), GetLong());
//                var k4 = new KeyValue256(4000ul, GetLong(), GetLong(), GetLong());
//                var k5 = new KeyValue256(5000ul, GetLong(), GetLong(), GetLong());
//                var k6 = new KeyValue256(6000ul, GetLong(), GetLong(), GetLong());

//                node.CreateEmptyNode(0);

//                node.Insert(k2);
//                node.Insert(k5);
//                node.Insert(k3);
//                node.Insert(k1);
//                node.Insert(k4);
//                node.Insert(k6);

//                node.Split(1);

//                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(1u, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)3, node.RecordCount);

//                node.GetFirstValue(tmp);
//                Assert.IsTrue(m_methods.IsExact(k1, tmp));
//                node.GetLastValue(tmp);
//                Assert.IsTrue(m_methods.IsExact(k3, tmp));

//                m_methods.Clear(tmp);
//                Assert.IsTrue(m_methods.IsExact(tmp, node.LowerKey));
//                Assert.IsTrue(m_methods.IsEqual(k4, node.UpperKey));

//                Assert.IsTrue(node.TryGet(k1, tmp));
//                Assert.IsTrue(m_methods.IsExact(k1, tmp));

//                Assert.IsTrue(node.TryGet(k2, tmp));
//                Assert.IsTrue(m_methods.IsExact(k2, tmp));

//                Assert.IsTrue(node.TryGet(k3, tmp));
//                Assert.IsTrue(m_methods.IsExact(k3, tmp));

//                Assert.IsFalse(node.TryGet(k4, tmp));

//                Assert.IsFalse(node.TryGet(k5, tmp));

//                Assert.IsFalse(node.TryGet(k6, tmp));

//                node.SeekToRightSibling();

//                Assert.AreEqual(0u, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)3, node.RecordCount);
//                node.GetFirstValue(tmp);
//                Assert.IsTrue(m_methods.IsExact(k4, tmp));
//                node.GetLastValue(tmp);
//                Assert.IsTrue(m_methods.IsExact(k6, tmp));

//                Assert.IsTrue(m_methods.IsEqual(k4, node.LowerKey));
//                m_methods.Clear(tmp);
//                Assert.IsTrue(m_methods.IsExact(tmp, node.UpperKey));

//                Assert.IsFalse(node.TryGet(k1, tmp));

//                Assert.IsFalse(node.TryGet(k2, tmp));

//                Assert.IsFalse(node.TryGet(k3, tmp));

//                Assert.IsTrue(node.TryGet(k4, tmp));
//                Assert.IsTrue(m_methods.IsExact(k4, tmp));

//                Assert.IsTrue(node.TryGet(k5, tmp));
//                Assert.IsTrue(m_methods.IsExact(k5, tmp));

//                Assert.IsTrue(node.TryGet(k6, tmp));
//                Assert.IsTrue(m_methods.IsExact(k6, tmp));

//                node.SeekToLeftSibling();

//                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(1u, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)3, node.RecordCount);
//                node.GetFirstValue(tmp);
//                Assert.IsTrue(m_methods.IsExact(k1, tmp));
//                node.GetLastValue(tmp);
//                Assert.IsTrue(m_methods.IsExact(k3, tmp));

//                m_methods.Clear(tmp);
//                Assert.IsTrue(m_methods.IsExact(tmp, node.LowerKey));
//                Assert.IsTrue(m_methods.IsEqual(k4, node.UpperKey));

//                Assert.IsTrue(node.TryGet(k1, tmp));
//                Assert.IsTrue(m_methods.IsExact(k1, tmp));

//                Assert.IsTrue(node.TryGet(k2, tmp));
//                Assert.IsTrue(m_methods.IsExact(k2, tmp));

//                Assert.IsTrue(node.TryGet(k3, tmp));
//                Assert.IsTrue(m_methods.IsExact(k3, tmp));

//                Assert.IsFalse(node.TryGet(k4, tmp));

//                Assert.IsFalse(node.TryGet(k5, tmp));

//                Assert.IsFalse(node.TryGet(k6, tmp));

//                node.Insert(k4);
//                node.Insert(k5);
//                node.Insert(k6);

//                node.Split(2);

//                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(2u, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)3, node.RecordCount);

//                node.SeekToRightSibling();

//                Assert.AreEqual(0u, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(1u, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)3, node.RecordCount);

//                node.SeekToRightSibling();

//                Assert.AreEqual(2u, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)3, node.RecordCount);
//            }
//        }

//    }
//}

