////******************************************************************************************************
////  UnionTreeStreamReader'2.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  2/16/2014 - Steven E. Chisholm
////       Generated original version of source code. 
////
////******************************************************************************************************

//using System.Collections.Generic;
//using GSF.SortedTreeStore.Tree;

//namespace GSF.SortedTreeStore
//{

//    public class UnionTreeStreamReader<TKey, TValue>
//        : TreeStream<TKey, TValue>
//        where TKey : SortedTreeTypeBase<TKey>, new()
//        where TValue : SortedTreeTypeBase<TValue>, new()
//    {
//        private List<BufferedTreeStream<TKey, TValue>> m_tablesOrigList;
//        CustomSortHelper<BufferedTreeStream<TKey, TValue>> m_sortedArchiveStreams;
//        BufferedTreeStream<TKey, TValue> m_firstTable;
//        TKey m_readWhileUpperBounds = new TKey();
//        TKey m_nextArchiveStreamLowerBounds = new TKey();

//        public UnionTreeStreamReader(List<TreeStream<TKey, TValue>> tables)
//        {
//            m_tablesOrigList = new List<BufferedTreeStream<TKey, TValue>>();

//            foreach (var table in tables)
//            {
//                m_tablesOrigList.Add(new BufferedTreeStream<TKey, TValue>(0, table));
//            }

//            m_sortedArchiveStreams = new CustomSortHelper<BufferedTreeStream<TKey, TValue>>(m_tablesOrigList, CompareStreams);

//            m_readWhileUpperBounds.SetMin();

//        }

//        /// <summary>
//        /// Does an unconditional seek operation to the provided key.
//        /// </summary>
//        /// <param name="key"></param>
//        void SeekToKey(TKey key)
//        {
//            foreach (var table in m_sortedArchiveStreams.Items)
//            {
//                table.SeekToKeyAndUpdateCacheValue(key);
//            }
//            m_sortedArchiveStreams.Sort();

//            //Remove any duplicates
//            RemoveDuplicatesIfExists();

//            if (m_sortedArchiveStreams.Items.Length > 0)
//            {
//                m_firstTable = m_sortedArchiveStreams[0];
//            }
//            else
//            {
//                m_firstTable = null;
//            }

//            SetReadWhileUpperBoundsValue();
//        }

//        public override void Cancel()
//        {
//            EOS = true;

//            if (m_tablesOrigList != null)
//            {
//                m_tablesOrigList.ForEach(x => x.Dispose());
//                m_tablesOrigList = null;
//            }
//        }

//        /// <summary>
//        /// Gets if the stream is always in sequential order. Do not return true unless it is Guaranteed that 
//        /// the data read from this stream is sequential.
//        /// </summary>
//        public override bool IsAlwaysSequential
//        {
//            get
//            {
//                return true;
//            }
//        }

//        /// <summary>
//        /// Gets if the stream will never return duplicate keys. Do not return true unless it is Guaranteed that 
//        /// the data read from this stream will never contain duplicates.
//        /// </summary>
//        public override bool NeverContainsDuplicates
//        {
//            get
//            {
//                return true;
//            }
//        }

//        /// <summary>
//        /// Advances the stream to the next value. 
//        /// If before the beginning of the stream, advances to the first value
//        /// </summary>
//        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
//        public override bool Read(TKey key, TValue value)
//        {

//            if (m_firstTable != null)
//            {
//                if (m_firstTable.Read(key, value, m_readWhileUpperBounds))
//                {
//                    return true;
//                    if (key.IsLessThanOrEqualTo(m_readWhileUpperBounds))
//                    {
//                        return true;
//                    }
//                    else
//                    {
//                        m_firstTable.UnRead(key, value);
//                    }
//                }
//            }
//            return ReadCatchAll(key, value);
//        }

//        bool ReadCatchAll(TKey key, TValue value)
//        {
//        TryAgain:
//            if (m_firstTable == null)
//            {
//                Cancel();
//                return false;
//            }
//            if (m_firstTable.ReadWhile(key, value, m_readWhileUpperBounds))
//            {
//                return true;
//            }
//            ReadWhileFollowupActions();
//            goto TryAgain;
//        }

//        void ReadWhileFollowupActions()
//        {
//            //There are certain followup requirements when a ReadWhile method returns false.
//            //Condition 1:
//            //  The end of the node has been reached. 
//            //Response: 
//            //  It returned false to allow for additional checks such as timeouts to occur.
//            //  Do Nothing.
//            //
//            //Condition 2:
//            //  The archive stream may no longer be in order and needs to be checked
//            //Response:
//            //  Resort the archive stream
//            //
//            //Condition 3:
//            //  The end of the frame has been reached
//            //Response:
//            //  Advance to the next frame
//            //  Also test the edge case where the current point might be equal to the end of the frame
//            //      since this is an inclusive filter and ReadWhile is exclusive.
//            //      If it's part of the frame, return true after Advancing the frame and the point.
//            //

//            //Update the cached values for the table so proper analysis can be done.
//            m_firstTable.UpdateCachedValue();

