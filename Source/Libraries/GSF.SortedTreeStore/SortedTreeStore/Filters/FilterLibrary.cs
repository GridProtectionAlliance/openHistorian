//******************************************************************************************************
//  FilterLibrary.cs - Gbtc
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
//  2/22/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using GSF.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Filters
{
    /// <summary>
    /// Contains all of the filters for the <see cref="SortedTreeStore"/>. 
    /// </summary>
    public class FilterLibrary
    {
        private readonly object m_syncRoot;
        private readonly Dictionary<Guid, CreateFilterBase> m_filters;
        private readonly Dictionary<Guid, CreateSeekFilterBase> m_seekFilters;

        internal FilterLibrary()
        {
            m_syncRoot = new object();
            m_filters = new Dictionary<Guid, CreateFilterBase>();
            m_seekFilters = new Dictionary<Guid, CreateSeekFilterBase>();
        }

        /// <summary>
        /// Registers this type
        /// </summary>
        /// <param name="encoding"></param>
        public void Register(CreateFilterBase encoding)
        {
            lock (m_syncRoot)
            {
                m_filters.Add(encoding.FilterType, encoding);
            }
        }

        /// <summary>
        /// Registers this type
        /// </summary>
        /// <param name="encoding"></param>
        public void Register(CreateSeekFilterBase encoding)
        {
            lock (m_syncRoot)
            {
                m_seekFilters.Add(encoding.FilterType, encoding);
            }
        }

        public MatchFilterBase<TKey, TValue> GetMatchFilter<TKey, TValue>(Guid filter, BinaryStreamBase stream)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            CreateFilterBase encoding;

            lock (m_syncRoot)
            {
                if (m_filters.TryGetValue(filter, out encoding))
                {
                    return encoding.Create<TKey, TValue>(stream);
                }
            }
            throw new Exception("Filter not found");
        }

        public SeekFilterBase<TKey> GetSeekFilter<TKey>(Guid filter, BinaryStreamBase stream)
            where TKey : SortedTreeTypeBase<TKey>, new()
        {
            Type keyType = typeof(TKey);

            CreateSeekFilterBase encoding;

            lock (m_syncRoot)
            {
                if (m_seekFilters.TryGetValue(filter, out encoding))
                {
                    return encoding.Create<TKey>(stream);
                }
            }
            throw new Exception("Filter not found");
        }
    }
}
