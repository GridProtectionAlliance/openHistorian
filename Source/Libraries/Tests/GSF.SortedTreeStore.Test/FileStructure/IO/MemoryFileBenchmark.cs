using System;
using GSF;
using GSF.IO.Unmanaged;
using NUnit.Framework;

namespace GSF.IO.FileStructure.Media
{
    [TestFixture]
    internal class MemoryFileBenchmark
    {
        [Test]
        public void Test1()
        {
            MemoryPoolFile file = new MemoryPoolFile(Globals.MemoryPool);

            BinaryStreamIoSessionBase session = file.CreateIoSession();

            BlockArguments blockArguments = new BlockArguments();
            blockArguments.IsWriting = true;
            blockArguments.Position = 10000000;
            session.GetBlock(blockArguments);


            Console.WriteLine("Get Block\t" + StepTimer.Time(10, () =>
            {
                blockArguments.Position = 100000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 200000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 300000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 400000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 500000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 600000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 700000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 800000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 900000;
                session.GetBlock(blockArguments);
                blockArguments.Position = 1000000;
                session.GetBlock(blockArguments);
            }));
        }
    }
}