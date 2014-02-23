//******************************************************************************************************
//  SortedTreeHeader.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  2/22/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace GSF.SortedTreeStore.Tree
{
    //Header 109:
    // Guid SparseIndexType = Always SortedTree.FixedSizeNode //first byte is 109
    // Guid TreeNodeType
    // Int BlockSize
    // Byte Version = 0
    // uint LastAllocatedBlock
    // uint RootNodeIndexAddress
    // byte RootNodeLevel
    // bool IsEmpty //Never Actually Used.
    //

    //Header 1:
    // byte Version = 1
    // Int BlockSize
    // Guid KeyCompressionMethod = Guid.Empty if not valid
    // Guid ValueCompressionMethod = Guid.Empty if not valid
    // Guid KeyValueCompressionMethod = Guid.Empty if not Valid
    // uint LastAllocatedBlock
    // uint RootNodeIndexAddress
    // byte RootNodeLevel
    // bool IsEmpty
    //
    
    public class SortedTreeHeader
    {
        Guid m_sparseIndexType;
        private Guid m_treeNodeType;
        private int m_blockSize;
        private uint m_lastAllocatedBlock;
        private uint m_rootNodeIndexAddress;
        private byte m_rootNodeLevel;
        /// <summary>
        /// Determines if the tree has any data in it.
        /// </summary>
        public bool IsEmpty
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets if the sorted tree needs to be flushed to the disk. 
        /// </summary>
        public bool IsDirty
        {
            get;
            private set;
        }

        public SortedTreeHeader()
        {

        }


        /// <summary>
        /// Loads the header.
        /// </summary>
        private void LoadHeader(BinaryStreamBase Stream)
        {
            Stream.Position = 0;
            if (m_sparseIndexType != Stream.ReadGuid())
                throw new Exception("Header Corrupt");
            if (m_treeNodeType != Stream.ReadGuid())
                throw new Exception("Header Corrupt");
            if (m_blockSize != Stream.ReadInt32())
                throw new Exception("Header Corrupt");
            if (Stream.ReadUInt8() != 0)
                throw new Exception("Header Corrupt");
            m_lastAllocatedBlock = Stream.ReadUInt32();
            m_rootNodeIndexAddress = Stream.ReadUInt32();
            m_rootNodeLevel = Stream.ReadUInt8();
            IsEmpty = Stream.ReadBoolean();
        }

        /// <summary>
        /// Writes the first page of the SortedTree as long as the <see cref="IsDirty"/> flag is set.
        /// After returning, the IsDirty flag is cleared.
        /// </summary>
        private void SaveHeader(BinaryStreamBase Stream)
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

            Stream.Position = oldPosotion;
            IsDirty = false;
        }
    }
}
