using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GSF.Collections
{
    [TestFixture]
    class WeakListTest
    {
        [Test]
        public void Test()
        {
            var rand = new Random(3);

            List<string> list1 = new List<string>();
            WeakList<string> list2 = new WeakList<string>();

            for (int x = 0; x < 1000; x++)
            {
                var str = x.ToString();
                list1.Add(str);
                list2.Add(str);

                if (!list1.SequenceEqual(list2))
                    throw new Exception("Lists are not the same.");
            }

            for (int x = 1000; x < 2000; x++)
            {
                var str = x.ToString();
                var removeItem = list1[rand.Next(list1.Count)];
                list1.Remove(removeItem);
                list2.Remove(removeItem);

                if (!list1.SequenceEqual(list2))
                    throw new Exception("Lists are not the same.");

                list1.Add(str);
                list2.Add(str);

                if (!list1.SequenceEqual(list2))
                    throw new Exception("Lists are not the same.");
            }

            for (int x = 0; x < 100; x++)
            {
                list1.RemoveAt(rand.Next(list1.Count));
                GC.Collect();

                if (!list1.SequenceEqual(list2))
                    throw new Exception("Lists are not the same.");
            }



        }


        

    }
}
