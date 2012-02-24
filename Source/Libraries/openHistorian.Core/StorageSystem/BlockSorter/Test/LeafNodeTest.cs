using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.StorageSystem.BlockSorter
{
    class LeafNodeTest
    {
        public static void Test()
        {
            BinaryStream stream = new BinaryStream(new PooledMemoryStream());
            TreeHeader tree = new TreeHeader(stream, 4096);

            if (LeafNode.LeafStructureSize != 16) throw new Exception();
            if (LeafNode.CalculateMaximumChildren(4096) != (4096 - NodeHeader.Size) / LeafNode.LeafStructureSize) throw new Exception();

            uint nodeIndex = LeafNode.CreateEmptyNode(tree);
            if (nodeIndex != 2) throw new Exception();

            NodeHeader node = new NodeHeader(tree, nodeIndex);
            if (node.Level != 0) throw new Exception();
            if (node.ChildCount != 0) throw new Exception();
            if (node.PreviousNode != 0) throw new Exception();
            if (node.NextNode != 0) throw new Exception();

            if (LeafNode.SeekToKey(tree, 1, nodeIndex) != SearchResults.StartOfEndOfStream) throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size) throw new Exception();

            TestTryInsert(nodeIndex, tree);

            if (LeafNode.GetValueAddress(tree, 3, nodeIndex) != 2345234L) throw new Exception();
            if (LeafNode.GetValueAddress(tree, 9, nodeIndex) != 2322244L) throw new Exception();
            if (LeafNode.GetValueAddress(tree, 0, nodeIndex) != 2324344L) throw new Exception();
            if (LeafNode.GetValueAddress(tree, 4, nodeIndex) != -1) throw new Exception();
            if (LeafNode.GetValueAddress(tree, 20202020, nodeIndex) != -1) throw new Exception();
            if (LeafNode.GetValueAddress(tree, -022020202, nodeIndex) != -1) throw new Exception();

            TestSplit();
        }

        static void TestSplit()
        {
            BinaryStream stream = new BinaryStream(new PooledMemoryStream());
            TreeHeader tree = new TreeHeader(stream, 4096);
            uint nodeIndex = LeafNode.CreateEmptyNode(tree);

            for (int x = 0; x < tree.MaximumLeafNodeChildren; x++)
                LeafNode.TryInsertKey(tree, x, 12L,nodeIndex);

            uint newNodeIndex;
            long middleKey;

            LeafNode.SplitNode(tree,nodeIndex, out newNodeIndex, out middleKey);

            if (newNodeIndex!=3) throw new Exception();
            if (middleKey != 127) throw new Exception();

            for (int x = 0; x < middleKey; x++)
            {
                if (LeafNode.GetValueAddress(tree, x, nodeIndex) != 12) throw new Exception();
                if (LeafNode.GetValueAddress(tree, x, newNodeIndex) != -1) throw new Exception();
            }
            for (int x = (int)middleKey; x < tree.MaximumLeafNodeChildren; x++)
            {
                if (LeafNode.GetValueAddress(tree, x, nodeIndex) != -1) throw new Exception();
                if (LeafNode.GetValueAddress(tree, x, newNodeIndex) != 12) throw new Exception();
            }

            NodeHeader node = new NodeHeader(tree,nodeIndex);
            if (node.ChildCount != 127) throw new Exception();
            if (node.Level != 0) throw new Exception();
            if (node.NextNode != newNodeIndex) throw new Exception();
            if (node.PreviousNode != 0) throw new Exception();

            node = new NodeHeader(tree, newNodeIndex);
            if (node.ChildCount != 128) throw new Exception();
            if (node.Level != 0) throw new Exception();
            if (node.NextNode != 0) throw new Exception();
            if (node.PreviousNode != nodeIndex) throw new Exception();
        }

        static void TestTryInsert(uint nodeIndex, TreeHeader tree)
        {
            if (LeafNode.TryInsertKey(tree, 3, 2345234L, nodeIndex) != InsertResults.InsertedOK) throw new Exception();
            if (LeafNode.TryInsertKey(tree, 3, 23244L, nodeIndex) != InsertResults.DuplicateKeyError) throw new Exception();
            if (LeafNode.TryInsertKey(tree, 9, 2322244L, nodeIndex) != InsertResults.InsertedOK) throw new Exception();
            if (LeafNode.TryInsertKey(tree, 0, 2324344L, nodeIndex) != InsertResults.InsertedOK) throw new Exception();

            if (LeafNode.SeekToKey(tree, 0, nodeIndex) != SearchResults.StartOfExactMatch) throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size) throw new Exception();
            if (tree.Stream.ReadInt64() != 0) throw new Exception();
            if (tree.Stream.ReadInt64() != 2324344L) throw new Exception();

            if (LeafNode.SeekToKey(tree, -1, nodeIndex) != SearchResults.RightAfterClosestMatchWithoutGoingOver)
                throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size) throw new Exception();
            if (tree.Stream.ReadInt64() != 0) throw new Exception();
            if (tree.Stream.ReadInt64() != 2324344L) throw new Exception();

            if (LeafNode.SeekToKey(tree, 1, nodeIndex) != SearchResults.RightAfterClosestMatchWithoutGoingOver)
                throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 1 * LeafNode.LeafStructureSize)
                throw new Exception();
            if (tree.Stream.ReadInt64() != 3) throw new Exception();
            if (tree.Stream.ReadInt64() != 2345234L) throw new Exception();

            if (LeafNode.SeekToKey(tree, 3, nodeIndex) != SearchResults.StartOfExactMatch) throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 1 * LeafNode.LeafStructureSize)
                throw new Exception();
            if (tree.Stream.ReadInt64() != 3) throw new Exception();
            if (tree.Stream.ReadInt64() != 2345234L) throw new Exception();

            if (LeafNode.SeekToKey(tree, 5, nodeIndex) != SearchResults.RightAfterClosestMatchWithoutGoingOver)
                throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 2 * LeafNode.LeafStructureSize)
                throw new Exception();
            if (tree.Stream.ReadInt64() != 9) throw new Exception();
            if (tree.Stream.ReadInt64() != 2322244L) throw new Exception();

            if (LeafNode.SeekToKey(tree, 9, nodeIndex) != SearchResults.StartOfExactMatch) throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 2 * LeafNode.LeafStructureSize)
                throw new Exception();
            if (tree.Stream.ReadInt64() != 9) throw new Exception();
            if (tree.Stream.ReadInt64() != 2322244L) throw new Exception();

            if (LeafNode.SeekToKey(tree, 100, nodeIndex) != SearchResults.StartOfEndOfStream) throw new Exception();
            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 3 * LeafNode.LeafStructureSize)
                throw new Exception();
            if (tree.Stream.ReadInt64() != 0) throw new Exception();
            if (tree.Stream.ReadInt64() != 0) throw new Exception();

            for (int x = 0; x < 252; x++)
            {
                if (LeafNode.TryInsertKey(tree, 10000 - x, 2345234L, nodeIndex) != InsertResults.InsertedOK) throw new Exception();
            }
            if (LeafNode.TryInsertKey(tree, 10000, 2345234L, nodeIndex) != InsertResults.NodeIsFullError) throw new Exception();
            if (LeafNode.TryInsertKey(tree, -1000, 2345234L, nodeIndex) != InsertResults.NodeIsFullError) throw new Exception();



        }
    }
}
