//******************************************************************************************************
//  TreeHeader.cs - Gbtc
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
//  2/22/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace openHistorian.Core.StorageSystem.Generic
{
    public partial class BPlusTree<TKey, TValue>
    {
        public static Guid FileType = new Guid("{7bfa9083-701e-4596-8273-8680a739271d}");
        BinaryStream Stream;
        int BlockSize;
        uint RootIndexAddress;
        byte RootIndexLevel;
        int MaximumLeafNodeChildren;
        int MaximumInternalNodeChildren;
        uint NextUnallocatedBlock;

        void TreeHeader(BinaryStream stream)
        {
            Load(stream);
        }

        void TreeHeader(BinaryStream stream, int blockSize)
        {
            Stream = stream;
            BlockSize = blockSize;
            MaximumLeafNodeChildren = LeafNodeCalculateMaximumChildren();
            MaximumInternalNodeChildren = InternalNodeCalculateMaximumChildren();
            NextUnallocatedBlock = 1;
            RootIndexAddress = LeafNodeCreateEmptyNode();
            RootIndexLevel = 0;
            Save(stream);
            Load(stream);
        }
        void Load(BinaryStream stream)
        {
            Stream = stream;
            Stream.Position = 0;
            if (FileType != stream.ReadGuid())
                throw new Exception("Header Corrupt");
            if (Stream.ReadByte() != 0)
                throw new Exception("Header Corrupt");
            NextUnallocatedBlock = stream.ReadUInt32();
            BlockSize = stream.ReadInt32();
            MaximumLeafNodeChildren = LeafNodeCalculateMaximumChildren();
            MaximumInternalNodeChildren = InternalNodeCalculateMaximumChildren();
            RootIndexAddress = stream.ReadUInt32();
            RootIndexLevel = stream.ReadByte();
        }
        void Save(BinaryStream stream)
        {
            stream.Position = 0;
            stream.Write(FileType);
            stream.Write((byte)0); //Version
            stream.Write(NextUnallocatedBlock);
            stream.Write(BlockSize);
            stream.Write(RootIndexAddress); //Root Index
            stream.Write(RootIndexLevel); //Root Index
        }

        /// <summary>
        /// Returns the node index address for a freshly allocated block.
        /// The node address is block alligned.
        /// </summary>
        /// <returns></returns>
        uint AllocateNewNode()
        {
            uint newBlock = NextUnallocatedBlock;
            NextUnallocatedBlock++;
            return newBlock;
        }

    }
}
