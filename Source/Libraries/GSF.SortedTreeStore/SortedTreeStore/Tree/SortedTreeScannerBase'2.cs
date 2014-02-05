//******************************************************************************************************
//  SortedTreeScannerBase`2.cs - Gbtc
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
//  3/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Filters;

namespace GSF.SortedTreeStore.Tree
{

    /// <summary>
    /// Base class for reading from any implementation of a sorted trees.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract unsafe class SortedTreeScannerBase<TKey, TValue>
        : SeekableTreeStream<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        private const int OffsetOfVersion = 0;
        private const int OffsetOfNodeLevel = OffsetOfVersion + sizeof(byte);
        private const int OffsetOfRecordCount = OffsetOfNodeLevel + sizeof(byte);
        private const int OffsetOfValidBytes = OffsetOfRecordCount + sizeof(ushort);
        private const int OffsetOfLeftSibling = OffsetOfValidBytes + sizeof(ushort);
        private const int OffsetOfRightSibling = OffsetOfLeftSibling + IndexSize;
        private const int OffsetOfLowerBounds = OffsetOfRightSibling + IndexSize;
        private const int IndexSize = sizeof(uint);

        private readonly Func<TKey, byte, uint> m_lookupKey;
        private readonly TKey m_tempKey;
        //private TKey m_lowerKey;
        //private TKey m_upperKey;
        protected SortedTreeKeyMethodsBase<TKey> KeyMethods;
        protected SortedTreeValueMethodsBase<TValue> ValueMethods;

        /// <summary>
        /// The index of the current node.
        /// </summary>
        protected uint NodeIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// The number of records in the current node.
        /// </summary>
        protected ushort RecordCount
        {
            get;
            private set;
        }

        /// <summary>
        /// The node index of the previous sibling.
        /// uint.MaxValue means there is no sibling to the right.
        /// </summary>
        protected uint LeftSiblingNodeIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// The node index of the next sibling. 
        /// uint.MaxValue means there is no sibling to the right.
        /// </summary>
        protected uint RightSiblingNodeIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// The pointer that is right after the header of the node.
        /// </summary>
        protected byte* Pointer { get; private set; }

        /// <summary>
        /// The pointer version of the <see cref="Pointer"/>. 
        /// Compare to Stream.PointerVersion to find out if 
        /// this pointer is current.
        /// </summary>
        protected long PointerVersion { get; private set; }

        private readonly byte m_version;
        private readonly byte m_level;
        private readonly int m_blockSize;
        protected readonly BinaryStreamBase Stream;

        /// <summary>
        /// The index number of the next key/value that needs to be read.
        /// The valid range of this field is [0, RecordCount - 1]
        /// </summary>
        protected int IndexOfNextKeyValue;

        /// <summary>
        /// The number of bytes in the header of any given node.
        /// </summary>
        protected int HeaderSize { get; private set; }
        //protected int OffsetOfUpperBounds;

        protected SortedTreeScannerBase(byte level, int blockSize, BinaryStreamBase stream, Func<TKey, byte, uint> lookupKey, byte version)
        {
            m_tempKey = new TKey();
            //m_lowerKey = new TKey();
            //m_upperKey = new TKey();
            m_version = version;
            m_lookupKey = lookupKey;
            m_level = level;

            //m_currentNode = new Node(stream, blockSize);
            KeyMethods = m_tempKey.CreateKeyMethods();
            ValueMethods = new TValue().CreateValueMethods();

            //OffsetOfUpperBounds = OffsetOfLowerBounds + KeySize;
            HeaderSize = OffsetOfLowerBounds + 2 * KeyMethods.Size;
            m_blockSize = blockSize;
            Stream = stream;
            PointerVersion = -1;
            IndexOfNextKeyValue = 0;
            RecordCount = 0;
        }

        /// <summary>
        /// Using <see cref="Pointer"/> advance to the next KeyValue
        /// </summary>
        protected abstract void ReadNext();

