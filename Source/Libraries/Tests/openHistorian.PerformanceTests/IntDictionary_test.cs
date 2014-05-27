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
//    class IntDictionary_test
//    {
//        [Test]
//        public void DoesItWorkLookup()
//        {
//            Random r = new Random();
//            int[] ids = new int[1000];
//            Dictionary<int, int> source = new Dictionary<int, int>();
//            for (int x = 0; x < ids.Length; x++)
//            {
//                int value = r.Next();
//                source.Add(value, x);
//                ids[x] = value;
//            }

//            IntIndex compare = new IntIndex(source.Keys);

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
//            Random r = new Random();
//            int[] ids = new int[1000];
//            Dictionary<int, int> source = new Dictionary<int, int>();
//            for (int x = 0; x < ids.Length; x++)
//            {
//                int value = r.Next();
//                source.Add(value, x);
//                ids[x] = value;
//            }

//            IntDictionary<int> compare = new IntDictionary<int>(source);

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
//            int scale = 100000;

//            Random r = new Random(0);
//            int[] ids = new int[5000000/scale];
//            Dictionary<int, int> source = new Dictionary<int, int>();
//            for (int x = 0; x < ids.Length; x++)
//            {
//                int value = r.Next();
//                if (source.ContainsKey(value))
//                {
//                    x--;
//                }
//                else
//                {
//                    source.Add(value, x);
//                    ids[x] = value;
//                }
//            }

//            IntIndex compare = new IntIndex(source.Keys);
//            IntDictionary<int> compare2 = new IntDictionary<int>(source);

//            Stopwatch sw = new Stopwatch();

//            //-------------prime

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 1*scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    source.ContainsKey(ids[x]);
//                }

//            sw.Stop();


//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 1 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare.ContainsKey(ids[x]);
//                }

//            sw.Stop();

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 1 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare2.ContainsKey(ids[x]);
//                }

//            sw.Stop();

//            //----------run


//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 2 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    source.ContainsKey(ids[x]);
//                }

//            sw.Stop();

//            Console.WriteLine(2 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 5 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare.ContainsKey(ids[x]);
//                }

//            sw.Stop();

//            Console.WriteLine(5 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

//            sw.Reset();
//            sw.Start();

//            for (int loop = 0; loop < 5 * scale; loop++)
//                for (int x = 0; x < ids.Length; x++)
//                {
//                    compare2.ContainsKey(ids[x]);
//                }

//            sw.Stop();

//            Console.WriteLine(5 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

//        }

//        [Test]
//        public void CompareTryGetKey()
//        {
//            int scale = 100000;

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
