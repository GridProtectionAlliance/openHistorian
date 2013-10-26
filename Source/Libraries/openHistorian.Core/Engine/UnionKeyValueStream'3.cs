//******************************************************************************************************
//  UnionKeyValueStream'3.cs - Gbtc
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
//  10/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace openHistorian.Engine
{
    public class UnionKeyValueStream<T, TKey, TValue>
        : KeyValueStream<TKey, TValue>
        where T : KeyValueStream<TKey, TValue>, IComparable<T>
        where TKey : HistorianKeyBase<TKey>, new()
        where TValue : HistorianValueBase<TValue>, new()
    {

        public T FirstItem { get; private set; }

        T[] m_items;
        int[] m_order;

        public UnionKeyValueStream(IEnumerable<T> list)
        {
            m_items = list.ToArray();
            Sort();
        }

        public override bool Read()
        {
        TryAgain:
            if (FirstItem == null || !FirstItem.IsValid)
            {
                IsValid = false;
                return false;
            }
            else
            {
                FirstItem.CurrentKey.CopyTo(CurrentKey);
                FirstItem.CurrentValue.CopyTo(CurrentValue);
                FirstItem.Read();

                if (m_items.Length >= 2)
                {
                    //If list is no longer in order
                    int compare = FirstItem.CompareTo(m_items[1]);
                    if (compare == 0 && FirstItem.IsValid)
                    {
                        //If a duplicate entry is found, advance the position of the duplicate entry
                    }
                    if (compare > 0)
                    {
                        //Sorting the array is very expensive. 
                        //Since we know that the list was previously sorted
                        //and only element 0 is out of order,
                        //we can parse the list and insert element 0 into
                        //it's proper order.
                        var itemToMove = m_items[0];
                        m_items[0] = m_items[1];
                        for (int i = 1; i < m_items.Length; i++)
                        {
                            //if the end of the list is reached
                            //the itemToMove goes at the end of the list.
                            if (i == m_items.Length - 1)
                            {
                                m_items[i] = itemToMove;
                                break;
                            }
                            //If the itemToMove is still greater than the following element
                            //move the element up one spot and keep searching.
                            else if (itemToMove.CompareTo(m_items[i + 1]) > 0)
                            {
                                m_items[i] = m_items[i + 1];
                            }
                            else
                            {
                                m_items[i] = itemToMove;
                                break;
                            }
                        }
                        FirstItem = m_items[0];
                    }
                }
                IsValid = true;
                return true;
            }
        }

        public void Sort()
        {
            Array.Sort(m_items);
            if (m_items.Length > 0)
                FirstItem = m_items[0];
        }
        public void Sort(int index)
        {
            
        }
    }
}
