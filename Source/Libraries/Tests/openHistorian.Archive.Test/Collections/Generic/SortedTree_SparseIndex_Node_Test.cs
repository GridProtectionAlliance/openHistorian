//using System;
//using System.Diagnostics;
//using GSF.IO.Unmanaged;
//using NUnit.Framework;

//namespace openHistorian.Collections.Generic
//{
//    [TestFixture]
//    public class SortedTree_SparseIndex_Node_Test
//    {
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
//                var node = new SparseIndex<KeyValue256, KeyValue256Methods>.Node(bs, 1024, 1);
//                var k1 = new KeyValue256();
//                var k2 = new KeyValue256();
//                var k3 = new KeyValue256();
//                var k4 = new KeyValue256();
//                k1.Key1 = 1000;
//                k2.Key1 = 2000;
//                k3.Key1 = 3000;
//                k4.Key1 = 4000;

//                node.CreateRootNode(0, 1, 100, k2, 200);

//                Assert.AreEqual(0u, node.NodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);
//                Assert.AreEqual(100u, node.GetFirstPointer());
//                Assert.AreEqual(200u, node.GetLastPointer());

//                Assert.AreEqual(100u, node.Get(k1));
//                Assert.AreEqual(200u, node.Get(k2));
//                Assert.AreEqual(200u, node.Get(k3));
//                Assert.AreEqual(200u, node.Get(k4));

//                node.LoadNode(0);

//                Assert.AreEqual(0u, node.NodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);
//                Assert.AreEqual(100u, node.GetFirstPointer());
//                Assert.AreEqual(200u, node.GetLastPointer());

//                Assert.AreEqual(100u, node.Get(k1));
//                Assert.AreEqual(200u, node.Get(k2));
//                Assert.AreEqual(200u, node.Get(k3));
//                Assert.AreEqual(200u, node.Get(k4));

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
//                var node = new SparseIndex<KeyValue256, KeyValue256Methods>.Node(bs, 1024, 1);
//                var k1 = new KeyValue256();
//                var k2 = new KeyValue256();
//                var k3 = new KeyValue256();
//                var k4 = new KeyValue256();
//                k1.Key1 = 1000;
//                k2.Key1 = 2000;
//                k3.Key1 = 3000;
//                k4.Key1 = 4000;

//                node.CreateRootNode(0, 1, 100, k2, 200);
//                node.Insert(k1, 1000);
//                node.Insert(k3, 3000);
//                node.Insert(k4, 4000);

//                Assert.AreEqual((ushort)4, node.RecordCount);
//                Assert.AreEqual(100u, node.GetFirstPointer());
//                Assert.AreEqual(4000u, node.GetLastPointer());

//                Assert.AreEqual(1000u, node.Get(k1));
//                Assert.AreEqual(200u, node.Get(k2));
//                Assert.AreEqual(3000u, node.Get(k3));
//                Assert.AreEqual(4000u, node.Get(k4));

//                node.LoadNode(0);

//                Assert.AreEqual(100u, node.GetFirstPointer());
//                Assert.AreEqual(4000u, node.GetLastPointer());

//                Assert.AreEqual(1000u, node.Get(k1));
//                Assert.AreEqual(200u, node.Get(k2));
//                Assert.AreEqual(3000u, node.Get(k3));
//                Assert.AreEqual(4000u, node.Get(k4));

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
//                var node = new SparseIndex<KeyValue256, KeyValue256Methods>.Node(bs, 1024, 1);
//                var k0 = new KeyValue256();
//                var k1 = new KeyValue256();
//                var k2 = new KeyValue256();
//                var k3 = new KeyValue256();
//                var k4 = new KeyValue256();
//                var k5 = new KeyValue256();
//                k0.Key1 = 1;
//                k1.Key1 = 1000;
//                k2.Key1 = 2000;
//                k3.Key1 = 3000;
//                k4.Key1 = 4000;
//                k5.Key1 = 5000;

//                node.CreateRootNode(0, 1, 100, k2, 2000);
//                node.Insert(k5, 5000);
//                node.Insert(k3, 3000);
//                node.Insert(k1, 1000);
//                node.Insert(k4, 4000);

//                node.Split(1);

//                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(1u, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)2, node.RecordCount);
//                Assert.AreEqual(100u, node.GetFirstPointer());
//                Assert.AreEqual(2000u, node.GetLastPointer());
//                Assert.AreEqual(0ul, node.LowerKey.Key1);
//                Assert.AreEqual(3000ul, node.UpperKey.Key1);

