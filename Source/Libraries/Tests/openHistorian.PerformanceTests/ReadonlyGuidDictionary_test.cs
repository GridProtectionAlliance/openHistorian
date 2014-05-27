//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Management.Instrumentation;
//using System.Text;
//using System.Threading.Tasks;
//using GSF.Collections;
//using NUnit.Framework;

//namespace openHistorian.PerformanceTests
//{
//    [TestFixture]
//    class ReadonlyGuidDictionary_test
//    {
//        [Test]
//        public void DoesItWorkLookup()
//        {
//            Guid[] ids = new Guid[1000];
//            Dictionary<Guid, int> source = new Dictionary<Guid, int>();
//            for (int x = 0; x < ids.Length; x++)
//            {
//                Guid value = Guid.NewGuid();
//                source.Add(value, x);
//                ids[x] = value;
//            }
//            GuidIndex compare = new GuidIndex(source.Keys);

//            for (int x = 0; x < ids.Length; x++)
//            {
//                if (source.ContainsKey(ids[x]) != compare.ContainsKey(ids[x]))
//                    throw new Exception();
//            }


//            int value1, value2;
//            for (int x = 0; x < ids.Length; x++)
//            {
//                if (!source.TryGetValue(ids[x], out value1))
//                    throw new Exception();
//                if (!compare.TryGetValue(ids[x], out value2))
//                    throw new Exception();

//                if (value1 != value2)
//                    throw new Exception();
//            }

//            for (int x = 0; x < ids.Length; x++)
//            {
//                if (source[ids[x]] != compare[ids[x]])
//                    throw new Exception();
//            }

//        }

//        [Test]
//        public void DoesItWorkDictionary()
//        {
//            Guid[] ids = new Guid[1000];
//            Dictionary<Guid, int> source = new Dictionary<Guid, int>();
//            for (int x = 0; x < ids.Length; x++)
//            {
//                Guid value = Guid.NewGuid();
//                source.Add(value, x);
//                ids[x] = value;
//            }
//            GuidDictionary<int> compare = new GuidDictionary<int>(source);

//            for (int x = 0; x < ids.Length; x++)
//            {
//                if (source.ContainsKey(ids[x]) != compare.ContainsKey(ids[x]))
//                    throw new Exception();
//            }


//            int value1, value2;
//            for (int x = 0; x < ids.Length; x++)
//            {
//                if (!source.TryGetValue(ids[x], out value1))
//                    throw new Exception();
//                if (!compare.TryGetValue(ids[x], out value2))
//                    throw new Exception();

//                if (value1 != value2)
//                    throw new Exception();
//            }

//            for (int x = 0; x < ids.Length; x++)
//            {
//                if (source[ids[x]] != compare[ids[x]])
//                    throw new Exception();
//            }

//        }

//        [Test]
//        public void CompareContainsKey()
//        {
//            Guid[] ids = new Guid[5000000];
//            Dictionary<Guid, int> source = new Dictionary<Guid, int>(5000000);
//            for (int x = 0; x < ids.Length; x++)
//            {
//                Guid value = Guid.NewGuid();
//                source.Add(value, x);
//                ids[x] = value;
//            }
//            GuidIndex compare = new GuidIndex(source.Keys);
//            GuidDictionary<int> compare2 = new GuidDictionary<int>(source);


//            Stopwatch sw = new Stopwatch();

//            //-------------prime

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 1; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    source.ContainsKey(ids[x]);
//                }

//            sw.Stop();


//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 1; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare.ContainsKey(ids[x]);
//                }

//            sw.Stop();

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 1; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare2.ContainsKey(ids[x]);
//                }

//            sw.Stop();


//            //----------run


//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 2; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    source.ContainsKey(ids[x]);
//                }

//            sw.Stop();

//            Console.WriteLine(2 * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 5; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare.ContainsKey(ids[x]);
//                }

//            sw.Stop();

//            Console.WriteLine(5 * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 5; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare2.ContainsKey(ids[x]);
//                }

//            sw.Stop();

//            Console.WriteLine(5 * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

//        }

//        [Test]
//        public void CompareTryGetKey()
//        {
//            int scale = 100;

//            int value2;
//            Guid[] ids = new Guid[5000000 / scale];
//            Dictionary<Guid, int> source = new Dictionary<Guid, int>();
//            for (int x = 0; x < ids.Length; x++)
//            {
//                Guid value = Guid.NewGuid();
//                source.Add(value, x);
//                ids[x] = value;
//            }
//            GuidIndex compare = new GuidIndex(source.Keys);
//            GuidDictionary<int> compare2 = new GuidDictionary<int>(source);

//            Stopwatch sw = new Stopwatch();

//            //-------------prime

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 1 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    source.TryGetValue(ids[x], out value2);
//                }

//            sw.Stop();


//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 1 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare.TryGetValue(ids[x], out value2);
//                }

//            sw.Stop();

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 1 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare2.TryGetValue(ids[x], out value2);
//                }

//            sw.Stop();

//            //----------run


//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 2 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    source.TryGetValue(ids[x], out value2);
//                }

//            sw.Stop();

//            Console.WriteLine(2 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 5 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare.TryGetValue(ids[x], out value2);
//                }

//            sw.Stop();

//            Console.WriteLine(5 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 5 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare2.TryGetValue(ids[x], out value2);
//                }

//            sw.Stop();

//            Console.WriteLine(5 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);



//        }
//    }
//}
