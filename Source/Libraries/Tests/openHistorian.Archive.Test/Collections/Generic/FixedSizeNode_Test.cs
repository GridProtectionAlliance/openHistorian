using System;
using System.Diagnostics;
using GSF.IO.Unmanaged;
using NUnit.Framework;

namespace openHistorian.Collections.Generic
{
    public static class Extension_FixedSizeNode_uint_uint
    {
        public static uint Get(this FixedSizeNode<Box<uint>, Box<uint>> tree, uint key)
        {
            Box<uint> k = new Box<uint>(key);
            Box<uint> v = new Box<uint>();
            if (!tree.TryGet(k, v))
                throw new Exception();
            return v.Value;
        }

        public static bool TryGet(this FixedSizeNode<Box<uint>, Box<uint>> tree, uint key, out uint value)
        {
            Box<uint> k = new Box<uint>(key);
            Box<uint> v = new Box<uint>();
            bool rv = tree.TryGet(k, v);
            value = v.Value;
            return rv;
        }

        public static uint GetOrGetNext(this FixedSizeNode<Box<uint>, Box<uint>> tree, uint key)
        {
            Box<uint> k = new Box<uint>(key);
            Box<uint> v = new Box<uint>();
            tree.GetOrGetNext(k, v);
            return v.Value;
        }

        public static bool TryInsert(this FixedSizeNode<Box<uint>, Box<uint>> tree, uint key, uint value)
        {
            Box<uint> k = new Box<uint>(key);
            Box<uint> v = new Box<uint>(value);
            bool rv = tree.TryInsert(k, v);
            return rv;
        }

        public static void Insert(this FixedSizeNode<Box<uint>, Box<uint>> tree, uint key, uint value)
        {
            Box<uint> k = new Box<uint>(key);
            Box<uint> v = new Box<uint>(value);
            if (!tree.TryInsert(k, v))
                throw new Exception();
        }

        public static void GetFirstKeyValue(this FixedSizeNode<Box<uint>, Box<uint>> tree, out uint key, out uint value)
        {
            Box<uint> k = new Box<uint>();
            Box<uint> v = new Box<uint>();
            tree.TryGetFirstRecord(k, v);
            key = k.Value;
            value = v.Value;
        }

        public static uint GetFirstKey(this FixedSizeNode<Box<uint>, Box<uint>> tree)
        {
            Box<uint> k = new Box<uint>();
            Box<uint> v = new Box<uint>();
            tree.TryGetFirstRecord(k, v);
            return k.Value;
        }

        public static uint GetFirstValue(this FixedSizeNode<Box<uint>, Box<uint>> tree)
        {
            Box<uint> k = new Box<uint>();
            Box<uint> v = new Box<uint>();
            tree.TryGetFirstRecord(k, v);
            return v.Value;
        }

        public static void GetLastKeyValue(this FixedSizeNode<Box<uint>, Box<uint>> tree, out uint key, out uint value)
        {
            Box<uint> k = new Box<uint>();
            Box<uint> v = new Box<uint>();
            tree.TryGetLastRecord(k, v);
            key = k.Value;
            value = v.Value;
        }

        public static uint GetLastKey(this FixedSizeNode<Box<uint>, Box<uint>> tree)
        {
            Box<uint> k = new Box<uint>();
            Box<uint> v = new Box<uint>();
            tree.TryGetLastRecord(k, v);
            return k.Value;
        }

        public static uint GetLastValue(this FixedSizeNode<Box<uint>, Box<uint>> tree)
        {
            Box<uint> k = new Box<uint>();
            Box<uint> v = new Box<uint>();
            tree.TryGetLastRecord(k, v);
            return v.Value;
        }

        public static bool KeyInsideBounds(this FixedSizeNode<Box<uint>, Box<uint>> tree, uint key)
        {
            Box<uint> k = new Box<uint>(key);
            return tree.IsKeyInsideBounds(k);
        }

        public static uint UpperKey(this FixedSizeNode<Box<uint>, Box<uint>> tree)
        {
            return tree.UpperKey.Value;
        }

        public static uint LowerKey(this FixedSizeNode<Box<uint>, Box<uint>> tree)
        {
            return tree.LowerKey.Value;
        }
    }

    [TestFixture]
    public class FixedSizeNode_Test
    {
        private const int Max = 1000000;

