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
        where TKey : class, ISortedTreeValue<TKey>, new()
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
        protected int KeySize;

        protected TKey UpperKey = new TKey();
        protected TKey LowerKey = new TKey();
        private readonly Func<TKey, byte, uint> m_lookupKey;
        private readonly TKey m_tempKey;
        //private TKey m_lowerKey;
        //private TKey m_upperKey;
        protected SortedTreeTypeMethodsBase<TKey> KeyMethods;
        protected SortedTreeTypeMethodsBase<TValue> ValueMethods;

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
        protected readonly PointerVersionBox StreamPointer;

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
            KeyMethods = m_tempKey.CreateValueMethods();
            ValueMethods = new TValue().CreateValueMethods();
            KeySize = KeyMethods.Size;

            //OffsetOfUpperBounds = OffsetOfLowerBounds + KeySize;
            HeaderSize = OffsetOfLowerBounds + 2 * KeyMethods.Size;
            m_blockSize = blockSize;
            Stream = stream;
            StreamPointer = stream.PointerVersionBox;
            PointerVersion = -1;
            IndexOfNextKeyValue = 0;
            RecordCount = 0;
        }

        protected abstract void InternalPeek(TKey key, TValue value);

        protected abstract void InternalRead(TKey key, TValue value);

        protected abstract bool InternalRead(TKey key, TValue value, KeyMatchFilterBase<TKey> filter);

        protected abstract bool InternalReadWhile(TKey key, TValue value, TKey upperBounds);

        protected abstract bool InternalReadWhile(TKey key, TValue value, TKey upperBounds, KeyMatchFilterBase<TKey> filter);

        /// <summary>
        /// Using <see cref="Pointer"/> advance to the search location of the provided <see cref="key"/>
        /// </summary>
        /// <param name="key">the key to advance to</param>
        protected abstract void FindKey(TKey key);

        #region [ Peek ]

        /// <summary>
        /// Reads the next point, but doees not advance the position of the stream.
        /// </summary>
        /// <param name="key">the key to write the results to</param>
        /// <param name="value">the value to write the results to</param>
        /// <returns>
        /// True if a point is found. 
        /// False if the end of the stream has been encountered.
        /// </returns>
        public bool Peek(TKey key, TValue value)
        {
            if (StreamPointer.Version == PointerVersion)
            {
                //A light weight function that can be called quickly since 99% of the time, this logic statement will return successfully.
                if (IndexOfNextKeyValue < RecordCount)
                {
                    InternalPeek(key, value);
                    return true;
                }
            }
            return PeekCatchAll(key, value);
        }

        protected bool PeekCatchAll(TKey key, TValue value)
        {
            //If there are no more records in the current node.
            if (IndexOfNextKeyValue >= RecordCount)
            {
                //If the last leaf node, return false
                if (RightSiblingNodeIndex == uint.MaxValue)
                {
                    KeyMethods.Clear(key);
                    ValueMethods.Clear(value);
                    EOS = true;
                    return false;
                }
                LoadNode(RightSiblingNodeIndex);
            }
            //if the pointer data is no longer valid, refresh the pointer
            if (StreamPointer.Version != PointerVersion)
            {
                RefreshPointer();
            }
            //Reads the next key in the sequence.
            InternalPeek(key, value);
            return true;
        }

        #endregion

        #region [ ReadWhile ]

        /// <summary>
        /// Continues to advance the stream 
        /// but stops short of returning the point that is equal to
        /// the provided key.
        /// </summary>
        /// <param name="key">Where to store the key</param>
        /// <param name="value">Where to store the value</param>
        /// <param name="upperBounds">the test condition. Will return false if the returned point would have 
        /// exceeded this value</param>
        /// <returns>
        /// Returns true if the point returned is valid. 
        /// Returns false if:
        ///     The point read is greater than or equal to <see cref="upperBounds"/>.
        ///     The end of the stream is reached.
        ///     The end of the current node has been reached.
        /// </returns>
        public virtual bool ReadWhile(TKey key, TValue value, TKey upperBounds)
        {
            if (StreamPointer.Version == PointerVersion)
            {
                //A light weight function that can be called quickly since 99% of the time, this logic statement will return successfully.
                if (IndexOfNextKeyValue < RecordCount)
                {
                    if (KeyMethods.IsLessThan(UpperKey, upperBounds))
                    {
                        InternalRead(key, value);
                        return true;
                    }
                    return InternalReadWhile(key, value, upperBounds);
                }
            }
            return ReadWhileCatchAll(key, value, upperBounds);
        }

        protected bool ReadWhileCatchAll(TKey key, TValue value, TKey upperBounds)
        {
            //If there are no more records in the current node.
            if (IndexOfNextKeyValue >= RecordCount)
            {
                //If the last leaf node, return false
                if (RightSiblingNodeIndex == uint.MaxValue)
                {
                    KeyMethods.Clear(key);
                    ValueMethods.Clear(value);
                    EOS = true;
                    return false;
                }
                LoadNode(RightSiblingNodeIndex);
            }
            //if the pointer data is no longer valid, refresh the pointer
            if (StreamPointer.Version != PointerVersion)
            {
                RefreshPointer();
            }
            //Reads the next key in the sequence.
            if (KeyMethods.IsLessThan(UpperKey, upperBounds))
            {
                InternalRead(key, value);
                return true;
            }
            return InternalReadWhile(key, value, upperBounds);
        }


        /// <summary>
        /// Using the provided filter, continues to advance the stream 
        /// but stops short of returning the point that is equal to
        /// the provided key.
        /// </summary>
        /// <param name="key">Where to store the key</param>
        /// <param name="value">Where to store the value</param>
        /// <param name="upperBounds">the test condition. Will return false if the returned point would have 
        /// exceeded this value</param>
        /// <param name="filter">the filter to apply to the reading.</param>
        /// <returns>
        /// Returns true if the point returned is valid. 
        /// Returns false if:
        ///     The point read is greater than or equal to <see cref="upperBounds"/>.
        ///     The end of the stream is reached.
        ///     The end of the current node has been reached.
        /// </returns>
        public virtual bool ReadWhile(TKey key, TValue value, TKey upperBounds, KeyMatchFilterBase<TKey> filter)
        {
            if (StreamPointer.Version == PointerVersion && IndexOfNextKeyValue < RecordCount)
            {
                if (KeyMethods.IsLessThan(UpperKey, upperBounds))
                {
                    return InternalRead(key, value, filter);
                }
                return InternalReadWhile(key, value, upperBounds, filter);
            }
            return ReadWhileCatchAll(key, value, upperBounds, filter);
        }

        protected bool ReadWhileCatchAll(TKey key, TValue value, TKey upperBounds, KeyMatchFilterBase<TKey> filter)
        {
            //If there are no more records in the current node.
            if (IndexOfNextKeyValue >= RecordCount)
            {
                //If the last leaf node, return false
                if (RightSiblingNodeIndex == uint.MaxValue)
                {
                    KeyMethods.Clear(key);
                    ValueMethods.Clear(value);
                    EOS = true;
                    return false;
                }
                LoadNode(RightSiblingNodeIndex);
            }
            //if the pointer data is no longer valid, refresh the pointer
            if (StreamPointer.Version != PointerVersion)
            {
                RefreshPointer();
            }
            //Reads the next key in the sequence.
            if (KeyMethods.IsLessThan(UpperKey, upperBounds))
            {
                return InternalRead(key, value, filter);
            }
            return InternalReadWhile(key, value, upperBounds, filter);
        }

        #endregion

        #region [ Read ]

        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        public override bool Read(TKey key, TValue value)
        {
            if (StreamPointer.Version == PointerVersion)
            {
                //A light weight function that can be called quickly since 99% of the time, this logic statement will return successfully.
                if (IndexOfNextKeyValue < RecordCount)
                {
                    InternalRead(key, value);
                    return true;
                }
            }
            return ReadCatchAll(key, value);
        }

        /// <summary>
        /// A catch all read function. That can be called if overriding <see cref="Read"/> in a derived class.
        /// </summary>
        /// <returns></returns>
        protected bool ReadCatchAll(TKey key, TValue value)
        {
            //If there are no more records in the current node.
            if (IndexOfNextKeyValue >= RecordCount)
            {
                //If the last leaf node, return false
                if (RightSiblingNodeIndex == uint.MaxValue)
                {
                    KeyMethods.Clear(key);
                    ValueMethods.Clear(value);
                    EOS = true;
                    return false;
                }
                LoadNode(RightSiblingNodeIndex);
            }
            //if the pointer data is no longer valid, refresh the pointer
            if (StreamPointer.Version != PointerVersion)
            {
                RefreshPointer();
            }
            //Reads the next key in the sequence.
            InternalRead(key, value);
            return true;
        }

        #endregion

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
            FindKey(key);
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
            KeyMethods.Read(ptr + OffsetOfLowerBounds, LowerKey);
            KeyMethods.Read(ptr + OffsetOfUpperBounds, UpperKey);
            IndexOfNextKeyValue = 0;
            OnNoadReload();
        }

        /// <summary>
        /// Gets the pointer for the provided block.
        /// </summary>
        private void RefreshPointer()
        {
            Pointer = Stream.GetReadPointer(NodeIndex * m_blockSize, m_blockSize) + HeaderSize;
            PointerVersion = StreamPointer.Version;
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

    }
}