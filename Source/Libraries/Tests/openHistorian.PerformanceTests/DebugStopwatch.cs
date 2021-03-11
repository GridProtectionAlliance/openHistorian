using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using NUnit.Framework;

namespace openHistorian
{
    public class DebugStopwatch
    {
        private readonly Stopwatch sw;

        public DebugStopwatch()
        {
            GCSettings.LatencyMode = GCLatencyMode.Batch;
            sw = new Stopwatch();
        }

        public void DoGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void Start(bool skipCollection = false)
        {
            if (skipCollection)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            sw.Restart();
        }

        public void Stop(double maximumTime)
        {
            sw.Stop();
            Assert.IsTrue(sw.Elapsed.TotalMilliseconds <= maximumTime);
        }

        public void Stop(double minimumTime, double maximumTime)
        {
            sw.Stop();
            Assert.IsTrue(sw.Elapsed.TotalMilliseconds >= minimumTime);
            Assert.IsTrue(sw.Elapsed.TotalMilliseconds <= maximumTime);
        }

        public double TimeEvent(Action function)
        {
            GC.Collect();
            function();
            int count = 0;
            sw.Reset();
            while (sw.Elapsed.TotalSeconds < .25)
            {
                sw.Start();
                function();
                sw.Stop();
                count++;
            }
            return sw.Elapsed.TotalSeconds / count;
        }

        public double TimeEventMedian(Action function)
        {
            List<double> values = new List<double>();
            GC.Collect();
            function();
            int count = 0;
            Stopwatch swTotal = new Stopwatch();
            swTotal.Start();
            while (swTotal.Elapsed.TotalSeconds < 1 && values.Count < 100)
            {
                sw.Restart();
                function();
                sw.Stop();
                values.Add(sw.Elapsed.TotalSeconds);
            }

            return values[(values.Count - 1) >> 1];
        }
    }
}