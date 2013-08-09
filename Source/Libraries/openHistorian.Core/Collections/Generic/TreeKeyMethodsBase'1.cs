//******************************************************************************************************
//  TreeKeyMethodsBase.cs - Gbtc
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
//  4/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.IO;
using GSF.IO;

namespace openHistorian.Collections.Generic
{
    /// <summary>
    /// Specifies all of the core methods that need to be implemented for a <see cref="SortedTree"/> to be able
    /// to utilize this type of key.
    /// </summary>
    /// <remarks>
    /// There are many functions that are generically implemented in this class that can be overridden
    /// for vastly superiour performance.
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    public abstract class TreeKeyMethodsBase<TKey>
        : CreateKeyMethodBase<TKey>
        where TKey : class, new()
    {
        protected TKey m_tempKey = new TKey();
        public int m_lastFoundIndex;

        public int Size
        {
            get;
            private set;
        }

        public abstract void Clear(TKey key);

        public abstract void SetMin(TKey key);

        public abstract void SetMax(TKey key);

        public abstract int CompareTo(TKey left, TKey right);

        public abstract unsafe void Write(byte* stream, TKey data);

        public abstract unsafe void Read(byte* stream, TKey data);

        protected abstract int GetSize();

        protected TreeKeyMethodsBase()
        {
            Size = GetSize();
        }

        public virtual unsafe void WriteMax(byte* stream)
        {
            SetMax(m_tempKey);
            Write(stream, m_tempKey);
        }

        public virtual unsafe void WriteMin(byte* stream)
        {
            SetMin(m_tempKey);
            Write(stream, m_tempKey);
        }

        public virtual unsafe void WriteNull(byte* stream)
        {
            Clear(m_tempKey);
            Write(stream, m_tempKey);
        }

        public virtual unsafe void Copy(TKey source, TKey destination)
        {
            byte* ptr = stackalloc byte[Size];
            Write(ptr, source);
            Read(ptr, destination);
        }

        public virtual unsafe void Read(BinaryStreamBase stream, TKey data)
        {
            byte* ptr = stackalloc byte[Size];
            stream.Read(ptr, Size);
            Read(ptr, data);
        }

        public virtual unsafe void Read(BinaryReader reader, TKey data)
        {
            byte* ptr = stackalloc byte[Size];
            for (int x = 0; x < Size; x++)
            {
                ptr[x] = reader.ReadByte();
            }
            Read(ptr, data);
        }

        public virtual unsafe void Write(BinaryWriter writer, TKey data)
        {
            byte* ptr = stackalloc byte[Size];
            Write(ptr, data);
            for (int x = 0; x < Size; x++)
            {
                writer.Write(ptr[x]);
            }
        }

        public virtual unsafe void Write(BinaryStreamBase stream, TKey data)
        {
            byte* ptr = stackalloc byte[Size];
            Write(ptr, data);
            stream.Write(ptr, Size);
        }

        public virtual unsafe int BinarySearch(byte* pointer, TKey key, int recordCount, int keyValueSize)
        {
            TKey compareKey = m_tempKey;
            if (m_lastFoundIndex == recordCount - 1)
            {
                Read(pointer + keyValueSize * m_lastFoundIndex, compareKey);
                if (IsGreaterThan(key, compareKey)) //Key > CompareKey
                {
                    m_lastFoundIndex++;
                    return ~recordCount;
                }
            }
            else if (m_lastFoundIndex < recordCount)
            {
                Read(pointer + keyValueSize * (m_lastFoundIndex + 1), compareKey);
                if (IsEqual(key, compareKey))
                {
                    m_lastFoundIndex++;
                    return m_lastFoundIndex;
                }
            }
            return BinarySearch2(pointer, key, recordCount, keyValueSize);
        }

        protected virtual unsafe int BinarySearch2(byte* pointer, TKey key, int recordCount, int keyPointerSize)
        {
            if (recordCount == 0)
                return ~0;
            TKey compareKey = m_tempKey;
            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = recordCount - 1;

            if (m_lastFoundIndex <= recordCount)
            {
                m_lastFoundIndex = Math.Min(m_lastFoundIndex, recordCount - 1);
                Read(pointer + keyPointerSize * m_lastFoundIndex, compareKey);

                if (IsEqual(key, compareKey)) //Are Equal
                    return m_lastFoundIndex;
                if (IsGreaterThan(key, compareKey)) //Key > CompareKey
                {
                    //Value is greater, check the next key
                    m_lastFoundIndex++;

                    //There is no greater key
                    if (m_lastFoundIndex == recordCount)
                        return ~recordCount;

                    Read(pointer + keyPointerSize * m_lastFoundIndex, compareKey);

                    if (IsEqual(key, compareKey)) //Are Equal
                        return m_lastFoundIndex;
                    if (IsGreaterThan(key, compareKey)) //Key > CompareKey
                        searchLowerBoundsIndex = m_lastFoundIndex + 1;
                    else
                        return ~m_lastFoundIndex;
                }
                else
                {
                    //Value is lesser, check the previous key
                    //There is no lesser key;
                    if (m_lastFoundIndex == 0)
                        return ~0;

                    m_lastFoundIndex--;
                    Read(pointer + keyPointerSize * m_lastFoundIndex, compareKey);

                    if (IsEqual(key, compareKey)) //Are Equal
                        return m_lastFoundIndex;
                    if (IsGreaterThan(key, compareKey)) //Key > CompareKey
                    {
                        m_lastFoundIndex++;
                        return ~(m_lastFoundIndex);
                    }
                    else
                        searchHigherBoundsIndex = m_lastFoundIndex - 1;
                }
            }

            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
            {
                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);

                Read(pointer + keyPointerSize * currentTestIndex, compareKey);

                if (IsEqual(key, compareKey)) //Are Equal
                {
                    m_lastFoundIndex = currentTestIndex;
                    return currentTestIndex;
                }
                if (IsGreaterThan(key, compareKey)) //Key > CompareKey
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }

            m_lastFoundIndex = searchLowerBoundsIndex;

            return ~searchLowerBoundsIndex;
        }

