using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.Diagnostics;
using NUnit.Framework;

namespace SampleCode.GSF.Diagnostics
{
    [TestFixture]
    public class Logging_Examples
    {
        public class RootClass : ILogSourceDetails
        {
            private LogSource m_source;
            public ChildClass A;
            public ChildClass B;
            public ChildClass C;

            public RootClass()
            {
                m_source = Logger.CreateSource(this);
                A = new ChildClass(m_source, "A");
                B = new ChildClass(m_source, "B");
                C = new ChildClass(m_source, "C");
            }
            public string GetSourceDetails()
            {
                return "This is the root. Random Guid: " + Guid.NewGuid();
            }

            public void WriteMessage()
            {
                m_source.Publish(VerboseLevel.Information, "Root Function Called");
            }
        }

        public class ChildClass : ILogSourceDetails
        {
            private LogSource m_source;
            private string m_value;

            public ChildClass(LogSource parent, string value)
            {
                m_source = Logger.CreateSource(this, parent);
                m_value = value;

            }

            public string GetSourceDetails()
            {
                return "Child " + m_value;
            }

            public void WriteMessage()
            {
                m_source.Publish(VerboseLevel.Information, "Child Function Called");
            }
        }

        [Test]
        public void Example1()
        {
            var subscriber = Logger.CreateSubscriber();
            subscriber.Verbose = VerboseLevel.All;
            subscriber.Subscribe(Logger.LookupType("SampleCode"));
            subscriber.Log += subscriber_Log;
            var c = new RootClass();
            c.A.WriteMessage();
            c.B.WriteMessage();
            c.C.WriteMessage();
            c.WriteMessage();
        }

        void subscriber_Log(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.GetMessage(true));
        }






    }
}
