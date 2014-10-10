//******************************************************************************************************
//  NodeHeader`1.cs - Gbtc
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
//  10/09/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace GSF.SortedTreeStore.Tree.Specialized
{
    /// <summary>
    /// Contains basic data about a node in the SortedTree.
    /// </summary>
    /// <typeparam name="TKey">The key that the SortedTree contains.</typeparam>
    public unsafe class NodeHeader<TKey>
        where TKey : SortedTreeTypeBase<TKey>, new()
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

        protected readonly byte Version;
        protected int KeySize;
        private byte* m_pointer;
        private byte* m_pointerAfterHeader;
        private long m_pointerReadVersion;
        private long m_pointerWriteVersion;
        protected SortedTreeTypeMethods<TKey> KeyMethods;
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
        /// <param name="version">The version code of the node.</param>
        protected NodeHeader(byte level, byte version)
        {
            Level = level;
            KeyMethods = new TKey().CreateValueMethods();
            Version = version;
            KeySize = new TKey().Size;
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
        private int OffsetOfUpperBounds
        {
            get
            {
                return OffsetOfLowerBounds + KeySize;
            }
        }

        /// <summary>
        /// Gets the byte offset of the header size.
        /// </summary>
        protected int HeaderSize
        {
            get
            {
                return OffsetOfLowerBounds + KeySize * 2;
            }
        }

        /// <summary>
        /// Gets the node index of this current node.
        /// </summary>
        public uint NodeIndex
        {
            get
            {
                return m_nodeIndex;
            }
        }

        /// <summary>
        /// Gets/Sets the number of unused bytes in the node.
        /// </summary>
        protected ushort RemainingBytes
        {
            get
            {
                return (ushort)(BlockSize - m_validBytes);
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

        /// <summary>
        /// The index of the right sibling. <see cref="uint.MaxValue"/> is the null case.
        /// </summary>
        public uint RightSiblingNodeIndex
        {
            get
            {
                return m_rightSiblingNodeIndex;
            }
            set
            {
                *(uint*)(GetWritePointer() + OffsetOfRightSibling) = value;
                m_rightSiblingNodeIndex = value;
            }
        }

        /// <summary>
        /// Is the index of the right sibling null. i.e. equal to <see cref="uint.MaxValue"/>
        /// </summary>
        protected bool IsRightSiblingIndexNull
        {
            get
            {
                return m_rightSiblingNodeIndex == uint.MaxValue;
            }
        }

        /// <summary>
        /// The lower bounds of the node. This is an inclusive bounds and always valid.
        /// </summary>
        public TKey LowerKey
        {
            get
            {
                return m_lowerKey;
            }
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
            get
            {
                return m_upperKey;
            }
            set
            {
                value.Write(GetWritePointer() + OffsetOfUpperBounds);
                value.CopyTo(m_upperKey);
            }
        }

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
                if (ptr[OffsetOfVersion] != Version)
                    throw new Exception("Unknown node Version.");
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
            var key = new TKey();
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
        /// Gets a write compatible pointer for the current node.
        /// </summary>
        /// <returns></returns>
        protected byte* GetWritePointer()
        {
            if (Stream.PointerVersion != m_pointerWriteVersion)
                UpdateWritePointer();
            return m_pointer;
        }

        private void UpdateReadPointer()
        {
            bool ptrSupportsWrite;
            m_pointer = Stream.GetReadPointer(BlockSize * NodeIndex, BlockSize, out ptrSupportsWrite);
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