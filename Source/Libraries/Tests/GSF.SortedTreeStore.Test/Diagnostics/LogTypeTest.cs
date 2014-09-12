using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian.Collections;

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
                public readonly LogType LogType = Logger.LookupType(typeof(T2<T22>));
                
            }
        }

    }
}
