//******************************************************************************************************
//  SortedTreeHeader.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/22/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Data;
using GSF.IO;

namespace GSF.Snap.Tree
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
    // EncodingDefinition method
    // uint LastAllocatedBlock
    // uint RootNodeIndexAddress
    // byte RootNodeLevel
    //

    internal class SortedTreeHeader
    {
        public EncodingDefinition TreeNodeType;
        public int BlockSize;
        public uint LastAllocatedBlock;
        public uint RootNodeIndexAddress;
        public byte RootNodeLevel;
      
        private bool m_isDirty;

        /// <summary>
        /// Gets if the sorted tree needs to be flushed to the disk. 
        /// </summary>
        public bool IsDirty
        {
            get => m_isDirty;
            set
            {
                if (!value)
                    throw new Exception();
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Loads the header.
        /// </summary>
        public void LoadHeader(BinaryStreamBase stream)
        {
            stream.Position = 0;
            byte version = stream.ReadUInt8();
            if (version == 109)
            {
                stream.Position = 0;
                if (EncodingDefinition.FixedSizeCombinedEncoding != new EncodingDefinition(stream.ReadGuid()))
                    throw new Exception("Header Corrupt");
                if (TreeNodeType != new EncodingDefinition(stream.ReadGuid()))
                    throw new Exception("Header Corrupt");
                if (BlockSize != stream.ReadInt32())
                    throw new Exception("Header Corrupt");
                if (stream.ReadUInt8() != 0)
                    throw new Exception("Header Corrupt");
                LastAllocatedBlock = stream.ReadUInt32();
                RootNodeIndexAddress = stream.ReadUInt32();
                RootNodeLevel = stream.ReadUInt8();
            }
            else if (version == 1)
            {
                if (BlockSize != stream.ReadInt32())
                    throw new Exception("Header Corrupt");
                if (TreeNodeType != new EncodingDefinition(stream))
                    throw new Exception("Header Corrupt");
                LastAllocatedBlock = stream.ReadUInt32();
                RootNodeIndexAddress = stream.ReadUInt32();
                RootNodeLevel = stream.ReadUInt8();
            }
            else
            {
                throw new VersionNotFoundException();
            }
        }

        /// <summary>
        /// Writes the first page of the SortedTree as long as the <see cref="IsDirty"/> flag is set.
        /// After returning, the IsDirty flag is cleared.
        /// </summary>
        public void SaveHeader(BinaryStreamBase stream)
        {
            if (!IsDirty)
                return;
            long oldPosotion = stream.Position;
            stream.Position = 0;
            stream.Write((byte)1);
            stream.Write(BlockSize);
            TreeNodeType.Save(stream);
            stream.Write(LastAllocatedBlock);
            stream.Write(RootNodeIndexAddress); //Root Index
            stream.Write(RootNodeLevel); //Root Index

            stream.Position = oldPosotion;
            m_isDirty = false;
        }
    }
}
