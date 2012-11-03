using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;

namespace openHistorian.V2
{
    public class DebugStopwatch
    {
        Stopwatch sw;
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

    }
}
