//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using openHistorian.Collections.Generic;

//namespace openHistorian.Collections
//{
//    [TestFixture]
//    public class SortedTree256MethodsTest
//    {
//        [Test]
//        public void TestClear()
//        {
//            KeyValue256Methods m = new KeyValue256Methods();

//            var k1 = m.Create();

//            k1.Key1 = 1;
//            k1.Key2 = 2;
//            k1.Value1 = 3;
//            k1.Value2 = 4;

//            Assert.IsTrue(k1.Key1 == 1);
//            Assert.IsTrue(k1.Key2 == 2);
//            Assert.IsTrue(k1.Value1 == 3);
//            Assert.IsTrue(k1.Value2 == 4);

//            m.Clear(k1);

//            Assert.IsTrue(k1.Key1 == 0);
//            Assert.IsTrue(k1.Key2 == 0);
//            Assert.IsTrue(k1.Value1 == 0);
//            Assert.IsTrue(k1.Value2 == 0);
//        }

//        [Test]
//        public void TestCompare()
//        {
//            KeyValue256Methods m = new KeyValue256Methods();

//            var k1 = m.Create();
//            var k2 = m.Create();

//            k1.Key1 = 1;
//            k1.Key2 = 2;
//            k1.Value1 = 3;
//            k1.Value2 = 4;

//            k2.Key1 = 1;
//            k2.Key2 = 2;
//            k2.Value1 = 3;
//            k2.Value2 = 4;

//            ExpectEqualTo(m, k1, k2);

//            k1.Value1 = 0;
//            k1.Value2 = 0;

//            ExpectEqualTo(m, k1, k2);

//            k1.Key1 = 2;

//            ExpectGreaterThan(m, k1, k2);

//            k1.Key1 = 1;
//            k1.Key2 = 3;

//            ExpectGreaterThan(m, k1, k2);

//            k1.Key2 = 1;

//            ExpectLessThan(m, k1, k2);

//            k1.Key2 = 2;
//            k1.Key1 = 0;

//            ExpectLessThan(m, k1, k2);

//            k1.Key2 = 3;

//            ExpectLessThan(m, k1, k2);

//            k1.Key1 = 2;
//            k1.Key2 = 0;

//            ExpectGreaterThan(m, k1, k2);


//        }

//        public void ExpectLessThan(SortedTreeMethodsBase<KeyValue256> m, KeyValue256 k1, KeyValue256 k2)
//        {
//            Assert.IsTrue(m.IsLessThan(k1, k2));
//            Assert.IsTrue(m.CompareTo(k1, k2) < 0);
//            Assert.IsTrue(m.IsLessThanOrEqualTo(k1, k2));

//            Assert.IsFalse(m.CompareTo(k1, k2) == 0);
//            Assert.IsFalse(m.IsEqual(k1, k2));

//            Assert.IsFalse(m.IsGreaterThanOrEqualTo(k1, k2));
//            Assert.IsFalse(m.IsGreaterThan(k1, k2));
//            Assert.IsFalse(m.CompareTo(k1, k2) > 0);

//            Assert.IsTrue(m.IsNotEqual(k1, k2));
//        }

//        public void ExpectGreaterThan(SortedTreeMethodsBase<KeyValue256> m, KeyValue256 k1, KeyValue256 k2)
//        {
//            Assert.IsFalse(m.IsLessThan(k1, k2));
//            Assert.IsFalse(m.CompareTo(k1, k2) < 0);
//            Assert.IsFalse(m.IsLessThanOrEqualTo(k1, k2));

//            Assert.IsFalse(m.CompareTo(k1, k2) == 0);
//            Assert.IsFalse(m.IsEqual(k1, k2));

//            Assert.IsTrue(m.IsGreaterThanOrEqualTo(k1, k2));
//            Assert.IsTrue(m.IsGreaterThan(k1, k2));
//            Assert.IsTrue(m.CompareTo(k1, k2) > 0);

//            Assert.IsTrue(m.IsNotEqual(k1, k2));
//        }

//        public void ExpectEqualTo(SortedTreeMethodsBase<KeyValue256> m, KeyValue256 k1, KeyValue256 k2)
//        {
//            Assert.IsFalse(m.IsLessThan(k1, k2));
//            Assert.IsFalse(m.CompareTo(k1, k2) < 0);

//            Assert.IsTrue(m.IsLessThanOrEqualTo(k1, k2));
//            Assert.IsTrue(m.CompareTo(k1, k2) == 0);
//            Assert.IsTrue(m.IsEqual(k1, k2));
//            Assert.IsTrue(m.IsGreaterThanOrEqualTo(k1, k2));

//            Assert.IsFalse(m.IsGreaterThan(k1, k2));
//            Assert.IsFalse(m.CompareTo(k1, k2) > 0);

//            Assert.IsFalse(m.IsNotEqual(k1, k2));
//        }


//    }
//}