        /// <summary>
        /// Using <see cref="Pointer"/> advance to the next KeyValue that is contained in the provided filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <remarks> Be sure to modify <see cref="IndexOfNextKeyValue"/> and compare that to <see cref="RecordCount"/> 
        /// to determine if we are at the end of the stream.
        /// </remarks>
        protected abstract void ReadNext(KeyMatchFilterBase<TKey> filter);

        /// <summary>
        /// Using <see cref="Pointer"/> advance to the search location of the provided <see cref="key"/>
        /// </summary>
        /// <param name="key">the key to advance to</param>
        /// <returns>
        /// The index position of the key that was found that is greater than or equal to <see cref="key"/>
        /// </returns>
        protected abstract int FindKey(TKey key);

        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        public override bool Read()
        {
            if (Stream.PointerVersion == PointerVersion)
            {
                //A light weight function that can be called quickly since 99% of the time, this logic statement will return successfully.
                if (IndexOfNextKeyValue < RecordCount)
                {
                    ReadNext();
                    IndexOfNextKeyValue++;
                    IsValid = true;
                    return true;
                }
            }
            return Read2();
        }

        /// <summary>
        /// A catch all read function. That can be called if overriding <see cref="Read"/> in a derived class.
        /// </summary>
        /// <returns></returns>
        protected bool Read2()
        {
            //return false;
            //If there are no more records in the current node.
            if (IndexOfNextKeyValue >= RecordCount)
            {
                //If the last leaf node, return false
                if (RightSiblingNodeIndex == uint.MaxValue)
                {
                    KeyMethods.Clear(CurrentKey);
                    ValueMethods.Clear(CurrentValue);
                    IsValid = false;
                    return false;
                }
                LoadNode(RightSiblingNodeIndex);
            }
            //if the pointer data is no longer valid, refresh the pointer
            if (Stream.PointerVersion != PointerVersion)
            {
                RefreshPointer();
            }
            //Reads the next key in the sequence.
            ReadNext();
            IndexOfNextKeyValue++;
            IsValid = true;
            return true;
        }

        public override bool Read(KeyMatchFilterBase<TKey> filter)
        {
            if (Stream.PointerVersion == PointerVersion)
            {
                //A light weight function that can be called quickly since 99% of the time, this logic statement will return successfully.
                if (IndexOfNextKeyValue < RecordCount)
                {
                    ReadNext(filter);
                    if (IndexOfNextKeyValue <= RecordCount)
                    {
                        IsValid = true;
                        return true;
                    }
                }
            }
            return Read2(filter);
        }

        /// <summary>
        /// A catch all read function. That can be called if overriding <see cref="Read"/> in a derived class.
        /// </summary>
        /// <returns></returns>
        protected bool Read2(KeyMatchFilterBase<TKey> filter)
        {
        ReadAgain:

            //return false;
            //If there are no more records in the current node.
            if (IndexOfNextKeyValue >= RecordCount)
            {
                //If the last leaf node, return false
                if (RightSiblingNodeIndex == uint.MaxValue)
                {
                    KeyMethods.Clear(CurrentKey);
                    ValueMethods.Clear(CurrentValue);
                    IsValid = false;
                    return false;
                }
                LoadNode(RightSiblingNodeIndex);
            }
            //if the pointer data is no longer valid, refresh the pointer
            if (Stream.PointerVersion != PointerVersion)
            {
                RefreshPointer();
            }
            //Reads the next key in the sequence.
            ReadNext(filter);
            if (IndexOfNextKeyValue <= RecordCount)
            {
                IsValid = true;
                return true;
            }
            goto ReadAgain;
        }

        //public override bool Read(StreamFilterBase<TKey, TValue> filter)
        //{

        //    if (Stream.PointerVersion == PointerVersion)
        //    {
        //    ReadAgain:
        //        //A light weight function that can be called quickly since 99% of the time, this logic statement will return successfully.
        //        if (IndexOfNextKeyValue < RecordCount)
        //        {
        //            ReadNext();
        //            IndexOfNextKeyValue++;
        //            if (filter.ContinueReading(CurrentKey, CurrentValue))
        //                goto ReadAgain;

        //            IsValid = true;
        //            return true;
        //        }
        //    }
        //    return Read2(filter);
        //}

