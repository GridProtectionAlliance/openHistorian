using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.Diagnostics;
using NUnit.Framework;

namespace openHistorian.PerformanceTests.Diagnostics
{
    [TestFixture]
    public class StackTrace_Test
    {
        [Test]
        public void Test()
        {
            Console.WriteLine(Environment.StackTrace);

            RunMethod();
            DebugStopwatch sw = new DebugStopwatch();
            var time = sw.TimeEvent(() =>
            {
                for (int x = 0; x < 1000; x++)
                {
                    RunMethod3();
                }
            });
            Console.WriteLine(1000/time);
        }

        void RunMethod()
        {
            var str = Environment.StackTrace;
            if (str == null)
                throw new Exception();
        }
        void RunMethod2()
        {
            var st = new StackTrace(true);
            var frames = st.GetFrames();

            var sb = new StringBuilder();
            foreach (var frame in frames)
            {
                sb.AppendLine(frame.GetMethod().Name);
                sb.AppendLine(frame.GetMethod().Module.Assembly.FullName);
                sb.AppendLine(frame.GetFileName());
                sb.AppendLine(frame.GetFileLineNumber().ToString());
            }

            if (frames.Length==0)
                throw new Exception();
        }
        void RunMethod3()
        {
            var st = new LogStackTrace();
        }


    }
}
