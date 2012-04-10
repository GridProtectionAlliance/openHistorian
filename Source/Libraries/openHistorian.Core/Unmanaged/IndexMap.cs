//******************************************************************************************************
//  PageList.cs - Gbtc
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
//  3/16/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.V2.Unmanaged
{
    /// <summary>
    /// Provides a high speed lookup table. 
    /// This class is not thread safe.  
    /// Failing to synchronize calls will cause memory leaks and corrupt data.
    /// </summary>
    unsafe public class IndexMap<TValue>
        where TValue : struct, IKeyType<TValue>
    {
        #region [ Members ]

        const int ShiftBits = 13;
        const int ShiftBits2 = ShiftBits * 2;
        const int Mask = 4096 - 1;
        const int Mask2 = 4096 * 4096 - 1;
        const int InvertMask = ~Mask;
        const int InvertMask2 = ~Mask2;
        const int ValuesPerSingleIndex = 4096;
        const int ValuesPerDoubleIndex = 4096 * 4096;

        int m_singleIndex;
        byte* m_single;
        uint m_singleChildCount;

        int m_doubleIndex;
        byte* m_double;
        uint m_doubleChildCount;

        int m_tripleIndex;
        byte* m_triple;
        uint m_tripleChildCount;

        bool m_disposed;


        //Cached Lookup Table;

        bool m_containsValue;

        /// <summary>
        /// Contains the last key looked up
        /// </summary>
        int m_lastKeyLookup;

        /// <summary>
        /// Contains first address for last key if it exists.  Null otherwise.
        /// </summary>
        byte* m_firstIndirectBlock;

        /// <summary>
        /// Contains second address for last key if it exists.  Null otherwise.
        /// </summary>
        byte* m_secondIndirectBlock;

        /// <summary>
        /// Contains third address for last key if it exists.  Null otherwise.
        /// </summary>
        byte* m_thirdIndirectBlock;

        /// <summary>
        /// Contains address for the value for last key if it exists.  Null otherwise.
        /// </summary>
        byte* m_valueAddress;

        #endregion

        #region [ Constructors ]

        public IndexMap()
        {
            TValue value = default(TValue);
            if (value.Size != 16)
            {
                throw new Exception("The only TValue.Size that is currently supported is 16 bytes.");
            }
            m_single = null;
            m_double = null;
            m_triple = null;
            m_singleIndex = -1;
            m_doubleIndex = -1;
            m_tripleIndex = -1;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="IndexMap{TValue}"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~IndexMap()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Determines if the <see cref="IndexMap{TValue}"/> contains the key.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key is contained in this <see cref="IndexMap{TValue}"/></returns>
        public bool Contains(int key)
        {
            if (m_disposed)
                throw new ObjectDisposedException("Object Already Disposed");
            if (key < 0)
                throw new ArgumentOutOfRangeException("key");

            return FindValueAddress(key);
        }

        /// <summary>
        /// This removes an Key from the <see cref="IndexMap{TValue}"/>.
        /// </summary>
        /// <param name="key">The key to remove</param>
        public void Remove(int key)
        {
            if (m_disposed)
                throw new ObjectDisposedException("Object Already Disposed");
            if (key < 0)
                throw new ArgumentOutOfRangeException("key");

            if (!FindValueAddress(key))
            {
                throw new Exception("Entry does not exist");
            }

            *(int*)(m_valueAddress) = -1;
            DecrementParent(key);
            m_containsValue = false;
        }

        public void Add(int key, TValue value)
        {
            if (m_disposed)
                throw new ObjectDisposedException("Object Already Disposed");
            if (key < 0)
                throw new ArgumentOutOfRangeException("key");

            if (FindValueAddressEnsureCapacity(key))
            {
                throw new Exception("Duplicate Key");
            }
            m_containsValue = true;
            IncrementParent(key);

            value.Save(m_valueAddress);
        }

        /// <summary>
        /// Increments the parent's child counter;
        /// </summary>
        /// <param name="key">the key</param>
        void IncrementParent(int key)
        {
            if (key != m_lastKeyLookup || !m_containsValue)
                throw new ArgumentException("Item must be found first", "key");

            if (key < ValuesPerSingleIndex)
            {
                m_singleChildCount++;
            }
            else if (key < ValuesPerDoubleIndex)
            {
                AddToBlock(m_firstIndirectBlock, key >> ShiftBits, 1);
            }
            else
            {
                AddToBlock(m_secondIndirectBlock, key >> ShiftBits, 1);
            }
        }

        /// <summary>
        /// Decrements the child counter in the parents of the value.
        /// If the parent node now has no children, it will be released back to the buffer pool.
        /// This process is then recursive, removing all unused nodes in the tree.
        /// </summary>
        /// <param name="key">The key</param>
        void DecrementParent(int key)
        {
            if (key != m_lastKeyLookup || !m_containsValue)
                throw new ArgumentException("Item must be found first", "key");

            m_lastKeyLookup = -1;

            if (key < ValuesPerSingleIndex)
            {
                //subtract parent's child count
                m_singleChildCount--;
                if (m_singleChildCount == 0)
                {
                    //if there are no children for this node, remove it
                    BufferPool.ReleasePage(m_singleIndex);
                    m_single = null;
                    m_singleIndex = -1;
                }
            }
            else if (key < ValuesPerDoubleIndex)
            {
                int pageIndex;
                if (DecrementBlock(m_firstIndirectBlock, key >> ShiftBits, 1, out pageIndex))
                {
                    
                    //if there are no children for this node, remove it
                    BufferPool.ReleasePage(pageIndex);

                    //decrement the counter of the recently removed item.
                    m_doubleChildCount--;
                    if (m_doubleChildCount == 0)
                    {
                        //if there are no children for this node, remove it
                        BufferPool.ReleasePage(m_doubleIndex);
                        m_doubleIndex = -1;
                        m_double = null;
                    }
                }
            }
            else
            {

                int pageIndex;
                if (DecrementBlock(m_secondIndirectBlock, key >> ShiftBits, 1, out pageIndex))
                {
                    //if there are no children for this node, remove it
                    BufferPool.ReleasePage(pageIndex);

                    if (DecrementBlock(m_firstIndirectBlock, key >> ShiftBits2, 1, out pageIndex))
                    {
                        //if there are no children for this node, remove it
                        BufferPool.ReleasePage(pageIndex);

                        //decrement the counter of the recently removed item.
                        m_tripleChildCount--;
                        if (m_tripleChildCount == 0)
                        {
                            //if there are no children for this node, remove it
                            BufferPool.ReleasePage(m_tripleIndex);
                            m_tripleIndex = -1;
                            m_triple = null;
                        }
                    }
                }

            }
        }
        
        /// <summary>
        /// Returns true if value was decremented to 0
        /// </summary>
        /// <param name="blockAddress"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <param name="blockIndex"></param>
        /// <returns></returns>
        bool DecrementBlock(byte* blockAddress, int offset, int value, out int blockIndex)
        {
            if (blockAddress == null)
                throw new ArgumentNullException("blockAddress");

            blockAddress += ((offset & Mask) << 4);
            *(int*)(blockAddress + 4) -= value;

            if (*(int*)(blockAddress + 4)==0)
            {
                blockIndex = *(int*)(blockAddress);
                *(int*)(blockAddress) = -1;
                *(int*)(blockAddress + 4) = 0;
                return true;
            }
            blockIndex = -1;
            return false;
        }


        public TValue Get(int key)
        {
            TValue rv;
            if (!Get(key, out rv))
            {
                throw new Exception("Key not found");
            }
            return rv;
        }

        public bool Get(int key, out TValue value)
        {
            if (m_disposed)
                throw new ObjectDisposedException("Object Already Disposed");
            if (key < 0)
                throw new ArgumentOutOfRangeException("key");

            FindValueAddress(key);
            if (!m_containsValue)
            {
                value = default(TValue);
                return false;
            }
            value = default(TValue).Load(m_valueAddress);
            return true;
        }

        #endregion

        #region [ Helper Methods ]


        /// <summary>
        /// Looks up the key and populate local cached variables
        /// </summary>
        /// <param name="key">the key</param>
        bool FindValueAddress(int key)
        {
            m_lastKeyLookup = -1;
            //if the keys have not changed, return the cached value
            if (key == m_lastKeyLookup)
                return m_containsValue;

            if (key < ValuesPerSingleIndex)
            {
                FindValueAddressSingleLookup(key);
            }
            else if (key < ValuesPerDoubleIndex)
            {
                FindValueAddressDoubleLookup(key);
            }
            else
            {
                FindValueAddressTripleLookup(key);
            }
            m_lastKeyLookup = key;
            return m_containsValue;
        }

        void FindValueAddressSingleLookup(int key)
        {
            m_firstIndirectBlock = m_single;
            PopulateChildValueAddress(m_single, key);
        }

        void FindValueAddressDoubleLookup(int key)
        {
            //if the first index is the same, 
            //this block of code can be skipped.
            if ((m_lastKeyLookup | Mask) != (key | Mask))
            {
                m_firstIndirectBlock = m_double;
                if (!TryGetChildNodeAddress(m_firstIndirectBlock, key >> ShiftBits, out m_secondIndirectBlock))
                {
                    m_valueAddress = null;
                    m_containsValue = false;
                    return;
                }
            }
            PopulateChildValueAddress(m_secondIndirectBlock, key);
        }

        void FindValueAddressTripleLookup(int key)
        {
            //if the first index is the same, 
            //this block of code can be skipped.
            if ((m_lastKeyLookup | Mask2) != (key | Mask2))
            {
                m_firstIndirectBlock = m_triple;
                if (!TryGetChildNodeAddress(m_firstIndirectBlock, key >> ShiftBits2, out m_secondIndirectBlock))
                {
                    m_thirdIndirectBlock = null;
                    m_valueAddress = null;
                    m_containsValue = false;
                    return;
                }
            }

            if ((m_lastKeyLookup | Mask) != (key | Mask)) //if index has changed
            {
                if (!TryGetChildNodeAddress(m_secondIndirectBlock, key >> ShiftBits, out m_thirdIndirectBlock))
                {
                    m_valueAddress = null;
                    m_containsValue = false;
                    return;
                }
            }
            PopulateChildValueAddress(m_thirdIndirectBlock, key);
        }

        /// <summary>
        /// Looks up the key and populate local cached variables
        /// </summary>
        /// <param name="key">the key</param>
        bool FindValueAddressEnsureCapacity(int key)
        {
            m_lastKeyLookup = -1;

            if (m_lastKeyLookup == key && m_valueAddress != null)
                return m_containsValue;

            if (key < ValuesPerSingleIndex)
            {
                FindValueAddressSingleLookupAllocate(key);
            }
            else if (key < ValuesPerDoubleIndex)
            {
                FindValueAddressDoubleLookupAllocate(key);
            }
            else
            {
                FindValueAddressTripleLookupAllocate(key);
            }
            m_lastKeyLookup = key;
            return m_containsValue;
        }


        void FindValueAddressSingleLookupAllocate(int key)
        {
            if (m_single == null)
            {
                AllocateBlock(out m_singleIndex, out m_single);
                m_singleChildCount = 0;
            }
            m_firstIndirectBlock = m_single;
            PopulateChildValueAddressAllocate(m_firstIndirectBlock, key);
        }

        void FindValueAddressDoubleLookupAllocate(int key)
        {

            if (m_double == null)
            {
                AllocateBlock(out m_doubleIndex, out m_double);
                m_doubleChildCount = 0;
            }

            //if the first index is the same, 
            //this block of code can be skipped.
            if ((m_lastKeyLookup | Mask) != (key | Mask) || m_secondIndirectBlock == null)
            {
                bool blockAllocated;
                m_firstIndirectBlock = m_double;
                GetChildNodeAddressAllocate(m_firstIndirectBlock, key >> ShiftBits, out m_secondIndirectBlock, out blockAllocated);
                if (blockAllocated)
                    m_doubleChildCount++;
            }

            PopulateChildValueAddressAllocate(m_secondIndirectBlock, key);
        }

        void FindValueAddressTripleLookupAllocate(int key)
        {
            bool blockAllocated;
            if (m_triple == null)
            {
                AllocateBlock(out m_tripleIndex, out m_triple);
                m_tripleChildCount = 0;
            }

            //if the first index is the same, 
            //this block of code can be skipped.
            if ((m_lastKeyLookup | Mask2) != (key | Mask2) || m_secondIndirectBlock == null)
            {
                m_firstIndirectBlock = m_triple;
                GetChildNodeAddressAllocate(m_firstIndirectBlock, key >> ShiftBits2, out m_secondIndirectBlock, out blockAllocated);
                if (blockAllocated)
                    m_tripleChildCount++;
            }

            //if the second index is the same, 
            //this block of code can be skipped.
            if ((m_lastKeyLookup | Mask) != (key | Mask) || m_thirdIndirectBlock == null) //if index has changed
            {
                GetChildNodeAddressAllocate(m_secondIndirectBlock, key >> ShiftBits, out m_thirdIndirectBlock, out blockAllocated);
                if (blockAllocated)
                    AddToBlock(m_firstIndirectBlock, key >> ShiftBits2, 1);
            }

            PopulateChildValueAddressAllocate(m_thirdIndirectBlock, key);
        }


        /// <summary>
        /// Navigates to the block and to get the child address
        /// </summary>
        /// <param name="blockAddress">if null, return values will be null.</param>
        /// <param name="offset">the offset position to check for. 
        ///   The mask is taken care of in this function</param>
        void PopulateChildValueAddress(byte* blockAddress, int offset)
        {
            if (blockAddress == null)
            {
                m_valueAddress = null;
                m_containsValue = false;
            }
            else
            {
                m_valueAddress = blockAddress + ((offset & Mask) << 4);
                m_containsValue = default(TValue).IsNotNull(m_valueAddress);
            }
        }

        /// <summary>
        /// Navigates to the block and tries to get the child address
        /// </summary>
        /// <param name="blockAddress">if null, function always returns false.</param>
        /// <param name="offset">the offset position to check for. 
        /// The mask is taken care of in this function</param>
        /// <param name="childAddress">the address to set. null is assigned if it cannot be determined</param>
        /// <returns>true if found, false with <see cref="childAddress"/> set to null if not found.</returns>
        bool TryGetChildNodeAddress(byte* blockAddress, int offset, out byte* childAddress)
        {
            if (blockAddress == null)
            {
                childAddress = null;
                return false;
            }
            blockAddress += (offset & Mask) << 4;
            if (*(int*)blockAddress < 0)
            {
                childAddress = null;
                return false;
            }
            childAddress = (byte*)*(long*)(blockAddress + 8);
            return true;
        }


        /// <summary>
        /// Navigates to the block and tries to get the child address
        /// </summary>
        /// <param name="blockAddress">if null, function always returns false.</param>
        /// <param name="offset">the offset position to check for. 
        /// The mask is taken care of in this function</param>
        /// <returns>true if found, false if not found.</returns>
        void PopulateChildValueAddressAllocate(byte* blockAddress, int offset)
        {
            if (blockAddress == null)
                throw new ArgumentNullException("blockAddress");

            m_valueAddress = blockAddress + ((offset & Mask) << 4);
            m_containsValue = default(TValue).IsNotNull(m_valueAddress);
        }


        /// <summary>
        /// Navigates to the block and tries to get the child address
        /// </summary>
        /// <param name="blockAddress">if null, function always returns false.</param>
        /// <param name="offset">the offset position to check for. 
        /// The mask is taken care of in this function</param>
        /// <param name="childAddress">the address to set. null is assigned if it cannot be determined</param>
        /// <param name="blockAllocated">true if a block was child was missing and a block was allocated</param>
        void GetChildNodeAddressAllocate(byte* blockAddress, int offset, out byte* childAddress, out bool blockAllocated)
        {
            blockAllocated = false;
            if (blockAddress == null)
                throw new ArgumentNullException("blockAddress");

            blockAddress += ((offset & Mask) << 4);
            if (*(int*)blockAddress < 0)
            {
                blockAllocated = true;
                AllocateChild(blockAddress);
            }
            childAddress = (byte*)*(long*)(blockAddress + 8);
        }


        void AddToBlock(byte* blockAddress, int offset, int value)
        {
            if (blockAddress == null)
                throw new ArgumentNullException("blockAddress");

            blockAddress += ((offset & Mask) << 4);
            *(int*)(blockAddress + 4) += value;
        }


        /// <summary>
        /// Allocates a new block
        /// </summary>
        /// <param name="index"></param>
        /// <param name="address"></param>
        void AllocateBlock(out int index, out byte* address)
        {
            IntPtr ptr;
            index = BufferPool.AllocatePage(out ptr);
            address = (byte*)ptr.ToPointer();
            int* lpInt = (int*)address;
            for (int x = 0; x < 65536 / 4; x += 2)
            {
                lpInt[x] = -1;
                lpInt[x+1] = 0;
            }
        }

        void AllocateChild(byte* blockPointer)
        {
            byte* pointer;
            int index;
            AllocateBlock(out index, out pointer);

            *(int*)(blockPointer) = index;
            *(uint*)(blockPointer + 4) = 0;
            *(long*)(blockPointer + 8) = (long)pointer;
        }

        #endregion

        #region [ Implements IDisposable ]

        /// <summary>
        /// Releases all the resources used by the <see cref="IndexMap{TValue}"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="IndexMap{TValue}"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #endregion

    }
}
