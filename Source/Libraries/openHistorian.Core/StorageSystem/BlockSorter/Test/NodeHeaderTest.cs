using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.StorageSystem.BlockSorter
{
    class NodeHeaderTest
    {
        public static void Test()
        {
            BinaryStream stream = new BinaryStream(new PooledMemoryStream());

            NodeHeader header = default(NodeHeader);
            if (header.Level != 0) throw new Exception();
            if (header.ChildCount != 0) throw new Exception();
            if (header.PreviousNode != 0) throw new Exception();
            if (header.NextNode != 0) throw new Exception();
            header.Level = 2;
            header.ChildCount = 3;
            header.PreviousNode = 45;
            header.NextNode = 242;
            header.Save(stream);
            if (header.Level != 2) throw new Exception();
            if (header.ChildCount != 3) throw new Exception();
            if (header.PreviousNode != 45) throw new Exception();
            if (header.NextNode != 242) throw new Exception();
            header.Load(stream);
            if (header.Level != 0) throw new Exception();
            if (header.ChildCount != 0) throw new Exception();
            if (header.PreviousNode != 0) throw new Exception();
            if (header.NextNode != 0) throw new Exception();
            stream.Position = 0;
            header.Load(stream);
            if (header.Level != 2) throw new Exception();
            if (header.ChildCount != 3) throw new Exception();
            if (header.PreviousNode != 45) throw new Exception();
            if (header.NextNode != 242) throw new Exception();

            TreeHeader tree = new TreeHeader(stream,4096);
            header.Save(tree,1);
            if (stream.Position != 4096 + NodeHeader.Size) throw new Exception();
            header.Load(tree,2);
            if (header.Level != 0) throw new Exception();
            if (header.ChildCount != 0) throw new Exception();
            if (header.PreviousNode != 0) throw new Exception();
            if (header.NextNode != 0) throw new Exception();
            header.Load(tree,1);
            if (header.Level != 2) throw new Exception();
            if (header.ChildCount != 3) throw new Exception();
            if (header.PreviousNode != 45) throw new Exception();
            if (header.NextNode != 242) throw new Exception();

        }
    }
}
