//******************************************************************************************************
//  SortedTree`2.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  3/22/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Tree.TreeNodes;

namespace GSF.SortedTreeStore.Tree
{
    /// <summary>
    /// Provides the basic user methods with any derived B+Tree.  
    /// This base class translates all of the core methods into simple methods
    /// that must be implemented by classes derived from this base class.
    /// </summary>
    /// <remarks>
    /// This class does not support concurrent read operations.  This is due to the caching method of each tree.
    /// If concurrent read operations are desired, clone the tree.  
    /// Trees cannot be cloned if the user plans to write to the tree.
    /// </remarks>
    public class SortedTree<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        #region [ Members ]

        private byte m_rootNodeLevel;
        private uint m_rootNodeIndexAddress;
        private uint m_lastAllocatedBlock;
        protected SparseIndex<TKey> Indexer;
        protected SortedTreeNodeBase<TKey, TValue> LeafStorage;
        private readonly TValue m_tempValue = new TValue();

        private Guid m_sparseIndexType;
        private Guid m_treeNodeType;
        private int m_blockSize;
        private bool m_isInitialized;
        protected SortedTreeKeyMethodsBase<TKey> KeyMethods;
        protected SortedTreeValueMethodsBase<TValue> ValueMethods;

        #endregion

        #region [ Constructors ]

        internal SortedTree(BinaryStreamBase stream1, BinaryStreamBase stream2)
        {
            KeyMethods = new TKey().CreateKeyMethods();
            ValueMethods = new TValue().CreateValueMethods();
            Stream = stream1;
            StreamLeaf = stream2;
            m_isInitialized = false;
            AutoFlush = true;
        }

        internal void InitializeOpen()
        {
            if (m_isInitialized)
                throw new Exception("Duplicate calls to Initialize");
            m_isInitialized = true;

            SortedTree.ReadHeader(Stream, out m_sparseIndexType, out m_treeNodeType, out m_blockSize);
            //Since m_loadHeader is currently null in the constructor, 
            //this will load as much data that this tree can load.
            LoadHeader();

            Initialize();

            LoadHeader();
        }

        internal void InitializeCreate(Guid sparseIndexType, Guid treeNodeType, int blockSize)
        {
            if (m_isInitialized)
                throw new Exception("Duplicate calls to Initialize");
            m_isInitialized = true;

            m_sparseIndexType = sparseIndexType;
            m_treeNodeType = treeNodeType;
            m_blockSize = blockSize;

            IsEmpty = true;
            m_rootNodeLevel = 0;
            m_rootNodeIndexAddress = 1;
            m_lastAllocatedBlock = 1;

            Initialize();

            LeafStorage.CreateEmptyNode(m_rootNodeIndexAddress);
            IsDirty = true;
            SaveHeader();
        }

        private void Initialize()
        {
            Indexer = new SparseIndex<TKey>(m_sparseIndexType);
            LeafStorage = TreeNodeInitializer.CreateTreeNode<TKey, TValue>(m_treeNodeType, 0);
            Indexer.RootHasChanged += IndexerOnRootHasChanged;
            Indexer.Initialize(Stream, m_blockSize, GetNextNewNodeIndex, m_rootNodeLevel, m_rootNodeIndexAddress);
            LeafStorage.Initialize(StreamLeaf, m_blockSize, GetNextNewNodeIndex, Indexer);
        }

