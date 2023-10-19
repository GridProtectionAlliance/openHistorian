//******************************************************************************************************
//  FixedSizeNode`2.cs - Gbtc
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
//  04/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO.Unmanaged;
using GSF.Snap.Encoding;

namespace GSF.Snap.Tree
{
    /// <summary>
    /// A node for a <see cref="SortedTree"/> that is encoded in a fixed width. 
    /// This allows binary searches and faster writing.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public unsafe class FixedSizeNode<TKey, TValue>
        : SortedTreeNodeBase<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        private int m_maxRecordsPerNode;
        private readonly PairEncodingBase<TKey, TValue> m_encoding;

        /// <summary>
        /// Creates a new class
        /// </summary>
        /// <param name="level">the level of this node.</param>
        public FixedSizeNode(byte level)
            : base(level)
        {
            m_encoding = Library.Encodings.GetEncodingMethod<TKey, TValue>(EncodingDefinition.FixedSizeCombinedEncoding);
        }

        public override SortedTreeNodeBase<TKey, TValue> Clone(byte level)
        {
            return new FixedSizeNode<TKey, TValue>(level);
        }

        /// <summary>
        /// Returns a <see cref="SortedTreeScannerBase{TKey,TValue}"/>
        /// </summary>
        /// <returns></returns>
        public override SortedTreeScannerBase<TKey, TValue> CreateTreeScanner()
        {
            return new FixedSizeNodeScanner<TKey, TValue>(Level, BlockSize, Stream, SparseIndex.Get);
        }

        protected override void InitializeType()
        {
            m_maxRecordsPerNode = (BlockSize - HeaderSize) / KeyValueSize;

            if (m_maxRecordsPerNode < 4)
                throw new Exception("Tree must have at least 4 records per node. Increase the block size or decrease the size of the records.");
        }

        protected override int MaxOverheadWithCombineNodes => 0;

        protected override void Read(int index, TValue value)
        {
            value.Read(GetReadPointerAfterHeader() + KeySize + index * KeyValueSize);
        }

        protected override void Read(int index, TKey key, TValue value)
        {
            byte* ptr = GetReadPointerAfterHeader() + index * KeyValueSize;
            m_encoding.Decode(ptr, null, null, key, value, out _);
            //key.Read(ptr);
            //value.Read(ptr + KeySize);
        }

        protected override bool RemoveUnlessOverflow(int index)
        {
            if (index != RecordCount - 1)
            {
                byte* start = GetWritePointerAfterHeader() + index * KeyValueSize;
                Memory.Copy(start + KeyValueSize, start, (RecordCount - index - 1) * KeyValueSize);
            }

            //save the header
            RecordCount--;
            ValidBytes -= (ushort)KeyValueSize;
            return true;
        }

        protected override bool InsertUnlessFull(int index, TKey key, TValue value)
        {
            if (RecordCount >= m_maxRecordsPerNode)
                return false;

            byte* start = GetWritePointerAfterHeader() + index * KeyValueSize;

            if (index != RecordCount)
            {
                WinApi.MoveMemory(start + KeyValueSize, start, (RecordCount - index) * KeyValueSize);
            }

            //Insert the data

            m_encoding.Encode(start, null, null, key, value);
            //key.Write(start);
            //value.Write(start + KeySize);

            //save the header
            IncrementOneRecord(KeyValueSize);
            return true;
        }

        protected override void AppendSequentialStream(InsertStreamHelper<TKey, TValue> stream, out bool isFull)
        {
            //isFull = false;
            //return;

            //ToDo: Figure out why this code does not work.

            if (RecordCount >= m_maxRecordsPerNode)
            {
                isFull = true;
                return;
            }

            int recordsAdded = 0;
            int additionalValidBytes = 0;

            byte* writePointer = GetWritePointerAfterHeader();
            int offset = RecordCount * KeyValueSize;

        TryAgain:
            if (!stream.IsValid || !stream.IsStillSequential)
            {
                isFull = false;
                IncrementRecordCounts(recordsAdded, additionalValidBytes);
                return;
            }

            if (stream.IsKVP1)
            {
                //Key1,Value1 are the current record
                if (RemainingBytes - additionalValidBytes < KeyValueSize)
                {
                    isFull = true;
                    IncrementRecordCounts(recordsAdded, additionalValidBytes);
                    return;
                }

                stream.Key1.Write(writePointer + offset);
                stream.Value1.Write(writePointer + offset + KeySize);
                additionalValidBytes += KeyValueSize;
                recordsAdded++;
                offset += KeyValueSize;

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
                if (RemainingBytes - additionalValidBytes < KeyValueSize)
                {
                    isFull = true;
                    IncrementRecordCounts(recordsAdded, additionalValidBytes);
                    return;
                }

                stream.Key2.Write(writePointer + offset);
                stream.Value2.Write(writePointer + offset + KeySize);
                additionalValidBytes += KeyValueSize;
                recordsAdded++;
                offset += KeyValueSize;

                //Inlined stream.Next()
                stream.IsValid = stream.Stream.Read(stream.Key1, stream.Value1);
                stream.IsStillSequential = stream.Key2.IsLessThan(stream.Key1);
                stream.IsKVP1 = true;
                //End Inlined

                goto TryAgain;
            }
        }

