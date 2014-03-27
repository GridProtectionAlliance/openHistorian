////******************************************************************************************************
////  BufferedTreeStream'2.cs - Gbtc
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
////  3/25/2014 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using GSF.SortedTreeStore.Tree;

//namespace GSF.SortedTreeStore
//{
//    public class BufferedTreeStream<TKey, TValue>
//        : IDisposable
//        where TKey : SortedTreeTypeBase<TKey>, new()
//        where TValue : SortedTreeTypeBase<TValue>, new()
//    {
//        public TreeStream<TKey, TValue> Scanner;

//        /// <summary>
//        /// An index value that is used to disassociate the archive file. Passed to this class from the <see cref="SortedTreeEngineReaderSequential{TKey,TValue}"/>
//        /// </summary>
//        public int Index { get; private set; }

//        /// <summary>
//        /// Creates the table reader.
//        /// </summary>
//        /// <param name="index"></param>
//        /// <param name="table"></param>
//        public BufferedTreeStream(int index, TreeStream<TKey, TValue> table)
//        {
//            if (table.IsAlwaysSequential)

//                Index = index;
//            m_table = table;
//            m_snapshot = m_table.ActiveSnapshotInfo.CreateReadSnapshot();
//            Scanner = m_snapshot.GetTreeScanner();
//        }

//        public bool CacheIsValid = false;
//        public TKey CacheKey = new TKey();
//        TValue CacheValue = new TValue();
//        bool m_wasUnread;

//        public bool Read(TKey key, TValue value, TKey readWhileUpperBounds)
//        {
//            if (m_wasUnread)
//            {
                
//            }
//            if (Scanner.Read(key, value))
//            {
//                if (key.IsLessThan(readWhileUpperBounds))
//                {
//                    return true;
//                }
//                UnRead(key,value);
//                return false;
//            }

//        }

//        public void UnRead(TKey key, TValue value)
//        {

//        }

//        public void UpdateCachedValue()
//        {
//            CacheIsValid = Scanner.Read(CacheKey, CacheValue);
//            m_hasAlreadyRead = true;
//        }

//        public void SkipToNextKeyAndUpdateCachedValue()
//        {
//            CacheIsValid = Scanner.Read(CacheKey, CacheValue);
//            CacheIsValid = Scanner.Peek(CacheKey, CacheValue);
//        }

//        public void Dispose()
//        {
//            if (Scanner != null)
//            {
//                Scanner.Dispose();
//                Scanner = null;
//            }
//        }
//    }
//}
