//******************************************************************************************************
//  PointCollection`2.cs - Gbtc
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
//  2/5/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore
{
    public class PointCollection<TKey, TValue>
        : PointCollectionBase<TKey,TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        public byte[] RawData;
        public int HeadPosition;
        public int TailPosition;

        SortedTreeKeyMethodsBase<TKey> m_keyMethods;
        SortedTreeValueMethodsBase<TValue> m_valueMethods;
        int m_pointSize;
        int m_keySize;
        int m_valueSize;

        public PointCollection(int capacity)
        {
            m_keyMethods = new TKey().CreateKeyMethods();
            m_valueMethods = new TValue().CreateValueMethods();
            m_pointSize = m_keyMethods.Size + m_valueMethods.Size;
            m_keySize = m_keyMethods.Size;
            m_valueSize = m_valueMethods.Size;
            RawData = new byte[capacity * m_pointSize];
        }

       

        public override void Clear()
        {

        }

        unsafe public override void Enqueue(TKey key, TValue value)
        {
            fixed (byte* lp = RawData)
            {
                m_keyMethods.Write(lp + TailPosition, key);
                m_valueMethods.Write(lp + TailPosition + m_keySize, value);
                TailPosition += m_pointSize;
            }
        }

        unsafe public override void Dequeue(TKey key, TValue value)
        {
            fixed (byte* lp = RawData)
            {
                m_keyMethods.Read(lp + HeadPosition, key);
                m_valueMethods.Read(lp + HeadPosition + m_keySize, value);
                HeadPosition += m_pointSize;
            }
        }

    }
}
