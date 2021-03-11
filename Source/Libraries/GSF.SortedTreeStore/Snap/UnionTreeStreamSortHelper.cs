//******************************************************************************************************
//  UnionTreeStreamSortHelper'1.cs - Gbtc
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
//  10/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Collections.Generic;
using System.Linq;

namespace GSF.Snap
{
    public partial class UnionTreeStream<TKey, TValue>
    {
        /// <summary>
        /// Provides basic sorting methods that assist in UnionKeyValueStream's speed.
        /// </summary>
        private class UnionTreeStreamSortHelper
        {
            /// <summary>
            /// All of the items in this list.
            /// </summary>
            public readonly BufferedTreeStream[] Items;
            private int m_validRecords;

            /// <summary>
            /// Creates a new custom sort helper and presorts the list.
            /// </summary>
            /// <param name="items"></param>
            public UnionTreeStreamSortHelper(IEnumerable<BufferedTreeStream> items)
            {
                Items = items.ToArray();
                m_validRecords = Items.Length;
                Sort();
            }

            /// <summary>
            /// Indexer to get the specified item out of the list
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public BufferedTreeStream this[int index] => Items[index];

            /// <summary>
            /// Resorts the entire list. Uses an insertion sort routine
            /// </summary>
            public void Sort()
            {
                //A insertion sort routine.

                //Skip first item in list since it will always be sorted correctly
                for (int itemToInsertIndex = 1; itemToInsertIndex < m_validRecords; itemToInsertIndex++)
                {
                    BufferedTreeStream itemToInsert = Items[itemToInsertIndex];

                    int currentIndex = itemToInsertIndex - 1;
                    //While the current item is greater than itemToInsert, shift the value
                    while (currentIndex >= 0 && IsLessThan(itemToInsert, Items[currentIndex]))
                    {
                        Items[currentIndex + 1] = Items[currentIndex];
                        currentIndex--;
                    }
                    Items[currentIndex + 1] = itemToInsert;
                }

                for (int x = m_validRecords - 1; x >= 0; x--)
                {
                    if (Items[x].IsValid)
                    {
                        m_validRecords = x + 1;
                        break;
                    }

                    if (x == 0)
                    {
                        m_validRecords = 0;
                    }
                }
            }

            /// <summary>
            /// Resorts only the item at the specified index assuming:
            /// 1) all other items are properly sorted
            /// 2) this items's value increased.
            /// </summary>
            /// <param name="index">the index of the item to resort.</param>
            public void SortAssumingIncreased(int index)
            {
                BufferedTreeStream itemToMove = Items[index];
                if (!itemToMove.IsValid)
                {
                    m_validRecords--;
                    for (int x = index; x < m_validRecords; x++)
                    {
                        Items[x] = Items[x + 1];
                    }
                    Items[m_validRecords] = itemToMove;
                    return;
                }

                //ToDo: Consider an exponential search algorithm.
                int currentIndex = index + 1;
                while (currentIndex < m_validRecords && Items[currentIndex].CacheKey.IsLessThan(itemToMove.CacheKey))
                {
                    Items[currentIndex - 1] = Items[currentIndex];
                    currentIndex++;
                }
                Items[currentIndex - 1] = itemToMove;
            }

            private bool IsLessThan(BufferedTreeStream item1, BufferedTreeStream item2)
            {
                if (!item1.IsValid && !item2.IsValid)
                    return false;
                if (!item1.IsValid)
                    return false;
                if (!item2.IsValid)
                    return true;
                return item1.CacheKey.IsLessThan(item2.CacheKey);// item1.CurrentKey.CompareTo(item2.CurrentKey);
            }
        }
    }
}
