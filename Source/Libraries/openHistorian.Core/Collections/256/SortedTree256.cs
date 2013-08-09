////******************************************************************************************************
////  SortedTree256.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  4/5/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using System.Collections.Generic;
//using GSF;
//using GSF.IO;
//using openHistorian.Archive;
//using openHistorian.Collections.Generic;

//namespace openHistorian.Collections
//{
//    /// <summary>
//    /// Represents a collection of 128-bit key/128-bit values pairs that is very similiar to a <see cref="SortedList{int128,int128}"/> 
//    /// except it is optimal for storing millions to billions of entries and doing sequential scan of the data.
//    /// </summary>
//    public class SortedTree256
//        : SortedTree<UInt128, UInt128>
//    {
//        class TreeScanner 
//            : TreeScanner256Base
//        {
//            UInt128 m_tempKey = new UInt128();
//            TreeScannerBase<UInt128, UInt128> m_baseTree;
//            public TreeScanner(TreeScannerBase<UInt128, UInt128> baseTree)
//            {
//                m_baseTree = baseTree;
//            }
//            public override bool Read()
//            {
//                if (m_baseTree.Read())
//                {
//                    Key1 = m_baseTree.CurrentKey.Value1;
//                    Key2 = m_baseTree.CurrentKey.Value2;
//                    Value1 = m_baseTree.CurrentValue.Value1;
//                    Value2 = m_baseTree.CurrentValue.Value2;
//                    return true;
//                }
//                return false;
//            }

//            public override void SeekToKey(ulong key1, ulong key2)
//            {
//                m_tempKey.Value1 = key1;
//                m_tempKey.Value2 = key2;
//                m_baseTree.SeekToKey(m_tempKey);
//            }
//        }

//        UInt128 m_tempKey = new UInt128();
//        UInt128 m_tempValue = new UInt128();

//        //SortedTreeBase<KeyValue256> m_baseTree;

//        private SortedTree256(BinaryStreamBase stream1, BinaryStreamBase stream2)
//            : base(stream1, stream2)
//        {
//            FirstKey = ulong.MaxValue;
//            LastKey = ulong.MinValue;
//        }

//        protected override void OnLoadingHeader(BinaryStreamBase stream)
//        {
//            FirstKey = stream.ReadUInt64();
//            LastKey = stream.ReadUInt64();
//        }

//        protected override void OnSavingHeader(BinaryStreamBase stream)
//        {
//            stream.Write(FirstKey);
//            stream.Write(LastKey);
//        }

//        public ulong FirstKey { get; private set; }

//        public ulong LastKey { get; private set; }

//        public bool SkipIntermediateSaves
//        {
//            get
//            {
//                return !AutoFlush;
//            }
//            set
//            {
//                AutoFlush = !value;
//            }
//        }

//        /// <summary>
//        /// Adds the data contained in the <see cref="treeScanner"/> to this tree.
//        /// </summary>
//        /// <param name="treeScanner"></param>
//        /// <remarks>The tree is only read in order. No seeking of the tree occurs.</remarks>
//        public void Add(Stream256Base treeScanner)
//        {
//            ulong key1, key2, value1, value2;
//            while (treeScanner.Read(out key1, out key2, out value1, out value2))
//            {
//                Add(key1, key2, value1, value2);
//            }
//        }

//        /// <summary>
//        /// Adds the following data to the tree.
//        /// </summary>
//        /// <param name="key1">The unique key value.</param>
//        /// <param name="key2">The unique key value.</param>
//        /// <param name="value1">The value to insert.</param>
//        /// <param name="value2">The value to insert.</param>
//        public void Add(ulong key1, ulong key2, ulong value1, ulong value2)
//        {
//            m_tempKey.Value1 = key1;
//            m_tempKey.Value2 = key2;
//            m_tempValue.Value1 = value1;
//            m_tempValue.Value2 = value2;

//            if (key1 < FirstKey)
//            {
//                FirstKey = key1;
//                SetDirtyFlag();
//            }
//            if (key1 > LastKey)
//            {
//                LastKey = key1;
//                SetDirtyFlag();
//            }

//            Add(m_tempKey, m_tempValue);
//        }

//        /// <summary>
//        /// Gets the data for the following key. 
//        /// </summary>
//        /// <param name="key1">The key to look up.</param>
//        /// <param name="key2">The key to look up.</param>
//        /// <param name="value1">the value output</param>
//        /// <param name="value2">the value output</param>
//        public void Get(ulong key1, ulong key2, out ulong value1, out ulong value2)
//        {
//            m_tempKey.Value1 = key1;
//            m_tempKey.Value2 = key2;
//            Get(m_tempKey, m_tempValue);
//            value1 = m_tempValue.Value1;
//            value2 = m_tempValue.Value2;
//        }

