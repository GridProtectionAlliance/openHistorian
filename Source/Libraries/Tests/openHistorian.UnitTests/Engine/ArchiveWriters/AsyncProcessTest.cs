using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace openHistorian.Engine.ArchiveWriters
{
    [TestFixture]
    public class AsyncProcessTest
    {

        class IntBox
        {
            public int Value;
            public IntBox(int value)
            {
                Value = value;
            }
        }

        [Test]
        public void TestConstructor()
        {
            var state = new IntBox(2);
            var async = new AsyncProcess<IntBox>(Process, state);

            Assert.AreEqual(2, state.Value);
            async.Run();
            Thread.Sleep(5);
            Assert.AreEqual(2, state.Value);
            Thread.Sleep(20);
            Assert.AreEqual(3, state.Value);

            async.RunAfterDelay(new TimeSpan(TimeSpan.TicksPerMillisecond * 5));
            Thread.Sleep(12);
            Assert.AreEqual(3, state.Value);
            Thread.Sleep(5);
            Assert.AreEqual(4, state.Value);

            async.RunAfterDelay(new TimeSpan(TimeSpan.TicksPerMillisecond * 5));
            async.Run();
            Thread.Sleep(1);
            async.Run();
            Assert.AreEqual(4, state.Value);
            Thread.Sleep(10);
            Assert.AreEqual(5, state.Value);
            Thread.Sleep(25);
            Assert.AreEqual(6, state.Value);
            Thread.Sleep(30);
            Assert.AreEqual(6, state.Value);

        }

        void Process(IntBox state)
        {
            Thread.Sleep(10);
            state.Value++;
            Thread.Sleep(10);
        }
    }
}