        public virtual bool IsBetween(TKey lowerBounds, TKey key, TKey upperBounds)
        {
            return IsLessThanOrEqualTo(lowerBounds, key) && IsLessThan(key, upperBounds);
        }

        public virtual bool IsLessThanOrEqualTo(TKey left, TKey right)
        {
            return CompareTo(left, right) <= 0;
        }

        public virtual bool IsLessThan(TKey left, TKey right)
        {
            return CompareTo(left, right) < 0;
        }

        public virtual bool IsNotEqual(TKey left, TKey right)
        {
            return CompareTo(left, right) != 0;
        }

        public virtual bool IsGreaterThan(TKey left, TKey right)
        {
            return CompareTo(left, right) > 0;
        }

        public virtual unsafe bool IsGreaterThan(TKey left, byte* right)
        {
            return CompareTo(left, right) > 0;
        }

        public virtual unsafe bool IsGreaterThan(byte* left, TKey right)
        {
            return CompareTo(left, right) > 0;
        }

        public virtual bool IsGreaterThanOrEqualTo(TKey left, TKey right)
        {
            return CompareTo(left, right) >= 0;
        }

        public virtual bool IsEqual(TKey left, TKey right)
        {
            return CompareTo(left, right) == 0;
        }

        public virtual unsafe bool IsEqual(TKey left, byte* right)
        {
            return CompareTo(left, right) == 0;
        }

        public virtual unsafe int CompareTo(TKey left, byte* right)
        {
            Read(right, m_tempKey);
            return CompareTo(left, m_tempKey);
        }

        public virtual unsafe int CompareTo(byte* left, TKey right)
        {
            Read(left, m_tempKey);
            return CompareTo(m_tempKey, right);
        }

        public override TreeKeyMethodsBase<TKey> Create()
        {
            TreeKeyMethodsBase<TKey> obj = (TreeKeyMethodsBase<TKey>)MemberwiseClone();
            obj.m_tempKey = new TKey();
            return obj;
        }
    }
}