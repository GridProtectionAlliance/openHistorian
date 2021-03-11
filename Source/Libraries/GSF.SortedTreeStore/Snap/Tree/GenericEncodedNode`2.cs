//******************************************************************************************************
//  GenericEncodedNode`2.cs - Gbtc
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
//  05/07/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.Snap.Encoding;

namespace GSF.Snap.Tree
{
    /// <summary>
    /// A TreeNode abstract class that is used for linearly encoding a class.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public unsafe class GenericEncodedNode<TKey, TValue>
        : SortedTreeNodeBase<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        private readonly PairEncodingBase<TKey, TValue> m_encoding;
        private int m_maximumStorageSize;
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

        public GenericEncodedNode(PairEncodingBase<TKey, TValue> encoding, byte level)
            : base(level)
        {
            if (level != 0)
                throw new ArgumentException("Level for this type is only supported on the leaf level.");
            m_currentKey = new TKey();
            m_currentValue = new TValue();
            m_prevKey = new TKey();
            m_prevValue = new TValue();
            m_nullKey = new TKey();
            m_nullValue = new TValue();
            m_nullKey.Clear();
            m_nullValue.Clear();

            NodeIndexChanged += OnNodeIndexChanged;
            ClearNodeCache();

            m_encoding = encoding;

        }

        public override SortedTreeNodeBase<TKey, TValue> Clone(byte level)
        {
            return new GenericEncodedNode<TKey, TValue>(m_encoding.Clone(), level);
        }

        public override SortedTreeScannerBase<TKey, TValue> CreateTreeScanner()
        {
            return new GenericEncodedNodeScanner<TKey, TValue>(m_encoding, Level, BlockSize, Stream, SparseIndex.Get);
        }

        protected override int MaxOverheadWithCombineNodes => MaximumStorageSize * 2 + 1;

        protected int EncodeRecord(byte* stream, TKey prevKey, TValue prevValue, TKey currentKey, TValue currentValue)
        {
            return m_encoding.Encode(stream, prevKey, prevValue, currentKey, currentValue);
        }

        protected int DecodeRecord(byte* stream, TKey prevKey, TValue prevValue, TKey currentKey, TValue currentValue)
        {
            return m_encoding.Decode(stream, prevKey, prevValue, currentKey, currentValue, out _);
        }

        protected int MaximumStorageSize => m_encoding.MaxCompressionSize;

        protected override void InitializeType()
        {
            m_maximumStorageSize = MaximumStorageSize;
            m_buffer1 = new byte[MaximumStorageSize];
            m_buffer2 = new byte[MaximumStorageSize];
            if ((BlockSize - HeaderSize) / MaximumStorageSize < 4)
                throw new Exception("Tree must have at least 4 records per node. Increase the block size or decrease the size of the records.");
        }

        protected override void Read(int index, TValue value)
        {
            if (index == RecordCount)
                throw new Exception();
            SeekTo(index);
            m_currentValue.CopyTo(value);
        }

