//******************************************************************************************************
//  NullableLargeArray.cs - Gbtc
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
//  9/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;

namespace GSF.Collections
{
    /// <summary>
    /// Provides a high speed list that can have elements that can be null.
    /// It would be similiar to a List&lt;Nullable&lt;T&gt;&gt;() except provide high speed lookup for
    /// NextIndexOfNull like functions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NullableLargeArray<T> : IDisposable
    {
        private bool m_disposed;
        private ILargeArray<T> m_list;
        private readonly BitArray m_isUsed;

        public NullableLargeArray()
            : this(new LargeArray<T>())
        {
        }

        public NullableLargeArray(ILargeArray<T> baseList)
        {
            m_list = baseList;
            m_isUsed = new BitArray(false, m_list.Capacity);
        }

        /// <summary>
        /// Returns if the object is not null.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool HasValue(int index)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (index >= Capacity || index < 0)
                throw new ArgumentOutOfRangeException("index", "index is outside the bounds of the array");
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
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Gets the number of items that can be stored in the array.
        /// </summary>
        public int Capacity
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                return m_list.Capacity;
            }
        }

        /// <summary>
        /// Gets the number of items that are in the array that are not null
        /// </summary>
        public int CountUsed
        {
            get
            {
                return m_isUsed.SetCount;
            }
        }

        /// <summary>
        /// Gets the number of available spaces in the array. Equal to <see cref="Capacity"/> - <see cref="CountUsed"/>.
        /// </summary>
        public int CountFree
        {
            get
            {
                return Capacity - CountUsed;
            }
        }

        public T this[int index]
        {
            get
            {
                return GetValue(index);
            }
        }

        /// <summary>
        /// Increases the capacity of the array to at least the given length. Will not reduce the size.
        /// </summary>
        /// <param name="length"></param>
        /// <returns>The current length of the list.</returns>
        public int SetCapacity(int length)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
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
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (index >= Capacity || index < 0)
                throw new ArgumentOutOfRangeException("index", "index is outside the bounds of the array");
            m_isUsed.ClearBit(index);
            m_list[index] = default(T);
        }

        /// <summary>
        /// Sets a value in the list.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetValue(int index, T value)
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (index >= Capacity || index < 0)
                throw new ArgumentOutOfRangeException("index", "index is outside the bounds of the array");

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
            for (int x = 0; x < m_isUsed.Count; x++)
            {
                if (HasValue(x))
                {
                    yield return m_list[x];
                }
            }
        }

        /// <summary>
        /// Returns and IEnumerable of a lookup. 
        /// Useful if parsing for an internal member or function of <see cref="T"/>.
        /// This prevents the nesting required if needing to return an IEnumberable of a sub type.
        /// </summary>
        /// <typeparam name="TP"></typeparam>
        /// <param name="func">A function that will convert <see cref="T"/> to <see cref="TP"/>.</param>
        /// <returns></returns>
        public IEnumerable<TP> GetEnumerator<TP>(Func<T, TP> func)
        {
            for (int x = 0; x < m_isUsed.Count; x++)
            {
                if (HasValue(x))
                {
                    yield return func(m_list[x]);
                }
            }
        }

        private int FindFirstEmptyIndex()
        {
            return m_isUsed.FindClearedBit();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                m_list.Dispose();
                m_list = null;
            }
        }
    }
}