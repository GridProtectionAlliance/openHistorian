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

namespace openHistorian.V2.Unmanaged
{
    /// <summary>
    /// Provides an array of bits.  Much like the native .NET implementation, 
    /// however this focuses on providing a free space bit array.
    /// </summary>
    public sealed class BitArray
    {
        int[] m_array;
        int m_count;
        int m_setCount;
        int m_lastFoundClearedIndex;

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
        }

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

        public int ClearCount
        {
            get
            {
                return m_count - m_setCount;
            }
        }

        /// <summary>
        /// Returns the index of the first bit that is cleared. 
        /// -1 is returned if all bits are set.
        /// </summary>
        /// <returns></returns>
        public int FindClearedBit()
        {
            if (SetCount == 32)
                m_setCount = SetCount;
            //parse each item, 32 bits at a time
            int count = m_array.Length;
            for (int x = m_lastFoundClearedIndex >> 5; x < count; x++)
            {

                //Method based on a method found at: http://graphics.stanford.edu/~seander/bithacks.htm
                //Subtitle: Count the consecutive zero bits (trailing) on the right by binary search 
                //If the result is not -1, then use this element
                if (m_array[x] != -1)
                {
                    int value = m_array[x];
                    int position = 0;
                    if ((value & 0xffff) == 0xffff)
                    {
                        value >>= 16;
                        position += 16;
                    }
                    if ((value & 0xff) == 0xff)
                    {
                        value >>= 8;
                        position += 8;
                    }
                    if ((value & 0xf) == 0xf)
                    {
                        value >>= 4;
                        position += 4;
                    }
                    if ((value & 0x3) == 0x3)
                    {
                        value >>= 2;
                        position += 2;
                    }
                    position = position + (value & 0x1) + (x << 5);

                    m_lastFoundClearedIndex = position;
                    if (m_lastFoundClearedIndex >= m_count)
                        return -1;
                    return position;

                    ////parse each bit in that byte
                    ////ToDo: There is room for optimizations here.
                    //for (int y = x << 5; y < m_count; y++)
                    //{
                    //    if (!GetBit(y))
                    //    {
                    //        if (y != c)
                    //            throw new Exception();
                    //        m_lastFoundClearedIndex = y;
                    //        return y;
                    //    }
                    //}
                }
            }
            return -1;
        }
    }
}
