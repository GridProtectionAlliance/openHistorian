using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.StorageSystem.BlockSorter
{
    class InternalNodeTest
    {
        public static void Test()
        {
            BinaryStream stream = new BinaryStream(new PooledMemoryStream());
            TreeHeader tree = new TreeHeader(stream, 4096);

            if (InternalNode.InternalStructureSize != 12) throw new Exception();
            if (InternalNode.CalculateMaximumChildren(4096) != (4096 - NodeHeader.Size - sizeof(uint)) / InternalNode.InternalStructureSize) throw new Exception();

            uint nodeIndex = InternalNode.CreateEmptyNode(tree, 3, 2, 1, 9);
            if (nodeIndex != 2) throw new Exception();

            NodeHeader node = new NodeHeader(tree, nodeIndex);
            if (node.Level != 3) throw new Exception();
            if (node.ChildCount != 1) throw new Exception();
            if (node.PreviousNode != 0) throw new Exception();
            if (node.NextNode != 0) throw new Exception();

            if (InternalNode.SeekToKey(tree, 2, nodeIndex) != SearchResults.StartOfEndOfStream) throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 4 + 1 * InternalNode.InternalStructureSize) throw new Exception();

            TestTryInsert(nodeIndex, tree);

            if (InternalNode.GetNodeIndex(tree, 3, nodeIndex) != 2345234L) throw new Exception();
            if (InternalNode.GetNodeIndex(tree, 9, nodeIndex) != 2322244L) throw new Exception();
            if (InternalNode.GetNodeIndex(tree, 0, nodeIndex) != 2324344) throw new Exception();
            if (InternalNode.GetNodeIndex(tree, 4, nodeIndex) != 2345234) throw new Exception();
            if (InternalNode.GetNodeIndex(tree, 20202020, nodeIndex) != 2345234) throw new Exception();
            if (InternalNode.GetNodeIndex(tree, -022020202, nodeIndex) != 2) throw new Exception();

            TestSplit();
        }
        static void TestSplit()
        {
            BinaryStream stream = new BinaryStream(new PooledMemoryStream());
            TreeHeader tree = new TreeHeader(stream, 4096);
            uint nodeIndex = InternalNode.CreateEmptyNode(tree, 1, 22, 0, 0);

            for (int x = 1; x < tree.MaximumInternalNodeChildren; x++)
                InternalNode.TryInsertKey(tree, x, (uint)x, nodeIndex);

            uint newNodeIndex;
            long middleKey;

            InternalNode.SplitNode(tree, nodeIndex, out newNodeIndex, out middleKey);

            if (newNodeIndex != 3) throw new Exception();
            if (middleKey != 170) throw new Exception();

            for (int x = 0; x < middleKey; x++)
            {
                if (InternalNode.GetNodeIndex(tree, x, nodeIndex) != x) throw new Exception();
                if (InternalNode.GetNodeIndex(tree, x, newNodeIndex) != 0) throw new Exception();
            }
            for (int x = (int)middleKey; x < tree.MaximumLeafNodeChildren; x++)
            {
                if (InternalNode.GetNodeIndex(tree, x, nodeIndex) != middleKey-1) throw new Exception();
                if (InternalNode.GetNodeIndex(tree, x, newNodeIndex) != x) throw new Exception();
            }

            NodeHeader node = new NodeHeader(tree, nodeIndex);
            if (node.ChildCount != 170) throw new Exception();
            if (node.Level != 1) throw new Exception();
            if (node.NextNode != newNodeIndex) throw new Exception();
            if (node.PreviousNode != 0) throw new Exception();

            node = new NodeHeader(tree, newNodeIndex);
            if (node.ChildCount != 170) throw new Exception();
            if (node.Level != 1) throw new Exception();
            if (node.NextNode != 0) throw new Exception();
            if (node.PreviousNode != nodeIndex) throw new Exception();
        }

        static void TestTryInsert(uint nodeIndex, TreeHeader tree)
        {
            if (InternalNode.TryInsertKey(tree, 3, 2345234, nodeIndex) != InsertResults.InsertedOK) throw new Exception();
            if (InternalNode.TryInsertKey(tree, 3, 23244, nodeIndex) != InsertResults.DuplicateKeyError) throw new Exception();
            if (InternalNode.TryInsertKey(tree, 9, 2322244, nodeIndex) != InsertResults.InsertedOK) throw new Exception();
            if (InternalNode.TryInsertKey(tree, 0, 2324344, nodeIndex) != InsertResults.InsertedOK) throw new Exception();

            if (InternalNode.SeekToKey(tree, 0, nodeIndex) != SearchResults.StartOfExactMatch) throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 4) throw new Exception();
            if (tree.Stream.ReadInt64() != 0) throw new Exception();
            if (tree.Stream.ReadInt32() != 2324344L) throw new Exception();

            if (InternalNode.SeekToKey(tree, -1, nodeIndex) != SearchResults.RightAfterClosestMatchWithoutGoingOver)
                throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 4) throw new Exception();
            if (tree.Stream.ReadInt64() != 0) throw new Exception();
            if (tree.Stream.ReadInt32() != 2324344L) throw new Exception();

            if (InternalNode.SeekToKey(tree, 2, nodeIndex) != SearchResults.RightAfterClosestMatchWithoutGoingOver)
                throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 4 + 2 * InternalNode.InternalStructureSize)
                throw new Exception();
            if (tree.Stream.ReadInt64() != 3) throw new Exception();
            if (tree.Stream.ReadInt32() != 2345234L) throw new Exception();

            if (InternalNode.SeekToKey(tree, 3, nodeIndex) != SearchResults.StartOfExactMatch) throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 4 + 2 * InternalNode.InternalStructureSize)
                throw new Exception();
            if (tree.Stream.ReadInt64() != 3) throw new Exception();
            if (tree.Stream.ReadInt32() != 2345234L) throw new Exception();

            if (InternalNode.SeekToKey(tree, 5, nodeIndex) != SearchResults.RightAfterClosestMatchWithoutGoingOver)
                throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 4 + 3 * InternalNode.InternalStructureSize)
                throw new Exception();
            if (tree.Stream.ReadInt64() != 9) throw new Exception();
            if (tree.Stream.ReadInt32() != 2322244L) throw new Exception();

            if (InternalNode.SeekToKey(tree, 9, nodeIndex) != SearchResults.StartOfExactMatch) throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 4 + 3 * InternalNode.InternalStructureSize)
                throw new Exception();
            if (tree.Stream.ReadInt64() != 9) throw new Exception();
            if (tree.Stream.ReadInt32() != 2322244L) throw new Exception();

            if (InternalNode.SeekToKey(tree, 100, nodeIndex) != SearchResults.StartOfEndOfStream) throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 4 + 4 * InternalNode.InternalStructureSize)
                throw new Exception();
            if (tree.Stream.ReadInt64() != 0) throw new Exception();
            if (tree.Stream.ReadInt32() != 0) throw new Exception();

            for (int x = 0; x < 336; x++)
            {
                if (InternalNode.TryInsertKey(tree, 10000 - x, 2345234, nodeIndex) != InsertResults.InsertedOK) throw new Exception();
            }
            if (InternalNode.TryInsertKey(tree, 10000, 2345234, nodeIndex) != InsertResults.NodeIsFullError) throw new Exception();
            if (InternalNode.TryInsertKey(tree, -1000, 2345234, nodeIndex) != InsertResults.NodeIsFullError) throw new Exception();



        }
    }
}
