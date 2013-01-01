using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openHistorian.PerformanceTests
{
    public static class StepTimer
    {
        static Dictionary<string, Stopwatch> AllStopwatches;

        static StepTimer()
        {
            AllStopwatches=new Dictionary<string, Stopwatch>();
        }

        public static Stopwatch Start(string name)
        {
            if (!AllStopwatches.ContainsKey(name))
            {
                AllStopwatches.Add(name,new Stopwatch());
            }
            var sw = AllStopwatches[name];
            sw.Start();
            return sw;
        }

        public static void Stop(Stopwatch sw)
        {
            sw.Stop();
        }

        public static void Reset()
        {
            AllStopwatches.Clear();
        }

        public static string GetResults()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in AllStopwatches)
            {
                sb.Append(kvp.Key + '\t' + kvp.Value.Elapsed.TotalMilliseconds.ToString());
            }
            return sb.ToString();
        }
    }
}
