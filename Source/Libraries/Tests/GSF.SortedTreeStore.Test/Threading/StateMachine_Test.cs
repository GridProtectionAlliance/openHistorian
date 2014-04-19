using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GSF.Threading
{
    [TestFixture]
    public class StateMachine_Test
    {
        [Test]
        public void Test()
        {
            var sm = new StateMachine(1);
            using (var s = sm.Lock())
            {
                Assert.AreEqual(s.State, 1);
                try
                {
                    s.Reacquire();
                    Assert.Fail("Should not have gotten here.");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex);
                }
            }
        }

        [Test]
        public void Test2()
        {
            var sm = new StateMachine(1);
            using (var s = sm.Lock())
            {
                Assert.AreEqual(s.State, 1);
                s.Release();
                Assert.AreNotEqual(s.State, 1);
                s.Reacquire();
                Assert.AreEqual(s.State, 1);
                s.Release(2);
                Assert.AreNotEqual(s.State, 2);
                s.Reacquire();
                Assert.AreEqual(s.State, 2);
                s.Release();
                try
                {
                    s.Release();
                    Assert.Fail("Should not have gotten here.");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex);
                }
            }
        }
    }
}
