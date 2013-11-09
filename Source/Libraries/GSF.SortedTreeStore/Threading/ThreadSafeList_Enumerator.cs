//******************************************************************************************************
//  ThreadSafeList_Enumerator.cs - Gbtc
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
//  1/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace GSF.Threading
{
    public partial class ThreadSafeList<T>
    {
        private class Enumerator : IEnumerator<T>
        {
            private T m_nextItem;
            private bool m_disposed;
            private bool m_nextItemExists;
            private Iterator m_iterator;

            public Enumerator(Iterator iterator)
            {
                m_iterator = iterator;
                m_nextItemExists = false;
            }

            public void Dispose()
            {
                if (!m_disposed)
                {
                    if (m_nextItemExists)
                        m_iterator.UnsafeUnregisterItem();

                    m_disposed = true;
                    m_nextItemExists = false;
                    m_nextItem = default(T);
                    m_iterator = null;
                }
            }

            public bool MoveNext()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (m_nextItemExists)
                    m_iterator.UnsafeUnregisterItem();

                m_nextItemExists = m_iterator.UnsafeTryGetNextItem(out m_nextItem);
                return m_nextItemExists;
            }

            public void Reset()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (m_nextItemExists)
                    m_iterator.UnsafeUnregisterItem();

                m_nextItem = default(T);
                m_nextItemExists = false;
                m_iterator.Reset();
            }

            public T Current
            {
                get
                {
                    if (!m_nextItemExists)
                        throw new InvalidOperationException("Past the end of the array, or never called MoveNext()");
                    return m_nextItem;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }
        }
    }
}