        [Test]
        public void Test_CreateRootNode_Get()
        {
            uint rootKey = 0;
            byte rootLevel = 0;

            uint nextKeyIndex = 0;
            Func<uint> getNextKey = () =>
            {
                nextKeyIndex++;
                return nextKeyIndex - 1;
            };
            Action<Box<uint>, uint, byte> addToParent = (int32, u, arg3) => int32 = int32;
            Func<Box<uint>, uint> findLeafNode = int32 => 0;

            Stopwatch swWrite = new Stopwatch();
            Stopwatch swRead = new Stopwatch();
            using (BinaryStream bs = new BinaryStream())
            {
                uint k, v;
                FixedSizeNode<Box<uint>, Box<uint>> node = new FixedSizeNode<Box<uint>, Box<uint>>(0, new KeyMethodsUInt32(), new ValueMethodsUInt32());

                node.Initialize(bs, 1024, getNextKey, null);

                node.CreateEmptyNode(0);
                node.Insert(1, 100);
                node.Insert(2, 200);

                Assert.AreEqual(0u, node.NodeIndex);
                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
                Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);

                Assert.AreEqual(1u, node.GetFirstKey());
                Assert.AreEqual(100u, node.GetFirstValue());
                Assert.AreEqual(2u, node.GetLastKey());
                Assert.AreEqual(200u, node.GetLastValue());

                Assert.AreEqual(100u, node.Get(1));
                Assert.AreEqual(200u, node.Get(2));
                Assert.AreEqual(100u, node.GetOrGetNext(1));
                Assert.AreEqual(200u, node.GetOrGetNext(2));
                Assert.AreEqual(200u, node.GetOrGetNext(3));

                node.SetNodeIndex(0);

                Assert.AreEqual(0u, node.NodeIndex);
                Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
                Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);

                Assert.AreEqual(1u, node.GetFirstKey());
                Assert.AreEqual(100u, node.GetFirstValue());
                Assert.AreEqual(2u, node.GetLastKey());
                Assert.AreEqual(200u, node.GetLastValue());