//        /// <summary>
//        /// Returns a <see cref="ITreeScanner256"/> that can be used to parse throught the tree.
//        /// </summary>
//        /// <returns></returns>
//        public TreeScanner256Base GetTreeScanner()
//        {
//            return new TreeScanner(CreateTreeScanner());
//        }

//        public void Save()
//        {
//            Flush();
//        }

//        #region [ Static Methods ]

//        public static bool IsTypeKnown(Guid type)
//        {
//            if (type == default(SortedTree256CompressionNone).FileType)
//                return true;
//            if (type == default(SortedTree256CompressionDeltaEncoded).FileType)
//                return true;
//            if (type == default(SortedTree256CompressionTSEncoded).FileType)
//                return true;
//            if (type == default(SortedTree256CompressionTS2Encoded).FileType)
//                return true;
//            return false;
//        }

//        public static SortedTree256 Open(BinaryStreamBase stream)
//        {
//            return Open(stream, stream);
//        }


//        public static SortedTree256 Open(BinaryStreamBase stream1, BinaryStreamBase stream2)
//        {
//            Guid type;
//            int blockSize;
//            //SortedTree.ReadHeader(stream1, out type, out blockSize);

//            //if (!IsTypeKnown(type))

//            var tree = new SortedTree256(stream1, stream2);
//            tree.InitializeOpen();

//            //if (type == default(SortedTree256CompressionNone).FileType)
//            //{
//            //    tree.m_baseTree = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionNone>
//            //            (stream1, stream2, tree.OnLoadingHeader, tree.OnSavingHeader);
//            //}
//            //else if (type == default(SortedTree256CompressionDeltaEncoded).FileType)
//            //{
//            //    tree.m_baseTree = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionDeltaEncoded>
//            //            (stream1, stream2, tree.OnLoadingHeader, tree.OnSavingHeader);
//            //}
//            //else if (type == default(SortedTree256CompressionTSEncoded).FileType)
//            //{
//            //    tree.m_baseTree = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionTSEncoded>
//            //            (stream1, stream2, tree.OnLoadingHeader, tree.OnSavingHeader);
//            //}
//            //else if (type == default(SortedTree256CompressionTS2Encoded).FileType)
//            //{
//            //    tree.m_baseTree = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionTS2Encoded>
//            //            (stream1, stream2, tree.OnLoadingHeader, tree.OnSavingHeader);
//            //}
//            //else
//            //{
//            //    throw new Exception("Unknown Type");
//            //}
//            return tree;
//        }

//        public static SortedTree256 Create(BinaryStreamBase stream, int blockSize, CompressionMethod compression = CompressionMethod.None)
//        {
//            return Create(stream, stream, blockSize, compression);
//        }
//        public static SortedTree256 Create(BinaryStreamBase stream1, BinaryStreamBase stream2, int blockSize, CompressionMethod compression = CompressionMethod.None)
//        {
//            var tree = new SortedTree256(stream1, stream2);
//            tree.InitializeCreate(CreateFixedSizeNode.TypeGuid, CreateFixedSizeNode.TypeGuid, blockSize);

//            //if (compression == CompressionMethod.None)
//            //{
//            //    tree.m_baseTree = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionNone>
//            //            (stream1, stream2, blockSize, tree.OnLoadingHeader, tree.OnSavingHeader);
//            //}
//            //else if (compression == CompressionMethod.DeltaEncoded)
//            //{
//            //    tree.m_baseTree = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionDeltaEncoded>
//            //            (stream1, stream2, blockSize, tree.OnLoadingHeader, tree.OnSavingHeader);
//            //}
//            //else if (compression == CompressionMethod.TimeSeriesEncoded)
//            //{
//            //    tree.m_baseTree = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionTSEncoded>
//            //            (stream1, stream2, blockSize, tree.OnLoadingHeader, tree.OnSavingHeader);
//            //}
//            //else if (compression == CompressionMethod.TimeSeriesEncoded2)
//            //{
//            //    tree.m_baseTree = new SortedTree<KeyValue256, KeyValue256Methods, SortedTree256CompressionTS2Encoded>
//            //            (stream1, stream2, blockSize, tree.OnLoadingHeader, tree.OnSavingHeader);
//            //}
//            //else
//            //{
//            //    throw new Exception("Unknown Type");
//            //}
//            return tree;
//        }

//        #endregion

//    }
//}

