//******************************************************************************************************
//  CombinedEncodingDictionary.cs - Gbtc
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
using GSF.Snap.Definitions;

namespace GSF.Snap.Encoding
{
    /// <summary>
    /// A helper class for all of the specific implementations that lookup encoding methods.
    /// </summary>
    internal class PairEncodingDictionary
    {
        private readonly object m_syncRoot;
        private readonly Dictionary<EncodingDefinition, PairEncodingDefinitionBase> m_combinedEncoding;
        private readonly Dictionary<Tuple<EncodingDefinition, Type>, PairEncodingDefinitionBase> m_keyTypedCombinedEncoding;
        private readonly Dictionary<Tuple<EncodingDefinition, Type>, PairEncodingDefinitionBase> m_valueTypedCombinedEncoding;
        private readonly Dictionary<Tuple<EncodingDefinition, Type, Type>, PairEncodingDefinitionBase> m_keyValueTypedCombinedEncoding;

        /// <summary>
        /// Creates a new EncodingDictionary
        /// </summary>
        public PairEncodingDictionary()
        {
            m_syncRoot = new object();
            m_combinedEncoding = new Dictionary<EncodingDefinition, PairEncodingDefinitionBase>();
            m_keyTypedCombinedEncoding = new Dictionary<Tuple<EncodingDefinition, Type>, PairEncodingDefinitionBase>();
            m_valueTypedCombinedEncoding = new Dictionary<Tuple<EncodingDefinition, Type>, PairEncodingDefinitionBase>();
            m_keyValueTypedCombinedEncoding = new Dictionary<Tuple<EncodingDefinition, Type, Type>, PairEncodingDefinitionBase>();
        }

        /// <summary>
        /// Registers this type
        /// </summary>
        /// <param name="encoding"></param>
        public void Register(PairEncodingDefinitionBase encoding)
        {
            if (encoding is null)
                throw new ArgumentNullException("encoding");

            lock (m_syncRoot)
            {
                if (encoding.KeyTypeIfNotGeneric is null && encoding.ValueTypeIfNotGeneric is null)
                {
                    m_combinedEncoding.Add(encoding.Method, encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric != null && encoding.ValueTypeIfNotGeneric is null)
                {
                    m_keyTypedCombinedEncoding.Add(Tuple.Create(encoding.Method, encoding.KeyTypeIfNotGeneric), encoding);
                }
                else if (encoding.KeyTypeIfNotGeneric is null && encoding.ValueTypeIfNotGeneric != null)
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
        /// Attempts to get the specified encoding method from the dictionary. Will register the types if never registered before.
        /// </summary>
        /// <typeparam name="TKey">The key</typeparam>
        /// <typeparam name="TValue">The value</typeparam>
        /// <param name="encodingMethod">the encoding method</param>
        /// <param name="encoding">an output if the encoding method exists.</param>
        /// <returns>True if the encoding value was found, false otherwise.</returns>
        public bool TryGetEncodingMethod<TKey, TValue>(EncodingDefinition encodingMethod, out PairEncodingDefinitionBase encoding)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            if (encodingMethod is null)
                throw new ArgumentNullException("encodingMethod");

            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            lock (m_syncRoot)
            {
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
