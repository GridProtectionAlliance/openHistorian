//******************************************************************************************************
//  EncodedNodeBase`2.cs - Gbtc
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
//  5/7/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF;

namespace GSF.SortedTreeStore.Tree.TreeNodes
{
    /// <summary>
    /// A TreeNode abstract class that is used for linearly encoding a class.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract unsafe class EncodedNodeBase<TKey, TValue>
        : SortedTreeNodeBase<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        private int m_nextOffset;
        private int m_currentOffset;
        private int m_currentIndex;
        private readonly TKey m_nullKey;
        private readonly TValue m_nullValue;
        private readonly TKey m_currentKey;
        private readonly TValue m_currentValue;
        private readonly TKey m_prevKey;
        private readonly TValue m_prevValue;
        private byte[] m_buffer1;
        private byte[] m_buffer2;

        protected EncodedNodeBase(byte level, byte version)
            : base(level, version)
        {
            if (level != 0)
                throw new ArgumentException("Level for this type is only supported on the leaf level.");
            m_currentKey = new TKey();
            m_currentValue = new TValue();
            m_prevKey = new TKey();
            m_prevValue = new TValue();
            m_nullKey = new TKey();
            m_nullValue = new TValue();
            KeyMethods.Clear(m_nullKey);
            ValueMethods.Clear(m_nullValue);

            NodeIndexChanged += OnNodeIndexChanged;
            ClearNodeCache();

        }

        protected override void InitializeType()
        {
            m_buffer1 = new byte[MaximumStorageSize];
            m_buffer2 = new byte[MaximumStorageSize];
            if ((BlockSize - HeaderSize) / MaximumStorageSize < 4)
                throw new Exception("Tree must have at least 4 records per node. Increase the block size or decrease the size of the records.");
        }

        /// <summary>
        /// Encodes this record to the provided stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="prevKey"></param>
        /// <param name="prevValue"></param>
        /// <param name="currentKey"></param>
        /// <param name="currentValue"></param>
        /// <returns>returns the number of bytes read from the stream</returns>
        protected abstract int EncodeRecord(byte* stream, TKey prevKey, TValue prevValue, TKey currentKey, TValue currentValue);

        /// <summary>
        /// Decodes the record from the stream.
        /// </summary>
        /// <param name="stream">the stream where the record is stored</param>
        /// <param name="buffer">a temporary buffer than can be used to decode the stream if needed</param>
        /// <param name="prevKey">the key value that was read</param>
        /// <param name="prevValue">the previous value that was read</param>
        /// <param name="currentKey">where to store the decoded key</param>
        /// <param name="currentValue">where to store the decoded value</param>
        /// <returns></returns>
        protected abstract int DecodeRecord(byte* stream, byte* buffer, TKey prevKey, TValue prevValue, TKey currentKey, TValue currentValue);

        /// <summary>
        /// The maximum size that will ever be needed to encode or decode this data.
        /// </summary>
        protected abstract int MaximumStorageSize
        {
            get;
        }

        
        protected override void Read(int index, TValue value)
        {
            if (index == RecordCount)
                throw new Exception();
            fixed (byte* buffer = m_buffer1)
                SeekTo(index, buffer);
            ValueMethods.Copy(m_currentValue, value);
        }

        protected override void Read(int index, TKey key, TValue value)
        {
            if (index == RecordCount)
                throw new Exception();
            fixed (byte* buffer = m_buffer1)
                SeekTo(index, buffer);
            KeyMethods.Copy(m_currentKey, key);
            ValueMethods.Copy(m_currentValue, value);
        }

        protected override bool RemoveUnlessOverflow(int index)
        {
            throw new NotImplementedException();
            //if (index != (RecordCount - 1))
            //{
            //    byte* start = GetWritePointerAfterHeader() + index * KeyValueSize;
            //    Memory.Copy(start + KeyValueSize, start, (RecordCount - index - 1) * KeyValueSize);
            //}

            ////save the header
            //RecordCount--;
            //ValidBytes -= (ushort)KeyValueSize;
            //return true;
        }

