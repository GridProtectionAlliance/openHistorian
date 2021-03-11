//******************************************************************************************************
//  SortedPointBuffer`2.cs - Gbtc
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
//  02/05/2014 - Steven E. Chisholm
//       Generated original version of source code.
//  11/27/2014 - J. Ritchie Carroll
//       Added duplicated key handling for derived classes.
//     
//******************************************************************************************************

using System;
using GSF.Snap.Tree;

namespace GSF.Snap.Collection
{
    /// <summary>
    /// A temporary point buffer that is designed to write unsorted data to it, 
    /// then read the data back out sorted. 
    /// </summary>
    /// <typeparam name="TKey">The key type to use</typeparam>
    /// <typeparam name="TValue">The value type to use</typeparam>
    /// <remarks>
    /// This class is not thread safe. 
    /// </remarks>
    public class SortedPointBuffer<TKey, TValue>
        : TreeStream<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        private readonly KeyValueMethods<TKey, TValue> m_methods;

        /// <summary>
        /// Contains indexes of sorted data.
        /// </summary>
        private int[] m_sortingBlocks1;
        /// <summary>
        /// Contains indexes of sorted data.
        /// </summary>
        /// <remarks>
        /// Two blocks are needed to do a merge sort since 
        /// this class uses indexes instead of actually moving
        /// the raw values.
        /// </remarks>
        private int[] m_sortingBlocks2;

        /// <summary>
        /// A block of data for storing the keys.
        /// </summary>
        private TKey[] m_keyData;
        /// <summary>
        /// A block of data for storing the values.
        /// </summary>
        private TValue[] m_valueData;

        /// <summary>
        /// The maximum number of items that can be stored in this buffer.
        /// </summary>
        private int m_capacity;
        /// <summary>
        /// The index of the next point to dequeue.
        /// </summary>
        private int m_dequeueIndex;
        /// <summary>
        /// The index of the next point to write.
        /// </summary>
        private int m_enqueueIndex;
        /// <summary>
        /// Gets if the stream is currently reading. 
        /// The stream was not designed to be read from and written to at the same time. So the mode must be changed.
        /// </summary>
        private bool m_isReadingMode;

        private readonly bool m_removeDuplicates;

        private readonly Action<TKey, TKey> m_duplicateHandler;

        /// <summary>
        /// Creates a <see cref="SortedPointBuffer{TKey,TValue}"/> that can hold only exactly the specified <see cref="capacity"/>.
        /// </summary>
        /// <param name="capacity">The maximum number of items that can be stored in this class</param>
        /// <param name="removeDuplicates">specifies if the point buffer should remove duplicate key values upon reading.</param>
        public SortedPointBuffer(int capacity, bool removeDuplicates)
        {
            m_methods = Library.GetKeyValueMethods<TKey, TValue>();
            m_removeDuplicates = removeDuplicates;
            SetCapacity(capacity);
            m_isReadingMode = false;
        }

        /// <summary>
        /// Creates a <see cref="SortedPointBuffer{TKey,TValue}"/> that can hold only exactly the specified <see cref="capacity"/>
        /// using the specified duplicate handler.
        /// </summary>
        /// <param name="capacity">The maximum number of items that can be stored in this class</param>
        /// <param name="duplicateHandler">Function that will handle encountered duplicates.</param>
        protected SortedPointBuffer(int capacity, Action<TKey, TKey> duplicateHandler)
            : this(capacity, duplicateHandler is null)
        {
            m_duplicateHandler = duplicateHandler;
        }

        /// <summary>
        /// Gets if the stream will never return duplicate keys. Do not return true unless it is Guaranteed that 
        /// the data read from this stream will never contain duplicates.
        /// </summary>
        public override bool NeverContainsDuplicates => m_removeDuplicates || (object)m_duplicateHandler != null;

        /// <summary>
        /// Gets if the stream is always in sequential order. Do not return true unless it is Guaranteed that 
        /// the data read from this stream is sequential.
        /// </summary>
        public override bool IsAlwaysSequential => true;

        /// <summary>
        /// Gets the current number of items in the buffer
        /// </summary>
        public int Count => m_enqueueIndex - m_dequeueIndex;

        /// <summary>
        /// Gets if this buffer is empty
        /// </summary>
        public bool IsEmpty => m_dequeueIndex == m_enqueueIndex;

