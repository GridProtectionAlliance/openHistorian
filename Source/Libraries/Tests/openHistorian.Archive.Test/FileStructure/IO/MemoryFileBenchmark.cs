using System;
using GSF;
using GSF.IO.Unmanaged;
using NUnit.Framework;

namespace openHistorian.FileStructure.IO
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
            blockArguments.isWriting = true;
            blockArguments.position = 10000000;
            session.GetBlock(blockArguments);


            Console.WriteLine("Get Block\t" + StepTimer.Time(10, () =>
            {
                blockArguments.position = 100000;
                session.GetBlock(blockArguments);
                blockArguments.position = 200000;
                session.GetBlock(blockArguments);
                blockArguments.position = 300000;
                session.GetBlock(blockArguments);
                blockArguments.position = 400000;
                session.GetBlock(blockArguments);
                blockArguments.position = 500000;
                session.GetBlock(blockArguments);
                blockArguments.position = 600000;
                session.GetBlock(blockArguments);
                blockArguments.position = 700000;
                session.GetBlock(blockArguments);
                blockArguments.position = 800000;
                session.GetBlock(blockArguments);
                blockArguments.position = 900000;
                session.GetBlock(blockArguments);
                blockArguments.position = 1000000;
                session.GetBlock(blockArguments);
            }));
        }
    }
}