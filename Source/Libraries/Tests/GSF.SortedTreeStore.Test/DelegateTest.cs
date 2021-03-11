using System;
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
            if (MyEvent is null)
                System.Console.WriteLine("1: Null");
            else
                System.Console.WriteLine("1: " + MyEvent.GetType().ToString());

            MyEvent += Test;

            if (MyEvent is null)
                System.Console.WriteLine("2: Null");
            else
                System.Console.WriteLine("2: " + MyEvent.GetType().ToString());

            MyEvent -= Test;

            if (MyEvent is null)
                System.Console.WriteLine("3: Null");
            else
                System.Console.WriteLine("3: " + MyEvent.GetType().ToString());

        }


    }
}