                Assert.AreEqual(100u, node.Get(1));
                Assert.AreEqual(200u, node.Get(2));
                Assert.AreEqual(100u, node.GetOrGetNext(1));
                Assert.AreEqual(200u, node.GetOrGetNext(2));
                Assert.AreEqual(200u, node.GetOrGetNext(3));
            }
        }

        //[Test]
        //public void Test_Split()
        //{
        //    uint rootKey = 0;
        //    byte rootLevel = 0;

        //    uint nextKeyIndex = 0;
        //    Func<uint> getNextKey = () =>
        //    {
        //        nextKeyIndex++;
        //        return nextKeyIndex - 1;
        //    };

        //    Stopwatch swWrite = new Stopwatch();
        //    Stopwatch swRead = new Stopwatch();
        //    using (var bs = new BinaryStream())
        //    {
        //        var node = new SparseIndex<KeyValue256, KeyValue256Methods>.Node(bs, 1024, 1);
        //        var k0 = new KeyValue256();
        //        var k1 = new KeyValue256();
        //        var k2 = new KeyValue256();
        //        var k3 = new KeyValue256();
        //        var k4 = new KeyValue256();
        //        var k5 = new KeyValue256();
        //        k0.Key1 = 1;
        //        k1.Key1 = 1000;
        //        k2.Key1 = 2000;
        //        k3.Key1 = 3000;
        //        k4.Key1 = 4000;
        //        k5.Key1 = 5000;

        //        node.CreateRootNode(0, 1, 100, k2, 2000);
        //        node.Insert(k5, 5000);
        //        node.Insert(k3, 3000);
        //        node.Insert(k1, 1000);
        //        node.Insert(k4, 4000);

        //        node.Split(1);

        //        Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
        //        Assert.AreEqual(1u, node.RightSiblingNodeIndex);
        //        Assert.AreEqual((ushort)2, node.RecordCount);
        //        Assert.AreEqual(100u, node.GetFirstPointer());
        //        Assert.AreEqual(2000u, node.GetLastPointer());
        //        Assert.AreEqual(0ul, node.LowerKey.Key1);
        //        Assert.AreEqual(3000ul, node.UpperKey.Key1);

        //        Assert.AreEqual(100u, node.Get(k0));
        //        Assert.AreEqual(1000u, node.Get(k1));
        //        Assert.AreEqual(2000u, node.Get(k2));
        //        Assert.AreEqual(2000u, node.Get(k3));
        //        Assert.AreEqual(2000u, node.Get(k4));
        //        Assert.AreEqual(2000u, node.Get(k5));

        //        node.SeekToRightSibling();

        //        Assert.AreEqual(0u, node.LeftSiblingNodeIndex);
        //        Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);
        //        Assert.AreEqual((ushort)2, node.RecordCount);
        //        Assert.AreEqual(3000u, node.GetFirstPointer());
        //        Assert.AreEqual(5000u, node.GetLastPointer());
        //        Assert.AreEqual(3000ul, node.LowerKey.Key1);
        //        Assert.AreEqual(0ul, node.UpperKey.Key1);

        //        Assert.AreEqual(3000u, node.Get(k0));
        //        Assert.AreEqual(3000u, node.Get(k1));
        //        Assert.AreEqual(3000u, node.Get(k2));
        //        Assert.AreEqual(3000u, node.Get(k3));
        //        Assert.AreEqual(4000u, node.Get(k4));
        //        Assert.AreEqual(5000u, node.Get(k5));

        //        node.SeekToLeftSibling();

        //        Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
        //        Assert.AreEqual(1u, node.RightSiblingNodeIndex);
        //        Assert.AreEqual((ushort)2, node.RecordCount);
        //        Assert.AreEqual(100u, node.GetFirstPointer());
        //        Assert.AreEqual(2000u, node.GetLastPointer());
        //        Assert.AreEqual(0ul, node.LowerKey.Key1);
        //        Assert.AreEqual(3000ul, node.UpperKey.Key1);

        //        Assert.AreEqual(100u, node.Get(k0));
        //        Assert.AreEqual(1000u, node.Get(k1));
        //        Assert.AreEqual(2000u, node.Get(k2));
        //        Assert.AreEqual(2000u, node.Get(k3));
        //        Assert.AreEqual(2000u, node.Get(k4));
        //        Assert.AreEqual(2000u, node.Get(k5));


        //        node.Insert(k3, 303);
        //        node.Insert(k4, 404);
        //        node.Insert(k5, 505);

        //        node.Split(2);

        //        Assert.AreEqual(uint.MaxValue, node.LeftSiblingNodeIndex);
        //        Assert.AreEqual(2u, node.RightSiblingNodeIndex);
        //        Assert.AreEqual((ushort)2, node.RecordCount);
        //        Assert.AreEqual(100u, node.GetFirstPointer());
        //        Assert.AreEqual(2000u, node.GetLastPointer());
        //        Assert.AreEqual(0ul, node.LowerKey.Key1);
        //        Assert.AreEqual(3000ul, node.UpperKey.Key1);

        //        Assert.AreEqual(100u, node.Get(k0));
        //        Assert.AreEqual(1000u, node.Get(k1));
        //        Assert.AreEqual(2000u, node.Get(k2));
        //        Assert.AreEqual(2000u, node.Get(k3));
        //        Assert.AreEqual(2000u, node.Get(k4));
        //        Assert.AreEqual(2000u, node.Get(k5));

        //        node.SeekToRightSibling();

        //        Assert.AreEqual(0u, node.LeftSiblingNodeIndex);
        //        Assert.AreEqual(1u, node.RightSiblingNodeIndex);
        //        Assert.AreEqual((ushort)2, node.RecordCount);
        //        Assert.AreEqual(303u, node.GetFirstPointer());
        //        Assert.AreEqual(505u, node.GetLastPointer());
        //        Assert.AreEqual(3000ul, node.LowerKey.Key1);
        //        Assert.AreEqual(3000ul, node.UpperKey.Key1);

        //        Assert.AreEqual(303u, node.Get(k0));
        //        Assert.AreEqual(303u, node.Get(k1));
        //        Assert.AreEqual(303u, node.Get(k2));
        //        Assert.AreEqual(303u, node.Get(k3));
        //        Assert.AreEqual(404u, node.Get(k4));
        //        Assert.AreEqual(505u, node.Get(k5));

        //        node.SeekToRightSibling();

        //        Assert.AreEqual(2u, node.LeftSiblingNodeIndex);
        //        Assert.AreEqual(uint.MaxValue, node.RightSiblingNodeIndex);
        //        Assert.AreEqual((ushort)2, node.RecordCount);
        //        Assert.AreEqual(3000u, node.GetFirstPointer());
        //        Assert.AreEqual(5000u, node.GetLastPointer());
        //        Assert.AreEqual(3000ul, node.LowerKey.Key1);
        //        Assert.AreEqual(0ul, node.UpperKey.Key1);

        //        Assert.AreEqual(3000u, node.Get(k0));
        //        Assert.AreEqual(3000u, node.Get(k1));
        //        Assert.AreEqual(3000u, node.Get(k2));
        //        Assert.AreEqual(3000u, node.Get(k3));
        //        Assert.AreEqual(4000u, node.Get(k4));
        //        Assert.AreEqual(5000u, node.Get(k5));
        //    }
        //}
    }
}