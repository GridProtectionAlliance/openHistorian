//using System;
//using NUnit.Framework;
//using GSF.IO;
//using GSF.IO.Unmanaged;

//namespace openHistorian.Collections
//{
//    [TestFixture]
//    class SortedTree256TS32EncodedTest
//    {

//        class TestEncoding : SortedTree256TS32Encoded
//        {
//            Random r = new Random(4);

//            public TestEncoding(BinaryStreamBase stream, int blockSize)
//                : base(stream, blockSize)
//            {
//            }

//            public unsafe void TestEncodingMethod()
//            {
//                for (int x = 0; x < 1000; x++)
//                {
//                    TestDelta(0); //Values are the same
//                    TestDelta(GetRandomValue(3)); //values require at least 0 extra byte
//                    TestDelta(GetRandomValue(3 + 8)); //values require at least 1 extra byte
//                    TestDelta(GetRandomValue(3 + 16)); //values require at least 2 extra byte
//                    TestDelta(GetRandomValue(3 + 24)); //values require at least 3 extra byte
//                    TestDelta(GetRandomValue(3 + 32)); //values require at least 4 extra byte
//                    TestDelta(GetRandomValue(3 + 40)); //values require at least 5 extra byte
//                    TestDelta(GetRandomValue(3 + 48)); //values require at least 6 extra byte
//                    TestDelta(GetRandomValue(2 + 56)); //values require at least 7 extra byte
//                    TestDelta(GetRandomValue(64)); //values require at least 8 extra byte

//                    TestDelta2(0); //Values are the same
//                    TestDelta2(GetRandomValue(3)); //values require at least 0 extra byte
//                    TestDelta2(GetRandomValue(3 + 8)); //values require at least 1 extra byte
//                    TestDelta2(GetRandomValue(3 + 16)); //values require at least 2 extra byte
//                    TestDelta2(GetRandomValue(3 + 24)); //values require at least 3 extra byte
//                    TestDelta2(GetRandomValue(3 + 32)); //values require at least 4 extra byte
//                    TestDelta2(GetRandomValue(3 + 40)); //values require at least 5 extra byte
//                    TestDelta2(GetRandomValue(3 + 48)); //values require at least 6 extra byte
//                    TestDelta2(GetRandomValue(2 + 56)); //values require at least 7 extra byte
//                    TestDelta2(GetRandomValue(64)); //values require at least 8 extra byte
//                }
//            }

//            public ulong GetRandomValue(int maxBits)
//            {
//                byte[] data = new byte[8];
//                r.NextBytes(data);
//                if (maxBits >= 64)
//                    return BitConverter.ToUInt64(data, 0);
//                return BitConverter.ToUInt64(data, 0) & ((1ul << maxBits) - 1);
//            }

//            public void TestDelta(ulong delta)
//            {
//                TestDelta(delta, ulong.MinValue);
//                TestDelta(delta, ulong.MaxValue >> 1);
//                TestDelta(delta, ulong.MaxValue);
//            }

//            public void TestDelta(ulong delta,ulong baseValue)
//            {
//                TestBeforeAndAfter(1, 1, 1, baseValue, 1, 0, 1, baseValue);//Test Same
//                TestBeforeAndAfter(1, 1, 1, baseValue+delta, 1, 0, 1, baseValue);//Test Add
//                TestBeforeAndAfter(1, 1, 1, baseValue - delta, 1, 0, 1, baseValue);//Test Sub
//            }

//            public void TestDelta2(ulong delta)
//            {
//                TestDelta2(delta, ulong.MinValue);
//                TestDelta2(delta, ulong.MaxValue >> 1);
//                TestDelta2(delta, ulong.MaxValue);
//            }

//            public void TestDelta2(ulong delta, ulong baseValue)
//            {
//                TestBeforeAndAfter(delta, delta, delta, delta, baseValue, baseValue, baseValue, baseValue);//Test Same
//                TestBeforeAndAfter(delta + baseValue, delta + baseValue, delta + baseValue, delta + baseValue, baseValue, baseValue, baseValue, baseValue);//Test Same
//                TestBeforeAndAfter(delta - baseValue, delta - baseValue, delta - baseValue, delta - baseValue, baseValue, baseValue, baseValue, baseValue);//Test Same
//                TestBeforeAndAfter(delta - baseValue, delta + baseValue, delta + baseValue, delta - baseValue, baseValue, baseValue, baseValue, baseValue);//Test Same
//                TestBeforeAndAfter(delta - baseValue, delta + baseValue, delta - baseValue, delta + baseValue, baseValue, baseValue, baseValue, baseValue);//Test Same
//            }

//            public unsafe void TestBeforeAndAfter(ulong key1, ulong key2, ulong value1, ulong value2,
//                ulong prevKey1, ulong prevKey2, ulong prevValue1, ulong prevValue2)
//            {
//                int before1 = r.Next();
//                int before2 = r.Next();
//                int after1 = r.Next();
//                int after2 = r.Next();

//                int length;
//                base.StreamLeaf.Position = 234;
//                base.StreamLeaf.Write(before1);
//                base.StreamLeaf.Write(before2);
//                byte[] result = new byte[64];
//                fixed (byte* lp = result)
//                {
//                    length = base.EncodeRecord(lp, key1, key2, value1, value2, prevKey1, prevKey2, prevValue1, prevValue2);
//                    base.StreamLeaf.Write(result, 0, length);
//                    base.StreamLeaf.Write(after1);
//                    base.StreamLeaf.Write(after2);
//                }
//                base.StreamLeaf.Position = 234 + 4 + 4;

//                base.DecodeNextRecord(ref prevKey1, ref prevKey2, ref prevValue1, ref prevValue2);
//                Assert.AreEqual(prevKey1, key1);
//                Assert.AreEqual(prevKey2, key2);
//                Assert.AreEqual(prevValue1, value1);
//                Assert.AreEqual(prevValue2, value2);
//                Assert.AreEqual(base.StreamLeaf.Position, 234L + 4 + 4 + length);

//                base.StreamLeaf.Position = 234 + 4 + 4;
//                base.StreamLeaf.Position = 234;
//                Assert.AreEqual(before1, StreamLeaf.ReadInt32());
//                Assert.AreEqual(before2, StreamLeaf.ReadInt32());

//                base.StreamLeaf.Position = 234L + 4 + 4 + length;
//                Assert.AreEqual(after1, StreamLeaf.ReadInt32());
//                Assert.AreEqual(after2, StreamLeaf.ReadInt32());
//            }


//        }

//        [Test]
//        public void TestEncodingData()
//        {
//            using (BinaryStream bs = new BinaryStream())
//            {
//                TestEncoding test = new TestEncoding(bs, 4096);
//                test.TestEncodingMethod();

//            }
//        }

//    }
//}