        /// <summary>
        /// Gets if no more items can be added to this list.
        /// List must be cleared before any more items can be added.
        /// </summary>
        public bool IsFull => m_capacity == m_enqueueIndex;

        /// <summary>
        /// Gets/Sets the current mode of the point buffer.
        /// </summary>
        /// <remarks>
        /// This class is not designed to be read from and written to at the same time.
        /// This is because sorting must occur right before reading from this stream.
        /// </remarks>
        public bool IsReadingMode
        {
            get => m_isReadingMode;
            set
            {
                if (m_isReadingMode != value)
                {
                    m_isReadingMode = value;
                    if (m_isReadingMode)
                    {
                        Sort();
                        if (m_removeDuplicates)
                            RemoveDuplicates();
                        else
                            HandleDuplicates();
                    }
                    else
                    {
                        Clear();
                    }
                }
            }
        }

        private void SetCapacity(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException("capacity", "must be greater than 0");

            m_capacity = capacity;
            m_sortingBlocks1 = new int[capacity];
            m_sortingBlocks2 = new int[capacity];
            m_keyData = new TKey[capacity];
            for (int x = 0; x < capacity; x++)
            {
                m_keyData[x] = new TKey();
            }

            m_valueData = new TValue[capacity];
            for (int x = 0; x < capacity; x++)
            {
                m_valueData[x] = new TValue();
            }
        }

        /// <summary>
        /// Clears all of the items in this list.
        /// </summary>
        private void Clear()
        {
            m_dequeueIndex = 0;
            m_enqueueIndex = 0;
            SetEos(false);
        }

        /// <summary>
        /// Attempts to enqueue the provided item to the list.
        /// </summary>
        /// <param name="key">the key to add</param>
        /// <param name="value">the value to add</param>
        /// <returns>true if the item was successfully enqueued. False if the queue is full.</returns>
        /// <exception cref="InvalidOperationException">Occurs if <see cref="IsReadingMode"/> is set to true</exception>
        public bool TryEnqueue(TKey key, TValue value)
        {
            if (m_isReadingMode)
                throw new InvalidOperationException("Cannot enqueue to a list that is in ReadMode");
            if (IsFull)
                return false;
            m_methods.Copy(key, value, m_keyData[m_enqueueIndex], m_valueData[m_enqueueIndex]);
            m_enqueueIndex++;
            return true;
        }

        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        /// <exception cref="InvalidOperationException">Occurs if <see cref="IsReadingMode"/> is set to false</exception>
        protected override bool ReadNext(TKey key, TValue value)
        {
            if (!m_isReadingMode)
                throw new InvalidOperationException("Cannot read from a list that is not in ReadMode");
            if (IsEmpty)
                return false;

            //Since this class is fixed in size. Bounds checks are not necessary as they will always be valid.
            int index = m_sortingBlocks1[m_dequeueIndex];
            m_methods.Copy(m_keyData[index], m_valueData[index], key, value);

            m_dequeueIndex++;
            return true;
        }

        /// <summary>
        /// Overrides the default behavior that disposes the stream when the end of the stream has been encountered.
        /// </summary>
        protected override void EndOfStreamReached()
        {
            SetEos(true);
        }

        /// <summary>
        /// Reads the specified item from the sorted list.
        /// </summary>
        /// <param name="index">the index of the item to read. Note: Bounds checking is not done.</param>
        /// <param name="key">the key to write to</param>
        /// <param name="value">the value to write to</param>
        internal void ReadSorted(int index, TKey key, TValue value)
        {
            if (!m_isReadingMode)
                throw new InvalidOperationException("Cannot read from a list that is not in ReadMode");
            //Since this class is fixed in size. Bounds checks are not necessary as they will always be valid.
            m_keyData[m_sortingBlocks1[index]].CopyTo(key);
            m_valueData[m_sortingBlocks1[index]].CopyTo(value);
        }