        ///// <summary>
        ///// A catch all read function. That can be called if overriding <see cref="Read"/> in a derived class.
        ///// </summary>
        ///// <returns></returns>
        //protected bool Read2(StreamFilterBase<TKey, TValue> filter)
        //{
        //ReadAgain:

        //    //return false;
        //    //If there are no more records in the current node.
        //    if (IndexOfNextKeyValue >= RecordCount)
        //    {
        //        //If the last leaf node, return false
        //        if (RightSiblingNodeIndex == uint.MaxValue)
        //        {
        //            KeyMethods.Clear(CurrentKey);
        //            ValueMethods.Clear(CurrentValue);
        //            IsValid = false;
        //            return false;
        //        }
        //        LoadNode(RightSiblingNodeIndex);
        //    }
        //    //if the pointer data is no longer valid, refresh the pointer
        //    if (Stream.PointerVersion != PointerVersion)
        //    {
        //        RefreshPointer();
        //    }
        //    //Reads the next key in the sequence.
        //    ReadNext();
        //    IndexOfNextKeyValue++;

        //    if (filter.ContinueReading(CurrentKey, CurrentValue))
        //        goto ReadAgain;

        //    IsValid = true;
        //    return true;
        //}



        /// <summary>
        /// Seeks to the start of SortedTree.
        /// </summary>
        public virtual void SeekToStart()
        {
            KeyMethods.SetMin(m_tempKey);
            SeekToKey(m_tempKey);
        }

        /// <summary>
        /// Seeks the stream to the first value greater than or equal to <see cref="key"/>
        /// </summary>
        /// <param name="key">the key to seek to.</param>
        public override void SeekToKey(TKey key)
        {
            LoadNode(FindLeafNodeAddress(key));
            IndexOfNextKeyValue = FindKey(key);
        }

        /// <summary>
        /// Loads the header data for the provided node.
        /// </summary>
        /// <param name="index">the node index</param>
        /// <exception cref="ArgumentNullException">occurs when <see cref="index"/>
        /// is equal to uint.MaxValue</exception>
        private void LoadNode(uint index)
        {
            if (index == uint.MaxValue)
                throw new ArgumentNullException("index", "Cannot be uint.MaxValue. Which is null.");
            NodeIndex = index;

            RefreshPointer();

            byte* ptr = Pointer - HeaderSize;
            if (ptr[OffsetOfVersion] != m_version)
                throw new Exception("Unknown node Version.");
            if (ptr[OffsetOfNodeLevel] != m_level)
                throw new Exception("This node is not supposed to access the underlying node level.");
            RecordCount = *(ushort*)(ptr + OffsetOfRecordCount);
            LeftSiblingNodeIndex = *(uint*)(ptr + OffsetOfLeftSibling);
            RightSiblingNodeIndex = *(uint*)(ptr + OffsetOfRightSibling);
            //KeyMethods.Read(ptr + OffsetOfLowerBounds, m_lowerKey);
            //KeyMethods.Read(ptr + OffsetOfUpperBounds, m_upperKey);
            IndexOfNextKeyValue = 0;
            OnNoadReload();
        }

        /// <summary>
        /// Gets the pointer for the provided block.
        /// </summary>
        private void RefreshPointer()
        {
            Pointer = Stream.GetReadPointer(NodeIndex * m_blockSize, m_blockSize) + HeaderSize;
            PointerVersion = Stream.PointerVersion;
        }

        /// <summary>
        /// Gets the block index when seeking for the provided key.
        /// </summary>
        /// <param name="key">the key to start the search from.</param>
        /// <returns></returns>
        protected uint FindLeafNodeAddress(TKey key)
        {
            return m_lookupKey(key, m_level);
        }

        /// <summary>
        /// Occurs when a node's data is reset.
        /// Derived classes can override this 
        /// method if fields need to be reset when a node is loaded.
        /// </summary>
        protected virtual void OnNoadReload()
        {
        }

        //public override void SetFilter(Filters.StreamFilterBase<TKey, TValue> filter)
        //{
        //    Filter = filter;
        //    //if (filter != null)
        //    //    filter.SetKeyValueReferences(CurrentKey, CurrentValue);
        //}
    }
}