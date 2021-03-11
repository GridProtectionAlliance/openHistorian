//******************************************************************************************************
//  ThreadSafeList_Enumerator.cs - Gbtc
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
using System.Collections;
using System.Collections.Generic;

namespace GSF.Threading
{
    public partial class ThreadSafeList<T>
    {
        /// <summary>
        /// The Enumerator for a <see cref="ThreadSafeList{T}"/>
        /// </summary>
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

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            public T Current
            {
                get
                {
                    if (!m_nextItemExists)
                        throw new InvalidOperationException("Past the end of the array, or never called MoveNext()");
                    return m_nextItem;
                }
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <returns>
            /// The current element in the collection.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            object IEnumerator.Current => Current;


            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
            public bool MoveNext()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (m_nextItemExists)
                    m_iterator.UnsafeUnregisterItem();

                m_nextItemExists = m_iterator.UnsafeTryGetNextItem(out m_nextItem);
                return m_nextItemExists;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
            public void Reset()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (m_nextItemExists)
                    m_iterator.UnsafeUnregisterItem();

                m_nextItem = default;
                m_nextItemExists = false;
                m_iterator.Reset();
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                if (!m_disposed)
                {
                    if (m_nextItemExists)
                        m_iterator.UnsafeUnregisterItem();

                    m_disposed = true;
                    m_nextItemExists = false;
                    m_nextItem = default;
                    m_iterator = null;
                }
            }
        }
    }
}