        /// <summary>
        /// Does a sort of the data. using a merge sort like algorithm.
        /// </summary>
        private unsafe void Sort()
        {
            //InitialSort
            int count = Count;

            for (int x = 0; x < count; x += 2)
            {
                //Can't sort the last entry if not
                if (x + 1 == count)
                {
                    m_sortingBlocks1[x] = x;
                }
                else if (m_keyData[x].IsLessThanOrEqualTo(m_keyData[x + 1]))
                {
                    m_sortingBlocks1[x] = x;
                    m_sortingBlocks1[x + 1] = x + 1;
                }
                else
                {
                    m_sortingBlocks1[x] = x + 1;
                    m_sortingBlocks1[x + 1] = x;
                }
            }

            bool shouldSwap = false;

            fixed (int* block1 = m_sortingBlocks1, block2 = m_sortingBlocks2)
            {
                int stride = 2;
                while (true)
                {
                    if (stride >= count)
                        break;

                    shouldSwap = true;
                    SortLevel(block1, block2, m_keyData, count, stride);
                    stride *= 2;

                    if (stride >= count)
                        break;

                    shouldSwap = false;
                    SortLevel(block2, block1, m_keyData, count, stride);
                    stride *= 2;
                }
            }

            if (shouldSwap)
            {
                int[] b1 = m_sortingBlocks1;
                m_sortingBlocks1 = m_sortingBlocks2;
                m_sortingBlocks2 = b1;
            }
        }

        /// <summary>
        /// Does a merge sort on the provided level.
        /// </summary>
        /// <param name="srcIndex">where the current indexes exist</param>
        /// <param name="dstIndex">where the final indexes should go</param>
        /// <param name="ptr">the data</param>
        /// <param name="count">the number of entries at this level</param>
        /// <param name="stride">the number of compares per level</param>
        private unsafe void SortLevel(int* srcIndex, int* dstIndex, TKey[] ptr, int count, int stride)
        {
            for (int xStart = 0; xStart < count; xStart += stride + stride)
            {
                int d = xStart;
                int dEnd = Math.Min(xStart + stride + stride, count);
                int i1 = xStart;
                int i1End = Math.Min(xStart + stride, count);
                int i2 = Math.Min(xStart + stride, count);
                int i2End = Math.Min(xStart + stride + stride, count);

                if (d != dEnd && i1 != i1End && i2 != i2End)
                {
                    //Check to see if already in order, then I can shortcut
                    if (ptr[srcIndex[i1End - 1]].IsLessThanOrEqualTo(ptr[srcIndex[i2]]))
                    {
                        for (int i = d; i < dEnd; i++)
                        {
                            dstIndex[i] = srcIndex[i];
                        }
                        continue;
                    }
                }

                while (d < dEnd)
                {
                    if (i1 == i1End)
                    {
                        dstIndex[d] = srcIndex[i2];
                        d++;
                        i2++;
                    }
                    else if (i2 == i2End)
                    {
                        dstIndex[d] = srcIndex[i1];
                        d++;
                        i1++;
                    }
                    else if (ptr[srcIndex[i1]].IsLessThanOrEqualTo(ptr[srcIndex[i2]]))
                    {
                        dstIndex[d] = srcIndex[i1];
                        d++;
                        i1++;
                    }
                    else
                    {
                        dstIndex[d] = srcIndex[i2];
                        d++;
                        i2++;
                    }
                }
            }
        }

        private void RemoveDuplicates()
        {
            int skipCount = 0;
            for (int x = 0; x < m_enqueueIndex - 1; x++)
            {
                if (skipCount > 0)
                    m_sortingBlocks1[x - skipCount] = m_sortingBlocks1[x];

                if (m_keyData[m_sortingBlocks1[x - skipCount]].IsEqualTo(m_keyData[m_sortingBlocks1[x + 1]]))
                {
                    skipCount++;
                }
            }

            m_enqueueIndex -= skipCount;
        }

        private void HandleDuplicates()
        {
            if (m_duplicateHandler is null)
                return;

            TKey left, right;
            bool keysManipulated = false;

            // Handle any encountered duplicates using derived class handler
            for (int x = 0; x < m_enqueueIndex - 1; x++)
            {
                left = m_keyData[m_sortingBlocks1[x]];
                right = m_keyData[m_sortingBlocks1[x + 1]];

                if (left.IsEqualTo(right))
                {
                    m_duplicateHandler(left, right);
                    keysManipulated = true;
                }
            }

            // Validate that duplicates were properly handled
            if (keysManipulated)
            {
                // Since derived class function manipulated keys to manage duplicates we have
                // to re-sort for safety - if keys were managed properly the tree should still
                // be sorted so this second sort will be fast
                Sort();

                // We cannot corrupt the tree so we remove any unhandled duplicates - if derived
                // class duplicate handler did its job, nothing will be removed
                RemoveDuplicates();
            }
        }
    }
}
