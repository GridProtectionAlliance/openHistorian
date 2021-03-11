using System;
using GSF.Diagnostics;
using NUnit.Framework;

namespace SampleCode.GSF.Diagnostics
{
    [TestFixture]
    public class Logging_Examples
    {
        public class RootClass : DisposableLoggingClassBase
        {
            public ChildClass A;
            public ChildClass B;
            public ChildClass C;

            public RootClass()
                : base(MessageClass.Application)
            {
                Log.InitialStackMessages.Union("Class Date", DateTime.Now.ToLongDateString());

                using (Logger.AppendStackMessages(LogStackMessages.Empty.Union("Class Date", DateTime.Now.ToLongDateString())))
                {
                    A = new ChildClass("A");
                    B = new ChildClass("B");
                    C = new ChildClass("C");
                }
            }
            public string GetSourceDetails()
            {
                return "This is the root. Random Guid: " + Guid.NewGuid();
            }

            public void WriteMessage()
            {
                Log.Publish(MessageLevel.Info, "Root Function Called");
            }
        }

        public class ChildClass : DisposableLoggingClassBase
        {
            private readonly string m_value;

            public ChildClass(string value)
                : base(MessageClass.Application)
            {
                m_value = value;
            }

            public string GetSourceDetails()
            {
                return "Child " + m_value;
            }

            public void WriteMessage()
            {
                Log.Publish(MessageLevel.Info, "Child Function Called");
            }
        }

        [Test]
        public void Example1()
        {
            LogSubscriber subscriber = Logger.CreateSubscriber(VerboseLevel.All);
            subscriber.NewLogMessage += subscriber_Log;
            RootClass c = new RootClass();
            c.A.WriteMessage();
            c.B.WriteMessage();
            c.C.WriteMessage();
            c.WriteMessage();
        }

        void subscriber_Log(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.GetMessage());
        }






    }
}