//            //Check Condition 1
//            if (m_firstTable.CacheIsValid && m_firstTable.CacheKey.IsLessThan(m_readWhileUpperBounds))
//                return;

//            //Since condition 2 and 3 can occur at the same time, verifying the sort of the Archive Stream is a good thing to do.
//            VerifyArchiveStreamSortingOrder();
//        }


//        //-------------------------------------------------------------

//        /// <summary>
//        /// Will verify that the stream is in the proper order and remove any duplicates that were found. 
//        /// May be called after every single read, but better to be called
//        /// when a ReadWhile function returns false.
//        /// </summary>
//        void VerifyArchiveStreamSortingOrder()
//        {
//            if (EOS)
//                return;

//            m_sortedArchiveStreams[0].UpdateCachedValue();

//            if (m_sortedArchiveStreams.Items.Length > 1)
//            {
//                //If list is no longer in order
//                int compare = CompareStreams(m_sortedArchiveStreams[0], m_sortedArchiveStreams[1]);
//                if (compare == 0 && m_sortedArchiveStreams[0].CacheIsValid)
//                {
//                    //If a duplicate entry is found, advance the position of the duplicate entry
//                    RemoveDuplicatesFromList();
//                    SetReadWhileUpperBoundsValue();
//                }
//                if (compare > 0)
//                {
//                    m_sortedArchiveStreams.SortAssumingIncreased(0);
//                    m_firstTable = m_sortedArchiveStreams[0];
//                    SetReadWhileUpperBoundsValue();
//                }
//                if (compare == 0 && !m_sortedArchiveStreams[0].CacheIsValid)
//                {
//                    EOS = true;
//                    m_firstTable = null;
//                }
//            }
//            else
//            {
//                if (!m_sortedArchiveStreams[0].CacheIsValid)
//                {
//                    EOS = true;
//                    m_firstTable = null;
//                }
//            }
//        }

//        /// <summary>
//        /// Compares two Archive Streams together for proper sorting.
//        /// </summary>
//        /// <param name="item1"></param>
//        /// <param name="item2"></param>
//        /// <returns></returns>
//        int CompareStreams(BufferedTreeStream<TKey, TValue> item1, BufferedTreeStream<TKey, TValue> item2)
//        {
//            if (!item1.CacheIsValid && !item2.CacheIsValid)
//                return 0;
//            if (!item1.CacheIsValid)
//                return 1;
//            if (!item2.CacheIsValid)
//                return -1;
//            return item1.CacheKey.CompareTo(item2.CacheKey);// item1.CurrentKey.CompareTo(item2.CurrentKey);
//        }



//        /// <summary>
//        /// Checks the first 2 Archive Streams for a duplicate entry. If one exists, then removes the duplicate and resorts the list.
//        /// </summary>
//        void RemoveDuplicatesIfExists()
//        {
//            if (m_sortedArchiveStreams.Items.Length > 1)
//            {
//                if (CompareStreams(m_sortedArchiveStreams[0], m_sortedArchiveStreams[1]) == 0 && m_sortedArchiveStreams[0].CacheIsValid)
//                {
//                    //If a duplicate entry is found, advance the position of the duplicate entry
//                    RemoveDuplicatesFromList();
//                }
//            }
//        }

//        /// <summary>
//        /// Call this function when the same point exists in multiple archive files. It will
//        /// read past the duplicate point in all other archive files and then resort the tables.
//        /// 
//        /// Assums that the archiveStream's cached value is current.
//        /// </summary>
//        void RemoveDuplicatesFromList()
//        {
//            int lastDuplicateIndex = -1;
//            for (int index = 1; index < m_sortedArchiveStreams.Items.Length; index++)
//            {
//                if (CompareStreams(m_sortedArchiveStreams[0], m_sortedArchiveStreams[index]) == 0)
//                {
//                    m_sortedArchiveStreams[index].SkipToNextKeyAndUpdateCachedValue();
//                    lastDuplicateIndex = index;
//                }
//                else
//                {
//                    break;
//                }
//            }

//            //Resorts the list in reverse order.
//            for (int j = lastDuplicateIndex; j > 0; j--)
//                m_sortedArchiveStreams.SortAssumingIncreased(j);

//            SetReadWhileUpperBoundsValue();
//        }

//        /// <summary>
//        /// Sets the read while upper bounds value. 
//        /// Which is the lesser of 
//        /// The first point in the adjacent table or
//        /// The end of the current seek window.
//        ///  </summary>
//        void SetReadWhileUpperBoundsValue()
//        {
//            if (m_sortedArchiveStreams.Items.Length > 1 && m_sortedArchiveStreams[1].CacheIsValid)
//            {
//                m_sortedArchiveStreams[1].CacheKey.CopyTo(m_nextArchiveStreamLowerBounds);
//            }
//            else
//            {
//                m_nextArchiveStreamLowerBounds.SetMax();
//            }
//            m_nextArchiveStreamLowerBounds.CopyTo(m_readWhileUpperBounds);
//        }

//    }
//}