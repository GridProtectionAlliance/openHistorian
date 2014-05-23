//******************************************************************************************************
//  DualEncodingDictionary`1.cs - Gbtc
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
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Encoding
{
    /// <summary>
    /// A helper class for all of the specific implementations that lookup encoding methods.
    /// </summary>
    /// <typeparam name="T">The value in the dictionary</typeparam>
    internal class DualEncodingDictionary<T>
        where T : CreateDoubleValueBase
    {
        private readonly object m_syncRoot;
        private readonly HashSet<Type> m_registeredTypes;
        private readonly Dictionary<EncodingDefinition, T> m_combinedEncoding;
        private readonly Dictionary<Tuple<EncodingDefinition, Type>, T> m_keyTypedCombinedEncoding;
        private readonly Dictionary<Tuple<EncodingDefinition, Type>, T> m_valueTypedCombinedEncoding;
        private readonly Dictionary<Tuple<EncodingDefinition, Type, Type>, T> m_keyValueTypedCombinedEncoding;

        /// <summary>
        /// Creates a new EncodingDictionary
        /// </summary>
        public DualEncodingDictionary()
        {
            m_syncRoot = new object();
            m_registeredTypes = new HashSet<Type>();
            m_combinedEncoding = new Dictionary<EncodingDefinition, T>();
            m_keyTypedCombinedEncoding = new Dictionary<Tuple<EncodingDefinition, Type>, T>();
            m_valueTypedCombinedEncoding = new Dictionary<Tuple<EncodingDefinition, Type>, T>();
            m_keyValueTypedCombinedEncoding = new Dictionary<Tuple<EncodingDefinition, Type, Type>, T>();
        }

        /// <summary>
        /// Registers the provided type.
        /// </summary>
        /// <typeparam name="TTreeType"></typeparam>
        public void Register<TTreeType>()
            where TTreeType : SortedTreeTypeBase, new()
        {
            TTreeType type = new TTreeType();
            lock (m_syncRoot)
            {
                if (m_registeredTypes.Add(type.GetType()))
                {
                    IEnumerable encodingMethods = type.GetEncodingMethods();
                    if (encodingMethods == null)
                        return;

                    foreach (var method in encodingMethods)
                    {
                        var single = method as T;
                        if (single != null)
                            Register(single);
                    }
                }
            }
        }

        /// <summary>
        /// Registers this type
        /// </summary>
        /// <param name="encoding"></param>
        public void Register(T encoding)
        {
            if ((object)encoding == null)
                throw new ArgumentNullException("encoding");

            lock (m_syncRoot)
            {
                if (encoding.KeyTypeIfNotGeneric == null && encoding.ValueTypeIfNotGeneric == null)
                {
                    m_combinedEncoding.Add(encoding.Method, encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric != null && encoding.ValueTypeIfNotGeneric == null)
                {
                    m_keyTypedCombinedEncoding.Add(Tuple.Create(encoding.Method, encoding.KeyTypeIfNotGeneric), encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric == null && encoding.ValueTypeIfNotGeneric != null)
                {
                    m_valueTypedCombinedEncoding.Add(Tuple.Create(encoding.Method, encoding.ValueTypeIfNotGeneric), encoding);
                }
                else
                {
                    m_keyValueTypedCombinedEncoding.Add(Tuple.Create(encoding.Method, encoding.KeyTypeIfNotGeneric, encoding.ValueTypeIfNotGeneric), encoding);
                }
            }
        }

        /// <summary>
        /// Attempts to get the specified encoing method from the dictionary. Will register the types if never registered before.
        /// </summary>
        /// <typeparam name="TKey">The key</typeparam>
        /// <typeparam name="TValue">The value</typeparam>
        /// <param name="encodingMethod">the encoding method</param>
        /// <param name="encoding">an output if the encoding method exists.</param>
        /// <returns>True if the encoding value was found, false otherwise.</returns>
        public bool TryGetEncodingMethod<TKey, TValue>(EncodingDefinition encodingMethod, out T encoding)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            if ((object)encodingMethod == null)
                throw new ArgumentNullException("encodingMethod");

            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            lock (m_syncRoot)
            {
                if (!m_registeredTypes.Contains(keyType))
                {
                    Register<TKey>();
                }
                if (!m_registeredTypes.Contains(valueType))
                {
                    Register<TValue>();
                }

                if (m_keyValueTypedCombinedEncoding.TryGetValue(Tuple.Create(encodingMethod, keyType, valueType), out encoding)
                    || m_keyTypedCombinedEncoding.TryGetValue(Tuple.Create(encodingMethod, keyType), out encoding)
                    || m_valueTypedCombinedEncoding.TryGetValue(Tuple.Create(encodingMethod, valueType), out encoding)
                    || m_combinedEncoding.TryGetValue(encodingMethod, out encoding))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
