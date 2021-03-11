using NUnit.Framework;

namespace GSF.Diagnostics
{
    [TestFixture]
    public class LogTypeTest
    {
        //private readonly static LogType LogType = LogType.Create(typeof(LogTypeTest));

        [Test]
        public void Test()
        {
            new T1<int?,string>.T2<long?>();

            System.Console.WriteLine(0);
        }

        public class T1<T11,T12>
        {
            public class T2<T22>
            {
                public readonly LogPublisher LogType = Logger.CreatePublisher(typeof(T2<T22>),MessageClass.Component);
                
            }
        }

    }
}