        /// <summary>
        /// Inserts a point before the current position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool InsertUnlessFull(int index, TKey key, TValue value)
        {
            fixed (byte* buffer = m_buffer1)
            {
                if (index == RecordCount)
                {
                    //Insert After
                    SeekTo(index, buffer);
                    return InsertAfter(key, value);
                }
                else
                {
                    //Insert Between
                    SeekTo(index, buffer);
                    return InsertBetween(key, value);
                }
            }
        }

        private bool InsertAfter(TKey key, TValue value)
        {
            fixed (byte* buffer = m_buffer1)
            {
                int length = EncodeRecord(buffer, m_prevKey, m_prevValue, key, value);

                if (RemainingBytes < length)
                    return false;

                EncodeRecord(GetWritePointer() + m_nextOffset, m_prevKey, m_prevValue, key, value);
                //WinApi.MoveMemory(GetWritePointer() + m_nextOffset, buffer, length);
                IncrementOneRecord(length);

                KeyMethods.Copy(key, m_currentKey);
                ValueMethods.Copy(value, m_currentValue);
                m_nextOffset = m_currentOffset + length;
                //ResetPositionCached();

                return true;
            }
        }

        private bool InsertBetween(TKey key, TValue value)
        {
            fixed (byte* buffer = m_buffer1, buffer2 = m_buffer2)
            {
                int shiftDelta1 = EncodeRecord(buffer, m_prevKey, m_prevValue, key, value);
                int shiftDelta2 = EncodeRecord(buffer2, key, value, m_currentKey, m_currentValue);
                int shiftDelta = shiftDelta1 + shiftDelta2;

                shiftDelta -= (m_nextOffset - m_currentOffset);

                if (RemainingBytes < shiftDelta)
                    return false;

                Stream.Position = NodePosition + m_currentOffset;
                if (shiftDelta < 0)
                    Stream.RemoveBytes(-shiftDelta, (int)(ValidBytes - m_currentOffset));
                else
                    Stream.InsertBytes(shiftDelta, (int)(ValidBytes - m_currentOffset));

                Stream.Write(buffer, shiftDelta1);
                Stream.Write(buffer2, shiftDelta2);

                IncrementOneRecord(shiftDelta);

                KeyMethods.Copy(key, m_currentKey);
                ValueMethods.Copy(value, m_currentValue);
                m_nextOffset = m_currentOffset + shiftDelta1;

                //ResetPositionCached();

                return true;
            }
        }

        //protected override bool InsertUnlessFull(int index, TKey key, TValue value)
        //{
        //    throw new NotImplementedException();
        //    if (RecordCount >= m_maxRecordsPerNode)
        //        return false;

        //    byte* start = GetWritePointerAfterHeader() + index * KeyValueSize;

        //    if (index != RecordCount)
        //    {
        //        WinApi.MoveMemory(start + KeyValueSize, start, (RecordCount - index) * KeyValueSize);
        //    }

        //    //Insert the data
        //    KeyMethods.Write(start, key);
        //    ValueMethods.Write(start + KeySize, value);

        //    //save the header
        //    IncrementOneRecord(KeyValueSize);
        //    return true;
        //}

        protected override int GetIndexOf(TKey key)
        {
            fixed (byte* buffer = m_buffer1)
                SeekTo(key, buffer);
            if (m_currentIndex == RecordCount) //Beyond the end of the list
                return ~RecordCount;
            if (KeyMethods.IsEqual(m_currentKey, key))
                return m_currentIndex;
            return ~m_currentIndex;
        }

