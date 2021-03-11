//******************************************************************************************************
//  NullableLargeArray.cs - Gbtc
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
//  9/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace GSF.Collections
{
    /// <summary>
    /// Provides a high speed list that can have elements that can be null.
    /// It would be similiar to a List&lt;Nullable&lt;T&gt;&gt;() except provide high speed lookup for
    /// NextIndexOfNull like functions.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public class NullableLargeArray<T> : IEnumerable<T>
    {
        private readonly LargeArray<T> m_list;
        private readonly BitArray m_isUsed;

        /// <summary>
        /// Creates a <see cref="NullableLargeArray{T}"/>
        /// </summary>
        public NullableLargeArray()
        {
            m_list = new LargeArray<T>();
            m_isUsed = new BitArray(false, m_list.Capacity);
        }

        /// <summary>
        /// Returns if the object is not null.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool HasValue(int index)
        {
            //Bounds checking is done in BitArray
            return m_isUsed[index];
        }

        /// <summary>
        /// Tries to get the following value for the list.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns>True if the item exists. False if null.</returns>
        public bool TryGetValue(int index, out T value)
        {
            if (HasValue(index))
            {
                value = m_list[index];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Gets the number of items that can be stored in the array.
        /// </summary>
        public int Capacity => m_list.Capacity;

        /// <summary>
        /// Gets the number of items that are in the array that are not null
        /// </summary>
        public int CountUsed => m_isUsed.SetCount;

        /// <summary>
        /// Gets the number of available spaces in the array. Equal to <see cref="Capacity"/> - <see cref="CountUsed"/>.
        /// </summary>
        public int CountFree => m_isUsed.ClearCount;

        /// <summary>
        /// Gets the provided item from the array. 
        /// </summary>
        /// <param name="index">the index of the item</param>
        /// <returns>The item.</returns>
        public T this[int index]
        {
            get => GetValue(index);
            set => SetValue(index, value);
        }

        /// <summary>
        /// Increases the capacity of the array to at least the given length. Will not reduce the size.
        /// </summary>
        /// <param name="length"></param>
        /// <returns>The current length of the list.</returns>
        public int SetCapacity(int length)
        {
            if (length > Capacity)
            {
                m_list.SetCapacity(length);
                m_isUsed.SetCapacity(m_list.Capacity);
            }
            return Capacity;
        }

        /// <summary>
        /// Gets the specified item from the list.  Throws an exception if the item is null.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetValue(int index)
        {
            if (!HasValue(index))
                throw new NullReferenceException();
            return m_list[index];
        }

        /// <summary>
        /// Sets the following item to null.
        /// </summary>
        /// <param name="index"></param>
        public void SetNull(int index)
        {
            m_isUsed.ClearBit(index);
            m_list[index] = default;
        }

        /// <summary>
        /// Sets a value in the list.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetValue(int index, T value)
        {
            m_isUsed.SetBit(index);
            m_list[index] = value;
        }

        /// <summary>
        /// Replaces and existing value in the list. Throws an exception if the existing item is null.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void OverwriteValue(int index, T value)
        {
            if (!HasValue(index))
                throw new IndexOutOfRangeException("index does not exist");
            m_list[index] = value;
        }

        /// <summary>
        /// Adds a new value to the list and locates it at the nearest possible empty location.
        /// If there is not enough room, the list is automatically expanded.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>the index where the value was placed.</returns>
        public int AddValue(T value)
        {
            int index = FindFirstEmptyIndex();
            if (index < 0)
            {
                SetCapacity(Capacity + 1);
                index = FindFirstEmptyIndex();
            }
            SetValue(index, value);
            return index;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the non-null elements of this collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (int index in m_isUsed.GetAllSetBits())
            {
                yield return m_list[index];
            }
        }

        private int FindFirstEmptyIndex()
        {
            return m_isUsed.FindClearedBit();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Clears all elements in the list
        /// </summary>
        public void Clear()
        {
            m_list.Clear();
            m_isUsed.ClearAll();
        }
    }
}