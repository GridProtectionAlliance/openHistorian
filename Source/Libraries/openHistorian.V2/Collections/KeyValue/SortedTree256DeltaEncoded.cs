//******************************************************************************************************
//  SortedTree256DeltaEncoded.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  4/5/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.V2.IO;

namespace openHistorian.V2.Collections.KeyValue
{
    /// <summary>
    /// Represents a collection of 128-bit key/128-bit values pairs that is very similiar to a <see cref="SortedList{int128,int128}"/> 
    /// except it is optimal for storing millions to billions of entries and doing sequential scan of the data.
    /// </summary>
    public class SortedTree256DeltaEncoded : SortedTree256EncodedLeafNodeBase
    {

        // {0EF85145-F110-4F6F-937A-F90801CE5F2D}
        static Guid s_fileType = new Guid(0x0ef85145, 0xf110, 0x4f6f, 0x93, 0x7a, 0xf9, 0x08, 0x01, 0xce, 0x5f, 0x2d);
        public static Guid GetFileType()
        {
            return s_fileType;
        }

        /// <summary>
        /// Loads an existing <see cref="SortedTree256"/>
        /// from the provided stream.
        /// </summary>
        /// <param name="stream">The stream to load from</param>
        public SortedTree256DeltaEncoded(BinaryStreamBase stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Creates an empty <see cref="SortedTree256"/> 
        /// and writes the data to the provided stream. 
        /// </summary>
        /// <param name="stream">The stream to use to store the tree.</param>
        /// <param name="blockSize">The size in bytes of a single block.</param>
        public SortedTree256DeltaEncoded(BinaryStreamBase stream, int blockSize)
            : base(stream, blockSize)
        {
        }

        protected override Guid FileType
        {
            get
            {
                return s_fileType;
            }
        }

        protected override unsafe int EncodeRecord(byte* buffer, ulong key1, ulong key2, ulong value1, ulong value2, ulong prevKey1, ulong prevKey2, ulong prevValue1, ulong prevValue2)
        {
            int size = 0;
            Compression.Write7Bit(buffer, ref size, key1 ^ prevKey1);
            Compression.Write7Bit(buffer, ref size, key2 ^ prevKey2);
            Compression.Write7Bit(buffer, ref size, value1 ^ prevValue1);
            Compression.Write7Bit(buffer, ref size, value2 ^ prevValue2);
            return size;
        }

        protected override void DecodeNextRecord(ref ulong curKey1, ref ulong curKey2, ref ulong curValue1, ref ulong curValue2)
        {
            curKey1 ^= Stream.Read7BitUInt64();
            curKey2 ^= Stream.Read7BitUInt64();
            curValue1 ^= Stream.Read7BitUInt64();
            curValue2 ^= Stream.Read7BitUInt64();
        }
    }
}
