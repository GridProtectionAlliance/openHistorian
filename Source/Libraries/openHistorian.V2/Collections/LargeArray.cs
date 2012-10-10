//******************************************************************************************************
//  LargeArray.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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

namespace openHistorian.V2.Collections
{
    /// <summary>
    /// Since large arrays expand slowly, this class can quickly grow an array with millions of elements.
    /// It is highly advised that these objects are structs since keeping a list of millions of classes 
    /// will cause the garbage collection cycles to become very slow.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LargeArray<T> : ILargeArray<T>
    {
        bool m_disposed;
        int m_size;
        int m_bitShift;
        int m_mask;
        List<T[]> m_array;

        public LargeArray()
            : this(1024)
        {

        }

        public LargeArray(int jaggedArrayDepth)
        {
            m_size = (int)BitMath.RoundUpToNearestPowerOfTwo((uint)jaggedArrayDepth);
            m_mask = m_size - 1;
            m_bitShift = BitMath.CountBitsSet((uint)m_mask);
            m_array = new List<T[]>();
        }

        public T this[int index]
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName); 
                if (index >= Capacity || index < 0)
                    throw new ArgumentOutOfRangeException("index", "index is outside the bounds of the array");
                return m_array[index >> m_bitShift][index & m_mask];
            }
            set
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName); 
                if (index >= Capacity || index < 0)
                    throw new ArgumentOutOfRangeException("index", "index is outside the bounds of the array");
                m_array[index >> m_bitShift][index & m_mask] = value;
            }
        }

        /// <summary>
        /// Gets the number of items in the array.
        /// </summary>
        public int Capacity
        {
            get
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return m_size * m_array.Count;
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

            while (length > Capacity)
            {
                m_array.Add(new T[m_size]);
            }
            return Capacity;
        }

        /// <summary>
        /// Disposes resources. Note this does not call dispose on the elements of the array.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!m_disposed)
            {
                m_array = null;
                m_disposed = true;
            }
        }

    }
}
