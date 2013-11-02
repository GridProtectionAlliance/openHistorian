//******************************************************************************************************
//  TreeKeyMethodsBase`1.cs - Gbtc
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

//#define GetTreeKeyMethodsCallCount

using System;
using System.Collections.Generic;
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
        : IComparer<TKey> 
        where TKey : class, new()
    {

#if GetTreeKeyMethodsCallCount
        public static void ClearStats()
        {
            CallMethods = new long[100];
        }

        static public long[] CallMethods = new long[100];
        public enum Method
            : int
        {
            WriteMax,
            WriteMin,
            WriteNull,
            Copy,
            ReadBinaryStreamBase,
            ReadBinaryReader,
            WriteBinaryWriter,
            WriteBinaryStreamBase,
            BinarySearch,
            BinarySearch2,
            IsBetween,
            IsLessThanOrEqualTo,
            IsLessThan,
            IsNotEqual,
            IsGreaterThan,
            IsGreaterThanPointer,
            IsGreaterThanPointer2,
            IsGreaterThanOrEqualTo,
            IsEqual,
            IsEqualPointer,
            CompareToPointer,
            CompareToPointer2,
            Create
        }
        public static void WriteToConsole()
        {
            Console.WriteLine("KeyMethodsBase calls");
            for (int x = 0; x < 23; x++)
            {
                Console.WriteLine(CallMethods[x] + "\t" + ((Method)(x)).ToString());
            }
        }
#endif
        
        protected TKey TempKey = new TKey();
       
        protected int LastFoundIndex;

        /// <summary>
        /// The fixed size of this key
        /// </summary>
        public int Size
        {
            get;
            private set;
        }
        /// <summary>
        /// Clears the key
        /// </summary>
        /// <param name="key"></param>
        public abstract void Clear(TKey key);
        /// <summary>
        /// Sets the provided key to it's minimum value
        /// </summary>
        /// <param name="key"></param>
        public abstract void SetMin(TKey key);
        /// <summary>
        /// Sets the privided key to it's maximum value
        /// </summary>
        /// <param name="key"></param>
        public abstract void SetMax(TKey key);

        /// <summary>
        /// Compares two keys together
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public abstract int CompareTo(TKey left, TKey right);

        /// <summary>
        /// Writes the key to the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public abstract unsafe void Write(byte* stream, TKey data);

        /// <summary>
        /// Reads the key from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public abstract unsafe void Read(byte* stream, TKey data);

        /// <summary>
        /// Writes the <see cref="currentKey"/> as a delta from the <see cref="previousKey"/> to the provided stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="currentKey"></param>
        /// <param name="previousKey"></param>
        public abstract void WriteCompressed(BinaryStreamBase stream, TKey currentKey, TKey previousKey);

        /// <summary>
        /// Reads the <see cref="currentKey"/> as a delta from the <see cref="previousKey"/> from the provided stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="currentKey"></param>
        /// <param name="previousKey"></param>
        public abstract void ReadCompressed(BinaryStreamBase stream, TKey currentKey, TKey previousKey);

        /// <summary>
        /// Gets the size of this class when serialized
        /// </summary>
        /// <returns></returns>
        protected abstract int GetSize();

        protected TreeKeyMethodsBase()
        {
            Size = GetSize();
        }

        /// <summary>
        /// The Guid uniquely defining this type. 
        /// It is important to uniquely tie 1 type to 1 guid.
        /// </summary>
        public abstract Guid GenericTypeGuid
        {
            get;
        }

        /// <summary>
        /// Writes the maximum value to the provided stream
        /// </summary>
        /// <param name="stream"></param>
        public virtual unsafe void WriteMax(byte* stream)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.WriteMax]++;
#endif
            SetMax(TempKey);
            Write(stream, TempKey);
        }
        /// <summary>
        /// Writes the minimum value to the provided stream
        /// </summary>
        /// <param name="stream"></param>
        public virtual unsafe void WriteMin(byte* stream)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.WriteMin]++;
#endif
            SetMin(TempKey);
            Write(stream, TempKey);
        }
        /// <summary>
        /// Writes null to the provided stream (hint: Clear state)
        /// </summary>
        /// <param name="stream"></param>
        public virtual unsafe void WriteNull(byte* stream)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.WriteNull]++;
