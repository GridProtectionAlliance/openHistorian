using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace openHistorian.Core.Unmanaged
{
    unsafe public class PageList
    {
        const int ShiftBits = 13;
        const int Mask = 4096 - 1;
        const int ItemsPerSingleIndex = 4096;
        const int ItemsPerDoubleIndex = 4096 * 4096;

        int m_singleIndex;
        byte* m_single;

        int m_doubleIndex;
        byte* m_double;

        int m_tripleIndex;
        byte* m_triple;

        public PageList()
        {
            m_single = (byte*)0;
            m_double = (byte*)0;
            m_triple = (byte*)0;
            m_singleIndex = -1;
            m_doubleIndex = -1;
            m_tripleIndex = -1;
        }

        public void Add(int location, int nodeIndex, uint meta, byte* pointer)
        {
            if (location < 0)
                throw new ArgumentOutOfRangeException("location");

            if (location < ItemsPerSingleIndex)
            {
                if (m_singleIndex < 0)
                    AllocateSingle();

                *(int*)(m_single + (location << 4)) = nodeIndex;
                *(uint*)(m_single + (location << 4) + 4) = meta;
                *(long*)(m_single + (location << 4) + 8) = (long)pointer;
            }
            else if (location < ItemsPerDoubleIndex)
            {
                //double
                if (m_doubleIndex < 0)
                    AllocateDouble();

                int firstLocation = location >> ShiftBits;
                int secondLocation = location & Mask;
                byte* secondPointer;

                //If not exists, allocate
                if (*(int*)(m_double + (firstLocation << 4)) < 0)
                {
                    int secondIndex;
                    AllocateBlock(out secondIndex, out secondPointer);

                    *(int*)(m_double + (firstLocation << 4)) = secondIndex;
                    *(uint*)(m_double + (firstLocation << 4) + 4) = 1;
                    *(long*)(m_double + (firstLocation << 4) + 8) = (long)secondPointer;
                }

                secondPointer = (byte*)*(long*)(m_double + (firstLocation << 4) + 8);

                *(int*)(secondPointer + (secondLocation << 4)) = nodeIndex;
                *(uint*)(secondPointer + (secondLocation << 4) + 4) = meta;
                *(long*)(secondPointer + (secondLocation << 4) + 8) = (long)pointer;
            }
            else
            {
                //triple
                if (m_tripleIndex < 0)
                    AllocateTriple();

                int firstLocation = location >> (ShiftBits * 2);
                int secondLocation = (location >> ShiftBits) & Mask;
                int thirdLocation = location & Mask;

                byte* secondPointer;
                byte* thirdPointer;

                //If not exists, allocate
                if (*(int*)(m_triple + (firstLocation << 4)) < 0)
                {
                    int secondIndex;
                    AllocateBlock(out secondIndex, out secondPointer);

                    *(int*)(m_triple + (firstLocation << 4)) = secondIndex;
                    *(uint*)(m_triple + (firstLocation << 4) + 4) = 1;
                    *(long*)(m_triple + (firstLocation << 4) + 8) = (long)secondPointer;
                }
                secondPointer = (byte*)*(long*)(m_triple + (firstLocation << 4) + 8);
                //If not exists, allocate

                if (*(int*)(secondPointer + (secondLocation << 4)) < 0)
                {
                    int thirdIndex;
                    AllocateBlock(out thirdIndex, out thirdPointer);

                    *(int*)(secondPointer + (secondLocation << 4)) = thirdIndex;
                    *(uint*)(secondPointer + (secondLocation << 4) + 4) = 1;
                    *(long*)(secondPointer + (secondLocation << 4) + 8) = (long)thirdPointer;
                }

                thirdPointer = (byte*)*(long*)(secondPointer + (secondLocation << 4) + 8);

                *(int*)(thirdPointer + (thirdLocation << 4)) = nodeIndex;
                *(uint*)(thirdPointer + (thirdLocation << 4) + 4) = meta;
                *(long*)(thirdPointer + (thirdLocation << 4) + 8) = (long)pointer;
            }
        }

        public bool Get(int location, out int nodeIndex, out uint meta, out byte* pointer)
        {
            if (location < 0)
                throw new ArgumentOutOfRangeException("location");

            if (location < ItemsPerSingleIndex)
            {
                if (m_singleIndex < 0)
                {
                    nodeIndex = -1;
                    meta = 0;
                    pointer = (byte*)0;
                    return false;
                }

                nodeIndex = *(int*)(m_single + (location << 4));
                meta = *(uint*)(m_single + (location << 4) + 4);
                pointer = (byte*)*(long*)(m_single + (location << 4) + 8);
            }
            else if (location < ItemsPerDoubleIndex)
            {
                //double
                if (m_doubleIndex < 0)
                {
                    nodeIndex = -1;
                    meta = 0;
                    pointer = (byte*)0;
                    return false;
                }

                int firstLocation = location >> ShiftBits;
                int secondLocation = location & Mask;
                byte* secondPointer;

                //If not exists, allocate
                if (*(int*)(m_double + (firstLocation << 4)) < 0)
                {
                    nodeIndex = -1;
                    meta = 0;
                    pointer = (byte*)0;
                    return false;
                }

                secondPointer = (byte*)*(long*)(m_double + (firstLocation << 4) + 8);

                nodeIndex = *(int*)(secondPointer + (secondLocation << 4));
                meta = *(uint*)(secondPointer + (secondLocation << 4) + 4);
                pointer = (byte*)*(long*)(secondPointer + (secondLocation << 4) + 8);

            }
            else
            {
                //triple
                if (m_tripleIndex < 0)
                {
                    nodeIndex = -1;
                    meta = 0;
                    pointer = (byte*)0;
                    return false;
                }

                int firstLocation = location >> (ShiftBits * 2);
                int secondLocation = (location >> ShiftBits) & Mask;
                int thirdLocation = location & Mask;

                byte* secondPointer;
                byte* thirdPointer;

                //If not exists, allocate
                if (*(int*)(m_triple + (firstLocation << 4)) < 0)
                {
                    nodeIndex = -1;
                    meta = 0;
                    pointer = (byte*)0;
                    return false;
                }
                secondPointer = (byte*)*(long*)(m_triple + (firstLocation << 4) + 8);
                //If not exists, allocate

                if (*(int*)(secondPointer + (secondLocation << 4)) < 0)
                {
                    nodeIndex = -1;
                    meta = 0;
                    pointer = (byte*)0;
                    return false;
                }

                thirdPointer = (byte*)*(long*)(secondPointer + (secondLocation << 4) + 8);

                nodeIndex = *(int*)(thirdPointer + (thirdLocation << 4));
                meta = *(uint*)(thirdPointer + (thirdLocation << 4) + 4);
                pointer = (byte*)*(long*)(thirdPointer + (thirdLocation << 4) + 8);
            }
            return true;
        }

        void AllocateBlock(out int index, out byte* address)
        {
            IntPtr ptr;
            index = BufferPool.AllocatePage(out ptr);
            address = (byte*)ptr.ToPointer();
            int* lpInt = (int*)address;
            for (int x = 0; x < 65536 / 4; x += 4)
            {
                lpInt[x] = -1;
            }
        }

        void AllocateSingle()
        {
            AllocateBlock(out m_singleIndex, out m_single);
        }
        void AllocateDouble()
        {
            AllocateBlock(out m_doubleIndex, out m_double);
        }
        void AllocateTriple()
        {
            AllocateBlock(out m_tripleIndex, out m_triple);
        }

    }
}