//                Assert.AreEqual(100u, node.Get(k0));
//                Assert.AreEqual(1000u, node.Get(k1));
//                Assert.AreEqual(2000u, node.Get(k2));
//                Assert.AreEqual(2000u, node.Get(k3));
//                Assert.AreEqual(2000u, node.Get(k4));
//                Assert.AreEqual(2000u, node.Get(k5));

//                node.SeekToRightSibling();

//                Assert.AreEqual(0u, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)2, node.RecordCount);
//                Assert.AreEqual(3000u, node.GetFirstPointer());
//                Assert.AreEqual(5000u, node.GetLastPointer());
//                Assert.AreEqual(3000ul, node.LowerKey.Key1);
//                Assert.AreEqual(0ul, node.UpperKey.Key1);

//                Assert.AreEqual(3000u, node.Get(k0));
//                Assert.AreEqual(3000u, node.Get(k1));
//                Assert.AreEqual(3000u, node.Get(k2));
//                Assert.AreEqual(3000u, node.Get(k3));
//                Assert.AreEqual(4000u, node.Get(k4));
//                Assert.AreEqual(5000u, node.Get(k5));

//                node.SeekToLeftSibling();

//                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(1u, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)2, node.RecordCount);
//                Assert.AreEqual(100u, node.GetFirstPointer());
//                Assert.AreEqual(2000u, node.GetLastPointer());
//                Assert.AreEqual(0ul, node.LowerKey.Key1);
//                Assert.AreEqual(3000ul, node.UpperKey.Key1);

//                Assert.AreEqual(100u, node.Get(k0));
//                Assert.AreEqual(1000u, node.Get(k1));
//                Assert.AreEqual(2000u, node.Get(k2));
//                Assert.AreEqual(2000u, node.Get(k3));
//                Assert.AreEqual(2000u, node.Get(k4));
//                Assert.AreEqual(2000u, node.Get(k5));


//                node.Insert(k3, 303);
//                node.Insert(k4, 404);
//                node.Insert(k5, 505);

//                node.Split(2);

//                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(2u, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)2, node.RecordCount);
//                Assert.AreEqual(100u, node.GetFirstPointer());
//                Assert.AreEqual(2000u, node.GetLastPointer());
//                Assert.AreEqual(0ul, node.LowerKey.Key1);
//                Assert.AreEqual(3000ul, node.UpperKey.Key1);

//                Assert.AreEqual(100u, node.Get(k0));
//                Assert.AreEqual(1000u, node.Get(k1));
//                Assert.AreEqual(2000u, node.Get(k2));
//                Assert.AreEqual(2000u, node.Get(k3));
//                Assert.AreEqual(2000u, node.Get(k4));
//                Assert.AreEqual(2000u, node.Get(k5));

//                node.SeekToRightSibling();

//                Assert.AreEqual(0u, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(1u, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)2, node.RecordCount);
//                Assert.AreEqual(303u, node.GetFirstPointer());
//                Assert.AreEqual(505u, node.GetLastPointer());
//                Assert.AreEqual(3000ul, node.LowerKey.Key1);
//                Assert.AreEqual(3000ul, node.UpperKey.Key1);

//                Assert.AreEqual(303u, node.Get(k0));
//                Assert.AreEqual(303u, node.Get(k1));
//                Assert.AreEqual(303u, node.Get(k2));
//                Assert.AreEqual(303u, node.Get(k3));
//                Assert.AreEqual(404u, node.Get(k4));
//                Assert.AreEqual(505u, node.Get(k5));

//                node.SeekToRightSibling();

//                Assert.AreEqual(2u, node.LeftSiblingNodeIndex);
//                Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);
//                Assert.AreEqual((ushort)2, node.RecordCount);
//                Assert.AreEqual(3000u, node.GetFirstPointer());
//                Assert.AreEqual(5000u, node.GetLastPointer());
//                Assert.AreEqual(3000ul, node.LowerKey.Key1);
//                Assert.AreEqual(0ul, node.UpperKey.Key1);

//                Assert.AreEqual(3000u, node.Get(k0));
//                Assert.AreEqual(3000u, node.Get(k1));
//                Assert.AreEqual(3000u, node.Get(k2));
//                Assert.AreEqual(3000u, node.Get(k3));
//                Assert.AreEqual(4000u, node.Get(k4));
//                Assert.AreEqual(5000u, node.Get(k5));
//            }
//        }

//    }
//}