#endif
            Clear(TempKey);
            Write(stream, TempKey);
        }
        /// <summary>
        /// Copies the source to the destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public virtual unsafe void Copy(TKey source, TKey destination)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.Copy]++;
#endif
            byte* ptr = stackalloc byte[Size];
            Write(ptr, source);
            Read(ptr, destination);
        }
        /// <summary>
        /// Reads the provided key from the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public virtual unsafe void Read(BinaryStreamBase stream, TKey data)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.ReadBinaryStreamBase]++;
#endif
            byte* ptr = stackalloc byte[Size];
            stream.Read(ptr, Size);
            Read(ptr, data);
        }
        /// <summary>
        /// Reads the provided key from the BinaryReader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="data"></param>
        public virtual unsafe void Read(BinaryReader reader, TKey data)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.ReadBinaryReader]++;
#endif
            byte* ptr = stackalloc byte[Size];
            for (int x = 0; x < Size; x++)
            {
                ptr[x] = reader.ReadByte();
            }
            Read(ptr, data);
        }
        /// <summary>
        /// Writes the provided data to the BinaryWriter
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="data"></param>
        public virtual unsafe void Write(BinaryWriter writer, TKey data)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.WriteBinaryWriter]++;
#endif
            byte* ptr = stackalloc byte[Size];
            Write(ptr, data);
            for (int x = 0; x < Size; x++)
            {
                writer.Write(ptr[x]);
            }
        }
        /// <summary>
        /// Writes the provided data to the Stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public virtual unsafe void Write(BinaryStreamBase stream, TKey data)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.WriteBinaryStreamBase]++;
#endif
            byte* ptr = stackalloc byte[Size];
            Write(ptr, data);
            stream.Write(ptr, Size);
        }
        /// <summary>
        /// Does a binary search on the data to find the best location for the <see cref="key"/>
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="key"></param>
        /// <param name="recordCount"></param>
        /// <param name="keyValueSize"></param>
        /// <returns></returns>
        public virtual unsafe int BinarySearch(byte* pointer, TKey key, int recordCount, int keyValueSize)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.BinarySearch]++;
#endif
            TKey compareKey = TempKey;
            if (LastFoundIndex == recordCount - 1)
            {
                Read(pointer + keyValueSize * LastFoundIndex, compareKey);
                if (IsGreaterThan(key, compareKey)) //Key > CompareKey
                {
                    LastFoundIndex++;
                    return ~recordCount;
                }
            }
            else if (LastFoundIndex < recordCount)
            {
                Read(pointer + keyValueSize * (LastFoundIndex + 1), compareKey);
                if (IsEqual(key, compareKey))
                {
                    LastFoundIndex++;
                    return LastFoundIndex;
                }
            }
            return BinarySearch2(pointer, key, recordCount, keyValueSize);
        }

        /// <summary>
        /// A catch all BinarySearch.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="key"></param>
        /// <param name="recordCount"></param>
        /// <param name="keyPointerSize"></param>
        /// <returns></returns>
        protected virtual unsafe int BinarySearch2(byte* pointer, TKey key, int recordCount, int keyPointerSize)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.BinarySearch2]++;
