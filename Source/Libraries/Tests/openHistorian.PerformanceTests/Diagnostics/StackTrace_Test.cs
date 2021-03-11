using System;
using System.Diagnostics;
using System.Text;
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
            double time = sw.TimeEvent(() =>
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
            string str = Environment.StackTrace;
            if (str is null)
                throw new Exception();
        }
        void RunMethod2()
        {
            StackTrace st = new StackTrace(true);
            StackFrame[] frames = st.GetFrames();

            StringBuilder sb = new StringBuilder();
            foreach (StackFrame frame in frames)
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
            _ = new LogStackTrace();
        }


    }
}
