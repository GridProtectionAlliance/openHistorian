using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NPlot
{
    public static class StepTimer
    {
        private static readonly Dictionary<string, Stopwatch> AllStopwatches;

        static StepTimer()
        {
            AllStopwatches = new Dictionary<string, Stopwatch>();
        }

        public static Stopwatch Start(string name)
        {
            lock (AllStopwatches)
            {
                if (!AllStopwatches.ContainsKey(name))
                {
                    AllStopwatches.Add(name, new Stopwatch());
                }
                Stopwatch sw = AllStopwatches[name];
                sw.Start();
                return sw;
            }
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
            foreach (KeyValuePair<string, Stopwatch> kvp in AllStopwatches)
            {
                sb.AppendLine(kvp.Key + '\t' + kvp.Value.Elapsed.TotalMilliseconds.ToString());
            }
            return sb.ToString();
        }

        public static string GetResultsPercent()
        {
            double total = 0;
            foreach (KeyValuePair<string, Stopwatch> kvp in AllStopwatches)
            {
                total += kvp.Value.Elapsed.TotalMilliseconds;
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, Stopwatch> kvp in AllStopwatches)
            {
                sb.AppendLine(kvp.Key + '\t' + (kvp.Value.Elapsed.TotalMilliseconds / total).ToString("0.0%"));
            }
            return sb.ToString();
        }
    }
}