        protected override void Read(int index, TKey key, TValue value)
        {
            if (index == RecordCount)
                throw new Exception();
            SeekTo(index);
            m_currentKey.CopyTo(key);
            m_currentValue.CopyTo(value);
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
        /// Requests that the current stream is inserted into the tree. Sequential insertion can only occur while the stream
        /// is in order and is entirely past the end of the tree. 
        /// </summary>
        /// <param name="stream">the stream data to insert</param>
        /// <param name="isFull">if returning from this function while the node is not yet full, this means the stream 
        /// can no longer be inserted sequentially and we must break out to the root and insert one at a time.</param>
        protected override void AppendSequentialStream(InsertStreamHelper<TKey, TValue> stream, out bool isFull)
        {
            int recordsAdded = 0;
            int additionalValidBytes = 0;
            byte* writePointer = GetWritePointer();

            fixed (byte* buffer = m_buffer1)
            {
                SeekTo(RecordCount);

                if (RecordCount > 0)
                {
                    m_currentKey.CopyTo(stream.PrevKey);
                    m_currentValue.CopyTo(stream.PrevValue);
                }
                else
                {
                    stream.PrevKey.Clear();
                    stream.PrevValue.Clear();
                }

                TryAgain:

                if (!stream.IsValid || !stream.IsStillSequential)
                {
                    isFull = false;
                    IncrementRecordCounts(recordsAdded, additionalValidBytes);
                    ClearNodeCache();
                    return;
                }

                int length;
                if (stream.IsKVP1)
                {
                    //Key1,Value1 are the current record
                    if (RemainingBytes - additionalValidBytes < m_maximumStorageSize)
                    {
                        length = EncodeRecord(buffer, stream.Key2, stream.Value2, stream.Key1, stream.Value1);
                        if (RemainingBytes - additionalValidBytes < length)
                        {
                            isFull = true;
                            IncrementRecordCounts(recordsAdded, additionalValidBytes);
                            ClearNodeCache();
                            return;
                        }
                    }

                    length = EncodeRecord(writePointer + m_nextOffset, stream.Key2, stream.Value2, stream.Key1, stream.Value1);
                    additionalValidBytes += length;
                    recordsAdded++;
                    m_currentOffset = m_nextOffset;
                    m_nextOffset = m_currentOffset + length;

                    //Inlined stream.Next()
                    stream.IsValid = stream.Stream.Read(stream.Key2, stream.Value2);
                    stream.IsStillSequential = stream.Key1.IsLessThan(stream.Key2);
                    stream.IsKVP1 = false;
                    //End Inlined
                    goto TryAgain;
                }
                else
                {
                    //Key2,Value2 are the current record
                    if (RemainingBytes - additionalValidBytes < m_maximumStorageSize)
                    {
                        length = EncodeRecord(buffer, stream.Key1, stream.Value1, stream.Key2, stream.Value2);
                        if (RemainingBytes - additionalValidBytes < length)
                        {
                            isFull = true;
                            IncrementRecordCounts(recordsAdded, additionalValidBytes);
                            ClearNodeCache();
                            return;
                        }
                    }

                    length = EncodeRecord(writePointer + m_nextOffset, stream.Key1, stream.Value1, stream.Key2, stream.Value2);
                    additionalValidBytes += length;
                    recordsAdded++;
                    m_currentOffset = m_nextOffset;
                    m_nextOffset = m_currentOffset + length;

                    //Inlined stream.Next()
                    stream.IsValid = stream.Stream.Read(stream.Key1, stream.Value1);
                    stream.IsStillSequential = stream.Key2.IsLessThan(stream.Key1);
                    stream.IsKVP1 = true;
                    //End Inlined

                    goto TryAgain;
                }
            }
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
            if (index == RecordCount)
            {
                //Insert After
                SeekTo(index);
                return InsertAfter(key, value);
            }
            else
            {
                //Insert Between
                SeekTo(index);
                return InsertBetween(key, value);
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

                key.CopyTo(m_currentKey);
                value.CopyTo(m_currentValue);
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

                shiftDelta -= m_nextOffset - m_currentOffset;

                if (RemainingBytes < shiftDelta)
                    return false;

                Stream.Position = NodePosition + m_currentOffset;
                if (shiftDelta < 0)
                    Stream.RemoveBytes(-shiftDelta, ValidBytes - m_currentOffset);
                else
                    Stream.InsertBytes(shiftDelta, ValidBytes - m_currentOffset);

                Stream.Write(buffer, shiftDelta1);
                Stream.Write(buffer2, shiftDelta2);

                IncrementOneRecord(shiftDelta);

                key.CopyTo(m_currentKey);
                value.CopyTo(m_currentValue);
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
            if (m_currentKey.IsEqualTo(key))
                return m_currentIndex;
            return ~m_currentIndex;
        }

        protected override void Split(uint newNodeIndex, TKey dividingKey)
        {
            fixed (byte* buffer = m_buffer1)
            {
                ClearNodeCache();
                while (m_currentOffset < BlockSize >> 1)
                {
                    Read();
                }

                int storageSize = EncodeRecord(buffer, m_nullKey, m_nullValue, m_currentKey, m_currentValue);

                //Determine how many entries to shift on the split.
                int recordsInTheFirstNode = m_currentIndex; // divide by 2.
                int recordsInTheSecondNode = RecordCount - m_currentIndex;
                long sourceStartingAddress = NodePosition + m_nextOffset;
                long targetStartingAddress = newNodeIndex * BlockSize + HeaderSize + storageSize;
                int bytesToMove = ValidBytes - m_nextOffset;

                //lookup the dividing key
                m_currentKey.CopyTo(dividingKey);

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

                //update the original header
                RecordCount = (ushort)recordsInTheFirstNode;
                ValidBytes = (ushort)m_currentOffset;
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
            if (m_currentIndex == 0 && key.IsLessThan(m_prevKey))
                return;

            if (m_currentIndex >= 0 && m_prevKey.IsLessThan(key))
            {
                if (!m_currentKey.IsLessThan(key) || m_currentIndex == RecordCount)
                {
                    return;
                }
                
                while (Read() && m_currentKey.IsLessThan(key))
                {
                }
            }
            else
            {
                ClearNodeCache();
                
                while (Read() && m_currentKey.IsLessThan(key))
                {
                }
            }
        }

        /// <summary>
        /// Continue to seek until the end of the list is found or 
        /// until the <see cref="m_currentKey"/> >= <see cref="key"/>
        /// </summary>
        /// <param name="index"></param>
        private void SeekTo(int index)
        {
            //Reset();
            //for (int x = 0; x <= index; x++)
            //    Read();
            if (m_currentIndex > index)
            {
                ClearNodeCache();

                for (int x = 0; x <= index; x++)
                    Read();
            }
            else
            {
                for (int x = m_currentIndex; x < index; x++)
                    Read();
            }
        }

        private void OnNodeIndexChanged(object sender, EventArgs e)
        {
            ClearNodeCache();
        }

        private void ClearNodeCache()
        {
            m_nextOffset = HeaderSize;
            m_currentOffset = HeaderSize;
            m_currentIndex = -1;
            m_prevKey.Clear();
            m_prevValue.Clear();
            m_currentKey.Clear();
            m_currentValue.Clear();
        }

        private bool Read()
        {
            if (m_currentIndex == RecordCount)
            {
                throw new Exception("Read past the end of the stream");
            }

            m_currentKey.CopyTo(m_prevKey);
            m_currentValue.CopyTo(m_prevValue);
            m_currentOffset = m_nextOffset;
            m_currentIndex++;

            if (m_currentIndex == RecordCount)
                return false;

            m_nextOffset += DecodeRecord(GetReadPointer() + m_nextOffset, m_prevKey, m_prevValue, m_currentKey, m_currentValue);
            return true;
        }

    #endregion
    }
}