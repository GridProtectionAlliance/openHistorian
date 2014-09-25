using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian.Collections;

namespace openHistorian.PerformanceTests
{
    [TestFixture]
    public class GCTime
    {

        private List<HistorianKey[]> m_objects = new List<HistorianKey[]>();

        [Test]
        public void Test()
        {
            for (int x = 0; x < 100; x++)
                AddItemsAndTime();
        }

        void AddItemsAndTime()
        {
            var array = new HistorianKey[10000];
            for (int x = 0; x < array.Length; x++)
                array[x] = new HistorianKey();

            m_objects.Add(array);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Stopwatch sw = new Stopwatch();
            var swap = m_objects[0][0];
            m_objects[0][0] = m_objects[0][1];
            m_objects[0][1] = swap;
            m_objects[0][m_objects.Count] = null;

            sw.Start();
            GC.Collect();
            sw.Stop();
            Console.WriteLine("{0}0k items: {1}ms", m_objects.Count.ToString(), sw.Elapsed.TotalMilliseconds.ToString("0.00"));
        }

    }
}
