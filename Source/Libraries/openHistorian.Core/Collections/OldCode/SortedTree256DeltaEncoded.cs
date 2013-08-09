////******************************************************************************************************
////  SortedTree256DeltaEncoded.cs - Gbtc
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

//namespace openHistorian.Collections
//{
//    /// <summary>
//    /// Represents a collection of 128-bit key/128-bit values pairs that is very similiar to a <see cref="SortedList{int128,int128}"/> 
//    /// except it is optimal for storing millions to billions of entries and doing sequential scan of the data.
//    /// </summary>
//    public class SortedTree256DeltaEncoded : SortedTree256EncodedLeafNodeBase
//    {

//        // {0EF85145-F110-4F6F-937A-F90801CE5F2D}
//        static Guid s_fileType = new Guid(0x0ef85145, 0xf110, 0x4f6f, 0x93, 0x7a, 0xf9, 0x08, 0x01, 0xce, 0x5f, 0x2d);
//        public static Guid GetFileType()
//        {
//            return s_fileType;
//        }

//        /// <summary>
//        /// Loads an existing <see cref="SortedTree256"/>
//        /// from the provided stream.
//        /// </summary>
//        /// <param name="stream">The stream to load from</param>
//        public SortedTree256DeltaEncoded(BinaryStreamBase stream)
//            : base(stream, stream)
//        {
//        }

//        /// <summary>
//        /// Loads an existing <see cref="SortedTree256"/>
//        /// from the provided stream.
//        /// </summary>
//        /// <param name="stream">The stream to load from</param>
//        public SortedTree256DeltaEncoded(BinaryStreamBase stream1, BinaryStreamBase stream2)
//            : base(stream1, stream2)
//        {
//        }

//        /// <summary>
//        /// Creates an empty <see cref="SortedTree256"/> 
//        /// and writes the data to the provided stream. 
//        /// </summary>
//        /// <param name="stream">The stream to use to store the tree.</param>
//        /// <param name="blockSize">The size in bytes of a single block.</param>
//        public SortedTree256DeltaEncoded(BinaryStreamBase stream, int blockSize)
//            : base(stream, stream, blockSize)
//        {
//        }

//         /// <summary>
//        /// Creates an empty <see cref="SortedTree256"/> 
//        /// and writes the data to the provided stream. 
//        /// </summary>
//        /// <param name="stream">The stream to use to store the tree.</param>
//        /// <param name="blockSize">The size in bytes of a single block.</param>
//        public SortedTree256DeltaEncoded(BinaryStreamBase stream1, BinaryStreamBase stream2, int blockSize)
//            : base(stream1, stream2, blockSize)
//        {
//        }

//        protected override Guid FileType
//        {
//            get
//            {
//                return s_fileType;
//            }
//        }

//        protected override int MaximumEncodingSize
//        {
//            get
//            {
//                return 50;
//            }
//        }

//        protected override TreeScanner256Base LeafNodeGetScanner()
//        {
//            return new TreeScanner(this);
//        }

//        protected override unsafe int EncodeRecord(byte* buffer, KeyValuePair256 currentKey, KeyValuePair256 previousKey)
//        {
//            int size = 0;
//            Compression.Write7Bit(buffer, ref size, currentKey.Key1 ^ previousKey.Key1);
//            Compression.Write7Bit(buffer, ref size, currentKey.Key2 ^ previousKey.Key2);
//            Compression.Write7Bit(buffer, ref size, currentKey.Value1 ^ previousKey.Value1);
//            Compression.Write7Bit(buffer, ref size, currentKey.Value2 ^ previousKey.Value2);
//            return size;
//        }

//        protected override void DecodeNextRecord(KeyValuePair256 currentKey)
//        {
//            currentKey.Key1 ^= StreamLeaf.Read7BitUInt64();
//            currentKey.Key2 ^= StreamLeaf.Read7BitUInt64();
//            currentKey.Value1 ^= StreamLeaf.Read7BitUInt64();
//            currentKey.Value2 ^= StreamLeaf.Read7BitUInt64();
//        }

//        protected void DecodeNextRecord()
//        {
//            CurrentKey.Key1 ^= StreamLeaf.Read7BitUInt64();
//            CurrentKey.Key2 ^= StreamLeaf.Read7BitUInt64();
//            CurrentKey.Value1 ^= StreamLeaf.Read7BitUInt64();
//            CurrentKey.Value2 ^= StreamLeaf.Read7BitUInt64();
//        }

//        protected unsafe int EncodeRecord(byte* buffer)
//        {
//            int size = 0;
//            Compression.Write7Bit(buffer, ref size, CurrentKey.Key1 ^ PreviousKey.Key1);
//            Compression.Write7Bit(buffer, ref size, CurrentKey.Key2 ^ PreviousKey.Key2);
//            Compression.Write7Bit(buffer, ref size, CurrentKey.Value1 ^ PreviousKey.Value1);
//            Compression.Write7Bit(buffer, ref size, CurrentKey.Value2 ^ PreviousKey.Value2);
//            return size;
//        }

//        class TreeScanner : EncodedLeafTreeScannerBase
//        {
//            public TreeScanner(SortedTree256EncodedLeafNodeBase tree) : base(tree)
//            {
//            }

//            protected override unsafe void DecodeNextRecord()
//            {
//                Key1 ^= Stream.Read7BitUInt64();
//                Key2 ^= Stream.Read7BitUInt64();
//                Value1 ^= Stream.Read7BitUInt64();
//                Value2 ^= Stream.Read7BitUInt64();
//            }
//        }


//    }
//}

