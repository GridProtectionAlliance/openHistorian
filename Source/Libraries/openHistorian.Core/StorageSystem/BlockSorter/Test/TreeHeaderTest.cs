using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.StorageSystem.BlockSorter
{
    class TreeHeaderTest : TreeHeader
    {
        public TreeHeaderTest(BinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
        }
        public TreeHeaderTest(BinaryStream stream)
            : base(stream)
        {
        }

        public static void Test()
        {
            BinaryStream stream = new BinaryStream(new PooledMemoryStream());
            TreeHeaderTest header = new TreeHeaderTest(stream, 4096);

            if (header.Stream != stream) throw new Exception();
            if (header.BlockSize != 4096) throw new Exception();
            if (header.RootIndexAddress != 1) throw new Exception();
            if (header.RootIndexLevel != 0) throw new Exception();
            if (header.MaximumLeafNodeChildren != (4096 - NodeHeader.Size) / LeafNode.LeafStructureSize) throw new Exception();
            if (header.MaximumInternalNodeChildren != (4096 - NodeHeader.Size - sizeof(uint)) / InternalNode.InternalStructureSize) throw new Exception();
            if (header.NextUnallocatedByte != 2 * 4096) throw new Exception();

            if (header.AllocateSpace(20) != 2 * 4096) throw new Exception();
            if (header.NextUnallocatedByte != 2 * 4096 + 20) throw new Exception();
            if (header.AllocateSpace(30) != 2 * 4096 + 20) throw new Exception();
            if (header.NextUnallocatedByte != 2 * 4096 + 50) throw new Exception();
            if (header.AllocateNewNode() != 3) throw new Exception();
            if (header.NextUnallocatedByte != 4 * 4096) throw new Exception();
            if (header.AllocateSpace(30) != 4 * 4096) throw new Exception();
            if (header.NextUnallocatedByte != 4 * 4096+30) throw new Exception();

            header.BlockSize = 1244;
            header.RootIndexAddress = 3;
            header.RootIndexLevel = 4;
            header.MaximumLeafNodeChildren = 225; //These values will not save
            header.MaximumInternalNodeChildren = 235; //These values will not save
            header.NextUnallocatedByte = 12000;

            header.Save(stream);

            header = new TreeHeaderTest(stream);
            if (header.Stream != stream) throw new Exception();
            if (header.BlockSize != 1244) throw new Exception();
            if (header.RootIndexAddress != 3) throw new Exception();
            if (header.RootIndexLevel != 4) throw new Exception();
            if (header.MaximumLeafNodeChildren != (1244 - NodeHeader.Size) / LeafNode.LeafStructureSize) throw new Exception();
            if (header.MaximumInternalNodeChildren != (1244 - NodeHeader.Size - sizeof(uint)) / InternalNode.InternalStructureSize) throw new Exception();
            if (header.NextUnallocatedByte != 12000) throw new Exception();

            header.NavigateToNode(3);
            if (header.Stream.Position != 3 * 1244) throw new Exception();


        }
    }
}
