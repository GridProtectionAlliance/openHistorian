//******************************************************************************************************
//  ContinuousBuffer.cs - Gbtc
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
//  1/26/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//*****************************************************************************************************

using System;
using System.Collections.Generic;

namespace openHistorian.V2
{
    /// <summary>
    /// Implements a circular buffer that can be absolutely indexed. This means that when items are added to this buffer, their index 
    /// will be unique and sequentially numbered.  This item can be retrieved calling the GetItem() function until
    /// it is removed from this buffer.
    /// This class behaves much like a circular buffer that can be indexed.
    /// </summary>
    /// <typeparam name="T">The type to make the elements.  Like byte[].</typeparam>
    class ContinuousBuffer<T>
    {
        /// <summary>
        /// Contains the array of objects
        /// </summary>
        T[] m_Items;

        /// <summary>
        /// Contains the head pointer of the circular buffer.
        /// </summary>
        int m_Head;
        /// <summary>
        /// Contains the tail of the circular buffer
        /// </summary>
        int m_Tail;
        /// <summary>
        /// The number of items in the buffer
        /// </summary>
        int m_Count;
        /// <summary>
        /// The first index that exists in the buffer.
        /// </summary>
        long m_FirstIndex;

        /// <summary>
        /// Gets the first index that exists in the buffer.  
        /// Trying to retrieve any index lower than this will throw an out of bounds exception.
        /// </summary>
        public long FirstIndex
        {
            get
            {
                return m_FirstIndex;
            }
        }

        /// <summary>
        /// The number of items that are currently in the buffer.
        /// </summary>
        public int Count
        {
            get
            {
                return m_Count;
            }
        }
        /// <summary>
        /// Represents the last item that can be indexed in this buffer.
        /// </summary>
        public long LastIndex
        {
            get
            {
                return m_Count + m_FirstIndex - 1;
            }
        }

        /// <summary>
        /// Adds an item buffer.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The index of the item</returns>
        public long Add(T item)
        {
            return -1;
        }

        /// <summary>
        /// Removes the earilest item from the buffer.
        /// </summary>
        /// <returns></returns>
        public T Remove()
        {
            return default(T);
        }
        public T GetItem(long Index)
        {
            return default(T);
        }

        public T this[long Index]
        {
            get
            {
                return GetItem(Index);
            }
        }
    }
}
