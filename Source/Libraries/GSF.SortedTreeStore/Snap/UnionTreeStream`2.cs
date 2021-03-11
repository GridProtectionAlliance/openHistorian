//******************************************************************************************************
//  UnionTreeStream'2.cs - Gbtc
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
//  09/23/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System.Collections.Generic;
using System.Linq;

namespace GSF.Snap
{
    /// <summary>
    /// Does a union of <see cref="TreeStream{TKey,TValue}"/>.
    /// Ensures that the data is read sequentially and duplicates are removed.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public partial class UnionTreeStream<TKey, TValue>
        : TreeStream<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        private BufferedTreeStream[] m_tablesOrigList;
        private readonly UnionTreeStreamSortHelper m_sortedArchiveStreams;
        private TreeStream<TKey, TValue> m_firstStream;
        private BufferedTreeStream m_firstTable;

        private readonly TKey m_readWhileUpperBounds = new TKey();
        private readonly TKey m_nextArchiveStreamLowerBounds = new TKey();
        private readonly bool m_ownsStreams;




        /// <summary>
        /// Creates a union stream reader from the supplied data.
        /// </summary>
        /// <param name="streams">all of the tables to combine in the union</param>
        /// <param name="ownsStream">if this class owns the streams, it will call dispose when <see cref="Dispose"/> is called.
        /// Otherwise, the streams will not be disposed.</param>
        public UnionTreeStream(IEnumerable<TreeStream<TKey, TValue>> streams, bool ownsStream)
        {
            m_firstStream = null;
            m_ownsStreams = ownsStream;
            m_tablesOrigList = streams.Select(x => new BufferedTreeStream(x)).ToArray();

            m_sortedArchiveStreams = new UnionTreeStreamSortHelper(m_tablesOrigList);
            m_readWhileUpperBounds.SetMin();

            foreach (BufferedTreeStream table1 in m_sortedArchiveStreams.Items)
            {
                table1.EnsureCache();
            }
            m_sortedArchiveStreams.Sort();

            //Remove any duplicates
            RemoveDuplicatesIfExists();

            if (m_sortedArchiveStreams.Items.Length > 0)
            {
                m_firstTable = m_sortedArchiveStreams[0];
            }
            else
            {
                m_firstTable = null;
            }

            SetReadWhileUpperBoundsValue();
        }

        protected override void Dispose(bool disposing)
        {
            if (m_tablesOrigList != null && m_ownsStreams)
            {
                foreach (BufferedTreeStream table in m_tablesOrigList)
                {
                    table.Dispose();
                }
                m_tablesOrigList = null;
            }
            base.Dispose(disposing);
        }

        public override bool IsAlwaysSequential => true;

        public override bool NeverContainsDuplicates => true;

        protected override bool ReadNext(TKey key, TValue value)
        {
            if (m_firstStream != null && m_firstStream.Read(key, value))
            {
                if (key.IsLessThan(m_readWhileUpperBounds))
                {
                    return true;
                }
                m_firstTable.WriteToCache(key, value);
            }
            return Read2(key, value);
        }

        private bool Read2(TKey key, TValue value)
        {
        TryAgain:
            if (m_firstStream is null)
            {
                //If m_firstStream is null, this means either: 
                //  the value is cached.
                // or
                //  the end of the stream has occured.
                if (m_firstTable != null && m_firstTable.IsValid)
                {
                    //The value is cached.
                    m_firstTable.Read(key, value);
                    m_firstStream = m_firstTable.Stream;
                    return true;
                }
                //The end of the steam has been reached.
                return false;
            }

            //Condition 1:
            //  The archive stream may no longer be in order and needs to be checked
            //Response:
            //  Resort the archive stream
            //
            //Condition 2:
            //  The end of the frame has been reached
            //Response:
            //  Advance to the next frame
            //  Also test the edge case where the current point might be equal to the end of the frame
            //      since this is an inclusive filter and ReadWhile is exclusive.
            //      If it's part of the frame, return true after Advancing the frame and the point.
            //

            //Since condition 1 and 2 can occur at the same time, verifying the sort of the Archive Stream is a good thing to do.
            // Will verify that the stream is in the proper order and remove any duplicates that were found. 
            // May be called after every single read, but better to be called
            // when a ReadWhile function returns false.

            if (m_sortedArchiveStreams.Items.Length > 1)
            {
                //If list is no longer in order
                int compare = CompareStreams(m_sortedArchiveStreams[0], m_sortedArchiveStreams[1]);
                if (compare == 0) //A duplicate value was found.
                {
                    //If a duplicate entry is found, advance the position of the duplicate entry
                    RemoveDuplicatesFromList();
                    SetReadWhileUpperBoundsValue();
                    
                    m_firstTable = m_sortedArchiveStreams[0];
                }
                else if (compare > 0) //List is out of order
                {
                    m_sortedArchiveStreams.SortAssumingIncreased(0);
                    m_firstTable = m_sortedArchiveStreams[0];
                    SetReadWhileUpperBoundsValue();
                }
            }
            else
            {
                //If only 1 stream, we can't resort, so we are done.
                if (!m_sortedArchiveStreams[0].IsValid)
                {
                    return false;
                }
            }

            m_firstStream = null; //Ensure that the if block is executed when repeating this function call.
            goto TryAgain;
        }

        //-------------------------------------------------------------

       

        /// <summary>
        /// Compares two Archive Streams together for proper sorting.
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        private int CompareStreams(BufferedTreeStream item1, BufferedTreeStream item2)
        {
            if (!item1.IsValid && !item2.IsValid)
                return 0;
            if (!item1.IsValid)
                return 1;
            if (!item2.IsValid)
                return -1;
            return item1.CacheKey.CompareTo(item2.CacheKey);// item1.CurrentKey.CompareTo(item2.CurrentKey);
        }

        /// <summary>
        /// Checks the first 2 Archive Streams for a duplicate entry. If one exists, then removes the duplicate and resorts the list.
        /// </summary>
        private void RemoveDuplicatesIfExists()
        {
            if (m_sortedArchiveStreams.Items.Length > 1)
            {
                if (CompareStreams(m_sortedArchiveStreams[0], m_sortedArchiveStreams[1]) == 0 && m_sortedArchiveStreams[0].IsValid)
                {
                    //If a duplicate entry is found, advance the position of the duplicate entry
                    RemoveDuplicatesFromList();
                }
            }
        }

        /// <summary>
        /// Call this function when the same point exists in multiple archive files. It will
        /// read past the duplicate point in all other archive files and then resort the tables.
        /// 
        /// Assums that the archiveStream's cached value is current.
        /// </summary>
        private void RemoveDuplicatesFromList()
        {
            if (!m_sortedArchiveStreams[0].IsValid)
                return;

            int lastDuplicateIndex = -1;
            for (int index = 1; index < m_sortedArchiveStreams.Items.Length; index++)
            {
                if (CompareStreams(m_sortedArchiveStreams[0], m_sortedArchiveStreams[index]) == 0)
                {
                    m_sortedArchiveStreams[index].ReadToCache();
                    lastDuplicateIndex = index;
                }
                else
                {
                    break;
                }
            }

            //Resorts the list in reverse order.
            for (int j = lastDuplicateIndex; j > 0; j--)
                m_sortedArchiveStreams.SortAssumingIncreased(j);
        }

        /// <summary>
        /// Sets the read while upper bounds value. 
        /// Which is the lesser of 
        /// The first point in the adjacent table or
        /// The end of the current seek window.
        ///  </summary>
        private void SetReadWhileUpperBoundsValue()
        {
            if (m_sortedArchiveStreams.Items.Length > 1 && m_sortedArchiveStreams[1].IsValid)
            {
                m_sortedArchiveStreams[1].CacheKey.CopyTo(m_nextArchiveStreamLowerBounds);
            }
            else
            {
                m_nextArchiveStreamLowerBounds.SetMax();
            }
            m_nextArchiveStreamLowerBounds.CopyTo(m_readWhileUpperBounds);
        }

    }
}