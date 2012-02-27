//using System;

//namespace openHistorian.Core.StorageSystem.Generic
//{
//    class LeafNodeTest
//    {
//        public static void Test()
//        {
//            BinaryStream stream = new BinaryStream(new PooledMemoryStream());
//            BPlusTree<TreeTypeLong, TreeTypeLong> tree = new BPlusTree<TreeTypeLong, TreeTypeLong>(stream,4096);


//            if (tree.LeafStructureSize != 16) throw new Exception();
//            if (tree.MaximumLeafNodeChildren != (4096 - NodeHeader.Size) / tree.LeafStructureSize) throw new Exception();

//            uint nodeIndex = tree.LeafNodeCreateEmptyNode();
//            if (nodeIndex != 2) throw new Exception();

//            NodeHeader node = new NodeHeader(stream,4096, nodeIndex);
//            if (node.Level != 0) throw new Exception();
//            if (node.ChildCount != 0) throw new Exception();
//            if (node.PreviousNode != 0) throw new Exception();
//            if (node.NextNode != 0) throw new Exception();

//            if (tree.LeafNodeSeekToKey(1, nodeIndex) != SearchResults.StartOfEndOfStream) throw new Exception();
//            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size) throw new Exception();

//            TestTryInsert(nodeIndex, tree);

//            if (tree.LeafNodeGetValueAddress( 3, nodeIndex).Value != 2345234L) throw new Exception();
//            if (tree.LeafNodeGetValueAddress(9, nodeIndex).Value != 2322244L) throw new Exception();
//            if (tree.LeafNodeGetValueAddress(0, nodeIndex).Value != 2324344L) throw new Exception();
//            if (tree.LeafNodeGetValueAddress(4, nodeIndex) != null) throw new Exception();
//            if (tree.LeafNodeGetValueAddress(20202020, nodeIndex) != null) throw new Exception();
//            if (tree.LeafNodeGetValueAddress(-022020202, nodeIndex) != null) throw new Exception();

//            //TestSplit();
//        }

//        //static void TestSplit()
//        //{
//        //    BinaryStream stream = new BinaryStream(new PooledMemoryStream());
//        //    TreeHeader tree = new TreeHeader(stream, 4096);
//        //    uint nodeIndex = LeafNode.CreateEmptyNode(tree);

//        //    for (int x = 0; x < tree.MaximumLeafNodeChildren; x++)
//        //        LeafNode.TryInsertKey(tree, x, 12L, nodeIndex);

//        //    uint newNodeIndex;
//        //    long middleKey;

//        //    LeafNode.SplitNode(tree, nodeIndex, out newNodeIndex, out middleKey);

//        //    if (newNodeIndex != 3) throw new Exception();
//        //    if (middleKey != 127) throw new Exception();

//        //    for (int x = 0; x < middleKey; x++)
//        //    {
//        //        if (LeafNode.GetValueAddress(tree, x, nodeIndex) != 12) throw new Exception();
//        //        if (LeafNode.GetValueAddress(tree, x, newNodeIndex) != -1) throw new Exception();
//        //    }
//        //    for (int x = (int)middleKey; x < tree.MaximumLeafNodeChildren; x++)
//        //    {
//        //        if (LeafNode.GetValueAddress(tree, x, nodeIndex) != -1) throw new Exception();
//        //        if (LeafNode.GetValueAddress(tree, x, newNodeIndex) != 12) throw new Exception();
//        //    }

//        //    NodeHeader node = new NodeHeader(tree, nodeIndex);
//        //    if (node.ChildCount != 127) throw new Exception();
//        //    if (node.Level != 0) throw new Exception();
//        //    if (node.NextNode != newNodeIndex) throw new Exception();
//        //    if (node.PreviousNode != 0) throw new Exception();

//        //    node = new NodeHeader(tree, newNodeIndex);
//        //    if (node.ChildCount != 128) throw new Exception();
//        //    if (node.Level != 0) throw new Exception();
//        //    if (node.NextNode != 0) throw new Exception();
//        //    if (node.PreviousNode != nodeIndex) throw new Exception();
//        //}