#endif
            if (recordCount == 0)
                return ~0;
            TKey compareKey = TempKey;
            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = recordCount - 1;

            if (LastFoundIndex <= recordCount)
            {
                LastFoundIndex = Math.Min(LastFoundIndex, recordCount - 1);
                Read(pointer + keyPointerSize * LastFoundIndex, compareKey);

                if (IsEqual(key, compareKey)) //Are Equal
                    return LastFoundIndex;
                if (IsGreaterThan(key, compareKey)) //Key > CompareKey
                {
                    //Value is greater, check the next key
                    LastFoundIndex++;

                    //There is no greater key
                    if (LastFoundIndex == recordCount)
                        return ~recordCount;

                    Read(pointer + keyPointerSize * LastFoundIndex, compareKey);

                    if (IsEqual(key, compareKey)) //Are Equal
                        return LastFoundIndex;
                    if (IsGreaterThan(key, compareKey)) //Key > CompareKey
                        searchLowerBoundsIndex = LastFoundIndex + 1;
                    else
                        return ~LastFoundIndex;
                }
                else
                {
                    //Value is lesser, check the previous key
                    //There is no lesser key;
                    if (LastFoundIndex == 0)
                        return ~0;

                    LastFoundIndex--;
                    Read(pointer + keyPointerSize * LastFoundIndex, compareKey);

                    if (IsEqual(key, compareKey)) //Are Equal
                        return LastFoundIndex;
                    if (IsGreaterThan(key, compareKey)) //Key > CompareKey
                    {
                        LastFoundIndex++;
                        return ~(LastFoundIndex);
                    }
                    else
                        searchHigherBoundsIndex = LastFoundIndex - 1;
                }
            }

            while (searchLowerBoundsIndex <= searchHigherBoundsIndex)
            {
                int currentTestIndex = searchLowerBoundsIndex + (searchHigherBoundsIndex - searchLowerBoundsIndex >> 1);

                Read(pointer + keyPointerSize * currentTestIndex, compareKey);

                if (IsEqual(key, compareKey)) //Are Equal
                {
                    LastFoundIndex = currentTestIndex;
                    return currentTestIndex;
                }
                if (IsGreaterThan(key, compareKey)) //Key > CompareKey
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }

            LastFoundIndex = searchLowerBoundsIndex;

            return ~searchLowerBoundsIndex;
        }

        /// <summary>
        /// Gets if lowerBounds &lt;= key &lt; upperBounds
        /// </summary>
        /// <param name="lowerBounds"></param>
        /// <param name="key"></param>
        /// <param name="upperBounds"></param>
        /// <returns></returns>
        public virtual bool IsBetween(TKey lowerBounds, TKey key, TKey upperBounds)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.IsBetween]++;
#endif
            return IsLessThanOrEqualTo(lowerBounds, key) && IsLessThan(key, upperBounds);
        }

        /// <summary>
        /// Gets if left &lt;= right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsLessThanOrEqualTo(TKey left, TKey right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.IsLessThanOrEqualTo]++;
#endif
            return CompareTo(left, right) <= 0;
        }

        /// <summary>
        /// Gets if left &lt; right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsLessThan(TKey left, TKey right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.IsLessThan]++;
#endif
            return CompareTo(left, right) < 0;
        }

        /// <summary>
        /// Gets if left != right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsNotEqual(TKey left, TKey right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.IsNotEqual]++;
#endif
            return CompareTo(left, right) != 0;
        }

        /// <summary>
        /// Gets if left &gt; right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsGreaterThan(TKey left, TKey right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.IsGreaterThan]++;
#endif
            return CompareTo(left, right) > 0;
        }

        /// <summary>
        /// Gets if left &gt;= right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual unsafe bool IsGreaterThan(TKey left, byte* right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.IsGreaterThanPointer]++;
#endif
            return CompareTo(left, right) > 0;
        }

        /// <summary>
        /// Gets if left &gt; right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual unsafe bool IsGreaterThan(byte* left, TKey right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.IsGreaterThanPointer2]++;
#endif
            return CompareTo(left, right) > 0;
        }
        /// <summary>
        /// Gets if left &gt;= right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsGreaterThanOrEqualTo(TKey left, TKey right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.IsGreaterThanOrEqualTo]++;
#endif
            return CompareTo(left, right) >= 0;
        }
        /// <summary>
        /// Gets if left == right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual bool IsEqual(TKey left, TKey right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.IsEqual]++;
#endif
            return CompareTo(left, right) == 0;
        }
        /// <summary>
        /// Gets if left == right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual unsafe bool IsEqual(TKey left, byte* right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.IsEqualPointer]++;
#endif
            return CompareTo(left, right) == 0;
        }
        /// <summary>
        /// Compares Left to Right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual unsafe int CompareTo(TKey left, byte* right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.CompareToPointer]++;
#endif
            Read(right, TempKey);
            return CompareTo(left, TempKey);
        }

        /// <summary>
        /// Compares Left to Right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual unsafe int CompareTo(byte* left, TKey right)
        {
#if GetTreeKeyMethodsCallCount
            CallMethods[(int)Method.CompareToPointer2]++;
#endif
            Read(left, TempKey);
            return CompareTo(TempKey, right);
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
        public int Compare(TKey x, TKey y)
        {
            return CompareTo(x, y);
        }
        
    }
}