        protected override void Split(uint newNodeIndex, TKey dividingKey)
        {
            fixed (byte* buffer = m_buffer1)
            {
                ClearNodeCache();
                while (m_currentOffset < (BlockSize >> 1))
                {
                    Read(buffer);
                }

                int storageSize = EncodeRecord(buffer, m_nullKey, m_nullValue, m_currentKey, m_currentValue);

                //Determine how many entries to shift on the split.
                int recordsInTheFirstNode = m_currentIndex; // divide by 2.
                int recordsInTheSecondNode = RecordCount - m_currentIndex;
                long sourceStartingAddress = NodePosition + m_nextOffset;
                long targetStartingAddress = newNodeIndex * BlockSize + HeaderSize + storageSize;
                int bytesToMove = ValidBytes - m_nextOffset;

                //lookup the dividing key
                KeyMethods.Copy(m_currentKey, dividingKey);

                //do the copy
                Stream.Copy(sourceStartingAddress, targetStartingAddress, bytesToMove);

                Stream.Position = targetStartingAddress - storageSize;
                Stream.Write(buffer, storageSize);

                //Create the header of the second node.
                CreateNewNode(newNodeIndex, (ushort)recordsInTheSecondNode,
                              (ushort)(HeaderSize + bytesToMove + storageSize),
                              NodeIndex, RightSiblingNodeIndex, dividingKey, UpperKey);

                //update the node that was the old right sibling
                if (RightSiblingNodeIndex != uint.MaxValue)
                    SetLeftSiblingProperty(RightSiblingNodeIndex, NodeIndex, newNodeIndex);

                //update the origional header
                RecordCount = (ushort)recordsInTheFirstNode;
                ValidBytes = (ushort)(m_currentOffset);
                RightSiblingNodeIndex = newNodeIndex;
                UpperKey = dividingKey;

                ClearNodeCache();
            }
        }

        //unsafe void SplitNodeThenInsert(long firstNodeIndex)
        //{
        //    //do the copy
        //    StreamLeaf.Copy(sourceStartingAddress, targetStartingAddress, copyLength);

        //    StreamLeaf.Position = targetStartingAddress - storageSize;
        //    StreamLeaf.Write(buffer, storageSize);

        //    //update the node that was the old right sibling
        //    if (currentNode.RightSiblingNodeIndex != 0)
        //    {
        //        m_tempNode.SetCurrentNode(currentNode.RightSiblingNodeIndex);
        //        m_tempNode.LeftSiblingNodeIndex = secondNodeIndex;
        //        m_tempNode.Save();
        //    }

        //    //update the second header
        //    m_newNode.ValidBytes = copyLength + storageSize + NodeHeader.Size;
        //    m_newNode.LeftSiblingNodeIndex = firstNodeIndex;
        //    m_newNode.RightSiblingNodeIndex = currentNode.RightSiblingNodeIndex;
        //    m_newNode.Save();

        //    //update the first header
        //    currentNode.ValidBytes = (int)(m_insertScanner.PositionStartOfCurrent - firstNodeIndex * BlockSize);
        //    currentNode.RightSiblingNodeIndex = secondNodeIndex;
        //    currentNode.Save();

        //    NodeWasSplit(0, firstNodeIndex, CurrentKey.Key1, CurrentKey.Key2, secondNodeIndex);
        //    m_insertScanner.Reset();

        //    if (KeyToBeInserted > CurrentKey)
        //    {
        //        LeafNodeInsert(secondNodeIndex);
        //    }
        //    else
        //    {
        //        LeafNodeInsert(firstNodeIndex);
        //    }
        //}

        protected override void TransferRecordsFromRightToLeft(Node<TKey> left, Node<TKey> right, int bytesToTransfer)
        {
            throw new NotImplementedException();
            //int recordsToTransfer = (bytesToTransfer - HeaderSize) / KeyValueSize;
            ////Transfer records from Right to Left
            //long sourcePosition = right.NodePosition + HeaderSize;
            //long destinationPosition = left.NodePosition + HeaderSize + left.RecordCount * KeyValueSize;
            //Stream.Copy(sourcePosition, destinationPosition, KeyValueSize * recordsToTransfer);

            ////Removes empty spaces from records on the right.
            //Stream.Position = right.NodePosition + HeaderSize;
            //Stream.RemoveBytes(recordsToTransfer * KeyValueSize, (right.RecordCount - recordsToTransfer) * KeyValueSize);

            ////Update number of records.
            //left.RecordCount += (ushort)recordsToTransfer;
            //left.ValidBytes += (ushort)(recordsToTransfer * KeyValueSize);
            //right.RecordCount -= (ushort)recordsToTransfer;
            //right.ValidBytes -= (ushort)(recordsToTransfer * KeyValueSize);
        }

