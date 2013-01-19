using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace GSF.Collections
{
    [TestFixture]
    class IsolatedQueueFileBacked
    {

        unsafe struct TestType : ILoadable
        {
            public byte Item1;
            public short Item2;
            public int Item3;
            public long Item4;

            public TestType(byte value)
            {
                Item1 = value;
                Item2 = (short)(value * 2);
                Item3 = (Item2 * 2);
                Item4 = (Item3 * 2L);
            }

            public int InMemorySize
            {
                get
                {
                    return sizeof(TestType);

                }
            }

            public int OnDiskSize
            {
                get
                {
                    return 16;
                }
            }

            public void Save(System.IO.BinaryWriter writer)
            {
                writer.Write((byte)1);
                writer.Write(Item1);
                writer.Write(Item2);
                writer.Write(Item3);
                writer.Write(Item4);
            }

            public void Load(System.IO.BinaryReader reader)
            {
                byte version = reader.ReadByte();
                if (version != 1)
                    throw new VersionNotFoundException("unknown");
                Item1 = reader.ReadByte();
                Item2 = reader.ReadInt16();
                Item3 = reader.ReadInt32();
                Item4 = reader.ReadInt64();

                if (Item1 * 2 != Item2)
                    throw new Exception("Corrupt");
                if (Item2 * 2 != Item3)
                    throw new Exception("Corrupt");
                if (Item3 * 2 != Item4)
                    throw new Exception("Corrupt");
            }
        }

        [Test]
        public void Test()
        {
            const string Path = @"C:\Temp\Buffer\";
            const string Prefix = @"Data";
            const int cnt = 1000000;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var collection = new IsolatedQueueFileBacked<TestType>(Path, Prefix, 3 * 1024 * 1024, 1024 * 1024))
            {
                for (int x = 0; x < cnt; x++)
                {
                    collection.Enqueue(new TestType((byte)(x % 251)));
                }
                TestType T;

                collection.TryDequeue(out T);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds);

        }
        [Test]
        public void TestDequeue()
        {
            const string Path = @"C:\Temp\Buffer\";
            const string Prefix = @"Data";
            int cnt = 0;

            var collection = new IsolatedQueueFileBacked<TestType>(Path, Prefix, 1 * 1024 * 1024, 1024 * 1024);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            TestType T;

            while (collection.TryDequeue(out T))
                cnt++;
            sw.Stop();
            collection.Dispose();
            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine(cnt);

        }


    }
}
