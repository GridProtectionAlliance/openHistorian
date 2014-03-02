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
    public static class FilterLibrary
    {
        private static readonly object SyncRoot;
        private static readonly Dictionary<Guid, CreateFilterBase> Filters;
        private static readonly Dictionary<Guid, CreateSeekFilterBase> SeekFilters;
        private static readonly HashSet<Type> RegisteredTypes;

        static FilterLibrary()
        {
            SyncRoot = new object();
            Filters = new Dictionary<Guid, CreateFilterBase>();
            SeekFilters = new Dictionary<Guid, CreateSeekFilterBase>();
            RegisteredTypes = new HashSet<Type>();

            Register(new PointIDFilter());
            Register(new TimestampFilter());
        }

        public static void Register<T>()
            where T : SortedTreeTypeBase<T>, new()
        {

            T type = new T();
            lock (SyncRoot)
            {
                if (RegisteredTypes.Add(type.GetType()))
                {
                    IEnumerable encodingMethods = type.GetEncodingMethods();
                    if (encodingMethods == null)
                        return;

                    foreach (var method in encodingMethods)
                    {
                        var match = method as CreateFilterBase;
                        var seek = method as CreateSeekFilterBase;
                        if (match != null)
                            Register(match);
                        else if (seek != null)
                            Register(seek);
                    }
                }
            }
        }

        /// <summary>
        /// Registers this type
        /// </summary>
        /// <param name="encoding"></param>
        public static void Register(CreateFilterBase encoding)
        {
            lock (SyncRoot)
            {
                Filters.Add(encoding.FilterType, encoding);
            }
        }

        /// <summary>
        /// Registers this type
        /// </summary>
        /// <param name="encoding"></param>
        public static void Register(CreateSeekFilterBase encoding)
        {
            lock (SyncRoot)
            {
                SeekFilters.Add(encoding.FilterType, encoding);
            }
        }


        public static MatchFilterBase<TKey, TValue> GetMatchFilter<TKey, TValue>(Guid filter, BinaryStreamBase stream)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            CreateFilterBase encoding;

            lock (SyncRoot)
            {
                if (!RegisteredTypes.Contains(keyType))
                {
                    Register<TKey>();
                }
                if (!RegisteredTypes.Contains(valueType))
                {
                    Register<TValue>();
                }

                if (Filters.TryGetValue(filter, out encoding))
                {
                    return encoding.Create<TKey, TValue>(stream);
                }
            }
            throw new Exception("Filter not found");
        }

        public static SeekFilterBase<TKey> GetSeekFilter<TKey>(Guid filter, BinaryStreamBase stream)
            where TKey : SortedTreeTypeBase<TKey>, new()
        {
            Type keyType = typeof(TKey);

            CreateSeekFilterBase encoding;

            lock (SyncRoot)
            {
                if (!RegisteredTypes.Contains(keyType))
                {
                    Register<TKey>();
                }

                if (SeekFilters.TryGetValue(filter, out encoding))
                {
                    return encoding.Create<TKey>(stream);
                }
            }
            throw new Exception("Filter not found");
        }
    }
}