        protected override void TransferRecordsFromLeftToRight(Node<TKey> left, Node<TKey> right, int bytesToTransfer)
        {
            throw new NotImplementedException();
            //int recordsToTransfer = (bytesToTransfer - HeaderSize) / KeyValueSize;
            ////Shift existing records to make room for copy
            //Stream.Position = right.NodePosition + HeaderSize;
            //Stream.InsertBytes(recordsToTransfer * KeyValueSize, right.RecordCount * KeyValueSize);

            ////Transfer records from Left to Right
            //long sourcePosition = left.NodePosition + HeaderSize + (left.RecordCount - recordsToTransfer) * KeyValueSize;
            //long destinationPosition = right.NodePosition + HeaderSize;
            //Stream.Copy(sourcePosition, destinationPosition, KeyValueSize * recordsToTransfer);

            ////Update number of records.
            //left.RecordCount -= (ushort)recordsToTransfer;
            //left.ValidBytes -= (ushort)(recordsToTransfer * KeyValueSize);
            //right.RecordCount += (ushort)recordsToTransfer;
            //right.ValidBytes += (ushort)(recordsToTransfer * KeyValueSize);
        }

        #region [ Starter Code ]

        /// <summary>
        /// Continue to seek until the end of the list is found or 
        /// until the <see cref="m_currentKey"/> >= <see cref="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="buffer"></param>
        private void SeekTo(TKey key, byte* buffer)
        {
            //ToDo: Optimize this seek algorithm
            if (m_currentIndex == 0 && KeyMethods.IsLessThan(key, m_prevKey))
                return;
            if (m_currentIndex >= 0 && KeyMethods.IsLessThan(m_prevKey, key))
            {
                if (!KeyMethods.IsLessThan(m_currentKey, key) || m_currentIndex == RecordCount)
                {
                    return;
                }
                while (Read(buffer) && KeyMethods.IsLessThan(m_currentKey, key))
                    ;
            }
            else
            {
                ClearNodeCache();
                while (Read(buffer) && KeyMethods.IsLessThan(m_currentKey, key))
                    ;
            }
        }

        /// <summary>
        /// Continue to seek until the end of the list is found or 
        /// until the <see cref="m_currentKey"/> >= <see cref="key"/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="buffer"></param>
        private void SeekTo(int index, byte* buffer)
        {
            //Reset();
            //for (int x = 0; x <= index; x++)
            //    Read();
            if (m_currentIndex > index)
            {
                ClearNodeCache();
                for (int x = 0; x <= index; x++)
                    Read(buffer);
            }
            else
            {
                for (int x = m_currentIndex; x < index; x++)
                    Read(buffer);
            }
        }

        protected void OnNodeIndexChanged(object sender, EventArgs e)
        {
            ClearNodeCache();
        }

        protected void ClearNodeCache()
        {
            m_nextOffset = HeaderSize;
            m_currentOffset = HeaderSize;
            m_currentIndex = -1;
            KeyMethods.Clear(m_prevKey);
            ValueMethods.Clear(m_prevValue);
            KeyMethods.Clear(m_currentKey);
            ValueMethods.Clear(m_currentValue);
        }

        private bool Read(byte* buffer)
        {
            if (m_currentIndex == RecordCount)
            {
                throw new Exception("Read past the end of the stream");
            }

            KeyMethods.Copy(m_currentKey, m_prevKey);
            ValueMethods.Copy(m_currentValue, m_prevValue);
            m_currentOffset = m_nextOffset;
            m_currentIndex++;

            if (m_currentIndex == RecordCount)
                return false;

            m_nextOffset += DecodeRecord(GetReadPointer() + m_nextOffset, buffer, m_prevKey, m_prevValue, m_currentKey, m_currentValue);
            return true;
        }

        #endregion
    }
}