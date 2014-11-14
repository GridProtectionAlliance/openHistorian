using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GSF
{
    [TestFixture]
    public class DelegateTest
    {
        public event Action MyEvent;

        [Test]
        public void Test()
        {
            if (MyEvent == null)
                System.Console.WriteLine("1: Null");
            else
                System.Console.WriteLine("1: " + MyEvent.GetType().ToString());

            MyEvent += Test;

            if (MyEvent == null)
                System.Console.WriteLine("2: Null");
            else
                System.Console.WriteLine("2: " + MyEvent.GetType().ToString());

            MyEvent -= Test;

            if (MyEvent == null)
                System.Console.WriteLine("3: Null");
            else
                System.Console.WriteLine("3: " + MyEvent.GetType().ToString());

        }


    }
}
