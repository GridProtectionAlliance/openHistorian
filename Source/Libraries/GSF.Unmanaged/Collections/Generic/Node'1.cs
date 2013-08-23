//******************************************************************************************************
//  TreeNodeBase_Node.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  3/15/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace openHistorian.Collections.Generic
{
    public unsafe class Node<TKey>
        where TKey : class, new()
    {
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
        protected TreeKeyMethodsBase<TKey> KeyMethods;
        protected byte Level;
        protected int BlockSize;
        protected BinaryStreamBase Stream;
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
        /// <param name="keyMethods"></param>
        protected Node(byte level, TreeKeyMethodsBase<TKey> keyMethods, byte version)
        {
            Level = level;
            KeyMethods = keyMethods;
            Version = version;
            KeySize = keyMethods.Size;
        }

        /// <summary>
        /// The constructor that is to be used to if not inheriting this object. 
        /// Automatically initializes the node.
        /// </summary>
        /// <param name="keyMethods"></param>
        /// <param name="stream"></param>
        /// <param name="blockSize"></param>
        /// <param name="level"></param>
        public Node(TreeKeyMethodsBase<TKey> keyMethods, BinaryStreamBase stream, int blockSize, byte level, byte version)
            : this(level, keyMethods, version)
        {
            InitializeNode(stream, blockSize);
        }

        /// <summary>
        /// Initializes the node. To be called 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="blockSize"></param>
        protected void InitializeNode(BinaryStreamBase stream, int blockSize)
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

        //public byte* Pointer
        //{
        //    get
        //    {
        //        return m_pointer;
        //    }
        //}

        public long PointerWriteVersion
        {
            get
            {
                return m_pointerWriteVersion;
            }
        }

        public long PointerReadVersion
        {
            get
            {
                return m_pointerReadVersion;
            }
        }

        private int OffsetOfUpperBounds
        {
            get
            {
                return OffsetOfLowerBounds + KeySize;
            }
        }

        protected int HeaderSize
        {
            get
            {
                return OffsetOfLowerBounds + KeySize * 2;
            }
        }

        public uint NodeIndex
        {
            get
            {
                return m_nodeIndex;
            }
        }

        public ushort RecordCount
        {
            get
            {
                return m_recordCount;
            }
            set
            {
                *(ushort*)(GetWritePointer() + OffsetOfRecordCount) = value;
                m_recordCount = value;
            }
        }

        public ushort RemainingBytes
        {
            get
            {
                return (ushort)(BlockSize - m_validBytes);
            }
        }

        public ushort ValidBytes
        {
            get
            {
                return m_validBytes;
            }
            set
            {
                *(ushort*)(GetWritePointer() + OffsetOfValidBytes) = value;
                m_validBytes = value;
            }
        }

        protected void IncrementOneRecord(int keyValueSize)
        {
            ushort* ptr = (ushort*)(GetWritePointer() + OffsetOfRecordCount);
            m_recordCount++;
            m_validBytes += (ushort)keyValueSize;
            ptr[0]++;
            ptr[1] += (ushort)keyValueSize;
        }

        /// <summary>
        /// The index of the left sibling. uint.MaxValue is the null case.
        /// </summary>
        public uint LeftSiblingNodeIndex
        {
            get
            {
                return m_leftSiblingNodeIndex;
            }
            set
            {
                *(uint*)(GetWritePointer() + OffsetOfLeftSibling) = value;
                m_leftSiblingNodeIndex = value;
            }
        }

        /// <summary>
        /// The index of the right sibling. uint.MaxValue is the null case.
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
        /// Is the index of the right sibling null. i.e. equal to uint.MaxValue
        /// </summary>
        public bool IsRightSiblingIndexNull
        {
            get
            {
                return m_rightSiblingNodeIndex == uint.MaxValue;
            }
        }

        /// <summary>
        /// Is the index of the left sibling null. i.e. equal to uint.MaxValue
        /// </summary>
        public bool IsLeftSiblingIndexNull
        {
            get
            {
                return m_leftSiblingNodeIndex == uint.MaxValue;
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
                KeyMethods.Write(GetWritePointer() + OffsetOfLowerBounds, value);
                KeyMethods.Copy(value, m_lowerKey);
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
                KeyMethods.Write(GetWritePointer() + OffsetOfUpperBounds, value);
                KeyMethods.Copy(value, m_upperKey);
            }
        }

        /// <summary>
        /// The position that points to the location right after the header which is the 
        /// start of the data within the node.
        /// </summary>
        public long StartOfDataPosition
        {
            get
            {
                return NodeIndex * BlockSize + HeaderSize;
            }
        }

        /// <summary>
        /// Gets the first position for the current node.
        /// </summary>
        public long NodePosition
        {
            get
            {
                return BlockSize * NodeIndex;
            }
        }

        /// <summary>
        /// Invalidates the current node.
        /// </summary>
        public void Clear()
        {
            ResetPositionCached();
            //InsideNodeBoundary = m_BoundsFalse;
            m_nodeIndex = uint.MaxValue;
            m_pointerReadVersion = -1;
            m_pointerWriteVersion = -1;
            m_recordCount = 0;
            m_validBytes = (ushort)HeaderSize;
            m_leftSiblingNodeIndex = uint.MaxValue;
            m_rightSiblingNodeIndex = uint.MaxValue;
            KeyMethods.Clear(UpperKey);
            KeyMethods.Clear(LowerKey);
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
                ResetPositionCached();
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
                KeyMethods.Read(ptr + OffsetOfLowerBounds, LowerKey);
                KeyMethods.Read(ptr + OffsetOfUpperBounds, UpperKey);
            }
        }

        /// <summary>
        /// Creates an empty node on the provided key. Only use this to create the initial node of the tree. Not necessary for any other calls.
        /// </summary>
        /// <param name="newNodeIndex"></param>
        public void CreateEmptyNode(uint newNodeIndex)
        {
            byte* ptr = Stream.GetWritePointer(newNodeIndex * BlockSize, BlockSize);
            ptr[OffsetOfVersion] = Version;
            ptr[OffsetOfNodeLevel] = Level;
            *(ushort*)(ptr + OffsetOfRecordCount) = 0;
            *(ushort*)(ptr + OffsetOfValidBytes) = (ushort)HeaderSize;
            *(uint*)(ptr + OffsetOfLeftSibling) = uint.MaxValue;
            *(uint*)(ptr + OffsetOfRightSibling) = uint.MaxValue;
            KeyMethods.WriteMin(ptr + OffsetOfLowerBounds);
            KeyMethods.WriteMax(ptr + OffsetOfUpperBounds);
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
        public void CreateNewNode(uint nodeIndex, ushort recordCount, ushort validBytes, uint leftSibling, uint rightSibling, TKey lowerKey, TKey upperKey)
        {
            byte* ptr = Stream.GetWritePointer(nodeIndex * BlockSize, BlockSize);
            ptr[OffsetOfVersion] = Version;
            ptr[OffsetOfNodeLevel] = Level;
            *(ushort*)(ptr + OffsetOfRecordCount) = recordCount;
            *(ushort*)(ptr + OffsetOfValidBytes) = validBytes;
            *(uint*)(ptr + OffsetOfLeftSibling) = leftSibling;
            *(uint*)(ptr + OffsetOfRightSibling) = rightSibling;
            KeyMethods.Write(ptr + OffsetOfLowerBounds, lowerKey);
            KeyMethods.Write(ptr + OffsetOfUpperBounds, upperKey);
        }

        /// <summary>
        /// Determines if the <see cref="key"/> resides within the bounds of the current node.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyInsideBounds(TKey key)
        {
            return (NodeIndex != uint.MaxValue) &&
                   (LeftSiblingNodeIndex == uint.MaxValue || KeyMethods.IsLessThanOrEqualTo(LowerKey, key)) &&
                   (RightSiblingNodeIndex == uint.MaxValue || KeyMethods.IsLessThan(key, UpperKey));
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
        /// It is used by <see cref="Split"/>.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="nodeIndex"></param>
        public void SetLeftSiblingProperty(uint nodeIndex, uint oldValue, uint newValue)
        {
            byte* ptr = Stream.GetWritePointer(BlockSize * nodeIndex, BlockSize);
            if (ptr[OffsetOfVersion] != Version)
                throw new Exception("Unknown node Version.");
            if (ptr[OffsetOfNodeLevel] != Level)
                throw new Exception("This node is not supposed to access the underlying node level.");

            if (*(uint*)(ptr + OffsetOfLeftSibling) != oldValue)
                throw new Exception("old value is not what was expected in the node.");
            *(uint*)(ptr + OffsetOfLeftSibling) = newValue;

            if (NodeIndex == nodeIndex)
                m_leftSiblingNodeIndex = newValue;
        }

        public int GetValidBytes(uint nodeIndex)
        {
            byte* ptr = Stream.GetReadPointer(BlockSize * nodeIndex, BlockSize);
            if (ptr[OffsetOfVersion] != Version)
                throw new Exception("Unknown node Version.");
            if (ptr[OffsetOfNodeLevel] != Level)
                throw new Exception("This node is not supposed to access the underlying node level.");
            return *(ushort*)(ptr + OffsetOfValidBytes);
        }

        public ushort GetRecordCount(uint nodeIndex)
        {
            byte* ptr = Stream.GetReadPointer(BlockSize * nodeIndex, BlockSize);
            if (ptr[OffsetOfVersion] != Version)
                throw new Exception("Unknown node Version.");
            if (ptr[OffsetOfNodeLevel] != Level)
                throw new Exception("This node is not supposed to access the underlying node level.");
            return *(ushort*)(ptr + OffsetOfRecordCount);
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

        protected virtual void ResetPositionCached()
        {
        }
    }
}