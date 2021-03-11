//******************************************************************************************************
//  FilterLibrary.cs - Gbtc
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
//  02/22/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Snap.Definitions;

namespace GSF.Snap.Filters
{
    /// <summary>
    /// Contains all of the filters for the <see cref="Snap"/>. 
    /// </summary>
    public class FilterLibrary
    {
        private static readonly LogPublisher Log = Logger.CreatePublisher(typeof(FilterLibrary), MessageClass.Framework);

        private readonly object m_syncRoot;
        private readonly Dictionary<Guid, MatchFilterDefinitionBase> m_filters;
        private readonly Dictionary<Guid, SeekFilterDefinitionBase> m_seekFilters;

        internal FilterLibrary()
        {
            m_syncRoot = new object();
            m_filters = new Dictionary<Guid, MatchFilterDefinitionBase>();
            m_seekFilters = new Dictionary<Guid, SeekFilterDefinitionBase>();
        }

        /// <summary>
        /// Registers this type
        /// </summary>
        /// <param name="encoding"></param>
        public void Register(MatchFilterDefinitionBase encoding)
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
        public void Register(SeekFilterDefinitionBase encoding)
        {
            lock (m_syncRoot)
            {
                m_seekFilters.Add(encoding.FilterType, encoding);
            }
        }

        public MatchFilterBase<TKey, TValue> GetMatchFilter<TKey, TValue>(Guid filter, BinaryStreamBase stream)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            try
            {
                lock (m_syncRoot)
                {
                    if (m_filters.TryGetValue(filter, out MatchFilterDefinitionBase encoding))
                    {
                        return encoding.Create<TKey, TValue>(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Publish(MessageLevel.Error, "Match Filter Exception", string.Format("ID: {0} Key: {1} Value: {2}", filter.ToString(), typeof(TKey).ToString(), typeof(TValue).ToString()), null, ex);
                throw;
            }
            Log.Publish(MessageLevel.Info, "Missing Match Filter", string.Format("ID: {0} Key: {1} Value: {2}", filter.ToString(), typeof(TKey).ToString(), typeof(TValue).ToString()));
            throw new Exception("Filter not found");
        }

        public SeekFilterBase<TKey> GetSeekFilter<TKey>(Guid filter, BinaryStreamBase stream)
            where TKey : SnapTypeBase<TKey>, new()
        {
            try
            {
                lock (m_syncRoot)
                {
                    if (m_seekFilters.TryGetValue(filter, out SeekFilterDefinitionBase encoding))
                    {
                        return encoding.Create<TKey>(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Publish(MessageLevel.Error, "Seek Filter Exception", string.Format("ID: {0} Key: {1}", filter.ToString(), typeof(TKey).ToString()), null, ex);
                throw;
            }

            Log.Publish(MessageLevel.Info, "Missing Seek Filter", string.Format("ID: {0} Key: {1}", filter.ToString(), typeof(TKey).ToString()));
            throw new Exception("Filter not found");
        }
    }
}
