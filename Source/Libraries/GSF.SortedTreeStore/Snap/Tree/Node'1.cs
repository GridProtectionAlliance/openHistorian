//******************************************************************************************************
//  Node`1.cs - Gbtc
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
//  03/15/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace GSF.Snap.Tree
{
    /// <summary>
    /// Contains basic data about a node in the SortedTree.
    /// </summary>
    /// <typeparam name="TKey">The key that the SortedTree contains.</typeparam>
    public unsafe class Node<TKey>
        where TKey : SnapTypeBase<TKey>, new()
    {
        /// <summary>
        /// Occurs when the node index is changed or cleared.
        /// </summary>
        protected event EventHandler NodeIndexChanged;

        protected const int OffsetOfVersion = 0;
        protected const int OffsetOfNodeLevel = OffsetOfVersion + sizeof(byte);
        protected const int OffsetOfRecordCount = OffsetOfNodeLevel + sizeof(byte);
        protected const int OffsetOfValidBytes = OffsetOfRecordCount + sizeof(ushort);
        protected const int OffsetOfLeftSibling = OffsetOfValidBytes + sizeof(ushort);
        protected const int OffsetOfRightSibling = OffsetOfLeftSibling + IndexSize;
        protected const int OffsetOfLowerBounds = OffsetOfRightSibling + IndexSize;
        protected const int IndexSize = sizeof(uint);

        private const byte Version = 0;
        protected int KeySize;
        private byte* m_pointer;
        private byte* m_pointerAfterHeader;
        private long m_pointerReadVersion;
        private long m_pointerWriteVersion;
        protected SnapTypeCustomMethods<TKey> KeyMethods;
        protected byte Level;
        protected int BlockSize;
        protected BinaryStreamPointerBase Stream;
        private uint m_nodeIndex;
        private ushort m_recordCount;
        private ushort m_validBytes;
        private uint m_leftSiblingNodeIndex;
        private uint m_rightSiblingNodeIndex;
        private TKey m_lowerKey;
        private TKey m_upperKey;
        private bool m_initialized;

        /// <summary>
        /// The constructor that is used for inheriting. Must call Initialize before using it.
        /// </summary>
        /// <param name="level"></param>
        protected Node(byte level)
        {
            Level = level;
            KeyMethods = new TKey().CreateValueMethods();
            KeySize = new TKey().Size;
        }

        /// <summary>
        /// The constructor that is to be used to if not inheriting this object. 
        /// Automatically initializes the node.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="blockSize"></param>
        /// <param name="level"></param>
        public Node(BinaryStreamPointerBase stream, int blockSize, byte level)
            : this(level)
        {
            InitializeNode(stream, blockSize);
        }

        /// <summary>
        /// Initializes the node. To be called once
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="blockSize"></param>
        protected void InitializeNode(BinaryStreamPointerBase stream, int blockSize)
        {
            if (m_initialized)
                throw new Exception("Duplicate calls to initialize");
            m_initialized = true;

            Stream = stream;
            BlockSize = blockSize;
            m_lowerKey = new TKey();
            m_upperKey = new TKey();
            Clear();
        }

        /// <summary>
        /// Gets the byte offset of the upper bouds key
        /// </summary>
        private int OffsetOfUpperBounds => OffsetOfLowerBounds + KeySize;

        /// <summary>
        /// Gets the byte offset of the header size.
        /// </summary>
        protected int HeaderSize => OffsetOfLowerBounds + KeySize * 2;

        /// <summary>
        /// Gets the node index of this current node.
        /// </summary>
        public uint NodeIndex => m_nodeIndex;

        /// <summary>
        /// Gets/Sets the number of records in this node.
        /// </summary>
        public ushort RecordCount
        {
            get => m_recordCount;
            set
            {
                *(ushort*)(GetWritePointer() + OffsetOfRecordCount) = value;
                m_recordCount = value;
            }
        }

        /// <summary>
        /// Gets/Sets the number of unused bytes in the node.
        /// </summary>
        protected ushort RemainingBytes => (ushort)(BlockSize - m_validBytes);

        /// <summary>
        /// The number of bytes that are used in this node.
        /// </summary>
        public ushort ValidBytes
        {
            get => m_validBytes;
            set
            {
                *(ushort*)(GetWritePointer() + OffsetOfValidBytes) = value;
                m_validBytes = value;
            }
        }

        /// <summary>
        /// Modifies both the <see cref="RecordCount"/> and <see cref="ValidBytes"/> in one function call.
        /// </summary>
        /// <param name="additionalValidBytes">the number of bytes to increase <see cref="ValidBytes"/> by</param>
        protected void IncrementOneRecord(int additionalValidBytes)
        {
            ushort* ptr = (ushort*)(GetWritePointer() + OffsetOfRecordCount);
            m_recordCount++;
            m_validBytes += (ushort)additionalValidBytes;
            ptr[0]++;
            ptr[1] += (ushort)additionalValidBytes;
        }

        protected void IncrementRecordCounts(int recordCount, int additionalValidBytes)
        {
            ushort* ptr = (ushort*)(GetWritePointer() + OffsetOfRecordCount);
            m_recordCount += (ushort)recordCount;
            m_validBytes += (ushort)additionalValidBytes;
            ptr[0] += (ushort)recordCount;
            ptr[1] += (ushort)additionalValidBytes;
        }

        /// <summary>
        /// The index of the left sibling. <see cref="uint.MaxValue"/> is the null case.
        /// </summary>
        public uint LeftSiblingNodeIndex
        {
            get => m_leftSiblingNodeIndex;
            set
            {
                *(uint*)(GetWritePointer() + OffsetOfLeftSibling) = value;
                m_leftSiblingNodeIndex = value;
            }
        }

        /// <summary>
        /// The index of the right sibling. <see cref="uint.MaxValue"/> is the null case.
        /// </summary>
        public uint RightSiblingNodeIndex
        {
            get => m_rightSiblingNodeIndex;
            set
            {
                *(uint*)(GetWritePointer() + OffsetOfRightSibling) = value;
                m_rightSiblingNodeIndex = value;
            }
        }

        /// <summary>
        /// Is the index of the right sibling null. i.e. equal to <see cref="uint.MaxValue"/>
        /// </summary>
        protected bool IsRightSiblingIndexNull => m_rightSiblingNodeIndex == uint.MaxValue;

        /// <summary>
        /// Is the index of the left sibling null. i.e. equal to <see cref="uint.MaxValue"/>
        /// </summary>
        protected bool IsLeftSiblingIndexNull => m_leftSiblingNodeIndex == uint.MaxValue;

        /// <summary>
        /// The lower bounds of the node. This is an inclusive bounds and always valid.
        /// </summary>
        public TKey LowerKey
        {
            get => m_lowerKey;
            set
            {
                value.Write(GetWritePointer() + OffsetOfLowerBounds);
                value.CopyTo(m_lowerKey);
            }
        }

        /// <summary>
        /// The upper bounds of the node. This is an exclusive bounds and is valid 
        /// when there is a sibling to the right. If there is no sibling to the right,
        /// it should still be valid except for the maximum key value condition.
        /// </summary>
        public TKey UpperKey
        {
            get => m_upperKey;
            set
            {
                value.Write(GetWritePointer() + OffsetOfUpperBounds);
                value.CopyTo(m_upperKey);
            }
        }

        /// <summary>
        /// The position that points to the location right after the header which is the 
        /// start of the data within the node.
        /// </summary>
        protected long StartOfDataPosition => NodeIndex * BlockSize + HeaderSize;

        /// <summary>
        /// Gets the first position for the current node.
        /// </summary>
        public long NodePosition => BlockSize * NodeIndex;

        /// <summary>
        /// Invalidates the current node.
        /// </summary>
        public void Clear()
        {
            if (NodeIndexChanged != null)
                NodeIndexChanged(this, EventArgs.Empty);
            //InsideNodeBoundary = m_BoundsFalse;
            m_nodeIndex = uint.MaxValue;
            m_pointerReadVersion = -1;
            m_pointerWriteVersion = -1;
            m_recordCount = 0;
            m_validBytes = (ushort)HeaderSize;
            m_leftSiblingNodeIndex = uint.MaxValue;
            m_rightSiblingNodeIndex = uint.MaxValue;
            UpperKey.Clear();
            LowerKey.Clear();
        }

        /// <summary>
        /// Sets the node data to the following node index. 
        /// The node must be initialized before calling this method.
        /// </summary>
        /// <param name="nodeIndex"></param>
        public void SetNodeIndex(uint nodeIndex)
        {
            if (nodeIndex == uint.MaxValue)
                throw new Exception("Invalid Node Index");
            if (m_nodeIndex != nodeIndex)
            {
                if (NodeIndexChanged != null)
                    NodeIndexChanged(this, EventArgs.Empty);
                m_nodeIndex = nodeIndex;
                m_pointerReadVersion = -1;
                m_pointerWriteVersion = -1;
                byte* ptr = GetReadPointer();
                if (ptr[OffsetOfNodeLevel] != Level)
                    throw new Exception("This node is not supposed to access the underlying node level.");
                m_recordCount = *(ushort*)(ptr + OffsetOfRecordCount);
                m_validBytes = *(ushort*)(ptr + OffsetOfValidBytes);
                m_leftSiblingNodeIndex = *(uint*)(ptr + OffsetOfLeftSibling);
                m_rightSiblingNodeIndex = *(uint*)(ptr + OffsetOfRightSibling);
                LowerKey.Read(ptr + OffsetOfLowerBounds);
                UpperKey.Read(ptr + OffsetOfUpperBounds);
            }
        }

        /// <summary>
        /// Creates an empty node on the provided key. Only use this to create the initial node of the tree. Not necessary for any other calls.
        /// </summary>
        /// <param name="newNodeIndex"></param>
        public void CreateEmptyNode(uint newNodeIndex)
        {
            TKey key = new TKey();
            byte* ptr = Stream.GetWritePointer(newNodeIndex * BlockSize, BlockSize);
            ptr[OffsetOfVersion] = Version;
            ptr[OffsetOfNodeLevel] = Level;
            *(ushort*)(ptr + OffsetOfRecordCount) = 0;
            *(ushort*)(ptr + OffsetOfValidBytes) = (ushort)HeaderSize;
            *(uint*)(ptr + OffsetOfLeftSibling) = uint.MaxValue;
            *(uint*)(ptr + OffsetOfRightSibling) = uint.MaxValue;
            key.SetMin();
            key.Write(ptr + OffsetOfLowerBounds);
            key.SetMax();
            key.Write(ptr + OffsetOfUpperBounds);
            SetNodeIndex(newNodeIndex);
        }

        /// <summary>
        /// Creates a new node with the provided data.
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="validBytes"></param>
        /// <param name="leftSibling"></param>
        /// <param name="rightSibling"></param>
        /// <param name="lowerKey"></param>
        /// <param name="upperKey"></param>
        protected void CreateNewNode(uint nodeIndex, ushort recordCount, ushort validBytes, uint leftSibling, uint rightSibling, TKey lowerKey, TKey upperKey)
        {
            byte* ptr = Stream.GetWritePointer(nodeIndex * BlockSize, BlockSize);
            ptr[OffsetOfVersion] = Version;
            ptr[OffsetOfNodeLevel] = Level;
            *(ushort*)(ptr + OffsetOfRecordCount) = recordCount;
            *(ushort*)(ptr + OffsetOfValidBytes) = validBytes;
            *(uint*)(ptr + OffsetOfLeftSibling) = leftSibling;
            *(uint*)(ptr + OffsetOfRightSibling) = rightSibling;
            lowerKey.Write(ptr + OffsetOfLowerBounds);
            upperKey.Write(ptr + OffsetOfUpperBounds);
        }

        /// <summary>
        /// Determines if the <see cref="key"/> resides within the bounds of the current node.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyInsideBounds(TKey key)
        {
            return NodeIndex != uint.MaxValue &&
                   (LeftSiblingNodeIndex == uint.MaxValue || LowerKey.IsLessThanOrEqualTo(key)) &&
                   (RightSiblingNodeIndex == uint.MaxValue || key.IsLessThan(UpperKey));
        }

        /// <summary>
        /// Seeks the current node to the right sibling node. Throws an exception if the navigation fails.
        /// </summary>
        public void SeekToRightSibling()
        {
            SetNodeIndex(RightSiblingNodeIndex);
        }

        /// <summary>
        /// Seeks the current node to the left sibling node. Throws an exception if the navigation fails.
        /// </summary>
        public void SeekToLeftSibling()
        {
            SetNodeIndex(LeftSiblingNodeIndex);
        }

        /// <summary>
        /// This function is a special purpose function to set the right sibling of an internal node.
        /// It is used by Split
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="nodeIndex"></param>
        protected void SetLeftSiblingProperty(uint nodeIndex, uint oldValue, uint newValue)
        {
            byte* ptr = Stream.GetWritePointer(BlockSize * nodeIndex, BlockSize);
            if (ptr[OffsetOfNodeLevel] != Level)
                throw new Exception("This node is not supposed to access the underlying node level.");

            if (*(uint*)(ptr + OffsetOfLeftSibling) != oldValue)
                throw new Exception("old value is not what was expected in the node.");
            *(uint*)(ptr + OffsetOfLeftSibling) = newValue;

            if (NodeIndex == nodeIndex)
                m_leftSiblingNodeIndex = newValue;
        }

        /// <summary>
        /// Gets the number of valid bytes of the foreign <see cref="nodeIndex"/> without seeking this current node to it.
        /// </summary>
        /// <param name="nodeIndex">the node to use</param>
        /// <returns></returns>
        protected int GetValidBytes(uint nodeIndex)
        {
            byte* ptr = Stream.GetReadPointer(BlockSize * nodeIndex, BlockSize);
            if (ptr[OffsetOfNodeLevel] != Level)
                throw new Exception("This node is not supposed to access the underlying node level.");
            return *(ushort*)(ptr + OffsetOfValidBytes);
        }

        /// <summary>
        /// Gets a read compatible pointer of the current node.
        /// </summary>
        /// <returns></returns>
        protected byte* GetReadPointer()
        {
            if (Stream.PointerVersion != m_pointerReadVersion)
                UpdateReadPointer();
            return m_pointer;
        }

        /// <summary>
        /// Gets the pointer after the header.
        /// </summary>
        /// <returns></returns>
        public byte* GetReadPointerAfterHeader()
        {
            if (Stream.PointerVersion != m_pointerReadVersion)
                UpdateReadPointer();
            return m_pointerAfterHeader;
        }

        /// <summary>
        /// Gets a write compatible pointer for the current node.
        /// </summary>
        /// <returns></returns>
        protected byte* GetWritePointer()
        {
            if (Stream.PointerVersion != m_pointerWriteVersion)
                UpdateWritePointer();
            return m_pointer;
        }

        /// <summary>
        /// Gets the pointer after the header.
        /// </summary>
        /// <returns></returns>
        protected byte* GetWritePointerAfterHeader()
        {
            if (Stream.PointerVersion != m_pointerWriteVersion)
                UpdateWritePointer();
            return m_pointerAfterHeader;
        }

        private void UpdateReadPointer()
        {
            m_pointer = Stream.GetReadPointer(BlockSize * NodeIndex, BlockSize, out bool ptrSupportsWrite);
            m_pointerAfterHeader = m_pointer + HeaderSize;
            m_pointerReadVersion = Stream.PointerVersion;
            if (ptrSupportsWrite)
                m_pointerWriteVersion = Stream.PointerVersion;
            else
                m_pointerWriteVersion = -1;
        }

        private void UpdateWritePointer()
        {
            m_pointer = Stream.GetWritePointer(BlockSize * NodeIndex, BlockSize);
            m_pointerAfterHeader = m_pointer + HeaderSize;
            m_pointerReadVersion = Stream.PointerVersion;
            m_pointerWriteVersion = Stream.PointerVersion;
        }
    }
}