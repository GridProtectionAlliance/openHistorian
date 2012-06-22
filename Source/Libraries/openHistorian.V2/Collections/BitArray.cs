//******************************************************************************************************
//  BitArray.cs - Gbtc
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
//  3/20/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.V2.Collections
{
    /// <summary>
    /// Provides an array of bits.  Much like the native .NET implementation, 
    /// however this focuses on providing a free space bit array.
    /// </summary>
    public sealed class BitArray
    {
        #region [ Members ]

        int[] m_array;
        int m_count;
        int m_setCount;
        int m_lastFoundClearedIndex;
        bool m_initialState;

        #endregion

        #region [ Constructors ]
       
        /// <summary>
        /// Initializes <see cref="BitArray"/>.
        /// </summary>
        /// <param name="initialState">Set to true to initial will all elements set.  False to have all elements cleared.</param>
        public BitArray(bool initialState) :
            this(32, initialState)
        {
        }

        /// <summary>
        /// Initializes <see cref="BitArray"/>.
        /// </summary>
        /// <param name="count">The number of bit positions to support</param>
        /// <param name="initialState">Set to true to initial will all elements set.  False to have all elements cleared.</param>
        public BitArray(int count, bool initialState)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if ((count & 31) != 0)
                m_array = new int[(count >> 5) + 1];
            else
                m_array = new int[count >> 5];

            if (initialState)
            {
                m_setCount = count;
                for (int x = 0; x < m_array.Length; x++)
                {
                    m_array[x] = -1;
                }
            }
            else
            {
                m_setCount = 0;
            }
            m_count = count;
            m_initialState = initialState;
        }

        #endregion

        #region [ Properties ]

        public bool this[int index]
        {
            get
            {
                return GetBit(index);
            }
            set
            {
                if (value)
                    SetBit(index);
                else
                    ClearBit(index);
            }
        }

        /// <summary>
        /// Gets the number of items in the array.
        /// </summary>
        public int Count
        {
            get
            {
                return m_count;
            }
        }

        /// <summary>
        /// Gets the number of bits that are set in this array.
        /// </summary>
        public int SetCount
        {
            get
            {
                return m_setCount;
            }
        }

        /// <summary>
        /// Gets the number of bits that are cleared in this array.
        /// </summary>
        public int ClearCount
        {
            get
            {
                return m_count - m_setCount;
            }
        }
        
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the status of the corresponding bit.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>True if Set.  False if Cleared</returns>
        public bool GetBit(int index)
        {
            if (index < 0 || index >= m_count)
                throw new ArgumentOutOfRangeException("index");
            return (m_array[index >> 5] & (1 << (index & 31))) != 0;
        }

        /// <summary>
        /// Sets the corresponding bit to true
        /// </summary>
        /// <param name="index"></param>
        public void SetBit(int index)
        {
            if (index < 0 || index >= m_count)
                throw new ArgumentOutOfRangeException("index");
            int bit = 1 << (index & 31);
            if ((m_array[index >> 5] & bit) == 0) //if bit is cleared
            {
                m_setCount++;
                m_array[index >> 5] |= bit;
            }
        }

        /// <summary>
        /// Sets the corresponding bit to true. 
        /// Returns true if the bit state was changed.
        /// </summary>
        /// <param name="index"></param>
        /// <remarks>True if the bit state was changed. False if the bit was already set.</remarks>
        public bool TrySetBit(int index)
        {
            if (index < 0 || index >= m_count)
                throw new ArgumentOutOfRangeException("index");
            int bit = 1 << (index & 31);
            if ((m_array[index >> 5] & bit) == 0) //if bit is cleared
            {
                m_setCount++;
                m_array[index >> 5] |= bit;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the corresponding bit to false
        /// </summary>
        /// <param name="index"></param>
        public void ClearBit(int index)
        {
            if (index < 0 || index >= m_count)
                throw new ArgumentOutOfRangeException("index");
            int bit = 1 << (index & 31);
            if ((m_array[index >> 5] & bit) != 0) //if bit is set
            {
                m_lastFoundClearedIndex = 0;
                m_setCount--;
                m_array[index >> 5] &= ~bit;
            }
        }

        /// <summary>
        /// Sets the corresponding bit to false.
        /// Returns true if the bit state was changed.
        /// </summary>
        /// <param name="index"></param>
        /// <remarks>True if the bit state was changed. False if the bit was already cleared.</remarks>
        public bool TryClearBit(int index)
        {
            if (index < 0 || index >= m_count)
                throw new ArgumentOutOfRangeException("index");
            int bit = 1 << (index & 31);
            if ((m_array[index >> 5] & bit) != 0) //if bit is set
            {
                m_lastFoundClearedIndex = 0;
                m_setCount--;
                m_array[index >> 5] &= ~bit;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Increases the capacity of the bit array. Decreasing capacity is currently not supported
        /// </summary>
        /// <param name="capacity">the number of bits to support</param>
        /// <returns></returns>
        public void SetCapacity(int capacity)
        {
            int[] array;

            if (m_count >= capacity)
                return;
            if ((capacity & 31) != 0)
                array = new int[(capacity >> 5) + 1];
            else
                array = new int[capacity >> 5];

            m_array.CopyTo(array, 0);
            if (m_initialState)
            {
                m_setCount += capacity - m_count;
                for (int x = m_array.Length; x < array.Length; x++)
                {
                    array[x] = -1;
                }
            }
            m_array = array;
            m_count = capacity;
        }

        /// <summary>
        /// Returns the index of the first bit that is cleared. 
        /// -1 is returned if all bits are set.
        /// </summary>
        /// <returns></returns>
        public int FindClearedBit()
        {
            //parse each item, 32 bits at a time
            int count = m_array.Length;
            for (int x = m_lastFoundClearedIndex >> 5; x < count; x++)
            {
                //If the result is not -1, then use this element
                if (m_array[x] != -1)
                {
                    int position = HelperFunctions.FindFirstClearedBit(m_array[x]) + (x << 5); ;
                    m_lastFoundClearedIndex = position;
                    if (m_lastFoundClearedIndex >= m_count)
                        return -1;
                    return position;
                }
            }
            return -1;
        }

        #endregion
        
    }
}
