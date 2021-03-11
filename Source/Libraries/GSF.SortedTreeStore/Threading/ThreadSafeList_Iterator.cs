//******************************************************************************************************
//  ThreadSafeList_Iterator.cs - Gbtc
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
//  1/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;

namespace GSF.Threading
{
    public partial class ThreadSafeList<T>
    {
        /// <summary>
        /// Parses through a list in a thread safe mannor.
        /// </summary>
        private class Iterator
        {
            private readonly ThreadSafeList<T> m_list;
            private long m_currentIndex;
            private long m_lastVersion;
            private int m_lastVersionIndex;

            private Wrapper m_itemCurrentlyLocked;

            public Iterator(ThreadSafeList<T> list)
            {
                m_list = list;
                Reset();
            }

            /// <summary>
            /// Only 1 item can be obtained at a time. Failing to call <see cref="UnsafeUnregisterItem"/> will
            /// result in a infinite loop. Therefore wrap each call in a Try/Finally block.
            /// </summary>
            /// <param name="item">an output parameter for the next item</param>
            /// <returns></returns>
            public bool UnsafeTryGetNextItem(out T item)
            {
                if (m_itemCurrentlyLocked != null)
                    throw new Exception("Invalid use of ThreadSafeIterator. Must Call Unregister before calling another TryGetNextItem");

                Wrapper currentObject = null;
                //Get the next item in the list.
                lock (m_list.m_syncRoot)
                {
                    IList<long> keys = m_list.m_list.Keys;
                    for (int x = GetStartingIndex(); x < keys.Count; x++)
                    {
                        long k = keys[x];
                        if (k > m_currentIndex)
                        {
                            m_lastVersion = m_list.m_version;
                            m_lastVersionIndex = x;
                            m_currentIndex = k;
                            currentObject = m_list.m_list.Values[x];
                            Interlocked.Increment(ref currentObject.ReferencedCount);
                            break;
                        }
                    }
                }

                if (currentObject is null)
                {
                    item = default;
                    return false;
                }
                m_itemCurrentlyLocked = currentObject;
                item = currentObject.Item;
                return true;
            }

            /// <summary>
            /// Should be called each time after <see cref="UnsafeTryGetNextItem"/>.
            /// Note, it is optional to call this function if <see cref="UnsafeTryGetNextItem"/> returns false.
            /// </summary>
            public void UnsafeUnregisterItem()
            {
                if (m_itemCurrentlyLocked is null)
                    return;
                Interlocked.Decrement(ref m_itemCurrentlyLocked.ReferencedCount);
                m_itemCurrentlyLocked = null;
            }

            public void Reset()
            {
                m_lastVersion = -1;
                m_lastVersionIndex = -1;
                m_currentIndex = -1;
            }

            private int GetStartingIndex()
            {
                if (m_lastVersion == m_list.m_version)
                {
                    return m_lastVersionIndex + 1;
                }
                return 0;
            }
        }
    }
}