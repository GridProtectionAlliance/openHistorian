//******************************************************************************************************
//  BitArray.cs - Gbtc
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
//  3/20/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GSF.Collections
{

    /// <summary>
    /// Provides an array of bits.  Much like the native .NET implementation, 
    /// however this focuses on providing a free space bit array.
    /// </summary>
    public sealed class BitArray
    {
        #region [ Members ]

        /// <summary>
        /// The number of bits to shift to get the index of the array
        /// </summary>
        public const int BitsPerElementShift = 5;
        /// <summary>
        /// The mask to apply to get the bit position of the value
        /// </summary>
        public const int BitsPerElementMask = BitsPerElement - 1;
        /// <summary>
        /// The number of bits per array element.
        /// </summary>
        public const int BitsPerElement = sizeof(int) * 8;

        private int[] m_array;
        private int m_count;
        private int m_setCount;
        private int m_lastFoundClearedIndex;
        private int m_lastFoundSetIndex;
        private readonly bool m_initialState;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes <see cref="BitArray"/>.
        /// </summary>
        /// <param name="initialState">Set to true to initial will all elements set.  False to have all elements cleared.</param>
        /// <param name="count">The number of bit positions to support</param>
        public BitArray(bool initialState, int count = BitsPerElement)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            //If the number does not lie on a 32 bit boundary, add 1 to the number of items in the array.
            if ((count & BitsPerElementMask) != 0)
                m_array = new int[(count >> BitsPerElementShift) + 1];
            else
                m_array = new int[count >> BitsPerElementShift];


            if (initialState)
            {
                m_setCount = count;
                for (int x = 0; x < m_array.Length; x++)
                {
                    m_array[x] = -1; // (-1 is all bits set)
                }
            }
            else
            {
                //.NET initializes all memory with zeroes.
                m_setCount = 0;
            }
            m_count = count;
            m_initialState = initialState;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets/Sets individual bits in this array.
        /// </summary>
        /// <param name="index">the bit position to get.</param>
        /// <returns></returns>
        public bool this[int index]
        {
            get => GetBit(index);
            set
            {
                if (value)
                    SetBit(index);
                else
                    ClearBit(index);
            }
        }

        /// <summary>
        /// Gets the number of bits this array contains.
        /// </summary>
        public int Count => m_count;

        /// <summary>
        /// Gets the number of bits that are set in this array.
        /// </summary>
        public int SetCount => m_setCount;

        /// <summary>
        /// Gets the number of bits that are cleared in this array.
        /// </summary>
        public int ClearCount => m_count - m_setCount;

    #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the status of the corresponding bit.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>True if Set.  False if Cleared</returns>
        public bool GetBit(int index)
        {
            Validate(index);
            return (m_array[index >> BitsPerElementShift] & (1 << (index & BitsPerElementMask))) != 0;
        }

        /// <summary>
        /// Gets the status of the corresponding bit.
        /// This method does not validate the bounds of the array, 
        /// and will be Aggressively Inlined.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>True if Set.  False if Cleared</returns>
        /// <remarks>
        /// The exact speed varies, but has been shown to be anywhere from 1 to 6 times faster. 
        /// (All smaller than a few nanoseconds. But in an inner loop, this can be a decent improvement.)
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetBitUnchecked(int index)
        {
            return (m_array[index >> BitsPerElementShift] & (1 << (index & BitsPerElementMask))) != 0;
        }

        /// <summary>
        /// Sets the corresponding bit to true
        /// </summary>
        /// <param name="index"></param>
        public void SetBit(int index)
        {
            TrySetBit(index);
        }

        /// <summary>
        /// Sets the corresponding bit to true. 
        /// Returns true if the bit state was changed.
        /// </summary>
        /// <param name="index"></param>
        /// <remarks>True if the bit state was changed. False if the bit was already set.</remarks>
        public bool TrySetBit(int index)
        {
            Validate(index);
            int subBit = 1 << (index & BitsPerElementMask);
            int element = index >> BitsPerElementShift;
            int value = m_array[element];
            if ((value & subBit) == 0) //if bit is set
            {
                m_lastFoundSetIndex = 0;
                m_setCount++;
                m_array[element] = value | subBit;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all bits.
        /// </summary>
        public void ClearAll()
        {
            m_setCount = 0;
            Array.Clear(m_array, 0, m_array.Length);
        }

        

        /// <summary>
        /// Sets the corresponding bit to false
        /// </summary>
        /// <param name="index"></param>
        public void ClearBit(int index)
        {
            TryClearBit(index);
        }

        /// <summary>
        /// Sets the corresponding bit to false.
        /// Returns true if the bit state was changed.
        /// </summary>
        /// <param name="index"></param>
        /// <remarks>True if the bit state was changed. False if the bit was already cleared.</remarks>
        public bool TryClearBit(int index)
        {
            Validate(index);
            int subBit = 1 << (index & BitsPerElementMask);
            int element = index >> BitsPerElementShift;
            int value = m_array[element];
            if ((value & subBit) != 0) //if bit is set
            {
                m_lastFoundClearedIndex = 0;
                m_setCount--;
                m_array[element] = value & ~subBit;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears a series of bits
        /// </summary>
        /// <param name="index">the starting index to clear</param>
        /// <param name="length">the length of bits</param>
        public void ClearBits(int index, int length)
        {
            Validate(index, length);
            for (int x = index; x < index + length; x++)
            {
                ClearBit(x);
            }
        }

        /// <summary>
        /// Sets all bits.
        /// </summary>
        public void SetAll()
        {
            m_setCount = m_count;
            for (int x = 0; x < m_array.Length; x++)
            {
                m_array[x] = -1; // (-1 is all bits set)
            }
        }

        /// <summary>
        /// Sets a series of bits
        /// </summary>
        /// <param name="index">the starting index to clear</param>
        /// <param name="length">the length of bits</param>
        public void SetBits(int index, int length)
        {
            Validate(index, length);
            for (int x = index; x < index + length; x++)
            {
                SetBit(x);
            }
        }
        /// <summary>
        /// Determines if any of the provided bits are set.
        /// </summary>
        /// <param name="index">the starting index</param>
        /// <param name="length">the length of the run</param>
        /// <returns></returns>
        public bool AreAllBitsSet(int index, int length)
        {
            Validate(index, length);
            for (int x = index; x < index + length; x++)
            {
                if ((m_array[x >> BitsPerElementShift] & (1 << (x & BitsPerElementMask))) == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if any of the provided bits are cleared.
        /// </summary>
        /// <param name="index">the starting index</param>
        /// <param name="length">the length of the run</param>
        /// <returns></returns>
        public bool AreAllBitsCleared(int index, int length)
        {
            Validate(index, length);
            for (int x = index; x < index + length; x++)
            {
                if ((m_array[x >> BitsPerElementShift] & (1 << (x & BitsPerElementMask))) != 0)
                    return false;
            }
            return true;
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

            //If the number does not lie on a 32 bit boundary, add 1 to the number of items in the array.
            if ((capacity & BitsPerElementMask) != 0)
                array = new int[(capacity >> BitsPerElementShift) + 1];
            else
                array = new int[capacity >> BitsPerElementShift];

            m_array.CopyTo(array, 0);

            //If initial state is to set all of the bits, set them.
            //Note: Since the initial state already initialized any remaining bits
            //after m_count, this does not need to be done again.
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
        /// Verifies that the <see cref="BitArray"/> has the capacity 
        /// to store the provided number of elements.
        /// If not, the bit array will autogrow by a factor of 2 or at least the capacity
        /// </summary>
        /// <param name="capacity"></param>
        public void EnsureCapacity(int capacity)
        {
            if (capacity > m_count)
            {
                SetCapacity(Math.Max(m_array.Length * BitsPerElement * 2, capacity));
            }
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
            for (int x = m_lastFoundClearedIndex >> BitsPerElementShift; x < count; x++)
            {
                //If the result is not -1 (all bits set), then use this element
                if (m_array[x] != -1)
                {
                    int position = BitMath.CountTrailingOnes((uint)m_array[x]) + (x << BitsPerElementShift);
                    m_lastFoundClearedIndex = position;
                    if (m_lastFoundClearedIndex >= m_count)
                        return -1;
                    return position;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first bit that is set. 
        /// -1 is returned if all bits are set.
        /// </summary>
        /// <returns></returns>
        public int FindSetBit()
        {
            //parse each item, 32 bits at a time
            int count = m_array.Length;
            for (int x = m_lastFoundSetIndex >> BitsPerElementShift; x < count; x++)
            {
                //If the result is not 0 (all bits cleared), then use this element
                if (m_array[x] != 0)
                {
                    int position = BitMath.CountTrailingZeros((uint)m_array[x]) + (x << BitsPerElementShift);
                    m_lastFoundSetIndex = position;
                    if (m_lastFoundSetIndex >= m_count)
                        return -1;
                    return position;
                }
            }
            return -1;
        }

        /// <summary>
        /// Yields a list of all bits that are set.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetAllSetBits()
        {
            int count = m_array.Length;
            for (int x = 0; x < count; x++)
            {
                //if all bits are cleared, this entire section can be skipped
                if (m_array[x] != 0)
                {
                    int end = Math.Min(x * BitsPerElement + BitsPerElement, m_count);
                    for (int k = x * BitsPerElement; k < end; k++)
                    {
                        if (GetBitUnchecked(k))
                            yield return k;
                    }
                }
            }
        }

        /// <summary>
        /// Yields a list of all bits that are cleared.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetAllClearedBits()
        {
            int count = m_array.Length;
            for (int x = 0; x < count; x++)
            {
                //if all bits are cleared, this entire section can be skipped
                if (m_array[x] != -1)
                {
                    int end = Math.Min(x * BitsPerElement + BitsPerElement, m_count);
                    for (int k = x * BitsPerElement; k < end; k++)
                    {
                        if (!GetBitUnchecked(k))
                            yield return k;
                    }
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Validate(int index)
        {
            if (index < 0 || index >= m_count)
                ThrowException(index);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowException(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Must be greater than or equal to zero.");
            if (index >= m_count)
                throw new ArgumentOutOfRangeException("index", "Exceedes the length of the array.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Validate(int index, int length)
        {
            if (index < 0 || length < 0 || index + length > m_count)
                ThrowException(index, length);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowException(int index, int length)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Must be greater than or equal to zero.");
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "Must be greater than or equal to zero.");
            if (index + length > m_count)
                throw new ArgumentOutOfRangeException("length", "index + length exceedes the length of the array.");
        }


        #endregion
    }
}