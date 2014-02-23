//******************************************************************************************************
//  SortedTreeTypeMethods`1.cs - Gbtc
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
//  2/22/2014 - Steven E. Chisholm
//       Combined Value and Key methods into a single class.
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.IO.Unmanaged;

namespace GSF.SortedTreeStore.Tree
{
    /// <summary>
    /// Specifies all of the core methods that need to be implemented for a <see cref="SortedTree"/> to be able
    /// to utilize this type of key.
    /// </summary>
    /// <remarks>
    /// There are many functions that are generically implemented in this class that can be overridden
    /// for vastly superiour performance.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class SortedTreeTypeMethods<T>
        where T : SortedTreeTypeBase<T>, new()
    {
        protected T TempKey = new T();
        protected T TempKey2 = new T();
        protected int LastFoundIndex;

        public SortedTreeTypeMethods()
        {
            Size = GetSize();
        }

        /// <summary>
        /// The fixed size of this value
        /// </summary>
        public int Size
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the size of this class when serialized
        /// </summary>
        /// <returns></returns>
        protected virtual int GetSize()
        {
            return TempKey.GetSize;
        }
        
        /// <summary>
        /// Writes the maximum value to the provided stream
        /// </summary>
        /// <param name="stream"></param>
        public virtual unsafe void WriteMax(byte* stream)
        {
            TempKey.SetMax();
            TempKey.Write(stream);
        }

        /// <summary>
        /// Writes the minimum value to the provided stream
        /// </summary>
        /// <param name="stream"></param>
        public virtual unsafe void WriteMin(byte* stream)
        {
            TempKey.SetMin();
            TempKey.Write(stream);
        }

        /// <summary>
        /// Writes null to the provided stream (hint: Clear state)
        /// </summary>
        /// <param name="stream"></param>
        public virtual unsafe void WriteNull(byte* stream)
        {
            TempKey.Clear();
            TempKey.Write(stream);
        }

        /// <summary>
        /// Copies the source to the destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public virtual unsafe void Copy(byte* source, byte* destination)
        {
            Memory.Copy(source, destination, Size);
        }

        /// <summary>
        /// Does a binary search on the data to find the best location for the <see cref="key"/>
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="key"></param>
        /// <param name="recordCount"></param>
        /// <param name="keyValueSize"></param>
        /// <returns></returns>
        public virtual unsafe int BinarySearch(byte* pointer, T key, int recordCount, int keyValueSize)
        {
            T compareKey = TempKey;
            if (LastFoundIndex == recordCount - 1)
            {
                compareKey.Read(pointer + keyValueSize * LastFoundIndex);
                if (key.IsGreaterThan(compareKey)) //Key > CompareKey
                {
                    LastFoundIndex++;
                    return ~recordCount;
                }
            }
            else if (LastFoundIndex < recordCount)
            {
                compareKey.Read(pointer + keyValueSize * (LastFoundIndex + 1));
                if (key.IsEqual(compareKey))
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
        protected virtual unsafe int BinarySearch2(byte* pointer, T key, int recordCount, int keyPointerSize)
        {
            if (recordCount == 0)
                return ~0;
            T compareKey = TempKey;
            int searchLowerBoundsIndex = 0;
            int searchHigherBoundsIndex = recordCount - 1;

            if (LastFoundIndex <= recordCount)
            {
                LastFoundIndex = Math.Min(LastFoundIndex, recordCount - 1);
                compareKey.Read(pointer + keyPointerSize * LastFoundIndex);

                if (key.IsEqual(compareKey)) //Are Equal
                    return LastFoundIndex;
                if (key.IsGreaterThan(compareKey)) //Key > CompareKey
                {
                    //Value is greater, check the next key
                    LastFoundIndex++;

                    //There is no greater key
                    if (LastFoundIndex == recordCount)
                        return ~recordCount;

                    compareKey.Read(pointer + keyPointerSize * LastFoundIndex);

                    if (key.IsEqual(compareKey)) //Are Equal
                        return LastFoundIndex;
                    if (key.IsGreaterThan(compareKey)) //Key > CompareKey
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
                    compareKey.Read(pointer + keyPointerSize * LastFoundIndex);

                    if (key.IsEqual(compareKey)) //Are Equal
                        return LastFoundIndex;
                    if (key.IsGreaterThan(compareKey)) //Key > CompareKey
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

                compareKey.Read(pointer + keyPointerSize * currentTestIndex);

                if (key.IsEqual(compareKey)) //Are Equal
                {
                    LastFoundIndex = currentTestIndex;
                    return currentTestIndex;
                }
                if (key.IsGreaterThan(compareKey)) //Key > CompareKey
                    searchLowerBoundsIndex = currentTestIndex + 1;
                else
                    searchHigherBoundsIndex = currentTestIndex - 1;
            }

            LastFoundIndex = searchLowerBoundsIndex;

            return ~searchLowerBoundsIndex;
        }

        #region [ Compare Operations ]

        /// <summary>
        /// Gets if left &gt;= right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual unsafe bool IsGreaterThan(T left, byte* right)
        {
            return CompareTo(left, right) > 0;
        }

        /// <summary>
        /// Gets if left &gt; right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual unsafe bool IsGreaterThan(byte* left, T right)
        {
            return CompareTo(left, right) > 0;
        }

        /// <summary>
        /// Compares Left to Right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual unsafe int CompareTo(T left, byte* right)
        {
            TempKey.Read(right);
            return left.CompareTo(TempKey);
        }

        /// <summary>
        /// Compares Left to Right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual unsafe int CompareTo(byte* left, byte* right)
        {
            TempKey.Read(left);
            TempKey2.Read(right);
            return TempKey.CompareTo(TempKey2);
        }

        /// <summary>
        /// Compares Left to Right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public virtual unsafe int CompareTo(byte* left, T right)
        {
            TempKey.Read(left);
            return TempKey.CompareTo(right);
        }

        #endregion

    }
}