//        static void TestTryInsert(uint nodeIndex, BPlusTree<TreeTypeLong, TreeTypeLong> tree)
//        {
//            if (tree.LeafNodeTryInsertKey( 3, 2345234L, nodeIndex) != InsertResults.InsertedOK) throw new Exception();
//            if (tree.LeafNodeTryInsertKey( 3, 23244L, nodeIndex) != InsertResults.DuplicateKeyError) throw new Exception();
//            if (tree.LeafNodeTryInsertKey( 9, 2322244L, nodeIndex) != InsertResults.InsertedOK) throw new Exception();
//            if (tree.LeafNodeTryInsertKey( 0, 2324344L, nodeIndex) != InsertResults.InsertedOK) throw new Exception();

//            if (tree.LeafNodeSeekToKey( 0, nodeIndex) != SearchResults.StartOfExactMatch) throw new Exception();
//            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size) throw new Exception();
//            if (tree.Stream.ReadInt64() != 0) throw new Exception();
//            if (tree.Stream.ReadInt64() != 2324344L) throw new Exception();

//            if (tree.LeafNodeSeekToKey( -1, nodeIndex) != SearchResults.RightAfterClosestMatchWithoutGoingOver)
//                throw new Exception();
//            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size) throw new Exception();
//            if (tree.Stream.ReadInt64() != 0) throw new Exception();
//            if (tree.Stream.ReadInt64() != 2324344L) throw new Exception();

//            if (tree.LeafNodeSeekToKey( 1, nodeIndex) != SearchResults.RightAfterClosestMatchWithoutGoingOver)
//                throw new Exception();
//            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 1 * tree.LeafStructureSize)
//                throw new Exception();
//            if (tree.Stream.ReadInt64() != 3) throw new Exception();
//            if (tree.Stream.ReadInt64() != 2345234L) throw new Exception();

//            if (tree.LeafNodeSeekToKey( 3, nodeIndex) != SearchResults.StartOfExactMatch) throw new Exception();
//            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 1 * tree.LeafStructureSize)
//                throw new Exception();
//            if (tree.Stream.ReadInt64() != 3) throw new Exception();
//            if (tree.Stream.ReadInt64() != 2345234L) throw new Exception();

//            if (tree.LeafNodeSeekToKey( 5, nodeIndex) != SearchResults.RightAfterClosestMatchWithoutGoingOver)
//                throw new Exception();
//            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 2 * tree.LeafStructureSize)
//                throw new Exception();
//            if (tree.Stream.ReadInt64() != 9) throw new Exception();
//            if (tree.Stream.ReadInt64() != 2322244L) throw new Exception();

//            if (tree.LeafNodeSeekToKey( 9, nodeIndex) != SearchResults.StartOfExactMatch) throw new Exception();
//            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 2 * tree.LeafStructureSize)
//                throw new Exception();
//            if (tree.Stream.ReadInt64() != 9) throw new Exception();
//            if (tree.Stream.ReadInt64() != 2322244L) throw new Exception();

//            if (tree.LeafNodeSeekToKey( 100, nodeIndex) != SearchResults.StartOfEndOfStream) throw new Exception();
//            if (tree.Stream.Position != nodeIndex * 4096 + NodeHeader.Size + 3 * tree.LeafStructureSize)
//                throw new Exception();
//            if (tree.Stream.ReadInt64() != 0) throw new Exception();
//            if (tree.Stream.ReadInt64() != 0) throw new Exception();

//            for (int x = 0; x < 252; x++)
//            {
//                if (tree.LeafNodeTryInsertKey( 10000 - x, 2345234L, nodeIndex) != InsertResults.InsertedOK) throw new Exception();
//            }
//            if (tree.LeafNodeTryInsertKey( 10000, 2345234L, nodeIndex) != InsertResults.NodeIsFullError) throw new Exception();
//            if (tree.LeafNodeTryInsertKey( -1000, 2345234L, nodeIndex) != InsertResults.NodeIsFullError) throw new Exception();



//        }
//    }
//}
