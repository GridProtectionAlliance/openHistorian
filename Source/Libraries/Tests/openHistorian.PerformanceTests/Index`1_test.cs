using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using GSF.Collections;
using GSF.IO.FileStructure;
using NUnit.Framework;

namespace openHistorian.PerformanceTests
{
    [TestFixture]
    class Index_test
    {
        [Test]
        public void DoesItWorkLookup()
        {
            Random r = new Random();
            DoesItWorkLookup<int>(r.Next);
            DoesItWorkDictionary<int>(r.Next);
            Console.WriteLine("Contains Int 10");
            CompareContainsKey<int>(r.Next, 10);
            Console.WriteLine("Contains Guid 10");
            CompareContainsKey<Guid>(Guid.NewGuid, 10);

            Console.WriteLine("TryGet Int 10");
            CompareTryGetKey<int>(r.Next, 10);
            Console.WriteLine("TryGet Guid 10");
            CompareTryGetKey<Guid>(Guid.NewGuid, 10);
        }

        [Test]
        public void DoesItWorkLookup2()
        {
            Random r = new Random();
            DoesItWorkLookup<int>(r.Next);
            DoesItWorkDictionary<int>(r.Next);
        }

        [Test]
        public void CompareTryGetGuid()
        {
            Random r = new Random(0);
            CompareTryGetKey<int>(r.Next, 10000);
            r = new Random(0);
            CompareContainsKey<int>(r.Next, 10000);
        }

        public void DoesItWorkLookup<T>(Func<T> nextRandom)
            where T : IEquatable<T>
        {
            T[] ids = new T[1000];
            Dictionary<T, int> source = new Dictionary<T, int>();
            for (int x = 0; x < ids.Length; x++)
            {
                T value = nextRandom();
                if (source.ContainsKey(value))
                {
                    x--;
                }
                else
                {
                    source.Add(value, x);
                    ids[x] = value;
                }
            }

            Index<T> compare = new Index<T>(source.Keys);

            for (int x = 0; x < ids.Length; x++)
            {
                if (source.ContainsKey(ids[x]) != compare.ContainsKey(ids[x]))
                    throw new Exception();
            }

            int value1, value2;
            for (int x = 0; x < ids.Length; x++)
            {
                if (!source.TryGetValue(ids[x], out value1))
                    throw new Exception();
                if (!compare.TryGetValue(ids[x], out value2))
                    throw new Exception();

                if (value1 != value2)
                    throw new Exception();
            }

            for (int x = 0; x < ids.Length; x++)
            {
                if (source[ids[x]] != compare[ids[x]])
                    throw new Exception();
            }

        }

        public void DoesItWorkDictionary<T>(Func<T> nextRandom)
            where T : IEquatable<T>
        {
            T[] ids = new T[1000];
            Dictionary<T, int> source = new Dictionary<T, int>();
            for (int x = 0; x < ids.Length; x++)
            {
                T value = nextRandom();
                if (source.ContainsKey(value))
                {
                    x--;
                }
                else
                {
                    source.Add(value, x);
                    ids[x] = value;
                }
            }

            FastDictionary<T, int> compare = new FastDictionary<T, int>(source);

            for (int x = 0; x < ids.Length; x++)
            {
                if (source.ContainsKey(ids[x]) != compare.ContainsKey(ids[x]))
                    throw new Exception();
            }

            int value1, value2;
            for (int x = 0; x < ids.Length; x++)
            {
                if (!source.TryGetValue(ids[x], out value1))
                    throw new Exception();
                if (!compare.TryGetValue(ids[x], out value2))
                    throw new Exception();

                if (value1 != value2)
                    throw new Exception();
            }

            for (int x = 0; x < ids.Length; x++)
            {
                if (source[ids[x]] != compare[ids[x]])
                    throw new Exception();
            }

        }

        public void CompareContainsKey<T>(Func<T> nextRandom, int scale = 10)
            where T : IEquatable<T>
        {
            T[] ids = new T[5000000 / scale];
            Dictionary<T, int> source = new Dictionary<T, int>();
            for (int x = 0; x < ids.Length; x++)
            {
                T value = nextRandom();
                if (source.ContainsKey(value))
                {
                    x--;
                }
                else
                {
                    source.Add(value, x);
                    ids[x] = value;
                }
            }

            Index<T> compare = new Index<T>(source.Keys);
            FastDictionary<T, int> compare2 = new FastDictionary<T, int>(source);

            Stopwatch sw = new Stopwatch();

            //-------------prime

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 1 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    source.ContainsKey(ids[x]);
                }

            sw.Stop();


            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 1 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare.ContainsKey(ids[x]);
                }

            sw.Stop();

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 1 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare2.ContainsKey(ids[x]);
                }

            sw.Stop();

            //----------run


            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 2 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    source.ContainsKey(ids[x]);
                }

            sw.Stop();

            Console.WriteLine(2 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 5 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare.ContainsKey(ids[x]);
                }

            sw.Stop();

            Console.WriteLine(5 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 5 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare2.ContainsKey(ids[x]);
                }

            sw.Stop();

            Console.WriteLine(5 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);
        }

        public void CompareTryGetKey<T>(Func<T> nextRandom, int scale = 10)
            where T : IEquatable<T>
        {
            T[] ids = new T[5000000 / scale];
            Console.WriteLine("Count: " + ids.Length.ToString());
            Dictionary<T, int> source = new Dictionary<T, int>();
            for (int x = 0; x < ids.Length; x++)
            {
                T value = nextRandom();
                if (source.ContainsKey(value))
                {
                    x--;
                }
                else
                {
                    source.Add(value, x);
                    ids[x] = value;
                }
            }

            Index<T> compare = new Index<T>(source.Keys);
            FastDictionary<T, int> compare2 = new FastDictionary<T, int>(source);

            Stopwatch sw = new Stopwatch();

            int value2;

            //-------------prime

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 1 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    source.TryGetValue(ids[x], out value2);
                }

            sw.Stop();


            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 1 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare.TryGetValue(ids[x], out value2);
                }

            sw.Stop();

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 1 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare2.TryGetValue(ids[x], out value2);
                }

            sw.Stop();

            //----------run


            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 2 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    source.TryGetValue(ids[x], out value2);
                }

            sw.Stop();

            Console.WriteLine(2 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 5 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare.TryGetValue(ids[x], out value2);
                }

            sw.Stop();

            Console.WriteLine(5 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 5 * scale; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare2.TryGetValue(ids[x], out value2);
                }

            sw.Stop();

            Console.WriteLine(5 * scale * ids.Length / sw.Elapsed.TotalSeconds / 1000000);



        }
    }
}