        private void IndexerOnRootHasChanged(object sender, EventArgs eventArgs)
        {
            m_rootNodeLevel = Indexer.RootNodeLevel;
            m_rootNodeIndexAddress = Indexer.RootNodeIndexAddress;
            SetDirtyFlag();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets if the sorted tree needs to be flushed to the disk. 
        /// </summary>
        public bool IsDirty
        {
            get;
            private set;
        }

        /// <summary>
        /// The sorted tree will not continuely call the <see cref="Flush"/> method every time the header is changed.
        /// When setting this to false, flushes must be manually invoked. Failing to do this can corrupt the SortedTree. 
        /// Only set if you can gaurentee that <see cref="Flush"/> will be called before disposing this class.
        /// </summary>
        public bool AutoFlush
        {
            get;
            set;
        }

        /// <summary>
        /// Determines if the tree has any data in it.
        /// </summary>
        public bool IsEmpty
        {
            get;
            private set;
        }

        /// <summary>
        /// Contains the block size that the tree nodes will be alligned on.
        /// </summary>
        protected int BlockSize
        {
            get
            {
                return m_blockSize;
            }
        }

        /// <summary>
        /// Contains the stream for reading and writing.
        /// </summary>
        protected BinaryStreamBase Stream
        {
            get;
            private set;
        }

        protected BinaryStreamBase StreamLeaf
        {
            get;
            private set;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Flushes any header data that may have changed to the main stream.
        /// </summary>
        public void Flush()
        {
            if (!m_isInitialized)
                throw new Exception("Class has not been initialized");
            SaveHeader();
        }

        /// <summary>
        /// Sets a flag that requires that the header data is no longer valid.
        /// </summary>
        public void SetDirtyFlag()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Adds the provided key/value to the Tree.
        /// </summary>
        /// <param name="key">the key to add</param>
        /// <param name="value">the value to add</param>
        public void Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
                throw new Exception("Key already exists");
        }
        /// <summary>
        /// Attempts to add the provided key/value to the Tree.
        /// </summary>
        /// <param name="key">the key to add</param>
        /// <param name="value">the value to add</param>
        /// <returns>returns true if successful, false if a duplicate key was found</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (LeafStorage.TryInsert(key, value))
            {
                if (IsDirty && AutoFlush)
                    SaveHeader();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds all of the points in the stream to the Tree
        /// </summary>
        /// <param name="stream">stream to add</param>
        public void AddRange(TreeStream<TKey, TValue> stream)
        {
            while (stream.Read())
            {
                Add(stream.CurrentKey, stream.CurrentValue);
            }
        }
        /// <summary>
        /// Adds all of the items in the stream to this tree. Skips any dulpicate entries.
        /// </summary>
        /// <param name="stream">the stream to add.</param>
        public void TryAddRange(TreeStream<TKey, TValue> stream)
        {
            while (stream.Read())
            {
                TryAdd(stream.CurrentKey, stream.CurrentValue);
            }
        }
        /// <summary>
        /// Tries to remove the following key from the tree.
        /// </summary>
        /// <param name="key">the key to remove</param>
        /// <returns>true if successful, false otherwise.</returns>
        public bool TryRemove(TKey key)
        {
            if (LeafStorage.TryRemove(key))
            {
                if (IsDirty && AutoFlush)
                    SaveHeader();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the following key from the Tree. Assignes to the value.
        /// </summary>
        /// <param name="key">the key to look for</param>
        /// <param name="value">the place to store the value</param>
        public void Get(TKey key, TValue value)
        {
            if (!TryGet(key, value))
                throw new Exception("Key does not exists");
        }
        /// <summary>
        /// Attempts to get the following key from the Tree. Assigns to the value.
        /// </summary>
        /// <param name="key">the key to look for</param>
        /// <param name="value">the place to store the value</param>
        /// <returns>True if successful, False otherwise.</returns>
        public bool TryGet(TKey key, TValue value)
        {
            return LeafStorage.TryGet(key, value);
        }

        /// <summary>
        /// Gets the lower and upper bounds of this tree.
        /// </summary>
        /// <param name="lowerBounds">The first key in the tree</param>
        /// <param name="upperBounds">The final key in the tree</param>
        /// <remarks>
        /// If the tree contains no data. <see cref="lowerBounds"/> is set to it's maximum value
        /// and <see cref="upperBounds"/> is set to it's minimum value.
        /// </remarks>
        public void GetKeyRange(TKey lowerBounds, TKey upperBounds)
        {
            if (LeafStorage.TryGetFirstRecord(lowerBounds, m_tempValue) &&
                LeafStorage.TryGetLastRecord(upperBounds, m_tempValue))
            {
                return;
            }
            KeyMethods.SetMax(lowerBounds);
            KeyMethods.SetMin(upperBounds);
        }

        #endregion

        #region [ Protected Methods ]

        protected virtual void OnLoadingHeader(BinaryStreamBase stream)
        {
        }

        protected virtual void OnSavingHeader(BinaryStreamBase stream)
        {
        }

        /// <summary>
        /// Creates a tree scanner that can be used to seek this tree.
        /// </summary>
        /// <returns></returns>
        public SortedTreeScannerBase<TKey, TValue> CreateTreeScanner()
        {
            return LeafStorage.CreateTreeScanner();
        }

        /// <summary>
        /// Returns the node index address for a freshly allocated block.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Also saves the header data</remarks>
        protected uint GetNextNewNodeIndex()
        {
            m_lastAllocatedBlock++;
            SetDirtyFlag();
            return m_lastAllocatedBlock;
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Loads the header.
        /// </summary>
        private void LoadHeader()
        {
            Stream.Position = 0;
            if (m_sparseIndexType != Stream.ReadGuid())
                throw new Exception("Header Corrupt");
            if (m_treeNodeType != Stream.ReadGuid())
                throw new Exception("Header Corrupt");
            if (m_blockSize != Stream.ReadInt32())
                throw new Exception("Header Corrupt");
            if (Stream.ReadByte() != 0)
                throw new Exception("Header Corrupt");
            m_lastAllocatedBlock = Stream.ReadUInt32();
            m_rootNodeIndexAddress = Stream.ReadUInt32();
            m_rootNodeLevel = Stream.ReadByte();
            IsEmpty = Stream.ReadBoolean();
            OnLoadingHeader(Stream);
        }

        /// <summary>
        /// Writes the first page of the SortedTree as long as the <see cref="IsDirty"/> flag is set.
        /// After returning, the IsDirty flag is cleared.
        /// </summary>
        private void SaveHeader()
        {
            if (!IsDirty)
                return;
            long oldPosotion = Stream.Position;
            Stream.Position = 0;
            Stream.Write(m_sparseIndexType);
            Stream.Write(m_treeNodeType);
            Stream.Write(m_blockSize);
            Stream.Write((byte)0); //version
            Stream.Write(m_lastAllocatedBlock);
            Stream.Write(m_rootNodeIndexAddress); //Root Index
            Stream.Write(m_rootNodeLevel); //Root Index
            Stream.Write(IsEmpty);
            OnSavingHeader(Stream);

            Stream.Position = oldPosotion;
            IsDirty = false;
        }

        /// <summary>
        /// Opens a sorted tree using the provided stream.
        /// </summary>
        /// <param name="stream">the stream to use to open.</param>
        /// <returns></returns>
        public static SortedTree<TKey, TValue> Open(BinaryStreamBase stream)
        {
            return Open(stream, stream);
        }
        /// <summary>
        /// Opens a SortedTree using the provided streams. (two streams pointing to the same data source)
        /// </summary>
        /// <param name="stream1">the first stream</param>
        /// <param name="stream2">the second stream</param>
        /// <returns></returns>
        public static SortedTree<TKey, TValue> Open(BinaryStreamBase stream1, BinaryStreamBase stream2)
        {
            SortedTree<TKey, TValue> tree = new SortedTree<TKey, TValue>(stream1, stream2);
            tree.InitializeOpen();
            return tree;
        }

        /// <summary>
        /// Creates a new FixedSize SortedTree using the provided stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public static SortedTree<TKey, TValue> Create(BinaryStreamBase stream, int blockSize)
        {
            return Create(stream, stream, blockSize);
        }
        /// <summary>
        /// Creates a new FixedSize SortedTree using the provided streams.
        /// </summary>
        /// <param name="stream1"></param>
        /// <param name="stream2"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public static SortedTree<TKey, TValue> Create(BinaryStreamBase stream1, BinaryStreamBase stream2, int blockSize)
        {
            return Create(stream1, stream2, blockSize, SortedTree.FixedSizeNode);
        }
        /// <summary>
        /// Creates a new SortedTree writing to the provided stream and using the specified compression metho for the tree node.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="blockSize"></param>
        /// <param name="treeNodeType"></param>
        /// <returns></returns>
        public static SortedTree<TKey, TValue> Create(BinaryStreamBase stream, int blockSize, Guid treeNodeType)
        {
            return Create(stream, stream, blockSize, treeNodeType);
        }
        /// <summary>
        /// Creates a new SortedTree writing to the provided streams and using the specified compression metho for the tree node.
        /// </summary>
        /// <param name="stream1"></param>
        /// <param name="stream2"></param>
        /// <param name="blockSize"></param>
        /// <param name="treeNodeType"></param>
        /// <returns></returns>
        public static SortedTree<TKey, TValue> Create(BinaryStreamBase stream1, BinaryStreamBase stream2, int blockSize, Guid treeNodeType)
        {
            SortedTree<TKey, TValue> tree = new SortedTree<TKey, TValue>(stream1, stream2);
            tree.InitializeCreate(SortedTree.FixedSizeNode, treeNodeType, blockSize);
            return tree;
        }

        #endregion
    }
}