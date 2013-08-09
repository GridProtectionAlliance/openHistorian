////******************************************************************************************************
////  SortedTreeStruct.cs - Gbtc
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
//using GSF.IO;
//using openHistorian.Archive;
//using openHistorian.Collections.Generic;

//namespace openHistorian.Collections
//{
//    public class SortedTreeStruct<TKey, TValue>
//        : SortedTree<Box<TKey>, Box<TValue>>
//        where TKey : new()
//        where TValue : new()
//    {
//        public class TreeScanner
//        {
//            public TKey Key
//            {
//                get
//                {
//                    return CurrentKey.Value;
//                }
//            }

//            public TValue Value
//            {
//                get
//                {
//                    return CurrentValue.Value;
//                }
//            }

//            //public uint CurrentKey;
//            //public uint CurrentValue;

//            public Box<TKey> CurrentKey;
//            public Box<TValue> CurrentValue;
//            public TreeScannerBase<Box<TKey>, Box<TValue>> m_baseTree;

//            public TreeScanner(TreeScannerBase<Box<TKey>, Box<TValue>> baseTree)
//            {
//                CurrentKey = baseTree.CurrentKey;
//                CurrentValue = baseTree.CurrentValue;
//                m_baseTree = baseTree;
//            }

//            public virtual bool Read()
//            {
//                return m_baseTree.Read();
//            }

//            public void SeekToKey(TKey key)
//            {
//                CurrentKey.Value = key;
//                m_baseTree.SeekToKey(CurrentKey);
//            }
//        }

//        private readonly Box<TKey> m_tempKey = new Box<TKey>();
//        private readonly Box<TValue> m_tempValue = new Box<TValue>();

//        private SortedTreeStruct(BinaryStreamBase stream1, BinaryStreamBase stream2)
//            : base(stream1, stream2)
//        {
//        }

//        protected override void OnLoadingHeader(BinaryStreamBase stream)
//        {
//        }

//        protected override void OnSavingHeader(BinaryStreamBase stream)
//        {
//        }

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

//        public void Remove(TKey key)
//        {
//            m_tempKey.Value = key;
//            if (!TryRemove(m_tempKey))
//                throw new Exception("Key Not Found");
//        }

//        /// <summary>
//        /// Adds the following data to the tree.
//        /// </summary>
//        /// <param name="key">The unique key value.</param>
//        /// <param name="value">The value to insert.</param>
//        public void Add(TKey key, TValue value)
//        {
//            m_tempKey.Value = key;
//            m_tempValue.Value = value;
//            if (!TryAdd(m_tempKey, m_tempValue))
//                throw new Exception("Duplicate Key");
//        }

//        /// <summary>
//        /// Gets the data for the following key. 
//        /// </summary>
//        /// <param name="key">The key to look up.</param>
//        public TValue Get(TKey key)
//        {
//            m_tempKey.Value = key;
//            if (!TryGet(m_tempKey, m_tempValue))
//                throw new Exception("Missing Key");
//            return m_tempValue.Value;
//        }

//        /// <summary>
//        /// Returns a <see cref="ITreeScanner256"/> that can be used to parse throught the tree.
//        /// </summary>
//        /// <returns></returns>
//        //public TreeScanner GetTreeScanner()
//        public TreeScanner GetTreeScanner()
//        {
//            return new TreeScanner(CreateTreeScanner());
//            //return m_baseTree.CreateTreeScanner();
//        }

//        public void Save()
//        {
//            Flush();
//        }

//        #region [ Static Methods ]

//        public static SortedTreeStruct<TKey, TValue> Open(BinaryStreamBase stream)
//        {
//            return Open(stream, stream);
//        }

//        public static SortedTreeStruct<TKey, TValue> Open(BinaryStreamBase stream1, BinaryStreamBase stream2)
//        {
//            SortedTreeStruct<TKey, TValue> tree = new SortedTreeStruct<TKey, TValue>(stream1, stream2);
//            tree.InitializeOpen();
//            return tree;
//        }

//        public static SortedTreeStruct<TKey, TValue> Create(BinaryStreamBase stream, int blockSize)
//        {
//            return Create(stream, stream, blockSize);
//        }

//        public static SortedTreeStruct<TKey, TValue> Create(BinaryStreamBase stream1, BinaryStreamBase stream2, int blockSize)
//        {
//            if (compression == CompressionMethod.None)
//            {
//                SortedTreeStruct<TKey, TValue> tree = new SortedTreeStruct<TKey, TValue>(stream1, stream2);
//                tree.InitializeCreate(CreateFixedSizeNode.TypeGuid, CreateFixedSizeNode.TypeGuid, blockSize);
//                return tree;
//            }
//            else
//            {
//                throw new Exception("Unknown Type");
//            }
//        }

//        #endregion
//    }
//}