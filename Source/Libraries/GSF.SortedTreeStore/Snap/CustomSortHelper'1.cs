//******************************************************************************************************
//  CustomSortHelper'1.cs - Gbtc
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace GSF.Snap
{
    /// <summary>
    /// Provides basic sorting methods that assist in UnionKeyValueStream's speed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomSortHelper<T>
    {
        /// <summary>
        /// All of the items in this list.
        /// </summary>
        public T[] Items;

        private readonly Func<T, T, bool> m_isLessThan;

        /// <summary>
        /// Creates a new custom sort helper and presorts the list.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="isLessThan"></param>
        public CustomSortHelper(IEnumerable<T> items, Func<T, T, bool> isLessThan)
        {
            Items = items.ToArray();
            m_isLessThan = isLessThan;
            Sort();
        }

        /// <summary>
        /// Indexer to get the specified item out of the list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        /// <summary>
        /// Resorts the entire list. Uses an insertion sort routine
        /// </summary>
        public void Sort()
        {
            //A insertion sort routine.

            //Skip first item in list since it will always be sorted correctly
            for (int itemToInsertIndex = 1; itemToInsertIndex < Items.Length; itemToInsertIndex++)
            {
                T itemToInsert = Items[itemToInsertIndex];

                int currentIndex = itemToInsertIndex - 1;
                //While the current item is greater than itemToInsert, shift the value
                while (currentIndex >= 0 && m_isLessThan(itemToInsert, Items[currentIndex]))
                {
                    Items[currentIndex + 1] = Items[currentIndex];
                    currentIndex--;
                }
                Items[currentIndex + 1] = itemToInsert;
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
            T itemToMove = Items[index];
            int currentIndex = index + 1;
            while (currentIndex < Items.Length && m_isLessThan(Items[currentIndex], itemToMove))
            {
                Items[currentIndex - 1] = Items[currentIndex];
                currentIndex++;
            }
            Items[currentIndex - 1] = itemToMove;
        }

    }
}
