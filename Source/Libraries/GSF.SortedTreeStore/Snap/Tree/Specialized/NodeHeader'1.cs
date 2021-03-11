//******************************************************************************************************
//  NodeHeader`1.cs - Gbtc
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
//  10/09/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace GSF.Snap.Tree.Specialized
{
    /// <summary>
    /// Contains basic data about a node in the SortedTree.
    /// </summary>
    /// <typeparam name="TKey">The key that the SortedTree contains.</typeparam>
    public unsafe class NodeHeader<TKey>
        where TKey : SnapTypeBase<TKey>, new()
    {
        protected const int OffsetOfVersion = 0;
        protected const int OffsetOfNodeLevel = OffsetOfVersion + 1;
        protected const int OffsetOfRecordCount = OffsetOfNodeLevel + sizeof(byte);
        protected const int OffsetOfValidBytes = OffsetOfRecordCount + sizeof(ushort);
        protected const int OffsetOfLeftSibling = OffsetOfValidBytes + sizeof(ushort);
        protected const int OffsetOfRightSibling = OffsetOfLeftSibling + IndexSize;
        protected const int OffsetOfLowerBounds = OffsetOfRightSibling + IndexSize;
        protected const int IndexSize = sizeof(uint);

        /// <summary>
        /// Header data
        /// </summary>
        public const byte Version = 0;
        public readonly byte Level;
        public ushort RecordCount;
        public ushort ValidBytes;
        public uint LeftSiblingNodeIndex;
        public uint RightSiblingNodeIndex;
        public TKey LowerKey;
        public TKey UpperKey;

        public int KeySize;
        public uint NodeIndex;
        public int BlockSize;

        /// <summary>
        /// The constructor that is used for inheriting. Must call Initialize before using it.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="blockSize"></param>
        public NodeHeader(byte level, int blockSize)
        {
            Level = level;
            BlockSize = blockSize;
            LowerKey = new TKey();
            UpperKey = new TKey();
            KeySize = UpperKey.Size;
        }

        /// <summary>
        /// Gets the byte offset of the upper bounds key
        /// </summary>
        private int OffsetOfUpperBounds => OffsetOfLowerBounds + KeySize;

        /// <summary>
        /// Gets the byte offset of the header size.
        /// </summary>
        public int HeaderSize => OffsetOfLowerBounds + KeySize * 2;

        /// <summary>
        /// Gets/Sets the number of unused bytes in the node.
        /// </summary>
        public ushort RemainingBytes => (ushort)(BlockSize - ValidBytes);

        public void Save(byte* ptr)
        {
            ptr[0] = Version;
            ptr[OffsetOfNodeLevel] = Level;
            *(ushort*)(ptr + OffsetOfRecordCount) = RecordCount;
            *(ushort*)(ptr + OffsetOfValidBytes) = ValidBytes;
            *(uint*)(ptr + OffsetOfLeftSibling) = LeftSiblingNodeIndex;
            *(uint*)(ptr + OffsetOfRightSibling) = RightSiblingNodeIndex;
            LowerKey.Write(ptr + OffsetOfLowerBounds);
            UpperKey.Write(ptr + OffsetOfUpperBounds);
        }

    }
}