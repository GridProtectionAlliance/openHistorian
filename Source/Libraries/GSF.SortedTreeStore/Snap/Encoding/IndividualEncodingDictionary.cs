//******************************************************************************************************
//  IndividualEncodingDictionary.cs - Gbtc
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
    internal class IndividualEncodingDictionary
    {
        private readonly object m_syncRoot;
        private readonly Dictionary<Guid, IndividualEncodingDefinitionBase> m_combinedEncoding;
        private readonly Dictionary<Tuple<Guid, Type>, IndividualEncodingDefinitionBase> m_keyTypedCombinedEncoding;

        /// <summary>
        /// Creates a new EncodingDictionary
        /// </summary>
        public IndividualEncodingDictionary()
        {
            m_syncRoot = new object();
            m_combinedEncoding = new Dictionary<Guid, IndividualEncodingDefinitionBase>();
            m_keyTypedCombinedEncoding = new Dictionary<Tuple<Guid, Type>, IndividualEncodingDefinitionBase>();
        }

        /// <summary>
        /// Registers this type
        /// </summary>
        /// <param name="encoding"></param>
        public void Register(IndividualEncodingDefinitionBase encoding)
        {
            lock (m_syncRoot)
            {
                if (encoding.TypeIfNotGeneric is null)
                {
                    m_combinedEncoding.Add(encoding.Method, encoding);
                }
                else if (encoding.TypeIfNotGeneric != null )
                {
                    m_keyTypedCombinedEncoding.Add(Tuple.Create(encoding.Method, encoding.TypeIfNotGeneric), encoding);
                }
            }
        }

        /// <summary>
        /// Attempts to get the specified encoding method from the dictionary. Will register the types if never registered before.
        /// </summary>
        /// <typeparam name="TTree">The value</typeparam>
        /// <param name="encodingMethod">the encoding method</param>
        /// <param name="encoding">an output if the encoding method exists.</param>
        /// <returns>True if the encoding value was found, false otherwise.</returns>
        public bool TryGetEncodingMethod<TTree>(Guid encodingMethod, out IndividualEncodingDefinitionBase encoding)
            where TTree : SnapTypeBase<TTree>, new()
        {
            Type keyType = typeof(TTree);
            lock (m_syncRoot)
            {
                if (m_keyTypedCombinedEncoding.TryGetValue(Tuple.Create(encodingMethod, keyType), out encoding)
                    || m_combinedEncoding.TryGetValue(encodingMethod, out encoding))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
