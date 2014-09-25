//******************************************************************************************************
//  LargeSortedPointBuffer'2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
using GSF.SortedTreeStore.Services.Reader;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Collection
{
    /// <summary>
    /// Contains a verly large sorted point buffer. 
    /// Recommended size is more than 10 thousand elements.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>
    /// This internally implements a jagged arrray sorting method.
    /// </remarks>
    public class LargeSortedPointBuffer<TKey, TValue>
        : TreeStream<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private SortedPointBuffer<TKey, TValue>[] m_buffers;
        private int m_currentBuffer;
        private int m_capacity;
        private bool m_isReadingMode;
        private UnionTreeStreamReader<TKey, TValue> m_reader;

        /// <summary>
        /// Create a large sorted point buffer
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="multiplier"></param>
        public LargeSortedPointBuffer(int capacity, int multiplier)
        {
            m_capacity = capacity;
            m_buffers = new SortedPointBuffer<TKey, TValue>[multiplier];
            m_currentBuffer = 0;
            for (int x = 0; x < m_buffers.Length; x++)
            {
                m_buffers[x] = new SortedPointBuffer<TKey, TValue>(capacity, true);
            }
        }

        protected override bool ReadNext(TKey key, TValue value)
        {
            return m_reader.Read(key, value);
        }

        public bool TryEnqueue(TKey key, TValue value)
        {
        TryAgain:
            if (m_buffers[m_currentBuffer].TryEnqueue(key, value))
                return true;
            if (m_currentBuffer == m_buffers.Length - 1)
                return false;
            m_currentBuffer++;
            goto TryAgain;
        }

        public int Count
        {
            get
            {
                return m_currentBuffer * m_capacity + m_buffers[m_currentBuffer].Count;
            }
        }

        public bool IsFull
        {
            get
            {
                return m_currentBuffer == m_buffers.Length - 1 && m_buffers[m_currentBuffer].IsFull;
            }
        }

        /// <summary>
        /// Gets/Sets the current mode of the point buffer.
        /// </summary>
        /// <remarks>
        /// This class is not designed to be read from and written to at the same time.
        /// This is because sorting must occur right before reading from this stream.
        /// </remarks>
        public bool IsReadingMode
        {
            get
            {
                return m_isReadingMode;
            }
            set
            {
                if (m_isReadingMode != value)
                {
                    m_isReadingMode = value;
                    if (m_isReadingMode)
                    {
                        Sort();
                    }
                    else
                    {
                        Clear();
                    }
                }

            }
        }

        void Sort()
        {
            var lst = new List<TreeStream<TKey, TValue>>();
            foreach (var buffer in m_buffers)
            {
                if (buffer.Count > 0)
                {
                    buffer.IsReadingMode = true;
                    lst.Add(buffer);
                }
            }
            m_reader = new UnionTreeStreamReader<TKey, TValue>(lst, false);
        }

        void Clear()
        {
            SetEos(false);
            m_currentBuffer = 0;
            m_reader = null;
            foreach (var buffer in m_buffers)
            {
                try
                {
                    buffer.IsReadingMode = false;
                }
                catch (Exception)
                {
                    System.Console.Write("Error");
                    throw;
                }
            }
        }

        /// <summary>
        /// Overrides the default behavior that disposes the stream when the end of the stream has been encountered.
        /// </summary>
        protected override void EndOfStreamReached()
        {
            SetEos(true);
        }

    }
}