        protected override int GetIndexOf(TKey key)
        {
            return KeyMethods.BinarySearch(GetReadPointerAfterHeader(), key, RecordCount, KeyValueSize);
        }

        protected override void Split(uint newNodeIndex, TKey dividingKey)
        {
            //Determine how many entries to shift on the split.
            int recordsInTheFirstNode = RecordCount >> 1; // divide by 2.
            int recordsInTheSecondNode = RecordCount - recordsInTheFirstNode;

            long sourceStartingAddress = StartOfDataPosition + KeyValueSize * recordsInTheFirstNode;
            long targetStartingAddress = newNodeIndex * BlockSize + HeaderSize;

            //lookup the dividing key
            dividingKey.Read(Stream.GetReadPointer(sourceStartingAddress, KeySize));

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, recordsInTheSecondNode * KeyValueSize);

            //Create the header of the second node.
            CreateNewNode(newNodeIndex, (ushort)recordsInTheSecondNode,
                          (ushort)(HeaderSize + recordsInTheSecondNode * KeyValueSize),
                          NodeIndex, RightSiblingNodeIndex, dividingKey, UpperKey);

            //update the node that was the old right sibling
            if (RightSiblingNodeIndex != uint.MaxValue)
                SetLeftSiblingProperty(RightSiblingNodeIndex, NodeIndex, newNodeIndex);

            //update the original header
            RecordCount = (ushort)recordsInTheFirstNode;
            ValidBytes = (ushort)(HeaderSize + recordsInTheFirstNode * KeyValueSize);
            RightSiblingNodeIndex = newNodeIndex;
            UpperKey = dividingKey;
        }

        protected override void TransferRecordsFromRightToLeft(Node<TKey> left, Node<TKey> right, int bytesToTransfer)
        {
            int recordsToTransfer = (bytesToTransfer - HeaderSize) / KeyValueSize;
            //Transfer records from Right to Left
            long sourcePosition = right.NodePosition + HeaderSize;
            long destinationPosition = left.NodePosition + HeaderSize + left.RecordCount * KeyValueSize;
            Stream.Copy(sourcePosition, destinationPosition, KeyValueSize * recordsToTransfer);

            //Removes empty spaces from records on the right.
            Stream.Position = right.NodePosition + HeaderSize;
            Stream.RemoveBytes(recordsToTransfer * KeyValueSize, (right.RecordCount - recordsToTransfer) * KeyValueSize);

            //Update number of records.
            left.RecordCount += (ushort)recordsToTransfer;
            left.ValidBytes += (ushort)(recordsToTransfer * KeyValueSize);
            right.RecordCount -= (ushort)recordsToTransfer;
            right.ValidBytes -= (ushort)(recordsToTransfer * KeyValueSize);
        }

        protected override void TransferRecordsFromLeftToRight(Node<TKey> left, Node<TKey> right, int bytesToTransfer)
        {
            int recordsToTransfer = (bytesToTransfer - HeaderSize) / KeyValueSize;
            //Shift existing records to make room for copy
            Stream.Position = right.NodePosition + HeaderSize;
            Stream.InsertBytes(recordsToTransfer * KeyValueSize, right.RecordCount * KeyValueSize);

            //Transfer records from Left to Right
            long sourcePosition = left.NodePosition + HeaderSize + (left.RecordCount - recordsToTransfer) * KeyValueSize;
            long destinationPosition = right.NodePosition + HeaderSize;
            Stream.Copy(sourcePosition, destinationPosition, KeyValueSize * recordsToTransfer);

            //Update number of records.
            left.RecordCount -= (ushort)recordsToTransfer;
            left.ValidBytes -= (ushort)(recordsToTransfer * KeyValueSize);
            right.RecordCount += (ushort)recordsToTransfer;
            right.ValidBytes += (ushort)(recordsToTransfer * KeyValueSize);
        }
    }
}