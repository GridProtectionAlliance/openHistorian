using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using GSF.Collections;
using NUnit.Framework;

namespace openHistorian.PerformanceTests
{
    [TestFixture]
    class ReadonlyGuidDictionary_test
    {
        [Test]
        public void DoesItWork()
        {
            Guid[] ids = new Guid[1000];
            Dictionary<Guid, string> source = new Dictionary<Guid, string>();
            for (int x = 0; x < ids.Length; x++)
            {
                Guid value = Guid.NewGuid();
                source.Add(value, x.ToString());
                ids[x] = value;
            }
            ReadonlyGuidDictionary<string> compare = new ReadonlyGuidDictionary<string>(source);


            for (int x = 0; x < ids.Length; x++)
            {
                if (source.ContainsKey(ids[x]) != compare.ContainsKey(ids[x]))
                    throw new Exception();
            }


            string value1, value2;
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

        [Test]
        public void CompareContainsKey()
        {
            Guid[] ids = new Guid[1000];
            Dictionary<Guid, string> source = new Dictionary<Guid, string>();
            for (int x = 0; x < ids.Length; x++)
            {
                Guid value = Guid.NewGuid();
                source.Add(value, x.ToString());
                ids[x] = value;
            }
            ReadonlyGuidDictionary<string> compare = new ReadonlyGuidDictionary<string>(source);


            Stopwatch sw = new Stopwatch();

            //-------------prime

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 10000; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    source.ContainsKey(ids[x]);
                }

            sw.Stop();


            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 10000; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare.ContainsKey(ids[x]);
                }

            sw.Stop();


            //----------run


            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 10000; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    source.ContainsKey(ids[x]);
                }

            sw.Stop();

            Console.WriteLine(10000 * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 10000; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare.ContainsKey(ids[x]);
                }

            sw.Stop();

            Console.WriteLine(10000 * ids.Length / sw.Elapsed.TotalSeconds / 1000000);



        }

        [Test]
        public void CompareTryGetKey()
        {
            string value;
            Guid[] ids = new Guid[1000];
            Dictionary<Guid, string> source = new Dictionary<Guid, string>();
            for (int x = 0; x < ids.Length; x++)
            {
                Guid id = Guid.NewGuid();
                source.Add(id, x.ToString());
                ids[x] = id;
            }
            ReadonlyGuidDictionary<string> compare = new ReadonlyGuidDictionary<string>(source);


            Stopwatch sw = new Stopwatch();

            //-------------prime

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 10000; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    source.TryGetValue(ids[x], out value);
                }

            sw.Stop();


            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 10000; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare.TryGetValue(ids[x], out value);
                }

            sw.Stop();


            //----------run


            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 10000; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    source.TryGetValue(ids[x], out value);
                }

            sw.Stop();

            Console.WriteLine(10000 * ids.Length / sw.Elapsed.TotalSeconds / 1000000);

            sw.Reset();
            sw.Start();

            for (int loop = 0; loop < 10000; loop++)
                for (int x = 0; x < ids.Length; x++)
                {
                    compare.TryGetValue(ids[x], out value);
                }

            sw.Stop();

            Console.WriteLine(10000 * ids.Length / sw.Elapsed.TotalSeconds / 1000000);



        }
    }
}
