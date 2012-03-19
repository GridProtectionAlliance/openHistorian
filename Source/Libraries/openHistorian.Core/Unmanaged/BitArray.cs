using System;

namespace openHistorian.Core.Unmanaged
{
    public sealed class BitArray
    {
        int[] m_array;
        int m_count;
        int m_setCount;

        public BitArray(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if ((count & 31) != 0)
                m_array = new int[count >> 5 + 1];
            else
                m_array = new int[count >> 5];
            m_count = count;
            m_setCount = 0;
        }

        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= m_count)
                    throw new ArgumentOutOfRangeException("index");
                return (m_array[index >> 5] & (1 << (index & 31))) != 0;
            }
            set
            {
                if (index < 0 || index >= m_count)
                    throw new ArgumentOutOfRangeException("index");
                int bit = 1 << (index & 31);
                int oldValue = m_array[index >> 5];
                if (value)
                {
                    //set
                    m_array[index >> 5] = oldValue | bit;
                    if (m_array[index >> 5] != oldValue)
                        m_setCount++;
                }
                else
                {
                    //clear
                    m_array[index >> 5] = oldValue & ~bit;
                    if (m_array[index >> 5] != oldValue)
                        m_setCount--;
                }
            }
        }

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
        /// Returns the first bit that is cleared. 
        /// -1 is returned if all bits are set.
        /// </summary>
        /// <returns></returns>
        public int FindClearedBit()
        {
            for (int x = 0; x < m_array.Length; x++)
            {
                if (m_array[x >> 5] != -1) //if a single bit is set, the value will not be -1.
                {
                    for (int y=x<<5; y<m_count; y++)
                    {
                        if (!this[y])
                        {
                            return y;
                        }
                    }
                }
            }
            return -1;
        }


    }
}
